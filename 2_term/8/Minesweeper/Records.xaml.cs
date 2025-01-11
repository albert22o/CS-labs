using System.Runtime.InteropServices;
using System.Windows;

using Microsoft.Data.Sqlite;

namespace Minesweeper
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 16)]
	internal struct RecordData
	{
		// -- 16 bytes alignment (x64) --
		public int Counter { get; set; }
		public string PlayerName { get; set; }
		public int Scores { get; set; }
	}

	/// <summary>
	/// Interaction logic for Records.xaml
	/// </summary>
	public partial class Records : Window
	{
		private readonly SqliteConnection _connection;

		public Records()
		{
			_connection = new() { ConnectionString = MainWindow.DB_CONNECTION_STRING };
			SQLitePCL.Batteries.Init();

			InitializeComponent();
			LoadRecords();
		}

		private void LoadRecords()
		{
			try
			{
				List<RecordData> records = [];

				_connection.Open();
				string sqlQuery = "SELECT * FROM Records ORDER BY Scores DESC";
				SqliteCommand sqlCommand = new(sqlQuery, _connection);
				SqliteDataReader sqlReader = sqlCommand.ExecuteReader();

				while (sqlReader.Read())
				{
					int id = sqlReader.GetInt32(0);
					string playerName = sqlReader.GetString(1);
					int scores = sqlReader.GetInt32(2);
					records.Add(new RecordData { Counter = id, PlayerName = playerName, Scores = scores });
				}

				sqlReader.Close();
				RecordsDataGrid.ItemsSource = records;
				_connection.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при чтении базы данных: {ex.Message}", "ОШИБКА" , MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}