using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace GraphStudio.Domain;

public static class GraphDefaults
{
    public static GraphDocument CreateNewDocumentWithDefaults()
    {
        Console.WriteLine("Creating default graph document...");
        SchemaDefinition defaultSchema = CreateNewSchemaWithDefaults();
        var doc = new GraphDocument(Guid.NewGuid(), "Default", defaultSchema.Id, defaultSchema, new Dictionary<Guid, Node>(), new Dictionary<Guid, Edge>(), 0);
        Console.WriteLine($"Created default graph document with ID: {doc.Id}");
        return doc;
    }
    public static SchemaDefinition CreateNewSchemaWithDefaults() =>
        new(Guid.NewGuid(),
            "Default Schema",
            "A default schema with basic node and edge types.",
            [CreateNewNodeTypeWithDefaults()], [CreateNewEdgeTypeWithDefaults()], CreateNewNodeTypeWithDefaults(), CreateNewEdgeTypeWithDefaults());

    public static NodeType CreateNewNodeTypeWithDefaults() =>
        new(Guid.NewGuid(), "Node", "Node", [],[],new Dictionary<string,JsonElement>(), GetDefaultNodeStyle());
    public static NodeStyle GetDefaultNodeStyle() =>
        new(80, 80, NodeShape.Ellipse, Color.LightGray, Color.Black, Color.Black);
    public static EdgeType CreateNewEdgeTypeWithDefaults() =>
        new(Guid.NewGuid(), "Relates to", "Relates to", false, false, [], [], new Dictionary<string,JsonElement>(),
           GetDefaultEdgeStyle());
    public static EdgeStyle GetDefaultEdgeStyle() =>
        new(CurveStyle.Bezier, Color.Black);
}
