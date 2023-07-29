using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XboxAuthNet.OAuth.CodeFlow;

namespace XboxAuthNet.Uno.Platforms.Uno
{
    public class UnoWebViewHandler
    {
        private const int UIWidth = 566;
        private const string WebView2UserDataFolder = "%UserProfile%/.msal/webview2/data";

        private ICodeFlowUrlChecker? _uriChecker;
        private CodeFlowAuthorizationResult _authCode;
        private UnoWindow MainWindow = new UnoWindow();
        private WebView2 MainWebView;

        

        public UnoWebViewHandler()
        {
            if (MainWindow.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Place the frame in the current Window
                MainWindow.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(UnoPageWithWebView));

                try
                {
                    MainWebView = (WebView2)((StackPanel)((UnoPageWithWebView)rootFrame.Content).Content).Children[0];
                }
                catch
                {
                    throw new Exception("Page is messed up");
                }
            }
            else
            {
                if (MainWebView != null)
                {
                    MainWebView = new WebView2();
                }
            }

            MainWebView.CoreWebView2Initialized += WebView2Control_CoreWebView2InitializationCompleted;
            MainWebView.NavigationStarting += WebView2Control_NavigationStarting;
        }

        public CodeFlowAuthorizationResult DisplayDialogAndInterceptUri(
        Uri uri, ICodeFlowUrlChecker uriChecker, CancellationToken cancellationToken)
        {
            this._uriChecker = uriChecker;

            // Starts the navigation
            MainWebView.Source = uri;
            DisplayDialog(cancellationToken);

            if (_authCode.IsEmpty)
                throw new InvalidOperationException("_authCode was empty");
            return _authCode;
        }

        public void DisplayDialogAndNavigateUri(Uri uri, CancellationToken cancellationToken)
        {
            // Starts the navigation
            MainWebView.Source = uri;

            using (cancellationToken.Register(CloseIfOpen))
            {
                MainWindow.Open();
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        

        private void CloseIfOpen()
        {
            if (MainWindow.IsActivated)
            {
                MainWindow.Close();
            }
        }


        private void WebView2Control_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (CheckForEndUrl(new Uri(e.Uri)))
            {
                // _logger.Verbose("[WebView2Control] Redirect URI reached. Stopping the interactive view");
                e.Cancel = true;
            }
            else
            {
                // _logger.Verbose("[WebView2Control] Navigating to " + e.Uri);
            }
        }

        private bool CheckForEndUrl(Uri url)
        {
            if (_uriChecker == null)
                throw new InvalidOperationException("_uriChecker was null");

            var result = _uriChecker.GetAuthCodeResult(url);

            if (!result.IsEmpty)
            {
                // This should close the dialog

                _authCode = result;
                _uriChecker = null;
            }

            return !result.IsEmpty;
        }

        private void DisplayDialog(CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(CloseIfOpen))
            {
                MainWindow.Open();
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private void WebView2Control_CoreWebView2InitializationCompleted(WebView2 sender, CoreWebView2InitializedEventArgs e)
        {
            //_logger.Verbose("[WebView2Control] CoreWebView2InitializationCompleted ");
            MainWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            MainWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            MainWebView.CoreWebView2.Settings.AreHostObjectsAllowed = false;
            MainWebView.CoreWebView2.Settings.IsScriptEnabled = true;
            MainWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            MainWebView.CoreWebView2.Settings.IsStatusBarEnabled = true;
            MainWebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;

            MainWebView.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            MainWindow.Title = MainWebView.CoreWebView2.DocumentTitle ?? "";
        }
    }
}
