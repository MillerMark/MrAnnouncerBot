class DiceLayer {
  diceFrontCanvas: HTMLCanvasElement;
  diceBackCanvas: HTMLCanvasElement;
  diceFrontContext: CanvasRenderingContext2D;
  diceBackContext: CanvasRenderingContext2D;
  diceFireball: Sprites;
  d20Fire: Sprites;
  roll1Stink: Sprites;
  dicePortal: Sprites;
  dicePortalTop: Sprites;
  magicRing: Sprites;
  diceBlow: Sprites;
  allFrontLayerEffects: SpriteCollection;
  allBackLayerEffects: SpriteCollection;

	constructor() {
    this.loadSprites();
  }

  loadSprites() {
    Part.loadSprites = true;
    const fps30: number = 33;
    const fps20: number = 50;

    this.allFrontLayerEffects = new SpriteCollection();
    this.allBackLayerEffects = new SpriteCollection();

    this.diceFireball = new Sprites("/Dice/Roll20Fireball/DiceFireball", 71, fps30, AnimationStyle.Sequential, true);
    this.diceFireball.originX = 104;
    this.diceFireball.originY = 155;
    this.allFrontLayerEffects.add(this.diceFireball);

    this.d20Fire = new Sprites("/Dice/D20Fire/D20Fire", 180, fps30, AnimationStyle.Loop, true);
    this.d20Fire.originX = 151;
    this.d20Fire.originY = 149;
    this.d20Fire.returnFrameIndex = 72;
    this.allBackLayerEffects.add(this.d20Fire);

    this.dicePortal = new Sprites("/Dice/DiePortal/DiePortal", 73, fps20, AnimationStyle.Sequential, true);
    this.dicePortal.originX = 189;
    this.dicePortal.originY = 212;
    this.allBackLayerEffects.add(this.dicePortal);

    this.roll1Stink = new Sprites("/Dice/Roll1/Roll", 172, fps30, AnimationStyle.Loop, true);
    this.roll1Stink.originX = 150;
    this.roll1Stink.originY = 150;
    this.allBackLayerEffects.add(this.roll1Stink);

    this.magicRing = new Sprites("/Dice/MagicRing/MagicRing", 180, fps30, AnimationStyle.Loop, true);
    this.magicRing.returnFrameIndex = 30;
    this.magicRing.originX = 140;
    this.magicRing.originY = 112;
    this.allFrontLayerEffects.add(this.magicRing);

    this.diceBlow = new Sprites("/Dice/Blow/DiceBlow", 41, fps30, AnimationStyle.Sequential, true);
    this.diceBlow.originX = 178;
    this.diceBlow.originY = 170;
    this.allFrontLayerEffects.add(this.diceBlow);

    this.dicePortalTop = new Sprites("/Dice/DiePortal/DiePortalTop", 73, fps20, AnimationStyle.Sequential, true);
    this.dicePortalTop.originX = 189;
    this.dicePortalTop.originY = 212;
    this.allFrontLayerEffects.add(this.dicePortalTop);
    
  }

  getContext() {
    this.diceFrontCanvas = <HTMLCanvasElement>document.getElementById("diceFrontCanvas");
    this.diceBackCanvas = <HTMLCanvasElement>document.getElementById("diceBackCanvas");
    this.diceFrontContext = this.diceFrontCanvas.getContext("2d");
    this.diceBackContext = this.diceBackCanvas.getContext("2d");
  }

  renderCanvas() {
    if (!this.diceFrontContext || !this.diceBackContext)
      this.getContext();

    this.diceFrontContext.clearRect(0, 0, 1920, 1080);
    this.diceBackContext.clearRect(0, 0, 1920, 1080);
    var now: number = performance.now();
    this.allFrontLayerEffects.updatePositions(now);
    this.allBackLayerEffects.updatePositions(now);
    this.allFrontLayerEffects.draw(this.diceFrontContext, now);
    this.allBackLayerEffects.draw(this.diceBackContext, now);
  }

  testFireball(x: number, y: number) {
    this.diceFireball.add(x, y, 0);
  }

  testMagicRing(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    this.magicRing.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
  }

  testDiceBlow(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    this.diceBlow.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
  }

  testD20Fire(x: number, y: number) {
    this.d20Fire.add(x, y);
  }

  testRoll1Stink(x: number, y: number) {
    this.roll1Stink.add(x, y);
  }

  clearLoopingAnimations(): any {
    this.magicRing.sprites = [];
    this.d20Fire.sprites = [];
    this.roll1Stink.sprites = [];
  }

  testPortal(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    this.dicePortal.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
    this.dicePortalTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
  }

  rollDice(diceRollData: string): any {
    console.log(diceRollData);
  } 
}