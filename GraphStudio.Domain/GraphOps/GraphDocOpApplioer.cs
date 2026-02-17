using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GraphStudio.Domain.CanvasOps;

public static class GraphDocOpApplioer
{
    public static GraphDocument Apply(GraphDocument doc, CanvasOp op)
    {
        string kind = op.Kind;
        Console.WriteLine($"Applying CanvasOp of kind: {kind} to docId: {doc.Id}");
        return op.Kind switch
        {
            "AddNode" => ApplyAddNode(doc, (AddNodeOp)op),
            "AddEdge" => ApplyAddEdge(doc, (AddEdgeOp)op),
            "DeleteSelected" => ApplyDeleteSelected(doc, (DeleteSelectedOp)op),
            "SetNodeProps" => ApplySetNodeProps(doc, (SetNodePropsOp)op),
            "SetNodeLabel" => ApplySetNodeLabel(doc, (SetNodeLabelOp)op),
            _ => doc
        };
    }
    public static GraphDocument ApplySetNodeLabel(GraphDocument doc, SetNodeLabelOp op)
    {
        if (!Guid.TryParse(op.NodeId, out var nodeId))
            return doc;
        if (!doc.Nodes.TryGetValue(nodeId, out var node))
            return doc;
        var updatedNode = node with { Label = op.Label };
        var nodes = new Dictionary<Guid, Node>(doc.Nodes)
        {
            [nodeId] = updatedNode
        };
        return doc with { Nodes = nodes };
    }
    private static GraphDocument ApplyAddNode(GraphDocument doc, AddNodeOp op)
    {
        Console.WriteLine($"ApplyAddNoe\tGraphDocumentId\t{doc.Id}\t{op.NodeId}");
        var nodeId = Guid.Parse(op.NodeId);
        var nodeTypeId = Guid.Parse(op.NodeTypeId);
        var x = op.X;
        var y = op.Y;

        NodeType resolvedNodeType = doc.Schema.NodeTypes.FirstOrDefault(nt => nt.Id == nodeTypeId)
            ?? GraphDefaults.CreateNewNodeTypeWithDefaults();
        var newNode = new Node(nodeId,
            resolvedNodeType.Id, resolvedNodeType, op.Label ?? resolvedNodeType.Name, new NodePosition(x, y), new Dictionary<string, JsonElement>());

        var nodes = new Dictionary<Guid, Node>(doc.Nodes)
        {
            [nodeId] = newNode
        };
        Console.WriteLine($"Finished ApplyAddNode\tGraphDocumentId\t{doc.Id}\t{op.NodeId}");
        return doc with { Nodes = nodes };
    }

    private static GraphDocument ApplyAddEdge(GraphDocument doc, AddEdgeOp op)
    {
        var edgeId = Guid.Parse(op.EdgeId);
        var fromId = Guid.Parse(op.FromNodeId);
        var toId = Guid.Parse(op.ToNodeId);

        // Basic integrity checks (keep it simple for now)
        if (!doc.Nodes.ContainsKey(fromId) || !doc.Nodes.ContainsKey(toId))
            return doc;

        var newEdge = new Edge(
            Id: edgeId,
            FromNodeId: fromId,
            ToNodeId: toId,
            EdgeTypeId: doc.Schema.DefaultEdgeType.Id,
            EdgeType: doc.Schema.DefaultEdgeType,
            IsDirected: op.Directed,
            Properties: new Dictionary<string, JsonElement>()
        );

        var edges = new Dictionary<Guid, Edge>(doc.Edges)
        {
            [edgeId] = newEdge
        };

        return doc with { Edges = edges };
    }
    private static GraphDocument ApplyDeleteSelected(GraphDocument doc, DeleteSelectedOp op)
    {
        var nodes = new Dictionary<Guid, Node>(doc.Nodes);
        var edges = new Dictionary<Guid, Edge>(doc.Edges);

        foreach (var sel in op.SelectedElements)
        {
            if (!Guid.TryParse(sel.Id, out var id))
                continue;

            if (sel.Group == "nodes")
            {
                // Remove node
                nodes.Remove(id);

                // Remove connected edges
                var toRemove = edges.Values
                    .Where(e => e.FromNodeId == id || e.ToNodeId == id)
                    .Select(e => e.Id)
                    .ToList();

                foreach (var edgeId in toRemove)
                    edges.Remove(edgeId);
            }
            else if (sel.Group == "edges")
            {
                edges.Remove(id);
            }
        }

        return doc with
        {
            Nodes = nodes,
            Edges = edges
        };
    }
    private static GraphDocument ApplySetNodeProps(GraphDocument doc, SetNodePropsOp op)
    {
        if (!Guid.TryParse(op.NodeId, out var nodeId))
            return doc;

        if (!doc.Nodes.TryGetValue(nodeId, out var node))
            return doc;

        // Merge existing props with new props (new wins)
        var mergedProps = new Dictionary<string, JsonElement>(node.Properties);
        foreach (var (k, v) in op.Props)
            mergedProps[k] = v;

        var updatedNode = node with { Properties = mergedProps };

        var nodes = new Dictionary<Guid, Node>(doc.Nodes)
        {
            [nodeId] = updatedNode
        };

        return doc with { Nodes = nodes };
    }


}