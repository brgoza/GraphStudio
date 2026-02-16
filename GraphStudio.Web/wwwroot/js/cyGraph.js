const instances = new Map();

/** Simple throttle (time-based) */
function throttle(fn, ms)
{
    let last = 0;
    let timer = null;
    let pendingArgs = null;

    return (...args) =>
    {
        const now = Date.now();
        const remaining = ms - (now - last);

        if (remaining <= 0)
        {
            last = now;
            fn(...args);
        } else
        {
            pendingArgs = args;
            if (!timer)
            {
                timer = setTimeout(() =>
                {
                    timer = null;
                    last = Date.now();
                    fn(...pendingArgs);
                    pendingArgs = null;
                }, remaining);
            }
        }
    };
}

/** Batch node moves and send them to .NET */
function createMoveBatcher(dotNetRef, flushMs)
{
    const moves = new Map(); // nodeId -> {x,y}
    const flush = throttle(() =>
    {
        if (moves.size === 0) return;
        const payload = Array.from(moves.entries()).map(([id, pos]) => ({
            nodeId: id,
            x: pos.x,
            y: pos.y
        }));
        moves.clear();
        dotNetRef.invokeMethodAsync("OnNodeMovesCommitted", payload);
    }, flushMs);

    return {
        record(node)
        {
            moves.set(node.id(), node.position());
            flush();
        },
        flushNow()
        {
            if (moves.size === 0) return;
            const payload = Array.from(moves.entries()).map(([id, pos]) => ({
                nodeId: id,
                x: pos.x,
                y: pos.y
            }));
            moves.clear();
            dotNetRef.invokeMethodAsync("OnNodeMovesCommitted", payload);
        }
    };
}

function normalizeElements(elements)
{
    // Expect elements in Cytoscape JSON form:
    // { group: 'nodes'|'edges', data:{...}, position?:{x,y}, classes?:string }
    return elements;
}

function opApply(cy, op)
{
    switch (op.kind)
    {
        case "AddNode": {
            cy.add({
                group: "nodes",
                data: { nodeId: op.nodeId, nodeTypeId: op.nodeTypeId, nodeTypeName:  op.nodeTypeName, label: op.label ?? op.nodeTypeName, ...op.data },
                position: { x: op.x, y: op.y },
                classes: op.classes ?? ""
            });
            return;
        }
        case "RemoveNode": {
            cy.getElementById(op.nodeId).remove();
            return;
        }
        case "DeleteSelected": {
            for (const sel of op.selected)
            {
                cy.getElementById(sel.id).remove();
            }
            return;
        }
        case "SetNodeProps": {
            const n = cy.getElementById(op.nodeId);
            if (n.empty()) return;
            // Store props under data.props, and optionally mirror label
            const existing = n.data("props") || {};
            n.data("props", { ...existing, ...op.props });
            if (op.props && op.props.label != null) n.data("label", op.props.label);
            return;
        }
        case "SetNodeLabel": {
            const n = cy.getElementById(op.nodeId);
            if (n.empty()) return;
            n.data("label", op.label);
            return;
        }
        case "MoveNodes": {
            cy.batch(() =>
            {
                for (const m of op.moves)
                {
                    const n = cy.getElementById(m.nodeId);
                    if (!n.empty()) n.position({ x: m.x, y: m.y });
                }
            });
            return;
        }
        case "AddEdge": {
            cy.add({
                group: "edges",
                data: {
                    id: op.edgeId,
                    source: op.fromNodeId,
                    target: op.toNodeId,
					edgeTypeId: op.edgeTypeId,
                    type: op.relType,
                    label: op.label ?? op.relType,
                    ...op.data
                },
                classes: op.classes ?? ""
            });
            return;
        }
        case "RemoveEdge": {
            cy.getElementById(op.edgeId).remove();
            return;
        }
        case "SetEdgeProps": {
            const e = cy.getElementById(op.edgeId);
            if (e.empty()) return;
            const existing = e.data("props") || {};
            e.data("props", { ...existing, ...op.props });
            if (op.props && op.props.label != null) e.data("label", op.props.label);
            return;
        }
        case "SetClasses": {
            const el = cy.getElementById(op.elementId);
            if (el.empty()) return;
            // Replace or toggle; here we replace for simplicity
            el.classes(op.classes ?? "");
            return;
        }
        default:
            // Unknown op kind - ignore to keep client resilient
            return;
    }
}

