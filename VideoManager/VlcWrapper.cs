using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VideoManager
{
    class VlcInstance : IDisposable
    {
        internal IntPtr Handle;

        public VlcInstance()
        {
            Handle = LibVlc.libvlc_new(0, null);
        }

        public void Dispose()
        {
            LibVlc.libvlc_release(Handle);
        }
    }



    class VlcMedia : IDisposable
    {
        internal IntPtr Handle;

        private VlcMedia()
        {
        }

        public static VlcMedia CreateFromFilepath(VlcInstance instance, string file)
        {
            VlcMedia m = new VlcMedia();
            m.Handle = LibVlc.libvlc_media_new_path(instance.Handle, file);
            return m;
        }

        public static VlcMedia CreateFromUrl(VlcInstance instance, string url)
        {
            VlcMedia m = new VlcMedia();
            m.Handle = LibVlc.libvlc_media_new_location(instance.Handle, url);
            return m;
        }

        public static VlcMedia CreateFromFileDescriptor(VlcInstance instance, int fd)
        {
            VlcMedia m = new VlcMedia();
            m.Handle = LibVlc.libvlc_media_new_fd(instance.Handle, fd);
            return m;
        }

        public void Dispose()
        {
            LibVlc.libvlc_media_release(Handle);
        }
    }



    class VlcMediaPlayer : IDisposable
    {
        #region Fields
        internal IntPtr Handle;
		private VlcInstance instance;
        private IntPtr drawable;
        private bool isKeyInputConsumed, isMouseInputConsumed;
		public enum PlayingStatus { PLAYING, PAUSED, STOPPED };
        #endregion


		#region Properties
		public PlayingStatus Status { get; set; }
        
        public IntPtr Drawable
        {
            get
            {
                return drawable;
            }
            set
            {
                LibVlc.libvlc_media_player_set_hwnd(Handle, value);
                drawable = value;
            }
        }

		public IntPtr EventManager { get; private set; }

        public bool IsFullscreen { get; set; }

        public bool IsKeyInputConsumed
        {
            get
            {
                return isKeyInputConsumed;
            }
            set
            {
                LibVlc.libvlc_video_set_key_input(Handle, value);
                isKeyInputConsumed = value;
            }
        }

        public bool IsMouseInputConsumed
        {
            get
            {
                return isMouseInputConsumed;
            }
            set
            {
                LibVlc.libvlc_video_set_mouse_input(Handle, value);
                isMouseInputConsumed = value;
            }
        }

		public long Length { get; private set; }

		public string LengthString { get; private set; }

        public long Time
        {
            get
            {
                return LibVlc.libvlc_media_player_get_time(Handle);
            }
        }

		public string TimeString { get; private set; }

        public float Position
        {
            get
            {
                return LibVlc.libvlc_media_player_get_position(Handle);
            }
            set
            {
                LibVlc.libvlc_media_player_set_position(Handle, value);
            }
        }
        #endregion


        #region Events
		#region Custom Events
		#region Playing Status Changed
		public class PlayingStatusEventArgs : EventArgs
        {
            public PlayingStatus Status { get; set; }

            public PlayingStatusEventArgs(PlayingStatus status)
            {
                Status = status;
            }
        }
        public delegate void PlayingStatusChangedHandler(object sender, PlayingStatusEventArgs e);
        public event PlayingStatusChangedHandler PlayingStatusChanged;
		#endregion

		#endregion

		#region VLC Events
		#region Delegates
		private LibVlc.EventCallbackDelegate vlcLengthChangedDelegate;
		public delegate void LengthChangedHandler(object sender, EventArgs e);
		public event LengthChangedHandler LengthChanged;

		private LibVlc.EventCallbackDelegate vlcPositionChangedDelegate;
		public delegate void PositionChangedHandler(object sender, EventArgs e);
		public event PositionChangedHandler PositionChanged;

		private LibVlc.EventCallbackDelegate vlcTimeChangedDelegate;
		public delegate void TimeChangedHandler(object sender, EventArgs e);
		public event TimeChangedHandler TimeChanged;
		#endregion

		#endregion
		#endregion


		#region Constructors and Destructors
		public VlcMediaPlayer()
		{
			instance = new VlcInstance();
			Handle = LibVlc.libvlc_media_player_new(instance.Handle);
			IsKeyInputConsumed = false;
			IsMouseInputConsumed = false;
			RegisterEvents();
		}

        public VlcMediaPlayer(VlcMedia media)
        {            
            Handle = LibVlc.libvlc_media_player_new_from_media(media.Handle);
            IsKeyInputConsumed = false;
            IsMouseInputConsumed = false;
        }

        public void Dispose()
		{
			DeregisterEvents();
			LibVlc.libvlc_media_player_release(Handle);
			if (instance != null)
				instance.Dispose();
        }
        #endregion


		#region Event (De)Registering
		private void RegisterEvents()
		{
			EventManager = LibVlc.libvlc_media_player_event_manager(Handle);
			vlcLengthChangedDelegate = new LibVlc.EventCallbackDelegate(VideoLengthChanged);
			LibVlc.libvlc_event_attach(EventManager, LibVlc.libvlc_event_type_t.libvlc_MediaPlayerLengthChanged,
				vlcLengthChangedDelegate, IntPtr.Zero);
			vlcPositionChangedDelegate = new LibVlc.EventCallbackDelegate(VideoPositionChanged);
			LibVlc.libvlc_event_attach(EventManager, LibVlc.libvlc_event_type_t.libvlc_MediaPlayerPositionChanged,
				vlcPositionChangedDelegate, IntPtr.Zero);
			vlcTimeChangedDelegate = new LibVlc.EventCallbackDelegate(VideoTimeChanged);
			LibVlc.libvlc_event_attach(EventManager, LibVlc.libvlc_event_type_t.libvlc_MediaPlayerTimeChanged,
				vlcTimeChangedDelegate, IntPtr.Zero);
		}

		private void DeregisterEvents()
		{
			/*
			LibVlc.libvlc_event_detach(EventManager, LibVlc.libvlc_event_type_t.libvlc_MediaPlayerLengthChanged,
				vlcLengthChangedDelegate, this.Handle);
			LibVlc.libvlc_event_detach(EventManager, LibVlc.libvlc_event_type_t.libvlc_MediaPlayerPositionChanged,
				vlcPositionChangedDelegate, this.Handle);
			LibVlc.libvlc_event_detach(EventManager, LibVlc.libvlc_event_type_t.libvlc_MediaPlayerTimeChanged,
				vlcTimeChangedDelegate, this.Handle);
			*/
		}
		#endregion


		#region Playback Control
		public void SetMedia(VlcMedia media)
		{
			LibVlc.libvlc_media_player_set_media(Handle, media.Handle);
		}

		public void SetMediaFile(string path)
		{
			if (!File.Exists(path))
				return;
			using (VlcMedia media = VlcMedia.CreateFromFilepath(instance, path))
			{
				SetMedia(media);
			}
		}

        public void Play()
        {
			LibVlc.libvlc_media_player_play(Handle);

            Status = PlayingStatus.PLAYING;

            if (PlayingStatusChanged != null)
                PlayingStatusChanged(this, new PlayingStatusEventArgs(Status));
        }

        public void Pause()
        {
            if (Status != PlayingStatus.PLAYING)
                return;

            LibVlc.libvlc_media_player_pause(Handle);

            Status = PlayingStatus.PAUSED;

            if (PlayingStatusChanged != null)
                PlayingStatusChanged(this, new PlayingStatusEventArgs(Status));
        }

        public void Stop()
        {
            LibVlc.libvlc_media_player_stop(Handle);

            Status = PlayingStatus.STOPPED;

            if (PlayingStatusChanged != null)
                PlayingStatusChanged(this, new PlayingStatusEventArgs(Status));
        }
        #endregion


        #region Sound Control
        public void SetVolume(double volume)
        {
			int volumePercent = (int)Math.Round(volume * 100);
			LibVlc.libvlc_audio_set_volume(Handle, volumePercent);
        }
        #endregion


		#region VLC Event Handlers
		private string GetLongTimeAsString(long time)
		{
			long totalSeconds = time / 1000;
			long hours = totalSeconds / 3600;
			long minutes = (totalSeconds - hours * 3600) / 60;
			long seconds = totalSeconds - hours * 3600 - minutes * 60;
			return ((hours > 0) ? (hours.ToString() + ":") : "") + 
				minutes.ToString("0#") + ":" + seconds.ToString("0#");
		}

		private void VideoLengthChanged(IntPtr userdata)
		{
			Length = LibVlc.libvlc_media_player_get_length(this.Handle);

			LengthString = GetLongTimeAsString(Length);

			if (LengthChanged != null)
				LengthChanged(this, new EventArgs());
		}

		private void VideoPositionChanged(IntPtr userdata)
		{
			if (PositionChanged != null)
				PositionChanged(this, new EventArgs());
		}

		private void VideoTimeChanged(IntPtr userdata)
		{
			TimeString = GetLongTimeAsString(Time);

			if (TimeChanged != null)
				TimeChanged(this, new EventArgs());
		}
		#endregion
	}
}
