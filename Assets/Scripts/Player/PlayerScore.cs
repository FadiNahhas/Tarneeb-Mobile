using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerScore : MonoBehaviour
    {
        [TabGroup("References"), SerializeField] private GameObject scoreObj;
        [TabGroup("References"), SerializeField] private Image scoreBackground;
        [TabGroup("References"), SerializeField] private TextMeshProUGUI scoreText;

        [TabGroup("Colors"), SerializeField] private Color defaultColor;
        [TabGroup("Colors"), SerializeField] private Color successColor;
        [TabGroup("Colors"), SerializeField] private Color failColor;

        private void OnDisable()
        {
            HideScore();
        }

        private void ShowScore()
        {
            scoreObj.SetActive(true);
        }
        
        private void HideScore()
        {
            scoreObj.SetActive(false);
        }
        
        public void SetScore(int score)
        {
            ShowScore();
            scoreText.text = score.ToString();
        }
        
        public void ResetScore()
        {
            scoreText.text = "0";
            scoreBackground.color = defaultColor;
        }

        public void SetSuccess(bool success)
        {
            scoreBackground.color = success ? successColor : failColor;
        }
    }
}
