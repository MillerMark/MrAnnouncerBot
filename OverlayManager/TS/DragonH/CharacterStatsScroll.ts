/* 

('', x, y, fontSize, TextAlign.Left);
('', x, y, fontSize, TextAlign.Center, TextDisplay.PlusMinus);


const attributeTop: number = 203;
const attributeLeft: number = 44;
const attributeDistanceY: number = 91;
const modOffset: number = 32;
const raceClassAlignmentX: number = 135;
const levelInspXpY: number = 74;
const nameFontSize: number = 10.5;
const tempHpFontSize: number = 10.5;
const hitDiceFontSize: number = 11;
const bigNumberFontSize: number = 16;
const raceClassAlignmentFontSize: number = 9;
const levelInspXpFontSize: number = 14;
const acInitSpeedY: number = 187;
const hpTempHpX: number = 138;
const deathSave1X: number = 228;
const deathSave2X: number = 255;
const deathSave3X: number = 280;
const deathSaveLifeY: number = 253;
const deathSaveDeathY: number = 284;


('name', 68, 127, nameFontSize);
('level', 157, levelInspXpY, levelInspXpFontSize);
('inspiration', 224, levelInspXpY, levelInspXpFontSize);
('experience', 290, levelInspXpY, levelInspXpFontSize);
('raceClass', raceClassAlignmentX, 36, raceClassAlignmentFontSize, TextAlign.Left);
('alignment', raceClassAlignmentX, 129, raceClassAlignmentFontSize, TextAlign.Left);
('strength', attributeLeft, attributeTop, bigNumberFontSize);
('strengthMod', attributeLeft, attributeTop + modOffset, modFontSize, TextAlign.Center, TextDisplay.PlusMinus);
('dexterity', attributeLeft, attributeTop + attributeDistanceY, bigNumberFontSize);
('dexterityMod', attributeLeft, attributeTop + attributeDistanceY + modOffset, modFontSize, TextAlign.Center, TextDisplay.PlusMinus);
('constitution', attributeLeft, attributeTop + 2 * attributeDistanceY, bigNumberFontSize);
('constitutionMod', attributeLeft, attributeTop + 2 * attributeDistanceY + modOffset, modFontSize, TextAlign.Center, TextDisplay.PlusMinus);
('intelligence', attributeLeft, attributeTop + 3 * attributeDistanceY, bigNumberFontSize);
('intelligenceMod', attributeLeft, attributeTop + 3 * attributeDistanceY + modOffset, modFontSize, TextAlign.Center, TextDisplay.PlusMinus);
('wisdom', attributeLeft, attributeTop + 4 * attributeDistanceY, bigNumberFontSize);
('wisdomMod', attributeLeft, attributeTop + 4 * attributeDistanceY + modOffset, modFontSize, TextAlign.Center, TextDisplay.PlusMinus);
('charisma', attributeLeft, attributeTop + 5 * attributeDistanceY, bigNumberFontSize);
('charismaMod', attributeLeft, attributeTop + 5 * attributeDistanceY + modOffset, modFontSize, TextAlign.Center, TextDisplay.PlusMinus);

('armorClass', 135, acInitSpeedY, bigNumberFontSize);
('initiative', 203, acInitSpeedY, bigNumberFontSize);
('speed', 272, acInitSpeedY, bigNumberFontSize);
('hitPoints', hpTempHpX, 251, bigNumberFontSize);
('tempHitPoints', hpTempHpX, 299, tempHpFontSize);
('hitDice', 183, 349, hitDiceFontSize);


const deathSave1X: number = 228;
const deathSave2X: number = 255;
const deathSave3X: number = 280;
const deathSaveLifeY: number = 253;
const deathSaveDeathY: number = 284;
const deathSaveRadius: number = 10;
// booleans...
('deathSaveLife1', deathSave1X, deathSaveLifeY, deathSaveRadius);
('deathSaveLife2', deathSave2X, deathSaveLifeY, deathSaveRadius);
('deathSaveLife3', deathSave3X, deathSaveLifeY, deathSaveRadius);

('deathSaveDeath1', deathSave1X, deathSaveDeathY, deathSaveRadius);
('deathSaveDeath2', deathSave2X, deathSaveDeathY, deathSaveRadius);
('deathSaveDeath3', deathSave3X, deathSaveDeathY, deathSaveRadius);


*/

