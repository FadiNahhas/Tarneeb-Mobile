using SharedLibrary;

namespace Constants
{
    public static class RankSuitFormatting
    {
        public static string GetRankString(int value) => value switch
        {
            2 => "2",
            3 => "3",
            4 => "4",
            5 => "5",
            6 => "6",
            7 => "7",
            8 => "8",
            9 => "9",
            10 => "10",
            11 => "J",
            12 => "Q",
            13 => "K",
            14 => "A",
            _ => "Invalid Rank"
        };
        
        public static string GetSuitString(Suit? suit) => suit switch
        {
            Suit.Clubs => "]",
            Suit.Diamonds => "[",
            Suit.Hearts => "{",
            Suit.Spades => "}",
            _ => "Invalid Suit"
        };
    }

}