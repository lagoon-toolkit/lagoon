// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;

namespace Lagoon.UI.Helpers.Route;

/// <summary>
/// Resolves components for an application.
/// </summary>
internal static class RouteTableFactory
{
    private static readonly ConcurrentDictionary<RouteKey, RouteTable> _cache = new();
    private static readonly IComparer<RouteEntry> _routePrecedence = Comparer<RouteEntry>.Create(RouteComparison);

    public static RouteTable Create(RouteKey routeKey)
    {
        if (_cache.TryGetValue(routeKey, out RouteTable resolvedComponents))
        {
            return resolvedComponents;
        }

        List<Type> componentTypes = GetRouteableComponents(routeKey);
        RouteTable routeTable = Create(componentTypes);
        _cache.TryAdd(routeKey, routeTable);
        return routeTable;
    }

    public static void ClearCaches()
    {
        _cache.Clear();
    }

    private static List<Type> GetRouteableComponents(RouteKey routeKey)
    {
        List<Type> routeableComponents = new();
        if (routeKey.AppAssembly is not null)
        {
            GetRouteableComponents(routeableComponents, routeKey.AppAssembly);
        }

        if (routeKey.AdditionalAssemblies is not null)
        {
            foreach (Assembly assembly in routeKey.AdditionalAssemblies)
            {
                // We don't need process the assembly if it's the app assembly.
                if (assembly != routeKey.AppAssembly)
                {
                    GetRouteableComponents(routeableComponents, assembly);
                }
            }
        }
        return routeableComponents;

        static void GetRouteableComponents(List<Type> routeableComponents, Assembly assembly)
        {
            foreach (Type type in assembly.ExportedTypes)
            {
                if (typeof(IComponent).IsAssignableFrom(type) && type.IsDefined(typeof(RouteAttribute)))
                {
                    routeableComponents.Add(type);
                }
            }
        }
    }

    internal static RouteTable Create(List<Type> componentTypes)
    {
#if ORIGINAL_CODE
        Dictionary<Type, string[]> templatesByHandler = new();
        foreach (Type componentType in componentTypes)
        {
            // We're deliberately using inherit = false here.
            //
            // RouteAttribute is defined as non-inherited, because inheriting a route attribute always causes an
            // ambiguity. You end up with two components (base class and derived class) with the same route.
            object[] routeAttributes = componentType.GetCustomAttributes(typeof(RouteAttribute), inherit: false);
            string[] templates = new string[routeAttributes.Length];
            for (int i = 0; i < routeAttributes.Length; i++)
            {
                RouteAttribute attribute = (RouteAttribute)routeAttributes[i];
                templates[i] = attribute.Template;
            }
            templatesByHandler.Add(componentType, templates);
        }
#else
        // For Lagoon we ignore route that have been overloaded
        HashSet<string> loadedTemplates = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<Type, string[]> templatesByHandler = new();
        foreach (Type componentType in componentTypes)
        {
            // We're deliberately using inherit = false here.
            //
            // RouteAttribute is defined as non-inherited, because inheriting a route attribute always causes an
            // ambiguity. You end up with two components (base class and derived class) with the same route.
            object[] routeAttributes = componentType.GetCustomAttributes(typeof(RouteAttribute), inherit: false);
            List<string> componentTemplates = new();
            foreach (RouteAttribute attribute in routeAttributes.Cast<RouteAttribute>())
            {
                string template = attribute.Template;
                if (template.StartsWith("./"))
                {
                    template = template[1..];
                }
                template = template.Trim('/');
                if (!loadedTemplates.Contains(template))
                {
#if false
                    Lagoon.Helpers.Trace.ToConsole($"Add route [{template}] for {componentType.FullName}");
#endif
                    loadedTemplates.Add(template);
                    componentTemplates.Add(template);
                }
#if false
                else
                {
                    Lagoon.Helpers.Trace.ToConsole($"Ignored route [{template}] for {componentType.FullName}");
                }
#endif
            }
            templatesByHandler.Add(componentType, componentTemplates.ToArray());
        }
#endif
        return Create(templatesByHandler);
    }

    internal static RouteTable Create(Dictionary<Type, string[]> templatesByHandler)
    {
        List<RouteEntry> routes = new();
        foreach ((Type type, string[] templates) in templatesByHandler)
        {
            HashSet<string> allRouteParameterNames = new(StringComparer.OrdinalIgnoreCase);
            (RouteTemplate, HashSet<string>)[] parsedTemplates = new (RouteTemplate, HashSet<string>)[templates.Length];
            for (int i = 0; i < templates.Length; i++)
            {
                RouteTemplate parsedTemplate = TemplateParser.ParseTemplate(templates[i]);
                HashSet<string> parameterNames = GetParameterNames(parsedTemplate);
                parsedTemplates[i] = (parsedTemplate, parameterNames);

                foreach (string parameterName in parameterNames)
                {
                    allRouteParameterNames.Add(parameterName);
                }
            }

            foreach ((RouteTemplate parsedTemplate, HashSet<string> routeParameterNames) in parsedTemplates)
            {
                List<string> unusedRouteParameterNames = GetUnusedParameterNames(allRouteParameterNames, routeParameterNames);
                RouteEntry entry = new(parsedTemplate, type, unusedRouteParameterNames);
                routes.Add(entry);
            }
        }

        routes.Sort(_routePrecedence);
        return new RouteTable(routes.ToArray());
    }

