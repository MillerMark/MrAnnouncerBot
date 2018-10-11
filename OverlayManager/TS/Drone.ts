//` ![](204DC0A5D26C752B4ED0E8696EBE637B.png)

var splatSoundEffect = new Audio(Folders.assets + 'Sound Effects/Splat.mp3');

class Drone extends SpriteProxy {
  displayName: string;
  userId: string;
  backgroundColor: string = '#fff';
  outlineColor: string = '#888';
  height: number;
  width: number;
  leftThrustOffTime: number;
  leftThrustOnTime: number;
  rightThrustOffTime: number;
  rightThrustOnTime: number;
  upThrustOffTime: number;
  upThrustOnTime: number;
  downThrustOffTime: number;
  downThrustOnTime: number;

  private _color: string;
  private lastUpdateTime: number;
  private wasThrustingRight: boolean;
  private wasThrustingUp: boolean;
  private wasThrustingDown: boolean;
  private wasThrustingLeft: boolean;
  private lastVelocityY: number;
  private lastVelocityX: number;
  private lastNow: number;


  constructor(startingFrameNumber: number, public x: number, public y: number) {
    super(startingFrameNumber, x, y);
    let now: number = performance.now();
    this.leftThrustOffTime = now;
    this.leftThrustOnTime = now;
    this.rightThrustOffTime = now;
    this.rightThrustOnTime = now;
    this.upThrustOffTime = now;
    this.upThrustOnTime = now;
    this.downThrustOffTime = now;
    this.downThrustOnTime = now;
    this.lastUpdateTime = now;
    this.lastVelocityX = 0;
    this.lastVelocityY = 0;
    this.lastNow = now;
  }

  get color(): string {
    return this._color;
  }

  set color(newValue: string) {
    if (this._color === newValue)
      return;

    let textHsl: HueSatLight = HueSatLight.fromHex(newValue);
    let backgroundHsl: HueSatLight = HueSatLight.clone(textHsl);
    let outlineHsl: HueSatLight = HueSatLight.clone(textHsl);
    if (backgroundHsl.isBright()) {
      backgroundHsl.light = 0.02;
      if (textHsl.getPerceivedBrightness() < 0.7)
        textHsl.light += 0.15;
      outlineHsl.light = 0.3;
    }
    else {
      if (textHsl.getPerceivedBrightness() > 0.3)
        textHsl.light -= 0.15;
      backgroundHsl.light = 0.98;
      outlineHsl.light = 0.92;
    }

    this.backgroundColor = backgroundHsl.toHex();
    this.outlineColor = outlineHsl.toHex();
    this._color = textHsl.toHex();
  }

  updatePosition(now: number) {
    var secondsPassed = (now - this.timeStart) / 1000;

    var hAccel = this.getHorizontalThrust(now);
    var vAccel = this.getVerticalThrust(now);

    this.updateFrameIndex(now, hAccel, vAccel);
    var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, hAccel);

    var justTurnedOffLeftTruster: boolean = this.wasThrustingLeft && this.leftThrustOffTime <= now;
    var justTurnedOffRightTruster: boolean = this.wasThrustingRight && this.rightThrustOffTime <= now;

    if (justTurnedOffLeftTruster)
      this.wasThrustingLeft = false;

    if (justTurnedOffRightTruster)
      this.wasThrustingRight = false;

    var yDisplacement = Physics.getDisplacement(secondsPassed, this.velocityY, vAccel);

    var justTurnedOffUpTruster: boolean = this.wasThrustingUp && this.upThrustOffTime <= now;
    var justTurnedOffDownTruster: boolean = this.wasThrustingDown && this.downThrustOffTime <= now;

    if (justTurnedOffUpTruster)
      this.wasThrustingUp = false;

    if (justTurnedOffDownTruster)
      this.wasThrustingDown = false;

