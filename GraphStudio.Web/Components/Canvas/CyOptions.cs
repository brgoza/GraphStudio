using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace GraphStudio.Web.Components.Canvas;

public sealed class CyOptions
{
    [JsonPropertyName("elements")]
    public List<CyElement> Elements { get; init; } = new();

    [JsonPropertyName("style")]
    public List<CyStyleRule> Style { get; init; } = new()
    {
        CyStyleRule.NodesDefault(),
        CyStyleRule.EdgesDefault(),
        CyStyleRule.SelectedDefault()
    };

    [JsonPropertyName("layout")]
    public CyLayoutOptions Layout { get; init; } = CyLayoutOptions.Preset();

    [JsonPropertyName("wheelSensitivity")]
    public double WheelSensitivity { get; init; } = 0.2;

    [JsonPropertyName("minZoom")]
    public double MinZoom { get; init; } = 0.1;

    [JsonPropertyName("maxZoom")]
    public double MaxZoom { get; init; } = 3.0;

    [JsonPropertyName("moveFlushMs")]
    public int MoveFlushMs { get; init; } = 120;

    [JsonPropertyName("emitMovesDuringDrag")]
    public bool EmitMovesDuringDrag { get; init; } = false;
}
