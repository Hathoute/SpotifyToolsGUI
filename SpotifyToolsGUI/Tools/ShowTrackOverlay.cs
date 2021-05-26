using SpotifyAPI.Web;
using SpotifyToolsGUI.Tools.Helpers;
using SpotifyToolsGUI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.Media.Control;

namespace SpotifyToolsGUI.Tools {
    class ShowTrackOverlay {

        /// <summary>
        /// Shows the track overlay.
        /// The Track overlay is a fancy way to show current playing song at every instance.
        /// </summary>
        static ShowTrackOverlay() {
        }

        public static ShowTrackOverlay Instance { get; } = new ShowTrackOverlay();

        /// <summary>
        /// Current state of the overlay.
        /// </summary>
        public bool Running {
            get => _running;
            private set {
                _running = value;
                ChangeOverlayState();
            }
        }

        private bool _running;

        /// <summary>
        /// The Window that called the Overlay.
        /// </summary>
        public MainWindow GUIWindow;

        /// <summary>
        /// The overlay window that is displayed to the user.
        /// </summary>
        public TrackOverlay OverlayWindow;

        /// <summary>
        /// Holds the title of the previously displayed track.
        /// </summary>
        private string PreviousTitle = null;

        /// <summary>
        /// The current session of Spotify's MediaControlsSession.
        /// </summary>
        private GlobalSystemMediaTransportControlsSession CurrentSession;

        /// <summary>
        /// Tells whether the Overlay has to be updated with new song data.
        /// </summary>
        private bool ShouldUpdate = true;

        private Thread UpdateThread;

        public void Start() {
            OverlayWindow = new TrackOverlay();
            OverlayWindow.Show();
            OverlayWindow.SetOverlayPosition(-10, 10);

            if (ConfigManager.UseWindowsMediaSessions) {
                CurrentSession = MediaControl.GetSession();
                if (CurrentSession is null) {
                    MessageBox.Show("Could not get the current media session of Spotify. Try again later.");
                    return;
                }

                CurrentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
                ShouldUpdate = true;
                UpdateThread = new Thread(UpdateLoop);
                UpdateThread.Start();
            }
            else {
                WindowsEvents.Instance.TitleChange += OnTitleChange;
            }

            Running = true;
        }

        public void Stop() {
            if (ConfigManager.UseWindowsMediaSessions) {
                CurrentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
                UpdateThread.Abort();
            }
            else {
                WindowsEvents.Instance.TitleChange -= OnTitleChange;
            }

            OverlayWindow.Close();
            Running = false;
        }

        private void ChangeOverlayState() {
            GUIWindow.txtOverlayStatus.Text = Running ? "Enabled" : "Disabled";
            GUIWindow.txtOverlayStatus.Foreground = Running ? Brushes.Green : Brushes.Red;
            GUIWindow.btnToggleOverlay.Content = Running ? "Disable" : "Enable";
        }

        private void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args) {
            ShouldUpdate = true;
        }

        private void UpdateLoop() {
            while (Thread.CurrentThread.IsAlive) {
                Thread.Sleep(1000);

                if (!ShouldUpdate)
                    continue;

                var mediaProperties = MediaControl.GetMediaProperties(CurrentSession);
                OverlayWindow.Dispatcher.Invoke(() => {
                    OverlayWindow.SetOverlayData(mediaProperties);
                });
                ShouldUpdate = false;
            }
        }

        public void OnTitleChange(string newTitle) {
            if (newTitle == PreviousTitle || !newTitle.Contains("-"))
                return;

            var mediaProperties = MediaControl.GetMediaProperties(CurrentSession);
            OverlayWindow.SetOverlayData(mediaProperties);
        }

        // TODO: Let the user choose the overlay position.
        public void OnOverlayPositionChanged() {
        }
    }
}
