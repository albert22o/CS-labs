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
		private const string IP_ADDRESS = "127.0.0.1";
		private const int PORT = 7000;
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

		~MainWindow()
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
				finalDataString.Append(username);
				finalDataString.Append(DELIMETER);
				finalDataString.Append(message);

				_sentData = Encoding.UTF8.GetBytes(finalDataString.ToString());
				_socket.Connect(_endPoint);
				_socket.Send(_sentData);

				StringBuilder responseData = new();
				byte[] data = new byte[MAX_DATA_LENGTH];
				int responseDataSize = default;

				do
				{
					responseDataSize = _socket.Receive(data);
					responseData.Append(Encoding.UTF8.GetString(data, 0, responseDataSize));
				}
				while (_socket.Available > 0);

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