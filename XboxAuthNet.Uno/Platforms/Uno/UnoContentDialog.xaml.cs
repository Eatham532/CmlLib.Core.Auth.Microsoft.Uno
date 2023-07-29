using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.WebUI;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.Web.WebView2.Core;
using XboxAuthNet.OAuth.CodeFlow;

namespace XboxAuthNet.Uno.Platforms.Uno
{
    public sealed partial class UnoContentDialog : ContentDialog
    {
        public bool? IsSuccessful = null;

        private const string WebView2UserDataFolder = "%UserProfile%/.msal/webview2/data";

        private ICodeFlowUrlChecker? _uriChecker;
        private CodeFlowAuthorizationResult _authCode;
        private Window _ownerWindow;

        public UnoContentDialog(XamlRoot xamlRoot)
        {
            XamlRoot = xamlRoot;
            XamlRoot.Changed += ((e, s) => AdjustWebView());

            this.InitializeComponent();
        }

        private void AdjustWebView()
        {
            var x = DialogWebView.ActualHeight;
            var y = DialogWebView.ActualWidth;
        }
        

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public void DisplayDialogAndNavigateUri(Uri uri, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Navigate to URI
            DialogWebView.Source = uri;


            ShowDialogAsync(cancellationToken);
        }

        private void CloseDialogIfOpen()
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
        }

        public async Task<CodeFlowAuthorizationResult> DisplayDialogAndInterceptUri(Uri uri, ICodeFlowUrlChecker checker, CancellationToken cancellationToken)
        {
            _uriChecker = checker;

            cancellationToken.Register(CloseDialogIfOpen);

            // Navigate to URI
            DialogWebView.Source = uri;
            DialogWebView.NavigationStarting += DialogWebViewOnNavigationStarting;
            DialogWebView.CoreWebView2Initialized += DialogWebView_CoreWebView2Initialized;

            // Show dialog
            await DisplayDialogAsync(cancellationToken);

            if (_authCode.IsEmpty)
                throw new InvalidOperationException("_authCode was empty");
            return _authCode;
        }

        private void DialogWebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            DialogWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            DialogWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            DialogWebView.CoreWebView2.Settings.AreHostObjectsAllowed = false;
            DialogWebView.CoreWebView2.Settings.IsScriptEnabled = true;
            DialogWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            DialogWebView.CoreWebView2.Settings.IsStatusBarEnabled = true;
            DialogWebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
        }

        private void DialogWebViewOnNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs e)
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
                IsSuccessful = true;
                _authCode = result;
                _uriChecker = null;
                Hide();
            }

            return !result.IsEmpty;
        }


        // Dont ask. 
        private async Task DisplayDialogAsync(CancellationToken cancellationToken)
        {
            await ShowDialogAsync(cancellationToken);

            switch (IsSuccessful)
            {
                case true:
                    break;
                case false:
                        
                case null:
                    throw new InvalidOperationException(
                        "WebView2 failed");
            }
        }

        public async Task ShowDialogAsync(CancellationToken cancellationToken)
        {
            AdjustWebView();

            await using (cancellationToken.Register(CloseDialogIfOpen))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await ShowAsync();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _authCode = new CodeFlowAuthorizationResult();
            IsSuccessful = false;
            Hide();
        }
    }
}
