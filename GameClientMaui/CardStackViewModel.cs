using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class CardStackViewModel : ObservableObject
    {
        // 메신저
        private readonly IMessenger _messenger;

        [ObservableProperty]
        private string _name = "";

        [ObservableProperty]
        private ObservableCollection<CardViewModel> _cards;

        [ObservableProperty]
        private long _updateTick = 1;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value; // 엔진의 값을 바꾸고
                    OnPropertyChanged();      // UI에 알림
                    OnPropertyChanged(nameof(BackgroundColor));
                    OnPropertyChanged(nameof(BorderColor));
                }
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return Color.Parse("#FFFFFF");
            }
        }

        public Color BorderColor
        {
            get
            {
                if (_isSelected)
                    return Color.Parse("#E0FFE0");
                else
                    return Color.Parse("#E0E0E0");
            }
        }

        public CardStackViewModel(IMessenger messenger, ObservableCollection<CardViewModel> cards, string name)
        {
            _messenger = messenger;
            _cards = cards;
            _name = name;
        }

        [RelayCommand]
        private void CardClick(CardViewModel cardVM)
        {
            Trace.WriteLine($"카드 클릭 {((ICard)cardVM).GetString()}");
            int index = _cards.IndexOf(cardVM);
            //_messenger.Send(new CardStackClickMessage(_name, index));
            _messenger.Send(new CardViewModelClickMessage(this, cardVM));
        }

        [RelayCommand]
        private void Click(CardStackViewModel cardStackVM)
        {
            Trace.WriteLine($"스택 클릭 {cardStackVM.ToString()}");
            //_messenger.Send(new CardStackClickMessage(_name, -1));
            _messenger.Send(new CardViewModelClickMessage(this, null));
        }
    }

}
