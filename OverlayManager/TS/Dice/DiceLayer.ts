class DiceLayer {
  diceCanvas: HTMLCanvasElement;
  diceContext: CanvasRenderingContext2D;
  diceFireball: Sprites;
  allEffects: SpriteCollection;

	constructor() {
    this.loadSprites();
  }

  loadSprites() {
    Part.loadSprites = true;
    const fps30: number = 33;

    this.allEffects = new SpriteCollection();
    this.diceFireball = new Sprites("/Dice/Roll20Fireball/DiceFireball", 71, fps30, AnimationStyle.Sequential, true);
    this.diceFireball.originX = 108;
    this.diceFireball.originY = 158;
    this.allEffects.add(this.diceFireball);
  }

  getContext() {
    this.diceCanvas = <HTMLCanvasElement>document.getElementById("diceCanvas");
    this.diceContext = this.diceCanvas.getContext("2d");
  }

  renderCanvas() {
    if (!this.diceContext)
      this.getContext();

    this.diceContext.clearRect(0, 0, 1920, 1080);
    var now: number = performance.now();
    this.allEffects.updatePositions(now);
    this.allEffects.draw(this.diceContext, now);
  }

  testFireball() {
    this.diceFireball.add(960, 540, 0);
  }

  rollDice(diceRollData: string): any {
    console.log(diceRollData);
  } 
}