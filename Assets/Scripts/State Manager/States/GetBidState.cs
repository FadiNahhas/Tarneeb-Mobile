using Controllers;
using Data;
using Events;
using SharedLibrary;

namespace State_Manager.States
{
    class GetBidState : GameStateBase
    {
        private readonly GameStateManager _gameStateManager;
        private Game Game => _gameStateManager.Game;

        private int _currentSeatTurn;
        public GetBidState(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }
        
        public override void OnEnter()
        {
            _currentSeatTurn = Game.DealerSeat + LocalGameController.NextPlayer + Game.CurrentRound.Bids.Count;
            _currentSeatTurn %= Game.PlayerCount;
            
            var player = _gameStateManager.Seats[_currentSeatTurn].Player as LocalPlayer;
            
            GlobalEvents.InvokeTurnChanged(_currentSeatTurn);
            
            GetBid(player);
        }
        
        public override void OnExit() {}

        private async void GetBid(LocalPlayer localPlayer)
        {
            var canMatch = localPlayer.Seat == Game.DealerSeat;
            
            if (localPlayer.IsAi)
            {
                var bid = await localPlayer.AIController.GetBid(Round.MinBidValue, Game.CurrentRound.WinningBid.Value, canMatch);
                Game.CurrentRound.AddBid(bid);
                GlobalEvents.InvokeBidPlaced(localPlayer.Seat, bid.Value);
                return;
            }

            if (!localPlayer.IsLocal) return;
            GlobalEvents.OnLocalPlayerBid += OnLocalPlayerBid;
            GlobalEvents.InvokeGetBidFromLocalPlayer(Game.CurrentRound.WinningBid.Value, canMatch);
        }
        
        private void OnLocalPlayerBid(int bid)
        {
            Game.CurrentRound.AddBid(new Order(bid, _currentSeatTurn));
            
            GlobalEvents.InvokeBidPlaced(_currentSeatTurn, bid);
            GlobalEvents.OnLocalPlayerBid -= OnLocalPlayerBid;
        }


    }
}