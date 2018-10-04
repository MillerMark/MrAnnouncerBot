class SpriteProxy {
  frameIndex: any;
  timeStart: number;
  velocityX: number;
  velocityY: number;
  startX: any;
  startY: any;

  constructor(startingFrameNumber: number, public x: number, public y: number) {
    this.frameIndex = startingFrameNumber;
    this.timeStart = performance.now();
    this.velocityX = 0;
    this.velocityY = 0;
    this.startX = x;
    this.startY = y;
  }

  getHorizontalThrust(): number {
  	return 0;
  }

  getVerticalThrust(): number {
    return gravityGames.activePlanet.gravity;
  }

  bounce(left: number, top: number, right: number, bottom: number, width: number, height: number, now: number) {
    var secondsPassed = (now - this.timeStart) / 1000;
    var horizontalBounceDecay = 1;
    var verticalBounceDecay = 1;

    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalThrust());
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalThrust());

    var hitLeftWall = velocityX < 0 && this.x < left;
    var hitRightWall = velocityX > 0 && this.x + width > right;

    var hitTopWall = velocityY < 0 && this.y < top;
    var hitBottomWall = velocityY > 0 && this.y + height > bottom;

    var newVelocityX = velocityX;
    var newVelocityY = velocityY;
    if (hitLeftWall || hitRightWall)
      newVelocityX = -velocityX * horizontalBounceDecay;
    if (hitTopWall || hitBottomWall)
      newVelocityY = -velocityY * verticalBounceDecay;

    if (hitLeftWall || hitRightWall || hitTopWall || hitBottomWall) {
      this.x = this.startX + Physics.metersToPixels(Physics.getDisplacement(secondsPassed, this.velocityX, this.getHorizontalThrust()));
      this.y = this.startY + Physics.metersToPixels(Physics.getDisplacement(secondsPassed, this.velocityY, this.getVerticalThrust()));
      this.changeVelocity(newVelocityX, newVelocityY, now);
    }
    return hitBottomWall;
  }

  advanceFrame(frameCount: number, returnFrameIndex: number = 0, startIndex: number = 0, endBounds: number = 0) {
    this.frameIndex++;
    if (startIndex != 0 && endBounds != 0) {
      if (this.frameIndex >= endBounds)
        this.frameIndex = startIndex;
    }
    else if (this.frameIndex >= frameCount)
      this.frameIndex = returnFrameIndex;
  }

  changingDirection(now: number): void {
    var secondsPassed = (now - this.timeStart) / 1000;
    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalThrust());
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalThrust());
    this.changeVelocity(velocityX, velocityY, now);
  }

  changeVelocity(velocityX, velocityY, now) {
    this.timeStart = now;
    this.velocityX = velocityX;
    this.velocityY = velocityY;
    this.startX = this.x;
    this.startY = this.y;
  }

  drawAdornments(context: CanvasRenderingContext2D, now: number): void {
    // Descendants can override if they want to draw...
  }

  matches(matchData: string): boolean {
    return false; // Descendants can override if they want to implement a custom search/find functionality...
  }

  updatePosition(now) {
    var secondsPassed = (now - this.timeStart) / 1000;

    var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, this.getHorizontalThrust());
    this.x = this.startX + Physics.metersToPixels(xDisplacement);

    var yDisplacement = Physics.getDisplacement(secondsPassed, this.velocityY, this.getVerticalThrust());
    this.y = this.startY + Physics.metersToPixels(yDisplacement);
  }
}