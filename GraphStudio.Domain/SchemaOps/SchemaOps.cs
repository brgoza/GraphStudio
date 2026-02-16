using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphStudio.Domain.SchemaOps;

public interface ISchemaOp
{
    string Kind { get; }
}

public abstract record SchemaOp([property: JsonPropertyName("kind")] string Kind) : ISchemaOp;

// ---- Node types ----
public sealed record AddNodeTypeOp(
    [property: JsonPropertyName("nodeType")] NodeType NodeType
) : SchemaOp("AddNodeType");

public sealed record UpdateNodeTypeOp(
    [property: JsonPropertyName("nodeType")] NodeType NodeType
) : SchemaOp("UpdateNodeType");

public sealed record DeleteNodeTypeOp(
    [property: JsonPropertyName("nodeTypeId")] Guid NodeTypeId
) : SchemaOp("DeleteNodeType");

public sealed record SetDefaultNodeTypeOp(
    [property: JsonPropertyName("nodeTypeId")] Guid NodeTypeId
) : SchemaOp("SetDefaultNodeType");

// ---- Edge types ----
public sealed record AddEdgeTypeOp(
    [property: JsonPropertyName("edgeType")] EdgeType EdgeType
) : SchemaOp("AddEdgeType");

public sealed record UpdateEdgeTypeOp(
    [property: JsonPropertyName("edgeType")] EdgeType EdgeType
) : SchemaOp("UpdateEdgeType");

public sealed record DeleteEdgeTypeOp(
    [property: JsonPropertyName("edgeTypeId")] Guid EdgeTypeId
) : SchemaOp("DeleteEdgeType");

public sealed record SetDefaultEdgeTypeOp(
    [property: JsonPropertyName("edgeTypeId")] Guid EdgeTypeId
) : SchemaOp("SetDefaultEdgeType");
