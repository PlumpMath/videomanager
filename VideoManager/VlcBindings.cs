using System;
using System.Runtime.InteropServices;

namespace VideoManager
{
    static class LibVlc
    {
        #region Core
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libvlc_new(
            int argc, 
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] argv);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_release(IntPtr instance);
        #endregion


        #region Media
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libvlc_media_new_location(
            IntPtr p_instance,
            [MarshalAs(UnmanagedType.LPStr)] string psz_mrl);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libvlc_media_new_path(
            IntPtr p_instance,
            [MarshalAs(UnmanagedType.LPStr)] string filepath);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libvlc_media_new_fd(IntPtr p_instance, int fd);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_media_release(IntPtr p_meta_desc);
        #endregion


		#region Media Player
		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr libvlc_media_player_new(IntPtr instance);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libvlc_media_player_new_from_media(IntPtr media);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_media_player_release(IntPtr player);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_media_player_set_hwnd(IntPtr player, IntPtr drawable);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern void libvlc_media_player_set_media(IntPtr player, IntPtr media);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_media_player_play(IntPtr player);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_media_player_pause(IntPtr player);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_media_player_stop(IntPtr player);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern long libvlc_media_player_get_time(IntPtr player);        // int64

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern long libvlc_media_player_get_length(IntPtr player);        // int64

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern float libvlc_media_player_get_position(IntPtr player);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_media_player_set_position(IntPtr player, float pos);
        #endregion


        #region Video
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_video_set_key_input(IntPtr player, bool vlcInterceptsKeyInput);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libvlc_video_set_mouse_input(IntPtr player, bool vlcInterceptsKeyInput);
        #endregion


        #region Audio
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr libvlc_audio_set_volume(IntPtr player, int volume);
        #endregion


		#region Events
		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr libvlc_media_player_event_manager(IntPtr player);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void EventCallbackDelegate(IntPtr userdata);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern int libvlc_event_attach(IntPtr event_manager, libvlc_event_type_t event_type,
			EventCallbackDelegate callback, IntPtr user_data);

		[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl)]
		public static extern void libvlc_event_detach(IntPtr event_manager, libvlc_event_type_t event_type,
			EventCallbackDelegate callback, IntPtr user_data);

		internal enum libvlc_event_type_t
		{
			libvlc_MediaMetaChanged = 0,
			libvlc_MediaSubItemAdded,
			libvlc_MediaDurationChanged,
			libvlc_MediaParsedChanged,
			libvlc_MediaFreed,
			libvlc_MediaStateChanged, 	
			libvlc_MediaSubItemTreeAdded, 	
			libvlc_MediaPlayerMediaChanged = 0x100, 	
			libvlc_MediaPlayerNothingSpecial, 	
			libvlc_MediaPlayerOpening,
			libvlc_MediaPlayerBuffering, 	
			libvlc_MediaPlayerPlaying,
			libvlc_MediaPlayerPaused, 	
			libvlc_MediaPlayerStopped,
			libvlc_MediaPlayerForward, 	
			libvlc_MediaPlayerBackward, 	
			libvlc_MediaPlayerEndReached,
			libvlc_MediaPlayerEncounteredError,
			libvlc_MediaPlayerTimeChanged,
			libvlc_MediaPlayerPositionChanged, 	
			libvlc_MediaPlayerSeekableChanged,
			libvlc_MediaPlayerPausableChanged,
			libvlc_MediaPlayerTitleChanged, 	
			libvlc_MediaPlayerSnapshotTaken, 	
			libvlc_MediaPlayerLengthChanged, 	
			libvlc_MediaPlayerVout,
			libvlc_MediaListItemAdded = 0x200, 	
			libvlc_MediaListWillAddItem, 	
			libvlc_MediaListItemDeleted, 	
			libvlc_MediaListWillDeleteItem,
			libvlc_MediaListViewItemAdded = 0x300, 	
			libvlc_MediaListViewWillAddItem, 	
			libvlc_MediaListViewItemDeleted, 	
			libvlc_MediaListViewWillDeleteItem,
			libvlc_MediaListPlayerPlayed = 0x400, 	
			libvlc_MediaListPlayerNextItemSet, 	
			libvlc_MediaListPlayerStopped,
			libvlc_MediaDiscovererStarted = 0x500, 	
			libvlc_MediaDiscovererEnded,
			libvlc_VlmMediaAdded = 0x600, 	
			libvlc_VlmMediaRemoved, 	
			libvlc_VlmMediaChanged,
			libvlc_VlmMediaInstanceStarted,
			libvlc_VlmMediaInstanceStopped,
			libvlc_VlmMediaInstanceStatusInit,	
			libvlc_VlmMediaInstanceStatusOpening, 	
			libvlc_VlmMediaInstanceStatusPlaying, 	
			libvlc_VlmMediaInstanceStatusPause, 	
			libvlc_VlmMediaInstanceStatusEnd, 	
			libvlc_VlmMediaInstanceStatusError 
		}
		#endregion
	}
}