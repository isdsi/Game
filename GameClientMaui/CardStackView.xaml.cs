using GameClientPoco;

namespace GameClientMaui;

public partial class CardStackView : ContentView
{
    public static readonly BindableProperty CardsProperty =
        BindableProperty.Create(nameof(Cards), typeof(IEnumerable<CardViewModel>), typeof(CardStackView),
            propertyChanged: (bindable, oldVal, newVal) => ((CardStackView)bindable).UpdateStack());

    // Card -> CardViewModel로 변경!
    public IEnumerable<CardViewModel> Cards
    {
        get => (IEnumerable<CardViewModel>)GetValue(CardsProperty);
        set => SetValue(CardsProperty, value);
    }

    public static readonly BindableProperty OffsetYProperty =
    BindableProperty.Create(nameof(OffsetY), typeof(int), typeof(CardStackView),
        defaultValue: 30,
        propertyChanged: (bindable, oldVal, newVal) => ((CardStackView)bindable).UpdateStack());

    public int OffsetY
    {
        get => (int)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public CardStackView()
	{
		InitializeComponent();
	}

    // Convenience strongly-typed accessor for the view model in code-behind.
    public CardStackViewModel? ViewModel
    {
        get => BindingContext as CardStackViewModel;
        set => BindingContext = value;
    }
        
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        // 뷰의 바인딩 컨텍스트가 변경되는 시점에 Cards 속성을 뷰모델의 것과 바인딩한다.
        if (BindingContext is CardStackViewModel vm)
        {
            // 뷰의 BindableProperty와 뷰모델의 속성을 연결합니다.
            // MyStackColorProperty: View에 정의된 BindableProperty
            // nameof(vm.ThemeColor): ViewModel에 정의된 속성 이름
            this.SetBinding(CardStackView.CardsProperty, nameof(vm.Cards));
        }
    }

    private void UpdateStack()
    {
        if (CardGrid == null)
            return;
        if (ViewModel == null)
            return;
        CardGrid.Children.Clear();
        if (ViewModel.Cards == null) return;

        int index = 0;
        foreach (var cardVM in ViewModel.Cards)
        {
            // 이제 cardVM을 그대로 BindingContext에 주입하면 끝!
            var cardView = new CardView { BindingContext = cardVM };

            CardGrid.Add(cardView);
            cardView.TranslationY = index * 30;
            index++;
        }
    }
}