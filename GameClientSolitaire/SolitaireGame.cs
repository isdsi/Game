using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GameClientSolitaire
{
    public enum Suit { Spades, Hearts, Diamonds, Clubs }

    public class Card
    {
        public Suit Suit { get; }
        public int Rank { get; }
        public bool IsFaceUp { get; set; }

        public Card(Suit suit, int rank)
        {
            Suit = suit;
            Rank = rank;
            IsFaceUp = false;
        }

        public string GetColor() => (Suit == Suit.Hearts || Suit == Suit.Diamonds) ? "Red" : "Black";

        public override string ToString()
        {
            if (!IsFaceUp) return "[??]";
            string r = Rank switch
            {
                1 => "A", 11 => "J", 12 => "Q", 13 => "K", _ => Rank.ToString()
            };
            char s = Suit switch
            {
                Suit.Spades => '♠', Suit.Hearts => '♥', Suit.Diamonds => '♦', Suit.Clubs => '♣', _ => ' '
            };
            return $"[{r}{s}]";
        }
    }

    public class SolitaireGame
    {
        private List<Card> deck = new List<Card>();
        private List<Card> waste = new List<Card>();
        private List<Card>[] foundations = new List<Card>[4];
        private List<Card>[] piles = new List<Card>[7];

        // 시스템 로그
        private ILogger _logger;

        private readonly int _seed;

        public SolitaireGame(ILogger logger, int seed = 777)
        {
            _logger = logger;
            _seed = seed;
            Console.OutputEncoding = Encoding.UTF8;
            InitializeGame();
        }

        private void InitializeGame()
        {
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
            {
                for (int r = 1; r <= 13; r++) deck.Add(new Card(s, r));
            }

            Random rnd = new Random(_seed);
            deck = deck.OrderBy(x => rnd.Next()).ToList();

            for (int i = 0; i < 4; i++) foundations[i] = new List<Card>();

            for (int i = 0; i < 7; i++)
            {
                piles[i] = new List<Card>();
                for (int j = 0; j <= i; j++)
                {
                    Card c = deck[0];
                    deck.RemoveAt(0);
                    if (j == i) c.IsFaceUp = true;
                    piles[i].Add(c);
                }
            }
        }

        public void Play()
        {
            while (true)
            {
                //Console.Clear();
                DrawBoard();
                Console.WriteLine("\n[ 명령어 안내 ]");
                Console.WriteLine(" d: 카드 뽑기 | mw 1: 쓰레기통->더미1 | m 1 2 3: 더미1(3장)->더미2");
                Console.WriteLine(" f 1 2: 더미1->F2 | fw: 쓰레기통->F | q: 종료");
                Console.Write("\n명령 입력 > ");
                
                string? input = Console.ReadLine()?.ToLower();
                if (string.IsNullOrEmpty(input) || input == "q") break;
                
                ProcessInput(input);
                CheckFlipTopCards();
            }
        }

        private void DrawBoard()
        {
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("   SOLITAIRE PRO - FULL INTERACTION VERSION");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            
            string deckStr = deck.Count > 0 ? "[XX]" : "[  ]";
            string wasteStr = waste.Count > 0 ? waste.Last().ToString() : "[  ]";
            Console.WriteLine($"덱: {deckStr} ({deck.Count}장)    쓰레기통: {wasteStr}");
            
            Console.Write("파운데이션: ");
            for (int i = 0; i < 4; i++)
            {
                string fndStr = foundations[i].Count > 0 ? foundations[i].Last().ToString() : "[  ]";
                Console.Write($"{i+1}:{fndStr} ");
            }
            
            Console.WriteLine("\n\n테이블 더미 (1~7):");
            int maxHeight = piles.Max(p => p.Count);
            for (int row = 0; row < Math.Max(maxHeight, 1); row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (row < piles[col].Count)
                        Console.Write($"{piles[col][row]}   ");
                    else
                        Console.Write("        ");
                }
                Console.WriteLine();
            }
        }

        private void ProcessInput(string input)
        {
            var command = CommandParser.Parse(input);
            if (!command.IsValid) return;

            ExecuteCommand(command);
        }

        public void ExecuteCommand(GameCommand command)
        {
            switch (command.Type)
            {
                case CommandType.Draw:
                    HandleDraw(); // 기존 d 로직
                    break;
                case CommandType.MoveWasteToPile: // 쓰레기통에서 더미로 이동 (mw [to])
                    if (waste.Count > 0)
                    {
                        if (CanMoveToPile(waste.Last(), command.To))
                        {
                            piles[command.To].Add(waste.Last());
                            waste.RemoveAt(waste.Count - 1);
                        }
                    }
                    break;
                case CommandType.MoveToPile: // 더미 간 이동 (m [from] [to] [count])
                    MoveCards(command.From, command.To, command.Count);
                    break;
                case CommandType.MoveToFoundation: // 더미에서 파운데이션으로
                    if (piles[command.From].Count > 0 && CanMoveToFoundation(piles[command.From].Last(), command.To))
                    {
                        foundations[command.To].Add(piles[command.From].Last());
                        piles[command.From].RemoveAt(piles[command.From].Count - 1);
                    }                
                    break;
                case CommandType.MoveWasteToFoundation: // 쓰레기통에서 파운데이션으로
                    if (waste.Count > 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (CanMoveToFoundation(waste.Last(), i))
                            {
                                foundations[i].Add(waste.Last());
                                waste.RemoveAt(waste.Count - 1);
                                break;
                            }
                        }
                    }
                    break;
            } 
        }

        private void HandleDraw()
        {
            if (deck.Count == 0) 
            { 
                deck.AddRange(waste); 
                waste.Clear(); 
                deck.ForEach(c => c.IsFaceUp = false); 
                deck.Reverse(); // 덱을 다시 뒤집을 때 순서 유지
            }
            else 
            { 
                Card c = deck[0]; 
                deck.RemoveAt(0); 
                c.IsFaceUp = true; 
                waste.Add(c); 
            }
        }

        private void MoveCards(int from, int to, int count)
        {
            if (from < 0 || from > 6 || to < 0 || to > 6) return;
            if (piles[from].Count < count) return;

            int startIndex = piles[from].Count - count;
            Card firstCardOfBunch = piles[from][startIndex];

            if (firstCardOfBunch.IsFaceUp && CanMoveToPile(firstCardOfBunch, to))
            {
                List<Card> bunch = piles[from].GetRange(startIndex, count);
                piles[from].RemoveRange(startIndex, count);
                piles[to].AddRange(bunch);
            }
        }

        private bool CanMoveToPile(Card card, int toIdx)
        {
            if (toIdx < 0 || toIdx > 6) return false;
            if (piles[toIdx].Count == 0) return card.Rank == 13; // King only
            
            Card target = piles[toIdx].Last();
            return target.IsFaceUp && target.Rank == card.Rank + 1 && target.GetColor() != card.GetColor();
        }

        private bool CanMoveToFoundation(Card card, int fIdx)
        {
            if (fIdx < 0 || fIdx > 3) return false;
            if (foundations[fIdx].Count == 0) return card.Rank == 1; // Ace only
            
            Card target = foundations[fIdx].Last();
            return target.Suit == card.Suit && target.Rank == card.Rank - 1;
        }

        private void CheckFlipTopCards()
        {
            foreach (var p in piles)
            {
                if (p.Count > 0 && !p.Last().IsFaceUp) p.Last().IsFaceUp = true;
            }
        }
    }
}
