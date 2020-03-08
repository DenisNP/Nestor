using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nestor.DictBuilder
{
    internal static class Utils
    {
        internal static void SaveListToFile<T>(List<T> list, string fileName)
        {
            Console.Write($"Saving file {fileName}...");
            using var file = new StreamWriter(fileName);
            foreach (var line in list)
            {
                file.WriteLine(line);
            }

            Console.WriteLine($"ok, lines wrote: {list.Count}");
        }
        
        internal static int AddOrCheck<T>(this Dictionary<T, int> dict, T value)
        {
            if (dict.ContainsKey(value))
            {
                return dict[value];
            }

            var index = dict.Count;
            dict.Add(value, index);

            return index;
        }
    }
}