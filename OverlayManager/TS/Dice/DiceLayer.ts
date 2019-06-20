enum RollScope {
	ActivePlayer,
	Everyone,
	Individuals
}

enum DiceRollType {
	None,
	SkillCheck,
	Attack,
	SavingThrow,
	FlatD20,
	DeathSavingThrow,
	PercentageRoll,
	WildMagic,
	BendLuckAdd,
	BendLuckSubtract,
	LuckRollLow,
	LuckRollHigh,
	DamageOnly,
	HealthOnly,
	ExtraOnly,
	ChaosBolt,
	Initiative
}

enum SpriteType {
	PawPrint,
	Raven,
	Spiral,
	Smoke,
	SmokeExplosion,
	SparkTrail,
	SmallSparks
}

class TrailingEffect {
	constructor(dto: any) {
		this.MinForwardDistanceBetweenPrints = dto.MinForwardDistanceBetweenPrints;
		this.LeftRightDistanceBetweenPrints = dto.LeftRightDistanceBetweenPrints;
		this.Index = dto.Index;
		this.Type = dto.Type;
		this.OnPrintPlaySound = dto.OnPrintPlaySound;
		this.MinSoundInterval = dto.MinSoundInterval;
		this.PlusMinusSoundInterval = dto.PlusMinusSoundInterval;
		this.NumRandomSounds = dto.NumRandomSounds;
		this.intervalBetweenSounds = 0;
	}

	intervalBetweenSounds: number;
	NumRandomSounds: number;
	MinForwardDistanceBetweenPrints: number;
	LeftRightDistanceBetweenPrints: number;
	Index: number;
	Type: SpriteType;
	OnPrintPlaySound: string;
	MinSoundInterval: number;
	PlusMinusSoundInterval: number;
}

const fps40: number = 25;
const fps30: number = 33;
const fps25: number = 40;
const fps20: number = 50;
const matchNormalDieColorsToSpecial: boolean = true;
//const damageDieBackgroundColor: string = '#e49836';
//const damageDieFontColor: string = '#000000';

const successFontColor: string = '#ffffff';
const successOutlineColor: string = '#000000';
const failFontColor: string = '#000000';
const failOutlineColor: string = '#ffffff';

class DiceLayer {
	players: Array<Character> = [];
	private readonly totalDamageTime: number = 7000;
	private readonly totalRollScoreTime: number = 7000;
	private readonly damageModifierTime: number = 5000;
	private readonly rollModifierTime: number = 4000;
	private readonly bonusRollTime: number = 5000;
	private readonly resultTextTime: number = 9000;

	static readonly bonusRollDieColor: string = '#ffcd6d'; // '#adfff4'; // '#ff81bf'; // '#a3ffe6'
	static readonly bonusRollFontColor: string = '#000000';

	static readonly badLuckDieColor: string = '#000000';
	static readonly badLuckFontColor: string = '#ffffff';
	static readonly goodLuckDieColor: string = '#15ff81';
	static readonly goodLuckFontColor: string = '#000000';

	static readonly damageDieBackgroundColor: string = '#a40017';
	static readonly damageDieFontColor: string = '#ffffff';

	static readonly healthDieBackgroundColor: string = '#0074c8';
	static readonly healthDieFontColor: string = '#ffffff';

	static readonly extraDieBackgroundColor: string = '#404040';
	static readonly extraDieFontColor: string = '#ffffff';

	static matchOozeToDieColor: boolean = true;
	animations: Animations = new Animations();
	diceFrontCanvas: HTMLCanvasElement;
	diceBackCanvas: HTMLCanvasElement;
	diceFrontContext: CanvasRenderingContext2D;
	diceBackContext: CanvasRenderingContext2D;
	diceFireball: Sprites;
	cloverRing: Sprites;
	badLuckRing: Sprites;
	sparkTrail: Sprites;
	haloSpins: Sprites;
	//d20Fire: Sprites;
	puff: Sprites;
	freeze: Sprites;
	freezePop: Sprites;
	diceSparks: Sprites;
	sneakAttackTop: Sprites;
	sneakAttackBottom: Sprites;
	pawPrints: Sprites;
	//stars: Sprites;
	dicePortal: Sprites;
	handGrabsDiceTop: Sprites;
	handGrabsDiceBottom: Sprites;
	dicePortalTop: Sprites;
	magicRing: Sprites;
	spirals: Sprites;
	halos: Sprites;
	ravens: Sprites[];
	diceBlowColoredSmoke: Sprites;
	diceBombBase: Sprites;
	diceBombTop: Sprites;
	dieSteampunkTunnel: Sprites;
	allFrontLayerEffects: SpriteCollection;
	allBackLayerEffects: SpriteCollection;
	activePlayerDieColor: string = '#fcd5a6';
	activePlayerDieFontColor: string = '#ffffff';
	activePlayerHueShift: number = 0;
	playerID: number;
  playerDataSet: boolean = false;

	constructor() {
		this.loadSprites();
	}

