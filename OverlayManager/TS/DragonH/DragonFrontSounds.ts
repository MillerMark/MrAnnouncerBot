class DragonFrontSounds extends SoundManager {
  constructor(soundPath: string) {
    super(soundPath);
  }

  playFlameOn() {
    this.safePlayMP3('FlameOn');
  }

  playGear0_5() {
    this.safePlayMP3('Gear0_5');
  }

  playGear0_2() {
    this.safePlayMP3('Gear0_2');
  }

  playGear0_25() {
    this.safePlayMP3('Gear0_25');
  }

  playGear1_0() {
    this.safePlayMP3('Gear1_0');
  }
  playGear1_8() {
    this.safePlayMP3('Gear1_8');
  }

  playGear2_6() {
    this.safePlayMP3('Gear2_6');
  }

  playHeavyPoof() {
    this.safePlayMP3('HeavyPoof');
  }
}