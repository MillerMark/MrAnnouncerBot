class CoinManager {
	constructor() {

	}
	allCoins: SpriteCollection = new SpriteCollection();

	loadCoin(name: string): Sprites {
		let coins: Sprites = new Sprites(`Coins/${name}`, 60, fps30, AnimationStyle.Loop, true);
		coins.name = name;
		coins.originX = 17;
		coins.originY = 17;
		coins.moves = true;
		return coins;
	}

	loadResources() {
		this.allCoins.add(this.loadCoin('Gold'));
		this.allCoins.add(this.loadCoin('Silver'));
		this.allCoins.add(this.loadCoin('Copper'));
		this.allCoins.add(this.loadCoin('Electrum'));
		this.allCoins.add(this.loadCoin('Platinum'));
	}

	addCoinsByName(numCoins: number, coinName: string, x: number): any {
		let sprites: Sprites = this.allCoins.getSpritesByName(coinName);
		if (!sprites)
			return;
		let now: number = performance.now();
		if (numCoins > 0) {
			for (let i = 0; i < numCoins; i++)
				this.addDroppingCoin(sprites, x, now);
		}
		else {
			numCoins = Math.abs(numCoins);
			for (let i = 0; i < numCoins; i++)
				this.addFlyingCoin(sprites, x, now);
		}
	}

	private addDroppingCoin(sprites: Sprites, x: number, now: number) {
		let y: number = 0;
		let sprite: SpriteProxy = sprites.add(x, y, -1);
		sprite.verticalThrustOverride = 9.8;
		if (Random.chancePercent(50))
			sprite.animationReverseOverride = true;
		sprite.frameIntervalOverride = fps30 * Random.between(0.5, 2);
		sprite.timeStart = now + Random.between(0, 800);
		sprite.velocityX = Random.between(-1.75, 1.75);
		sprite.velocityY = Random.between(0, -3);
		sprite.rotation = Random.max(360);
		sprite.autoRotationDegeesPerSecond = Random.between(-10, 10);
		sprite.initialRotation = sprite.rotation;
		sprite.expirationDate = sprite.timeStart + 3000;
		sprite.fadeOutTime = 0;
		sprite.fadeInTime = 0;
	}

	static readonly halfHeightDmFrame: number = 212;
	static readonly halfWidthDmFrame: number = 213;
	static readonly gravity: number = 9.8;

	private addFlyingCoin(sprites: Sprites, x: number, now: number) {
		let y: number = screenHeight;
		x += Random.between(-100, 100);
		let sprite: SpriteProxy = sprites.add(x, y, -1);
		sprite.verticalThrustOverride = CoinManager.gravity;
		if (Random.chancePercent(50))
			sprite.animationReverseOverride = true;
		sprite.frameIntervalOverride = fps30 * Random.between(0.5, 2);
		sprite.timeStart = now + Random.between(0, 800);

		let airTime: number = CoinManager.getAirTimeToDmBoxSec();

		sprite.rotation = Random.max(360);
		sprite.autoRotationDegeesPerSecond = Random.between(-10, 10);
		sprite.initialRotation = sprite.rotation;

		sprite.velocityX = Physics.pixelsToMeters(screenWidth - CoinManager.halfWidthDmFrame - x) / airTime;
		sprite.velocityY = -Physics.getFinalVelocity(airTime, 0, CoinManager.gravity);
		sprite.expirationDate = sprite.timeStart + airTime * 1000;
		sprite.fadeOutTime = 300;
		sprite.fadeInTime = 0;
	}

	static getAirTimeToDmBoxSec(): number {
		return CoinManager.getAirTimeSec(screenHeight - CoinManager.halfHeightDmFrame);
	}

	static getAirTimeFullDropSec(): number {
		return CoinManager.getAirTimeSec(screenHeight);
	}

	static getAirTimeSec(totalHeightPx: number) {
		let heightMeters: number = Physics.pixelsToMeters(totalHeightPx);
		return Physics.getDropTime(heightMeters, CoinManager.gravity);
	}

	addCoins(coins: Coins, x: number) {
		if (coins.NumGold != 0)
			this.addCoinsByName(coins.NumGold, 'Gold', x);
		if (coins.NumSilver != 0)
			this.addCoinsByName(coins.NumSilver, 'Silver', x);
		if (coins.NumCopper != 0)
			this.addCoinsByName(coins.NumCopper, 'Copper', x);
		if (coins.NumElectrum != 0)
			this.addCoinsByName(coins.NumElectrum, 'Electrum', x);
		if (coins.NumPlatinum != 0)
			this.addCoinsByName(coins.NumPlatinum, 'Platinum', x);
	}

	draw(context: CanvasRenderingContext2D, now: number) {
		context.globalAlpha = 1;
		this.allCoins.updatePositions(now);
		this.allCoins.draw(context, now);
	}
}

