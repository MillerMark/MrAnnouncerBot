class CharacterStatsScroll extends WorldObject {
  static readonly centerY: number = 414;
  static readonly centerX: number = 174;

  scrollRolls: Sprites;
  scrollSlam: Sprites;
  scrollBacks: Sprites;
  openStart: number;
  static readonly scrollOpenOffsets: number[] = [47, 48, 75, 106, 143, 215, 243, 267, 292, 315,  // 0-9
    333, 347, 360, 367, 380, 385, 395, 411, 415, 420,  // 10-19
    424, 428, 432, 432]; // One extra...

  private readonly framerateMs: number = 33; // 33 milliseconds == 30 fps
  topEmitter: Emitter;
  bottomEmitter: Emitter;

  constructor() {
    super();
    this.buildGoldDust();
  }

  slam(): void {
    this.scrollSlam.add(0, 0, 0);
  }

  open(now: number): void {
    this.openStart = now;
    this.scrollRolls.sprites = [];
    this.scrollBacks.sprites = [];
    this.scrollRolls.add(0, 0, 0);
    this.scrollBacks.add(0, 0, 0);
    this.topEmitter.start();
    this.bottomEmitter.start();
  }

  preUpdate(now: number, timeScale: number, world: World) {
    super.preUpdate(now, timeScale, world);
    this.topEmitter.preUpdate(now, timeScale, world);
    this.bottomEmitter.preUpdate(now, timeScale, world);
  }

  update(now: number, timeScale: number, world: World) {
    super.update(now, timeScale, world);
    this.topEmitter.update(now, timeScale, world);
    this.bottomEmitter.update(now, timeScale, world);
  }

  render(now: number, timeScale: number, world: World) {
    super.render(now, timeScale, world);

    if (!this.openStart || this.scrollRolls.sprites.length == 0) {
      return;
    }

    let elapsedTime: number = now - this.scrollRolls.lastTimeWeAdvancedTheFrame / 1000;
    let frameIndex: number = this.scrollRolls.sprites[0].frameIndex;
    let frameFraction: number = elapsedTime / (this.framerateMs / 1000);
    const maxFrameFraction: number = 0.99999999;
    if (frameFraction > maxFrameFraction) {
      frameFraction = maxFrameFraction;
    }

    let frameIndexPrecise: number = frameIndex + frameFraction;

    //let frameIndexPrecise: number = frameIndex;
    //console.log('frameIndex: ' + frameIndex);

    // ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)

    if (frameIndex < 22) {
      let offset: number = CharacterStatsScroll.scrollOpenOffsets[frameIndex];

      let decimalOffset: number = frameIndexPrecise - frameIndex;
      let distanceBetweenOffsets: number = CharacterStatsScroll.scrollOpenOffsets[frameIndex + 1] - offset;
      let superPreciseOffset: number = offset + distanceBetweenOffsets * decimalOffset;
      //console.log('offset: ' + offset);
      let baseAnim: Part = this.scrollBacks.baseAnimation;
      let sw = this.scrollBacks.spriteWidth;
      let dx: number = 0;
      let dh: number = superPreciseOffset * 2;
      let dy = CharacterStatsScroll.centerY - superPreciseOffset;
      let sh = dh;
      baseAnim.drawCroppedByIndex(world.ctx, dx, dy, baseAnim.frameIndex, dx, dy, sw, sh, sw, dh);

      if (frameIndex == 0) {
        this.topEmitter.start();
        this.bottomEmitter.start();
      }

      this.topEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY - superPreciseOffset);
      this.bottomEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY + superPreciseOffset);
    }
    else {
      this.scrollBacks.draw(world.ctx, now * 1000);
      this.topEmitter.stop();
      this.bottomEmitter.stop();
    }

    this.topEmitter.render(now, timeScale, world);
    this.bottomEmitter.render(now, timeScale, world);

    this.scrollRolls.draw(world.ctx, now * 1000);

    this.scrollSlam.draw(world.ctx, now * 1000);
  }

  buildGoldDust(): any {
    let windSpeed: number = 0.8;

    this.topEmitter = this.getMagicDustEmitter(windSpeed);
    this.bottomEmitter = this.getMagicDustEmitter(-windSpeed);

    this.topEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY - CharacterStatsScroll.scrollOpenOffsets[0]);
    this.bottomEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY + CharacterStatsScroll.scrollOpenOffsets[0]);
  }

  private getMagicDustEmitter(windSpeed: number): Emitter {
    const scrollLength: number = 348;
    let emitter: Emitter = new Emitter(new Vector(scrollLength / 2, 460));
    emitter.stop();
    emitter.setRectShape(scrollLength, 1);
    emitter.saturation.target = 0.9;
    emitter.saturation.relativeVariance = 0.2;
    emitter.hue = new TargetValue(40, 0, 0, 60);
    emitter.hue.absoluteVariance = 10;
    emitter.brightness.target = 0.7;
    emitter.brightness.relativeVariance = 0.5;
    emitter.particlesPerSecond = 2500;
    emitter.particleRadius.target = 0.8;
    emitter.particleRadius.relativeVariance = 2;
    emitter.particleLifeSpanSeconds = 1.5;
    emitter.particleGravity = 0;
    emitter.particleInitialVelocity.target = 0;
    emitter.particleInitialVelocity.relativeVariance = 0.5;
    emitter.gravity = 0;
    emitter.airDensity = 0.2;
    emitter.particleAirDensity = 1;
    emitter.particleWind = new Vector(0, windSpeed);

    return emitter;
  }

  loadResources(): void {
    var assetFolderName: string = Folders.assets;
    Folders.assets = 'GameDev/Assets/DragonH/';

    this.scrollRolls = new Sprites("Scroll/Open/ScrollOpen", 23, this.framerateMs, AnimationStyle.SequentialStop, true);
    this.scrollSlam = new Sprites("Scroll/Slam/Slam", 8, this.framerateMs, AnimationStyle.Sequential, true);
    this.scrollBacks = new Sprites("Scroll/Backs/Back", 2, this.framerateMs, AnimationStyle.Static);

    Folders.assets = assetFolderName;
  }
}