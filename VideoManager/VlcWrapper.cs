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
                LibVlc.libvlc_media_player_set_hwnd(Handle, value);
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

        public void SetVolume(int volume)
        {
            LibVlc.libvlc_audio_set_volume(Handle, volume);
        }
    }
}
