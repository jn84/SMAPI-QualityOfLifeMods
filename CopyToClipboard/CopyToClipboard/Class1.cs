using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyToClipboard
{
    public class Class1
    {
        [STAThread]
        static void Main( string[] args ) {
            Clipboard.SetText("test");
        }

    }
}
