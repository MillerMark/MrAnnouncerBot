class InGameCreatureManager {
	inGameCreaturesParchmentBackground: Sprites;
	inGameScrollAppearAnimation: Sprites;
	inGameScrollDisappearAnimation: Sprites;
	inGameCreaturesParchmentShadow: Sprites;
	inGameCreaturesParchmentTarget: Sprites;
	
	constructor() {

	}

	update(timestamp: number) {
		this.inGameCreaturesParchmentBackground.updatePositionsForFreeElements(timestamp);
		this.inGameCreaturesParchmentShadow.updatePositionsForFreeElements(timestamp);
		this.inGameCreaturesParchmentTarget.updatePositionsForFreeElements(timestamp);
		this.inGameScrollAppearAnimation.updatePositionsForFreeElements(timestamp);
		this.inGameScrollDisappearAnimation.updatePositionsForFreeElements(timestamp);
	}

	loadResources() {
		this.inGameCreaturesParchmentBackground = new Sprites('Scroll/InGameCreatures/ParchmentBackground', 2, fps30, AnimationStyle.Static);
		this.inGameCreaturesParchmentShadow = new Sprites('Scroll/InGameCreatures/ParchmentPicShadow', 2, fps30, AnimationStyle.Static);
		this.inGameCreaturesParchmentTarget = new Sprites('Scroll/InGameCreatures/Target', 1, fps30, AnimationStyle.Static);
		this.inGameScrollAppearAnimation = new Sprites('Scroll/InGameCreatures/ScrollAppear/ScrollAppear', 115, fps30, AnimationStyle.Sequential, true);
		this.inGameScrollAppearAnimation.originX = 105;
		this.inGameScrollAppearAnimation.originY = 3;
		this.inGameScrollDisappearAnimation = new Sprites('Scroll/InGameCreatures/ScrollDisappear/ScrollDisappear', 217, fps30, AnimationStyle.Sequential, true);
		this.inGameScrollDisappearAnimation.originX = 49;
		this.inGameScrollDisappearAnimation.originY = 2;

		this.inGameCreaturesParchmentBackground.disableGravity();
		this.inGameCreaturesParchmentShadow.disableGravity();
		this.inGameCreaturesParchmentTarget.disableGravity();
		this.inGameScrollAppearAnimation.disableGravity();
		this.inGameScrollDisappearAnimation.disableGravity();
	}

	inGameCreatures: Array<InGameCreature> = [];

	static readonly inGameScrollStatWidth = 135;
	static readonly minInGameStatFontSize = 14;
	static readonly inGameNameStatFontSize = 28;
	static readonly inGameIndexStatFontSize = 48;
	static readonly inGameNumberCenterX = 142;
	static readonly inGameNumberCenterY = 76;
	static readonly inGameNumberMaxWidth = 40;
	static readonly inGameAllOtherStatFontSize = 20;
	static readonly inGameCreatueStatFontName = 'Calibri';
	static readonly inGameStatOffsetX = 18;
	static readonly inGameStatTopMargin = -12;  // Put top of scroll slightly offscreen
	static readonly spaceBetweenInGameStatLines = 1;

	drawInGameCreature(context: CanvasRenderingContext2D, inGameCreature: InGameCreature, x: number, y: number, alpha: number) {
		context.fillStyle = '#ffffff';
		const nameOffsetY = 135;

		context.globalAlpha = alpha;
		try {
			this.drawInGameIndex(context, inGameCreature.Index, x, alpha);
			this.drawInGameCreatureHeadshot(context, inGameCreature, x);

			context.textAlign = 'left';
			context.textBaseline = 'top';
			x += InGameCreatureManager.inGameStatOffsetX;
			y += InGameCreatureManager.inGameStatTopMargin + nameOffsetY;
			let fontHeight = InGameCreatureManager.inGameNameStatFontSize;
			let yTop: number = this.drawInGameStat(context, inGameCreature.Name, fontHeight, x, y);
			fontHeight = InGameCreatureManager.inGameAllOtherStatFontSize;
			yTop = this.drawInGameStat(context, inGameCreature.Kind, fontHeight, x, yTop);
			if (inGameCreature.Alignment !== 'any' && inGameCreature.IsEnemy)
				yTop = this.drawInGameStat(context, inGameCreature.Alignment, fontHeight, x, yTop);
			const healthDescription: string = this.getHealthDescription(inGameCreature.Health);
			this.drawInGameStat(context, healthDescription, fontHeight, x, yTop);
			if (inGameCreature.Health === 0) {
				// TODO: draw red "X" across creature's headshot.
				// context.moveTo(x, 0);
			}
		}
		finally {
			context.globalAlpha = 1;
		}
	}

	drawInGameCreatureHeadshot(context: CanvasRenderingContext2D, inGameCreature: InGameCreature, x: number) {
		if (!inGameCreature.imageLoaded)
			return;
		const cropHeight: number = InGameCreature.ImageHeight * inGameCreature.CropWidth / InGameCreature.ImageWidth;
		const dx = 12;
		const dy = 34;
		const dw: number = InGameCreature.ImageWidth;
		const dh: number = InGameCreature.ImageHeight;
		const sx: number = inGameCreature.CropX;
		const sy: number = inGameCreature.CropY;
		const sw: number = inGameCreature.CropWidth;
		const sh: number = cropHeight;
		const border = 1;

		context.drawImage(inGameCreature.image, sx, sy, sw, sh, x + dx + border, dy + border + InGameCreatureManager.inGameStatTopMargin, dw - 2 * border, dh - 2 * border);

		// ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)

	}

	drawInGameIndex(context: CanvasRenderingContext2D, index: number, x: number, alpha: number) {
		const text: string = index.toString();
		let fontHeight: number = InGameCreatureManager.inGameIndexStatFontSize;
		context.font = `${fontHeight}px ${InGameCreatureManager.inGameCreatueStatFontName}`;
		while (context.measureText(text).width > InGameCreatureManager.inGameNumberMaxWidth && fontHeight > InGameCreatureManager.minInGameStatFontSize) {
			fontHeight--;
			context.font = `${fontHeight}px ${InGameCreatureManager.inGameCreatueStatFontName}`;
		}
		context.textAlign = 'center';
		context.textBaseline = 'middle';
		context.globalAlpha = 0.5 * alpha;
		context.fillText(text, x + InGameCreatureManager.inGameNumberCenterX, InGameCreatureManager.inGameNumberCenterY + InGameCreatureManager.inGameStatTopMargin);
		context.globalAlpha = 1 * alpha;
	}

	getHealthDescription(health: number): string {
		if (health === 0)
			return 'Dead';
		if (health < 0.1)
			return 'Near Death';
		if (health < 0.2)
			return 'Badly Bloodied';
		if (health < 0.3)
			return 'Bloodied';
		if (health < 0.4)
			return 'Beaten';
		if (health < 0.5)
			return 'Roughed Up';
		if (health < 0.7)
			return 'Bruised';
		if (health < 0.8)
			return 'Slightly Bruised';
		if (health < 0.9)
			return 'Looks Good';
		return 'Healthy';
	}

	drawInGameStat(context: CanvasRenderingContext2D, name: string, fontHeight: number, x: number, y: number): number {
		context.font = `${fontHeight}px ${InGameCreatureManager.inGameCreatueStatFontName}`;
		while (context.measureText(name).width > InGameCreatureManager.inGameScrollStatWidth && fontHeight > InGameCreatureManager.minInGameStatFontSize) {
			fontHeight--;
			context.font = `${fontHeight}px ${InGameCreatureManager.inGameCreatueStatFontName}`;
		}
		if (context.measureText(name).width > InGameCreatureManager.inGameScrollStatWidth) // Wait - still can't fit the name at 8px???!!
		{
			const ellipsis = '…';
			let rootName = name;
			while (context.measureText(rootName + ellipsis).width > InGameCreatureManager.inGameScrollStatWidth) {
				rootName = rootName.substr(0, rootName.length - 1);
			}
			name = rootName + ellipsis;
		}
		context.fillText(name, x, y);

		return y + fontHeight + InGameCreatureManager.spaceBetweenInGameStatLines;
	}

	drawInGameCreatures(context: CanvasRenderingContext2D, nowMs: number) {
		this.inGameCreaturesParchmentBackground.draw(context, nowMs);

		for (let i = 0; i < this.inGameCreatures.length; i++) {
			const inGameCreature: InGameCreature = this.inGameCreatures[i]
			const alpha: number = this.getAlphaForCreature(inGameCreature);
			const sprite: SpriteProxy = this.getParchmentSpriteForCreature(inGameCreature);
			if (sprite)
				this.drawInGameCreature(context, this.inGameCreatures[i], sprite.x, 0, alpha);
		}

		this.inGameCreaturesParchmentShadow.draw(context, nowMs);
		this.inGameCreaturesParchmentTarget.draw(context, nowMs);

		this.inGameScrollAppearAnimation.draw(context, nowMs);
		this.inGameScrollDisappearAnimation.draw(context, nowMs);
	}

	getParchmentSpriteForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.inGameCreaturesParchmentBackground.sprites, inGameCreature);
	}

	getParchmentShadowForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.inGameCreaturesParchmentShadow.sprites, inGameCreature);
	}

	getScrollAppearForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.inGameScrollAppearAnimation.sprites, inGameCreature);
	}

	getScrollDisappearForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.inGameScrollDisappearAnimation.sprites, inGameCreature);
	}

	getTargetSpriteForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.inGameCreaturesParchmentTarget.sprites, inGameCreature);
	}

	getSpriteForCreature(sprites: SpriteProxy[], inGameCreature: InGameCreature): SpriteProxy {
		for (let i = 0; i < sprites.length; i++) {
			if (sprites[i].data === inGameCreature)
				return sprites[i];
		}
		return null;
	}

	getAlphaForCreature(inGameCreature: InGameCreature): number {
		const creatureSprite: SpriteProxy = this.getParchmentSpriteForCreature(inGameCreature);
		if (creatureSprite)
			return creatureSprite.getAlpha(performance.now());
		return 0;
	}

	public readonly miniScrollLeftMargin = 375;
	public readonly miniScrollWidth = 180;

	// In-game Creature commands...
	//! ------------------------------------------
	setInGameCreatures(inGameCreatures: Array<InGameCreature>) {
		this.inGameCreatures = [];
		for (let i = 0; i < inGameCreatures.length; i++) {
			this.inGameCreatures.push(new InGameCreature(inGameCreatures[i]));
		}

		let x: number = this.miniScrollLeftMargin;
		this.inGameCreaturesParchmentBackground.sprites = [];
		this.inGameCreaturesParchmentShadow.sprites = [];
		this.inGameCreaturesParchmentTarget.sprites = [];

		let delayMs: number = 0;
		let timeBetweenArrivals: number = 300;
		for (let i = 0; i < this.inGameCreatures.length; i++) {
			this.addInGameCreature(this.inGameCreatures[i], x, delayMs);
			x += this.miniScrollWidth;
			delayMs += timeBetweenArrivals;
			timeBetweenArrivals -= 40;
			if (timeBetweenArrivals < 50)
				timeBetweenArrivals = 50;
		}
	}

	private addInGameCreature(inGameCreature: InGameCreature, x: number, delayMs = 0) {
		let frameIndex = 0;
		if (inGameCreature.IsEnemy)
			frameIndex = 1;

		const saturation: number = MathEx.clamp(inGameCreature.Health * 100, 0, 100);
		const parchmentSprite: SpriteProxy = this.inGameCreaturesParchmentBackground.addShifted(x, InGameCreatureManager.inGameStatTopMargin, frameIndex, 0, saturation);
		parchmentSprite.data = inGameCreature;
		parchmentSprite.delayStart = 700;
		parchmentSprite.fadeInTime = 1000;
		parchmentSprite.delayStart = delayMs;
		const parchmentShadowSprite: SpriteProxy = this.inGameCreaturesParchmentShadow.addShifted(x, InGameCreatureManager.inGameStatTopMargin, frameIndex, 0, saturation);
		parchmentShadowSprite.data = inGameCreature;
		parchmentShadowSprite.delayStart = 700;
		parchmentShadowSprite.fadeInTime = 1000;
		parchmentShadowSprite.delayStart = delayMs;
		const scrollSmokeHueShift = this.getCreatureSmokeHueShift(inGameCreature);
		const appearSprite: SpriteProxy = this.inGameScrollAppearAnimation.addShifted(x, InGameCreatureManager.inGameStatTopMargin, 0, scrollSmokeHueShift, saturation);
		appearSprite.data = inGameCreature;
		appearSprite.delayStart = delayMs;
		if (inGameCreature.IsTargeted) {
			const targetSprite: SpriteProxy = this.inGameCreaturesParchmentTarget.add(x, InGameCreatureManager.inGameStatTopMargin);
			targetSprite.data = inGameCreature;
			targetSprite.delayStart = delayMs;
		}
	}

	private getCreatureSmokeHueShift(inGameCreature: InGameCreature) {
		let scrollSmokeHueShift = 0;
		if (!inGameCreature.IsEnemy)
			scrollSmokeHueShift = 250;
		return scrollSmokeHueShift;
	}

	processInGameCreatureCommand(command: string, inGameCreatures: Array<InGameCreature>) {
		if (command === 'Set')
			this.setInGameCreatures(inGameCreatures);
		else if (command === 'Remove')
			this.removeInGameCreatures(inGameCreatures);
		else if (command === 'Add')
			this.addInGameCreatures(inGameCreatures);
	}

	removeInGameCreatures(inGameCreatures: InGameCreature[]) {
		for (let i = 0; i < inGameCreatures.length; i++) {
			const inGameCreature: InGameCreature = this.getInGameCreatureByIndex(inGameCreatures[i].Index);
			const creatureSprite: SpriteProxy = this.getParchmentSpriteForCreature(inGameCreature);
			const shadowSprite: SpriteProxy = this.getParchmentShadowForCreature(inGameCreature);
			const targetSprite: SpriteProxy = this.getTargetSpriteForCreature(inGameCreature);
			creatureSprite.logData = true;
			const fadeOutTimeMs = 800;
			const delayBeforeFadeOutMs = 800;
			creatureSprite.fadeOutAfter(delayBeforeFadeOutMs, fadeOutTimeMs);
			shadowSprite.fadeOutAfter(delayBeforeFadeOutMs, fadeOutTimeMs);
			if (targetSprite)
				targetSprite.fadeOutAfter(delayBeforeFadeOutMs, fadeOutTimeMs);
			const scrollSmokeHueShift = this.getCreatureSmokeHueShift(inGameCreature);
			const disappearAnimation: SpriteProxy = this.inGameScrollDisappearAnimation.addShifted(creatureSprite.x, creatureSprite.y, 0, scrollSmokeHueShift);
			disappearAnimation.data = inGameCreature;
			inGameCreature.removing = true;
			this.removeInGameCreature(inGameCreature, delayBeforeFadeOutMs + fadeOutTimeMs / 2);

			disappearAnimation.addOnCycleCallback(this.creatureDisappearAnimationComplete.bind(this));
		}
	}

	moveSpriteTo(sprite: SpriteProxy, targetX: number, delayMs: number) {
		if (!sprite)
			return;
		const moveTime = 750;
		sprite.ease(performance.now() + delayMs, sprite.x, sprite.y, targetX, sprite.y, moveTime);
	}

	moveCreatureTo(creature: InGameCreature, targetX: number, delayMs: number) {
		this.moveSpriteTo(this.getParchmentSpriteForCreature(creature), targetX, delayMs);
		this.moveSpriteTo(this.getParchmentShadowForCreature(creature), targetX, delayMs);
		this.moveSpriteTo(this.getTargetSpriteForCreature(creature), targetX, delayMs);
		this.moveSpriteTo(this.getScrollAppearForCreature(creature), targetX, delayMs);
		this.moveSpriteTo(this.getScrollDisappearForCreature(creature), targetX, delayMs);
	}

	removeInGameCreature(creature: InGameCreature, delayMs: number): void {
		const creatureIndexInArray: number = this.inGameCreatures.indexOf(creature);
		if (creatureIndexInArray < 0) {
			return;
		}

		let isRightOfRemovedCreature = false;
		let targetX: number = this.miniScrollLeftMargin;
		for (let i = 0; i < this.inGameCreatures.length; i++) {
			const thisCreature: InGameCreature = this.inGameCreatures[i];

			if (thisCreature.Index === creature.Index) {
				isRightOfRemovedCreature = true;
			}
			else if (isRightOfRemovedCreature && !thisCreature.removing) {
				this.moveCreatureTo(thisCreature, targetX, delayMs);
				delayMs += 200; // ms between each move.
			}

			if (!thisCreature.removing) {
				targetX += this.miniScrollWidth;
			}
		}
	}

	creatureDisappearAnimationComplete(sprite: SpriteProxy, now: number) {
		const inGameCreature: InGameCreature = sprite.data as InGameCreature;
		if (inGameCreature) {
			const creatureIndexInArray: number = this.inGameCreatures.indexOf(inGameCreature);
			this.inGameCreatures.splice(creatureIndexInArray, 1);
		}
	}

	getInGameCreatureByIndex(index: number): InGameCreature {
		for (let i = 0; i < this.inGameCreatures.length; i++) {
			if (this.inGameCreatures[i].Index === index)
				return this.inGameCreatures[i];
		}
		return null;
	}

	addInGameCreatures(inGameCreatures: InGameCreature[]) {
		for (let i = 0; i < inGameCreatures.length; i++) {
			//const inGameCreature: InGameCreature = this.getInGameCreatureByIndex(inGameCreatures[i].Index);
			//const creatureSprite: SpriteProxy = this.getParchmentSpriteForCreature(inGameCreature);
			//const shadowSprite: SpriteProxy = this.getParchmentShadowForCreature(inGameCreature);
			//const targetSprite: SpriteProxy = this.getTargetSpriteForCreature(inGameCreature);
			//creatureSprite.logData = true;
			//const fadeOutTimeMs = 800;
			//const delayBeforeFadeOutMs = 800;
			//creatureSprite.fadeOutIn(delayBeforeFadeOutMs, fadeOutTimeMs);
			//shadowSprite.fadeOutIn(delayBeforeFadeOutMs, fadeOutTimeMs);
			//if (targetSprite)
			//	targetSprite.fadeOutIn(delayBeforeFadeOutMs, fadeOutTimeMs);
			//const scrollSmokeHueShift = this.getCreatureSmokeHueShift(inGameCreature);
			//const disappearAnimation: SpriteProxy = this.inGameScrollDisappearAnimation.addShifted(creatureSprite.x, creatureSprite.y, 0, scrollSmokeHueShift);
			//disappearAnimation.data = inGameCreature;
			//inGameCreature.removing = true;
			//this.removeInGameCreature(inGameCreature, delayBeforeFadeOutMs + fadeOutTimeMs / 2);

			//disappearAnimation.addOnCycleCallback(this.creatureDisappearAnimationComplete.bind(this));
		}
	}
}
