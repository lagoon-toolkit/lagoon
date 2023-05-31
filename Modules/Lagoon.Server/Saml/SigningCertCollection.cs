using System.Security.Cryptography.X509Certificates;

namespace Lagoon.Server.Saml;

/// <summary>
/// Certificate public keys.
/// </summary>
public class SigningCertCollection : List<SigningCert>
{

    /// <summary>
    /// Adds a new certificate.
    /// </summary>
    /// <param name="base64">Certificate.</param>
    public void Add(string base64)
    {
        Add(SigningCert.FromBase64(base64));
    }

    /// <summary>
    /// Adds a new certificate.
    /// </summary>
    /// <param name="cert">Certificate.</param>
    public void Add(X509Certificate2 cert)
    {
        Add(SigningCert.FromCert(cert));
    }

    /// <summary>
    /// Indicates whether the key is contained in the collection.
    /// </summary>
    /// <param name="key">Certificate for signature.</param>
    /// <returns><c>True</c> if the key is contained in the collection; <c>False</c> otherwise.</returns>
    public new bool Contains(SigningCert key)
    {
        return Contains(key.ToBase64());
    }

    /// <summary>
    /// Indicates whether the key is contained in the collection.
    /// </summary>
    /// <param name="key">Public key.</param>
    /// <returns><c>True</c> if the key is contained in the collection; <c>False</c> otherwise.</returns>
    public bool Contains(string key)
    {
        foreach (SigningCert cert in this)
        {
            if (cert.ToBase64() == key)
            {
                return true;
            }
        }
        return false;
    }

}
