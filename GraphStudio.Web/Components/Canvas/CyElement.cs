using System.Text.Json.Serialization;

namespace GraphStudio.Web.Components.Canvas;

public sealed class CyElement
{
    [JsonPropertyName("group")]
    public string Group { get; init; } = default!; // "nodes" or "edges"
    
    [JsonPropertyName("data")]
    public Dictionary<string, object?> Data { get; init; } = new();

    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CyPosition? Position { get; init; }

    [JsonPropertyName("classes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Classes { get; init; }
}

public sealed record CyPosition(
    [property: JsonPropertyName("x")] double X,
    [property: JsonPropertyName("y")] double Y
);
