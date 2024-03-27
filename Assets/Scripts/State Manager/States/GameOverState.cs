using Events;
using SharedLibrary;

namespace State_Manager.States
{
    public class GameOverState : GameStateBase
    {
        private readonly GameStateManager _gameStateManager;
        private Game Game => _gameStateManager.Game;
        
        public GameOverState(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }
        
        public override void OnEnter()
        {
            foreach (var team in _gameStateManager.Teams)
            {
                if (team.Score >= Game.WinningScore)
                {
                    GlobalEvents.InvokeGameOver(team);
                    return;
                }
            }
        }

        public override void OnExit() { }
    }
}