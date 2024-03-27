using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using State_Manager;
using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
    public class CardViewer : OdinEditorWindow
    {
        [MenuItem("Tools/Card Viewer")]
        public static void OpenWindow()
        {
            GetWindow<CardViewer>().Show();
        }

        private int _selectedPlayer;
        
        [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
        private void Player1()
        {
            _selectedPlayer = 0;
        }
        
        [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
        private void Player2()
        {
            _selectedPlayer = 1;
        }
        
        [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
        private void Player3()
        {
            _selectedPlayer = 2;
        }
        
        [HorizontalGroup("Buttons"), Button(ButtonSizes.Large)]
        private void Player4()
        {
            _selectedPlayer = 3;
        }
        
        [OnInspectorGUI]
        private void DisplayHand()
        {
            var gameStateManager = FindObjectOfType<GameStateManager>();
            if (gameStateManager == null) return;

            var players = gameStateManager.Seats.Select(seat => seat.Player).ToList();
            if (players.Count <= _selectedPlayer) return;
            
            var player = players[_selectedPlayer];
            if (player == null) return;
            
            GUILayout.Label("Player's Hand:");
            foreach (var card in player.Hand)
            {
                // Using GUILayout to draw the card string
                GUILayout.Label(card.ToString());
            }
        }
    }
}