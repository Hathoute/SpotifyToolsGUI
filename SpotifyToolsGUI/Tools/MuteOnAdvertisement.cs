using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using SpotifyToolsGUI.Tools.Helpers;
using System.Windows.Media;

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
        /// Mute on advertisement state.
        /// </summary>
        public bool Enabled = false;
        
        /// <summary>
        /// Starts listening to the namechange event and mutes whenever an advertisement is played.
        /// </summary>
        public void Start() {
            if (!WindowsEvents.Instance.Initialize())
                return;

            WindowsEvents.Instance.TitleChange += OnTitleChange;
            ChangeMuteState(true);
        }

        /// <summary>
        /// Stops listening to the namechange event, advertisement will not be muted after calling this.
        /// </summary>
        public void Stop() {
            WindowsEvents.Instance.TitleChange -= OnTitleChange;
            ChangeMuteState(false);
        }

        void ChangeMuteState(bool start) {
            Enabled = start;
            GUIWindow.txtMuteStatus.Text = start ? "Enabled" : "Disabled";
            GUIWindow.txtMuteStatus.Foreground = start ? Brushes.Green : Brushes.Red;
            GUIWindow.btnToggleMute.Content = start ? "Disable" : "Enable";
        }

        private void OnTitleChange(string windowTitle) {
            Instance.GUIWindow.txtWindowTitle.Text = windowTitle;
            var mute = VolumeMixer.GetApplicationMute((int)WindowsEvents.Instance.SpotifyProcessId);

            if (!(windowTitle.Contains("-") || windowTitle.Contains(" ") || windowTitle.Equals("Spotify"))) {
                // This is an advertisement.
                // Used this condition to cover probably all the major languages (EN, FR, AR, ...)
                VolumeMixer.SetApplicationMute((int)WindowsEvents.Instance.SpotifyProcessId, true);
            }
            else if(mute.HasValue && mute.Value) {
                VolumeMixer.SetApplicationMute((int)WindowsEvents.Instance.SpotifyProcessId, false);
            }
        }
    }
}
