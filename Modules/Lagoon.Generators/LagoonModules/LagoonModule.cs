using System.Diagnostics;
using System.Xml;

namespace Lagoon.Generators;

/// <summary>
/// Representing a Lagoon module.
/// </summary>
internal class LagoonModule : IComparable<LagoonModule>
{

    #region fields

    private IconList _iconList;

    #endregion

    #region properties

    public string AssemblyName { get; }

    public string AssemblyPath { get; }

    public bool HasIcons => AssemblyName.StartsWith("Lagoon.UI");

    public bool HasRootableComponents { get; private set; }

    public int Priority { get; private set; }

    public string ProjectDirectory { get; }

    public bool IsPackage { get; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance of Lagoon module.
    /// </summary>
    /// <param name="assemblyPath">The path of the DLL.</param>
    public LagoonModule(string assemblyPath)
    {
        AssemblyPath = assemblyPath;
        AssemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
        // Detect the root path of the module
        string nuspecName = $"{AssemblyName.ToLowerInvariant()}.nuspec";
        string path = Path.GetDirectoryName(assemblyPath);
        while (!string.IsNullOrEmpty(path) && !AssemblyName.Equals(Path.GetFileName(path), StringComparison.OrdinalIgnoreCase))
        {
            if (File.Exists(Path.Combine(path, nuspecName)))
            {
                IsPackage = true;
                break;
            }
            path = Path.GetDirectoryName(path);
        }
        ProjectDirectory = path;
#if DEBUG
        if (string.IsNullOrEmpty(ProjectDirectory) && !Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif
        // Load lagoon special properties
        if (IsPackage)
        {
            //TODO IsPackage
            throw new NotImplementedException();
        }
        else
        {
            LoadProjectFileProperties();
        }
    }

    #endregion

    #region methods

    /// <summary>
    /// Reload informations from source files.
    /// </summary>
    internal void Update()
    {
        if (!IsPackage)
        {
            LoadProjectFileProperties();
        }
    }

    /// <summary>
    /// Extract Lagoon properties from the csproj file.
    /// </summary>
    private void LoadProjectFileProperties()
    {
        using (XmlReader xmlReader = XmlReader.Create(Path.Combine(ProjectDirectory, $"{AssemblyName}.csproj")))
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }
                switch (xmlReader.Name)
                {
                    case "LagoonPriority":
                        xmlReader.Read();
                        Priority = int.Parse(xmlReader.Value);
                        break;
                    case "HasRootableComponents":
                        xmlReader.Read();
                        HasRootableComponents = "True".Equals(xmlReader.Value, StringComparison.InvariantCultureIgnoreCase);
                        break;
                }
            }
        }
#if DEBUG //TOCLEAN
        Log.ToFile(this, $"Module: {AssemblyName}, Priority: {Priority}, HasRootableComponents: {HasRootableComponents}");
#endif
    }

    /// <summary>
    /// Get the loaded icon list.
    /// </summary>
    /// <returns></returns>
    public IconList GetIcons(System.Threading.CancellationToken cancellationToken)
    {
        if (_iconList is null)
        {
            if (IsPackage)
            {
                //TODO IsPackage
                throw new NotImplementedException();
            }
            else
            {
                _iconList = new(Path.Combine(ProjectDirectory, "resources", "icons"));
            }
        }
        _iconList?.Load(cancellationToken);
        return _iconList;
    }

    /// <summary>
    /// Compare the current instance to another.
    /// </summary>
    /// <param name="other">The other instance.</param>
    /// <returns>The difference of priority.</returns>
    int IComparable<LagoonModule>.CompareTo(LagoonModule other)
    {
        if (other is null)
        {
            return 1;
        }
        int result = other.Priority - Priority;
        if (result == 0)
        {
            return string.Compare(AssemblyName, other.AssemblyName);
        }
        return result;
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return $"{AssemblyName} [{Priority}] [{(HasRootableComponents ? 'C' : '-')}] ({AssemblyPath})";
    }

    #endregion

}
