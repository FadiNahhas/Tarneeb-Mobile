using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Helpers;
using Helpers.Extensions;
using SharedLibrary;

namespace AI
{
    public class AIController
    {
        private const int Delay = 1000; // Delay for AI to make a move
        private LocalPlayer LocalPlayer { get; } // Reference to the player

        private readonly Dictionary<Suit, int> _suitWeights = new(); // Weights for each suit

        private readonly Dictionary<Suit, int> _suitCardAmount = new(); // Amount of cards for each suit

        private bool _startedLastPlay; // If the AI started the last play

        private bool _isHighestBidder; // If the AI is the highest bidder

        private bool _opponentsOutOfTrumpCards; // If only the AI's teammate has trump cards

        private List<Card> _otherInPlayTrumpCards; // List of all trump cards

        public AIController(LocalPlayer localPlayer)
        {
            LocalPlayer = localPlayer;
        }

        private Task WeighSuits()
        {
            _suitWeights.Clear();
            _suitCardAmount.Clear();
            _opponentsOutOfTrumpCards = false;
            _startedLastPlay = false;
            _isHighestBidder = false;
            
            // Get the amount of cards for each suit
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                var cards = LocalPlayer.Hand.CardsWithSuit(suit);
                _suitCardAmount.Add(suit, cards.Count);
            }

            // Check if player has at least 5 cards of a suit
            var hasDominantSuit = _suitCardAmount.Any(suit => suit.Value >= 5);
            Suit? dominantSuit = null;

            // If player has a dominant suit, then that suit is the dominant suit
            if (hasDominantSuit)
                dominantSuit = _suitCardAmount.OrderByDescending(suit => suit.Value).FirstOrDefault().Key;

            // Loop through each suit and weigh them
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                // Get all cards of the suit
                var cards = LocalPlayer.Hand.CardsWithSuit(suit);
                var totalScore = 0;

                // If the player has a dominant suit
                if (dominantSuit != null)
                {
                    // If the current suit is the dominant suit
                    if (suit == dominantSuit)

                        // If the player has 5, 6, or 7+ cards of the dominant suit, add 4, 6, or 8 points respectively
                        switch (_suitCardAmount[suit])
                        {
                            case 5:
                                totalScore += 4;
                                break;
                            case 6:
                                totalScore += 6;
                                break;
                            case >= 7:
                                totalScore += 8;
                                break;
                        }
                    // If not the dominant suit
                    else
                        switch (_suitCardAmount[suit])
                        {
                            case >= 5: // If the player has 5 or more cards of the suit, add 2 points
                                totalScore += 2;
                                break;
                            case < 3: // If the player has less than 3 cards of the suit, subtract 1 point
                                totalScore -= 1;
                                break;
                        }
                }
                else // If the player doesn't have a dominant suit
                {
                    switch (_suitCardAmount[suit])
                    {
                        case >= 5: // If the player has 5 or more cards of the suit, add 2 points
                            totalScore += 2;
                            break;
                        case < 3: // If the player has less than 3 cards of the suit, subtract 1 point
                            totalScore -= 1;
                            break;
                    }
                }

                // Loop through each card and add points based on the card value
                // If the card value is greater than 10, add the difference between the card value and 10
                foreach (var card in cards)
                {
                    var pointsToAdd = card.Value - 10;
                    pointsToAdd = Math.Clamp(pointsToAdd, 0, 4);
                    totalScore += pointsToAdd;

                    if (card.Value >= 13 && suit == dominantSuit)
                        totalScore += 3;
                }

                // Add the total score to the suit weights
                _suitWeights.Add(suit, totalScore);
            }

