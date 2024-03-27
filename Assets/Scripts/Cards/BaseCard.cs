using Constants;
using SharedLibrary;
using TMPro;
using UnityEngine;

namespace Cards
{
    public class BaseCard : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI smallSuitText;
        [SerializeField] private TextMeshProUGUI largeSuitText;
        [SerializeField] private GameObject backCover;
        
        public Card Card { get; private set; }
        
        protected bool IsFaceDown { get; private set; }
        
        public void Initialize(Card card, bool faceDown = false)
        {
            Card = card;
            IsFaceDown = faceDown;
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            if (IsFaceDown)
            {
                backCover.SetActive(true);
                return;
            }
            
            rankText.text = RankSuitFormatting.GetRankString(Card.Value);
            smallSuitText.text = RankSuitFormatting.GetSuitString(Card.Suit);
            largeSuitText.text = smallSuitText.text;
            
            SetColor();
        }

        private void SetColor()
        {
            var color = Colors.GetSuitColor(Card.Suit);
            
            rankText.color = color;
            smallSuitText.color = color;
            largeSuitText.color = color;
        }
        
        public void ToggleFaceDown(bool value)
        {
            backCover.SetActive(value);
            IsFaceDown = value;
        }
        
        public void DestroyCard() => Destroy(gameObject);
    }
}