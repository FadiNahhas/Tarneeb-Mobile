using System;
using AI;

namespace Data
{
    [Serializable]
    public class LocalPlayer : SharedLibrary.Player
    {
        public AIController AIController;

        public LocalPlayer(string name) : base(name) { }

        public class PlayerBuilder
        {
            private readonly LocalPlayer _localPlayer;

            public PlayerBuilder(string name)
            {
                _localPlayer = new LocalPlayer(name);
            }
            
            public PlayerBuilder AsAi()
            {
                _localPlayer.IsAi = true;
                _localPlayer.AIController = new AIController(_localPlayer);
                return this;
            }
            
            public PlayerBuilder AsLocal()
            {
                _localPlayer.IsLocal = true;
                return this;
            }
            
            public LocalPlayer Build()
            {
                return _localPlayer;
            }
        }
    }
}