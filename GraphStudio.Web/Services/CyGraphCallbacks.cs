using GraphStudio.Domain;

using Microsoft.JSInterop;

namespace GraphStudio.Web.Services;


public sealed class CyGraphCallbacks
{
    private readonly Action<IReadOnlyList<SelectedElementDto>> _onSelection;
    private readonly Action<IReadOnlyList<NodeMoveDto>> _onMoves;
    private readonly Action<IReadOnlyList<SelectedElementDto>> _onDelete;
    private readonly Action<NodePosition> _onCanvasContextMenu;

    public CyGraphCallbacks(
        Action<IReadOnlyList<SelectedElementDto>> onSelection,
        Action<IReadOnlyList<NodeMoveDto>> onMoves,
        Action<IReadOnlyList<SelectedElementDto>> onDelete,
        Action<NodePosition> onCanvasContextMenu)
    {
        _onSelection = onSelection;
        _onMoves = onMoves;
        _onDelete = onDelete;
        _onCanvasContextMenu = onCanvasContextMenu;

    }

    [JSInvokable]
    public Task OnSelectionChanged(List<SelectedElementDto> selected)
    {
        _onSelection(selected);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnNodeMovesCommitted(List<NodeMoveDto> moves)
    {
        _onMoves(moves);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnDeleteRequested(List<SelectedElementDto> selected)
    {
        _onDelete(selected);
        return Task.CompletedTask;
    }
    [JSInvokable]
    public Task OnCanvasContextMenu(NodePosition position)
    {
        _onCanvasContextMenu(position);
        return Task.CompletedTask;
    } 
    // Add others as needed: OnCanvasContextMenu, OnElementContextMenu, etc.
}

public sealed record SelectedElementDto(string Id, string Group);
public sealed record NodeMoveDto(string NodeId, double X, double Y);
