using System.Collections;
using System.Text.RegularExpressions;

namespace Lagoon.Server.Application;

/// <summary>
/// The offlie file manager.
/// </summary>
public class OfflineAssetManager : IEnumerable<OfflineAsset>
{

    #region properties

    /// <summary>
    /// The asset list.
    /// </summary>
    private List<OfflineAsset> Assets { get; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="assets">The asset list.</param>
    public OfflineAssetManager(List<OfflineAsset> assets)
    {
        Assets = assets;
    }

    #endregion

    #region methods

    /// <summary>
    /// Add a new offline asset.
    /// </summary>
    /// <param name="url">The URL.</param>
    public void Add(string url)
    {
        Assets.Add(new() { Url = url });
    }

    /// <summary>
    /// Enumerate the assets.
    /// </summary>
    /// <returns>The list of assets.</returns>
    public IEnumerable<OfflineAsset> Enumerate()
    {
        return Assets;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<OfflineAsset> GetEnumerator()
    {
        return Assets.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Assets.GetEnumerator();
    }

    #endregion

    #region Url methods

    /// <summary>
    /// Include all the file with the specific extension.
    /// </summary>
    /// <param name="urls">The list of URLs</param>
    public void Include(params string[] urls)
    {
        ProcessUrl(false,  urls);
    }

    /// <summary>
    /// Exclude all the file with the specific extension.
    /// </summary>
    /// <param name="urls">The list of URLs</param>
    public void Exclude(params string[] urls)
    {
        ProcessUrl(true,  urls);
    }

    /// <summary>
    /// Exclude all the file with this URLs.
    /// </summary>
    /// <param name="exclude"></param>
    /// <param name="urls">The list of URLs.</param>
    private void ProcessUrl(bool exclude, string[] urls)
    {
        foreach (OfflineAsset a in Assets)
        {
            foreach (string url in urls)
            {
                if (a.Url.Equals(url, StringComparison.OrdinalIgnoreCase))
                {
                    a.Exclude = exclude;
                    break;
                }
            }
        }
    }

    #endregion

    #region Extensions methods

    /// <summary>
    /// Include all the file with the specific extension.
    /// </summary>
    /// <param name="extensions">The list of extensions</param>
    public void IncludeExtensions(params string[] extensions)
    {
        ProcessExtensions(false, false, extensions);
    }

    /// <summary>
    /// Include all the file with the specific extension and exclude the other ones.
    /// </summary>
    /// <param name="extensions">The list of extensions</param>
    public void IncludeOnlyExtensions(params string[] extensions)
    {
        ProcessExtensions(false, true, extensions);
    }

    /// <summary>
    /// Exclude all the file with the specific extension.
    /// </summary>
    /// <param name="extensions">The list of extensions</param>
    public void ExcludeExtensions(params string[] extensions)
    {
        ProcessExtensions(true, false, extensions);
    }

    /// <summary>
    /// Exclude all the file with the specific extension and include the other ones.
    /// </summary>
    /// <param name="extensions">The list of extensions</param>
    public void ExcludeOnlyExtensions(params string[] extensions)
    {
        ProcessExtensions(true, true, extensions);
    }

    /// <summary>
    /// Exclude all the file with the specific extension.
    /// </summary>
    /// <param name="exclude"></param>
    /// <param name="reset"></param>
    /// <param name="extensions">The list of extensions</param>
    private void ProcessExtensions(bool exclude, bool reset, string[] extensions)
    {
        foreach (OfflineAsset a in Assets)
        {
            if (reset)
            {
                a.Exclude = !exclude;
            }
            foreach (string ext in extensions)
            {
                if (a.Url.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                {
                    a.Exclude = exclude;
                    break;
                }
            }
        }
    }

    #endregion

    #region RegEx methods

    /// <summary>
    /// Include all url that match the regular expression.
    /// </summary>
    /// <param name="regexs">The list of regular expressions</param>
    public void IncludeRegex(params string[] regexs)
    {
        ProcessRegEx(false, false, regexs);
    }

    /// <summary>
    /// Include all url that match the regular expression.
    /// </summary>
    /// <param name="regexs">The list of regular expressions</param>
    public void IncludeOnlyRegex(params string[] regexs)
    {
        ProcessRegEx(false, true, regexs);
    }

    /// <summary>
    /// Exclude all url that match the regular expression.
    /// </summary>
    /// <param name="regexs">The list of regular expressions</param>
    public void ExcludeRegex(params string[] regexs)
    {
        ProcessRegEx(true, false, regexs);
    }

    /// <summary>
    /// Exclude all url that match the regular expression.
    /// </summary>
    /// <param name="regexs">The list of regular expressions</param>
    public void ExcludeOnlyRegex(params string[] regexs)
    {
        ProcessRegEx(true, true, regexs);
    }

    /// <summary>
    /// Exclude/include urls.
    /// </summary>
    /// <param name="exclude">Exclude or include.</param>
    /// <param name="reset">Change also the false results.</param>
    /// <param name="regexs">The list of regular expression.</param>
    private void ProcessRegEx(bool exclude, bool reset, string[] regexs)
    {
        var cregexs = regexs.Select(r=>new Regex(r)).ToArray();
        foreach (OfflineAsset a in Assets)
        {
            if (reset)
            {
                a.Exclude = !exclude;
            }
            foreach (var regex in cregexs)
            {
                if (regex.IsMatch(a.Url))
                {
                    a.Exclude = exclude;
                    break;
                }
            }
        }
    }

    #endregion
}