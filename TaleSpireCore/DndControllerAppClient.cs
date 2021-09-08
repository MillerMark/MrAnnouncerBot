using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace TaleSpireCore
{
	public static class DndControllerAppClient
	{
		private const string DndMachine = "192.168.1.137";

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
}
