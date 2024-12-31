using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Win32;

using NAudio.Wave;

namespace Lab_No6
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly List<MusicTrack> _loadedPlaylist, _initialPlaylist;

		private AudioFileReader? _mp3Reader;
		private WaveOutEvent? _outputDevice;

		private MusicTrack? _currentPlayingTrack;

		private readonly BitmapImage _playIconImage;
		private readonly BitmapImage _pauseIconImage;

		private readonly DispatcherTimer _trackTimer;
		private readonly Random _trackRandom;

		private bool _isRepeating;
		private bool _isShuffled;
		private bool _isPlayPressed;

		private int _tracksCount;

		private string _playlistDirectory;

		public MainWindow()
		{
			_loadedPlaylist = [];
			_initialPlaylist = [];
			
			_playIconImage = new(new Uri("/Icons/play-button.png", UriKind.Relative));
			_pauseIconImage = new(new Uri("/Icons/pause-button.png", UriKind.Relative));

			_trackTimer = new();
			_trackTimer.Tick += TrackTimer_Tick;
			_trackTimer.Interval = new TimeSpan(0, 0, 1);
			_trackRandom = new();

			InitializeComponent();

			PlayStopTrack.IsEnabled = false;
			PlayPrevTrack.IsEnabled = false;
			PlayNextTrack.IsEnabled = false;
			PlayShuffleTracks.IsEnabled = false;
			PlayRepeatTrack.IsEnabled = false;
			ResetTrack.IsEnabled = false;
			TrackChronometrage.IsEnabled = false;

			_playlistDirectory = string.Empty;
		}

		private static void Swap<T>(ref T first, ref T second)
			=> (second, first) = (first, second);

		private void InitializeTrack(int index)
		{
			_currentPlayingTrack = _loadedPlaylist[index];

			CurrentTrackPosition.Content = "00:00";

			ArtistLabel.Content = _currentPlayingTrack.ArtistName;
			TrackLabel.Content = _currentPlayingTrack.TrackName;
			TotalTrackLength.Content = string.Format(
				"{0:D2}:{1:D2}",
				_currentPlayingTrack.TrackLength.Minutes,
				_currentPlayingTrack.TrackLength.Seconds
				);
			AlbumCoverView.Source = _currentPlayingTrack.AlbumArt;
			AlbumName.Content = _currentPlayingTrack.AlbumName;

			if (_isPlayPressed)
				StartTrackPlaying();
		}

		private static void CopyPlaylist(List<MusicTrack> source, List<MusicTrack> destination)
		{
			foreach (var track in source)
				destination.Add(track);
		} 

		private void StartTrackPlaying()
		{
			if (_outputDevice == null)
			{
				_outputDevice = new WaveOutEvent();
				_outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;
			}

			if (_mp3Reader == null)
			{
				_mp3Reader = new AudioFileReader(_currentPlayingTrack!.PathToTrack.ToString());
				_outputDevice.Init(_mp3Reader);

				TrackChronometrage.Maximum = _mp3Reader.TotalTime.TotalSeconds;
				TrackChronometrage.Value = default;
			}

			TrackChronometrage.IsEnabled = true;
			_outputDevice.Play();
			_trackTimer.Start();
		}


		private void PauseTrackPlaying()
		{
			_outputDevice?.Pause();
			_trackTimer.Stop();
		}

		private void StopTrackPlaying()
		{
			_outputDevice!.Dispose();
			_outputDevice = null;
			_mp3Reader!.Dispose();
			_mp3Reader = null;

			_trackTimer.Stop();

			TrackChronometrage.IsEnabled = false;
			CurrentTrackPosition.Content = "00:00";
			TrackChronometrage.Value = default;
		}

		private void ShowPlaylist()
		{
			for (_tracksCount = default; _tracksCount < _loadedPlaylist.Count; ++_tracksCount)
				TrackList.Items.Add(
					$"{_tracksCount + 1}: {_loadedPlaylist[_tracksCount].ArtistName} - {_loadedPlaylist[_tracksCount].TrackName}, " +
					$"{string.Format("{0:D2}:{1:D2}", _loadedPlaylist[_tracksCount].TrackLength.Minutes, _loadedPlaylist[_tracksCount].TrackLength.Seconds)}"
					);
		}

		private void ShufflePlaylist()
		{
			for (int i = default; i < _loadedPlaylist.Count; ++i)
			{
				int j = _trackRandom.Next(0, _loadedPlaylist.Count);
				int k;

				while (true)
				{
					k = _trackRandom.Next(0, _loadedPlaylist.Count);

					if (j == k) continue;
					else break;
				}

				(MusicTrack m1, MusicTrack m2) = (_loadedPlaylist[j], _loadedPlaylist[k]);
				Swap(ref m1, ref m2);
				(_loadedPlaylist[j], _loadedPlaylist[k]) = (m1, m2);
			}
		}

		private void TrackTimer_Tick(object? sender, EventArgs e)
		{
			if (_outputDevice?.PlaybackState == PlaybackState.Playing && _mp3Reader != null)
			{
				TrackChronometrage.Value = _mp3Reader.CurrentTime.TotalSeconds;
				CurrentTrackPosition.Content = string.Format("{0:D2}:{1:D2}", _mp3Reader.CurrentTime.Minutes, _mp3Reader.CurrentTime.Seconds);
			}
		}

		private void OutputDevice_PlaybackStopped(object? sender, StoppedEventArgs e)
		{
			if (TrackChronometrage.Value + 3 >= TrackChronometrage.Maximum)
			{
				StopTrackPlaying();

				if (_isRepeating)
				{
					StartTrackPlaying();
				}
				else
				{
					++TrackList.SelectedIndex;
					_trackTimer.Start();
				}
			}
		}
		
		private void PlayStopTrack_Click(object sender, RoutedEventArgs e)
		{
			Button? current = sender as Button;

			if (!_isPlayPressed)
			{
				current!.Content = new Image
				{
					Source = _pauseIconImage,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Width = 64,
					Height = 64,
					Stretch = Stretch.Fill
				};
				PlayStopTrack.Background = Brushes.DodgerBlue;

				StartTrackPlaying();
				_isPlayPressed = true;
			}
			else
			{
				current!.Content = new Image
				{
					Source = _playIconImage,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Width = 64,
					Height = 64,
					Stretch = Stretch.Fill
				};
				PlayStopTrack.Background = Brushes.DimGray;

				PauseTrackPlaying();
				_isPlayPressed = false;
			}
		}

		private void PlayPrevTrack_Click(object sender, RoutedEventArgs e)
		{
			if (TrackList.SelectedIndex - 1 >= 0)
			{
				if (_isPlayPressed || _outputDevice?.PlaybackState == PlaybackState.Paused) StopTrackPlaying();
				
				--TrackList.SelectedIndex;
			}
		}

		private void PlayNextTrack_Click(object sender, RoutedEventArgs e)
		{
			if (TrackList.SelectedIndex < TrackList.Items.Count - 1)
			{
				if (_isPlayPressed || _outputDevice?.PlaybackState == PlaybackState.Paused) StopTrackPlaying();
				
				++TrackList.SelectedIndex;
			}
		}

		private void PlayShuffleTracks_Click(object sender, RoutedEventArgs e)
		{
			if (!_isShuffled)
			{
				_isShuffled = true;
				PlayShuffleTracks.Background = Brushes.BlueViolet;

				ShufflePlaylist();
				TrackList.Items.Clear();
				ShowPlaylist();
			}
			else
			{
				_isShuffled = false;
				PlayShuffleTracks.Background = Brushes.DodgerBlue;

				_loadedPlaylist.Clear();

				CopyPlaylist(_initialPlaylist, _loadedPlaylist);
				TrackList.Items.Clear();
				ShowPlaylist();
			}
		}

		private void PlayRepeatTrack_Click(object sender, RoutedEventArgs e)
		{
			if (_isRepeating)
			{
				_isRepeating = false;
				PlayNextTrack.IsEnabled = true;
				PlayPrevTrack.IsEnabled = true;
				PlayRepeatTrack.Background = Brushes.DodgerBlue;
			}
			else
			{
				_isRepeating = true;
				PlayPrevTrack.IsEnabled = false;
				PlayNextTrack.IsEnabled = false;
				PlayRepeatTrack.Background = Brushes.BlueViolet;
			}
		}

		private void LoadPlaylist_Click(object sender, RoutedEventArgs e)
		{
			OpenFolderDialog ofold = new()
			{
				DefaultDirectory = @"C:\Users\Nekitt\Documents\Учеба\ПиОГИ\Lab_No6"
			};

			bool? isSuccessful = ofold.ShowDialog();

			if (isSuccessful!.Value)
			{
				_playlistDirectory = ofold.FolderName;
				DirectoryInfo plInfo = new(_playlistDirectory);
				FileInfo[] mp3Tracks = plInfo.GetFiles("*.mp3");

				if (mp3Tracks.Length == default)
				{
					MessageBox.Show("Отсутствуют mp3-файлы в указанной папке!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Hand);
					SystemSounds.Question.Play();

					return;
				}

				if (_loadedPlaylist.Count != 0 && _initialPlaylist.Count != 0)
				{
					_loadedPlaylist.Clear();
					_initialPlaylist.Clear();
				}

				foreach (var track in mp3Tracks)
					_loadedPlaylist.Add(new MusicTrack(ofold.FolderName + '\\' + track.Name));

				CopyPlaylist(_loadedPlaylist, _initialPlaylist);

				if (TrackList.Items.Count != 0) TrackList.Items.Clear();

				ShowPlaylist();

				MessageBox.Show("Плейлист загружен!", "SOUNDRIVE", MessageBoxButton.OK, MessageBoxImage.Information);
				SystemSounds.Exclamation.Play();

				PlayStopTrack.IsEnabled = true;
				PlayPrevTrack.IsEnabled = true;
				PlayNextTrack.IsEnabled = true;
				PlayRepeatTrack.IsEnabled = true;
				PlayShuffleTracks.IsEnabled = true;
				ResetTrack.IsEnabled = true;

				TrackList.SelectedIndex = default;
			}
			else
			{
				MessageBox.Show("Не удалось открыть плейлист!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				SystemSounds.Question.Play();

				return;
			}
		}

		private void AddTrackToPlaylist_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog fileDialog = new()
			{
				DefaultExt = ".mp3",
				DefaultDirectory = @"C:\Users\Nekitt\Documents\Учеба\ПиОГИ\Lab_No6",
				RestoreDirectory = true,
				Filter = "MP3 Files (.mp3)|*.mp3"
			};

			bool? isSuccessful = fileDialog.ShowDialog();

			if (isSuccessful!.Value)
			{
				_loadedPlaylist.Add(new MusicTrack(fileDialog.FileName));
				TrackList.Items.Add(
					$"{_tracksCount++}: {_loadedPlaylist[_tracksCount].ArtistName} - {_loadedPlaylist[_tracksCount].TrackName}, " +
					$"{string.Format("{0:D2}:{1:D2}", _loadedPlaylist[_tracksCount].TrackLength.Minutes, _loadedPlaylist[_tracksCount].TrackLength.Seconds)}"
					);

				MessageBox.Show("Трек загружен в плейлист!", "SOUNDRIVE", MessageBoxButton.OK, MessageBoxImage.Information);
				SystemSounds.Exclamation.Play();

				PlayStopTrack.IsEnabled = true;
				PlayPrevTrack.IsEnabled = true;
				PlayNextTrack.IsEnabled = true;
				PlayRepeatTrack.IsEnabled = true;
				PlayShuffleTracks.IsEnabled = true;
				ResetTrack.IsEnabled = true;

				TrackList.SelectedIndex = default;
			}
			else
			{
				MessageBox.Show("Не удалось загрузить трек!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				SystemSounds.Question.Play();

				return;
			}
		}

		private void ResetTrack_Click(object sender, RoutedEventArgs e)
		{
			if (_outputDevice != null && _outputDevice.PlaybackState != PlaybackState.Stopped)
			{
				PlayStopTrack!.Content = new Image
				{
					Source = _playIconImage,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Width = 64,
					Height = 64,
					Stretch = Stretch.Fill
				};
				PlayStopTrack.Background = Brushes.DimGray;

				StopTrackPlaying();
				_isPlayPressed = false;
			}
		}

		private void TrackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (TrackList.SelectedItem != null)
				InitializeTrack(TrackList.SelectedIndex);
		}

		private void TrackChronometrage_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			CurrentTrackPosition.Content = $"{(int)TrackChronometrage.Value / 60:D2}:{(int)TrackChronometrage.Value % 60:D2}";
		}

		private void VolumeRegulator_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (_mp3Reader != null)
				_mp3Reader!.Volume = (float)VolumeRegulator.Value / 100f;
		}

		private void TrackChronometrage_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			_outputDevice?.Pause();
			long lengthInBytes = _mp3Reader!.Length;
			double position = lengthInBytes / TrackChronometrage.Maximum * TrackChronometrage.Value;
			_mp3Reader.Position = (long)position;
			_outputDevice?.Play();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			base.OnClosed(e);
			_trackTimer.Tick -= TrackTimer_Tick;
			_trackTimer.Stop();

			if (_outputDevice != null && _mp3Reader != null)
			{
				_outputDevice.Dispose();
				_outputDevice = null;

				_mp3Reader!.Dispose();
				_mp3Reader = null;
			}
		}
	}
}