﻿//! Changes here should be kept in sync with SoundCommandType in SoundCommandType.cs
enum SoundCommandType {
	VolumeUp,
	VolumeDown,
	SetVolume,
	SuppressVolume,
	ChangeTheme,
	//PlaySoundFile,
	//ChangeWeather,
	//ChangeLocationAmbience
}

enum SongState {
	fadingIn,
	playingNormal,
	fadingOut,
	finishedPlaying
}

class SoundCommand {
	constructor(public type: SoundCommandType, public numericData: number, public strData: string) {

	}
}

class Theme {
	constructor(public name: string, public count: number) {

	}
}

class MusicPlayer {
	static themes: Array<Theme> = [];
	static activeGenre: string = 'Travel';
	static activeSongCount: number;
	static volume: number = 2;
	static saveVolume: number = 0;
	static readonly suppressVolumeLevel: number = 0.5;
	static readonly crossFadeTime: number = 5000;

	static nextMusicPlayer: MusicPlayer;
	static thisMusicPlayer: MusicPlayer;

	state: SongState;
	fadeInTime: number = MusicPlayer.crossFadeTime;
	fadeOutTime: number = MusicPlayer.crossFadeTime;
	expirationDate: number;


	soundManager: SoundManager = new SoundManager('GameDev/Assets/DragonH/Music');
	activeSong: HTMLAudioElement;
	static suppressingVolume: boolean = false;
	static suppressingVolumeEnds: number;
	timeStart: number;

	constructor() {
		this.timeStart = performance.now();
		const minSecondsToPlay: number = 10;
		this.expirationDate = this.timeStart + minSecondsToPlay * 1000 + this.fadeOutTime;
	}

	static Initialize() {
		MusicPlayer.addSongs('Bittersweet', 27);
		MusicPlayer.addSongs('Battle', 42);
		MusicPlayer.addSongs('Travel', 34);
		MusicPlayer.addSongs('Suspense', 60);
		MusicPlayer.addSongs('TestBattle', 10);
		MusicPlayer.addSongs('TestSuspense', 10);
	}

	private static changeActiveGenre(newGenre: string): void {
		MusicPlayer.themes.forEach(function (theme: Theme) {
			if (theme.name.toLowerCase() == newGenre.toLowerCase()) {
				MusicPlayer.activeGenre = theme.name;
				MusicPlayer.activeSongCount = theme.count;
				tellDM(`Switching theme music to ${MusicPlayer.activeGenre}...`);
				if (!MusicPlayer.thisMusicPlayer && !MusicPlayer.nextMusicPlayer) {
					MusicPlayer.thisMusicPlayer = new MusicPlayer();
					MusicPlayer.thisMusicPlayer.playRandomSong();
				}
				return;
			}
		}, this);
	}

	static changeGenre(newGenre: string): void {
		MusicPlayer.changeActiveGenre(newGenre);
		// Cross-fade songs now. That means create a second MusicPlayer and expire the current song.
		if (MusicPlayer.nextMusicPlayer) {
			MusicPlayer.nextMusicPlayer.pauseActiveSong();
		}

		if (MusicPlayer.thisMusicPlayer) {
			MusicPlayer.thisMusicPlayer.expirationDate = performance.now() + MusicPlayer.crossFadeTime;

			MusicPlayer.nextMusicPlayer = new MusicPlayer();
			MusicPlayer.nextMusicPlayer.playRandomSong();
		}
		else {
			MusicPlayer.thisMusicPlayer = MusicPlayer.nextMusicPlayer;
			if (!MusicPlayer.thisMusicPlayer)
				MusicPlayer.thisMusicPlayer = new MusicPlayer();

			MusicPlayer.thisMusicPlayer.playRandomSong();
			MusicPlayer.nextMusicPlayer = null;
		}
	}

	static stopMusic(): void {

	}



	pauseActiveSong() {
		if (this.activeSong)
			this.activeSong.pause();
	}

