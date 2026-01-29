using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using GameClientPoco;

namespace GameClientMaui
{

    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<CardViewModel> DeckVMs { get; }
        
        public ObservableCollection<CardViewModel> WasteVMs { get; }
        
        public ObservableCollection<CardViewModel>[] FoundationVMs { get; }
        
        public ObservableCollection<CardViewModel>[] PileVMs { get; }

        private Solitaire _solitaire;

        public MainViewModel(Solitaire solitaire)
        {
            _solitaire = solitaire;
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
                DeckVMs.Add(new CardViewModel(card));
            }
            foreach (var card in _solitaire.Waste)
            {
                WasteVMs.Add(new CardViewModel(card));
            }
            for(int i = 0; i < _solitaire.Foundations.Length; i++)
            {
                foreach (var card in _solitaire.Foundations[i])
                {
                    FoundationVMs[i].Add(new CardViewModel(card));
                }
            }
            for (int i = 0; i < _solitaire.Piles.Length; i++)
            {
                foreach (var card in _solitaire.Piles[i])
                {
                    PileVMs[i].Add(new CardViewModel(card));
                }
            }
        }
    }
}
