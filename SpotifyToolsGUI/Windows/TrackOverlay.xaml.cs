using System;
using System.Collections.Generic;
using System.IO;
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
using Windows.Graphics.Capture;
using Windows.Media.Control;
using SpotifyToolsGUI.Tools.Helpers;

namespace SpotifyToolsGUI.Windows {
    /// <summary>
    /// Interaction logic for TrackOverlay.xaml
    /// </summary>
    public partial class TrackOverlay : Window {
        public TrackOverlay() {
            InitializeComponent();
            Topmost = true;
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

        public void SetOverlayData(GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties) {
            lblTitle.Content = mediaProperties.Title;
            lblArtist.Content = mediaProperties.Artist + " • " + mediaProperties.AlbumTitle;
            imgAlbum.Source = CropBitmapImage(MediaControl.GetBitmapImageFromStream(mediaProperties.Thumbnail));
            imgAlbum.Stretch = Stretch.UniformToFill;
        }

        public void SetOverlayData(string title, string artist, BitmapImage bmp) {
            lblTitle.Content = title;
            lblArtist.Content = artist;
            imgAlbum.Source = bmp;
        }

        private ImageSource CropBitmapImage(BitmapImage img) {
            var cropRectangle = new Int32Rect(33, 0, 234, 234);
            return new CroppedBitmap(img, cropRectangle);
        }
    }
}
