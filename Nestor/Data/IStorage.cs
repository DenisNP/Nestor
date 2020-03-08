using System.Collections.Generic;

namespace Nestor.Data
{
    public interface IStorage
    {
        List<string> GetPrefixes();
        List<string> GetSuffixes();
        List<string> GetTags();
        List<string> GetTagGroups();

        string GetPrefix(int id);
        string GetSuffix(int id);
    }
}