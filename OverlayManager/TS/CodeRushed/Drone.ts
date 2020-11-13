// TODO: Consider refactoring this to a Zap class or a SoundEffects class.
var zapSoundEffects: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
const numZapSoundEffects: number = 5;
var splatSoundEffect: HTMLAudioElement;

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


const dronePathExtension: number = 18;

//` ![](204DC0A5D26C752B4ED0E8696EBE637B.png)
class Drone extends ColorShiftingSpriteProxy {
  static readonly width: number = 192;
  static readonly height: number = 90;
  firstCoinCollection: number;
  mostRecentCoinCollection: number;

  static createAt(x: number, y: number, now: number,
    createSprite: (spriteArray: Sprites, now: number, createSpriteFunc?: (x: number, y: number, frameCount: number) => SpriteProxy) => SpriteProxy, createDrone: (x: number, y: number, frameCount: number) => Drone, userId: string, displayName: string, color: string): any {
    if (!(activeDroneGame instanceof DroneGame))
			return;
		
    let drones: Sprites = activeDroneGame.dronesRed;
		let myDrone: Drone = <Drone>createSprite(drones, now, createDrone);
		let hsl: HueSatLight = HueSatLight.fromHex(color);
		if (hsl)
			myDrone.setHueSatBrightness(hsl.hue * 360, 100, 125);
		//if (displayName == "CodeRushed" || displayName == "wil_bennett")
		//	myDrone.scale = 1.5;
		myDrone.height = drones.spriteHeight * myDrone.scale;
		myDrone.width = drones.spriteWidth * myDrone.scale;
		myDrone.displayName = displayName;
		const initialBurnTime: number = 800;
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
  }
  coinCount: number = 0;
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

	constructor(startingFrameNumber: number, x: number, y: number) {
		super(startingFrameNumber, new Vector(x, y));
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

	static create(x: number, y: number, frameCount: number): Drone {
    return new Drone(Random.intMax(frameCount), x, y);
  }


  hitWall(now: number) {
    if (this.health > 1) {
      const minTimeBetweenExplosions: number = 250;
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
    if (!(activeDroneGame instanceof DroneGame))
      return;
    activeDroneGame.allDrones.destroy(this.userId, addDroneExplosion);
  }

  updatePosition(now: number) {
    this.storeLastPosition();
    const secondsPassed = (now - this.timeStart) / 1000;

    const hAccel = this.getHorizontalThrust(now);
    const vAccel = this.getVerticalThrust(now);

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
      this.meteor.x = this.x + Drone.width * this.scale / 2 - meteorWidth / 2;
			this.meteor.y = this.y + Drone.height * this.scale / 2 - meteorHeight + meteorAdjustY;
    }

    if (this.parentSparkSprites !== undefined) {
      const sparkAge: number = now - this.sparkCreationTime;
      if (sparkAge > 1000) {
        console.log('spark out.');
        this.parentSparkSprites = null;
      }
      else {
				this.sparkX = this.x + Drone.width * this.scale / 2 - this.parentSparkSprites.originX;
				this.sparkY = this.y + Drone.height * this.scale / 2 - this.parentSparkSprites.originY;
      }
    }
  }

  setSparks(parentSparkSprites: Sprites): any {
    this.parentSparkSprites = parentSparkSprites;
    this.sparkCreationTime = performance.now();
    this.sparkFrameIndex = 0;
		this.sparkX = this.x + Drone.width * this.scale / 2 - this.parentSparkSprites.originX;
		this.sparkY = this.y + Drone.height * this.scale / 2 - this.parentSparkSprites.originY;
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
    this.frameIndex = pitchIndex * 10 + rollIndex;
  }

  HorizontalThrust = 2;
  VerticalThrust = 2;

  getHorizontalThrust(now: number): number {
    let thrust = 0;
    if (this.rightThrustOffTime > now)
      thrust += this.HorizontalThrust;
    if (this.leftThrustOffTime > now)
      thrust -= this.HorizontalThrust;
    return thrust;
  }

  getVerticalThrust(now: number): number {
    let thrust = 0;
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
    const roll: number = Math.floor((this.frameIndex % 10) / 2);

    const healthIndex: number = pitch * 20 + roll * 4 + 4 - this.health;

    if (!(activeDroneGame instanceof DroneGame))
      return;
		activeDroneGame.droneHealthLights.baseAnimation.drawByIndex(context, this.x / this.scale, this.y / this.scale, healthIndex, this.scale);

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
    let centerX: number = this.x + this.width / 2;
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

  static readonly coinFadeTime: number = 0.5;
  static readonly coinDuration: number = 2.5;

  private drawCoinsCollected(context: CanvasRenderingContext2D, now: number) {
    var secondsSinceFirstCollection: number = (now - this.firstCoinCollection || 0) / 1000;
    var secondsSinceMostRecentCollection: number = (now - this.mostRecentCoinCollection || 0) / 1000;

    if (secondsSinceMostRecentCollection > (Drone.coinFadeTime * 2 + Drone.coinDuration))
      return;

    var alpha: number = 1;

    if (secondsSinceFirstCollection < Drone.coinFadeTime) {
      // Fade in...
      alpha = secondsSinceFirstCollection / Drone.coinFadeTime;
    }

    if (secondsSinceMostRecentCollection > Drone.coinFadeTime + Drone.coinDuration) {
      alpha = 1 - (secondsSinceMostRecentCollection - (Drone.coinFadeTime + Drone.coinDuration)) / Drone.coinFadeTime;
    }

    const fontSize: number = 14;
    this.centerTextInRect(context, 'Coins: ' + this.coinCount.toString(), this.y, fontSize, alpha);
  }

  private drawUserName(context: CanvasRenderingContext2D) {
    const fontSize: number = 14 * this.scale;
		let yTop: number = this.y + this.height;
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
    const secondsPassed = (now - this.timeStart) / 1000;
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
    const droneCenterX: number = this.x + this.width / 2;
    const droneCenterY: number = this.y + this.height / 2;
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

	getHalfWidth(): number {
		return Drone.width * this.scale / 2;
	}

	getHalfHeight(): number {
		return Drone.height * this.scale / 2;
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
    const secondsPassed = (now - this.timeStart) / 1000;

    /* 
     * 
     * this.rightThrustOffTime, this.leftThrustOffTime
     * if thrusters are on, figure out if we cross the given x before thrusters go off. 
     * 
     * If so, then we can incorporate acceleration into the equation.
     * */


    const velocityX = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
    const velocityY = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityY, this.getVerticalThrust(now));

		const centerX: number = this.x + Drone.width * this.scale / 2;
		const centerY: number = this.y + Drone.height * this.scale / 2;
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

    if (secondsSinceFirstCollection > Drone.coinFadeTime * 2 + Drone.coinDuration) {
      this.firstCoinCollection = now;
      this.mostRecentCoinCollection = now;
    }
    else
      this.mostRecentCoinCollection = now;
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