using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GameClientPoco;
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
    public partial class CardViewModel : ObservableObject
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
                    OnPropertyChanged(nameof(CardColor));
                }
            }
        }

        public Color CardColor
        {
            get
            {
#if RELEASE
                return Color.Parse(_card.GetColor();
#endif
#if DEBUG
                if (Color.Parse(_card.GetColor()) == Colors.Red)
                {
                    return Color.Parse("#FFE0E0");
                }
                else
                    return Color.Parse("#E0E0E0");
#endif
            }
        }

        // 메신저
        private readonly IMessenger _messenger;

        public CardViewModel(Card card, IMessenger messenger)
        {
            _card = card;
            _messenger = messenger;
        }

        [RelayCommand(CanExecute = nameof(CanFaceUp))]
        private void Click()
        {
            IsFaceUp = !IsFaceUp;
            _messenger.Send(new CardCommandMessage(
                new CardCommand{ Type = CommandType.Draw }) 
                );
        }

        private bool CanFaceUp()
        {
            return true;
        }
    }
}
