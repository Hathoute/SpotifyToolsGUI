using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpotifyToolsGUI.Tools.Helpers {
    class WindowsEvents {

        static WindowsEvents() {
        }

        public static WindowsEvents Instance { get; set; } = new WindowsEvents();

        /// <summary>
        /// The spotify process which we are listening to.
        /// </summary>
        public uint SpotifyProcessId;

        /// <summary>
        /// Title change action.
        /// The string passed is the new window title.
        /// </summary>
        public Action<string> TitleChange;

        /// <summary>
        /// Title change hook.
        /// </summary>
        public IntPtr TitleChangeHook { get; private set; }

        private bool IsInitialized = false;

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
        /// Starts listening to the namechange event.
        /// </summary>
        public bool Initialize() {
            if (IsInitialized)
                return true;

            var process = Process.GetProcessesByName("Spotify").FirstOrDefault(x => !x.MainWindowTitle.Equals(""));
            if (process == null) {
                MessageBox.Show("Could not find the Spotify process");
                return false;
            }
            SpotifyProcessId = (uint)process.Id;

            if (TitleChangeHook != IntPtr.Zero) {
                return false;
            }

            // Listen for foreground changes across all processes/threads on current desktop...
            TitleChangeHook = SetWinEventHook(EVENT_OBJECT_NAMECHANGE, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero,
                    procDelegate, SpotifyProcessId, 0, WINEVENT_OUTOFCONTEXT);

            IsInitialized = true;
            return true;
        }

        /// <summary>
        /// Stops listening to the namechange event.
        /// </summary>
        public void Stop() {
            UnhookWinEvent(TitleChangeHook);
            TitleChangeHook = IntPtr.Zero;
            IsInitialized = false;
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
            var process = Process.GetProcessById((int)Instance.SpotifyProcessId);
            if (process == null) {
                // Process not found, probably the spotify client was killed while the app is open.
                MessageBox.Show("Could not find the Spotify process.\n");
                Instance.Stop();
            }

            Instance.TitleChange?.Invoke(process.MainWindowTitle);
        }
    }
}
