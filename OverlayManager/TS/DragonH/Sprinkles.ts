enum Layer {
	Back,
	Front
}

class VelocityDelta {
	constructor(public newVelocity: number, public time: number) {

	}
}

class AnimationCommand {
	constructor(public animationName: string, public velocity: number = 0, public reversed: boolean = false, public frameIntervalOverride: number = undefined) {

	}
}

class QueuedAnimation {
	constructor(public hornBodyPair: HornBodyPair, public reverse: boolean, public timeStart: number, public frameIntervalOverride: number = undefined) {

	}
}


class AnimationPair {
	constructor(public body: SpriteProxy, public horn: SpriteProxy, public frameCount: number) {

	}
}


class HornBodyPair {
	name: string;
	body: Sprites;
	horn: Sprites;
	justAdded: boolean;

	constructor(folder: string, fileBaseName: string, frameCount: number, originX: number, originY: number, hornDeltaX: number, hornDeltaY: number, animationStyle: AnimationStyle = AnimationStyle.Loop, frameRate: number = fps30) {
		this.name = folder;
		this.body = new Sprites(`/Sprinkles/${folder}/${fileBaseName}Body`, frameCount, frameRate, animationStyle, true);
		this.body.originX = originX;
		this.body.originY = originY;
		this.horn = new Sprites(`/Sprinkles/${folder}/${fileBaseName}Horn`, frameCount, frameRate, animationStyle, true);
		this.horn.originX = originX - hornDeltaX;
		this.horn.originY = originY - hornDeltaY;
	}

	draw(context: CanvasRenderingContext2D, now: number): number {
		this.updatePositions(now);
		this.body.draw(context, now);
		return this.horn.draw(context, now);
	}

	updatePositions(now: number): void {
		this.body.updatePositions(now);
		this.horn.updatePositions(now);
	}
}

//Folders.assets = 'GameDev/Assets/DragonH/';

enum SprinkleState {
	Offscreen,
	IdleStand,
	IdleGraze,
	Attack, // Dies, BattleCry, PushUpAttack, ScoopAttack, Stab Attack
	WalkingForwards,
	WalkingBackwards,
	TransitioningToIdle,
	TransitioningToWalk,
	TransitioningToAttack
}

enum Direction {
	FacingLeft,
	FacingRight
}

const Sprinkles_BattleCry = 'BattleCry';
const Sprinkles_PushUpAttack = 'PushUpAttack';
const Sprinkles_ScoopAttack = 'ScoopAttack';
const Sprinkles_StabAttack = 'StabAttack';
const Sprinkles_Walk = 'Walk';
const Sprinkles_WalkBackwards = 'WalkBackwards';
const Sprinkles_Dies = 'Dies';
const Sprinkles_Graze = 'Graze';
const Sprinkles_Idle = 'Idle';
const Sprinkles_IdleToAttack = 'IdleToAttack';
const Sprinkles_AttackToIdle = 'AttackToIdle';
const Sprinkles_IdleToWalk = 'IdleToWalk';
const Sprinkles_WalkToIdle = 'WalkToIdle';


class Sprinkles {
	static readonly hornHueDegreesShiftPerSecond: number = 30;
	static readonly WalkRight: string = 'Walk Right';
	static readonly WalkLeft: string = 'Walk Left';
	static readonly WalkForwards: string = 'Walk Forwards';
	static readonly WalkBackwards: string = 'Walk Backwards';
	static readonly SprinklesBlast: string = 'SprinklesBlast';
	static readonly Flip: string = 'Flip';
	static readonly SprinklesTears: string = 'SprinklesTears';
	static readonly BodyModifier: string = '.Body';
	static readonly HornModifier: string = '.Horn';
	static readonly forwardsBodyHueShiftMaxValue: number = 30;
	static readonly backwardsBodyHueShiftMinValue: number = -30;

	direction: Direction = Direction.FacingLeft;

	private _state: SprinkleState = SprinkleState.Offscreen;

	dragonSharedSounds: SoundManager;

	get state(): SprinkleState {
		return this._state;
	}

	set state(newValue: SprinkleState) {
		if (this._state === newValue)
			return;
		this._state;
		let oldState: SprinkleState = this._state;
		this._state = newValue;
		this.stateChanged(oldState, newValue);
	}
	layer: Layer;
	private _x: number = 200;
	startX: number;
	timeStart: number;
	velocityX: number = 0;
	static WalkingVelocity: number = 6;
	private _alive: boolean;

	get alive(): boolean {
		return this._alive;
	}

	set alive(newValue: boolean) {
		if (this._alive === newValue)
			return;
		this._alive = newValue;
		this.aliveChanged();
	}

	intervalHandle: number;

	static breathInterval: number = 4000;

