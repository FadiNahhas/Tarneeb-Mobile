using System.Collections.Generic;
using Events;
using Helpers.Dependency_Injection;
using SharedLibrary;
using Sirenix.OdinInspector;
using UnityEngine;
using Hand = Player.Hand;

namespace UI.Controllers
{
    public class PlayerHandsController : MonoBehaviour, IDependencyProvider
    {
        [TabGroup("UI Elements"), SerializeField]  
        private Hand localPlayerHand;
        
        [TabGroup("UI Elements"), SerializeField]  
        private Hand player1Hand;
        
        [TabGroup("UI Elements"), SerializeField]  
        private Hand[] hands;
        
        [Provide] public PlayerHandsController ProvideSelf() => this;
        
        private void OnEnable()
        {
            GlobalEvents.OnLocalPlayerSeatChange += OnLocalPlayerSeatChanged;
        }
        
        private void OnDisable()
        {
            GlobalEvents.OnLocalPlayerSeatChange -= OnLocalPlayerSeatChanged;
        }

        private void OnLocalPlayerSeatChanged(int newSeat)
        {
            // If the local player is not seated hide the local player seat and show the bottom seat
            if (newSeat == -1)
            {
                hands[0] = player1Hand;
                return;
            }
            
            // If the local player is seated show the local player seat and hide the bottom seat
            hands[0] = localPlayerHand;
        }
        
        public void AddCard(int seat, Card card, bool isFaceDown = false) => hands[seat].AddCard(card, isFaceDown);

        public void AddCards(int seat, List<Card> cards, bool isFaceDown = false) => hands[seat].AddCards(cards, isFaceDown);

        public void RemoveCard(int seat, Card card) => hands[seat].RemoveCard(card);

        public void HighlightCards(int seat, Suit? suit) => hands[seat].HighlightPlayableCards(suit);
    }
}