	loadSprites() {
		Part.loadSprites = true;

		globalBypassFrameSkip = true;

		this.allFrontLayerEffects = new SpriteCollection();
		this.allBackLayerEffects = new SpriteCollection();

		//! Items added later are drawn on top of earlier items.
		this.diceFireball = new Sprites("/Dice/Roll20Fireball/DiceFireball", 71, fps30, AnimationStyle.Sequential, true);
		this.diceFireball.originX = 104;
		this.diceFireball.originY = 155;
		this.allFrontLayerEffects.add(this.diceFireball);

		this.cloverRing = new Sprites("/Dice/Luck/CloverRing", 120, fps30, AnimationStyle.Loop, true);
		this.cloverRing.originX = 141;
		this.cloverRing.originY = 137;
		this.allFrontLayerEffects.add(this.cloverRing);

		this.badLuckRing = new Sprites("/Dice/Luck/BadLuckRing", 120, fps30, AnimationStyle.Loop, true);
		this.badLuckRing.originX = 141;
		this.badLuckRing.originY = 137;
		this.allFrontLayerEffects.add(this.badLuckRing);

		this.freeze = new Sprites("/Dice/Freeze/Freeze", 30, fps30, AnimationStyle.SequentialStop, true);
		this.freeze.originX = 80;
		this.freeze.originY = 80;
		this.allFrontLayerEffects.add(this.freeze);

		this.freezePop = new Sprites("/Dice/Freeze/Pop", 50, fps30, AnimationStyle.Sequential, true);
		this.freezePop.originX = 182;
		this.freezePop.originY = 136;
		this.allFrontLayerEffects.add(this.freezePop);

		this.sparkTrail = new Sprites("/Dice/SparkTrail/SparkTrail", 46, fps40, AnimationStyle.Sequential, true);
		this.sparkTrail.originX = 322;
		this.sparkTrail.originY = 152;
		this.allFrontLayerEffects.add(this.sparkTrail);

		this.pawPrints = new Sprites("/Dice/TigerPaw/TigerPaw", 76, fps30, AnimationStyle.Loop, true);
		this.pawPrints.originX = 50;
		this.pawPrints.originY = 66;
		this.allBackLayerEffects.add(this.pawPrints);

		//this.stars = new Sprites("/Dice/Star/Star", 60, fps30, AnimationStyle.Loop, true);
		//this.stars.originX = 170;
		//this.stars.originY = 165;
		//this.allBackLayerEffects.add(this.stars);

		//this.d20Fire = new Sprites("/Dice/D20Fire/D20Fire", 180, fps30, AnimationStyle.Loop, true);
		//this.d20Fire.originX = 151;
		//this.d20Fire.originY = 149;
		//this.d20Fire.returnFrameIndex = 72;
		//this.allBackLayerEffects.add(this.d20Fire);

		this.dicePortal = new Sprites("/Dice/DiePortal/DiePortal", 73, fps30, AnimationStyle.Sequential, true);
		this.dicePortal.originX = 189;
		this.dicePortal.originY = 212;
		this.allBackLayerEffects.add(this.dicePortal);

		this.handGrabsDiceTop = new Sprites("/Dice/HandGrab/HandGrabsDiceTop", 54, fps30, AnimationStyle.Sequential, true);
		this.handGrabsDiceTop.originX = 153;
		this.handGrabsDiceTop.originY = 127;
		this.allFrontLayerEffects.add(this.handGrabsDiceTop);

		this.handGrabsDiceBottom = new Sprites("/Dice/HandGrab/HandGrabsDiceBottom", 54, fps30, AnimationStyle.Sequential, true);
		this.handGrabsDiceBottom.originX = 153;
		this.handGrabsDiceBottom.originY = 127;
		this.allBackLayerEffects.add(this.handGrabsDiceBottom);

		this.dicePortalTop = new Sprites("/Dice/DiePortal/DiePortalTop", 73, fps30, AnimationStyle.Sequential, true);
		this.dicePortalTop.originX = 189;
		this.dicePortalTop.originY = 212;
		this.allFrontLayerEffects.add(this.dicePortalTop);

		this.diceSparks = new Sprites("/Dice/Sparks/Spark", 49, fps20, AnimationStyle.Loop, true);
		this.diceSparks.originX = 170;
		this.diceSparks.originY = 158;
		this.allBackLayerEffects.add(this.diceSparks);

		this.magicRing = new Sprites("/Dice/MagicRing/MagicRing", 180, fps40, AnimationStyle.Loop, true);
		this.magicRing.returnFrameIndex = 60;
		this.magicRing.originX = 140;
		this.magicRing.originY = 112;
		this.allFrontLayerEffects.add(this.magicRing);

		this.halos = new Sprites("/Dice/Halo/Halo", 90, fps30, AnimationStyle.Loop, true);
		this.halos.originX = 190;
		this.halos.originY = 190;
		this.allFrontLayerEffects.add(this.halos);

		this.haloSpins = new Sprites("/Dice/PaladinSpin/PaladinSpin", 85, fps30, AnimationStyle.Loop, true);
		this.haloSpins.originX = 200;
		this.haloSpins.originY = 200;
		this.allFrontLayerEffects.add(this.haloSpins);

		this.ravens = [];
		this.loadRavens(3);

		this.spirals = new Sprites("/Dice/Spiral/Spiral", 64, fps40, AnimationStyle.Sequential, true);
		this.spirals.originX = 134;
		this.spirals.originY = 107;
		this.allBackLayerEffects.add(this.spirals);

		this.diceBlowColoredSmoke = new Sprites("/Dice/Blow/DiceBlow", 41, fps40, AnimationStyle.Sequential, true);
		this.diceBlowColoredSmoke.originX = 178;
		this.diceBlowColoredSmoke.originY = 170;
		this.allFrontLayerEffects.add(this.diceBlowColoredSmoke);

		this.diceBombBase = new Sprites("/Dice/DieBomb/DieBombBase", 49, fps30, AnimationStyle.Sequential, true);
		this.diceBombBase.originX = 295;
		this.diceBombBase.originY = 316;
		this.allBackLayerEffects.add(this.diceBombBase);

		this.dieSteampunkTunnel = new Sprites("/Dice/SteampunkTunnel/SteampunkTunnelBack", 178, 28, AnimationStyle.Sequential, true);
		this.dieSteampunkTunnel.originX = 142;
		this.dieSteampunkTunnel.originY = 145;
		this.allBackLayerEffects.add(this.dieSteampunkTunnel);

		this.diceBombTop = new Sprites("/Dice/DieBomb/DieBombTop", 39, fps30, AnimationStyle.Sequential, true);
		this.diceBombTop.originX = 295;
		this.diceBombTop.originY = 316;
		this.allFrontLayerEffects.add(this.diceBombTop);

		this.sneakAttackTop = new Sprites("/Dice/SneakAttack/SneakAttackTop", 91, fps30, AnimationStyle.Sequential, true);
		this.sneakAttackTop.originX = 373;
		this.sneakAttackTop.originY = 377;
		this.allFrontLayerEffects.add(this.sneakAttackTop);

		this.puff = new Sprites("/Dice/SneakAttack/Puff", 32, fps30, AnimationStyle.Sequential, true);
		this.puff.originX = 201;
		this.puff.originY = 404;
		this.allFrontLayerEffects.add(this.puff);

		this.sneakAttackBottom = new Sprites("/Dice/SneakAttack/SneakAttackBottom", 91, fps30, AnimationStyle.Sequential, true);
		this.sneakAttackBottom.originX = 373;
		this.sneakAttackBottom.originY = 377;
		this.allBackLayerEffects.add(this.sneakAttackBottom);
	}

