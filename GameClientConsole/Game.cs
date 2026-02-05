using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using GameClientPoco;

namespace GameClientConsole
{
    public class Game
    {
        private ILogger _logger;

        private Solitaire<Card> _solitaire;
        
        private IList<Card> _deck;
        private IList<Card> _waste;
        private IList<Card>[] _foundations = new IList<Card>[Solitaire<Card>.FoundationCount];
        private IList<Card>[] _piles = new IList<Card>[Solitaire<Card>.PileCount];

        public Game(ILogger logger)
        {
            _logger = logger;
            _deck = new List<Card>();
            _waste = new List<Card>();
            for (int i = 0; i < Solitaire<Card>.FoundationCount; i++) _foundations[i] = new List<Card>();
            for (int i = 0; i < Solitaire<Card>.PileCount; i++)
            {
                _piles[i] = new List<Card>();
            }
            _solitaire = new Solitaire<Card>(_logger, _deck, _waste, _foundations, _piles,
                (s, r) => new Card(s, r), 777);
            Console.OutputEncoding = Encoding.UTF8;
        }

        public void Play()
        {
            while (true)
            {
                // 승리 여부 확인
                if (_solitaire.IsGameWon())
                {
                    Console.WriteLine("\n축하합니다! 모든 카드를 맞추어 승리하셨습니다! 🎉");
                    break;
                }
                //Console.Clear();
                DrawBoard();
                Console.WriteLine("\n[ 명령어 안내 ]");
                Console.WriteLine(" d: 카드 뽑기 | mwp 1: Waste->Pile1 | mfp 1 4 : Foundation1 -> Pile4 | mpp 1 2 3: Pile1(3장)->Pile2");
                Console.WriteLine(" mpf 1 2: Pile1->Foundation2 | mwf (2): Waste->Foundation(인자 있으면 2번)| q: 종료");
                Console.Write("\n명령 입력 > ");
                
                string? input = Console.ReadLine()?.ToLower();
                if (string.IsNullOrEmpty(input) || input == "q") break;

                _logger.LogInformation(input);
                ProcessInput(input);
                _solitaire.CheckFlipTopCards(); // 최하단 카드를 open 한다.
            }
        }

        private void DrawBoard()
        {
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("   SOLITAIRE PRO - FULL INTERACTION VERSION");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            
            string deckStr = _deck.Count > 0 ? "[XX]" : "[  ]";
            string wasteStr = _waste.Count > 0 ? ((ICard)_waste.Last()).GetString() : "[  ]";
            Console.WriteLine($"Deck: {deckStr} ({_deck.Count}장)    Waste: {wasteStr}");
            
            Console.Write("Foundation: ");
            for (int i = 0; i < Solitaire<Card>.FoundationCount; i++)
            {
                string fndStr = _foundations[i].Count > 0 ? ((ICard)_foundations[i].Last()).GetString() : "[  ]";
                Console.Write($"{i+1}:{fndStr} ");
            }
            
            Console.WriteLine("\n\nPile (1~7):");
            int maxHeight = _piles.Max(p => p.Count);
            for (int row = 0; row < Math.Max(maxHeight, 1); row++)
            {
                for (int col = 0; col < Solitaire<Card>.PileCount; col++)
                {
                    if (row < _piles[col].Count)
                        Console.Write($"{((ICard)_piles[col][row]).GetString()}   ");
                    else
                        Console.Write("        ");
                }
                Console.WriteLine();
            }
        }

        public void ProcessInput(string input)
        {
            var command = CommandParser.Parse(input);
            if (!command.IsValid) return;

            _solitaire.ExecuteCommand(command);
        }
    }
}