    private static HashSet<string> GetParameterNames(RouteTemplate routeTemplate)
    {
        HashSet<string> parameterNames = new(StringComparer.OrdinalIgnoreCase);
        foreach (TemplateSegment segment in routeTemplate.Segments)
        {
            if (segment.IsParameter)
            {
                parameterNames.Add(segment.Value);
            }
        }

        return parameterNames;
    }

    private static List<string> GetUnusedParameterNames(HashSet<string> allRouteParameterNames, HashSet<string> routeParameterNames)
    {
        List<string> unusedParameters = null;
        foreach (string item in allRouteParameterNames)
        {
            if (!routeParameterNames.Contains(item))
            {
                unusedParameters ??= new();
                unusedParameters.Add(item);
            }
        }

        return unusedParameters;
    }

    /// <summary>
    /// Route precedence algorithm.
    /// We collect all the routes and sort them from most specific to
    /// less specific. The specificity of a route is given by the specificity
    /// of its segments and the position of those segments in the route.
    /// * A literal segment is more specific than a parameter segment.
    /// * A parameter segment with more constraints is more specific than one with fewer constraints
    /// * Segment earlier in the route are evaluated before segments later in the route.
    /// For example:
    /// /Literal is more specific than /Parameter
    /// /Route/With/{parameter} is more specific than /{multiple}/With/{parameters}
    /// /Product/{id:int} is more specific than /Product/{id}
    ///
    /// Routes can be ambiguous if:
    /// They are composed of literals and those literals have the same values (case insensitive)
    /// They are composed of a mix of literals and parameters, in the same relative order and the
    /// literals have the same values.
    /// For example:
    /// * /literal and /Literal
    /// /{parameter}/literal and /{something}/literal
    /// /{parameter:constraint}/literal and /{something:constraint}/literal
    ///
    /// To calculate the precedence we sort the list of routes as follows:
    /// * Shorter routes go first.
    /// * A literal wins over a parameter in precedence.
    /// * For literals with different values (case insensitive) we choose the lexical order
    /// * For parameters with different numbers of constraints, the one with more wins
    /// If we get to the end of the comparison routing we've detected an ambiguous pair of routes.
    /// </summary>
    internal static int RouteComparison(RouteEntry x, RouteEntry y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        RouteTemplate xTemplate = x.Template;
        RouteTemplate yTemplate = y.Template;
        int minSegments = Math.Min(xTemplate.Segments.Length, yTemplate.Segments.Length);
        int currentResult = 0;
        for (int i = 0; i < minSegments; i++)
        {
            TemplateSegment xSegment = xTemplate.Segments[i];
            TemplateSegment ySegment = yTemplate.Segments[i];

            int xRank = GetRank(xSegment);
            int yRank = GetRank(ySegment);

            currentResult = xRank.CompareTo(yRank);

#if ORIGINAL_CODE
            // If they are both literals we can disambiguate
            if ((xRank, yRank) == (0, 0))
            {
                currentResult = StringComparer.OrdinalIgnoreCase.Compare(xSegment.Value, ySegment.Value);
            }
#else
            if (currentResult == 0)
            {
                // If they are both literals we can disambiguate
                if (xRank == 0)
                {
                    currentResult = StringComparer.OrdinalIgnoreCase.Compare(xSegment.Value, ySegment.Value);
                }
                // If they are both parameters with constraints
                else if (xRank is 1 or 3)
                {
                    // If only one is a string, the other one is prioritize
                    currentResult = GetFirstParameterTypeRank(xSegment).CompareTo(GetFirstParameterTypeRank(ySegment));
                }
            }
#endif
            if (currentResult != 0)
            {
                break;
            }
        }

        if (currentResult == 0)
        {
            currentResult = xTemplate.Segments.Length.CompareTo(yTemplate.Segments.Length);
        }

        return currentResult == 0
            ? throw new InvalidOperationException($@"The following routes are ambiguous:
'{x.Template.TemplateText}' in '{x.Handler.FullName}'
'{y.Template.TemplateText}' in '{y.Handler.FullName}'
")
            : currentResult;
    }

    private static int GetFirstParameterTypeRank(TemplateSegment xSegment)
    {
        // Check if the constraint is on a string
        return  xSegment.Constraints[0].TypeRank;
    }

    private static int GetRank(TemplateSegment xSegment)
    {
        return xSegment switch
        {
            // Literal
            { IsParameter: false } => 0,
            // Parameter with constraints
            { IsParameter: true, IsCatchAll: false, Constraints.Length: > 0 } => 1,
            // Parameter without constraints
            { IsParameter: true, IsCatchAll: false, Constraints.Length: 0 } => 2,
            // Catch all parameter with constraints
            { IsParameter: true, IsCatchAll: true, Constraints.Length: > 0 } => 3,
            // Catch all parameter without constraints
            { IsParameter: true, IsCatchAll: true, Constraints.Length: 0 } => 4,
            // The segment is not correct
            _ => throw new InvalidOperationException($"Unknown segment definition '{xSegment}.")
        };
    }
}