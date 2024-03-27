using Helpers.Dependency_Injection;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [Inject] private BaseCommandHandler _commandHandler;

        private void Awake()
        {
            _commandHandler = FindObjectOfType<BaseCommandHandler>();
        }

        public void StartGame() => _commandHandler.StartGame();
    }
}