using System;
using System.Collections.Generic;
using System.IO;

namespace Nestor.DictBuilder
{
    public static class Utils
    {
        public static void SaveListToFile<T>(List<T> list, string fileName)
        {
            Console.Write($"Saving file {fileName}...");
            using var file = new StreamWriter(fileName);
            foreach (var line in list)
            {
                file.WriteLine(line);
            }

            Console.WriteLine($"ok, lines wrote: {list.Count}");
        }
    }
}