using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MrAnnouncerBot
{
	public static class DocChecker
	{
		const string STR_GuyPrefix = "Guy - ";
		static string GetSceneName(string sceneName)
		{
			if (sceneName.StartsWith(STR_GuyPrefix))
				return sceneName.Substring(STR_GuyPrefix.Length);
			return sceneName;
		}

		static void AddLevelDocs(StringBuilder finalReadme)
		{
			var levels = CsvData.Get<LevelDto>(FileName.SceneLevels);
			var levelTemplate = File.ReadAllText(FileName.LevelTemplate).Split("\r\n");

			bool needToAddScenes = false;

			var scenes = CsvData.Get<SceneDto>(FileName.SceneData);
			foreach (var level in levels)
			{
				List<SceneDto> filteredScenes = scenes.Where(x => x.LevelStr == level.Level && x.LimitToUser == "").ToList();
				if (filteredScenes.Count == 0)
					continue;

				foreach (string line in levelTemplate)
				{
					string replacedLine = line.Replace("$LevelNumber$", level.Level)
																		.Replace("$LevelDoc$", level.Doc);
					if (needToAddScenes)
					{
						foreach (SceneDto scene in filteredScenes)
						{
							string sceneLine = replacedLine.Replace("$Shortcut$", scene.ChatShortcut)
																						 .Replace("$Scene$", GetSceneName(scene.SceneName))
																						 .Replace("$Category$", scene.Category);
							finalReadme.AppendLine(sceneLine);
						}
						needToAddScenes = false;
					}
					else
						finalReadme.AppendLine(replacedLine);

					if (line.StartsWith("|") && line.IndexOf("-") > 0)
						needToAddScenes = true;
				}
			}

		}
		public static void GenerateIfNecessary()
		{
			string readmeTemplate = File.ReadAllText(FileName.ReadmeTemplate);
			string[] readmeTemplateLines = readmeTemplate.Split("\r\n");
			StringBuilder finalReadme = new StringBuilder();
			foreach (string line in readmeTemplateLines)
			{
				if (line == "//LevelDocs")
					AddLevelDocs(finalReadme);
				else
					finalReadme.AppendLine(line);
			}

			File.WriteAllText(@"..\..\..\..\README.md", finalReadme.ToString());
		}
	}
}