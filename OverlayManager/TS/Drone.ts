// TODO: Consider refactoring this to a Zap class or a SoundEffects class.
var zapSoundEffects: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
const numZapSoundEffects: number = 5;

function loadZaps() {
  for (var i = 0; i < numZapSoundEffects; i++) {
    zapSoundEffects.push(new Audio(Folders.assets + `Sound Effects/ElectricZap${i}.wav`))
  }
}

function playZap() {
  let zapIndex: number = Math.floor(Math.random() * numZapSoundEffects);
  zapSoundEffects[zapIndex].play();
}


var splatSoundEffect = new Audio(Folders.assets + 'Sound Effects/Splat.mp3');

const droneWidth: number = 192;
const droneHeight: number = 90;
const dronePathExtension: number = 18;
//const droneWidth: number = 128;
//const droneHeight: number = 60;

//` ![](204DC0A5D26C752B4ED0E8696EBE637B.png)

class Drone extends SpriteProxy {
  health: number = 4;
  meteor: Meteor;
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
  lastTimeWeAdvancedTheSparksFrame: number;
  sparkFrameInterval: number = 20;

  private _color: string;
  private lastUpdateTime: number;
  private wasThrustingRight: boolean;
  private wasThrustingUp: boolean;
  private wasThrustingDown: boolean;
  private wasThrustingLeft: boolean;
  private lastVelocityY: number;
  private lastVelocityX: number;
  private lastNow: number;
  meteorJustThrown: Meteor;
  meteorThrowTime: number;
  parentSparkSprites: Sprites;
  sparkFrameIndex: number;
  sparkX: number;
  sparkY: number;
  sparkCreationTime: number;

  constructor(startingFrameNumber: number, public x: number, public y: number) {
    super(startingFrameNumber, x, y);
    this.fadeOnDestroy = false;
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
    this.lastTimeWeAdvancedTheSparksFrame = now;
  }

  hitWall(now: number) {
    if (this.health > 1) {
      const minTimeBetweenExplosions: number = 250;
      if (!this.sparkCreationTime || now - this.sparkCreationTime > minTimeBetweenExplosions) {
        this.health--;
        playZap();
      }
      switch (Math.floor(Math.random() * 8)) {
        case 0:
          this.setSparks(downAndRightSparks);
          break;
        case 1:
          this.setSparks(downAndLeftSparks);
          break;
        case 2:
          this.setSparks(left1Sparks);
          break;
        case 3:
          this.setSparks(left2Sparks);
          break;
        case 4:
          this.setSparks(right1Sparks);
          break;
        case 5:
          this.setSparks(right2Sparks);
          break;
        case 6:
          this.setSparks(upAndRightSparks);
          break;
        case 7:
          this.setSparks(upAndLeftSparks);
          break;
      }
    }
    else {

      this.selfDestruct();
    }
      
  }