class DragonFrontGame extends DragonGame {
	coinManager: CoinManager = new CoinManager();
	textAnimations: Animations = new Animations();

	showFpsMessage(message: string): any {
		this.addUpperRightStatusMessage(message, '#000000', '#ffffff');
	}

	readonly fullScreenDamageThreshold: number = 15;
	readonly heavyDamageThreshold: number = 15;
	readonly mediumDamageThreshold: number = 6;
	readonly lightDamageThreshold: number = 4;

	readonly panelScale: number = 0.67;
	readonly panelShiftY: number = 16;
	readonly panelWidth: number = 391;
	readonly panelMargin: number = 34;
	readonly maxPanelWidth: number = (this.panelWidth - this.panelMargin * 2) * this.panelScale;

	layerSuffix: string = 'Front';
	emitter: Emitter;
	shouldDrawCenterCrossHairs: boolean = false;
	denseSmoke: Sprites;
	shield: Sprites;
	poof: Sprites;
	fred: Fred = new Fred();
	bloodGushA: BloodSprites;	// Totally contained - 903 high.
	bloodGushB: BloodSprites;  // Full screen, not contained at all - blood escapes top and right edges.
	bloodGushC: BloodSprites;	// Full screen, not contained at all - blood escapes top and right edges.
	bloodGushD: BloodSprites;  // Totally contained, 575 pixels high.
	bloodGushE: BloodSprites;  // Totally contained, 700 pixels high.

	dndTimeDatePanel: SpriteProxy;
	clockPanel: Sprites;
	charmed: Sprites;
	restrained: Sprites;
	sparkShower: Sprites;
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
	fireWall: Sprites;
	stars: Sprites;
	fumes: Sprites;
	allBackEffects: SpriteCollection;
	allFrontEffects: SpriteCollection;
	bloodEffects: SpriteCollection;
	dragonFrontSounds: DragonFrontSounds;
	activePlayerId: number;
	playerDataSet: boolean = false;

	constructor(context: CanvasRenderingContext2D) {
		super(context);
		this.dragonFrontSounds = new DragonFrontSounds('GameDev/Assets/DragonH/SoundEffects');
	}

	update(timestamp: number) {
		this.updateGravity();
		super.update(timestamp);
	}

	updateScreen(context: CanvasRenderingContext2D, now: number) {
		this.drawTimePlusEffects(context, now);
		this.allBackEffects.draw(context, now);

		if (!this.playerDataSet) {
			this.ShowWaitingForInitializationMessage(context, '#00ff00', 'Front Overlay is waiting for player data to be initialized.', 200);
		}

		super.updateScreen(context, now);

		if (this.shouldDrawCenterCrossHairs)
			drawCrossHairs(myContext, screenCenterX, screenCenterY);

		this.fred.draw(context, now);

		this.bloodEffects.draw(context, now);

		this.coinManager.draw(context, now);
		this.showNameplates(context, now);
		this.allFrontEffects.draw(context, now);
		this.textAnimations.removeExpiredAnimations(now);
		this.textAnimations.updatePositions(now);
		this.textAnimations.render(context, now);
	}

