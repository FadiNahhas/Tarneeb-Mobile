namespace State_Manager
{
    public interface IGameState
    {
        public void OnEnter();
        
        public void OnExit();
    }
}