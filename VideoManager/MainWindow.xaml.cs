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
using System.IO;

namespace VideoManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VlcMediaPlayer player;
        SettingsWindow settingsWindow;
        FullscreenWindow fw;
        System.Windows.Forms.Control videoControl;

        public MainWindow()
        {
            InitializeComponent();

            fw = new FullscreenWindow(this);

            gridSplitter.DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(gridSplitter_DragDelta);
            wfh.Child.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(PanelVideo_DoubleClick);
        }

        void PanelVideo_DoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            SetToFullscreen();
        }


		#region Fullscreen
		public void SetToFullscreen()
        {
            if (player != null && !player.IsFullscreen) // fullscreen only when video loaded, back always
            {
                player.IsFullscreen = true;
                videoControl = wfh.Child.Parent;
                wfh.Child.Parent = fw.wfh.Child;        // this.panelVideo.parent = fw.panelVideo
                wfh.Child.Dock = System.Windows.Forms.DockStyle.Fill;
                fw.Show();
            }
        }


        public void SetBackToNormalScreen()     // TODO event based
        {
            wfh.Child.Dock = System.Windows.Forms.DockStyle.None;
            System.Windows.Forms.Control ctrl = fw.wfh.Child.Controls[0];
            ctrl.Parent = videoControl;     // fw.panelVideo.child.parent = this.panelVideo.parent
            player.IsFullscreen = false;
        }
		#endregion


		private void gridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            int delta = (int)(mainGrid.ColumnDefinitions[0].ActualWidth + e.HorizontalChange);
            if (delta > 0)
                mainGrid.ColumnDefinitions[0].Width = new GridLength(delta);
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
            if (!VideoMgr.InitLibraryPath())
            {
                MessageBox.Show("DLLs not found. Please enter the path to your VLC libraries!");
                settingsWindow.ShowDialog();
            }

            if (VideoMgr.InitLibraryPath())
                Utils.SetDllDirectory(Properties.Settings.Default.LibraryPath);
            else
            {
                MessageBox.Show("DLLs still not found. Exiting...");
                Application.Current.Shutdown();
                return;
            }

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
				double plWidth = Properties.Settings.Default.PlaylistWidth;
				this.mainGrid.ColumnDefinitions[2].Width = 
					new GridLength(this.mainGrid.ActualWidth - this.gridSplitter.ActualWidth - plWidth);
				this.mainGrid.ColumnDefinitions[0].Width = new GridLength(plWidth);
            }


            // init VLC
			player = new VlcMediaPlayer();

			player.Drawable = wfh.Child.Handle;

			player.PlayingStatusChanged += new VlcMediaPlayer.PlayingStatusChangedHandler(PlayingStatusChanged);
			player.LengthChanged += new VlcMediaPlayer.LengthChangedHandler(LengthChanged);
			player.PositionChanged += new VlcMediaPlayer.PositionChangedHandler(PositionChanged);
			player.TimeChanged += new VlcMediaPlayer.TimeChangedHandler(TimeChanged);

			player.SetVolume(sliVolume.Value);


            // init input
            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(keyDown), false);


            // init UI
            sliVolume.Value = Properties.Settings.Default.Volume;


			// init database
			if (!Page.LoadPagesFromDatabase() || Page.GetDefaultPage() == null)
			{
				Page p = new Page(null, "YouTube", "http://youtube.com/", "YT");
				p.SaveToDatabase();
				if (p.ID.HasValue)
					Properties.Settings.Default.DefaultPageId = p.ID.Value;
				else
					System.Windows.Forms.MessageBox.Show("Default Page didn't receive an ID!");
			}

			// init playlist
			Playlist.ActivePlaylist = new Playlist();
			dgPlaylist.ItemsSource = Playlist.ActivePlaylist.Videos;
        }


		#region Window Close Events
		private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// save window state
			Properties.Settings.Default.WindowWidth = this.Width;
			Properties.Settings.Default.WindowHeight = this.Height;
			Properties.Settings.Default.WasMaximized = (this.WindowState == System.Windows.WindowState.Maximized);
			Properties.Settings.Default.WindowLeft = this.Left;
			Properties.Settings.Default.WindowTop = this.Top;
			Properties.Settings.Default.PlaylistWidth = this.mainGrid.ColumnDefinitions[0].Width.Value;
			Properties.Settings.Default.Volume = (int)sliVolume.Value;
			Properties.Settings.Default.Save();

			// free memory
			if (player != null)
				player.Dispose();
		}


        private void mainWindow_Closed(object sender, EventArgs e)
        {
        }
		#endregion


        private void keyDown(object sender, KeyEventArgs e)
        {
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == Key.Enter)// && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.System)))
                if (player != null)
                    SetToFullscreen();
        }


		#region Button Click Handlers
		private void button1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "";
            ofd.Filter = VideoMgr.GetAllowedFiletypesSelector();
            if (ofd.ShowDialog() != true)
                return;

			player.SetMediaFile(ofd.FileName);	// TODO player event media changed -> setmediafile, volume, etc.
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (player != null)
                player.Play();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (player != null)
                player.Pause();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (player != null)
                player.Stop();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {            
            settingsWindow.Show();
		}

		private void btnAddFile_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.Multiselect = true;
			if (Directory.Exists(Properties.Settings.Default.VideoDir))
				ofd.InitialDirectory = Properties.Settings.Default.VideoDir;
			ofd.CheckFileExists = true;
			ofd.Filter = VideoMgr.GetAllowedFiletypesSelector();
			bool? res = ofd.ShowDialog();
			if (res.HasValue && res.Value)
				Playlist.ActivePlaylist.AddVideos(ofd.FileNames);
		}

		private void btnAddDir_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog fd = 
				new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
			//System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
			fd.EnsurePathExists = true;
			fd.IsFolderPicker = true;
			fd.Title = "Choose a directory...";
			fd.InitialDirectory = Properties.Settings.Default.VideoDir;			
			if (fd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.OK)
				Playlist.ActivePlaylist.AddVideosFromDirectory(fd.FileName);
		}
		#endregion


		#region Volume Slider
		private void sliVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player != null)
                player.SetVolume(sliVolume.Value);
        }
		#endregion


		#region Event Handlers

		private void PlayingStatusChanged(object sender, VlcMediaPlayer.PlayingStatusEventArgs e)
		{
			switch (e.Status)
			{
				case VlcMediaPlayer.PlayingStatus.PLAYING:
					break;
				case VlcMediaPlayer.PlayingStatus.PAUSED:
					break;
				case VlcMediaPlayer.PlayingStatus.STOPPED:
					break;
				default: 
					break;
			}
		}


		private void LengthChanged(object sender, EventArgs e)
		{
			// TODO save length to database
		}


		private void PositionChanged(object sender, EventArgs e)
		{
			float pos = player.Position;
			sliTime.Dispatcher.Invoke(new Action(() => sliTime.Value = pos), null);
		}


		private void TimeChanged(object sender, EventArgs e)
		{
			string strTime = player.LengthString + " / " + player.TimeString;
			lblVideoTimeLength.Dispatcher.Invoke(new Action(() => lblVideoTimeLength.Content = strTime), null);
		}
		#endregion
	}
}
