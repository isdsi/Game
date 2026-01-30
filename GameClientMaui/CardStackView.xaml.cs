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
#if DEBUG
        // 디버그 모드일 때는 눈에 확 띄는 오렌지색!
        this.Resources["CardBackgroundColor"] = Colors.White;
        this.Resources["CardBorderColor"] = Color.Parse("#F8F8F8");
#endif
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
            cardView.TranslationY = index * OffsetY;
            index++;
        }
    }
}