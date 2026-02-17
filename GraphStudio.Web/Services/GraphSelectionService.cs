using System;

using GraphStudio.Domain.CanvasOps;

namespace GraphStudio.Web.Services;

public sealed class GraphSelectionService
{
    public event Action? Changed;

    //public string? SelectedElementId { get; private set; } // nodeId or edgeId (string)
    //public string? SelectedGroup { get; private set; }     // "nodes" or "edges"
    public List<SelectedElement> SelectedElements { get; private set; } = [];
    
    public void SetSelection(List<SelectedElementDto> selectedElements)
    {
        SelectedElements = selectedElements.Select(dto => new SelectedElement(dto.Id, dto.Group)).ToList();
        Changed?.Invoke();
    }

    public void Clear() => SetSelection([]);
}
