using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskNo2
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

        private void AddString_Click(object sender, RoutedEventArgs e)
        {
            SummaryText.AppendText(InputText.Text + '\n'); // Добавление строки к нижнему текстовому полю
            InputText.Text = string.Empty; // Очищение верхнего текстового поля
        }
    }
}