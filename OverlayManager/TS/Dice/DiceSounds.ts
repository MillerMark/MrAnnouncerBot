class DiceSounds extends SoundManager {
  hitDice: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  hitFloor: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  hitWall: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  settles: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
  
  lastSettleTime: Date;

  constructor(soundPath: string) {
    super(soundPath);
    this.loadPercentageSoundEffects(this.hitDice, "Dice/Die", 6);
    this.loadPercentageSoundEffects(this.hitFloor, "Dice/Floor", 4);
    this.loadPercentageSoundEffects(this.hitWall, "Dice/Wall", 5);
    this.loadPercentageSoundEffects(this.settles, "Dice/Settles", 7);
  }

  playOpenDiePortal() {
    this.safePlayMP3('DiePortal');
  }

  playSteampunkTunnel() {
    this.safePlayMP3('SteampunkTunnel');
  }

  playFireball() {
    this.safePlayMP3('Fireball2');
  }

  playDiceBlow() {
    this.safePlayMP3('DiceBlow');
  }

  playDieBomb() {
    const numDieBombSounds: number = 2;
    var index: number = Math.floor(Math.random() * numDieBombSounds) + 1;
    this.safePlayMP3(`DieBomb${index}`);
  }

  playWallHit(percentage: number): void {
    this.play(this.getPercentageAudio(this.hitWall, percentage));
  }

  playFloorHit(percentage: number): void {
    this.play(this.getPercentageAudio(this.hitFloor, percentage));
  }

  playSettle(): any {
    if (this.lastSettleTime) {
      var now: Date = new Date();
      if (now.valueOf() - this.lastSettleTime.valueOf() < 500)
        return;
    }

    this.play(this.getPercentageAudio(this.settles, Math.random()));
    this.lastSettleTime = new Date();
  }

  playDiceHit(percentage: number): any {
    this.play(this.getPercentageAudio(this.hitDice, percentage));
  }
}