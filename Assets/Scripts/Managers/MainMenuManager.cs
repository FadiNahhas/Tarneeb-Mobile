using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Class that manages the UI for the main menu and game list that inherits from ServerMessageHandler
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [TabGroup("References")]
        [SerializeField] private Transform titleReference;
        [SerializeField] private Transform menuScreenReference;
        
        protected void Start()
        {
            // Show main menu on start
            ShowMainMenu();
        }
        
        #region Main Menu Button Methods

        /// <summary>
        /// Method that starts an offline game
        /// </summary>
        public void PlayOfflineGame()
        {
            GameManager.Instance.StartOfflineGame();
        }
        
        /// <summary>
        /// Called by quit button to quit the game
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        
        #endregion
        
        #region Visual UI Methods

        /// <summary>
        /// Enables and disables UI elements to show the main menu
        /// </summary>
        private void ShowMainMenu()
        {
            titleReference.gameObject.SetActive(true);
            menuScreenReference.gameObject.SetActive(true);
        }
        
        #endregion
    }
}
