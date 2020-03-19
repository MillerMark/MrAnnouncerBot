enum Layer {
	Back,
	Front
}

class VelocityDelta {
	constructor(public newVelocity: number, public time: number) {

	}
}

class AnimationCommand {
	constructor(public animationName: string, public velocity: number = 0, public reversed: boolean = false) {
		
	}
}

class QueuedAnimation {
	constructor(public hornBodyPair: HornBodyPair, public reverse: boolean, public timeStart: number) {

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
	Idle, // Idle or Graze
	Attack, // Dies, BattleCry, PushUpAttack, ScoopAttack, Stab Attack
	Walking,
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
	static readonly BodyModifier: string = '.Body';
	static readonly HornModifier: string = '.Horn';
	state: SprinkleState = SprinkleState.Offscreen;
	layer: Layer;
	private _x: number = 200;
	bodyHueShift: number = 0;
	startX: number;
	timeStart: number;
	velocityX: number = 0;
	static WalkingVelocity: number = 6;

	get x(): number {
		return this._x;
	}

	set x(newValue: number) {
		if (this._x !== newValue)
			console.log(`x = ${newValue}`);
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

	hornHue: number = 0;
	//lastAnimationTimeStart: number = 0;
	parts: Array<HornBodyPair>;
	commandQueue: Array<AnimationCommand>;


	constructor() {
		this.startX = this.x;
		this.timeStart = performance.now();
		this.layer = Layer.Back;
		this.parts = new Array<HornBodyPair>();
		this.commandQueue = new Array<AnimationCommand>();
		this.loadParts();
	}

	velocityQueue: Array<VelocityDelta> = new Array<VelocityDelta>();
	animationQueue: Array<QueuedAnimation> = new Array<QueuedAnimation>();

	queueVelocityChange(velocityX: number, now: number) {
		this.changeVelocity(velocityX, now); // Try immediate..
		//this.velocityQueue.push(new VelocityDelta(velocityX, now));
	}

	private queueAnimation(hornBodyPair: HornBodyPair, reverse: boolean, now: number) {
		this.animationQueue.push(new QueuedAnimation(hornBodyPair, reverse, now));
	}

	changeVelocity(velocityX: number, now: number) {
		console.log(`changeVelocity(${velocityX}, ${now});`);
		let previousStartX: number = this.startX;

		if (this.velocityX !== velocityX)
			console.log(`changing velocity from ${this.velocityX} to ${velocityX}!`);

		//if (this.velocityX === -6 && velocityX === 0) {
		//	debugger;
		//}
		this.timeStart = now;
		this.velocityX = velocityX;
		this.startX = this.x;

		if (previousStartX !== this.startX)
			if (previousStartX < 1600 && this.startX > 0) {
				console.log(`this.startX changing from ${previousStartX} to ${this.startX}`);
			}
	}

	updatePosition(now: number) {
		if (this.velocityX != 0) {
			var secondsPassed = (now - this.timeStart) / 1000;
			var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, 0);
			let newX: number = this.startX + Physics.metersToPixels(xDisplacement);
			let jumpDelta: number = Math.abs(this.x - newX);
			console.log('jumpDelta: ' + jumpDelta);
			this.x = newX;
		}
		this.dequeueVelocityDeltas();
	}

	dequeueVelocityDeltas(): void {
		if (this.velocityQueue.length > 0)
			console.log('this.velocityQueue.length: ' + this.velocityQueue.length);
		while (this.velocityQueue.length > 0) {
			let velocityDelta: VelocityDelta = this.velocityQueue.shift();
			this.changeVelocity(velocityDelta.newVelocity, velocityDelta.time);
		}
	}

	dequeueAnimations(now: number): boolean {
		let dequeuedAnimations: boolean = false;
		while (this.animationQueue.length > 0) {
			let queuedAnimation: QueuedAnimation = this.animationQueue.shift();
			this.addAnimation(queuedAnimation.hornBodyPair, now, queuedAnimation.reverse);
			dequeuedAnimations = true;
		}

		return dequeuedAnimations;
	}

	loadParts(): any {
		const originX: number = 123;
		const originY: number = 659;
		this.parts.push(new HornBodyPair(Sprinkles_BattleCry, 'BattleCry - ', 139, originX, originY, 0, 55));
		this.parts.push(new HornBodyPair(Sprinkles_Dies, 'Dies - ', 160, originX + 40, originY + 23, 0, 51, AnimationStyle.SequentialStop));
		this.parts.push(new HornBodyPair(Sprinkles_Graze, 'Graze - ', 301, originX + 25, originY + 7, 0, 231));
		this.parts.push(new HornBodyPair(Sprinkles_Idle, 'Idle', 201, originX - 6, originY + 14, 0, 225, AnimationStyle.Loop, fps20));
		this.parts.push(new HornBodyPair(Sprinkles_IdleToAttack, 'IdleToAttack - ', 21, originX - 3, originY + 12, 0, 67, AnimationStyle.SequentialStop));
		this.parts.push(new HornBodyPair(Sprinkles_IdleToWalk, 'IdleToWalk - ', 21, originX - 4, originY + 9, 0, 232, AnimationStyle.SequentialStop));
		this.parts.push(new HornBodyPair(Sprinkles_PushUpAttack, 'PushUpAttack - ', 79, originX + 21, originY + 43, 0, 0));
		this.parts.push(new HornBodyPair(Sprinkles_ScoopAttack, 'ScoopAttack - ', 129, originX + 250, originY + 127, 0, 0));
		this.parts.push(new HornBodyPair(Sprinkles_StabAttack, 'StabAttack - ', 79, originX + 113, originY + 3, 0, 39));
		this.parts.push(new HornBodyPair(Sprinkles_Walk, 'Walk - ', 81, originX - 4, originY + 12, 0, 257));
	}

	hasAnimationsRunning(): boolean {
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			if (hornBodyPair.body.sprites.length > 0)
				return true;
		}
		return false;
	}

