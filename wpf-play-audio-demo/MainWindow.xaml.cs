using System.Windows;
using wpf_play_audio_demo.PopView;

namespace wpf_play_audio_demo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PlayMusicByLocal(object sender, RoutedEventArgs e)
        {
            new PlayLocalAudioView().ShowDialog();
        }

        private void PlayMusicByHttp(object sender, RoutedEventArgs e)
        {
            new PlayHttpAudioView().ShowDialog();
        }
    }
}