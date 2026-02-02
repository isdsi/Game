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
        private ObservableCollection<CardViewModel> _cards = new ObservableCollection<CardViewModel>();

        [ObservableProperty]
        private long _updateTick = 1;

        public CardStackViewModel(IMessenger messenger, string name)
        {
            _messenger = messenger;
            _name = name;
        }

        [RelayCommand]
        private void CardClick(CardViewModel cardVM)
        {
            Trace.WriteLine($"카드 클릭 {cardVM.Card.ToString()}");
            int index = _cards.IndexOf(cardVM);
            _messenger.Send(new CardStackClickMessage(_name, index));
        }
    }

}
