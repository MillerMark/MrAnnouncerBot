enum HighlightEmitterType {
	circular,
	rectangular
}

enum DistributionOrientation {
	Horizontal,
	Vertical
}

class EmitterDistribution {
	orientation: DistributionOrientation;
	constructor(public centerX: number, public centerY: number, public width: number, public height: number, public spread: number) {
		this.orientation = DistributionOrientation.Vertical;
	}
}

class HighlightEmitter {
	width: number;
	height: number;
	radius: number;
	type: HighlightEmitterType;
	emitter: Emitter;

	constructor(public name: string, public center: Vector) {
		this.type = HighlightEmitterType.circular;
		this.radius = 3;
	}

	preUpdate(now: number, timeScale: number, world: World): any {
		if (this.emitter) {
			this.emitter.preUpdate(now, timeScale, world);
		}
	}

	update(now: number, timeScale: number, world: World): any {
		if (this.emitter) {
			this.emitter.update(now, timeScale, world);
		}
	}

	render(now: number, timeScale: number, world: World): void {
		if (this.emitter) {
			this.emitter.render(now, timeScale, world);
		}
	}

	setRectangular(width: number, height: number): HighlightEmitter {
		this.type = HighlightEmitterType.rectangular;
		this.width = width;
		this.height = height;
		return this;
	}

	setCircular(radius: number): HighlightEmitter {
		this.type = HighlightEmitterType.circular;
		this.radius = radius;
		return this;
	}

	start(): void {
		if (!this.emitter)
			this.emitter = HighlightEmitter.createBaseEmitter(this.center);

		if (this.type === HighlightEmitterType.circular) {
			this.emitter.radius = this.radius;
		}
		else {
			this.emitter.setRectShape(this.width, this.height);
		}

		const standardLength: number = 290;
		const idealParticlesPerSecond: number = 200;
		this.emitter.particlesPerSecond = idealParticlesPerSecond * this.getPerimeter() / standardLength;

		this.emitter.start();
	}

	stop() {
		if (this.emitter) {
			this.emitter.stop();
		}
	}

	getPerimeter(): number {
		if (this.type === HighlightEmitterType.circular) {
			return this.radius * MathEx.TWO_PI;
		}
		else {
			return 2 * this.width + 2 * this.height;
		}
	}

	static createBaseEmitter(center: Vector): Emitter {
		var emitter: Emitter;
		emitter = new Emitter(center);
		emitter.saturation.target = 0.9;
		emitter.saturation.relativeVariance = 0.2;
		emitter.hue = new TargetValue(40, 0, 0, 60);
		emitter.hue.absoluteVariance = 10;
		emitter.brightness.target = 0.7;
		emitter.brightness.relativeVariance = 0.5;
		emitter.particlesPerSecond = 300;
		emitter.particleRadius.target = 1;
		emitter.particleRadius.relativeVariance = 0.3;
		emitter.particleLifeSpanSeconds = 1.7;
		emitter.particleGravity = 0;
		emitter.particleInitialVelocity.target = 0.1;
		emitter.particleInitialVelocity.relativeVariance = 0.5;
		emitter.particleMaxOpacity = 0.8;
		emitter.particleAirDensity = 0;
		emitter.emitterEdgeSpread = 0.2;
		return emitter;
	}
}

class HighlightEmitterPages {
	emitters: Array<HighlightEmitter> = new Array<HighlightEmitter>();
	constructor() {

	}

	render(now: number, timeScale: number, world: World) {
		this.emitters.forEach(function (emitter: HighlightEmitter) {
			emitter.render(now, timeScale, world);
		});
	}

	find(itemID: string): HighlightEmitter {
		return this.emitters.find(s => s.name === itemID);
	}
}

class CharacterStatsScroll extends WorldObject {
	scrollOpenSfx: HTMLAudioElement;
	scrollWooshSfx: HTMLAudioElement;
	scrollPoofSfx: HTMLAudioElement;
	scrollCloseSfx: HTMLAudioElement;
	scrollSlamSfx: HTMLAudioElement;

	characters: Array<Character> = new Array<Character>();
	pages: Array<StatPage> = new Array<StatPage>();
	highlightEmitterPages: Array<HighlightEmitterPages> = new Array<HighlightEmitterPages>();

	selectedStatPageIndex: number = -1;

	deEmphasisSprite: SpriteProxy;
	currentScrollRoll: SpriteProxy;
	currentScrollBack: SpriteProxy;
	activeCharacter: Character = null;

	static readonly centerY: number = 424;
	static readonly centerX: number = 174;

	state: ScrollState = ScrollState.none;
	scrollRolls: Sprites;
	scrollPoofBack: Sprites;
	scrollPoofFront: Sprites;
	emphasisSprites: Sprites;
	emphasisIndices: Array<number> = [];
	emitterIndices: Array<string> = [];

