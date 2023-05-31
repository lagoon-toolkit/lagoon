using Microsoft.CodeAnalysis;

namespace Lagoon.Generators;

internal class BuildContext
{

    #region properties

    public string AssemblyName { get; }

    public string Configuration { get; }

    public string OutputPath { get; }

    public string PackageContentPath { get; }

    public string ProjectDirectory { get; }

    public string TargetFramework { get; }

    #endregion

    #region constructors

    private BuildContext() { }

    public BuildContext(string configuration, string targetFramework, string projectDirectory)
        : this(configuration, targetFramework, projectDirectory,
             Path.Combine(projectDirectory, "obj", configuration, targetFramework))
    { }

    public BuildContext(string configuration, string targetFramework, string projectDirectory, string intermediateOutputPath)
    {
        Configuration = configuration;
        TargetFramework = targetFramework;
        ProjectDirectory = projectDirectory;
        AssemblyName = Path.GetFileName(projectDirectory);
        OutputPath = Path.Combine(intermediateOutputPath, "Lagoon");
        PackageContentPath = Path.Combine(OutputPath, "PackageContent");
    }

    public BuildContext(string projectDirectory, BuildContext context)
        : this(context.Configuration, context.TargetFramework, projectDirectory)
    { }

    /// <summary>
    /// Try to deserialize the string.
    /// </summary>
    /// <param name="serializedBuildContext">All properties serialized to a string.</param>
    /// <returns>The build context.</returns>
    internal static BuildContext TryDeserialize(string serializedBuildContext)
    {
        // Replace the path separator on linux
        if (Path.DirectorySeparatorChar != '\\')
        {
            serializedBuildContext = serializedBuildContext.Replace('\\', Path.DirectorySeparatorChar);
        }
        // Split parameters
        string[] properties = serializedBuildContext.Split('|');
        if (properties.Length != 4)
        {
            return null;
        }
        return new(properties[0], properties[1], properties[2], $"{properties[2]}{Path.DirectorySeparatorChar}{properties[3]}");
    }

    /// <summary>
    /// Extract from the reference path the configuration and the target framework;
    /// Extract the project directory from the generator context.
    /// </summary>
    /// <param name="lagoonReferences">Reference to lagoon dlls</param>
    /// <returns></returns>
    internal static BuildContext TryDetect(GeneratorExecutionContext generatorContext, List<LagoonModule> lagoonReferences)
    {
        string projectDirectory, targetFramework, configuration;
        // Search the project directory
        string assemblyName = generatorContext.Compilation.AssemblyName;
        string path = generatorContext.Compilation.SyntaxTrees.FirstOrDefault()?.FilePath;
        while (!string.IsNullOrEmpty(path) && !Path.GetFileName(path).Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
        {
            path = Path.GetDirectoryName(path);
        }
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        projectDirectory = path;
        // Add the TargetFrameWork and the Configuration informations
        foreach (LagoonModule reference in lagoonReferences)
        {
            path = Path.GetDirectoryName(reference.AssemblyPath);
            targetFramework = Path.GetFileName(path);
            path = Path.GetDirectoryName(path);
            configuration = Path.GetFileName(path);
            path = Path.GetDirectoryName(path);
            if (Path.GetFileName(path) == "bin")
            {
                return new BuildContext(configuration, targetFramework, projectDirectory);
            }
        }
        return null;
    }

    #endregion

    #region methods

    public string GetAssemblyPath()
    {
        return BuildAssemblyPath(Path.GetFileName(ProjectDirectory));
    }

    public string BuildAssemblyPath(string assemblyName)
    {
        return Path.Combine(ProjectDirectory, "bin", Configuration, TargetFramework, $"{assemblyName}.dll");
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return $"{Configuration}|{TargetFramework}|{ProjectDirectory}|{GetRelativeOutputPath()}";
    }

    /// <summary>
    /// Get the relative output path.
    /// </summary>
    /// <returns></returns>
    private string GetRelativeOutputPath()
    {
        if (OutputPath.StartsWith(ProjectDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return OutputPath.Substring(ProjectDirectory.Length + 1);
        }
        return null;
    }

    internal bool HashChanged(string name, string hash)
    {
        string file = Path.Combine(OutputPath, $"{name}.hash");
        return !File.Exists(file) || File.ReadAllText(file, Encoding.UTF8) != hash;
    }

    internal void SaveHash(string name, string hash)
    {
        File.WriteAllText(Path.Combine(OutputPath, $"{name}.hash"), hash, Encoding.UTF8);
    }

    #endregion

}
