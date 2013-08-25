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
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void btnBrowseLibPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            fd.Description = "Select the library path:";
            fd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            fd.ShowNewFolderButton = false;
            System.Windows.Forms.DialogResult dr = fd.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                txtLibPath.Text = fd.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LibraryPath = txtLibPath.Text;
            Properties.Settings.Default.Save();

            // init libraries
            Environment.SetEnvironmentVariable("VLC_PLUGIN_PATH", Properties.Settings.Default.LibraryPath + "\\");
            Utils.SetDllDirectory(Properties.Settings.Default.LibraryPath);

            this.Hide();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reload();
            txtLibPath.Text = Properties.Settings.Default.LibraryPath;
        }
    }
}
