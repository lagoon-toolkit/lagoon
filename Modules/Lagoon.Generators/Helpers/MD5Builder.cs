namespace Lagoon.Generators;

internal class MD5Builder : IDisposable
{
    private System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create();

    public void Dispose()
    {
        _md5?.Dispose();
    }

    public void Add(string value)
    {

        Add(System.Text.Encoding.UTF8.GetBytes(value));
    }

    public void Add(DateTime value)
    {
        Add(BitConverter.GetBytes(value.Ticks));
    }

    public void Add(byte[] data)
    {
        _md5.TransformBlock(data, 0, data.Length, data, 0);
    }


    public void AddFolder(string path, string searchPattern)
    {
        if (Directory.Exists(path))
        {
            foreach (string file in Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories))
            {
                Add(file);
                Add(File.GetLastWriteTime(file));
            }
        }
    }

    public void AddSubFolder(string path, string subfolderName, string searchPattern)
    {
        AddFolder(Path.Combine(path, subfolderName), searchPattern);
    }

    public string GetHash()
    {
        _md5.TransformFinalBlock(new byte[] { 13 }, 0, 1);
        string result = Convert.ToBase64String(_md5.Hash);
        _md5.Dispose();
        _md5 = null;
        return result;
    }

}
