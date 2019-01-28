enum Orientation {
  Horizontal,
  Vertical
}

enum WallStyle {
  None,
  Solid,
  Dashed,
  Double
}

class EndCap extends SpriteProxy{
  constructor(startingFrameNumber: number, x: number, y: number, lifeSpanMs: number = -1) {
    super(startingFrameNumber, x, y, lifeSpanMs);
  }

  static readonly suction: number = 30;
  static readonly gravity: number = 20;
  static readonly initialExpansionVelocity: number = 6; // m/s
  static readonly expansionTime: number = 1.25;  // sec
  static readonly totalCapDisplacement: number = Physics.getDisplacement(EndCap.expansionTime,
                                                                         EndCap.initialExpansionVelocity,
                                                                         EndCap.suction);

  getVerticalThrust(now: number): number {
    return EndCap.gravity;
  }
}

class Wall extends SpriteProxy {
  endCap1: EndCap;
  endCap2: EndCap;
  actualLength: number;
  halfActualLength: number;
  finalCapLeft: number;
  finalCapTop: number;
  airTime: number;
  capX: number;
  capY: number;
  stillFlying: boolean;

  constructor(startingFrameNumber: number, x: number, y: number,
    public orientation: Orientation, public wallStyle: WallStyle,
    public length: number) {
    super(startingFrameNumber, x, y, -1);
    this.actualLength = 0;

    let capIndex: number;

    if (!(activeBackGame instanceof DroneGame))
      return;

    this.finalCapLeft = x - activeBackGame.endCaps.spriteWidth / 2;
    this.finalCapTop = y - activeBackGame.endCaps.spriteHeight / 2;
    if (orientation === Orientation.Horizontal) {
      capIndex = 0;  // ![](AADF5C4D211EE772BCCD9F0A76FC3D5F.png;;;0.01824,0.01824)
    }
    else { // Orientation.Vertical
      capIndex = 2;  // ![](F159D9E3D6CFB1F0529BA74BF477DE15.png;;0,0,68,48;0.02000,0.02000)
    }

    let ySafetyDrop: number = activeBackGame.endCaps.spriteHeight;
    let heightMeters: number = Physics.pixelsToMeters(screenHeight - this.finalCapTop + ySafetyDrop);
    this.airTime = Physics.getDropTime(heightMeters, EndCap.gravity);
    let initialVelocityY: number = Physics.getFinalVelocity(this.airTime, 0, EndCap.gravity);
    let initialVelocityX: number = 3;
    this.capX = this.finalCapLeft - Physics.metersToPixels(Physics.getDisplacement(this.airTime, initialVelocityX, 0));
    this.capY = screenHeight + ySafetyDrop;

    // TODO: Figure out starting point for cap...
    this.endCap1 = new EndCap(capIndex, this.capX, this.capY);
    this.endCap2 = new EndCap(capIndex + 1, this.capX, this.capY);
    let now: number = performance.now();
    this.endCap1.changeVelocity(initialVelocityX, -initialVelocityY, now);
    this.endCap2.changeVelocity(initialVelocityX, -initialVelocityY, now);
    this.stillFlying = true;

    activeBackGame.endCaps.sprites.push(this.endCap1);
    activeBackGame.endCaps.sprites.push(this.endCap2);
    super.cropped = true;
  }

  destroyBy(lifeTimeMs: number): any {
    super.destroyBy(lifeTimeMs);
    this.endCap1.expirationDate = this.expirationDate;
    this.endCap2.expirationDate = this.expirationDate;
  }