	scrollEmphasisMain: Sprites;
	scrollEmphasisSkills: Sprites;
	scrollEmphasisEquipment: Sprites;
	scrollSlam: Sprites;
	scrollBacks: Sprites;
	playerHeadshots: Sprites;

	// TODO: consolidate with page, if possible.
	pageIndex: number;

	static readonly scrollOpenOffsets: number[] = [47,  // one extra at the beginning (to simplify code that looks at the previous/next offset)
		47, 48, 75, 106, 143, 215, 243, 267, 292, 315,  // 1-10
		333, 347, 360, 367, 380, 385, 395, 411, 415, 420,  // 11-20
		424, 428, 432,  // 21-23
		432]; // One extra at the end (to simplify code that looks at the previous/next offset)...

	private readonly framerateMs: number = 33; // 33 milliseconds == 30 fps
	topEmitter: Emitter;
	bottomEmitter: Emitter;

	// diagnostics:
	lastFrameIndex: number;
	lastElapsedTime: number;

	constructor() {
		super();
		this._page = ScrollPage.main;
		this.buildGoldDust();
		this.pageIndex = this._page;
		this.activeCharacter = null;
		this.deEmphasisSprite = new SpriteProxy(0, 0, 0);
		this.deEmphasisSprite.fadeInTime = 700;
		this.deEmphasisSprite.fadeOutTime = 700;
		this.deEmphasisSprite.expirationDate = activeBackGame.nowMs;
		for (var i = 0; i < 4; i++) {
			this.highlightEmitterPages.push(new HighlightEmitterPages());
		}
		this.addHighlightEmitters();
	}

	initializePlayerData(players: Array<Character>): void {
		
		this.characters = [];
		for (var i = 0; i < players.length; i++) {
			try {
				this.characters.push(new Character(players[i]));
			}
			catch (ex) {
				console.error('Unable to create new Character: ' + ex);
			}
		}
	}


	sendScrollLayerCommand(commandData: string): void {
		if (commandData == "Close")
			this.close();
		if (commandData == "ClearHighlighting")
			this.clearEmphasis();
	}

