using DG.Tweening;
using Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    public class InteractableCard : BaseCard, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private const float DefaultScale = 1f;

        [SerializeField] private GameObject dim;
        
        private bool _isHighlighted;
        
        private bool LegalPlay => _isHighlighted && !IsFaceDown;

        [Header("Hover Animations")]
        [SerializeField] private float hoverScale = 1.2f;

        [SerializeField] private float hoverAnimationTime = 0.25f;
        
        private void PlayCard() => GlobalEvents.InvokeLocalPlayerPlayedCard(Card);
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsFaceDown)
                return;
            transform.DOScale(hoverScale, hoverAnimationTime);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsFaceDown)
                return;
            transform.DOScale(DefaultScale, hoverAnimationTime);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!LegalPlay)
                return;
            PlayCard();
        }
        
        public void SetHighlight(bool value)
        {
            dim.SetActive(!value);
            _isHighlighted = value;
        }
    }
}