enum ScrollPage {
  main,
  skills
}

enum ScrollState {
  none,
  slamming,
  slammed,
  unrolling,
  unrolled,
  closing,
  closed,
  paused
}

class CharacterStatsScroll extends WorldObject {
  static readonly centerY: number = 424;
  static readonly centerX: number = 174;

  state: ScrollState = ScrollState.none;
  scrollRolls: Sprites;
  scrollSlam: Sprites;
  scrollBacks: Sprites;
  pageIndex: number;

  static readonly scrollOpenOffsets: number[] = [47, 47,  // one extra at the beginning
    48, 75, 106, 143, 215, 243, 267, 292, 315,  // 0-9
    333, 347, 360, 367, 380, 385, 395, 411, 415, 420,  // 10-19
    424, 428, 432, 432]; // One extra...

  private readonly framerateMs: number = 33; // 33 milliseconds == 30 fps
  topEmitter: Emitter;
  bottomEmitter: Emitter;

  // diagnostics:
  lastFrameIndex: number;
  lastElapsedTime: number;

  constructor() {
    super();
    this._page = ScrollPage.main;
    this.buildGoldDust();
    this.pageIndex = 0;
  }

  private _page: ScrollPage;

  get page(): ScrollPage {
    return this._page;
  }

  set page(newValue: ScrollPage) {
    if (this._page !== newValue) {
      this.close();
      this._page = newValue;
    }
  }

  slam(): void {
    this.scrollSlam.add(0, 0, 0);
    this.state = ScrollState.slamming;
  }

  open(now: number): void {
    switch (this.state) {
      case ScrollState.none:
        this.slam();
        break;
      case ScrollState.closed:
        this.unroll();
        break;
      //case ScrollState.unrolled:
      //  this.close(now);
      //  break;
    }
  }

  close(): any {
    this.state = ScrollState.closing;
    this.scrollRolls.baseAnimation.reverse = true;
  }

  private unroll(): void {
    this.state = ScrollState.unrolling;
    this.scrollRolls.baseAnimation.reverse = false;
    this.scrollRolls.sprites = [];
    this.scrollBacks.sprites = [];
    this.scrollRolls.add(0, 0, 0);
    this.scrollBacks.add(0, 0, this._page);
    this.pageIndex = this._page;
    this.topEmitter.start();
    this.bottomEmitter.start();
  }

  preUpdate(now: number, timeScale: number, world: World): void {
    super.preUpdate(now, timeScale, world);
    this.topEmitter.preUpdate(now, timeScale, world);
    this.bottomEmitter.preUpdate(now, timeScale, world);
  }

  update(now: number, timeScale: number, world: World): void {
    super.update(now, timeScale, world);
    this.topEmitter.update(now, timeScale, world);
    this.bottomEmitter.update(now, timeScale, world);

    if (this.state === ScrollState.slammed) {
      this.unroll(); // do we queue?
    }
  }

  scrollIsVisible(): boolean {
    return this.state === ScrollState.unrolling || this.state === ScrollState.unrolled ||
      this.state === ScrollState.closing || this.state === ScrollState.closed ||
      this.state === ScrollState.paused;
  }