            return Task.CompletedTask;
        }

        public async Task<Order> GetBid(int minimumOrder, int currentHighestOrder, bool canMatch)
        {
            await WeighSuits();

            await Task.Delay(Delay);

            // Get the total weight of the player's hand
            var totalHandWeight = _suitWeights.Sum(suit => suit.Value);

            // Calculate the bid based on the total weight of the player's hand
            var bid = totalHandWeight switch
            {
                <= 12 => 0,
                > 12 and <= 16 => minimumOrder,
                > 16 and <= 21 => minimumOrder + 1,
                > 21 and <= 26 => minimumOrder + 2,
                > 26 => minimumOrder + 3
            };

            // If the current highest order is 0 and the AI can match the order (last to order), return the minimum order
            if (currentHighestOrder == 0 && canMatch) return new Order(minimumOrder, LocalPlayer.Seat);

            // If the AI's order is less than the current highest order, pass
            if (bid < currentHighestOrder) return new Order(0, LocalPlayer.Seat);

            // If the AI's order is equal to the current highest order, and the AI can match the order, return the order otherwise pass
            if (bid == currentHighestOrder) return new Order(canMatch ? bid : 0, LocalPlayer.Seat);

            // If the AI's order is greater than the current highest order, if the AI can match the order, match it, if can't match return the higher order value
            if (bid > currentHighestOrder) return new Order(canMatch ? currentHighestOrder : bid, LocalPlayer.Seat);

            // If the order is bigger or equal to the minimum order, return the order, otherwise pass
            return bid >= minimumOrder ? new Order(bid, LocalPlayer.Seat) : new Order(0, LocalPlayer.Seat);
        }

        public async Task<Suit> GetTrumpSuit()
        {
            // If the suit weights are empty, weigh the suits
            if (_suitWeights.Count == 0) await WeighSuits();

            await Task.Delay(Delay);
            
            // Get the suit with the biggest weight
            var chosenSuit = _suitWeights.OrderByDescending(suit => suit.Value).FirstOrDefault().Key;

            // Create a list of all trump cards that are not in the player's hand
            _otherInPlayTrumpCards = new List<Card>();
            for (var i = 2; i <= 14; i++)
            {
                var card = new Card(i, chosenSuit);
                if (LocalPlayer.Hand.Contains(card))
                    continue;
                _otherInPlayTrumpCards.Add(new Card(i, chosenSuit));
            }

            // Since the player is choosing the trump suit, the player is the highest bidder
            _isHighestBidder = true;

            // Return the chosen suit
            return chosenSuit;
        }

        public async Task<Card> GetCard(Game currentGame)
        {
            await Task.Delay(Delay);
            
            var trumpSuit = currentGame.CurrentRound.Trump; // Store the current round's trump suit
            var trumpCards = LocalPlayer.Hand.CardsWithSuit(trumpSuit); // Get all trump cards in the player's hand

            var trumpCardsPlayedThisPlay = currentGame.CurrentRound.CurrentPlay
                .PlayedCards // Get all trump cards played this play
                .Where(playedCard => playedCard.Card.Suit == trumpSuit)
                .ToList();

            var trumpCardsPlayedThisRound = currentGame.CurrentRound
                .AllPlayedCards.CardsWithSuit(trumpSuit); // Get all trump cards played this round

            var otherPlayersHaveTrumpCards =
                trumpCards.Count + trumpCardsPlayedThisRound.Count < 13; // Check if other players have trump cards
            
            var trumpCardsRemaining = new List<Card>();

            // If the player is the highest bidder, store all trump cards remaining that are not in the player's hand
            if (_isHighestBidder)
            {
                foreach (var card in _otherInPlayTrumpCards)
                {
                    if (currentGame.CurrentRound.AllPlayedCards.Contains(card) || trumpCards.Contains(card))
                        continue;
                    trumpCardsRemaining.Add(card);
                }
            }

            // If the player started the previous play, and there are trump cards remaining, play the highest trump card (try to draw out all trump cards)
            if (_startedLastPlay && otherPlayersHaveTrumpCards)
            {
                var lastPlay = currentGame.CurrentRound.Plays[^2]; // Get previous play
                
                var wasLeadSuitTrump = lastPlay.LeadingSuit == trumpSuit; // Check if the lead suit was trump

                var trumpCardsPlayedByOpponents = lastPlay.PlayedCards.Where(card => // Get all trump cards played by opponents
                    card.Card.GetSuit() == trumpSuit && IsOpponent(card.Seat)).ToList();
                
                // If the lead suit was trump, and no opponents have played trump cards, set opponents out of trump cards to true
                if (wasLeadSuitTrump)
                {
                    _opponentsOutOfTrumpCards = trumpCardsPlayedByOpponents.Count == 0;
                }
            }

            /*
             * First to play
             */
            if (currentGame.CurrentRound.CurrentPlay.PlayedCards.Count == 0)
            {
                // If this is the first play, start with trump suit
                if (currentGame.CurrentRound.AllPlayedCards.Count == 0)
                {
                    var highestTrumpCard = trumpCards.HighestCard();

                    // if highest trump card is Ace, play it
                    if (highestTrumpCard.Value == 14)
                    {
                        _startedLastPlay = true;
                        return await Task.FromResult(highestTrumpCard);
                    }

                    // if highest trump card is not an Ace, play lowest card
                    _startedLastPlay = true;
                    return await Task.FromResult(trumpCards.LowestCard());
                }

                // If player is highest bidder and there are trump cards remaining with other players, play trump card
                if (_isHighestBidder && trumpCardsRemaining.Count > 0 && trumpCards.Count > 0 &&
                    !_opponentsOutOfTrumpCards)
                {
                    // If player has the highest trump card, play it
                    var highestTrumpCard = trumpCards.HighestCard();

                    var highestRemainingTrumpCard = trumpCardsRemaining.HighestCard();

                    if (highestTrumpCard.Value > highestRemainingTrumpCard.Value)
                    {
                        _startedLastPlay = true;
                        return await Task.FromResult(highestTrumpCard);
                    }

                    _startedLastPlay = true;
                    return await Task.FromResult(trumpCards.Last());
                }

                // If not the first play
                // Play highest card
                Card? highestCard = LocalPlayer.Hand.HighestCard();
                var nonTrumpCards = LocalPlayer.Hand.CardsWithoutSuit(trumpSuit);

                // If the player only has trump cards, play the lowest value one;
                if (nonTrumpCards.Count == 0)
                {
                    highestCard = LocalPlayer.Hand.LowestCard();
                }
                
                // If highest is trump suit and other players no longer have trump cards, get highest non-trump card, and the player has non-trump cards
                if (highestCard.Value.GetSuit() == trumpSuit && _opponentsOutOfTrumpCards && nonTrumpCards.Count > 0)
                {
                    highestCard = nonTrumpCards.HighestCard();
                }
                
                // Check if all higher cards from the same suit have been played
                var playedCardsOfSameSuitWithHigherValue = currentGame.CurrentRound.AllPlayedCards
                    .Where(card =>
                        card.GetSuit() == highestCard.Value.GetSuit() && card.Value > highestCard.Value.Value).ToList();
                var amountOfHigherCards = 14 - highestCard.Value.Value; // Amount of possible higher cards

                // If all higher cards have been played, play highest card
                if (playedCardsOfSameSuitWithHigherValue.Count == amountOfHigherCards)
                {
                    _startedLastPlay = true;
                    return await Task.FromResult(highestCard.Value);
                }

                // play lowest card of that suit
                if (LocalPlayer.Hand.HasSuit(highestCard.Value.GetSuit()))
                {
                    _startedLastPlay = true;
                    return await Task.FromResult(LocalPlayer.Hand.LowestCardWithSuit(highestCard.Value.GetSuit()));
                }

                // Play random lowest card except trump
                _startedLastPlay = true;
                return await Task.FromResult(LocalPlayer.Hand.LowestCardWithSuit(trumpSuit));
            }

            /*
             * Not first to play
             */
            _startedLastPlay = false;
            var leadSuit = currentGame.CurrentRound.CurrentPlay.LeadingSuit; // Store lead suit
            
            if (leadSuit == null)
            {
                throw new Exception("Lead suit is null");
            }
            
            var leadSuitCards = LocalPlayer.Hand.CardsWithSuit(leadSuit.Value); // Get all cards of lead suit

            var leadCardsPlayedThisPlay =
                currentGame.CurrentRound.CurrentPlay.PlayedCards
                    .CardsWithSuit(leadSuit.Value); // Get all lead cards played this round

            var leadCardsPlayedThisRound =
                currentGame.CurrentRound.AllPlayedCards
                    .CardsWithSuit(leadSuit.Value); // Get all lead cards played this round
            /*
             * Has lead suit
             */
            if (LocalPlayer.Hand.HasSuit(leadSuit.Value))
            {
                // If lead suit is not trump suit
                if (trumpSuit != leadSuit)
                {
                    // If trump has been played, play lowest card
                    if (currentGame.CurrentRound.CurrentPlay.PlayedCards.ContainsSuit(trumpSuit))
                    {
                        return await Task.FromResult(leadSuitCards.LowestCard());
                    }
                }

                // if highest played card is higher than the player's highest card, play lowest card
                var highestPlayedCard =
                    currentGame.CurrentRound.CurrentPlay.PlayedCards.HighestPlayedCardWithSuit(leadSuit.Value);
                
                if (highestPlayedCard.Card.Value > leadSuitCards.HighestCard().Value)
                {
                    return await Task.FromResult(leadSuitCards.LowestCard());
                }

                // If last to play and teammate has the highest card, play lowest card
                if (IsTeammate(highestPlayedCard.Seat) && currentGame.CurrentRound.CurrentPlay.PlayedCards.Count == 3)
                {
                    return await Task.FromResult(leadSuitCards.LowestCard());
                }

                // If teammate has highest cards, and all cards between teammate highest and player highest have been played, play lowest card
                var potentialCard = leadSuitCards.HighestCard();
                
                var inBetweenCards = 
                    currentGame.CurrentRound.AllPlayedCards.CardsWithinRange(highestPlayedCard.Card.Value, potentialCard.Value);
                
                // If potential card is 9 and highest played card is 7, there are 1 possible cards in between (9 - 7 - 1 = 1)
                // If potential card is Ace and highest played card is 10, there are 3 possible cards in between (14 - 10 - 1 = 3)
                var possibleNumberOfCards = potentialCard.Value - highestPlayedCard.Card.Value - 1;
                
                var allCardsBetweenPlayed = inBetweenCards.Count == possibleNumberOfCards;
                
                if (IsTeammate(highestPlayedCard.Seat) && allCardsBetweenPlayed)
                {
                    return await Task.FromResult(leadSuitCards.LowestCard());
                }

                // If last to play, play the lowest card that is higher than the highest card played
                if (currentGame.CurrentRound.CurrentPlay.PlayedCards.Count == 3)
                {
                    var potentialCards = leadSuitCards.CardsAbove(highestPlayedCard.Card.Value);

                    return await Task.FromResult(potentialCards.Count > 0 ? 
                        potentialCards.First() : leadSuitCards.LowestCard());
                }

                // Play highest card
                return await Task.FromResult(leadSuitCards.HighestCard());
            }

            /*
             * Doesn't have lead suit
             */

            // If player has trump cards
            if (trumpCards.Count > 0)
            {
                // Check if a player has non-trump non-lead cards, and play lowest one
                var hasNonTrumpCard = LocalPlayer.Hand.CardsWithoutSuit(trumpSuit).Count > 0;

                // If there are other trump cards played
                if (trumpCardsPlayedThisPlay.Count > 0)
                {
                    var highestTrumpCardThisPlay = trumpCardsPlayedThisPlay.HighestPlayedCard(); // Get highest trump card played this play
                    
                    // If the highest trump card is higher than the player's highest trump card
                    if (highestTrumpCardThisPlay.Card.Value > trumpCards.HighestCard().Value)
                    {
                        // Check if a player has non-trump non-lead cards, and play lowest one, otherwise play lowest trump

                        return await Task.FromResult(hasNonTrumpCard ? LocalPlayer.Hand.LowestCardWithoutSuit(trumpSuit) : trumpCards.LowestCard());
                    }

                    // If the highest trump card played is by a teammate
                    if (IsTeammate(highestTrumpCardThisPlay.Seat))
                    {
                        // If the player has non trump cards, play lowest one, otherwise play lowest trump
                        return await Task.FromResult(hasNonTrumpCard ? LocalPlayer.Hand.LowestCardWithoutSuit(trumpSuit) : trumpCards.LowestCard());
                    }

                    // If player has higher trump card and the current highest card is not a teammate, play it
                    var higherTrumpCards = trumpCards.CardsAbove(highestTrumpCardThisPlay.Card.Value);

                    if (higherTrumpCards.Count > 0)
                    {
                        return await Task.FromResult(higherTrumpCards.HighestCard());
                    }
                }
                else // If no trump cards have been played
                {
                    // Get highest played card of lead suit
                    var highestPlayedLeadSuitCard = leadCardsPlayedThisPlay.HighestPlayedCardWithSuit(leadSuit.Value);
                    
                    // Get all remaining higher cards of the same suit
                    var playedCardsHigherThanHighestPlayed =
                        leadCardsPlayedThisRound.CardsAbove(highestPlayedLeadSuitCard.Card.Value);
                    
                    var possibleHigherCardsCount = 14 - highestPlayedLeadSuitCard.Card.Value;
                    
                    var isHighestPossibleCard = playedCardsHigherThanHighestPlayed.Count == possibleHigherCardsCount;
                    
                    // If the highest played card is a teammate and there are no remaining higher cards of the same suit, play lowest non trump
                    if (IsTeammate(highestPlayedLeadSuitCard.Seat) && isHighestPossibleCard)
                    {
                        if (hasNonTrumpCard)
                        {
                            return await Task.FromResult(LocalPlayer.Hand.LowestCardWithoutSuit(trumpSuit));
                        }
                    }
                }

                // Play lowest trump card
                return await Task.FromResult(trumpCards.LowestCard());
            }

            // Play lowest value card
            return await Task.FromResult(LocalPlayer.Hand.LowestCard());
        }

        private bool IsTeammate(int seatId)
        {
            return LocalPlayer.Seat == 0 || LocalPlayer.Seat == 2 ? seatId == 0 || seatId == 2 : seatId == 1 || seatId == 3;
        }
        
        private bool IsOpponent(int seatId)
        {
            return !IsTeammate(seatId);
        }
    }
}