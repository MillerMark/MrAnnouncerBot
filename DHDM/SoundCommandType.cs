using System;
using System.Linq;

namespace DHDM
{
	//! Changes here should be kept in sync with SoundCommandType in MusicPlayer.ts
	public enum SoundCommandType
	{
		VolumeUp,
		VolumeDown,
		SetVolume,
		SuppressVolume,
		ChangeTheme,
		//PlaySoundFile,
		//ChangeWeather,
		//ChangeLocationAmbience
	}
}
