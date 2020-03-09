namespace Nestor.DictBuilder
{
    class Program
    {
        public static void Main(string[] args)
        {
            // var n = new NestorMorph();
            LoadNestorDictionary();
        }

        private static void LoadNestorDictionary()
        {
            new NestorBuilder().BuildDictionary("hagen");
        }
    }
}