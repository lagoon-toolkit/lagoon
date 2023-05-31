using System.Xml;

namespace Lagoon.Generators;

/// <summary>
/// Aides à la manipulation de données au format XML.
/// </summary>
public static class XmlHelper
{


    /// <summary>
    /// Retourne les noeuds correspondant au XPath.
    /// </summary>
    /// <param name="p_o_xml">Le document XML.</param>
    /// <param name="p_s_xpath">Expression XPath.</param>
    /// <returns>Les noeuds correspondant au XPath</returns>
    /// <remarks>Cette méthode fonctionne également Si un "xmlns" est défini sur un noeud.</remarks>
    public static XmlNode SelectSingleNode(XmlDocument p_o_xml, string p_s_xpath)
    {
        XmlNamespaceManager l_o_nsm = new(p_o_xml.NameTable);

        if (p_o_xml.DocumentElement.NamespaceURI != null)
        {
            l_o_nsm.AddNamespace("dz", p_o_xml.DocumentElement.NamespaceURI);
            p_s_xpath = AddDzPrefix(p_s_xpath);
        }
        return p_o_xml.DocumentElement.SelectSingleNode(p_s_xpath, l_o_nsm);
    }

    /// <summary>
    /// Retourne le sous noeud correspondant au XPath.
    /// </summary>
    /// <param name="p_o_node">Noeud.</param>
    /// <param name="p_s_xpath">Expression XPath.</param>
    /// <returns>Le sous noeud correspondant au XPath.</returns>
    public static XmlNode SelectSingleNode(XmlNode p_o_node, string p_s_xpath)
    {
        XmlNamespaceManager l_o_nsm = new(p_o_node.OwnerDocument.NameTable);

        if (p_o_node.NamespaceURI != null)
        {
            l_o_nsm.AddNamespace("dz", p_o_node.NamespaceURI);
            p_s_xpath = AddDzPrefix(p_s_xpath);
        }
        return p_o_node.SelectSingleNode(p_s_xpath, l_o_nsm);
    }

    /// <summary>
    /// Retourne les noeuds correspondant au XPath.
    /// </summary>
    /// <param name="p_o_xml">Le document XML.</param>
    /// <param name="p_s_xpath">Expression XPath.</param>
    /// <returns>Les noeuds correspondant au XPath.</returns>
    /// <remarks>Cette méthode fonctionne également Si un "xmlns" est défini sur un noeud.</remarks>
    public static XmlNodeList SelectNodes(XmlDocument p_o_xml, string p_s_xpath)
    {
        XmlNamespaceManager l_o_nsm = new(p_o_xml.NameTable);

        if (p_o_xml.DocumentElement.NamespaceURI != null)
        {
            l_o_nsm.AddNamespace("dz", p_o_xml.DocumentElement.NamespaceURI);
            p_s_xpath = AddDzPrefix(p_s_xpath);
        }
        return p_o_xml.DocumentElement.SelectNodes(p_s_xpath, l_o_nsm);
    }

    /// <summary>
    /// Retourne les noeuds correspondant au XPath.
    /// </summary>
    /// <param name="p_o_node">Noeud.</param>
    /// <param name="p_s_xpath">Expression XPath.</param>
    /// <returns>Les noeuds correspondant au XPath.</returns>
    /// <remarks>Cette méthode fonctionne également Si un "xmlns" est défini sur un noeud.</remarks>
    public static XmlNodeList SelectNodes(XmlNode p_o_node, string p_s_xpath)
    {
        XmlNamespaceManager l_o_nsm = new(p_o_node.OwnerDocument.NameTable);

        if (p_o_node.NamespaceURI != null)
        {
            l_o_nsm.AddNamespace("dz", p_o_node.NamespaceURI);
            p_s_xpath = AddDzPrefix(p_s_xpath);
        }
        return p_o_node.SelectNodes(p_s_xpath, l_o_nsm);
    }

    /// <summary>
    /// Retourne le chemin xpath avec le prefix correspondant à l'espace de nom du noeud principal.
    /// </summary>
    /// <param name="p_s_xpath">Expression XPath.</param>
    /// <returns>Le chemin xpath avec le prefix correspondant à l'espace de nom du noeud principal.</returns>
    private static string AddDzPrefix(string p_s_xpath)
    {
        StringBuilder l_sb = new(p_s_xpath);

        l_sb.Replace("/", "/dz:");
        l_sb.Replace("/dz:/dz:", "//dz:");
        return l_sb.ToString();
    }

