// TODO: Consider refactoring this to a Zap class or a SoundEffects class.
const zapSoundEffects: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
const numZapSoundEffects: number = 5;
let splatSoundEffect: HTMLAudioElement;

function loadSoundEffects() {
  if (loadCopyrightedContent) {
    for (var i = 0; i < numZapSoundEffects; i++) {
      zapSoundEffects.push(new Audio(Folders.assets + `Sound Effects/ElectricZap${i}.wav`))
    }
    splatSoundEffect = new Audio(Folders.assets + 'Sound Effects/Splat.mp3');
  }
}

function playZap() {
  let zapIndex: number = Math.floor(Math.random() * numZapSoundEffects);
  zapSoundEffects[zapIndex].play();
}


const dronePathExtension = 18;

class GravityWell {
  // TODO: Immunity needs to last about 3 seconds
  //! Need to sync up the two portals. Expiration date settings seems to be failing.
  //! Weird flicker sometimes.

  immuneSprites: Array<SpriteProxy> = [];

  addImmune(sprite: SpriteProxy) {
    this.immuneSprites.push(sprite);
  }

  isImmuneTo(sprite: SpriteProxy): boolean {
    for (let i = 0; i < this.immuneSprites.length; i++) {
      if (this.immuneSprites[i] === sprite)
        return true;
    }
    return false;
  }

  connectedPortal: GravityWell = undefined;

  isPortal(): boolean {
    return this.connectedPortal !== undefined;
  }

  isPortalTo(newGravityWell: GravityWell, hueShift: number) {
    this.outerCore.hueShift = hueShift;
    this.innerOrb.hueShift = hueShift - 280;
    this.connectedPortal = newGravityWell;
    if (newGravityWell.expireTime > this.expireTime) {
      this.expireTime = newGravityWell.expireTime;
      this.outerCore.expirationDate = newGravityWell.expireTime;
      this.innerOrb.expirationDate = newGravityWell.expireTime;
    }
  }

  constructor(public x: number, public y: number, public owner: SpriteProxy, public expireTime: number,
    public outerCore: ColorShiftingSpriteProxy,
    public innerOrb: ColorShiftingSpriteProxy) {

  }
}

class GravityWells {
  static allGravityWells: Array<GravityWell> = new Array<GravityWell>();

  static getGravityWellCount(sprite: SpriteProxy): number {
    let count: number = 0;
    for (let i = 0; i < GravityWells.allGravityWells.length; i++) {
      const gravityWell: GravityWell = GravityWells.allGravityWells[i];
      if (gravityWell.owner === sprite) {
        count++;
      }
    }
    return count;
  }

  static getFirstGravityWell(sprite: SpriteProxy): GravityWell {
    let count: number = 0;
    for (let i = 0; i < GravityWells.allGravityWells.length; i++) {
      const gravityWell: GravityWell = GravityWells.allGravityWells[i];
      if (gravityWell.owner === sprite) {
        return gravityWell;
      }
    }
    return null;
  }

  static add(x: number, y: number, owner: SpriteProxy, expireTime: number, outerCore: ColorShiftingSpriteProxy, innerOrb: ColorShiftingSpriteProxy): boolean {
    const existingWellCount: number = GravityWells.getGravityWellCount(owner);
    if (existingWellCount >= 2) {
      return false;
    }

    const newGravityWell: GravityWell = new GravityWell(x, y, owner, expireTime, outerCore, innerOrb);

    if (existingWellCount == 1) {
      const existingGravityWell: GravityWell = GravityWells.getFirstGravityWell(owner);
      if (existingGravityWell) {
        newGravityWell.isPortalTo(existingGravityWell, 45);
        existingGravityWell.isPortalTo(newGravityWell, 220);
      }

      // Special case - create a portal!!!!
    }
    GravityWells.allGravityWells.push(newGravityWell);
    return true;
  }

  /**
   * Moves the drone closer to the gravity well. 
   * @param sprite the drone to influence
   * @param nowMS the game time in MS
   * @returns true if the drone is over the gravity well. 
   */
  static influence(sprite: SpriteProxy, nowMS: number): boolean {
    if (GravityWells.allGravityWells.length === 0)
      return false;

    sprite.horizontalThrustOverride = 0;
    sprite.verticalThrustOverride = 0;

    for (let i = 0; i < GravityWells.allGravityWells.length; i++) {
      const gravityWell: GravityWell = GravityWells.allGravityWells[i];
      if (gravityWell.owner === sprite)
        continue;

      if (gravityWell.expireTime <= nowMS)
        continue;

      if (gravityWell.isImmuneTo(sprite))
        continue;

      if (!(sprite instanceof BaseDrone))
        continue;

      const maxDistance: number = 1920;
      const distanceToWell: number = MathEx.clamp(sprite.getDistanceToXY(gravityWell.x - sprite.width / 2, gravityWell.y - sprite.height / 2), 0, maxDistance);
      const minDistanceToWellToExplodeOrTransport: number = 50; // px

      if (distanceToWell < minDistanceToWellToExplodeOrTransport) {
        if (gravityWell.isPortal()) {
          sprite.move(gravityWell.connectedPortal.x - sprite.x - sprite.width / 2, gravityWell.connectedPortal.y - sprite.y - sprite.height / 2);
          gravityWell.connectedPortal.addImmune(sprite);
          sprite.changingDirection(nowMS);
          return;
        }
        return true;
      }

      const deltaX: number = gravityWell.x - sprite.getCenterX();
      const deltaY: number = gravityWell.y - sprite.getCenterY();

      const inverseDistance: number = maxDistance - distanceToWell;
      const inverseMultiplier: number = inverseDistance / maxDistance;
      let targetVector: Vector = new Vector(deltaX, deltaY);
      let normalizedTargetVector: Vector = targetVector.normalize(1);
      const gravityWellForce: number = 3;
      const scaledGravityWellForce: number = inverseMultiplier * gravityWellForce;
      sprite.horizontalThrustOverride += normalizedTargetVector.x * scaledGravityWellForce;
      sprite.verticalThrustOverride += normalizedTargetVector.y * scaledGravityWellForce;



    }

    return false;
  }

