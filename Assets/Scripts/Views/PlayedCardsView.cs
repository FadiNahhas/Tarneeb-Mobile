using System.Collections.Generic;
using Cards;
using Helpers;
using Helpers.Dependency_Injection;
using SharedLibrary;
using UnityEngine;

namespace Views
{
    public class PlayedCardsView : MonoBehaviour, IDependencyProvider
    {
        [Inject] private SeatMapper _seatMapper;
        [SerializeField] private BaseCard[] playedCards;
    
        [Provide]
        public PlayedCardsView Provide() => this;

        public void SetCard(int seatId, Card card)
        {
            int mappedSeatId = _seatMapper.Map(seatId);
            playedCards[mappedSeatId].gameObject.SetActive(true);
            playedCards[mappedSeatId].Initialize(card);
        }
        
        public void ClearCards()
        {
            foreach (var card in playedCards)
            {
                card.gameObject.SetActive(false);
            }
        }

        public List<Card> GetCards()
        {
            var cards = new List<Card>();
            
            foreach (var card in playedCards)
            {
                if (card.gameObject.activeSelf)
                {
                    cards.Add(card.Card);
                }
            }
            
            return cards;
        }
    }
}
