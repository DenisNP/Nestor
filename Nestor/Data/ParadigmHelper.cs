using System.Collections.Generic;

namespace Nestor.Data
{
    public static class ParadigmHelper
    {
        public static string ToString(ushort[] paradigm)
        {
            return string.Join(" ", paradigm);
        }

        public static ushort[] FromRules(List<MorphRule> rules)
        {
            int len = rules.Count;
            var p = new ushort[len * 4];
            for (var i = 0; i < rules.Count; i++)
            {
                MorphRule rule = rules[i];
                p[i] = rule.Prefix;
                p[len + i] = rule.Suffix;
                p[len * 2 + i] = rule.Stress;
                p[len * 3 + i] = rule.TagGroup;
            }

            return p;
        }

        public static ushort[] Empty()
        {
            return new ushort[4];
        }
    }
    
    public struct MorphRule
    {
        public ushort Prefix;
        public ushort Suffix;
        public ushort Stress;
        public ushort TagGroup;
    }
}