  static cleanUpExpired(now: number) {
    for (let i = GravityWells.allGravityWells.length - 1; i >= 0; i--) {
      const gravityWell: GravityWell = GravityWells.allGravityWells[i];
      if (gravityWell.expireTime < now) {
        GravityWells.allGravityWells.splice(i, 1);
      }
    }
  }
}

class BaseDrone extends ColorShiftingSpriteProxy {
  static frameWidth: number = 192;
  static frameHeight: number = 90;
  firstCoinCollection: number;
  mostRecentCoinCollection: number;
  targetPosition: Vector;
  userProfileImage: HTMLImageElement;
  userProfileLoaded: boolean;

  static baseCreateAt(sprites: Sprites, x: number, y: number, now: number,
    createSprite: (spriteArray: Sprites, now: number, createSpriteFunc?: (x: number, y: number, frameCount: number) => SpriteProxy) => SpriteProxy,
    createDrone: (x: number, y: number, frameCount: number) => BaseDrone,
    userId: string, displayName: string, color: string, profileImageUrl: string,
    width: number, height: number): BaseDrone {

    if (!(activeDroneGame instanceof DroneGame))
      return null;

    const myDrone: BaseDrone = createSprite(sprites, now, createDrone) as BaseDrone;
    const hsl: HueSatLight = HueSatLight.fromHex(color);
    if (hsl)
      myDrone.setHueSatBrightness(hsl.hue * 360, 100, 125);

    //if (displayName == "CodeRushed")
    //	myDrone.scale = 1.5;

    myDrone.height = sprites.spriteHeight * myDrone.scale;
    myDrone.width = sprites.spriteWidth * myDrone.scale;
    myDrone.displayName = displayName;
    myDrone.profileImageUrl = profileImageUrl;
    const initialBurnTime = 800;
    if (x < 960)
      myDrone.rightThrustOffTime = now + initialBurnTime;
    else
      myDrone.leftThrustOffTime = now + initialBurnTime;

    if (y > 540)
      myDrone.velocityY = -2;
    else
      myDrone.velocityY = 1;

    myDrone.userId = userId;
    if (color === '')
      color = '#49357b';
    myDrone.color = color;

    return myDrone;
  }

  coinCount = 0;
  health = 4;
  meteor: Meteor;
  displayName: string;
  profileImageUrl: string;
  userId: string;
  backgroundColor = '#fff';
  outlineColor = '#888';
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
  sparkFrameInterval = 20;

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

