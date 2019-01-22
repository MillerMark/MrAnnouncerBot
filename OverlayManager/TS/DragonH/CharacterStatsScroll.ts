class CharacterStatsScroll extends WorldObject {
  scrollOpenSfx: HTMLAudioElement;
  scrollWooshSfx: HTMLAudioElement;
  scrollCloseSfx: HTMLAudioElement;
  scrollSlamSfx: HTMLAudioElement;

  characters: Array<Character> = new Array<Character>();
  pages: Array<StatPage> = new Array<StatPage>();

  selectedStatPageIndex: number = -1;

  private _selectedCharacterIndex: number;

  get selectedCharacterIndex(): number {
    return this._selectedCharacterIndex;
  }

  set selectedCharacterIndex(newValue: number) {
    this._selectedCharacterIndex = newValue;
  }

  static readonly centerY: number = 424;
  static readonly centerX: number = 174;

  state: ScrollState = ScrollState.none;
  scrollRolls: Sprites;
  scrollEmphasisMain: Sprites;
  scrollEmphasisSkills: Sprites;
  scrollSlam: Sprites;
  scrollBacks: Sprites;
  players: Sprites;

  // TODO: consolidate with page, if possible.
  pageIndex: number;

  static readonly scrollOpenOffsets: number[] = [47,  // one extra at the beginning (to simplify code that looks at the previous/next offset)
    47, 48, 75, 106, 143, 215, 243, 267, 292, 315,  // 1-10
    333, 347, 360, 367, 380, 385, 395, 411, 415, 420,  // 11-20
    424, 428, 432,  // 21-23
    432]; // One extra at the end (to simplify code that looks at the previous/next offset)...

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
    this.pageIndex = this._page;
    this._selectedCharacterIndex = -1;
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
    this.play(this.scrollWooshSfx);
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
    this.play(this.scrollCloseSfx);
  }

  private unroll(): void {
    this.state = ScrollState.unrolling;
    this.scrollRolls.baseAnimation.reverse = false;
    this.scrollRolls.sprites = [];
    this.scrollBacks.sprites = [];
    this.scrollRolls.add(0, 0, 0);
    this.scrollBacks.add(0, 0, this._page);
    this.players.sprites = [];
    this.players.add(0, 0, this.selectedCharacterIndex);
    this.pageIndex = this._page;
    this.selectedStatPageIndex = this._page - 1;
    this.topEmitter.start();
    this.bottomEmitter.start();
    this.play(this.scrollOpenSfx);
  }

  play(audio: HTMLAudioElement): any {
    const playPromise = audio.play();
    if (playPromise["[[PromiseStatus]]"] === 'rejected') {
      console.log(playPromise["[[PromiseValue]]"].message);
    }
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

    const picX: number = 17;
    const picWidth: number = 104;
    const picY: number = 24;
    const picHeight: number = 90;

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

      let topData: number = 0;
      let bottomData: number = Infinity;

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
        topData = dy;
        bottomData = dy + dh;
        let sh: number = dh;
        let dw: number = sw;
        let sx: number = dx;
        let sy: number = dy;

        baseAnim.drawCroppedByIndex(world.ctx, sx, sy, this.pageIndex, dx, dy, sw, sh, sw, dh);
        this.drawHighlighting(now, timeScale, world, sx, sy, dx, dy, sw, sh, dw, dh);

        let picOffsetY: number = topData - picY;
        let picCroppedHeight: number = picY + picHeight - topData;

        if (picCroppedHeight > 0) {
          // ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)

          this.players.baseAnimation.drawCroppedByIndex(world.ctx, picX, picY + picOffsetY, this.selectedCharacterIndex, 0, picOffsetY, picWidth, picCroppedHeight, picWidth, picCroppedHeight);
        }

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
        this.players.baseAnimation.drawByIndex(world.ctx, picX, picY, this.selectedCharacterIndex);
        this.topEmitter.stop();
        this.bottomEmitter.stop();
        this.state = ScrollState.unrolled;
        this.drawHighlighting(now, timeScale, world);
      }



      this.topEmitter.render(now, timeScale, world);
      this.bottomEmitter.render(now, timeScale, world);

      this.drawCharacterStats(world.ctx, topData, bottomData);

      this.scrollRolls.draw(world.ctx, now * 1000);
    }
    else {
      // console.log('Scroll is not visible!');
    }

    if (this.state === ScrollState.slamming) {
      this.scrollSlam.draw(world.ctx, now * 1000);
      if (this.scrollSlam.sprites[0].frameIndex === this.scrollSlam.baseAnimation.frameCount - 1) {
        this.state = ScrollState.slammed;

        this.play(this.scrollSlamSfx);
      }
    }

    if (justClosed) {
      this.state = ScrollState.closed;
      this.open(now);
    }
  }

  drawHighlighting(now: number, timeScale: number, world: World, sx: number = 0, sy: number = 0, dx: number = 0, dy: number = 0, sw: number = 0, sh: number = 0, dw: number = 0, dh: number = 0): void {
    if (!this.weAreHighlighting())
      return;

    this.drawDeEmphasisLayer(world, now, sx, sy, dx, dy, sw, sh, dw, dh);
    if (dh > 0) {
      this.scrollEmphasisMain.drawCropped(world.ctx, now * 1000, sx, sy, dx, dy, sw, sh, dw, dh);
      this.scrollEmphasisSkills.drawCropped(world.ctx, now * 1000, sx, sy, dx, dy, sw, sh, dw, dh);
    }
    else {
      this.scrollEmphasisMain.draw(world.ctx, now * 1000);
      this.scrollEmphasisSkills.draw(world.ctx, now * 1000);
    }
  }

  drawDeEmphasisLayer(world: World, now: number, sx: number = 0, sy: number = 0, dx: number = 0, dy: number = 0, sw: number = 0, sh: number = 0, dw: number = 0, dh: number = 0): void {
    if (dh > 0) {
      this.scrollBacks.baseAnimation.drawCroppedByIndex(world.ctx, dx, dy, 0, sx, sy, sw, sh, dw, dh);
    }
    else {
      this.scrollBacks.baseAnimation.drawByIndex(world.ctx, 0, 0, 0);
    }
  }

  weAreHighlighting(): any {
    return this.scrollEmphasisMain.sprites.length > 0 || this.scrollEmphasisSkills.sprites.length > 0;
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
    this.scrollBacks = new Sprites("Scroll/Backs/Back", 4, this.framerateMs, AnimationStyle.Static);
    this.players = new Sprites("Scroll/Players/Player", 4, this.framerateMs, AnimationStyle.Static);
    this.scrollEmphasisMain = new Sprites("Scroll/Emphasis/Main/EmphasisMain", 27, this.framerateMs, AnimationStyle.Static);
    this.scrollEmphasisSkills = new Sprites("Scroll/Emphasis/Skills/EmphasisSkills", 27, this.framerateMs, AnimationStyle.Static);

    this.scrollWooshSfx = new Audio(Folders.assets + 'SoundEffects/scrollWoosh.mp3');
    this.scrollOpenSfx = new Audio(Folders.assets + 'SoundEffects/scrollOpen.mp3');
    this.scrollCloseSfx = new Audio(Folders.assets + 'SoundEffects/scrollClose.mp3');
    this.scrollSlamSfx = new Audio(Folders.assets + 'SoundEffects/scrollSlam.mp3');

    Folders.assets = assetFolderName;
  }

  drawCharacterStats(context: CanvasRenderingContext2D, topData: number, bottomData: number): void {
    if (this.selectedCharacterIndex < 0)
      return;

    if (this.selectedStatPageIndex < 0)
      return;
    let activeCharacter: Character = this.characters[this.selectedCharacterIndex];
    let activePage: StatPage = this.pages[this.selectedStatPageIndex];

    activePage.render(context, activeCharacter, topData, bottomData);
  }

  clear(): any {
    this.characters = [];
    this.pages = [];
    this.selectedCharacterIndex = -1;
    this.selectedStatPageIndex = -1;
    this.state = ScrollState.none;
  }

  playerPageChanged(playerID: number, pageID: number, playerData: string): any {

    console.log(`playerPageChanged(${playerID}, ${pageID}, ${playerData})`);
    if (this.selectedCharacterIndex !== playerID) {
      this.selectedCharacterIndex = playerID;
      this._page = pageID;
      this.state = ScrollState.none;
      this.open(performance.now());
    }
    else if (this.page != pageID) {
      this.state = ScrollState.none;
      this.page = pageID;
      this.open(performance.now());
    }

  }

  addEmphasis(emphasisIndex: number): void {
    if (this.page === ScrollPage.main)
      this.scrollEmphasisMain.add(0, 0, emphasisIndex);
    else if (this.page === ScrollPage.skills)
      this.scrollEmphasisSkills.add(0, 0, emphasisIndex);
  }


  focusItem(playerID: number, pageID: number, itemID: string): any {
    console.log(`focusItem(${playerID}, ${pageID}, ${itemID})`);
  }

  unfocusItem(playerID: number, pageID: number, itemID: string): any {
    console.log(`unfocusItem(${playerID}, ${pageID}, ${itemID})`);
  }

}