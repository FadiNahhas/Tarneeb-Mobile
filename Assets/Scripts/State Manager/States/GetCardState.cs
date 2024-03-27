using Data;
using Events;
using SharedLibrary;

namespace State_Manager.States
{
    public class GetCardState : GameStateBase
    {
        private readonly GameStateManager _gameStateManager;
        private Game Game => _gameStateManager.Game;
        private bool FirstPlay => Game.CurrentRound.Plays.Count == 1;
        private int CardsPlayed => Game.CurrentRound.CurrentPlay.PlayedCardCount;
        private int _currentSeatTurn;
        
        public GetCardState(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }
        
        public override void OnEnter()
        {
            if (FirstPlay)
                _currentSeatTurn = (Game.CurrentRound.WinningBid.SeatId + CardsPlayed) % Game.PlayerCount;
            else
                _currentSeatTurn = (Game.CurrentRound.Plays[^2].WinnerSeatId + CardsPlayed) % Game.PlayerCount;
            
            GlobalEvents.InvokeTurnChanged(_currentSeatTurn);
            var player = _gameStateManager.Seats[_currentSeatTurn].Player as LocalPlayer;
            
            GetCard(player);
        }

        public override void OnExit() {}

        private async void GetCard(LocalPlayer localPlayer)
        {
            if (localPlayer.IsAi)
            {
                var card = await localPlayer.AIController.GetCard(Game);
                
                var playedCard = new PlayedCard(card, localPlayer.Seat);
                Game.CurrentRound.CurrentPlay.PlayCard(playedCard);
                localPlayer.RemoveCard(card);
                
                GlobalEvents.InvokeCardPlayed(playedCard);
                return;
            }

            if (!localPlayer.IsLocal) return;
            GlobalEvents.OnLocalPlayerPlayedCard += HandleLocalPlayerCardPlayed;
            GlobalEvents.InvokeGetCardFromLocalPlayer(Game.CurrentRound.CurrentPlay.LeadingSuit);
        }

        private void HandleLocalPlayerCardPlayed(Card card)
        {
            var playedCard = new PlayedCard(card, _currentSeatTurn);
            var player = _gameStateManager.Seats[_currentSeatTurn].Player;
            player.RemoveCard(card);
            
            Game.CurrentRound.CurrentPlay.PlayCard(playedCard);
            
            GlobalEvents.InvokeCardPlayed(playedCard);
            GlobalEvents.OnLocalPlayerPlayedCard -= HandleLocalPlayerCardPlayed;
        }
    }
}