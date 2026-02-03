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
                Console.WriteLine(" d: 카드 뽑기 | mw 1: 쓰레기통->더미1 | m 1 2 3: 더미1(3장)->더미2");
                Console.WriteLine(" f 1 2: 더미1->F2 | fw: 쓰레기통->F | q: 종료");
                Console.Write("\n명령 입력 > ");
                
                string? input = Console.ReadLine()?.ToLower();
                if (string.IsNullOrEmpty(input) || input == "q") break;
                
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
            Console.WriteLine($"덱: {deckStr} ({_deck.Count}장)    쓰레기통: {wasteStr}");
            
            Console.Write("파운데이션: ");
            for (int i = 0; i < Solitaire<Card>.FoundationCount; i++)
            {
                string fndStr = _foundations[i].Count > 0 ? ((ICard)_foundations[i].Last()).GetString() : "[  ]";
                Console.Write($"{i+1}:{fndStr} ");
            }
            
            Console.WriteLine("\n\n테이블 더미 (1~7):");
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
