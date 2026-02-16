using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace GraphStudio.Domain;

public static class Utilities
{
    public static FileInfo SaveDocToJsonFile(this GraphDocument doc, string outputFilename)
    {
        var json = JsonSerializer.Serialize(doc);
        File.WriteAllText(outputFilename, json);
        return new FileInfo(outputFilename);
    }
    public static GraphDocument? LoadDocFromJsonFile(string inputFilename)
    {
        var json = File.ReadAllText(inputFilename);
        return JsonSerializer.Deserialize<GraphDocument>(json);
       
    }
}
