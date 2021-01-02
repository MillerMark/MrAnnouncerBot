enum ValidationAction {
	None,
	Warn,
	Stop
}

class ValidationIssueDto {
	constructor(public ValidationAction: ValidationAction, public FloatText: string, public PlayerId: number) {

	}
}

interface INameplateRenderer {
	getPlateWidth(context: CanvasRenderingContext2D, player: Character, playerIndex: number): number;
	inCombat: boolean;
}

interface ITextFloater {
	addFloatingText(xPos: number, text: string, fontColor: string, outlineColor: string): TextEffect;
}

interface ScaleFactor {
	scaleFactor: number;
}

class DragonFrontGame extends DragonGame implements INameplateRenderer, ITextFloater {
	coinManager: CoinManager = new CoinManager();
	inGameCreatureManager: InGameCreatureManager = new InGameCreatureManager();
	speechBubbleManager: SpeechBubbleManager = new SpeechBubbleManager(this, this.inGameCreatureManager);

	showFpsMessage(message: string): any {
		this.addUpperRightStatusMessage(message, '#000000', '#ffffff');
	}

	fpsWindow: FpsWindow;
	readonly fullScreenDamageThreshold: number = 15;
	readonly heavyDamageThreshold: number = 15;
	readonly mediumDamageThreshold: number = 6;
	readonly lightDamageThreshold: number = 4;

	readonly panelScale: number = 0.67;
	readonly panelShiftY: number = 16;
	readonly panelWidth: number = 391;
	readonly panelMargin: number = 34;
	readonly maxPanelWidth: number = (this.panelWidth - this.panelMargin * 2) * this.panelScale;

	layerSuffix = 'Front';
	emitter: Emitter;
	shouldDrawCenterCrossHairs = false;
	denseSmoke: Sprites;
	shield: Sprites;
	poof: Sprites;
	fred: Fred = new Fred();
	bloodGushA: BloodSprites;	// Totally contained - 903 high.
	bloodGushB: BloodSprites;  // Full screen, not contained at all - blood escapes top and right edges.
	bloodGushC: BloodSprites;	// Full screen, not contained at all - blood escapes top and right edges.
	bloodGushD: BloodSprites;  // Totally contained, 575 pixels high.
	bloodGushE: BloodSprites;  // Totally contained, 700 pixels high.

	sparkTrailBurst: Sprites;
	swirlSmokeA: Sprites;
	swirlSmokeB: Sprites;
	swirlSmokeC: Sprites;

	dndTimeDatePanel: SpriteProxy;
	clockPanel: Sprites;
	charmed: Sprites;
	bigX: Sprites;
	bigWarning: Sprites;
	restrained: Sprites;
	fireWorks: Sprites;
	sparkShower: Sprites;
	magicSparksA: Sprites;
	magicSparksB: Sprites;
	magicSparksC: Sprites;
	magicSparksD: Sprites;
	magicSparksE: Sprites;
	smokeWaveA: Sprites;
	smokeWaveB: Sprites;
	waterA: Sprites;
	waterB: Sprites;
	smokeBlastA: Sprites;
	smokeBlastB: Sprites;
	sparkBurstA: Sprites;
	sparkBurstB: Sprites;
	sparkBurstC: Sprites;
	sparkBurstD: Sprites;
	embersLarge: Sprites;
	embersMedium: Sprites;
	spellHits1: Sprites;
	spellHits2: Sprites;
	spellHits3: Sprites;
	spellHits4: Sprites;
	spellHits5: Sprites;
	spellMisses: Sprites;
	nameplateParts: Sprites;
	nameplateMain: Sprites;
	nameplateHighlight: Sprites;
	nameplateHighlightTopMedium: Sprites;
	nameplateHighlightTopShort: Sprites;
	nameplateHighlightLeft: Sprites;
	nameplateHighlightRight: Sprites;
	fireWall: Sprites;
	stars: Sprites;
	fumes: Sprites;
	allBackEffects: SpriteCollection;
	allFrontEffects: SpriteCollection;
	bloodEffects: SpriteCollection;
	dragonFrontSounds: DragonFrontSounds;
	activePlayerId: number;
	playerDataSet = false;

	//nameplatesCanvas: HTMLCanvasElement;
	//nameplatesContext: CanvasRenderingContext2D;
	clockCanvas: HTMLCanvasElement;
	clockContext: CanvasRenderingContext2D;

	initializeOffscreenCanvases() {
		//this.nameplatesCanvas = document.createElement('canvas');
		//this.nameplatesCanvas.width = myCanvas.width;
		//this.nameplatesCanvas.height = myCanvas.height;
		//this.nameplatesContext = this.nameplatesCanvas.getContext('2d');

		this.clockCanvas = document.createElement('canvas');
		this.clockCanvas.width = myCanvas.width;
		this.clockCanvas.height = myCanvas.height;
		this.clockContext = this.clockCanvas.getContext('2d');
	}

	constructor(context: CanvasRenderingContext2D) {
		super(context);
		this.fireBallSound = 'FireBallCloseDelayed';
		this.dragonFrontSounds = new DragonFrontSounds('GameDev/Assets/DragonH/SoundEffects');
		this.conditionManager.initialize(this, this, this.dragonFrontSounds);
		this.cardManager.initialize(this, this.inGameCreatureManager, this.dragonFrontSounds);
		this.initializeOffscreenCanvases();
	}

	update(timestamp: number) {
		this.updateGravity();
		this.playerStats.update(this, this.dragonFrontSounds, timestamp);
		this.conditionManager.update(timestamp);
		this.cardManager.update(timestamp);
		super.update(timestamp);
		this.inGameCreatureManager.update(timestamp);
	}

	updateScreen(context: CanvasRenderingContext2D, nowMs: number) {
		this.drawTimePlusEffects(context, nowMs);
		this.allBackEffects.draw(context, nowMs);

		if (!this.playerDataSet) {
			this.ShowWaitingForInitializationMessage(context, '#00ff00', 'Front Overlay is waiting for player data to be initialized.', 200);
		}

		super.updateScreen(context, nowMs);

		if (this.shouldDrawCenterCrossHairs)
			drawCrossHairs(myContext, screenCenterX, screenCenterY);

		this.fred.draw(context, nowMs);

		this.bloodEffects.draw(context, nowMs);

		this.playerStats.draw(context, nowMs);

		this.conditionManager.draw(context, nowMs);
		this.cardManager.draw(context, nowMs);
		//this.playerStats.drawDiagnostics(context, this, this, this.players);

		this.coinManager.draw(context, nowMs);
		this.allFrontEffects.updatePositions(nowMs);
		this.allFrontEffects.draw(context, nowMs);
		this.calibrationCursor.updatePositions(nowMs);
		this.calibrationCursor.draw(context, nowMs);

		this.updateSkeletalTrackingEffects(context, nowMs);

		if (this.showFpsWindow) {
			if (!this.fpsWindow) {
				this.fpsWindow = new FpsWindow('Front', 2);
			}
			this.fpsWindow.showAllFramerates(this.timeBetweenFramesQueue, this.drawTimeForEachFrameQueue, context, nowMs);
		}

		context.drawImage(this.clockCanvas, 0, 0);
		//context.drawImage(this.nameplatesCanvas, 0, 0);
		this.drawNameplates(context, nowMs);
		this.playerStats.drawTopLevelStatus(context, nowMs);

		this.bigX.draw(context, nowMs);
		this.bigWarning.draw(context, nowMs);

		this.renderTextAnimations(context, nowMs);
		this.speechBubbleManager.draw(context, nowMs);

		if (this.calibrationPosition) {
			context.fillStyle = '#ff0000';
			context.font = '32px Arial';
			context.fillText(this.calibrationPosition, 700, 760);
		}

		if (this.skeletalData2d) {
			if (this.skeletalData2d.ShowLiveHandPosition) {
				this.showDiagnosticsLiveHandPosition(context, nowMs);
			}
		}
	}

	protected drawTimePlusEffects(context: CanvasRenderingContext2D, now: number) {
		super.drawClockLayerEffects(context, now);
		//this.drawGameTime(context);
	}

	protected updateClockFromDto(dto: any) {
		super.updateClockFromDto(dto);
		if (dto.Message) {
			this.showClockMessage(dto.Message);
		}
		this.refreshClock();
	}

	showClockMessage(message: string): void {
		let fillColor: string;
		let outlineColor: string;
		if (this.inCombat) {
			fillColor = '#fedeec';
			outlineColor = '#5e2a2a';
		}
		else {
			fillColor = '#deeffe';
			outlineColor = '#2d4c75';
		}

		this.addFloatingText(1760, message, fillColor, outlineColor);
	}