    /// <summary>
    /// Retourne La liste des noeuds ayant le nom passé en paramètre et enfants du noeud passé en paramètre.
    /// </summary>
    /// <param name="p_o_node">Noeud parent.</param>
    /// <param name="p_s_subGroupTagName">Tag du sous noeud contenant les éléments à parcourir.</param>
    /// <returns>La liste des noeuds ayant le nom passé en paramètre et enfants du noeud passé en paramètre.
    /// </returns>
    public static IEnumerable<XmlNode> SelectGroupChildNodes(XmlNode p_o_node, string p_s_subGroupTagName)
    {
        foreach (XmlNode l_o_group in p_o_node.ChildNodes)
        {
            if (p_s_subGroupTagName.Equals(l_o_group.Name, StringComparison.OrdinalIgnoreCase))
            {
                foreach (XmlNode l_o_node in l_o_group.ChildNodes)
                    yield return l_o_node;
            }
        }
    }



    /// <summary>
    /// Ajoute un nouvel élément dans le fichier xml
    /// </summary>
    /// <param name="p_o_parent">Noeud parent</param>
    /// <param name="p_s_name">Nom du nouveau noeud</param>
    /// <param name="p_s_attributes">Attributs du nouveau noeud ou InnerText s'il n'y a qu'une valeur</param>
    /// <returns></returns>
    public static XmlElement AddNode(XmlNode p_o_parent, string p_s_name, params string[] p_s_attributes)
    {
        return InsertElement(p_o_parent, null, false, p_s_name, p_s_attributes);
    }

    /// <summary>
    /// Ajoute un nouvel élément dans le fichier xml
    /// </summary>
    /// <param name="p_s_namespaceUri">URL de l'espace de nom de l'élément.</param>
    /// <param name="p_o_parent">Noeud parent.</param>
    /// <param name="p_s_name">Nom du nouveau noeud</param>
    /// <param name="p_s_attributes">Attributs du nouveau noeud ou InnerText s'il n'y a qu'une valeur</param>
    /// <returns></returns>
    public static XmlElement AddNode(string p_s_namespaceUri, XmlNode p_o_parent, string p_s_name, params string[] p_s_attributes)
    {
        return InsertElement(p_s_namespaceUri, p_o_parent, null, false, p_s_name, p_s_attributes);
    }

    /// <summary>
    /// Ajoute un nouvel élément dans le fichier xml
    /// </summary>
    /// <param name="p_o_item">Noeud avant lequel insérer l'élément.</param>
    /// <param name="p_s_name">Nom du nouveau noeud</param>
    /// <param name="p_s_attributes">Attributs du nouveau noeud ou InnerText s'il n'y a qu'une valeur</param>
    /// <returns>Le nouveau noeud.</returns>
    public static XmlElement InsertNode(XmlNode p_o_item, string p_s_name, params string[] p_s_attributes)
    {
        return InsertElement(p_o_item.ParentNode, p_o_item, true, p_s_name, p_s_attributes);
    }

    /// <summary>
    /// Ajoute un nouvel élément dans le fichier xml
    /// </summary>
    /// <param name="p_s_namespaceUri">URL de l'espace de nom de l'élément.</param>
    /// <param name="p_o_item">Noeud avant lequel insérer l'élément.</param>
    /// <param name="p_s_name">Nom du nouveau noeud</param>
    /// <param name="p_s_attributes">Attributs du nouveau noeud ou InnerText s'il n'y a qu'une valeur</param>
    /// <returns>Le nouveau noeud.</returns>
    public static XmlElement InsertNode(string p_s_namespaceUri, XmlNode p_o_item, string p_s_name, params string[] p_s_attributes)
    {
        return InsertElement(p_s_namespaceUri, p_o_item.ParentNode, p_o_item, true, p_s_name, p_s_attributes);
    }

    /// <summary>
    /// Ajoute un nouvel élément dans le fichier xml
    /// </summary>
    /// <param name="p_o_parent">Noeud parent</param>
    /// <param name="p_o_relative">Noeud enfant par rapport auquel se positionner</param>
    /// <param name="p_b_before">Inserer avant le noeud indiqué</param>
    /// <param name="p_s_name">Nom du nouveau noeud</param>
    /// <param name="p_as_attributes">Attributs du nouveau noeud ou InnerText s'il n'y a qu'une valeur</param>
    /// <returns></returns>
    private static XmlElement InsertElement(XmlNode p_o_parent, XmlNode p_o_relative, bool p_b_before, string p_s_name, params string[] p_as_attributes)
    {
        return InsertElement(p_o_parent.NamespaceURI, p_o_parent, p_o_relative, p_b_before, p_s_name, p_as_attributes);
    }