  render(now: number, timeScale: number, world: World): void {
    super.render(now, timeScale, world);

    let justClosed: boolean = false;

    if (this.scrollIsVisible() && this.scrollRolls.sprites.length != 0) {
      let elapsedTime: number = now - this.scrollRolls.lastTimeWeAdvancedTheFrame / 1000;
      let frameIndex: number = this.scrollRolls.sprites[0].frameIndex;

      if (this.state === ScrollState.paused) {
        frameIndex = this.lastFrameIndex;
        elapsedTime = this.lastElapsedTime;
      }

      let nextFrameIndex: number;

      let stillAnimating: boolean;

      if (this.state === ScrollState.closing || this.state === ScrollState.closed) {
        stillAnimating = frameIndex > 0;
        nextFrameIndex = frameIndex - 1;
      }
      else { // opening...
        stillAnimating = frameIndex < 22;
        nextFrameIndex = frameIndex + 1;
      }

      let frameFraction: number = elapsedTime / (this.framerateMs / 1000);
      const maxFrameFraction: number = 0.99999999;
      if (frameFraction > maxFrameFraction) {
        frameFraction = maxFrameFraction;
      }

      if (this.state === ScrollState.closing)
        frameFraction = 1 - frameFraction;

      let frameIndexPrecise: number = frameIndex + frameFraction;

      if (stillAnimating || this.state === ScrollState.closing || this.state === ScrollState.closed) {
        let offset: number = CharacterStatsScroll.scrollOpenOffsets[frameIndex + 1];

        let decimalOffset: number = frameIndexPrecise - frameIndex;
        let distanceBetweenOffsets: number = CharacterStatsScroll.scrollOpenOffsets[nextFrameIndex + 1] - offset;

        let superPreciseOffset: number = offset + distanceBetweenOffsets * decimalOffset;

        let baseAnim: Part = this.scrollBacks.baseAnimation;
        let sw = this.scrollBacks.spriteWidth;
        let dx: number = 0;
        let dh: number = offset * 2;
        let dy = CharacterStatsScroll.centerY - offset;
        let sh = dh;
        baseAnim.drawCroppedByIndex(world.ctx, dx, dy, this.pageIndex, dx, dy, sw, sh, sw, dh);

        //if (this.state === ScrollState.closing && frameIndex <= 7) {
        //  this.state = ScrollState.paused;
        //  this.scrollRolls.animationStyle = AnimationStyle.Static;
        //  this.lastFrameIndex = frameIndex;
        //  this.lastElapsedTime = elapsedTime;
        //}

        if (frameIndex === 0 && this.state === ScrollState.unrolling) {
          this.topEmitter.start();
          this.bottomEmitter.start();
        }

        if (frameIndex === 0 && this.state === ScrollState.closing) {
          justClosed = true;
        }

        this.topEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY - superPreciseOffset);
        this.bottomEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY + superPreciseOffset);
      }
      else {
        this.scrollBacks.draw(world.ctx, now * 1000);
        this.topEmitter.stop();
        this.bottomEmitter.stop();
        this.state = ScrollState.unrolled;
      }

      this.topEmitter.render(now, timeScale, world);
      this.bottomEmitter.render(now, timeScale, world);

      this.scrollRolls.draw(world.ctx, now * 1000);
    }
    else
      console.log('Scroll is not visible!');

    if (this.state === ScrollState.slamming) {
      this.scrollSlam.draw(world.ctx, now * 1000);
      if (this.scrollSlam.sprites[0].frameIndex === this.scrollSlam.baseAnimation.frameCount - 1) {
        this.state = ScrollState.slammed;
      }
    }

    if (justClosed) {
      this.state = ScrollState.closed;
      this.open(now);
    }
  }

  buildGoldDust(): any {
    let windSpeed: number = 0.8;

    this.topEmitter = this.getMagicDustEmitter(windSpeed);
    this.bottomEmitter = this.getMagicDustEmitter(-windSpeed);

    this.topEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY - CharacterStatsScroll.scrollOpenOffsets[1]);
    this.bottomEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY + CharacterStatsScroll.scrollOpenOffsets[1]);
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
    emitter.particleLifeSpanSeconds = 1;
    emitter.particleGravity = 0;
    emitter.particleInitialVelocity.target = 0.4;
    emitter.particleInitialVelocity.relativeVariance = 0.5;
    emitter.initialParticleDirection = new Vector(0, Math.sign(windSpeed));
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