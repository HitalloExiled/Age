# AGE Scene Format

**Status:** draft — file format rules only.

The AGE scene format is an HTML-like XML dialect. Nesting expresses the node tree; there are no `parent=` attributes and no NodePaths. Tags use this file's `PascalCase` convention; the explicit aspect wrapper is `<Composition>`.

---

## 1. Core model

### 1.1 The file is viewport content

A `.scene` file is always **the content of a viewport**, never a viewport itself. The root element is a `<Scene>`.

A viewport hosts **one** Scene; the Scene carries all of its aspects internally.

### 1.2 Placement defines role

There is exactly one rule for interpreting every element in a file — **where it sits defines what it is**:

| placement | role |
|---|---|
| `<Scene>` as file root | the scene this file defines |
| `<Scene>` under another `<Scene>` | nested scene |
| `<Scene>` under a `<SubViewport>` | inline scene for that viewport |
| `<SubViewport>` under a `<Scene>` | nested viewport declaration |
| `<Resources>` entries | passive assets (materials, textures, geometry) |
| `<Composition>` under a `<Scene>` | the scene's own `World3D`/`World2D`/`Canvas` |
| everything else under `World3D` / `World2D` / `Canvas` | content of that aspect |

### 1.3 `Composition`

`World3D`, `World2D` and `Canvas` are grouped explicitly so self aspects never mix with nested children:

```xml
<Scene name="main">
  <Composition>
    <World3D>...</World3D>
    <World2D>...</World2D>
    <Canvas>...</Canvas>
  </Composition>

  <Scene id="compass">...</Scene>
  <SubViewport id="map-viewport" ... />
</Scene>
```

Rules:

- At most one `<Composition>` per `<Scene>`; inside it at most one each of `World3D`, `World2D`, `Canvas`, in that fixed order.
- `World3D`/`World2D`/`Canvas` appear **only** inside `<Composition>`. A bare world directly under `<Scene>` is an error.
- `Scene` / `SubViewport` / `Resources` never appear inside `<Composition>`.

### 1.4 Aspects: `World3D`, `World2D`, `Canvas`

- `<World3D>` — world content: `Spatial3D` nodes (`Mesh`, `Camera3D`, …)
- `<World2D>` — flat world content: `Spatial2D` nodes (`Sprite`, …)
- `<Canvas>` — chrome/UI content: Elements (`FlexBox`, `Text`, …)

The asymmetry between them is intentional: `World2D` and `World3D` are **worlds** — content arranged in space, seen through a camera — while UI is **chrome**: a screen-space overlay, layout-driven, with no camera.

Rules:

- At most one `Canvas` per `<Scene>` (follows from a single `Composition`).
- Aspect containers are optional; a composition may hold any subset.
- Content is routed by container: everything under `World3D` belongs to the 3D aspect, everything under `World2D` to the 2D aspect, the `Canvas` to the UI aspect. No type-guessing — the subtree announces its aspect by which container holds it.

---

## 2. Identifiers and references

### 2.1 Flat namespace

`id="…"` on any element registers it in a single **file-wide namespace**. Ids are forward-referenceable. Definition order never matters.

### 2.2 Reference syntax

| form | meaning |
|---|---|
| `#id` | reference an element declared elsewhere in this file |
| `res://path` | reference an external file |

Every reference-kind attribute uses the same forms — `scene="#compass"`, `texture="#map-viewport"`, `viewport="#compass-viewport"`, `material="#mirror"`.

### 2.3 External scenes

`scene="res://map.scene"` references another scene file. Sharing within a file happens through `#id`.

---

## 3. SubViewport

`<SubViewport>` declares a nested render target. It is legal **only as a direct child of a `<Scene>`** (root or nested).

Attributes:

| attribute | type | meaning |
|---|---|---|
| `size` | `"W H"` | render target size in pixels |
| `scene` | `#id \| res://path` | the scene this viewport shows |
| `filter` | token list | which aspects this viewport shows, e.g. `"3D UI"`. Absent = all aspects. |

