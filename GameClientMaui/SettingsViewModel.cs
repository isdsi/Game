using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Storage;

namespace GameClientMaui
{
    public class SettingsViewModel : ObservableObject
    {
        public bool IsDraw1
        {
            get => Preferences.Default.Get("DrawCount", 1) == 1;
            set
            {
                if (value)
                {
                    Preferences.Default.Set("DrawCount", 1);
                    OnPropertyChanged(nameof(IsDraw1));
                    OnPropertyChanged(nameof(IsDraw3));
                }
            }
        }

        public bool IsDraw3
        {
            get => Preferences.Default.Get("DrawCount", 1) == 3;
            set
            {
                if (value)
                {
                    Preferences.Default.Set("DrawCount", 3);
                    OnPropertyChanged(nameof(IsDraw1));
                    OnPropertyChanged(nameof(IsDraw3));
                }
            }
        }
    }
}