	executeCommand(commandData: string, now: number): void {
		this.addCommand(commandData);
		if (!this.hasAnimationsRunning())
			this.executeNextCommand(now);
	}

	addCommand(commandData: string, velocity: number = 0): void {
		this.commandQueue.push(new AnimationCommand(commandData, velocity));
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
			this.moveAllSpritesToY(hornBodyPair.body.sprites, y);
			this.moveAllSpritesToY(hornBodyPair.horn.sprites, y);
		}
	}

	moveAllSpritesToY(sprites: SpriteProxy[], y: number): void {
		for (let j = 0; j < sprites.length; j++) {
			sprites[j].y = y;
		}
	}


	setPositionX(x: number): any {
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			this.moveAllSpritesToX(hornBodyPair.body, x);
			this.moveAllSpritesToX(hornBodyPair.horn, x);
		}
	}

	moveAllSpritesToX(sprites: Sprites, x: number): void {
		for (let j = 0; j < sprites.sprites.length; j++) {
			sprites.sprites[j].x = x - sprites.originX;
		}
	}

	doSomethingCreative(now: number) {
		switch (this.state) {
			case SprinkleState.Idle:
				if (this.lastAnimationCycled && this.lastAnimationCycled.startsWith(Sprinkles_Idle)) {
					this.addCommand(Sprinkles_Graze);
				}
				else if (!this.lastAnimationCycled || this.lastAnimationCycled.startsWith(Sprinkles_Graze)) {
					this.addCommand(Sprinkles_Idle);
				}
				break;
		}

		if (this.hasCommandsInQueue())
			this.executeNextCommand(now);
	}

	private hasCommandsInQueue() {
		return this.commandQueue.length > 0;
	}

	getNextCommand(): AnimationCommand {
		return this.commandQueue.shift();
	}

	clearCommandQueue(): void {
		this.commandQueue = [];
	}

	executeNextCommand(now: number = -1): void {
		if (now === -1)
			now = performance.now();
		if (!this.hasCommandsInQueue()) {
			this.doSomethingCreative(now);
			return;
		}

		let command: AnimationCommand = this.getNextCommand();
		switch (command.animationName) {
			case 'Walk Left':
				this.walkLeft();
				return;
			case 'Walk Right':
				this.walkRight();
				return;
		}
		if (command.animationName === 'Appear Behind') {
			this.x = 2020;
			//this.x = 1220;
			this.queueVelocityChange(0, performance.now());
			this.layer = Layer.Back;
			this.addCommand(Sprinkles_Walk, -Sprinkles.WalkingVelocity);
			//this.addCommand(Sprinkles_Walk);
			this.addCommand(Sprinkles_WalkToIdle, -Sprinkles.WalkingVelocity / 2.0);
			this.addCommand(Sprinkles_Idle);
			this.executeNextCommand(now);
			return;
		}

		var reverse: boolean = false;

		switch (command.animationName) {
			case Sprinkles_AttackToIdle:
				this.state = SprinkleState.TransitioningToIdle;
				command.animationName = Sprinkles_IdleToAttack;
				reverse = true;
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_WalkToIdle:
				this.state = SprinkleState.TransitioningToIdle;
				command.animationName = Sprinkles_IdleToWalk;
				reverse = true;
				// TODO: consider slowing down velocity as we transition to idle.
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_PushUpAttack:
			case Sprinkles_ScoopAttack:
			case Sprinkles_BattleCry:
			case Sprinkles_StabAttack:
				this.state = SprinkleState.Attack;
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_Walk:
				this.state = SprinkleState.Walking;
				this.queueVelocityChange(command.velocity, now);
				break;
			case Sprinkles_WalkBackwards:
				this.state = SprinkleState.WalkingBackwards;
				this.queueVelocityChange(command.velocity, now);
				command.animationName = Sprinkles_Walk;
				reverse = true;
				break;
			case Sprinkles_Idle:
			case Sprinkles_Graze:
				this.state = SprinkleState.Idle;
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

		this.queueAnimation(this.getHornBodyPair(command.animationName), reverse, now);
	}

	walkLeft(): void {
		this.transitionToWalk(Sprinkles_Walk, -Sprinkles.WalkingVelocity);
	}

	walkRight(): void {
		this.transitionToWalk(Sprinkles_WalkBackwards, Sprinkles.WalkingVelocity);
	}

	transitionToWalk(walkCommand: string, walkingVelocity: number = 0): void {
		switch (this.state) {
			case SprinkleState.TransitioningToAttack:
			case SprinkleState.Attack:
				this.clearCommandQueue()
				this.addCommand(Sprinkles_AttackToIdle);
				this.addCommand(Sprinkles_IdleToWalk, walkingVelocity / 2.0);
				this.addCommand(walkCommand, walkingVelocity);
				break;


			case SprinkleState.TransitioningToIdle:
			case SprinkleState.Idle:
				this.clearCommandQueue()
				this.addCommand(Sprinkles_IdleToWalk, walkingVelocity / 2.0);
				this.addCommand(walkCommand, walkingVelocity);
				break;

			default:
				this.clearCommandQueue();
				this.addCommand(walkCommand, walkingVelocity);
				break;
		}
	}


	private addAnimation(hornBodyPair: HornBodyPair, now: number, reverse: boolean) {
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

		let body: SpriteProxy = hornBodyPair.body.addShifted(this.x, this.y, startingFrameIndex, this.bodyHueShift);
		body.name = hornBodyPair.name + Sprinkles.BodyModifier;
		body.timeStart = now;

		let horn = hornBodyPair.horn.addShifted(this.x, this.y, startingFrameIndex, this.hornHue); // 30 degrees of hue shift every second.
		horn.name = hornBodyPair.name + Sprinkles.HornModifier;
		horn.addOnCycleCallback(this.animationCycled.bind(this));
		horn.onExpire = this.onSpriteExpire.bind(this)
		horn.hueShiftPerSecond = 30;
		horn.timeStart = now;

		hornBodyPair.justAdded = true;
		this.newAnimationsAdded = true;
	}


	needToDrawNewAnimations: boolean;

	onSpriteExpire() {
		this.needToDrawNewAnimations = !this.hasAnimationsRunning();
	}

	animationJustCycled: boolean;


	lastAnimationCycled: string;
	animationCycled(sprite: SpriteProxy, now: number) {
		this.lastAnimationCycled = sprite.name;
		console.log('Animation Cycled: ' + sprite.name);
		if (sprite.name == Sprinkles_Dies + Sprinkles.HornModifier) {
			this.sayGoodbye();
			return;
		}

		if (sprite.name.endsWith(Sprinkles.HornModifier)) {
			let colorShiftingSpriteProxy: ColorShiftingSpriteProxy = <ColorShiftingSpriteProxy>(sprite);
			this.hornHue = colorShiftingSpriteProxy.getCurrentHueShift(performance.now());
			//this.lastAnimationTimeStart = colorShiftingSpriteProxy.timeStart;
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

		if (this.animationJustCycled) {
			this.animationJustCycled = false;
		}

		if (this.newAnimationsAdded) {
			this.drawNewAnimations(context, now);
		}
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