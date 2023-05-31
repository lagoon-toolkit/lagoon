using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;

namespace Lagoon.Server.Saml;


/// <summary>
/// Certificate for signing messages.
/// </summary>
public class SigningCert
{
    private X509Certificate2 _cert;

    /// <summary>
    /// Initialization of a new instance from the representation in base 64.
    /// </summary>
    /// <param name="cert">The certificate.</param>
    /// <returns>Initialization of a new instance from the representation in base 64.</returns>
    public static SigningCert FromBase64(string cert)
    {
        cert = CleanupBase64(cert);
        return string.IsNullOrEmpty(cert)
            ? throw new ArgumentNullException(nameof(cert))
            : new SigningCert(new X509Certificate2(Convert.FromBase64String(cert)));
    }

    /// <summary>
    /// Initialization of a new instance from the representation in base 64.
    /// </summary>
    /// <param name="cert">The certificate.</param>
    /// <returns>Initialization of a new instance from the representation in base 64.</returns>
    public static SigningCert FromCert(X509Certificate2 cert)
    {
        return new SigningCert(cert);
    }

    /// <summary>
    /// Initialisation d'une nouvelle instance de la classe.
    /// </summary>
    /// <param name="cert">The certificate.</param>
    private SigningCert(X509Certificate2 cert)
    {
        _cert = cert;
    }

    /// <summary>
    /// Returns the key in Base64 format.
    /// </summary>
    /// <param name="includePrivateKey">Indicates if the private key must be contained in the certificate in base 64 format.</param>
    /// <returns>The key in Base64 format.</returns>
    public string ToBase64(bool includePrivateKey = false)
    {
        return Convert.ToBase64String(_cert.Export(includePrivateKey ? X509ContentType.Pfx : X509ContentType.Cert));
    }

    /// <summary>
    /// Returns the key with a line break every 64 characters.
    /// </summary>
    /// <param name="newLine">Characters used for line breaks.</param>
    /// <param name="lineLength">Maximum length of a line.</param>
    /// <returns>The key with a line break every 64 characters.</returns>
    public string ToBase64(int lineLength, string newLine = "\n")
    {
        string base64 = ToBase64();
        int newLineCount = base64.Length / lineLength;
        StringBuilder l_sb = new(base64, base64.Length + (newLineCount * newLine.Length));

        for (int l_i = newLineCount; l_i >= 1; l_i += -1)
        {
            l_sb.Insert(l_i * lineLength, newLine);
        }

        return l_sb.ToString();
    }

    /// <summary>
    /// Return the certificate.
    /// </summary>
    /// <returns>The certificate.</returns>
    public X509Certificate2 ToCert()
    {
        return _cert;
    }

    /// <summary>
    /// Returns the key in Base64 format.
    /// </summary>
    /// <returns>The key in Base64 format.</returns>
    public override string ToString()
    {
        return $"{_cert.Subject} ({_cert.NotBefore} --> {_cert.NotAfter})";
    }

    /// <summary>
    /// Returns the string in base 64 representing the certificate without line break and without space.
    /// </summary>
    /// <param name="cert">Chain representing the certificate.</param>
    /// <returns>The string in base 64 representing the certificate without line break and without space.</returns>
    internal static string CleanupBase64(string cert)
    {
        if (string.IsNullOrEmpty(cert))
        {
            return cert;
        }

        StringBuilder l_sb = new(cert);
        l_sb.Replace(" ", "");
        l_sb.Replace(Constants.vbTab, "");
        l_sb.Replace(Constants.vbCr, "");
        l_sb.Replace(Constants.vbLf, "");
        return l_sb.ToString();
    }

}
