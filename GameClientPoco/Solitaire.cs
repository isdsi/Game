using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameClientPoco
{
    public class Solitaire<T> where T : ICard
    {
        // 이제 외부에서 주입된 리스트(ObservableCollection 포함)를 참조합니다.
        private IList<T> _deck;
        private IList<T> _waste;
        private IList<T>[] _foundations;
        private IList<T>[] _piles;

        public const int FoundationCount = 4;
        public const int PileCount = 7;
        public const int SuitCardCount = 13;

        // 시스템 로그
        private ILogger _logger;

        private readonly int _seed;

        private readonly Func<Suit, int, T> _cardFactory;

        public Solitaire(ILogger logger,
            IList<T> deck,
            IList<T> waste,
            IList<T>[] foundations,
            IList<T>[] piles,
            Func<Suit, int, T> cardFactory,
            int seed = 777)
        {
            _logger = logger;
            _deck = deck;
            _waste = waste;
            _foundations = foundations;
            _piles = piles;
            _cardFactory = cardFactory;
            _seed = seed;

            foreach (Suit s in Enum.GetValues(typeof(Suit)))
            {
                for (int r = 1; r <= SuitCardCount; r++) _deck.Add(_cardFactory(s, r));
            }

            Random rnd = new Random(_seed);

            //_deck = _deck.OrderBy(x => rnd.Next()).ToList();
            // OrderBy 대신 코드 내에서 랜덤 정렬
            int n = _deck.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                // 값만 맞바꾸기 (Swap)
                T value = _deck[k];
                _deck[k] = _deck[n];
                _deck[n] = value;
            }

            for (int i = 0; i < PileCount; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    T c = _deck[0];
                    _deck.RemoveAt(0);
                    if (j == i) c.IsFaceUp = true;
                    _piles[i].Add(c);
                }
            }
        }


        public bool ExecuteCommand(CardCommand command)
        {
            bool result = false;
            switch (command.Type)
            {
                case CommandType.Draw:
                    HandleDraw(); // 기존 d 로직
                    break;
                case CommandType.MoveWasteToPile: // 쓰레기통에서 더미로 이동 (mw [to])
                    if (_waste.Count > 0)
                    {
                        if (CanMoveWasteToPile(_waste.Last(), command.To))
                        {
                            _piles[command.To].Add(_waste.Last());
                            _waste.RemoveAt(_waste.Count - 1);
                            result = true;
                        }
                    }
                    break;
                case CommandType.MovePileToPile: // 더미 간 이동 (m [from] [to] [count])
                    MoveCards(command.From, command.To, command.Count);
                    result = true;
                    break;
                case CommandType.MovePileToFoundation: // 더미에서 파운데이션으로
                    if (_piles[command.From].Count > 0 && CanMovePileToFoundation(_piles[command.From].Last(), command.To))
                    {
                        _foundations[command.To].Add(_piles[command.From].Last());
                        _piles[command.From].RemoveAt(_piles[command.From].Count - 1);
                        result = true;
                    }
                    break;
                case CommandType.MoveWasteToFoundation: // 쓰레기통에서 파운데이션으로
                    if (_waste.Count > 0)
                    {
                        for (int i = 0; i < FoundationCount; i++)
                        {
                            if (CanMovePileToFoundation(_waste.Last(), i))
                            {
                                _foundations[i].Add(_waste.Last());
                                _waste.RemoveAt(_waste.Count - 1);
                                result = true;
                                break;
                            }
                        }
                    }
                    break;
            }

            return result;
        }

        private void HandleDraw()
        {
            if (_deck.Count == 0)
            {
                //_deck.AddRange(_waste);
                foreach (var c in _waste)
                {
                    _deck.Add(c);
                }
                _waste.Clear();

                //_deck.ForEach(c => c.IsFaceUp = false);
                foreach (var c in _waste)
                {
                    c.IsFaceUp = false;
                }
                _deck.Reverse(); // 덱을 다시 뒤집을 때 순서 유지
            }
            else
            {
                T c = _deck[0];
                _deck.RemoveAt(0);
                c.IsFaceUp = true;
                _waste.Add(c);
            }
        }

        private void MoveCards(int from, int to, int count)
        {
            if (from < 0 || from > (PileCount - 1) || to < 0 || to > (PileCount - 1)) return;
            if (_piles[from].Count < count) return;

            int startIndex = _piles[from].Count - count;
            T firstCardOfBunch = _piles[from][startIndex];

            if (firstCardOfBunch.IsFaceUp && CanMoveWasteToPile(firstCardOfBunch, to))
            {

                // 1. 옮길 카드 뭉치(bunch)를 먼저 확보합니다.
                //List<ICard> bunch = _piles[from].GetRange(startIndex, count);
                List<T> bunch = new List<T>();
                for (int i = 0; i < count; i++)
                {
                    // startIndex 위치의 카드를 count만큼 순차적으로 읽어옵니다.
                    bunch.Add(_piles[from][startIndex + i]);
                }

                // 2. 원본 더미에서 해당 구간을 삭제합니다.
                // 뒤에서부터 지워야 인덱스 붕괴를 막을 수 있습니다.
                //_piles[from].RemoveRange(startIndex, count);
                for (int i = (startIndex + count - 1); i >= startIndex; i--)
                {
                    _piles[from].RemoveAt(i);
                }

                // 3. 대상 더미(to)에 확보한 뭉치를 추가합니다.
                //_piles[to].AddRange(bunch);
                for (int i = 0; i < bunch.Count; i++)
                {
                    _piles[to].Add(bunch[i]);
                }
            }
        }

        private bool CanMoveWasteToPile(T card, int toIdx)
        {
            if (toIdx < 0 || toIdx > (PileCount - 1)) return false;
            if (_piles[toIdx].Count == 0) return card.Rank == SuitCardCount; // King only

            T target = _piles[toIdx].Last();
            return target.IsFaceUp && target.Rank == card.Rank + 1 && target.GetColor() != card.GetColor();
        }

        private bool CanMovePileToFoundation(T card, int fIdx)
        {
            if (fIdx < 0 || fIdx > (FoundationCount - 1)) return false;
            if (_foundations[fIdx].Count == 0) return card.Rank == 1; // Ace only

            T target = _foundations[fIdx].Last();
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
            // FoundationCount개의 파운데이션이 각각 SuitCardCount장의 카드를 가지고 있으면 승리
            return _foundations.All(f => f.Count == SuitCardCount);
        }
    }
}
