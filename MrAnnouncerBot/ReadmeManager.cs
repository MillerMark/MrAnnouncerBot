﻿using Newtonsoft.Json;
using SheetsPersist;
using System;
using BotCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MrAnnouncerBot
{
	public static class ReadmeManager
	{
		const string STR_GuyPrefix = "Guy - ";
		static string GetSceneName(string sceneName)
		{
			if (sceneName.StartsWith(STR_GuyPrefix))
				return sceneName.Substring(STR_GuyPrefix.Length);
			return sceneName;
		}

		static string GetPicMarkDown(string pic)
		{
			if (string.IsNullOrEmpty(pic))
				return "";

			return $"![]({pic})" + Environment.NewLine + Environment.NewLine;
		}

		static void AddLevelDocs(StringBuilder finalReadme)
		{
			var levels = CsvData.Get<LevelDto>(FileName.SceneLevels);
			var levelTemplate = File.ReadAllText(FileName.LevelTemplate).Split("\r\n");

			bool needToAddScenes = false;

			//var scenes = CsvData.Get<SceneDto>(FileName.SceneData);
			var scenes = GoogleSheets.Get<SceneDto>();
			foreach (var level in levels)
			{
				List<SceneDto> filteredScenes = scenes.Where(x => x.LevelStr == level.Level && x.LimitToUser == "")
					.OrderBy(x => x.ChatShortcut)
					.ToList();
				if (filteredScenes.Count == 0)
					continue;

				foreach (string line in levelTemplate)
				{
					string replacedLine = line.Replace("$LevelNumber$", level.Level)
																		.Replace("$LevelDoc$", level.Doc)
																		.Replace("$LevelPic$", GetPicMarkDown(level.Pic));
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
		public static bool NeedToGenerateNewReadme()
		{
			//const int numFilesTracked = 4;  // Change this if we ever add more files to track.

			//TimeStampChecker timeStampChecker = TimeStampChecker.Load();
			//if (timeStampChecker != null && timeStampChecker.TimeStamps.Count == numFilesTracked)
			//{
			//	if (timeStampChecker.NoChanges())
			//		return false;
			//}
			//else
			//{
			//	timeStampChecker = BuildTimeStampCheckers();

			//	if (timeStampChecker.TimeStamps.Count != numFilesTracked)
			//	{
			//		throw new Exception($"timeStampChecker.TimeStamps.Count ({timeStampChecker.TimeStamps.Count}) != numFilesTracked ({numFilesTracked})");
			//	}
			//}
			return true;
		}

		//private static TimeStampChecker BuildTimeStampCheckers()
		//{
		//	TimeStampChecker timeStampChecker = new TimeStampChecker();
		//	timeStampChecker.Add(FileName.ReadmeTemplate);
		//	timeStampChecker.Add(FileName.SceneData);
		//	timeStampChecker.Add(FileName.LevelTemplate);
		//	timeStampChecker.Add(FileName.SceneLevels);
		//	return timeStampChecker;
		//}

		public static void GenerateNewReadme()
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

			//var timeStampChecker = BuildTimeStampCheckers();
			//timeStampChecker.Save();
		}
	}
}