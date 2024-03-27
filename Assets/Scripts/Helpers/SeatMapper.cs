using Events;
using Helpers.Dependency_Injection;
using UnityEngine;

namespace Helpers
{
    public class SeatMapper : MonoBehaviour, IDependencyProvider
    {
        private const int PlayerCount = 4;
        private const int NotSeated = -1;
        private const int BottomSeat = 0;
        
        public static int LocalPlayerSeat { get; private set; }
        public int MappedLocalPlayerSeat => Map(LocalPlayerSeat);

        [Provide]
        public SeatMapper Provide() => this;
        
        private void OnEnable()
        {
            GlobalEvents.OnLocalPlayerSeatChange += SetLocalPlayerSeat;
        }
        
        private void OnDisable()
        {
            GlobalEvents.OnLocalPlayerSeatChange -= SetLocalPlayerSeat;
        }

        private void SetLocalPlayerSeat(int newSeat)
        {
            LocalPlayerSeat = newSeat;
        }

        public int Map(int seat)
        {
            if (LocalPlayerSeat is NotSeated or BottomSeat) return seat;
            
            var mappedSeat = seat - LocalPlayerSeat + PlayerCount;
            mappedSeat %= PlayerCount;

            return mappedSeat;
        }
    }
}