	protected drawTimePlusEffects(context: CanvasRenderingContext2D, now: number) {
		super.drawClockLayerEffects(context, now);
		this.drawGameTime(context);
	}

	private drawGameTime(context: CanvasRenderingContext2D) {
		if (!this.dndTimeStr)
			return;
		const timeFont: string = 'px Baskerville Old Face';
		const verticalMargin: number = 10;
		const timeHeight: number = 32;
		const dateHeight: number = 24;
		context.font = timeHeight + timeFont;
		let timeWidth: number = context.measureText(this.dndTimeStr.trim()).width;
		let timeHalfWidth: number = timeWidth / 2;
		let centerX: number = this.getClockX();
		let centerY: number = this.clockBottomY - timeHeight / 2 - verticalMargin;
		if (this.inCombat)
			context.fillStyle = '#500506';
		else
			context.fillStyle = '#0b0650';
		let lastColonPos: number = this.dndTimeStr.lastIndexOf(':');
		let firstTimePart: string = this.dndTimeStr.substr(0, lastColonPos).trim();
		let lastTimePart: string = this.dndTimeStr.substr(lastColonPos).trim();
		let leftX: number = centerX - timeHalfWidth;
		context.textAlign = 'left';
		context.textBaseline = 'middle';
		context.fillText(firstTimePart, leftX, centerY);
		let firstTimePartWidth: number = context.measureText(firstTimePart).width;
		context.globalAlpha = 0.75;
		context.fillText(lastTimePart, leftX + firstTimePartWidth, centerY);
		context.globalAlpha = 1;
		context.textAlign = 'center';
		centerY += timeHeight;
		context.font = dateHeight + timeFont;
		let dateFontScale: number = 1;
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
		this.loadWarHammer();
		this.loadLongSword();
		this.loadJavelin();
	}

	private loadWarHammer() {
		const warHammerOriginX: number = 516;
		const warHammerOriginY: number = 685;
		this.loadWeapon('WarHammer', 'MagicBackHead', warHammerOriginX, warHammerOriginY);
		this.loadWeapon('WarHammer', 'Weapon', warHammerOriginX, warHammerOriginY);
		this.loadWeapon('WarHammer', 'MagicFrontHead', warHammerOriginX, warHammerOriginY);
		this.loadWeapon('WarHammer', 'MagicHandle', warHammerOriginX, warHammerOriginY);
	}

	private loadLongSword() {
		const longSwordOriginX: number = 410;
		const longSwordOriginY: number = 717;
		this.loadWeapon('LongSword', 'MagicB', longSwordOriginX, longSwordOriginY);  // Big fire effect in back.
		this.loadWeapon('LongSword', 'Weapon', longSwordOriginX, longSwordOriginY);
		this.loadWeapon('LongSword', 'MagicA', longSwordOriginX, longSwordOriginY);
	}

	private loadJavelin() {
		const javelinOriginX: number = 426;
		const javelinOriginY: number = 999;
		this.loadWeapon('Javelin', 'Weapon', javelinOriginX, javelinOriginY);
		this.loadWeapon('Javelin', 'Magic', javelinOriginX, javelinOriginY);
	}

	private loadStaff() {
		const staffOriginX: number = 352;
		const staffOriginY: number = 744;
		this.loadWeapon('Staff', 'Magic', staffOriginX, staffOriginY);
		this.loadWeapon('Staff', 'Weapon', staffOriginX, staffOriginY);
	}

	private loadBattleAxe() {
		const battleAxeOriginX: number = 341;
		const battleAxeOriginY: number = 542;
		this.loadWeapon('BattleAxe', 'MagicA', battleAxeOriginX, battleAxeOriginY);
		this.loadWeapon('BattleAxe', 'MagicB', battleAxeOriginX, battleAxeOriginY);
		this.loadWeapon('BattleAxe', 'Weapon', battleAxeOriginX, battleAxeOriginY);
	}

