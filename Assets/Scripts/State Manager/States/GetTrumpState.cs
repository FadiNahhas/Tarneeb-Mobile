using Data;
using Events;
using Helpers;
using SharedLibrary;

namespace State_Manager.States
{
    public class GetTrumpState : GameStateBase
    {
        private readonly GameStateManager _gameStateManager;
        private Game Game => _gameStateManager.Game;
        
        private int _currentSeatTurn;
        
        public GetTrumpState(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }
        
        public override void OnEnter()
        {
            _currentSeatTurn = Game.CurrentRound.WinningBid.SeatId;
            var highestBidder = _gameStateManager.Seats[_currentSeatTurn].Player as LocalPlayer;
            
            GlobalEvents.InvokeTurnChanged(_currentSeatTurn);
            
            GetTrump(highestBidder);
        }

        public override void OnExit() {}

        private async void GetTrump(LocalPlayer localPlayer)
        {
            if (localPlayer.IsAi)
            {
                var trump = await localPlayer.AIController.GetTrumpSuit();
                Game.CurrentRound.SetTrump(trump);
                GlobalEvents.InvokeTrumpChosen(localPlayer.Seat, trump);
                return;
            }

            if (!localPlayer.IsLocal) return;
            
            GlobalEvents.OnLocalPlayerChoseTrump += OnLocalPlayerChoseTrump;
            GlobalEvents.InvokeGetTrumpFromLocalPlayer();
        }
        
        private void OnLocalPlayerChoseTrump(Suit trump)
        {
            Game.CurrentRound.SetTrump(trump);
            GlobalEvents.InvokeTrumpChosen(_currentSeatTurn, trump);
            
            GlobalEvents.OnLocalPlayerChoseTrump -= OnLocalPlayerChoseTrump;
        }
    }
}