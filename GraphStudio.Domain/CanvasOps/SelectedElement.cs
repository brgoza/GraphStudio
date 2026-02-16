using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GraphStudio.Domain.CanvasOps;

public sealed record SelectedElement(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("group")] string Group
);