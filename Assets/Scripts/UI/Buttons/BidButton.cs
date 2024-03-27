using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class BidButton : MonoBehaviour
    {
        const int MinValue = 0;
        const int MaxValue = 13;
        
        [SerializeField] private int value;
        public int Value => value;
    
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        public void OnClick() => GlobalEvents.InvokeLocalPlayerBid(value);

        public void ToggleInteractable(bool interactable)
        {
            if (button == null)
            {
                Debug.LogWarning($"Order button with value {value} does not have a button component.");
                return;
            }
            
            button.interactable = interactable;
        }
        
        private void OnValidate()
        {
            value = Mathf.Clamp(value, MinValue, MaxValue);
            if (value is > 0 and < 7)
            {
                Debug.LogWarning($"Order button with value {value} is out of range. Value should be between 7 and 13 or 0.");
                return;
            }

            UpdateButtonText();
        }
        
        private void UpdateButtonText()
        {
            if (buttonText == null)
            {
                Debug.LogWarning($"Order button with value {value} does not have a button text component.");
                return;
            }

            buttonText.text = value == 0 ? "Pass" : value.ToString();
        }
    }
}
