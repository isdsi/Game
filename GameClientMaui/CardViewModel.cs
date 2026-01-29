using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameClientMaui
{
    public enum Suit { Spades, Hearts, Diamonds, Clubs }

    public class CardViewModel : INotifyPropertyChanged
    {
        public Suit Suit { get; }
        public int Rank { get; }

        private bool _isFaceUp;
        public bool IsFaceUp
        {
            get => _isFaceUp;
            set => SetProperty(ref _isFaceUp, value);
        }

        public string GetColor() => (Suit == Suit.Hearts || Suit == Suit.Diamonds) ? "Red" : "Black";

        public override string ToString()
        {
            if (!IsFaceUp) return "[??]";
            string r = Rank switch
            {
                1 => "A",
                11 => "J",
                12 => "Q",
                13 => "K",
                _ => Rank.ToString()
            };
            char s = Suit switch
            {
                Suit.Spades => '♠',
                Suit.Hearts => '♥',
                Suit.Diamonds => '♦',
                Suit.Clubs => '♣',
                _ => ' '
            };
            return $"[{r}{s}]";
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

        public ICommand FaceUpCommand { get; }

        public CardViewModel(Suit suit, int rank)
        {
            Suit = suit;
            Rank = rank;
            _isFaceUp = false;

            FaceUpCommand = new RelayCommand(FaceUp);
        }

        private void FaceUp()
        {
            IsFaceUp = !IsFaceUp;
        }
    }
}
