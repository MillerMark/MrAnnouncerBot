 //` ![](204DC0A5D26C752B4ED0E8696EBE637B.png)
class Drone extends SpriteProxy {
  displayName: string;
  userId: string;
  color: string;

  constructor(startingFrameNumber: number, public x: number, public y: number) {
    super(startingFrameNumber, x, y);
  }

  getHorizontalThrust(): number {
    return 1;
  }

  getVerticalThrust(): number {
    return -1;
  }

  drawAdornments(context: CanvasRenderingContext2D, now: number): void {
    context.font = '20px Arial';
    context.fillStyle = '#000';
    context.fillText(this.displayName, this.x, this.y);
  }
}
