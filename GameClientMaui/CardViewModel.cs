using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GameClientPoco;

namespace GameClientMaui
{
    public class CardViewModel : INotifyPropertyChanged
    {
        private Card _card;

        public Card Card => _card;

        public Suit Suit => _card.Suit;

        public int Rank => _card.Rank;

        public string CardString => _card.ToString();

        public bool IsFaceUp
        {
            get => _card.IsFaceUp;
            set
            {
                if (_card.IsFaceUp != value)
                {
                    _card.IsFaceUp = value; // 엔진의 값을 바꾸고
                    OnPropertyChanged();      // UI에 알림
                    OnPropertyChanged(nameof(CardString));
                }
            }
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


        public CardViewModel(Card card)
        {
            _card = card;

            FaceUpCommand = new RelayCommand(FaceUp);
        }

        private void FaceUp()
        {
            IsFaceUp = !IsFaceUp;
        }
    }
}
