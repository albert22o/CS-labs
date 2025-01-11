using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Data.Sqlite;

namespace Minesweeper
{
	internal enum MineSweeperValues
	{
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4,
		Five = 5,
		Six = 6,
		Seven = 7,
		Eight = 8,
		Mine = 9,
		Empty = 10
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		internal static readonly string DB_CONNECTION_STRING =
			@"Data Source=C:\Users\Nekitt\Documents\УЧЕБА\ПиОГИ\Minesweeper\records_table.db;Mode=ReadWrite;";

		private const int GAME_BUTTON_SIZE = 25;

		private readonly BitmapImage _flagIcon = new(new Uri("/icons/flag.png", UriKind.Relative));
		private readonly BitmapImage _mineIcon = new(new Uri("/icons/mine.png", UriKind.Relative));
		private readonly BitmapImage _victorySmile = new(new Uri("/icons/victory-smile.png", UriKind.Relative));
		private readonly BitmapImage _defaultSmile = new(new Uri("/icons/default-smile.png", UriKind.Relative));
		private readonly BitmapImage _clickingSmile = new(new Uri("/icons/clicking-smile.png", UriKind.Relative));
		private readonly BitmapImage _losingSmile = new(new Uri("/icons/losing-smile.png", UriKind.Relative));

		private readonly SqliteConnection _connection;
		private readonly DispatcherTimer _timer;
		private readonly Random _random;
		private TimeSpan _currentTime;

		private int[,] _gameField;

		private bool _isGameEnded;
		private bool _isGameOver;
		private int _openedSlots;
		private int _steps;

		public MainWindow()
		{
			_connection = new() { ConnectionString = DB_CONNECTION_STRING };
			SQLitePCL.Batteries.Init();
			_random = new Random();
			_timer = new() { Interval = new TimeSpan(0, 0, 1) };
			_timer.Tick += Timer_Tick;
			_currentTime = default;
			_gameField = new int[FieldSize.Item1, FieldSize.Item2];

			InitializeComponent();

			GameField.Rows = FieldSize.Item2;
			GameField.Columns = FieldSize.Item1;
			GameField.Width = FieldSize.Item1 * GAME_BUTTON_SIZE;
			GameField.Height = FieldSize.Item2 * GAME_BUTTON_SIZE;
			GameField.Margin = new Thickness(default);
			Header.Width = GameField.Width;
			UniformGridWrapper.Width = GameField.Width;
			UniformGridWrapper.Height = GameField.Height;
			UniformGridWrapper.Margin = new Thickness(default);

			(ScoresLabel.Margin, Smiley.Margin, TimerLabel.Margin) = Header.Width switch
			{
				225 => (new Thickness(2, 2, 2, 2), new Thickness(34, 2, 2, 2), new Thickness(36, 2, 2, 2)),
				400 => (new Thickness(2, 2, 2, 2), new Thickness(126, 2, 2, 2), new Thickness(121, 2, 2, 2)),
				600 => (new Thickness(2, 2, 2, 2), new Thickness(225, 2, 2, 2), new Thickness(220, 2, 2, 2)),
				_ => default
			};
			Smiley.Content = new Image { Source = _defaultSmile };

			for (int i = default; i < FieldSize.Item2; ++i)
				for (int j = default; j < FieldSize.Item1; ++j)
				{
					Button gameButton = new() { Tag = i * FieldSize.Item1 + j };
					(gameButton.Width, gameButton.Height) = (GAME_BUTTON_SIZE, GAME_BUTTON_SIZE);
					gameButton.Content = string.Empty;
					gameButton.Margin = new Thickness(default);
					gameButton.PreviewMouseDown += GameButton_MouseDown;
					gameButton.PreviewMouseUp += GameButton_MouseUp;
					GameField.Children.Add(gameButton);
				}

			_timer.Start();
		}

		internal static ValueTuple<int, int> FieldSize { get; set; }

		internal static int MinesAmount { get; set; }
		internal static int Attempts { get; set; }
		internal static string PlayerName { get; set; } = "NO-NAME";

		private static void LerpOffsetValues(ref int left, ref int right, ref int up, ref int down)
		{
			if (left < 0) left = default;
			else if (right > FieldSize.Item1 - 1) right = FieldSize.Item1 - 1;

			if (up < 0) up = default;
			else if (down > FieldSize.Item2 - 1) down = FieldSize.Item2 - 1;
		}

