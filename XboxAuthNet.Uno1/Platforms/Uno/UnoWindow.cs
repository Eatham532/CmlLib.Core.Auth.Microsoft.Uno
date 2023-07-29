

using Windows.UI.Core;
using Microsoft.UI.Xaml;

namespace XboxAuthNet.Uno.Platforms.Uno
{
    public class UnoWindow
    {
        public bool IsActivated;
        private Window _thisWindow;

        public UnoWindow()
        {
            _thisWindow = new Window();

            _thisWindow.Activated += UnoWindow_Activated;
            _thisWindow.Closed += UnoWindow_Closed;
        }

        private void UnoWindow_Closed(object sender, CoreWindowEventArgs e)
        {
            IsActivated = false;
        }

        private void UnoWindow_Closed(object sender, Microsoft.UI.Xaml.WindowEventArgs args)
        {
            IsActivated = false;
        }

        private void UnoWindow_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            IsActivated = true;
        }

        public bool WindowOpen = false;

        public void Open()
        {
            _thisWindow.Activate();
        }

        public async void OpenAsync()
        {
            _thisWindow.Activate();


        }
    }
}
