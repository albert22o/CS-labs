using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

using Microsoft.Win32;

namespace Lab_No6_Video
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly DispatcherTimer _timer;
		private TimeSpan _totalTime;

		public MainWindow()
		{
			InitializeComponent();

			_timer = new DispatcherTimer();
			_timer.Tick += new EventHandler(Timer_Tick);
			_timer.Interval = new TimeSpan(0, 0, 1);
			_timer.Start();

			VideoPause.IsEnabled = false;
			VideoStop.IsEnabled = false;
			VideoChronometrage.IsEnabled = false;
		}

		private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
		{
			VideoChronometrage.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
			VideoChronometrage.TickFrequency = 10;
			VideoPause.IsEnabled = true;
			VideoStop.IsEnabled = true;
			VideoChronometrage.IsEnabled = true;
			_totalTime = VideoPlayer.NaturalDuration.TimeSpan;
			TotalTime.Content = $"{_totalTime.Hours:D2}:{_totalTime.Minutes:D2}:{_totalTime.Seconds:D2}";
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			try
			{
				VideoChronometrage.Value = VideoPlayer.Position.TotalSeconds;
				CurrentTime.Content = $"{VideoPlayer.Position.Hours:D2}:{VideoPlayer.Position.Minutes:D2}:{VideoPlayer.Position.Seconds:D2}";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void VideoPlay_Click(object sender, RoutedEventArgs e)
		{
			if (VideoPlayer.IsLoaded) VideoPlayer.Play();
			_timer.Start();
		}

		private void VideoPause_Click(object sender, RoutedEventArgs e)
		{
			VideoPlayer.Pause();
			_timer.Stop();
		}

		private void VideoStop_Click(object sender, RoutedEventArgs e)
		{
			VideoPlayer.Stop();
			_timer.Stop();
			VideoChronometrage.Value = default;
			CurrentTime.Content = "00:00:00";
		}

		private void VideoChronometrage_ValueChanged(object sender, DragCompletedEventArgs e)
		{
			_timer.Stop();
			int sliderValue = (int)VideoChronometrage.Value;
			CurrentTime.Content = $"{sliderValue / 3600:D2}:{sliderValue / 60:D2}:{sliderValue % 60:D2}";
			TimeSpan currentPosition = new TimeSpan(0, 0, 0, sliderValue);
			VideoPlayer.Position = currentPosition;
			_timer.Start();
		}

		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			double sliderValue = (double)VolumeSlider.Value;
			VideoPlayer.Volume = sliderValue;
		}

		private void VideoOpen_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				OpenFileDialog fileDialog = new OpenFileDialog()
				{
					InitialDirectory = @"C:\Users\Nekitt\Documents\Учеба\ПиОГИ\Lab_No6_Video"
				};

				fileDialog.ShowDialog();
				VideoPlayer.Source = new Uri(fileDialog.FileName, UriKind.Relative);
				VideoName.Content = fileDialog.FileName;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}