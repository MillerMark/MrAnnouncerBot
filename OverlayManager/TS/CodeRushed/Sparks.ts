class Sparks extends SpriteProxy {
  verticalThrust: number;
  horizontalThrust: number;
  parent: Sprites;

  constructor(startingFrameNumber: number, x: number, y: number) {
    super(startingFrameNumber, x, y);
    this.verticalThrust = 0;
    this.horizontalThrust = 0;
    this.systemDrawn = false;
  }

  draw(baseAnimation: Part, context: CanvasRenderingContext2D, now: number, spriteWidth: number, spriteHeight: number): void {
    //console.log('draw(' + this.x + ', ' + this.y + ')');
		baseAnimation.drawByIndex(context, this.x, this.y, this.frameIndex, this.scale);
  }

  getHorizontalThrust(now: number): number {
    return this.horizontalThrust;
  }

  getVerticalThrust(now: number): number {
    return this.verticalThrust;
  }

  setParent(parent: Sprites): any {
    this.parent = parent;
    if (this.parent != null)
      this.parent.sprites.push(this);
  }
}