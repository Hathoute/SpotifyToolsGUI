using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;

namespace SpotifyToolsGUI.Tools {
    class MuteOnAdvertisement {

        /// <summary>
        /// Mutes the spotify client whenever it plays an advertisement.
        /// </summary>
        static MuteOnAdvertisement() {
        }

        public static MuteOnAdvertisement Instance { get; } = new MuteOnAdvertisement();

        /// <summary>
        /// The Window that called the MuteOnAdvertisement.
        /// </summary>
        public MainWindow GUIWindow;

        /// <summary>
        /// The spotify process which we are listening to.
        /// </summary>
        public uint SpotifyProcessId;

        /// <summary>
        /// Changename hook.
        /// </summary>
        public IntPtr Hook { get; private set; }

        public bool Enabled => Hook != IntPtr.Zero;

        // Delegate and imports from pinvoke.net:
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
                IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
                hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
                uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        static extern bool GetWindowThreadProcessId(IntPtr hWinEventHook);

        // Constants from winuser.h
        const uint EVENT_OBJECT_NAMECHANGE = 0x800C; // hwnd ID idChild is item w/ name change
        const uint WINEVENT_OUTOFCONTEXT = 0;

        // Need to ensure delegate is not collected while we're using it,
        // storing it in a class field is simplest way to do this.
        static WinEventDelegate procDelegate = new WinEventDelegate(WinEventProc);
        
        /// <summary>
        /// Starts listening to the namechange event and mutes whenever an advertisement is played.
        /// </summary>
        public bool Start() {
            var process = Process.GetProcessesByName("Spotify").FirstOrDefault(x => !x.MainWindowTitle.Equals(""));
            if (process == null) {
                MessageBox.Show("Could not find the Spotify process");
                return false;
            }
            SpotifyProcessId = (uint)process.Id;

            if (Hook != IntPtr.Zero) {
                return false;
            }

            // Listen for foreground changes across all processes/threads on current desktop...
            Hook = SetWinEventHook(EVENT_OBJECT_NAMECHANGE, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero,
                    procDelegate, SpotifyProcessId, 0, WINEVENT_OUTOFCONTEXT);

            return true;
        }

        /// <summary>
        /// Stops listening to the namechange event, advertisement will not be muted after calling this.
        /// </summary>
        public bool Stop() {
            if(Hook == IntPtr.Zero) {
                return false;
            }

            UnhookWinEvent(Hook);
            Hook = IntPtr.Zero;

            return true;
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
            var process = Process.GetProcessById((int)Instance.SpotifyProcessId);
            if(process == null) {
                // Process not found, probably the spotify client was killed while the app is open.
                MessageBox.Show("Could not find the Spotify process.\n" +
                    "Mute Advertisements is now turned off.");
                Instance.Stop();
            }

            var mute = VolumeMixer.GetApplicationMute(process.Id);

            Instance.GUIWindow.txtWindowTitle.Text = process.MainWindowTitle;

            if (!(process.MainWindowTitle.Contains("-") || process.MainWindowTitle.Contains(" "))) {
                // This is an advertisement.
                // Used this condition to cover probably all the major languages (EN, FR, AR, ...)
                VolumeMixer.SetApplicationMute(process.Id, true);
            }
            else if(mute.HasValue && mute.Value) {
                VolumeMixer.SetApplicationMute(process.Id, false);
            }
        }
    }
}