	static trimVolume() {
		MusicPlayer.volume = Math.round(MusicPlayer.volume);
		if (MusicPlayer.volume > 11)
			MusicPlayer.volume = 11;
		if (MusicPlayer.volume < 1)
			MusicPlayer.volume = 1;
	}

	static volumeDown(): void {
		MusicPlayer.setVolumeTo(MusicPlayer.volume - 1);
	}


	static volumeUp(): void {
		MusicPlayer.setVolumeTo(MusicPlayer.volume + 1);
	}

	static setVolumeTo(volStr: number | string): void {
		let newVolume: number = +volStr;
		if (!newVolume)
			return;

		MusicPlayer.volume = newVolume;
		MusicPlayer.actOnNewVolume();
	}

	static actOnNewVolume(): void {
		MusicPlayer.trimVolume();
		MusicPlayer.setVolumeForInstance(MusicPlayer.nextMusicPlayer);
		MusicPlayer.setVolumeForInstance(MusicPlayer.thisMusicPlayer);
		MusicPlayer.reportVolume();
	}

	private static setVolumeForInstance(player: MusicPlayer) {
		if (player)
			player.setVolumeForActiveSong();
	}

	static reportVolume(): any {
		tellDM('Theme music volume is: ' + MusicPlayer.volume);
	}

	setVolumeForActiveSong() {
		let actualVolume: number;
		let thisVolume: number = Math.round(MusicPlayer.volume);
		if (thisVolume >= 4)
			actualVolume = thisVolume - 3;
		else
			switch (thisVolume) {
				case 3:
					actualVolume = 0.75;
					break;
				case 2:
					actualVolume = 0.5;
					break;
				case 1:
					actualVolume = 0.25;
					break;
			}
		if (this.activeSong)
			this.activeSong.volume = this.fadeVolumeMultiplier * actualVolume / 8;
	}

	static addSongs(theme: string, count: number) {
		MusicPlayer.themes.push(new Theme(theme, count));
	}

	static updateMusicPlayers(now: number) {
		let thisPlayer: MusicPlayer = MusicPlayer.thisMusicPlayer;
		if (thisPlayer) {
			thisPlayer.update(now);
			if (thisPlayer.state === SongState.playingNormal && MusicPlayer.nextMusicPlayer) {
				MusicPlayer.nextMusicPlayer.pauseActiveSong();
				MusicPlayer.nextMusicPlayer = null;  // next player is null as long as this song is normally playing.
			}
			if (thisPlayer.state === SongState.fadingOut && MusicPlayer.nextMusicPlayer === null) {
				MusicPlayer.nextMusicPlayer = new MusicPlayer();
				MusicPlayer.nextMusicPlayer.playRandomSong();
			}
			if (thisPlayer.state === SongState.finishedPlaying) {
				MusicPlayer.thisMusicPlayer = MusicPlayer.nextMusicPlayer;
				if (!MusicPlayer.thisMusicPlayer || MusicPlayer.thisMusicPlayer.state == SongState.finishedPlaying)
					MusicPlayer.thisMusicPlayer = new MusicPlayer();
				MusicPlayer.nextMusicPlayer = null;
			}
		}
		if (MusicPlayer.nextMusicPlayer) {
			MusicPlayer.nextMusicPlayer.update(now);
			if (MusicPlayer.nextMusicPlayer.state !== SongState.fadingIn) {
				if (thisPlayer)
					thisPlayer.pauseActiveSong();
				MusicPlayer.thisMusicPlayer = MusicPlayer.nextMusicPlayer;
				MusicPlayer.nextMusicPlayer = null;
			}
		}
	}

