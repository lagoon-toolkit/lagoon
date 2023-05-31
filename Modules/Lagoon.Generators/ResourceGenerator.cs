using Microsoft.CodeAnalysis;

namespace Lagoon.Generators;


[Generator]
public class ResourceGenerator : ISourceGenerator
{

    #region methods

    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUG
        Log.ToFile($"Initialize {System.DateTime.Now} {System.Environment.Version}");
#endif 
    }

    public void Execute(GeneratorExecutionContext context)
    {
#if DEBUG
        try
        {
#endif
        // Load the compilation context define in Lagoon.UI.PreBuild.Targets
        BuildContext buildContext = null;
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.LagoonBuildContext", out string serializedBuildContext))
        {
            buildContext = BuildContext.TryDeserialize(serializedBuildContext);
            Directory.CreateDirectory(buildContext.PackageContentPath);
//                File.WriteAllText(Path.Combine(buildContext.PackageContentPath, "Test.txt"), buildContext.ToString());
#if DEBUG
                Log.ToFile($"MsBuild context :\n\t{buildContext}");
#endif
        }
        // Load the lagoon references
        List<LagoonModule> lagoonReferences = new();
        foreach (MetadataReference reference in context.Compilation.References)
        {
            if (Path.GetFileName(reference.Display).StartsWith("Lagoon."))
            {
                lagoonReferences.Add(LagoonModuleManager.Get(reference, buildContext));
            }
        }
        lagoonReferences.Sort();
#if DEBUG
            foreach (LagoonModule lr in lagoonReferences)
            {
                Log.ToFile(this, $"\t * {lr}");
            }
#endif
        if (context.CancellationToken.IsCancellationRequested)
        {
#if DEBUG
                Log.ToFile($"CANCELLED !");
#endif
            return;
        }
        // Try to detect configuration from folder names (.net 3.1)
        buildContext ??= BuildContext.TryDetect(context, lagoonReferences);
        // Ensure the context is loaded
        if (string.IsNullOrEmpty(buildContext?.ProjectDirectory))
        {
#if DEBUG
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Launch();
                }
                throw new System.Exception("The buildContext loading failed !");
#else
            return;

#endif
        }
        // Get the LagoonModule corresponding to the current project build
        LagoonModule current = LagoonModuleManager.Get(buildContext.GetAssemblyPath());
        // Load the icons defined in this project
        IconList iconList = current.GetIcons(context.CancellationToken);
        // Create the icons.svg and the res.xml files
        if (buildContext.HashChanged("icons", iconList.Hash))
        {
            iconList.SaveToSvgFile(Path.Combine(buildContext.PackageContentPath, "icons.svg"), false);
            iconList.SaveToXmlFile(Path.Combine(buildContext.PackageContentPath, "res.xml"));
            buildContext.SaveHash("icons", iconList.Hash);
        }
        // Merge current icon list with icolist of rferenced projects
        IconsMerger x = new(iconList);
        foreach (LagoonModule r in lagoonReferences)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
#if DEBUG
                    Log.ToFile($"CANCELLED !");
#endif
                return;
            }
            if (r.HasIcons)
            {
                x.Add(r.GetIcons(context.CancellationToken));
            }
        }
        // Merge *.scss into one "styles.scss" file
        string mainScssPath = Path.Combine(buildContext.ProjectDirectory, "resources", "styles", "main.scss");
        string stylesPath = Path.Combine(buildContext.PackageContentPath, "styles.scss");
        string scssHash = ScssDocument.CalculateFolderHash(mainScssPath);
        //File.WriteAllText(@$"C:\Temp\__Hashes\{DateTime.Now:HH-mm-ss}-{scssHash}-{buildContext.HashChanged("scss", scssHash)}-{Guid.NewGuid():N}.txt", 
        //    $"{buildContext.ProjectDirectory}\n{mainScssPath}");
        if (!File.Exists(stylesPath) || buildContext.HashChanged("scss", scssHash))
        {
            if (scssHash is null)
            {
                // They're nostyles.scss source file
                if (File.Exists(stylesPath))
                {
                    File.Delete(stylesPath);
                }
            }
            else
            {
                // We combine all the source files into one
                ScssDocument scssDoc = new(mainScssPath, $"_content/{buildContext.AssemblyName}/");
                scssDoc.SaveTo(stylesPath);
            }
            buildContext.SaveHash("scss", scssHash);
        }
        // Generate the sources files
        if (context.CancellationToken.IsCancellationRequested)
        {
#if DEBUG
                Log.ToFile($"CANCELLED !");
#endif
            return;
        }
        context.AddSource("IconNames.g.cs", GenerateIconNamesFile(x.Symbols, x.Aliases));
#if DEBUG
        }
        catch (System.Exception ex)
        {
            Log.ToFile(this, $"ERROR !\n{ex}");
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
            throw;
        }
#endif
    }

    private string GenerateIconNamesFile(SortedDictionary<string, IconSymbol> symbols, SortedDictionary<string, IconAlias> aliases)
    {
        StringBuilder cs = new();
        cs.AppendLine("namespace Lagoon.UI.Components");
        cs.AppendLine("{");
        cs.AppendLine("    internal static partial class IconNames");
        cs.AppendLine("    {");
        cs.AppendLine("        public static class All");
        cs.AppendLine("        {");
        // Fill files
        foreach (KeyValuePair<string, IconSymbol> entry in symbols)
        {
            cs.Append("            public const string ");
            cs.Append(entry.Key);
            cs.Append(" = \"");
            cs.Append(entry.Value.Id);
            cs.AppendLine("\";");
        }
        cs.AppendLine("        }");
        // Aliases
        foreach (KeyValuePair<string, IconAlias> entry in aliases)
        {
            cs.AppendLine("        /// <summary>");
            cs.Append("        /// Alias to All.");
            cs.AppendLine(entry.Value.Value);
            cs.AppendLine("        /// </summary>");
            cs.Append("        public const string ");
            cs.Append(entry.Key);
            cs.Append(" = \"");
            cs.Append(IconList.ID_ALIAS_PREFIX);
            cs.Append(entry.Key);
            cs.AppendLine("\";");
        }
        cs.AppendLine("    }");
        cs.AppendLine("}");
        return cs.ToString();
    }

    #endregion

}
