using Microsoft.UI.Xaml;

namespace XboxAuthNet.OAuth.CodeFlow;

public class WebUIOptions
{   
    public object? ParentObject { get; set; }
    public XamlRoot XamlRoot { get; set; }
    public string? Title { get; set; }
    public SynchronizationContext? SynchronizationContext { get; set; }
}
