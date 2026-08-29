# AGE Scene Format

**Status:** draft — format and semantics only. Loader implementation, engine workstreams and input architecture are out of scope here.

The AGE scene format is an HTML-like XML dialect. Nesting expresses the node tree; there are no `parent=` attributes and no NodePaths. The file is a loading recipe: it describes a tree of Scenes, Worlds, Canvases and SubViewports that the runtime assembles by re-parenting nodes into live viewports.

---

## 1. Core model

### 1.1 The file is viewport content

A `.scene` file is always **the content of a viewport**, never a viewport itself. The root element is a `<Scene>`; the host viewport is *external* — provided by code at load time:

```
viewport.Scene = Load("main.scene")   // host decides, file does not care
```

A viewport hosts **one** Scene; the Scene carries all of its aspects internally. "Root type is not important": the loader reads one `<Scene>` tree and hands it to whichever viewport requested it.

### 1.2 Placement defines role

There is exactly one rule for interpreting every element in a file — **where it sits defines what it is**:

| placement | role | behavior |
|---|---|---|
| `<Scene>` under a Viewport (host or SubViewport) | **active** | renders; its aspects route to that viewport's buffers |
| `<Scene>` under another `<Scene>` | **noop** | inert declaration — a template waiting to be claimed |
| `<SubViewport>` under any `<Scene>` | hosts the next level of viewports | |
| `<Resources>` entries | **inert data** | passive assets (materials, textures, geometry) |
| everything else under a `<Scene>` | content | routed to the nearest ancestor viewport |

Activity propagates down from the anchoring viewport: a SubViewport inside a noop scene is dormant until that scene activates. A noop scene is simply not connected to a viewport yet — inert by construction, no special flag required.

### 1.3 Aspects: `World3D`, `World2D`, `Canvas`

A Scene's direct children may include **up to one of each aspect container**:

- `<World3D>` — world content: `Spatial3D` nodes (`Mesh`, `Camera3D`, …)
- `<World2D>` — flat world content: `Spatial2D` nodes (`Sprite`, …)
- `<Canvas>` — chrome/UI content: Elements (`FlexBox`, `Text`, …)

The asymmetry between them is intentional: `World2D` and `World3D` are **worlds** — content arranged in space, seen through a camera — while UI is **chrome**: a screen-space overlay, layout-driven, with no camera. For that reason there is no third world-style container; the `Canvas` itself *is* the UI aspect of a scene. A generic `UI` container/class was considered and rejected — too generic to name anything the Canvas doesn't already.

Rules:

- At most one `Canvas` per `<Scene>`.
- Aspect containers are optional; a scene may have any subset.
- Routing is by container, with fixed predictable paths: everything under `World3D` feeds the 3D pass, everything under `World2D` the 2D pass, the `Canvas` the UI pass. No type-guessing — the subtree announces its pipeline by which container holds it.

---

## 2. Identifiers and references

### 2.1 Flat namespace

`id="…"` on any element registers it in a single **file-wide namespace**. Ids are forward-referenceable: references resolve in a second pass after the full parse. Definition order never matters.

### 2.2 Reference syntax

| form | meaning |
|---|---|
| `#id` | reference an element declared elsewhere in this file |
| `res://path` | load an external asset (fresh instance per reference) |

Every reference-kind attribute uses the same forms — `scene="#compass"`, `texture="#map-viewport"`, `viewport="#compass-viewport"`, `material="#mirror"`.

### 2.3 External scenes

`scene="res://map.scene"` loads another scene file as a **fresh instance** on each reference. Cross-file sharing therefore does not happen implicitly; sharing within a file happens through `#id`. (Caching policy for `res://` loads is an open item.)

---

## 3. Ownership: slot vs share

When a viewport binds a scene via `scene="…"`, the relationship resolves automatically into one of two modes — same syntax, different claim state:

### Owns (slot path)

The scene has no parent yet (it is a noop template, an inline child, or a fresh `res://` load). The viewport claims it: re-parents it into its scene slot and takes responsibility for **rendering and the update loop**.

### References (override path)

The scene already belongs to someone else. The viewport adopts the owner's render context and **renders it without ticking it**.

Invariants:

- **One owner per instance, many viewers.** The owner simulates the world once; N viewports render it from N cameras (split-screen falls out of duplicate refs).
- First claimant wins ownership; later claimants become viewers. Document order decides who slots.
- Inline `<Scene>` inside a SubViewport is born owned by that viewport.
- `filter` affects **rendering only**. An owned scene updates all of its aspects regardless of filter — excluded aspects are simply not rendered into that viewport. A scene stops updating only when explicitly paused.

---

## 4. SubViewport

`<SubViewport>` declares a nested render target. It is legal **only as a direct child of a `<Scene>`** (root or nested).

Attributes:

| attribute | type | meaning |
|---|---|---|
| `size` | `"W H"` | render target size in pixels |
| `scene` | `#id \| res://path` | the scene to bind (see §3) |
| `filter` | token list | which aspects render into this viewport, e.g. `"3D UI"`. Absent = all aspects. |

Content — three mutually compatible ways to provide a scene:

```xml
<!-- 1. inline: born owned -->
<SubViewport id="a" size="200 200">
    <Scene>
        <World3D>...</World3D>
    </Scene>
</SubViewport>

<!-- 2. internal reference: first ref owns, rest view -->
<SubViewport id="b" size="100 100" scene="#compass"/>

<!-- 3. external file: fresh instance per load -->
<SubViewport id="c" size="100 100" scene="res://map.scene"/>
```

An inline scene obeys the placement law automatically: a `<Scene>` under a SubViewport (= a Viewport) is born active.