	playRandomSong() {
		const fiveMinutes: number = 5 * 60 * 1000;
		let index: number = Random.intBetween(1, MusicPlayer.activeSongCount);
		let tries: number = 0;
		const maxTries: number = 30;
		let fileName: string = this.getFileName(index);
		while (tries < maxTries && this.soundManager.playedRecently(fileName, fiveMinutes)) {
			index = Random.intBetween(1, MusicPlayer.activeSongCount);
			fileName = this.getFileName(index);
			tries++;
		}
		//console.log('playRandomSong - fileName: ' + fileName);
		if (this.activeSong) {
			this.activeSong.pause();
		}
		this.activeSong = this.soundManager.safePlayMp3ReturnAudio(fileName, fiveMinutes);
		if (!this.activeSong)
			this.activeSong = this.soundManager.safePlayMp3ReturnAudio(fileName, 0);

		this.setVolumeForActiveSong();

		this.activeSong.addEventListener('loadedmetadata', function () {
			console.log(`${fileName} duration: ` + this.activeSong.duration);
			let durationSec: number = this.activeSong.duration * 1000;
			this.expirationDate = this.timeStart + durationSec;
		}.bind(this), false);
	}

	private isInFadeOut(lifeRemaining: number): boolean {
		return lifeRemaining < this.fadeOutTime;
	}

	// TODO: consolidate duplication with similar function in AnimatedElement.
	getLifeRemaining(now: number) {
		let lifeRemaining: number = 0;
		if (this.expirationDate) {
			lifeRemaining = this.expirationDate - now;
		}
		return lifeRemaining;
	}

	getVolumeMultiplier(now: number): number {
		let msAlive: number = now - this.timeStart;

		if (msAlive < this.fadeInTime) {
			this.state = SongState.fadingIn;
			return msAlive / this.fadeInTime;
		}

		if (this.expirationDate) {
			let lifeRemaining: number = this.getLifeRemaining(now);
			if (lifeRemaining < 0) {
				this.state = SongState.finishedPlaying;
				return 0;
			}
			if (this.isInFadeOut(lifeRemaining)) {
				this.state = SongState.fadingOut;
				return lifeRemaining / this.fadeOutTime;
			}
		}

		this.state = SongState.playingNormal;
		return 1;
	}

	update(now: number): void {
		let newFadeVolumeMultiplier: number = Math.round(this.getVolumeMultiplier(now) * 10) / 10;
		if (newFadeVolumeMultiplier != this.fadeVolumeMultiplier) {
			this.fadeVolumeMultiplier = newFadeVolumeMultiplier;
			this.setVolumeForActiveSong();
		}

		if (MusicPlayer.suppressingVolume && now > MusicPlayer.suppressingVolumeEnds) {
			this.restoreVolume();
		}
	}

	static suppressVolume(seconds: number, now: number): void {
		MusicPlayer.suppressingVolume = true;

		if (MusicPlayer.volume != MusicPlayer.suppressVolumeLevel) {
			MusicPlayer.saveVolume = MusicPlayer.volume;
			MusicPlayer.volume = MusicPlayer.suppressVolumeLevel;
			this.setVolumeForInstance(MusicPlayer.thisMusicPlayer);
			this.setVolumeForInstance(MusicPlayer.nextMusicPlayer);
			MusicPlayer.suppressingVolumeEnds = now + seconds * 1000;
			console.log(`Suppressing music volume for ${seconds} seconds.`);
		}
	}

	private restoreVolume() {
		MusicPlayer.suppressingVolume = false;

		this.restorePlayerVolume(MusicPlayer.thisMusicPlayer);
		this.restorePlayerVolume(MusicPlayer.nextMusicPlayer);
	}

	private restorePlayerVolume(musicPlayer: MusicPlayer) {
		if (musicPlayer != null) {
			MusicPlayer.volume = MusicPlayer.saveVolume;
			musicPlayer.setVolumeForActiveSong();
		}
	}

	destroying(): void {
		if (this.activeSong)
			this.activeSong.pause();
		this.activeSong = null;
	}

	fadeVolumeMultiplier: number = 1;

	getFileName(index: number): string {
		return `${MusicPlayer.activeGenre}/song (${index.toString()})`;
	}
}

MusicPlayer.Initialize();