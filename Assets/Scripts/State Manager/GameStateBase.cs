namespace State_Manager
{
    public abstract class GameStateBase : IGameState
    {
        public abstract void OnEnter();
        
        public abstract void OnExit();
    }
}