		private bool CheckBounds(int x, int y)
		{
			int x_leftOffset = x - 1;
			int x_rightOffset = x + 1;
			int y_upOffset = y - 1;
			int y_downOffset = y + 1;

			LerpOffsetValues(ref x_leftOffset, ref x_rightOffset, ref y_upOffset, ref y_downOffset);

			for (int i = x_leftOffset; i <= x_rightOffset; ++i)
				for (int j = y_upOffset; j <= y_downOffset; ++j)
					if (i != x && j != y && _gameField[i, j] == 0) return true;

			return false;
		}

		private static ValueTuple<int, int> GetButtonCoordsByAxis(int tag)
			=> (tag / FieldSize.Item1, tag % FieldSize.Item1);

		private void GenerateGameFieldMines(int tag)
		{
			int x_buttonCoord = GetButtonCoordsByAxis(tag).Item1;
			int y_buttonCoord = GetButtonCoordsByAxis(tag).Item2;

			for (int i = default; i < MinesAmount; ++i)
			{
				int x_mineSpawnCoord = _random.Next(0, FieldSize.Item1);
				int y_mineSpawnCoord = _random.Next(0, FieldSize.Item2);

				if (_gameField[x_mineSpawnCoord, y_mineSpawnCoord] == (int)MineSweeperValues.Mine ||
					!CheckBounds(x_mineSpawnCoord, y_mineSpawnCoord) ||
					(x_buttonCoord == x_mineSpawnCoord && y_buttonCoord == y_mineSpawnCoord))
				{
					--i;

					continue;
				}

				_gameField[x_mineSpawnCoord, y_mineSpawnCoord] = (int)MineSweeperValues.Mine;
			}
		}

		private int CountScoresOfMinesNearby(int x, int y)
		{
			int result = default;

			int x_leftOffset = x - 1;
			int x_rightOffset = x + 1;
			int y_upOffset = y - 1;
			int y_downOffset = y + 1;

			LerpOffsetValues(ref x_leftOffset, ref x_rightOffset, ref y_upOffset, ref y_downOffset);

			for (int i = x_leftOffset; i <= x_rightOffset; ++i)
				for (int j = y_upOffset; j <= y_downOffset; ++j)
					if (_gameField[i, j] == (int)MineSweeperValues.Mine) ++result;

			return result;
		}

		private void EvaluateGameNearArea()
		{
			for (int i = default; i < FieldSize.Item1; ++i)
				for (int j = default; j < FieldSize.Item2; ++j)
					if (_gameField[i, j] != (int)MineSweeperValues.Mine) _gameField[i, j] = CountScoresOfMinesNearby(i, j);
		}

		private void SetClickedGameButton(int x, int y)
		{
			int x_leftOffset = x - 1;
			int x_rightOffset = x + 1;
			int y_upOffset = y - 1;
			int y_downOffset = y + 1;

			LerpOffsetValues(ref x_leftOffset, ref x_rightOffset, ref y_upOffset, ref y_downOffset);

			if (_gameField[x, y] == 0)
			{
				_gameField[x, y] = (int)MineSweeperValues.Empty;

				SetClickedGameButton(x_leftOffset, y);
				SetClickedGameButton(x, y_downOffset);
				SetClickedGameButton(x_rightOffset, y);
				SetClickedGameButton(x, y_upOffset);

				SetClickedGameButton(x_leftOffset, y_upOffset);
				SetClickedGameButton(x_leftOffset, y_downOffset);
				SetClickedGameButton(x_rightOffset, y_upOffset);
				SetClickedGameButton(x_rightOffset, y_downOffset);
			}
			else if (_gameField[x, y] >= 0 && _gameField[x, y] < (int)MineSweeperValues.Empty && _gameField[x, y] != (int)MineSweeperValues.Mine)
				_gameField[x, y] += (int)MineSweeperValues.Empty;
		}

