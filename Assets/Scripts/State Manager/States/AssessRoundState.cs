using System.Linq;
using Events;
using SharedLibrary;

namespace State_Manager.States
{
    public class AssessRoundState : GameStateBase
    {
        private const int OppositeTeamScoreThreshold = 7;
        
        private readonly GameStateManager _gameStateManager;
        private Game Game => _gameStateManager.Game;
        
        public AssessRoundState(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }
        
        public override void OnEnter()
        {
            var round = Game.CurrentRound;
            var bidder = _gameStateManager.Seats[round.WinningBid.SeatId].Player;
            var biddingTeam = _gameStateManager.GetTeam(bidder);

            var biddingTeamTotalScore = biddingTeam.Seats.Sum(seat => seat.Player.Score);
            
            // If the bidding team made their bid, they get the points they scored. If they didn't, they lose the points they bid.
            if (biddingTeamTotalScore >= round.WinningBid.Value)
            {
                biddingTeam.AddPoints(biddingTeamTotalScore);
                GlobalEvents.InvokeRoundAssessed(_gameStateManager.Teams);
                
                return;
            }

            biddingTeam.AddPoints(-round.WinningBid.Value);
            
            // Check if the opposite team scored 7 or higher
            
            var oppositeTeam = _gameStateManager.GetOppositeTeam(bidder);
            
            var oppositeTeamTotalScore = oppositeTeam.Seats.Sum(seat => seat.Player.Score);
            
            if (oppositeTeamTotalScore >= OppositeTeamScoreThreshold)
            {
                oppositeTeam.AddPoints(oppositeTeamTotalScore);
            }
            
            GlobalEvents.InvokeRoundAssessed(_gameStateManager.Teams);
        }

        public override void OnExit()
        {
            // Reset player scores
            
            foreach (var seat in _gameStateManager.Seats)
            {
                seat.Player.ResetScore();
            }
        }
    }
}