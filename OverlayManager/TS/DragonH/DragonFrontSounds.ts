class DragonFrontSounds extends SoundManager {
  constructor(soundPath: string) {
    super(soundPath);
  }

  playFlameOn() {
    this.safePlayMp3('FlameOn');
  }

  //playGear0_5() {
  //  this.safePlayMp3('Gear0_5');
  //}

  //playGear0_2() {
  //  this.safePlayMp3('Gear0_2');
  //}

  //playGear0_25() {
  //  this.safePlayMp3('Gear0_25');
  //}

  //playGear1_0() {
  //  this.safePlayMp3('Gear1_0');
  //}
  //playGear1_8() {
  //  this.safePlayMp3('Gear1_8');
  //}

  //playGear2_6() {
  //  this.safePlayMp3('Gear2_6');
  //}

  //playHeavyPoof() {
  //  this.safePlayMp3('HeavyPoof');
  //}
}