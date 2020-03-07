using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
    }
}