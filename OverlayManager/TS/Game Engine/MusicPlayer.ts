//! Changes here should be kept in sync with SoundCommandType in SoundCommandType.cs
enum SoundCommandType {
	VolumeUp,
	VolumeDown,
	SetVolume,
	SuppressVolume,
	ChangeFolder,
	StopPlaying
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
	constructor(public type: SoundCommandType, public numericData: number, public strData: string, public mainFolder: string) {

	}
}

class SoundFolder {
	constructor(public folderName: string, public count: number) {

	}
}

class SimpleMusicPlayer {
	state: SongState;
	timeStart: number;
	expirationDate: number;
	soundManager: SoundManager = new SoundManager('GameDev/Assets/DragonH/SoundEffects');
	activeSong: HTMLAudioElement;

	constructor() {
		this.timeStart = performance.now();
		const minSecondsToPlay: number = 10;
		this.expirationDate = this.timeStart + minSecondsToPlay * 1000 + CrossfadePlayer.crossFadeTime;
	}



	pauseActiveSong() {
		if (this.activeSong)
			this.activeSong.pause();
	}

	setVolumeForActiveSong(volume: number) {
		let actualVolume: number;
		let thisVolume: number = Math.round(volume);
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

	playRandomSong(mainFolder: string, activeFolder: string, activeFileCount: number, volume: number) {
		const oneMinuteMs: number = 60 * 1000;
		const tenMinutesMS: number = 10 * oneMinuteMs;
		let index: number = Random.intBetween(1, activeFileCount);
		let tries = 0;
		const maxTries = 30;
		let fileName: string = this.getFileName(mainFolder, activeFolder, index);
		while (tries < maxTries && this.soundManager.playedRecently(fileName, tenMinutesMS)) {
			index = Random.intBetween(1, activeFileCount);
			fileName = this.getFileName(mainFolder, activeFolder, index);
			tries++;
		}

		if (this.activeSong) {
			this.activeSong.pause();
		}

		//console.log(`Playing "${fileName}.mp3"...`);
		this.activeSong = this.soundManager.safePlayMp3ReturnAudio(fileName, tenMinutesMS);
		if (!this.activeSong)
			this.activeSong = this.soundManager.safePlayMp3ReturnAudio(fileName, 0);

		this.setVolumeForActiveSong(volume);

		this.activeSong.addEventListener('loadedmetadata', function () {
			//console.log(`${fileName} duration: ` + this.activeSong.duration);
			const durationSec: number = this.activeSong.duration * 1000;
			this.expirationDate = this.timeStart + durationSec;
		}.bind(this), false);
	}

	private isInFadeOut(lifeRemaining: number): boolean {
		return lifeRemaining < CrossfadePlayer.crossFadeTime;
	}

	// TODO: consolidate duplication with similar function in AnimatedElement.
	getLifeRemaining(now: number) {
		let lifeRemaining = 0;
		if (this.expirationDate) {
			lifeRemaining = this.expirationDate - now;
		}
		return lifeRemaining;
	}

	getVolumeMultiplier(now: number): number {
		const msAlive: number = now - this.timeStart;

		if (msAlive < CrossfadePlayer.crossFadeTime) {
			this.state = SongState.fadingIn;
			return msAlive / CrossfadePlayer.crossFadeTime;
		}

		if (this.expirationDate) {
			const lifeRemaining: number = this.getLifeRemaining(now);
			if (lifeRemaining < 0) {
				this.state = SongState.finishedPlaying;
				return 0;
			}
			if (this.isInFadeOut(lifeRemaining)) {
				this.state = SongState.fadingOut;
				return lifeRemaining / CrossfadePlayer.crossFadeTime;
			}
		}

		this.state = SongState.playingNormal;
		return 1;
	}

	update(now: number, volume: number): void {
		let newFadeVolumeMultiplier: number = Math.round(this.getVolumeMultiplier(now) * 10) / 10;
		if (newFadeVolumeMultiplier != this.fadeVolumeMultiplier) {
			this.fadeVolumeMultiplier = newFadeVolumeMultiplier;
			this.setVolumeForActiveSong(volume);
		}
	}

	destroying(): void {
		if (this.activeSong)
			this.activeSong.pause();
		this.activeSong = null;
	}

	fadeVolumeMultiplier: number = 1;

	getFileName(mainFolder: string, activeFolder: string, index: number): string {
		return `${mainFolder}/${activeFolder}/song (${index.toString()})`;
	}
}

// This class can smoothly crossfade sound files from a known folder.
class CrossfadePlayer {
	static readonly crossFadeTime: number = 4000;
	static readonly suppressVolumeLevel: number = 0.5;

