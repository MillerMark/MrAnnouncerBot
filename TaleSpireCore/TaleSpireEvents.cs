using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TaleSpireCore
{
	public static class TaleSpireEvents
	{
		const string MarkMachine = "192.168.1.136";
		static bool serverStarted;
		static void HandleEvent(string data)
		{

		}
		public static void StartSocketServer()
		{
			if (serverStarted)
				return;

			serverStarted = true;
			Task.Factory
				.StartNew(
					() =>
					{
						int port = 998;
						UnityEngine.Debug.Log($"Starting Events Listening Socket at {MarkMachine} and Port: {port}");
						IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(MarkMachine), port);
						Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

						try
						{
							listener.Bind(localEndPoint);
							listener.Listen(10);

							while (true)
							{
								while (true)
								{
									Socket socket = listener.Accept();
									string data = string.Empty;

									byte[] buffer = TaleSpireUtils.ReceiveAll(socket);
									int bytesRec = buffer.Length;
									data += Encoding.UTF8.GetString(buffer, 0, bytesRec);

									HandleEvent(data);

									//byte[] cmdResult = Encoding.UTF8.GetBytes();
									//socket.Send(cmdResult);

									socket.Shutdown(SocketShutdown.Both);
									socket.Close();
								}
							}
						}
						catch (Exception e)
						{
							UnityEngine.Debug.Log(e.ToString());
						}
					});
		}
	}
}
