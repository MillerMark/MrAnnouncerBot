class CharacterStatsScroll extends WorldObject {

	setSoundManager(dragonBackSounds: DragonBackSounds): any {
		this.spellBook.setSoundManager(dragonBackSounds);
	}
	scrollOpenSfx: HTMLAudioElement;
	scrollWooshSfx: HTMLAudioElement;
	scrollPoofSfx: HTMLAudioElement;
	scrollCloseSfx: HTMLAudioElement;
	scrollSlamSfx: HTMLAudioElement;

	characters: Array<Character> = new Array<Character>();
	pages: Array<StatPage> = new Array<StatPage>();
	highlightEmitterPages: Array<HighlightEmitterPages> = new Array<HighlightEmitterPages>();

	selectedStatPageIndex = -1;

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
	concentrationIcons: Sprites;
	spinningConcentrationIcons: Sprites;
	spellItemIcons: Sprites;
	morePowerIcons: Sprites;
	emphasisIndices: Array<number> = [];
	emitterIndices: Array<string> = [];
	spellBook: SpellBook;

	scrollEmphasisMain: Sprites;
	scrollEmphasisSkills: Sprites;
	scrollEmphasisEquipment: Sprites;
	scrollEmphasisSpells: Sprites;
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
	morePowerIcon: SpriteProxy;
	concentrationIcon: SpriteProxy;
	spinningConcentrationIcon: SpriteProxy;
	spellItemIcon: SpriteProxy;
	spellIconYOffset = 0;
	browserIsOBS = true;

	constructor() {
		super();
		this.spellBook = new SpellBook();

		this.browserIsOBS = browserIsOBS();

		if (this.browserIsOBS)
			this.spellIconYOffset = 4;

		this._page = ScrollPage.main;
		this.buildGoldDust();
		this.pageIndex = this._page;
		this.activeCharacter = null;
		this.deEmphasisSprite = new SpriteProxy(0, 0, 0);
		this.deEmphasisSprite.fadeInTime = 700;
		this.deEmphasisSprite.fadeOutTime = 700;
		this.deEmphasisSprite.expirationDate = activeBackGame.nowMs;

		const numPagesPlusShadow = 5;
		for (let i = 0; i < numPagesPlusShadow; i++) {
			this.highlightEmitterPages.push(new HighlightEmitterPages());
		}
		this.addHighlightEmitters();
	}

	initializePlayerData(players: Array<Character>): void {

		this.characters = [];
		for (let i = 0; i < players.length; i++) {
			try {
				this.characters.push(new Character(players[i]));
			}
			catch (ex) {
				console.error('Unable to create new Character: ' + ex);
			}
		}
	}


	sendScrollLayerCommand(commandData: string): void {
		if (commandData === "Close") {
			this.needImmediateReopen = false;
			this.close();
		}
		if (commandData === "ClearHighlighting")
			this.clearEmphasis();
	}

	private addHighlightEmitters() {
		const nameCenterX = 68;
		const nameCenterY = 127;
		const nameWidth = 104;
		const nameHeight = 28;

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


		// spells page...
		this.highlightEmitterPages[ScrollPage.spells].emitters.push(
			new HighlightEmitter(emphasisSpells[emphasisSpells.NameHeadshot], new Vector(nameCenterX, nameCenterY)).setRectangular(nameWidth, nameHeight));

		this.highlightEmitterPages[ScrollPage.spells].emitters.push(
			new HighlightEmitter(emphasisSpells[emphasisSpells.SpellcastingAbility], new Vector(223, 52)).setRectangular(186, 50));

		let spellSaveAttackY: number = 112;
		let spellSaveAttackHeight: number = 50;

		this.highlightEmitterPages[ScrollPage.spells].emitters.push(
			new HighlightEmitter(emphasisSpells[emphasisSpells.SpellAttackBonus], new Vector(172, spellSaveAttackY)).setRectangular(83, spellSaveAttackHeight));

		this.highlightEmitterPages[ScrollPage.spells].emitters.push(
			new HighlightEmitter(emphasisSpells[emphasisSpells.SpellSaveDC], new Vector(269, spellSaveAttackY + 2)).setRectangular(97, spellSaveAttackHeight));

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
		const name: string = this.getNameFromIndex(scrollPage, enumIndex);
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

			if (this.state !== ScrollState.disappearing && this.state !== ScrollState.slamming)
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
		else if (scrollPage === ScrollPage.spells)
			return emphasisSpells[enumIndex];
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

	close() {
		this.state = ScrollState.closing;
		this.scrollRolls.baseAnimation.reverse = true;
		this.play(this.scrollCloseSfx);
	}

	get headshotIndex(): number {
		if (this.activeCharacter)
			return this.activeCharacter.headshotIndex;
		return 0;
	}

	showingGoldDustEmitters = true;

	private unroll(): void {
		this.state = ScrollState.unrolling;
		this.scrollRolls.baseAnimation.reverse = false;
		this.scrollRolls.spriteProxies = [];
		this.scrollBacks.spriteProxies = [];
		this.currentScrollRoll = this.scrollRolls.add(0, 0, 0);
		this.currentScrollBack = this.scrollBacks.add(0, 0, this._page);
		this.playerHeadshots.spriteProxies = [];
		this.playerHeadshots.add(0, 0, this.headshotIndex);
		this.pageIndex = this._page;
		this.selectedStatPageIndex = this._page - 1;
		if (this.showingGoldDustEmitters) {
			this.topEmitter.start();
			this.bottomEmitter.start();
		}
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
		if (this.showingGoldDustEmitters) {
			this.topEmitter.preUpdate(now, timeScale, world);
			this.bottomEmitter.preUpdate(now, timeScale, world);
			this.highlightEmitterPages[this.page].emitters.forEach(function (highlightEmitter: HighlightEmitter) {
				highlightEmitter.preUpdate(now, timeScale, world);
			});
		}
	}

	update(now: number, timeScale: number, world: World): void {
		super.update(now, timeScale, world);
		if (this.showingGoldDustEmitters) {
			this.topEmitter.update(now, timeScale, world);
			this.bottomEmitter.update(now, timeScale, world);
		}

		if (this.state === ScrollState.slammed) {
			this.unroll(); // do we queue?
		}

		if (this.showingGoldDustEmitters) {
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

	makeScrollDisappear(nowMs: number): void {
		const fadeOutTime = 450;
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

		const scrollCenterX = 170;
		const scrollCenterY = 440;

		let hueShift = 0;
		if (this.activeCharacter) {
			hueShift = this.activeCharacter.hueShift;
		}
		this.scrollPoofBack.addShifted(scrollCenterX, scrollCenterY, 0, hueShift);
		this.scrollPoofFront.add(scrollCenterX, scrollCenterY, 0);
		this.play(this.scrollPoofSfx);
		this.state = ScrollState.disappearing;
	}

	private startQueuedEmitters(): void {
		if (this.showingGoldDustEmitters) {
			while (this.emitterIndices.length > 0) {
				const emitter: HighlightEmitter = this.highlightEmitterPages[this.page].find(this.emitterIndices.pop());
				if (emitter) {
					emitter.start();
				}
			}
		}
	}

	drawHighlighting(now: number, timeScale: number, world: World, sx: number = 0, sy: number = 0, dx: number = 0, dy: number = 0, sw: number = 0, sh: number = 0, dw: number = 0, dh: number = 0): void {
		if (!this.weAreHighlighting(now))
			return;

		this.drawDeEmphasisLayer(world, now, sx, sy, dx, dy, sw, sh, dw, dh);

		if (this.showingGoldDustEmitters) {
			this.highlightEmitterPages[this.page].emitters.forEach(function (highlightEmitter: HighlightEmitter) {
				highlightEmitter.render(now, timeScale, world);
			});
		}

		if (dh > 0) {
			if (this.page === ScrollPage.main)
				this.scrollEmphasisMain.drawCropped(world.ctx, now * 1000, sx, sy, dx, dy, sw, sh, dw, dh);
			if (this.page === ScrollPage.skills)
				this.scrollEmphasisSkills.drawCropped(world.ctx, now * 1000, sx, sy, dx, dy, sw, sh, dw, dh);
			if (this.page === ScrollPage.equipment)
				this.scrollEmphasisEquipment.drawCropped(world.ctx, now * 1000, sx, sy, dx, dy, sw, sh, dw, dh);
			if (this.page === ScrollPage.spells)
				this.scrollEmphasisSpells.drawCropped(world.ctx, now * 1000, sx, sy, dx, dy, sw, sh, dw, dh);
		}
		else {
			if (this.page === ScrollPage.main)
				this.scrollEmphasisMain.draw(world.ctx, now * 1000);
			if (this.page === ScrollPage.skills)
				this.scrollEmphasisSkills.draw(world.ctx, now * 1000);
			if (this.page === ScrollPage.equipment)
				this.scrollEmphasisEquipment.draw(world.ctx, now * 1000);
			if (this.page === ScrollPage.spells)
				this.scrollEmphasisSpells.draw(world.ctx, now * 1000);
		}
	}

	drawDeEmphasisLayer(world: World, now: number, sx = 0, sy = 0, dx = 0, dy = 0, sw = 0, sh = 0, dw = 0, dh = 0): void {
		const newAlpha: number = this.deEmphasisSprite.getAlpha(now * 1000);
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
		const windSpeed = 0.8;

		this.topEmitter = this.getMagicDustScrollOpenEmitter(windSpeed);
		this.bottomEmitter = this.getMagicDustScrollOpenEmitter(-windSpeed);

		this.topEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY - CharacterStatsScroll.scrollOpenOffsets[1]);
		this.bottomEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY + CharacterStatsScroll.scrollOpenOffsets[1]);
	}

	private getMagicDustScrollOpenEmitter(windSpeed: number): Emitter {
		const scrollLength = 348;
		const emitter: Emitter = this.getMagicDustEmitter(scrollLength / 2, 460);

		emitter.particleWind = new Vector(0, windSpeed);
		emitter.initialParticleDirection = new Vector(0, Math.sign(windSpeed));
		emitter.setRectShape(scrollLength, 1);

		return emitter;
	}

	private getMagicDustEmitter(x: number, y: number) {
		const emitter: Emitter = new Emitter(new Vector(x, y));
		emitter.stop();
		emitter.saturation.target = 0.9;
		emitter.saturation.relativeVariance = 0.2;
		emitter.hue = new TargetValue(40, 0, 0, 60);
		emitter.hue.absoluteVariance = 10;
		emitter.brightness.target = 0.7;
		emitter.brightness.relativeVariance = 0.5;
		emitter.particlesPerSecond = 500;
		emitter.maxTotalParticles = 1000;
		emitter.particleRadius.target = 0.8;
		emitter.particleRadius.relativeVariance = 2;
		emitter.particleLifeSpanSeconds = 1;
		emitter.particleGravity = 0;
		emitter.particleInitialVelocity.target = 0.4;
		emitter.particleInitialVelocity.relativeVariance = 0.5;
		emitter.gravity = 0;
		emitter.airDensity = 0.2;
		emitter.particleAirDensity = 1;
		return emitter;
	}

	static readonly spinningConcentrationIconDegreesPerSecond: number = 40;

	loadResources(): void {
		this.spellBook.loadResources();
		const assetFolderName: string = Folders.assets;
		Folders.assets = 'GameDev/Assets/DragonH/';

		this.concentrationIcons = new Sprites("Scroll/Spells/Concentration/MiniConcentration", 1, 0, AnimationStyle.Static);
		this.morePowerIcons = new Sprites("Scroll/Spells/MorePower/MorePower", 9, 0, AnimationStyle.Static);
		this.morePowerIcon = this.morePowerIcons.add(0, 0, 8);
		this.morePowerIcon.scale = 0.45;
		this.morePowerIcon.opacity = 0.6;
		this.concentrationIcon = this.concentrationIcons.add(0, 0, 0);
		this.concentrationIcon.opacity = 0.6;

		this.spinningConcentrationIcons = new Sprites("Scroll/Spells/Concentration/MiniConcentration", 1, 0, AnimationStyle.Static);

		this.spinningConcentrationIcon = this.spinningConcentrationIcons.add(0, 0, 0);
		this.spinningConcentrationIcon.autoRotationDegeesPerSecond = CharacterStatsScroll.spinningConcentrationIconDegreesPerSecond;
		this.spinningConcentrationIcons.originX = 8;
		this.spinningConcentrationIcons.originY = 8;
		this.spinningConcentrationIcon.initialRotation = 0;

		this.spellItemIcons = new Sprites("Scroll/Spells/Concentration/MiniWand", 1, 0, AnimationStyle.Static);
		this.spellItemIcon = this.spellItemIcons.add(0, 0, 0);
		this.spellItemIcon.opacity = 0.6;

		this.scrollRolls = new Sprites("Scroll/Open/ScrollOpen", 23, this.framerateMs, AnimationStyle.SequentialStop, true);
		this.scrollPoofBack = new Sprites("Scroll/Close/ScrollPoof", 44, this.framerateMs, AnimationStyle.Sequential, true);
		this.scrollPoofFront = new Sprites("Scroll/Close/ScrollFire", 63, this.framerateMs, AnimationStyle.Sequential, true);
		this.scrollPoofFront.originX = 370;
		this.scrollPoofFront.originY = 325;
		this.scrollPoofBack.originX = 370;
		this.scrollPoofBack.originY = 325;

		this.scrollSlam = new Sprites("Scroll/Slam/Slam", 8, this.framerateMs, AnimationStyle.Sequential, true);
		this.scrollBacks = new Sprites("Scroll/Backs/Back", 5, this.framerateMs, AnimationStyle.Static);
		const totalKnownPlayers = 22;  //+++ Important - when adding new player headshots, increment totalKnownPlayers!
		this.playerHeadshots = new Sprites("Scroll/Players/Player", totalKnownPlayers, this.framerateMs, AnimationStyle.Static);
		this.scrollEmphasisMain = new Sprites("Scroll/Emphasis/Main/EmphasisMain", 27, this.framerateMs, AnimationStyle.Static);
		this.scrollEmphasisSkills = new Sprites("Scroll/Emphasis/Skills/EmphasisSkills", 27, this.framerateMs, AnimationStyle.Static);
		this.scrollEmphasisEquipment = new Sprites("Scroll/Emphasis/Equipment/EmphasisEquipment", 5, this.framerateMs, AnimationStyle.Static);
		this.scrollEmphasisSpells = new Sprites("Scroll/Emphasis/Spells/EmphasisSpells", 4, this.framerateMs, AnimationStyle.Static);

		this.scrollWooshSfx = new Audio(Folders.assets + 'SoundEffects/scrollWoosh.mp3');
		this.scrollOpenSfx = new Audio(Folders.assets + 'SoundEffects/scrollOpen.mp3');
		this.scrollCloseSfx = new Audio(Folders.assets + 'SoundEffects/scrollClose.mp3');
		this.scrollSlamSfx = new Audio(Folders.assets + 'SoundEffects/scrollSlam.mp3');
		this.scrollPoofSfx = new Audio(Folders.assets + 'SoundEffects/scrollPoof.mp3');

		Folders.assets = assetFolderName;
	}

	drawCharacterStats(now: number, context: CanvasRenderingContext2D, topData: number, bottomData: number): void {
		if (!this.activeCharacter)
			return;

		//console.log('this.selectedStatPageIndex: ' + this.selectedStatPageIndex);

		if (this.selectedStatPageIndex < 0) {
			return;
		}

		const activeCharacter: Character = this.activeCharacter;
		const activePage: StatPage = this.pages[this.selectedStatPageIndex];
		//console.log(activePage);
		activePage.render(context, activeCharacter, topData, bottomData);
	}

	isOnPage(page: ScrollPage): boolean {
		return this.selectedStatPageIndex + 1 === page;
	}

	drawAdditionalData(nowSec: number, context: CanvasRenderingContext2D, activeCharacter: Character, topData: number, bottomData: number): any {
		if (!activeCharacter.SpellData)
			return;
		if (this.isOnPage(ScrollPage.spells)) {
			this.drawSpellData(nowSec, activeCharacter, context, topData, bottomData);
		}
	}

	static readonly iconIndent: number = 4;

	drawSpellItem(now: number, context: CanvasRenderingContext2D, spellDataItem: SpellDataItem, x: number, y: number, topData: number, bottomData: number): number {
		const indent = 6;

		const spellName: string = spellDataItem.Name;
		const saveTop: number = y;

		const middleY: number = y + this.spellNameFontHeight / 2;
		const itemIsOnScreen = middleY > topData && middleY < bottomData;

		if (itemIsOnScreen)
			this.drawSpellGroupItem(context, x + indent, y, spellName);

		y += this.spellNameFontHeight

		const spellNameWidth: number = context.measureText(spellName).width;
		let spellNameRight: number = x + indent + spellNameWidth;
		if (spellNameRight > this.rightMostTextX) {
			this.rightMostTextX = spellNameRight;
		}


		if (spellDataItem.FromChargedItem) {
			spellNameRight += CharacterStatsScroll.iconIndent;
			this.spellItemIcon.x = spellNameRight;
			spellNameRight += this.spellItemIcons.spriteWidth * this.spinningConcentrationIcon.scale;
			this.spellItemIcon.y = saveTop + this.spellIconYOffset;
			if (itemIsOnScreen)
				this.spellItemIcons.draw(context, now * 1000);
		}
		else if (spellDataItem.IsConcentratingNow) {
			spellNameRight += CharacterStatsScroll.iconIndent;
			this.spinningConcentrationIcon.x = spellNameRight;
			spellNameRight += this.spinningConcentrationIcons.spriteWidth * this.spinningConcentrationIcon.scale;
			this.spinningConcentrationIcon.y = saveTop + this.spellIconYOffset;
			if (itemIsOnScreen)
				this.spinningConcentrationIcons.draw(context, now * 1000);
		}
		else if (spellDataItem.RequiresConcentration) {
			spellNameRight += CharacterStatsScroll.iconIndent;
			this.concentrationIcon.x = spellNameRight;
			spellNameRight += this.concentrationIcons.spriteWidth * this.concentrationIcon.scale;
			this.concentrationIcon.y = saveTop + this.spellIconYOffset;
			if (itemIsOnScreen)
				this.concentrationIcons.draw(context, now * 1000);
		}

		if (spellDataItem.MorePowerfulAtHigherLevels) {
			spellNameRight += CharacterStatsScroll.iconIndent;
			this.morePowerIcon.x = spellNameRight;
			spellNameRight += this.morePowerIcons.spriteWidth * this.morePowerIcon.scale;
			this.morePowerIcon.y = saveTop + this.spellIconYOffset;
			if (itemIsOnScreen)
				this.morePowerIcons.draw(context, now * 1000);
		}


		if (this.activeSpellData && this.activeSpellData.name == spellName) {
			const calloutMarginX = 8;
			this.calloutPointX = spellNameRight + calloutMarginX;
			if (this.calloutPointX > this.rightMostTextX) {
				this.rightMostTextX = this.calloutPointX;
			}
			this.calloutPointY = saveTop;
			if (itemIsOnScreen)
				this.drawActiveSpellData = true;
		}
		return y;
	}

	drawSpellGroup(now: number, context: CanvasRenderingContext2D, item: SpellGroup, x: number, y: number, topData: number, bottomData: number): number {
		y += this.drawSpellGroupTitle(context, x, y, item, topData, bottomData);

		this.drawSpellBackground(context, x, y, item.SpellDataItems.length, topData, bottomData);

		const belowTitleSpacing = 5;
		y += belowTitleSpacing;

		item.SpellDataItems.forEach(function (spellDataItem: SpellDataItem) {
			y = this.drawSpellItem(now, context, spellDataItem, x, y, topData, bottomData)
		}, this);

		const interGroupSpacing = 12;
		y += interGroupSpacing;
		return y;
	}

	static readonly spellDataLeft: number = 30;
	static readonly spellDataTop: number = 179;

	rightMostTextX: number;
	calloutPointX: number;
	calloutPointY: number;
	drawActiveSpellData = false;
	activeSpellData: ActiveSpellData;

	private drawSpellData(nowSec: number, activeCharacter: Character, context: CanvasRenderingContext2D, topData: number, bottomData: number) {
		const x: number = CharacterStatsScroll.spellDataLeft;
		let y: number = CharacterStatsScroll.spellDataTop;


		this.drawActiveSpellData = false;
		this.rightMostTextX = x;

		this.activeSpellData = activeCharacter.ActiveSpell;

		activeCharacter.SpellData.forEach(function (item: SpellGroup) {
			y = this.drawSpellGroup(nowSec, context, item, x, y, topData, bottomData);
		}, this);

		const minTop: number = Math.max(InGameCreatureManager.NpcScrollHeight, this.calloutPointY) + SpellBook.spellHeaderHeight;

		if (this.drawActiveSpellData) {
			if (this.calloutPointY >= topData && this.calloutPointY <= bottomData) {
				this.calloutPointX += this.drawActiveSpellIndicator(context, this.calloutPointX, this.calloutPointY, this.activeSpellData);
				this.spellBook.draw(nowSec, context, Math.max(this.rightMostTextX, this.calloutPointX), minTop, activeCharacter);
			}
		}
		else if (activeCharacter.forceShowSpell)
			this.spellBook.draw(nowSec, context, 600, minTop, activeCharacter);
	}

	readonly spellNameFontHeight: number = 18;
	readonly obsRectTextYOffset: number = 4;  // OBS rectangles seem to draw 4px closer to the top of the screen relative to text than in Chrome.

	drawActiveSpellIndicator(context: CanvasRenderingContext2D, x: number, y: number, spell: ActiveSpellData): number {
		let width = 0;
		context.fillStyle = '#aa0000';
		context.beginPath();
		const centerY: number = y + this.spellNameFontHeight / 2 + this.getBrowserGraphicsYOffset() / 2;
		const indicatorRadius = 4;
		context.arc(x, centerY, indicatorRadius, 0, 2 * Math.PI);
		width = 2 * indicatorRadius;
		context.fill();
		if (spell && spell.spellLevel < spell.spellSlotLevel) {
			const slotLevelStr = `[${spell.spellSlotLevel}]`;
			context.fillText(slotLevelStr, x + width, y);
			width += context.measureText(slotLevelStr).width;
		}
		return width;
	}

	getBrowserGraphicsYOffset(): number {
		if (this.browserIsOBS)
			return this.obsRectTextYOffset;
		return 0;
	}

	drawSpellBackground(context: CanvasRenderingContext2D, x: number, y: number, numSpells: number, topData: number, bottomData: number): void {
		const width: number = 305 - x;

		context.fillStyle = '#dfd1b7';

		let browserAdjustY = 0;

		if (this.browserIsOBS)
			browserAdjustY = this.obsRectTextYOffset;

		let top: number = y + browserAdjustY;
		let height: number = numSpells * this.spellNameFontHeight + this.spellNameFontHeight / 2;
		let bottom: number = top + height;

		if (top < topData)
			top = topData;

		if (bottom > bottomData)
			bottom = bottomData;

		height = bottom - top;

		if (height <= 0)
			return;

		context.fillRect(x, top, width, height);
	}

	drawSpellGroupItem(context: CanvasRenderingContext2D, x: number, y: number, spellName: string): void {
		context.font = this.spellNameFontHeight + 'px Calibri';
		context.fillStyle = '#3a1f0c';
		const maxWidth: number = 305 - x;
		context.fillText(spellName, x, y, maxWidth);
	}

	drawSpellGroupTitle(context: CanvasRenderingContext2D, x: number, y: number, spellGroup: SpellGroup, topData: number, bottomData: number): number {
		const groupTitleHeight = 22;

		const middleY: number = y + groupTitleHeight / 2;
		const textIsOnScreen = middleY > topData && middleY < bottomData;

		if (textIsOnScreen) {
			context.textAlign = 'left';
			context.textBaseline = 'top';
			context.font = groupTitleHeight + 'px Calibri';
			context.fillStyle = '#3a1f0c';
			const titleWidth: number = context.measureText(spellGroup.Name).width;
			const maxWidth: number = 305 - x;
			context.fillText(spellGroup.Name, x, y, maxWidth);
			const chargeBoxTopMargin = 1;
			this.drawChargeBoxes(context, x + titleWidth, y + chargeBoxTopMargin, spellGroup.TotalCharges, spellGroup.ChargesUsed);
		}

		return groupTitleHeight;
	}

	drawChargeBoxes(context: CanvasRenderingContext2D, x: number, y: number, totalCharges: number, chargesUsed: number): void {
		const side = 18;
		const minLeft = 72;
		const marginRight = 3;
		const indent = 4;
		const innerSide: number = side - indent * 2;
		let left: number = x;
		if (left < minLeft)
			left = minLeft;

		let browserAdjustY = 0;

		if (this.browserIsOBS)
			browserAdjustY = this.obsRectTextYOffset;

		const top: number = y + browserAdjustY;

		for (let i = 0; i < totalCharges; i++) {
			context.fillStyle = '#dfd1b7';
			context.fillRect(left, top, side, side);
			if (i < chargesUsed) {
				context.fillStyle = '#764d1e';
				context.fillRect(left + indent, top + indent, innerSide, innerSide);
			}
			left += side + marginRight;
		}
	}

	clear(): void {
		this.characters = [];
		this.pages = [];
		this.activeCharacter = null;
		this.selectedStatPageIndex = -1;
		this.state = ScrollState.none;
	}

	playerDataChanged(playerID: number, pageID: number, playerData: string): boolean {
		let newPageId: number = pageID;
		if (newPageId === -1)
			newPageId = this._page;

		const newPageIsValid: boolean = newPageId !== -1;

		//console.log(`playerDataChanged(${playerID}, ${pageID}, ${playerData})`);
		//console.log('newPageId: ' + newPageId);
		//console.log('this.page: ' + ScrollPage[this.page]);
		//console.log('this.state: ' + ScrollState[this.state]);

		let changedActiveCharacter = false;
		if (!this.activeCharacter || this.activeCharacter.playerID !== playerID) {
			//console.log('changing player!!!');
			this.clearEmphasis();
			if (newPageIsValid) {
				this.setActiveCharacter(playerID);
				changedActiveCharacter = true;
				this._page = newPageId;
				this.state = ScrollState.none;
				this.open(performance.now());
			}
		}
		else if (this.page !== newPageId || this.state === ScrollState.closed || this.state === ScrollState.closing || this.state === ScrollState.disappearing) {
			//console.log('changing page!!!');
			this.clearEmphasis();

			if (newPageIsValid) {
				this.page = newPageId;
				if (this.state === ScrollState.disappearing) {
					this.state = ScrollState.none;
					this.needImmediateReopen = false;
				}
				this.open(performance.now());
			}
			else {
				// console.log('new page is not valid!');
			}
		}
		else {
			// console.log('Data changed, but no update!');
		}

		if (playerData !== '') {
			this.updatePlayerData(playerData);
		}
		return changedActiveCharacter;
	}

	setActiveCharacter(playerID: number): void {
		for (let i = 0; i < this.characters.length; i++) {
			const thisCharacter: Character = this.characters[i];
			if (thisCharacter.playerID === playerID) {
				this.activeCharacter = thisCharacter;
				return;
			}
		}
	}

	getCharacter(playerID: number): Character {
		for (let i = 0; i < this.characters.length; i++) {
			const thisCharacter: Character = this.characters[i];
			if (thisCharacter.playerID === playerID) {
				return thisCharacter;
			}
		}
		return null;
	}


	updatePlayerData(playerData: string): Character {
		const sentChar = JSON.parse(playerData);
		const character: Character = this.getCharacter(sentChar.playerID);
		if (character !== null) {
			character.copyAttributesFrom(sentChar);
			if (!character.ActiveSpell)
				this.spellBook.lastSpellName = '';
		}

		return character;
	}

	readonly fadeTime: number = 300;

	private clearEmphasis() {
		this.scrollEmphasisMain.spriteProxies = [];
		this.scrollEmphasisSkills.spriteProxies = [];
		this.scrollEmphasisEquipment.spriteProxies = [];
		this.scrollEmphasisSpells.spriteProxies = [];
	}

	addEmphasis(emphasisIndex: number): void {
		if (this.page === ScrollPage.main)
			this.scrollEmphasisMain.add(0, 0, emphasisIndex).setFadeTimes(this.fadeTime, this.fadeTime);
		else if (this.page === ScrollPage.skills)
			this.scrollEmphasisSkills.add(0, 0, emphasisIndex).setFadeTimes(this.fadeTime, this.fadeTime);
		else if (this.page === ScrollPage.equipment)
			this.scrollEmphasisEquipment.add(0, 0, emphasisIndex).setFadeTimes(this.fadeTime, this.fadeTime);
		else if (this.page === ScrollPage.spells)
			this.scrollEmphasisSpells.add(0, 0, emphasisIndex).setFadeTimes(this.fadeTime, this.fadeTime);
	}


	addParticleEmphasis(itemID: string): void {
		if (this.showingGoldDustEmitters) {
			const activeHighlightPage: HighlightEmitterPages = this.highlightEmitterPages[this.page];
			const emitter: HighlightEmitter = activeHighlightPage.find(itemID);
			if (emitter) {
				emitter.start();
			}
			for (let i = 0; i < activeHighlightPage.emitters.length; i++) {
				const highlightEmitter: HighlightEmitter = activeHighlightPage.emitters[i];
				if (highlightEmitter !== emitter)
					highlightEmitter.stop();
			}
		}
	}

	queueParticleEmphasis(itemID: string): void {
		if (this.showingGoldDustEmitters) {
			this.emitterIndices.push(itemID);
		}
	}



	removeParticleEmphasis(itemID: string): void {
		const emitter: HighlightEmitter = this.highlightEmitterPages[this.page].find(itemID);
		if (emitter) {
			emitter.stop();
		}
	}



	currentlyEmphasizing(): boolean {
		const now: number = activeBackGame.nowMs;
		return this.scrollEmphasisMain.hasAnyAlive(now) ||
			this.scrollEmphasisSkills.hasAnyAlive(now) ||
			this.scrollEmphasisEquipment.hasAnyAlive(now) ||
			this.scrollEmphasisSpells.hasAnyAlive(now);
	}

	focusItem(playerID: number, pageID: number, itemID: string): void {
		const previouslyEmphasizing: boolean = this.currentlyEmphasizing();

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
		else if (pageID === ScrollPage.spells) {
			this.emphasisSprites = this.scrollEmphasisSpells;
			this.emphasisIndices.push(emphasisSpells[itemID]);
		}

		if (this.emphasisSprites)
			if (pageID === this.pageIndex)
				this.emphasisSprites.add(0, 0, this.emphasisIndices.pop()).setFadeTimes(this.fadeTime, this.fadeTime);

		const currentlyEmphasizing: boolean = this.currentlyEmphasizing();

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
		const previouslyEmphasizing: boolean = this.currentlyEmphasizing();

		this.removeParticleEmphasis(itemID);

		if (pageID === ScrollPage.main) {
			const emphasisIndex: number = emphasisMain[itemID];
			this.scrollEmphasisMain.killByFrameIndex(emphasisIndex, activeBackGame.nowMs);
			this.scrollEmphasisSkills.spriteProxies = [];
			this.scrollEmphasisEquipment.spriteProxies = [];
			this.scrollEmphasisSpells.spriteProxies = [];
		}
		else if (pageID === ScrollPage.skills) {
			const emphasisIndex: number = emphasisSkills[itemID];
			this.scrollEmphasisSkills.killByFrameIndex(emphasisIndex, activeBackGame.nowMs);
			this.scrollEmphasisMain.spriteProxies = [];
			this.scrollEmphasisEquipment.spriteProxies = [];
			this.scrollEmphasisSpells.spriteProxies = [];
		}
		else if (pageID === ScrollPage.equipment) {
			const emphasisIndex: number = emphasisEquipment[itemID];
			this.scrollEmphasisEquipment.killByFrameIndex(emphasisIndex, activeBackGame.nowMs);
			this.scrollEmphasisMain.spriteProxies = [];
			this.scrollEmphasisSkills.spriteProxies = [];
			this.scrollEmphasisSpells.spriteProxies = [];
		}
		else if (pageID === ScrollPage.spells) {
			const emphasisIndex: number = emphasisSpells[itemID];
			this.scrollEmphasisSpells.killByFrameIndex(emphasisIndex, activeBackGame.nowMs);
			this.scrollEmphasisMain.spriteProxies = [];
			this.scrollEmphasisSkills.spriteProxies = [];
			this.scrollEmphasisEquipment.spriteProxies = [];
		}

		const currentlyEmphasizing: boolean = this.currentlyEmphasizing();

		if (!currentlyEmphasizing && previouslyEmphasizing && activeBackGame) {
			this.deEmphasisSprite.expirationDate = activeBackGame.nowMs + this.deEmphasisSprite.fadeOutTime;
		}

	}

	drawStateDiagnostics(ctx: CanvasRenderingContext2D) {
		ctx.fillStyle = '#ff8080';
		ctx.font = '32px Arial';
		ctx.fillText('State: ' + ScrollState[this.state], 1200, 680);
		if (this.activeCharacter) {
			ctx.fillStyle = '#8080ff';
			ctx.font = '32px Arial';
			ctx.fillText('Active Player: ' + this.activeCharacter.firstName, 1200, 620);
		}
	}

	render(nowSec: number, timeScale: number, world: World): void {
		super.render(nowSec, timeScale, world);

		//this.drawStateDiagnostics(world.ctx);

		let justClosed = false;

		const picX = 17;
		const picWidth = 104;
		const picY = 24;
		const picHeight = 90;

		if (this.scrollIsVisible() && this.scrollRolls.spriteProxies.length !== 0) {
			justClosed = this.renderScroll(nowSec, justClosed, world, picY, picHeight, picX, picWidth, timeScale);
		}
		else if (this.state === ScrollState.disappearing) {
			this.renderDisappearAnimation(world, nowSec);
		}
		else {
			//console.log('Scroll is not visible!');
		}

		if (this.state === ScrollState.slamming) {
			this.renderSlam(world, nowSec);
		}

		if (justClosed) {
			this.closeScrollNow(nowSec);
		}
	}

	private renderScroll(nowSec: number, justClosed: boolean, world: World, picY: number, picHeight: number, picX: number, picWidth: number, timeScale: number): boolean {
		let elapsedTime: number = nowSec - this.scrollRolls.lastTimeWeAdvancedTheFrame / 1000;
		let scrollRollsFrameIndex: number = this.scrollRolls.spriteProxies[0].frameIndex;

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
		const maxFrameFraction = 0.99999999;
		if (frameFraction > maxFrameFraction) {
			frameFraction = maxFrameFraction;
		}

		if (this.state === ScrollState.closing)
			frameFraction = 1 - frameFraction;

		let topData = 0;
		let bottomData = Infinity;

		const frameIndexPrecise: number = scrollRollsFrameIndex + frameFraction;

		if (stillAnimating || this.state === ScrollState.closing) {
			({ topData, bottomData, justClosed } = this.drawCroppedScroll(scrollRollsFrameIndex, frameIndexPrecise, nextFrameIndex, topData, bottomData, world, picY, picHeight, picX, picWidth, nowSec, timeScale, justClosed));
		}
		else {
			this.drawFullScroll(world, nowSec, picX, picY, timeScale);
		}

		this.drawGoldDustEmitters(nowSec, timeScale, world);

		this.drawCharacterStats(nowSec, world.ctx, topData, bottomData);

		this.scrollRolls.draw(world.ctx, nowSec * 1000);
		this.drawCharacterData(nowSec, world, topData, bottomData);
		return justClosed;
	}

	private closeScrollNow(nowSec: number) {
		if (this.needImmediateReopen) {
			this.open(nowSec);
			this.needImmediateReopen = false;
		}
		else
			this.makeScrollDisappear(nowSec * 1000);
	}

	private renderSlam(world: World, nowSec: number) {
		this.scrollSlam.draw(world.ctx, nowSec * 1000);
		if (this.scrollSlam.spriteProxies.length === 0 || this.scrollSlam.spriteProxies[0].frameIndex === this.scrollSlam.baseAnimation.frameCount - 1) {
			this.state = ScrollState.slammed;
			this.play(this.scrollSlamSfx);
		}
	}

	private renderDisappearAnimation(world: World, nowSec: number) {
		this.scrollRolls.draw(world.ctx, nowSec * 1000);
		this.scrollPoofBack.draw(world.ctx, nowSec * 1000);
		this.scrollPoofFront.draw(world.ctx, nowSec * 1000);
		if (this.scrollPoofFront.spriteProxies.length > 0) {
			const poofFrameIndex: number = this.scrollPoofFront.spriteProxies[0].frameIndex;
			if (poofFrameIndex === 0) {
				this.state = ScrollState.none;
			}
		}
		else {
			this.state = ScrollState.none;
		}
	}

	private drawCharacterData(nowSec: number, world: World, topData: number, bottomData: number) {
		if (this.activeCharacter) {
			const activeCharacter: Character = this.activeCharacter;
			this.drawAdditionalData(nowSec, world.ctx, activeCharacter, topData, bottomData);
		}
	}

	private drawGoldDustEmitters(nowSec: number, timeScale: number, world: World) {
		if (this.showingGoldDustEmitters) {
			this.topEmitter.render(nowSec, timeScale, world);
			this.bottomEmitter.render(nowSec, timeScale, world);
		}
	}

	private drawFullScroll(world: World, nowSec: number, picX: number, picY: number, timeScale: number) {
		this.scrollBacks.draw(world.ctx, nowSec * 1000);
		this.playerHeadshots.baseAnimation.drawByIndex(world.ctx, picX, picY, this.headshotIndex);
		this.drawHighlighting(nowSec, timeScale, world);
		this.state = ScrollState.unrolled;

		if (this.showingGoldDustEmitters) {
			this.topEmitter.stop();
			this.bottomEmitter.stop();
			this.startQueuedEmitters();
		}
	}

	private drawCroppedScroll(scrollRollsFrameIndex: number, frameIndexPrecise: number, nextFrameIndex: number, topData: number, bottomData: number, world: World, picY: number, picHeight: number, picX: number, picWidth: number, nowSec: number, timeScale: number, justClosed: boolean) {
		const offset: number = CharacterStatsScroll.scrollOpenOffsets[scrollRollsFrameIndex + 1];

		const decimalOffset: number = frameIndexPrecise - scrollRollsFrameIndex;
		const distanceBetweenOffsets: number = CharacterStatsScroll.scrollOpenOffsets[nextFrameIndex + 1] - offset;

		const superPreciseOffset: number = offset + distanceBetweenOffsets * decimalOffset;

		const baseAnim: Part = this.scrollBacks.baseAnimation;
		const sw = this.scrollBacks.spriteWidth;
		const dx = 0;
		const dh: number = offset * 2;
		const dy = CharacterStatsScroll.centerY - offset;
		topData = dy;
		bottomData = dy + dh;
		const sh: number = dh;
		const dw: number = sw;
		const sx: number = dx;
		const sy: number = dy;

		baseAnim.drawCroppedByIndex(world.ctx, sx, sy, this.pageIndex, dx, dy, sw, sh, sw, dh);

		const picOffsetY: number = topData - picY;
		const picCroppedHeight: number = picY + picHeight - topData;

		if (picCroppedHeight > 0) {
			// ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)
			this.playerHeadshots.baseAnimation.drawCroppedByIndex(world.ctx, picX, picY + picOffsetY, this.headshotIndex, 0, picOffsetY, picWidth, picCroppedHeight, picWidth, picCroppedHeight);
		}
		this.drawHighlighting(nowSec, timeScale, world, sx, sy, dx, dy, sw, sh, dw, dh);

		//if (this.state === ScrollState.closing && frameIndex <= 7) {
		//  this.state = ScrollState.paused;
		//  this.scrollRolls.animationStyle = AnimationStyle.Static;
		//  this.lastFrameIndex = frameIndex;
		//  this.lastElapsedTime = elapsedTime;
		//}
		if (this.showingGoldDustEmitters && scrollRollsFrameIndex === 0 && this.state === ScrollState.unrolling) {
			this.topEmitter.start();
			this.bottomEmitter.start();
		}

		if (scrollRollsFrameIndex === 0 && this.state === ScrollState.closing) {
			justClosed = true;
			this.state = ScrollState.closed;
		}

		if (this.showingGoldDustEmitters) {
			this.topEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY - superPreciseOffset);
			this.bottomEmitter.position = new Vector(CharacterStatsScroll.centerX, CharacterStatsScroll.centerY + superPreciseOffset);
		}
		return { topData, bottomData, justClosed };
	}
} 