	soundFolders: Array<SoundFolder> = [];
	activeFolder: string = '';
	activeFileCount: number;
	volume: number = 4;
	stopping: boolean;
	saveVolume: number = 0;
	nextMusicPlayer: SimpleMusicPlayer;
	thisMusicPlayer: SimpleMusicPlayer;
	mainFolder: string;

	suppressingVolume: boolean = false;
	suppressingVolumeEnds: number;


	constructor() {

	}

	changeFolder(newFolder: string): void {
		this.stopping = false;
		this.changeSubfolder(newFolder);
		// Cross-fade songs now. That means create a second MusicPlayer and expire the current song.
		if (this.nextMusicPlayer) {
			this.nextMusicPlayer.pauseActiveSong();
		}

		if (this.thisMusicPlayer) {
			this.thisMusicPlayer.expirationDate = performance.now() + CrossfadePlayer.crossFadeTime;

			this.nextMusicPlayer = new SimpleMusicPlayer();
			this.nextMusicPlayer.playRandomSong(this.mainFolder, this.activeFolder, this.activeFileCount, this.volume);
		}
		else {
			this.thisMusicPlayer = this.nextMusicPlayer;
			if (!this.thisMusicPlayer)
				this.thisMusicPlayer = new SimpleMusicPlayer();

			this.thisMusicPlayer.playRandomSong(this.mainFolder, this.activeFolder, this.activeFileCount, this.volume);
			this.nextMusicPlayer = null;
		}
	}

	trimVolume() {
		this.volume = Math.round(this.volume);
		if (this.volume > 11)
			this.volume = 11;
		if (this.volume < 1)
			this.volume = 1;
	}

	volumeDown(): void {
		this.setVolumeTo(this.volume - 1);
	}


	volumeUp(): void {
		this.setVolumeTo(this.volume + 1);
	}

	setVolumeTo(volStr: number | string): void {
		let newVolume: number = +volStr;
		if (!newVolume)
			return;

		this.volume = newVolume;
		this.actOnNewVolume();
	}

	actOnNewVolume(): void {
		this.trimVolume();
		this.setVolumeForInstance(this.nextMusicPlayer);
		this.setVolumeForInstance(this.thisMusicPlayer);
		this.reportVolume();
	}

	reportVolume(): any {
		tellDM(`Volume for ${this.mainFolder} player is: ` + this.volume);
	}

	addFolder(folderName: string, fileCount: number) {
		this.soundFolders.push(new SoundFolder(folderName, fileCount));
	}

	stopPlaying(): void {
		this.stopping = true;
		if (this.thisMusicPlayer) {
			this.thisMusicPlayer.expirationDate = performance.now() + CrossfadePlayer.crossFadeTime;
		}

		if (this.nextMusicPlayer) {
			this.nextMusicPlayer.expirationDate = performance.now() + CrossfadePlayer.crossFadeTime;
		}
	}

	private restoreVolume() {
		this.suppressingVolume = false;

		this.restorePlayerVolume(this.thisMusicPlayer);
		this.restorePlayerVolume(this.nextMusicPlayer);
	}

	private restorePlayerVolume(musicPlayer: SimpleMusicPlayer) {
		if (musicPlayer !== null) {
			this.volume = this.saveVolume;
			musicPlayer.setVolumeForActiveSong(this.volume);
		}
	}

	updateMusicPlayers(now: number) {
		if (this.suppressingVolume && now > this.suppressingVolumeEnds) {
			this.restoreVolume();
		}

		const thisPlayer: SimpleMusicPlayer = this.thisMusicPlayer;
		if (thisPlayer) {
			thisPlayer.update(now, this.volume);

			if (thisPlayer.state === SongState.playingNormal && this.nextMusicPlayer) {
				this.nextMusicPlayer.pauseActiveSong();
				this.nextMusicPlayer = null;  // next player is null as long as this song is normally playing.
			}
			if (thisPlayer.state === SongState.fadingOut && this.nextMusicPlayer === null && !this.stopping) {
				this.nextMusicPlayer = new SimpleMusicPlayer();
				this.nextMusicPlayer.playRandomSong(this.mainFolder, this.activeFolder, this.activeFileCount, this.volume);
			}
			if (thisPlayer.state === SongState.finishedPlaying) {
				this.thisMusicPlayer = this.nextMusicPlayer;
				if ((!this.thisMusicPlayer || this.thisMusicPlayer.state === SongState.finishedPlaying) && !this.stopping)
					this.thisMusicPlayer = new SimpleMusicPlayer();
				this.nextMusicPlayer = null;
			}
		}
		if (this.nextMusicPlayer) {
			this.nextMusicPlayer.update(now, this.volume);
			if (this.nextMusicPlayer.state !== SongState.fadingIn) {
				if (thisPlayer)
					thisPlayer.pauseActiveSong();
				this.thisMusicPlayer = this.nextMusicPlayer;
				this.nextMusicPlayer = null;
			}
		}
	}

