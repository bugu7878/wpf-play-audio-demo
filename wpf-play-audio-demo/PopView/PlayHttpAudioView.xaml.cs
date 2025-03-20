using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace wpf_play_audio_demo.PopView
{
    public partial class PlayHttpAudioView : Window
    {
        public PlayHttpAudioView()
        {
            InitializeComponent();
            Init();
        }

        private bool _isPlaying;
        private bool _isUserDragging;
        private DispatcherTimer _dispatcherTimer;

        // 初始化
        private void Init()
        {
            _isPlaying = false;
            _isUserDragging = false;

            // 更新进度条、播放时间
            _dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _dispatcherTimer.Tick += DispatcherTimerCallBack;
        }

        // 设置音源：修改 HTTP 地址
        private void SetMediaElementSource()
        {
            mediaElement.Source = new Uri("http://192.168.1.190:90/home/20250306142907_8538_8532.wav");
        }

        // 定时器回调：进度条，播放时间
        private void DispatcherTimerCallBack(object sender, EventArgs e)
        {
            // 防御性编程检查
            if (!mediaElement.NaturalDuration.HasTimeSpan) return;

            // 更新进度条
            if (!_isUserDragging)
            {
                var nowTimeSeconds = mediaElement.Position.TotalSeconds; // 获取媒体当前播放位置的秒数
                var totalTimeSeconds = mediaElement.NaturalDuration.TimeSpan.TotalSeconds; // 获取媒体总秒数
                sliderProgress.Value = (nowTimeSeconds / totalTimeSeconds) * 100;
            }

            // 更新播放时间
            var timeSpan = mediaElement.Position;
            txtCurrentTime.Text = $"{(int)timeSpan.TotalMinutes:00}:{timeSpan.Seconds:00}";
        }

        // 倍速播放选择
        private void CmbSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 防御性编程检查
            if (mediaElement == null) return;

            // 设置倍速
            var indexToSpeedMap = new Dictionary<int, double>
            {
                { 0, 0.5 },
                { 1, 1.0 },
                { 2, 1.5 },
                { 3, 2.0 }
            };
            mediaElement.SpeedRatio = indexToSpeedMap[cmbSpeed.SelectedIndex];
        }

        // 播放暂停按钮
        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            // 防御性编程检查：检查媒体源是否存在
            if (mediaElement.Source == null)
            {
                SetMediaElementSource();
            }

            // 暂停 -> 播放
            if (mediaElement.IsLoaded && !_isPlaying)
            {
                mediaElement.Play();
                btnPlayPause.Content = "暂停";
            }

            // 播放 -> 暂停
            if (mediaElement.IsLoaded && _isPlaying)
            {
                mediaElement.Pause();
                btnPlayPause.Content = "播放";
            }

            _isPlaying = !_isPlaying;
        }

        // 开始拖动进度条
        private void SliderProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isUserDragging = true;
        }

        // 停止拖动进度条
        private void SliderProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isUserDragging = false;
        }

        // 进度条的值发生改变
        private void SliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // 防御性编程检查
            if (!_isUserDragging || !mediaElement.NaturalDuration.HasTimeSpan) return;

            // 改变进度条的值
            var sliderProgressValue = sliderProgress.Value / 100; // 播放进度比例转换为百分之多少
            var totalTimeSeconds = mediaElement.NaturalDuration.TimeSpan.TotalSeconds; // 获取媒体总秒数
            mediaElement.Position = TimeSpan.FromSeconds(sliderProgressValue * totalTimeSeconds);

            // TODO 如果直接将进度条拖拽到最后，会有异常情况出现
        }

        // 音量条的值发生改变
        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement != null)
            {
                mediaElement.Volume = sliderVolume.Value;
            }
        }

        // 控件成功加载指定媒体文件并开始播放时触发
        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                sliderProgress.Maximum = 100;

                var timeSpan = mediaElement.NaturalDuration.TimeSpan;
                txtTotalTime.Text = $"{(int)timeSpan.TotalMinutes:00}:{timeSpan.Seconds:00}";
            }

            _dispatcherTimer.Start();
        }

        // 控件播放完当前的媒体文件时触发
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            txtCurrentTime.Text = "00:00";
            sliderProgress.Value = 0;
            btnPlayPause.Content = "播放";
            mediaElement.Stop();
            _dispatcherTimer.Stop();
            _isPlaying = false;
            mediaElement.Source = null;
        }
    }
}