	private loadClub() {
		const clubOriginX: number = 440;
		const clubOriginY: number = 745;
		this.loadWeapon('Club', 'Weapon', clubOriginX, clubOriginY);
		this.loadWeapon('Club', 'Magic', clubOriginX, clubOriginY);
	}

	private loadGreatSword() {
		const greatSwordOriginX: number = 306;
		const greatSwordOriginY: number = 837;
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
		this.fred.loadResources();
		this.coinManager.loadResources();

		this.denseSmoke = new Sprites('Smoke/Dense/DenseSmoke', 116, fps30, AnimationStyle.Sequential, true);
		this.denseSmoke.name = 'DenseSmoke';
		this.denseSmoke.originX = 309;
		this.denseSmoke.originY = 723;

		this.shield = new Sprites('Weapons/Shield/Shield', 88, fps30, AnimationStyle.Sequential, true);
		this.shield.name = 'Shield';
		this.shield.originX = 125;
		this.shield.originY = 445;

		this.poof = new Sprites('Smoke/Poof/Poof', 67, fps30, AnimationStyle.Sequential, true);
		this.poof.name = 'Puff';
		this.poof.originX = 229;
		this.poof.originY = 698;

		this.sparkShower = new Sprites('Sparks/Big/BigSparks', 63, fps30, AnimationStyle.Sequential, true);
		this.sparkShower.name = 'SparkShower';
		this.sparkShower.originX = 443;
		this.sparkShower.originY = 595;

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

		this.nameplateParts.addShifted(0, DragonFrontGame.nameCenterY, 0, 0);
		this.nameplateParts.addShifted(0, DragonFrontGame.nameCenterY, 1, 0);
		this.nameplateParts.addShifted(0, DragonFrontGame.nameCenterY, 2, 0);

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
		this.allFrontEffects.add(this.sparkShower);
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
		this.bloodEffects.add(this.bloodGushA);
		this.bloodEffects.add(this.bloodGushB);
		this.bloodEffects.add(this.bloodGushC);
		this.bloodEffects.add(this.bloodGushD);
		this.bloodEffects.add(this.bloodGushE);
		this.allBackEffects.add(this.charmed);
		this.allBackEffects.add(this.restrained);
		this.allWindupEffects.add(this.shield);

		this.clockPanel = new Sprites('Clock/TimeDisplayPanel', 2, fps30, AnimationStyle.Static);
		this.clockPanel.name = 'ClockPanel';
		this.clockPanel.originX = DragonGame.ClockOriginX;
		this.clockPanel.originY = 67;

		this.dndTimeDatePanel = this.clockPanel.add(this.getClockX(), this.panelShiftY + this.getClockY()).setScale(this.panelScale);

		this.clockLayerEffects.add(this.fireWall);
		this.clockLayerEffects.add(this.clockPanel);
	}

	private createFireBallBehindClock(hue: number): any {
		let x: number;
		let y: number;
		x = this.getClockX() - 90 * this.clockScale;
		const fireBallAdjust: number = 11;
		y = this.clockBottomY - this.clockPanel.originY + fireBallAdjust;
		let pos: Vector = new Vector(x - this.fireBallBack.originX, y - this.fireBallBack.originY);
		this.fireBallBack.sprites.push(new ColorShiftingSpriteProxy(0, pos).setHueSatBrightness(hue).setScale(this.clockScale));
		this.fireBallFront.sprites.push(new ColorShiftingSpriteProxy(0, pos).setHueSatBrightness(hue).setScale(this.clockScale));
		this.dragonFrontSounds.safePlayMp3('HeavyPoof');
	}

	private createFireWallBehindClock() {
		const displayMargin: number = -10;
		let fireWall: SpriteProxy = this.fireWall.add(this.getClockX(), this.clockBottomY - this.panelScale * this.clockPanel.originY + displayMargin);
		fireWall.scale = 0.6 * this.clockScale;
		fireWall.opacity = 0.8;
		fireWall.fadeOutTime = 400;
		this.dragonFrontSounds.safePlayMp3('FlameOn');
	}