	private drawGameTime(context: CanvasRenderingContext2D) {
		if (!this.dndTimeStr)
			return;
		const timeFont = 'px Baskerville Old Face';
		const verticalMargin = 10;
		const timeHeight = 32;
		const dateHeight = 24;
		context.font = timeHeight + timeFont;
		const timeWidth: number = context.measureText(this.dndTimeStr.trim()).width;
		const timeHalfWidth: number = timeWidth / 2;
		const centerX: number = this.getClockX();
		let centerY: number = this.clockBottomY - timeHeight / 2 - verticalMargin;
		if (this.inCombat)
			context.fillStyle = '#500506';
		else
			context.fillStyle = '#0b0650';
		const lastColonPos: number = this.dndTimeStr.lastIndexOf(':');
		const firstTimePart: string = this.dndTimeStr.substr(0, lastColonPos).trim();
		const lastTimePart: string = this.dndTimeStr.substr(lastColonPos).trim();
		const leftX: number = centerX - timeHalfWidth;
		context.textAlign = 'left';
		context.textBaseline = 'middle';
		context.fillText(firstTimePart, leftX, centerY);
		const firstTimePartWidth: number = context.measureText(firstTimePart).width;
		context.globalAlpha = 0.75;
		context.fillText(lastTimePart, leftX + firstTimePartWidth, centerY);
		context.globalAlpha = 1;
		context.textAlign = 'center';
		centerY += timeHeight;
		context.font = dateHeight + timeFont;
		let dateFontScale = 1;
		let tryFontSize: number = dateHeight * dateFontScale;
		while (context.measureText(this.dndDateStr).width > this.maxPanelWidth && tryFontSize > 6) {
			dateFontScale *= 0.95;
			tryFontSize = dateHeight * dateFontScale;
			context.font = tryFontSize + timeFont;
		}
		context.fillText(this.dndDateStr, centerX, centerY);
	}

	removeAllGameElements(now: number): void {
		super.removeAllGameElements(now);
	}

	initialize() {
		this.loadWeapons();
		super.initialize();
		// TODO: Optimize inGameCreatureManager so we are at least not actually spending any time drawing or pushing pixels on the top layer. Find out why drawing is needed to keep this in sync with the back layer.
		this.inGameCreatureManager.initialize(this, null, this.dragonFrontSounds, -InGameCreatureManager.creatureScrollHeight - 100);
		gravityGames = new GravityGames();
		Folders.assets = 'GameDev/Assets/DroneGame/';  // So GravityGames can load planet Earth?
	}

	private loadWeapons() {
		globalBypassFrameSkip = true;
		Folders.assets = 'GameDev/Assets/DragonH/';

		this.loadGreatSword();
		this.loadStaff();
		this.loadBattleAxe();
		this.loadClub();
		this.loadBow();
		this.loadRapier();
		this.loadWarHammer();
		this.loadLongSword();
		this.loadJavelin();
		this.loadBoomerang();
		this.loadMace();
	}

	private loadWarHammer() {
		const warHammerOriginX = 516;
		const warHammerOriginY = 685;
		this.loadWeapon('WarHammer', 'MagicBackHead', warHammerOriginX, warHammerOriginY);
		this.loadWeapon('WarHammer', 'Weapon', warHammerOriginX, warHammerOriginY);
		this.loadWeapon('WarHammer', 'MagicFrontHead', warHammerOriginX, warHammerOriginY);
		this.loadWeapon('WarHammer', 'MagicHandle', warHammerOriginX, warHammerOriginY);
	}

	private loadBoomerang() {
		const boomerangOriginX = 229;
		const boomerangOriginY = 304;
		this.loadWeapon('Boomerang', 'Boomerang', boomerangOriginX, boomerangOriginY, 101);
	}

	private loadMace() {
		const maceOriginX = 251;
		const maceOriginY = 447;
		this.loadWeapon('Mace', 'Mace', maceOriginX, maceOriginY);
	}

	private loadLongSword() {
		const longSwordOriginX = 410;
		const longSwordOriginY = 717;
		this.loadWeapon('LongSword', 'MagicB', longSwordOriginX, longSwordOriginY);  // Big fire effect in back.
		this.loadWeapon('LongSword', 'Weapon', longSwordOriginX, longSwordOriginY);
		this.loadWeapon('LongSword', 'MagicA', longSwordOriginX, longSwordOriginY);
	}

	private loadJavelin() {
		const javelinOriginX = 426;
		const javelinOriginY = 999;
		this.loadWeapon('Javelin', 'Weapon', javelinOriginX, javelinOriginY);
		this.loadWeapon('Javelin', 'Magic', javelinOriginX, javelinOriginY);
	}

	private loadStaff() {
		const staffOriginX = 352;
		const staffOriginY = 744;
		this.loadWeapon('Staff', 'Magic', staffOriginX, staffOriginY);
		this.loadWeapon('Staff', 'Weapon', staffOriginX, staffOriginY);
	}

	private loadBattleAxe() {
		const battleAxeOriginX = 341;
		const battleAxeOriginY = 542;
		this.loadWeapon('BattleAxe', 'MagicA', battleAxeOriginX, battleAxeOriginY);
		this.loadWeapon('BattleAxe', 'MagicB', battleAxeOriginX, battleAxeOriginY);
		this.loadWeapon('BattleAxe', 'Weapon', battleAxeOriginX, battleAxeOriginY);
	}

	private loadClub() {
		const clubOriginX = 440;
		const clubOriginY = 745;
		this.loadWeapon('Club', 'Weapon', clubOriginX, clubOriginY);
		this.loadWeapon('Club', 'Magic', clubOriginX, clubOriginY);
	}

	private loadBow() {
		const bowOriginX = 380;
		const bowOriginY = 570;
		this.loadWeapon('Bow', 'BowMagic', bowOriginX, bowOriginY);
		this.loadWeapon('Bow', 'Bow', bowOriginX, bowOriginY);
		this.loadWeapon('Bow', 'MagicArrow', bowOriginX, bowOriginY);
		this.loadWeapon('Bow', 'ArrowFlame', bowOriginX, bowOriginY);
	}

	private loadRapier() {
		const rapierOriginX = 423;
		const rapierOriginY = 600;
		this.loadWeapon('Rapier', 'MagicBack', rapierOriginX, rapierOriginY);
		this.loadWeapon('Rapier', 'Weapon', rapierOriginX, rapierOriginY);
		this.loadWeapon('Rapier', 'MagicFront', rapierOriginX, rapierOriginY);
	}

	private loadGreatSword() {
		const greatSwordOriginX = 306;
		const greatSwordOriginY = 837;
		this.loadWeapon('GreatSword', 'Magic', greatSwordOriginX, greatSwordOriginY);
		this.loadWeapon('GreatSword', 'Weapon', greatSwordOriginX, greatSwordOriginY);
	}

	start() {
		super.start();
		gravityGames.selectPlanet('Earth');
		gravityGames.newGame();

		this.updateGravity();
		if (this.emitter)
			this.world.addCharacter(this.emitter);
	}

