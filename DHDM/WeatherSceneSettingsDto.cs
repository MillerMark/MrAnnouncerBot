//#define profiling
using System;
using GoogleHelper;
using System.Linq;

namespace DHDM
{
	[SheetName("DnD")]
	[TabName("Weather")]
	public class WeatherSceneSettingsDto
	{
		public string Keyword { get; set; }
		public string IconItem { get; set; }
		public string Duration { get; set; }
		public string BackItem { get; set; }
		public string BackSceneDuration { get; set; }
		public string FrontItem { get; set; }
		public string FrontSceneDuration { get; set; }
	}
}