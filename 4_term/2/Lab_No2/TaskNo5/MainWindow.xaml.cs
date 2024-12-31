using System.Windows;
using System.Windows.Threading;

namespace TaskNo5
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly System.Timers.Timer _timer;
		private TimeOnly _tickingTime;
		private int _savesCount;
		private bool _isTimerRunning;

		public MainWindow()
		{
			_timer = new System.Timers.Timer(1_000);
			_tickingTime = new TimeOnly(0, 0, 0);
			_timer.Elapsed += OnElapsed;

			InitializeComponent();
		}

		private void OnElapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			_tickingTime = _tickingTime.Add(new TimeSpan(0, 0, 1));
			Dispatcher.Invoke(() => CurrentTimer.Content = $"{_tickingTime:T}");
        }

        private void TimerStart_Click(object sender, RoutedEventArgs e)
        {
			_timer.Start();
			_isTimerRunning = true;
        }

        private void TimerReset_Click(object sender, RoutedEventArgs e)
        {
			_timer.Stop();
			_isTimerRunning = false;
			_tickingTime = new TimeOnly(0, 0, 0);
			CurrentTimer.Content = "0:00:00";
        }

        private void TimerRemember_Click(object sender, RoutedEventArgs e)
        {
			if (!_isTimerRunning)
			{
				MessageBox.Show("Для сохранения значения запустите таймер!", "Задание №5", MessageBoxButton.OK, MessageBoxImage.Error);

				return;
			}

			++_savesCount;

			if (TimeFormatting.IsChecked!.Value == true)
				TimerMemory.Text += $"Время {_savesCount}: {_tickingTime:T}\n";
			else
				TimerMemory.Text += $"Время {_savesCount}: {_tickingTime.Second} сек.\n";
        }
    }
}