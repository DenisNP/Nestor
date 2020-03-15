using System;
using System.Collections.Generic;
using System.IO;

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
        
        internal static int ComplexAdd<T>(Dictionary<string, int> dict, List<T> list, T value, Func<T, string> convert)
        {
            if (value == null || (value is string s && s == ""))
            {
                return -1;
            }
            
            var index = dict.AddOrCheck(convert(value));
            if (index == list.Count)
            {
                list.Add(value);
            } 
            else if (index > list.Count)
            {
                throw new ArgumentException(
                    "Index in dictionary is greater than list size, something went wrong"
                );    
            }

            return index;
        }
    }
}