using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XboxAuthNet.OAuth.CodeFlow;
using XboxAuthNet.Uno.Platforms.Uno;

namespace XboxAuthNet.Platforms.Uno
{
    public class UnoWindowWebUI : IWebUI
    {
        private readonly XamlRoot _xamlRoot;
        private readonly SynchronizationContext? _synchronizationContext;
        private UnoContentDialog _dialog;

        public UnoWindowWebUI(WebUIOptions options)
        {
            _xamlRoot = options.XamlRoot;
            _synchronizationContext = options.SynchronizationContext;
            _dialog = new UnoContentDialog(_xamlRoot);
        }

        public async Task<CodeFlowAuthorizationResult> DisplayDialogAndInterceptUri(Uri uri, ICodeFlowUrlChecker uriChecker, CancellationToken cancellationToken)
        {
            CodeFlowAuthorizationResult result = new CodeFlowAuthorizationResult();
            await UIThreadHelper.InvokeUIActionOnSafeThread(async () =>
            {
                result = await _dialog.DisplayDialogAndInterceptUri(uri, uriChecker, cancellationToken);
            }, _synchronizationContext, cancellationToken);

            return result;
        }

        public Task DisplayDialogAndNavigateUri(Uri uri, CancellationToken cancellationToken)
        {
            UIThreadHelper.InvokeUIActionOnSafeThread(() =>
            {
                _dialog.DisplayDialogAndNavigateUri(uri, cancellationToken);
                return null;

            }, _synchronizationContext, cancellationToken);
            return null;
        }

        public static bool IsWebView2Available()
        {
            try
            {
                string wv2Version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                return !string.IsNullOrEmpty(wv2Version);
            }
            catch (Exception ex) when (ex is BadImageFormatException || ex is DllNotFoundException)
            {
                return false;
                //throw new MsalClientException(MsalError.WebView2LoaderNotFound, MsalErrorMessage.WebView2LoaderNotFound, ex);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
