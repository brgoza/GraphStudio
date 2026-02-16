using System.Text.Json;

using GraphStudio.Domain;
using GraphStudio.Domain.CanvasOps;
using GraphStudio.Domain.SchemaOps;
using GraphStudio.Web.Components.Canvas;

using Microsoft.IdentityModel.Tokens;

namespace GraphStudio.Web.Services;

public sealed class GraphInteractionService
{
    private readonly GraphSelectionState _selection;
    private GraphDocument _doc = GraphDefaults.GetDefaultDocument();
    private GraphCanvas? _canvas;
    private SchemaDefinition? _schema;

    public event Action? Changed;

    public SchemaDefinition? TryGetSchema() => _doc.Schema;
    public GraphInteractionService(GraphSelectionState selection)
    {
        _selection = selection;
    }
    public Guid CurrentDocumentId { get; private set; }
    public List<string> SelectedNodeIds { get; } = new();
    public List<string> SelectedEdgeIds { get; } = new();

    public async Task SubmitSchemaOpsAsync(IReadOnlyList<SchemaOp> ops)
    {
        // Apply to domain (document)
        var schema = _doc.Schema;
        foreach (var op in ops)
            schema = SchemaOpApplier.Apply(schema, op);

        _doc = _doc with { Schema = schema };

        Changed?.Invoke();

        // Optional: regenerate canvas styles if schema changes affect styling
        await RefreshCanvasStylesFromSchemaAsync();
    }
    private async Task RefreshCanvasStylesFromSchemaAsync()
    {
        if (_canvas is null)
            return;

        var style = CytoscapeStyleBuilder.FromSchema(_doc.Schema);
        await _canvas.SetStyleAsync(style);
    }

    public FileInfo? ExportCurrentDocumentToJsonFile(string? filename = null)
    {
        if (_doc is null) return null;
        filename ??= $"graph-{CurrentDocumentId}.json";
        return _doc.SaveDocToJsonFile(filename);
    }
    public Task AttachAsync(Guid documentId, GraphCanvas canvas)
    {
        CurrentDocumentId = documentId;
        _canvas = canvas;
        return Task.CompletedTask;
    }

    public async Task SubmitLocalOpsAsync(IReadOnlyList<CanvasOp> ops)
    {
        // 1) apply to domain
        foreach (var op in ops)
            _doc = CanvasOpApplier.Apply(_doc, op);

        // 2) apply to canvas
        if (_canvas is not null)
            await _canvas.ApplyOpsFromUiAsync(ops.ToArray());
    }

    public async Task DeleteSelectionAsync()
    {
        if (_selection.SelectedElements.IsNullOrEmpty()) return;

        var op = new DeleteSelectedOp(_selection.SelectedElements);

        await SubmitLocalOpsAsync([op]);
        _selection.Clear();
    }

    public Task FitAsync() => _canvas?.FitAsync() ?? Task.CompletedTask;

    public DomainNodeSnapshot? TryGetNode(string nodeId)
    {
        if (!Guid.TryParse(nodeId, out var id))
            return null;

        if (!_doc.Nodes.TryGetValue(id, out var node))
            return null;

        return new DomainNodeSnapshot(
            NodeId: node.Id.ToString(),
            Type: node.NodeType.Name,
            Label: node.Label ?? node.NodeType.Name,
            Properties: node.Properties
        );
    }

    public DomainEdgeSnapshot? TryGetEdge(string edgeId)
    {
        if (!Guid.TryParse(edgeId, out var id))
            return null;

        if (!_doc.Edges.TryGetValue(id, out var edge))
            return null;

        return new DomainEdgeSnapshot(
            EdgeId: edge.Id.ToString(),
            Type: edge.EdgeType.Name,
            FromNodeId: edge.FromNodeId.ToString(),
            ToNodeId: edge.ToNodeId.ToString(),
            Label: edge.Properties.TryGetValue("label", out var label)
                && label.ValueKind == JsonValueKind.String
                    ? label.GetString()
                    : null,
            Properties: edge.Properties
        );
    }

    public void SetSelectedNodeIds(IEnumerable<string> nodeIds)
    {
        SelectedNodeIds.Clear();
        SelectedNodeIds.AddRange(nodeIds);
        Changed?.Invoke();
    }

    // Call this from GraphCanvas selection callback:
    public void OnSelectionChanged(IReadOnlyList<SelectedElementDto> selected)
    {
        SelectedNodeIds.Clear();
        foreach (var s in selected)
        {
            if (s.Group == "edges") SelectedEdgeIds.Add(s.Id);
            if (s.Group == "nodes") SelectedNodeIds.Add(s.Id);
        }
        _selection.SetSelection(selected.ToList());
    }
    public void OnSelectionCleared()
    {
        SelectedNodeIds.Clear();
        _selection.Clear();
    }
    public void OnCanvasContextMenu(NodePosition position)
    {
        // Handle canvas context menu event (e.g., show a custom context menu at the given coordinates)
        Console.WriteLine($"Canvas context menu requested at ({position.X}, {position.Y})");
    }
}

// Snapshots used by inspector (simple)
public sealed record DomainNodeSnapshot(string NodeId, string Type, string? Label, IReadOnlyDictionary<string, JsonElement> Properties);
public sealed record DomainEdgeSnapshot(string EdgeId, string Type, string FromNodeId, string ToNodeId, string? Label, IReadOnlyDictionary<string, JsonElement> Properties);