	setPlayerData(playerData: string): any {
		this.playerDataSet = true;
		this.players = [];
		let playerDto: Array<Character> = JSON.parse(playerData);
		for (var i = 0; i < playerDto.length; i++) {
			this.players.push(new Character(playerDto[i]));
		}
	}


	loadRavens(count: number): any {
		for (var i = 0; i < count; i++) {
			let raven = new Sprites(`/Dice/Ravens/${i + 1}/Ravens`, 36, fps30, AnimationStyle.Sequential, true);
			raven.originX = 44;
			raven.originY = 477;
			this.allBackLayerEffects.add(raven);
			this.ravens.push(raven);
		}
	}

	addBackgroundRect(x: number, y: number, width: number, height: number, lifespan: number): AnimatedRectangle {
		const borderThickness: number = 2;
		const opacity: number = 0.95;
		return this.animations.addRectangle(x, y, width, height, '#f5e0b7', '#ae722c', lifespan, borderThickness, opacity);
	}

	setMultiplayerFades(animatedElement: AnimatedElement) {
		animatedElement.fadeOutTime = 1000;
		animatedElement.fadeInTime = 1000;
	}

	showMultiplayerResults(title: string, initiativeSummary: PlayerRoll[], hiddenThreshold: number = 0): any {
		let x: number = 10;
		let y: number = 10;
		const lineHeight: number = 70;
		const margins: number = 5;
		let backgroundRect: AnimatedRectangle = this.addBackgroundRect(2, 2, 50, 50 /* lineHeight * initiativeSummary.length + margins * 2 */, DiceLayer.multiplePlayerSummaryDuration);
		this.setMultiplayerFades(backgroundRect);
		var lastRollWasHigherThanThreshold: boolean = true;
		if (title) {
			let titleEffect: TextEffect = this.showMultiplayerResultTitle(title, x, y);
			titleEffect.boundingRect = backgroundRect;
			titleEffect.fontSize = 15;
			y += lineHeight;
			backgroundRect.height += lineHeight;
			x += 30;
		}

		for (var i = 0; i < initiativeSummary.length; i++) {
			let playerRoll: PlayerRoll = initiativeSummary[i];
			if (playerRoll.roll + playerRoll.modifier < hiddenThreshold && lastRollWasHigherThanThreshold) {
				if (hiddenThreshold != 0) {
					const separatorHeight: number = 20;
					let line: AnimatedLine = this.addSeparatorHorizontalLine(x, y, 350, '#ff0000', DiceLayer.multiplePlayerSummaryDuration);
					this.setMultiplayerFades(line);
					y += separatorHeight;
					backgroundRect.height += separatorHeight;
				}
				lastRollWasHigherThanThreshold = false;
			}
			let effect: TextEffect = this.showPlayerRoll(playerRoll, x, y);
			effect.boundingRect = backgroundRect;
			y += lineHeight;
		}
	}