	loadResources(): void {
		super.loadResources();
		Folders.assets = 'GameDev/Assets/DragonH/';

		this.inGameCreatureManager.loadResources();

		this.fred.loadResources();
		this.coinManager.loadResources();
		this.speechBubbleManager.loadResources();

		this.denseSmoke = new Sprites('Smoke/Dense/DenseSmoke', 116, fps30, AnimationStyle.Sequential, true);
		this.denseSmoke.name = 'DenseSmoke';
		this.denseSmoke.originX = 309;
		this.denseSmoke.originY = 723;

		this.smokeColumnBack = new Sprites('SmokeColumn/SmokeColumnBack', 77, fps30, AnimationStyle.Sequential, true);
		this.smokeColumnBack.name = 'SmokeColumnBack';
		this.smokeColumnBack.originX = 361;
		this.smokeColumnBack.originY = 1044;

		this.smokeColumnFront = new Sprites('SmokeColumn/SmokeColumnFront', 77, fps30, AnimationStyle.Sequential, true);
		this.smokeColumnFront.name = 'SmokeColumnFront';
		this.smokeColumnFront.originX = 361;
		this.smokeColumnFront.originY = 1044;

		// TODO: Consider adding fireballs to another SpriteCollection. Not really windups.
		this.backLayerEffects.add(this.smokeColumnBack);
		this.backLayerEffects.add(this.smokeColumnFront);

		this.shield = new Sprites('Weapons/Shield/Shield', 88, fps30, AnimationStyle.Sequential, true);
		this.shield.name = 'Shield';
		this.shield.originX = 125;
		this.shield.originY = 445;

		this.poof = new Sprites('Smoke/Poof/Poof', 67, fps30, AnimationStyle.Sequential, true);
		this.poof.name = 'Puff';
		this.poof.originX = 229;
		this.poof.originY = 698;

		this.fireWorks = new Sprites('Sparks/Big/BigSparks', 63, fps30, AnimationStyle.Sequential, true);
		this.fireWorks.name = 'Fireworks';
		this.fireWorks.originX = 443;
		this.fireWorks.originY = 595;

		this.calibrationCursor = new Sprites('LeapMotion/CalibrationUI/Cursor', 1, fps30, AnimationStyle.Static);
		this.calibrationCursor.originX = 21;
		this.calibrationCursor.originY = 22;

		this.calibrationDiscoverability = new Sprites('LeapMotion/CalibrationUI/CalibrationUI', 4, fps30, AnimationStyle.Static);

		this.sparkShower = new Sprites('Sparks/SparkShower/SparkShower', 75, fps30, AnimationStyle.Sequential, true);
		this.sparkShower.name = 'SparkShower';
		this.sparkShower.originX = 500;
		this.sparkShower.originY = 325;

		this.magicSparksA = this.loadMagicSparks('A');
		this.magicSparksB = this.loadMagicSparks('B');
		this.magicSparksC = this.loadMagicSparks('C');
		this.magicSparksD = this.loadMagicSparks('D');
		this.magicSparksE = this.loadMagicSparks('E');

		this.swirlSmokeA = this.loadSwirlSmoke('A', 136, 395, 397);
		this.swirlSmokeB = this.loadSwirlSmoke('B', 131, 415, 363);
		this.swirlSmokeC = this.loadSwirlSmoke('C', 129, 410, 476);

		this.sparkTrailBurst = new Sprites('SpellEffects/SparkTrailBurst/SparkTrailBurst', 54, fps30, AnimationStyle.Sequential, true);
		this.sparkTrailBurst.name = 'SparkTrailBurst';
		this.sparkTrailBurst.originX = 508;
		this.sparkTrailBurst.originY = 408;

		this.smokeWaveA = new Sprites('Smoke/Waves/SmokeWaveA/SmokeWaveA', 157, fps30, AnimationStyle.Sequential, true);
		this.smokeWaveA.name = 'SmokeWaveA';
		this.smokeWaveA.originX = 466;
		this.smokeWaveA.originY = 330;

		this.smokeWaveB = new Sprites('Smoke/Waves/SmokeWaveB/SmokeWaveB', 174, fps30, AnimationStyle.Sequential, true);
		this.smokeWaveB.name = 'SmokeWaveB';
		this.smokeWaveB.originX = 370;
		this.smokeWaveB.originY = 282;

		this.waterA = new Sprites('Water/WaterA/WaterA', 206, fps30, AnimationStyle.Sequential, true);
		this.waterA.name = 'WaterA';
		this.waterA.originX = 395;
		this.waterA.originY = 750;

		this.waterB = new Sprites('Water/WaterB/WaterB', 254, fps30, AnimationStyle.Sequential, true);
		this.waterB.name = 'WaterB';
		this.waterB.originX = 311;
		this.waterB.originY = 950;

		this.smokeBlastA = new Sprites('Smoke/Blast/SmokeBlastA/SmokeBlastA', 71, fps30, AnimationStyle.Sequential, true);
		this.smokeBlastA.name = 'SmokeBlastA';
		this.smokeBlastA.originX = 350;
		this.smokeBlastA.originY = 380;

		this.sparkBurstA = new Sprites('Sparks/SparkBurstA/SparkBurstA', 56, fps30, AnimationStyle.Sequential, true);
		this.sparkBurstA.name = 'SparkBurstA';
		this.sparkBurstA.originX = 416;
		this.sparkBurstA.originY = 525;

		this.sparkBurstB = new Sprites('Sparks/SparkBurstB/SparkBurstB', 37, fps30, AnimationStyle.Sequential, true);
		this.sparkBurstB.name = 'SparkBurstB';
		this.sparkBurstB.originX = 463;
		this.sparkBurstB.originY = 375;

		this.sparkBurstC = new Sprites('Sparks/SparkBurstC/SparkBurstC', 64, fps30, AnimationStyle.Sequential, true);
		this.sparkBurstC.name = 'SparkBurstC';
		this.sparkBurstC.originX = 497;
		this.sparkBurstC.originY = 470;


		this.sparkBurstD = new Sprites('Sparks/SparkBurstD/SparkBurstD', 53, fps30, AnimationStyle.Sequential, true);
		this.sparkBurstD.name = 'SparkBurstD';
		this.sparkBurstD.originX = 371;
		this.sparkBurstD.originY = 397;

		this.smokeBlastB = new Sprites('Smoke/Blast/SmokeBlastB/SmokeBlastB', 82, fps30, AnimationStyle.Sequential, true);
		this.smokeBlastB.name = 'SmokeBlastB';
		this.smokeBlastB.originX = 337;
		this.smokeBlastB.originY = 334;

		this.embersLarge = new Sprites('Embers/Large/EmberLarge', 93, fps30, AnimationStyle.Sequential, true);
		this.embersLarge.name = 'EmbersLarge';
		this.embersLarge.originX = 431;
		this.embersLarge.originY = 570;
		//this.embersLarge.originX = 504;
		//this.embersLarge.originY = 501;

		this.embersMedium = new Sprites('Embers/Medium/EmberMedium', 91, fps30, AnimationStyle.Sequential, true);
		this.embersMedium.name = 'EmbersMedium';
		//this.embersMedium.originX = 431;
		//this.embersMedium.originY = 570;
		this.embersMedium.originX = 504;
		this.embersMedium.originY = 501;

		/* Three/four kinds of hits:
		 
		 1a. Successful ranged spell attack (the d20 score is above or equal to the min threshold)
		 1b. Failed ranged spell attack (the d20 score is below the min threshold)
		 2. Any other spell that requires a die roll (commit to the cast)
		 3. Any other spell as soon as it is activated.

		 */

		this.spellHits1 = new Sprites('PlayerEffects/Spells/SpellHits/Hit1/Hit', 28, fps25, AnimationStyle.Sequential, true);
		this.spellHits1.name = "SpellHit1";
		this.spellHits1.originX = 176;
		this.spellHits1.originY = 238;


		this.spellHits2 = new Sprites('PlayerEffects/Spells/SpellHits/Hit2/Hit', 27, fps25, AnimationStyle.Sequential, true);
		this.spellHits2.name = "SpellHit2";
		this.spellHits2.originX = 283;
		this.spellHits2.originY = 248;


		this.spellHits3 = new Sprites('PlayerEffects/Spells/SpellHits/Hit3/Hit', 27, fps25, AnimationStyle.Sequential, true);
		this.spellHits3.name = "SpellHit3";
		this.spellHits3.originX = 290;
		this.spellHits3.originY = 390;


		this.spellHits4 = new Sprites('PlayerEffects/Spells/SpellHits/Hit4/Hit', 27, fps25, AnimationStyle.Sequential, true);
		this.spellHits4.name = "SpellHit4";
		this.spellHits4.originX = 320;
		this.spellHits4.originY = 300;


		this.spellHits5 = new Sprites('PlayerEffects/Spells/SpellHits/Hit5/Hit', 26, fps25, AnimationStyle.Sequential, true);
		this.spellHits5.name = "SpellHit5";
		this.spellHits5.originX = 190;
		this.spellHits5.originY = 420;

		this.spellMisses = new Sprites('PlayerEffects/Spells/SpellHits/Miss/Miss', 34, fps25, AnimationStyle.Sequential, true);
		this.spellMisses.name = "SpellMiss";
		this.spellMisses.originX = 186;
		this.spellMisses.originY = 237;

		this.nameplateParts = new Sprites('Nameplates/Piece', 3, fps30, AnimationStyle.Static, true);
		this.nameplateParts.originX = 19;
		this.nameplateParts.originY = 8;

		this.nameplateMain = new Sprites('Nameplates/Backplate', 1, fps30, AnimationStyle.Static, true);
		this.nameplateMain.originX = 179;
		this.nameplateMain.originY = 4;

		this.nameplateParts.add(0, DragonFrontGame.nameCenterY, 0);
		this.nameplateParts.add(0, DragonFrontGame.nameCenterY, 1);
		this.nameplateParts.add(0, DragonFrontGame.nameCenterY, 2);

		this.stars = new Sprites('SpinningStars/SpinningStars', 120, fps20, AnimationStyle.Loop, true);
		this.stars.name = 'Stars';
		this.stars.originX = 155;
		this.stars.originY = 495;

		this.fumes = new Sprites('Fumes/Fumes', 112, fps30, AnimationStyle.Loop, true);
		this.fumes.name = 'Fumes';
		this.fumes.originX = 281;
		this.fumes.originY = 137;

		this.bloodGushA = new BloodSprites('Blood/Gush/A/GushA', 69, fps30, AnimationStyle.Sequential, true);
		this.bloodGushA.name = 'BloodGush';
		this.bloodGushA.height = 903;
		this.bloodGushA.originX = 0;
		this.bloodGushA.originY = 903;

		this.bloodGushB = new BloodSprites('Blood/Gush/B/GushB', 78, fps30, AnimationStyle.Sequential, true);
		this.bloodGushB.name = 'BloodGush';
		this.bloodGushB.originX = 57;
		this.bloodGushB.originY = 1080;
		this.bloodGushB.height = 1080;

		this.bloodGushC = new BloodSprites('Blood/Gush/C/GushC', 89, fps30, AnimationStyle.Sequential, true);
		this.bloodGushC.name = 'BloodGush';
		this.bloodGushC.originX = 114;
		this.bloodGushC.originY = 1080;
		this.bloodGushC.height = 1080;

		this.bloodGushD = new BloodSprites('Blood/Gush/D/GushD', 48, fps30, AnimationStyle.Sequential, true);
		this.bloodGushD.name = 'BloodGush';
		this.bloodGushD.originX = 123;
		this.bloodGushD.originY = 571;
		this.bloodGushD.height = 575;

		this.bloodGushE = new BloodSprites('Blood/Gush/E/GushE', 30, fps30, AnimationStyle.Sequential, true);
		this.bloodGushE.name = 'BloodGush';
		this.bloodGushE.originX = 82;
		this.bloodGushE.originY = 698;
		this.bloodGushE.height = 700;

		this.charmed = new Sprites('Charmed/Charmed', 179, fps30, AnimationStyle.Loop, true);
		this.charmed.name = 'Heart';
		this.charmed.originX = 155;
		this.charmed.originY = 269;

		this.bigX = new Sprites('BigX/BigX', 125, fps30, AnimationStyle.Loop, true);
		this.bigX.originX = 452;
		this.bigX.originY = 499;
		this.bigX.returnFrameIndex = 16;

		this.bigWarning = new Sprites('BigX/Warning', 125, fps30, AnimationStyle.Loop, true);
		this.bigWarning.originX = 435;
		this.bigWarning.originY = 450;
		this.bigWarning.returnFrameIndex = 16;

		this.restrained = new Sprites('Restrained/Chains', 20, fps30, AnimationStyle.Loop, true);
		this.restrained.name = 'Restrained';
		this.restrained.originX = 213;
		this.restrained.originY = 299;

		Folders.assets = 'GameDev/Assets/DragonH/';
		this.fireWall = new Sprites('FireWall/FireWall', 121, fps20, AnimationStyle.Loop, true);
		this.fireWall.name = 'FireWall';
		this.fireWall.originX = 300;
		this.fireWall.originY = 300;

		this.allBackEffects = new SpriteCollection();
		this.allFrontEffects = new SpriteCollection();
		this.bloodEffects = new SpriteCollection();
		this.allBackEffects.add(this.denseSmoke);
		this.allBackEffects.add(this.poof);
		this.allBackEffects.add(this.stars);
		this.allBackEffects.add(this.fumes);
		this.allFrontEffects.add(this.fireWorks);
		//this.allFrontEffects.add(this.calibrationCursor);
		this.allFrontEffects.add(this.calibrationDiscoverability);
		this.allFrontEffects.add(this.sparkShower);
		this.allFrontEffects.add(this.magicSparksA);
		this.allFrontEffects.add(this.magicSparksB);
		this.allFrontEffects.add(this.magicSparksC);
		this.allFrontEffects.add(this.swirlSmokeA);
		this.allFrontEffects.add(this.swirlSmokeB);
		this.allFrontEffects.add(this.swirlSmokeC);
		this.allFrontEffects.add(this.sparkTrailBurst);
		this.allFrontEffects.add(this.magicSparksD);
		this.allFrontEffects.add(this.magicSparksE);
		this.allFrontEffects.add(this.smokeWaveA);
		this.allFrontEffects.add(this.smokeWaveB);
		this.allFrontEffects.add(this.smokeBlastA);
		this.allFrontEffects.add(this.smokeBlastB);
		this.allBackEffects.add(this.waterA);
		this.allBackEffects.add(this.waterB);
		this.allFrontEffects.add(this.sparkBurstA);
		this.allFrontEffects.add(this.sparkBurstB);
		this.allFrontEffects.add(this.sparkBurstC);
		this.allFrontEffects.add(this.sparkBurstD);
		this.allFrontEffects.add(this.embersLarge);
		this.allFrontEffects.add(this.embersMedium);
		this.allFrontEffects.add(this.spellHits1);
		this.allFrontEffects.add(this.spellHits2);
		this.allFrontEffects.add(this.spellHits3);
		this.allFrontEffects.add(this.spellHits4);
		this.allFrontEffects.add(this.spellHits5);
		this.allFrontEffects.add(this.spellMisses);

		for (let i = 0; i < this.allFrontEffects.allSprites.length; i++) {
			this.allFrontEffects.allSprites[i].moves = true;
			this.allFrontEffects.allSprites[i].disableGravity();
		}

		this.bloodEffects.add(this.bloodGushA);
		this.bloodEffects.add(this.bloodGushB);
		this.bloodEffects.add(this.bloodGushC);
		this.bloodEffects.add(this.bloodGushD);
		this.bloodEffects.add(this.bloodGushE);
		this.allBackEffects.add(this.charmed);
		this.allBackEffects.add(this.restrained);
		this.allWindupEffects.add(this.shield);

		this.clockPanel = new Sprites('Clock/TimeDisplayPanel', 3, fps30, AnimationStyle.Static);
		this.clockPanel.name = 'ClockPanel';
		this.clockPanel.originX = DragonGame.ClockOriginX;
		this.clockPanel.originY = 67;

		this.dndTimeDatePanel = this.clockPanel.add(this.getClockX(), this.panelShiftY + this.getClockY()).setScale(this.panelScale);

		this.clockLayerEffects.add(this.fireWall);

		this.playerStats.loadResources();
		this.conditionManager.loadResources();
		this.cardManager.loadResources();

	}

