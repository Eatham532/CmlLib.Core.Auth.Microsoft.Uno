using System;
using System.Threading.Tasks;
using XboxAuthNet.Game.Authenticators;
using XboxAuthNet.Game.SessionStorages;
using XboxAuthNet.OAuth;
using XboxAuthNet.OAuth.CodeFlow;

namespace XboxAuthNet.Game.OAuth;

public class MicrosoftOAuthSignout : MicrosoftOAuth
{
    private readonly Action<CodeFlowBuilder> builderInvoker;

    public MicrosoftOAuthSignout(
        MicrosoftOAuthParameters parameters,
        Action<CodeFlowBuilder> builderInvoker)
    : base(parameters)
    {
        this.builderInvoker = builderInvoker;
    }

    protected override async ValueTask<MicrosoftOAuthResponse> Authenticate(
        AuthenticateContext context, MicrosoftOAuthParameters parameters)
    {
        var apiClient = parameters.ClientInfo.CreateApiClientForOAuthCode(context.HttpClient);
        var builder = new CodeFlowBuilder(apiClient);

        if (context.UiElement != null)
        {
            builder.WithUIElement(context.UiElement);
        }

        builderInvoker.Invoke(builder);
        var codeFlow = builder.Build();   

        context.Logger.LogMicrosoftOAuthSignout();
        await codeFlow.Signout(context.CancellationToken);
        return null;
    }
}