	addSeparatorHorizontalLine(x: number, y: number, width: number, color: string, lifespan: number): AnimatedLine {
		return this.animations.addLine(x, y, width, color, lifespan, 2);
	}

	static readonly multiplePlayerSummaryDuration: number = 14000;

	showMultiplayerResultTitle(title: string, x: number, y: number): TextEffect {
		let textEffect: TextEffect = this.animations.addText(new Vector(x, y), title, DiceLayer.multiplePlayerSummaryDuration);
		textEffect.fontColor = '#000000';
		textEffect.outlineColor = '#ffffff';
		this.setMultiplayerResultText(textEffect);
		return textEffect;
	}

	setMultiplayerResultText(textEffect: TextEffect): any {
		textEffect.textAlign = 'left';
		textEffect.textBaseline = 'top';
		textEffect.scale = 2.5;
		textEffect.opacity = 0.90;
		textEffect.targetScale = 3.5;
		this.setMultiplayerFades(textEffect);
	}

	showPlayerRoll(playerRoll: PlayerRoll, x: number, y: number): TextEffect {
		if (!playerRoll.name)
			playerRoll.name = diceLayer.getPlayerName(playerRoll.id);
		var message: string = `${playerRoll.name} - ${playerRoll.roll + playerRoll.modifier}`;
		if (playerRoll.modifier > 0)
			message += ` (+${playerRoll.modifier})`;
		else if (playerRoll.modifier < 0)
			message += ` (${playerRoll.modifier})`;

		let textEffect: TextEffect = this.animations.addText(new Vector(x, y), message, DiceLayer.multiplePlayerSummaryDuration);
		textEffect.fontColor = this.getDieColor(playerRoll.id);
		textEffect.outlineColor = this.activePlayerDieFontColor;
		this.setMultiplayerResultText(textEffect);
		return textEffect;
	}

	showResult(resultMessage: string, success: boolean): any {
		if (!resultMessage)
			return;
		let textEffect: TextEffect = this.animations.addText(new Vector(960, 100), resultMessage, this.resultTextTime);
		if (success) {
			textEffect.fontColor = successFontColor;
			textEffect.outlineColor = successOutlineColor;
		}
		else {
			textEffect.fontColor = failFontColor;
			textEffect.outlineColor = failOutlineColor;
		}
		textEffect.scale = 1;
		textEffect.velocityY = 0.2;
		textEffect.opacity = 0.90;
		textEffect.targetScale = 9;
		textEffect.fadeOutTime = 1000;
		textEffect.fadeInTime = 200;
	}

	showTotalHealthDamage(totalDamage: number, success: boolean, label: string, fontColor: string, outlineColor: string): any {
		var damageTime: number = this.totalDamageTime;
		if (!success)
			damageTime = 2 * damageTime / 3;
		let textEffect: TextEffect = this.animations.addText(new Vector(960, 750), `${label}${totalDamage}`, damageTime);
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.elasticIn = true;
		textEffect.scale = 4;
		textEffect.velocityY = 0.4;
		textEffect.opacity = 0.90;
		if (success)
			textEffect.targetScale = 8;
		else
			textEffect.targetScale = 3;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}

	showDamageHealthModifier(modifier: number, success: boolean, fontColor: string, outlineColor: string): any {
		if (modifier == 0)
			return;
		let totalDamage: string;
		if (modifier < 0)
			totalDamage = modifier.toString();
		else
			totalDamage = '+' + modifier.toString();

		var damageTime: number = this.damageModifierTime;
		if (!success)
			damageTime = 2 * damageTime / 3;

		let yPos: number;
		let targetScale: number;
		let velocityY: number;
		if (success) {
			targetScale = 4;
			velocityY = 0.5;
			yPos = 880;
		}
		else {
			targetScale = 2;
			velocityY = 0.5;
			yPos = 810;
		}
		let textEffect: TextEffect = this.animations.addText(new Vector(960, yPos), `(${totalDamage})`, damageTime);
		textEffect.targetScale = targetScale;
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.elasticIn = true;
		textEffect.scale = 3;
		textEffect.velocityY = velocityY;
		textEffect.opacity = 0.90;

		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}

	showBonusRoll(totalBonusStr: string, dieColor: string = undefined, fontColor: string = undefined): any {
		let textEffect: TextEffect = this.animations.addText(new Vector(50, 900), totalBonusStr, this.bonusRollTime);
		textEffect.textAlign = 'left';
		if (dieColor === undefined)
			dieColor = DiceLayer.bonusRollDieColor;
		if (fontColor === undefined)
			fontColor = DiceLayer.bonusRollFontColor;
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = dieColor;
		textEffect.elasticIn = true;
		textEffect.scale = 3;
		textEffect.velocityY = 0.6;
		textEffect.opacity = 0.90;
		textEffect.targetScale = 9;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}

	showDieTotal(thisRollStr: string): void {
		let textEffect: TextEffect = this.animations.addText(new Vector(960, 540), thisRollStr, this.totalRollScoreTime);
		textEffect.fontColor = this.activePlayerDieColor;
		textEffect.elasticIn = true;
		textEffect.outlineColor = this.activePlayerDieFontColor;
		textEffect.scale = 10;
		textEffect.opacity = 0.90;
		textEffect.targetScale = 30;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}

