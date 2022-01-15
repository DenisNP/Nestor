using System;
using System.Linq;

namespace Nestor.Poetry
{
    public class Foot
    {
        public FootType Type { get; }

        public Foot(FootType type)
        {
            Type = type;
        }
        
        public string Name => Type switch {
            FootType.Unknown => "",
            FootType.Iambic => "ямб",
            FootType.Chorea => "хорей",
            FootType.Dactyl => "дактиль",
            FootType.Amphibrachium => "амфибрахий",
            FootType.Anapest => "анапест",
            _ => throw new ArgumentOutOfRangeException()
        };
        
        public string Description => Type switch {
            FootType.Unknown => "",
            FootType.Iambic => "Двусложный размер с ударением на второй слог",
            FootType.Chorea => "Двусложный размер с ударением на первый слог",
            FootType.Dactyl => "Трёхсложный размер с ударением на первый слог",
            FootType.Amphibrachium => "Трёхсложный размер с ударением на второй слог",
            FootType.Anapest => "Трёхсложный размер с ударением на третий слог",
            _ => throw new ArgumentOutOfRangeException()
        };
        
        public int[] Mask => Type switch {
            FootType.Unknown => new []{0},
            FootType.Iambic => new []{0,1},
            FootType.Chorea => new []{1,0},
            FootType.Dactyl => new []{1,0,0},
            FootType.Amphibrachium => new []{0,1,0},
            FootType.Anapest => new []{0,0,1},
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

        public string Schema => Type switch
        {
            FootType.Unknown => "",
            FootType.Iambic => " — |",
            FootType.Chorea => "| —",
            FootType.Dactyl => "| — —",
            FootType.Amphibrachium => "— | —",
            FootType.Anapest => "— — |",
            _ => throw new ArgumentOutOfRangeException()
        };

        public int[] GetMaskOfLength(int l)
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
}