	loadMagicSparks(path: string): Sprites {
		const magicSparks: Sprites = new Sprites(`Sparks/Magic/${path}/SparkMagic${path}`, 35, fps30, AnimationStyle.Sequential, true);
		magicSparks.name = `SparkMagic${path}`;
		magicSparks.originX = 364;
		magicSparks.originY = 484;
		return magicSparks;
	}


	loadSwirlSmoke(path: string, frameCount: number, originX: number, originY: number): Sprites {
		const swirlSmoke: Sprites = new Sprites(`SpellEffects/SwirlSmoke/${path}/SwirlSmoke${path}`, frameCount, fps30, AnimationStyle.Sequential, true);
		swirlSmoke.name = `SwirlSmoke${path}`;
		swirlSmoke.originX = originX;
		swirlSmoke.originY = originY;
		return swirlSmoke;
	}

	private createFireBallBehindClock(hue: number): any {
		let x: number;
		let y: number;
		x = this.getClockX() - 90 * this.clockScale;
		const fireBallAdjust = 11;
		y = this.clockBottomY - this.clockPanel.originY + fireBallAdjust;
		const pos: Vector = new Vector(x - this.fireBallBack.originX, y - this.fireBallBack.originY);
		this.fireBallBack.spriteProxies.push(new ColorShiftingSpriteProxy(0, pos).setHueSatBrightness(hue).setScale(this.clockScale));
		this.fireBallFront.spriteProxies.push(new ColorShiftingSpriteProxy(0, pos).setHueSatBrightness(hue).setScale(this.clockScale));
		this.dragonFrontSounds.safePlayMp3('HeavyPoof');
	}

	private createFireWallBehindClock() {
		const displayMargin = -10;
		const fireWall: SpriteProxy = this.fireWall.add(this.getClockX(), this.clockBottomY - this.panelScale * this.clockPanel.originY + displayMargin);
		fireWall.scale = 0.6 * this.clockScale;
		fireWall.opacity = 0.8;
		fireWall.fadeOutTime = 400;
		this.dragonFrontSounds.safePlayMp3('FlameOn');
	}

	exitingCombat() {
		super.exitingCombat();
		this.dndTimeDatePanel.frameIndex = 0;
		this.fireWall.spriteProxies = [];
		this.createFireBallBehindClock(200);
		this.refreshNameplates();
	}

	refreshNameplates() {
		const activeCreatureId: number = this.playerStats.ActiveTurnCreatureID;
		this.playerStats.ActiveTurnCreatureID = -1;  // Forces a rebuild of highlighting.
		this.playerStats.setActiveTurnCreatureID(this, this, this.context, activeCreatureId, this.players);
		this.playerStats.moveAllTargets(this, this, this.context, this.players);
		this.conditionManager.movePlayerConditions(this, this, this.dragonFrontSounds, this.context, this.players);
	}

	refreshClock() {
		if (this.hideClock)
			return;
		this.clockContext.clearRect(0, 0, 1920, 1080);
		this.clockPanel.draw(this.clockContext, performance.now());
		this.drawGameTime(this.clockContext);
	}

	enteringCombat() {
		super.enteringCombat();
		this.dndTimeDatePanel.frameIndex = 1;
		this.createFireWallBehindClock();
		this.createFireBallBehindClock(330);
		this.refreshNameplates();
	}

