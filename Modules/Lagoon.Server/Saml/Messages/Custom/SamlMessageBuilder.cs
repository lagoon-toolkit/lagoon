namespace Lagoon.Server.Saml.Messages.Custom;

/// <summary>
/// Enables the generation of a SAML request.
/// </summary>
public class SamlMessageBuilder
{

    #region fields

    /// <summary>
    /// The message content.
    /// </summary>
    private StringBuilder _content;

    #endregion

    #region constructors


    /// <summary>
    /// Initialization of a new instance of the class.
    /// </summary>
    /// <param name="templateName">Name of the template file.</param>
    public SamlMessageBuilder(string templateName)
    {
        Type handlerType = typeof(SamlHandler);
        string resourceName = $"{handlerType.Namespace}.Messages.Templates.{templateName}";
        Stream resStream = handlerType.Assembly.GetManifestResourceStream(resourceName);
        if (resStream is null)
        {
            throw new Exception($"The \"{resourceName}\" resource is not found in {handlerType.Assembly.GetName()}.");
        }
        using (resStream)
        {
            using (StreamReader sr = new(resStream, Encoding.UTF8))
            {
                _content = new StringBuilder(sr.ReadToEnd());
            }
        }
    }

    #endregion

    #region methods

    /// <summary>
    /// Replacement of a value in the model.
    /// </summary>
    /// <param name="name">Parameter name.</param>
    /// <param name="value">Value of the parameter.</param>
    public void Replace(string name, string value)
    {
        _content.Replace("{" + name + "}", value);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>String that represents the current object.</returns>
    public override string ToString()
    {
        return _content.ToString();
    }

    /// <summary>
    /// Get the UTF8 encoded content.
    /// </summary>
    /// <returns>The UTF8 encoded content.</returns>
    public byte[] ToArray()
    {
        return Encoding.UTF8.GetBytes(_content.ToString());
    }

    #endregion

}
