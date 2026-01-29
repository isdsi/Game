using System.Collections.ObjectModel;

namespace GameClientMaui
{
    public partial class MainPage : ContentPage
    {
        // 뷰모델을 멤버로 보유
        private readonly MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();

            // 뷰모델 생성 및 주입
            _viewModel = new MainViewModel();

            // 이 페이지의 영혼(BindingContext)을 결정
            BindingContext = _viewModel;
        }
    }
}
