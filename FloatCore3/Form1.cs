using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloatCore3
{

    public partial class Form1 : Form
    {

        // frameless-but-native: keep the window styles (tiling/snap work) and
        // remove the frame visuals via NCCALCSIZE; square corners via DWM.
        private const int WM_NCCALCSIZE = 0x0083;
        private const int SM_CXSIZEFRAME = 32;
        private const int SM_CYSIZEFRAME = 33;
        private const int SM_CXPADDEDBORDER = 92;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int pref = 1;          // DWMWCP_DONOTROUND - square corners
            DwmSetWindowAttribute(Handle, 33, ref pref, 4);
            int black = 0x000000;  // black window border
            DwmSetWindowAttribute(Handle, 34, ref black, 4);
            RegisterHotKey(Handle, HOTKEY_NEWWINDOW, MOD_SHIFT | MOD_WIN | MOD_NOREPEAT, (uint)'N');
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_NEWWINDOW)
            {
                // the os consumes the modifier releases with the chord - re-release
                // them so win/shift don't stay stuck down for other apps
                keybd_event(0x5B, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);   // lwin up
                keybd_event(0x5C, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);   // rwin up
                keybd_event(0xA0, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);   // lshift up
                keybd_event(0xA1, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);   // rshift up
                TrySpawnInstance();
                return;
            }
            if (m.Msg == WM_NCCALCSIZE && m.WParam != IntPtr.Zero)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    // maximized: keep content inside the monitor (frame overhang otherwise bleeds off-screen)
                    int sx = GetSystemMetrics(SM_CXSIZEFRAME) + GetSystemMetrics(SM_CXPADDEDBORDER);
                    int sy = GetSystemMetrics(SM_CYSIZEFRAME) + GetSystemMetrics(SM_CXPADDEDBORDER);
                    // rgrc[0] is the first RECT of NCCALCSIZE_PARAMS at LParam
                    int l = System.Runtime.InteropServices.Marshal.ReadInt32(m.LParam, 0);
                    int t = System.Runtime.InteropServices.Marshal.ReadInt32(m.LParam, 4);
                    int r = System.Runtime.InteropServices.Marshal.ReadInt32(m.LParam, 8);
                    int b = System.Runtime.InteropServices.Marshal.ReadInt32(m.LParam, 12);
                    System.Runtime.InteropServices.Marshal.WriteInt32(m.LParam, 0, l + sx);
                    System.Runtime.InteropServices.Marshal.WriteInt32(m.LParam, 4, t + sy);
                    System.Runtime.InteropServices.Marshal.WriteInt32(m.LParam, 8, r - sx);
                    System.Runtime.InteropServices.Marshal.WriteInt32(m.LParam, 12, b - sy);
                    m.Result = System.IntPtr.Zero;
                    return;
                }
                // normal: the whole window is client - no native frame or caption visuals
                m.Result = System.IntPtr.Zero;
                return;
            }
            const int wmNcHitTest = 0x84;
            const int htMaxButton = 9;

            // snap layouts flyout: let the os own the max-button region
            if (maxButton != null && maxButton.Visible)
            {
                var mb = maxButton.Bounds;
                var ptMax = PointToClient(new Point((int)(m.LParam.ToInt64() & 0xFFFF), (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16)));
                if (ptMax.X >= mb.Left && ptMax.X < mb.Right && ptMax.Y >= mb.Top && ptMax.Y < mb.Bottom)
                {
                    m.Result = (IntPtr)htMaxButton;
                    return;
                }
            }

            const int htLeft = 10;
            const int htRight = 11;
            const int htTop = 12;
            const int htTopLeft = 13;
            const int htTopRight = 14;
            const int htBottom = 15;
            const int htBottomLeft = 16;
            const int htBottomRight = 17;

            if (m.Msg == wmNcHitTest && WindowState == FormWindowState.Normal)
            {
                int x = (int)(m.LParam.ToInt64() & 0xFFFF);
                int y = (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16);
                Point pt = PointToClient(new Point(x, y));
                Size clientSize = ClientSize;
                ///allow resize on the lower right corner
                if (pt.X >= clientSize.Width - 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htBottomLeft : htBottomRight);
                    return;
                }
                ///allow resize on the lower left corner
                if (pt.X <= 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htBottomRight : htBottomLeft);
                    return;
                }
                ///allow resize on the upper right corner
                if (pt.X <= 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htTopRight : htTopLeft);
                    return;
                }
                ///allow resize on the upper left corner
                if (pt.X >= clientSize.Width - 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htTopLeft : htTopRight);
                    return;
                }
                ///allow resize on the top border
                if (pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htTop);
                    return;
                }
                ///allow resize on the bottom border
                if (pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htBottom);
                    return;
                }
                ///allow resize on the left border
                if (pt.X <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htLeft);
                    return;
                }
                ///allow resize on the right border
                if (pt.X >= clientSize.Width - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htRight);
                    return;
                }
            }
            base.WndProc(ref m);
        }
        public Form1()
        {
            InitializeComponent();

            // fullscreen stays inside the window: tiling keeps working and the
            // page's fullscreen element is constrained to the window bounds
            textBox1.Enter += textBox1_Enter;
        }

        private string _initialUrl;

        public Form1(string initialUrl) : this()
        {
            _initialUrl = initialUrl;
        }

        public Form1(string initialUrl, bool popup, CoreWebView2Environment env) : this()
        {
            _initialUrl = initialUrl;
            _isPopup = popup;
            _sharedEnvironment = env;
        }

        public Form1(string initialUrl) : this()
        {
            _initialUrl = initialUrl;
        }

        // --- win+shift+n global hotkey -> spawn a new process instance ---
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_NEWWINDOW = 0x0901;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;
        private const uint MOD_NOREPEAT = 0x4000;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(System.IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(System.IntPtr hWnd, int id);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void keybd_event(byte vk, byte scan, uint dwFlags, System.UIntPtr dwExtraInfo);
        private const uint KEYEVENTF_KEYUP = 0x0002;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UnregisterHotKey(Handle, HOTKEY_NEWWINDOW);
            base.OnHandleDestroyed(e);
        }

        // spawn a new executable instance; a named mutex keeps the racing
        // hooks of multiple running instances from spawning more than one
        private static void TrySpawnInstance()
        {
            try
            {
                using (var mutex = new System.Threading.Mutex(true, "Local\\FloatCore3-SpawnLock", out var createdNew))
                {
                    if (!createdNew) { return; }
                    System.Threading.Thread.Sleep(600);   // cover the hooks of the other instances
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to spawn a new window: " + ex.Message);
            }
        }

        // "Always on Top" = pure behavior toggle (z-order + focus chrome);
        // the borderless skin is permanent.
        private bool _wmResyncDone;
        private string _initialUrl;
        private bool _isPopup;
        private CoreWebView2Environment _sharedEnvironment;

        // one explicit environment for the whole process: every webview (main,
        // popups, standbys) shares this browser process and its profile store
        private static CoreWebView2Environment _sharedEnv;
        private static CoreWebView2Environment GetSharedEnvironment()
        {
            if (_sharedEnv == null)
            {
                var udf = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(Application.ExecutablePath),
                    "FloatCore3.exe.WebView2");
                _sharedEnv = Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, udf, null).GetAwaiter().GetResult();
            }
            return _sharedEnv;
        }

        // pre-initialized hidden webview so oidc/print popups get real popup
        // semantics inside a window we own (window.opener, postMessage, close)
        private Form1 _standbyPopup;
        private bool _standbyReady;
        private bool _standbyCreating;

        private void EnsureStandbyPopup()
        {
            if (_standbyPopup != null || _standbyCreating) { return; }
            if (webView21.CoreWebView2 == null) { return; }
            _standbyCreating = true;
            var env = GetSharedEnvironment();
            var popup = new Form1(null, true, env);
            _standbyPopup = popup;
            popup.FormClosed += (s, ev) => { if (_standbyPopup == popup) { _standbyPopup = null; } };
            popup.webView21.EnsureCoreWebView2Async(env).ContinueWith(t =>
            {
                _standbyCreating = false;
                _standbyReady = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // re-fire EVENT_OBJECT_SHOW once so event-driven window managers
            // (GlazeWM, komorebi, ...) pick the window up reliably
            if (_wmResyncDone) { return; }
            _wmResyncDone = true;
            Visible = false;
            Visible = true;
        }

        private void SetAlwaysOnTop(bool on)
        {
            this.TopMost = on;
            if (this.alwaysOnTopToolStripMenuItem.Checked != on)
            {
                this.alwaysOnTopToolStripMenuItem.Checked = on;
            }
        }
        private void MakeMax()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // borderless maximize otherwise covers the taskbar too; keep it inside the working area like a normal window
                this.MaximizedBounds = Screen.FromControl(this).WorkingArea;
                this.WindowState = FormWindowState.Maximized;
                this.Padding = new Padding(0, 0, 0, 0);
            }
            else 
            { 
                this.WindowState = FormWindowState.Normal;
                this.Padding = new Padding(2, 2, 2, 2);
            }
        }
        // Right-click on the title bar strip toggles window behaviors (e.g. always on top).
        private void TopBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) { return; }
            // only the strip above the web content is the title bar
            if (this.PointToClient(Cursor.Position).Y > TitleBarBottom()) { return; }
            this.alwaysOnTopToolStripMenuItem.Checked = this.TopMost;
            this.titleBarContextMenu.Show(Cursor.Position);
        }

        // bottom edge of the title bar strip, in form client coordinates
        private int TitleBarBottom()
        {
            return this.PointToClient(this.webView21.PointToScreen(Point.Empty)).Y;
        }

        private void alwaysOnTopToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            SetAlwaysOnTop(this.alwaysOnTopToolStripMenuItem.Checked);
        }

        private void UpdateTopMostForFullScreen()
        {
            if (this.webView21.CoreWebView2 == null) { return; }
            this.TopMost = this.webView21.CoreWebView2.ContainsFullScreenElement;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized) { UpdateTopMostForFullScreen(); }
        }

        private void MakeActive()
        {
            /* No Longer necessary, we are none style always
             if (this.FormBorderStyle != FormBorderStyle.Sizable)
            { 
                this.FormBorderStyle = FormBorderStyle.Sizable;
                //fix the location bug, it appears to shift left 4 pixels when switching border styles.
                this.Location = new Point((this.Location.X - 4), this.Location.Y);
                this.Size = new Size(this.Size.Width - 4, this.Size.Height-35);
            }*/
            SetWindowOpacity(255);
            if (this.textBox1.Visible != true)
            {
                this.textBox1.Visible = true;
                this.maxButton.Visible = true;
                this.closeButton.Visible = true;
                this.titleButton.Visible = true;
            }
        }

        private void webView21_MouseOver(object sender, EventArgs e)
        {
            MakeActive();

        }

        private void MakeTransp()
        {
            // transparency only in always-on-top float mode: opaque windows are
            // tileable by window managers (ws_ex_layered gets them ignored)
            SetWindowOpacity(this.alwaysOnTopToolStripMenuItem.Checked ? (byte)204 : (byte)255);
            /* No Longer necessary, we are none style always
            if (this.FormBorderStyle != FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Location = new Point((this.Location.X + 4), this.Location.Y);
                this.Size = new Size(this.Size.Width + 4, this.Size.Height+35 );
            }*/
            if (this.Opacity != 0.8) { this.Opacity = 0.8; }
            if (this.textBox1.Visible != false)
            {
                this.textBox1.Visible = false;
                this.maxButton.Visible = false;
                this.closeButton.Visible = false;
                this.titleButton.Visible = false;
            }
            

        }
        private void webView21_MouseLeave(object sender, EventArgs e)
        {
            MakeTransp();

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "miniWebView";
            this.BackColor = Color.FromArgb(0, 0, 0);
            SetAlwaysOnTop(_isPopup);   // popups float above their opener
            if (_sharedEnvironment != null)
            {
                webView21.EnsureCoreWebView2Async(GetSharedEnvironment());
            }

            if (_initialUrl != null)
            {
                try { webView21.Source = new Uri(_initialUrl); } catch { }
            }

            if (_isPopup)
            {
                this.Size = new Size(1000, 700);
            }
        }
        private void Form1_CustomizeMenu() { 
        this.webView21.CoreWebView2.ContextMenuRequested += delegate (object sender,CoreWebView2ContextMenuRequestedEventArgs args)
            {
                IList<CoreWebView2ContextMenuItem> menuList = args.MenuItems;
        // add new item to end of collection
                CoreWebView2ContextMenuItem newItem =
                            this.webView21.CoreWebView2.Environment.CreateContextMenuItem(
            "Display Page Uri", null, CoreWebView2ContextMenuItemKind.Command);
        newItem.CustomItemSelected += delegate (object send, Object ex)
                {
                    string pageUri = args.ContextMenuTarget.PageUri;

                    System.Threading.SynchronizationContext.Current.Post((_) =>
                    {
                        TrySpawnInstance();
                        //MessageBox.Show(pageUri, "Page Uri", System.Windows.Forms.MessageBoxButtons.OK);
                    }, null);

                };
                menuList.Insert(menuList.Count, newItem);
            };

            // site-driven popups (oidc login, print, etc) render in our own
            // managed standby window - window.opener and postMessage keep working
            this.webView21.CoreWebView2.NewWindowRequested += (s, ev) =>
            {
                ev.Handled = true;
                if (ev.WindowFeatures.HasSize && _standbyReady)
                {
                    var standby = _standbyPopup;
                    _standbyPopup = null;
                    _standbyReady = false;
                    ev.NewWindow = standby.webView21.CoreWebView2;
                    standby.webView21.Source = new Uri(ev.Uri);
                    standby.Show();
                    EnsureStandbyPopup();
                }
                else
                {
                    // plain links: redirect in place
                    webView21.Source = new Uri(ev.Uri);
                }
            };

            // popup pages close themselves when the flow completes
            this.webView21.CoreWebView2.WindowCloseRequested += (s, ev) =>
            {
                Close();
            };

            // concurrent env creation (multiple instances racing the shared
            // browser process) can fail transiently - retry with backoff
            this.webView21.CoreWebView2.ContainsFullScreenElementChanged += delegate (object sender, object args)
            {
                if (this.WindowState == FormWindowState.Minimized) { return; }

                // the winforms control auto-maximizes the host window when the
                // page goes fullscreen - counteract so fullscreen stays inside
                // the window and tiling keeps working
                if (webView21.CoreWebView2.ContainsFullScreenElement && WindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                }

                UpdateTopMostForFullScreen();
            };
    }


        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.N)
            {
                e.Handled = true;
                TrySpawnInstance();
            }
            e.Handled = true;
        }

        private int _webviewInitRetries;
        private bool _customizeDone;

        private async void webView21_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            // target=_blank links and window.open redirect in place
            // (chrome-style: no new window, no unmanaged popup webview)
            this.webView21.CoreWebView2.NewWindowRequested += (s, ev) =>
            {
                ev.Handled = true;
                webView21.Source = new Uri(ev.Uri);
            };

            // concurrent env creation (multiple instances racing the shared
            // browser process) can fail transiently - retry with backoff
            if (!e.IsSuccess)
            {
                while (_webviewInitRetries < 3)
                {
                    _webviewInitRetries++;
                    await Task.Delay(1500 * _webviewInitRetries);
                    try
                    {
                        await webView21.EnsureCoreWebView2Async();
                        if (!_customizeDone) { _customizeDone = true; Form1_CustomizeMenu(); }
                        return;
                    }
                    catch { continue; }
                }
                return;
            }
            if (!_customizeDone) { _customizeDone = true; Form1_CustomizeMenu(); }
        }




        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // only Enter is intercepted - arrows, home/end, delete, ctrl+a/v/x
            // all flow through natively for normal text editing
            if (e.KeyCode != Keys.Enter) { return; }

            e.SuppressKeyPress = true;
            NavigateFromUrlBar();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            // standard url bar behavior: select everything so typing replaces it
            textBox1.SelectAll();
        }

        private void NavigateFromUrlBar()
        {
            var raw = textBox1.Text.Trim();
            if (raw.Length == 0) { return; }

            // https:// enforcement: prepend when no scheme was typed
            if (!System.Text.RegularExpressions.Regex.IsMatch(raw, @"^[a-zA-Z][a-zA-Z0-9+.\-]*://"))
            {
                raw = "https://" + raw;
            }

            try
            {
                webView21.Source = new Uri(raw);
                textBox2.Visible = false;
            }
            catch (Exception ex)
            {
                textBox2.Text = "Invalid URL: " + ex.Message;
                textBox2.Visible = true;
            }
        }

        /*
        private void webView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            this.textBox1.Text = this.webView21.Source.ToString();
        }

        private void webView21_ContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e)
        {
            this.textBox1.Text = this.webView21.Source.ToString();
        }
        */
        private void webView21_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            this.textBox1.Text = this.webView21.Source.ToString();
        }



        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void maxButton_Click(object sender, EventArgs e)
        {
            MakeMax();
        }

        private void minButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            
        }

        /* Stuff*/
        //***********************************************************
        //This gives us the ability to resize the borderless from any borders instead of just the lower right corner

        //***********************************************************
        // per-window transparency via raw layered attributes - the winforms
        // Opacity property throws on this frameless (nccalcsize-stripped) window
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x00080000;
        private const uint LWA_ALPHA = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowLongW(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLongW(IntPtr hWnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        /// 255 = fully opaque (layered style removed); lower = see-through
        private void SetWindowOpacity(byte alpha)
        {
            int ex = GetWindowLongW(Handle, GWL_EXSTYLE);
            if (alpha < 255)
            {
                if ((ex & WS_EX_LAYERED) == 0) { SetWindowLongW(Handle, GWL_EXSTYLE, ex | WS_EX_LAYERED); }
                SetLayeredWindowAttributes(Handle, 0, alpha, LWA_ALPHA);
            }
            else if ((ex & WS_EX_LAYERED) != 0)
            {
                SetWindowLongW(Handle, GWL_EXSTYLE, ex & ~WS_EX_LAYERED);   // opaque: drop layered so wms keep tiling us
            }
        }
        //***********************************************************
        //This gives us the drop shadow behind the borderless form
        private const int CS_DROPSHADOW = 0x20000;
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_POPUP = unchecked((int)0x80000000);
                const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
                CreateParams cp = base.CreateParams;
                cp.Style = (cp.Style & ~WS_POPUP) | WS_OVERLAPPEDWINDOW;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
        //***********************************************************



        // Incantations from the olden times
        // This allows us to use User32 interfaces to directly drag the window.
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private void tableLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
            ReleaseCapture();
        }

        private void tableLayoutPanel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            MakeMax();
        }
    }
}