	showRollModifier(rollModifier: number, luckBend: number = 0): void {
		if (rollModifier == 0 && luckBend == 0)
			return;
		var rollModStr: string = rollModifier.toString();
		if (rollModifier > 0)
			rollModStr = '+' + rollModStr;

		var rollLuckModStr: string = '';
		if (luckBend != 0) {
			rollLuckModStr = luckBend.toString();
			if (luckBend > 0)
				rollLuckModStr = '+' + rollLuckModStr;

			rollLuckModStr = ', ' + rollLuckModStr;
		}

		let textEffect: TextEffect = this.animations.addText(new Vector(960, 250), `(${rollModStr}${rollLuckModStr})`, this.rollModifierTime);
		textEffect.elasticIn = true;
		textEffect.fontColor = this.activePlayerDieColor;
		textEffect.outlineColor = this.activePlayerDieFontColor;
		textEffect.velocityY = -0.1;
		textEffect.scale = 2;
		textEffect.opacity = 0.90;
		textEffect.targetScale = 5;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 500;
	}

	addDieValueLabel(centerPos: Vector, value: string, highlight: boolean = false) {
		let textEffect: TextEffect = this.animations.addText(centerPos, value, 5000);
		if (highlight)
			textEffect.fontColor = '#ff0000';
		textEffect.scale = 6;
		textEffect.opacity = 0.75;
		textEffect.targetScale = 0.5;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}

	indicateBonusRoll(message: string): any {
		let textEffect: TextEffect = this.animations.addText(new Vector(960, 800), message, 3000);
		textEffect.scale = 6;
		textEffect.opacity = 0.75;
		textEffect.targetScale = 10;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 800;
	}

	bendingLuck(message: string, luckMultiplier: number): any {
		let textEffect: TextEffect = this.animations.addText(new Vector(960, 540), message, 3000);

		if (luckMultiplier < 0) {
			// Bad luck
			textEffect.outlineColor = DiceLayer.badLuckFontColor;
			textEffect.fontColor = DiceLayer.badLuckDieColor;
		}
		else {
			textEffect.outlineColor = DiceLayer.goodLuckFontColor;
			textEffect.fontColor = DiceLayer.goodLuckDieColor;
		}

		textEffect.scale = 6;
		textEffect.opacity = 0.75;
		textEffect.targetScale = 10;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 800;
	}


	addDieText(die: any, message: string, fontColor: string, outlineColor: string, lifeSpan: number = 1500, scaleAdjust: number = 1): any {
		let centerPos: Vector = getScreenCoordinates(die.getObject());
		if (centerPos == null)
			return;
		let textEffect: TextEffect = this.animations.addText(centerPos.add(new Vector(0, 80 * scaleAdjust)), message, lifeSpan);
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.scale = 3 * scaleAdjust;
		textEffect.waitToScale = 400;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 600;
		textEffect.targetScale = 1 * scaleAdjust;
	}

	addDieTextAfter(die: any, message: string, fontColor: string, outlineColor: string, timeout: number = 0, lifeSpan: number = 1500, scaleAdjust: number = 1) {
		if (timeout > 0)
			setTimeout(function () {
				this.addDieText(die, message, fontColor, outlineColor, lifeSpan, scaleAdjust);
			}.bind(this), timeout);
		else
			this.addDieText(die, message, fontColor, outlineColor, lifeSpan, scaleAdjust);
	}

	addDisadvantageText(die: any, timeout: number = 0, isLuckyFeat: boolean = false): any {
		var message: string;
		if (isLuckyFeat)
			message = 'Lucky Feat';
		else
			message = 'Disadvantage';
		this.addDieTextAfter(die, message, this.activePlayerDieColor, this.activePlayerDieColor, timeout);
	}

	addAdvantageText(die: any, timeout: number = 0, isLuckyFeat: boolean = false): any {
		var message: string;
		if (isLuckyFeat)
			message = 'Lucky Feat';
		else
			message = 'Advantage';
		this.addDieTextAfter(die, message, this.activePlayerDieColor, this.activePlayerDieColor, timeout);
	}


	mouseDownInCanvas(e) {
		//if (effectOverride != undefined) {
		//  var enumIndex: number = <number>effectOverride;
		//  let totalElements: number = Object.keys(DieEffect).length / 2;
		//  enumIndex++;
		//  if (enumIndex >= totalElements)
		//    enumIndex = 0;
		//  effectOverride = <DieEffect>enumIndex;
		//  console.log('New effect: ' + DieEffect[effectOverride]);
		//}
	}

	getContext() {
		this.diceFrontCanvas = <HTMLCanvasElement>document.getElementById("diceFrontCanvas");
		this.diceFrontCanvas.onmousedown = this.mouseDownInCanvas;
		this.diceBackCanvas = <HTMLCanvasElement>document.getElementById("diceBackCanvas");
		this.diceFrontContext = this.diceFrontCanvas.getContext("2d");
		this.diceBackContext = this.diceBackCanvas.getContext("2d");
	}

