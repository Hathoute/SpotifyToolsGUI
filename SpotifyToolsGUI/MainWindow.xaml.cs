using SpotifyToolsGUI.Tools;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotifyToolsGUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            MuteOnAdvertisement.Instance.GUIWindow = this;
            ShowTrackOverlay.Instance.GUIWindow = this;

            // Automatically start MuteAdvertisement.
            MuteOnAdvertisement.Instance.Start();

            // Automatically start ShowTrackOverlay.
            ShowTrackOverlay.Instance.Start();
        }

        private void BtnToggleMute_Click(object sender, RoutedEventArgs e) {
            if (!MuteOnAdvertisement.Instance.Enabled)
                MuteOnAdvertisement.Instance.Start();
            else
                MuteOnAdvertisement.Instance.Stop();
        }
    }
}
