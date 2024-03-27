using System.Collections.Generic;
using System.Linq;
using Player;
using SharedLibrary;

namespace Helpers.Extensions
{
    public static class HandExtension
    {
        public static bool HasSuit(this Hand hand, Suit suit)
        {
            return hand.GetCards().Any(card => card.Suit == suit);
        }
        
        public static List<Card> GetCardsWithSuit(this Hand hand, Suit suit)
        {
            return hand.GetCards().Where(card => card.Suit == suit).ToList();
        }

        public static List<Card> GetCardsWithoutSuit(this Hand hand, Suit suit)
        {
            return hand.GetCards().Where(card => card.Suit != suit).ToList();
        }
        
        public static Card GetHighestValueCard(this Hand hand)
        {
            return hand.GetCards().OrderByDescending(card => card.Value).First();
        }
        
        public static Card GetLowestValueCard(this Hand hand)
        {
            return hand.GetCards().OrderBy(card => card.Value).First();
        }

        public static Card GetLowestValueCardWithoutSuit(this Hand hand, Suit suit)
        {
            return hand.GetCardsWithoutSuit(suit).OrderBy(card => card.Value).First();
        }
        
        public static Card GetLowestCardWithSuit(this Hand hand, Suit suit)
        {
            return hand.GetCardsWithSuit(suit).OrderBy(card => card.Value).First();
        }

        public static Card GetHighestCardWithoutSuit(this Hand hand, Suit suit)
        {
            return hand.GetCardsWithoutSuit(suit).OrderByDescending(card => card.Value).First();
        }
    }
}