	renderCanvas() {
		if (!this.diceFrontContext || !this.diceBackContext)
			this.getContext();

		this.diceFrontContext.clearRect(0, 0, 1920, 1080);
		this.diceBackContext.clearRect(0, 0, 1920, 1080);
		if (!this.playerDataSet) {
			this.diceFrontContext.font = '38px Arial';
			this.diceFrontContext.fillStyle = '#ff0000';
			this.diceFrontContext.textAlign = 'left';
			this.diceFrontContext.textBaseline = 'top';
			this.diceFrontContext.fillText("Waiting for player data to be initialized.", 10, 10);
		}
		var now: number = performance.now();
		this.allFrontLayerEffects.updatePositions(now);
		this.allBackLayerEffects.updatePositions(now);
		this.allFrontLayerEffects.draw(this.diceFrontContext, now);
		this.allBackLayerEffects.draw(this.diceBackContext, now);
		this.animations.removeExpiredAnimations(now);
		this.animations.updatePositions(now);
		this.animations.render(this.diceFrontContext, now);
	}

	addFireball(x: number, y: number) {
		this.diceFireball.add(x, y, 0).rotation = 90;
	}

	addMagicRing(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
		let magicRing = this.magicRing.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		magicRing.rotation = Math.random() * 360;
		return magicRing;
	}

	addLuckyRing(x: number, y: number): SpriteProxy {
		let luckyRing = this.cloverRing.add(x, y, Math.floor(Math.random() * 120));
		return luckyRing;
	}

	addBadLuckRing(x: number, y: number): SpriteProxy {
		let badLuckRing = this.badLuckRing.add(x, y, Math.floor(Math.random() * 120));
		return badLuckRing;
	}

	addFreezeBubble(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
		let freeze: SpriteProxy = this.freeze.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		freeze.rotation = Math.random() * 360;
		freeze.fadeInTime = 500;
		return freeze;
	}

	addFeezePop(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
		let freezePop = this.freezePop.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		freezePop.rotation = Math.random() * 360;
		return freezePop;
	}

	addHalo(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
		let halo = this.halos.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		halo.rotation = Math.random() * 360;
		halo.opacity = 0.9;
		halo.fadeInTime = 500;
		halo.fadeOutTime = 500;
		halo.fadeOnDestroy = true;
		halo.autoRotationDegeesPerSecond = 10;
		return halo;
	}

	addHaloSpin(x: number, y: number, hueShift: number = 0, angle: number): SpriteProxy {
		let haloSpin = this.haloSpins.addShifted(x, y, 0, hueShift);
		haloSpin.rotation = angle;
		haloSpin.fadeInTime = 500;
		haloSpin.fadeOutTime = 500;
		haloSpin.fadeOnDestroy = true;
		return haloSpin;
	}


	addRaven(x: number, y: number, angle: number) {
		let index: number = Math.floor(Math.random() * this.ravens.length);
		let raven = this.ravens[index].addShifted(x, y, 0, this.activePlayerHueShift + Random.plusMinus(35));
		raven.rotation = angle + 180 + Random.plusMinus(45);
		//diceSounds.playRandom('BirdFlap', 4);
		return raven;
	}

	blowColoredSmoke(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
		this.diceBlowColoredSmoke.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
	}

	//addD20Fire(x: number, y: number) {
	//  this.d20Fire.add(x, y).rotation = Math.random() * 360;
	//}

	addPuff(x: number, y: number, angle: number) {
		let hueShift = this.activePlayerHueShift + Random.plusMinus(10);
		var angleShift: number = 0;
		if (Math.random() > 0.5)
			angleShift = 180;
		this.puff.addShifted(x, y, 0, hueShift).rotation = angle + angleShift;
	}

	addSparkTrail(x: number, y: number, angle: number) {
		this.sparkTrail.addShifted(x, y, 0, this.activePlayerHueShift + Random.plusMinus(35)).rotation = angle + 180 + Random.plusMinus(15);
	}

	clearResidualEffects(): any {
		this.magicRing.sprites = [];
		this.cloverRing.sprites = [];
		this.badLuckRing.sprites = [];
		this.halos.sprites = [];
		this.haloSpins.sprites = [];
		//this.stars.sprites = [];
		//this.d20Fire.sprites = [];
		this.clearTextEffects();
	}

	clearTextEffects() {
		this.animations.clear();
	}


	addSteampunkTunnel(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
		// no rotation on SteampunkTunnel - shadows expect light source from above.
		this.dieSteampunkTunnel.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
	}

	addSneakAttack(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
		let rotation: number = Math.random() * 360;
		this.sneakAttackBottom.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = rotation;
		this.sneakAttackTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = rotation;
	}

	addDiceBomb(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
		this.diceBombBase.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
		this.diceBombTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
	}

	addPortal(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
		this.dicePortal.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		this.dicePortalTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
	}

	testDiceGrab(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
		var handRotation: number = 90 - Math.random() * 180;
		this.handGrabsDiceBottom.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = handRotation;
		this.handGrabsDiceTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = handRotation;
	}

