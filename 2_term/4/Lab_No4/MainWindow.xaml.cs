using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Data.Sqlite;
using Microsoft.Win32;

namespace Lab_No4
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 16)]
	public struct DataRecord
	{
		public int Id { get; set; }
		public string SNP { get; set; }
		public short Math { get; set; }
		public short Physics { get; set; }
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly SqliteConnection _connection;

		private DataRecord _selectedRecord, _inputRecord;

		private string _dataBaseFilePath;

		public MainWindow()
		{
			_connection = new();
			_dataBaseFilePath = string.Empty;

			InitializeComponent();
		}

		private bool ParseInputValues()
		{
			if (!int.TryParse(IdInput.Text, out int id) || id < 0)
				return false;

			string[] words = SNPInput.Text.Split(' ');

			if (words.Length != 3)
				return false;
			else if (words[0].Any(c => !char.IsLetter(c)) || words[1].Any(c => !char.IsLetter(c)) || words[2].Any(c => !char.IsLetter(c)))
				return false;

			if (!(short.TryParse(MathInput.Text, out short math) && short.TryParse(PhysInput.Text, out short phys)))
				return false;
			else if (math is < 2 or > 5 || phys is < 2 or > 5)
				return false;

			_inputRecord = new()
			{
				Id = id,
				SNP = SNPInput.Text,
				Math = math,
				Physics = phys
			};

			return true;
		}

		private void UpdateDataGrid()
		{
			DataBaseProjection.Items.Clear();

			_connection.Open();
			string sqlFetchDataPrompt
				= "SELECT * FROM students_snp, students_grades WHERE students_snp.Id = students_grades.Id ORDER BY students_snp.SNP";
			SqliteCommand sqlFetchDataCommand = new(sqlFetchDataPrompt, _connection);
			SqliteDataReader sqlFetchReader = sqlFetchDataCommand.ExecuteReader();

			while (sqlFetchReader.Read())
			{
				DataRecord record = new()
				{
					Id = Convert.ToInt32(sqlFetchReader["Id"].ToString()!),
					SNP = sqlFetchReader["SNP"].ToString()!,
					Math = Convert.ToInt16(sqlFetchReader["MathGrade"].ToString()!),
					Physics = Convert.ToInt16(sqlFetchReader["PhysicsGrade"].ToString()!)
				};
				DataBaseProjection.Items.Add(record);
			}

			_connection.Close();

			if (DataBaseProjection.Items.Count > 0)
			{
				EditRecordButton.IsEnabled = true;
				DeleteRecordButton.IsEnabled = true;
			}
		}

		private void DBConnectButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDB = new()
			{
				FileName = "База данных",
				DefaultExt = ".db",
				DefaultDirectory = "C:\\",
				Filter = "База данных SQLite (.db)|*.db",
				RestoreDirectory = true
			};
			bool? isSuccessful = openDB.ShowDialog();

			if (isSuccessful!.Value)
			{
				_dataBaseFilePath = openDB.FileName;
				_connection.ConnectionString = $"Data Source={_dataBaseFilePath};";
				SQLitePCL.Batteries.Init();
				_connection.Open();

				if (_connection.State == ConnectionState.Open)
				{
					MessageBox.Show("Подключение к БД успешно выполнено!", "Состояние подключения", MessageBoxButton.OK, MessageBoxImage.Information);
					DBConnectButton.BorderBrush = Brushes.Green;
					DBFilePath.Content += _dataBaseFilePath;
					DBShowDataButton.IsEnabled = true;
					AddRecordButton.IsEnabled = true;
				}

				_connection.Close();
			}
			else
				MessageBox.Show("Не удалось подключиться к БД!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void DBShowDataButton_Click(object sender, RoutedEventArgs e)
			=> UpdateDataGrid();

		private void AddRecordButton_Click(object sender, RoutedEventArgs e)
		{
			if (!ParseInputValues())
			{
				MessageBox.Show("Введены некорректные значения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

				return;
			}

			_connection.Open();
			string sqlAddDataPrompt =
				$"INSERT INTO students_snp (Id, SNP) VALUES ({_inputRecord.Id}, {_inputRecord.SNP})";
			SqliteCommand sqlAddDataCommand = new(sqlAddDataPrompt, _connection);
			sqlAddDataCommand.ExecuteNonQuery();
			sqlAddDataPrompt =
				$"INSERT INTO students_grades (Id, MathGrade, PhysicsGrade) VALUES ({_inputRecord.Id}, {_inputRecord.Math}, {_inputRecord.Physics})";
			SqliteCommand sqlAddDataCommand1 = new(sqlAddDataPrompt, _connection);
			sqlAddDataCommand1.ExecuteNonQuery();
			_connection.Close();
		}

		private void EditRecordButton_Click(object sender, RoutedEventArgs e)
		{
			if (!ParseInputValues())
			{
				MessageBox.Show("Введены некорректные значения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

				return;
			}

			_connection.Open();
			string sqlEditDataPrompt =
				$"UPDATE students_snp SET SNP={_inputRecord.SNP} WHERE Id={_inputRecord.Id}";
			SqliteCommand sqlEditDataCommand = new(sqlEditDataPrompt, _connection);
			sqlEditDataCommand.ExecuteNonQuery();
			sqlEditDataPrompt =
				$"UPDATE students_grades SET MathGrade={_inputRecord.Math}, PhysicsGrade={_inputRecord.Physics} WHERE Id={_inputRecord.Id}";
			SqliteCommand sqlEditDataCommand1 = new(sqlEditDataPrompt, _connection);
			sqlEditDataCommand1.ExecuteNonQuery();
			_connection.Close();

			UpdateDataGrid();
		}

		private void DeleteRecordButton_Click(object sender, RoutedEventArgs e)
		{
			if (!int.TryParse(IdInput.Text, out int id) || id < 0)
			{
				MessageBox.Show("Некорректное или отрицательное значение идентификатора!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

				return;
			}

			_connection.Open();
			string sqlDeleteDataPrompt =
				$"DELETE FROM students_snp WHERE Id={id}";
			SqliteCommand sqlDeleteDataCommand = new(sqlDeleteDataPrompt, _connection);
			sqlDeleteDataCommand.ExecuteNonQuery();
			sqlDeleteDataPrompt =
				$"DELETE FROM students_grades WHERE Id={id}";
			SqliteCommand sqlDeleteDataCommand1 = new(sqlDeleteDataPrompt, _connection);
			sqlDeleteDataCommand1.ExecuteNonQuery();
			_connection.Close();

			UpdateDataGrid();
		}

		private void DataBaseProjection_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (DataBaseProjection.SelectedItem != null)
			{
				_selectedRecord = (DataRecord)DataBaseProjection.SelectedItem;
				IdInput.Text = _selectedRecord.Id.ToString();
				SNPInput.Text = _selectedRecord.SNP;
				MathInput.Text = _selectedRecord.Math.ToString();
				PhysInput.Text = _selectedRecord.Physics.ToString();
			}
		}
	}
}