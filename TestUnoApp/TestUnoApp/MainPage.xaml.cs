using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using XboxAuthNet.OAuth;
using XboxAuthNet.OAuth.CodeFlow;
using XboxAuthNet.Platforms.Uno;
using XboxAuthNet.Uno.Platforms.Uno;

namespace TestUnoApp
{
    public sealed partial class MainPage : Page
    {
        private JELoginHandler? _loginHandler;

        public MainPage()
        {
            this.InitializeComponent();

            _loginHandler = JELoginHandlerBuilder.BuildDefault();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Start((Button)sender);
            }
            else
            {
                throw new Exception("WTF");
            }
        }

        private async Task Start(Button button)
        {
            MSession session;

            session = await _loginHandler.Authenticate(button ,CancellationToken.None);

            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            var path = new MinecraftPath();
            var launcher = new CMLauncher(path);

            launcher.FileChanged += (e) =>
            {
                System.Diagnostics.Debug.WriteLine("FileKind: " + e.FileKind.ToString());
                System.Diagnostics.Debug.WriteLine("FileName: " + e.FileName);
                System.Diagnostics.Debug.WriteLine("ProgressedFileCount: " + e.ProgressedFileCount);
                System.Diagnostics.Debug.WriteLine("TotalFileCount: " + e.TotalFileCount);
            };
            launcher.ProgressChanged += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("{0}%", e.ProgressPercentage);
            };

            var versions = await launcher.GetAllVersionsAsync();
            foreach (var v in versions)
            {
                System.Diagnostics.Debug.WriteLine(v.Name);
            }

            var process = await launcher.CreateProcessAsync("1.16.5", new MLaunchOption
            {
                MaximumRamMb = 2048,
                Session = session,
                VersionType = session.Username,
            });

            process.Start();

        }

        private void Logout_OnClick(object sender, RoutedEventArgs e)
        {
            _loginHandler.Signout();
        }
    }
}