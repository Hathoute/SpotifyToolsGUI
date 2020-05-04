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

            // Automatically start MuteAdvertisement.
            OnMuteStateChanged(MuteOnAdvertisement.Instance.Start());
        }

        private void BtnToggleMute_Click(object sender, RoutedEventArgs e) {
            if(!MuteOnAdvertisement.Instance.Enabled) {
                var result = MuteOnAdvertisement.Instance.Start();
                OnMuteStateChanged(result);
            }
            else {
                MuteOnAdvertisement.Instance.Stop();
                OnMuteStateChanged(false);
            }
        }

        void OnMuteStateChanged(bool start) {
            txtMuteStatus.Text = start ? "Enabled" : "Disabled";
            txtMuteStatus.Foreground = start ? Brushes.Green : Brushes.Red;
            btnToggleMute.Content = start ? "Disable" : "Enable";
        }
    }
}
