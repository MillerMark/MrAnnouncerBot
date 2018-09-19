using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using OverlayManager.Hubs;
using TwitchLib.Client;

namespace OverlayManager.Pages
{
	public class IndexModel : PageModel
	{
		readonly IHubContext<CodeRushedHub, IOverlayCommands> hub;
		public IndexModel(IHubContext<CodeRushedHub, IOverlayCommands> hub)
		{
			this.hub = hub;
		}
		public void OnGet()
		{

		}

		public void OnPost()
		{
		}
	}
}
