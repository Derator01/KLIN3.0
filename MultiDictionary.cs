namespace KLIN.Storage

public class MultiDictionary
{
    private readonly Dictionary<string, bool> _bools = new();
    private readonly Dictionary<string, int> _ints = new();
    private readonly Dictionary<string, float> _floats = new();
    private readonly Dictionary<string, string> _strings = new();

    public MultiDictionary()
    {

    }

    public T Get<T>(string key)
    {
        switch(typeof(T))
        {
            case Bool:
                return _bools[key];
                break;
            default:
                return null;
        }
    }
}