﻿using CommunityToolkit.Mvvm.Messaging;
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

        this.SizeChanged += (s, e) => UpdateStack();
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

        double cardHeight = 120;
        double currentOffsetY = OffsetY;

        if (ViewModel.Type == StackType.Pile && ViewModel.Cards.Count > 1)
        {
            double totalHeight = (ViewModel.Cards.Count - 1) * OffsetY + cardHeight;
            if (this.Height > 0 && totalHeight > this.Height / 2)
            {
                currentOffsetY = (this.Height / 2 - cardHeight) / (ViewModel.Cards.Count - 1);
                if (currentOffsetY < 0) currentOffsetY = 0;
            }
        }

        // 1. 현재 카드들의 화면상 위치(부모 컨테이너 기준)를 저장합니다.
        // 스택 간 이동 시, 이전 스택에서의 절대 위치를 기억하기 위함입니다.
        foreach (var child in CardAbsoluteStack.Children)
        {
            if (child is CardView cv && cv.BindingContext is CardViewModel cvm)
            {
                // this.X/Y: 부모 내 스택 위치, cv.X/Y: 스택 내 카드 위치, Translation: 애니메이션 오프셋
                cvm.Bounds = new Rect(this.X + cv.X + cv.TranslationX, this.Y + cv.Y + cv.TranslationY, cv.Width, cv.Height);
            }
        }

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
            AbsoluteLayout.SetLayoutBounds(cardView, new Rect(0, index * currentOffsetY, 1, cardHeight));
            AbsoluteLayout.SetLayoutFlags(cardView, AbsoluteLayoutFlags.WidthProportional);

            // 목표 오프셋 계산
            double targetTranslationY = 0;

            CardAbsoluteStack.Children.Add(cardView);
            if (ViewModel.Type == StackType.Waste)
            {
                if (DrawCount == 1)
                {
                    targetTranslationY = index * OffsetY;
                }
                else if (DrawCount == 3)
                {
                    int count = ViewModel.Cards.Count;
                    if (count <= 3)
                    {
                        targetTranslationY = index * 15;
                    }
                    else
                    {
                        if (index < count - 3)
                            targetTranslationY = index * 1;
                        else
                            targetTranslationY = (count - 3) * 1 + (index - (count - 3)) * 15;
                    }
                }
            }
            else
            {
                targetTranslationY = index * currentOffsetY;                
            }

            cardView.TranslationY = targetTranslationY;

            // 2. 애니메이션: 이전 위치(Bounds)가 있다면 그곳에서부터 현재 위치로 이동
            if (cardVM.Bounds.HasValue)
            {
                Rect prev = cardVM.Bounds.Value;
                // 현재 스택 기준에서의 시작 좌표 계산 (LayoutY인 index * OffsetY를 고려해야 함)
                double startX = prev.X - this.X; 
                double startY = prev.Y - this.Y - (index * currentOffsetY);

                // 위치 차이가 있을 때만 애니메이션 실행
                if (Math.Abs(startX) > 1 || Math.Abs(startY - targetTranslationY) > 1)
                {
                    cardView.TranslationX = startX;
                    cardView.TranslationY = startY;
                    cardView.TranslateTo(0, targetTranslationY, 250, Easing.Linear);
                }
            }

            index++;
        }
    }
}