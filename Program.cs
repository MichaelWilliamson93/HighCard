using System;
using System.Collections.Generic;
using System.Linq;

namespace HighCard
{
    public enum Suit { Spades, Clubs, Hearts, Diamonds }
    public enum Face { Ace = 1, Jack = 11, Queen = 12, King = 13 }
    public class Card
    {
        public int Value { get; set; }
        public virtual string CardDesc()
        {
            return Value.ToString();
        }
        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Card c = (Card)obj;
                return (Value == c.Value);
            }
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    public class SuitCard : Card
    {
        public Suit Suit { get; set; }
        public override string CardDesc()
        {
            return Value + " of " + Suit;
        }
    }

    public class FaceCard : SuitCard
    {
        public override string CardDesc()
        {
            return (Face)Value + " of " + Suit;
        }
    }

    public class WildCard : Card
    {
        private readonly string Face = "Joker";
        public override string CardDesc()
        {
            return Face;
        }
    }

    public class Scoreboard
    {
        public string? Name { get; set; }
        public int Score { get; set; }
    }

    public class Deck
    {
        private readonly byte maxCards;
        private readonly int suits;
        private readonly byte wildCards;
        private List<Card> orderedDeck;
        private Stack<Card> gameDeck;
        public Deck()
        {
            maxCards = 54;
            suits = Enum.GetNames(typeof(Suit)).Length;
            wildCards = 2;
            orderedDeck = new();
            gameDeck = new();
        }
        public void BuildDeck()
        {
            int suitSize = (maxCards - wildCards) / suits;
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                for (int j = 1; j <= suitSize; j++)
                {
                    if (j == 1)
                    {
                        orderedDeck.Add(new FaceCard() { Suit = suit, Value = j });
                    }
                    else if (j > 1 && j < 11)
                    {
                        orderedDeck.Add(new SuitCard() { Suit = suit, Value = j });
                    }
                    else
                    {
                        orderedDeck.Add(new FaceCard() { Suit = suit, Value = j });
                    }
                }
            }
            for (int i = 0; i < wildCards; i++)
            {
                orderedDeck.Add(new WildCard() { Value = suitSize + 1 });
            }
        }
        public void Shuffle()
        {
            List<Card> randomCards = new();
            Random r = new();
            randomCards = orderedDeck.OrderBy(x => r.Next()).ToList();
            foreach (Card card in randomCards)
            {
                gameDeck.Push(card);
            }
        }
        public Card DrawCards()
        {
            return gameDeck.Pop();
        }
        public Boolean DeckEmpty()
        {
            return gameDeck.Count != 0;
        }

        public List<Card> getOrderedDeck()
        {
            return orderedDeck;
        }
        public Stack<Card> getGameDeck()
        {
            return gameDeck;
        }
    }

    public class Game
    {
        private Scoreboard player;
        private Scoreboard dealer;
        private Scoreboard ties;
        private Deck gameDeck;
        public Game()
        {
            player = new Scoreboard() { Name = "Player", Score = 0 };
            dealer = new Scoreboard() { Name = "Dealer", Score = 0 };
            ties = new Scoreboard() { Name = "Ties", Score = 0 };
            gameDeck = new Deck();
            gameDeck.BuildDeck();
            gameDeck.Shuffle();
        }
        public void Play(int numGames)
        {
            for (int i = 0; i < numGames; i++)
            {
                if (gameDeck.DeckEmpty())
                {
                    Card playersCard = DealCards(player);
                    Card dealersCard = DealCards(dealer);

                    switch (DetermineResult(playersCard.Value, dealersCard.Value))
                    {
                        case 1:
                            player.Score++;
                            Console.WriteLine("\n" + player.Name + " has won. \n");
                            break;
                        case 2:
                            dealer.Score++;
                            Console.WriteLine("\n" + dealer.Name + " has won. \n");
                            break;
                        case 3:
                            ties.Score++;
                            Console.WriteLine("\nIt's a Tie.\n");
                            break;
                        default:
                            Console.WriteLine("\nError, result has not been found.\n");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nNo cards left in deck, reshuffling the cards.\n");
                    gameDeck.Shuffle();
                }
            }
            Console.Write("Scoreboard\n" + player.Name + ": " + player.Score + "\n" + dealer.Name + ": " + dealer.Score + "\n" + ties.Name + ": " + ties.Score + "\n");
        }

        private Card DealCards(Scoreboard player)
        {
            Card dealtCard = gameDeck.DrawCards();
            Console.WriteLine(player.Name + " has been dealt a " + dealtCard.CardDesc() + ".");
            return dealtCard;
        }

        public static int DetermineResult(int playersCard, int dealersCard)
        {
            if (playersCard > dealersCard)
            {
                return 1;
            }
            else if (playersCard < dealersCard)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game game = new ();
            Console.WriteLine("Please enter the number of games you would like to play of High Card.");
            if (!int.TryParse(Console.ReadLine(), out int response))
            {
                Console.WriteLine("\nInvalid value entered");
            }
            else
            {
                Console.Clear();
                game.Play(response);
            }
        }
    }
}