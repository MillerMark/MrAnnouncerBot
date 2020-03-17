enum Layer {
	Back,
	Front
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

	draw(context: CanvasRenderingContext2D, now: number): void {
		this.updatePositions(now);
		this.body.draw(context, now);
		this.horn.draw(context, now);
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
const Sprinkles_Dies = 'Dies';
const Sprinkles_Graze = 'Graze';
const Sprinkles_Idle = 'Idle';
const Sprinkles_IdleToAttack = 'IdleToAttack';
const Sprinkles_AttackToIdle = 'AttackToIdle';
const Sprinkles_IdleToWalk = 'IdleToWalk';
const Sprinkles_WalkToIdle = 'WalkToIdle';


class Sprinkles {
	state: SprinkleState = SprinkleState.Offscreen;
	layer: Layer;
	private _x: number = 200;
	startX: number;
	timeStart: number;
	velocityX: number = 0;
	static WalkingVelocity: number = 6;

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

	hornHue: number = 0;
	lastAnimationTimeStart: number = 0;
	parts: Array<HornBodyPair>;
	commandQueue: Array<string>;


	constructor() {
		this.startX = this.x;
		this.timeStart = performance.now();
		this.layer = Layer.Back;
		this.parts = new Array<HornBodyPair>();
		this.commandQueue = new Array<string>();
		this.loadParts();
	}

	changeVelocity(velocityX: number, now: number) {
		this.timeStart = now;
		this.velocityX = velocityX;
		this.startX = this.x;
	}

	updatePosition(now: number) {
		var secondsPassed = (now - this.timeStart) / 1000;

		var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, 0);
		this.x = this.startX + Physics.metersToPixels(xDisplacement);
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

	executeCommand(commandData: string): any {
		this.commandQueue.push(commandData);
		if (this.hasAnimationsRunning())
			return;
		this.executeNextCommand();
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
			this.moveAllSpritesToX(hornBodyPair.body.sprites, x);
			this.moveAllSpritesToX(hornBodyPair.horn.sprites, x);
		}
	}

	moveAllSpritesToX(sprites: SpriteProxy[], x: number): void {
		for (let j = 0; j < sprites.length; j++) {
			sprites[j].x = x;
		}
	}

	executeNextCommand(): void {
		if (this.commandQueue.length == 0)
			return;

		let commandData: string = this.commandQueue.shift();
		if (commandData === 'Appear Behind') {
			this.x = 1920;
			this.velocityX = 0;
			this.timeStart = performance.now();
			this.startX = this.x;
			this.layer = Layer.Back;
			this.commandQueue.push('Walk');
			this.commandQueue.push('Walk');
			this.commandQueue.push('WalkToIdle');
			this.commandQueue.push(Sprinkles_Idle);
			this.executeNextCommand();
			//this.commandQueue.push('Graze');
			//this.commandQueue.push('Idle');
			//this.commandQueue.push('IdleToWalk');
			//this.commandQueue.push('Walk');
			//this.commandQueue.push('Walk');
			//this.commandQueue.push('Walk');
			//this.commandQueue.push('WalkToIdle');
			//this.commandQueue.push('Idle');
			//this.commandQueue.push('IdleToAttack');
			//this.commandQueue.push('Dies');
			//this.commandQueue.push('IdleToAttack');
			//this.commandQueue.push('PushUpAttack');
			//this.commandQueue.push('ScoopAttack');
			//this.commandQueue.push('StabAttack');
			//this.commandQueue.push('BattleCry');
			//this.commandQueue.push(Sprinkles_Dies);
		}
		else if (commandData == Sprinkles_WalkToIdle) {
			this.addAnimation(this.getHornBodyPair(Sprinkles_IdleToWalk), true);  // Reversing 
			this.changeVelocity(0, performance.now());
		}
		else if (commandData == Sprinkles_AttackToIdle) {
			this.addAnimation(this.getHornBodyPair(Sprinkles_IdleToAttack), true);  // Reversing 
			this.changeVelocity(0, performance.now());
		}
		else {
			switch (commandData) {
				case Sprinkles_PushUpAttack:
				case Sprinkles_ScoopAttack:
				case Sprinkles_BattleCry:
				case Sprinkles_StabAttack:
					this.state = SprinkleState.Attack;
					this.changeVelocity(0, performance.now());
					break;
				case Sprinkles_Walk:
					if (this.state == SprinkleState.Walking)
						return;
					this.state = SprinkleState.Walking;
					this.changeVelocity(-Sprinkles.WalkingVelocity, performance.now());
					break;
				case Sprinkles_Idle:
				case Sprinkles_Graze:
					this.state = SprinkleState.Idle;
					this.changeVelocity(0, performance.now());
					break;
				case Sprinkles_IdleToAttack:
					this.state = SprinkleState.TransitioningToAttack;
					this.changeVelocity(0, performance.now());
					break;
				case Sprinkles_AttackToIdle:
					this.state = SprinkleState.TransitioningToAttack;
					this.changeVelocity(0, performance.now());
					break;
			}

			this.addAnimation(this.getHornBodyPair(commandData));
		}
	}

	private addAnimation(hornBodyPair: HornBodyPair, reverse: boolean = false) {
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

		let body: SpriteProxy = hornBodyPair.body.add(this.x, this.y, startingFrameIndex);
		body.name = hornBodyPair.name;
		body.timeStart--;

		let horn = hornBodyPair.horn.addShifted(this.x, this.y, startingFrameIndex, this.hornHue); // 30 degrees of hue shift every second.
		horn.name = hornBodyPair.name;
		horn.addOnCycleCallback(this.animationCycled.bind(this));
		horn.hueShiftPerSecond = 30;
		horn.timeStart--;  // Needed to eliminate flicker for chained animations.
		hornBodyPair.justAdded = true;
		this.newAnimationsAdded = true;
	}

	animationCycled(sprite: SpriteProxy) {
		if (sprite.name == Sprinkles_Dies) {
			this.sayGoodbye();
		}
		//console.log('Animation Cycled: ' + sprite.name);
		//this.changeVelocity(0, performance.now());
		if (sprite instanceof ColorShiftingSpriteProxy) {
			let colorShiftingSpriteProxy: ColorShiftingSpriteProxy = <ColorShiftingSpriteProxy>(sprite);
			this.hornHue = colorShiftingSpriteProxy.getCurrentHueShift(performance.now());
			this.lastAnimationTimeStart = colorShiftingSpriteProxy.timeStart;
		}
		this.executeNextCommand();
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

		if (this.hasAnimationsRunning()) {
			this.updatePosition(now);
		}

		this.newAnimationsAdded = false;
		this.parts.forEach(function (hornBodyPair: HornBodyPair) {
			hornBodyPair.draw(context, now);
		});

		if (this.newAnimationsAdded) {
			this.drawNewAnimations(context, now);
		}
	}

	drawNewAnimations(context: CanvasRenderingContext2D, now: number): any {
		for (let i = 0; i < this.parts.length; i++) {
			let hornBodyPair: HornBodyPair = this.parts[i];
			if (hornBodyPair.justAdded) {
				hornBodyPair.justAdded = false;
				hornBodyPair.draw(context, now);
			}
		}
	}
}