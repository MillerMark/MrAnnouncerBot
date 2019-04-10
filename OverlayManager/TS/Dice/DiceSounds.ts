class DiceSounds {
  hitDice: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  hitFloor: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  hitWall: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  settles: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  lastSettleTime: Date;

  constructor() {
    this.loadSoundEffects(this.hitDice, "Die", 6);
    this.loadSoundEffects(this.hitFloor, "Floor", 4);
    this.loadSoundEffects(this.hitWall, "Wall", 5);
    this.loadSoundEffects(this.settles, "Settles", 7);
  }

  loadSoundEffects(sounds: Array<HTMLAudioElement>, baseFileName: string, numSounds: number): void {
    for (var i = 0; i < numSounds; i++) {
      let fileIndex: number = i + 1;
      sounds.push(new Audio(`GameDev/Assets/DragonH/SoundEffects/Dice/${baseFileName}${fileIndex}.wav`))
    }
  }

  getAudio(name: string, elements: HTMLAudioElement[], percentage: number): any {
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
    try {
      audio.play();
    }
    catch (ex) {
      console.log('Exception on play: ' + ex);
    }
  }
}