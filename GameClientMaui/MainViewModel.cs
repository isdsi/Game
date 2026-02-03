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
    public partial class MainViewModel : ObservableObject,
        IRecipient<CardStackClickMessage>

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

            // 카드 콜렉션 초기화
            //_solitaire.InitializeGame();

            DeckStack = new CardStackViewModel(_messenger, _deck, "Deck");
            WasteStack = new CardStackViewModel(_messenger, _waste, "Waste");
            FoundationStacks = new CardStackViewModel[Solitaire<CardViewModel>.FoundationCount];
            PileStacks = new CardStackViewModel[Solitaire<CardViewModel>.PileCount];
            

            // 컬랙션 생성 하기
            for (int i = 0; i < Solitaire<CardViewModel>.FoundationCount; i++)
            {
                FoundationStacks[i] = new CardStackViewModel(_messenger, _foundations[i], $"Foundations{i}");
            }
            
            for (int i = 0; i < Solitaire<CardViewModel>.PileCount; i++)
            {
                PileStacks[i] = new CardStackViewModel(_messenger, _piles[i], $"Piles{i}");
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

        public void Receive(CardStackClickMessage message)
        {
            Trace.WriteLine($"메세지 수신 {message.ToString()} ");
            if (message.StackName == "Deck")
            {
                _solitaire.ExecuteCommand(new CardCommand { Type = CommandType.Draw, IsValid = false });
                UpdateStack();
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

        public override string ToString()
        {
            return $"StackName {_stackName} Index {_index}";
        }
    }
}
