using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoManager
{
    class VlcInstance : IDisposable
    {
        internal IntPtr Handle;

        public VlcInstance(string[] args)
        {
            // init DLL path
            /*string dllDir = AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + "libs";
            LibVlc.SetDllDirectory(dllDir);*/

            // init VLC
            VlcException ex = new VlcException();
            Handle = LibVlc.libvlc_new(args.Length, args, ref ex.Ex);
            if (ex.IsRaised) throw ex;
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
            VlcException ex = new VlcException();
            Handle = LibVlc.libvlc_media_new(instance.Handle, url, ref ex.Ex);
            if (ex.IsRaised) throw ex;
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
            VlcException ex = new VlcException();
            Handle = LibVlc.libvlc_media_player_new_from_media(media.Handle, ref ex.Ex);
            if (ex.IsRaised) throw ex;
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
                VlcException ex = new VlcException();
                LibVlc.libvlc_media_player_set_drawable(Handle, value, ref ex.Ex);
                if (ex.IsRaised) throw ex;
                drawable = value;
            }
        }

        public bool IsPlaying { get { return playing && !paused; } }

        public bool IsPaused { get { return playing && paused; } }

        public bool IsStopped { get { return !playing; } }

        public void Play()
        {
            VlcException ex = new VlcException();
            LibVlc.libvlc_media_player_play(Handle, ref ex.Ex);
            if (ex.IsRaised) throw ex;

            playing = true;
            paused = false;
        }

        public void Pause()
        {
            VlcException ex = new VlcException();
            LibVlc.libvlc_media_player_pause(Handle, ref ex.Ex);
            if (ex.IsRaised) throw ex;

            if (playing)
                paused ^= true;
        }

        public void Stop()
        {
            VlcException ex = new VlcException();
            LibVlc.libvlc_media_player_stop(Handle, ref ex.Ex);
            if (ex.IsRaised) throw ex;

            playing = false;
            paused = false;
        }
    }

    class VlcException : Exception
    {
        internal libvlc_exception_t Ex;

        public VlcException()
            : base()
        {
            Ex = new libvlc_exception_t();
            LibVlc.libvlc_exception_init(ref Ex);
        }

        public bool IsRaised { get { return LibVlc.libvlc_exception_raised(ref Ex) != 0; } }

        public override string Message { get { return LibVlc.libvlc_exception_get_message(ref Ex); } }
    }
}
