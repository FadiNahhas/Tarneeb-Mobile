using Events;
using Helpers;
using SharedLibrary;
using UnityEngine;

namespace Audio
{
    public class AudioController : MonoBehaviour
    {
        private void OnEnable()
        {
            GlobalEvents.OnTurnChanged += OnPlayerTurnChanged;
            GlobalEvents.OnDealingComplete += OnCardsDealt;
            
            GlobalEvents.OnBidPlaced += OnBidPlaced;
            GlobalEvents.OnCardPlayed += OnCardPlayed;
        }
        
        private void OnDisable()
        {
            GlobalEvents.OnTurnChanged -= OnPlayerTurnChanged;
            GlobalEvents.OnDealingComplete -= OnCardsDealt;
            
            GlobalEvents.OnBidPlaced -= OnBidPlaced;
            GlobalEvents.OnCardPlayed -= OnCardPlayed;
        }
        
        private void OnCardsDealt()
        {
            AudioManager.Instance.PlaySound("Shuffle");
        }
        
        private void OnBidPlaced(int seat, int bidValue)
        {
            if (bidValue != 0) return;
            AudioManager.Instance.PlaySound("Pass");

            // Play sound for bid value
        }
        
        private void OnPlayerTurnChanged(int seat)
        {
            if (seat != SeatMapper.LocalPlayerSeat) return;
            AudioManager.Instance.PlaySound("PlayerTurn");
        }
        
        private void OnCardPlayed(PlayedCard card)
        {
            AudioManager.Instance.PlaySound("PlayCard");
        }
    }
}