    /// <summary>
    /// Ajoute un nouvel élément dans le fichier xml
    /// </summary>
    /// <param name="p_s_namespaceUri">URL de l'espace de nom de l'élément.</param>
    /// <param name="p_o_parent">Noeud parent</param>
    /// <param name="p_o_relative">Noeud enfant par rapport auquel se positionner</param>
    /// <param name="p_b_before">Inserer avant le noeud indiqué</param>
    /// <param name="p_s_name">Nom du nouveau noeud</param>
    /// <param name="p_as_attributes">Attributs du nouveau noeud ou InnerText s'il n'y a qu'une valeur</param>
    /// <returns></returns>
    private static XmlElement InsertElement(string p_s_namespaceUri, XmlNode p_o_parent, XmlNode p_o_relative, bool p_b_before, string p_s_name, params string[] p_as_attributes)
    {
        XmlElement l_o_elt;

        l_o_elt = p_o_parent.OwnerDocument.CreateElement(p_s_name, p_s_namespaceUri);
        // On ajoute les couples de paramètres en tant que nom/valeur d'attributs
        AddAttributes(l_o_elt, p_as_attributes);
        // On ajoute le dernier éléments solitaire en tant que contenu texte de la balise
        if (p_as_attributes.Length % 2 != 0)
            l_o_elt.InnerText = p_as_attributes[p_as_attributes.Length - 1];
        if (p_b_before)
            p_o_parent.InsertBefore(l_o_elt, p_o_relative);
        else if (p_o_relative == null)
            p_o_parent.AppendChild(l_o_elt);
        else
            p_o_parent.InsertAfter(l_o_elt, p_o_relative);
        return l_o_elt;
    }



    /// <summary>
    /// Insère un nouveau commentaire dans le fichier Web.Config
    /// </summary>
    /// <param name="p_o_parent">Noeud parent</param>
    /// <param name="p_s_comment">Commentaire à inscrire</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static XmlComment AddComment(XmlNode p_o_parent, string p_s_comment)
    {
        XmlComment l_o_elt;

        l_o_elt = p_o_parent.OwnerDocument.CreateComment(p_s_comment);
        p_o_parent.AppendChild(l_o_elt);
        return l_o_elt;
    }

    /// <summary>
    /// Insère un nouveau commentaire dans le fichier Web.Config
    /// </summary>
    /// <param name="p_o_parent">Noeud parent</param>
    /// <param name="p_s_comment">Commentaire à inscrire</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static XmlComment InsertComment(XmlNode p_o_parent, string p_s_comment)
    {
        return InsertComment(p_o_parent, p_o_parent.FirstChild, true, p_s_comment);
    }

    /// <summary>
    /// Insère un nouveau commentaire dans le fichier Web.Config
    /// </summary>
    /// <param name="p_o_parent">Noeud parent</param>
    /// <param name="p_o_relative">Noeud enfant par rapport auquel se positionner</param>
    /// <param name="p_b_before">Inserer avant le noeud indiqué</param>
    /// <param name="p_s_comment">Commentaire à inscrire</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private static XmlComment InsertComment(XmlNode p_o_parent, XmlNode p_o_relative, bool p_b_before, string p_s_comment)
    {
        XmlComment l_o_elt;

        l_o_elt = p_o_parent.OwnerDocument.CreateComment(p_s_comment);
        if (p_b_before)
            p_o_parent.InsertBefore(l_o_elt, p_o_relative);
        else
            p_o_parent.InsertAfter(l_o_elt, p_o_relative);
        return l_o_elt;
    }

    /// <summary>
    /// Ajoute un noeud en recopiant un autre noeud.
    /// </summary>
    /// <param name="p_o_node">Noeud source.</param>
    /// <param name="p_o_dst">Noeud de destination.</param>
    /// <returns>Le nouveau noeud cloné.</returns>
    public static XmlNode AddNodeClone(XmlNode p_o_node, XmlNode p_o_dst)
    {
        XmlNode l_o_node = p_o_dst.OwnerDocument.ImportNode(p_o_node, true);

        p_o_dst.AppendChild(l_o_node);
        return l_o_node;
    }

