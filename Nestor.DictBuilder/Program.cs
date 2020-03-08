namespace Nestor.DictBuilder
{
    class Program
    {
        public static void Main(string[] args)
        {
            LoadNestorDictionary();
        }

        private static void LoadNestorDictionary()
        {
            new NestorLoader().BuildDictionary("hagen");
        }
    }
}