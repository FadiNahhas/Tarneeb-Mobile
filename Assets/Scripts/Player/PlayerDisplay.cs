using System;
using Controllers;
using Events;
using Helpers;
using Helpers.Dependency_Injection;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerDisplay : MonoBehaviour
    {
        private const int NotSeated = -1;
        
        private const string SitText = "Sit";
        private const string CannotSitText = "";

        [TabGroup("Settings"), SerializeField, Range(0, 3)] private int id;
        [TabGroup("References") ,SerializeField] private TextMeshProUGUI playerName;
        [TabGroup("References") ,SerializeField] private Image playerImage;
        [TabGroup("References") ,SerializeField] private Image turnIndicator;
        [TabGroup("References") ,SerializeField] private Button button;
        
        [Inject] BaseCommandHandler _commandHandler;

        private void OnEnable()
        {
            GlobalEvents.OnLocalPlayerSeatChange += OnLocalPlayerSeatChange;
        }
        
        private void OnDisable()
        {
            GlobalEvents.OnLocalPlayerSeatChange -= OnLocalPlayerSeatChange;
        }

        public void SetName(string displayName)
        {
            //TODO: Add player profile image and assign it to playerImage
            playerName.text = displayName;
        }
        
        public void ToggleTurnIndicator(bool isTurn)
        {
            turnIndicator.enabled = isTurn;
        }

        private void OnLocalPlayerSeatChange(int seat)
        {
            SetButtonState(seat == NotSeated);
        }

        private void SetButtonState(bool canSit)
        {
            button.interactable = canSit;
            playerName.text = canSit ? SitText : CannotSitText;
        }

        public void OnPlayerSit() => _commandHandler.Sit(id);
    }
}
