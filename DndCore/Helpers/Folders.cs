using System;
using System.Linq;

namespace DndCore
{
	public static class Folders
	{
		public static bool UseTestData { get; set; }
		public const string CoreData = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\DndCore\Data";
		public const string TestCoreData = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\DndCore\Data\TestData";
		public static string InCoreData(string fileName)
		{
			string folder = UseTestData? TestCoreData: CoreData;
			return System.IO.Path.Combine(folder, fileName);
		}
	}
}
