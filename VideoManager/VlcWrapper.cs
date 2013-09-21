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
        private IntPtr drawable;
        private bool isKeyInputConsumed, isMouseInputConsumed;
        public enum PlayingStatus { PLAYING, PAUSED, STOPPED }
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

        public int Length
        {
            get
            {
                return (int)LibVlc.libvlc_media_player_get_length(Handle);
            }
        }

        public int Time
        {
            get
            {
                return (int)LibVlc.libvlc_media_player_get_time(Handle);
            }
        }

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

        #region Constructors and Destructors
        public VlcMediaPlayer(VlcMedia media)
        {            
            Handle = LibVlc.libvlc_media_player_new_from_media(media.Handle);
            IsKeyInputConsumed = false;
            IsMouseInputConsumed = false;
        }

        public void Dispose()
        {
            LibVlc.libvlc_media_player_release(Handle);
        }
        #endregion

        #region Playback Control
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
        public void SetVolume(int volume)
        {
            LibVlc.libvlc_audio_set_volume(Handle, volume);
        }
        #endregion
    }
}
