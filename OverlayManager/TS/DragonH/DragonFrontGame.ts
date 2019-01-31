class DragonFrontGame extends GamePlusQuiz {
  shouldDrawCenterCrossHairs: boolean = false;
  denseSmoke: Sprites;
  puff: Sprites;

  constructor(context: CanvasRenderingContext2D) {
    super(context);
  }

  update(timestamp: number) {
    this.updateGravity();
    super.update(timestamp);
  }

  updateScreen(context: CanvasRenderingContext2D, now: number) {
    super.updateScreen(context, now);

    if (this.shouldDrawCenterCrossHairs)
      drawCrossHairs(myContext, screenCenterX, screenCenterY);

    this.denseSmoke.draw(context, now);
    this.puff.draw(context, now);
  }

  removeAllGameElements(now: number): void {
    super.removeAllGameElements(now);
  }

  initialize() {
    super.initialize();

    //Folders.assets = 'GameDev/Assets/DroneGame/';
  }

  start() {
    super.start();
    this.updateGravity();
  }

  loadResources(): void {
    super.loadResources();
    Folders.assets = 'GameDev/Assets/DragonH/';
    const fps30: number = 33;
    this.denseSmoke = new Sprites('Smoke/Dense/DenseSmoke', 116, fps30, AnimationStyle.Sequential, true);
    this.denseSmoke.name = 'DenseSmoke';
    this.denseSmoke.originX = 309;
    this.denseSmoke.originY = 723;

    this.puff = new Sprites('Smoke/Poof/Poof', 67, fps30, AnimationStyle.Sequential, true);
    this.puff.name = 'Puff';
    this.puff.originX = 229;
    this.puff.originY = 698;
  }

  executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    if (super.executeCommand(command, params, userId, userName, displayName, color, now))
      return true;
    if (command === "Cross2") {
      this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
    }
  }

  test(testCommand: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    if (super.test(testCommand, userId, userName, displayName, color, now))
      return true;

    if (testCommand === "Cross2") {
      console.log('draw Cross Hairs');
      this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
    }

    // poof 40 50
    if (testCommand.startsWith("poof")) {
      let split: string[] = testCommand.split(' ');

      let hue: number = 0;
      let saturation: number = 100;
      let brightness: number = 100;

      if (split.length === 2) {
        hue = +split[1];
      }
      else if (split.length === 3) {
        hue = +split[1];
        saturation = +split[2];
      }
      else if (split.length > 3) {
        hue = +split[1];
        saturation = +split[2];
        brightness = +split[3];
      }

      this.denseSmoke.sprites.push(new ColorShiftingSpriteProxy(0, new Vector(1050 - this.denseSmoke.originX, 1080 - this.denseSmoke.originY)).setHueSatBrightness(hue, saturation, brightness));
    }

    return false;
  }

  triggerEffect(effectData: string): void {
    let dto: any = JSON.parse(effectData);
    let center: Vector = new Vector(dto.center.x, dto.center.y);
    let sprites: Sprites;
    if (dto.spriteName === 'DenseSmoke')
      sprites = this.denseSmoke;
    else if (dto.spriteName === 'Puff')
      sprites = this.puff;

    let spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(center), dto.startFrameIndex, dto.hueShift, dto.saturation, dto.brightness);
    spritesEffect.start();
  }

} 