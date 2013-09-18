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
        #region Fields and Properties
        internal IntPtr Handle;
        private IntPtr drawable;
        private bool playing, paused, isKeyInputConsumed, isMouseInputConsumed;
        
        public bool IsPlaying { get { return playing && !paused; } }

        public bool IsPaused { get { return playing && paused; } }

        public bool IsStopped { get { return !playing; } }
        
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

            playing = true;
            paused = false;
        }

        public void Pause()
        {            
            LibVlc.libvlc_media_player_pause(Handle);

            if (playing)
                paused ^= true;
        }

        public void Stop()
        {            
            LibVlc.libvlc_media_player_stop(Handle);

            playing = false;
            paused = false;
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
