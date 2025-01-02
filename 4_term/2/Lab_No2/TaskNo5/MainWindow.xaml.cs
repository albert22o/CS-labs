using System.Windows;
using System.Windows.Threading;

namespace TaskNo5
{

    public partial class MainWindow : Window
    {
        // Таймер для отсчёта времени.
        private readonly System.Timers.Timer _timer;

        // Текущее значение времени таймера.
        private TimeOnly _tickingTime;

        // Счётчик сохранённых значений времени.
        private int _savesCount;

        // Флаг, показывающий, работает ли таймер.
        private bool _isTimerRunning;

        public MainWindow()
        {
            // Инициализация таймера с интервалом в 1 секунду.
            _timer = new System.Timers.Timer(1_000);
            _tickingTime = new TimeOnly(0, 0, 0);

            // Подписка на событие "тик" таймера.
            _timer.Elapsed += OnElapsed;

            InitializeComponent();
        }

        private void OnElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // Увеличиваем значение времени на 1 секунду.
            _tickingTime = _tickingTime.Add(new TimeSpan(0, 0, 1));

            // Обновляем интерфейс из UI-потока.
            Dispatcher.Invoke(() => CurrentTimer.Content = $"{_tickingTime:T}");
        }


        private void TimerStart_Click(object sender, RoutedEventArgs e)
        {
            _timer.Start();
            _isTimerRunning = true;
        }


        private void TimerReset_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _isTimerRunning = false;
            _tickingTime = new TimeOnly(0, 0, 0);
            CurrentTimer.Content = "0:00:00"; // Обновляем интерфейс.
        }

    
        private void TimerRemember_Click(object sender, RoutedEventArgs e)
        {
            if (!_isTimerRunning)
            {
                // Показываем сообщение об ошибке, если таймер не работает.
                MessageBox.Show("Для сохранения значения запустите таймер!", "Задание №5", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ++_savesCount;

            // Добавляем сохранённое значение в текстовое поле, форматируя его в зависимости от состояния флажка.
            if (TimeFormatting.IsChecked!.Value == true)
                TimerMemory.Text += $"Время {_savesCount}: {_tickingTime:T}\n"; // Полный формат времени.
            else
                TimerMemory.Text += $"Время {_savesCount}: {_tickingTime.Second} сек.\n"; // Только секунды.
        }
    }
}
