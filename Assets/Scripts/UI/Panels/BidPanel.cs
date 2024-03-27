using System.Collections.Generic;
using UI.Buttons;
using UnityEngine;

namespace UI.Panels
{
    public class BidPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<BidButton> orderButtons;
        private BidButton PassButton => GetButtonWithValue(0);
        
        public void Setup(int currentHighestOrderValue, bool isDealer)
        {
            switch (currentHighestOrderValue)
            {
                case 0: // If no one has ordered
                {
                    foreach (var button in orderButtons)
                    {
                        // If the button is the pass button
                        if (button.Value == 0)
                        {
                            // Disable the pass button if the player is the dealer (last player to order), otherwise enable it
                            button.ToggleInteractable(!isDealer);
                            continue;
                        }
                    
                        // Enable all other buttons
                        button.ToggleInteractable(true);
                    }

                    break;
                }
                case > 0: // If someone has already ordered
                {
                    foreach (var button in orderButtons)
                    {
                        // If the button is the pass button
                        if (button.Value == 0)
                        {
                            // Enable the pass button
                            button.ToggleInteractable(true);
                            continue;
                        }
                        
                        // If the button's value is less than the current highest order value
                        if (button.Value < currentHighestOrderValue)
                        {
                            // Disable the button
                            button.ToggleInteractable(false);
                            continue;
                        }
                        
                        // If the button's value is equal to the current highest order value
                        if (button.Value == currentHighestOrderValue)
                        {
                            // Enable the button if the player is the dealer (last player to order), otherwise disable it
                            button.ToggleInteractable(isDealer);
                            continue;
                        }
                        
                        // If the button's value is greater than the current highest order value
                        if (button.Value > currentHighestOrderValue)
                        {
                            // Enable the button
                            button.ToggleInteractable(true);
                        }
                    }

                    break;
                }
            }
        }
        
        #region Helpers

        private BidButton GetButtonWithValue(int value)
        {
            if (orderButtons == null || orderButtons.Count == 0)
            {
                Debug.LogWarning("Order buttons list is empty.");
                return null;
            }
            
            if (value is < 0 or > 13 or > 0 and < 7)
            {
                Debug.LogWarning($"Value {value} is out of range. Value should be between 0 and 13.");
                return null;
            }
            
            return orderButtons.Find(button => button.Value == value);
        }
        
        [ContextMenu("Fetch Buttons")]
        private void FetchButtons()
        {
            orderButtons = new List<BidButton>(GetComponentsInChildren<BidButton>());
        }
        
        #endregion
    }
}
