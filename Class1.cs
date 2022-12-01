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
            CreateFile();


    }

    private void ParseFileContent(string contents)
    {
        string[] params = contents.Split(|);

        if (contents.Split(|).Length != 4)
            return;

        string bools = params[0];
        string ints = params[1];
        string floats = params[2];
        string strings = params[3];

        if (bools.Length > 0)
            foreach (var boolKeyVal in bools.Split('\r'))
                _bools.Add(boolKeyVal.Split('=')[0], Bool.Parse(boolKeyVal.Split('=')[1]));
        if (ints.Length > 0)
            foreach (var intKeyVal in ints.Split('\r'))
                _ints.Add(intKeyVal.Split('=')[0], Int.Parse(intKeyVal.Split('=')[1]));
        if (floats.Length > 0)
            foreach (var floatKeyVal in ints.Split('\r'))
                _floats.Add(floatKeyVal.Split('=')[0], floats.Parse(intKeyVal.Split('=')[1]));
        if (strings.Length > 0)
            foreach (var stringKeyVal in ints.Split('\r'))
                _strings.Add(stringKeyVal.Split('=')[0], intKeyVal.Split('=')[1]);
    }

    private void CreateFile()
    {
        var file = File.Create(_fullPath);
        file.Close();
        File.SetAttributes(_fullPath, FileAttributes.Hidden);
    }
}