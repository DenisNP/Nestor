namespace Nestor.DictBuilder
{
    class Program
    {
        public static void Main(string[] args)
        {
            // LoadNestorDictionary();
            LoadNestorChroniclesDictionary();
        }

        private static void LoadNestorDictionary()
        {
            new NestorLoader().BuildDictionary("hagen", "dict_new.bin");
        }

        private static void LoadNestorChroniclesDictionary()
        {
            const int from = 0;
            const int to = 20000;
            
            new NestorChroniclesLoader().BuildDictionary(
                "model_large",
                "wiki_ruscorp.bin",
                from,
                to
            );
        }
    }
}