using Events;
using Helpers.Dependency_Injection;
using Player;
using SharedLibrary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Controllers
{
    public class OrdersController : MonoBehaviour, IDependencyProvider
    {
        [TabGroup("UI Elements"), SerializeField] 
        private PlayerOrder localPlayerOrder;
        
        [TabGroup("UI Elements"), SerializeField] 
        private PlayerOrder player1Order;
        
        [TabGroup("UI Elements"), SerializeField] 
        private PlayerOrder[] playerOrders = new PlayerOrder[4];
        
        [Provide]
        public OrdersController Provide() => this;
        
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
                playerOrders[0] = player1Order;
                return;
            }
            
            // If the local player is seated show the local player seat and hide the bottom seat
            playerOrders[0] = localPlayerOrder;
        }
        
        public void SetValue(int seat, int value)
        {
            playerOrders[seat].SetValue(value);
        }

        public void SetSuit(int seat, Suit suit)
        {
            playerOrders[seat].SetSuit(suit);

            for (int i = 0; i < playerOrders.Length; i++)
            {
                if (seat == i) continue;
                playerOrders[i].ClearOrder();
            }
        }
        
        public void ResetOrders()
        {
            foreach (var playerOrder in playerOrders)
            {
                playerOrder.ClearOrder();
            }
        }
        
    }
}