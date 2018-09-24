using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OverlayManager.Hubs
{
	public interface IOverlayCommands
	{
		Task ExecuteCommand(string command, string args, string userId, string displayName, string color);
	}

	public class CodeRushedHub: Hub<IOverlayCommands>
	{
		public void Chat(string message)
		{
			// TODO: Implement this!
		}
	}
}
