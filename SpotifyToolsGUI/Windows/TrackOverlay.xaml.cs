using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotifyToolsGUI.Windows {
    /// <summary>
    /// Interaction logic for TrackOverlay.xaml
    /// </summary>
    public partial class TrackOverlay : Window {
        public TrackOverlay() {
            InitializeComponent();
        }

        /// <summary>
        /// Specifies the location for the current overlay.
        /// <br/>A positive x means that the overlay's position will be at the left, a negative x means at the right.
        /// <br/>A positive y means that the overlay's position will be at the top, a negative y means at the bottom.
        /// </summary>
        /// <param name="x">Vertical margin in pixels from bounds</param>
        /// <param name="y">Horizontal margin in pixels from bounds</param>
        public void SetOverlayPosition(int x, int y) {
            if (x > 0)
                this.Left = x;
            else
                this.Left = SystemParameters.PrimaryScreenWidth - this.Width + x;

            if (y > 0)
                this.Top = y;
            else
                this.Top = SystemParameters.PrimaryScreenHeight - this.Height + y;
        }

        public void SetOverlayData(SpotifyAPI.Web.FullTrack track) {
            lblTitle.Content = track.Name;
            lblArtist.Content = track.Artists[0] + " • " + track.Album.Name;
            imgAlbum.Source = new BitmapImage(new Uri(track.Album.Images[0].Url));
        }

        public void SetOverlayData(string title, string artist) {
            lblTitle.Content = title;
            lblArtist.Content = artist;
            imgAlbum.Source = null;
        }
    }
}
