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
        public ObservableCollection<CardViewModel> CardVMs { get; }

        public MainViewModel()
        {        
            CardVMs = new ObservableCollection<CardViewModel>
            {
                new CardViewModel(new Card(Suit.Spades, 1)),
                new CardViewModel(new Card(Suit.Hearts, 12)),
                new CardViewModel(new Card(Suit.Diamonds, 7)),
                new CardViewModel(new Card(Suit.Clubs, 13))
            };
        }
    }
}
