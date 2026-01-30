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

        private Solitaire _solitaire;

        private ILoggerFactory _loggerFactory;
        private ILogger _logger;

        // IMessenger는 애플리케이션 전체에서 공유되는 싱글톤입니다.
        public static readonly IMessenger _messenger = WeakReferenceMessenger.Default;

        public MainPage()
        {
            InitializeComponent();

            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = _loggerFactory.CreateLogger("GameClient");

            _solitaire = new Solitaire(_logger, 777);

            // 뷰모델 생성 및 주입
            _viewModel = new MainViewModel(_solitaire, _messenger);

            // 이 페이지의 영혼(BindingContext)을 결정
            BindingContext = _viewModel;
        }
    }
}
