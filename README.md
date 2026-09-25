# miniWebView

MiniWebView is a minimalistic web browser that uses the WebView2 library.  

To keep the interface clean there's a menubar, and a urlbar and that's it.  

The urlbar hides itself when you tab away (in floating mode).  The app runs in two modes, switched by the "Always on Top" toggle:

- **Window mode (default):** a normal resizable window with a native title bar, snap/Win+arrow keys, taskbar presence.  Right-click the title bar and pick **Always on Top** (system menu) to switch.
- **Floating mode:** the original always-on-top widget - borderless, floats above everything, urlbar and chrome hide when you click away (80% opacity).  Right-click the title bar strip and untick **Always on Top** (context menu) to switch back.

Fullscreen video stays inside the window boundary; while anything is fullscreen the window floats above other apps regardless of mode.  In floating mode there's some transparency. (80%)

Active:

![image](https://github.com/user-attachments/assets/4081541a-9cbf-440e-813c-34a34e967d8b)

After you click away:

![image](https://github.com/user-attachments/assets/1c0eab88-2e46-4082-8aa2-c07a778f9f58)
