using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GameClientPoco;
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
        public CardStackViewModel Deck { get; }
        public CardStackViewModel Waste { get; }
        public CardStackViewModel[] Foundations { get; }
        public CardStackViewModel[] Piles { get; }

        private Solitaire _solitaire;

        // 메신저
        private readonly IMessenger _messenger;

        public MainViewModel(Solitaire solitaire, IMessenger messenger)
        {
            _solitaire = solitaire;
            _messenger = messenger;
            _messenger.Register<CardStackClickMessage>(this);

            Deck = new CardStackViewModel(_messenger, "Deck");
            Waste = new CardStackViewModel(_messenger, "Waste");
            Foundations = new CardStackViewModel[_solitaire.Foundations.Length];
            Piles = new CardStackViewModel[_solitaire.Piles.Length];
            

            // 컬랙션 생성 하기
            for (int i = 0; i < _solitaire.Foundations.Length; i++)
            {
                Foundations[i] = new CardStackViewModel(_messenger, $"Foundations{i}");
            }
            
            for (int i = 0; i < _solitaire.Piles.Length; i++)
            {
                Piles[i] = new CardStackViewModel(_messenger, $"Piles{i}");
            }

            UpdateStack();
        }

        private void UpdateStack()
        {
            // 카드 제거
            Deck.Cards.Clear();
            Waste.Cards.Clear();
            for (int i = 0; i < _solitaire.Foundations.Length; i++)
            {
                Foundations[i].Cards.Clear();
            }
            for (int i = 0; i < _solitaire.Piles.Length; i++)
            {
                Piles[i].Cards.Clear();
            }

            // 카드 넣기
            foreach (var card in _solitaire.Deck)
            {
                Deck.Cards.Add(new CardViewModel(card, _messenger));
            }
            Deck.UpdateTick++;

            foreach (var card in _solitaire.Waste)
            {
                Waste.Cards.Add(new CardViewModel(card, _messenger));
            }
            Waste.UpdateTick++;

            for (int i = 0; i < _solitaire.Foundations.Length; i++)
            {
                foreach (var card in _solitaire.Foundations[i])
                {
                    Foundations[i].Cards.Add(new CardViewModel(card, _messenger));
                }
                Foundations[i].UpdateTick++;
            }
            for (int i = 0; i < _solitaire.Piles.Length; i++)
            {
                foreach (var card in _solitaire.Piles[i])
                {
                    Piles[i].Cards.Add(new CardViewModel(card, _messenger));
                }
                Piles[i].UpdateTick++;
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
