class InGameCreatureManager {
	parchmentBackground: Sprites;
	deathX: Sprites;
	scrollAppear: Sprites;
	scrollDisappear: Sprites;
	parchmentShadow: Sprites;
	target: Sprites;
	allBloodSprites: SpriteCollection = new SpriteCollection();
	bloodA: Sprites;
	bloodB: Sprites;
	bloodC: Sprites;
	bloodD: Sprites;
	bloodE: Sprites;
	bloodGushing: Sprites;

	constructor() {

	}

	update(timestamp: number) {
		this.parchmentBackground.updatePositionsForFreeElements(timestamp);
		this.deathX.updatePositionsForFreeElements(timestamp);
		this.parchmentShadow.updatePositionsForFreeElements(timestamp);
		this.target.updatePositionsForFreeElements(timestamp);
		this.scrollAppear.updatePositionsForFreeElements(timestamp);
		this.scrollDisappear.updatePositionsForFreeElements(timestamp);
	}

	loadResources() {
		const saveBypassFrameSkip: boolean = globalBypassFrameSkip;
		globalBypassFrameSkip = true;
		this.parchmentBackground = new Sprites('Scroll/InGameCreatures/ParchmentBackground', 2, fps30, AnimationStyle.Static);
		this.deathX = new Sprites('Scroll/InGameCreatures/ParchmentDeathX', 1, fps30, AnimationStyle.Static);
		this.parchmentShadow = new Sprites('Scroll/InGameCreatures/ParchmentPicShadow', 2, fps30, AnimationStyle.Static);
		//this.inGameCreaturesParchmentTarget = new Sprites('Scroll/InGameCreatures/Target', 1, fps30, AnimationStyle.Static);
		this.target = new Sprites('Scroll/InGameCreatures/EnemyTarget/EnemyTarget', 102, fps30, AnimationStyle.Loop, true);
		this.target.originX = 41 - InGameCreatureManager.targetLeft;
		this.target.originY = 45;

		this.bloodA = this.loadBloodSprites('A', 'NpcBloodBurstA', 257, 133, 21);
		this.bloodB = this.loadBloodSprites('B', 'NpcBloodBurstB', 263, 122, 21);
		this.bloodC = this.loadBloodSprites('C', 'NpcBloodBurstC', 263, 139, 15);
		this.bloodD = this.loadBloodSprites('D', 'NpcBloodBurstD', 264, 133, 31);
		this.bloodE = this.loadBloodSprites('E', 'NpcBloodBurstE', 261, 195, 31);
		this.bloodGushing = this.loadBloodSprites('Severed', 'Severed', 473, 142, 226);
		this.bloodGushing.segmentSize = 88;
		this.bloodGushing.returnFrameIndex = 53;
		this.bloodGushing.animationStyle = AnimationStyle.Loop;

		this.scrollAppear = new Sprites('Scroll/InGameCreatures/ScrollAppear/ScrollAppear', 115, fps30, AnimationStyle.Sequential, true);
		this.scrollAppear.originX = 105;
		this.scrollAppear.originY = 3;
		this.scrollDisappear = new Sprites('Scroll/InGameCreatures/ScrollDisappear/ScrollDisappear', 217, fps30, AnimationStyle.Sequential, true);
		this.scrollDisappear.originX = 49;
		this.scrollDisappear.originY = 2;

		this.parchmentBackground.disableGravity();
		this.deathX.disableGravity();
		this.parchmentShadow.disableGravity();
		this.target.disableGravity();
		this.scrollAppear.disableGravity();
		this.scrollDisappear.disableGravity();

		globalBypassFrameSkip = saveBypassFrameSkip;
	}


	loadBloodSprites(folderName: string, fileName: string, originX: number, originY: number, frameCount: number): Sprites {
		const sprites: Sprites = new Sprites(`Scroll/InGameCreatures/Blood/${folderName}/${fileName}`, frameCount, fps30, AnimationStyle.Sequential, true);
		sprites.originX = originX;
		sprites.originY = originY;
		this.allBloodSprites.add(sprites);
		return sprites;
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
	static readonly headShotCenterX = 62;
	static readonly headShotCenterY = 70;
	static readonly targetLeft = 64;
	static readonly targetTop = 114;
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
		this.parchmentBackground.draw(context, nowMs);

		for (let i = 0; i < this.inGameCreatures.length; i++) {
			const inGameCreature: InGameCreature = this.inGameCreatures[i]
			const alpha: number = this.getAlphaForCreature(inGameCreature);
			const sprite: SpriteProxy = this.getParchmentSpriteForCreature(inGameCreature);
			if (sprite)
				this.drawInGameCreature(context, this.inGameCreatures[i], sprite.x, 0, alpha);
		}

		this.parchmentShadow.draw(context, nowMs);

		this.allBloodSprites.draw(context, nowMs);

		this.deathX.draw(context, nowMs);
		this.target.draw(context, nowMs);

		this.scrollAppear.draw(context, nowMs);
		this.scrollDisappear.draw(context, nowMs);

	}

	getParchmentSpriteForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.parchmentBackground.sprites, inGameCreature);
	}

	getParchmentDeathXForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.deathX.sprites, inGameCreature);
	}

	getParchmentShadowForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.parchmentShadow.sprites, inGameCreature);
	}

	getScrollAppearForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.scrollAppear.sprites, inGameCreature);
	}

	getScrollDisappearForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.scrollDisappear.sprites, inGameCreature);
	}

	getTargetSpriteForCreature(inGameCreature: InGameCreature): SpriteProxy {
		return this.getSpriteForCreature(this.target.sprites, inGameCreature);
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
	setInGameCreatures(inGameCreatures: Array<InGameCreature>, soundManager: SoundManager) {
		this.inGameCreatures = [];
		for (let i = 0; i < inGameCreatures.length; i++) {
			this.inGameCreatures.push(new InGameCreature(inGameCreatures[i]));
		}

		let x: number = this.miniScrollLeftMargin;
		this.parchmentBackground.sprites = [];
		this.parchmentShadow.sprites = [];
		this.deathX.sprites = [];
		this.target.sprites = [];

		let delayMs = 0;
		let timeBetweenArrivals = 300;
		let first = true;
		for (let i = 0; i < this.inGameCreatures.length; i++) {
			this.addInGameCreature(this.inGameCreatures[i], x, delayMs);

			if (first)
				soundManager.playMp3In(delayMs, 'InGameScrolls/InGameScrollAppearFirst');
			else
				soundManager.playMp3In(delayMs, 'InGameScrolls/InGameScrollAppear');

			first = false;

			x += this.miniScrollWidth;
			delayMs += timeBetweenArrivals;
			timeBetweenArrivals -= 40;
			if (timeBetweenArrivals < 50)
				timeBetweenArrivals = 50;
		}
	}
	updateInGameCreature(updatedGameCreature: InGameCreature, soundManager: SoundManager, healthChangeAnimationDelayMs: number) {
		const existingCreature: InGameCreature = this.getInGameCreatureByIndex(updatedGameCreature.Index);
		if (!existingCreature) {
			this.insertInGameCreature(updatedGameCreature);
			return healthChangeAnimationDelayMs;
		}

		existingCreature.Alignment = updatedGameCreature.Alignment;
		existingCreature.CropWidth = updatedGameCreature.CropWidth;
		existingCreature.CropX = updatedGameCreature.CropX;
		existingCreature.CropY = updatedGameCreature.CropY;
		if (existingCreature.Health !== updatedGameCreature.Health) {
			if (existingCreature.Health === 0) { // Was previously dead - now alive. 
				const deathXSprite: SpriteProxy = this.getParchmentDeathXForCreature(existingCreature);
				if (deathXSprite)
					deathXSprite.fadeOutNow(500);
			}
			existingCreature.Health = updatedGameCreature.Health;
			const parchmentSprite: ColorShiftingSpriteProxy = this.getParchmentSpriteForCreature(existingCreature) as ColorShiftingSpriteProxy;
			if (parchmentSprite) {
				parchmentSprite.saturationPercent = updatedGameCreature.Health * 100;
			}

			if (existingCreature.Health === 0) { // Was previously alive - now dead. 
				const x: number = this.getX(existingCreature);
				this.addDeathSprite(x, existingCreature, healthChangeAnimationDelayMs);
				healthChangeAnimationDelayMs += InGameCreatureManager.timeBetweenMoves;
			}
		}

		existingCreature.Kind = updatedGameCreature.Kind;
		existingCreature.Name = updatedGameCreature.Name;

		if (existingCreature.ImageURL !== updatedGameCreature.ImageURL)
			existingCreature.setImageUrl(updatedGameCreature.ImageURL);
		if (existingCreature.IsEnemy !== updatedGameCreature.IsEnemy) {
			existingCreature.IsEnemy = updatedGameCreature.IsEnemy;

			const parchmentShadow: SpriteProxy = this.getParchmentShadowForCreature(existingCreature);
			if (parchmentShadow)
				parchmentShadow.fadeOutNow(1000);
			const parchmentSprite: SpriteProxy = this.getParchmentSpriteForCreature(existingCreature);
			if (parchmentSprite)
				parchmentSprite.fadeOutNow(1000);

			const x: number = this.getX(existingCreature);
			const frameIndex = this.getFriendEnemyFrameIndex(existingCreature);
			this.addParchment(existingCreature, x, frameIndex, 0);
		}

		if (existingCreature.IsTargeted !== updatedGameCreature.IsTargeted) {
			existingCreature.IsTargeted = updatedGameCreature.IsTargeted;

			if (existingCreature.IsTargeted) {
				const x: number = this.getX(existingCreature);
				this.addTarget(existingCreature, x);
			}
			else {
				const target: SpriteProxy = this.getTargetSpriteForCreature(existingCreature);
				if (target)
					target.fadeOutNow(1000);
			}
		}

		if (updatedGameCreature.PercentDamageJustInflicted > 0) {
			// Show damage...
			this.showDamage(existingCreature, updatedGameCreature.PercentDamageJustInflicted, healthChangeAnimationDelayMs, soundManager);
			healthChangeAnimationDelayMs += InGameCreatureManager.timeBetweenMoves;
		}
		if (updatedGameCreature.PercentHealthJustGiven > 0) {
			// TODO: Show health...
			//healthChangeAnimationDelayMs += InGameCreatureManager.timeBetweenMoves;
		}
		return healthChangeAnimationDelayMs;
	}

	showDamage(existingCreature: InGameCreature, percentDamageJustInflicted: number, delayMs: number, soundManager: SoundManager) {
		const x: number = this.getX(existingCreature);
		// TODO: hue/sat/bright shift from existing creature's blood color modifiers
		const hueShift = 0;
		let sprites: Sprites;
		const gushingThreshold = 0.3;  // 
		let expirationDate = 0;
		let scale = 1;
		let overTheTopImpact = 1 + percentDamageJustInflicted;

		if (existingCreature.Health === 0)   // Just killed the creature.
			overTheTopImpact *= 1.2;

		if (percentDamageJustInflicted > gushingThreshold || percentDamageJustInflicted > gushingThreshold / 2 && existingCreature.Health === 0) {
			sprites = this.bloodGushing;
			scale = MathEx.clamp(2 * percentDamageJustInflicted / gushingThreshold, 0.5, 2);
			expirationDate = performance.now() + 6000 + 2000 * percentDamageJustInflicted / gushingThreshold;
		}
		else {
			if (Random.chancePercent(20))
				sprites = this.bloodA;
			else if (Random.chancePercent(25))
				sprites = this.bloodB;
			else if (Random.chancePercent(33))
				sprites = this.bloodC;
			else if (Random.chancePercent(50))
				sprites = this.bloodD;
			else
				sprites = this.bloodE;
			scale = MathEx.clamp(2 * percentDamageJustInflicted / gushingThreshold, 0.3, 2);
		}

		if (overTheTopImpact >= 1.5)
			soundManager.playMp3In(delayMs, 'Damage/Heavy/GushHeavy[13]');
		else if (overTheTopImpact >= 0.75)
			soundManager.playMp3In(delayMs, 'Damage/Medium/GushMedium[29]');
		else
			soundManager.playMp3In(delayMs, 'Damage/Light/GushLight[15]');

		const sprite: SpriteProxy = sprites.addShifted(x + InGameCreatureManager.headShotCenterX, InGameCreatureManager.headShotCenterY, 0, hueShift);
		sprite.delayStart = delayMs;
		sprite.scale = scale;
		if (Random.chancePercent(50))
			sprite.flipHorizontally = true;
		if (expirationDate > 0)
			sprite.expirationDate = expirationDate;
		// TODO: Consider changing frame rate (via sprite.frameIntervalOverride) to be proportional to scale, so smaller scales render faster and larger scales render more slowly (to keep gravity feeling consistent)..
	}

	getX(creature: InGameCreature): number {
		const sprite: SpriteProxy = this.getParchmentSpriteForCreature(creature);
		if (sprite)
			return sprite.x;
		return 0;
	}

	updateInGameCreatures(inGameCreatureDtos: Array<InGameCreature>, soundManager: SoundManager) {
		const numCreaturesBeforeUpdate: number = this.inGameCreatures.length;
		let healthChangeAnimationDelayMs = 0;
		for (let i = 0; i < inGameCreatureDtos.length; i++) {
			healthChangeAnimationDelayMs = this.updateInGameCreature(inGameCreatureDtos[i], soundManager, healthChangeAnimationDelayMs);
		}
		if (this.inGameCreatures.length > numCreaturesBeforeUpdate) { // Creatures added!
			this.repositionInGameScrolls();
			this.createAnimationsForRecentlyAddedCreatures(soundManager);
		}

		// Now look for creatures that we should no longer be showing...
		let delayMs = 0;
		for (let i = 0; i < this.inGameCreatures.length; i++) {
			const existingCreature: InGameCreature = this.inGameCreatures[i];
			if (existingCreature.removing)
				continue;

			let stillShowCreature = false;
			for (let i = 0; i < inGameCreatureDtos.length; i++) {
				if (inGameCreatureDtos[i].Index === existingCreature.Index) {  // Found creature. We are good.
					stillShowCreature = true;
					break;
				}
			}

			if (!stillShowCreature) { // Did not find the creature. Animate their removal.
				this.removeCreatureWithAnimation(this.inGameCreatures[i], soundManager, delayMs);
				delayMs += InGameCreatureManager.timeBetweenMoves;
			}
		}
	}

	private addInGameCreature(inGameCreature: InGameCreature, x: number, delayMs = 0) {
		const frameIndex = this.getFriendEnemyFrameIndex(inGameCreature);
		const saturation: number = this.addParchment(inGameCreature, x, frameIndex, delayMs);
		const scrollSmokeHueShift = this.getCreatureSmokeHueShift(inGameCreature);
		const appearSprite: SpriteProxy = this.scrollAppear.addShifted(x, InGameCreatureManager.inGameStatTopMargin, 0, scrollSmokeHueShift, saturation);
		appearSprite.data = inGameCreature;
		appearSprite.delayStart = delayMs;
		if (inGameCreature.IsTargeted) {
			this.addTarget(inGameCreature, x, delayMs);
		}
	}

	private getFriendEnemyFrameIndex(inGameCreature: InGameCreature) {
		let frameIndex = 0;
		if (inGameCreature.IsEnemy)
			frameIndex = 1;
		return frameIndex;
	}

	private addParchment(inGameCreature: InGameCreature, x: number, frameIndex: number, delayMs: number) {
		const saturation: number = MathEx.clamp(inGameCreature.Health * 100, 0, 100);
		const parchmentSprite: SpriteProxy = this.parchmentBackground.addShifted(x, InGameCreatureManager.inGameStatTopMargin, frameIndex, 0, saturation);
		parchmentSprite.data = inGameCreature;
		parchmentSprite.fadeInTime = 1000;
		parchmentSprite.delayStart = delayMs;
		const parchmentShadowSprite: SpriteProxy = this.parchmentShadow.addShifted(x, InGameCreatureManager.inGameStatTopMargin, frameIndex, 0, saturation);
		parchmentShadowSprite.data = inGameCreature;
		parchmentShadowSprite.fadeInTime = 1000;
		parchmentShadowSprite.delayStart = delayMs;
		if (inGameCreature.Health === 0)
			this.addDeathSprite(x, inGameCreature, delayMs);
		return saturation;
	}

	private addDeathSprite(x: number, inGameCreature: InGameCreature, delayMs = 0) {
		// TODO: add support for color-changing the X to match the creature's blood color.
		const deathXSprite: SpriteProxy = this.deathX.addShifted(x, InGameCreatureManager.inGameStatTopMargin, 0);
		deathXSprite.data = inGameCreature;
		deathXSprite.fadeInTime = 1000;
		deathXSprite.delayStart = delayMs;
	}

	private addTarget(inGameCreature: InGameCreature, x: number, delayMs = 0) {
		let hueShift = 0;
		if (!inGameCreature.IsEnemy)
			hueShift = 220;
		const targetSprite: SpriteProxy = this.target.addShifted(x, InGameCreatureManager.inGameStatTopMargin + InGameCreatureManager.targetTop, -1, hueShift);
		targetSprite.data = inGameCreature;
		targetSprite.delayStart = delayMs;
		targetSprite.fadeInTime = 500;
	}

	private getCreatureSmokeHueShift(inGameCreature: InGameCreature) {
		let scrollSmokeHueShift = 0;
		if (!inGameCreature.IsEnemy)
			scrollSmokeHueShift = 250;
		return scrollSmokeHueShift;
	}

	processInGameCreatureCommand(command: string, inGameCreatures: Array<InGameCreature>, soundManager: SoundManager) {
		if (command === 'Set')
			this.setInGameCreatures(inGameCreatures, soundManager);
		else if (command === 'Update')
			this.updateInGameCreatures(inGameCreatures, soundManager);
		else if (command === 'Remove')
			this.removeInGameCreatures(inGameCreatures, soundManager);
		else if (command === 'Add')
			this.addInGameCreatures(inGameCreatures, soundManager);
	}

	sameCreaturesInGame(inGameCreatures: InGameCreature[]) {
		for (let i = 0; i < inGameCreatures.length; i++) {
			if (!this.getInGameCreatureByIndex(inGameCreatures[i].Index))
				return false;
		}
		return true;
	}

	removeInGameCreatures(inGameCreatures: InGameCreature[], soundManager: SoundManager) {
		for (let i = 0; i < inGameCreatures.length; i++) {
			const thisCreatureIndex: number = inGameCreatures[i].Index;
			let inGameCreature: InGameCreature = this.getInGameCreatureByIndex(thisCreatureIndex);
			const maxTries = 100;  // infinite loop protection.
			let numTries = 0;
			while (inGameCreature && numTries < maxTries) {  // To delete extra of the same index which can happen if the StreamDeck keys are rapidly hit repeatedly.
				numTries++;
				this.removeCreatureWithAnimation(inGameCreature, soundManager);

				inGameCreature = this.getInGameCreatureByIndex(thisCreatureIndex);
			}
		}
	}

	private removeCreatureWithAnimation(inGameCreature: InGameCreature, soundManager: SoundManager, delayMs = 0) {
		const creatureSprite: SpriteProxy = this.getParchmentSpriteForCreature(inGameCreature);
		if (creatureSprite !== null) {
			const shadowSprite: SpriteProxy = this.getParchmentShadowForCreature(inGameCreature);
			const targetSprite: SpriteProxy = this.getTargetSpriteForCreature(inGameCreature);
			const deathXSprite: SpriteProxy = this.getParchmentDeathXForCreature(inGameCreature);
			creatureSprite.logData = true;
			const fadeOutTimeMs = 800;
			const delayBeforeFadeOutMs = 800;
			creatureSprite.fadeOutAfter(delayMs + delayBeforeFadeOutMs, fadeOutTimeMs);
			shadowSprite.fadeOutAfter(delayMs + delayBeforeFadeOutMs, fadeOutTimeMs);
			if (targetSprite)
				targetSprite.fadeOutAfter(delayMs + delayBeforeFadeOutMs, fadeOutTimeMs);
			if (deathXSprite)
				deathXSprite.fadeOutAfter(delayMs + delayBeforeFadeOutMs, fadeOutTimeMs);
			const scrollSmokeHueShift = this.getCreatureSmokeHueShift(inGameCreature);
			const disappearAnimation: SpriteProxy = this.scrollDisappear.addShifted(creatureSprite.x, creatureSprite.y, 0, scrollSmokeHueShift);
			disappearAnimation.delayStart = delayMs;
			disappearAnimation.data = inGameCreature;
			inGameCreature.removing = true;
			this.removeInGameCreature(inGameCreature, delayMs + delayBeforeFadeOutMs + fadeOutTimeMs / 2);
			disappearAnimation.addOnCycleCallback(this.creatureDisappearAnimationComplete.bind(this));
			soundManager.safePlayMp3('InGameScrolls/InGameScrollDisappear');
		}
	}

	moveSpriteTo(sprites: Sprites, creature: InGameCreature, targetX: number, delayMs: number) {
		const sprite: SpriteProxy = this.getSpriteForCreature(sprites.sprites, creature);
		if (!sprite)
			return;
		const moveTime = 750;
		// Since we are moving X we must account for the originX
		const adjustedX: number = targetX - sprites.originX;
		sprite.ease(performance.now() + delayMs, sprite.x, sprite.y, adjustedX, sprite.y, moveTime);
	}

	moveCreatureTo(creature: InGameCreature, targetX: number, delayMs: number) {
		this.moveSpriteTo(this.parchmentBackground, creature, targetX, delayMs);
		this.moveSpriteTo(this.parchmentShadow, creature, targetX, delayMs);
		this.moveSpriteTo(this.deathX, creature, targetX, delayMs);
		this.moveSpriteTo(this.target, creature, targetX, delayMs);
		this.moveSpriteTo(this.scrollAppear, creature, targetX, delayMs);
		this.moveSpriteTo(this.scrollDisappear, creature, targetX, delayMs);
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
				delayMs += InGameCreatureManager.timeBetweenMoves;; // ms between each move.
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
			if (this.inGameCreatures[i].Index === index && !this.inGameCreatures[i].removing)
				return this.inGameCreatures[i];
		}
		return null;
	}

	getInsertionIndex(inGameCreature: InGameCreature): number {
		let numSkippable = 0;
		for (let i = 0; i < this.inGameCreatures.length; i++) {
			const thisCreature: InGameCreature = this.inGameCreatures[i];
			if (thisCreature.removing || thisCreature.justAdded)
				numSkippable++;

			if (inGameCreature.Index < thisCreature.Index)
				return i - numSkippable;
		}
		return this.inGameCreatures.length;
	}

	addInGameCreatures(inGameCreatures: InGameCreature[], soundManager: SoundManager) {
		console.log(`Add ${inGameCreatures.length} creature(s).`);
		for (let i = 0; i < inGameCreatures.length; i++) {
			const creatureDtoToAdd: InGameCreature = inGameCreatures[i];
			this.insertInGameCreature(creatureDtoToAdd);
		}

		this.repositionInGameScrolls();
		this.createAnimationsForRecentlyAddedCreatures(soundManager);
	}

	private insertInGameCreature(creatureDtoToAdd: InGameCreature) {
		const insertionIndex: number = this.getInsertionIndex(creatureDtoToAdd);
		console.log('insertionIndex: ' + insertionIndex);
		const newCreature: InGameCreature = new InGameCreature(creatureDtoToAdd);
		newCreature.justAdded = true;
		if (insertionIndex < this.inGameCreatures.length) { // Insert the new creature at the right spot.
			this.inGameCreatures.splice(insertionIndex, 0, newCreature);
		}
		else { // Add the new creature to the end of the list.
			this.inGameCreatures.push(newCreature);
		}
	}

	createAnimationsForRecentlyAddedCreatures(soundManager: SoundManager) {
		let delayMs = 0;
		let first = true;
		for (let i = this.inGameCreatures.length - 1; i >= 0; i--) { // Reverse order
			const inGameCreature: InGameCreature = this.inGameCreatures[i];

			const indexToCreatureStillInGame: number = this.getIndexToCreatureStillInGame(inGameCreature);
			const targetX: number = this.miniScrollLeftMargin + indexToCreatureStillInGame * this.miniScrollWidth;

			if (inGameCreature.justAdded) {
				inGameCreature.justAdded = false;
				this.addInGameCreature(inGameCreature, targetX, delayMs);
			}
			delayMs += InGameCreatureManager.timeBetweenMoves;

			if (first)
				soundManager.playMp3In(delayMs, 'InGameScrolls/InGameScrollAppearFirst');
			else
				soundManager.playMp3In(delayMs, 'InGameScrolls/InGameScrollAppear');

			first = false;
		}
	}

	static readonly timeBetweenMoves = 200;

	private repositionInGameScrolls() {
		let delayMs = 0;
		for (let i = this.inGameCreatures.length - 1; i >= 0; i--) { // Reverse order
			const inGameCreature: InGameCreature = this.inGameCreatures[i];
			const indexToCreatureStillInGame: number = this.getIndexToCreatureStillInGame(inGameCreature);
			const targetX: number = this.miniScrollLeftMargin + indexToCreatureStillInGame * this.miniScrollWidth;
			if (!inGameCreature.justAdded) {
				this.moveCreatureTo(inGameCreature, targetX, delayMs);
				delayMs += InGameCreatureManager.timeBetweenMoves;
			}
		}
	}

	getIndexToCreatureStillInGame(inGameCreature: InGameCreature): number {
		let index = 0;
		for (let i = 0; i < this.inGameCreatures.length; i++) {
			if (this.inGameCreatures[i] === inGameCreature)
				return index;
			if (!this.inGameCreatures[i].removing)
				index++;
		}
		return index;
	}
}