	getDiceRollData(diceRollData: string): DiceRollData {
		let dto: any = JSON.parse(diceRollData);
		let diceRoll: DiceRollData = new DiceRollData();
		diceRoll.type = dto.Type;
		diceRoll.kind = dto.Kind;
		diceRoll.damageDice = dto.DamageDice;
		diceRoll.modifier = dto.Modifier;
		diceRoll.hiddenThreshold = dto.HiddenThreshold;
		diceRoll.isMagic = dto.IsMagic;
		diceRoll.isSneakAttack = dto.IsSneakAttack;
		diceRoll.isPaladinSmiteAttack = dto.IsPaladinSmiteAttack;
		diceRoll.isWildAnimalAttack = dto.IsWildAnimalAttack;
		diceRoll.throwPower = dto.ThrowPower;
		diceRoll.onFirstContactSound = dto.OnFirstContactSound;
		diceRoll.onFirstContactEffect = dto.OnFirstContactEffect;
		diceRoll.onRollSound = dto.OnRollSound;
		diceRoll.minCrit = dto.MinCrit;
		diceRoll.inspiration = dto.Inspiration;
		diceRoll.successMessage = dto.SuccessMessage;
		diceRoll.failMessage = dto.FailMessage;
		diceRoll.critFailMessage = dto.CritFailMessage;
		diceRoll.critSuccessMessage = dto.CritSuccessMessage;
		diceRoll.numHalos = dto.NumHalos;
		diceRoll.rollScope = dto.RollScope;
		diceRoll.individualFilter = dto.IndividualFilter;
		diceRoll.skillCheck = dto.SkillCheck;
		diceRoll.savingThrow = dto.SavingThrow;

		for (var i = 0; i < dto.TrailingEffects.length; i++) {
			diceRoll.trailingEffects.push(new TrailingEffect(dto.TrailingEffects[i]));
		}

		if (diceRoll.throwPower < 0.2)
			diceRoll.throwPower = 0.2;
		if (diceRoll.throwPower > 2.0)
			diceRoll.throwPower = 2.0;
		return diceRoll;
	}

	smallSpark(x: number, y: number, angle: number = -1): SpriteProxy {
		if (angle == -1)
			angle = Math.random() * 360;
		let spark = this.diceSparks.addShifted(x, y, Math.round(Math.random() * this.diceSparks.sprites.length), Math.random() * 360);
		spark.expirationDate = performance.now() + 180;
		spark.fadeOutTime = 0;
		spark.opacity = 0.8;
		spark.rotation = angle;
		return spark;
	}

	addSpiral(x: number, y: number, angle: number): SpriteProxy {
		let spiral = this.spirals.addShifted(x, y, Math.round(Math.random() * this.spirals.sprites.length), diceLayer.activePlayerHueShift + Random.plusMinus(20));
		spiral.rotation = Random.between(0, 360);
		return spiral;
	}

	addPawPrint(x: number, y: number, angle: number): SpriteProxy {
		let pawPrint = this.pawPrints.addShifted(x, y, Math.round(Math.random() * this.pawPrints.sprites.length), diceLayer.activePlayerHueShift);
		pawPrint.rotation = angle;
		pawPrint.expirationDate = performance.now() + 4000;
		pawPrint.fadeOutTime = 2000;
		pawPrint.fadeInTime = 500;
		pawPrint.opacity = 0.9;
		return pawPrint;
	}

	//addStar(x: number, y: number): SpriteProxy {
	//  let star = this.stars.addShifted(x, y, Math.round(Math.random() * this.stars.sprites.length), diceLayer.activePlayerHueShift);
	//  star.autoRotationDegeesPerSecond = 15 + Math.round(Math.random() * 20);
	//  if (Math.random() < 0.5)
	//    star.autoRotationDegeesPerSecond = -star.autoRotationDegeesPerSecond;
	//  star.fadeInTime = 1000;
	//  star.fadeOutTime = 500;
	//  star.opacity = 0.75;
	//  return star;
	//}

	clearDice(): void {
		removeRemainingDice();
	}

	rollDice(diceRollDto: string): void {
		let diceRollData: DiceRollData = this.getDiceRollData(diceRollDto);
		console.log(diceRollData);
		pleaseRollDice(diceRollData);

		//  { "Type": 1, "Kind": 0, "DamageDice": "2d8+6", "Modifier": 1.0, "HiddenThreshold": 12.0, "IsMagic": true }
	}

	getDieColor(playerID: number): string {
		switch (playerID) {
			case -1:
				return this.activePlayerDieColor;
			case 0:
				return '#710138';
			case 1:
				return '#00641d';
			case 2:
				return '#401260';
			case 3:
				return '#04315a';
		}
		return '#000000';
	}

	getDieFontColor(playerID: number): string {
		return this.activePlayerDieFontColor;
	}

	getPlayerName(playerID: number): string {
		switch (playerID) {
			case 0:
				return 'Willy';
			case 1:
				return 'Shemo';
			case 2:
				return 'Merkin';
			case 3:
				return 'Ava';
		}
		return '';
	}

	getHueShift(playerID: number): number {
		switch (playerID) {
			case 0:
				return 0;
			case 1:
				return 138;
			case 2:
				return 260;
			case 3:
				return 210;
		}
		return 0;
	}

