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

  safePlayMP3(fileName: string) {
    if (this.playedRecently(fileName))
      return;
    let media = new Audio(`${this.soundPath}/${fileName}.mp3`);
    const playPromise = media.play();
    if (playPromise !== null) {
      playPromise.catch(() => { })
    }
    this.playingNow(fileName);
  }

  static readonly threshold: number = 50; // ms

  playedRecently(key: string) {
    for (var i = 0; i < this.lastPlayTimes.length; i++) {
      let keyTime: KeyTime = <KeyTime>this.lastPlayTimes[i];
      if (keyTime.key === key) {
        var now: number = performance.now();
        return now - keyTime.time < SoundManager.threshold;
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
}