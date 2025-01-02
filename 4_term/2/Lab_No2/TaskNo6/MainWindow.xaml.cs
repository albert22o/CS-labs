using System.IO;
using System.Windows;

using Microsoft.Win32;

namespace TaskNo6
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаём диалог для открытия файла.
            OpenFileDialog openDialog = new()
            {
                FileName = "Документ", // Имя файла по умолчанию.
                DefaultExt = ".txt", // Расширение по умолчанию.
                Filter = "Текстовые документы (.txt)|*.txt", // Фильтр файлов.
                InitialDirectory = "C:\\", // Начальная директория.
                RestoreDirectory = true // Восстанавливать начальную директорию после работы.
            };

            // Открываем диалог и проверяем результат.
            bool? isSuccessful = openDialog.ShowDialog();

            if (isSuccessful!.Value)
            {
                // Открываем поток файла для чтения.
                Stream fs = openDialog.OpenFile();

                using StreamReader reader = new(fs);
                // Читаем содержимое файла и выводим его в текстовое поле.
                FileTextContent.Text = reader.ReadToEnd();
            }
            else
            {
                // Выводим сообщение об ошибке, если файл не удалось открыть.
                MessageBox.Show("Не удалось открыть файл!", "Задание №6", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаём диалог для сохранения файла.
            SaveFileDialog saveDialog = new()
            {
                FileName = "Документ", // Имя файла по умолчанию.
                DefaultExt = ".txt", // Расширение по умолчанию.
                Filter = "Текстовые документы (.txt)|*.txt", // Фильтр файлов.
                InitialDirectory = "C:\\", // Начальная директория.
                RestoreDirectory = true // Восстанавливать начальную директорию после работы.
            };

            // Открываем диалог и проверяем результат.
            bool? isSuccessful = saveDialog.ShowDialog();

            if (isSuccessful!.Value)
            {
                // Открываем поток файла для записи.
                Stream fs = saveDialog.OpenFile();

                using StreamWriter writer = new(fs);
                // Записываем содержимое текстового поля в файл.
                writer.Write(FileTextContent.Text);
            }
            else
            {
                // Выводим сообщение об ошибке, если файл не удалось сохранить.
                MessageBox.Show("Не удалось сохранить файл!", "Задание №6", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
