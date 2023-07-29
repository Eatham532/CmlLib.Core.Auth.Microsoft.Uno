using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XboxAuthNet.OAuth.CodeFlow;

namespace XboxAuthNet.Uno.Platforms.Uno
{
    internal class UnoWindowWebUI : IWebUI
    {
        private readonly object? _parent;
        private readonly SynchronizationContext? _synchronizationContext;

        public UnoWindowWebUI(WebUIOptions options)
        {
            _parent = options.ParentObject;
            _synchronizationContext = options.SynchronizationContext;
        }

        public Task<CodeFlowAuthorizationResult> DisplayDialogAndInterceptUri(Uri uri, ICodeFlowUrlChecker uriChecker, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DisplayDialogAndNavigateUri(Uri uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
