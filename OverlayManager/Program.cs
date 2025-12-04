using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace OverlayManager {
	public class Program {
		public static void Main(string[] args) {
			CreateWebHostBuilder(args).Build().Run();
		}

		public static string WebRootFolder {
			get {
				if (Debugger.IsAttached)
					return "wwwroot";
				else
					return "..\\..\\..\\wwwroot";
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
				WebHost.CreateDefaultBuilder(args).UseUrls("http://localhost:64303/").UseWebRoot(WebRootFolder)
						.UseStartup<Startup>();
	}
}
