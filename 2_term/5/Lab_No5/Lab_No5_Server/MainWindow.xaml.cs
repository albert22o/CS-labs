using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Lab_No5_Server
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string IP_ADDRESS = "127.0.0.1";
		private const int PORT = 7000;
		private const int MAX_AVAILABLE_CONNECTIONS = 5;
		private const int MAX_DATA_LENGTH = 512;
		private const char DELIMETER = '$';

		private readonly IPEndPoint _endPoint;
		private readonly Socket _socket;
		private readonly Thread _processThread;

		private bool _isServerRunning;

		public MainWindow()
		{
			_endPoint = new(IPAddress.Parse(IP_ADDRESS), PORT);
			_socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_socket.Bind(_endPoint);
			_socket.Listen(MAX_AVAILABLE_CONNECTIONS);
			_processThread = new(new ThreadStart(ProcessServer));

			InitializeComponent();
		}

		~MainWindow()
		{
			if (_processThread.ThreadState == ThreadState.Running) _processThread.Join();
			_socket.Shutdown(SocketShutdown.Both);
			_socket.Close();
			_socket.Dispose();
		}

		private void ProcessServer()
		{
			while (true)
			{
				Socket listener = _socket.Accept();

				try
				{
					StringBuilder requestData = new();
					byte[] data = new byte[MAX_DATA_LENGTH];
					int requestDataSize = default;
					string reversedMessage = string.Empty;

					do
					{
						requestDataSize = listener.Receive(data);
						requestData.Append(Encoding.UTF8.GetString(data, 0, requestDataSize));
						string requestDataInStr = requestData.ToString();
						int delimeterIndex = requestDataInStr.IndexOf(DELIMETER);
						string username = requestData.ToString()[..delimeterIndex];
						string message = requestData.ToString()[(delimeterIndex + 1)..];
						Dispatcher.BeginInvoke(() => RequestList.Items.Add($"Сообщение {message} получено от {username}."));
						reversedMessage = Reverse(message);
					}
					while (listener.Available > 0);

					listener.Send(Encoding.UTF8.GetBytes(reversedMessage));
					listener.Shutdown(SocketShutdown.Both);
					Dispatcher.BeginInvoke(() => RequestList.Items.Add($"Отправляю ответ {reversedMessage}"));
				}
				catch (Exception ex)
				{
					Dispatcher.BeginInvoke(() => RequestList.Items.Add($"Ошибка {ex.Message}"));
				}
				finally
				{
					listener.Close();
				}
			}
		}

		private static string Reverse(string input)
		{
			StringBuilder builder = new();

			for (int i = input.Length - 1; i >= 0; --i)
				builder.Append(input[i]);

			return builder.ToString();
		}

		private void StartStopServer_Click(object sender, RoutedEventArgs e)
		{
			if (!_isServerRunning)
			{
				_isServerRunning = true;
				if (_processThread.ThreadState == ThreadState.Unstarted) _processThread.Start();
				else if (Monitor.IsEntered(_processThread)) Monitor.Exit(_processThread);
				StartStopServer.Content = "Остановить сервер";
			}
			else
			{
				_isServerRunning = false;
				Monitor.Enter(_processThread);
				StartStopServer.Content = "Запустить сервер";
			}
		}
	}
}