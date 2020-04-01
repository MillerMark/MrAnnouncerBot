using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OverlayManager
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static string WebRootFolder
		{
			get
			{
				if (Debugger.IsAttached)
					return "wwwroot";
				else
					return "..\\..\\..\\wwwroot";
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
				WebHost.CreateDefaultBuilder(args).UseUrls("http://localhost:44303/").UseWebRoot(WebRootFolder)
						.UseStartup<Startup>();
	}
}
