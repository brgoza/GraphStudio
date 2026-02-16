using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace GraphStudio.Domain;

public sealed record SchemaDefinition(Guid Id,
                                      string Name,
                                      string Description,
                                      IReadOnlyList<NodeType> NodeTypes,
                                      IReadOnlyList<EdgeType> EdgeTypes,
                                      NodeType DefaultNodeType,
                                      EdgeType DefaultEdgeType
                                     );
public sealed record NodeType(Guid Id,
                               string Name,
                               string Description,
                               HashSet<string> AllowedProperties,
                               HashSet<string> RequiredProperties,
                               IReadOnlyDictionary<string, JsonElement> DefaultProperties,
                               NodeStyle Style);
public sealed record NodeStyle(
    double Width,
    double Height,
    NodeShape Shape,
    Color BackgroundColor,
    Color BorderColor,
    Color Color);

public enum NodeShape
{
    Ellipse,
    Triangle,
    RoundTriangle,
    Rectangle,
    RoundRectangle,
    BottomRoundRectangle,
    CutRectangle,
    Barrel,
    Rhomboid,
    RightRhomboid,
    Diamond,
    RoundDiamond,
    Pentagon,
    RoundPentagon,
    Hexagon,
    RoundHexagon,
    ConcaveHexagon,
    Heptagon,
    RoundHeptagon,
    Octagon
}

public sealed record EdgeType(Guid Id,
                             string Name,
                             string DefaultLabel,
                             bool IsWeighted,
                             bool IsDirected,
                             HashSet<string> AllowedProperties,
                             HashSet<string> RequiredProperties,
                             IReadOnlyDictionary<string, JsonElement> Properties,
                             EdgeStyle Style);
public sealed record EdgeStyle(CurveStyle CurveStyle,
                                Color Color);
public enum CurveStyle
{
    HayStack,
    Bezier,
    Taxi,
    RoundTaxi
}