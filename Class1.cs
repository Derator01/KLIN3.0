using System.Text;

namespace KLIN;

public sealed class Klin
{
    private bool FileExists { get => File.Exists(_fullPath); }
    private readonly string _fullPath;

    private readonly Dictionary<string, bool> _bools = new();
    private readonly Dictionary<string, int> _ints = new();
    private readonly Dictionary<string, float> _floats = new();
    private readonly Dictionary<string, string> _strings = new();

    public Klin(string name = "Config.klin", string path = "./")
    {
        if (path[^1] != '/')
            _ = path.Append('/');

        _fullPath = path + name;

        Init();
    }

    public Klin(string path)
    {
        _fullPath = path;
        Init();
    }

    private void Init()
    {
        if (FileExists)
        {
            CreateFile();
            return;
        }
        SetFileAttributesReadable();
        var bytes = File.ReadAllBytes(_fullPath);
        SetFileAttributesUnReadable();

        var decoded = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        ParseFileContent(decoded);
    }

    private void SetFileAttributesReadable()
    {
        File.SetAttributes(_fullPath, FileAttributes.Normal);
    }

    private void SetFileAttributesUnReadable()
    {
        File.SetAttributes(_fullPath, FileAttributes.Hidden | FileAttributes.Encrypted | FileAttributes.ReadOnly);
    }

    private void ParseFileContent(string contents)
    {
        string[] parameters = contents.Split('|');

        if (contents.Split('|').Length != 4)
            return;

        string bools = parameters[0];
        string ints = parameters[1];
        string floats = parameters[2];
        string strings = parameters[3];

        if (bools.Length > 0)
            foreach (var boolKeyVal in bools.Split('\r'))
                _bools.Add(boolKeyVal.Split('=')[0], bool.Parse(boolKeyVal.Split('=')[1]));
        if (ints.Length > 0)
            foreach (var intKeyVal in ints.Split('\r'))
                _ints.Add(intKeyVal.Split('=')[0], int.Parse(intKeyVal.Split('=')[1]));
        if (floats.Length > 0)
            foreach (var floatKeyVal in ints.Split('\r'))
                _floats.Add(floatKeyVal.Split('=')[0], float.Parse(floatKeyVal.Split('=')[1]));
        if (strings.Length > 0)
            foreach (var stringKeyVal in ints.Split('\r'))
                _strings.Add(stringKeyVal.Split('=')[0], stringKeyVal.Split('=')[1]);
    }

    private void SaveToFile()
    {
        string toSave = "";

        foreach (var keyVal in _bools)
            toSave += $"{keyVal.Key}={keyVal.Value}\r";
        toSave += "|";
        foreach (var keyVal in _ints)
            toSave += $"{keyVal.Key}={keyVal.Value}\r";
        toSave += "|";
        foreach (var keyVal in _floats)
            toSave += $"{keyVal.Key}={keyVal.Value}\r";
        toSave += "|";
        foreach (var keyVal in _strings)
            toSave += $"{keyVal.Key}={keyVal.Value}\r";
        SetFileAttributesReadable();
        File.WriteAllBytes(_fullPath, Encoding.UTF8.GetBytes(toSave));
        SetFileAttributesUnReadable();
    }

    private void CreateFile()
    {
        var file = File.Create(_fullPath);
        file.Close();
        File.SetAttributes(_fullPath, FileAttributes.Hidden);
    }
}