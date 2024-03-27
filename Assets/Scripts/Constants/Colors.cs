using SharedLibrary;
using UnityEngine;

namespace Constants
{
    public static class Colors
    {
        public const string Red = "#6A0004FF";
        public const string Black = "#050505FF";
        
        public static Color GetSuitColor(Suit? suit)
        {
            switch (suit)
            {
                case Suit.Clubs or Suit.Spades:
                    ColorUtility.TryParseHtmlString(Black, out var color);
                    return color;
                case Suit.Diamonds or Suit.Hearts:
                    ColorUtility.TryParseHtmlString(Red, out color);
                    return color;
                default:
                    return Color.white;
            }
        }
    }
}