	exitingTimeFreeze() {
		super.exitingTimeFreeze();
		this.dndTimeDatePanel.frameIndex = 0;
		this.fireWall.spriteProxies = [];
		this.createFireBallBehindClock(200);
	}

	enteringTimeFreeze() {
		super.enteringTimeFreeze();
		this.dndTimeDatePanel.frameIndex = 2;
		this.createFireBallBehindClock(270);
	}

	executeCommand(command: string, params: string, userInfo: UserInfo, now: number): boolean {
		if (super.executeCommand(command, params, userInfo, now))
			return true;

		if (command === "Cross2") {
			this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
		}
	}

	testBloodEmitter(): void {
		this.emitter = new Emitter(new Vector(screenCenterX + 280, 1080));
		this.emitter.radius = 1;
		this.emitter.saturation.target = 0.9;
		this.emitter.saturation.relativeVariance = 0.2;
		this.emitter.hue.absoluteVariance = 10;
		this.emitter.hue.target = 0;
		this.emitter.brightness.target = 0.4;
		this.emitter.brightness.relativeVariance = 0.3;
		this.emitter.particlesPerSecond = 1400;

		this.emitter.particleRadius.target = 2.5;
		this.emitter.particleRadius.relativeVariance = 0.8;

		this.emitter.particleLifeSpanSeconds = 2;
		this.emitter.maxTotalParticles = 2000;
		this.emitter.particleGravity = 9.8;
		this.emitter.particleInitialVelocity.target = 0.8;
		this.emitter.particleInitialVelocity.relativeVariance = 0.25;
		this.emitter.gravity = 0;
		this.emitter.airDensity = 0; // 0 == vaccuum.
		this.emitter.particleAirDensity = 0.1;  // 0 == vaccuum.

		const sprayAngle: number = Random.intBetween(270 - 45, 270 + 45);
		let minVelocity = 9;
		let maxVelocity = 16;
		const angleAwayFromUp: number = Math.abs(sprayAngle - 270);
		if (angleAwayFromUp < 15)
			maxVelocity = 10;
		else if (angleAwayFromUp > 35)
			minVelocity = 13;
		else
			maxVelocity = 18;
		this.emitter.bonusParticleVelocityVector = Vector.fromPolar(sprayAngle, Random.intBetween(minVelocity, maxVelocity));

		this.emitter.renderOldestParticlesLast = true;
	}

	test(testCommand: string, userInfo: UserInfo, now: number): boolean {
		if (super.test(testCommand, userInfo, now))
			return true;

		if (testCommand === "Cross2") {
			this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
		}

		if (testCommand === "SayIt1") {
			this.speechBubbleManager.sayOrThinkSomething(this.world.ctx, '2 says: It worked!');
		}

		if (testCommand === "SayIt2") {
			this.speechBubbleManager.sayOrThinkSomething(this.world.ctx, '2 says: It seems to be really working just a bit better!');
		}

		if (testCommand === "SayIt3") {
			this.speechBubbleManager.sayOrThinkSomething(this.world.ctx, '2 thinks: Now I\'m thinking something really smarty!');
		}


		if (testCommand.toLowerCase() === 'wave') {
			this.moveFred('Wave');
			return true;
		}

		if (testCommand.toLowerCase() === 'thumbsup') {
			this.moveFred('ThumbsUp');
			return true;
		}

		if (testCommand.toLowerCase() === 'thumbsdown') {
			this.moveFred('ThumbsDown');
			return true;
		}

		if (testCommand.toLowerCase() === 'noidea') {
			this.moveFred('NoIdea');
			return true;
		}

		if (testCommand.toLowerCase() === 'fistup') {
			this.moveFred('FistUp');
			return true;
		}

		if (testCommand.toLowerCase() === 'clasped') {
			this.moveFred('HandsClasped');
			return true;
		}

		if (testCommand.toLowerCase() === 'killem') {
			this.moveFred('KillEm');
			return true;
		}

		if (testCommand === 'clear') {
			this.world.removeCharacter(this.emitter);
			this.testBloodEmitter();
			this.world.addCharacter(this.emitter);
			return true;
		}

		if (testCommand === 'blood') {
			//console.log('Blood');
			this.world.removeCharacter(this.emitter);
			this.testBloodEmitter();
			this.world.addCharacter(this.emitter);
			return true;
		}

		// poof 40 50
		if (testCommand.startsWith("poof")) {
			const split: string[] = testCommand.split(' ');

			let hue = 0;
			let saturation = 100;
			let brightness = 100;

			if (split.length === 2) {
				hue = +split[1];
			}
			else if (split.length === 3) {
				hue = +split[1];
				saturation = +split[2];
			}
			else if (split.length > 3) {
				hue = +split[1];
				saturation = +split[2];
				brightness = +split[3];
			}

			this.denseSmoke.spriteProxies.push(new ColorShiftingSpriteProxy(0, new Vector(450 - this.denseSmoke.originX, 1080 - this.denseSmoke.originY)).setHueSatBrightness(hue, saturation, brightness));
		}


		if (testCommand.startsWith("fb")) {
			const split: string[] = testCommand.split(' ');

			let hue = 0;
			let saturation = 100;
			let brightness = 100;

			if (split.length === 2) {
				hue = +split[1];
			}
			else if (split.length === 3) {
				hue = +split[1];
				saturation = +split[2];
			}
			else if (split.length > 3) {
				hue = +split[1];
				saturation = +split[2];
				brightness = +split[3];
			}

			this.fireBallBack.spriteProxies.push(new SpriteProxy(0, 450 - this.fireBallBack.originX, 1080 - this.fireBallBack.originY));
			this.fireBallFront.spriteProxies.push(new ColorShiftingSpriteProxy(0, new Vector(450 - this.fireBallFront.originX, 1080 - this.fireBallFront.originY)).setHueSatBrightness(hue, saturation, brightness));
		}

		return false;
	}

	protected triggerSoundEffect(dto): void {
		let soundFileName: string = dto.soundFileName;
		if (!soundFileName)
			return;
		if (soundFileName.indexOf('.') < 0)
			soundFileName += '.mp3';
		//console.log("Playing " + Folders.assets + 'SoundEffects/' + soundFileName);
		new Audio(Folders.assets + 'SoundEffects/' + soundFileName).play();
	}

	protected triggerEmitter(dto: any, center: Vector): void {
		this.world.removeCharacter(this.emitter);

		//console.log('emitter: ' + dto);

		this.emitter = new Emitter(new Vector(center.x, center.y), new Vector(dto.emitterInitialVelocity.x, dto.emitterInitialVelocity.y));
		if (dto.emitterShape === EmitterShape.Circular) {
			this.emitter.radius = dto.radius;
		}
		else {
			this.emitter.setRectShape(dto.width, dto.height);
		}

		this.emitter.particleMass = dto.particleMass;
		this.emitter.initialParticleDirection = new Vector(dto.particleInitialDirection.x, dto.particleInitialDirection.y);
		this.emitter.particleWind = new Vector(dto.particleWindDirection.x, dto.particleWindDirection.y);
		this.emitter.minParticleSize = dto.minParticleSize;
		this.emitter.gravityCenter = new Vector(dto.gravityCenter.x, dto.gravityCenter.y);
		this.emitter.particleFadeInTime = dto.fadeInTime;
		this.emitter.wind = new Vector(dto.emitterWindDirection.x, dto.emitterWindDirection.y);

		this.transferTargetValue(this.emitter.brightness, dto.brightness);
		this.transferTargetValue(this.emitter.hue, dto.hue);
		this.transferTargetValue(this.emitter.saturation, dto.saturation);

		this.emitter.maxConcurrentParticles = dto.maxConcurrentParticles;
		this.emitter.particleMaxOpacity = dto.maxOpacity;

		this.emitter.particlesPerSecond = dto.particlesPerSecond;

		this.transferTargetValue(this.emitter.particleRadius, dto.particleRadius);

		this.emitter.emitterEdgeSpread = dto.edgeSpread;

		this.emitter.particleLifeSpanSeconds = dto.lifeSpan;
		if (dto.maxTotalParticles === 'Infinity') {
			this.emitter.maxTotalParticles = Infinity;
		}
		else {
			this.emitter.maxTotalParticles = dto.maxTotalParticles;
		}

		this.emitter.particleGravity = dto.particleGravity;
		this.emitter.particleGravityCenter = new Vector(dto.particleGravityCenter.x, dto.particleGravityCenter.y);
		this.transferTargetValue(this.emitter.particleInitialVelocity, dto.particleInitialVelocity);

		this.emitter.gravity = dto.emitterGravity;
		this.emitter.airDensity = dto.emitterAirDensity;
		this.emitter.particleAirDensity = dto.particleAirDensity;

		this.emitter.bonusParticleVelocityVector = new Vector(dto.bonusVelocityVector.x, dto.bonusVelocityVector.y);

		this.emitter.renderOldestParticlesLast = dto.renderOldestParticlesLast;

		this.world.addCharacter(this.emitter);
		//console.log(this.emitter);
	}


