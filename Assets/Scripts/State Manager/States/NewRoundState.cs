using System.Threading.Tasks;
using Events;
using SharedLibrary;

namespace State_Manager.States
{
    public class NewRoundState : GameStateBase
    {
        private const int NextPlayer = 1;
        private const int CardDealDelay = 100;
        
        private readonly GameStateManager _gameStateManager;
        private Game Game => _gameStateManager.Game;
        
        public NewRoundState(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }
        
        public override void OnEnter()
        {
            StartRound();
        }

        public override void OnExit() { }

        private async void StartRound()
        {
            var round = _gameStateManager.NewRound();
            
            int seatToDeal = (Game.DealerSeat + NextPlayer) % Game.PlayerCount;

            while (round.Deck.CardsRemaining() != 0)
            {
                var card = round.Deck.DrawCard();
                _gameStateManager.Seats[seatToDeal].Player.AddCard(card);
                GlobalEvents.InvokeCardDealt(seatToDeal, card);
                await Task.Delay(CardDealDelay);
                seatToDeal = (seatToDeal + NextPlayer) % Game.PlayerCount;
            }

            GlobalEvents.InvokeDealingComplete();
        }
    }
}