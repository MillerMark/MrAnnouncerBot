class KeyTime {
	key: string;
	time: number;

	constructor(key: string, time: number) {
		this.key = key;
		this.time = time;
	}
}

class SoundManager {
	lastPlayTimes: Array<KeyTime> = new Array<KeyTime>();

	constructor(public soundPath: string) {

	}

	// Gets an audio file from a list of audio files based on a specified percentage.
	// So if I have 5 files, each in increasing intensity, then this will pick the 
	// file that approximates the percentage passed in. So 0% will match index 0, and
	// 100% will match the last item in the array.
	getPercentageAudio(elements: HTMLAudioElement[], percentage: number): HTMLAudioElement {
		let index: number = Math.floor(Math.max(Math.min(percentage, 1) * (elements.length - 1), 0));
		return elements[index];
	}

	// Loads an array of sounds numbered 1..n, 
	loadPercentageSoundEffects(sounds: Array<HTMLAudioElement>, baseFileName: string, numSounds: number): void {
		for (var i = 0; i < numSounds; i++) {
			let fileIndex: number = i + 1;
			sounds.push(new Audio(`${this.soundPath}/${baseFileName}${fileIndex}.wav`))
		}
	}

	safePlayMp3ReturnAudio(fileName: string, compareThreshold: number = -1): HTMLAudioElement {
		let baseFileName: string = fileName;
		let bracketIndex: number = fileName.indexOf('[');
		if (bracketIndex >= 0) {
			let countStr: string = fileName.substr(bracketIndex + 1);
			let closingBracketIndex: number = countStr.indexOf(']');
			if (closingBracketIndex > 0) {
				countStr = countStr.substr(0, closingBracketIndex);
			}
			baseFileName = fileName.substr(0, bracketIndex);

			let count: number = +countStr;
			if (count > 0) {
				let index: number = Math.floor(Math.random() * count + 1);
				fileName = baseFileName + index.toString();
			}
		}

		if (this.playedRecently(baseFileName, compareThreshold))
			return null;

		let media: HTMLAudioElement = new Audio(`${this.soundPath}/${fileName}.mp3`);
		const playPromise = media.play();
		if (playPromise !== null) {
			playPromise.catch(() => { })
		}
		this.playingNow(baseFileName);
		return media;
	}

	safePlayMp3(fileName: string, compareThreshold: number = -1): boolean {
		let audio: HTMLAudioElement = this.safePlayMp3ReturnAudio(fileName, compareThreshold);
		if (audio)
			return true;
		else
			return false;
	}

	static readonly threshold: number = 50; // ms

	playedRecently(key: string, compareThreshold: number = -1) {
		if (compareThreshold == -1)
			compareThreshold = SoundManager.threshold;
		for (var i = 0; i < this.lastPlayTimes.length; i++) {
			let keyTime: KeyTime = <KeyTime>this.lastPlayTimes[i];
			if (keyTime.key === key) {
				var now: number = performance.now();
				return now - keyTime.time < compareThreshold;
			}
		}
		return false;
	}

	playingNow(key: string) {
		var now: number = performance.now();
		for (var i = 0; i < this.lastPlayTimes.length; i++) {
			let keyTime: KeyTime = <KeyTime>this.lastPlayTimes[i];
			if (keyTime.key === key) {
				keyTime.time = now;
				return;
			}
		}

		this.lastPlayTimes.push(new KeyTime(key, now));
	}

	play(audio: HTMLAudioElement): void {
		const playPromise = audio.play();
		if (playPromise !== null) {
			playPromise.catch(() => { })
		}
	}

	playRandom(fileName: string, count: number, compareThreshold: number = -1) {
		return this.safePlayMp3(`${fileName}[${count}]`, compareThreshold);
	}

	playMp3In(milliseconds: number, mp3Name: string): any {
		setTimeout(function () { this.safePlayMp3(mp3Name); }.bind(this), milliseconds);
	}
}