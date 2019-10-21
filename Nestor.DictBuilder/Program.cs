namespace NestorDictBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            new Loader().BuildDictionary("hagen.zip", "dict.bin");
        }
    }
}