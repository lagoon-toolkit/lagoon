using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace Lagoon.Generators;

internal static class LagoonModuleManager
{
    #region constants

    private const string DLL_EXTENSION = ".dll";

    #endregion

    #region fields

    private static Dictionary<string, LagoonModule> _cache = new();
    private static object _lock = new();

    public static Dictionary<string, LagoonModule>.KeyCollection Keys => _cache.Keys;

    #endregion

    #region methods

    /// <summary>
    /// Get the Lagoon mudule corresponding to the DLL path. (It can be shared with othetr compilation process)
    /// </summary>
    /// <param name="reference">The reference.</param>
    /// <param name="buildContext">The context from MsBuild.</param>
    /// <returns>The module.</returns>
    public static LagoonModule Get(MetadataReference reference, BuildContext buildContext)
    {
        string assemblyPath = reference.Display;
        // The RoselynCodeAnalysisService references somethimes the source and not the dll
        if (!assemblyPath.EndsWith(DLL_EXTENSION, StringComparison.OrdinalIgnoreCase))
        {
            assemblyPath = BuildAssemblyPath(reference, buildContext);
        }
        return Get(assemblyPath);
    }

    /// <summary>
    /// Get the Lagoon mudule corresponding to the DLL path. (It can be shared with othetr compilation process)
    /// </summary>
    /// <param name="assemblyPath">The path of the DLL file.</param>
    /// <returns>The module.</returns>
    public static LagoonModule Get(string assemblyPath)
    {
        LagoonModule module;
        lock (_lock)
        {
            if (!_cache.TryGetValue(assemblyPath, out module))
            {
                // Add the new lagoon module
                module = new(assemblyPath);
                _cache.Add(assemblyPath, module);
            }
            else
            {
                // Reload properties from source path
                module.Update();
            }
        }
        return module;
    }

    /// <summary>
    /// Build a DLL path from the path of the first source file and the MsBuild context.
    /// </summary>
    /// <param name="reference">The reference to get.</param>
    /// <param name="buildContext">The MsBuild context.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static string BuildAssemblyPath(MetadataReference reference, BuildContext buildContext)
    {
        string assemblyName = reference.Display;
        string path = Path.GetDirectoryName((reference as CompilationReference).Compilation.SyntaxTrees.First().FilePath);
        while (!string.IsNullOrEmpty(path) && !assemblyName.Equals(Path.GetFileName(path), StringComparison.OrdinalIgnoreCase))
        {
            path = Path.GetDirectoryName(path);
        }
        if (string.IsNullOrEmpty(path))
        {
            throw new Exception($"Fail to build the assembly path for {assemblyName} !");
        }
        if (buildContext is null)
        {
#if DEBUG
            //TODO Trouver la configuration et la cible : Env.Argument ?
            var args = Environment.GetCommandLineArgs();

            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            buildContext = new BuildContext("Release", "netstandard2.1", path);
        }
        else
        {
            buildContext = new BuildContext(path, buildContext);
        }
        return buildContext.GetAssemblyPath();
    }

    #endregion


}
