using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GraphStudio.Domain;

public sealed class GraphState
{
    public GraphDocument Document { get; private set; } =
        GraphDefaults.GetDefaultDocument();

    public string SerializeDocument() => JsonSerializer.Serialize(Document);
    public static GraphDocument Deserialize(string json) =>
        JsonSerializer.Deserialize<GraphDocument>(json) 
        ?? throw new InvalidOperationException("Failed to deserialize GraphDocument from JSON.");
    public void Update(GraphDocument doc) => Document = doc;
}