	aliveChanged(): void {
		if (this.alive) {
			if (this.intervalHandle === undefined)
				this.intervalHandle = setInterval(this.breatheFire.bind(this), Sprinkles.breathInterval);
		}
		else {
			if (this.intervalHandle !== undefined)
				clearInterval(this.intervalHandle);
			this.intervalHandle = undefined;
		}
	}

	get x(): number {
		return this._x;
	}

	set x(newValue: number) {
		this._x = newValue;
		this.setPositionX(this._x);
	}

	private _y: number = 1070;

	get y(): number {
		return this._y;
	}

	set y(newValue: number) {
		this._y = newValue;
		this.setPositionY(this._y);
	}

	static readonly bodyHueShiftDegreesPerSecond: number = 8;
	hornHue: number = 0;
	bodyHue: number = 0;
	bodyHueShiftPerSecond: number = Sprinkles.bodyHueShiftDegreesPerSecond;
	//lastAnimationTimeStart: number = 0;
	parts: Array<HornBodyPair>;
	adornments: SpriteCollection;
	commandQueue: Array<AnimationCommand>;


	constructor() {
		this.startX = this.x;
		this.timeStart = performance.now();
		this.layer = Layer.Back;
		this.parts = new Array<HornBodyPair>();
		this.adornments = new SpriteCollection();
		this.commandQueue = new Array<AnimationCommand>();
		this.loadParts();
	}

	animationQueue: Array<QueuedAnimation> = new Array<QueuedAnimation>();

	queueVelocityChange(velocityX: number, now: number) {
		this.changeVelocity(velocityX, now); // Try immediate..
		//this.velocityQueue.push(new VelocityDelta(velocityX, now));
	}

	private queueAnimation(hornBodyPair: HornBodyPair, reverse: boolean, now: number, frameIntervalOverride: number = undefined) {
		if (!hornBodyPair) {
			debugger;
		}
		this.animationQueue.push(new QueuedAnimation(hornBodyPair, reverse, now, frameIntervalOverride));
	}

	changeVelocity(velocityX: number, now: number) {
		let previousStartX: number = this.startX;

		this.timeStart = now;
		this.velocityX = velocityX;
		this.startX = this.x;
	}

	updatePosition(now: number) {
		if (this.velocityX != 0) {
			var secondsPassed = (now - this.timeStart) / 1000;
			var xDisplacement = Physics.getDisplacementMeters(secondsPassed, this.velocityX, 0);
			this.x = this.startX + Physics.metersToPixels(xDisplacement);
		}
	}

	dequeueAnimations(now: number): boolean {
		let dequeuedAnimations: boolean = false;
		if (this.animationQueue.length > 0) {
			let queuedAnimation: QueuedAnimation = this.animationQueue.shift();
			while (!queuedAnimation || !queuedAnimation.hornBodyPair) {
				queuedAnimation = this.animationQueue.shift();
			}
			if (queuedAnimation && queuedAnimation.hornBodyPair)
				console.log(`Starting animation: ${queuedAnimation.hornBodyPair.name} at ${new Date()}`);
			this.addAnimation(queuedAnimation.hornBodyPair, now, queuedAnimation.reverse, queuedAnimation.frameIntervalOverride);
			dequeuedAnimations = true;
		}

		return dequeuedAnimations;
	}

	static readonly idleFrameInterval: number = fps20;
	static readonly idleLowSpeedFrameInterval: number = Sprinkles.idleFrameInterval * 1.2;
	static readonly idleHighSpeedFrameInterval: number = Sprinkles.idleFrameInterval * 0.8;

	loadParts(): any {
		Folders.assets = 'GameDev/Assets/DragonH/';
		const originX: number = 123;
		const originY: number = 659;
		this.parts.push(new HornBodyPair(Sprinkles_BattleCry, 'BattleCry - ', 139, originX, originY, 0, 55));
		this.parts.push(new HornBodyPair(Sprinkles_Dies, 'Dies - ', 160, originX + 40, originY + 23, 0, 51, AnimationStyle.SequentialStop));
		this.parts.push(new HornBodyPair(Sprinkles_Graze, 'Graze - ', 301, originX + 25, originY + 7, 0, 231));
		this.parts.push(new HornBodyPair(Sprinkles_Idle, 'Idle', 201, originX - 6, originY + 14, 0, 225, AnimationStyle.Loop, Sprinkles.idleFrameInterval));
		this.parts.push(new HornBodyPair(Sprinkles_IdleToAttack, 'IdleToAttack - ', 21, originX - 3, originY + 12, 0, 67, AnimationStyle.SequentialStop));
		this.parts.push(new HornBodyPair(Sprinkles_IdleToWalk, 'IdleToWalk - ', 21, originX - 4, originY + 9, 0, 232, AnimationStyle.SequentialStop));
		this.parts.push(new HornBodyPair(Sprinkles_PushUpAttack, 'PushUpAttack - ', 79, originX + 21, originY + 43, 0, 0));
		this.parts.push(new HornBodyPair(Sprinkles_ScoopAttack, 'ScoopAttack - ', 129, originX + 250, originY + 127, 0, 0));
		this.parts.push(new HornBodyPair(Sprinkles_StabAttack, 'StabAttack - ', 79, originX + 113, originY + 3, 0, 39));
		this.parts.push(new HornBodyPair(Sprinkles_Walk, 'Walk - ', 81, originX - 4, originY + 12, 0, 257));
		this.addAdornment('/Sprinkles/FireExhale/FireExhale', 125, originX - 165, originY - 400, 'FireBreath');
		this.addAdornment('/Sprinkles/Idle/IdleSprinkleBlast', 55, originX + 97, originY + 4, Sprinkles.SprinklesBlast);
		this.addAdornment('/Sprinkles/Tears/Tears', 91, originX + -75, originY - 289, Sprinkles.SprinklesTears, AnimationStyle.Loop);
	}