	exitingCombat() {
		super.exitingCombat();
		this.dndTimeDatePanel.frameIndex = 0;
		this.fireWall.sprites = [];
		this.createFireBallBehindClock(200);
	}

	enteringCombat() {
		super.enteringCombat();
		this.dndTimeDatePanel.frameIndex = 1;
		this.createFireWallBehindClock();
		this.createFireBallBehindClock(330);
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

		let sprayAngle: number = Random.intBetween(270 - 45, 270 + 45);
		let minVelocity: number = 9;
		let maxVelocity: number = 16;
		let angleAwayFromUp: number = Math.abs(sprayAngle - 270);
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
			console.log('draw Cross Hairs');
			this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
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
			console.log('Blood');
			this.world.removeCharacter(this.emitter);
			this.testBloodEmitter();
			this.world.addCharacter(this.emitter);
			return true;
		}

		// poof 40 50
		if (testCommand.startsWith("poof")) {
			let split: string[] = testCommand.split(' ');

			let hue: number = 0;
			let saturation: number = 100;
			let brightness: number = 100;

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

			this.denseSmoke.sprites.push(new ColorShiftingSpriteProxy(0, new Vector(450 - this.denseSmoke.originX, 1080 - this.denseSmoke.originY)).setHueSatBrightness(hue, saturation, brightness));
		}


		if (testCommand.startsWith("fb")) {
			let split: string[] = testCommand.split(' ');

			let hue: number = 0;
			let saturation: number = 100;
			let brightness: number = 100;

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

			this.fireBallBack.sprites.push(new SpriteProxy(0, 450 - this.fireBallBack.originX, 1080 - this.fireBallBack.originY));
			this.fireBallFront.sprites.push(new ColorShiftingSpriteProxy(0, new Vector(450 - this.fireBallFront.originX, 1080 - this.fireBallFront.originY)).setHueSatBrightness(hue, saturation, brightness));
		}

