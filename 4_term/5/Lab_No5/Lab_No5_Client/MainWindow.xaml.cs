using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Lab_No5_Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string IP_ADDRESS = "127.0.0.1";// Локальный адресс нашего ПК
        private const int PORT = 7000;// Порт для подключения
        private const int MAX_DATA_LENGTH = 512;
		private const char DELIMETER = '$';

		private readonly IPEndPoint _endPoint;
		private readonly Socket _socket;

		private byte[] _sentData;

		public MainWindow()
		{
			_endPoint = new(IPAddress.Parse(IP_ADDRESS), PORT);
			_socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_sentData = new byte[MAX_DATA_LENGTH];

			InitializeComponent();
		}

		~MainWindow()// Деструктор для закрытия сокета при выходк из приложения
        {
			_socket.Shutdown(SocketShutdown.Both);
			_socket.Close();
			_socket.Dispose();
		}

		private void SendMessage_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(UserNameField.Text) || string.IsNullOrEmpty(MessageField.Text))
				{
					MessageBox.Show("Введите данные перед отправкой на сервер!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

					return;
				}

				string username = UserNameField.Text;
				string message = MessageField.Text;

				ServerResponse.Text = string.Empty;

				StringBuilder finalDataString = new();
				finalDataString.Append(username); // Добавляем имя того, кто отправил сообщение
				finalDataString.Append(DELIMETER); // Добавляем разделитель
				finalDataString.Append(message); // Добавляем само сообщение

				_sentData = Encoding.UTF8.GetBytes(finalDataString.ToString());
				_socket.Connect(_endPoint); // Подключаемся к порту локалхоста
				_socket.Send(_sentData); // Отправляем сообщение на порт

				StringBuilder responseData = new();
				byte[] data = new byte[MAX_DATA_LENGTH];
				int responseDataSize = default;

				do
				{
					responseDataSize = _socket.Receive(data);// Записываем ответ сервера
					responseData.Append(Encoding.UTF8.GetString(data, 0, responseDataSize));// Выводим ответ сервера
				}
				while (_socket.Available > 0);// Ждем ответа от сервера

				ServerResponse.Text = responseData.ToString();
				_socket.Shutdown(SocketShutdown.Both);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				_socket.Close();
			}
		}
	}
}