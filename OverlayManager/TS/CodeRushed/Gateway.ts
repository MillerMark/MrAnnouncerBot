﻿class Gateways extends Sprites {
	constructor(baseAnimationName, expectedFrameCount, frameInterval: number, animationStyle: AnimationStyle, padFileIndex: boolean = false, hitFloorFunc?, onLoadedFunc?) {
		super(baseAnimationName, expectedFrameCount, frameInterval, animationStyle, padFileIndex, hitFloorFunc, onLoadedFunc);
	}

  releaseDrone(now: number, userId: string, displayName: string, color: string, profileImageUrl: string, gatewayNum: number): any {
		if (!(activeDroneGame instanceof DroneGame))
			return;
		activeDroneGame.allDrones.destroy(userId, addDroneExplosion);
		let foundGateway = this.find(gatewayNum.toString());
		if (foundGateway instanceof Gateway)
      foundGateway.releaseDrone(now, userId, displayName, color, profileImageUrl);
	}
}

class WarpInSprite extends SpriteProxy {
	public userId: string;
	public displayName: string;
  public profileImageUrl: string;
	public color: string;
	createdDrone: boolean = false;

	constructor(startingFrameNumber: number, x: number, y: number) {
		super(startingFrameNumber, x, y);
	}

	cycled(now: number) {
		super.cycled(now);

    if (!this.createdDrone) {
      // TODO: Integrate with Roundie.
      Drone.createAt(this.x - BaseDrone.frameWidth * this.scale / 2, this.y - BaseDrone.frameHeight * this.scale / 2, now,
        this.createSprite.bind(this), BaseDrone.create, this.userId, this.displayName, this.color, this.profileImageUrl);
			this.createdDrone = true;
		}

		this.expirationDate = now;
	}

	createSprite(spriteArray: Sprites, now: number, createSpriteFunc?: (x: number, y: number, frameCount: number) => SpriteProxy): SpriteProxy {
		let x = this.x;
		let y = this.y;

		let newSprite: SpriteProxy;
		if (createSpriteFunc)
			newSprite = createSpriteFunc(x, y, spriteArray.baseAnimation.frameCount);
		else
			newSprite = new SpriteProxy(Random.intMax(spriteArray.baseAnimation.frameCount), x, y);

		spriteArray.spriteProxies.push(newSprite);
		return newSprite;
	}
}

class Gateway extends SpriteProxy {
	static readonly size: number = 195;
	ID: number;

	constructor(startingFrameNumber: number, x: number, y: number) {
		super(startingFrameNumber, x, y);
	}

	matches(matchData: any): boolean {
		return matchData === this.ID.toString();
	}

  releaseDrone(now: number, userId: string, displayName: string, color: string, profileImageUrl: string): any {
		var x: number = this.x + Gateway.size / 2 - BaseDrone.frameWidth * this.scale / 2;
    var y: number = this.y + Gateway.size / 2 - BaseDrone.frameHeight * this.scale / 2;

		let warpInSprite: WarpInSprite = new WarpInSprite(0, x, y);
		warpInSprite.userId = userId;
		warpInSprite.displayName = displayName;
		warpInSprite.profileImageUrl = profileImageUrl;
		warpInSprite.color = color;
		if (!(activeDroneGame instanceof DroneGame))
			return;
		activeDroneGame.warpIns.spriteProxies.push(warpInSprite);
	}

	drawBackground(context: CanvasRenderingContext2D, now: number): void {
		// Create a background for the Gateway number that we will draw later in drawAdornments.
		context.fillStyle = '#000';
		const rectSize: number = 60;
		context.globalAlpha = 0.6;
		let x: number = this.x + (Gateway.size - rectSize) / 2;
		let y: number = this.y + (Gateway.size - rectSize) / 2;
		context.fillRect(x, y, rectSize, rectSize);
		context.globalAlpha = 1;
	}

	drawAdornments(context: CanvasRenderingContext2D, now: number): void {
		let x: number = this.x + Gateway.size / 2;
		let y: number = this.y + Gateway.size / 2;
		context.textAlign = 'center';
		context.textBaseline = 'middle';
		context.font = '20px Arial';
		context.fillStyle = '#fff';
		context.fillText(this.ID.toString(), x, y);
	}
}