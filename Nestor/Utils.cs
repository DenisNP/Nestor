using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Nestor
{
    internal static class Utils
    {
        internal static Stream LoadFile(string fileName)
        {
            var assembly = Assembly.GetCallingAssembly();
            var file = assembly.GetManifestResourceStream($"Nestor.Dict.{fileName}");

            if (file != null)
            {
                return file;
            }
            
            throw new IOException($"Cannot load file {fileName}");
        }

        internal static void LoadFileToList(List<string> list, string fileName)
        {
            using var fileStream = LoadFile(fileName);
            LoadStreamToList(list, fileStream);
        }

        internal static void LoadStreamToList(List<string> list, Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.IsNullOrEmpty())
                {
                    list.Add(line);
                }
            }
        }

        internal static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        internal static int AddOrCheck<T>(this List<T> list, T value)
        {
            var idx = list.IndexOf(value);
            if (idx < 0)
            {
                list.Add(value);
                return list.Count - 1;
            }

            return idx;
        }

        internal static string Join<T>(this IEnumerable<T> list, string separator)
        {
            return string.Join(separator, list);
        }
        
        internal static string GetOrEmpty(this List<string> list, int id)
        {
            return id == 0 ? "" : list[id - 1];
        }
    }
}