using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace GraphStudio.Domain;

public static class GraphDefaults
{
    public static GraphDocument GetDefaultDocument()
    {
        SchemaDefinition defaultSchema = GetDefaultSchema();
        return new GraphDocument(Guid.NewGuid(), "Default", defaultSchema.Id, defaultSchema, new Dictionary<Guid, Node>(), new Dictionary<Guid, Edge>(), 0);

    }
    public static SchemaDefinition GetDefaultSchema() =>
        new(Guid.NewGuid(),
            "Default Schema",
            "A default schema with basic node and edge types.",
            [GetDefaultNodeType()], [GetDefaultEdgeType()], GetDefaultNodeType(), GetDefaultEdgeType());

    public static NodeType GetDefaultNodeType() =>
        new(Guid.NewGuid(), "Node", "Node", [],[],new Dictionary<string,JsonElement>(), GetDefaultNodeStyle());
    public static NodeStyle GetDefaultNodeStyle() =>
        new(80, 80, NodeShape.Ellipse, Color.LightGray, Color.Black, Color.Black);
    public static EdgeType GetDefaultEdgeType() =>
        new(Guid.NewGuid(), "Relates to", "Relates to", false, false, [], [], new Dictionary<string,JsonElement>(),
           GetDefaultEdgeStyle());
    public static EdgeStyle GetDefaultEdgeStyle() =>
        new(CurveStyle.Bezier, Color.Black);
}
