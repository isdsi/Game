using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace GameClientMaui
{
    public class SolitaireGameViewModel : INotifyPropertyChanged
    {
        private List<CardViewModel> deck = new List<CardViewModel>();
        private ObservableCollection<CardViewModel> waste = new ObservableCollection<CardViewModel>();
        private List<CardViewModel>[] foundations = new List<CardViewModel>[4];
        private ObservableCollection<CardViewModel>[] piles = new ObservableCollection<CardViewModel>[7];

        // 시스템 로그
        private ILogger? _logger;
        private readonly int _seed;

        // Observable Properties for UI Binding
        private ObservableCollection<CardViewModel> _wasteDisplay;
        public ObservableCollection<CardViewModel> WasteDisplay
        {
            get => _wasteDisplay;
            set => SetProperty(ref _wasteDisplay, value);
        }

        private ObservableCollection<CardViewModel>[] _foundationDisplay;
        public ObservableCollection<CardViewModel>[] FoundationDisplay
        {
            get => _foundationDisplay;
            set => SetProperty(ref _foundationDisplay, value);
        }

        private ObservableCollection<CardViewModel>[] _pilesDisplay;
        public ObservableCollection<CardViewModel>[] PilesDisplay
        {
            get => _pilesDisplay;
            set => SetProperty(ref _pilesDisplay, value);
        }

        private int _deckCount;
        public int DeckCount
        {
            get => _deckCount;
            set => SetProperty(ref _deckCount, value);
        }

        private bool _isGameWon;
        public bool IsGameWon
        {
            get => _isGameWon;
            set => SetProperty(ref _isGameWon, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // Commands
        public ICommand DrawCommand { get; }
        public ICommand MoveWasteToPileCommand { get; }
        public ICommand MoveToPileCommand { get; }
        public ICommand MoveToFoundationCommand { get; }
        public ICommand MoveWasteToFoundationCommand { get; }
        public ICommand NewGameCommand { get; }

        public SolitaireGameViewModel(ILogger? logger = null, int seed = 777)
        {
            _logger = logger;
            _seed = seed;

            // Initialize Observable Collections
            for (int i = 0; i < 4; i++) foundations[i] = new List<CardViewModel>();
            for (int i = 0; i < 7; i++) piles[i] = new ObservableCollection<CardViewModel>();

            _wasteDisplay = new ObservableCollection<CardViewModel>();
            _foundationDisplay = new ObservableCollection<CardViewModel>[4];
            for (int i = 0; i < 4; i++) _foundationDisplay[i] = new ObservableCollection<CardViewModel>();

            _pilesDisplay = new ObservableCollection<CardViewModel>[7];
            for (int i = 0; i < 7; i++) _pilesDisplay[i] = new ObservableCollection<CardViewModel>();

            // Initialize Commands
            DrawCommand = new RelayCommand(HandleDraw);
            MoveWasteToPileCommand = new RelayCommand<int>(pile => MoveWasteToPile(pile));
            MoveToPileCommand = new RelayCommand<(int from, int to, int count)>(args => MoveCards(args.from, args.to, args.count));
            MoveToFoundationCommand = new RelayCommand<(int from, int to)>(args => MoveCardToFoundation(args.from, args.to));
            MoveWasteToFoundationCommand = new RelayCommand(MoveWasteToFoundation);
            NewGameCommand = new RelayCommand(InitializeGame);

            _statusMessage = "";
            InitializeGame();
        }

        private void InitializeGame()
        {
            deck.Clear();
            waste.Clear();
            for (int i = 0; i < 4; i++)
            {
                foundations[i].Clear();
                _foundationDisplay[i].Clear();
            }
            for (int i = 0; i < 7; i++)
            {
                piles[i].Clear();
                _pilesDisplay[i].Clear();
            }

            foreach (Suit s in Enum.GetValues(typeof(Suit)))
            {
                for (int r = 1; r <= 13; r++) deck.Add(new CardViewModel(s, r));
            }

            Random rnd = new Random(_seed);
            deck = deck.OrderBy(x => rnd.Next()).ToList();

            for (int i = 0; i < 4; i++) foundations[i] = new List<CardViewModel>();

            for (int i = 0; i < 7; i++)
            {
                piles[i] = new ObservableCollection<CardViewModel>();
                for (int j = 0; j <= i; j++)
                {
                    CardViewModel c = deck[0];
                    deck.RemoveAt(0);
                    if (j == i) c.IsFaceUp = true;
                    piles[i].Add(c);
                    _pilesDisplay[i].Add(c);
                }
            }

            UpdateDisplayCollections();
            IsGameWon = false;
            StatusMessage = "게임이 시작되었습니다!";
        }

        public void MoveWasteToPile(int to)
        {
            if (waste.Count > 0 && to >= 0 && to <= 6)
            {
                if (CanMoveToPile(waste.Last(), to))
                {
                    CardViewModel card = waste.Last();
                    piles[to].Add(card);
                    _pilesDisplay[to].Add(card);
                    waste.RemoveAt(waste.Count - 1);
                    _wasteDisplay.RemoveAt(_wasteDisplay.Count - 1);
                    StatusMessage = $"쓰레기통에서 더미 {to + 1}로 이동했습니다.";
                }
                else
                {
                    StatusMessage = "이동할 수 없습니다!";
                }
            }
        }

        public void MoveCards(int from, int to, int count)
        {
            if (from < 0 || from > 6 || to < 0 || to > 6) return;
            if (piles[from].Count < count) return;

            int startIndex = piles[from].Count - count;
            CardViewModel firstCardOfBunch = piles[from][startIndex];

            if (firstCardOfBunch.IsFaceUp && CanMoveToPile(firstCardOfBunch, to))
            {
                // Use LINQ instead of GetRange
                List<CardViewModel> bunch = piles[from].Skip(startIndex).Take(count).ToList();

                // Remove from source pile
                for (int i = 0; i < count; i++)
                {
                    piles[from].RemoveAt(piles[from].Count - 1);
                    _pilesDisplay[from].RemoveAt(_pilesDisplay[from].Count - 1);
                }

                // Add to destination pile
                foreach (var card in bunch)
                {
                    piles[to].Add(card);
                    _pilesDisplay[to].Add(card);
                }
                StatusMessage = $"더미 {from + 1}에서 {to + 1}로 {count}장 이동했습니다.";
            }
            else
            {
                StatusMessage = "이동할 수 없습니다!";
            }
        }

        public void MoveCardToFoundation(int from, int to)
        {
            if (from < 0 || from > 6 || to < 0 || to > 3) return;
            if (piles[from].Count > 0 && CanMoveToFoundation(piles[from].Last(), to))
            {
                CardViewModel card = piles[from].Last();
                foundations[to].Add(card);
                _foundationDisplay[to].Add(card);
                piles[from].RemoveAt(piles[from].Count - 1);
                _pilesDisplay[from].RemoveAt(_pilesDisplay[from].Count - 1);
                CheckFlipTopCards();
                CheckWinCondition();
                StatusMessage = $"더미 {from + 1}에서 파운데이션 {to + 1}로 이동했습니다.";
            }
            else
            {
                StatusMessage = "이동할 수 없습니다!";
            }
        }

        public void MoveWasteToFoundation()
        {
            if (waste.Count > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (CanMoveToFoundation(waste.Last(), i))
                    {
                        CardViewModel card = waste.Last();
                        foundations[i].Add(card);
                        _foundationDisplay[i].Add(card);
                        waste.RemoveAt(waste.Count - 1);
                        _wasteDisplay.RemoveAt(_wasteDisplay.Count - 1);
                        CheckWinCondition();
                        StatusMessage = $"쓰레기통에서 파운데이션 {i + 1}로 이동했습니다.";
                        return;
                    }
                }
                StatusMessage = "쓰레기통의 카드를 이동할 수 없습니다!";
            }
        }

        private void HandleDraw()
        {
            if (deck.Count == 0)
            {
                deck.AddRange(waste.ToList());
                waste.Clear();
                _wasteDisplay.Clear();
                deck.ForEach(c => c.IsFaceUp = false);
                deck.Reverse();
                StatusMessage = "덱을 리셋했습니다.";
            }
            else
            {
                CardViewModel c = deck[0];
                deck.RemoveAt(0);
                c.IsFaceUp = true;
                waste.Add(c);
                _wasteDisplay.Add(c);
                StatusMessage = "카드를 뽑았습니다.";
            }
            DeckCount = deck.Count;
        }

        private bool CanMoveToPile(CardViewModel card, int toIdx)
        {
            if (toIdx < 0 || toIdx > 6) return false;
            if (piles[toIdx].Count == 0) return card.Rank == 13; // King only

            CardViewModel target = piles[toIdx].Last();
            return target.IsFaceUp && target.Rank == card.Rank + 1 && target.GetColor() != card.GetColor();
        }

        private bool CanMoveToFoundation(CardViewModel card, int fIdx)
        {
            if (fIdx < 0 || fIdx > 3) return false;
            if (foundations[fIdx].Count == 0) return card.Rank == 1; // Ace only

            CardViewModel target = foundations[fIdx].Last();
            return target.Suit == card.Suit && target.Rank == card.Rank - 1;
        }

        public void CheckFlipTopCards()
        {
            for (int i = 0; i < 7; i++)
            {
                if (piles[i].Count > 0 && !piles[i].Last().IsFaceUp)
                {
                    piles[i].Last().IsFaceUp = true;
                }
            }
        }

        private void CheckWinCondition()
        {
            if (IsGameWonCheck())
            {
                IsGameWon = true;
                StatusMessage = "축하합니다! 모든 카드를 맞추어 승리하셨습니다! 🎉";
            }
        }

        private bool IsGameWonCheck()
        {
            return foundations.All(f => f.Count == 13);
        }

        private void UpdateDisplayCollections()
        {
            DeckCount = deck.Count;
            _wasteDisplay.Clear();
            if (waste.Count > 0)
            {
                _wasteDisplay.Add(waste.Last());
            }
        }

        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple RelayCommand implementation for MAUI
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute?.Invoke();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is T typedParameter)
                return _canExecute?.Invoke(typedParameter) ?? true;
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is T typedParameter)
                _execute?.Invoke(typedParameter);
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
