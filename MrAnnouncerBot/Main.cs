using CsvHelper;
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace MrAnnouncerBot
{
	class Program
	{
		static void Main(string[] args)
		{
			var mrAnnouncerBot = new MrAnnouncerBot();
			mrAnnouncerBot.Run();
			Console.ReadLine();
			// TODO: async, task, etc.
			mrAnnouncerBot.Disconnect();
		}
	}
}
