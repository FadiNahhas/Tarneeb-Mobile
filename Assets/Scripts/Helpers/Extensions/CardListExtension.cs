using System.Collections.Generic;
using System.Linq;
using SharedLibrary;

namespace Helpers.Extensions
{
    public static class CardListExtension
    {
        public static bool HasSuit(this List<Card> cards, Suit suit)
        {
            return cards.Any(card => card.Suit == suit);
        }
        
        public static List<Card> CardsWithSuit(this List<Card> cards, Suit suit)
        {
            return cards.Where(card => card.Suit == suit).ToList();
        }
        
        public static List<Card> CardsWithoutSuit(this List<Card> cards, Suit suit)
        {
            return cards.Where(card => card.Suit != suit).ToList();
        }
        
        public static Card HighestCard(this List<Card> cards)
        {
            return cards.OrderByDescending(card => card.Value).First();
        }
        
        public static Card LowestCard(this List<Card> cards)
        {
            return cards.OrderBy(card => card.Value).First();
        }
        
        public static Card LowestCardWithoutSuit(this List<Card> cards, Suit suit)
        {
            return cards.CardsWithoutSuit(suit).OrderBy(card => card.Value).First();
        }
        
        public static Card LowestCardWithSuit(this List<Card> cards, Suit suit)
        {
            return cards.CardsWithSuit(suit).OrderBy(card => card.Value).First();
        }
        
        public static Card HighestCardWithoutSuit(this List<Card> cards, Suit suit)
        {
            return cards.CardsWithoutSuit(suit).OrderByDescending(card => card.Value).First();
        }
        
        public static Card HighestCardWithSuit(this List<Card> cards, Suit suit)
        {
            return cards.CardsWithSuit(suit).OrderByDescending(card => card.Value).First();
        }
        
        public static List<Card> CardsWithinRange(this List<Card> cards, int start, int end)
        {
            return cards.Where(card => card.Value > start && card.Value < end).ToList();
        }
        
        public static List<Card> CardsAbove(this List<Card> cards, int value)
        {
            return cards.Where(card => card.Value > value).OrderBy(card => card.Value).ToList();
        }

        public static List<PlayedCard> CardsWithSuit(this List<PlayedCard> playedCards, Suit suit)
        {
            return playedCards.Where(playedCard => playedCard.Card.GetSuit() == suit).ToList();
        }
        
        public static bool ContainsSuit(this List<PlayedCard> playedCards, Suit suit)
        {
            return playedCards.Any(playedCard => playedCard.Card.GetSuit() == suit);
        }
        
        public static PlayedCard HighestPlayedCard(this List<PlayedCard> playedCards)
        {
            return playedCards.OrderByDescending(playedCard => playedCard.Card.Value).First();
        }
        
        public static PlayedCard HighestPlayedCardWithSuit(this List<PlayedCard> playedCards, Suit suit)
        {
            return playedCards.CardsWithSuit(suit).OrderByDescending(playedCard => playedCard.Card.Value).First();
        }
    }
}