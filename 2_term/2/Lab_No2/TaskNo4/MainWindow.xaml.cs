using System.Windows;
using System.Windows.Controls;

namespace TaskNo4
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly int[] _years;
		private readonly string[] _months;
		private readonly int[] _days;

		private ValueTuple<int, int, int> _chosenData;

		private bool _isYearChosen;

		public MainWindow()
		{
			_years = new int[124];
			_months = new string[12];
			_days = new int[31];

			int currentYear = 2024;

			for (int i = 0; i < _years.Length; ++i)
				_years[i] = currentYear--; // Заполняем все года в обратном порядке
			// Заполняем месяца
			_months[0] = "Январь";
			_months[1] = "Февраль";
			_months[2] = "Март";
			_months[3] = "Апрель";
			_months[4] = "Май";
			_months[5] = "Июнь";
			_months[6] = "Июль";
			_months[7] = "Август";
			_months[8] = "Сентябрь";
			_months[9] = "Октябрь";
			_months[10] = "Ноябрь";
			_months[11] = "Декабрь";
			// Заполняем все возможные дни в месяце
			for (int i = 0; i < _days.Length; ++i)
				_days[i] = i + 1;

			InitializeComponent();
			// Присваеваем Combobox-ам определенные массивы
			YearChoise.ItemsSource = _years;
			MonthChoise.ItemsSource = _months;
			DayChoise.ItemsSource = _days;
		}
		// Функция для вычисления високосного года
		private static bool IsLeapYear(int year)
			=> (year % 400 == 0) || ((year % 4 == 0) && (year % 100 != 0));
		// Вычисление дней для фервраля
		private void EvaluateFebruaryDays()
		{
			if (_chosenData.Item2 == 2 && IsLeapYear(_chosenData.Item1))
				DayChoise.ItemsSource = _days.Where(d => d <= 29);
			else if (_chosenData.Item2 == 2)
				DayChoise.ItemsSource = _days.Where(d => d <= 28);
		}
		// Поведение при выборе года
		private void YearChoise_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DayChoise.ItemsSource = _days;
			_chosenData.Item1 = (int)(sender as ComboBox)!.SelectedItem;
			_isYearChosen = true;
			EvaluateFebruaryDays();
		}
        // Поведение при выборе месяца
        private void MonthChoise_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DayChoise.ItemsSource = _days;
			string month = (string)(sender as ComboBox)!.SelectedItem;

			_chosenData.Item2 = month switch
			{
				"Январь" => 1,
				"Февраль" => 2,
				"Март" => 3,
				"Апрель" => 4,
				"Май" => 5,
				"Июнь" => 6,
				"Июль" => 7,
				"Август" => 8,
				"Сентябрь" => 9,
				"Октябрь" => 10,
				"Ноябрь" => 11,
				"Декабрь" => 12,
				_ => default
			};

			EvaluateFebruaryDays();
			// Для определенных месяцев ставим 30 дней
			if (_chosenData.Item2 == 4 ||
				_chosenData.Item2 == 6 ||
				_chosenData.Item2 == 9 ||
				_chosenData.Item2 == 11)
				DayChoise.ItemsSource = _days.Where(d => d <= 30);

			if (_isYearChosen) DayChoise.IsEnabled = true;// Ставим возможность выбрать день
		}
		// Обработка поведения выбора дня
		private void DayChoise_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_chosenData.Item3 = (int)(sender as ComboBox)!.SelectedItem;

			DateTime currentDate = DateTime.Today;
			DateTime chosenDate = new(_chosenData.Item1, _chosenData.Item2, _chosenData.Item3);
			TimeSpan diff = currentDate.Subtract(chosenDate);
			int years = (int)(diff.TotalDays / 365.25);
			diff -= TimeSpan.FromDays(years * 365);
			int months = (int)(diff.TotalDays / 30.44);
			diff -= TimeSpan.FromDays(months * 30);
			int days = (int)diff.TotalDays;
			string message = $"С момента выбранной даты прошло {years} лет, {months} месяцев и {days} дней.";
			MessageBox.Show(message, "Задание №4", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}