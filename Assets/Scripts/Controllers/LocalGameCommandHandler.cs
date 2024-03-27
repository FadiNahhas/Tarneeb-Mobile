using Helpers.Dependency_Injection;

namespace Controllers
{
    public class LocalGameCommandHandler : BaseCommandHandler
    {
        [Inject] private LocalGameController _localGameController;
        
        public override void StartGame()
        {
            _localGameController.StartGame();
        }

        public override void Sit(int seat)
        {
            throw new System.NotImplementedException();
        }
        
        public override void Stand()
        {
            throw new System.NotImplementedException();
        }

        public override void Leave() => _localGameController.Leave();
    }
}