	private transferTargetValue(brightness: TargetValue, anyTargetValue: any): void {
		brightness.target = anyTargetValue.value;
		brightness.relativeVariance = anyTargetValue.relativeVariance;
		brightness.absoluteVariance = anyTargetValue.absoluteVariance;
		brightness.drift = anyTargetValue.drift;
		brightness.binding = anyTargetValue.targetBinding;
	}

	protected triggerAnimationDto(dto: any, center: Vector) {
		let sprites: Sprites;
		let horizontallyFlippable: boolean = false;
		//console.log('dto.spriteName: ' + dto.spriteName);
		if (dto.spriteName === 'DenseSmoke')
			sprites = this.denseSmoke;
		else if (dto.spriteName === 'Poof')
			sprites = this.poof;

		// TODO: Update blood animation to pass in a severity number instead of a string in WPF app.
		else if (dto.spriteName === 'BloodGush') {
			sprites = this.bloodGushA;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodLarger') {
			sprites = this.bloodGushB;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodLarge') {
			sprites = this.bloodGushC;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodMedium') {
			sprites = this.bloodGushD;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodSmall') {
			sprites = this.bloodGushE;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodSmaller') {
			sprites = this.bloodGushA;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodSmallest') {
			sprites = this.bloodGushB;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'SmokeBlast') {
			if (Random.chancePercent(50))
				sprites = this.smokeBlastA;
			else
				sprites = this.smokeBlastB;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'SmokeWave') {
			if (Random.chancePercent(50))
				sprites = this.smokeWaveA;
			else
				sprites = this.smokeWaveB;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'SparkBurst') {
			if (Random.chancePercent(25))
				sprites = this.sparkBurstA;
			else if (Random.chancePercent(33))
				sprites = this.sparkBurstB;
			else if (Random.chancePercent(50))
				sprites = this.sparkBurstC;
			else
				sprites = this.sparkBurstD;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'Water') {
			if (Random.chancePercent(50))
				sprites = this.waterA;
			else
				sprites = this.waterB;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'Heart')
			sprites = this.charmed;
		else if (dto.spriteName === 'Restrained')
			sprites = this.restrained;
		else if (dto.spriteName === 'Fireworks')
			sprites = this.fireWorks;
		else if (dto.spriteName === 'SparkShower')
			sprites = this.sparkShower;
		else if (dto.spriteName === 'SparkMagicA')
			sprites = this.magicSparksA;
		else if (dto.spriteName === 'SparkMagicB')
			sprites = this.magicSparksB;
		else if (dto.spriteName === 'SparkMagicC')
			sprites = this.magicSparksC;
		else if (dto.spriteName === 'SparkMagicD')
			sprites = this.magicSparksD;
		else if (dto.spriteName === 'SparkMagicE')
			sprites = this.magicSparksE;
		else if (dto.spriteName === 'EmbersLarge')
			sprites = this.embersLarge;
		else if (dto.spriteName === 'EmbersMedium')
			sprites = this.embersMedium;
		else if (dto.spriteName === 'Stars')
			sprites = this.stars;
		else if (dto.spriteName === 'Fumes')
			sprites = this.fumes;
		else if (dto.spriteName === 'FireBall')
			sprites = this.fireBallBack;
		else if (dto.spriteName === 'SmokeColumn')
			sprites = this.smokeColumnBack;
		else {
			for (let i = 0; i < this.allBackEffects.allSprites.length; i++) {
				if (dto.spriteName === this.allBackEffects.allSprites[i].name) {
					sprites = this.allBackEffects.allSprites[i];
					break;
				}
			}
			for (let i = 0; i < this.allFrontEffects.allSprites.length; i++) {
				if (dto.spriteName === this.allFrontEffects.allSprites[i].name) {
					sprites = this.allFrontEffects.allSprites[i];
					break;
				}
			}
		}

		let flipHorizontally = false;

		if (dto.horizontalFlip || horizontallyFlippable && Random.chancePercent(50))
			flipHorizontally = true;

		if (!sprites) {
			console.error(`"${dto.spriteName}" sprite not found.`);
			return;
		}

		let adjustCenter: Vector;
		if (dto.xOffset !== 0 || dto.yOffset !== 0) {
			adjustCenter = new Vector(center.x + dto.xOffset, center.y + dto.yOffset);
		}
		else
			adjustCenter = center;

		const spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(adjustCenter), dto.startFrameIndex, dto.hueShift, dto.saturation, dto.brightness,
			flipHorizontally, dto.verticalFlip, dto.scale, dto.rotation, dto.autoRotation, dto.velocityX, dto.velocityY);

		spritesEffect.start();

		sprites = null;

		if (dto.spriteName === 'FireBall') {
			sprites = this.fireBallFront;
		}
		else if (dto.spriteName === 'SmokeColumn') {
			sprites = this.smokeColumnFront;
		}

