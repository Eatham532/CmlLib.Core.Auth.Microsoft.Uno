using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using XboxAuthNet.Game.SessionStorages;

namespace XboxAuthNet.Game.Authenticators;

public class AuthenticateContext
{
    public AuthenticateContext(
        ISessionStorage sessionStorage, 
        HttpClient httpClient,
        CancellationToken cancellationToken,
        ILogger logger) =>
        (CancellationToken, SessionStorage, HttpClient, Logger) = 
        (cancellationToken, sessionStorage, httpClient, logger);

    public AuthenticateContext(
        ISessionStorage sessionStorage,
        HttpClient httpClient,
        CancellationToken cancellationToken,
        ILogger logger, UIElement uiElement) =>
        (CancellationToken, SessionStorage, HttpClient, Logger, UiElement) =
        (cancellationToken, sessionStorage, httpClient, logger, uiElement);

    public CancellationToken CancellationToken { get; }
    public ISessionStorage SessionStorage { get; }
    public HttpClient HttpClient { get; }
    public ILogger Logger { get; }

    public UIElement? UiElement = null;
}