export function create(hostElement, dotNetRef, options)
{
    const handleId = crypto.randomUUID();

    const cy = cytoscape({
        container: hostElement,
        elements: normalizeElements(options?.elements ?? []),
        style: options?.style ?? [],
        layout: options?.layout ?? { name: "preset" },
        wheelSensitivity: options?.wheelSensitivity ?? 0.2,
        minZoom: options?.minZoom ?? 0.1,
        maxZoom: options?.maxZoom ?? 3
    });

    // Prefer deterministic editor behavior
    cy.autoungrabify(false);

    const moveBatcher = createMoveBatcher(dotNetRef, options?.moveFlushMs ?? 120);

    // Selection events (batched by Cytoscape naturally; we still throttle)
    const selectionNotify = throttle(() =>
    {
        const selected = cy.$(":selected").map(el => ({ id: el.id(), group: el.group() }));
        dotNetRef.invokeMethodAsync("OnSelectionChanged", selected);
    }, 60);

    cy.on("select unselect", "node, edge", selectionNotify);

    // Node move tracking
    cy.on("dragfree", "node", evt => moveBatcher.record(evt.target));
    // Optionally record during drag (heavier). Use if you need live presence:
    if (options?.emitMovesDuringDrag)
    {
        const dragMove = throttle((node) => moveBatcher.record(node), 80);
        cy.on("drag", "node", evt => dragMove(evt.target));
    }

    // Delete key handling
    const keydown = (e) =>
    {
        if (e.key === "Delete" || e.key === "Backspace")
        {
            const selected = cy.$(":selected").map(el => ({ id: el.id(), group: el.group() }));
            if (selected.length > 0) dotNetRef.invokeMethodAsync("OnDeleteRequested", selected);
        }
    };
    window.addEventListener("keydown", keydown);

    // Context menu (right-click) - optional
    cy.on("cxttap", (evt) =>
    {
        // evt.target is cy (background) or element
        const t = evt.target;
        const pos = evt.position; // model coords
        if (t === cy)
        {
            dotNetRef.invokeMethodAsync("OnCanvasContextMenu", { x: pos.x, y: pos.y });
        } else
        {
            dotNetRef.invokeMethodAsync("OnElementContextMenu", { id: t.id(), group: t.group(), x: pos.x, y: pos.y });
        }
    });

    instances.set(handleId, {
        cy,
        dotNetRef,
        moveBatcher,
        keydown
    });

    return handleId;
}

export function dispose(handleId)
{
    const inst = instances.get(handleId);
    if (!inst) return;
    window.removeEventListener("keydown", inst.keydown);
    inst.cy.destroy();
    instances.delete(handleId);
}

export function applyOps(handleId, ops)
{
    const inst = instances.get(handleId);
    if (!inst) return;

    const cy = inst.cy;
    cy.batch(() =>
    {
        for (const op of ops) opApply(cy, op);
    });
}

export function setStyle(handleId, style)
{
    const inst = instances.get(handleId);
    if (!inst) return;
    inst.cy.style(style);
}

export function runLayout(handleId, layoutOptions)
{
    const inst = instances.get(handleId);
    if (!inst) return;
    inst.cy.layout(layoutOptions).run();
}

export function fit(handleId, padding = 30)
{
    const inst = instances.get(handleId);
    if (!inst) return;
    inst.cy.fit(undefined, padding);
}

export function setViewport(handleId, viewport)
{
    const inst = instances.get(handleId);
    if (!inst) return;
    // viewport: { zoom, panX, panY }
    inst.cy.zoom(viewport.zoom);
    inst.cy.pan({ x: viewport.panX, y: viewport.panY });
}

export function getViewport(handleId)
{
    const inst = instances.get(handleId);
    if (!inst) return null;
    const pan = inst.cy.pan();
    return { zoom: inst.cy.zoom(), panX: pan.x, panY: pan.y };
}

