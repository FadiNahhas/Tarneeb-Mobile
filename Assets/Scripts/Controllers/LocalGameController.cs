using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AI;
using Data;
using Events;
using Helpers;
using Helpers.Dependency_Injection;
using Managers;
using SharedLibrary;
using Sirenix.OdinInspector;
using State_Manager;
using State_Manager.States;
using UnityEngine;

namespace Controllers
{
    public class LocalGameController : MonoBehaviour, IDependencyProvider
    {
        const int LocalPlayerSeat = 0;
        public const int NextPlayer = 1;
        private const int PlayAssessmentDelay = 1000;

        [TabGroup("References"), SerializeField, ReadOnly, Inject]
        private GameStateManager gameStateManager;
        
        [SerializeField] 
        private List<LocalPlayer> players = new();
        private Game Game => gameStateManager.Game;
        private Round Round => Game.CurrentRound;
        
        [Provide]
        public LocalGameController Provide() => this;
        
        /// <summary>
        /// Subscribes to the game events
        /// </summary>
        private void OnEnable()
        {
            GlobalEvents.OnNewGame += OnNewGame;
            GlobalEvents.OnDealingComplete += OnDealingComplete;
            GlobalEvents.OnBidPlaced += OnBidPlaced;
            GlobalEvents.OnTrumpChosen += OnTrumpChosen;
            GlobalEvents.OnCardPlayed += OnCardPlayed;
            GlobalEvents.OnPlayAssessed += OnPlayAssessed;
            GlobalEvents.OnRoundAssessed += OnRoundAssessed;
        }

        /// <summary>
        /// Unsubscribes from the game events
        /// </summary>
        private void OnDisable()
        {
            GlobalEvents.OnNewGame -= OnNewGame;
            GlobalEvents.OnDealingComplete -= OnDealingComplete;
            GlobalEvents.OnBidPlaced -= OnBidPlaced;
            GlobalEvents.OnTrumpChosen -= OnTrumpChosen;
            GlobalEvents.OnCardPlayed -= OnCardPlayed;
            GlobalEvents.OnPlayAssessed -= OnPlayAssessed;
            GlobalEvents.OnRoundAssessed -= OnRoundAssessed;
        }
        
        public void SetWinningScore(int score) => gameStateManager.SetWinningScore(score);
        
        /// <summary>
        /// Runs when the scene is loaded to assign the local player to a seat
        /// </summary>
        public void SitLocalPlayer(string displayName)
        {
            // Create a new local player and add it to the list of players
            var localPlayer = new LocalPlayer.PlayerBuilder(displayName)
                    .AsLocal()    
                    .Build();
            
            players.Add(localPlayer);
            
            GlobalEvents.InvokeLocalPlayerSeatChange(LocalPlayerSeat);
            
            gameStateManager.SitPlayer(localPlayer, LocalPlayerSeat);
        }
        
        /// <summary>
        /// Called when the start game button is clicked
        /// </summary>
        public void StartGame()
        {
            CreateAiPlayers();
            gameStateManager.ChangeState(new NewRoundState(gameStateManager));
        }
        
        /// <summary>
        /// Called when the leave game button is clicked
        /// </summary>
        public void Leave()
        {
            GameManager.Instance.LeaveOfflineGame();
        }

        #region AI Players Methods

        /// <summary>
        /// Sets up AI players for the game
        /// </summary>
        private void CreateAiPlayers()
        {
            var names = AINameGenerator.GetNameList();
            for (var i = players.Count; i < Game.PlayerCount; i++)
            {
                // Create a new AI player and add it to the list of players
                var player = new LocalPlayer.PlayerBuilder(names[i])
                    .AsAi()
                    .Build();
                players.Add(player);
                gameStateManager.SitPlayer(player, i);
            }
        }

        /// <summary>
        /// Deletes AI players from the game at the end of the game
        /// </summary>
        private void RemoveAiPlayers()
        {
            for (int i = players.Count - 1; i >= 0; i--)
            {
                if (players[i].IsAi)
                {
                    players.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when the play again button is clicked
        /// </summary>
        private void OnNewGame()
        {
            RemoveAiPlayers();
            gameStateManager.ResetGame();
            gameStateManager.ChangeState(new WaitingToStartState(gameStateManager));
        }
        
        private void OnDealingComplete()
        {
            gameStateManager.ChangeState(new GetBidState(gameStateManager));
        }
        
        private void OnBidPlaced(int seat, int value)
        {
            // If all players have placed a bid, move to the next state
            if (Game.CurrentRound.Bids.Count == Game.PlayerCount)
            {
                gameStateManager.ChangeState(new GetTrumpState(gameStateManager));
                return;
            }
            
            gameStateManager.ChangeState(new GetBidState(gameStateManager));
        }
        
        private void OnTrumpChosen(int seat, Suit suit)
        {
            gameStateManager.NewPlay();
            gameStateManager.ChangeState(new GetCardState(gameStateManager));
        }

        private void OnCardPlayed(PlayedCard playedCard)
        {
            // If all players have played a card, assess the play
            if (Game.CurrentRound.CurrentPlay.PlayedCardCount != Game.PlayerCount)
            {
                gameStateManager.ChangeState(new GetCardState(gameStateManager));
                return;
            }
            
            gameStateManager.ChangeState(new AssessPlayState(gameStateManager));
        }
        
        private async void OnPlayAssessed(SharedLibrary.Player winner)
        {
            // Delay to allow players to see the winning card
            await Task.Delay(PlayAssessmentDelay);
            
            // If the round is over, assess the round
            if (Round.Plays.Count == Round.CardsPerPlayer)
            {
                // Assess round
                gameStateManager.ChangeState(new AssessRoundState(gameStateManager));
                return;
            }
         
            // If round is not over, start a new play
            gameStateManager.NewPlay();
            gameStateManager.ChangeState(new GetCardState(gameStateManager));
        }
        
        private void OnRoundAssessed(Team[] teams)
        {
            // If a team has reached the winning score, end the game, otherwise start a new round
            if (teams.Any(t => t.Score >= Game.WinningScore))
            {
                gameStateManager.ChangeState(new GameOverState(gameStateManager));
                return;
            }
            
            gameStateManager.ChangeState(new NewRoundState(gameStateManager));
        }

        #endregion

    }
}