using System.Drawing;

using GraphStudio.Domain;

namespace GraphStudio.Web.Services;

public static class CytoscapeStyleBuilder
{
    public static object[] FromSchema(SchemaDefinition schema)
    {
        var rules = new List<object>
        {
            // Base defaults (always present)
            Rule("node", new Dictionary<string, object?>
            {
                ["label"] = "data(label)",
                ["text-valign"] = "center",
                ["text-halign"] = "center",
                ["width"] = 80,
                ["height"] = 40,
                ["shape"] = "round-rectangle"
            }),

            Rule("edge", new Dictionary<string, object?>
            {
                ["label"] = "data(label)",
                ["curve-style"] = "bezier",
                ["target-arrow-shape"] = "triangle"
            }),

            Rule(":selected", new Dictionary<string, object?>
            {
                ["border-width"] = 3
            })
        };

        // Node-type specific rules
        foreach (var nt in schema.NodeTypes)
        {
            rules.Add(Rule($"node[nodeTypeId = '{nt.Id}']", NodeStyle(nt.Style)));
        }

        // Edge-type specific rules
        foreach (var et in schema.EdgeTypes)
        {
            rules.Add(Rule($"edge[edgeTypeId = '{et.Id}']", EdgeStyle(et.Style, et.IsDirected)));
        }

        return rules.ToArray();
    }

    private static object Rule(string selector, Dictionary<string, object?> style)
        => new { selector, style };

    private static Dictionary<string, object?> NodeStyle(NodeStyle s)
        => new()
        {
            ["width"] = s.Width,
            ["height"] = s.Height,
            ["shape"] = ToCyShape(s.Shape),
            ["background-color"] = ToHex(s.BackgroundColor),
            ["border-color"] = ToHex(s.BorderColor),
            ["border-width"] = 2,
            ["color"] = ToHex(s.Color),
            ["text-wrap"] = "wrap",
            ["text-max-width"] = s.Width
        };

    private static Dictionary<string, object?> EdgeStyle(EdgeStyle s, bool isDirected)
        => new()
        {
            ["curve-style"] = ToCyCurve(s.CurveStyle),
            ["line-color"] = ToHex(s.Color),
            ["target-arrow-color"] = ToHex(s.Color),
            ["target-arrow-shape"] = isDirected ? "triangle" : "none",
            ["arrow-scale"] = 1.0
        };

    private static string ToCyShape(NodeShape shape)
        => shape switch
        {
            NodeShape.Ellipse => "ellipse",
            NodeShape.Triangle => "triangle",
            NodeShape.RoundTriangle => "round-triangle",
            NodeShape.Rectangle => "rectangle",
            NodeShape.RoundRectangle => "round-rectangle",
            NodeShape.BottomRoundRectangle => "bottom-round-rectangle",
            NodeShape.CutRectangle => "cut-rectangle",
            NodeShape.Barrel => "barrel",
            NodeShape.Rhomboid => "rhomboid",
            NodeShape.RightRhomboid => "right-rhomboid",
            NodeShape.Diamond => "diamond",
            NodeShape.RoundDiamond => "round-diamond",
            NodeShape.Pentagon => "pentagon",
            NodeShape.RoundPentagon => "round-pentagon",
            NodeShape.Hexagon => "hexagon",
            NodeShape.RoundHexagon => "round-hexagon",
            NodeShape.ConcaveHexagon => "concave-hexagon",
            NodeShape.Heptagon => "heptagon",
            NodeShape.RoundHeptagon => "round-heptagon",
            NodeShape.Octagon => "octagon",
            _ => "ellipse"
        };

    private static string ToCyCurve(CurveStyle curve)
        => curve switch
        {
            CurveStyle.HayStack => "haystack",
            CurveStyle.Bezier => "bezier",
            CurveStyle.Taxi => "taxi",
            CurveStyle.RoundTaxi => "round-taxi",
            _ => "haystack"
        };

    private static string ToHex(Color c)
        => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