    /// <summary>
    /// Retourne le texte contenu dans un noeud enfant du noeud passé en paramètre.
    /// </summary>
    /// <param name="p_o_node">Noeud parent.</param>
    /// <param name="p_s_childName">Tag du noeud enfant.</param>
    /// <param name="p_s_default">Valeur par défaut.</param>
    /// <returns>Le texte contenu dans un noeud enfant du noeud passé en paramètre.</returns>
    public static string ChildNodeValue(XmlNode p_o_node, string p_s_childName, string p_s_default = null)
    {
        if (p_o_node == null)
            return p_s_default;
        foreach (XmlNode l_o_node in p_o_node.ChildNodes)
        {
            if (p_s_childName.Equals(l_o_node.Name, StringComparison.OrdinalIgnoreCase))
                return l_o_node.InnerText;
        }
        return p_s_default;
    }


    /// <summary>
    /// Ajoute de nouveaux attributs à un noeud
    /// </summary>
    /// <param name="p_o_elt">Noeud</param>
    /// <param name="p_s_attributes">Couples attributs,valeur</param>
    public static void AddAttributes(XmlNode p_o_elt, params string[] p_s_attributes)
    {
        for (int l_i = 0; l_i <= p_s_attributes.Length - 2; l_i += 2)
            AddAttribute(p_o_elt, p_s_attributes[l_i], p_s_attributes[l_i + 1]);
    }

    /// <summary>
    /// Ajoute de nouveaux attributs à un noeud
    /// </summary>
    /// <param name="p_o_node">Noeud</param>
    /// <param name="p_s_name">Nom de l'attribut</param>
    /// <param name="p_s_value">Valeur de l'attribut</param>
    /// <returns>Le nouvel attribut créé.</returns>
    public static XmlAttribute AddAttribute(XmlNode p_o_node, string p_s_name, string p_s_value)
    {
        XmlAttribute l_o_attr;

        l_o_attr = p_o_node.OwnerDocument.CreateAttribute(p_s_name);
        l_o_attr.Value = p_s_value;
        p_o_node.Attributes.Append(l_o_attr);
        return l_o_attr;
    }

    /// <summary>
    /// Supprime un attribut et retourne <c>True</c> si la suppression a effectivement eu lieu.
    /// </summary>
    /// <param name="p_o_node">Noeud.</param>
    /// <param name="p_s_attr">Nom de l'attribut.</param>
    /// <returns><c>True</c> si la suppression a effectivement eu lieu.</returns>
    public static bool RemoveAttribute(XmlNode p_o_node, string p_s_attr)
    {
        XmlAttribute l_o_attr;

        if (p_o_node == null)
            return false;
        l_o_attr = p_o_node.Attributes[p_s_attr];
        if (l_o_attr == null)
            return false;
        p_o_node.Attributes.Remove(l_o_attr);
        return true;
    }

    /// <summary>
    /// Retourne la valeur d'un attribut pour un noeud.
    /// </summary>
    /// <param name="p_o_node">Noeud.</param>
    /// <param name="p_s_attr">Nom de l'attribut.</param>
    /// <param name="p_s_default">Valeur par défaut si l'attribut n'existe pas.</param>
    /// <returns>La valeur d'un attribut pour un noeud.</returns>
    public static string GetAttributeValue(XmlNode p_o_node, string p_s_attr, string p_s_default = null)
    {
        XmlAttribute l_o_attr;

        if (p_o_node == null)
            return p_s_default;
        l_o_attr = p_o_node.Attributes[p_s_attr];
        if (l_o_attr == null)
            return p_s_default;
        return l_o_attr.Value;
    }

    /// <summary>
    /// Enregistre la valeur d'un attribut pour un noeud.
    /// </summary>
    /// <param name="p_o_node">Noeud.</param>
    /// <param name="p_s_attr">Nom de l'attribut.</param>
    /// <param name="p_s_value">Valeur de l'attribut.</param>
    public static void SetAttributeValue(XmlNode p_o_node, string p_s_attr, string p_s_value)
    {
        XmlAttribute l_o_attr = p_o_node.Attributes[p_s_attr];

        if (l_o_attr == null)
            AddAttribute((XmlElement)p_o_node, p_s_attr, p_s_value);
        else
            l_o_attr.Value = p_s_value;
    }
}
