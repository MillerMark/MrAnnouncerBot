enum Orientation {
  Horizontal,
  Vertical
}

enum WallType {
  Solid,
  Dashed
}

class Wall extends SpriteProxy {
  endCap1: SpriteProxy;
  endCap2: SpriteProxy;
  actualLength: number;
  halfActualLength: number;

  constructor(startingFrameNumber: number, x: number, y: number,
              public orientation: Orientation, public wallType: WallType,
              public length: number) {
    super(startingFrameNumber, x, y, -1);
    this.actualLength = 0;
    if (orientation === Orientation.Horizontal) {
      let capLeft: number = x - endCaps.spriteWidth / 2;
      this.endCap1 = new SpriteProxy(0, capLeft, y);
      this.endCap2 = new SpriteProxy(1, capLeft, y);
    }
    else {
      let capTop: number = y - endCaps.spriteHeight / 2;
      this.endCap1 = new SpriteProxy(2, x, capTop);
      this.endCap2 = new SpriteProxy(3, x, capTop);
    }
    endCaps.sprites.push(this.endCap1);
    endCaps.sprites.push(this.endCap2);
    super.cropped = true;
  }

  updatePosition(now: number) {
    var secondsPassed = (now - this.timeStart) / 1000;
    const extendTime: number = 2;
    
    if (secondsPassed < extendTime)
      this.halfActualLength = this.length * (secondsPassed / extendTime) / 2;
    else
      this.halfActualLength = this.length / 2;

    let halfCapLength: number = endCaps.spriteHeight / 2;
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
}