	private addHighlightEmitters() {
		const nameCenterX: number = 68;
		const nameCenterY: number = 127;
		const nameWidth: number = 104;
		const nameHeight: number = 28;

		// main page...
		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.NameHeadshot], new Vector(nameCenterX, nameCenterY)).setRectangular(nameWidth, nameHeight));

		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.RaceClass], new Vector(227, 37)).setRectangular(189, 24));

		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.Level], new Vector(157, 79)).setRectangular(51, 51));

		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.Inspiration], new Vector(225, 80)).setRectangular(51, 51));

		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.ExperiencePoints], new Vector(289, 80)).setRectangular(51, 51));

		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.Alignment], new Vector(225, 130)).setRectangular(189, 24));


		const acInitSpeedY: number = 196;
		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.ArmorClass], new Vector(134, acInitSpeedY)).setCircular(28));

		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.Initiative], new Vector(203, acInitSpeedY)).setRectangular(51, 51));

		const speedWidth: number = 60;
		const speedHeight: number = 53;
		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.Speed], new Vector(273, acInitSpeedY)).setRectangular(speedWidth, speedHeight));


		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.HitPoints], new Vector(139, 280)).setRectangular(67, 95));

		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.TempHitPoints], new Vector(139, 280)).setRectangular(67, 95));

		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.DeathSaves], new Vector(241, 277)).setRectangular(111, 90));


		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.HitDice], new Vector(204, 350)).setRectangular(197, 26));

		const profPercepGoldCenterY: number = 413;
		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.ProficiencyBonus], new Vector(123, profPercepGoldCenterY)).setCircular(31));


		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.Perception], new Vector(201, profPercepGoldCenterY)).setRectangular(77, 53));


		const goldWidth: number = 72;
		const goldHeight: number = 58;
		this.highlightEmitterPages[ScrollPage.main].emitters.push(
			new HighlightEmitter(emphasisMain[emphasisMain.GoldPieces], new Vector(288, profPercepGoldCenterY)).setRectangular(goldWidth, goldHeight));

		const abilityDistribution: EmitterDistribution = new EmitterDistribution(45, 207, 54, 84, 91);
		this.addDistribution(ScrollPage.main, emphasisMain.Strength, emphasisMain.Charisma, abilityDistribution);

		const savingThrowDistribution: EmitterDistribution = new EmitterDistribution(210, 481, 211, 36, 35);
		this.addDistribution(ScrollPage.main, emphasisMain.SavingStrength, emphasisMain.SavingCharisma, savingThrowDistribution);


		// skills page...

		this.highlightEmitterPages[ScrollPage.skills].emitters.push(
			new HighlightEmitter(emphasisSkills[emphasisSkills.NameHeadshot], new Vector(nameCenterX, nameCenterY)).setRectangular(nameWidth, nameHeight));

		this.highlightEmitterPages[ScrollPage.skills].emitters.push(
			new HighlightEmitter(emphasisSkills[emphasisSkills.Perception], new Vector(224, 50)).setRectangular(102, 54));

		this.highlightEmitterPages[ScrollPage.skills].emitters.push(
			new HighlightEmitter(emphasisSkills[emphasisSkills.ProficiencyBonus], new Vector(224, 115)).setRectangular(159, 54));

		this.addDistribution(ScrollPage.skills, emphasisSkills.Strength, emphasisSkills.Charisma, abilityDistribution);

		const skillsDistribution: EmitterDistribution = new EmitterDistribution(210, 188, 211, 29, 29);
		this.addDistribution(ScrollPage.skills, emphasisSkills.SkillsAcrobatics, emphasisSkills.SkillsSurvival, skillsDistribution);


		// equipment page...

		this.highlightEmitterPages[ScrollPage.equipment].emitters.push(
			new HighlightEmitter(emphasisEquipment[emphasisEquipment.NameHeadshot], new Vector(nameCenterX, nameCenterY)).setRectangular(nameWidth, nameHeight));

		const goldSpeedX: number = 172;
		this.highlightEmitterPages[ScrollPage.equipment].emitters.push(
			new HighlightEmitter(emphasisEquipment[emphasisEquipment.GoldPieces], new Vector(goldSpeedX, 52)).setRectangular(goldWidth, goldHeight));


		this.highlightEmitterPages[ScrollPage.equipment].emitters.push(
			new HighlightEmitter(emphasisEquipment[emphasisEquipment.Load], new Vector(262, 55)).setRectangular(71, 54));

		const speedWeightY: number = 113;
		this.highlightEmitterPages[ScrollPage.equipment].emitters.push(
			new HighlightEmitter(emphasisEquipment[emphasisEquipment.Speed], new Vector(goldSpeedX, speedWeightY)).setRectangular(speedWidth, speedHeight));

		this.highlightEmitterPages[ScrollPage.equipment].emitters.push(
			new HighlightEmitter(emphasisEquipment[emphasisEquipment.Weight], new Vector(261, speedWeightY)).setRectangular(80, 51));
	}

	private addDistribution(page: ScrollPage, first: number, last: number, dist: EmitterDistribution) {
		let x: number = dist.centerX;
		let y: number = dist.centerY;
		for (var i = first; i <= last; i++) {
			this.addRectangularHighlight(page, i, x, y, dist.width, dist.height);
			if (dist.orientation === DistributionOrientation.Vertical)
				y += dist.spread;
			else
				x += dist.spread;
		}
	}

	addRectangularHighlight(scrollPage: ScrollPage, enumIndex: number, centerX: number, centerY: number, width: number, height: number): any {
		let name: string = this.getNameFromIndex(scrollPage, enumIndex);
		if (name === null)
			return;
		this.highlightEmitterPages[scrollPage].emitters.push(new HighlightEmitter(name, new Vector(centerX, centerY)).setRectangular(width, height));
	}

	needImmediateReopen: boolean;

	private _page: ScrollPage;

	get page(): ScrollPage {
		return this._page;
	}

	set page(newValue: ScrollPage) {
		if (this._page !== newValue) {
			this.needImmediateReopen = true;
			this.close();
			this._page = newValue;
		}
	}

	private getNameFromIndex(scrollPage: ScrollPage, enumIndex: number): any {
		if (scrollPage === ScrollPage.main)
			return emphasisMain[enumIndex];
		else if (scrollPage === ScrollPage.skills)
			return emphasisSkills[enumIndex];
		else if (scrollPage === ScrollPage.equipment)
			return emphasisEquipment[enumIndex];
		return null;
	}

	slam(): void {
		this.play(this.scrollWooshSfx);
		this.scrollSlam.add(0, 0, 0);
		this.state = ScrollState.slamming;
	}

	open(now: number): void {
		switch (this.state) {
			case ScrollState.none:
				this.slam();
				break;
			case ScrollState.closed:
				this.unroll();
				break;
			//case ScrollState.unrolled:
			//  this.close(now);
			//  break;
		}
	}

	close(): any {
		this.state = ScrollState.closing;
		this.scrollRolls.baseAnimation.reverse = true;
		this.play(this.scrollCloseSfx);
	}

	get headshotIndex(): number {
		if (this.activeCharacter)
			return this.activeCharacter.headshotIndex;
		return 0;
	}
	
	

	private unroll(): void {
		this.state = ScrollState.unrolling;
		this.scrollRolls.baseAnimation.reverse = false;
		this.scrollRolls.sprites = [];
		this.scrollBacks.sprites = [];
		this.currentScrollRoll = this.scrollRolls.add(0, 0, 0);
		this.currentScrollBack = this.scrollBacks.add(0, 0, this._page);
		this.playerHeadshots.sprites = [];
		this.playerHeadshots.add(0, 0, this.headshotIndex);
		this.pageIndex = this._page;
		this.selectedStatPageIndex = this._page - 1;
		this.topEmitter.start();
		this.bottomEmitter.start();
		this.play(this.scrollOpenSfx);

		if (this.emphasisSprites) {
			while (this.emphasisIndices.length > 0) {
				this.emphasisSprites.add(0, 0, this.emphasisIndices.pop()).setFadeTimes(this.fadeTime, this.fadeTime);
			}
		}
	}

	play(audio: HTMLAudioElement): any {
		const playPromise = audio.play();
		if (playPromise["[[PromiseStatus]]"] === 'rejected') {
			console.log(playPromise["[[PromiseValue]]"].message);
		}
	}

	preUpdate(now: number, timeScale: number, world: World): void {
		super.preUpdate(now, timeScale, world);
		this.topEmitter.preUpdate(now, timeScale, world);
		this.bottomEmitter.preUpdate(now, timeScale, world);
		this.highlightEmitterPages[this.page].emitters.forEach(function (highlightEmitter: HighlightEmitter) {
			highlightEmitter.preUpdate(now, timeScale, world);
		});
	}

	static readonly showEmitters: boolean = true; // Related to perf issue.

	update(now: number, timeScale: number, world: World): void {
		super.update(now, timeScale, world);
		if (CharacterStatsScroll.showEmitters) {
			this.topEmitter.update(now, timeScale, world);
			this.bottomEmitter.update(now, timeScale, world);
		}

		if (this.state === ScrollState.slammed) {
			this.unroll(); // do we queue?
		}

		if (CharacterStatsScroll.showEmitters) {
			this.highlightEmitterPages[this.page].emitters.forEach(function (highlightEmitter: HighlightEmitter) {
				highlightEmitter.update(now, timeScale, world);
			});
		}
	}

	scrollIsVisible(): boolean {
		return this.state === ScrollState.unrolling || this.state === ScrollState.unrolled ||
			this.state === ScrollState.closing || this.state === ScrollState.closed ||
			this.state === ScrollState.paused;
	}

	render(now: number, timeScale: number, world: World): void {
		super.render(now, timeScale, world);

		let justClosed: boolean = false;

		const picX: number = 17;
		const picWidth: number = 104;
		const picY: number = 24;
		const picHeight: number = 90;

		if (this.scrollIsVisible() && this.scrollRolls.sprites.length != 0) {
			let elapsedTime: number = now - this.scrollRolls.lastTimeWeAdvancedTheFrame / 1000;
			let scrollRollsFrameIndex: number = this.scrollRolls.sprites[0].frameIndex;

			if (this.state === ScrollState.paused) {
				scrollRollsFrameIndex = this.lastFrameIndex;
				elapsedTime = this.lastElapsedTime;
			}

			let nextFrameIndex: number;

			let stillAnimating: boolean;

			if (this.state === ScrollState.closing || this.state === ScrollState.closed) {
				stillAnimating = scrollRollsFrameIndex > 0;
				nextFrameIndex = scrollRollsFrameIndex - 1;
			}
			else { // opening...
				stillAnimating = scrollRollsFrameIndex < 22;
				nextFrameIndex = scrollRollsFrameIndex + 1;
			}

			let frameFraction: number = elapsedTime / (this.framerateMs / 1000);
			const maxFrameFraction: number = 0.99999999;
			if (frameFraction > maxFrameFraction) {
				frameFraction = maxFrameFraction;
			}

			if (this.state === ScrollState.closing)
				frameFraction = 1 - frameFraction;

			let topData: number = 0;
			let bottomData: number = Infinity;

			let frameIndexPrecise: number = scrollRollsFrameIndex + frameFraction;

			if (stillAnimating || this.state === ScrollState.closing || this.state === ScrollState.closed) {
				let offset: number = CharacterStatsScroll.scrollOpenOffsets[scrollRollsFrameIndex + 1];

				let decimalOffset: number = frameIndexPrecise - scrollRollsFrameIndex;
				let distanceBetweenOffsets: number = CharacterStatsScroll.scrollOpenOffsets[nextFrameIndex + 1] - offset;

				let superPreciseOffset: number = offset + distanceBetweenOffsets * decimalOffset;

				let baseAnim: Part = this.scrollBacks.baseAnimation;
				let sw = this.scrollBacks.spriteWidth;
				let dx: number = 0;
				let dh: number = offset * 2;
				let dy = CharacterStatsScroll.centerY - offset;
				topData = dy;
				bottomData = dy + dh;
				let sh: number = dh;
				let dw: number = sw;
				let sx: number = dx;
				let sy: number = dy;

				baseAnim.drawCroppedByIndex(world.ctx, sx, sy, this.pageIndex, dx, dy, sw, sh, sw, dh);

				let picOffsetY: number = topData - picY;
				let picCroppedHeight: number = picY + picHeight - topData;

				if (picCroppedHeight > 0) {
					// ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)

					this.playerHeadshots.baseAnimation.drawCroppedByIndex(world.ctx, picX, picY + picOffsetY, this.headshotIndex, 0, picOffsetY, picWidth, picCroppedHeight, picWidth, picCroppedHeight);
				}
				this.drawHighlighting(now, timeScale, world, sx, sy, dx, dy, sw, sh, dw, dh);

				//if (this.state === ScrollState.closing && frameIndex <= 7) {
				//  this.state = ScrollState.paused;
				//  this.scrollRolls.animationStyle = AnimationStyle.Static;
				//  this.lastFrameIndex = frameIndex;
				//  this.lastElapsedTime = elapsedTime;
				//}

				if (scrollRollsFrameIndex === 0 && this.state === ScrollState.unrolling) {
					this.topEmitter.start();
					this.bottomEmitter.start();
				}

				if (scrollRollsFrameIndex === 0 && this.state === ScrollState.closing) {
					justClosed = true;
				}

				this.topEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY - superPreciseOffset);
				this.bottomEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY + superPreciseOffset);
			}
			else {
				this.scrollBacks.draw(world.ctx, now * 1000);
				this.playerHeadshots.baseAnimation.drawByIndex(world.ctx, picX, picY, this.headshotIndex);
				this.drawHighlighting(now, timeScale, world);
				this.topEmitter.stop();
				this.bottomEmitter.stop();
				this.state = ScrollState.unrolled;

				this.startQueuedEmitters();
			}

			this.topEmitter.render(now, timeScale, world);
			this.bottomEmitter.render(now, timeScale, world);

			this.drawCharacterStats(world.ctx, topData, bottomData);

			this.scrollRolls.draw(world.ctx, now * 1000);

		}
		else if (this.state === ScrollState.disappearing) {
			this.scrollRolls.draw(world.ctx, now * 1000);
			this.scrollPoofBack.draw(world.ctx, now * 1000);
			this.scrollPoofFront.draw(world.ctx, now * 1000);
			if (this.scrollPoofFront.sprites.length == 0) {
				console.error("this.scrollPoofFront.sprites.length == 0");
			}
			else {
				let poofFrameIndex: number = this.scrollPoofFront.sprites[0].frameIndex;
				if (poofFrameIndex === 0) {
					this.state = ScrollState.none;
				}
			}
		}
		else {
			// console.log('Scroll is not visible!');
		}

		if (this.state === ScrollState.slamming) {
			this.scrollSlam.draw(world.ctx, now * 1000);
			if (this.scrollSlam.sprites.length == 0) {
				console.error("this.scrollSlam.sprites.length == 0");
			}
			else if (this.scrollSlam.sprites[0].frameIndex === this.scrollSlam.baseAnimation.frameCount - 1) {
				this.state = ScrollState.slammed;
				this.play(this.scrollSlamSfx);
			}
		}

		if (justClosed) {
			if (this.needImmediateReopen) {
				this.state = ScrollState.closed;
				this.open(now);
				this.needImmediateReopen = false;
			}
			else
				this.makeScrollDisappear(now * 1000);
		}
	}

	makeScrollDisappear(nowMs: number): void {
		const fadeOutTime: number = 450;
		if (this.currentScrollRoll) {
			this.currentScrollRoll.expirationDate = nowMs + fadeOutTime;
			this.currentScrollRoll.fadeOutTime = fadeOutTime;
		}
		if (this.currentScrollBack) {
			this.currentScrollBack.expirationDate = nowMs + fadeOutTime;
			this.currentScrollBack.fadeOutTime = fadeOutTime;
		}

		this.currentScrollRoll = null;
		this.currentScrollBack = null;

		const scrollCenterX: number = 170;
		const scrollCenterY: number = 440;

		let hueShift: number = 0;
		if (this.activeCharacter) {
			hueShift = this.activeCharacter.hueShift;
		}
		this.scrollPoofBack.addShifted(scrollCenterX, scrollCenterY, 0, hueShift);
		this.scrollPoofFront.add(scrollCenterX, scrollCenterY, 0);
		this.play(this.scrollPoofSfx);
		this.state = ScrollState.disappearing;
	}

	startQueuedEmitters(): void {
		while (this.emitterIndices.length > 0) {
			let emitter: HighlightEmitter = this.highlightEmitterPages[this.page].find(this.emitterIndices.pop());
			if (emitter) {
				emitter.start();
			}
		}
	}

	drawHighlighting(now: number, timeScale: number, world: World, sx: number = 0, sy: number = 0, dx: number = 0, dy: number = 0, sw: number = 0, sh: number = 0, dw: number = 0, dh: number = 0): void {
		if (!this.weAreHighlighting(now))
			return;

		this.drawDeEmphasisLayer(world, now, sx, sy, dx, dy, sw, sh, dw, dh);

		this.highlightEmitterPages[this.page].emitters.forEach(function (highlightEmitter: HighlightEmitter) {
			highlightEmitter.render(now, timeScale, world);
		});

		if (dh > 0) {
			if (this.page === ScrollPage.main)
				this.scrollEmphasisMain.drawCropped(world.ctx, now * 1000, sx, sy, dx, dy, sw, sh, dw, dh);
			if (this.page === ScrollPage.skills)
				this.scrollEmphasisSkills.drawCropped(world.ctx, now * 1000, sx, sy, dx, dy, sw, sh, dw, dh);
			if (this.page === ScrollPage.equipment)
				this.scrollEmphasisEquipment.drawCropped(world.ctx, now * 1000, sx, sy, dx, dy, sw, sh, dw, dh);
		}
		else {
			if (this.page === ScrollPage.main)
				this.scrollEmphasisMain.draw(world.ctx, now * 1000);
			if (this.page === ScrollPage.skills)
				this.scrollEmphasisSkills.draw(world.ctx, now * 1000);
			if (this.page === ScrollPage.equipment)
				this.scrollEmphasisEquipment.draw(world.ctx, now * 1000);
		}
	}

	drawDeEmphasisLayer(world: World, now: number, sx: number = 0, sy: number = 0, dx: number = 0, dy: number = 0, sw: number = 0, sh: number = 0, dw: number = 0, dh: number = 0): void {
		let newAlpha: number = this.deEmphasisSprite.getAlpha(now * 1000);
		world.ctx.globalAlpha = newAlpha;
		if (dh > 0) {
			this.scrollBacks.baseAnimation.drawCroppedByIndex(world.ctx, dx, dy, 0, sx, sy, sw, sh, dw, dh);
		}
		else {
			this.scrollBacks.baseAnimation.drawByIndex(world.ctx, 0, 0, 0);
		}
		world.ctx.globalAlpha = 1;
	}

	weAreHighlighting(now: number): boolean {
		return this.currentlyEmphasizing() || this.deEmphasisSprite.expirationDate > now * 1000;
	}

	buildGoldDust(): void {
		let windSpeed: number = 0.8;

		this.topEmitter = this.getMagicDustEmitter(windSpeed);
		this.bottomEmitter = this.getMagicDustEmitter(-windSpeed);

		this.topEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY - CharacterStatsScroll.scrollOpenOffsets[1]);
		this.bottomEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY + CharacterStatsScroll.scrollOpenOffsets[1]);
	}

	private getMagicDustEmitter(windSpeed: number): Emitter {
		const scrollLength: number = 348;
		let emitter: Emitter = new Emitter(new Vector(scrollLength / 2, 460));
		emitter.stop();
		emitter.setRectShape(scrollLength, 1);
		emitter.saturation.target = 0.9;
		emitter.saturation.relativeVariance = 0.2;
		emitter.hue = new TargetValue(40, 0, 0, 60);
		emitter.hue.absoluteVariance = 10;
		emitter.brightness.target = 0.7;
		emitter.brightness.relativeVariance = 0.5;
		emitter.particlesPerSecond = 1250;
		emitter.maxTotalParticles = 3000;
		emitter.particleRadius.target = 0.8;
		emitter.particleRadius.relativeVariance = 2;
		emitter.particleLifeSpanSeconds = 1;
		emitter.particleGravity = 0;
		emitter.particleInitialVelocity.target = 0.4;
		emitter.particleInitialVelocity.relativeVariance = 0.5;
		emitter.initialParticleDirection = new Vector(0, Math.sign(windSpeed));
		emitter.gravity = 0;
		emitter.airDensity = 0.2;
		emitter.particleAirDensity = 1;
		emitter.particleWind = new Vector(0, windSpeed);

		return emitter;
	}

	loadResources(): void {
		var assetFolderName: string = Folders.assets;
		Folders.assets = 'GameDev/Assets/DragonH/';

		this.scrollRolls = new Sprites("Scroll/Open/ScrollOpen", 23, this.framerateMs, AnimationStyle.SequentialStop, true);
		this.scrollPoofBack = new Sprites("Scroll/Close/ScrollPoof", 44, this.framerateMs, AnimationStyle.Sequential, true);
		this.scrollPoofFront = new Sprites("Scroll/Close/ScrollFire", 63, this.framerateMs, AnimationStyle.Sequential, true);
		this.scrollPoofFront.originX = 370;
		this.scrollPoofFront.originY = 325;
		this.scrollPoofBack.originX = 370;
		this.scrollPoofBack.originY = 325;

		this.scrollSlam = new Sprites("Scroll/Slam/Slam", 8, this.framerateMs, AnimationStyle.Sequential, true);
		this.scrollBacks = new Sprites("Scroll/Backs/Back", 4, this.framerateMs, AnimationStyle.Static);
		const totalKnownPlayers: number = 6;
		this.playerHeadshots = new Sprites("Scroll/Players/Player", totalKnownPlayers, this.framerateMs, AnimationStyle.Static);
		this.scrollEmphasisMain = new Sprites("Scroll/Emphasis/Main/EmphasisMain", 27, this.framerateMs, AnimationStyle.Static);
		this.scrollEmphasisSkills = new Sprites("Scroll/Emphasis/Skills/EmphasisSkills", 27, this.framerateMs, AnimationStyle.Static);
		this.scrollEmphasisEquipment = new Sprites("Scroll/Emphasis/Equipment/EmphasisEquipment", 5, this.framerateMs, AnimationStyle.Static);

		this.scrollWooshSfx = new Audio(Folders.assets + 'SoundEffects/scrollWoosh.mp3');
		this.scrollOpenSfx = new Audio(Folders.assets + 'SoundEffects/scrollOpen.mp3');
		this.scrollCloseSfx = new Audio(Folders.assets + 'SoundEffects/scrollClose.mp3');
		this.scrollSlamSfx = new Audio(Folders.assets + 'SoundEffects/scrollSlam.mp3');
		this.scrollPoofSfx = new Audio(Folders.assets + 'SoundEffects/scrollPoof.mp3');

		Folders.assets = assetFolderName;
	}

	drawCharacterStats(context: CanvasRenderingContext2D, topData: number, bottomData: number): void {
		if (!this.activeCharacter)
			return;

		if (this.selectedStatPageIndex < 0)
			return;
		let activeCharacter: Character = this.activeCharacter;
		let activePage: StatPage = this.pages[this.selectedStatPageIndex];

		activePage.render(context, activeCharacter, topData, bottomData);
	}

	clear(): any {
		this.characters = [];
		this.pages = [];
		this.activeCharacter = null;
		this.selectedStatPageIndex = -1;
		this.state = ScrollState.none;
	}

	playerDataChanged(playerID: number, pageID: number, playerData: string): boolean {
		//console.log(`playerDataChanged(${playerID}, ${pageID}, ${playerData})`);

		let okayToChangePage: boolean = pageID !== -1;
		let newPageId: number = pageID;
		if (newPageId === -1)
			newPageId = this._page;

		let changedActiveCharacter: boolean = false;
		if (!this.activeCharacter || this.activeCharacter.playerID !== playerID) {
			this.clearEmphasis();
			if (okayToChangePage)
			{
				this.setActiveCharacter(playerID);
				changedActiveCharacter = true;
				this._page = newPageId;
				this.state = ScrollState.none;
				this.open(performance.now());
			}
		}
		else if (this.page != newPageId) {
			this.clearEmphasis();

			if (okayToChangePage)
			{
				this.state = ScrollState.none;
				this.page = newPageId;
				this.open(performance.now());
			}
		}

		if (playerData != '') {
			this.updatePlayerData(playerData);
		}
		return changedActiveCharacter;
	}

	setActiveCharacter(playerID: number): void {
		for (var i = 0; i < this.characters.length; i++) {
			let thisCharacter: Character = this.characters[i];
			if (thisCharacter.playerID == playerID) {
				this.activeCharacter = thisCharacter;
				return;
			}
		}
	}

	getCharacter(playerID: number): Character {
		for (var i = 0; i < this.characters.length; i++) {
			let thisCharacter: Character = this.characters[i];
			if (thisCharacter.playerID == playerID) {
				return thisCharacter;
			}
		}
		return null;
	}


	updatePlayerData(playerData: string): Character {
		let sentChar: any = JSON.parse(playerData);
		let character: Character = this.getCharacter(sentChar.playerID);
		if (character != null) {
			character.copyAttributesFrom(sentChar);
		}
		return character;
		//this.setActiveCharacter(sentChar.playerID);

		//if (!this.activeCharacter) {
		//	console.error('No active character: ' + this.activeCharacter);
		//	return;
		//}

		// this.activeCharacter.copyAttributesFrom(sentChar);
		//return this.activeCharacter;
	}

	readonly fadeTime: number = 300;

	private clearEmphasis() {
		this.scrollEmphasisMain.sprites = [];
		this.scrollEmphasisSkills.sprites = [];
		this.scrollEmphasisEquipment.sprites = [];
	}

	addEmphasis(emphasisIndex: number): void {
		if (this.page === ScrollPage.main)
			this.scrollEmphasisMain.add(0, 0, emphasisIndex).setFadeTimes(this.fadeTime, this.fadeTime);
		else if (this.page === ScrollPage.skills)
			this.scrollEmphasisSkills.add(0, 0, emphasisIndex).setFadeTimes(this.fadeTime, this.fadeTime);
		else if (this.page === ScrollPage.equipment)
			this.scrollEmphasisEquipment.add(0, 0, emphasisIndex).setFadeTimes(this.fadeTime, this.fadeTime);;
	}


	addParticleEmphasis(itemID: string): void {
		let emitter: HighlightEmitter = this.highlightEmitterPages[this.page].find(itemID);
		if (emitter) {
			emitter.start();
		}
	}

	queueParticleEmphasis(itemID: string): void {
		this.emitterIndices.push(itemID);
	}



	removeParticleEmphasis(itemID: string): void {
		let emitter: HighlightEmitter = this.highlightEmitterPages[this.page].find(itemID);
		if (emitter) {
			emitter.stop();
		}
	}



	currentlyEmphasizing(): boolean {
		let now: number = activeBackGame.nowMs;
		return this.scrollEmphasisMain.hasAnyAlive(now) ||
			this.scrollEmphasisSkills.hasAnyAlive(now) ||
			this.scrollEmphasisEquipment.hasAnyAlive(now);
	}

	focusItem(playerID: number, pageID: number, itemID: string): void {
		let previouslyEmphasizing: boolean = this.currentlyEmphasizing();
		if (pageID === this.pageIndex)
			this.addParticleEmphasis(itemID);
		else
			this.queueParticleEmphasis(itemID);

		this.emphasisSprites = null;

		if (pageID === ScrollPage.main) {
			this.emphasisSprites = this.scrollEmphasisMain;
			this.emphasisIndices.push(emphasisMain[itemID]);
		}
		else if (pageID === ScrollPage.skills) {
			this.emphasisSprites = this.scrollEmphasisSkills;
			this.emphasisIndices.push(emphasisSkills[itemID]);
		}
		else if (pageID === ScrollPage.equipment) {
			this.emphasisSprites = this.scrollEmphasisEquipment;
			this.emphasisIndices.push(emphasisEquipment[itemID]);
		}

		if (this.emphasisSprites)
			if (pageID === this.pageIndex)
				this.emphasisSprites.add(0, 0, this.emphasisIndices.pop()).setFadeTimes(this.fadeTime, this.fadeTime);

		let currentlyEmphasizing: boolean = this.currentlyEmphasizing();

		if (currentlyEmphasizing && !previouslyEmphasizing && activeBackGame) {
			if (this.deEmphasisSprite.expirationDate > activeBackGame.nowMs) {
				this.deEmphasisSprite.expirationDate = activeBackGame.nowMs;
			}
			else {
				this.deEmphasisSprite.timeStart = activeBackGame.nowMs;
			}
		}

		//console.log(`focusItem(${playerID}, ${pageID}, ${itemID})`);
	}

	unfocusItem(playerID: number, pageID: number, itemID: string): void {
		//console.log(`unfocusItem(${playerID}, ${pageID}, ${itemID})`);
		let previouslyEmphasizing: boolean = this.currentlyEmphasizing();

		this.removeParticleEmphasis(itemID);

		if (pageID === ScrollPage.main) {
			let emphasisIndex: number = emphasisMain[itemID];
			this.scrollEmphasisMain.killByFrameIndex(emphasisIndex, activeBackGame.nowMs);
			this.scrollEmphasisSkills.sprites = [];
			this.scrollEmphasisEquipment.sprites = [];
		}
		else if (pageID === ScrollPage.skills) {
			let emphasisIndex: number = emphasisSkills[itemID];
			this.scrollEmphasisSkills.killByFrameIndex(emphasisIndex, activeBackGame.nowMs);
			this.scrollEmphasisMain.sprites = [];
			this.scrollEmphasisEquipment.sprites = [];
		}
		else if (pageID === ScrollPage.equipment) {
			let emphasisIndex: number = emphasisEquipment[itemID];
			this.scrollEmphasisEquipment.killByFrameIndex(emphasisIndex, activeBackGame.nowMs);
			this.scrollEmphasisMain.sprites = [];
			this.scrollEmphasisSkills.sprites = [];
		}

		let currentlyEmphasizing: boolean = this.currentlyEmphasizing();

		if (!currentlyEmphasizing && previouslyEmphasizing && activeBackGame) {
			this.deEmphasisSprite.expirationDate = activeBackGame.nowMs + this.deEmphasisSprite.fadeOutTime;
		}

	}
} 