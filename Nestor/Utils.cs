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
    }
}