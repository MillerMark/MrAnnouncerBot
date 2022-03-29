using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OverlayManager.Hubs;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace OverlayManager
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddDirectoryBrowser();
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
			services.AddSignalR(o => o.EnableDetailedErrors = true);
			services.AddHostedService<BackgroundWorker>();
			services.AddMvc(options => options.EnableEndpointRouting = false);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostEnvironment env)
		{
			//app.UseDirectoryBrowser();
			if (env.IsDevelopment())
			{
				//app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			//app.UseStaticFiles();

			app.UseStaticFiles(new StaticFileOptions
			{
				ServeUnknownFileTypes = true
			});

			app.UseRouting();

			app.UseCookiePolicy();

			app.UseEndpoints(endpoints => {
				// Browser connects here.
				endpoints.MapHub<MrAnnouncerBotHub>("/MrAnnouncerBotHub");
				endpoints.MapHub<CodeRushedHub>("/CodeRushedHub");
			});
			app.UseMvc();
		}
	}
}
