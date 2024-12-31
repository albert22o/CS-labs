using System.Windows;

using Microsoft.VisualBasic;

namespace Minesweeper
{
	internal enum GameDifficulty
	{
		Easy,
		Medium,
		Hard
	}

	/// <summary>
	/// Interaction logic for DifficultyChoise.xaml
	/// </summary>
	public partial class DifficultyChoise : Window
	{
		private GameDifficulty _chosenDifficulty;

		public DifficultyChoise()
		{
			InitializeComponent();
		}

		private void EasyDifficulty_Click(object sender, RoutedEventArgs e)
		{
			_chosenDifficulty = GameDifficulty.Easy;
			Play.IsEnabled = true;
		}

		private void MediumDifficulty_Click(object sender, RoutedEventArgs e)
		{
			_chosenDifficulty = GameDifficulty.Medium;
			Play.IsEnabled = true;
		}

		private void HardDifficulty_Click(object sender, RoutedEventArgs e)
		{
			_chosenDifficulty = GameDifficulty.Hard;
			Play.IsEnabled = true;
		}

		private void Play_Click(object sender, RoutedEventArgs e)
		{
			string playerName = Interaction.InputBox("Введите ваш никнейм: ", "САПЕР (Лабораторная №8)");

			MainWindow.PlayerName = playerName;

			(MainWindow.FieldSize, MainWindow.MinesAmount, MainWindow.Attempts) = _chosenDifficulty switch
			{
				GameDifficulty.Easy => (new ValueTuple<int, int>(9, 9), 15, 6),
				GameDifficulty.Medium => (new ValueTuple<int, int>(16, 16), 45, 4),
				GameDifficulty.Hard => (new ValueTuple<int, int>(24, 24), 100, 2),
				_ => default
			};

			MainWindow window = new();
			window.Title += _chosenDifficulty switch
			{
				GameDifficulty.Easy => ": НИЗКАЯ СЛОЖНОСТЬ",
				GameDifficulty.Medium => ": СРЕДНЯЯ СЛОЖНОСТЬ",
				GameDifficulty.Hard => ": ВЫСОКАЯ СЛОЖНОСТЬ",
				_ => string.Empty
			};
			Close();
			window.Show();
		}

		private void Records_Click(object sender, RoutedEventArgs e)
		{
			Records recordsWindow = new();
			recordsWindow.Show();
		}
	}
}