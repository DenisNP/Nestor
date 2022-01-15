using System;
using System.Linq;

namespace Nestor.Poetry
{
    public record Foot(FootType Type)
    {
        public string Name => Type switch {
            FootType.Unknown => "",
            FootType.Iambic => "ямб",
            FootType.Chorea => "хорей",
            FootType.Dactyl => "дактиль",
            FootType.Amphibrachium => "амфибрахий",
            FootType.Anapest => "анапест",
            _ => throw new ArgumentOutOfRangeException()
        };

        public StressType[] Mask => Type switch {
            FootType.Unknown => new [] { StressType.StrictlyUnstressed },
            FootType.Iambic => new []{ StressType.StrictlyUnstressed, StressType.CanBeStressed },
            FootType.Chorea => new []{ StressType.CanBeStressed, StressType.StrictlyUnstressed },
            FootType.Dactyl => new []{ StressType.CanBeStressed, StressType.StrictlyUnstressed, StressType.StrictlyUnstressed},
            FootType.Amphibrachium => new []{ StressType.StrictlyUnstressed, StressType.CanBeStressed, StressType.StrictlyUnstressed },
            FootType.Anapest => new []{ StressType.StrictlyUnstressed, StressType.StrictlyUnstressed, StressType.CanBeStressed },
            _ => throw new ArgumentOutOfRangeException()
        };

        public int StepsCount
        {
            get
            {
                switch (Type)
                {
                    case FootType.Iambic:
                    case FootType.Chorea:
                        return 2;
                    case FootType.Dactyl:
                    case FootType.Amphibrachium:
                    case FootType.Anapest:
                        return 3;
                    case FootType.Unknown:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public StressType[] GetMaskOfLength(int l)
        {
            return Enumerable.Range(0, l).Select(x => Mask[x % Mask.Length]).ToArray();
        }
    }
    
    public enum FootType
    {
        Unknown,
        Iambic,
        Chorea,
        Dactyl,
        Amphibrachium,
        Anapest
    }
    
    public enum StressType
    {
        StrictlyUnstressed,
        CanBeStressed,
        StrictlyStressed,
    }
}