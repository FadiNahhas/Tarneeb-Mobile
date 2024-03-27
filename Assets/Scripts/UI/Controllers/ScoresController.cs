using Events;
using Helpers.Dependency_Injection;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Controllers
{
    public class ScoresController : MonoBehaviour, IDependencyProvider
    {
        [TabGroup("References"), Inject, SerializeField, ReadOnly]
        private TeamScoreController teamScoreController;
        
        [TabGroup("UI Elements"), SerializeField] 
        private PlayerScore localPlayerScore;
        
        [TabGroup("UI Elements"), SerializeField] 
        private PlayerScore player1Score;
        
        [TabGroup("UI Elements"), SerializeField] 
        private PlayerScore[] playerScores = new PlayerScore[4];
        
        [Provide]
        public ScoresController Provide() => this;
        
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
                playerScores[0] = player1Score;
                return;
            }
            
            // If the local player is seated show the local player seat and hide the bottom seat
            playerScores[0] = localPlayerScore;
        }

        #region Player Score Methods

        public void ResetScores()
        {
            foreach (var playerScore in playerScores)
            {
                playerScore.ResetScore();
            }
        }
        
        public void SetScore(int seat, int score)
        {
            playerScores[seat].SetScore(score);
        }

        public void SetSuccess(int seat, bool success)
        {
            playerScores[seat].SetSuccess(success);
        }
        #endregion

        #region Team Score Methods

        public void UpdateTeamScores(int seat, int score)
        {
            teamScoreController.SetScore(seat, score);
        }
        
        public void ResetTeamScores()
        {
            teamScoreController.ResetScores();
        }

        #endregion
        
    }
}