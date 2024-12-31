using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Win32;

namespace Lab_No3
{
	internal enum SelectedTimeFormat
	{
		DaysHoursMinutesSeconds,
		HoursMinutesSeconds,
		MinutesSeconds,
		OnlySeconds
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		internal static Dictionary<string, DateTime>? Timers { get; private set; }
		internal static MainWindow? Instance { get; private set; }

		internal static string? EditTimerKey { get; private set; }
		internal static bool IsEditing { get; private set; }

		private Window1? _otherInstance;
		private DateTime _currentTimer;
		private SelectedTimeFormat _timeFormat;

		public MainWindow()
		{
			InitializeComponent();

			Timers = [];
			_currentTimer = default;
			Instance = this;
		}

		private void RefreshTimersList()
		{
			DateTime[] dictValues = [.. Timers!.Values];
			List<string> datesInString = [];

			foreach (var date in dictValues)
				datesInString.Add(date.ToString("G"));

			TimersList.ItemsSource = datesInString;
		}

		#region FileSection

		private void LoadTimersList_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFile = new()
			{
				FileName = "Таймеры",
				DefaultExt = ".txt",
				Filter = "Текстовые документы (.txt)|*.txt",
				InitialDirectory = "C:\\",
				RestoreDirectory = true
			};
			bool? isSuccessful = openFile.ShowDialog();

			if (isSuccessful!.Value)
			{
				Stream fs = openFile.OpenFile();

				using StreamReader reader = new(fs);
				string parsedData = reader.ReadToEnd();

				if (!(parsedData.Contains('\n') && parsedData.Contains('#')))
				{
					MessageBox.Show("Данные файла некорректны! Каждый таймер должен заканчиваться переносом строки, а также имя и значение таймера должны разделяться знаком \'#\'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					reader.Close();

					return;
				}

				reader.Close();

				int countNewLines = parsedData.Count(c => c == '\n');
				
				for (int i = default; i < countNewLines; ++i)
				{
					int startParsingIndex = default;
					int endParsingIndex = parsedData.IndexOf('\n');
					string parsingPortion = parsedData.Substring(startParsingIndex, endParsingIndex);

					if (!parsingPortion.Contains('#'))
						continue;

					int delimeterIndex = parsingPortion.IndexOf('#');
					string timerName = parsingPortion[..delimeterIndex];

					if (!DateTime.TryParse(parsingPortion.AsSpan(delimeterIndex + 1, parsingPortion.Length - delimeterIndex - 1), out DateTime timerValue))
						continue;

					Timers!.Add(timerName, timerValue);

					parsedData = parsedData.Remove(startParsingIndex, endParsingIndex + 1);
				}
			}
			else
				MessageBox.Show("Не удалось открыть файл!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

			RefreshTimersList();
		}

		private void SaveTimersList_Click(object sender, RoutedEventArgs e)
		{
			if (Timers!.Count == 0)
			{
				MessageBox.Show("Отсутсвуют таймеры для сохранения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

				return;
			}

			SaveFileDialog saveFile = new()
			{
				FileName = "Таймеры",
				DefaultExt = ".txt",
				Filter = "Текстовые документы (.txt)|*.txt",
				InitialDirectory = "C:\\",
				RestoreDirectory = true
			};
			bool? isSuccessful = saveFile.ShowDialog();

			if (isSuccessful!.Value)
			{
				string[] timerKeys = [.. Timers!.Keys];
				string[] timerValues = new string[timerKeys.Length];

				for (int i = default; i < timerValues.Length; ++i)
					timerValues[i] = Timers[timerKeys[i]]!.ToString("G");

				Stream fs = saveFile.OpenFile();

				using StreamWriter writer = new(fs);

				for (int i = default; i < timerKeys.Length; ++i)
					writer.Write(timerKeys[i] + '#' + timerValues[i] + '\n');

				writer.Close();
			}
			else
				MessageBox.Show("Не удалось сохранить файл!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		#endregion

		#region TimersSection

		private void AddTimerButton_Click(object sender, RoutedEventArgs e)
		{
			IsEditing = false;
			_otherInstance = new();
			_otherInstance.TimerName.IsEnabled = true;
			_otherInstance.Title = "Добавление таймера";
			_otherInstance.Show();

			IsEnabled = false;
		}

		private void EditTimerButton_Click(object sender, RoutedEventArgs e)
		{
			IsEditing = true;

			if (Timers!.Values.Count == 0)
			{
				MessageBox.Show("Отсутствуют таймеры для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

				return;
			}

			if (TimersList.SelectedItem == null)
			{
				MessageBox.Show("Выберите таймер для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

				return;
			}

			_currentTimer = DateTime.Parse(TimersList.SelectedItem.ToString()!);

			foreach (var k in Timers.Keys)
				if (Timers[k] == _currentTimer)
				{
					EditTimerKey = k;
					Timers!.Remove(k);
				}

			_otherInstance = new();
			_otherInstance.TimerName.IsEnabled = false;
			_otherInstance.Title = "Изменение таймера";
			_otherInstance.Show();

			IsEnabled = false;
		}

		private void DeleteTimerButton_Click(object sender, RoutedEventArgs e)
		{
			if (Timers!.Values.Count == 0)
			{
				MessageBox.Show("Отсутствуют таймеры для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

				return;
			}

			if (TimersList.SelectedItem == null)
			{
				MessageBox.Show("Выберите таймер для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

				return;
			}

			_currentTimer = DateTime.Parse(TimersList.SelectedItem.ToString()!);

			foreach (var k in Timers.Keys)
				if (Timers[k] == _currentTimer)
					Timers.Remove(k);

			RefreshTimersList();
		}

		#endregion

		private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
			=> RefreshTimersList();

		private void TimersList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && TimersList.ItemsSource != null)
			{
				TimeSpan timeDiff = DateTime.Parse((sender as ListBox)!.SelectedItem.ToString()!) - DateTime.Now;

				string? timeLeft = _timeFormat switch
				{
					SelectedTimeFormat.DaysHoursMinutesSeconds
					=> $"До наступления указанной даты осталось {timeDiff.Days} дней, "
					+ $"{timeDiff.Hours} часов {timeDiff.Minutes} минут и {timeDiff.Seconds} секунд",
					SelectedTimeFormat.HoursMinutesSeconds
					=> $"До наступления указанной даты осталось {timeDiff.Days * timeDiff.Hours} часов, "
					+ $"{timeDiff.Minutes} минут и {timeDiff.Seconds} секунд",
					SelectedTimeFormat.MinutesSeconds
					=> $"До наступления указанной даты осталось {timeDiff.Days * timeDiff.Hours * timeDiff.Minutes} минут "
					+ $"и {timeDiff.Seconds} секунд",
					SelectedTimeFormat.OnlySeconds
					=> $"До наступления указанной даты осталось {timeDiff.Days * timeDiff.Hours * timeDiff.Minutes * timeDiff.Seconds} секунд",
					_ => "Некорректный формат отображения времени!"
				};

				MessageBox.Show(timeLeft, "Отсчет времени", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void FormatType_Selected(object sender, RoutedEventArgs e)
		{
			_timeFormat = (sender as ListBoxItem)!.Name switch
			{
				"DHMS" or "DHMS_1" => SelectedTimeFormat.DaysHoursMinutesSeconds,
				"HMS" or "HMS_1" => SelectedTimeFormat.HoursMinutesSeconds,
				"MS" or "MS_1" => SelectedTimeFormat.MinutesSeconds,
				"S" or "S_1" => SelectedTimeFormat.OnlySeconds,
				_ => default
			};
		}
	}
}