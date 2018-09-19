using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OverlayManager.Hubs
{
	public interface IOverlayCommands
	{
		Task ExecuteCommand(string command, string args);
	}

	public class CodeRushedHub: Hub<IOverlayCommands>
	{

	}
}
