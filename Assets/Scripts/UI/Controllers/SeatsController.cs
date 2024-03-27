using Events;
using Helpers.Dependency_Injection;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Controllers
{
    public class SeatsController : MonoBehaviour, IDependencyProvider
    {
        private const string EmptySeatText = "";
        
        [TabGroup("UI Elements"), SerializeField]  
        private PlayerDisplay localPlayerSeat;
        
        [TabGroup("UI Elements"), SerializeField]  
        private PlayerDisplay player1Seat;
        
        [TabGroup("UI Elements"), SerializeField] 
        private PlayerDisplay[] seats = new PlayerDisplay[4];
        
        [Provide]
        public SeatsController Provide() => this;
        
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
                localPlayerSeat.gameObject.SetActive(false);
                player1Seat.gameObject.SetActive(true);
                seats[0] = player1Seat;
                return;
            }
            
            // If the local player is seated show the local player seat and hide the bottom seat
            localPlayerSeat.gameObject.SetActive(true);
            player1Seat.gameObject.SetActive(false);
            seats[0] = localPlayerSeat;
        }
        
        public void SitPlayer(int seat, string displayName)
        {
            seats[seat].SetName(displayName);
        }
        
        public void StandPlayer(int seat)
        {
            seats[seat].SetName(EmptySeatText);
        }
        
        public void ToggleTurnIndicator(int seatId)
        {
            for (var index = 0; index < seats.Length; index++)
            {
                seats[index].ToggleTurnIndicator(seatId == index);
            }

        }
    }
}