using System.Windows;

namespace Lab_No3
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private (int hour, int minute, int second) _inputData;

		public Window1()
		{
			_inputData = default;

			InitializeComponent();
		}

		private bool ValidateInput()
		{
			if (!MainWindow.IsEditing && TimerName.Text == string.Empty)
			{
				SendErrorMessage("Введите имя для таймера!");

				return false;
			}

			if (!(int.TryParse(InputHours.Text, out _inputData.hour)
				&& int.TryParse(InputMinutes.Text, out _inputData.minute)
				&& int.TryParse(InputSeconds.Text, out _inputData.second)))
			{
				SendErrorMessage("Некорректное значение для введенного времени!");

				return false;
			}

			if ((_inputData.hour is < 0 or > 23)
				|| (_inputData.minute is < 0 or > 59)
				|| (_inputData.second is < 0 or > 59))
			{
				SendErrorMessage("Неверно введенное значение для часов (0-23) и минут (0-59)");

				return false;
			}

			if (InputDate.SelectedDate!.Value < DateTime.Today)
			{
				SendErrorMessage("Указанная дата не должна быть установлена раньше настоящего дня!");

				return false;
			}

			return true;
		}

		private void SendErrorMessage(string description)
		{
			TimerName.Text = string.Empty;
			InputHours.Text = string.Empty;
			InputMinutes.Text = string.Empty;
			InputSeconds.Text = string.Empty;
			InputDate.SelectedDate = DateTime.Today;

			MessageBox.Show(description, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void InputHours_GotFocus(object sender, RoutedEventArgs e)
			=> InputHours.Text = string.Empty;

		private void InputMinutes_GotFocus(object sender, RoutedEventArgs e)
			=> InputMinutes.Text = string.Empty;

		private void InputSeconds_GotFocus(object sender, RoutedEventArgs e)
			=> InputSeconds.Text = string.Empty;

		private void AcceptInput_Click(object sender, RoutedEventArgs e)
		{
			if (!ValidateInput())
				return;

			DateTime inputDate = new(
				InputDate.SelectedDate!.Value.Year,
				InputDate.SelectedDate!.Value.Month,
				InputDate.SelectedDate!.Value.Day,
				_inputData.hour,
				_inputData.minute,
				_inputData.second
				);

			if (!MainWindow.IsEditing)
				MainWindow.Timers!.Add(TimerName.Text, inputDate);
			else
				MainWindow.Timers![MainWindow.EditTimerKey!] = inputDate; 

			Close();
			MainWindow.Instance!.IsEnabled = true;
		}

		private void CancelInput_Click(object sender, RoutedEventArgs e)
		{
			Close();
			MainWindow.Instance!.IsEnabled = true;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			MainWindow.Instance!.IsEnabled = true;
		}
	}
}