Content — three mutually compatible ways to provide a scene:

```xml
<!-- 1. inline scene -->
<SubViewport id="a" size="200 200">
    <Scene>
        <Composition>
            <World3D>...</World3D>
        </Composition>
    </Scene>
</SubViewport>

<!-- 2. internal reference -->
<SubViewport id="b" size="100 100" scene="#compass"/>

<!-- 3. external file -->
<SubViewport id="c" size="100 100" scene="res://map.scene"/>
```

---

## 4. Property grammar

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
- Defaults are omitted — files state intent, not boilerplate.

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

## 5. Resources

The optional `<Resources>` block declares **passive assets**: things with no tree position of their own (materials, textures, geometry).

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

## 6. Consuming viewport output

A SubViewport's output is just another id-referencable thing, written three ways with zero special-case syntax:

```xml
<!-- 1. embedded as UI -->
<EmbeddedViewport viewport="#compass-viewport"/>

<!-- 2. sampled by 2D content -->
<Sprite texture="#map-viewport"/>

<!-- 3. mapped onto a 3D surface -->
<Material>
    <Diffuse texture="#terminal-viewport"/>
</Material>
```

A `<Canvas>` inside a nested scene belongs to that scene's composition, so a scene can carry its own chrome — a compass widget is 3D content plus a label, shipped as one unit.

Reference cycles (`#mirror` sampled by content that `mirror-viewport` shows) are permitted by the format.

---

## 7. Complete example

`docs/scene-example.xml` (canonical order: `Composition`, then `Resources`, then nested `Scene`s, then `SubViewport`s):

```xml
<Scene name="main">
  <Composition>
    <World3D>...</World3D>

    <World2D>
      <Sprite texture="#map-viewport" />
    </World2D>

    <Canvas>
      <EmbeddedViewport viewport="#compass-viewport"></EmbeddedViewport>
    </Canvas>
  </Composition>

  <Resources>
    <Material id="mirror">
      <Diffuse texture="#mirror-viewport" />
    </Material>
  </Resources>

  <Scene id="compass">
    <Composition>
      <World3D>
        <Camera3D active="true"></Camera3D>
        <Mesh shape="res://compass.blend" material="#mirror"></Mesh>
      </World3D>

      <Canvas>Compass</Canvas>
    </Composition>
  </Scene>

  <SubViewport id="map-viewport" scene="res://map.scene" size="200 200"></SubViewport>
  <SubViewport id="compass-viewport" scene="#compass" filter="3D UI" size="100 100"></SubViewport>

  <SubViewport id="mirror-viewport">
    <Scene>
      <Composition>
        <World3D>...</World3D>
        <World2D>...</World2D>
        <Canvas>...</Canvas>
      </Composition>
    </Scene>
  </SubViewport>
</Scene>
```

---

## 8. Validation rules

1. Root element is a `<Scene>`.
2. `<SubViewport>` appears only as a direct child of a `<Scene>`.
3. ≤ one `<Composition>` per `<Scene>`; inside it ≤ one each of `World3D`, `World2D`, `Canvas`, in that order. Bare `World3D`/`World2D`/`Canvas` directly under `<Scene>` is an error.
4. ≤ one `<Canvas>` per `<Scene>` (follows from 3).
5. Content-XOR-children; `value=` + bare content together is an error.
6. `<Resources>` holds passive assets only.
7. Unreferenced nested scenes are dead content — warn.
8. All `#id` references must resolve; unknown or duplicate ids are errors.
9. Open/close tags must match (`<Camera3>` vs `</Camera3D>` is an error).

---

## 9. Open items

- **Filter vocabulary:** tokens `2D/3D/UI` vs container tags `World3D/World2D/Canvas` — pick canonical spelling.
- **Geometry attribute name:** `shape=` vs `mesh=`.