		if (sprites) {
			const spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(adjustCenter), dto.startFrameIndex, dto.secondaryHueShift, dto.secondarySaturation, dto.secondaryBrightness,
				flipHorizontally, dto.verticalFlip, dto.scale, dto.rotation, dto.autoRotation, dto.velocityX, dto.velocityY);
			spritesEffect.start();
		}
	}


	static readonly nameCenterY: number = 1052;
	static readonly nameplateHalfHeight: number = 24;

	initializePlayerData(playerData: string) {
		super.initializePlayerData(playerData);

		this.playerDataSet = true;

		for (let i = 0; i < this.players.length; i++) {
			const centerX: number = this.getPlayerX(i);
			this.nameplateMain.addShifted(centerX, DragonFrontGame.nameCenterY - DragonFrontGame.nameplateHalfHeight, 0, 0);
		}

		this.refreshNameplates();
	}

	playerChanged(playerID: number, pageID: number, playerData: string): void {
		super.playerChanged(playerID, pageID, playerData);
		this.activePlayerId = playerID;
		this.refreshNameplates();
	}

	// TODO: Keep these in sync with those MainWindow.xaml.cs
	readonly Player_Lady: number = 0;
	readonly Player_Shemo: number = 1;
	readonly Player_Merkin: number = 2;
	readonly Player_Ava: number = 3;
	readonly Player_Fred: number = 4;
	readonly Player_Willy: number = 5;

	showValidationIssue(validationIssueDtoStr: string) {
		const validationIssue: ValidationIssueDto = JSON.parse(validationIssueDtoStr);
		console.log(validationIssue);
		let x = 960;
		let y = 540;
		let hueShift = 261;
		if (validationIssue.PlayerId >= 0) {
			const player: Character = this.getPlayer(validationIssue.PlayerId);
			hueShift = player.hueShift;
			x = this.getPlayerX(this.getPlayerIndex(validationIssue.PlayerId));
			y = 1080 - 300;
		}

		let sprite: SpriteProxy;
		if (validationIssue.ValidationAction === ValidationAction.Stop) {
			sprite = this.bigX.addShifted(x, y, 0, hueShift);
			sprite.expirationDate = performance.now() + 6000;
		}
		else {
			sprite = this.bigWarning.add(x, y);
			sprite.expirationDate = performance.now() + 3000;
			sprite.data = validationIssue.FloatText;
		}

		this.dragonFrontSounds.safePlayMp3('WrongAnswer/WrongAnswer[6]');
		this.dragonFrontSounds.safePlayMp3('WrongAnswer/BigXFire');

		sprite.fadeOutTime = 500;
		if (validationIssue.FloatText)
			if (validationIssue.PlayerId >= 0) {
				const player: Character = this.getPlayer(validationIssue.PlayerId);
				this.floatPlayerText(validationIssue.PlayerId, validationIssue.FloatText, player.dieBackColor, player.dieFontColor);
			}
			else {
				this.addFloatingText(960, validationIssue.FloatText, '#ffffff', '#000000');
			}
	}

	changePlayerWealth(playerWealthDto: string): void {
		const wealthChange: WealthChange = JSON.parse(playerWealthDto);

		let timeoutMs = 2000;
		let airTimeSec: number = CoinManager.getAirTimeFullDropSec();
		if (wealthChange.Coins.TotalGold < 0) {
			timeoutMs = 500;
			airTimeSec = CoinManager.getAirTimeToDmBoxSec();
		}
		for (let i = 0; i < wealthChange.PlayerIds.length; i++) {
			const playerId = wealthChange.PlayerIds[i];
			const playerX: number = this.getPlayerX(this.getPlayerIndex(playerId));
			this.coinManager.addCoins(wealthChange.Coins, playerX);
			let prefix = '';
			this.dragonFrontSounds.safePlayMp3('Coins/Coins[3]');
			if (wealthChange.Coins.TotalGold > 0)
				prefix = '+';
			setTimeout(() => {
				this.addFloatingText(playerX, `${prefix}${wealthChange.Coins.TotalGold} gp`, DragonFrontGame.FontColorGold, DragonFrontGame.FontOutlineGold);
			}, timeoutMs);

			setTimeout(() => {
				this.dragonFrontSounds.safePlayMp3('Coins/Coins[3]');
			}, airTimeSec * 1000);
		}
	}

	changePlayerHealth(playerHealthDto: string): void {
		const playerHealth: PlayerHealth = JSON.parse(playerHealthDto);

		for (let i = 0; i < playerHealth.PlayerIds.length; i++) {
			if (playerHealth.DamageHealth < 0) {
				this.showDamage(playerHealth, i);
			}
			else {
				this.showHealth(playerHealth, i);
			}
		}
	}

	playerStats: PlayerStatManager = new PlayerStatManager();
	conditionManager: ConditionManager = new ConditionManager();
	cardManager: CardManager = new CardManager();

	changePlayerStats(playerStatsDtoStr: string): void {
		//console.log(playerStatsDtoStr);
		const newPlayerStats: PlayerStatManager = new PlayerStatManager().deserialize(JSON.parse(playerStatsDtoStr));
		this.playerStats.handleCommand(this, this, this.context, this.dragonFrontSounds, newPlayerStats, this.players, this.conditionManager);
		// TODO: transfer stats across.
	}

	private showHealth(playerHealth: PlayerHealth, i: number) {
		setTimeout(() => {
			const playerX: number = this.getPlayerX(this.getPlayerIndex(playerHealth.PlayerIds[i]));
			let fontColor: string = DragonFrontGame.FontColorHealth;
			let outlineColor: string = DragonFrontGame.FontOutlineHealth;
			let suffix = 'hp';
			if (playerHealth.IsTempHitPoints) {
				fontColor = DragonFrontGame.FontColorTempHp;
				outlineColor = DragonFrontGame.FontOutlineTempHp;
				suffix = 'temp hp';
			}

			this.addFloatingText(playerX, `+${playerHealth.DamageHealth} ${suffix}`, fontColor, outlineColor);
		}, 2000);
		this.showHealthGain(playerHealth.PlayerIds[i], playerHealth.DamageHealth, playerHealth.IsTempHitPoints);
		this.dragonFrontSounds.playRandom('Healing/Healing', 5);
	}

	addUpperRightStatusMessage(text: string, outlineColor: string, fontColor: string): any {
		const textEffect: TextEffect = this.textAnimations.addText(new Vector(1920, 0), text, 3500);
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.scale = 1;
		textEffect.fadeOutTime = 2500;
		textEffect.fadeInTime = 600;
		textEffect.textAlign = 'right';
		textEffect.textBaseline = 'top';
	}

	static readonly FontColorDamage: string = '#ca0000';
	static readonly FontOutlineDamage: string = '#000000';
	static readonly FontColorHealth: string = '#5681d4';
	static readonly FontOutlineHealth: string = '#ffffff';
	static readonly FontColorTempHp: string = '#d4569d';
	static readonly FontColorTempHpOnNameplates: string = '#e6a0c8';
	static readonly FontOutlineTempHp: string = '#ffffff';
	static readonly FontColorGold: string = '#fedf80';
	static readonly FontOutlineGold: string = '#5f4527';


	private showDamage(playerHealth: PlayerHealth, i: number) {
		const playerX: number = this.getPlayerX(this.getPlayerIndex(playerHealth.PlayerIds[i]));
		this.addFloatingText(playerX, `${playerHealth.DamageHealth} hp`, DragonFrontGame.FontColorDamage, DragonFrontGame.FontOutlineDamage);
		let fredIsTakingDamage = false;
		let fredIsGettingHitByBlood = false;
		const splatterDirection: SplatterDirection = this.showDamageForPlayer(playerHealth.DamageHealth, playerHealth.PlayerIds[i]);
		if (playerHealth.PlayerIds[i] === this.Player_Fred) {
			fredIsTakingDamage = true;
		}
		else {
			fredIsGettingHitByBlood = this.fredIsGettingHitByBlood(playerHealth, splatterDirection, fredIsGettingHitByBlood, playerX);
		}
		if (!fredIsTakingDamage && fredIsGettingHitByBlood)
			setTimeout(this.raiseShield.bind(this), 700);
	}

	private fredIsGettingHitByBlood(playerHealth: PlayerHealth, splatterDirection: SplatterDirection, fredIsGettingHitByBlood: boolean, playerX: number) {
		if (playerHealth.DamageHealth < 0 && splatterDirection === SplatterDirection.Left) {
			const absDamage: number = -playerHealth.DamageHealth;
			if (absDamage >= this.heavyDamageThreshold)
				fredIsGettingHitByBlood = true;
			else if (absDamage >= this.mediumDamageThreshold) {
				if (playerX < 900)
					fredIsGettingHitByBlood = true;
			}
			else if (absDamage >= this.lightDamageThreshold) {
				if (playerX < 600)
					fredIsGettingHitByBlood = true;
			}
		}
		return fredIsGettingHitByBlood;
	}

	raiseShield() {
		if (this.shield.spriteProxies.length === 0) {
			this.shield.add(this.getPlayerX(0), 1080);
			this.dragonFrontSounds.playMp3In(100, 'Windups/ShieldUp');
		}
	}


	private showDamageForPlayer(damageHealth: number, playerId: number): SplatterDirection {
		let flipHorizontally = false;
		if (Random.chancePercent(50))
			flipHorizontally = true;
		let damageHealthSprites: BloodSprites;
		let scale = 1;
		if (damageHealth > 0)
			return SplatterDirection.None;

		const absDamage: number = -damageHealth;

		if (absDamage >= this.heavyDamageThreshold)
			this.dragonFrontSounds.playRandom('Damage/Heavy/GushHeavy', 13);
		else if (absDamage >= this.mediumDamageThreshold)
			this.dragonFrontSounds.playRandom('Damage/Medium/GushMedium', 29);
		else
			this.dragonFrontSounds.playRandom('Damage/Light/GushLight', 15);


		if (absDamage < this.fullScreenDamageThreshold) {
			damageHealthSprites = this.getScalableBlood();

			const desiredBloodHeight: number = 1080 * absDamage / this.fullScreenDamageThreshold;
			scale = desiredBloodHeight / damageHealthSprites.height;
		}
		else {
			if (Random.chancePercent(25)) {
				damageHealthSprites = this.bloodGushB;
			}
			else if (Random.chancePercent(33)) {
				damageHealthSprites = this.bloodGushC;
			}
			else if (Random.chancePercent(50)) {
				damageHealthSprites = this.bloodGushA;
			}
			else {
				damageHealthSprites = this.bloodGushD;
			}
			scale = 1080 / damageHealthSprites.height;
		}

		const playerIndex: number = this.getPlayerIndex(playerId);

		const x: number = this.getPlayerX(playerIndex);
		const center: Vector = new Vector(x, 1080);
		if (x < 300 && flipHorizontally && Random.chancePercent(70))
			flipHorizontally = false;
		const spritesEffect: SpritesEffect = new SpritesEffect(damageHealthSprites, new ScreenPosTarget(center), 0, 0, 100, 100, flipHorizontally);
		spritesEffect.scale = scale;
		spritesEffect.start();
		if (flipHorizontally)
			return SplatterDirection.Left;
		return SplatterDirection.Right;
	}

	getScalableBlood(): BloodSprites {
		if (Random.chancePercent(33)) {
			return this.bloodGushA;
		}
		else if (Random.chancePercent(50)) {
			return this.bloodGushD;
		}
		else
			return this.bloodGushE;
	}

	drawGame(nowMs: DOMHighResTimeStamp) {
		this.nowMs = nowMs;
		this.update(nowMs);
	}

	private prepareToDrawName(context: CanvasRenderingContext2D, player: Character, playerIndex: number) {
		context.textAlign = 'center';
		context.textBaseline = 'middle';
		context.font = '40px Enchanted Land';
		let playerName: string = player.name;
		const time: Date = new Date();
		// TODO: likelyMorningCodeRushedShow - also - not on a Sunday.
		const likelyMorningCodeRushedShow: boolean = time.getHours() < 16;

		const hasConditions = false;
		// TODO: Get nameplates to be shorter when the player has conditions. Next three lines are part of that, but conditions don't seem to resize or position themselves correctly when conditions toggle from None to any condition.
		//const playerStats: PlayerStats = this.playerStats.getPlayerStatsById(player.playerID);
		//if (playerStats) {
		//	// TODO: figure out why player.ActiveConditions is undefined.
		//	hasConditions = playerStats.Conditions !== Conditions.None && playerStats.Conditions !== undefined;
		//}

		if ((this.inCombat || likelyMorningCodeRushedShow || player.hitPoints < player.maxHitPoints || hasConditions) && playerName) {
			const spaceIndex: number = playerName.indexOf(' ');
			if (spaceIndex > 0) {
				playerName = playerName.substr(0, spaceIndex).trim();
				if (playerName === "L'il") // Special case.
					playerName = 'Cutie';
			}
		}
		const sprite: SpriteProxy = this.nameplateMain.spriteProxies[playerIndex];
		const nameplateMaxWidth = 358;
		let hpStr: string;
		if (player.tempHitPoints > 0)
			hpStr = '+' + player.tempHitPoints;
		else
			hpStr = player.hitPoints.toString() + '/' + player.maxHitPoints;
		const centerX: number = this.getPlayerX(playerIndex);
		const hidingHitPoints: boolean = !this.inCombat && player.hitPoints === player.maxHitPoints;
		let hpWidth: number = context.measureText(hpStr).width;
		const nameWidth: number = context.measureText(playerName).width;
		let nameHpMargin = 25;
		let additionalWidth: number = hpWidth + nameHpMargin;
		if (hidingHitPoints) {
			hpWidth = 0;
			additionalWidth = 0;
			nameHpMargin = 0;
		}
		const totalNameplateTextWidth: number = nameWidth + nameHpMargin + hpWidth;
		const innerNameplateMargin = 8;
		let horizontalMargin: number = (nameplateMaxWidth - totalNameplateTextWidth) / 2 - innerNameplateMargin * 2;
		if (horizontalMargin < 0) {
			horizontalMargin = 0;
		}
		return { horizontalMargin, sprite, centerX, additionalWidth, nameWidth, nameHpMargin, hpWidth, hidingHitPoints, playerName, hpStr };
	}

	getPlateWidth(context: CanvasRenderingContext2D, player: Character, playerIndex: number): number {
		if (!player)
			return;
		const { horizontalMargin }: { horizontalMargin: number; sprite: SpriteProxy; centerX: number; additionalWidth: number; nameWidth: number; nameHpMargin: number; hpWidth: number; hidingHitPoints: boolean; playerName: string; hpStr: string } = this.prepareToDrawName(context, player, playerIndex);
		const plateWidth: number = this.nameplateMain.spriteWidth - 2 * horizontalMargin;
		//console.log(`this.plateWidthCache[player.playerID] = ${plateWidth};`);
		return plateWidth;
	}

	drawNameplate(context: CanvasRenderingContext2D, player: Character, playerIndex: number): void {
		if (!player || !player.ShowingNameplate)
			return;

		const { horizontalMargin, sprite, centerX, additionalWidth, nameWidth, nameHpMargin, hpWidth, hidingHitPoints, playerName, hpStr }: { horizontalMargin: number; sprite: SpriteProxy; centerX: number; additionalWidth: number; nameWidth: number; nameHpMargin: number; hpWidth: number; hidingHitPoints: boolean; playerName: string; hpStr: string } = this.prepareToDrawName(context, player, playerIndex);

		const plateWidth: number = this.nameplateMain.spriteWidth - 2 * horizontalMargin;

		const height: number = this.nameplateMain.spriteHeight;
		this.nameplateMain.baseAnimation.drawCroppedByIndex(context, sprite.x + horizontalMargin, sprite.y, 0, horizontalMargin, 0, plateWidth, height, plateWidth, height);

		const leftX: number = centerX - plateWidth / 2;
		const yPos: number = sprite.y - this.nameplateParts.originY;
		const originX: number = this.nameplateParts.originX;
		this.nameplateParts.baseAnimation.drawByIndex(context, leftX - originX, yPos, 0);
		const rightX: number = centerX + plateWidth / 2;
		this.nameplateParts.baseAnimation.drawByIndex(context, rightX - originX, yPos, 1);

		const additionalHalfWidth: number = additionalWidth / 2;
		const nameCenter: number = centerX - additionalHalfWidth;
		const hpCenter: number = nameCenter + nameWidth / 2 + nameHpMargin + hpWidth / 2;

		if (!hidingHitPoints) {
			const separatorX: number = nameCenter + nameWidth / 2 + nameHpMargin / 2;
			this.nameplateParts.baseAnimation.drawByIndex(context, separatorX - originX, yPos, 2);
		}

		this.drawNameText(context, playerName, nameCenter, hidingHitPoints, hpStr, hpCenter);
	}

	private drawNameText(context: CanvasRenderingContext2D, playerName: string, nameCenter: number, hidingHitPoints: boolean, hpStr: string, hpCenter: number) {
		context.fillStyle = '#000000';
		let shadowOffset = -2;
		this.drawNameAndHitPoints(context, playerName, nameCenter, shadowOffset, hidingHitPoints, hpStr, hpCenter);
		shadowOffset = 2;
		context.globalAlpha = 0.5;
		this.drawNameAndHitPoints(context, playerName, nameCenter, shadowOffset, hidingHitPoints, hpStr, hpCenter);
		context.globalAlpha = 1;
		context.fillStyle = '#ffffff';
		this.drawNameAndHitPoints(context, playerName, nameCenter, 0, hidingHitPoints, hpStr, hpCenter);
	}

	private drawNameAndHitPoints(context: CanvasRenderingContext2D, playerName: string, nameCenter: number, shadowOffset: number, hidingHitPoints: boolean, hpStr: string, hpCenter: number) {
		context.fillText(playerName, nameCenter + shadowOffset, DragonFrontGame.nameCenterY + shadowOffset);
		if (!hidingHitPoints) {

			const slashIndex: number = hpStr.indexOf('/');
			if (slashIndex > 0 && context.fillStyle === '#ffffff') {
				const firstHp: string = hpStr.substr(0, slashIndex);
				const secondHp: string = hpStr.substr(slashIndex);
				const firstWidth: number = context.measureText(firstHp).width;
				const secondWidth: number = context.measureText(secondHp).width;
				const left: number = hpCenter - (firstWidth + secondWidth) / 2.0;
				context.textAlign = 'left';
				context.fillText(firstHp, left + shadowOffset, DragonFrontGame.nameCenterY + shadowOffset);
				context.fillStyle = '#b6a89a';
				context.fillText(secondHp, left + shadowOffset + firstWidth, DragonFrontGame.nameCenterY + shadowOffset);
				context.textAlign = 'center';
			}
			else if (hpStr.startsWith('+') && context.fillStyle === '#ffffff') {
				context.fillStyle = DragonFrontGame.FontColorTempHpOnNameplates;
				context.fillText(hpStr, hpCenter + shadowOffset, DragonFrontGame.nameCenterY + shadowOffset);
			}
			else
				context.fillText(hpStr, hpCenter + shadowOffset, DragonFrontGame.nameCenterY + shadowOffset);
		}
	}

	drawNameplates(context: CanvasRenderingContext2D, nowMs: number) {
		let activePlayer: Character = null;
		let playerIndex = 0;
		for (let i = 0; i < this.players.length; i++) {
			const player: Character = this.players[i];
			if (player.playerID === this.activePlayerId) {
				activePlayer = player;
				playerIndex = i;
				continue;
			}
			this.drawNameplate(context, player, i);
		}

		// Place active player on top:
		this.drawNameplate(context, activePlayer, playerIndex);
	}

	moveFred(movement: string): void {
		this.fred.playAnimation(movement, this.getPlayerX(this.getPlayerIndex(this.Player_Fred)));
	}

	floatPlayerText(playerId: number, message: string, fillColor: string, outlineColor: string): void {
		const playerX: number = this.getPlayerX(this.getPlayerIndex(playerId));
		this.addFloatingText(playerX, message, fillColor, outlineColor);
	}

	showFpsWindow: boolean;
	handleFpsChange(frameRateChangeData: FrameRateChangeData): void {
		this.changeFramerate(frameRateChangeData.FrameRate);
	}

	speechBubble(speechStr: string) {
		this.speechBubbleManager.sayOrThinkSomething(this.world.ctx, speechStr);
	}

	updateInGameCreatures(commandData: string) {
		const inGameCommand: InGameCommand = JSON.parse(commandData);
		this.inGameCreatureManager.processInGameCreatureCommand(inGameCommand.Command, inGameCommand.Creatures, this.dragonFrontSounds);
	}

	updateScreenBeforeWorldRender(context: CanvasRenderingContext2D, nowMs: number) {
		this.inGameCreatureManager.drawInGameCreatures(context, nowMs);
	}

	cardCommand(cardStr: string) {
		const cardDto: CardDto = JSON.parse(cardStr);
		if (cardDto.Command === 'ShowCard')
			this.cardManager.showCard(cardDto.Card);
		else if (cardDto.Command === 'Update Hands')
			this.cardManager.updateHands(cardDto.Hands);
	}
}

class PlayerHealth {
	PlayerIds: Array<number> = new Array<number>();
	DamageHealth: number;
	IsTempHitPoints: boolean;
	constructor() {
	}
}