  constructor(startingFrameNumber: number, x: number, y: number) {
    super(startingFrameNumber, new Vector(x, y));
    this.fadeOnDestroy = false;
    const now: number = performance.now();
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

  static create(x: number, y: number, frameCount: number): BaseDrone {
    return new BaseDrone(Random.intMax(frameCount), x, y);
  }

  hitWall(now: number) {
    if (this.health > 1) {
      const minTimeBetweenExplosions = 250;
      if (!this.sparkCreationTime || now - this.sparkCreationTime > minTimeBetweenExplosions) {
        this.health--;
        playZap();
      }
      if (!(activeDroneGame instanceof DroneGame))
        return;

      switch (Math.floor(Math.random() * 8)) {
        case 0:
          this.setSparks(activeDroneGame.downAndRightSparks);
          break;
        case 1:
          this.setSparks(activeDroneGame.downAndLeftSparks);
          break;
        case 2:
          this.setSparks(activeDroneGame.left1Sparks);
          break;
        case 3:
          this.setSparks(activeDroneGame.left2Sparks);
          break;
        case 4:
          this.setSparks(activeDroneGame.right1Sparks);
          break;
        case 5:
          this.setSparks(activeDroneGame.right2Sparks);
          break;
        case 6:
          this.setSparks(activeDroneGame.upAndRightSparks);
          break;
        case 7:
          this.setSparks(activeDroneGame.upAndLeftSparks);
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
    const velocityDegradeFactor = 0.3;
    this.changeVelocity(this.velocityX + meteor.velocityX * velocityDegradeFactor,
      this.velocityY + meteor.velocityY * velocityDegradeFactor, now);
    // HACK: Consider using Physics to calculate thrust durations to counteract momentum transfer of meteor catch (so drone returns to original catching position after a catch).
    const thrustDuration: number = Math.max(meteor.velocityY) / 5;
    if (meteor.velocityY > 0)
      this.droneUp(thrustDuration.toString());
    else if (meteor.velocityY < 0)
      this.droneDown(thrustDuration.toString());
  }

  selfDestruct() {
    this.isRemoving = true;
    if (!(activeDroneGame instanceof DroneGame))
      return;
    activeDroneGame.allDrones.destroy(this.userId, addDroneExplosion);
  }

  updatePosition(now: number) {
    if (GravityWells.influence(this, now)) {
      this.smokeIsOn = false;
      this.selfDestruct();
      return;
    }
    if (this.smokeIsOn) {
      if (now > this.smokeEmitterOffTime)
        this.smokeIsOn = false;
      else
        this.releaseSmokeIfNecessary(now);
    }

    this.storeLastPosition();
    const secondsPassed = (now - this.physicsTimeStart) / 1000;

    const hAccel = this.getHorizontalThrust(now);
    const vAccel = this.getVerticalThrust(now);

    this.clearTargetPositionIfWeHaveArrived();

    this.updateFrameIndex(now, hAccel, vAccel);
    const xDisplacement = Physics.getDisplacementMeters(secondsPassed, this.velocityX, hAccel);

    const justTurnedOffLeftTruster: boolean = this.wasThrustingLeft && this.leftThrustOffTime <= now;
    const justTurnedOffRightTruster: boolean = this.wasThrustingRight && this.rightThrustOffTime <= now;

    if (justTurnedOffLeftTruster)
      this.wasThrustingLeft = false;

    if (justTurnedOffRightTruster)
      this.wasThrustingRight = false;

    const yDisplacement = Physics.getDisplacementMeters(secondsPassed, this.velocityY, vAccel);

    const justTurnedOffUpTruster: boolean = this.wasThrustingUp && this.upThrustOffTime <= now;
    const justTurnedOffDownTruster: boolean = this.wasThrustingDown && this.downThrustOffTime <= now;

    if (justTurnedOffUpTruster)
      this.wasThrustingUp = false;

    if (justTurnedOffDownTruster)
      this.wasThrustingDown = false;

    const finalVelocityX = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityX, hAccel);
    const finalVelocityY = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityY, vAccel);

    if (justTurnedOffUpTruster || justTurnedOffDownTruster || justTurnedOffLeftTruster || justTurnedOffRightTruster) {
      this.changeVelocity(this.lastVelocityX, this.lastVelocityY, this.lastNow);
    }
    else {
      this.x = this.startX + Physics.metersToPixels(xDisplacement);
      this.y = this.startY + Physics.metersToPixels(yDisplacement);
    }

    if (!this.wasThrustingUp && !this.wasThrustingDown && !this.wasThrustingLeft && !this.wasThrustingRight) {
      const timeSinceLastUpdateMs: number = performance.now() - this.lastUpdateTime;
      if (timeSinceLastUpdateMs > 10) {
        //const decay: number = 1;
        const decay = 0.99;
        this.changeVelocityBy(decay, decay, now);
        this.lastUpdateTime = now;
      }
    }

    this.lastVelocityX = finalVelocityX;
    this.lastVelocityY = finalVelocityY;
    this.lastNow = now;
    if (this.meteor) {
      // TODO: Adjust height based on pitch.
      const pitch: number = Math.floor(this.frameIndex / 10);
      let meteorAdjustY = 0;
      if (pitch === 0)
        meteorAdjustY = -6;
      else if (pitch === 2)
        meteorAdjustY = 8;
      this.meteor.storeLastPosition();
      this.meteor.x = this.x + this.width / 2 - meteorWidth / 2;
      this.meteor.y = this.y + this.height / 2 - meteorHeight + meteorAdjustY;
    }

    if (this.parentSparkSprites != undefined && this.parentSparkSprites != null) {
      const sparkAge: number = now - this.sparkCreationTime;
      if (sparkAge > 1000) {
        console.log('spark out.');
        this.parentSparkSprites = null;
      }
      else {
        this.sparkX = this.x + this.width / 2 - this.parentSparkSprites.originX;
        this.sparkY = this.y + this.height / 2 - this.parentSparkSprites.originY;
      }
    }
  }

  clearTargetPositionIfWeHaveArrived() {
    if (!this.targetPosition)
      return;

    const minVelocityToKeepTrying = 0.2;

    const deltaX: number = this.getTargetDeltaX();
    const deltaY: number = this.getTargetDeltaY();
    if (!this.closeEnoughToShutDown(deltaX) || !this.closeEnoughToShutDown(deltaY))
      return;

    if (Math.abs(this.velocityX) < minVelocityToKeepTrying && Math.abs(this.velocityY) < minVelocityToKeepTrying) {
      this.targetPosition = null;
    }
  }

  setSparks(parentSparkSprites: Sprites): any {
    this.parentSparkSprites = parentSparkSprites;
    this.sparkCreationTime = performance.now();
    this.sparkFrameIndex = 0;
    this.sparkX = this.x + this.width / 2 - this.parentSparkSprites.originX;
    this.sparkY = this.y + this.height / 2 - this.parentSparkSprites.originY;
  }


  updateFrameIndex(now: number, hAccel: number, vAccel: number): any {
    let pitchIndex = 0;
    let rollIndex = 0;
    pitchIndex = Math.sign(vAccel) + 1;
    const highRollSwitchMs = 300;
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
    const segmentStartIndex: number = Sprites.GetSegmentStartIndex(this.frameIndex, 0, 2);
    const newFrameIndex: number = pitchIndex * 10 + rollIndex;
    const newSegmentStartIndex: number = Sprites.GetSegmentStartIndex(newFrameIndex, 0, 2);
    if (segmentStartIndex !== newSegmentStartIndex)
      this.frameIndex = newFrameIndex;
  }

  HorizontalThrust = 2;
  VerticalThrust = 2;

  private getTargetDeltaX(): number {
    // TODO: Check - no scaling??? Seems like it should use the Drone's scaled width
    return this.targetPosition.x - (this.x + BaseDrone.frameWidth / 2);
  }

  private getTargetDeltaY(): number {
    // TODO: Check - no scaling???
    return this.targetPosition.y - (this.y + BaseDrone.frameHeight / 2);
  }

  private getProportionalThrust(delta: number, maxDistance: number) {
    const maxThrust = 25;
    let thrust: number;
    if (!this.closeEnoughToShutDown(delta)) {
      thrust = Math.sign(delta) * maxThrust * Math.pow(Math.abs(delta) / maxDistance, 2);
    }
    return thrust;
  }

  private closeEnoughToShutDown(delta: number) {
    const minThresholdForThrust = 20; // pixels

    return Math.abs(delta) <= minThresholdForThrust;
  }

  getHorizontalThrust(now: number): number {
    if (this.targetPosition) {
      const maxDistance = 1920;
      const deltaX: number = this.getTargetDeltaX();
      const thrust: number = this.getProportionalThrust(deltaX, maxDistance);
      if (thrust)
        return thrust;
    }
    let thrust = 0;
    if (this.rightThrustOffTime > now)
      thrust += this.HorizontalThrust;
    if (this.leftThrustOffTime > now)
      thrust -= this.HorizontalThrust;

    if (this.horizontalThrustOverride !== undefined)
      thrust += this.horizontalThrustOverride;

    return thrust;
  }

  getVerticalThrust(now: number): number {
    if (this.targetPosition) {
      const maxDistance = 1080;
      const deltaY: number = this.getTargetDeltaY();
      const thrust: number = this.getProportionalThrust(deltaY, maxDistance);
      if (thrust)
        return thrust;
    }
    let thrust = 0;
    if (this.upThrustOffTime > now)
      thrust -= this.VerticalThrust;
    if (this.downThrustOffTime > now)
      thrust += this.VerticalThrust;
    if (this.verticalThrustOverride !== undefined)
      thrust += this.verticalThrustOverride;

    return thrust;
  }

  drawAdornments(context: CanvasRenderingContext2D, now: number): void {
    if (!(activeDroneGame instanceof DroneGame))
      return;

    this.drawUserName(context);
    this.drawCoinsCollected(context, now);

    if (this.parentSparkSprites) {
      const msPassed = now - this.lastTimeWeAdvancedTheSparksFrame;
      if (msPassed > this.sparkFrameInterval) {
        this.lastTimeWeAdvancedTheSparksFrame = now;
        this.sparkFrameIndex++;
        if (this.sparkFrameIndex >= this.parentSparkSprites.baseAnimation.frameCount)
          this.parentSparkSprites = null;
      }

      if (this.parentSparkSprites) {
        this.parentSparkSprites.baseAnimation.frameIndex = this.sparkFrameIndex;
        this.parentSparkSprites.baseAnimation.draw(context, this.sparkX, this.sparkY, this.scale);
      }
    }

  }

  private centerTextInRect(context: CanvasRenderingContext2D, text: string, yTop: number, fontSize: number, alpha: number) {
    let centerX: number = this.getCenterX();
    context.textBaseline = 'top';
    context.font = fontSize + 'px Arial';
    context.textAlign = 'center';
    const horizontalTextPadding: number = 4;
    let size = context.measureText(text);
    let textWidth: number = size.width + horizontalTextPadding;
    let halfWidth: number = textWidth / 2;
    context.fillStyle = this.backgroundColor;
    const outlineSize: number = 3;
    context.globalAlpha = 0.7 * alpha;
    context.fillRect(centerX - halfWidth - outlineSize, yTop - outlineSize, textWidth + outlineSize * 2, fontSize + outlineSize * 2);
    context.globalAlpha = 1 * alpha;
    context.strokeStyle = this.outlineColor;
    context.lineWidth = 2;
    context.strokeRect(centerX - halfWidth - outlineSize, yTop - outlineSize, textWidth + outlineSize * 2, fontSize + outlineSize * 2);
    context.fillStyle = this.color;
    context.fillText(text, centerX, yTop);
    context.globalAlpha = 1;
  }

  getCenterX(): number {
    return this.x + this.width / 2;
  }

  getCenterY(): number {
    return this.y + this.height / 2;
  }

  static readonly coinFadeTime: number = 0.5;
  static readonly coinDuration: number = 2.5;

  private drawCoinsCollected(context: CanvasRenderingContext2D, now: number) {
    var secondsSinceFirstCollection: number = (now - this.firstCoinCollection || 0) / 1000;
    var secondsSinceMostRecentCollection: number = (now - this.mostRecentCoinCollection || 0) / 1000;

    if (secondsSinceMostRecentCollection > (BaseDrone.coinFadeTime * 2 + BaseDrone.coinDuration))
      return;

    var alpha: number = 1;

    if (secondsSinceFirstCollection < BaseDrone.coinFadeTime) {
      // Fade in...
      alpha = secondsSinceFirstCollection / BaseDrone.coinFadeTime;
    }

    if (secondsSinceMostRecentCollection > BaseDrone.coinFadeTime + BaseDrone.coinDuration) {
      alpha = 1 - (secondsSinceMostRecentCollection - (BaseDrone.coinFadeTime + BaseDrone.coinDuration)) / BaseDrone.coinFadeTime;
    }

    const fontSize: number = 14;
    const offsetY: number = 25;
    this.centerTextInRect(context, 'Coins: ' + this.coinCount.toString(), this.y + offsetY, fontSize, alpha);
  }

  private drawUserName(context: CanvasRenderingContext2D) {
    const fontSize: number = 14 * this.scale;
    let yTop: number = this.y + this.height * 0.8;
    this.centerTextInRect(context, this.displayName, yTop, fontSize, 1);
  }

  getSplats(command: string): SplatSprites {
    if (!(activeDroneGame instanceof DroneGame))
      return;
    if (command === 'red')
      return activeDroneGame.redSplats;
    else if (command === 'black')
      return activeDroneGame.blackSplats;
    else if (command === 'white')
      return activeDroneGame.whiteSplats;
    else if (command === 'orange')
      return activeDroneGame.orangeSplats;
    else if (command === 'amber')
      return activeDroneGame.amberSplats;
    else if (command === 'yellow')
      return activeDroneGame.yellowSplats;
    else if (command === 'green')
      return activeDroneGame.greenSplats;
    else if (command === 'blue')
      return activeDroneGame.blueSplats;
    else if (command === 'cyan')
      return activeDroneGame.cyanSplats;
    else if (command === 'indigo')
      return activeDroneGame.indigoSplats;
    else if (command === 'magenta')
      return activeDroneGame.magentaSplats;
    else if (command === 'violet')
      return activeDroneGame.violetSplats;
    return null;
  }

  changeVelocityBy(deltaVelocityX: number, deltaVelocityY: number, now: number) {
    const secondsPassed = (now - this.physicsTimeStart) / 1000;
    const velocityX = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
    const velocityY = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityY, this.getVerticalThrust(now));

    let newVelocityX = velocityX * deltaVelocityX;
    let newVelocityY = velocityY * deltaVelocityY;

    if (Math.abs(newVelocityX) < 0.0008)
      newVelocityX = 0;
    if (Math.abs(newVelocityY) < 0.0008)
      newVelocityY = 0;

    if (newVelocityX === 0 && newVelocityY === 0 && newVelocityX === this.velocityX && newVelocityY === this.velocityY)
      return;

    this.changeVelocity(newVelocityX, newVelocityY, now);
  }