---

## 5. Property grammar

One grammar for all properties, regardless of depth:

### Scalars → attributes

```xml
<Timer wait-time="2" one-shot="true"/>
<Camera3D position="0 2 5" far="100"/>
<SubViewport size="200 200"/>
```

Conventions:

- Multi-component values are space-separated (`position="0 2 5"`).
- Method-call conventions are expressed as attributes where unambiguous: `active="true"` on a camera means "make current".
- Defaults are omitted — the loader applies class defaults; files state intent, not boilerplate.

### Objects → property elements

Object-valued properties are assigned through a **child element named after the property**. Its own attributes configure the assigned value:

```xml
<Material id="mirror">
    <Diffuse texture="#mirror-viewport"/>
</Material>
```

This keeps nesting bounded — no `diffuse-texture-source=` attribute soup — and generalizes to arbitrary object depth.

### Text content

Bare text inside an element becomes an implicit `Text` node with `Value` set:

```xml
<Canvas>Hello AGE</Canvas>
<!-- identical to -->
<Canvas><Text value="Hello AGE"/></Canvas>
```

Rules:

- An element carries **either text content or child elements — never both** (content-XOR-children).
- Whitespace collapses HTML-like; leading/trailing whitespace is trimmed.
- Explicit `<Text value="…"/>` remains available when attributes must accompany the text; combining `value=` with bare content on the same element is an error.

---

## 6. Resources

The optional top-level `<Resources>` block declares **passive assets**: things with no tree position of their own (materials, textures, geometry).

```xml
<Resources>
    <Material id="mirror">
        <Diffuse texture="#mirror-viewport"/>
    </Material>
</Resources>
```

- Resource ids share the flat namespace; any node in any (sub)scene may reference them.
- Scenes and SubViewports do **not** belong in Resources — their placement encodes their role (§1.2).
- Geometry bound at construction (e.g. `Mesh` vertex/index data) is expressed through asset references: `<Mesh shape="res://compass.blend"/>`.

---

## 7. Consuming viewport output

A SubViewport's rendered output is just another id-referencable thing, consumed three ways with zero special-case syntax:

```xml
<!-- 1. embedded as UI (samples color + id buffers; hit-testing chains through) -->
<EmbeddedViewport viewport="#compass-viewport"/>

<!-- 2. sampled by 2D content -->
<Sprite texture="#map-viewport"/>

<!-- 3. mapped onto a 3D surface (in-world screens / virtual machines) -->
<Material>
    <Diffuse texture="#terminal-viewport"/>
</Material>
```

Nested UI is therefore legal by construction: a `<Canvas>` inside a nested scene renders into the owning SubViewport's UI buffer, sized to that viewport (`Canvas` self-sizes to its viewport). A scene can carry its own chrome — a compass widget is 3D content plus a label, shipped as one unit.

Feedback loops (`#mirror` sampled by content that mirror-viewport renders) are permitted; frame-delay semantics are the renderer's concern, not the format's.

---

## 8. Complete example

```xml
<Scene name="main">

    <Resources>
        <Material id="mirror">
            <Diffuse texture="#mirror-viewport"/>
        </Material>
    </Resources>

    <!-- noop template: activates when compass-viewport claims it -->
    <Scene id="compass">
        <World3D>
            <Camera3D active="true"/>
            <Mesh shape="res://compass.blend" material="#mirror"/>
        </World3D>
        <Canvas>Compass</Canvas>
    </Scene>

    <!-- the host viewport's own aspects -->
    <World3D>
        ...
    </World3D>

    <World2D>
        <Sprite texture="#map-viewport"/>
    </World2D>

    <Canvas>
        <EmbeddedViewport viewport="#compass-viewport"/>
    </Canvas>

    <!-- active instances: owned by the root scene -->
    <SubViewport id="map-viewport"      scene="res://map.scene" size="200 200"/>
    <SubViewport id="compass-viewport"  scene="#compass" filter="3D UI" size="100 100"/>
    <SubViewport id="mirror-viewport">
        <Scene>
            <World3D>
                ...
            </World3D>
        </Scene>
    </SubViewport>

</Scene>
```

Reading of this file at runtime:

1. Parse; build detached tree; resolve `#refs`.
2. Host code assigns it to a viewport (`viewport.Scene = …`) → root becomes active; its `World3D`/`World2D`/`Canvas` route to the viewport's buffers.
3. Nested scenes stay suspended (noop) until claimed.
4. Each SubViewport binds its scene per §3: `map-viewport` owns a fresh `map.scene`; `compass-viewport` claims `#compass` (ownership) and renders only its 3D + UI aspects; `mirror-viewport` owns its inline scene.
5. The mirror material samples mirror-viewport's output; compass renders that material back through compass-viewport — the loop is data-driven from ids alone.

---

## 9. Validation rules (loader-enforced)

1. Root element is a `<Scene>`.
2. `<SubViewport>` appears only as a direct child of a `<Scene>`.
3. ≤ one `<Canvas>` per `<Scene>`.
4. Content-XOR-children; `value=` + bare content together is an error.
5. `<Resources>` holds passive assets only.
6. Unreferenced noop scenes are dead content — warn.
7. All `#id` references must resolve; unknown or duplicate ids are errors.

---

## 10. Open items

- **Filter vocabulary:** tokens `2D/3D/UI` vs container tags `World3D/World2D/Canvas` — pick canonical spelling.
- **Geometry attribute name:** `shape=` vs `mesh=`.
- **`res://` caching:** confirm fresh-per-reference vs shared cache.
- **Blur policy:** clicking empty 3D space blurs the focused Canvas, or focus persists (relevant to VM-screen UX).
