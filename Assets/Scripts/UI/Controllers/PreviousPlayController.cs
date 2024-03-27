using System;
using System.Collections.Generic;
using Cards;
using Helpers.Dependency_Injection;
using SharedLibrary;
using UnityEngine;

namespace UI.Controllers
{
    public class PreviousPlayController : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private BaseCard[] cards;
        
        [Provide]
        public PreviousPlayController Provide() => this;
        
        public void DisplayPlay(Play play)
        {
            foreach (var playedCard in play.PlayedCards)
            {
                cards[playedCard.Seat].Initialize(playedCard.Card);
            }
        }
        
        public void DisplayPlay(List<Card> playedCards)
        {
            for (var i = 0; i < playedCards.Count; i++)
            {
                cards[i].gameObject.SetActive(true);
                cards[i].Initialize(playedCards[i]);
            }
        }
    
        public void ClearDisplay()
        {
            foreach (var card in cards)
            {
                card.gameObject.SetActive(false);
            }
        }

        public void Reorganize(int shiftAmount)
        {
            Dictionary<int, Card> cardMap = new();
            foreach (var card in cards)
            {
                if (!card.gameObject.activeSelf) continue;
                var index = Array.IndexOf(cards, card);
                cardMap.Add(index, card.Card);
            }
            
            ClearDisplay();
            
            foreach (var (index, card) in cardMap)
            {
                var newIndex = (index - shiftAmount) % cards.Length;
                if (newIndex < 0) newIndex += cards.Length;
                //cards[newIndex].RefreshVisuals(card);
                cards[newIndex].gameObject.SetActive(true);
            }
        }
    }
}
