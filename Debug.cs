namespace Debugging;

public static class Debug
{
    private static KLIN.Klin storage = new();

    public static void Main()
    {
        storage.SetInt("Hell", 10);

        Console.WriteLine(storage.GetInt("Hell"));
    }
}