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
        //override to allow for borders to be resized on a None type formwindowstyle
        protected override void WndProc(ref Message m)
        {
            const int wmNcHitTest = 0x84;
            const int htLeft = 10;
            const int htRight = 11;
            const int htTop = 12;
            const int htTopLeft = 13;
            const int htTopRight = 14;
            const int htBottom = 15;
            const int htBottomLeft = 16;
            const int htBottomRight = 17;

            if (m.Msg == wmNcHitTest)
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
             

        }
        private void MakeMax()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                // maximized windows shouldn't be resized, this prevents a resize from happening
                this.Padding = new Padding(0, 0, 0, 0);
            }
            else 
            { 
                this.WindowState = FormWindowState.Normal;
                this.Padding = new Padding(2, 2, 2, 2);
            }
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
            if (this.Opacity != 1) { this.Opacity = 1; }
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ControlBox = true;
            this.Text = "miniWebView";
            this.BackColor = Color.FromArgb(0, 0, 0);
            this.FormBorderStyle = FormBorderStyle.None;

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
                        Program.StartNewForm();
                        //MessageBox.Show(pageUri, "Page Uri", System.Windows.Forms.MessageBoxButtons.OK);
                    }, null);

                };
                menuList.Insert(menuList.Count, newItem);
            };
    }


        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                e.Handled = true;
                //Close your app
                Application.Exit();
            }
            if (e.Alt && e.KeyCode == Keys.N)
            {
                e.Handled = true;
                Program.StartNewForm();
            }
            e.Handled = true;
        }

        private void webView21_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (this.webView21.CoreWebView2 != null) { Form1_CustomizeMenu(); }
        }




        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Form1 form1 = this;
                e.Handled = true;
                // cute, so supresskeypress makes the sound go away, so we don't beep if the Uri is properly formed
                try
                {
                    form1.webView21.Source = new Uri(form1.textBox1.Text);
                    e.SuppressKeyPress = true;
                    this.textBox2.Visible = false;
                }
                catch (Exception exc)
                {
                    this.textBox2.Text = (exc.ToString());
                    this.textBox2.Visible = true;
                    //LogError(e, "not a uri...");
                    //throw;
                }
                


            }
            e.Handled = true;
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
            Application.Exit();
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
        //***********************************************************
        //This gives us the drop shadow behind the borderless form
        private const int CS_DROPSHADOW = 0x20000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
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
