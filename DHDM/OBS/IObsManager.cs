//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	// TODO: Rename this to IDndObsManager
	public interface IObsManager
	{
		void PlayScene(string sceneName, int returnMs = -1);
		void PlaySceneAfter(string sceneName, int delayMs, int returnMs);
		void ShowPlateBackground(string sourceName);
		void ShowPlateForeground(string sourceName);
		void ShowWeather(string weatherKeyword);
	}
}