		return false;
	}



	protected triggerSoundEffect(dto: any): void {
		let soundFileName: string = dto.soundFileName;
		if (soundFileName.indexOf('.') < 0)
			soundFileName += '.mp3';
		console.log("Playing " + Folders.assets + 'SoundEffects/' + soundFileName);
		new Audio(Folders.assets + 'SoundEffects/' + soundFileName).play();
	}

	protected triggerEmitter(dto: any, center: Vector): void {
		this.world.removeCharacter(this.emitter);

		console.log('emitter: ' + dto);

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
		console.log(this.emitter);
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
		else if (dto.spriteName === 'SparkShower')
			sprites = this.sparkShower;
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

		let flipHorizontally: boolean = false;

		if (dto.horizontalFlip || horizontallyFlippable && Random.chancePercent(50))
			flipHorizontally = true;

		if (!sprites) {
			console.error(`"${dto.spriteName}" sprite not found.`);
			return;
		}

		let spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(center), dto.startFrameIndex, dto.hueShift, dto.saturation, dto.brightness,
			flipHorizontally, dto.verticalFlip, dto.scale, dto.rotation, dto.autoRotation);

		spritesEffect.start();

		if (dto.spriteName === 'FireBall') {
			sprites = this.fireBallFront;
			let spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(center), dto.startFrameIndex, dto.secondaryHueShift, dto.secondarySaturation, dto.secondaryBrightness);
			spritesEffect.start();
		}
	}

	static readonly nameCenterY: number = 1052;
	static readonly nameplateHalfHeight: number = 24;

	initializePlayerData(playerData: string): any {
		super.initializePlayerData(playerData);

		this.playerDataSet = true;

		for (var i = 0; i < this.players.length; i++) {
			let centerX: number = this.getPlayerX(i);
			this.nameplateMain.addShifted(centerX, DragonFrontGame.nameCenterY - DragonFrontGame.nameplateHalfHeight, 0, 0);
		}
	}

	playerChanged(playerID: number, pageID: number, playerData: string): void {
		super.playerChanged(playerID, pageID, playerData);
		this.activePlayerId = playerID;
	}

	// TODO: Keep these in sync with those MainWindow.xaml.cs
	readonly Player_Lady: number = 0;
	readonly Player_Shemo: number = 1;
	readonly Player_Merkin: number = 2;
	readonly Player_Ava: number = 3;
	readonly Player_Fred: number = 4;
	readonly Player_Willy: number = 5;

	changePlayerWealth(playerWealthDto: string): void {
		let wealthChange: WealthChange = JSON.parse(playerWealthDto);

		let timeoutMs: number = 2000;
		let airTimeSec: number = CoinManager.getAirTimeFullDropSec();
		if (wealthChange.Coins.TotalGold < 0) {
			timeoutMs = 500;
			airTimeSec = CoinManager.getAirTimeToDmBoxSec();
		}
		for (let i = 0; i < wealthChange.PlayerIds.length; i++) {
			let playerId = wealthChange.PlayerIds[i];
			let playerX: number = this.getPlayerX(this.getPlayerIndex(playerId));
			this.coinManager.addCoins(wealthChange.Coins, playerX);
			let prefix: string = '';
			this.dragonFrontSounds.safePlayMp3('Coins/Coins[3]');
			if (wealthChange.Coins.TotalGold > 0)
				prefix = '+';
			setTimeout(() => {
				this.addFloatingPlayerText(playerX, `${prefix}${wealthChange.Coins.TotalGold} gp`, DragonFrontGame.FontColorGold, DragonFrontGame.FontOutlineGold);
			}, timeoutMs);

			setTimeout(() => {
				this.dragonFrontSounds.safePlayMp3('Coins/Coins[3]');
			}, airTimeSec * 1000);
		}
	}

	changePlayerHealth(playerHealthDto: string): void {
		let playerHealth: PlayerHealth = JSON.parse(playerHealthDto);

		for (var i = 0; i < playerHealth.PlayerIds.length; i++) {
			if (playerHealth.DamageHealth < 0) {
				this.showDamage(playerHealth, i);
			}
			else {
				this.showHealth(playerHealth, i);
			}
		}
	}

	private showHealth(playerHealth: PlayerHealth, i: number) {
		setTimeout(() => {
			let playerX: number = this.getPlayerX(this.getPlayerIndex(playerHealth.PlayerIds[i]));
			let fontColor: string = DragonFrontGame.FontColorHealth;
			let outlineColor: string = DragonFrontGame.FontOutlineHealth;
			let suffix: string = 'hp';
			if (playerHealth.IsTempHitPoints) {
				fontColor = DragonFrontGame.FontColorTempHp;
				outlineColor = DragonFrontGame.FontOutlineTempHp;
				suffix = 'temp hp';
			}

			this.addFloatingPlayerText(playerX, `+${playerHealth.DamageHealth} ${suffix}`, fontColor, outlineColor);
		}, 2000);
		this.showHealthGain(playerHealth.PlayerIds[i], playerHealth.DamageHealth, playerHealth.IsTempHitPoints);
		this.dragonFrontSounds.playRandom('Healing/Healing', 5);
	}

	addUpperRightStatusMessage(text: string, outlineColor: string, fontColor: string): any {
		let textEffect: TextEffect = this.textAnimations.addText(new Vector(1920, 0), text, 3500);
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.scale = 1;
		textEffect.fadeOutTime = 2500;
		textEffect.fadeInTime = 600;
		textEffect.textAlign = 'right';
		textEffect.textBaseline = 'top';
	}

	addFloatingPlayerText(xPos: number, text: string, fontColor: string, outlineColor: string) {
		let textEffect: TextEffect = this.textAnimations.addText(new Vector(xPos, 1080), text, 3500);
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.scale = 1;
		textEffect.targetScale = 6;
		textEffect.fadeOutTime = 2500;
		textEffect.fadeInTime = 600;
		textEffect.velocityX = 0;
		textEffect.velocityY = -6;
		textEffect.verticalThrust = 1.3;
	}

	static readonly FontColorDamage: string = '#ca0000';
	static readonly FontOutlineDamage: string = '#000000';
	static readonly FontColorHealth: string = '#5681d4';
	static readonly FontOutlineHealth: string = '#ffffff';
	static readonly FontColorTempHp: string = '#d4569d';
	static readonly FontOutlineTempHp: string = '#ffffff';
	static readonly FontColorGold: string = '#fedf80';
	static readonly FontOutlineGold: string = '#5f4527';


	private showDamage(playerHealth: PlayerHealth, i: number) {
		let playerX: number = this.getPlayerX(this.getPlayerIndex(playerHealth.PlayerIds[i]));
		this.addFloatingPlayerText(playerX, `${playerHealth.DamageHealth} hp`, DragonFrontGame.FontColorDamage, DragonFrontGame.FontOutlineDamage);
		let fredIsTakingDamage: boolean = false;
		let fredIsGettingHitByBlood: boolean = false;
		let splatterDirection: SplatterDirection;
		splatterDirection = this.showDamageForPlayer(playerHealth.DamageHealth, playerHealth.PlayerIds[i]);
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
		if (playerHealth.DamageHealth < 0 && splatterDirection == SplatterDirection.Left) {
			let absDamage: number = -playerHealth.DamageHealth;
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
		if (this.shield.sprites.length === 0) {
			this.shield.add(this.getPlayerX(0), 1080);
			this.dragonFrontSounds.playMp3In(100, 'Windups/ShieldUp');
		}
	}


	private showDamageForPlayer(damageHealth: number, playerId: number): SplatterDirection {
		let flipHorizontally: boolean = false;
		if (Random.chancePercent(50))
			flipHorizontally = true;
		let damageHealthSprites: BloodSprites;
		let scale: number = 1;
		if (damageHealth > 0)
			return SplatterDirection.None;

		let absDamage: number = -damageHealth;

		if (absDamage >= this.heavyDamageThreshold)
			this.dragonFrontSounds.playRandom('Damage/Heavy/GushHeavy', 13);
		else if (absDamage >= this.mediumDamageThreshold)
			this.dragonFrontSounds.playRandom('Damage/Medium/GushMedium', 29);
		else
			this.dragonFrontSounds.playRandom('Damage/Light/GushLight', 15);


		if (absDamage < this.fullScreenDamageThreshold) {
			damageHealthSprites = this.getScalableBlood();

			let desiredBloodHeight: number = 1080 * absDamage / this.fullScreenDamageThreshold;
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

		let playerIndex: number = this.getPlayerIndex(playerId);

		let x: number = this.getPlayerX(playerIndex);
		let center: Vector = new Vector(x, 1080);
		if (x < 300 && flipHorizontally && Random.chancePercent(70))
			flipHorizontally = false;
		let spritesEffect: SpritesEffect = new SpritesEffect(damageHealthSprites, new ScreenPosTarget(center), 0, 0, 100, 100, flipHorizontally);
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

	showNameplate(context: CanvasRenderingContext2D, player: Character, playerIndex: number, now: number): void {
		if (!player || !player.ShowingNameplate)
			return;

		context.textAlign = 'center';
		context.textBaseline = 'middle';
		context.fillStyle = '#ffffff';
		//context.font = '31px Blackadder ITC';
		context.font = '31px Enchanted Land';

		if (!player)
			return;

		let playerName: string = player.name;

		if (this.inCombat) {
			let spaceIndex: number = playerName.indexOf(' ');
			if (spaceIndex > 0)
				playerName = playerName.substr(0, spaceIndex).trim();
		}

		let sprite: SpriteProxy = this.nameplateMain.sprites[playerIndex];

		const nameplateMaxWidth: number = 358;

		var hpStr: string;
		if (player.tempHitPoints > 0)
			hpStr = '+' + player.tempHitPoints;
		else
			hpStr = player.hitPoints.toString() + '/' + player.maxHitPoints;

		let centerX: number = this.getPlayerX(playerIndex);

		let hidingHitPoints: boolean = !this.inCombat && player.hitPoints === player.maxHitPoints;
		let hpWidth: number = context.measureText(hpStr).width;
		let nameWidth: number = context.measureText(playerName).width;
		let nameHpMargin: number = 25;
		let additionalWidth: number = hpWidth + nameHpMargin;
		if (hidingHitPoints) {
			hpWidth = 0;
			additionalWidth = 0;
			nameHpMargin = 0;
		}
		let additionalHalfWidth: number = additionalWidth / 2;
		let nameCenter: number = centerX - additionalHalfWidth;
		let hpCenter: number = nameCenter + nameWidth / 2 + nameHpMargin + hpWidth / 2;
		let totalNameplateTextWidth: number = nameWidth + nameHpMargin + hpWidth;

		const innerNameplateMargin: number = 8;
		let horizontalMargin: number = (nameplateMaxWidth - totalNameplateTextWidth) / 2 - innerNameplateMargin * 2;
		if (horizontalMargin < 0) {
			horizontalMargin = 0;
		}

		let plateWidth: number = this.nameplateMain.spriteWidth - 2 * horizontalMargin;

		let saveFilter: string = (context as any).filter;
		try {
			if (sprite instanceof ColorShiftingSpriteProxy) {
				sprite.hueShift = player.hueShift;
				sprite.shiftColor(context, now);
			}
			let height = this.nameplateMain.spriteHeight;
			this.nameplateMain.baseAnimation.drawCroppedByIndex(context, sprite.x + horizontalMargin, sprite.y, 0, horizontalMargin, 0, plateWidth, height, plateWidth, height);

			let leftX: number = centerX - plateWidth / 2;
			let yPos: number = sprite.y - this.nameplateParts.originY;
			let originX: number = this.nameplateParts.originX;
			this.nameplateParts.baseAnimation.drawByIndex(context, leftX - originX, yPos, 0);
			let rightX: number = centerX + plateWidth / 2;
			this.nameplateParts.baseAnimation.drawByIndex(context, rightX - originX, yPos, 1);
			if (!hidingHitPoints) {
				let separatorX: number = nameCenter + nameWidth / 2 + nameHpMargin / 2;
				this.nameplateParts.baseAnimation.drawByIndex(context, separatorX - originX, yPos, 2);
			}
		}
		finally {
			(context as any).filter = saveFilter;
		}

		context.fillText(playerName, nameCenter, DragonFrontGame.nameCenterY);
		if (!hidingHitPoints) {
			context.fillText(hpStr, hpCenter, DragonFrontGame.nameCenterY);
		}

		//this.nameplateParts.addShifted(centerX - nameplateHalfWidth, nameplateY, 0, player.hueShift);
		//this.nameplateParts.addShifted(centerX + nameplateHalfWidth, nameplateY, 1, player.hueShift);
		//this.nameplateParts.addShifted(centerX + separatorOffset, nameplateY, 2, player.hueShift);
	}

	showNameplates(context: CanvasRenderingContext2D, now: number) {
		let activePlayer: Character = null;
		let playerIndex: number = 0;
		for (var i = 0; i < this.players.length; i++) {
			let player: Character = this.players[i];
			if (player.playerID == this.activePlayerId) {
				activePlayer = player;
				playerIndex = i;
				continue;
			}
			this.showNameplate(context, player, i, now);
		}

		// Place active player on top:
		this.showNameplate(context, activePlayer, playerIndex, now);
	}

	moveFred(movement: string): void {
		// TODO: Implement this!
		this.fred.playAnimation(movement, this.getPlayerX(this.getPlayerIndex(this.Player_Fred)));
	}

}

class PlayerHealth {
	PlayerIds: Array<number> = new Array<number>();
	DamageHealth: number;
	IsTempHitPoints: boolean;
	constructor() {
	}
}
