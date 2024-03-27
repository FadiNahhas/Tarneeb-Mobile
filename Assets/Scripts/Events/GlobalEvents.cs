using System;
using SharedLibrary;

namespace Events
{
    public static class GlobalEvents
    {
        #region Game Flow

        public static event Action<bool> OnShowStartButton;
        public static void InvokeShowStartButton(bool show) => OnShowStartButton?.Invoke(show);
        
        public static event Action OnGameStarted;
        public static void InvokeGameStarted() => OnGameStarted?.Invoke();
        
        public static event Action OnNewGame;
        public static void InvokeNewGame() => OnNewGame?.Invoke();

        public static event Action<int> OnTurnChanged;
        public static void InvokeTurnChanged(int seat) => OnTurnChanged?.Invoke(seat);

        #endregion

        #region Seating

        public static event Action<int> OnLocalPlayerSeatChange;
        public static void InvokeLocalPlayerSeatChange(int seat) => OnLocalPlayerSeatChange?.Invoke(seat);
        
        public static event Action<SharedLibrary.Player> OnPlayerSit;
        public static void InvokePlayerSit(SharedLibrary.Player localPlayer) => OnPlayerSit?.Invoke(localPlayer);
        
        public static event Action<int> OnPlayerStand;
        public static void InvokePlayerStand(int seat) => OnPlayerStand?.Invoke(seat);

        #endregion

        #region Dealing

        public static event Action<int, Card> OnCardDealt;
        public static void InvokeCardDealt(int seat, Card card) => OnCardDealt?.Invoke(seat, card);

        public static event Action OnDealingComplete;
        public static void InvokeDealingComplete() => OnDealingComplete?.Invoke();

        #endregion

        #region Bidding

        public static event Action<int, int> OnBidPlaced;
        public static void InvokeBidPlaced(int seat, int bid) => OnBidPlaced?.Invoke(seat, bid);

        public static event Action<int, bool> OnGetBidFromLocalPlayer;
        public static void InvokeGetBidFromLocalPlayer(int value, bool canMatch) => OnGetBidFromLocalPlayer?.Invoke(value, canMatch);

        public static event Action<int> OnLocalPlayerBid;
        public static void InvokeLocalPlayerBid(int bid) => OnLocalPlayerBid?.Invoke(bid);

        #endregion
        
        #region Trump
        
        public static event Action<int, Suit> OnTrumpChosen;
        public static void InvokeTrumpChosen(int seat, Suit suit) => OnTrumpChosen?.Invoke(seat, suit);
        
        public static event Action OnGetTrumpFromLocalPlayer;
        public static void InvokeGetTrumpFromLocalPlayer() => OnGetTrumpFromLocalPlayer?.Invoke();

        public static event Action<Suit> OnLocalPlayerChoseTrump;
        public static void InvokeLocalPlayerChoseTrump(Suit suit) => OnLocalPlayerChoseTrump?.Invoke(suit);

        #endregion
        
        #region Card Play

        public static event Action<PlayedCard> OnCardPlayed;
        public static void InvokeCardPlayed(PlayedCard card) => OnCardPlayed?.Invoke(card);
        
        public static event Action<Suit?> OnGetCardFromLocalPlayer;
        public static void InvokeGetCardFromLocalPlayer(Suit? suit) => OnGetCardFromLocalPlayer?.Invoke(suit);

        public static event Action<Card> OnLocalPlayerPlayedCard;
        public static void InvokeLocalPlayerPlayedCard(Card card) => OnLocalPlayerPlayedCard?.Invoke(card);

        #endregion

        #region Assessment

        public static event Action<SharedLibrary.Player> OnPlayAssessed;
        public static void InvokePlayAssessed(SharedLibrary.Player localPlayer) => OnPlayAssessed?.Invoke(localPlayer);
        
        public static event Action<int, bool> OnSuccessDetermined;
        public static void InvokeSuccessDetermined(int seat, bool success) => OnSuccessDetermined?.Invoke(seat, success);

        public static event Action<Team[]> OnRoundAssessed;
        public static void InvokeRoundAssessed(Team[] teams) => OnRoundAssessed?.Invoke(teams);

        public static event Action<Team> OnGameOver;
        public static void InvokeGameOver(Team team) => OnGameOver?.Invoke(team);

        #endregion
    }
}