    let finalVelocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, hAccel);
    let finalVelocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, vAccel);

    if (justTurnedOffUpTruster || justTurnedOffDownTruster || justTurnedOffLeftTruster || justTurnedOffRightTruster) {
      this.changeVelocity(this.lastVelocityX, this.lastVelocityY, this.lastNow);
    }
    else {
      this.x = this.startX + Physics.metersToPixels(xDisplacement);
      this.y = this.startY + Physics.metersToPixels(yDisplacement);
    }

    if (!this.wasThrustingUp && !this.wasThrustingDown && !this.wasThrustingLeft && !this.wasThrustingRight) {
      let timeSinceLastUpdate: number = performance.now() - this.lastUpdateTime;
      if (timeSinceLastUpdate > 10) {
        this.changeVelocityBy(0.99, 0.99, now);
        this.lastUpdateTime = now;
      }
    }

    this.lastVelocityX = finalVelocityX;
    this.lastVelocityY = finalVelocityY;
    this.lastNow = now;
  }

  updateFrameIndex(now: number, hAccel: number, vAccel: number): any {
    var pitchIndex: number = 0;
    var rollIndex: number = 0;
    pitchIndex = Math.sign(vAccel) + 1;
    const highRollSwitchMs: number = 300;
    if (hAccel < 0)
      if (now - this.leftThrustOnTime > highRollSwitchMs && this.leftThrustOffTime - now > highRollSwitchMs)
        rollIndex = 9;
      else
        rollIndex = 7;
    else if (hAccel > 0)
      if (now - this.rightThrustOnTime > highRollSwitchMs && this.rightThrustOffTime - now > highRollSwitchMs)
        rollIndex = 1;
      else
        rollIndex = 3;
    else
      rollIndex = 5;
    this.frameIndex = pitchIndex * 10 + rollIndex;
  }

  HorizontalThrust: number = 2;
  VerticalThrust: number = 2;

  getHorizontalThrust(now: number): number {
    let thrust: number = 0;
    if (this.rightThrustOffTime > now)
      thrust += this.HorizontalThrust;
    if (this.leftThrustOffTime > now)
      thrust -= this.HorizontalThrust;
    return thrust;
  }

  getVerticalThrust(now: number): number {
    let thrust: number = 0;
    if (this.upThrustOffTime > now)
      thrust -= this.VerticalThrust;
    if (this.downThrustOffTime > now)
      thrust += this.VerticalThrust;
    return thrust;
  }

  drawAdornments(context: CanvasRenderingContext2D, now: number): void {
    const fontSize: number = 14;
    context.font = fontSize + 'px Arial';

    context.textBaseline = 'top'; //` ![](774083667316C80C98D43F9C370CC1C8.png;;0,68,400,130;0.02510,0.02510)
    context.textAlign = 'center';

    let centerX: number = this.x + this.width / 2;
    let yTop: number = this.y + this.height;
    let size = context.measureText(this.displayName);
    let halfWidth: number = size.width / 2;
    context.fillStyle = this.backgroundColor;
    const outlineSize: number = 3;
    context.globalAlpha = 0.7;
    context.fillRect(centerX - halfWidth - outlineSize, yTop - outlineSize, size.width + outlineSize * 2, fontSize + outlineSize * 2);
    context.globalAlpha = 1;

    context.strokeStyle = this.outlineColor;
    context.lineWidth = 2;
    context.strokeRect(centerX - halfWidth - outlineSize, yTop - outlineSize, size.width + outlineSize * 2, fontSize + outlineSize * 2);


    context.fillStyle = this.color;
    context.fillText(this.displayName, centerX, yTop);
  }

  getSplats(command: string): SplatSprites {
    if (command === 'red')
      return redSplotches;
    else if (command === 'black')
      return blackSplotches;
    else if (command === 'white')
      return whiteSplotches;
    else if (command === 'orange')
      return orangeSplotches;
    else if (command === 'amber')
      return amberSplotches;
    else if (command === 'yellow')
      return yellowSplotches;
    else if (command === 'green')
      return greenSplotches;
    else if (command === 'blue')
      return blueSplotches;
    else if (command === 'cyan')
      return cyanSplotches;
    else if (command === 'indigo')
      return indigoSplotches;
    else if (command === 'magenta')
      return magentaSplotches;
    else if (command === 'violet')
      return violetSplotches;
    return null;
  }

  changeVelocityBy(deltaVelocityX: number, deltaVelocityY: number, now: number) {
    var secondsPassed = (now - this.timeStart) / 1000;
    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalThrust(now));

    var newVelocityX = velocityX * deltaVelocityX;
    var newVelocityY = velocityY * deltaVelocityY;

    if (Math.abs(newVelocityX) < 0.0008)
      newVelocityX = 0;
    if (Math.abs(newVelocityY) < 0.0008)
      newVelocityY = 0;

    if (newVelocityX == 0 && newVelocityY == 0 && newVelocityX == this.velocityX && newVelocityY == this.velocityY)
      return;

    this.changeVelocity(newVelocityX, newVelocityY, now);
  }

  dropPaint(command: string, params: string): any {
    let splats: SplatSprites = this.getSplats(command);
    var droneCenterX: number = this.x + this.width / 2;
    var droneCenterY: number = this.y + this.height / 2;
    const leftRightAdjust: number = 40;
    const upDownAdjust: number = 20;
    var yAdjust: number = Math.random() * upDownAdjust + 15;
    var xAdjust: number = Math.random() * leftRightAdjust - leftRightAdjust / 2;
    splats.sprites.push(new SpriteProxy(0, droneCenterX - splats.spriteWidth / 2, droneCenterY - splats.spriteHeight / 2));
    let random: number = Math.random() * 100;
    if (random < 15) {
      splats.longDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.longDrips.spriteWidth / 2 + xAdjust, droneCenterY - yAdjust));
    }
    else if (random < 30) {
      splats.shortDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.longDrips.spriteWidth / 2 + xAdjust, droneCenterY - yAdjust));
      yAdjust = Math.random() * upDownAdjust;
      xAdjust = Math.random() * leftRightAdjust - leftRightAdjust / 2;
      splats.mediumDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.longDrips.spriteWidth / 2 + xAdjust, droneCenterY - yAdjust));
    }
    else if (random < 65) { // 35%
      splats.mediumDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.longDrips.spriteWidth / 2 + xAdjust, droneCenterY - yAdjust));
    }
    else if (random < 85) { // 20%
      splats.shortDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.longDrips.spriteWidth / 2 + xAdjust, droneCenterY - yAdjust));
    }

    splatSoundEffect.play();
  }

  // Picks a sequence of random numbers between 0 and maxBounds. pickCount is the number of random numbers selected.
  pick(pickCount: number, maxBounds: number): number[] {
    let result: number[] = [];
    let attempts: number = 0;
    while (result.length < pickCount && pickCount < maxBounds && attempts < 1000) {
      attempts++;
      let thisPick: number = Math.floor(Math.random() * maxBounds);
      if (result.indexOf(thisPick) === -1)
        result.push(thisPick);
    }
    return result;
  }


  matches(matchData: string): boolean {
    return this.userId == matchData;
  }

  getMs(durationSeconds: string): number {
    if (durationSeconds === '')
      durationSeconds = '1';

    return +durationSeconds * 1000;
  }

  checkFrameIndexAfterThrust(durationMs: number) {
    //const DroneFrameCheckOffsetMs: number = 100;
    //setTimeout(this.setFrameFromThrust.bind(this), durationMs + DroneFrameCheckOffsetMs);
  }

  droneDown(durationSeconds: string): any {
    let now: number = performance.now();
    if (this.downThrustOffTime < now)
      this.downThrustOnTime = now;
    else {
      // Down thrusters already engaged. Keep existing this.downThrustOnTime.
    }
    this.changingDirection(now);
    var durationMs: number = this.getMs(durationSeconds);
    this.downThrustOffTime = now + durationMs;
    this.wasThrustingDown = true;
    this.checkFrameIndexAfterThrust(durationMs);
  }

  droneUp(durationSeconds: string): any {
    let now: number = performance.now();
    if (this.upThrustOffTime < now)
      this.upThrustOnTime = now;
    else {
      // Up thrusters already engaged. Keep existing this.upThrustOnTime.
    }
    this.changingDirection(now);
    var durationMs: number = this.getMs(durationSeconds);
    this.upThrustOffTime = now + durationMs;
    this.wasThrustingUp = true;
    this.checkFrameIndexAfterThrust(durationMs);
  }

  droneLeft(durationSeconds: string): any {
    let now: number = performance.now();
    if (this.leftThrustOffTime < now)
      this.leftThrustOnTime = now;
    else {
      // Left thrusters already engaged. Keep existing this.leftThrustOnTime.
    }
    this.changingDirection(now);
    var durationMs: number = this.getMs(durationSeconds);
    this.leftThrustOffTime = now + durationMs;
    this.wasThrustingLeft = true;
    this.checkFrameIndexAfterThrust(durationMs);
  }

  droneRight(durationSeconds: string): any {
    let now: number = performance.now();
    if (this.rightThrustOffTime < now)
      this.rightThrustOnTime = now;
    else {
      // Right thrusters already engaged. Keep existing this.rightThrustOnTime.
    }
    this.changingDirection(now);
    var durationMs: number = this.getMs(durationSeconds);
    this.rightThrustOffTime = now + durationMs;
    this.wasThrustingRight = true;
    this.checkFrameIndexAfterThrust(durationMs);
  }
}
