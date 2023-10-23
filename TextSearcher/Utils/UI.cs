using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSearcher.Utils
{
    internal static class UI
    {
        public static bool Confirm(string text)
        {
            var ok = MessageBox.Show(
                text,
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
            );
            return ok == DialogResult.Yes;
        }
    }
}
