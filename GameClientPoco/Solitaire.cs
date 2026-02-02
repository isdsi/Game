using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        public void ExecuteCommand(CardCommand command)
        {
            switch (command.Type)
            {
                case CommandType.Draw:
                    HandleDraw(); // 기존 d 로직
                    break;
                case CommandType.MoveWasteToPile: // 쓰레기통에서 더미로 이동 (mw [to])
                    if (_waste.Count > 0)
                    {
                        if (CanMoveToPile(_waste.Last(), command.To))
                        {
                            _piles[command.To].Add(_waste.Last());
                            _waste.RemoveAt(_waste.Count - 1);
                        }
                    }
                    break;
                case CommandType.MoveToPile: // 더미 간 이동 (m [from] [to] [count])
                    MoveCards(command.From, command.To, command.Count);
                    break;
                case CommandType.MoveToFoundation: // 더미에서 파운데이션으로
                    if (_piles[command.From].Count > 0 && CanMoveToFoundation(_piles[command.From].Last(), command.To))
                    {
                        _foundations[command.To].Add(_piles[command.From].Last());
                        _piles[command.From].RemoveAt(_piles[command.From].Count - 1);
                    }
                    break;
                case CommandType.MoveWasteToFoundation: // 쓰레기통에서 파운데이션으로
                    if (_waste.Count > 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (CanMoveToFoundation(_waste.Last(), i))
                            {
                                _foundations[i].Add(_waste.Last());
                                _waste.RemoveAt(_waste.Count - 1);
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        private void HandleDraw()
        {
            if (_deck.Count == 0)
            {
                _deck.AddRange(_waste);
                _waste.Clear();
                _deck.ForEach(c => c.IsFaceUp = false);
                _deck.Reverse(); // 덱을 다시 뒤집을 때 순서 유지
            }
            else
            {
                Card c = _deck[0];
                _deck.RemoveAt(0);
                c.IsFaceUp = true;
                _waste.Add(c);
            }
        }

        private void MoveCards(int from, int to, int count)
        {
            if (from < 0 || from > 6 || to < 0 || to > 6) return;
            if (_piles[from].Count < count) return;

            int startIndex = _piles[from].Count - count;
            Card firstCardOfBunch = _piles[from][startIndex];

            if (firstCardOfBunch.IsFaceUp && CanMoveToPile(firstCardOfBunch, to))
            {
                List<Card> bunch = _piles[from].GetRange(startIndex, count);
                _piles[from].RemoveRange(startIndex, count);
                _piles[to].AddRange(bunch);
            }
        }

        private bool CanMoveToPile(Card card, int toIdx)
        {
            if (toIdx < 0 || toIdx > 6) return false;
            if (_piles[toIdx].Count == 0) return card.Rank == 13; // King only

            Card target = _piles[toIdx].Last();
            return target.IsFaceUp && target.Rank == card.Rank + 1 && target.GetColor() != card.GetColor();
        }

        private bool CanMoveToFoundation(Card card, int fIdx)
        {
            if (fIdx < 0 || fIdx > 3) return false;
            if (_foundations[fIdx].Count == 0) return card.Rank == 1; // Ace only

            Card target = _foundations[fIdx].Last();
            return target.Suit == card.Suit && target.Rank == card.Rank - 1;
        }

        public void CheckFlipTopCards()
        {
            foreach (var p in _piles)
            {
                if (p.Count > 0 && !p.Last().IsFaceUp) p.Last().IsFaceUp = true;
            }
        }

        public bool IsGameWon()
        {
            // 4개의 파운데이션이 각각 13장의 카드를 가지고 있으면 승리
            return _foundations.All(f => f.Count == 13);
        }
    }
}
