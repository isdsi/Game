using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GameClientPoco
{
    public class Solitaire
    {
        private List<Card> _deck = new List<Card>();
        private List<Card> _waste = new List<Card>();
        private List<Card>[] _foundations = new List<Card>[4];
        private List<Card>[] _piles = new List<Card>[7];

        public IReadOnlyList<Card> Deck => _deck;
        public IReadOnlyList<Card> Waste => _waste;
        public IReadOnlyList<Card>[] Foundations => _foundations;
        public IReadOnlyList<Card>[] Piles => _piles;

        // 시스템 로그
        private ILogger _logger;

        private readonly int _seed;

        public Solitaire(ILogger logger, int seed = 777)
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
                for (int r = 1; r <= 13; r++) _deck.Add(new Card(s, r));
            }

            Random rnd = new Random(_seed);
            _deck = _deck.OrderBy(x => rnd.Next()).ToList();

            for (int i = 0; i < 4; i++) _foundations[i] = new List<Card>();

            for (int i = 0; i < 7; i++)
            {
                _piles[i] = new List<Card>();
                for (int j = 0; j <= i; j++)
                {
                    Card c = _deck[0];
                    _deck.RemoveAt(0);
                    if (j == i) c.IsFaceUp = true;
                    _piles[i].Add(c);
                }
            }
        }
    }
}
