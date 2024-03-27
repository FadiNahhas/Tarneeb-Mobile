using System.Threading.Tasks;
using Events;
using Helpers;
using Helpers.Dependency_Injection;
using SharedLibrary;
using Sirenix.OdinInspector;
using UI.Panels;
using UnityEngine;
using Views;

namespace UI.Controllers
{
    public class GameViewController : MonoBehaviour, IDependencyProvider
    {
        private const int ClearTableDelay = 1000;
        private const int None = -1;
        
        [TabGroup("UI Elements"), SerializeField] 
        private GameObject startGameButton;

        private Transform _canvas;
        
        [Inject, TabGroup("References"), SerializeField, ReadOnly] 
        private SeatMapper seatMapper;
        
        [Inject, TabGroup("References"), SerializeField, ReadOnly] 
        private SeatsController seatsController;
        
        [Inject, TabGroup("References"), SerializeField, ReadOnly] 
        private ScoresController scoresController;
        
        [Inject, TabGroup("References"), SerializeField, ReadOnly] 
        private PlayerHandsController playerHandsController;
        
        [Inject, TabGroup("References"), SerializeField, ReadOnly] 
        private OrdersController ordersController;

        [Inject, TabGroup("References"), SerializeField, ReadOnly]
        private PlayedCardsView playedCards;
        
        [Inject, TabGroup("References"), SerializeField, ReadOnly]
        private PreviousPlayController previousPlay;

        [TabGroup("Prefabs"), SerializeField] private BidPanel bidPanelPrefab;
        [TabGroup("Prefabs"), SerializeField] private GameObject trumpPanelPrefab;
        [TabGroup("Prefabs"), SerializeField] private WinnersPanel winnersPanelPrefab;
        
        private BidPanel _bidPanelInstance;
        private GameObject _trumpPanelInstance;
        private WinnersPanel _winnersPanelInstance;

        private void Awake()
        {
            _canvas = FindObjectOfType<Canvas>().transform;
        }

        private void OnEnable()
        {
            // Game Start/End Events
            GlobalEvents.OnShowStartButton += OnShowStartButton;
            GlobalEvents.OnGameStarted += OnGameStarted;
            GlobalEvents.OnPlayAssessed += OnPlayAssessed;
            GlobalEvents.OnRoundAssessed += OnRoundAssessed;
            GlobalEvents.OnGameOver += OnGameOver;
            GlobalEvents.OnNewGame += OnNewGame;
            
            // Card Events
            GlobalEvents.OnCardDealt += GiveCard;
            GlobalEvents.OnGetCardFromLocalPlayer += GetCardFromLocalPlayer;
            GlobalEvents.OnCardPlayed += OnCardPlayed;
            
            // Seating Events
            GlobalEvents.OnPlayerSit += SitPlayer;
            GlobalEvents.OnPlayerStand += StandPlayer;
            
            // Bid Events
            GlobalEvents.OnGetBidFromLocalPlayer += DisplayBidPanel;
            GlobalEvents.OnLocalPlayerBid += HideBidPanel;
            GlobalEvents.OnBidPlaced += ShowBidValue;
            GlobalEvents.OnGetTrumpFromLocalPlayer += DisplayTrumpPanel;
            GlobalEvents.OnLocalPlayerChoseTrump += HideTrumpPanel;
            GlobalEvents.OnTrumpChosen += ShowBidSuit;
            GlobalEvents.OnSuccessDetermined += OnSuccessDetermined;
            
            // Turn Events
            GlobalEvents.OnTurnChanged += OnTurnChanged;
        }
        
        private void OnDisable()
        {
            // Game Start/End Events
            GlobalEvents.OnGameStarted -= OnGameStarted;
            GlobalEvents.OnPlayAssessed -= OnPlayAssessed;
            GlobalEvents.OnRoundAssessed -= OnRoundAssessed;
            GlobalEvents.OnGameOver -= OnGameOver;
            
            // Card Events
            GlobalEvents.OnCardDealt -= GiveCard;
            GlobalEvents.OnGetCardFromLocalPlayer -= GetCardFromLocalPlayer;
            GlobalEvents.OnCardPlayed -= OnCardPlayed;
            
            // Seating Events
            GlobalEvents.OnPlayerSit -= SitPlayer;
            GlobalEvents.OnPlayerStand -= StandPlayer;
            
            // Bid Events
            GlobalEvents.OnGetBidFromLocalPlayer -= DisplayBidPanel;
            GlobalEvents.OnLocalPlayerBid -= HideBidPanel;
            GlobalEvents.OnBidPlaced -= ShowBidValue;
            GlobalEvents.OnGetTrumpFromLocalPlayer -= DisplayTrumpPanel;
            GlobalEvents.OnLocalPlayerChoseTrump -= HideTrumpPanel;
            GlobalEvents.OnTrumpChosen -= ShowBidSuit;
            GlobalEvents.OnSuccessDetermined -= OnSuccessDetermined;
            
            // Turn Events
            GlobalEvents.OnTurnChanged -= OnTurnChanged;
        }

        [Provide] public GameViewController Provide() => this;

        #region Seating Methods

        /// <summary>
        /// Displays the player's name on the seat they are sitting in.
        /// </summary>
        /// <param name="localPlayer">Sitting player reference</param>
        private void SitPlayer(SharedLibrary.Player localPlayer)
        {
            var mappedSeat = seatMapper.Map(localPlayer.Seat);
            seatsController.SitPlayer(mappedSeat, localPlayer.Name);
        }
        
