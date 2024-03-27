using Events;
using Managers;
using Player;
using SharedLibrary;
using UnityEngine;

namespace UI.Panels
{
    public class WinnersPanel : MonoBehaviour
    {
        [SerializeField] private PlayerDisplay playerPrefab;
        [SerializeField] private Transform layoutGroup;

        public void Init(Team winningTeam)
        {
            foreach (var seat in winningTeam.Seats)
            {
                var playerDisplay = Instantiate(playerPrefab, layoutGroup);
                playerDisplay.SetName(seat.Player.Name);
            }
        }

        public void NewGame()
        {
            GlobalEvents.InvokeNewGame();
        }

        public void ReturnToMenu()
        {
            GameManager.Instance.LeaveOfflineGame();
        }
    }
}
