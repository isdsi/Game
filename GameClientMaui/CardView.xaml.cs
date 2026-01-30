using System;
using Microsoft.Maui.Controls;

namespace GameClientMaui
{
    public partial class CardView : ContentView
    {
        public CardView()
        {
            InitializeComponent();

#if DEBUG
            // 디버그 모드일 때는 눈에 확 띄는 오렌지색!
            this.Resources["CardBackgroundColor"] = Colors.White;
            this.Resources["CardBorderColor"] = Color.Parse("#F8F8F8");
            this.Resources["CardUpBackgroundColor"] = Colors.White;
            this.Resources["CardDownBackgroundColor"] = Color.Parse("#F8F8F8");
            this.Resources["CardLabelTextColor"] = Color.Parse("#E0E0E0");
            this.Resources["CardLabelTextColorHeartsDiamonds"] = Color.Parse("#FFE0E0");
#endif
        }

        // Convenience strongly-typed accessor for the view model in code-behind.
        public CardViewModel? ViewModel
        {
            get => BindingContext as CardViewModel;
            set => BindingContext = value;
        }
    }
}