namespace GraphStudio.Domain.SchemaOps;

public static class SchemaOpApplier
{
    public static SchemaDefinition Apply(SchemaDefinition schema, SchemaOp op) =>
        op switch
        {
            AddNodeTypeOp x => ApplyAddNodeType(schema, x),
            UpdateNodeTypeOp x => ApplyUpdateNodeType(schema, x),
            DeleteNodeTypeOp x => ApplyDeleteNodeType(schema, x),
            SetDefaultNodeTypeOp x => ApplySetDefaultNodeType(schema, x),

            AddEdgeTypeOp x => ApplyAddEdgeType(schema, x),
            UpdateEdgeTypeOp x => ApplyUpdateEdgeType(schema, x),
            DeleteEdgeTypeOp x => ApplyDeleteEdgeType(schema, x),
            SetDefaultEdgeTypeOp x => ApplySetDefaultEdgeType(schema, x),

            _ => schema
        };

    private static SchemaDefinition ApplyAddNodeType(SchemaDefinition schema, AddNodeTypeOp op)
    {
        var list = schema.NodeTypes.ToList();
        list.Add(op.NodeType);

        return schema with { NodeTypes = list };
    }

    private static SchemaDefinition ApplyUpdateNodeType(SchemaDefinition schema, UpdateNodeTypeOp op)
    {
        var list = schema.NodeTypes.ToList();
        var idx = list.FindIndex(t => t.Id == op.NodeType.Id);
        if (idx < 0) return schema;

        list[idx] = op.NodeType;

        // Keep default coherent
        var defaultNodeType = schema.DefaultNodeType.Id == op.NodeType.Id
            ? op.NodeType
            : schema.DefaultNodeType;

        return schema with { NodeTypes = list, DefaultNodeType = defaultNodeType };
    }

    private static SchemaDefinition ApplyDeleteNodeType(SchemaDefinition schema, DeleteNodeTypeOp op)
    {
        // You can choose your rule here. This one blocks deleting default.
        if (schema.DefaultNodeType.Id == op.NodeTypeId)
            return schema;

        var list = schema.NodeTypes.Where(t => t.Id != op.NodeTypeId).ToList();
        return schema with { NodeTypes = list };
    }

    private static SchemaDefinition ApplySetDefaultNodeType(SchemaDefinition schema, SetDefaultNodeTypeOp op)
    {
        var nt = schema.NodeTypes.FirstOrDefault(t => t.Id == op.NodeTypeId);
        if (nt is null) return schema;

        return schema with { DefaultNodeType = nt };
    }

    private static SchemaDefinition ApplyAddEdgeType(SchemaDefinition schema, AddEdgeTypeOp op)
    {
        var list = schema.EdgeTypes.ToList();
        list.Add(op.EdgeType);

        return schema with { EdgeTypes = list };
    }

    private static SchemaDefinition ApplyUpdateEdgeType(SchemaDefinition schema, UpdateEdgeTypeOp op)
    {
        var list = schema.EdgeTypes.ToList();
        var idx = list.FindIndex(t => t.Id == op.EdgeType.Id);
        if (idx < 0) return schema;

        list[idx] = op.EdgeType;

        var defaultEdgeType = schema.DefaultEdgeType.Id == op.EdgeType.Id
            ? op.EdgeType
            : schema.DefaultEdgeType;

        return schema with { EdgeTypes = list, DefaultEdgeType = defaultEdgeType };
    }

    private static SchemaDefinition ApplyDeleteEdgeType(SchemaDefinition schema, DeleteEdgeTypeOp op)
    {
        if (schema.DefaultEdgeType.Id == op.EdgeTypeId)
            return schema;

        var list = schema.EdgeTypes.Where(t => t.Id != op.EdgeTypeId).ToList();
        return schema with { EdgeTypes = list };
    }

    private static SchemaDefinition ApplySetDefaultEdgeType(SchemaDefinition schema, SetDefaultEdgeTypeOp op)
    {
        var et = schema.EdgeTypes.FirstOrDefault(t => t.Id == op.EdgeTypeId);
        if (et is null) return schema;

        return schema with { DefaultEdgeType = et };
    }
}
