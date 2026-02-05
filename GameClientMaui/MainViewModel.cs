using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GameClientPoco;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameClientMaui
{
    public enum State {  Unknown, MoveWasteTo, MovePileTo, MoveFoundationTo };

    public enum StackType { Unknown, Deck, Waste, Foundation, Pile };

    public partial class MainViewModel : ObservableObject,
        IRecipient<CardStackClickMessage>,
        IRecipient<CardViewModelClickMessage>
    {
        private ILogger _logger;

        private Solitaire<CardViewModel> _solitaire;

        private ObservableCollection<CardViewModel> _deck;
        private ObservableCollection<CardViewModel> _waste;
        private ObservableCollection<CardViewModel>[] _foundations = new ObservableCollection<CardViewModel>[4];
        private ObservableCollection<CardViewModel>[] _piles = new ObservableCollection<CardViewModel>[7];

        public CardStackViewModel DeckStack { get; }
        public CardStackViewModel WasteStack { get; }
        public CardStackViewModel[] FoundationStacks { get; }
        public CardStackViewModel[] PileStacks { get; }

        CardStackViewModel? SelectedStackVM = null;

        private State state = State.Unknown;

        // 메신저
        private readonly IMessenger _messenger;

        public MainViewModel(ILogger logger, IMessenger messenger)
        {
            _logger = logger;

            // 카드 콜렉션 생성
            _deck = new ObservableCollection<CardViewModel>();
            _waste = new ObservableCollection<CardViewModel>();
            for (int i = 0; i < Solitaire<CardViewModel>.FoundationCount; i++) _foundations[i] = new ObservableCollection<CardViewModel>();
            for (int i = 0; i < Solitaire<CardViewModel>.PileCount; i++) _piles[i] = new ObservableCollection<CardViewModel>();

            // 카드 콜렉션 주입
            _solitaire = new Solitaire<CardViewModel>(_logger, _deck, _waste, _foundations, _piles,
                (s, r) => new CardViewModel(s, r), 777);

            _messenger = messenger;
            _messenger.Register<CardStackClickMessage>(this);
            _messenger.Register<CardViewModelClickMessage>(this);

            // 카드 콜렉션 초기화
            //_solitaire.InitializeGame();

            DeckStack = new CardStackViewModel(_messenger, _deck, StackType.Deck, 0);
            WasteStack = new CardStackViewModel(_messenger, _waste, StackType.Waste, 0);
            FoundationStacks = new CardStackViewModel[Solitaire<CardViewModel>.FoundationCount];
            PileStacks = new CardStackViewModel[Solitaire<CardViewModel>.PileCount];
            

            // 컬랙션 생성 하기
            for (int i = 0; i < Solitaire<CardViewModel>.FoundationCount; i++)
            {
                FoundationStacks[i] = new CardStackViewModel(_messenger, _foundations[i], StackType.Foundation, i);
            }
            
            for (int i = 0; i < Solitaire<CardViewModel>.PileCount; i++)
            {
                PileStacks[i] = new CardStackViewModel(_messenger, _piles[i], StackType.Pile, i);
            }

            UpdateStack();
        }

        private void UpdateStack()
        {
            DeckStack.UpdateTick++;
            WasteStack.UpdateTick++;
            for (int i = 0; i < Solitaire<CardViewModel>.FoundationCount; i++)
            {
                FoundationStacks[i].UpdateTick++;
            }
            for (int i = 0; i < Solitaire<CardViewModel>.PileCount; i++)
            {
                PileStacks[i].UpdateTick++;
            }
        }

        public int IndexOf(CardStackViewModel[] stackVMArray, CardStackViewModel stackVM)
        {
            for (int i = 0; i < stackVMArray.Length; i++)
            {
                if (stackVMArray[i] == stackVM) 
                    return i;
            }
            return -1;
        }

        public void Receive(CardStackClickMessage message)
        {
            Trace.WriteLine($"메세지 수신 {message.GetString()} ");
            if (message.StackName == "Deck")
            {
                _solitaire.ExecuteCommand(new CardCommand { Type = CommandType.Draw, IsValid = false });
                UpdateStack();
            }
        }

        public void Receive(CardViewModelClickMessage message)
        {
            Trace.WriteLine($"메세지 수신 {message.GetString()} ");
            
            switch (state)
            {
                case State.Unknown:
                    if (message.StackVM != null && message.StackVM.Type == StackType.Deck)
                    {
                        _solitaire.ExecuteCommand(new CardCommand { Type = CommandType.Draw, IsValid = false });
                        UpdateStack();
                    }
                    else if (message.StackVM != null && message.StackVM.Type == StackType.Waste)
                    {
                        message.StackVM.IsSelected = true;
                        SelectedStackVM = message.StackVM;
                        state = State.MoveWasteTo;
                    }
                    else if (message.StackVM != null && message.StackVM.Type == StackType.Pile)
                    {
                        message.StackVM.IsSelected = true;
                        SelectedStackVM = message.StackVM;
                        state = State.MovePileTo;
                    }
                    break;

                case State.MoveWasteTo:
                    if (SelectedStackVM == null)
                    {
                        state = State.Unknown;
                        break;
                    }
                    if (message.StackVM != null && message.StackVM.Type == StackType.Foundation)
                    {
                        CardCommand command = new CardCommand
                        {
                            Type = CommandType.MoveWasteToFoundation,
                            To = message.StackVM.Index
                        };
                        if (_solitaire.ExecuteCommand(command) == true)
                        {
                            _solitaire.CheckFlipTopCards();
                            UpdateStack();
                        }
                    }
                    if (message.StackVM != null && message.StackVM.Type == StackType.Pile)
                    {
                        CardCommand command = new CardCommand 
                        {
                            Type = CommandType.MoveWasteToPile,
                            To = message.StackVM.Index
                        };
                        if (_solitaire.ExecuteCommand(command) == true)
                        {
                            _solitaire.CheckFlipTopCards();
                            UpdateStack();
                        }
                    }
                    if (SelectedStackVM != null)
                        SelectedStackVM.IsSelected = false;
                    state = State.Unknown;
                    break;

                case State.MoveFoundationTo:
                    if (SelectedStackVM == null)
                    {
                        state = State.Unknown;
                        break;
                    }
                    if (message.StackVM != null && message.StackVM.Type == StackType.Pile)
                    {
                        CardCommand command = new CardCommand
                        {
                            Type = CommandType.MoveFoundationToPile,
                            From = SelectedStackVM.Index,
                            To = message.StackVM.Index
                        };
                        if (_solitaire.ExecuteCommand(command) == true)
                        {
                            _solitaire.CheckFlipTopCards();
                            UpdateStack();
                        }
                    }
                    SelectedStackVM.IsSelected = false;
                    state = State.Unknown;
                    break;

                case State.MovePileTo:
                    if (SelectedStackVM == null)
                    {
                        state = State.Unknown;
                        break;
                    }
                    if (message.StackVM != null && message.StackVM.Type == StackType.Foundation)
                    {
                        CardCommand command = new CardCommand
                        {
                            Type = CommandType.MovePileToFoundation,
                            From = SelectedStackVM.Index,
                            To = message.StackVM.Index
                        };
                        if (_solitaire.ExecuteCommand(command) == true)
                        {
                            _solitaire.CheckFlipTopCards();
                            UpdateStack();
                        }
                    }
                    if (message.StackVM != null && message.StackVM.Type == StackType.Pile)
                    {
                        CardCommand command = new CardCommand
                        {
                            Type = CommandType.MovePileToPile,
                            From = SelectedStackVM.Index,
                            To = message.StackVM.Index,
                            Count = 1
                        };
                        if (_solitaire.ExecuteCommand(command) == true)
                        {
                            _solitaire.CheckFlipTopCards();
                            UpdateStack();
                        }
                    }
                    SelectedStackVM.IsSelected = false;
                    state = State.Unknown;
                    break;
            }
        }
    }

    public class CardCommandMessage
    {
        private CardCommand _command {get; }
        public CardCommand Command
        {
            get => _command;
        }

        public CardCommandMessage(CardCommand command)
        {
            _command = command;
        }
    }

    public class CardStackClickMessage
    {
        private string _stackName { get; set; } = "";
        public string StackName
        {
            get => _stackName;
        }

        private int _index { get; }
        public int Index
        {
            get => _index;
        }

        public CardStackClickMessage(string stackName, int index)
        {
            _stackName = stackName;
            _index = index;
        }

        public string GetString()
        {
            return $"StackName {_stackName} Index {_index}";
        }
    }

    public class CardViewModelClickMessage
    {
        public CardStackViewModel? StackVM { get; set; }
        public CardViewModel? CardVM { get; set; }

        public CardViewModelClickMessage(CardStackViewModel? stackVM, CardViewModel? cardVM)
        {
            CardVM = cardVM;
            StackVM = stackVM;
        }

        public string GetString()
        {
            string s = "";
            if (StackVM != null)
            {
                s += $"StackVM {StackVM.Type.ToString()} ";
            }
            if (CardVM != null)
            {
                s += $"{((ICard)CardVM).GetString()}";
            }
            return s;
        }
    }
}
