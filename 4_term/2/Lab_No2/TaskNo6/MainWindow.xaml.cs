using System.IO;
using System.Windows;

using Microsoft.Win32;

namespace TaskNo6
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new()
			{
				FileName = "Документ",
				DefaultExt = ".txt",
				Filter = "Текстовые документы (.txt)|*.txt",
				InitialDirectory = "C:\\",
				RestoreDirectory = true
			};
			bool? isSuccessful = openDialog.ShowDialog();

			if (isSuccessful!.Value)
			{
				Stream fs = openDialog.OpenFile();

				using StreamReader reader = new(fs);
				FileTextContent.Text = reader.ReadToEnd();
			}
			else
				MessageBox.Show("Не удалось открыть файл!", "Задание №6", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveDialog = new()
			{
				FileName = "Документ",
				DefaultExt = ".txt",
				Filter = "Текстовые документы (.txt)|*.txt",
				InitialDirectory = "C:\\",
				RestoreDirectory = true
			};
			bool? isSuccessful = saveDialog.ShowDialog();

			if (isSuccessful!.Value)
			{
				Stream fs = saveDialog.OpenFile();

				using StreamWriter writer = new(fs);
				writer.Write(FileTextContent.Text);
			}
			else
				MessageBox.Show("Не удалось сохранить файл!", "Задание №6", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}