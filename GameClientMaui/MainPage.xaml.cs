using CommunityToolkit.Mvvm.Messaging;
using GameClientPoco;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace GameClientMaui
{
    public partial class MainPage : ContentPage
    {
        // 뷰모델을 멤버로 보유
        private readonly MainViewModel _viewModel;


        private ILoggerFactory _loggerFactory;
        private ILogger _logger;

        // IMessenger는 애플리케이션 전체에서 공유되는 싱글톤입니다.
        public static readonly IMessenger _messenger = WeakReferenceMessenger.Default;

        public MainPage()
        {
            InitializeComponent();

            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = _loggerFactory.CreateLogger("GameClient");

            // 뷰모델 생성 및 주입
            _viewModel = new MainViewModel(_logger, _messenger);

            // 이 페이지의 영혼(BindingContext)을 결정
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.RefreshSettings();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (GameCanvas == null || width <= 0 || height <= 0) return;

            // 1. 기준 해상도 (이 크기를 바탕으로 모든 카드를 배치하셨죠?)
            double targetWidth = 800;
            double targetHeight = 730;

            // 2. 창 크기에 따른 비율 계산
            double scaleX = width / targetWidth;
            double scaleY = height / targetHeight;

            // 비율 유지 (Uniform)
            double finalScale = Math.Min(scaleX, scaleY);

            // 3. 안전장치: scale이 0이 되지 않도록 방어
            if (finalScale <= 0) finalScale = 0.1;

            // 4. 스케일 적용 (중앙 기준)
            GameCanvas.Scale = finalScale;

            // 5. 위치 보정: 
            // AbsoluteLayout의 크기를 기준으로 스케일이 먹히기 때문에
            // 별도의 TranslationX/Y 없이도 Horizontal/VerticalOptions="Center"가 
            // Grid 내부에서 어느 정도 중앙을 잡아줄 것입니다.
        }
    }
}
