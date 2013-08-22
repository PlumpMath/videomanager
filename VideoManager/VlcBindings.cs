using System;
using System.Runtime.InteropServices;

namespace VideoManager
{
    static class LibVlc
    {
        #region core
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libvlc_new(int argc,
            [MarshalAs(UnmanagedType.LPArray,
             ArraySubType = UnmanagedType.LPStr)] string[] argv);

        [DllImport("libvlc")]
        public static extern void libvlc_release(IntPtr instance);
        #endregion

        #region media
        [DllImport("libvlc")]
        public static extern IntPtr libvlc_media_new(IntPtr p_instance,
          [MarshalAs(UnmanagedType.LPStr)] string psz_mrl);

        [DllImport("libvlc")]
        public static extern void libvlc_media_release(IntPtr p_meta_desc);
        #endregion

        #region media player
        [DllImport("libvlc")]
        public static extern IntPtr libvlc_media_player_new_from_media(IntPtr media);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_release(IntPtr player);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_set_drawable(IntPtr player, IntPtr drawable);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_play(IntPtr player);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_pause(IntPtr player);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_stop(IntPtr player);
        #endregion
    }
}