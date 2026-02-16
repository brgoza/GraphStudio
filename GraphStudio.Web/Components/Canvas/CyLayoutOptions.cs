using System.Text.Json.Serialization;

namespace GraphStudio.Web.Components.Canvas;

public sealed class CyLayoutOptions
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = "preset";

    // Add more layout properties later (animate, spacingFactor, etc.)

    public static CyLayoutOptions Preset() => new() { Name = "preset" };
    public static CyLayoutOptions Grid() => new() { Name = "grid" };
    public static CyLayoutOptions Dagre() => new() { Name = "dagre" };
}
