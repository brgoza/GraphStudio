using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace GraphStudio.Domain;

public class GraphDocumentFactory
{
    private readonly ILogger<GraphDocumentFactory> _logger;
    public GraphDocumentFactory(ILogger<GraphDocumentFactory> logger)
    {
        _logger = logger;
    }
    public GraphDocument CreateNewDocument()
    {
        var doc = GraphDefaults.CreateNewDocumentWithDefaults();
        _logger.LogInformation("Created new GraphDocument with Id: {DocumentId}", doc.Id);
        return doc;
    }
}