	private changeSubfolder(newFolder: string): void {
		this.soundFolders.forEach(function (folder: SoundFolder) {
			if (folder.folderName.toLowerCase() === newFolder.toLowerCase()) {
				this.activeFolder = folder.folderName;
				this.activeFileCount = folder.count;
				tellDM(`Switching ${this.mainFolder} to ${this.activeFolder}...`);
				if (!this.thisMusicPlayer && !this.nextMusicPlayer) {
					this.thisMusicPlayer = new SimpleMusicPlayer();
					this.thisMusicPlayer.playRandomSong(this.mainFolder, this.activeFolder, this.activeFileCount, this.volume);
				}
				return;
			}
		}, this);
	}

	private setVolumeForInstance(player: SimpleMusicPlayer) {
		if (player)
			player.setVolumeForActiveSong(this.volume);
	}

	suppressVolume(seconds: number, now: number): void {
		this.suppressingVolume = true;

		if (this.volume != CrossfadePlayer.suppressVolumeLevel) {
			this.saveVolume = this.volume;
			this.volume = CrossfadePlayer.suppressVolumeLevel;
			this.setVolumeForInstance(this.thisMusicPlayer);
			this.setVolumeForInstance(this.nextMusicPlayer);
			this.suppressingVolumeEnds = now + seconds * 1000;
			console.log(`Suppressing ${this.mainFolder} volume for ${seconds} seconds.`);
		}
	}

	static allCrossfadePlayers: CrossfadePlayer[] = new Array<CrossfadePlayer>();

	static InitializeThemeMusicPlayer(): void {
		let themeMusicPlayer: CrossfadePlayer = new CrossfadePlayer();
		themeMusicPlayer.mainFolder = 'Music';
		themeMusicPlayer.addFolder('Bittersweet', 27);
		themeMusicPlayer.addFolder('Battle', 46);
		themeMusicPlayer.addFolder('Travel', 35);
		themeMusicPlayer.addFolder('Suspense', 62);
		themeMusicPlayer.addFolder('Inspirational', 66);
		themeMusicPlayer.addFolder('Ethereal', 47);
		themeMusicPlayer.addFolder('Foreboding', 38);
		themeMusicPlayer.addFolder('Happy', 39);
		themeMusicPlayer.addFolder('Funk', 105);
		CrossfadePlayer.allCrossfadePlayers.push(themeMusicPlayer);
	}
	static InitializeWeatherPlayer(): void {
		let weatherPlayer: CrossfadePlayer = new CrossfadePlayer();
		weatherPlayer.mainFolder = 'Weather';
		weatherPlayer.addFolder('rain', 6);
		weatherPlayer.addFolder('wind', 6);
		weatherPlayer.addFolder('rainWind', 2);
		weatherPlayer.addFolder('sunny', 3);
		weatherPlayer.addFolder('hurricane', 4);
		weatherPlayer.addFolder('thunder', 3);
		//weatherPlayer.addFolder('thunder', 2);
		//weatherPlayer.addFolder('hurricane', 2);
		CrossfadePlayer.allCrossfadePlayers.push(weatherPlayer);
	}

	static InitializeCrossFadePlayers(): void {
		CrossfadePlayer.InitializeThemeMusicPlayer();
		CrossfadePlayer.InitializeWeatherPlayer();
	}

	static getPlayer(mainFolder: string): CrossfadePlayer {
		let result: CrossfadePlayer = null;
		for (let i = 0; i < CrossfadePlayer.allCrossfadePlayers.length; i++) {
			let player: CrossfadePlayer = CrossfadePlayer.allCrossfadePlayers[i];
			if (player.mainFolder === mainFolder) {
				result = player;
				break;
			}
		}

		return result;
	}


	static changePlayerFolder(mainFolder: string, subFolder: string): any {
		let player: CrossfadePlayer = CrossfadePlayer.getPlayer(mainFolder);
		if (player)
			player.changeFolder(subFolder);
	}

	static setVolumeTo(mainFolder: string, newVolume: number) {
		let player: CrossfadePlayer = CrossfadePlayer.getPlayer(mainFolder);
		if (player)
			player.setVolumeTo(newVolume);
	}

	static stopPlayer(mainFolder: string) {
		let player: CrossfadePlayer = CrossfadePlayer.getPlayer(mainFolder);
		if (player)
			player.stopPlaying();
	}

	static updateMusicPlayers(now: number): any {
		for (let i = 0; i < CrossfadePlayer.allCrossfadePlayers.length; i++) {
			CrossfadePlayer.allCrossfadePlayers[i].updateMusicPlayers(now)
		}
	}
}

CrossfadePlayer.InitializeCrossFadePlayers();
