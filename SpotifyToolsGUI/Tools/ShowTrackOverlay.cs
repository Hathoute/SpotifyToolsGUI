using SpotifyAPI.Web;
using SpotifyToolsGUI.Tools.Helpers;
using SpotifyToolsGUI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// The Window that called the MuteOnAdvertisement.
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

        public void Start() {
            OverlayWindow = new TrackOverlay();
            OverlayWindow.Show();
            OverlayWindow.SetOverlayPosition(-10, 10);

            WindowsEvents.Instance.TitleChange += OnTitleChange;
        }

        public void Stop() {
            OverlayWindow.Close();
        }

        public async void OnTitleChange(string newTitle) {
            if (newTitle == PreviousTitle || !newTitle.Contains("-"))
                return;

            //TODO: Implement access token.
            var client = new SpotifyClient("AccessToken");
            var request = new SearchRequest(SearchRequest.Types.Track, newTitle);
            var response = await client.Search.Item(request);

            if(response.Tracks.Total == 0)
                return;

            var track = response.Tracks.Items.First();
            
        }

        // TODO: Let the user choose the overlay position.
        public void OnOverlayPositionChanged() {
        }
    }
}
