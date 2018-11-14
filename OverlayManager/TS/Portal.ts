class Portal extends SpriteProxy {
  static readonly size: number = 160;
   background: SpriteProxy;
  
  constructor(startingFrameNumber: number, public x: number, public y: number) {
    super(startingFrameNumber, x, y);
    this.background = portalBackground.add(x, y);
  }

  drop(): void {
    purpleMeteors.sprites.push(new Meteor(Random.getInt(purpleMeteors.baseAnimation.frameCount), this.x + (Portal.size - meteorWidth) / 2, this.y + (Portal.size - meteorHeight) / 2));
  }

  set delayStart(delayMs: number) {
    super.delayStart = delayMs;
    this.background.timeStart = performance.now() + delayMs;
  }
}