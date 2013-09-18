using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VideoManager
{
    /// <summary>
    /// Interaktionslogik für FullscreenWindow.xaml
    /// </summary>
    public partial class FullscreenWindow : Window
    {
        public MainWindow MainWin { get; set; }

        public FullscreenWindow(MainWindow mw)
        {
            InitializeComponent();

            MainWin = mw;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
                MainWin.SetBackToNormalScreen();
            }
        }
    }
}
