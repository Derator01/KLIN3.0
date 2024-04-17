namespace KLIN.Storage;

public class MultiDictionary
{
    private readonly Dictionary<Type, object> _dictionaryOfDictionaries = new();

    public bool ContainsKey<T>(string key)
    {
        Type type = typeof(T);

        if (_dictionaryOfDictionaries.ContainsKey(type))
        {
            var innerDict = (Dictionary<string, T>)_dictionaryOfDictionaries[type];
            return innerDict.ContainsKey(key);
        }

        return false;
    }

    public T Get<T>(string key)
    {
        Type type = typeof(T);

        if (_dictionaryOfDictionaries.ContainsKey(type))
        {
            var innerDict = (Dictionary<string, T>)_dictionaryOfDictionaries[type];
            return innerDict[key];
        }
        throw new Exception($"Any Keys of Type:{type} not found");
    }

    public void Set<T>(string key, T value)
    {
        Type type = typeof(T);

        if (!_dictionaryOfDictionaries.ContainsKey(type))
        {
            _dictionaryOfDictionaries[type] = new Dictionary<string, T>();
        }

        var innerDict = (Dictionary<string, T>)_dictionaryOfDictionaries[type];
        innerDict[key] = value;
    }

    public Dictionary<Type, object> GetDictionary()
    {
        return _dictionaryOfDictionaries;
    }

    public void AddDictionary<T>(Dictionary<string, T> dictionary)
    {
        Type type = typeof(T);
        _dictionaryOfDictionaries.Add(type, dictionary);
    }
}
