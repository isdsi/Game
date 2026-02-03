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
    public partial class CardViewModel : ObservableObject, ICard
    {
        public Suit Suit { get; }
        public int Rank { get; }
        public bool IsFaceUp { get; set; }

        public string CardString => ((ICard)this).GetString();

        public Color UiCardColor
        {
            get
            {
#if RELEASE
                return Color.Parse(((ICard)this).GetColor());
#endif
#if DEBUG
                if (Color.Parse(((ICard)this).GetColor()) == Colors.Red)
                {
                    return Color.Parse("#FFE0E0");
                }
                else
                    return Color.Parse("#E0E0E0");
#endif
            }
        }

        public CardViewModel(Suit suit, int rank)
        {
            Suit = suit;
            Rank = rank;
            IsFaceUp = false;
        }
    }
}
