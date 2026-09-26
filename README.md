# miniWebView

MiniWebView is a minimalistic frameless browser built on WebView2 — a window that tiles like a native tile in GlazeWM/komorebi/FancyZones, with an optional always-on-top floating mode.

## The window

The chrome is stripped at the Win32 level (`WM_NCCALCSIZE` + DWM): square corners, black border, custom black X/min/max buttons. The window keeps its real styles underneath, so native tiling, Win+arrow snapping, the Snap Layouts flyout (maximize-button hover), and taskbar-respecting maximize all work.

## Always on Top

Right-click the title bar strip and tick **Always on Top**: the window floats above everything, and the url bar + chrome hide when it loses focus (80% opacity). Untick to return to a normal opaque tile.

## URL bar

- Full text editing: arrows, Home/End, Delete, Ctrl+A/C/V
- Selects all on focus — type to replace, arrows to edit
- `https://` is prepended automatically when no scheme is typed; explicit `http://` and other schemes are respected
- Links that open new windows redirect in place

## Win+Shift+N

Spawns a new independent browser window (separate process — close them in any order). The chord is swallowed before the OS sees it, so it won't fight your notification center or tiling manager.

## Requirements

- Windows 10/11 with the [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) (preinstalled on up-to-date systems)

Active:

![image](https://github.com/user-attachments/assets/4081541a-9cbf-440e-813c-34a34e967d8b)

After you click away:

![image](https://github.com/user-attachments/assets/1c0eab88-2e46-4082-8aa2-c07a778f9f58)
