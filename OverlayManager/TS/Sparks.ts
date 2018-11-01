class Sparks extends SpriteProxy {
  verticalThrust: any;
  horizontalThrust: any;
  constructor(startingFrameNumber: number, public x: number, public y: number) {
    super(startingFrameNumber, x, y);
  }

  getHorizontalThrust(now: number): number {
    return this.horizontalThrust;
  }

  getVerticalThrust(now: number): number {
    return this.verticalThrust;
  }
}