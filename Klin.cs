using KLIN.Storage;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace KLIN;

public sealed class Klin
{
    private bool FileExists => File.Exists(_fullPath);
    private readonly string _fullPath;
    private readonly MultiDictionary _dictionary = new();
    public bool AutoSaveEnabled;

    /// <summary>
    /// Creates and Initializes a Klin instance.
    /// </summary>
    /// <param name="name">Name of file with its extension</param>
    public Klin(bool autoSave = true, string name = "Config.klin", string path = "./")
    {
        if (path[^1] != '/')
            path = (string)path.Append('/');

        AutoSaveEnabled = autoSave;
        _fullPath = Path.Combine(path, name);

        Load();
    }

    /// <summary>
    /// Creates and Initializes a Klin instance.
    /// </summary>
    /// <param name="path">Needs to include a file name.</param>
    public Klin(string path)
    {
        _fullPath = path;
        Load();
    }

    private void Load()
    {
        if (!FileExists)
        {
            CreateFile();
            return;
        }

        SetFileAttributesReadable();
        var contents = File.ReadAllText(_fullPath);
        SetFileAttributesUnReadable();

        LoadFromText(contents);
    }

    private void LoadFromText(string contents)
    {
        // Split the contents into individual lines
        var lines = contents.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            // Split each line into type name and serialized data
            var parts = line.Split('|');
            if (parts.Length != 2)
                continue;

            string typeName = parts[0];
            string serializedData = parts[1];

            // Resolve the type by name
            Type type = Type.GetType(typeName);
            if (type == null)
                continue;

            // Deserialize the JSON data into a dictionary
            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(typeof(string), type);
            var innerDict = JsonSerializer.Deserialize(serializedData, dictionaryType);

            MethodInfo addDictionaryMethod = typeof(MultiDictionary).GetMethod("AddDictionary").MakeGenericMethod(type);
            addDictionaryMethod.Invoke(_dictionary, [innerDict]);
        }
    }

    public T Get<T>(string key)
    {
        return _dictionary.Get<T>(key);
    }

    public bool TryGet<T>(string key, out T value)
    {
        try
        {
            value = _dictionary.Get<T>(key);
            return true;
        }
        catch (Exception)
        {
            value = default;
            return false;
        }
    }

    public void Set<T>(string key, T value)
    {
        _dictionary.Set(key, value);
        if (AutoSaveEnabled)
            Save();
    }

    public void Save()
    {
        StringBuilder toSave = new StringBuilder();

        var dictionaryOfDictionaries = _dictionary.GetDictionary();

        foreach (var kvp in dictionaryOfDictionaries)
        {

            // Serialize key-value pairs in the inner dictionary using JSON
            string serializedData = JsonSerializer.Serialize((object?)kvp.Value);

            // Append type name and serialized data to the output
            toSave.AppendLine($"{kvp.Key.FullName}|{serializedData}");
        }

        SetFileAttributesReadable();
        File.WriteAllText(_fullPath, toSave.ToString());
        SetFileAttributesUnReadable();
    }

    private void CreateFile()
    {
        File.Create(_fullPath).Close();

        File.SetAttributes(_fullPath, FileAttributes.Hidden);
    }

    private void SetFileAttributesReadable()
    {
        File.SetAttributes(_fullPath, FileAttributes.Normal);
    }

    private void SetFileAttributesUnReadable()
    {
        File.SetAttributes(_fullPath, FileAttributes.Hidden | FileAttributes.Encrypted | FileAttributes.ReadOnly);
    }
}
