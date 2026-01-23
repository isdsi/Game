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
                new CardViewModel(Suit.Spades, 1),
                new CardViewModel(Suit.Hearts, 12),
                new CardViewModel(Suit.Diamonds, 7),
                new CardViewModel(Suit.Clubs, 13)
            };
            BindingContext = this;
        }
    }
}
