using System.Collections.Generic;
using System.Linq;
using Cards;
using Helpers.Extensions;
using SharedLibrary;
using UnityEngine;

namespace Player
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] private InteractableCard cardPrefab;
        [SerializeField] private Transform cardsParent;
        private List<InteractableCard> Cards { get; set; } = new();
        
        private void Start()
        {
            if (!cardPrefab)
            {
                Debug.LogError("Failed to add card to player hand: card prefab is not set");
            }

            if (!cardsParent)
            {
                Debug.LogWarning("Cards parent is not set, using self as parent");
                cardsParent = transform;
            }
        }

        private void OnEnable()
        {
            cardsParent.gameObject.SetActive(true);
        }
        
        private void OnDisable()
        {
            cardsParent.gameObject.SetActive(false);
        }

        public void AddCard(Card cardData, bool isFaceDown = false)
        {
            var card = Instantiate(cardPrefab, cardsParent);
            card.Initialize(cardData, isFaceDown);
            Cards.Add(card);
            
            if (!isFaceDown)
                SortCards(); 
        }
        
        public void AddCards(List<Card> cards, bool isFaceDown = false)
        {
            foreach (var card in cards)
            {
                AddCard(card, isFaceDown);
            }
        }
        
        public void RemoveCard(Card card)
        {
            if (Cards.Count == 0)
            {
                Debug.LogError("Failed to remove card from player hand: hand is empty");
                return;
            }
            
            var cardObject = Cards.Find(c => c.Card.Equals(card));
            
            if (cardObject == null)
            {
                Debug.LogError("Failed to remove card from player hand: card not found");
                return;
            }
            
            cardObject.DestroyCard();
            Cards.Remove(cardObject);
        }
        
        public List<Card> GetCards() => Cards.Select(c => c.Card).ToList();
        
        public void HighlightPlayableCards(Suit? leadSuit)
        {
            if (Cards.Count == 0)
            {
                Debug.LogWarning("Can't highlight cards from player hand: hand is empty");
                return;
            }

            // If no lead suit (first to play), highlight all cards
            if (leadSuit == null)
            {
                foreach (var card in Cards)
                {
                    card.SetHighlight(true);
                }
                return;
            }
            
            // Get all cards with the lead suit
            var leadSuitCards = this.GetCardsWithSuit(leadSuit.Value).Count;

            // If player has the lead suit, highlight all cards with the lead suit
            if (leadSuitCards > 0)
            {
                foreach (var card in Cards)
                {
                    card.SetHighlight(card.Card.Suit == leadSuit);
                }
                return;
            }
            
            // If player doesn't have the lead suit, highlight all cards
            foreach (var card in Cards)
            {
                card.SetHighlight(true);
            }
        }

        public void ShowCards(bool value)
        {
            foreach (var card in Cards)
            {
                card.ToggleFaceDown(!value);
            }
        }

        public void ClearHand()
        {
            foreach (var c in Cards)
            {
                c.DestroyCard();
            }
            Cards.Clear();
        }

        private void SortCards()
        {
            Cards = Cards.OrderBy(card => card.Card.Suit).ThenByDescending(card => card.Card.Value).ToList();
            
            for (var i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.SetSiblingIndex(i);
            }
        }
        
    }
}
