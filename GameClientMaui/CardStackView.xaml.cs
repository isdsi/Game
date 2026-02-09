using CommunityToolkit.Mvvm.Messaging;
using GameClientPoco;
using Microsoft.Maui.Layouts;

namespace GameClientMaui;

public partial class CardStackView : ContentView
{
    public static readonly BindableProperty CardsProperty =
        BindableProperty.Create(nameof(Cards), typeof(IEnumerable<CardViewModel>), typeof(CardStackView),
            propertyChanged: (bindable, oldVal, newVal) => ((CardStackView)bindable).UpdateStack());

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

    public static readonly BindableProperty UpdateTickProperty =
    BindableProperty.Create(nameof(UpdateTick), typeof(long), typeof(CardStackView), 0L,
        propertyChanged: (bindable, oldVal, newVal) =>
        {
            if (bindable is CardStackView view)
            {
                // UI 스레드에서 안전하게 실행
                MainThread.BeginInvokeOnMainThread(() => view.UpdateStack());
            }
        });

    public long UpdateTick
    {
        get => (long)GetValue(UpdateTickProperty);
        set => SetValue(UpdateTickProperty, value);
    }

    public static readonly BindableProperty DrawCountProperty =
        BindableProperty.Create(nameof(DrawCount), typeof(int), typeof(CardStackView), 1,
            propertyChanged: (bindable, oldVal, newVal) => ((CardStackView)bindable).UpdateStack());

    public int DrawCount
    {
        get => (int)GetValue(DrawCountProperty);
        set => SetValue(DrawCountProperty, value);
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
            this.SetBinding(CardStackView.DrawCountProperty, nameof(vm.DrawCount));
        }
    }

    private void UpdateStack()
    {
        if (ViewModel == null)
            return;
        if (CardAbsoluteStack == null || ViewModel?.Cards == null)
            return;

        CardAbsoluteStack.Children.Clear();

        // 배경용 보더는 유지하고 싶다면 Clear 후에 다시 넣거나, 
        // 보더를 제외한 카드들만 관리하는 로직이 필요합니다.
        // 여기서는 단순화를 위해 카드들만 추가하는 예시입니다.

        int index = 0;
        foreach (var cardVM in ViewModel.Cards)
        {
            // 이제 cardVM을 그대로 BindingContext에 주입하면 끝!
            var cardView = new CardView { BindingContext = cardVM };

            // [핵심] 각 카드의 위치를 절대 좌표로 지정
            // X: 0, Y: index * OffsetY, Width: 부모너비(100%), Height: 카드높이(예: 120)
            AbsoluteLayout.SetLayoutBounds(cardView, new Rect(0, index * OffsetY, 1, 120));
            AbsoluteLayout.SetLayoutFlags(cardView, AbsoluteLayoutFlags.WidthProportional);

            CardAbsoluteStack.Children.Add(cardView);
            if (ViewModel.Type == StackType.Waste)
            {
                if (DrawCount == 1)
                {
                    cardView.TranslationY = index * OffsetY;
                }
                else if (DrawCount == 3)
                {
                    int count = ViewModel.Cards.Count;
                    if (count <= 3)
                    {
                        cardView.TranslationY = index * 30;
                    }
                    else
                    {
                        if (index < count - 3)
                            cardView.TranslationY = index * 1;
                        else
                            cardView.TranslationY = (count - 3) * 1 + (index - (count - 3)) * 30;
                    }
                }
            }
            else
            {
                cardView.TranslationY = index * OffsetY;
            }
            index++;
        }
    }
}