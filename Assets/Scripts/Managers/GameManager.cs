using System;
using System.Collections;
using Audio;
using Controllers;
using Helpers;
using Sirenix.OdinInspector;
using UI.Panels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    /// <summary>
    /// Main game manager class that handles the game state and server communication.<br/>
    /// Inherits from <see cref="PersistentSingleton{T}"/> to persist across scenes.
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        [field: SerializeField, TabGroup("Configuration"), ReadOnly] public string DisplayName { get; private set; } = String.Empty; // Stores the player's display name

        [SerializeField, TabGroup("Configuration"), ReadOnly] private int chosenWinningScore = 31;

        /// <summary>
        /// Register handlers when the game starts
        /// </summary>
        private void Start()
        {
            DisplayName = PlayerPrefs.GetString("DisplayName", String.Empty);
            if (Application.platform == RuntimePlatform.Android)
            {
                // Set the target frame rate to 60 on Android
                Application.targetFrameRate = 60;
            }
        }
        
        #region Coroutines
        /// <summary>
        /// Coroutine to join a room and display game status
        /// </summary>
        private IEnumerator JoinOfflineGame()
        {
            if (DisplayName == String.Empty)
            {
                yield return GetDisplayName();
            }

            yield return GetWinningScore();
            
            var asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex: 1);
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
            var gameController = FindObjectOfType<LocalGameController>();
            AudioManager.Instance.PlaySound("Join");
            
            gameController.SetWinningScore(chosenWinningScore);
            gameController.SitLocalPlayer(DisplayName);
        }

        /// <summary>
        /// Coroutine to get the display name from the user
        /// </summary>
        private IEnumerator GetDisplayName()
        {
            if (DisplayNamePanel.IsActive) yield break;
            DisplayNamePanel.GetDisplayName();
            
            while (DisplayNamePanel.Result == string.Empty)
            {
                yield return null;
            }
            
            SetDisplayName(DisplayNamePanel.Result);
            
            PlayerPrefs.SetString("DisplayName", DisplayName);
        }
        
        /// <summary>
        /// Displays the panel for choosing the winning score and waits for the result
        /// </summary>
        private IEnumerator GetWinningScore()
        {
            if (WinningScorePanel.IsActive) yield break;
            
            WinningScorePanel.GetWinningScore();
            
            while (!WinningScorePanel.Completed)
            {
                yield return null;
            }
            
            chosenWinningScore = WinningScorePanel.Result;
        }
        
        #endregion

        /// <summary>
        /// Method to set the player's display name
        /// </summary>
        private void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }
        
        /// <summary>
        /// Method to run the coroutine to start an offline game
        /// </summary>
        public void StartOfflineGame()
        {
            StartCoroutine(JoinOfflineGame());
        }
        
        /// <summary>
        /// Method to leave the offline game and return to the main menu
        /// </summary>
        public void LeaveOfflineGame()
        {
            SceneManager.LoadScene(0);
        }
    }
}