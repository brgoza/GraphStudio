using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphStudio.Domain;

public sealed record GraphDocument(
    Guid Id,
    string Name,
    Guid SchemaId,
    [property: JsonIgnore] SchemaDefinition Schema,
    IReadOnlyDictionary<Guid, Node> Nodes,
    IReadOnlyDictionary<Guid, Edge> Edges,
    long? Revision
);

public sealed record Node(
    Guid Id,
    Guid NodeTypeId,
    [property: JsonIgnore] NodeType NodeType,
    string Label,
    NodePosition Position,
    IReadOnlyDictionary<string, JsonElement> Properties
);

public sealed record NodePosition(double X, double Y);


public sealed record Edge(
    Guid Id,
    Guid FromNodeId,
    Guid ToNodeId,
    Guid EdgeTypeId,
    [property: JsonIgnore] EdgeType EdgeType,
    bool IsDirected,
    IReadOnlyDictionary<string, JsonElement> Properties
);
