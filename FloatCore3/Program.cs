using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloatCore3
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1(StartupUri(args));
            Application.Run(form1);


        }
        /// <summary>
        /// First argument, if it is an absolute http(s) or file URI; otherwise
        /// null, leaving the designer default in place. Registered as a browser
        /// we are handed whatever a link contains, so any other scheme is
        /// refused rather than passed to WebView2.
        /// </summary>
        static Uri StartupUri(string[] args)
        {
            if (args == null || args.Length == 0) { return null; }
            Uri uri;
            if (!Uri.TryCreate(args[0], UriKind.Absolute, out uri)) { return null; }
            if (uri.Scheme == Uri.UriSchemeHttp ||
                uri.Scheme == Uri.UriSchemeHttps ||
                uri.Scheme == Uri.UriSchemeFile)
            {
                return uri;
            }
            return null;
        }

        static public void StartNewForm()
        {
            return;
        }

    }
}
