using GameClientPoco;

namespace GameClientMaui;
// CardStackView.xaml.cs
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

    public CardStackView()
    {
        InitializeComponent();
    }

    // 2. 컨트롤이 완전히 로드되었을 때 다시 한 번 시도
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null) UpdateStack();
    }

    private void UpdateStack()
    {
        if (CardGrid == null)
            return;
        CardGrid.Children.Clear();
        if (Cards == null) return;

        int index = 0;
        foreach (var cardVM in Cards)
        {
            // 이제 cardVM을 그대로 BindingContext에 주입하면 끝!
            var cardView = new CardView { BindingContext = cardVM };

            CardGrid.Add(cardView);
            cardView.TranslationY = index * 30;
            index++;
        }
    }
}