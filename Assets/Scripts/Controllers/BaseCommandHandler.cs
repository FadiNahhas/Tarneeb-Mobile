using Helpers.Dependency_Injection;
using UnityEngine;

namespace Controllers
{
    public abstract class BaseCommandHandler : MonoBehaviour, IDependencyProvider
    {
        public abstract void StartGame();
        
        public abstract void Sit(int seat);
        public abstract void Leave();
        
        public abstract void Stand();
        
        [Provide]
        public BaseCommandHandler Provide() => this;
    }
}