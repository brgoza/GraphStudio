using GraphStudio.Domain.CanvasOps;
using GraphStudio.Web.Components.Canvas;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GraphStudio.Web.Services;

public sealed class CyGraphInterop : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;
    private DotNetObjectReference<CyGraphCallbacks>? _dotNetRef;
    private string? _handleId;

    public CyGraphInterop(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync(
        GraphCanvas canvas,
        ElementReference hostElement,
        CyGraphCallbacks callbacks,
        object options)
    {
        _module ??= await _js.InvokeAsync<IJSObjectReference>("import", "/js/cyGraph.js");
        _dotNetRef = DotNetObjectReference.Create(callbacks);

        _handleId = await _module.InvokeAsync<string>(
            "create", hostElement, _dotNetRef, options);
    }
    //public ValueTask ApplyOpsAsync(object ops) =>
    //    _module!.InvokeVoidAsync("applyOps", _handleId, ops);

    public async ValueTask ApplyOpsAsync(IReadOnlyList<object> ops) =>
      await   _module!.InvokeVoidAsync("applyOps", _handleId, ops);

    public ValueTask SetStyleAsync(object style) =>
        _module!.InvokeVoidAsync("setStyle", _handleId, style);

    public ValueTask RunLayoutAsync(object layoutOptions) =>
        _module!.InvokeVoidAsync("runLayout", _handleId, layoutOptions);

    public ValueTask FitAsync(int padding = 30) =>
        _module!.InvokeVoidAsync("fit", _handleId, padding);

    public ValueTask<CyViewport> GetViewportAsync() =>
        _module!.InvokeAsync<CyViewport>("getViewport", _handleId);

    public async ValueTask DisposeAsync()
    {
        if (_module is not null && _handleId is not null)
        {
            try { await _module.InvokeVoidAsync("dispose", _handleId); }
            catch { /* ignore dispose errors */ }
        }

        _dotNetRef?.Dispose();
        if (_module is not null) await _module.DisposeAsync();
    }
}
