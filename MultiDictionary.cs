namespace KLIN.Storage;

public class MultiDictionary : Dictionary<Type, object>
{
    /// <summary>
    /// Checks if the dictionary contains a specific key for a given type.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    public bool ContainsKey<T>(string key)
    {
        Type type = typeof(T);
        if (TryGetValue(type, out object? obj))
        {
            if (obj is Dictionary<string, T> innerDict)
            {
                return innerDict.ContainsKey(key);
            }
        }
        return false;
    }

    /// <summary>
    /// Tries to get a value from the dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the value.</param>
    /// <param name="value">When this method returns, contains the value associated with the key if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if the key was found; otherwise, <c>false</c>.</returns>
    public bool TryGet<T>(string key, out T? value)
    {
        Type type = typeof(T);
        if (TryGetValue(type, out object? obj))
        {
            if (obj is Dictionary<string, T> innerDict)
            {
                return innerDict.TryGetValue(key, out value);
            }
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Gets a value from the dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the value.</param>
    /// <returns>The value associated with the key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
    public T Get<T>(string key)
    {
        Type type = typeof(T);
        if (TryGetValue(type, out object? obj))
        {
            if (obj is Dictionary<string, T> innerDict)
            {
                return innerDict[key];
            }
            else
            {
                throw new KeyNotFoundException($"Inner dictionary for type {type.FullName} is not of the expected type."); //Added
            }
        }
        throw new KeyNotFoundException($"Type: {type.FullName} or Key: {key} not found");
    }

    /// <summary>
    /// Sets a value in the dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the value.</param>
    /// <param name="value">The value to set.</param>
    public void Set<T>(string key, T value)
    {
        Type type = typeof(T);
        if (!ContainsKey(type))
        {
            this[type] = new Dictionary<string, T>();
        }
        if (this[type] is Dictionary<string, T> innerDict)
        {
            innerDict[key] = value;
        }
        else
        {
            throw new InvalidOperationException($"Inner dictionary for type {type.FullName} is not of the expected type.");
        }
    }

    /// <summary>
    /// Adds a dictionary of values for a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to add.</param>
    public void AddDictionary<T>(Dictionary<string, T> dictionary)
    {
        Add(typeof(T), dictionary);
    }

    /// <summary>
    /// Adds a dictionary of values for a specific type.  This is a non-generic version.
    /// </summary>
    /// <param name="type">The type of the values in the dictionary.</param>
    /// <param name="dictionary">The dictionary to add.</param>
    public void AddDictionary(Type type, Dictionary<string, object> dictionary)
    {
        if (!ContainsKey(type))
        {
            Add(type, dictionary);
        }
        else
        {
            if (this[type] is Dictionary<string, object> existingDictionary)
            {
                foreach (var kvp in dictionary)
                {
                    existingDictionary[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                throw new InvalidOperationException($"Inner dictionary for type {type.FullName} is not of the expected type.");
            }
        }
    }
}