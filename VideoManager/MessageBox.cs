using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoManager
{
    public class MessageBox
    {
        public static void Show(string text)
        {
            System.Windows.Forms.MessageBox.Show(text);
        }
    }
}
