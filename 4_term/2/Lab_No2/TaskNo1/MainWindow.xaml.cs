using System.Windows;

namespace TaskNo1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _firstNumber, _secondNumber, _resultNumber;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool ValidateInput()
        {
            if (double.TryParse(FirstNumber.Text, out _firstNumber) && double.TryParse(SecondNumber.Text, out _secondNumber))
                return true;
            else
            {
                MessageBox.Show("Введены некорректные значения!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);

                FirstNumber.Text = string.Empty;
                SecondNumber.Text = string.Empty;

                return false;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            _resultNumber = _firstNumber + _secondNumber;
            ResultNumber.Text = _resultNumber.ToString();
        }

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            _resultNumber = _firstNumber - _secondNumber;
            ResultNumber.Text = _resultNumber.ToString();
        }

        private void MulButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            _resultNumber = _firstNumber * _secondNumber;
            ResultNumber.Text = _resultNumber.ToString();
        }

        private void DivButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            if (_secondNumber == 0.0)
            {
                MessageBox.Show("Делитель равен нулю!", "Ошибка времени выполнения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                _resultNumber = _firstNumber / _secondNumber;
                ResultNumber.Text = _resultNumber.ToString();
            }
        }
    }
}