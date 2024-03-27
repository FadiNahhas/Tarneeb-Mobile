using Events;

namespace State_Manager.States
{
    public class WaitingToStartState : GameStateBase
    {
        private readonly GameStateManager _gameStateManager;
        
        public WaitingToStartState(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        public override void OnEnter()
        {
            GlobalEvents.InvokeShowStartButton(true);
        }

        public override void OnExit()
        {
            _gameStateManager.NewGame();
            GlobalEvents.InvokeGameStarted();
            GlobalEvents.InvokeShowStartButton(false);
        }
    }
}