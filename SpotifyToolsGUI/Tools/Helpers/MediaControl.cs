using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Media.Control;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SpotifyToolsGUI.Tools.Helpers {
    public class MediaControl {

        public static GlobalSystemMediaTransportControlsSession GetSession() {
            var sessions = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
            return sessions.GetSessions()
                .FirstOrDefault(x => x.SourceAppUserModelId == "Spotify.exe");
        }

        public static GlobalSystemMediaTransportControlsSessionMediaProperties GetMediaProperties(GlobalSystemMediaTransportControlsSession session) {
            var sessions = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
            var s = sessions.GetSessions()
                .FirstOrDefault(x => x.SourceAppUserModelId == "Spotify.exe");
            return s.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
        }

        public static BitmapImage GetBitmapImageFromStream(IRandomAccessStreamReference randomStream) {
            var asyncOperation = randomStream.OpenReadAsync().GetAwaiter().GetResult();
            var stream = asyncOperation.AsStream();
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }
}
