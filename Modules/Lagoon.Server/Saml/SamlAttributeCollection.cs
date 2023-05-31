using System.Collections;
using System.Security.Claims;
using System.Xml;

namespace Lagoon.Server.Saml;


/// <summary>
/// List of attributes of an assertion.
/// </summary>
public class SamlAttributeCollection : IEnumerable<SamlAttribute>
{

    #region fields

    /// <summary>
    /// Dictionary of attributes.
    /// </summary>
    private List<SamlAttribute> _list = new();

    #endregion

    #region properties

    /// <summary>
    /// Returns the number of attributes.
    /// </summary>
    /// <value>The number of attributes.</value>
    /// <returns>The number of attributes.</returns>
    public int Count => _list.Count;

    /// <summary>
    /// Attribute corresponding to the index.
    /// </summary>
    /// <param name="index">Index of the attribute to return.</param>
    /// <value>Attribute corresponding to the index.</value>
    /// <returns>Attribute corresponding to the index.</returns>
    public SamlAttribute this[int index] => _list[index];

    /// <summary>
    /// Attribute corresponding to the name.
    /// </summary>
    /// <param name="name">Name of the attribute.</param>
    /// <value>Attribute corresponding to the name.</value>
    /// <returns>Attribute corresponding to the name.</returns>
    public SamlAttribute this[string name] => Find(name);

    #endregion

    #region methods

    /// <summary>
    /// Returns the value of the attribute whose name is passed in parameter. Otherwise the default value.
    /// </summary>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <value>The value of the attribute whose name is passed in parameter. Otherwise the default value.</value>
    /// <returns>The value of the attribute whose name is passed in parameter. Otherwise the default value.</returns>
    public string GetValue(string name, string defaultValue = null)
    {
        SamlAttribute ttr = Find(name);
        return ttr is null ? defaultValue : ttr.Value;
    }

    /// <summary>
    /// Returns the newly added attribute.
    /// </summary>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="value">Value of the attribute.</param>
    /// <returns>The newly added attribute.</returns>
    public SamlAttribute Add(string name, string value)
    {
        SamlAttribute ttr;

        ttr = Find(name);
        ttr ??= Add(name);
        ttr.Values.Add(value);
        return ttr;
    }

    /// <summary>
    /// Returns the newly added attribute.
    /// </summary>
    /// <param name="name">Name of the attribute.</param>
    /// <returns>The newly added attribute.</returns>
    public SamlAttribute Add(string name)
    {
        SamlAttribute ttr;

        ttr = Find(name);
        ttr ??= new SamlAttribute(name);
        _list.Add(ttr);
        return ttr;
    }

    /// <summary>
    /// Delete an attribute and all its values.
    /// </summary>
    /// <param name="name">Name of the attribute.</param>
    public void Remove(string name)
    {
        for (int l_i = _list.Count - 1; l_i <= 0; l_i++)
        {
            if (_list[l_i].Name.Equals(name))
            {
                _list.RemoveAt(l_i);
            }
        }
    }

    /// <summary>
    /// Delete an attribute and all its values.
    /// </summary>
    /// <param name="index">Index of the attribute.</param>
    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
    }

    /// <summary>
    /// Deletes all attributes.
    /// </summary>
    public void Clear()
    {
        _list.Clear();
    }

    /// <summary>
    /// Returns the attribute corresponding to the name passed in parameter.
    /// </summary>
    /// <param name="name">Name of the attribute.</param>
    /// <returns>The attribute corresponding to the name passed in parameter.</returns>
    public SamlAttribute Find(string name)
    {
        foreach (SamlAttribute ttr in _list)
        {
            if (ttr.Name.Equals(name))
            {
                return ttr;
            }
        }
        return default;
    }

    /// <summary>
    /// Returns the attribute enumerator.
    /// </summary>
    /// <returns>The attribute enumerator.</returns>
    public IEnumerator<SamlAttribute> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    /// <summary>
    /// Return as list of claims.
    /// </summary>
    /// <returns>A list of claims.</returns>
    internal IEnumerable<Claim> ToClaims(IdpMetadata idp, string nameIdentifier)
    {
        if (nameIdentifier is not null)
        {
            yield return new Claim(ClaimTypes.NameIdentifier, nameIdentifier, ClaimValueTypes.String, idp.EntityId);
        }
        foreach (SamlAttribute attr in _list)
        {
            yield return new Claim(attr.Name, attr.Value, ClaimValueTypes.String, idp.EntityId);
        }
    }


    #endregion

    #region private methods

    /// <summary>
    /// Returns the attribute enumerator.
    /// </summary>
    /// <returns>The attribute enumerator.</returns>
    private IEnumerator IEnumerable_GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return IEnumerable_GetEnumerator();
    }

    /// <summary>
    /// Loading attributes and their values from an XML node.
    /// </summary>
    /// <param name="node">XML node.</param>
    /// <param name="nsmgr">The namespace manager.</param>
    internal void LoadFromXmlNode(XmlNode node, XmlNamespaceManager nsmgr)
    {
        // We quit if there is no node passed in parameter
        if (node is null)
        {
            return;
        }
        // We browse all values of type string
        foreach (XmlNode subNode in node.SelectNodes(".//samla:AttributeValue", nsmgr))
        {
            // We keep only the attributes of type string
            string attrType = XmlHelper.XmlAttributeValue(subNode, "xsi:type");
            if (!string.IsNullOrEmpty(attrType) && attrType != "xs:string")
            {
                continue;
            }
            // Retrieving the name of the attribute
            string attrName = XmlHelper.XmlAttributeValue(subNode.ParentNode, "Name");
            // Conservation of the attribute and its value
            if (attrName is not null)
            {
                Add(attrName, subNode.InnerText);
            }
        }
    }

    #endregion

}
