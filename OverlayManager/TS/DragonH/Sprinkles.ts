enum Layer {
	Back,
	Front
}


class HornBodyPair {
	name: string;
	body: Sprites;
	horn: Sprites;
	justAdded: boolean;

	constructor(folder: string, fileBaseName: string, frameCount: number, originX: number, originY: number, hornDeltaX: number, hornDeltaY: number, animationStyle: AnimationStyle = AnimationStyle.Loop) {
		this.name = folder;
		this.body = new Sprites(`/Sprinkles/${folder}/${fileBaseName}Body`, frameCount, fps30, animationStyle, true);
		this.body.originX = originX;
		this.body.originY = originY;
		this.horn = new Sprites(`/Sprinkles/${folder}/${fileBaseName}Horn`, frameCount, fps30, animationStyle, true);
		this.horn.originX = originX - hornDeltaX;
		this.horn.originY = originY - hornDeltaY;
	}

	draw(context: CanvasRenderingContext2D, now: number): any {
		this.body.draw(context, now);
		this.horn.draw(context, now);
	}
}

//Folders.assets = 'GameDev/Assets/DragonH/';

class Sprinkles {
	layer: Layer;
	x: number = 200;
	y: number = 1070;
	hornHue: number = 0;
	lastAnimationTimeStart: number = 0;
	parts: Array<HornBodyPair>;
	commandQueue: Array<string>;


	constructor() {
		this.layer = Layer.Back;
		this.parts = new Array<HornBodyPair>();
		this.commandQueue = new Array<string>();
		this.loadParts();
	}

	loadParts(): any {
		const originX: number = 123;
		const originY: number = 659;
		this.parts.push(new HornBodyPair('BattleCry', 'BattleCry - ', 139, originX, originY, 0, 55));
		this.parts.push(new HornBodyPair('Dies', 'Dies - ', 160, originX + 40, originY + 23, 0, 51, AnimationStyle.SequentialStop));
		this.parts.push(new HornBodyPair('Graze', 'Graze - ', 301, originX + 25, originY + 7, 0, 231));
		this.parts.push(new HornBodyPair('Idle', 'Idle', 201, originX - 6, originY + 14, 0, 225));
		this.parts.push(new HornBodyPair('IdleToAttack', 'IdleToAttack - ', 21, originX - 3, originY + 12, 0, 67, AnimationStyle.SequentialStop));
		this.parts.push(new HornBodyPair('IdleToWalk', 'IdleToWalk - ', 21, originX - 4, originY + 9, 0, 232, AnimationStyle.SequentialStop));
		this.parts.push(new HornBodyPair('PushUpAttack', 'PushUpAttack - ', 79, originX + 21, originY + 43, 0, 0));
		this.parts.push(new HornBodyPair('ScoopAttack', 'ScoopAttack - ', 129, originX + 250, originY + 127, 0, 0));
		this.parts.push(new HornBodyPair('StabAttack', 'StabAttack - ', 79, originX + 113, originY + 3, 0, 39));
		this.parts.push(new HornBodyPair('Walk', 'Walk - ', 81, originX - 4, originY + 12, 0, 257));
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

	executeNextCommand(): void {
		if (this.commandQueue.length == 0)
			return;

		let commandData: string = this.commandQueue.shift();
		if (commandData === 'Appear Behind') {
			this.addAnimation(this.getHornBodyPair('Idle'));
			this.commandQueue.push('Graze');
			this.commandQueue.push('Idle');
			this.commandQueue.push('IdleToWalk');
			this.commandQueue.push('Walk');
			this.commandQueue.push('Idle');
			this.commandQueue.push('IdleToAttack');
			this.commandQueue.push('PushUpAttack');
			this.commandQueue.push('ScoopAttack');
			this.commandQueue.push('StabAttack');
			this.commandQueue.push('BattleCry');
			this.commandQueue.push('Dies');
		}
		else {
			this.addAnimation(this.getHornBodyPair(commandData));
		}
	}

	private addAnimation(hornBodyPair: HornBodyPair) {
		if (!hornBodyPair)
			return;
		this.clearAnimations();
		let body: SpriteProxy = hornBodyPair.body.add(this.x, this.y);
		body.timeStart--;

		let horn = hornBodyPair.horn.addShifted(this.x, this.y, 0, this.hornHue); // 30 degrees of hue shift every second.
		horn.addOnCycleCallback(this.animationCycled.bind(this));
		horn.hueShiftPerSecond = 30;
		horn.timeStart--;  // Needed to eliminate flicker for chained animations.
		hornBodyPair.justAdded = true;
		this.newAnimationsAdded = true;
	}

	animationCycled(sprite: SpriteProxy) {
		if (sprite instanceof ColorShiftingSpriteProxy) {
			let colorShiftingSpriteProxy: ColorShiftingSpriteProxy = <ColorShiftingSpriteProxy>(sprite);
			this.hornHue = colorShiftingSpriteProxy.getCurrentHueShift(performance.now());
			this.lastAnimationTimeStart = colorShiftingSpriteProxy.timeStart;
		}
		this.executeNextCommand();
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