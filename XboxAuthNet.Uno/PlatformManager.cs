using XboxAuthNet.OAuth.CodeFlow;
using XboxAuthNet;

namespace XboxAuthNet;

public class PlatformManager
{
    private static PlatformManager? _currentPlatformInstance;
    public static PlatformManager CurrentPlatform =>
        _currentPlatformInstance ??= new PlatformManager();

    public IWebUI CreateWebUI(WebUIOptions uiOptions)
    {
        IWebUI? ui = null;

        /*if (!XboxAuthNet.Platforms.WinForm.WebView2WebUI.IsWebView2Available())
            throw new Microsoft.Web.WebView2.Core.WebView2RuntimeNotFoundException();
        ui = new XboxAuthNet.Platforms.WinForm.WebView2WebUI(uiOptions);*/


#if HAS_UNO_WINUI || WINDOWS
        if (!XboxAuthNet.Platforms.Uno.UnoWindowWebUI.IsWebView2Available())
            throw new Exception("No WebView");
        ui = new XboxAuthNet.Platforms.Uno.UnoWindowWebUI(uiOptions);
#endif

        if (ui == null)
            throw new PlatformNotSupportedException("Current platform does not support to provide default WebUI.");
        return ui;
    }
}