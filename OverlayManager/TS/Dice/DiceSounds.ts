class KeyTime {
  key: string;
  time: number;

  constructor(key: string, time: number) {
    this.key = key;
    this.time = time;
  }
}

class DiceSounds {
  hitDice: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  hitFloor: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  hitWall: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  settles: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  lastPlayTimes: Array<KeyTime>=  new Array<KeyTime>();
  
  lastSettleTime: Date;

  constructor() {
    this.loadSoundEffects(this.hitDice, "Die", 6);
    this.loadSoundEffects(this.hitFloor, "Floor", 4);
    this.loadSoundEffects(this.hitWall, "Wall", 5);
    this.loadSoundEffects(this.settles, "Settles", 7);
  }

  playOpenDiePortal() {
    //new Audio('GameDev/Assets/DragonH/SoundEffects/DiePortal.mp3').play();
    this.safePlay('DiePortal');
  }

  playSteampunkTunnel() {
    //new Audio('GameDev/Assets/DragonH/SoundEffects/SteampunkTunnel.mp3').play();
    this.safePlay('SteampunkTunnel');
  }

  playFireball() {
    //new Audio('GameDev/Assets/DragonH/SoundEffects/Fireball2.mp3').play();
    this.safePlay('Fireball2');
  }

  playDiceBlow() {
    this.safePlay('DiceBlow');
  }

  safePlay(fileName: string) {
    if (this.playedRecently(fileName))
      return;
    let media = new Audio(`GameDev/Assets/DragonH/SoundEffects/${fileName}.mp3`);
    const playPromise = media.play();	
    if (playPromise !== null) {
      playPromise.catch(() => {  })
    }
    this.playingNow(fileName);
  }

  static readonly threshold: number = 50; // ms

  playedRecently(key: string) {
    for (var i = 0; i < this.lastPlayTimes.length; i++)
    {
      let keyTime: KeyTime = <KeyTime>this.lastPlayTimes[i];
      if (keyTime.key === key)
      {
        var now: number = performance.now();
        return now - keyTime.time < DiceSounds.threshold;
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

  playDieBomb() {
    const numDieBombSounds: number = 2;
    var index: number = Math.floor(Math.random() * numDieBombSounds) + 1;
    //new Audio(`GameDev/Assets/DragonH/SoundEffects/DieBomb${index}.mp3`).play();
    this.safePlay(`DieBomb${index}`);
  }

  loadSoundEffects(sounds: Array<HTMLAudioElement>, baseFileName: string, numSounds: number): void {
    for (var i = 0; i < numSounds; i++) {
      let fileIndex: number = i + 1;
      sounds.push(new Audio(`GameDev/Assets/DragonH/SoundEffects/Dice/${baseFileName}${fileIndex}.wav`))
    }
  }

  getAudio(name: string, elements: HTMLAudioElement[], percentage: number): HTMLAudioElement {
    let index: number = Math.floor(Math.max(Math.min(percentage, 1) * (elements.length - 1), 0));
    //console.log(name + (index + 1).toString());
    return elements[index];
  }

  playWallHit(percentage: number): void {
    this.play(this.getAudio('wall', this.hitWall, percentage));
  }

  playFloorHit(percentage: number): void {
    this.play(this.getAudio('floor', this.hitFloor, percentage));
  }

  playSettle(): any {
    if (this.lastSettleTime) {
      var now: Date = new Date();
      if (now.valueOf() - this.lastSettleTime.valueOf() < 500)
        return;
    }

    this.play(this.getAudio('settle', this.settles, Math.random()));
    this.lastSettleTime = new Date();
  }

  playDiceHit(percentage: number): any {
    this.play(this.getAudio('die', this.hitDice, percentage));
  }

  play(audio: HTMLAudioElement): void {
    const playPromise = audio.play();
    if (playPromise !== null) {
      playPromise.catch(() => { })
    }
  }
}