		private void SaveRecordToDatabase(int record)
		{
			string username = PlayerName;
			int scores = record;

			try
			{
				_connection.Open();
				string sqlQuery = "INSERT INTO Records (Username, Scores) VALUES (@Username, @Scores)";
				SqliteCommand sqlCommand = new(sqlQuery, _connection);
				sqlCommand.Parameters.AddWithValue("@Username", username);
				sqlCommand.Parameters.AddWithValue("@Scores", scores);
				sqlCommand.ExecuteNonQuery();
				_connection.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при записи рекорда: {ex.Message}", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			_currentTime = _currentTime.Add(new TimeSpan(0, 0, 1));
			Dispatcher.Invoke(() =>
			{
				TimerLabel.Content = $"{_currentTime.Minutes:D2}:{_currentTime.Seconds:D2}";
				ScoresLabel.Content = $"{_steps.ToString().PadLeft(4, '0')}";
			});
		}

		private void GameButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
				Smiley.Content = new Image { Source = _clickingSmile };

			if (_steps == 0) _steps = 1;

			if (e.RightButton == MouseButtonState.Pressed && _steps == 1) --_steps;

			int tag = (int)((Button)sender).Tag;

			if (_isGameEnded)
			{
				if (_isGameOver)
				{
					_isGameOver = false;

					TimerLabel.Content = "00:00";
					ScoresLabel.Content = "0000";

					int counter = -1;

					for (int i = default; i < FieldSize.Item2; ++i)
						for (int j = default; j < FieldSize.Item1; ++j)
						{
							++counter;
							((Button)GameField.Children[counter]).Background = new SolidColorBrush(Color.FromArgb(255, 0xDD, 0xDD, 0xDD));
							((Button)GameField.Children[counter]).Content = string.Empty;
						}
				}

				Smiley.Content = new Image { Source = _losingSmile };
				_gameField = new int[FieldSize.Item1, FieldSize.Item2];
				_openedSlots = default;
				_isGameEnded = false;

				if (!_isGameEnded)
				{
					TimerLabel.Content = "00:00";
					ScoresLabel.Content = "0000";
					_timer.Start();
				}
			}

			if (e.LeftButton == MouseButtonState.Pressed && _steps == 1)
			{
				GenerateGameFieldMines(tag);
				EvaluateGameNearArea();
			}

			int x_buttonCoord = GetButtonCoordsByAxis(tag).Item1;
			int y_buttonCoord = GetButtonCoordsByAxis(tag).Item2;

			if (e.LeftButton == MouseButtonState.Pressed && _steps >= 1)
			{
				SetClickedGameButton(x_buttonCoord, y_buttonCoord);

				if (_gameField[x_buttonCoord, y_buttonCoord] != (int)MineSweeperValues.Mine && _gameField[x_buttonCoord, y_buttonCoord] < 20)
				{
					int counter = -1;

					for (int i = default; i < FieldSize.Item2; ++i)
						for (int j = default; j < FieldSize.Item1; ++j)
						{
							++counter;

							if (_gameField[i, j] <= 19 && _gameField[i, j] >= 10)
							{
								_gameField[i, j] += 100;
								((Button)GameField.Children[counter]).Background = Brushes.White;
								((Button)GameField.Children[counter]).FontSize = 16;
								((Button)GameField.Children[counter]).FontWeight = FontWeights.Bold;
							}

							const int emptySlotValue = 100 + (int)MineSweeperValues.Empty;

							switch (_gameField[i, j])
							{
								case emptySlotValue:
									((Button)GameField.Children[counter]).Content = string.Empty;
									break;
								case emptySlotValue + (int)MineSweeperValues.One:
									((Button)GameField.Children[counter]).Content = "1";
									((Button)GameField.Children[counter]).Background = Brushes.Blue;
									break;
								case emptySlotValue + (int)MineSweeperValues.Two:
									((Button)GameField.Children[counter]).Content = "2";
									((Button)GameField.Children[counter]).Background = Brushes.Green;
									break;
								case emptySlotValue + (int)MineSweeperValues.Three:
									((Button)GameField.Children[counter]).Content = "3";
									((Button)GameField.Children[counter]).Background = Brushes.Red;
									break;
								case emptySlotValue + (int)MineSweeperValues.Four:
									((Button)GameField.Children[counter]).Content = "4";
									((Button)GameField.Children[counter]).Background = Brushes.Purple;
									break;
								case emptySlotValue + (int)MineSweeperValues.Five:
									((Button)GameField.Children[counter]).Content = "5";
									((Button)GameField.Children[counter]).Background = Brushes.Cyan;
									break;
								case emptySlotValue + (int)MineSweeperValues.Six:
									((Button)GameField.Children[counter]).Content = "6";
									((Button)GameField.Children[counter]).Background = Brushes.Magenta;
									break;
								case emptySlotValue + (int)MineSweeperValues.Seven:
									((Button)GameField.Children[counter]).Content = "7";
									((Button)GameField.Children[counter]).Background = Brushes.Orange;
									break;
								case emptySlotValue + (int)MineSweeperValues.Eight:
									((Button)GameField.Children[counter]).Content = "8";
									((Button)GameField.Children[counter]).Background = Brushes.Black;
									break;
							}

							if (_gameField[i, j] <= emptySlotValue + (int)MineSweeperValues.Mine && _gameField[i, j] >= emptySlotValue)
							{
								++_openedSlots;
								_gameField[i, j] -= (int)MineSweeperValues.Empty;
								++_steps;
							}
						}
				}
				else if (_gameField[x_buttonCoord, y_buttonCoord] == (int)MineSweeperValues.Mine)
				{
					_isGameEnded = true;
					_isGameOver = true;

					_timer.Stop();
					_currentTime = default;
					Smiley.Content = new Image { Source = _losingSmile };

					((Button)sender).Background = Brushes.Red;
					((Button)sender).Content = new Image { Source = _mineIcon };

					int counter = -1;

					for (int i = default; i < FieldSize.Item2; ++i)
						for (int j = default; j < FieldSize.Item1; ++j)
						{
							++counter;

							if ((_gameField[i, j] == (int)MineSweeperValues.Mine ||
								_gameField[i, j] == (int)MineSweeperValues.Mine + 20 ||
								_gameField[i, j] == (int)MineSweeperValues.Mine + 30) &&
								(Button)sender != (Button)GameField.Children[counter])
							{
								((Button)GameField.Children[counter]).Content = new Image { Source = _mineIcon };

								if (_gameField[i, j] == (int)MineSweeperValues.Mine + 20)
									((Button)GameField.Children[counter]).Background = new SolidColorBrush(Color.FromArgb(255, 0xFF, 0x3D, 0x00));
							}
						}

					if (--Attempts == 0)
					{
						int recordSteps = _steps - 1;
						SaveRecordToDatabase(recordSteps);

						if (MessageBox.Show($"ВЫ ПРОИГРАЛИ! ВАШ СЧЁТ: {recordSteps}", "GOOD LUCK NEXT TIME!", MessageBoxButton.OK, MessageBoxImage.Hand) ==
							MessageBoxResult.OK)
							Close();
					}

					_steps = default;
				}
			}

			if (e.RightButton == MouseButtonState.Pressed && _steps != 0)
			{
				int currentButtonOnField = _gameField[x_buttonCoord, y_buttonCoord];

				if (currentButtonOnField < 20)
				{
					_gameField[x_buttonCoord, y_buttonCoord] += 20;
					((Button)sender).Content = new Image { Source = _flagIcon };
				}
				else if (currentButtonOnField >= 20 && currentButtonOnField < 30)
				{
					_gameField[x_buttonCoord, y_buttonCoord] += 10;
					((Button)sender).Foreground = Brushes.Black;
					((Button)sender).FontSize = 16;
					((Button)sender).FontWeight = FontWeights.Bold;
					((Button)sender).Content = "?";
				}
				else if (currentButtonOnField >= 30 && currentButtonOnField < 40)
				{
					_gameField[x_buttonCoord, y_buttonCoord] -= 30;
					((Button)sender).Content = string.Empty;
				}
			}

			if (_openedSlots == FieldSize.Item1 * FieldSize.Item2 - MinesAmount)
			{
				Smiley.Content = new Image { Source = _victorySmile };
				_isGameEnded = true;
				int recordScores = _steps - 1;
				SaveRecordToDatabase(recordScores);
				_timer.Stop();

				_steps = default;

				if (MessageBox.Show($"МОИ ПОЗДРАВЛЕНИЯ! ВАШ СЧЕТ: {recordScores}", "ЭТО ПОБЕДА, ЭТО УСПЕХ", MessageBoxButton.OK, MessageBoxImage.Exclamation) ==
					MessageBoxResult.OK)
				{
					int counter = -1;

					for (int i = default; i < FieldSize.Item2; ++i)
						for (int j = default; j < FieldSize.Item1; ++j)
						{
							++counter;
							((Button)GameField.Children[counter]).Background = new SolidColorBrush(Color.FromArgb(255, 0xdd, 0xdd, 0xdd));
							((Button)GameField.Children[counter]).Content = string.Empty;
						}

					Close();
				}
			}
		}

		private void GameButton_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released && _isGameEnded)
				Smiley.Content = new Image { Source = _losingSmile };
			else if (e.LeftButton == MouseButtonState.Released || e.RightButton == MouseButtonState.Released)
				Smiley.Content = new Image { Source = _defaultSmile };
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			DifficultyChoise startScreen = new();
			startScreen.Show();
		}

		private void Smiley_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!_isGameEnded) Smiley.Content = new Image { Source = _clickingSmile };
		}

		private void Smiley_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!_isGameEnded) Smiley.Content = new Image { Source = _defaultSmile };
		}
	}
}