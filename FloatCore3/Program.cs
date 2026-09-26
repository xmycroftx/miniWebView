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

            // os shell launches pass the target url as argv (default browser
            // invocations, oidc redirects, protocol handlers)
            var url = args?.FirstOrDefault(a => a.StartsWith("http", StringComparison.OrdinalIgnoreCase) || Uri.TryCreate(a, UriKind.Absolute, out _));
            Form1 form1 = new Form1(url);
            Application.Run(form1);
        }


    }
}

