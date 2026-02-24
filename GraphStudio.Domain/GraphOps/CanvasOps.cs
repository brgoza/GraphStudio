using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphStudio.Domain.CanvasOps;

public interface ICanvasOp
{
    string Kind { get; }
}

public abstract record CanvasOp([property: JsonPropertyName("kind")] string Kind) : ICanvasOp;
// --------Selected elements --------
public sealed record DeleteSelectedOp(
    [property: JsonPropertyName("selected")] IReadOnlyList<SelectedElement> SelectedElements
) : CanvasOp("DeleteSelected");
// -------- Nodes --------

public sealed record AddNodeOp(
    [property: JsonPropertyName("nodeId")] string NodeId,
    [property: JsonPropertyName("nodeTypeId")] string NodeTypeId,
    [property: JsonPropertyName("nodeTypeName")] string NodeTypeName,
    [property: JsonPropertyName("x")] double X,
    [property: JsonPropertyName("y")] double Y,
    [property: JsonPropertyName("label")] string? Label = null,
    //[property: JsonPropertyName("data")] IReadOnlyDictionary<string, object?>? Data = null,
    [property: JsonPropertyName("classes")] string? Classes = null
) : CanvasOp("AddNode");

public sealed record RemoveNodeOp(
    [property: JsonPropertyName("nodeId")] string NodeId
) : CanvasOp("RemoveNode");

public sealed record SetNodePropsOp(
    [property: JsonPropertyName("nodeId")] string NodeId,
    [property: JsonPropertyName("props")] IReadOnlyDictionary<string, JsonElement> Props
) : CanvasOp("SetNodeProps");

public sealed record SetNodeLabelOp(
    [property: JsonPropertyName("nodeId")] string NodeId,
    [property: JsonPropertyName("label")] string Label
) : CanvasOp("SetNodeLabel");

public sealed record MoveNodesOp(
    [property: JsonPropertyName("moves")] IReadOnlyList<NodeMove> Moves
) : CanvasOp("MoveNodes");

public sealed record NodeMove(
    [property: JsonPropertyName("nodeId")] string NodeId,
    [property: JsonPropertyName("x")] double X,
    [property: JsonPropertyName("y")] double Y
);

// -------- Edges --------

public sealed record AddEdgeOp(
    [property: JsonPropertyName("edgeId")] string EdgeId,
    [property: JsonPropertyName("fromNodeId")] string FromNodeId,
    [property: JsonPropertyName("toNodeId")] string ToNodeId,
    [property: JsonPropertyName("edgeTypeId")] string EdgeTypeId,
    [property: JsonPropertyName("relType")] string RelType,
    [property: JsonPropertyName("directed")] bool Directed = true,
    [property: JsonPropertyName("label")] string? Label = null,
    [property: JsonPropertyName("data")] IReadOnlyDictionary<string, object?>? Data = null,
    [property: JsonPropertyName("classes")] string? Classes = null
) : CanvasOp("AddEdge");
public sealed record SwapEdgeEndpointsOp(
    [property: JsonPropertyName("edgeId")] string EdgeId
) : CanvasOp("SwapEdgeEndpoints");

public sealed record RemoveEdgeOp(
    [property: JsonPropertyName("edgeId")] string EdgeId
) : CanvasOp("RemoveEdge");

public sealed record SetEdgePropsOp(
    [property: JsonPropertyName("edgeId")] string EdgeId,
    [property: JsonPropertyName("props")] IReadOnlyDictionary<string, JsonElement> Props
) : CanvasOp("SetEdgeProps");

// -------- Visual state --------

public sealed record SetClassesOp(
    [property: JsonPropertyName("elementId")] string ElementId,
    [property: JsonPropertyName("classes")] string? Classes
) : CanvasOp("SetClasses");