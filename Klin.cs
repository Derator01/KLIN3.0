using System.Text;
using System.Text.Json;

namespace KLIN;

public sealed class Klin
{
    private readonly string _fullPath;
    private readonly MultiDictionary _data = new();
    public bool AutoSaveEnabled { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Klin"/> class.
    /// </summary>
    /// <param name="autoSave">Whether auto-save is enabled.</param>
    /// <param name="name">The name of the file (including extension).</param>
    /// <param name="path">The path to the file.</param>
    public Klin(bool autoSave = true, string name = "Config.klin", string path = "./")
    {
        // Use Path.Combine for safer path handling.
        _fullPath = Path.Combine(path, name);
        AutoSaveEnabled = autoSave;
        LoadData(); // Renamed to reflect purpose
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Klin"/> class.
    /// </summary>
    /// <param name="path">The full path to the file.</param>
    public Klin(string path)
    {
        _fullPath = path;
        LoadData(); // Renamed to reflect purpose.
    }

    private bool FileExists => File.Exists(_fullPath);

    private void LoadData() // Changed name from Load
    {
        if (!FileExists)
        {
            CreateFile();
            return;
        }

        try
        {
            SetFileAttributesReadability(true);
            string contents = File.ReadAllText(_fullPath);
            LoadFromText(contents);
        }
        finally
        {
            SetFileAttributesReadability(false);
        }
    }

    private void LoadFromText(string contents)
    {
        string[] lines = contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (parts.Length != 2)
            {
                continue; // Consider logging invalid lines.
            }

            string typeName = parts[0];
            string serializedData = parts[1];

            Type? type = Type.GetType(typeName);
            if (type == null)
            {
                continue; // Consider logging unknown types.
            }

            try
            {
                // Use the non-generic form of JsonSerializer.Deserialize
                object? innerDict = JsonSerializer.Deserialize(serializedData, typeof(Dictionary<string, object>));

                if (innerDict is Dictionary<string, object> dictionary)
                {
                    _data.AddDictionary(type, dictionary);
                }
                else
                {
                    // Log error, deserialized object was not a dictionary
                    Console.WriteLine($"Error: Deserialized object was not a dictionary for type {typeName}");
                }
            }
            catch (JsonException ex)
            {
                // Handle deserialization errors, possibly log them.
                Console.WriteLine($"Error deserializing data for type {typeName}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Retrieves a value from the Klin data.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the value.</param>
    /// <returns>The value associated with the key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
    public T Get<T>(string key)
    {
        return _data.Get<T>(key);
    }

    /// <summary>
    /// Tries to retrieve a value from the Klin data.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the value.</param>
    /// <param name="value">When this method returns, contains the value associated with the key if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if the key was found; otherwise, <c>false</c>.</returns>
    public bool TryGet<T>(string key, out T? value)
    {
        return _data.TryGet(key, out value);
    }

    /// <summary>
    /// Sets a value in the Klin data.
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="key">The key of the value.</param>
    /// <param name="value">The value to set.</param>
    public void Set<T>(string key, T value)
    {
        _data.Set(key, value);
        if (AutoSaveEnabled)
        {
            SaveData(); // Renamed to reflect purpose
        }
    }

    /// <summary>
    /// Saves the Klin data to the file.
    /// </summary>
    public void Save() // Changed to SaveData
    {
        SaveData();
    }

    private void SaveData() // Changed name from Save
    {
        StringBuilder stringBuilder = new();

        foreach (KeyValuePair<Type, object> kvp in _data)
        {
            // Serialize the inner dictionary.  No need to cast to object.
            string serializedData = JsonSerializer.Serialize(kvp.Value);
            stringBuilder.AppendLine($"{kvp.Key.FullName}|{serializedData}");
        }

        try
        {
            SetFileAttributesReadability(true);
            File.WriteAllText(_fullPath, stringBuilder.ToString());
        }
        finally
        {
            SetFileAttributesReadability(false);
        }
    }

    private void CreateFile()
    {
        try
        {
            using (File.Create(_fullPath)) { } // Use using to ensure the stream is closed.
            SetFileAttributesReadability(true);
        }
        finally
        {
            SetFileAttributesReadability(false); // Ensure attributes are reset even if CreateFile fails.
        }
    }

    private void SetFileAttributesReadability(bool readable)
    {
        File.SetAttributes(_fullPath, readable ? FileAttributes.Normal : (FileAttributes.Hidden | FileAttributes.Encrypted | FileAttributes.ReadOnly));
    }
}