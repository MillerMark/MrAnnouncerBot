using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace TaleSpireCore
{
	public static class DndControllerAppClient
	{
		private const string DndMachine = "192.168.22.252";

		public static void SendEventToServer(string command, string[] msgparams)
		{
			// Connect to a remote device.  
			try
			{
				int port = 998;
				IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(DndMachine), port);

				// Create a TCP/IP  socket.  
				Socket sender = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				// Connect the socket to the remote endpoint. Catch any errors.  
				try
				{
					sender.Connect(localEndPoint);

					Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

					// Encode the data string into a byte array.  
					byte[] msg = Encoding.UTF8.GetBytes(command + " " + string.Join(",", msgparams));

					// Send the data through the socket.  
					int bytesSent = sender.Send(msg);
					Console.WriteLine("Bytes sent:" + bytesSent.ToString());
					Console.WriteLine("Command Sent: " + Encoding.UTF8.GetString(msg, 0, bytesSent));
					
					// Release the socket.  
					sender.Shutdown(SocketShutdown.Both);
					sender.Close();
				}
				catch (ArgumentNullException ane)
				{
					Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
				}
				catch (SocketException se)
				{
					Console.WriteLine("SocketException : {0}", se.ToString());
				}
				catch (Exception e)
				{
					Console.WriteLine("Unexpected exception : {0}", e.ToString());
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public static void SendEventToServer(string eventName)
		{
			SendEventToServer(eventName, new string[] { });
		}
	}
	public static class TaleSpireClient
	{
		private const string TaleSpireMachine = "192.168.22.42";

		public static ApiResponse SendMessageToServer(string command, float num)
		{
			return SendMessageToServer(command, new string[] { num.ToString() });
		}

		static ApiResponse ToApiResponse(string data)
		{
			try
			{
				return JsonConvert.DeserializeObject<ApiResponse>(data);
			}
			catch (Exception ex)
			{
				return new ApiResponse(ex.Message, "Failed to deserialize response!", data);
			}
		}

		public static ApiResponse SendMessageToServer(string command, string[] msgparams)
		{
			// Data buffer for incoming data.  
			byte[] bytes = new byte[4 * 1024 * 1024];

			// Connect to a remote device.  
			try
			{
				int port = 999;
				IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(TaleSpireMachine), port);

				// Create a TCP/IP  socket.  
				Socket sender = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				// Connect the socket to the remote endpoint. Catch any errors.  
				try
				{
					sender.Connect(localEndPoint);

					Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

					// Encode the data string into a byte array.  
					byte[] msg = Encoding.UTF8.GetBytes(command + " " + string.Join(",", msgparams));

					// Send the data through the socket.  
					int bytesSent = sender.Send(msg);
					Console.WriteLine("Bytes sent:" + bytesSent.ToString());
					Console.WriteLine("Command Sent: " + Encoding.UTF8.GetString(msg, 0, bytesSent));
					sender.ReceiveTimeout = 3000;
					// Receive the response from the remote device.  
					string data = string.Empty;
					int bytesRec = 0;
					int sleeps = 0;
					while (sender.Available == 0 && sleeps < 3000)
					{
						System.Threading.Thread.Sleep(1);
						sleeps++;
					}
					while (sender.Available > 0)
					{
						bytesRec = sender.Receive(bytes);
						data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
					}
					//int bytesRec = sender.Receive(bytes, 0, sender.Available, SocketFlags.None);
					//int bytesRec = sender.Receive(bytes);
					//var data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
					Console.WriteLine("Server responded bytes: {0} {1}", bytesRec, data);
					// Release the socket.  
					sender.Shutdown(SocketShutdown.Both);
					sender.Close();
					return ToApiResponse(data);
				} catch (ArgumentNullException ane)
				{
					Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
					return ApiResponse.FromException(ane);
				} catch (SocketException se)
				{
					Console.WriteLine("SocketException : {0}", se.ToString());
					return ApiResponse.FromException(se);
				} catch (Exception e)
				{
					Console.WriteLine("Unexpected exception : {0}", e.ToString());
					return ApiResponse.FromException(e);
				}
			} catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return ApiResponse.FromException(e);
			}
		}
		public static ApiResponse SendMessageToServer(string message)
		{
			return SendMessageToServer(message, new string[] { });
		}
	}
}