	playerChanged(playerID: number): void {
		this.playerID = playerID;
		this.activePlayerDieFontColor = '#ffffff';

		this.activePlayerDieColor = this.getDieColor(playerID);
		this.activePlayerHueShift = this.getHueShift(playerID);
	}
}

class DiceRollData {
	type: DiceRollType;
	kind: DiceRollKind;
	damageDice: string;
	modifier: number;
	minCrit: number;
	hiddenThreshold: number;
	isMagic: boolean;
	isSneakAttack: boolean;
	isPaladinSmiteAttack: boolean;
	isWildAnimalAttack: boolean;
	throwPower: number;
	itsAD20Roll: boolean;
	trailingEffects: Array<TrailingEffect> = new Array<TrailingEffect>();
	bonusRolls: Array<BonusRoll> = null;
	onFirstContactSound: string;
	critSuccessMessage: string;
	successMessage: string;
	critFailMessage: string;
	failMessage: string;
	onFirstContactEffect: SpriteType;
	onRollSound: number;
	numHalos: number;
	individualFilter: number;
	skillCheck: Skills;
	savingThrow: Ability;
	rollScope: RollScope;
	wildMagic: WildMagic;
	inspiration: string;
	playBonusSoundAfter: number;
	bentLuckMultiplier: number;
	bentLuckRollData: DiceRollData = null;
	startedBonusDiceRoll: boolean;
	showedVantageMessage: boolean;
	timeLastRolledMs: number;
	appliedVantage: boolean = false;
	maxInspirationDiceAllowed: number = 1;
	numInspirationDiceCreated: number = 0;
	hasMultiPlayerDice: boolean = false;
	multiplayerSummary: Array<PlayerRoll> = null;
	constructor() {

	}

	getFirstBonusRollDescription(): string {
		if (!this.bonusRolls || this.bonusRolls.length == 0)
			return null;
		return this.bonusRolls[0].description;
	}

	addBonusRoll(diceStr: string, description: string, playerID: number = -1, dieBackColor: string = DiceLayer.bonusRollDieColor, dieTextColor: string = DiceLayer.bonusRollFontColor): BonusRoll {
		if (!this.bonusRolls)
			this.bonusRolls = [];
		let bonusRoll = new BonusRoll(diceStr, description, playerID, dieBackColor, dieTextColor);
		this.bonusRolls.push(bonusRoll);

		return bonusRoll;
	}
	addBonusDamageRoll(diceStr: string, description: string, playerID: number = -1, dieBackColor: string = DiceLayer.damageDieBackgroundColor, dieTextColor: string = DiceLayer.damageDieFontColor): BonusRoll {
		if (!this.bonusRolls)
			this.bonusRolls = [];
		let bonusRoll = new BonusRoll(diceStr, description, playerID, dieBackColor, dieTextColor);
		this.bonusRolls.push(bonusRoll);

		return bonusRoll;
	}
}

class BonusRoll {
	isMagic: boolean = false;
	constructor(public diceStr: string, public description: string, public playerID: number, public dieBackColor: string = DiceLayer.bonusRollDieColor, public dieTextColor: string = DiceLayer.bonusRollFontColor) {
		
	}
}

class AnimatedLine extends ScalableAnimation {
	constructor(x: number, y: number, public width: number, public lineColor: string, public lifeSpanMs: number, public lineThickness: number = 1) {
		super(x, y, lifeSpanMs);
	}

	render(context: CanvasRenderingContext2D, now: number) {
		context.globalAlpha = this.getAlpha(now) * this.opacity;
		context.strokeStyle = this.lineColor;
		context.lineWidth = this.lineThickness;
		context.lineJoin = "round";
		context.beginPath();
		let scale: number = this.getScale(now);
		context.moveTo(this.x, this.y);
		context.lineTo(this.x + this.width * scale, this.y);
		context.stroke();
		context.globalAlpha = 1;
	}

	getVerticalThrust(now: number): number {
		return 0;
	}
}

class AnimatedRectangle extends ScalableAnimation {
	margin: number = 5;
	constructor(public x: number, public y: number, public width: number, public height: number, public fillColor: string, public outlineColor: string, public lifeSpanMs: number, public lineThickness: number = 1) {
		super(x, y, lifeSpanMs);
	}

	render(context: CanvasRenderingContext2D, now: number) {
		context.globalAlpha = this.getAlpha(now) * this.opacity;
		context.strokeStyle = this.outlineColor;
		context.fillStyle = this.fillColor;
		context.lineWidth = this.lineThickness;
		context.lineJoin = "round";
		let scale: number = this.getScale(now);
		context.fillRect(this.x, this.y, this.width * scale, this.height * scale);
		context.strokeRect(this.x, this.y, this.width * scale, this.height * scale);
		context.globalAlpha = 1;
	}

	getVerticalThrust(now: number): number {
		return 0;
	}

	fitToRect(left: number, top: number, width: number, height: number): any {
		if (left - this.margin < this.x)
			this.x = left - this.margin;

		if (top - this.margin < this.y)
			this.y = top - this.margin;

		if (this.x + this.width < left + width + this.margin * 2)
			this.width = left + width + this.margin * 2 - this.x;

		if (this.y + this.height < top + height + this.margin * 2)
			this.height = top + height + this.margin * 2 - this.y;
	}

}