        private void StandPlayer(int seat)
        {
            var mappedSeat = seatMapper.Map(seat);
            seatsController.StandPlayer(mappedSeat);
        }

        #endregion

        #region Card Methods

        private void GiveCard(int playerSeat, Card card)
        {
            var mappedSeat = seatMapper.Map(playerSeat);
            playerHandsController.AddCard(mappedSeat, card, playerSeat != SeatMapper.LocalPlayerSeat);
        }
        
        private void GetCardFromLocalPlayer(Suit? suit)
        {
            playerHandsController.HighlightCards(seatMapper.MappedLocalPlayerSeat, suit);
        }

        private void OnCardPlayed(PlayedCard playedCard)
        {
            var mappedSeat = seatMapper.Map(playedCard.Seat);
            playedCards.SetCard(mappedSeat, playedCard.Card);
            playerHandsController.RemoveCard(mappedSeat, playedCard.Card);
        }

        #endregion

        #region Bid Methods

        private void DisplayBidPanel(int value, bool canMatch)
        {
            if (!_bidPanelInstance)
                _bidPanelInstance = Instantiate(bidPanelPrefab, _canvas);
            else
                _bidPanelInstance.gameObject.SetActive(true);
            
            _bidPanelInstance.Setup(value, canMatch);
        }

        private void HideBidPanel(int value)
        {
            _bidPanelInstance.gameObject.SetActive(false);
        }

        private void DisplayTrumpPanel()
        {
            if (!_trumpPanelInstance)
                _trumpPanelInstance = Instantiate(trumpPanelPrefab, _canvas);
            else
                _trumpPanelInstance.gameObject.SetActive(true);
        }
        
        private void HideTrumpPanel(Suit suit)
        {
            if (!_trumpPanelInstance)
            {
                Debug.LogWarning("Trump panel instance is null");
                return;
            }
            
            _trumpPanelInstance.gameObject.SetActive(false);
        }
        
        private void ShowBidValue(int seat, int value)
        {
            var mappedSeat = seatMapper.Map(seat);
            ordersController.SetValue(mappedSeat, value);
        }
        
        private void ShowBidSuit(int seat, Suit suit)
        {
            var mappedSeat = seatMapper.Map(seat);
            ordersController.SetSuit(mappedSeat, suit);
        }
        
        private void OnSuccessDetermined(int seat, bool success)
        {
            var mappedSeat = seatMapper.Map(seat);
            var mappedTeammateSeat = seatMapper.Map((seat + 2) % Game.PlayerCount);
            scoresController.SetSuccess(mappedSeat, success);
            scoresController.SetSuccess(mappedTeammateSeat, success);
        }

        #endregion

        #region Game Flow Methods

        private void OnNewGame()
        {
            if (_winnersPanelInstance)
            {
                _winnersPanelInstance.gameObject.SetActive(false);
            }
            previousPlay.ClearDisplay();
            
            scoresController.ResetTeamScores();
        }
        
        /// <summary>
        /// Hides start button and enables the player score UI.
        /// </summary>
        private void OnGameStarted()
        {
            for (var i = 0; i < Game.PlayerCount; i++) scoresController.SetScore(i, 0);
        }
        
        /// <summary>
        /// Toggles the turn indicator on for the current player, and off for the others.
        /// If it is not the local player's turn, unhighlight cards. 
        /// </summary>
        /// <param name="seat">The seat of the player whose turn it is.</param>
        private void OnTurnChanged(int seat)
        {
            var mappedSeat = seatMapper.Map(seat);
            seatsController.ToggleTurnIndicator(mappedSeat);
            
            if (seat == SeatMapper.LocalPlayerSeat) return;
            playerHandsController.HighlightCards(seatMapper.MappedLocalPlayerSeat, null);
        }
        private async void OnPlayAssessed(SharedLibrary.Player localPlayer)
        {
            var mappedSeat = seatMapper.Map(localPlayer.Seat);
            scoresController.SetScore(mappedSeat, localPlayer.Score);

            await Task.Delay(ClearTableDelay);
            
            var cards = playedCards.GetCards();
            playedCards.ClearCards();
            
            previousPlay.DisplayPlay(cards);
        }

        private void OnRoundAssessed(Team[] teams)
        {
            foreach (var team in teams)
            {
                foreach (var seat in team.Seats)
                {
                    var mappedSeat = seatMapper.Map(seat.Id);
                    scoresController.UpdateTeamScores(mappedSeat, team.Score);
                }
            }

            ResetUI();
        }

        private void OnShowStartButton(bool value)
        {
            startGameButton.SetActive(value);
        }

        private void ResetUI()
        {
            ordersController.ResetOrders();
            scoresController.ResetScores();
            previousPlay.ClearDisplay();
            playedCards.ClearCards();
            
            // Hide turn indicators
            for (var i = 0; i < Game.PlayerCount; i++)
            {
                seatsController.ToggleTurnIndicator(None);
            }
        }

        private void OnGameOver(Team winners)
        {
            if (!_winnersPanelInstance)
            {
                _winnersPanelInstance = Instantiate(winnersPanelPrefab, _canvas);
            }
            
            _winnersPanelInstance.gameObject.SetActive(true);
            _winnersPanelInstance.Init(winners);
        }
        
        #endregion
        
    }
}
