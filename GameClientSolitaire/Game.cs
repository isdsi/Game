using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using GameClientPoco;

namespace GameClientSolitaire
{
    public class Game
    {
        private Solitaire _solitaire;

        public Game(Solitaire solitaire)
        {
            _solitaire = solitaire;
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
            
            string deckStr = _solitaire.Deck.Count > 0 ? "[XX]" : "[  ]";
            string wasteStr = _solitaire.Waste.Count > 0 ? _solitaire.Waste.Last().ToString() : "[  ]";
            Console.WriteLine($"덱: {deckStr} ({_solitaire.Deck.Count}장)    쓰레기통: {wasteStr}");
            
            Console.Write("파운데이션: ");
            for (int i = 0; i < 4; i++)
            {
                string fndStr = _solitaire.Foundations[i].Count > 0 ? _solitaire.Foundations[i].Last().ToString() : "[  ]";
                Console.Write($"{i+1}:{fndStr} ");
            }
            
            Console.WriteLine("\n\n테이블 더미 (1~7):");
            int maxHeight = _solitaire.Piles.Max(p => p.Count);
            for (int row = 0; row < Math.Max(maxHeight, 1); row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (row < _solitaire.Piles[col].Count)
                        Console.Write($"{_solitaire.Piles[col][row]}   ");
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
