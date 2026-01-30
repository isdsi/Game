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

namespace GameClientMaui
{
    public partial class MainViewModel : ObservableObject,
        IRecipient<CardCommandMessage>
    {
        public ObservableCollection<CardViewModel> DeckVMs { get; }
        
        public ObservableCollection<CardViewModel> WasteVMs { get; }
        
        public ObservableCollection<CardViewModel>[] FoundationVMs { get; }
        
        public ObservableCollection<CardViewModel>[] PileVMs { get; }

        private Solitaire _solitaire;

        // 메신저
        private readonly IMessenger _messenger;

        public MainViewModel(Solitaire solitaire, IMessenger messenger)
        {
            _solitaire = solitaire;
            _messenger = messenger;
            _messenger.Register<CardCommandMessage>(this);

            DeckVMs = new ObservableCollection<CardViewModel>();
            WasteVMs = new ObservableCollection<CardViewModel>();
            FoundationVMs = new ObservableCollection<CardViewModel>[_solitaire.Foundations.Length];
            PileVMs = new ObservableCollection<CardViewModel>[_solitaire.Piles.Length];

            // 컬랙션 생성 하기
            for (int i = 0; i < _solitaire.Foundations.Length; i++)
            {
                FoundationVMs[i] = new ObservableCollection<CardViewModel>();
            }
            for (int i = 0; i < _solitaire.Piles.Length; i++)
            {
                PileVMs[i] = new ObservableCollection<CardViewModel>();
            }

            // 카드 넣기
            foreach (var card in _solitaire.Deck)
            {
                DeckVMs.Add(new CardViewModel(card, _messenger));
            }
            foreach (var card in _solitaire.Waste)
            {
                WasteVMs.Add(new CardViewModel(card, _messenger));
            }
            for(int i = 0; i < _solitaire.Foundations.Length; i++)
            {
                foreach (var card in _solitaire.Foundations[i])
                {
                    FoundationVMs[i].Add(new CardViewModel(card, _messenger));
                }
            }
            for (int i = 0; i < _solitaire.Piles.Length; i++)
            {
                foreach (var card in _solitaire.Piles[i])
                {
                    PileVMs[i].Add(new CardViewModel(card, _messenger));
                }
            }
        }

        public void Receive(CardCommandMessage message)
        {
            Trace.WriteLine($"메세지 수신 {message.ToString()} ");
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

}
