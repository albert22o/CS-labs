using System.Windows;

namespace TaskNo1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Поля для хранения значений первого, второго числа и результата
        private double _firstNumber, _secondNumber, _resultNumber;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод для проверки корректности введенных данных
        private bool ValidateInput()
        {
            // Проверяем, что оба текстовых поля содержат числа
            if (double.TryParse(FirstNumber.Text, out _firstNumber) && double.TryParse(SecondNumber.Text, out _secondNumber))
                return true; // Если всё корректно, возвращаем true
            else
            {
                // Если данные некорректны, показываем сообщение об ошибке
                MessageBox.Show("Введены некорректные значения!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);

                // Очищаем текстовые поля для ввода чисел
                FirstNumber.Text = string.Empty;
                SecondNumber.Text = string.Empty;

                return false; // Возвращаем false, чтобы остановить выполнение
            }
        }

        // Обработчик кнопки сложения
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return; // Проверяем входные данные, выходим при ошибке

            // Выполняем сложение и обновляем текстовое поле с результатом
            _resultNumber = _firstNumber + _secondNumber;
            ResultNumber.Text = _resultNumber.ToString();
        }

        // Обработчик кнопки вычитания
        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return; // Проверяем входные данные, выходим при ошибке

            // Выполняем вычитание и обновляем текстовое поле с результатом
            _resultNumber = _firstNumber - _secondNumber;
            ResultNumber.Text = _resultNumber.ToString();
        }

        // Обработчик кнопки умножения
        private void MulButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return; // Проверяем входные данные, выходим при ошибке

            // Выполняем умножение и обновляем текстовое поле с результатом
            _resultNumber = _firstNumber * _secondNumber;
            ResultNumber.Text = _resultNumber.ToString();
        }

        // Обработчик кнопки деления
        private void DivButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return; // Проверяем входные данные, выходим при ошибке

            // Проверяем деление на ноль
            if (_secondNumber == 0.0)
            {
                // Сообщаем об ошибке, если делитель равен нулю
                MessageBox.Show("Делитель равен нулю!", "Ошибка времени выполнения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Выполняем деление и обновляем текстовое поле с результатом
                _resultNumber = _firstNumber / _secondNumber;
                ResultNumber.Text = _resultNumber.ToString();
            }
        }
    }
}
