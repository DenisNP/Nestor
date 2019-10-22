namespace Nestor.DictBuilder
{
    class Program
    {
        public static void Main(string[] args)
        {
//            LoadNestorDictionary();
            LoadNestorChroniclesDictionary();
        }

        private static void LoadNestorDictionary()
        {
            new NestorLoader().BuildDictionary("hagen", "dict_new.bin");
        }

        private static void LoadNestorChroniclesDictionary()
        {
            new NestorChroniclesLoader().BuildDictionary("model_large", "wiki_ruscorp.bin");
        }
    }
}