  updatePosition(now: number) {
    this.storeLastPosition();
    var secondsPassed = (now - this.timeStart) / 1000;
    if (secondsPassed < this.airTime) {
      this.endCap1.updatePosition(now);
      this.endCap2.updatePosition(now);
    }
    else if (this.stillFlying) {
      this.stillFlying = false;
      this.endCap1.changeVelocity(0, 0, now);
      this.endCap2.changeVelocity(0, 0, now);
      this.endCap1.x = this.finalCapLeft;
      this.endCap2.x = this.finalCapLeft;
      this.endCap1.y = this.finalCapTop;
      this.endCap2.y = this.finalCapTop;
    }

    secondsPassed -= this.airTime;
    if (secondsPassed < 0)
      return;

    this.halfActualLength = this.length / 2;
    if (secondsPassed < EndCap.expansionTime) {
      //this.halfActualLength = this.length * (secondsPassed / extendTime) / 2;                                    	
      var currentCapDisplacement: number = Physics.getDisplacement(EndCap.expansionTime - secondsPassed, EndCap.initialExpansionVelocity, EndCap.suction);
      this.halfActualLength *= (EndCap.totalCapDisplacement - currentCapDisplacement) / EndCap.totalCapDisplacement;
    }

    if (!(activeBackGame instanceof DroneGame))
      return;

    let halfCapLength: number = activeBackGame.endCaps.spriteHeight / 2;
    if (this.orientation === Orientation.Horizontal) {
      this.endCap1.x = this.x - this.halfActualLength - halfCapLength;
      this.endCap1.y = this.y - halfCapLength;
      this.endCap2.x = this.x + this.halfActualLength - halfCapLength;
      this.endCap2.y = this.y - halfCapLength;
    }
    else {
      this.endCap1.x = this.x - halfCapLength;
      this.endCap1.y = this.y - this.halfActualLength - halfCapLength;
      this.endCap2.x = this.x - halfCapLength;
      this.endCap2.y = this.y + this.halfActualLength - halfCapLength;
    }
  }

  removing(): void {
    var now: number = performance.now();
    this.endCap1.expirationDate = now;
    this.endCap2.expirationDate = now;
    if (!(activeBackGame instanceof DroneGame))
      return;
    activeBackGame.endCaps.removeExpiredSprites(now + 1);
    super.removing();
  }

  draw(baseAnimation: Part, context: CanvasRenderingContext2D, now: number, spriteWidth: number, spriteHeight: number): void {
    let xOffset: number;
    let yOffset: number;
    let sx: number;
    let sy: number;
    let width: number;
    let height: number;
    let centerX: number = baseAnimation.images[0].width / 2;
    let centerY: number = baseAnimation.images[0].height / 2;

    // ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)
    const jiggleBonus: number = 3;

    if (this.orientation === Orientation.Horizontal) {
      height = spriteHeight;
      sy = 0;
      width = this.halfActualLength * 2 + jiggleBonus;
      sx = centerX - this.halfActualLength;
      xOffset = -this.halfActualLength - jiggleBonus / 2;
      yOffset = -spriteHeight / 2 + 1;
      //this.endCap2.x = this.x + this.halfActualLength - halfCapLength;
      //this.endCap2.y = this.y - halfCapLength;
    }
    else {
      // TODO: Fix for dashed lines. Pass in these dimensions?
      width = spriteWidth;
      sx = 0;
      height = this.halfActualLength * 2 + jiggleBonus;
      sy = centerY - this.halfActualLength;
      xOffset = -spriteWidth / 2 - 1;
      yOffset = -this.halfActualLength - jiggleBonus / 2;
    }

    baseAnimation.drawCroppedByIndex(context, this.x + xOffset, this.y + yOffset, this.frameIndex, sx, sy, width, height, width, height);
  }

  getLine(): any {
    if (!(activeBackGame instanceof DroneGame))
      return;
    
    return Line.fromCoordinates(this.endCap1.x + activeBackGame.endCaps.spriteWidth / 2, this.endCap1.y + activeBackGame.endCaps.spriteHeight / 2,
      this.endCap2.x + activeBackGame.endCaps.spriteWidth / 2, this.endCap2.y + activeBackGame.endCaps.spriteHeight / 2);
  }

}