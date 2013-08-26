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
        SettingsWindow settingsWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // AppData folder
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
            

            // init windows
            settingsWindow = new SettingsWindow();


            // init libraries
            if (System.IO.Directory.Exists(Properties.Settings.Default.LibraryPath))    // TODO check for plugins dir and libvlc DLLs
            {
                Utils.SetDllDirectory(Properties.Settings.Default.LibraryPath);
            }
            else
                settingsWindow.Show();

            string pluginPath = Environment.GetEnvironmentVariable("VLC_PLUGIN_PATH");
            if (pluginPath == null)
            {
                MessageBox.Show("You have to set the environment variable VLC_PLUGIN_PATH to " + 
                    Properties.Settings.Default.LibraryPath + " and restart " + 
                    Properties.Settings.Default.GeneralProductName + "!");
                Application.Current.Shutdown();
                return;
            }


            // restore old window state
            if (Properties.Settings.Default.WindowWidth > 0 && Properties.Settings.Default.WindowHeight > 0)
            {
                this.Width = Properties.Settings.Default.WindowWidth;
                this.Height = Properties.Settings.Default.WindowHeight;
                if (Properties.Settings.Default.WasMaximized)
                    this.WindowState = System.Windows.WindowState.Maximized;
                this.Left = Properties.Settings.Default.WindowLeft;
                this.Top = Properties.Settings.Default.WindowTop;
            }


            // init VLC
            instance = new VlcInstance();
            player = null;


            // init UI
            sliVolume.Value = (Properties.Settings.Default.Volume) / 10.0;
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "";
            ofd.Filter = "MPEG|*.mpg|AVI|*.avi|All|*.*";
            if (ofd.ShowDialog() != true)
                return;

            using (VlcMedia media = VlcMedia.CreateFromFilepath(instance, ofd.FileName))
            {
                if (player != null) 
                    player.Dispose();
                player = new VlcMediaPlayer(media);
            }

            player.Drawable = wfh.Child.Handle;

            SetVolume(sliVolume.Value);
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
            settingsWindow.Show();
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // save window state
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.WasMaximized = (this.WindowState == System.Windows.WindowState.Maximized);
            Properties.Settings.Default.WindowLeft = this.Left;
            Properties.Settings.Default.WindowTop = this.Top;
            Properties.Settings.Default.Save();

            // free memory
            if (player != null)
                player.Dispose();
            if (instance != null)
                instance.Dispose();
        }

        private void sliVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player != null)
                SetVolume(sliVolume.Value);
        }


        public void SetVolume(double vol)
        {
            int volume = (int)vol * 10;
            player.SetVolume(volume);
        }

    }
}
