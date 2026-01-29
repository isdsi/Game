using System;
using Microsoft.Maui.Controls;

namespace GameClientMaui
{
    public partial class CardView : ContentView
    {
        public CardView()
        {
            InitializeComponent();
        }

        // Convenience strongly-typed accessor for the view model in code-behind.
        public CardViewModel? ViewModel
        {
            get => BindingContext as CardViewModel;
            set => BindingContext = value;
        }
    }
}