	addAdornment(baseAnimationName: string, frameCount: number, originX: number, originY: number, name: string, animationStyle: AnimationStyle = AnimationStyle.Sequential): Sprites {
		let adornment: Sprites = new Sprites(baseAnimationName, frameCount, fps30, animationStyle, true);
		adornment.originX = originX;
		adornment.originY = originY;
		adornment.name = name;
		this.adornments.add(adornment);
		return adornment;
	}

	hasAnimationsRunning(): boolean {
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			if (hornBodyPair.body.sprites.length > 0)
				return true;
		}
		return false;
	}

	executesCommandNow(commandData: string, now: number): boolean {
		switch (commandData) {
			case Sprinkles.WalkRight:
				return this.walkRight(now);
			case Sprinkles.WalkLeft:
				return this.walkLeft(now);

			case Sprinkles.SprinklesBlast:
				this.addIdleAdornment(commandData);
				return true;
			case Sprinkles.Flip:
				this.flip();
				return true;
			case Sprinkles.SprinklesTears:
				this.addTears(commandData);
				return true;
		}
		return false;
	}

	horizontalScale: number = 1;

	flip(): void {
		if (this.direction === Direction.FacingLeft) {
			this.direction = Direction.FacingRight;
			this.horizontalScale = -1;
		}
		else {
			this.direction = Direction.FacingLeft;
			this.horizontalScale = 1;
		}

		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			for (let j = 0; j < hornBodyPair.body.sprites.length; j++) {
				let sprite: SpriteProxy = hornBodyPair.body.sprites[j];
				sprite.horizontalScale = this.horizontalScale;
			}
			for (let j = 0; j < hornBodyPair.horn.sprites.length; j++) {
				let sprite: SpriteProxy = hornBodyPair.horn.sprites[j];
				sprite.horizontalScale = this.horizontalScale;
			}
		}
		this.adornments.setHorizontalScale(this.horizontalScale);
	}

	private addTears(commandData: string) {
		let sprite: SpriteProxy = this.addIdleAdornment(commandData);
		if (!sprite) 
			return;

		sprite.fadeInTime = 600;
		sprite.fadeOutTime = 1000;

		if (sprite instanceof ColorShiftingSpriteProxy)
		{
			let colorShiftingSprite: ColorShiftingSpriteProxy = <ColorShiftingSpriteProxy>sprite;
			colorShiftingSprite.hueShiftPerSecond = Sprinkles.hornHueDegreesShiftPerSecond;
		}
	}

	changeWalkingDirection(now: number, compareState: SprinkleState, command: string): boolean {
		if (this.state === compareState) {
			this.clearCommandQueue();
			this.addCommand(command);
			this.finishActiveAnimation(now);
			return true;
		}
		return false;
	}

	walkRight(now: number): boolean {
		if (this.direction === Direction.FacingLeft)
			if (this.changeWalkingDirection(now, SprinkleState.WalkingBackwards, Sprinkles.WalkForwards))
				return true;

		if (this.direction === Direction.FacingRight)
			if (this.changeWalkingDirection(now, SprinkleState.WalkingForwards, Sprinkles.WalkBackwards))
				return true;

		return false;
	}

	walkLeft(now: number): boolean {
		if (this.direction === Direction.FacingLeft)
			if (this.changeWalkingDirection(now, SprinkleState.WalkingForwards, Sprinkles.WalkBackwards))
				return true;

		if (this.direction === Direction.FacingRight)
			if (this.changeWalkingDirection(now, SprinkleState.WalkingBackwards, Sprinkles.WalkForwards))
				return true;

		return false;
	}

	executeCommand(commandData: string, now: number): void {
		if (this.executesCommandNow(commandData, now))
			return;
		this.addCommand(commandData);
		if (!this.hasAnimationsRunning())
			this.executeNextCommand(now);
	}

	showQueue() {
		if (!this.commandQueue || this.commandQueue.length <= 0)
			console.log('commandQueue - []');
		else {
			let commands: string = '';
			for (let i = 0; i < this.commandQueue.length; i++) {
				let animationCommand: AnimationCommand = this.commandQueue[i];
				commands += animationCommand.animationName;
				if (i < this.commandQueue.length - 1)
					commands += ', ';
			}
			console.log(`commandQueue - [${commands}]`);
		}
	}

	shouldReverseWalkAnimation(): boolean {
		return this.direction === Direction.FacingRight;
	}

	addCommand(commandData: string, velocity: number = 0, reversed: boolean = false): AnimationCommand {
		let animationCommand: AnimationCommand = new AnimationCommand(commandData, velocity);
		console.log('addCommand: ' + commandData);
		this.commandQueue.push(animationCommand);
		this.showQueue();
		return animationCommand;
	}

	clearAnimations() {
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			hornBodyPair.body.removeAllSprites();
			hornBodyPair.horn.removeAllSprites();
		}
	}

	setPositionY(y: number): any {
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			this.moveAllSpritesToY(hornBodyPair.body, y);
			this.moveAllSpritesToY(hornBodyPair.horn, y);
		}

		for (let i = 0; i < this.adornments.allSprites.length; i++) {
			let sprites: Sprites = this.adornments.allSprites[i];
			this.moveAllSpritesToY(sprites, y);
		}
	}

	moveAllSpritesToY(sprites: Sprites, y: number): void {
		for (let j = 0; j < sprites.sprites.length; j++) {
			sprites[j].y = y - sprites.originY;
		}
	}

	setPositionX(x: number): any {
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			this.moveAllSpritesToX(hornBodyPair.body, x);
			this.moveAllSpritesToX(hornBodyPair.horn, x);
		}
		for (let i = 0; i < this.adornments.allSprites.length; i++) {
			let sprites: Sprites = this.adornments.allSprites[i];
			this.moveAllSpritesToX(sprites, x);
		}
	}

	moveAllSpritesToX(sprites: Sprites, x: number): void {
		for (let j = 0; j < sprites.sprites.length; j++) {
			sprites.sprites[j].x = x - sprites.originX;
		}
	}

	stepForward() {
		this.randomFrameRate(Sprinkles_IdleToWalk, false);
		this.randomFrameRate(Sprinkles_Walk, false).velocity = -Sprinkles.WalkingVelocity;
		this.randomFrameRate(Sprinkles_WalkToIdle, false);
		this.randomFrameRate(Sprinkles_Idle);
	}

	stepBackwards() {
		this.randomFrameRate(Sprinkles_IdleToWalk, false);
		this.randomFrameRate(Sprinkles_WalkBackwards, false).velocity = Sprinkles.WalkingVelocity;
		this.randomFrameRate(Sprinkles_WalkToIdle, false);
		this.randomFrameRate(Sprinkles_Idle);
	}

	doSomethingCreative(now: number) {
		switch (this.state) {
			case SprinkleState.IdleStand:
			case SprinkleState.IdleGraze:
				if (this.lastAnimationCycled && this.lastAnimationCycled.startsWith(Sprinkles_Idle)) {
					if (Random.chancePercent(50))
						this.randomFrameRate(Sprinkles_Idle);
					else if (Random.chancePercent(50))
						this.randomFrameRate(Sprinkles_Graze);
					else if (Random.chancePercent(33))
						this.stepForward();
					else if (Random.chancePercent(50))
						this.stepBackwards();
				}
				else if (!this.lastAnimationCycled || this.lastAnimationCycled.startsWith(Sprinkles_Graze))
					this.randomFrameRate(Sprinkles_Idle);
				break;
		}
	}

	getCurrentHornHueShift(now: number): number {
		let hornSprite: ColorShiftingSpriteProxy = <ColorShiftingSpriteProxy>this.getSprite(Sprinkles_Idle + Sprinkles.HornModifier);
		if (hornSprite === null)
			hornSprite = <ColorShiftingSpriteProxy>this.getSprite(Sprinkles_Walk + Sprinkles.HornModifier);
		if (hornSprite === null)
			hornSprite = <ColorShiftingSpriteProxy>this.getSprite(Sprinkles_WalkBackwards + Sprinkles.HornModifier);
		if (hornSprite !== null)
			return hornSprite.getCurrentHueShift(now);

		return undefined;
	}

	breatheFire(): void {
		if (this.state !== SprinkleState.IdleStand)
			return;
		let fireBreath: Sprites = this.adornments.getSpritesByName('FireBreath');
		if (!fireBreath)
			return;
		let hueShift: number = this.getCurrentHornHueShift(performance.now());
		if (hueShift)
			fireBreath.addShifted(this.x, this.y, 0, hueShift).horizontalScale = this.horizontalScale;
		this.dragonSharedSounds.safePlayMp3('Sprinkles/Breath[6]');
	}

	addIdleAdornment(animationName: string): SpriteProxy {
		if (!this.isIdle())
			return null;
		let sprites: Sprites = this.adornments.getSpritesByName(animationName);
		if (!sprites)
			return null;

		let yOffset: number = 0;
		if (this.state === SprinkleState.WalkingBackwards || this.state === SprinkleState.WalkingForwards)
			yOffset += 70;

		let hueShift: number = this.getCurrentHornHueShift(performance.now());
		let adornment: SpriteProxy;
		if (hueShift)
			adornment = sprites.insertShifted(this.x, this.y + yOffset, 0, hueShift);
		else
			adornment = sprites.insert(this.x, this.y + yOffset, 0);
		adornment.horizontalScale = this.horizontalScale;
		return adornment;
	}

	getSprite(name: string): SpriteProxy {
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			let foundSprite: SpriteProxy = hornBodyPair.body.getSprite(name);
			if (foundSprite != null)
				return foundSprite;

			foundSprite = hornBodyPair.horn.getSprite(name);
			if (foundSprite != null)
				return foundSprite;
		}
		return null;
	}

	private randomFrameRate(animationName: string, allowRandomReverse: boolean = true): AnimationCommand {
		let reverseAnimation: boolean = false;
		if (animationName === Sprinkles.WalkForwards || animationName === Sprinkles.WalkBackwards)
			reverseAnimation = this.shouldReverseWalkAnimation();

		let animationCommand: AnimationCommand = this.addCommand(animationName, 0, reverseAnimation);

		if (Random.chancePercent(10)) {
			animationCommand.frameIntervalOverride = this.lastAnimationFrameInterval;
		}

		if (this.lastAnimationFrameInterval === Sprinkles.idleFrameInterval)
			if (Random.chancePercent(50)) {
				animationCommand.frameIntervalOverride = Sprinkles.idleLowSpeedFrameInterval;
			}
			else {
				animationCommand.frameIntervalOverride = Sprinkles.idleHighSpeedFrameInterval;
			}
		else if (this.lastAnimationFrameInterval === Sprinkles.idleLowSpeedFrameInterval)
			if (Random.chancePercent(50)) {
				animationCommand.frameIntervalOverride = Sprinkles.idleFrameInterval;
			}
			else {
				animationCommand.frameIntervalOverride = Sprinkles.idleHighSpeedFrameInterval;
			}
		else
			if (Random.chancePercent(50)) {
				animationCommand.frameIntervalOverride = Sprinkles.idleFrameInterval;
			}
			else {
				animationCommand.frameIntervalOverride = Sprinkles.idleLowSpeedFrameInterval;
			}

		animationCommand.reversed = reverseAnimation;

		if (allowRandomReverse && Random.chancePercent(50))
			animationCommand.reversed = !animationCommand.reversed;

		if (animationCommand.frameIntervalOverride)
			this.lastAnimationFrameInterval = animationCommand.frameIntervalOverride;

		return animationCommand;
	}

	private hasCommandsInQueue() {
		return this.commandQueue.length > 0;
	}

	getNextCommand(): AnimationCommand {
		let animationCommand: AnimationCommand;
		animationCommand = this.commandQueue.shift()
		if (animationCommand)
			console.log('getNextCommand: ' + animationCommand.animationName);
		this.showQueue();
		return animationCommand;
	}

	clearCommandQueue(): void {
		console.log('Clear command queue');
		this.commandQueue = [];
	}

	executeHighLevelCommand(command: AnimationCommand, now: number): boolean {
		if (!command)
			return true;
		switch (command.animationName) {
			case Sprinkles.WalkForwards:
				this.walkForwards();
				this.executeNextCommand(now);
				return true;
			case Sprinkles.WalkBackwards:
				this.walkBackwards();
				this.executeNextCommand(now);
				return true;
		}
		if (command.animationName === 'Appear Behind') {
			this.alive = true;
			this.x = 2020;
			//this.x = 1220;
			this.queueVelocityChange(0, performance.now());
			this.layer = Layer.Back;
			if (this.direction === Direction.FacingRight)
				this.flip();
			this.addCommand(Sprinkles_Walk, -Sprinkles.WalkingVelocity);
			this.addCommand(Sprinkles_Walk, -Sprinkles.WalkingVelocity);
			this.addCommand(Sprinkles_WalkToIdle, -Sprinkles.WalkingVelocity / 2.0);
			this.addCommand(Sprinkles_Idle);
			this.executeNextCommand(now);
			return true;
		}
		return false;
	}

	getActiveAnimationPair(now: number): AnimationPair {
		for (let i = 0; i < this.parts.length; i++) {
			const hornBodyPair: HornBodyPair = this.parts[i];
			if (hornBodyPair.body.sprites.length > 0) {
				const body: SpriteProxy = hornBodyPair.body.sprites[0];
				const notYetGone: boolean = body.expirationDate && body.stillAlive(now) && !body.fadingOut(now);
				let stillHasFramesBeforeCycleComplete: boolean;
				if (body.animationReverseOverride)
					stillHasFramesBeforeCycleComplete = body.frameIndex > 0;
				else
					stillHasFramesBeforeCycleComplete = body.frameIndex < hornBodyPair.body.baseAnimation.frameCount - 1;

				if (notYetGone || stillHasFramesBeforeCycleComplete) {
					const horn: SpriteProxy = hornBodyPair.horn.sprites[0];
					return new AnimationPair(body, horn, hornBodyPair.horn.baseAnimation.frameCount);
				}
			}
		}
		return null;
	}

	finishActiveAnimation(now: number): void {
		const animationPair: AnimationPair = this.getActiveAnimationPair(now);
		if (!animationPair)
			return;

		const movingForward = !animationPair.body.animationReverseOverride;
		const movingBackwards = !movingForward;
		if (animationPair.body.frameIndex < animationPair.frameCount / 2) {
			if (movingForward) {
				animationPair.body.animationReverseOverride = true;
				animationPair.horn.animationReverseOverride = true;
				this.changeVelocity(-this.velocityX, now);
			}
		}
		else {
			if (movingBackwards) {
				animationPair.body.animationReverseOverride = true;
				animationPair.horn.animationReverseOverride = true;
				this.changeVelocity(-this.velocityX, now);
			}
		}

		if (animationPair.horn.frameIntervalOverride)
			animationPair.horn.frameIntervalOverride *= 0.75;
		else
			animationPair.horn.frameIntervalOverride = fps40;
		if (animationPair.body.frameIntervalOverride)
			animationPair.body.frameIntervalOverride *= 0.75;
		else
			animationPair.body.frameIntervalOverride = fps40;
	}


	executeNextCommand(now: number = -1): void {
		if (now === -1)
			now = performance.now();

		if (!this.hasCommandsInQueue()) {
			this.doSomethingCreative(now);
		}

		let command: AnimationCommand = this.getNextCommand();
		if (!command)
			return;

		console.log('Executing command: ' + command.animationName);

		if (this.executeHighLevelCommand(command, now))
			return;

		this.executeLowLevelCommand(command, now);
	}

	executeLowLevelCommand(command: AnimationCommand, now: number): any {
		if (!command)
			return;
		let reverse: boolean = command.reversed;

		if (command.animationName === Sprinkles.WalkLeft) {
			command.velocity = -Sprinkles.WalkingVelocity;
			if (this.direction == Direction.FacingLeft) {
				command.animationName = Sprinkles_Walk;
			}
			else {
				command.animationName = Sprinkles_WalkBackwards;
			}
		}
		else if (command.animationName === Sprinkles.WalkRight) {
			command.velocity = Sprinkles.WalkingVelocity;
			if (this.direction == Direction.FacingLeft) {
				command.animationName = Sprinkles_WalkBackwards;
			}
			else {
				command.animationName = Sprinkles_Walk;
			}
		}

		switch (command.animationName) {
			case Sprinkles_AttackToIdle:
				this.state = SprinkleState.TransitioningToIdle;
				command.animationName = Sprinkles_IdleToAttack;
				reverse = !reverse;
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_WalkToIdle:
				this.state = SprinkleState.TransitioningToIdle;
				command.animationName = Sprinkles_IdleToWalk;
				reverse = !reverse;
				// TODO: consider slowing down velocity as we transition to idle.
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_PushUpAttack:
			case Sprinkles_ScoopAttack:
			case Sprinkles_BattleCry:
			case Sprinkles_Dies:
			case Sprinkles_StabAttack:
				this.state = SprinkleState.Attack;
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_Walk:
				this.state = SprinkleState.WalkingForwards;
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_WalkBackwards:
				this.state = SprinkleState.WalkingBackwards;
				this.queueVelocityChange(command.velocity, now);
				command.animationName = Sprinkles_Walk;
				reverse = !reverse;
				break;
			case Sprinkles_Graze:
				this.state = SprinkleState.IdleGraze;
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_Idle:
				this.state = SprinkleState.IdleStand;
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_IdleToAttack:
				this.state = SprinkleState.TransitioningToAttack;
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_AttackToIdle:
				this.state = SprinkleState.TransitioningToAttack;
				this.queueVelocityChange(command.velocity, now);
				break;
		}

		console.log(`this.queueAnimation(${command.animationName})`);
		this.queueAnimation(this.getHornBodyPair(command.animationName), reverse, now, command.frameIntervalOverride);
	}

	walkForwards(): void {
		this.transitionToWalk(Sprinkles_Walk, -Sprinkles.WalkingVelocity);
	}

	walkBackwards(): void {
		this.transitionToWalk(Sprinkles_WalkBackwards, Sprinkles.WalkingVelocity);
	}

	transitionToWalk(walkCommand: string, walkingVelocity: number = 0): void {
		switch (this.state) {
			case SprinkleState.TransitioningToAttack:
			case SprinkleState.Attack:
				this.clearCommandQueue()
				this.addCommand(Sprinkles_AttackToIdle);
				this.addCommand(Sprinkles_IdleToWalk, walkingVelocity / 2.0, this.shouldReverseWalkAnimation());
				this.addCommand(walkCommand, walkingVelocity, this.shouldReverseWalkAnimation());
				break;


			case SprinkleState.TransitioningToIdle:
			case SprinkleState.IdleStand:
			case SprinkleState.IdleGraze:
				this.clearCommandQueue()
				this.addCommand(Sprinkles_IdleToWalk, walkingVelocity / 2.0, this.shouldReverseWalkAnimation());
				this.addCommand(walkCommand, walkingVelocity, this.shouldReverseWalkAnimation());
				break;

			default:
				this.clearCommandQueue();
				this.addCommand(walkCommand, walkingVelocity, this.shouldReverseWalkAnimation());
				break;
		}
	}


	private addAnimation(hornBodyPair: HornBodyPair, now: number, reverse: boolean, frameIntervalOverride: number = undefined) {
		if (!hornBodyPair)
			return;
		this.clearAnimations();

		let startingFrameIndex: number = 0;
		if (reverse) {
			startingFrameIndex = hornBodyPair.body.baseAnimation.frameCount - 1;
		}

		// TODO: We could simplify this and make it more flexible so individual sprites can be reversed.
		hornBodyPair.body.baseAnimation.reverse = reverse;
		hornBodyPair.horn.baseAnimation.reverse = reverse;

		let body = hornBodyPair.body.addShifted(this.x, this.y, startingFrameIndex, this.bodyHue);
		body.horizontalScale = this.horizontalScale;
		body.name = hornBodyPair.name + Sprinkles.BodyModifier;
		body.hueShiftPerSecond = this.bodyHueShiftPerSecond;
		body.timeStart = now;
		body.frameIntervalOverride = frameIntervalOverride;
		body.addOnFrameAdvanceCallback(this.onFrameAdvanced.bind(this))
		body.addOnCycleCallback(this.animationCycled.bind(this));

		const horn = hornBodyPair.horn.addShifted(this.x, this.y, startingFrameIndex, this.hornHue); // 30 degrees of hue shift every second.
		horn.horizontalScale = this.horizontalScale;
		horn.name = hornBodyPair.name + Sprinkles.HornModifier;
		horn.addOnCycleCallback(this.animationCycled.bind(this));
		horn.onExpire = this.onSpriteExpire.bind(this)
		horn.hueShiftPerSecond = Sprinkles.hornHueDegreesShiftPerSecond;
		horn.timeStart = now;
		horn.frameIntervalOverride = frameIntervalOverride;

		hornBodyPair.justAdded = true;
		this.newAnimationsAdded = true;
	}

	onFrameAdvanced(sprite: SpriteProxy, returnFrameIndex: number, reverse: boolean, now: number): void {
		if (sprite.name.endsWith(Sprinkles.BodyModifier)) {
			if (sprite instanceof ColorShiftingSpriteProxy) {
				const bodySprite: ColorShiftingSpriteProxy = sprite as ColorShiftingSpriteProxy;
				if (bodySprite.hueShiftPerSecond === 0)
					return;

				const currentHueShift: number = bodySprite.getCurrentHueShift(now);
				const exceededForwardLimit: boolean = bodySprite.hueShiftPerSecond > 0 && currentHueShift > Sprinkles.forwardsBodyHueShiftMaxValue;
				if (exceededForwardLimit) {
					bodySprite.setColorShiftMilestone(now);
					bodySprite.hueShiftPerSecond = -Math.abs(bodySprite.hueShiftPerSecond);
				}
				else {
					const exceededBackwardLimit: boolean = bodySprite.hueShiftPerSecond < 0 && currentHueShift < Sprinkles.backwardsBodyHueShiftMinValue;
					if (exceededBackwardLimit) {
						bodySprite.setColorShiftMilestone(now);
						bodySprite.hueShiftPerSecond = Math.abs(bodySprite.hueShiftPerSecond);
					}
				}
			}
		}
	}

	needToDrawNewAnimations: boolean;

	onSpriteExpire() {
		this.needToDrawNewAnimations = !this.hasAnimationsRunning();
	}

	animationJustCycled: boolean;


	lastAnimationCycled: string;
	lastAnimationFrameInterval: number;

	animationCycled(sprite: SpriteProxy, now: number) {
		this.lastAnimationCycled = sprite.name;

		if (sprite.frameIntervalOverride)
			this.lastAnimationFrameInterval = sprite.frameIntervalOverride;

		//console.log('Animation finished a cycle: ' + sprite.name);
		if (sprite.name == Sprinkles_Dies + Sprinkles.HornModifier) {
			this.sayGoodbye();
			this.alive = false;
			return;
		}

		if (sprite.name.endsWith(Sprinkles.HornModifier)) {
			let colorShiftingSpriteProxy: ColorShiftingSpriteProxy = <ColorShiftingSpriteProxy>(sprite);
			this.hornHue = colorShiftingSpriteProxy.getCurrentHueShift(now);
		}
		else if (sprite.name.endsWith(Sprinkles.BodyModifier)) {
			let colorShiftingSpriteProxy: ColorShiftingSpriteProxy = <ColorShiftingSpriteProxy>(sprite);
			this.bodyHue = colorShiftingSpriteProxy.getCurrentHueShift(now);
			this.bodyHueShiftPerSecond = colorShiftingSpriteProxy.hueShiftPerSecond;
		}

		this.executeNextCommand(now);
		this.animationJustCycled = true;
	}

	sayGoodbye(): any {
		const fadeOutTime: number = 2000;
		let deathSequence = this.getHornBodyPair(Sprinkles_Dies);
		for (let i = 0; i < deathSequence.body.sprites.length; i++) {
			let bodySprite: SpriteProxy = deathSequence.body.sprites[i];
			this.fadeOut(bodySprite, fadeOutTime);
		}
		for (let i = 0; i < deathSequence.horn.sprites.length; i++) {
			let hornSprite: SpriteProxy = deathSequence.horn.sprites[i];
			this.fadeOut(hornSprite, fadeOutTime);
		}
	}

	private fadeOut(bodySprite: SpriteProxy, fadeOutTime: number) {
		bodySprite.removeAllCycleCallbacks();
		bodySprite.expirationDate = performance.now() + fadeOutTime;
		bodySprite.fadeOutTime = fadeOutTime;
	}

	getHornBodyPair(name: string): HornBodyPair {
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			if (hornBodyPair && hornBodyPair.name === name)
				return hornBodyPair;
		}
		return null;
	}

	newAnimationsAdded: boolean;
	draw(context: CanvasRenderingContext2D, now: number, layer: Layer): void {
		if (layer != this.layer)
			return;

		this.newAnimationsAdded = false;

		let dequeuedAnimations: boolean = this.dequeueAnimations(now);
		if (!this.hasAnimationsRunning()) {
			return;
		}

		this.updatePosition(now);

		//if (dequeuedAnimations)
		//	this.changeVelocity(this.velocityX, now);

		this.parts.forEach(function (hornBodyPair: HornBodyPair) {
			hornBodyPair.draw(context, now);
		});

		this.adornments.draw(context, now);

		if (this.animationJustCycled) {
			this.animationJustCycled = false;
		}

		if (this.newAnimationsAdded) {
			this.drawNewAnimations(context, now);
		}
	}

	enumToStr(enumeration: any, value: any): string {
		for (var k in enumeration) if (enumeration[k] == value) return <string>k;
		return null;
	}

	private isStateIdle(state: SprinkleState) {
		return state === SprinkleState.IdleStand || state === SprinkleState.WalkingBackwards || state === SprinkleState.WalkingForwards;
	}

	private isIdle() {
		return this.isStateIdle(this.state);
	}


	stateChanged(oldState: SprinkleState, newState: SprinkleState): void {
		//console.log(`Changing state from ${this.enumToStr(SprinkleState, oldState)} to ${this.enumToStr(SprinkleState, newState)}`);
		if (this.isAdornable(oldState) && !this.isAdornable(newState))
			this.removeLoopingAdornments();
	}

	removeLoopingAdornments(): any {
		this.adornments.destroyAllInExactly(2000);
	}

	isAdornable(state: SprinkleState): any {
		return this.isStateIdle(state);
	}

	drawNewAnimations(context: CanvasRenderingContext2D, now: number): number {
		let numSpritesDrawn: number = 0;
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			if (hornBodyPair.justAdded) {
				hornBodyPair.justAdded = false;
				numSpritesDrawn += hornBodyPair.draw(context, now);
			}
		}
		return numSpritesDrawn;
	}
}