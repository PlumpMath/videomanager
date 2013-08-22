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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VlcInstance instance;
        VlcMediaPlayer player;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AppDataFolder = 
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + 
                System.IO.Path.DirectorySeparatorChar + Properties.Settings.Default.GeneralProductName;
#if DEBUG
            Properties.Settings.Default.AppDataFolder = 
                @"C:\Users\Franz\Documents\codeprojects\VideoManager\VideoManager\local";
#endif
            Properties.Settings.Default.ConnectionString = "data source=\"" + 
                Properties.Settings.Default.AppDataFolder + System.IO.Path.DirectorySeparatorChar + 
                Properties.Settings.Default.DatabaseFilename + "\"";

            //Database.FillFromDirectory(@"C:\Users\Franz\Documents\codeprojects\VideoManager\VideoManager\local\test", false);

            
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            if (player != null) 
                player.Dispose();
            instance.Dispose();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "";
            ofd.Filter = "MPEG|*.mpg|AVI|*.avi|All|*.*";
            if (ofd.ShowDialog() != true)
                return;

            using (VlcMedia media = new VlcMedia(instance, ofd.FileName))
            {
                if (player != null) player.Dispose();
                player = new VlcMediaPlayer(media);
            }

            System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
            panel.Height = 300;
            panel.Width = 300;

            System.Windows.Forms.Integration.WindowsFormsHost wfh = new System.Windows.Forms.Integration.WindowsFormsHost();
            wfh.Width = 300;
            wfh.Height = 300;
            wfh.Child = panel;
            player.Drawable = wfh.Handle;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            player.Play();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            string[] args = new string[] {
                /*"-I", "dummy", "--ignore-config",
                @"--plugin-path=C:\Users\Franz\Documents\codeprojects\VideoManager\VideoManager\libs\plugins",
                "--vout-filter=deinterlace", "--deinterlace-mode=blend"*/
            };

            instance = new VlcInstance();
            player = null;
        }
    }
}