  getAlpha(now: number): number {
    return 1;
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

  tossMeteor(velocityXStr: string, velocityYStr: string): any {
    if (!this.meteor)
      return;

    const maxVelocity: number = 50;
    let velocityX: number = +velocityXStr;
    velocityX = (Math.min(Math.abs(velocityX), maxVelocity) * Math.sign(velocityX));

    let velocityY: number = +velocityYStr;
    velocityY = (Math.min(Math.abs(velocityY), maxVelocity) * Math.sign(velocityY));

    let now: number = performance.now();
    this.changingDirection(now);
    this.meteor.changeVelocity(velocityX + this.velocityX, -velocityY + this.velocityY, now);
    this.meteor.owned = false;
    this.meteor.owner = null;
    this.meteorJustThrown = this.meteor;
    this.meteorThrowTime = now;
    this.meteor = null;
  }

  addMeteor(meteor: Meteor, now: number): void {
    this.meteor = meteor;
    meteor.owner = this;
    meteor.owned = true;
    meteor.changingDirection(now);
    const velocityDegradeFactor: number = 0.3;
    this.changeVelocity(this.velocityX + meteor.velocityX * velocityDegradeFactor,
      this.velocityY + meteor.velocityY * velocityDegradeFactor, now);
    // HACK: Consider using Physics to calculate thrust durations to counteract momentum transfer of meteor catch (so drone returns to original catching position after a catch).
    var thrustDuration: number = Math.max(meteor.velocityY) / 5;
    if (meteor.velocityY > 0)
      this.droneUp(thrustDuration.toString());
    else if (meteor.velocityY < 0)
      this.droneDown(thrustDuration.toString());
  }

  selfDestruct() {
    this.isRemoving = true;
    allDrones.destroy(this.userId, addDroneExplosion);
  }

  updatePosition(now: number) {
    this.storeLastPosition();
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
    if (this.meteor) {
      // TODO: Adjust height based on pitch.
      let pitch: number = Math.floor(this.frameIndex / 10);
      let meteorAdjustY: number = 0;
      if (pitch == 0)
        meteorAdjustY = -6;
      else if (pitch == 2)
        meteorAdjustY = 8;
      this.meteor.storeLastPosition();
      this.meteor.x = this.x + droneWidth / 2 - meteorWidth / 2;
      this.meteor.y = this.y + droneHeight / 2 - meteorHeight + meteorAdjustY;
    }

    if (this.parentSparkSprites != undefined) {
      let sparkAge: number = now - this.sparkCreationTime;
      if (sparkAge > 1000) {
        console.log('spark out.');
        this.parentSparkSprites = null;
      }
      else {
        this.sparkX = this.x + droneWidth / 2 - this.parentSparkSprites.originX;
        this.sparkY = this.y + droneHeight / 2 - this.parentSparkSprites.originY;
      }
    }
  }

  setSparks(parentSparkSprites: Sprites): any {
    this.parentSparkSprites = parentSparkSprites;
    this.sparkCreationTime = performance.now();
    this.sparkFrameIndex = 0;
    this.sparkX = this.x + droneWidth / 2 - this.parentSparkSprites.originX;
    this.sparkY = this.y + droneHeight / 2 - this.parentSparkSprites.originY;
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
    let pitch: number;
    if (this.frameIndex < 10) { // Up
      pitch = 0;
    }
    else if (this.frameIndex < 20) { // Flat
      pitch = 1;
    }
    else { // Down 
      pitch = 2;
    }
    let roll: number = Math.floor((this.frameIndex % 10) / 2);

    let healthIndex: number = pitch * 20 + roll * 4 + 4 - this.health;

    droneHealthLights.baseAnimation.drawByIndex(context, this.x, this.y, healthIndex);

    const fontSize: number = 14;
    context.font = fontSize + 'px Arial';

    context.textBaseline = 'top'; //` ![](774083667316C80C98D43F9C370CC1C8.png;;0,68,400,130;0.02510,0.02510)
    context.textAlign = 'center';

    let centerX: number = this.x + this.width / 2;
    let yTop: number = this.y + this.height;
    const horizontalTextPadding: number = 4;
    let size = context.measureText(this.displayName);
    let textWidth: number = size.width + horizontalTextPadding;
    let halfWidth: number = textWidth / 2;
    context.fillStyle = this.backgroundColor;
    const outlineSize: number = 3;
    context.globalAlpha = 0.7;
    context.fillRect(centerX - halfWidth - outlineSize, yTop - outlineSize, textWidth + outlineSize * 2, fontSize + outlineSize * 2);
    context.globalAlpha = 1;

    context.strokeStyle = this.outlineColor;
    context.lineWidth = 2;
    context.strokeRect(centerX - halfWidth - outlineSize, yTop - outlineSize, textWidth + outlineSize * 2, fontSize + outlineSize * 2);


    context.fillStyle = this.color;
    context.fillText(this.displayName, centerX, yTop);

    if (this.parentSparkSprites) {
      var msPassed = now - this.lastTimeWeAdvancedTheSparksFrame;
      if (msPassed > this.sparkFrameInterval) {
        this.lastTimeWeAdvancedTheSparksFrame = now;
        this.sparkFrameIndex++;
        if (this.sparkFrameIndex >= this.parentSparkSprites.baseAnimation.frameCount)
          this.parentSparkSprites = null;
      }

      if (this.parentSparkSprites) {
        this.parentSparkSprites.baseAnimation.frameIndex = this.sparkFrameIndex;
        this.parentSparkSprites.baseAnimation.draw(context, this.sparkX, this.sparkY);
      }
    }
  }

  getSplats(command: string): SplatSprites {
    if (command === 'red')
      return redSplats;
    else if (command === 'black')
      return blackSplats;
    else if (command === 'white')
      return whiteSplats;
    else if (command === 'orange')
      return orangeSplats;
    else if (command === 'amber')
      return amberSplats;
    else if (command === 'yellow')
      return yellowSplats;
    else if (command === 'green')
      return greenSplats;
    else if (command === 'blue')
      return blueSplats;
    else if (command === 'cyan')
      return cyanSplats;
    else if (command === 'indigo')
      return indigoSplats;
    else if (command === 'magenta')
      return magentaSplats;
    else if (command === 'violet')
      return violetSplats;
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
    const paintLifeSpan: number = 15 * 1000;
    splats.sprites.push(new SpriteProxy(0, droneCenterX - splats.spriteWidth / 2, droneCenterY - splats.spriteHeight / 2, paintLifeSpan));
    let yPos: number = droneCenterY - yAdjust;
    let random: number = Math.random() * 100;
    if (random < 15) {
      splats.longDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.longDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
    }
    else if (random < 30) {
      splats.shortDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.shortDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
      yAdjust = Math.random() * upDownAdjust;
      xAdjust = Math.random() * leftRightAdjust - leftRightAdjust / 2;
      splats.mediumDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.mediumDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
    }
    else if (random < 65) { // 35%
      splats.mediumDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.mediumDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
    }
    else if (random < 85) { // 20%
      splats.shortDrips.sprites.push(new SpriteProxy(0, droneCenterX - splats.shortDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
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


  matches(matchData: any): boolean {
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

  isHitBy(thisSprite: SpriteProxy): boolean {
    if (this.meteor || thisSprite.owned)
      return false;
    if (thisSprite == this.meteorJustThrown) {
      const minAirTimeForCatch: number = 500;
      if (performance.now() - this.meteorThrowTime < minAirTimeForCatch)
        return false;
      else
        this.meteorJustThrown = null;
    }
    return super.isHitBy(thisSprite);
  }

  getDistanceTo(otherSprite: SpriteProxy): number {
    let centerDroneX: number = this.x + droneWidth / 2;
    let centerDroneY: number = this.y + droneHeight / 2;

    let centerMeteorX: number = otherSprite.x + meteorWidth / 2;
    let centerMeteorY: number = otherSprite.y + meteorHeight / 2;

    let deltaX: number = centerDroneX - centerMeteorX;
    let deltaY: number = centerDroneY - centerMeteorY;

    return Math.sqrt(deltaX * deltaX + deltaY * deltaY);
  }

  removing(): void {
    console.log('this.selfDestruct();');
    this.selfDestruct();
  }

  destroying(): void {
    if (this.meteor) {
      let now: number = performance.now();
      this.changingDirection(now);
      this.meteor.changeVelocity(this.velocityX, this.velocityY, now);
      this.meteor.owned = false;
      this.meteor.owner = null;
    }
  }

  pathVector(spriteWidth: number, spriteHeight: number): Line {
    return super.pathVector(spriteWidth, spriteHeight).extend(dronePathExtension);
  }
}