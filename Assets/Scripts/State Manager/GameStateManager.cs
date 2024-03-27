using System.Linq;
using Events;
using Helpers;
using Helpers.Dependency_Injection;
using SharedLibrary;
using Sirenix.OdinInspector;
using State_Manager.States;
using UnityEngine;
using Random = UnityEngine.Random;

namespace State_Manager
{
    public class GameStateManager : MonoBehaviour, IDependencyProvider
    {
        [TabGroup("Settings"), SerializeField] private int winningScore = 31;
        
        public Game Game { get; private set; }

        public Seat[] Seats { get; private set; } = new Seat[Game.PlayerCount];
        
        public Team[] Teams { get; private set; } = new Team[2];
        
        private GameStateBase CurrentState { get; set; }
        
        [Provide] public GameStateManager Provide() => this;

        private void Start()
        {
            // Create new seats and setup teams
            CreateSeats();
            SetupTeams();
             
            // Change to default state
            ChangeState(new WaitingToStartState(this));
        }
        
        public void ChangeState(GameStateBase state)
        {
            if (CurrentState != null)
            {
                CurrentState.OnExit();
            }
            
            CurrentState = state;
            
            CurrentState.OnEnter();
        }

        #region Setup Methods

        /// <summary>
        /// Creates new empty seats for the game
        /// </summary>
        private void CreateSeats()
        {
            for (var i = 0; i < Game.PlayerCount; i++)
            {
                Seats[i] = new Seat(i);
            }
        }
        
        /// <summary>
        /// Sets up the teams for the game with opposing seats as team members
        /// </summary>
        private void SetupTeams()
        {
            Teams[0] = new Team(new[] {Seats[0], Seats[2]});
            Teams[1] = new Team(new[] {Seats[1], Seats[3]});
        }

        #endregion
        
        /// <summary>
        /// Called by the game manager to set the winning score for the game
        /// </summary>
        /// <param name="score">Winning score value</param>
        public void SetWinningScore(int score) => winningScore = score;

        #region Game Flow Methods

        public void NewGame() => Game = new Game(winningScore, Random.Range(0, Game.PlayerCount));
        
        public Round NewRound()
        {
            if (Game.Rounds.Count > 0)
            {
                Game.EndRound();
            }
 
            return Game.NewRound();
        }
        
        public void NewPlay()
        {
            Game.CurrentRound.NewPlay();
        }

        public void ResetGame()
        {
            ClearAIPlayers();
            SetupTeams();
        }
        
        #endregion
        
        /// <summary>
        /// Clears AI players at the end of the game
        /// </summary>
        private void ClearAIPlayers()
        {
            for (int i = 0; i < Game.PlayerCount; i++)
            {
                if (Seats[i].Player != null && Seats[i].Player.IsAi)
                {
                    Seats[i].Stand();
                }
            }
        }
        
        public void SitPlayer(SharedLibrary.Player player, int seatId)
        {
            Seats[seatId].Sit(player);
            GlobalEvents.InvokePlayerSit(player);
        }

        #region Helper Methods

        public SharedLibrary.Player GetPlayerInSeat(int seatId)
        {
            return Seats[seatId].Player;
        }
        
        public SharedLibrary.Player GetTeamMate(SharedLibrary.Player localPlayer)
        {
            return GetPlayerInSeat((localPlayer.Seat + 2) % Game.PlayerCount);
        }
        
        public Team GetTeam(SharedLibrary.Player localPlayer)
        {
            return Teams.FirstOrDefault(team => team.HasPlayer(localPlayer));
        }
        
        public Team GetOppositeTeam(SharedLibrary.Player localPlayer)
        {
            return Teams.FirstOrDefault(team => !team.HasPlayer(localPlayer));
        }

        #endregion
    }
}