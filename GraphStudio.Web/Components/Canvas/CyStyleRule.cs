using System.Text.Json.Serialization;

namespace GraphStudio.Web.Components.Canvas;

public sealed class CyStyleRule
{
    [JsonPropertyName("selector")]
    public string Selector { get; init; } = default!;

    [JsonPropertyName("style")]
    public Dictionary<string, object?> Style { get; init; } = new();

    public static CyStyleRule NodesDefault() => new()
    {
        Selector = "node",
        Style = new()
        {
            ["label"] = "data(label)",
            ["background-color"]= "green",
            ["text-valign"] = "center",
            ["text-halign"] = "center",
             ["width"] = 60,
             ["height"] = 60
        }
    };

    public static CyStyleRule EdgesDefault() => new()
    {
        Selector = "edge",
        Style = new()
        {
            ["curve-style"] = "bezier",
            ["target-arrow-shape"] = "triangle",
            ["label"] = "data(label)"
        }
    };

    public static CyStyleRule SelectedDefault() => new()
    {
        Selector = ":selected",
        Style = new()
        {
            ["border-width"] = 3
        }
    };
}
