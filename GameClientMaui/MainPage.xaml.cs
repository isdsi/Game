using GameClientPoco;
using System.Collections.ObjectModel;

namespace GameClientMaui
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<CardViewModel> Cards { get; }

        public MainPage()
        {
            InitializeComponent();

            Cards = new ObservableCollection<CardViewModel>
            {
                new CardViewModel(new Card(Suit.Spades, 1)),
                new CardViewModel(new Card(Suit.Hearts, 12)),
                new CardViewModel(new Card(Suit.Diamonds, 7)),
                new CardViewModel(new Card(Suit.Clubs, 13))
            };
            BindingContext = this;
        }
    }
}
