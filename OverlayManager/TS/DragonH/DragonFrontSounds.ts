class DragonFrontSounds extends SoundManager {
  constructor(soundPath: string) {
    super(soundPath);
  }

  playFlameOn() {
    this.safePlay('FlameOn');
  }

  playGear0_5() {
    this.safePlay('Gear0_5');
  }

  playGear0_2() {
    this.safePlay('Gear0_2');
  }

  playGear0_25() {
    this.safePlay('Gear0_25');
  }

  playGear1_0() {
    this.safePlay('Gear1_0');
  }
  playGear1_8() {
    this.safePlay('Gear1_8');
  }
  playGear2_6() {
    this.safePlay('Gear2_6');
  }
}