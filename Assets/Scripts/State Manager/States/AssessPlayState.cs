using Events;
using SharedLibrary;

namespace State_Manager.States
{
    public class AssessPlayState : GameStateBase
    {
        private readonly GameStateManager _gameStateManager;
        private Game Game => _gameStateManager.Game;
        private int _winnerSeat;
        
        public AssessPlayState(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }
        
        public override void OnEnter()
        {
            var play = Game.CurrentRound.CurrentPlay;
            _winnerSeat = play.AssessPlay();
            
            var winner = _gameStateManager.Seats[_winnerSeat].Player;
            winner.AddPoint();
            
            DetermineBidSuccess();
            GlobalEvents.InvokePlayAssessed(winner);
        }

        public override void OnExit() {}
        private void DetermineBidSuccess()
        {
            var bidder = _gameStateManager.GetPlayerInSeat(Game.CurrentRound.WinningBid.SeatId);
            var bidderTeammate = _gameStateManager.GetTeamMate(bidder);
            
            if (bidder.Score + bidderTeammate.Score >= Game.CurrentRound.WinningBid.Value)
            {
                GlobalEvents.InvokeSuccessDetermined(bidder.Seat, true);
            } 
            else if (bidder.Score + bidderTeammate.Score + Game.CurrentRound.PlaysRemaining < Game.CurrentRound.WinningBid.Value)
            {
                GlobalEvents.InvokeSuccessDetermined(bidder.Seat, false);
            }
        }
    }
}