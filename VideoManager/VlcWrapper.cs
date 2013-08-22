using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoManager
{
    class VlcInstance : IDisposable
    {
        internal IntPtr Handle;

        public VlcInstance()
        {
            // init VLC            
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

        public VlcMedia(VlcInstance instance, string url)
        {            
            Handle = LibVlc.libvlc_media_new(instance.Handle, url);
        }

        public void Dispose()
        {
            LibVlc.libvlc_media_release(Handle);
        }
    }

    class VlcMediaPlayer : IDisposable
    {
        internal IntPtr Handle;
        private IntPtr drawable;
        private bool playing, paused;

        public VlcMediaPlayer(VlcMedia media)
        {            
            Handle = LibVlc.libvlc_media_player_new_from_media(media.Handle);
        }

        public void Dispose()
        {
            LibVlc.libvlc_media_player_release(Handle);
        }

        public IntPtr Drawable
        {
            get
            {
                return drawable;
            }
            set
            {
                LibVlc.libvlc_media_player_set_drawable(Handle, value);
                drawable = value;
            }
        }

        public bool IsPlaying { get { return playing && !paused; } }

        public bool IsPaused { get { return playing && paused; } }

        public bool IsStopped { get { return !playing; } }

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
    }
}