  dropPaint(command: string, params: string): any {
    const splats: SplatSprites = this.getSplats(command);
    const droneCenterX: number = this.getCenterX();
    const droneCenterY: number = this.getCenterY();
    const leftRightAdjust = 40;
    const upDownAdjust = 20;
    let yAdjust: number = Math.random() * upDownAdjust + 15;
    let xAdjust: number = Math.random() * leftRightAdjust - leftRightAdjust / 2;
    const paintLifeSpan: number = 15 * 1000;
    splats.spriteProxies.push(new SpriteProxy(0, droneCenterX - splats.spriteWidth / 2, droneCenterY - splats.spriteHeight / 2, paintLifeSpan));
    const yPos: number = droneCenterY - yAdjust;
    const random: number = Math.random() * 100;
    if (random < 15) {
      splats.longDrips.spriteProxies.push(new SpriteProxy(0, droneCenterX - splats.longDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
    }
    else if (random < 30) {
      splats.shortDrips.spriteProxies.push(new SpriteProxy(0, droneCenterX - splats.shortDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
      yAdjust = Math.random() * upDownAdjust;
      xAdjust = Math.random() * leftRightAdjust - leftRightAdjust / 2;
      splats.mediumDrips.spriteProxies.push(new SpriteProxy(0, droneCenterX - splats.mediumDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
    }
    else if (random < 65) { // 35%
      splats.mediumDrips.spriteProxies.push(new SpriteProxy(0, droneCenterX - splats.mediumDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
    }
    else if (random < 85) { // 20%
      splats.shortDrips.spriteProxies.push(new SpriteProxy(0, droneCenterX - splats.shortDrips.spriteWidth / 2 + xAdjust, yPos, paintLifeSpan));
    }

    if (loadCopyrightedContent) {
      splatSoundEffect.play();
    }
  }

  // Picks a sequence of random numbers between 0 and maxBounds. pickCount is the number of random numbers selected.
  pick(pickCount: number, maxBounds: number): number[] {
    const result: number[] = [];
    let attempts = 0;
    while (result.length < pickCount && pickCount < maxBounds && attempts < 1000) {
      attempts++;
      const thisPick: number = Math.floor(Math.random() * maxBounds);
      if (result.indexOf(thisPick) === -1)
        result.push(thisPick);
    }
    return result;
  }


  matches(matchData: any): boolean {
    return this.userId === matchData;
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

  droneDown(durationSeconds: string): void {
    this.clearTargeting();
    const now: number = performance.now();
    if (this.downThrustOffTime < now)
      this.downThrustOnTime = now;
    else {
      // Down thrusters already engaged. Keep existing this.downThrustOnTime.
    }
    this.changingDirection(now);
    const durationMs: number = this.getMs(durationSeconds);
    this.downThrustOffTime = now + durationMs;
    this.wasThrustingDown = true;
    this.checkFrameIndexAfterThrust(durationMs);
  }

  droneUp(durationSeconds: string): void {
    this.clearTargeting();
    const now: number = performance.now();
    if (this.upThrustOffTime < now)
      this.upThrustOnTime = now;
    else {
      // Up thrusters already engaged. Keep existing this.upThrustOnTime.
    }
    this.changingDirection(now);
    const durationMs: number = this.getMs(durationSeconds);
    this.upThrustOffTime = now + durationMs;
    this.wasThrustingUp = true;
    this.checkFrameIndexAfterThrust(durationMs);
  }

  clearTargeting() {
    this.targetPosition = null;
  }

  flyTo(x: number, y: number): void {
    const now: number = performance.now();
    this.changingDirection(now);
    this.targetPosition = new Vector(x, y);
    console.log(`flyTo (${x}, ${y})`);
    // TODO: Fly to the specified location.
  }

  droneLeft(durationSeconds: string): void {
    this.clearTargeting();
    const now: number = performance.now();
    if (this.leftThrustOffTime < now)
      this.leftThrustOnTime = now;
    else {
      // Left thrusters already engaged. Keep existing this.leftThrustOnTime.
    }
    this.changingDirection(now);
    const durationMs: number = this.getMs(durationSeconds);
    this.leftThrustOffTime = now + durationMs;
    this.wasThrustingLeft = true;
    this.checkFrameIndexAfterThrust(durationMs);
  }

  droneRight(durationSeconds: string): void {
    this.clearTargeting();
    const now: number = performance.now();
    if (this.rightThrustOffTime < now)
      this.rightThrustOnTime = now;
    else {
      // Right thrusters already engaged. Keep existing this.rightThrustOnTime.
    }
    this.changingDirection(now);
    const durationMs: number = this.getMs(durationSeconds);
    this.rightThrustOffTime = now + durationMs;
    this.wasThrustingRight = true;
    this.checkFrameIndexAfterThrust(durationMs);
  }

  isHitBy(thisSprite: SpriteProxy): boolean {
    if (this.meteor || thisSprite.owned)
      return false;
    if (thisSprite === this.meteorJustThrown) {
      const minAirTimeForCatch = 500;
      if (performance.now() - this.meteorThrowTime < minAirTimeForCatch)
        return false;
      else
        this.meteorJustThrown = null;
    }
    return super.isHitBy(thisSprite);
  }

  getHalfWidth(): number {
    return this.width / 2;
  }

  getHalfHeight(): number {
    return this.height / 2;
  }

  getDistanceTo(otherSprite: SpriteProxy): number {
    let centerDroneX: number = this.x + this.getHalfWidth();
    let centerDroneY: number = this.y + this.getHalfHeight();

    let centerMeteorX: number = otherSprite.x + otherSprite.getHalfWidth();
    let centerMeteorY: number = otherSprite.y + otherSprite.getHalfHeight();

    let deltaX: number = centerDroneX - centerMeteorX;
    let deltaY: number = centerDroneY - centerMeteorY;

    return Math.sqrt(deltaX * deltaX + deltaY * deltaY);
  }

  removing(): void {
    this.selfDestruct();
  }

  destroying(): void {
    super.destroying();
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

  getFuturePoint(x: number, now: number): FuturePoint {
    const secondsPassed = (now - this.physicsTimeStart) / 1000;

    /* 
     * 
     * this.rightThrustOffTime, this.leftThrustOffTime
     * if thrusters are on, figure out if we cross the given x before thrusters go off. 
     * 
     * If so, then we can incorporate acceleration into the equation.
     * */


    const velocityX = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
    const velocityY = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityY, this.getVerticalThrust(now));

    const centerX: number = this.x + this.width / 2;
    const centerY: number = this.y + this.height / 2;
    const deltaXPixels = x - centerX;
    if (deltaXPixels === 0) {
      return new FuturePoint(x, centerY, now);
    }
    if (Math.sign(deltaXPixels) !== Math.sign(velocityX))
      return null;

    const metersToCrossover: number = Physics.pixelsToMeters(deltaXPixels);

    let secondsToCrossover: number;

    secondsToCrossover = metersToCrossover / velocityX;

    if (this.rightThrustOffTime > now || this.leftThrustOffTime > now) {
      let secondsOfRightThrustRemaining = Infinity;
      let secondsOfLeftThrustRemaining = Infinity;
      if (this.rightThrustOffTime > now)
        secondsOfRightThrustRemaining = (this.rightThrustOffTime - now) / 1000;
      if (this.leftThrustOffTime > now)
        secondsOfLeftThrustRemaining = (this.leftThrustOffTime - now) / 1000;

      const fewestSecondsOfThrust: number = Math.min(secondsOfRightThrustRemaining, secondsOfLeftThrustRemaining);

      const secondsToAcceleratedCrossover: number = Physics.getDropTimeSeconds(metersToCrossover, this.getHorizontalThrust(now), velocityX);

      if (fewestSecondsOfThrust >= secondsToAcceleratedCrossover)
        secondsToCrossover = secondsToAcceleratedCrossover;
      else {
        // TODO: Blend the two predictions?
      }
    }

    //console.log('metersToCrossover: ' + metersToCrossover.toFixed(2) + ', velocityX: ' + velocityX.toFixed(2) + ', secondsToCrossover: ' + secondsToCrossover.toFixed(2));

    const yAtCrossover: number = centerY + Physics.metersToPixels(velocityY * secondsToCrossover);
    if (yAtCrossover < 0 || yAtCrossover > screenHeight)
      return null;

    return new FuturePoint(x, yAtCrossover, now + secondsToCrossover * 1000);
  }

  justCollectedCoins(now: number): void {
    const secondsSinceFirstCollection: number = (now - this.mostRecentCoinCollection || 0) / 1000;

    if (secondsSinceFirstCollection > BaseDrone.coinFadeTime * 2 + BaseDrone.coinDuration) {
      this.firstCoinCollection = now;
      this.mostRecentCoinCollection = now;
    }
    else
      this.mostRecentCoinCollection = now;
  }

  getDistanceFromLastSmokeEmission(centerX: number, centerY: number): number {
    let deltaX: number = this.lastSmokeCenterX - centerX;
    let deltaY: number = this.lastSmokeCenterY - centerY;
    return Math.sqrt(deltaX * deltaX + deltaY * deltaY);
  }

  releaseSmokeIfNecessary(now: number) {
    let centerX: number = this.getCenterX();
    let centerY: number = this.getCenterY();
    let distanceFromLastSmokeEmission: number = this.getDistanceFromLastSmokeEmission(centerX, centerY);
    const minDistanceBetweenEmissions: number = 9;  // px
    const minTimeBetweenEmissionsMs: number = 2500;
    let timeSinceLastEmissionMs: number = now - this.lastSmokeEmissionTime;
    if (distanceFromLastSmokeEmission > minDistanceBetweenEmissions || timeSinceLastEmissionMs > minTimeBetweenEmissionsMs)
      this.releaseSmoke(now, centerX, centerY);
  }

  smokeOff() {
    this.smokeIsOn = false;
  }

  smokeOn(params: string, now: number) {
    let emitterDuration: number = parseInt(params);
    const maxSmokeEmitterDurationSeconds: number = 15;
    if (emitterDuration <= 0 || emitterDuration > maxSmokeEmitterDurationSeconds || isNaN(emitterDuration))
      emitterDuration = maxSmokeEmitterDurationSeconds;

    let smokeSprite: SpriteProxy = this.releaseSmoke(now, this.getCenterX(), this.getCenterY());
    this.smokeIsOn = true;
    this.smokeEmitterOffTime = now + emitterDuration * 1000;
  }

  dropGravityOrb(now: number) {
    const existingWellCount: number = GravityWells.getGravityWellCount(this);
    if (existingWellCount >= 2) {  // Already has 2 gravity wells.
      return;
    }
    let x: number = this.getCenterX();
    let y: number = this.getCenterY();
    const orbAssets: [ColorShiftingSpriteProxy, ColorShiftingSpriteProxy] = this.createGravityOrbAssets(now, x, y);
    GravityWells.add(x, y, this, now + BaseDrone.actualGravityOrbLifetimeMs, orbAssets[0], orbAssets[1]);
  }

  /**
   * Sets the smoke color
   * @param params a comma-separated list of hue (0-360), saturation (0-100), and brightness (0-200). Only hue is 
   * required. Saturation and Brightness are optional.
   */
  setSmokeColor(params: string) {
    if (!params) {
      this.restoreSmokeColorToDefault();
      return;
    }

    const parameters: string[] = params.split(',');
    if (parameters.length > 0) {
      this.smokeHueShift = MathEx.clamp(parseInt(parameters[0]), 0, 360);
      if (parameters.length > 1) {
        this.smokeSaturationPercent = MathEx.clamp(parseInt(parameters[1]), 0, 100);
        if (parameters.length > 2) {
          this.smokeBrightness = MathEx.clamp(parseInt(parameters[2]), 0, 200);
        }
        else {
          this.smokeBrightness = 100;
        }
      }
      else {
        this.smokeSaturationPercent = 100;
        this.smokeBrightness = 100;
      }
    }
    else {
      this.smokeHueShift = MathEx.clamp(parseInt(params), 0, 360);
    }
  }

  static readonly maxSmokeLifetimeSeconds: number = 18;

  setSmokeLifetime(params: string) {
    if (!params) {
      this.smokeLifetimeMs = BaseDrone.defaultSmokeLifetime;
      return;
    }

    const newSmokeLifetime: number = parseFloat(params);
    if (isNaN(newSmokeLifetime)) {
      this.smokeLifetimeMs = BaseDrone.defaultSmokeLifetime;
      return;
    }

    this.smokeLifetimeMs = MathEx.clamp(newSmokeLifetime, 2, BaseDrone.maxSmokeLifetimeSeconds) * 1000;
  }

  restoreSmokeColorToDefault() {
    const hsl: HueSatLight = HueSatLight.fromHex(this.color);
    this.smokeHueShift = hsl.hue * 360;
    this.smokeSaturationPercent = hsl.saturation * 100;
    this.smokeBrightness = hsl.light * 100;
  }

  smokeEmitterOffTime: number;
  lastSmokeCenterX: number;
  lastSmokeCenterY: number;

  smokeHueShift: number = -1;
  smokeSaturationPercent: number = 100;
  smokeBrightness: number = 100;
  static readonly defaultSmokeLifetime: number = 9000;
  smokeLifetimeMs: number = BaseDrone.defaultSmokeLifetime;
  lastSmokeEmissionTime: number;
  smokeIsOn: boolean;

  static readonly gravityOrbVisualLifetimeMs: number = 8500;
  static readonly actualGravityOrbLifetimeMs: number = BaseDrone.gravityOrbVisualLifetimeMs - 2600;

  createGravityOrbAssets(now: number, x: number, y: number): [ColorShiftingSpriteProxy, ColorShiftingSpriteProxy] {
    if (!(activeDroneGame instanceof DroneGame))
      return;

    const hsl: HueSatLight = HueSatLight.fromHex(this.color);

    let innerCore: ColorShiftingSpriteProxy = activeDroneGame.gravityOrbInnerCoreSprites.addShifted(x, y, 0, 0, 100, 100);
    innerCore.rotation = Math.random() * 360;
    innerCore.fadeInTime = 300;
    innerCore.fadeOutTime = 300;
    innerCore.fadeOnDestroy = true;
    innerCore.expirationDate = now + BaseDrone.gravityOrbVisualLifetimeMs;
    innerCore.playToEndOnExpire = true;

    let outerCore: ColorShiftingSpriteProxy = activeDroneGame.gravityOrbOuterRingSprites.addShifted(x, y, 0, hsl.hue * 360, 100, 125);
    outerCore.fadeInTime = 300;
    outerCore.fadeOutTime = 300;
    outerCore.fadeOnDestroy = true;
    outerCore.expirationDate = now + BaseDrone.gravityOrbVisualLifetimeMs;
    outerCore.playToEndOnExpire = true;

    return [outerCore, innerCore];
  }

  private releaseSmoke(now: number, x: number, y: number): SpriteProxy {
    if (!(activeDroneGame instanceof DroneGame))
      return;

    if (this.smokeHueShift < 0) {
      this.restoreSmokeColorToDefault();
    }

    this.lastSmokeEmissionTime = now;
    this.lastSmokeCenterX = x;
    this.lastSmokeCenterY = y;
    let sprite: SpriteProxy = activeDroneGame.smokeSprites.addShifted(this.lastSmokeCenterX, this.lastSmokeCenterY, 0,
      this.smokeHueShift, this.smokeSaturationPercent, this.smokeBrightness);
    sprite.rotation = Math.random() * 360;
    sprite.fadeInTime = 1000;
    sprite.fadeOutTime = Math.min(2 * this.smokeLifetimeMs / 3, this.smokeLifetimeMs - 1000);
    sprite.fadeOnDestroy = true;
    sprite.opacity = 0.6;
    sprite.expirationDate = performance.now() + this.smokeLifetimeMs;
    sprite.scale = 0.5;
    sprite.targetScale = 3 * this.smokeLifetimeMs / (BaseDrone.maxSmokeLifetimeSeconds * 1000);
    sprite.playToEndOnExpire = true;
    return sprite;
  }

  drawUserProfile(context: CanvasRenderingContext2D) {
    if (!this.userProfileImage) {
      this.userProfileImage = new Image();
      this.userProfileImage.src = this.profileImageUrl;
      this.userProfileImage.addEventListener(
        "load",
        () => {
          this.userProfileLoaded = true;
        },
        false
      );
    }
    const iconSize: number = 70;
    const yOffset: number = -iconSize / 2;
    if (this.userProfileLoaded) {
      let centerX: number = this.x + (this.width - iconSize) / 2;
      let imageTop: number = this.y + yOffset;
      context.drawImage(this.userProfileImage, centerX, imageTop, iconSize, iconSize);

      context.strokeStyle = this.color;
      context.lineWidth = 2;
      context.strokeRect(centerX, imageTop, iconSize, iconSize);
    }
  }

  drawBackground(context: CanvasRenderingContext2D, now: number): void {
    this.drawUserProfile(context);
  }
}

//` ![](204DC0A5D26C752B4ED0E8696EBE637B.png;;;0.05664,0.05664)
class Drone extends BaseDrone {
  static createAt(x: number, y: number, now: number,
    createSprite: (spriteArray: Sprites, now: number, createSpriteFunc?: (x: number, y: number, frameCount: number) => SpriteProxy) => SpriteProxy,
    createDrone: (x: number, y: number, frameCount: number) => BaseDrone,
    userId: string, displayName: string, color: string, profileImageUrl: string): BaseDrone {

    if (!(activeDroneGame instanceof DroneGame))
      return null;
    const myDrone: BaseDrone = BaseDrone.baseCreateAt(activeDroneGame.dronesRed,
      x, y, now,
      createSprite,
      createDrone,
      userId, displayName, color, profileImageUrl, 192, 90);
  }

  drawAdornments(context: CanvasRenderingContext2D, now: number): void {
    if (!(activeDroneGame instanceof DroneGame))
      return;

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
    const roll: number = Math.floor((this.frameIndex % 10) / 2);

    const healthIndex: number = pitch * 20 + roll * 4 + 4 - this.health;

    activeDroneGame.droneHealthLights.baseAnimation.drawByIndex(context, this.x / this.scale, this.y / this.scale, healthIndex, this.scale);

    super.drawAdornments(context, now);
  }
}

//` ![](Body21.png;;;0.01613,0.01613)

class Roundie extends BaseDrone {
  static createAt(x: number, y: number, now: number,
    createSprite: (spriteArray: Sprites, now: number, createSpriteFunc?: (x: number, y: number, frameCount: number) => SpriteProxy) => SpriteProxy,
    createDrone: (x: number, y: number, frameCount: number) => BaseDrone,
    userId: string, displayName: string, color: string, profileImageUrl: string): BaseDrone {

    if (!(activeDroneGame instanceof DroneGame))
      return null;

    const myDrone: BaseDrone = BaseDrone.baseCreateAt(activeDroneGame.roundies,
      x, y, now,
      createSprite,
      createDrone,
      userId, displayName, color, profileImageUrl, 562, 424);
  }
}


function addDroneExplosion(drone: SpriteProxy, spriteWidth: number, spriteHeight: number): void {
  const x: number = drone.x + spriteWidth / 2;
  const y: number = drone.y + spriteHeight / 2;
  if (!(activeDroneGame instanceof DroneGame))
    return;
  const thisDroneExplosion: Sprites = activeDroneGame.droneExplosions.allSprites[Math.floor(Math.random() * activeDroneGame.droneExplosions.allSprites.length)];
  const explosion: SpriteProxy = new SpriteProxy(0, x - drone.scale * thisDroneExplosion.spriteWidth / 2, y - drone.scale * thisDroneExplosion.spriteHeight / 2);
  explosion.scale = drone.scale;
  thisDroneExplosion.spriteProxies.push(explosion);
  new Audio(Folders.assets + 'Sound Effects/DroneGoBoom.wav').play();
}