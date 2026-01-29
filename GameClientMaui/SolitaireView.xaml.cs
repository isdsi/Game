using System;
using Microsoft.Maui.Controls;

namespace GameClientMaui
{
    public partial class SolitaireView : ContentPage
    {
        public SolitaireView()
        {
            InitializeComponent();

            // If your app uses DI, replace this with the injected view model.
            BindingContext = new SolitaireGameViewModel();
        }
/*
        // Strongly-typed accessor for convenience in code-behind (if needed).
        public SolitaireGameViewModel ViewModel
        {
            get => BindingContext as SolitaireGameViewModel ?? throw new InvalidOperationException("BindingContext is not a SolitaireGameViewModel");
            set => BindingContext = value;
        }*/
    }
}