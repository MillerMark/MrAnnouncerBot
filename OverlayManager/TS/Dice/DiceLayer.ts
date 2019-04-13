class DiceLayer {
  diceFrontCanvas: HTMLCanvasElement;
  diceBackCanvas: HTMLCanvasElement;
  diceFrontContext: CanvasRenderingContext2D;
  diceBackContext: CanvasRenderingContext2D;
  diceFireball: Sprites;
  dicePortal: Sprites;
  dicePortalTop: Sprites;
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

    this.dicePortal = new Sprites("/Dice/DiePortal/DiePortal", 73, fps20, AnimationStyle.Sequential, true);
    this.dicePortal.originX = 189;
    this.dicePortal.originY = 212;
    this.allBackLayerEffects.add(this.dicePortal);

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

  testPortal(x: number, y: number) {
    this.dicePortal.add(x, y, 0);
    this.dicePortalTop.add(x, y, 0);
  }

  rollDice(diceRollData: string): any {
    console.log(diceRollData);
  } 
}