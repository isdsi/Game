namespace GameClientMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            // 저장된 값 불러오기 (기본값은 800x600 등으로 설정)
            window.X = Preferences.Default.Get("WinX", 100.0);
            window.Y = Preferences.Default.Get("WinY", 100.0);
            window.Width = Preferences.Default.Get("WinWidth", 1000.0);
            window.Height = Preferences.Default.Get("WinHeight", 800.0);

            // 창이 닫힐 때 현재 위치/크기 저장하는 이벤트 연결
            window.Destroying += (s, e) =>
            {
                Preferences.Default.Set("WinX", window.X);
                Preferences.Default.Set("WinY", window.Y);
                Preferences.Default.Set("WinWidth", window.Width);
                Preferences.Default.Set("WinHeight", window.Height);
            };

            return window;
        }
    }
}
