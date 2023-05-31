using System.Xml;

namespace Lagoon.Server.Saml;

internal static class XmlHelper
{

    internal const string XML_NAMESPACE_PROTOCOL = "urn:oasis:names:tc:SAML:2.0:protocol";
    internal const string XML_NAMESPACE_XMLDSIGN = "http://www.w3.org/2000/09/xmldsig#";
    internal const string XML_NAMESPACE_METADATA = "urn:oasis:names:tc:SAML:2.0:metadata";
    internal const string XML_NAMESPACE_ASSERTION = "urn:oasis:names:tc:SAML:2.0:assertion";

    /// <summary>
    /// Adds a new element to the xml file.
    /// </summary>
    /// <param name="parent">Parent node.</param>
    /// <param name="name">Name of the new node.</param>
    /// <param name="attributes">Attributes of the new node or InnerText if there is only one value.</param>
    /// <returns></returns>
    public static XmlElement XmlAddNode(XmlNode parent, string name, params string[] attributes)
    {
        return XmlInsertElement(parent, null, false, name, attributes);
    }

    /// <summary>
    /// Adds a new element to the xml file.
    /// </summary>
    /// <param name="namespaceUri">URL of the element's namespace.</param>
    /// <param name="parent">Parent node.</param>
    /// <param name="name">Name of the new node.</param>
    /// <param name="attributes">Attributes of the new node or InnerText if there is only one value.</param>
    /// <returns></returns>
    public static XmlElement XmlAddNode(string namespaceUri, XmlNode parent, string name, params string[] attributes)
    {
        return XmlInsertElement(namespaceUri, parent, null, false, name, attributes);
    }

    /// <summary>
    /// Adds a new element to the xml file.
    /// </summary>
    /// <param name="item">Node before which to insert the element.</param>
    /// <param name="name">Name of the new node.</param>
    /// <param name="attributes">Attributes of the new node or InnerText if there is only one value.</param>
    /// <returns>The new node.</returns>
    public static XmlElement XmlInsertNode(XmlNode item, string name, params string[] attributes)
    {
        return XmlInsertElement(item.ParentNode, item, true, name, attributes);
    }

    /// <summary>
    /// Adds a new element to the xml file.
    /// </summary>
    /// <param name="namespaceUri">URL of the element's namespace.</param>
    /// <param name="item">Node before which to insert the element.</param>
    /// <param name="name">Name of the new node.</param>
    /// <param name="attributes">Attributes of the new node or InnerText if there is only one value</param>
    /// <returns>The new node.</returns>
    public static XmlElement XmlInsertNode(string namespaceUri, XmlNode item, string name, params string[] attributes)
    {
        return XmlInsertElement(namespaceUri, item.ParentNode, item, true, name, attributes);
    }

    /// <summary>
    /// Adds a new element to the xml file.
    /// </summary>
    /// <param name="parent">Noeud parent</param>
    /// <param name="relative">Child node to position yourself against.</param>
    /// <param name="before">Insert before the indicated node.</param>
    /// <param name="name">Name of the new node.</param>
    /// <param name="p_as_attributes">Attributes of the new node or InnerText if there is only one value.</param>
    /// <returns></returns>
    private static XmlElement XmlInsertElement(XmlNode parent, XmlNode relative, bool before, string name, params string[] p_as_attributes)
    {
        return XmlInsertElement(parent.NamespaceURI, parent, relative, before, name, p_as_attributes);
    }

    /// <summary>
    /// Adds a new element to the xml file.
    /// </summary>
    /// <param name="namespaceUri">URL of the element's namespace.</param>
    /// <param name="parent">The parent node.</param>
    /// <param name="relative">Child node to position yourself against.</param>
    /// <param name="before">Insert before the indicated node.</param>
    /// <param name="name">Name of the new node</param>
    /// <param name="p_as_attributes">Attributes of the new node or InnerText if there is only one value.</param>
    /// <returns></returns>
    private static XmlElement XmlInsertElement(string namespaceUri, XmlNode parent, XmlNode relative, bool before, string name, params string[] p_as_attributes)
    {
        XmlElement elt = parent.OwnerDocument.CreateElement(name, namespaceUri);
        // On ajoute les couples de paramètres en tant que nom/valeur d'attributs
        XmlAddAttributes(elt, p_as_attributes);
        // On ajoute le dernier éléments solitaire en tant que contenu texte de la balise
        if (p_as_attributes.Length % 2 != 0)
        {
            elt.InnerText = p_as_attributes[^1];
        }
        if (before)
        {
            parent.InsertBefore(elt, relative);
        }
        else if (relative == null)
        {
            parent.AppendChild(elt);
        }
        else
        {
            parent.InsertAfter(elt, relative);
        }
        return elt;
    }

    /// <summary>
    /// Adds new attributes to a node.
    /// </summary>
    /// <param name="elt">The node.</param>
    /// <param name="attributes">Couples attributes,value</param>
    public static void XmlAddAttributes(XmlNode elt, params string[] attributes)
    {
        for (int l_i = 0; l_i <= attributes.Length - 2; l_i += 2)
        {
            XmlAddAttribute(elt, attributes[l_i], attributes[l_i + 1]);
        }
    }

    /// <summary>
    /// Adds new attributes to a node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="value">Value of the attribute.</param>
    /// <returns>The newly created attribute.</returns>
    public static XmlAttribute XmlAddAttribute(XmlNode node, string name, string value)
    {
        XmlAttribute attr = node.OwnerDocument.CreateAttribute(name);
        attr.Value = value;
        node.Attributes.Append(attr);
        return attr;
    }

    /// <summary>
    /// Deletes an attribute and returns <c>True</c> if the deletion actually occurred.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns><c>true</c> if the deletion has actually taken place.</returns>
    public static bool XmlRemoveAttribute(XmlNode node, string attributeName)
    {
        if (node is null)
        {
            return false;
        }
        XmlAttribute attr = node.Attributes[attributeName];
        if (attr is null)
        {
            return false;
        }
        node.Attributes.Remove(attr);
        return true;
    }

    /// <summary>
    /// Returns the value of an attribute for a node.
    /// </summary>
    /// <param name="node">Node.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="defaultValue">Default value if the attribute does not exist.</param>
    /// <returns>The value of an attribute for a node.</returns>
    public static string XmlAttributeValue(XmlNode node, string attributeName, string defaultValue = null)
    {
        if (node is null)
        {
            return defaultValue;
        }
        XmlAttribute attr = node.Attributes[attributeName];
        return attr is null ? defaultValue : attr.Value;
    }

    /// <summary>
    /// Stores the value of an attribute for a node.
    /// </summary>
    /// <param name="node">Node.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="value">Value of the attribute.</param>
    public static void XmlSetAttributeValue(XmlNode node, string attributeName, string value)
    {
        XmlAttribute attr = node.Attributes[attributeName];
        if (attr is null)
        {
            XmlAddAttribute((XmlElement)node, attributeName, value);
        }
        else
        {
            attr.Value = value;
        }
    }

}
