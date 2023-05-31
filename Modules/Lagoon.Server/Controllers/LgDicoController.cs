using Lagoon.Core.Application;
using Lagoon.Internal;
using System.Text.RegularExpressions;
using System.Xml;

namespace Lagoon.Server.Controllers;


/// <summary>
/// Error log files controller.
/// </summary>
[ApiController]
[Route(Routes.DICO_ROUTE)]
[ApiExplorerSettings(IgnoreApi = true)]
public class LgDicoController : LgControllerBase
{

    /// <summary>
    /// Check if config is in DEBUG mode
    /// </summary>
    /// <param name="app">LgApplication</param>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public LgDicoController(ILgApplication app)
    {
        if (!app.ApplicationInformation.IsDevelopment)
        {
            throw new UnauthorizedAccessException("Should only be used in development.");
        }
    }

    /// <summary>
    /// Return the server-side dictionnary content as CSV format
    /// </summary>
    /// <param name="knowKeys">List of key managed on client side</param>
    /// <param name="includeHeader">If the response should include the header row</param>
    [HttpPost(Routes.DICO_CSV)]
    public IActionResult GetDicoServer([FromBody] IEnumerable<string> knowKeys, bool includeHeader = true)
    {
        // Rq: Since the Server side dictionnary has an access to the UI dictionnaries
        // we must exclude key comming from the client side.
        return Ok(LgApplicationBase.Current.DicoToCsv("Server", includeHeader, knowKeys, false));
    }

    /// <summary>
    /// Return the list of keys managed by the server ide
    /// </summary>
    /// <returns></returns>
    [HttpGet(Routes.DICO_KEYS)]
    public ActionResult<IEnumerable<string>> GetDicoServerKey()
    {
        try
        {
            return Ok(LgApplicationBase.Current.GetDicoKeys().ToList());
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Update the content of Dico.xml with the provided files
    /// </summary>
    /// <param name="backup">if <c>true</c> the Dico.xml files will be copied before the update of dicos</param>
    [HttpPost(Routes.DICO_UPDATE)]
    public IActionResult UpdateDico(bool backup)
    {
        try
        {
            // Regex used to split a csv line into cells
            Regex regexSplitCsvLine = new("(?<=^|;)(\"(?:[^\"]|\"\")*\"|[^;]*)");
            Microsoft.AspNetCore.Http.IFormFile file = Request.Form.Files[0];
            // Load existing dictionnaries
            XmlDocument dicoServer = LoadXmlDocument("./Dico.xml", backup);
            XmlDocument dicoClient = LoadXmlDocument("./../Client/Dico.xml", backup);
            // Reading file content    
            using (StreamReader reader = new(file.OpenReadStream()))
            {
                // Read 1st line as header
                string header = reader.ReadLine();
                List<string> headers = regexSplitCsvLine.Matches(header).Where(x => !string.IsNullOrEmpty(x.Value)).Select(x => x.Value.ToLower().TrimQuotes()).ToList();
                // Read data
                while (reader.Peek() >= 0)
                {
                    // Split row as cells
                    string row = reader.ReadLine();
                    IEnumerable<string> rows = regexSplitCsvLine.Matches(row).Select(x => x.Value.TrimQuotes());
                    string currentKey = rows.ElementAt(headers.IndexOf("key"));
                    string dicoSide = rows.ElementAt(headers.IndexOf("side"));
                    XmlDocument documentToUpdate = dicoSide == "Server" ? dicoServer : dicoClient;
                    XmlNode currentNode = documentToUpdate.SelectSingleNode($"/root/string[@key='{currentKey}']");
                    if (currentNode != null)
                    {
                        // The node already exist, add or update attributes
                        foreach (string h in headers)
                        {
                            if (h is not "key" and not "side")
                            {
                                if (currentNode.Attributes[h] != null)
                                {
                                    // Update an existing attribute
                                    currentNode.Attributes[h].Value = rows.ElementAt(headers.IndexOf(h));
                                }
                                else
                                {
                                    // Add missing attribute
                                    XmlAttribute newAttr = documentToUpdate.CreateAttribute(h);
                                    newAttr.Value = rows.ElementAt(headers.IndexOf(h));
                                    currentNode.Attributes.SetNamedItem(newAttr);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Create a new node
                        XmlNode newNode = documentToUpdate.CreateNode(XmlNodeType.Element, "string", "");
                        foreach (string k in headers)
                        {
                            if (k != "side")
                            {
                                XmlAttribute newAttr = documentToUpdate.CreateAttribute(k);
                                if (k == "key")
                                {
                                    // Add 'key' attribute
                                    newAttr.Value = currentKey;
                                }
                                else
                                {
                                    // Add translation
                                    newAttr.Value = rows.ElementAt(headers.IndexOf(k));

                                }
                                newNode.Attributes.SetNamedItem(newAttr);
                            }
                        }
                        documentToUpdate.DocumentElement.InsertAfter(newNode, documentToUpdate.DocumentElement.LastChild);
                    }
                }
            }
            // Save modifications
            dicoClient.Save("./../Client/Dico.xml");
            dicoServer.Save("./Dico.xml");
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Load an XmlDocument from a file
    /// </summary>
    /// <param name="path">Path to the xml file</param>
    /// <param name="backup">Indicate if we must keep the old dictionary in a backup file.</param>
    /// <returns></returns>
    public XmlDocument LoadXmlDocument(string path, bool backup)
    {
        if (backup)
        {
            // We use the ".tmp" extension to exclude the backup from git
            System.IO.File.Copy(path, path.Replace(".xml", "-backup.tmp"), true);
        }
        XmlDocument doc = new();
        doc.Load(path);
        return doc;
    }
}


internal static class DicoHelpers
{

    /// <summary>
    /// Remove quote and start/end if present
    /// </summary>
    /// <param name="str">string to trim quote</param>
    /// <returns>The trimmed string</returns>
    public static string TrimQuotes(this string str)
    {
        if (str is not null && str.StartsWith('"') && str.EndsWith('"'))
        {
            StringBuilder sb = new(str, 1, str.Length - 2, str.Length - 2);
            sb.Replace("\"\"", "\"");
            return sb.ToString();
        }
        else
        {
            return str;
        }
    }

}
