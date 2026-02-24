using System.Text.Json.Serialization;

namespace GraphStudio.Web.Components.Canvas
{
    public class CyViewport
    {
        [JsonPropertyName("zoom")]
        public double Zoom { get; set; }
        [JsonPropertyName("panX")]
        public double PanX { get; set; }
        [JsonPropertyName("panY")]
        public double PanY { get; set; }
    }
}
