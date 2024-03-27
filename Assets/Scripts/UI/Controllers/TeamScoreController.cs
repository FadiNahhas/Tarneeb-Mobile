using Helpers.Dependency_Injection;
using TMPro;
using UnityEngine;

namespace UI.Controllers
{
    public class TeamScoreController : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private TextMeshProUGUI[] scoreTexts;

        [Provide]
        public TeamScoreController Provide() => this;
        
        public void SetScore(int seatId, int score)
        {
            scoreTexts[seatId].text = score.ToString();
        }
    
        public void ResetScores()
        {
            foreach (var scoreText in scoreTexts)
            {
                scoreText.text = "0";
            }
        }

        public void MoveScoresAround()
        {
            var team1Score = scoreTexts[0].text;
            var team2Score = scoreTexts[1].text;

            for (int i = 0; i < scoreTexts.Length; i++)
            {
                scoreTexts[i].text = i % 2 == 0 ? team2Score : team1Score;
            }
        }
    
    }
}
