Here is a concise breakdown of each interface. These are the building blocks that allow your engine to communicate with the Wayland compositor for everything from rendering to hardware-specific input.

### Core Rendering & Window Management
* **`wl_compositor_interface`**: The fundamental factory for creating **surfaces**. It’s the "blank canvas" onto which you attach your Vulkan or SHM buffers.
* **`xdg_wm_base_interface`**: Manages the desktop lifecycle of your windows. It turns raw surfaces into "Toplevel" windows that can be moved, resized, and closed.
* **`wl_shm_interface`**: The Shared Memory manager. It allows the engine to share CPU-rendered pixel buffers with the compositor using memory-mapped files.
* **`wp_viewporter_interface`**: Provides scaling and cropping for surfaces. This lets your engine render at a lower internal resolution and have the compositor scale it to fit the window.
* **`wp_fractional_scale_manager_v1_interface`**: Modern HiDPI support. It allows the compositor to tell the engine to render at non-integer scales (like 1.25x or 1.5x) for crisp visuals on 4K displays.
* **`zxdg_decoration_manager_v1_interface`**: Negotiates window frames. It tells the compositor to draw the title bar and borders (**Server-Side Decorations**) instead of the engine doing it.

### Input & Interaction (The Seat)
* **`wl_seat_interface`**: Groups input devices (keyboard, mouse, touch). It’s the parent interface used to focus input and create hardware-specific listeners.
* **`wl_output_interface`**: Represents a physical monitor. It provides data on resolution, refresh rate, and the physical location of the screen in the global space.
* **`wp_cursor_shape_manager_v1_interface`**: Replaces raw pixel cursors. It lets the engine request standard OS cursor icons (like "wait" or "text") by name.
* **`zwp_relative_pointer_manager_v1_interface`**: Essential for games. It provides raw mouse movement (deltas) instead of absolute coordinates, preventing the "camera stop" when a mouse hits the screen edge.
* **`zwp_pointer_constraints_v1_interface`**: Allows the engine to **lock** or **confine** the mouse cursor within the window area.
* **`zwp_pointer_gestures_v1_interface`**: Handles multi-finger touchpad events like pinch-to-zoom and finger swipes.

### Data & Shell Utilities
* **`wl_data_device_manager_interface`**: Manages the system **Clipboard** and **Drag-and-Drop** operations for files and text.
* **`zwp_primary_selection_device_manager_v1_interface`**: Implements the Linux "Middle-click to paste" behavior for highlighted text.
* **`xdg_activation_v1_interface`**: Safely handles focus transfers. It allows your engine to request that its window be brought to the front (e.g., when a launcher starts the game).
* **`zxdg_exporter_v1 / v2_interface`**: Allows a window to export a "handle." This lets other processes (like a separate plugin UI) attach their own windows to yours.
* **`zwp_idle_inhibit_manager_v1_interface`**: Prevents the screen from dimming or going to sleep while the engine is running, even if the user isn't pressing keys.
* **`xdg_system_bell_v1_interface`**: Provides a standard way to trigger the system's audible "alert" or visual bell.

### Specialized Hardware & IME
* **`zwp_tablet_manager_v2_interface`**: Advanced support for Wacom-style tablets, including pressure sensitivity, pen tilt, and eraser tools.
* **`zwp_text_input_manager_v3_interface`**: The bridge for **IMEs** (Input Method Editors). It allows users to type complex characters (like CJK languages) into your engine's text fields.
