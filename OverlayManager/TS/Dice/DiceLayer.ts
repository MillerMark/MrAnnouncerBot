enum DiceGroup {
	Players,
	Viewers
}

class DieSpriteData {
	public group: DiceGroup;
	public bubbleId: string = null;
	constructor() {
	}
}

enum RollScope {
	ActivePlayer,
	Viewer,
	Individuals,
	ActiveInGameCreature,
	TargetedInGameCreatures
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
	Initiative,
	WildMagicD20Check,
	InspirationOnly,
	AddOnDice,
	NonCombatInitiative,
	HPCapacity,
	CastSimpleSpell,
	DamagePlusSavingThrow,
	OnlyTargetsSavingThrow,
	ViewerRoll
}

//enum SpriteType {
//	PawPrint,
//	Raven,
//	Spiral,
//	Smoke,
//	SmokeExplosion,
//	SparkTrail,
//	SmallSparks,
//	Fangs
//}

class TrailingEffect {
	constructor(dto) {
		this.MinForwardDistanceBetweenPrints = dto.MinForwardDistanceBetweenPrints;
		this.LeftRightDistanceBetweenPrints = dto.LeftRightDistanceBetweenPrints;
		this.Index = dto.Index;
		this.OnPrintPlaySound = dto.OnPrintPlaySound;
		this.NumRandomSounds = dto.NumRandomSounds;

		this.Name = dto.Name;
		this.EffectType = dto.EffectType;
		this.OnThrowSound = dto.OnThrowSound;
		this.OnFirstContactSound = dto.OnFirstContactSound;
		this.OnFirstContactEffect = dto.OnFirstContactEffect;
		this.NumHalos = dto.NumHalos;
		this.StartIndex = dto.StartIndex;
		this.FadeIn = dto.FadeIn;
		this.Lifespan = dto.Lifespan;
		this.FadeOut = dto.FadeOut;
		this.Opacity = dto.Opacity;
		this.Scale = dto.Scale;
		this.ScaleWithVelocity = dto.ScaleWithVelocity;
		this.MinScale = dto.MinScale;
		this.MaxScale = dto.MaxScale;
		this.ScaleVariance = dto.ScaleVariance;
		this.HueShift = dto.HueShift;
		this.HueShiftRandom = dto.HueShiftRandom;
		this.Saturation = dto.Saturation;
		this.Brightness = dto.Brightness;
		this.RotationOffset = dto.RotationOffset;
		this.RotationOffsetRandom = dto.RotationOffsetRandom;
		this.FlipHalfTime = dto.FlipHalfTime;
	}


	intervalBetweenSounds: Map<string, number> = new Map<string, number>();
	NumRandomSounds: number;
	MinForwardDistanceBetweenPrints: number;
	LeftRightDistanceBetweenPrints: number;
	Index: number;
	OnPrintPlaySound: string;

	Name: string;
	EffectType: string;
	OnThrowSound: string;
	OnFirstContactSound: string;
	OnFirstContactEffect: string;
	NumHalos: number;
	StartIndex: number;
	FadeIn: number;
	Lifespan: number;
	FadeOut: number;
	Opacity: number;
	Scale: number;
	ScaleWithVelocity: boolean;
	MinScale: number;
	MaxScale = 999999;
	ScaleVariance: number;
	HueShift: string;
	HueShiftRandom: number;
	Saturation: number;
	Brightness: number;
	RotationOffset: number;
	RotationOffsetRandom: number;
	FlipHalfTime: boolean;
}

//const matchNormalDieColorsToSpecial = true;
//const damageDieBackgroundColor: string = '#e49836';
//const damageDieFontColor: string = '#000000';

const successFontColor = '#ffffff';
const successOutlineColor = '#000000';
const failFontColor = '#000000';
const failOutlineColor = '#ffffff';

class PlayerRollOptions {
	PlayerID: number;
	Inspiration: string;
	VantageKind: VantageKind;

	constructor(dto: any) {
		this.PlayerID = dto.PlayerID;
		this.Inspiration = dto.Inspiration;
		this.VantageKind = dto.VantageKind;
	}
}

class DiceLayer {
	static numFiltersOnDieCleanup: number;
	static numFiltersOnRoll: number;
	static maxFiltersOnRoll = 22;
	static maxFiltersOnDieCleanup = 22;
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

	static matchOozeToDieColor = true;
	animations: Animations = new Animations();
	diceFrontCanvas: HTMLCanvasElement;
	diceBackCanvas: HTMLCanvasElement;
	diceFrontContext: CanvasRenderingContext2D;
	diceBackContext: CanvasRenderingContext2D;
	diceFireball: Sprites;
	diceShockwaveRed: Sprites;
	diceBurst: Sprites;
	diceSmokeRed: Sprites;
	diceSmokeyPortalBack: Sprites;
	diceSmokeyPortalTop: Sprites;
	cloverRing: Sprites;
	badLuckRing: Sprites;
	sparkTrail: Sprites;
	haloSpinRed: Sprites;
	haloSpinYellow: Sprites;
	haloSpinGreen: Sprites;
	haloSpinTeal: Sprites;
	haloSpinBlue: Sprites;
	haloSpinPurple: Sprites;
	haloSpinLavender: Sprites;
	haloSpinMaroon: Sprites;
	haloSpinFuschia: Sprites;
	damageFire: Sprites;
	damageFireB: Sprites;
	damageCold: Sprites;
	damageNecroticHead: Sprites;
	damageNecroticHeadB: Sprites;
	damageNecroticFog: Sprites;
	damageAcid: Sprites;
	damageAcidB: Sprites;
	topDieCloudRed: Sprites;
	topDieCloudWhite: Sprites;
	topDieCloudBlack: Sprites;
	topDieCloudBlue: Sprites;
	topDieCloudPurple: Sprites;
	topDieCloudGreen: Sprites;
	damageRadiant: Sprites;
	damageForce: Sprites;
	damageForceB: Sprites;
	damageBludgeoningMace: Sprites;
	damageBludgeoningFist: Sprites;
	damageBludgeoningWeapon: Sprites;
	damageSpinningCloudTrail: Sprites;
	damagePoison: Sprites;
	damagePiercingDagger: Sprites;
	damagePiercingTooth: Sprites;
	damagePiercingThickSword: Sprites;
	damagePiercingTrident: Sprites;
	damageSlashingSword: Sprites;
	damageSlashingLance: Sprites;
	health: Sprites;
	healthB: Sprites;
	damageSlashingAx: Sprites;
	damageThunder: Sprites;
	superiorityFire: Sprites;
	baneBack: Sprites;
	blessBack: Sprites;
	baneFront: Sprites;
	blessFront: Sprites;
	superiorityDragonHead: Sprites;
	superiorityDragonHeadB: Sprites;
	superiorityDragonHeadC: Sprites;
	superiorityDragonHeadD: Sprites;
	damagePsychic: Sprites;
	damagePsychicB: Sprites;
	damageLightningA: Sprites;
	damageLightningB: Sprites;
	damageLightningC: Sprites;
	damageLightningCloud: Sprites;
	inspirationParticles: Sprites;
	inspirationSmoke: Sprites;
	ripples: Sprites;
	//d20Fire: Sprites;
	smoke: Sprites;
	freeze: Sprites;
	freezePop: Sprites;
	diceSparks: Sprites;
	smokeExplosionTop: Sprites;
	smokeExplosionBottom: Sprites;
	pawPrints: Sprites;
	//stars: Sprites;
	dicePortal: Sprites;
	handGrabsDiceTop: Sprites;
	handGrabsDiceBottom: Sprites;
	dicePortalTop: Sprites;
	magicRingRed: Sprites;
	magicRingGold: Sprites;
	magicRingGreen: Sprites;
	magicRingBlue: Sprites;
	magicRingPurple: Sprites;
	magicRingMaroon: Sprites;
	spirals: Sprites;
	fangs: Sprites;
	//halos: Sprites;
	ravens: Sprites[];
	fireTrails: Sprites[];
	groundBurstTrails: Sprites[];
	frostTrails: Sprites[];
	diceBlowColoredSmoke: Sprites;
	diceBombBase: Sprites;
	diceBombTop: Sprites;
	dieSteampunkTunnel: Sprites;
	allFrontLayerEffects: SpriteCollection;
	allBackLayerEffects: SpriteCollection;
	activePlayerDieColor = '#fcd5a6';
	activePlayerDieFontColor = '#ffffff';
	activePlayerHueShift = 0;
	playerID: number;
	playerDataSet = false;

	constructor() {
		this.loadSprites();
	}

	initSprite(sprite: SpriteProxy, die: IDie) {
		const data: DieSpriteData = new DieSpriteData();
		data.group = die.group;
		sprite.data = data;
	}

	loadSprites() {
		Part.loadSprites = true;
		Folders.assets = 'GameDev/Assets/DragonH/';

		globalBypassFrameSkip = true;

		this.allFrontLayerEffects = new SpriteCollection();
		this.allBackLayerEffects = new SpriteCollection();

		//! Items added later are drawn on top of earlier items.


		this.pawPrints = new Sprites("/Dice/TigerPaw/TigerPaw", 76, fps30, AnimationStyle.Loop, true);
		this.pawPrints.name = 'PawPrint';
		this.pawPrints.originX = 50;
		this.pawPrints.originY = 66;
		this.allBackLayerEffects.add(this.pawPrints);

		this.diceFireball = new Sprites("/Dice/Roll20Fireball/DiceFireball", 71, fps30, AnimationStyle.Sequential, true);
		this.diceFireball.originX = 104;
		this.diceFireball.originY = 155;
		this.allFrontLayerEffects.add(this.diceFireball);

		this.loadDieBurstShockwaves();
		this.loadDieBursts();
		this.loadDieBurstSmokes();

		this.diceSmokeyPortalBack = this.loadSmokeyPortal('Back', 317, 288);
		this.allBackLayerEffects.add(this.diceSmokeyPortalBack);

		this.diceSmokeyPortalTop = this.loadSmokeyPortal('Front', 183, 173);
		this.allFrontLayerEffects.add(this.diceSmokeyPortalTop);

		this.sparkTrail = new Sprites("/Dice/SparkTrail/SparkTrail", 46, fps40, AnimationStyle.Sequential, true);
		this.sparkTrail.name = 'SparkTrail';
		this.sparkTrail.originX = 322;
		this.sparkTrail.originY = 152;
		this.allFrontLayerEffects.add(this.sparkTrail);



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
		this.diceSparks.name = 'Spark';
		this.diceSparks.originX = 170;
		this.diceSparks.originY = 158;
		this.allBackLayerEffects.add(this.diceSparks);

		//this.magicRing = new Sprites("/Dice/MagicRing/MagicRing", 180, fps40, AnimationStyle.Loop, true);
		//this.magicRing.name = 'MagicRing';
		//this.magicRing.returnFrameIndex = 60;
		//this.magicRing.originX = 140;
		//this.magicRing.originY = 112;
		//this.allFrontLayerEffects.add(this.magicRing);

		this.magicRingRed = this.createMagicRing('Red');
		this.magicRingGold = this.createMagicRing('Gold');
		this.magicRingGreen = this.createMagicRing('Green');
		this.magicRingBlue = this.createMagicRing('Blue');
		this.magicRingPurple = this.createMagicRing('Purple');
		this.magicRingMaroon = this.createMagicRing('Maroon');

		//this.halos = new Sprites("/Dice/Halo/Halo", 90, fps30, AnimationStyle.Loop, true);
		//this.halos.name = 'Halo';
		//this.halos.originX = 190;
		//this.halos.originY = 190;
		//this.allFrontLayerEffects.add(this.halos);

		//this.haloSpins = new Sprites("/Dice/PaladinSpin/PaladinSpin", 85, fps30, AnimationStyle.Loop, true);
		//this.haloSpins.name = 'HaloSpin';
		//this.haloSpins.originX = 200;
		//this.haloSpins.originY = 200;
		//this.allFrontLayerEffects.add(this.haloSpins);
		this.haloSpinRed = this.addHalo('Red');
		this.haloSpinYellow = this.addHalo('Yellow');
		this.haloSpinGreen = this.addHalo('Green');
		this.haloSpinTeal = this.addHalo('Teal');
		this.haloSpinBlue = this.addHalo('Blue');
		this.haloSpinPurple = this.addHalo('Purple');
		this.haloSpinLavender = this.addHalo('Lavendar');
		this.haloSpinMaroon = this.addHalo('Maroon');
		this.haloSpinFuschia = this.addHalo('Fuschia');

		this.damageFire = new Sprites("/Dice/Damage/Fire/Fire", 89, fps30, AnimationStyle.Loop, true);
		this.damageFire.originX = 133;
		this.damageFire.originY = 133;
		this.allBackLayerEffects.add(this.damageFire);

		this.damageFireB = new Sprites("/Dice/Damage/Fire/FireB", 89, fps30, AnimationStyle.Loop, true);
		this.damageFireB.originX = 133;
		this.damageFireB.originY = 133;
		this.allBackLayerEffects.add(this.damageFireB);

		this.damageCold = new Sprites("/Dice/Damage/Cold/Cold", 58, fps30, AnimationStyle.Loop, true);
		this.damageCold.originX = 123;
		this.damageCold.originY = 123;
		this.allBackLayerEffects.add(this.damageCold);

		this.damageNecroticHead = new Sprites("/Dice/Damage/Necrotic/NecroticHead", 195, fps30, AnimationStyle.Loop, true);
		this.damageNecroticHead.originX = 143;
		this.damageNecroticHead.originY = 98;
		this.allBackLayerEffects.add(this.damageNecroticHead);

		this.damageNecroticHeadB = new Sprites("/Dice/Damage/Necrotic/NecroticHeadB", 195, fps30, AnimationStyle.Loop, true);
		this.damageNecroticHeadB.originX = 143;
		this.damageNecroticHeadB.originY = 98;
		this.allBackLayerEffects.add(this.damageNecroticHeadB);

		this.damageNecroticFog = new Sprites("/Dice/Damage/Necrotic/NecroticFog", 103, fps20, AnimationStyle.Loop, true);
		this.damageNecroticFog.originX = 86;
		this.damageNecroticFog.originY = 86;
		this.allBackLayerEffects.add(this.damageNecroticFog);

		this.damageAcid = new Sprites("/Dice/Damage/Acid/Acid", 93, fps30, AnimationStyle.Loop, true);
		this.damageAcid.returnFrameIndex = 19;
		this.damageAcid.originX = 93;
		this.damageAcid.originY = 103;
		this.allBackLayerEffects.add(this.damageAcid);

		this.damageAcidB = new Sprites("/Dice/Damage/Acid/AcidB", 93, fps30, AnimationStyle.Loop, true);
		this.damageAcidB.returnFrameIndex = 19;
		this.damageAcidB.originX = 93;
		this.damageAcidB.originY = 103;
		this.allBackLayerEffects.add(this.damageAcidB);

		this.topDieCloudRed = new Sprites("/Dice/Damage/TopCloud/TopCloud_Red", 45, fps30, AnimationStyle.Loop, true);
		this.topDieCloudRed.name = 'TopCloud';
		this.topDieCloudRed.originX = 61;
		this.topDieCloudRed.originY = 90;
		this.allFrontLayerEffects.add(this.topDieCloudRed);

		this.topDieCloudWhite = new Sprites("/Dice/Damage/TopCloud/TopCloud_White", 45, fps30, AnimationStyle.Loop, true);
		this.topDieCloudWhite.name = 'TopCloud';
		this.topDieCloudWhite.originX = 61;
		this.topDieCloudWhite.originY = 90;
		this.allFrontLayerEffects.add(this.topDieCloudWhite);


		this.topDieCloudBlack = new Sprites("/Dice/Damage/TopCloud/TopCloud_Black", 45, fps30, AnimationStyle.Loop, true);
		this.topDieCloudBlack.name = 'TopCloud';
		this.topDieCloudBlack.originX = 61;
		this.topDieCloudBlack.originY = 90;
		this.allFrontLayerEffects.add(this.topDieCloudBlack);

		this.topDieCloudBlue = new Sprites("/Dice/Damage/TopCloud/TopCloud_Blue", 45, fps30, AnimationStyle.Loop, true);
		this.topDieCloudBlue.name = 'TopCloud';
		this.topDieCloudBlue.originX = 61;
		this.topDieCloudBlue.originY = 90;
		this.allFrontLayerEffects.add(this.topDieCloudBlue);

		this.topDieCloudPurple = new Sprites("/Dice/Damage/TopCloud/TopCloud_Purple", 45, fps30, AnimationStyle.Loop, true);
		this.topDieCloudPurple.name = 'TopCloud';
		this.topDieCloudPurple.originX = 61;
		this.topDieCloudPurple.originY = 90;
		this.allFrontLayerEffects.add(this.topDieCloudPurple);

		this.topDieCloudGreen = new Sprites("/Dice/Damage/TopCloud/TopCloud_Green", 45, fps30, AnimationStyle.Loop, true);
		this.topDieCloudGreen.name = 'TopCloud';
		this.topDieCloudGreen.originX = 61;
		this.topDieCloudGreen.originY = 90;
		this.allFrontLayerEffects.add(this.topDieCloudGreen);

		this.damageRadiant = new Sprites("/Dice/Damage/Radiant/Radiant", 38, fps30, AnimationStyle.Loop, true);
		this.damageRadiant.originX = 100;
		this.damageRadiant.originY = 100;
		this.allBackLayerEffects.add(this.damageRadiant);

		this.damageForce = new Sprites("/Dice/Damage/Force/Force", 90, fps30, AnimationStyle.Loop, true);
		this.damageForce.originX = 176;
		this.damageForce.originY = 64;
		this.allBackLayerEffects.add(this.damageForce);

		this.damageForceB = new Sprites("/Dice/Damage/Force/ForceB", 90, fps30, AnimationStyle.Loop, true);
		this.damageForceB.originX = 176;
		this.damageForceB.originY = 64;
		this.allBackLayerEffects.add(this.damageForceB);

		this.damageSpinningCloudTrail = new Sprites("/Dice/Damage/Bludgeoning/CloudTrail", 96, fps30, AnimationStyle.Loop, true);
		this.damageSpinningCloudTrail.name = 'CloudTrail';
		this.damageSpinningCloudTrail.originX = 155;
		this.damageSpinningCloudTrail.originY = 183;
		this.allBackLayerEffects.add(this.damageSpinningCloudTrail);

		this.damageBludgeoningMace = new Sprites("/Dice/Damage/Bludgeoning/mace", 84, fps30, AnimationStyle.Loop, true);
		this.damageBludgeoningMace.originX = 0;
		this.damageBludgeoningMace.originY = 52;
		this.allBackLayerEffects.add(this.damageBludgeoningMace);

		this.damageBludgeoningFist = new Sprites("/Dice/Damage/Bludgeoning/Punch", 43, fps30, AnimationStyle.Loop, true);
		this.damageBludgeoningFist.originX = 64;
		this.damageBludgeoningFist.originY = 246;
		this.allBackLayerEffects.add(this.damageBludgeoningFist);

		this.damageBludgeoningWeapon = new Sprites("/Dice/Damage/Bludgeoning/weapon", 2, fps30, AnimationStyle.Static, true);
		this.damageBludgeoningWeapon.originX = 0;
		this.damageBludgeoningWeapon.originY = 52;
		this.allBackLayerEffects.add(this.damageBludgeoningWeapon);

		this.damagePoison = new Sprites("/Dice/Damage/Poison/Poison", 103, fps30, AnimationStyle.Loop, true);
		this.damagePoison.name = 'Poison';
		this.damagePoison.originX = 111;
		this.damagePoison.originY = 115;
		this.allBackLayerEffects.add(this.damagePoison);

		this.damagePiercingDagger = new Sprites("/Dice/Damage/Piercing/PiercingDagger", 61, fps30, AnimationStyle.Loop, true);
		this.damagePiercingDagger.originX = 0;
		this.damagePiercingDagger.originY = 27;
		this.allBackLayerEffects.add(this.damagePiercingDagger);

		this.damagePiercingTooth = new Sprites("/Dice/Damage/Piercing/PiercingTooth", 61, fps30, AnimationStyle.Loop, true);
		this.damagePiercingTooth.originX = 0;
		this.damagePiercingTooth.originY = 27;
		this.allBackLayerEffects.add(this.damagePiercingTooth);

		this.damagePiercingThickSword = new Sprites("/Dice/Damage/Piercing/PiercingThickSword", 61, fps30, AnimationStyle.Loop, true);
		this.damagePiercingThickSword.originX = 0;
		this.damagePiercingThickSword.originY = 27;
		this.allBackLayerEffects.add(this.damagePiercingThickSword);

		this.damagePiercingTrident = new Sprites("/Dice/Damage/Piercing/PiercingTrident", 61, fps30, AnimationStyle.Loop, true);
		this.damagePiercingTrident.originX = 0;
		this.damagePiercingTrident.originY = 27;
		this.allBackLayerEffects.add(this.damagePiercingTrident);

		this.damageSlashingSword = new Sprites("/Dice/Damage/Slashing/SlashingSword", 68, fps30, AnimationStyle.Loop, true);
		this.damageSlashingSword.originX = 194;
		this.damageSlashingSword.originY = 31;
		this.allBackLayerEffects.add(this.damageSlashingSword);

		this.damageSlashingLance = new Sprites("/Dice/Damage/Slashing/SlashingLance", 68, fps30, AnimationStyle.Loop, true);
		this.damageSlashingLance.originX = 194;
		this.damageSlashingLance.originY = 31;
		this.allBackLayerEffects.add(this.damageSlashingLance);

		this.health = new Sprites("/Dice/Health/Health", 77, fps30, AnimationStyle.Loop, true);
		this.health.originX = 140;
		this.health.originY = 175;
		this.allFrontLayerEffects.add(this.health);

		this.healthB = new Sprites("/Dice/Health/HealthB", 77, fps30, AnimationStyle.Loop, true);
		this.healthB.originX = 140;
		this.healthB.originY = 175;
		this.allFrontLayerEffects.add(this.healthB);

		this.damageSlashingAx = new Sprites("/Dice/Damage/Slashing/SlashingAx", 68, fps30, AnimationStyle.Loop, true);
		this.damageSlashingAx.originX = 174;
		this.damageSlashingAx.originY = 31;
		this.allBackLayerEffects.add(this.damageSlashingAx);

		this.damageThunder = new Sprites("/Dice/Damage/Thunder/Thunder", 79, fps30, AnimationStyle.Loop, true);
		this.damageThunder.originX = 110;
		this.damageThunder.originY = 108;
		this.allBackLayerEffects.add(this.damageThunder);

		const baneOriginShiftY = 150;
		this.baneBack = new Sprites("/Dice/Damage/Bane/BaneBottom", 175, fps30, AnimationStyle.Loop, true);
		this.baneBack.originX = 51;
		this.baneBack.originY = 0 + baneOriginShiftY - 67;
		this.allBackLayerEffects.add(this.baneBack);

		this.baneFront = new Sprites("/Dice/Damage/Bane/BaneTop", 175, fps30, AnimationStyle.Loop, true);
		this.baneFront.originX = 51;
		this.baneFront.originY = 0 + baneOriginShiftY;
		this.allFrontLayerEffects.add(this.baneFront);

		this.blessBack = new Sprites("/Dice/Damage/Bless/BlessBack", 50, fps30, AnimationStyle.Loop, true);
		this.blessBack.originX = 133;
		this.blessBack.originY = 135;
		this.allBackLayerEffects.add(this.blessBack);

		this.blessFront = new Sprites("/Dice/Damage/Bless/BlessFront", 50, fps30, AnimationStyle.Loop, true);
		this.blessFront.originX = 79;
		this.blessFront.originY = 75;
		this.allFrontLayerEffects.add(this.blessFront);

		this.superiorityDragonHead = new Sprites("/Dice/Damage/Superiority/SuperiorityDragon", 81, fps20, AnimationStyle.Loop, true);
		this.superiorityDragonHead.originX = 74;
		this.superiorityDragonHead.originY = 205;
		this.allBackLayerEffects.add(this.superiorityDragonHead);

		this.superiorityDragonHeadB = new Sprites("/Dice/Damage/Superiority/SuperiorityDragonB", 81, fps20, AnimationStyle.Loop, true);
		this.superiorityDragonHeadB.originX = 74;
		this.superiorityDragonHeadB.originY = 205;
		this.allBackLayerEffects.add(this.superiorityDragonHeadB);

		this.superiorityDragonHeadC = new Sprites("/Dice/Damage/Superiority/SuperiorityDragonC", 81, fps20, AnimationStyle.Loop, true);
		this.superiorityDragonHeadC.originX = 74;
		this.superiorityDragonHeadC.originY = 205;
		this.allBackLayerEffects.add(this.superiorityDragonHeadC);

		this.superiorityDragonHeadD = new Sprites("/Dice/Damage/Superiority/SuperiorityDragonD", 81, fps20, AnimationStyle.Loop, true);
		this.superiorityDragonHeadD.originX = 74;
		this.superiorityDragonHeadD.originY = 205;
		this.allBackLayerEffects.add(this.superiorityDragonHeadD);

		//this.superiorityDragonMouth = new Sprites("/Dice/Damage/Superiority/SuperiorityMouth", 81, fps20, AnimationStyle.Loop, true);
		//this.superiorityDragonMouth.originX = 74;
		//this.superiorityDragonMouth.originY = 205;
		//this.allBackLayerEffects.add(this.superiorityDragonMouth);

		this.superiorityFire = new Sprites("/Dice/Damage/Superiority/SuperiorityFire", 103, fps30, AnimationStyle.Loop, true);
		this.superiorityFire.originX = 82;
		this.superiorityFire.originY = 86;
		this.allBackLayerEffects.add(this.superiorityFire);

		this.damagePsychic = new Sprites("/Dice/Damage/Psychic/Psychic", 75, fps30, AnimationStyle.Loop, true);
		this.damagePsychic.originX = 145;
		this.damagePsychic.originY = 143;
		this.allBackLayerEffects.add(this.damagePsychic);

		this.damagePsychicB = new Sprites("/Dice/Damage/Psychic/PsychicB", 75, fps30, AnimationStyle.Loop, true);
		this.damagePsychicB.originX = 145;
		this.damagePsychicB.originY = 143;
		this.allBackLayerEffects.add(this.damagePsychicB);

		this.damageLightningA = new Sprites("/Dice/Damage/Lightning/LightningA", 54, fps30, AnimationStyle.Loop, true);
		this.damageLightningA.originX = 34;
		this.damageLightningA.originY = 81;
		this.allBackLayerEffects.add(this.damageLightningA);

		this.damageLightningB = new Sprites("/Dice/Damage/Lightning/LightningB", 54, fps30, AnimationStyle.Loop, true);
		this.damageLightningB.originX = 34;
		this.damageLightningB.originY = 81;
		this.allBackLayerEffects.add(this.damageLightningB);

		this.damageLightningC = new Sprites("/Dice/Damage/Lightning/LightningC", 54, fps30, AnimationStyle.Loop, true);
		this.damageLightningC.originX = 34;
		this.damageLightningC.originY = 81;
		this.allBackLayerEffects.add(this.damageLightningC);

		this.damageLightningCloud = new Sprites("/Dice/Damage/Lightning/LightningCloud", 54, fps30, AnimationStyle.Loop, true);
		this.damageLightningCloud.originX = 99;
		this.damageLightningCloud.originY = 95;
		this.allBackLayerEffects.add(this.damageLightningCloud);

		this.inspirationParticles = new Sprites("/Dice/Inspiration/InspirationParticles", 64, fps30, AnimationStyle.Loop, true);
		this.inspirationParticles.name = 'InspirationParticles';
		this.inspirationParticles.originX = 215;
		this.inspirationParticles.originY = 106;
		this.allBackLayerEffects.add(this.inspirationParticles);

		this.inspirationSmoke = new Sprites("/Dice/Inspiration/InspirationSmoke", 177, fps30, AnimationStyle.Loop, true);
		this.inspirationSmoke.name = 'InspirationSmoke';
		this.inspirationSmoke.originX = 106;
		this.inspirationSmoke.originY = 99;
		this.allFrontLayerEffects.add(this.inspirationSmoke);

		this.ripples = new Sprites("/Dice/Ripple/Ripple", 38, fps30, AnimationStyle.Sequential, true);
		this.ripples.name = 'Ripple';
		this.ripples.originX = 121;
		this.ripples.originY = 126;
		this.allBackLayerEffects.add(this.ripples);

		this.ravens = [];
		this.loadRavens(3);

		this.fireTrails = [];
		this.groundBurstTrails = [];
		this.frostTrails = [];
		this.loadFireTrails();
		this.loadFrostTrails();

		const groundBurstTrail = new Sprites(`/Dice/GroundBurstTrail/GroundBurstTrail`, 128, fps30, AnimationStyle.SequentialStop, true);
		groundBurstTrail.originX = 129;
		groundBurstTrail.originY = 125;
		groundBurstTrail.name = "GroundBurstTrail";
		this.allBackLayerEffects.add(groundBurstTrail);

		const waterTrail = new Sprites(`/Dice/WaterTrail/WaterTrail`, 276, fps30, AnimationStyle.Sequential, true);
		waterTrail.originX = 174;
		waterTrail.originY = 182;
		waterTrail.name = "WaterTrail";
		this.allBackLayerEffects.add(waterTrail);

		this.spirals = new Sprites("/Dice/Spiral/Spiral", 64, fps40, AnimationStyle.Sequential, true);
		this.spirals.name = 'Spiral';
		this.spirals.originX = 134;
		this.spirals.originY = 107;
		this.allBackLayerEffects.add(this.spirals);

		this.fangs = new Sprites("/Dice/Fang/Fang", 79, fps30, AnimationStyle.Loop, true);
		this.fangs.name = 'Fangs';
		this.fangs.originX = 87;
		this.fangs.originY = 61;
		this.allBackLayerEffects.add(this.fangs);

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

		this.smokeExplosionTop = new Sprites("/Dice/SneakAttack/SneakAttackTop", 91, fps30, AnimationStyle.Sequential, true);
		this.smokeExplosionTop.name = 'SmokeExplosion';
		this.smokeExplosionTop.originX = 373;
		this.smokeExplosionTop.originY = 377;
		this.allFrontLayerEffects.add(this.smokeExplosionTop);

		this.smoke = new Sprites("/Dice/SneakAttack/Puff", 32, fps30, AnimationStyle.Sequential, true);
		this.smoke.name = 'Smoke';
		this.smoke.originX = 201;
		this.smoke.originY = 404;
		this.allFrontLayerEffects.add(this.smoke);

		this.smokeExplosionBottom = new Sprites("/Dice/SneakAttack/SneakAttackBottom", 91, fps30, AnimationStyle.Sequential, true);
		this.smokeExplosionBottom.name = 'SmokeExplosion';
		this.smokeExplosionBottom.originX = 373;
		this.smokeExplosionBottom.originY = 377;
		this.allBackLayerEffects.add(this.smokeExplosionBottom);

		this.cloverRing = new Sprites("/Dice/Luck/CloverRing", 120, fps30, AnimationStyle.Loop, true);
		this.cloverRing.name = 'CloverRing';
		this.cloverRing.originX = 141;
		this.cloverRing.originY = 137;
		this.allFrontLayerEffects.add(this.cloverRing);

		this.badLuckRing = new Sprites("/Dice/Luck/BadLuckRing", 120, fps30, AnimationStyle.Loop, true);
		this.badLuckRing.name = 'BadLuckRing';
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
	}

	private loadDieBurstSmokes() {
		this.diceSmokeRed = this.loadDieBurstSmoke('Red');
	}

	private loadDieBurstSmoke(color: string): Sprites {
		const result: Sprites = new Sprites(`/Dice/DieBurst/DieBurstSmoke_${color}`, 28, fps30, AnimationStyle.Sequential, true);
		result.originX = 136;
		result.originY = 170;
		this.allFrontLayerEffects.add(result);
		return result;
	}

	private loadDieBursts() {
		this.diceBurst = this.loadDieBurst('Red');
	}

	private loadDieBurstShockwaves() {
		this.diceShockwaveRed = this.loadDieBurstShockwave('Red');
	}

	private loadDieBurstShockwave(color: string): Sprites {
		const sprites: Sprites = new Sprites(`/Dice/DieBurst/DieBurstShockwave_${color}`, 44, fps30, AnimationStyle.Sequential, true);
		sprites.originX = 188;
		sprites.originY = 188;
		this.allBackLayerEffects.add(sprites);
		return sprites;
	}

	private loadDieBurst(color: string): Sprites {
		const sprites: Sprites = new Sprites(`/Dice/DieBurst/DieBurst_${color}`, 66, fps30, AnimationStyle.SequentialStop, true);
		sprites.originX = 145;
		sprites.originY = 145;
		this.allFrontLayerEffects.add(sprites);
		return sprites;
	}

	private loadSmokeyPortal(layer: string, originX: number, originY: number): Sprites {
		const sprites: Sprites = new Sprites(`/Dice/SmokeyPortal/ElectricPortal${layer}`, 102, fps30, AnimationStyle.Sequential, true);
		sprites.originX = originX;
		sprites.originY = originY;
		return sprites;
	}

	private addHalo(color: string) {
		const sprites: Sprites = new Sprites(`/Dice/PaladinSpin/PaladinSpin_${color}`, 85, fps30, AnimationStyle.Loop, true);
		sprites.name = 'HaloSpin';
		sprites.originX = 200;
		sprites.originY = 200;
		this.allFrontLayerEffects.add(sprites);
		return sprites;
	}

	private createMagicRing(color: string): Sprites {
		const rings: Sprites = new Sprites(`/Dice/MagicRing/MagicRing_${color}`, 120, fps30, AnimationStyle.Loop, true);
		rings.originX = 140;
		rings.originY = 112;
		this.allFrontLayerEffects.add(rings);
		return rings;
	}

	attachDamageFire(die: IDie, scale = 1, hueShift = 0): void {
		die.attachedDamage = true;
		die.attachedSprites.push(this.addDamageFire(die, 960, 540, Random.max(360), hueShift));
		die.origins.push(this.damageFire.getOrigin());

		this.addTopDieCloud(die, 5, Random.between(20, 45), 0.6, 0, 0, 0);

		diceSounds.safePlayMp3('Dice/Damage/Fire');
	}

	attachLabel(die: IDie, label: string, backgroundColor: string, textColor: string, scale = 1, diceGroup = DiceGroup.Players): void {
		label = label.trim();
		if (label.startsWith('"') && label.endsWith('"'))  // Remove quotes.
			label = label.substr(1, label.length - 2);
		const textEffect: TextEffect = this.animations.addText(new Vector(960, 540), label);
		textEffect.fontSize = 30 * scale;
		textEffect.offsetY = 80 * scale;
		textEffect.fontColor = textColor;
		textEffect.outlineColor = backgroundColor;
		textEffect.data = diceGroup;
		die.attachedLabels.push(textEffect);
	}

	attachDamageCold(die: IDie): void {
		die.attachedDamage = true;
		die.attachedSprites.push(this.addDamageCold(die, 960, 540, 0));
		die.origins.push(this.damageCold.getOrigin());
		this.addTopDieCloud(die, 5, Random.between(20, 45), 0.6, 0, 0, 150);
		diceSounds.safePlayMp3('Dice/Damage/Cold');
	}

	attachDamageNecrotic(die: IDie): void {
		die.attachedDamage = true;

		const numHeads = 3;

		let rotationOffset: number = Random.between(0, 360 / numHeads);
		const autoRotation: number = -Random.between(20, 45);

		die.attachedSprites.push(this.addDamageNecroticFog(die, 960, 540, rotationOffset));
		die.origins.push(this.damageNecroticFog.getOrigin());

		for (let i = 0; i < numHeads; i++) {
			die.attachedSprites.push(this.addDamageNecroticHead(die, 960, 540, rotationOffset + 0, autoRotation));
			die.origins.push(this.damageNecroticHead.getOrigin());
			rotationOffset += 360 / numHeads;
		}

		die.attachedSprites.push(this.addDamageNecroticFog(die, 960, 540, rotationOffset));
		die.origins.push(this.damageNecroticFog.getOrigin());

		this.addTopDieCloud(die, 3, Random.between(20, 45), 0.9, 30, 75, 75);

		diceSounds.safePlayMp3('Dice/Damage/Necrotic');
	}

	addTopDieCloud(die: IDie, count: number, autoRotationDegeesPerSecond: number, opacity: number, hueShift: number, saturation = -1, brightness = -1): void {
		const angleBetweenClouds = 360 / count;
		let rotationOffset: number = Random.between(0, angleBetweenClouds);
		for (let i = 0; i < count; i++) {
			hueShift = hueShift % 360;
			let topDieCloud: SpriteProxy;
			if (saturation < 60)
				if (brightness < 100)
					topDieCloud = this.topDieCloudBlack.add(0, 0, Sprites.startAtRandomFrame);
				else
					topDieCloud = this.topDieCloudWhite.add(0, 0, Sprites.startAtRandomFrame);
			else if (hueShift < 70)
				topDieCloud = this.topDieCloudRed.add(0, 0, Sprites.startAtRandomFrame);
			else if (hueShift < 160)
				topDieCloud = this.topDieCloudGreen.add(0, 0, Sprites.startAtRandomFrame);
			else if (hueShift < 250)
				topDieCloud = this.topDieCloudBlue.add(0, 0, Sprites.startAtRandomFrame);
			else
				topDieCloud = this.topDieCloudPurple.add(0, 0, Sprites.startAtRandomFrame);

			this.initSprite(topDieCloud, die);
			topDieCloud.rotation = rotationOffset;
			topDieCloud.initialRotation = rotationOffset;
			rotationOffset += angleBetweenClouds;
			topDieCloud.autoRotationDegeesPerSecond = autoRotationDegeesPerSecond;
			topDieCloud.fadeInTime = 500;
			topDieCloud.fadeOutTime = 500;
			topDieCloud.opacity = opacity;
			topDieCloud.scale = die.scale;
			topDieCloud.fadeOnDestroy = true;

			die.attachedSprites.push(topDieCloud);
			die.origins.push(this.topDieCloudRed.getOrigin());
		}
	}


	attachDamageAcid(die: IDie): void {
		die.attachedDamage = true;
		const rotationOffset: number = Random.plusMinus(24);
		die.attachedSprites.push(this.addDamageAcid(die, 960, 540, rotationOffset));
		die.origins.push(this.damageAcid.getOrigin());
		this.addTopDieCloud(die, 3, Random.between(20, 45), 0.5, 100, 200);
		diceSounds.safePlayMp3('Dice/Damage/Acid');
	}


	attachDamageRadiant(die: IDie): void {
		die.attachedDamage = true;
		const rotationOffset: number = Random.between(0, 360);
		die.attachedSprites.push(this.addDamageRadiant(die, 960, 540, rotationOffset));
		die.origins.push(this.damageRadiant.getOrigin());
		const cloudCount: number = Random.between(3, 5);
		this.addTopDieCloud(die, cloudCount, Random.between(20, 45), 0.4, 50, 0, 200);
		diceSounds.safePlayMp3('Dice/Damage/Radiant');
	}

	attachDamagePoison(die: IDie): void {
		die.attachedDamage = true;
		const rotationOffset: number = Random.between(0, 360);
		const hueShift: number = Random.plusMinus(30);
		die.attachedSprites.push(this.addDamagePoison(die, 960, 540, rotationOffset, hueShift));
		die.origins.push(this.damagePoison.getOrigin());
		this.addTopDieCloud(die, 4, Random.between(20, 45), 0.7, 100 + hueShift, 100, 100);
		diceSounds.safePlayMp3('Dice/Damage/Poison');
	}

	attachDamagePiercing(die: IDie): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.between(0, 360);
		const autoRotation: number = Random.plusMinusBetween(5, 10);
		this.addDagger(die, rotationOffset, autoRotation);
		rotationOffset += 90;
		this.addSword(die, rotationOffset, autoRotation);
		rotationOffset += 90;
		this.addTrident(die, rotationOffset, autoRotation);
		rotationOffset += 90;
		this.addTooth(die, rotationOffset, autoRotation);

		diceSounds.safePlayMp3('Dice/Damage/Piercing');
	}

	private addSword(die: IDie, rotationOffset: number, autoRotation: number) {
		die.attachedSprites.push(this.addDamagePiercingThickSword(die, 960, 540, rotationOffset, autoRotation));
		die.origins.push(this.damagePiercingThickSword.getOrigin());
	}

	private addTooth(die: IDie, rotationOffset: number, autoRotation: number) {
		die.attachedSprites.push(this.addDamagePiercingTooth(die, 960, 540, rotationOffset, autoRotation));
		die.origins.push(this.damagePiercingTooth.getOrigin());
	}

	private addTrident(die: IDie, rotationOffset: number, autoRotation: number) {
		die.attachedSprites.push(this.addDamagePiercingTrident(die, 960, 540, rotationOffset, autoRotation));
		die.origins.push(this.damagePiercingTrident.getOrigin());
	}

	private addDagger(die: IDie, rotationOffset: number, autoRotation: number) {
		die.attachedSprites.push(this.addDamagePiercingDagger(die, 960, 540, rotationOffset, autoRotation));
		die.origins.push(this.damagePiercingDagger.getOrigin());
	}

	attachDamageSlashing(die: IDie): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.between(0, 360);
		const autoRotation: number = Random.plusMinusBetween(60, 120);

		const numBlades = 5;
		const degreesBetweenBlades: number = 360 / numBlades;
		for (let i = 0; i < numBlades; i++) {
			const randomNum: number = Random.max(100);
			if (randomNum < 33) {
				die.attachedSprites.push(this.addDamageSlashingSword(die, 960, 540, rotationOffset, autoRotation));
				die.origins.push(this.damageSlashingSword.getOrigin());
			}
			else if (randomNum < 66) {
				die.attachedSprites.push(this.addDamageSlashingLance(die, 960, 540, rotationOffset, autoRotation));
				die.origins.push(this.damageSlashingLance.getOrigin());
			}
			else {
				die.attachedSprites.push(this.addDamageSlashingAx(die, 960, 540, rotationOffset, autoRotation));
				die.origins.push(this.damageSlashingAx.getOrigin());
			}
			rotationOffset += degreesBetweenBlades;
		}

		diceSounds.safePlayMp3('Dice/Damage/Slashing');
	}

	attachHealth(die: IDie): void {
		die.attachedSprites.push(this.addHealth(die, 960, 540));
		die.origins.push(this.health.getOrigin());
		diceSounds.safePlayMp3('Dice/health');
	}

	attachDamageThunder(die: IDie): void {
		die.attachedDamage = true;
		const rotationOffset: number = Random.between(0, 360);
		die.attachedSprites.push(this.addDamageThunder(die, 960, 540, rotationOffset));
		die.origins.push(this.damageThunder.getOrigin());
		this.addTopDieCloud(die, 3, Random.between(20, 45), 0.8, 260 + Random.plusMinus(20), 100, 50);
		diceSounds.safePlayMp3('Dice/Damage/Thunder');
	}

	attachSuperiority(die: IDie): void {
		const numHeads = 3;

		let rotationOffset: number = Random.between(0, 360 / numHeads);
		const autoRotation: number = Random.plusMinusBetween(20, 45);

		die.attachedSprites.push(this.addSuperiorityFire(die, 960, 540, rotationOffset));
		die.origins.push(this.superiorityFire.getOrigin());

		for (let i = 0; i < numHeads; i++) {
			die.attachedSprites.push(this.addSuperiorityDragonHead(die, 960, 540, rotationOffset + 0, autoRotation));
			die.origins.push(this.superiorityDragonHead.getOrigin());

			rotationOffset += 360 / numHeads;
		}

		//this.addTopDieCloud(die, 5, Random.between(20, 45), 0.2, 0, 0, 100);

		diceSounds.safePlayMp3('Dice/Damage/DragonRoar');
	}

	attachBless(die: IDie): void {
		const hsl = this.rgbToHSL(die.diceColor);
		const blessBackSprite: SpriteProxy = this.blessBack.addShifted(960, 540, -1, hsl.h);
		const blessFrontSprite: SpriteProxy = this.blessFront.addShifted(960, 540, -1, hsl.h);
		blessBackSprite.autoRotationDegeesPerSecond = Random.between(-20, 20);
		blessFrontSprite.autoRotationDegeesPerSecond = Random.between(-20, 20);
		this.initSprite(blessFrontSprite, die);
		this.initSprite(blessBackSprite, die);

		die.attachedSprites.push(blessBackSprite);
		die.origins.push(this.blessBack.getOrigin());

		die.attachedSprites.push(blessFrontSprite);
		die.origins.push(this.blessFront.getOrigin());

	}

	attachBane(die: IDie): void {
		const numHeads = 3;

		let rotationOffset: number = Random.between(0, 360 / numHeads);
		const autoRotation: number = Random.plusMinusBetween(20, 45);

		const hsl = this.rgbToHSL(die.diceColor);
		const saturation: number = hsl.s * 100;
		const brightness = 100; //(3 * 100 + hsl.l * 100) / 4;
		const hueShift: number = hsl.h;

		for (let i = 0; i < numHeads; i++) {
			const randomStartingFrameIndex: number = Math.floor(Random.max(this.baneBack.baseAnimation.frameCount));

			const thisHueShift: number = hueShift + Random.plusMinus(10);
			die.attachedSprites.push(this.addBane(die, this.baneBack, 960, 540, randomStartingFrameIndex, thisHueShift, saturation, brightness, rotationOffset, autoRotation));
			die.origins.push(this.baneBack.getOrigin());

			die.attachedSprites.push(this.addBane(die, this.baneFront, 960, 540, randomStartingFrameIndex, thisHueShift, saturation, brightness, rotationOffset, autoRotation));
			die.origins.push(this.baneFront.getOrigin());

			rotationOffset += 360 / numHeads;
		}

		//this.addTopDieCloud(die, 5, Random.between(20, 45), 0.2, 0, 0, 100);

		// TODO: Add a Bane sound.
		diceSounds.safePlayMp3('Dice/Damage/Bane');
	}

	addBane(die: IDie, sprites: Sprites, x: number, y: number, startingFrameIndex: number, hueShift: number, saturation: number, brightness: number, rotationOffset: number, autoRotation: number): SpriteProxy {
		const sprite: SpriteProxy = sprites.addShifted(x, y, startingFrameIndex, hueShift, saturation, brightness);
		sprite.rotation = rotationOffset;
		sprite.initialRotation = rotationOffset;
		sprite.autoRotationDegeesPerSecond = autoRotation;
		this.initSprite(sprite, die);
		return sprite;
	}

	attachDamageForce(die: IDie): void {
		die.attachedDamage = true;
		const rotationOffset: number = Random.plusMinus(60);
		die.attachedSprites.push(this.addDamageForce(die, 960, 540, rotationOffset));
		die.origins.push(this.damageForce.getOrigin());

		die.attachedSprites.push(this.addDamageForce(die, 960, 540, rotationOffset + 120));
		die.origins.push(this.damageForce.getOrigin());

		die.attachedSprites.push(this.addDamageForce(die, 960, 540, rotationOffset + 240));
		die.origins.push(this.damageForce.getOrigin());

		this.addTopDieCloud(die, 2, Random.between(20, 45), 0.7, 250 + Random.plusMinus(20), 100, 50);

		diceSounds.safePlayMp3('Dice/Damage/Force');
	}

	attachDamageBludgeoning(die: IDie): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.plusMinus(60);
		const autoRotationDegeesPerSecond: number = -Random.between(60, 100);

		this.addBludgeoningCloud(die, rotationOffset, autoRotationDegeesPerSecond);
		this.addBludgeoningCloud(die, rotationOffset + 120, autoRotationDegeesPerSecond);
		this.addBludgeoningCloud(die, rotationOffset + 240, autoRotationDegeesPerSecond);

		const numWeapons = 3;
		for (let i = 0; i < numWeapons; i++) {
			this.addRandomBludgeoningWeapon(die, rotationOffset, autoRotationDegeesPerSecond);
			rotationOffset += 360 / numWeapons;
		}

		diceSounds.safePlayMp3('Dice/Damage/Bludgeoning');
	}

	addRandomBludgeoningWeapon(die: IDie, rotationOffset: number, autoRotationDegeesPerSecond: number): void {
		if (Random.chancePercent(25)) {
			die.attachedSprites.push(this.addDamageBludgeoningMace(die, 960, 540, rotationOffset, autoRotationDegeesPerSecond));
			die.origins.push(this.damageBludgeoningMace.getOrigin());
		}
		else if (Random.chancePercent(33)) {
			const fist: SpriteProxy = this.addDamageBludgeoningFist(die, 960, 540, rotationOffset, autoRotationDegeesPerSecond);
			die.attachedSprites.push(fist);
			die.origins.push(this.damageBludgeoningFist.getOrigin());
		}
		else if (Random.chancePercent(50)) {
			die.attachedSprites.push(this.addDamageBludgeoningWeapon(960, 540, die.scale, rotationOffset, autoRotationDegeesPerSecond, 0));
			die.origins.push(this.damageBludgeoningWeapon.getOrigin());
		}
		else {
			die.attachedSprites.push(this.addDamageBludgeoningWeapon(960, 540, die.scale, rotationOffset, autoRotationDegeesPerSecond, 1));
			die.origins.push(this.damageBludgeoningWeapon.getOrigin());
		}
	}

	addBludgeoningCloud(die: IDie, rotationOffset: number, autoRotationDegeesPerSecond: number): void {
		die.attachedSprites.push(this.addDamageSpinningCloudTrail(die, 960, 540, rotationOffset + 240, autoRotationDegeesPerSecond, Sprites.startAtRandomFrame));
		die.origins.push(this.damageSpinningCloudTrail.getOrigin());
	}

	attachDamagePsychic(die: IDie): void {
		die.attachedDamage = true;
		const rotationOffset: number = Random.max(360);
		die.attachedSprites.push(this.addDamagePsychic(die, 960, 540, rotationOffset));
		die.origins.push(this.damagePsychic.getOrigin());
		this.addTopDieCloud(die, 3, Random.between(20, 45), 1, Random.max(360), 100, 150);
		diceSounds.safePlayMp3('Dice/Damage/Psychic');
	}


	attachDamageLightning(die: IDie): void {
		die.attachedDamage = true;
		const rotationOffset: number = Random.max(120);

		die.attachedSprites.push(this.addDamageLightningCloud(die, 960, 540, Random.max(360)));
		die.origins.push(this.damageLightningCloud.getOrigin());

		const autoRotateDPS: number = Random.plusMinusBetween(10, 30);

		die.attachedSprites.push(this.addDamageLightningA(die, 960, 540, rotationOffset, autoRotateDPS));
		die.origins.push(this.damageLightningA.getOrigin());

		die.attachedSprites.push(this.addDamageLightningB(die, 960, 540, rotationOffset + 120, autoRotateDPS));
		die.origins.push(this.damageLightningB.getOrigin());

		die.attachedSprites.push(this.addDamageLightningC(die, 960, 540, rotationOffset + 240, autoRotateDPS));
		die.origins.push(this.damageLightningC.getOrigin());

		this.addTopDieCloud(die, 3, Random.between(20, 45), 0.4, 250 + Random.plusMinus(30), 100, 150);
		diceSounds.safePlayMp3('Dice/Damage/Lightning');
	}


	initializePlayerData(playerData: string): void {
		this.playerDataSet = true;
		this.players = [];
		const playerDto: Array<Character> = JSON.parse(playerData);
		for (let i = 0; i < playerDto.length; i++) {
			this.players.push(new Character(playerDto[i]));
		}
	}


	loadRavens(count: number): void {
		for (let i = 0; i < count; i++) {
			const raven = new Sprites(`/Dice/Ravens/${i + 1}/Ravens`, 36, fps30, AnimationStyle.Sequential, true);
			raven.originX = 44;
			raven.originY = 477;
			this.allBackLayerEffects.add(raven);
			this.ravens.push(raven);
		}
	}

	loadFireTrails(): void {
		const fireTrailA = new Sprites(`/Dice/FireTrail/A/FireTrailA`, 68, fps30, AnimationStyle.Sequential, true);
		fireTrailA.originX = 115;
		fireTrailA.originY = 444;
		this.allBackLayerEffects.add(fireTrailA);
		this.fireTrails.push(fireTrailA);

		const fireTrailB = new Sprites(`/Dice/FireTrail/B/FireTrailB`, 61, fps30, AnimationStyle.Sequential, true);
		fireTrailB.originX = 73;
		fireTrailB.originY = 641;
		this.allBackLayerEffects.add(fireTrailB);
		this.fireTrails.push(fireTrailB);

		const fireTrailC = new Sprites(`/Dice/FireTrail/C/FireTrailC`, 36, fps30, AnimationStyle.Sequential, true);
		fireTrailC.originX = 142;
		fireTrailC.originY = 174;
		this.allBackLayerEffects.add(fireTrailC);
		this.fireTrails.push(fireTrailC);

		const fireTrailD = new Sprites(`/Dice/FireTrail/D/FireTrailD`, 78, fps30, AnimationStyle.Sequential, true);
		fireTrailD.originX = 110;
		fireTrailD.originY = 325;
		this.allBackLayerEffects.add(fireTrailD);
		this.fireTrails.push(fireTrailD);

		const fireTrailE = new Sprites(`/Dice/FireTrail/E/FireTrailE`, 58, fps30, AnimationStyle.Sequential, true);
		fireTrailE.originX = 152;
		fireTrailE.originY = 120;
		this.allBackLayerEffects.add(fireTrailE);
		this.fireTrails.push(fireTrailE);
	}

	loadFrostTrails(): void {
		const originX = 153;
		const originY = 400;
		const frostTrailA = new Sprites(`/Dice/FrostTrail/A/FrostTrailA`, 56, fps30, AnimationStyle.SequentialStop, true);
		frostTrailA.originX = originX;
		frostTrailA.originY = originY;
		this.allBackLayerEffects.add(frostTrailA);
		this.frostTrails.push(frostTrailA);

		const frostTrailB = new Sprites(`/Dice/FrostTrail/B/FrostTrailB`, 56, fps30, AnimationStyle.SequentialStop, true);
		frostTrailB.originX = originX;
		frostTrailB.originY = originY;
		this.allBackLayerEffects.add(frostTrailB);
		this.frostTrails.push(frostTrailB);

		const frostTrailC = new Sprites(`/Dice/FrostTrail/C/FrostTrailC`, 56, fps30, AnimationStyle.SequentialStop, true);
		frostTrailC.originX = originX;
		frostTrailC.originY = originY;
		this.allBackLayerEffects.add(frostTrailC);
		this.frostTrails.push(frostTrailC);

		const frostTrailD = new Sprites(`/Dice/FrostTrail/D/FrostTrailD`, 56, fps30, AnimationStyle.SequentialStop, true);
		frostTrailD.originX = originX;
		frostTrailD.originY = originY;
		this.allBackLayerEffects.add(frostTrailD);
		this.frostTrails.push(frostTrailD);

		const frostTrailE = new Sprites(`/Dice/FrostTrail/E/FrostTrailE`, 56, fps30, AnimationStyle.SequentialStop, true);
		frostTrailE.originX = originX;
		frostTrailE.originY = originY;
		this.allBackLayerEffects.add(frostTrailE);
		this.frostTrails.push(frostTrailE);
	}

	addBackgroundRect(x: number, y: number, width: number, height: number, lifespan: number): AnimatedRectangle {
		const borderThickness = 2;
		const opacity = 0.95;
		return this.animations.addRectangle(x, y, width, height, '#f5e0b7', '#ae722c', lifespan, borderThickness, opacity);
	}

	setMultiplayerFades(animatedElement: AnimatedElement) {
		animatedElement.fadeOutTime = 1000;
		animatedElement.fadeInTime = 1000;
	}

	showMultiplayerResults(title: string, rollSummary: PlayerRoll[], hiddenThreshold = 0): void {
		let x = 10;
		let y = 10;
		const lineHeight = 70;
		const backgroundRect: AnimatedRectangle = this.addBackgroundRect(2, 2, 50, 50 /* lineHeight * initiativeSummary.length + margins * 2 */, DiceLayer.multiplePlayerSummaryDuration);
		backgroundRect.data = DiceGroup.Players;
		this.setMultiplayerFades(backgroundRect);
		let lastRollWasHigherThanThreshold = true;
		if (title) {
			const titleEffect: TextEffect = this.showMultiplayerResultTitle(title, x, y);
			titleEffect.connectedShapes.push(backgroundRect);
			titleEffect.fontSize = 15;
			y += lineHeight;
			backgroundRect.height += lineHeight;
			x += 30;
		}

		const knownPlayerNames: Array<TextEffect> = [];

		let line: AnimatedLine = null;
		for (let i = 0; i < rollSummary.length; i++) {
			const playerRoll: PlayerRoll = rollSummary[i];
			if (playerRoll.roll + playerRoll.modifier + playerRoll.cardModifierTotal < hiddenThreshold && lastRollWasHigherThanThreshold) {
				if (hiddenThreshold !== 0) {
					const separatorHeight = 20;
					line = this.addSeparatorHorizontalLine(x, y, 10, '#ff0000', DiceLayer.multiplePlayerSummaryDuration);
					line.data = DiceGroup.Players;
					this.setMultiplayerFades(line);
					y += separatorHeight;
					backgroundRect.height += separatorHeight;
				}
				lastRollWasHigherThanThreshold = false;
			}
			const effect: TextEffect = this.showPlayerRoll(playerRoll, x, y);
			effect.connectedShapes.push(backgroundRect);
			knownPlayerNames.push(effect);
			y += lineHeight;
		}

		if (line) {
			knownPlayerNames.forEach(function (playerName: TextEffect) { playerName.connectedShapes.push(line) });
		}

		// knownPlayerNames  <<---- ??? 
	}

	addSeparatorHorizontalLine(x: number, y: number, width: number, color: string, lifespan: number): AnimatedLine {
		return this.animations.addLine(x, y, width, color, lifespan, 2);
	}

	static readonly multiplePlayerSummaryDuration: number = 14000;

	showMultiplayerResultTitle(title: string, x: number, y: number): TextEffect {
		const textEffect: TextEffect = this.animations.addText(new Vector(x, y), title, DiceLayer.multiplePlayerSummaryDuration);
		textEffect.data = DiceGroup.Players;
		textEffect.fontColor = '#000000';
		textEffect.outlineColor = '#ffffff';
		this.setMultiplayerResultText(textEffect);
		return textEffect;
	}

	setMultiplayerResultText(textEffect: TextEffect): void {
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
		const modifier: number = playerRoll.modifier + playerRoll.cardModifierTotal;
		let message = `${playerRoll.name}: ${playerRoll.roll + modifier}`;
		if (playerRoll.modifier > 0)
			message += ` (+${playerRoll.modifier}${playerRoll.cardModifiersStr})`;
		else if (playerRoll.modifier < 0)
			message += ` (${playerRoll.modifier}${playerRoll.cardModifiersStr})`;
		else if (playerRoll.cardModifierTotal !== 0)
			message += ` (${playerRoll.cardModifiersStr})`;

		const textEffect: TextEffect = this.animations.addText(new Vector(x, y), message, DiceLayer.multiplePlayerSummaryDuration);
		textEffect.data = DiceGroup.Players;
		textEffect.fontColor = this.getDieColor(playerRoll.id);
		textEffect.outlineColor = this.getDieFontColor(playerRoll.id);
		this.setMultiplayerResultText(textEffect);
		return textEffect;
	}

	showResult(resultMessage: string, success: boolean, diceGroup = DiceGroup.Players): void {
		if (!resultMessage)
			return;
		const textEffect: TextEffect = this.animations.addText(new Vector(960, 100), resultMessage, this.resultTextTime);
		textEffect.data = diceGroup;
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

	showTotalHealthDamage(totalHealthDamageStr: string, success: boolean, label: string, fontColor: string, outlineColor: string, diceGroup = DiceGroup.Players, textCenter: Vector = undefined): void {
		let damageTime: number = this.totalDamageTime;
		if (!success)
			damageTime = 2 * damageTime / 3;
		let velocityMultiplierY = 1;
		let velocityX = 0;
		if (textCenter) {
			if (textCenter.y > 540)
				velocityMultiplierY = -1.3;
			if (textCenter.x > 960)
				velocityX = -0.4;
			else
				velocityX = 0.4;
		}
		else
			textCenter = new Vector(960, 750);
		const textEffect: TextEffect = this.animations.addText(textCenter, `${label}${totalHealthDamageStr}`, damageTime);
		textEffect.data = diceGroup;
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.elasticIn = true;
		textEffect.scale = 4;
		textEffect.velocityX = velocityX;
		textEffect.velocityY = 0.4 * velocityMultiplierY;
		textEffect.opacity = 0.90;
		if (success)
			textEffect.targetScale = 8;
		else
			textEffect.targetScale = 3;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}

	showDamageHealthModifier(modifier: number, success: boolean, fontColor: string, outlineColor: string, diceGroup = DiceGroup.Players): void {
		if (modifier === 0)
			return;
		let totalDamage: string;
		if (modifier < 0)
			totalDamage = modifier.toString();
		else
			totalDamage = '+' + modifier.toString();

		let damageTime: number = this.damageModifierTime;
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
		const textEffect: TextEffect = this.animations.addText(new Vector(960, yPos), `(${totalDamage})`, damageTime);
		textEffect.data = diceGroup;
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

	showBonusRoll(totalBonusStr: string, dieColor: string = undefined, fontColor: string = undefined, diceGroup = DiceGroup.Players): void {
		const textEffect: TextEffect = this.animations.addText(new Vector(50, 900), totalBonusStr, this.bonusRollTime);
		textEffect.data = diceGroup;
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

	showDieTotal(thisRollStr: string, playerId = Character.invalidCreatureId, diceGroup = DiceGroup.Players): void {
		const textEffect: TextEffect = this.animations.addText(new Vector(960, 540), thisRollStr, this.totalRollScoreTime);
		textEffect.data = diceGroup;
		this.setTextColorToPlayer(textEffect, playerId);
		textEffect.elasticIn = true;
		textEffect.scale = 10;
		textEffect.opacity = 0.90;
		textEffect.targetScale = 30;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}

	showDieTotalMessage(totalRoll: number, modifier: number, dieTotalMessage: string, centerPos: Vector, fillColor: string, outlineColor: string, diceGroup = DiceGroup.Players) {
		const deltaX: number = 960 - centerPos.x;
		const deltaY: number = 540 - centerPos.y;
		const thrustX: number = deltaX * 0.0006;
		const thrustY: number = deltaY * 0.0006;
		let totalRollStr: string = totalRoll.toString();
		if (modifier > 0)
			totalRollStr += ` +${modifier}`;
		else if (modifier < 0)
			totalRollStr += ` ${modifier}`;
		const numberTextEffect: TextEffect = this.animations.addText(centerPos, totalRollStr, this.totalRollScoreTime);
		numberTextEffect.data = diceGroup;
		numberTextEffect.fontColor = fillColor;
		numberTextEffect.outlineColor = outlineColor;
		numberTextEffect.elasticIn = true;
		numberTextEffect.scale = 3;
		numberTextEffect.opacity = 0.90;
		numberTextEffect.targetScale = 8;
		numberTextEffect.fadeOutTime = 800;
		numberTextEffect.fadeInTime = 200;
		numberTextEffect.horizontalThrust = thrustX;
		numberTextEffect.verticalThrust = thrustY;

		const labelOffset = 80;
		const labelCenter: Vector = new Vector(centerPos.x, centerPos.y + labelOffset);
		const labelTextEffect: TextEffect = this.animations.addText(labelCenter, dieTotalMessage, this.totalRollScoreTime);
		labelTextEffect.data = diceGroup;
		labelTextEffect.fontColor = fillColor;
		labelTextEffect.outlineColor = outlineColor;
		labelTextEffect.elasticIn = true;
		labelTextEffect.scale = 0.5;
		labelTextEffect.opacity = 0.90;
		labelTextEffect.targetScale = 2;
		labelTextEffect.fadeOutTime = 800;
		labelTextEffect.fadeInTime = 200;
		labelTextEffect.horizontalThrust = thrustX;
		labelTextEffect.verticalThrust = thrustY;
	}


	private setTextColorToPlayer(textEffect: TextEffect, playerID = Character.invalidCreatureId) {
		if (playerID === Character.invalidCreatureId)
			playerID = this.playerID;
		textEffect.fontColor = this.getDieColor(playerID);
		textEffect.outlineColor = this.getDieFontColor(playerID);
	}

	showRollModifier(playerId = Character.invalidCreatureId, diceGroup = DiceGroup.Players, allModStrs: Array<string> = null, nat20 = false): void {
		const rollModStr = this.getModStr(allModStrs, nat20);
		if (!rollModStr)
			return;
		const textEffect: TextEffect = this.animations.addText(new Vector(960, 250), `(${rollModStr})`, this.rollModifierTime);
		textEffect.data = diceGroup;
		this.setTextColorToPlayer(textEffect, playerId);
		textEffect.elasticIn = true;
		textEffect.velocityY = -0.1;
		textEffect.scale = 2;
		textEffect.opacity = 0.90;
		textEffect.targetScale = 5;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 500;
	}

	getModStr(allModStrs: string[], nat20: boolean): string {
		let totalMod = 0;
		let rollModStr = '';
		// rollModStr = this.addMod(rollModStr, rollModifier);
		allModStrs.forEach((modStr: string) => {
			if (!nat20 || +modStr > 0) {
				let prefix = '';
				if (rollModStr.length > 0)
					prefix = ' ';
				rollModStr += `${prefix}${modStr}`;
				totalMod += +modStr;
			}
		});

		if (rollModStr.indexOf(' ') >= 0) {
			if (totalMod > 0)
				rollModStr += ` = +${totalMod}`;
			else
				rollModStr += ` = ${totalMod}`;
		}
		return rollModStr;
	}

	getModTotal(allModStrs: string[], nat20: boolean): number {
		let totalMod = 0;
		allModStrs.forEach((modStr: string) => {
			if (!nat20 || +modStr > 0) {
				totalMod += +modStr;
			}
		});

		return totalMod;
	}

	addMod(rollModStr: string, modifier: number): string {
		if (!rollModStr)
			rollModStr = '';
		let prefix = '';
		if (rollModStr.length > 0)
			prefix = ' ';

		if (modifier > 0)
			rollModStr += `${prefix}+${modifier}`;
		else if (modifier < 0)
			rollModStr += `${prefix}${modifier}`;

		return rollModStr;
	}


	addDieValueLabel(centerPos: Vector, value: string, highlight = false, diceGroup = DiceGroup.Players): void {
		const textEffect: TextEffect = this.animations.addText(centerPos, value, 5000);
		textEffect.data = diceGroup;
		if (highlight)
			textEffect.fontColor = '#ff0000';
		textEffect.scale = 6;
		textEffect.opacity = 0.75;
		textEffect.targetScale = 0.5;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}

	indicateBonusRoll(message: string, diceGroup = DiceGroup.Players): void {
		const textEffect: TextEffect = this.animations.addText(new Vector(960, 800), message, 3000);
		textEffect.data = diceGroup;
		textEffect.scale = 6;
		textEffect.opacity = 0.75;
		textEffect.targetScale = 10;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 800;
	}

	reportAddOnRoll(message: string, luckMultiplier: number, diceGroup = DiceGroup.Players): void {
		if (!message)
			return;
		const textEffect: TextEffect = this.animations.addText(new Vector(960, 540), message, 3000);
		textEffect.data = diceGroup;

		if (luckMultiplier < 0) {
			// Bad luck
			textEffect.outlineColor = DiceLayer.badLuckFontColor;
			textEffect.fontColor = DiceLayer.badLuckDieColor;
		}
		else if (luckMultiplier > 0) {
			textEffect.outlineColor = DiceLayer.goodLuckFontColor;
			textEffect.fontColor = DiceLayer.goodLuckDieColor;
		}

		textEffect.scale = 6;
		textEffect.opacity = 0.75;
		textEffect.targetScale = 10;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 800;
	}


	addDieText(centerPos: Vector, message: string, fontColor: string, outlineColor: string, lifeSpan = 1500, scaleAdjust = 1, diceGroup = DiceGroup.Players): void {
		if (centerPos === null)
			return;
		const textEffect: TextEffect = this.animations.addText(centerPos.add(new Vector(0, 80 * scaleAdjust)), message, lifeSpan);
		textEffect.data = diceGroup;
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.scale = 3 * scaleAdjust;
		textEffect.waitToScale = 400;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 600;
		textEffect.targetScale = 1 * scaleAdjust;
	}

	addDieTextAfter(centerPos: Vector, message: string, fontColor: string, outlineColor: string, timeout = 0, lifeSpan = 1500, scaleAdjust = 1, diceGroup = DiceGroup.Players) {
		if (timeout > 0)
			setTimeout(() => {
				this.addDieText(centerPos, message, fontColor, outlineColor, lifeSpan, scaleAdjust, diceGroup);
			}, timeout);
		else
			this.addDieText(centerPos, message, fontColor, outlineColor, lifeSpan, scaleAdjust, diceGroup);
	}

	addDisadvantageText(centerPos: Vector, timeout = 0, isLuckyFeat = false, diceGroup = DiceGroup.Players): void {
		let message: string;
		if (isLuckyFeat)
			message = 'Lucky Feat';
		else
			message = 'Disadvantage';
		this.addDieTextAfter(centerPos, message, this.activePlayerDieColor, this.activePlayerDieColor, timeout, 1500, 1, diceGroup);
	}

	addAdvantageText(centerPos: Vector, timeout = 0, isLuckyFeat = false, diceGroup = DiceGroup.Players): void {
		let message: string;
		if (isLuckyFeat)
			message = 'Lucky Feat';
		else
			message = 'Advantage';
		this.addDieTextAfter(centerPos, message, this.activePlayerDieColor, this.activePlayerDieColor, timeout, 1500, 1, diceGroup);
	}


	//mouseDownInCanvas(e) {
	//	if (effectOverride != undefined) {
	//	  var enumIndex: number = <number>effectOverride;
	//	  let totalElements: number = Object.keys(DieEffect).length / 2;
	//	  enumIndex++;
	//	  if (enumIndex >= totalElements)
	//	    enumIndex = 0;
	//	  effectOverride = <DieEffect>enumIndex;
	//	  console.log('New effect: ' + DieEffect[effectOverride]);
	//	}
	//}

	getContext() {
		this.diceFrontCanvas = document.getElementById("diceFrontCanvas") as HTMLCanvasElement;
		//this.diceFrontCanvas.onmousedown = this.mouseDownInCanvas;
		this.diceBackCanvas = document.getElementById("diceBackCanvas") as HTMLCanvasElement;
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
			this.diceFrontContext.globalAlpha = 0.5;
			this.diceFrontContext.fillText("Dice Roller is waiting for player data to be initialized.", 100, 100);
			this.diceFrontContext.globalAlpha = 1;
		}
		const now: number = performance.now();
		this.allFrontLayerEffects.updatePositions(now);
		this.allBackLayerEffects.updatePositions(now);
		this.allFrontLayerEffects.draw(this.diceFrontContext, now);
		this.allBackLayerEffects.draw(this.diceBackContext, now);
		this.animations.removeExpiredAnimations(now);
		this.animations.updatePositions(now);
		this.animations.render(this.diceFrontContext, now);
	}

	addFireball(x: number, y: number, scale = 1): SpriteProxy {
		const fireBall: SpriteProxy = this.diceFireball.add(x, y, 0);
		fireBall.scale = scale;
		fireBall.rotation = 90;
		return fireBall;
	}

	addGroundBurstAnimation(x: number, y: number, scale: number, hueShift: number, saturation = 100, brightness = 100) {
		DiceLayer.numFiltersOnDieCleanup++;
		const dieBurst: SpriteProxy = this.diceBurst.addShifted(x, y, 0, hueShift + Random.plusMinus(25), saturation, brightness);
		dieBurst.scale = scale;
		dieBurst.rotation = Math.random() * 360;
		dieBurst.fadeOutTime = 800;
		dieBurst.expirationDate = performance.now() + 4000;
		const shockwave: SpriteProxy = this.addShockwave(x, y, scale, hueShift + Random.plusMinus(30), saturation, brightness);
		shockwave.scale = scale;
		const smokeTop: SpriteProxy = this.addDieSmoke(x, y, scale, hueShift + Random.plusMinus(50), saturation, brightness);
		smokeTop.scale = scale;

		smokeTop.delayStart = 3 * fps30;
		shockwave.delayStart = 6 * fps30;
	}

	addSmokeyPortalAnimation(x: number, y: number, scale: number, hueShift: number, saturation = 100, brightness = 100) {
		const smokeyPortalBack: SpriteProxy = this.diceSmokeyPortalBack.addShifted(x, y, 0, hueShift, saturation, brightness);
		smokeyPortalBack.rotation = Random.max(360);
		smokeyPortalBack.scale = scale;

		const smokeyPortalTop: SpriteProxy = this.diceSmokeyPortalTop.addShifted(x, y, 0, hueShift, saturation, brightness);
		smokeyPortalTop.rotation = smokeyPortalBack.rotation;
		smokeyPortalTop.scale = scale;
	}

	private addDieSmoke(x: number, y: number, scale: number, hueShift: number, saturation: number, brightness: number) {
		DiceLayer.numFiltersOnDieCleanup++;
		const smokeTop: SpriteProxy = this.diceSmokeRed.addShifted(x, y, 0, hueShift, saturation, brightness);
		smokeTop.scale = scale;
		smokeTop.rotation = Math.random() * 360;
		return smokeTop;
	}

	private addShockwave(x: number, y: number, scale: number, hueShift: number, saturation: number, brightness: number) {
		hueShift = hueShift % 360;
		DiceLayer.numFiltersOnDieCleanup++;
		const shockwave: SpriteProxy = this.diceShockwaveRed.addShifted(x, y, 0, hueShift, saturation, brightness);
		shockwave.scale = scale;
		shockwave.rotation = Math.random() * 360;
		return shockwave;
	}

	addMagicRing(die: IDie, x: number, y: number, scale = 1, hueShift = 0): SpriteProxy {
		hueShift = hueShift % 360;
		let magicRing: SpriteProxy;

		if (DiceLayer.numFiltersOnRoll < DiceLayer.maxFiltersOnRoll) {
			DiceLayer.numFiltersOnRoll++;
			magicRing = this.magicRingRed.addShifted(x, y, 0, hueShift);
		}
		else if (hueShift < 30)
			magicRing = this.magicRingRed.add(x, y, 0);
		else if (hueShift < 50)
			magicRing = this.magicRingGold.add(x, y, 0);
		else if (hueShift < 166)
			magicRing = this.magicRingGreen.add(x, y, 0);
		else if (hueShift < 232)
			magicRing = this.magicRingBlue.add(x, y, 0);
		else if (hueShift < 279)
			magicRing = this.magicRingPurple.add(x, y, 0);
		else
			magicRing = this.magicRingMaroon.add(x, y, 0);

		this.initSprite(magicRing, die);
		magicRing.rotation = Math.random() * 360;
		magicRing.scale = scale;
		return magicRing;
	}

	addLuckyRing(die: IDie, x: number, y: number): SpriteProxy {
		const ring: SpriteProxy = this.cloverRing.add(x, y, Math.floor(Math.random() * 120));
		ring.scale = die.scale;
		this.initSprite(ring, die);
		return ring;
	}

	addBadLuckRing(die: IDie, x: number, y: number): SpriteProxy {
		const badLuckRing: SpriteProxy = this.badLuckRing.add(x, y, Math.floor(Math.random() * 120));
		badLuckRing.scale = die.scale;
		this.initSprite(badLuckRing, die);
		return badLuckRing;
	}

	addFreezeBubble(x: number, y: number, scale: number, hueShift = 0, saturationPercent = -1, brightness = -1): SpriteProxy {
		const freeze: SpriteProxy = this.freeze.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		freeze.rotation = Math.random() * 360;
		freeze.scale = scale;
		freeze.fadeInTime = 500;
		return freeze;
	}

	addFeezePop(x: number, y: number, scale: number, hueShift = 0, saturationPercent = -1, brightness = -1): SpriteProxy {
		const freezePop = this.freezePop.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		freezePop.rotation = Math.random() * 360;
		freezePop.scale = scale;
		return freezePop;
	}

	//addHalo(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
	//	let halo = this.halos.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
	//	halo.rotation = Math.random() * 360;
	//	halo.opacity = 0.9;
	//	halo.fadeInTime = 500;
	//	halo.fadeOutTime = 500;
	//	halo.fadeOnDestroy = true;
	//	halo.autoRotationDegeesPerSecond = 10;
	//	return halo;
	//}

	addHaloSpin(die: IDie, x: number, y: number, scale: number, hueShift: number, angle: number): SpriteProxy {
		let haloSpin: SpriteProxy;

		if (DiceLayer.numFiltersOnRoll < DiceLayer.maxFiltersOnRoll) {
			DiceLayer.numFiltersOnRoll++;
			haloSpin = this.haloSpinRed.addShifted(x, y, 0, hueShift);
		}
		else {
			hueShift = hueShift % 360;
			if (hueShift < 30 || hueShift >= 352)
				haloSpin = this.haloSpinRed.add(x, y, 0);
			else if (hueShift < 58)
				haloSpin = this.haloSpinYellow.add(x, y, 0);
			else if (hueShift < 131)
				haloSpin = this.haloSpinGreen.add(x, y, 0);
			else if (hueShift < 175)
				haloSpin = this.haloSpinTeal.add(x, y, 0);
			else if (hueShift < 213)
				haloSpin = this.haloSpinBlue.add(x, y, 0);
			else if (hueShift < 265)
				haloSpin = this.haloSpinPurple.add(x, y, 0);
			else if (hueShift < 281)
				haloSpin = this.haloSpinLavender.add(x, y, 0);
			else if (hueShift < 311)
				haloSpin = this.haloSpinMaroon.add(x, y, 0);
			else if (hueShift < 352)
				haloSpin = this.haloSpinFuschia.add(x, y, 0);
		}

		this.initSprite(haloSpin, die);
		haloSpin.rotation = angle;
		haloSpin.scale = scale;
		haloSpin.fadeInTime = 500;
		haloSpin.fadeOutTime = 500;
		haloSpin.fadeOnDestroy = true;
		return haloSpin;
	}

	addDamageFire(die: IDie, x: number, y: number, angle: number, hueShift = 0): SpriteProxy {
		let damageFire: SpriteProxy;
		if (hueShift !== 0)
			damageFire = this.damageFire.addShifted(x, y, Sprites.startAtRandomFrame, hueShift);
		else if (Random.chancePercent(50))
			damageFire = this.damageFire.add(x, y, Sprites.startAtRandomFrame);
		else
			damageFire = this.damageFireB.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageFire, die);
		damageFire.rotation = angle;
		damageFire.scale = die.scale;
		damageFire.fadeInTime = 500;
		damageFire.fadeOutTime = 500;
		damageFire.fadeOnDestroy = true;
		return damageFire;
	}

	addDamageCold(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		const damageCold = this.damageCold.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageCold, die);
		damageCold.rotation = angle;
		damageCold.scale = die.scale;
		damageCold.fadeInTime = 500;
		damageCold.fadeOutTime = 500;
		damageCold.fadeOnDestroy = true;
		return damageCold;
	}


	addDamageNecroticHead(die: IDie, x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let damageNecroticHead: SpriteProxy;

		if (Random.chancePercent(50))
			damageNecroticHead = this.damageNecroticHead.add(x, y, Sprites.startAtRandomFrame);
		else
			damageNecroticHead = this.damageNecroticHeadB.add(x, y, Sprites.startAtRandomFrame);

		this.initSprite(damageNecroticHead, die);
		damageNecroticHead.rotation = angle;
		damageNecroticHead.scale = die.scale;
		damageNecroticHead.autoRotationDegeesPerSecond = autoRotation;
		damageNecroticHead.initialRotation = angle;
		damageNecroticHead.fadeInTime = 500;
		damageNecroticHead.fadeOutTime = 500;
		damageNecroticHead.fadeOnDestroy = true;
		return damageNecroticHead;
	}

	addDamageNecroticFog(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		const damageNecroticFog = this.damageNecroticFog.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageNecroticFog, die);
		damageNecroticFog.rotation = angle;
		damageNecroticFog.scale = die.scale;
		damageNecroticFog.fadeInTime = 500;
		damageNecroticFog.fadeOutTime = 500;
		damageNecroticFog.fadeOnDestroy = true;
		return damageNecroticFog;
	}

	addDamageAcid(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		let damageAcid: SpriteProxy;
		if (Random.chancePercent(50))
			damageAcid = this.damageAcid.add(x, y, 0);
		else
			damageAcid = this.damageAcidB.add(x, y, 0);
		damageAcid.scale = die.scale;
		this.initSprite(damageAcid, die);
		damageAcid.rotation = angle;
		damageAcid.fadeInTime = 500;
		damageAcid.fadeOutTime = 500;
		damageAcid.fadeOnDestroy = true;
		return damageAcid;
	}

	addDamageRadiant(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		const damageRadiant = this.damageRadiant.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageRadiant, die);
		damageRadiant.rotation = angle;
		damageRadiant.scale = die.scale;
		damageRadiant.fadeInTime = 500;
		damageRadiant.fadeOutTime = 500;
		damageRadiant.fadeOnDestroy = true;
		return damageRadiant;
	}

	addDamageForce(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		let damageForce: SpriteProxy;
		if (Random.chancePercent(50))
			damageForce = this.damageForce.add(x, y, Sprites.startAtRandomFrame);
		else
			damageForce = this.damageForceB.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageForce, die);
		damageForce.rotation = angle;
		damageForce.scale = die.scale;
		damageForce.autoRotationDegeesPerSecond = 5;
		damageForce.initialRotation = angle;
		damageForce.fadeInTime = 500;
		damageForce.fadeOutTime = 500;
		damageForce.fadeOnDestroy = true;
		return damageForce;
	}

	addDamageBludgeoningMace(die: IDie, x: number, y: number, angle: number, autoRotationDegeesPerSecond: number): SpriteProxy {
		const damageBludgeoningMace = this.damageBludgeoningMace.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageBludgeoningMace, die);
		damageBludgeoningMace.rotation = angle;
		damageBludgeoningMace.scale = die.scale;
		damageBludgeoningMace.autoRotationDegeesPerSecond = autoRotationDegeesPerSecond;
		damageBludgeoningMace.initialRotation = angle;
		damageBludgeoningMace.fadeInTime = 500;
		damageBludgeoningMace.fadeOutTime = 500;
		damageBludgeoningMace.fadeOnDestroy = true;
		return damageBludgeoningMace;
	}


	addDamageBludgeoningFist(die: IDie, x: number, y: number, angle: number, autoRotationDegeesPerSecond: number): SpriteProxy {
		let hueShift = 0;
		if (Random.chancePercent(33))
			hueShift = 180 + Random.plusMinus(30);
		else if (Random.chancePercent(50))
			hueShift = -132 + Random.plusMinus(30);
		const damageBludgeoningFist: SpriteProxy = this.damageBludgeoningFist.addShifted(x, y, Sprites.startAtRandomFrame, hueShift);
		this.initSprite(damageBludgeoningFist, die);
		angle += 90;
		damageBludgeoningFist.scale = 0.8 * die.scale;
		damageBludgeoningFist.rotation = angle;
		damageBludgeoningFist.autoRotationDegeesPerSecond = autoRotationDegeesPerSecond;
		damageBludgeoningFist.initialRotation = angle;
		damageBludgeoningFist.fadeInTime = 500;
		damageBludgeoningFist.fadeOutTime = 500;
		damageBludgeoningFist.fadeOnDestroy = true;
		return damageBludgeoningFist;
	}

	addDamageBludgeoningWeapon(x: number, y: number, scale: number, angle: number, autoRotationDegeesPerSecond: number, startingFrame: number): SpriteProxy {
		const damageBludgeoningWeapon = this.damageBludgeoningWeapon.add(x, y, startingFrame);
		damageBludgeoningWeapon.rotation = angle;
		damageBludgeoningWeapon.scale = scale;
		damageBludgeoningWeapon.autoRotationDegeesPerSecond = autoRotationDegeesPerSecond;
		damageBludgeoningWeapon.initialRotation = angle;
		damageBludgeoningWeapon.fadeInTime = 500;
		damageBludgeoningWeapon.fadeOutTime = 500;
		damageBludgeoningWeapon.fadeOnDestroy = true;
		return damageBludgeoningWeapon;
	}

	addDamageSpinningCloudTrail(die: IDie, x: number, y: number, angle: number, autoRotationDegeesPerSecond: number, startingFrame: number): SpriteProxy {
		const damageSpinningCloudTrail = this.damageSpinningCloudTrail.add(x, y, startingFrame);
		this.initSprite(damageSpinningCloudTrail, die);
		damageSpinningCloudTrail.rotation = angle;
		damageSpinningCloudTrail.scale = die.scale;
		damageSpinningCloudTrail.opacity = 0.5;
		damageSpinningCloudTrail.autoRotationDegeesPerSecond = autoRotationDegeesPerSecond;
		damageSpinningCloudTrail.initialRotation = angle;
		damageSpinningCloudTrail.fadeInTime = 500;
		damageSpinningCloudTrail.fadeOutTime = 500;
		damageSpinningCloudTrail.fadeOnDestroy = true;
		return damageSpinningCloudTrail;
	}

	addDamagePoison(die: IDie, x: number, y: number, angle: number, hueShift: number): SpriteProxy {
		const damagePoison = this.damagePoison.addShifted(x, y, Sprites.startAtRandomFrame, hueShift);
		this.initSprite(damagePoison, die);
		damagePoison.rotation = angle;
		damagePoison.scale = die.scale;
		damagePoison.fadeInTime = 500;
		damagePoison.fadeOutTime = 500;
		damagePoison.fadeOnDestroy = true;
		return damagePoison;
	}

	addDamagePiercingDagger(die: IDie, x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		const damagePiercingDagger = this.damagePiercingDagger.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damagePiercingDagger, die);
		damagePiercingDagger.autoRotationDegeesPerSecond = autoRotation;
		damagePiercingDagger.initialRotation = angle;
		damagePiercingDagger.rotation = angle;
		damagePiercingDagger.scale = die.scale;
		damagePiercingDagger.fadeInTime = 500;
		damagePiercingDagger.fadeOutTime = 500;
		damagePiercingDagger.fadeOnDestroy = true;
		return damagePiercingDagger;
	}

	addDamagePiercingThickSword(die: IDie, x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		const damagePiercingThickSword = this.damagePiercingThickSword.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damagePiercingThickSword, die);
		damagePiercingThickSword.autoRotationDegeesPerSecond = autoRotation;
		damagePiercingThickSword.initialRotation = angle;
		damagePiercingThickSword.rotation = angle;
		damagePiercingThickSword.scale = die.scale;
		damagePiercingThickSword.fadeInTime = 500;
		damagePiercingThickSword.fadeOutTime = 500;
		damagePiercingThickSword.fadeOnDestroy = true;
		return damagePiercingThickSword;
	}

	addDamagePiercingTooth(die: IDie, x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		const damagePiercingTooth = this.damagePiercingTooth.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damagePiercingTooth, die);
		damagePiercingTooth.autoRotationDegeesPerSecond = autoRotation;
		damagePiercingTooth.initialRotation = angle;
		damagePiercingTooth.rotation = angle;
		damagePiercingTooth.scale = die.scale;
		damagePiercingTooth.fadeInTime = 500;
		damagePiercingTooth.fadeOutTime = 500;
		damagePiercingTooth.fadeOnDestroy = true;
		return damagePiercingTooth;
	}

	addDamagePiercingTrident(die: IDie, x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		const damagePiercingTrident = this.damagePiercingTrident.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damagePiercingTrident, die);
		damagePiercingTrident.autoRotationDegeesPerSecond = autoRotation;
		damagePiercingTrident.initialRotation = angle;
		damagePiercingTrident.rotation = angle;
		damagePiercingTrident.scale = die.scale;
		damagePiercingTrident.fadeInTime = 500;
		damagePiercingTrident.fadeOutTime = 500;
		damagePiercingTrident.fadeOnDestroy = true;
		return damagePiercingTrident;
	}

	addDamageSlashingSword(die: IDie, x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		const damageSlashingSword = this.damageSlashingSword.addShifted(x, y, Sprites.startAtRandomFrame, Random.max(360), 30 + Random.max(50));
		this.initSprite(damageSlashingSword, die);
		damageSlashingSword.autoRotationDegeesPerSecond = autoRotation;
		damageSlashingSword.initialRotation = angle;
		damageSlashingSword.rotation = angle;
		damageSlashingSword.scale = die.scale;
		damageSlashingSword.fadeInTime = 500;
		damageSlashingSword.fadeOutTime = 500;
		damageSlashingSword.fadeOnDestroy = true;
		return damageSlashingSword;
	}


	addDamageSlashingAx(die: IDie, x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		const damageSlashingAx = this.damageSlashingAx.addShifted(x, y, Sprites.startAtRandomFrame, Random.max(360), 30 + Random.max(50));
		this.initSprite(damageSlashingAx, die);
		damageSlashingAx.autoRotationDegeesPerSecond = autoRotation;
		damageSlashingAx.initialRotation = angle;
		damageSlashingAx.rotation = angle;
		damageSlashingAx.scale = die.scale;
		damageSlashingAx.fadeInTime = 500;
		damageSlashingAx.fadeOutTime = 500;
		damageSlashingAx.fadeOnDestroy = true;
		return damageSlashingAx;
	}

	addDamageSlashingLance(die: IDie, x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		const damageSlashingLance = this.damageSlashingLance.addShifted(x, y, Sprites.startAtRandomFrame, Random.max(360), 20 + Random.max(50));
		this.initSprite(damageSlashingLance, die);
		damageSlashingLance.autoRotationDegeesPerSecond = autoRotation;
		damageSlashingLance.initialRotation = angle;
		damageSlashingLance.rotation = angle;
		damageSlashingLance.scale = die.scale;
		damageSlashingLance.fadeInTime = 500;
		damageSlashingLance.fadeOutTime = 500;
		damageSlashingLance.fadeOnDestroy = true;
		return damageSlashingLance;
	}

	addHealth(die: IDie, x: number, y: number, scale: number = 1): SpriteProxy {
		let health: SpriteProxy;
		if (Random.chancePercent(50))
			health = this.health.add(x, y, Sprites.startAtRandomFrame);
		else
			health = this.healthB.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(health, die);
		health.scale = scale;
		health.fadeInTime = 500;
		health.fadeOutTime = 500;
		health.fadeOnDestroy = true;
		return health;
	}

	addDamageThunder(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		const damageThunder = this.damageThunder.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageThunder, die);
		damageThunder.rotation = angle;
		damageThunder.scale = die.scale;
		damageThunder.fadeInTime = 500;
		damageThunder.fadeOutTime = 500;
		damageThunder.fadeOnDestroy = true;
		return damageThunder;
	}

	addSuperiorityFire(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		const superiorityFire = this.superiorityFire.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(superiorityFire, die);
		superiorityFire.rotation = angle;
		superiorityFire.scale = die.scale;
		superiorityFire.fadeInTime = 500;
		superiorityFire.fadeOutTime = 500;
		superiorityFire.fadeOnDestroy = true;
		return superiorityFire;
	}

	addSuperiorityDragonHead(die: IDie, x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let superiorityDragonHead: SpriteProxy;
		if (Random.chancePercent(25))
			superiorityDragonHead = this.superiorityDragonHead.add(x, y, Sprites.startAtRandomFrame);
		else if (Random.chancePercent(33))
			superiorityDragonHead = this.superiorityDragonHeadB.add(x, y, Sprites.startAtRandomFrame);
		else if (Random.chancePercent(50))
			superiorityDragonHead = this.superiorityDragonHeadC.add(x, y, Sprites.startAtRandomFrame);
		else
			superiorityDragonHead = this.superiorityDragonHeadD.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(superiorityDragonHead, die);
		superiorityDragonHead.rotation = angle;
		superiorityDragonHead.scale = die.scale;
		superiorityDragonHead.initialRotation = angle;
		superiorityDragonHead.autoRotationDegeesPerSecond = autoRotation;
		superiorityDragonHead.fadeInTime = 500;
		superiorityDragonHead.fadeOutTime = 500;
		superiorityDragonHead.fadeOnDestroy = true;
		return superiorityDragonHead;
	}

	//addSuperiorityDragonMouth(x: number, y: number, startingFrameIndex: number, angle: number, autoRotation: number): SpriteProxy {
	//	let superiorityMouth = this.superiorityDragonMouth.add(x, y, startingFrameIndex);
	//	superiorityMouth.rotation = angle;
	//	superiorityMouth.initialRotation = angle;
	//	superiorityMouth.autoRotationDegeesPerSecond = autoRotation;
	//	superiorityMouth.fadeInTime = 500;
	//	superiorityMouth.fadeOutTime = 500;
	//	superiorityMouth.fadeOnDestroy = true;
	//	return superiorityMouth;
	//}

	addDamagePsychic(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		let damagePsychic: SpriteProxy;
		if (Random.chancePercent(50))
			damagePsychic = this.damagePsychic.add(x, y, Sprites.startAtRandomFrame);
		else
			damagePsychic = this.damagePsychicB.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damagePsychic, die);
		damagePsychic.rotation = angle;
		damagePsychic.scale = die.scale;
		damagePsychic.autoRotationDegeesPerSecond = Random.plusMinusBetween(5, 25);
		damagePsychic.initialRotation = angle;
		damagePsychic.fadeInTime = 500;
		damagePsychic.fadeOutTime = 500;
		damagePsychic.fadeOnDestroy = true;
		return damagePsychic;
	}


	addDamageLightningA(die: IDie, x: number, y: number, angle: number, autoRotateDegreesPerSecond: number): SpriteProxy {
		const damageLightningA = this.damageLightningA.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageLightningA, die);
		damageLightningA.rotation = angle + Random.plusMinus(10);
		damageLightningA.scale = die.scale;
		damageLightningA.autoRotationDegeesPerSecond = autoRotateDegreesPerSecond;
		damageLightningA.initialRotation = damageLightningA.rotation;
		damageLightningA.fadeInTime = 500;
		damageLightningA.fadeOutTime = 500;
		damageLightningA.fadeOnDestroy = true;
		return damageLightningA;
	}


	addDamageLightningB(die: IDie, x: number, y: number, angle: number, autoRotateDegreesPerSecond: number): SpriteProxy {
		const damageLightningB = this.damageLightningB.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageLightningB, die);
		damageLightningB.rotation = angle + Random.plusMinus(10);
		damageLightningB.scale = die.scale;
		damageLightningB.autoRotationDegeesPerSecond = autoRotateDegreesPerSecond;
		damageLightningB.initialRotation = damageLightningB.rotation;
		damageLightningB.fadeInTime = 500;
		damageLightningB.fadeOutTime = 500;
		damageLightningB.fadeOnDestroy = true;
		return damageLightningB;
	}

	addDamageLightningC(die: IDie, x: number, y: number, angle: number, autoRotateDegreesPerSecond: number): SpriteProxy {
		const damageLightningC = this.damageLightningC.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageLightningC, die);
		damageLightningC.rotation = angle + Random.plusMinus(10);
		damageLightningC.scale = die.scale;
		damageLightningC.autoRotationDegeesPerSecond = autoRotateDegreesPerSecond;
		damageLightningC.initialRotation = damageLightningC.rotation;
		damageLightningC.fadeInTime = 500;
		damageLightningC.fadeOutTime = 500;
		damageLightningC.fadeOnDestroy = true;
		return damageLightningC;
	}

	addDamageLightningCloud(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		const damageLightningCloud = this.damageLightningCloud.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(damageLightningCloud, die);
		damageLightningCloud.rotation = angle;
		damageLightningCloud.scale = die.scale;
		damageLightningCloud.fadeInTime = 500;
		damageLightningCloud.fadeOutTime = 500;
		damageLightningCloud.fadeOnDestroy = true;
		return damageLightningCloud;
	}


	addInspirationParticles(die: IDie, x: number, y: number, rotationDegeesPerSecond: number, hueShift = 0): SpriteProxy {
		const inspirationParticlesProxy = this.inspirationParticles.addShifted(x, y, 0, hueShift);
		this.initSprite(inspirationParticlesProxy, die);
		inspirationParticlesProxy.fadeInTime = 500;
		inspirationParticlesProxy.fadeOutTime = 500;
		inspirationParticlesProxy.fadeOnDestroy = true;
		inspirationParticlesProxy.autoRotationDegeesPerSecond = rotationDegeesPerSecond;
		inspirationParticlesProxy.initialRotation = Math.random() * 360;
		inspirationParticlesProxy.scale = die.scale;
		return inspirationParticlesProxy;
	}

	addInspirationSmoke(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		const inspirationSmokeProxy = this.inspirationSmoke.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(inspirationSmokeProxy, die);
		inspirationSmokeProxy.rotation = angle;
		inspirationSmokeProxy.scale = die.scale;
		inspirationSmokeProxy.fadeInTime = 500;
		inspirationSmokeProxy.fadeOutTime = 1000;
		inspirationSmokeProxy.fadeOnDestroy = true;
		inspirationSmokeProxy.expirationDate = performance.now() + 3000;
		return inspirationSmokeProxy;
	}

	addRipple(x: number, y: number, scale: number, angle: number, hueShift = 0): ColorShiftingSpriteProxy {
		const rippleProxy = this.ripples.addShifted(x, y, 0, hueShift);
		rippleProxy.rotation = angle;
		rippleProxy.scale = scale;
		return rippleProxy;
	}

	// TODO: Remove?
	addRaven(x: number, y: number, scale: number, angle: number) {
		const ravens: Sprites = this.getRavens();
		const raven = ravens.addShifted(x, y, 0, this.activePlayerHueShift + Random.plusMinus(35));
		raven.rotation = angle + 180 + Random.plusMinus(45);
		raven.scale = scale;
		return raven;
	}

	getRavens(): Sprites {
		const index: number = Math.floor(Math.random() * this.ravens.length);
		return this.ravens[index];
	}

	getFireTrails(): Sprites {
		const index: number = Math.floor(Math.random() * this.fireTrails.length);
		return this.fireTrails[index];
	}

	getFrostTrails(): Sprites {
		const index: number = Math.floor(Math.random() * this.frostTrails.length);
		return this.frostTrails[index];
	}

	blowColoredSmoke(x: number, y: number, scale = 1, hueShift = 0, saturationPercent = -1, brightness = -1) {
		const coloredSmoke: SpriteProxy = this.diceBlowColoredSmoke.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		coloredSmoke.rotation = Math.random() * 360;
		coloredSmoke.scale = scale;
	}

	//addD20Fire(x: number, y: number) {
	//  this.d20Fire.add(x, y).rotation = Math.random() * 360;
	//}

	// TODO: Remove?
	addSmoke(x: number, y: number, angle: number) {
		const hueShift = this.activePlayerHueShift + Random.plusMinus(10);
		let angleShift = 0;
		if (Math.random() > 0.5)
			angleShift = 180;
		this.smoke.addShifted(x, y, 0, hueShift).rotation = angle + angleShift;
	}

	// TODO: Remove?
	addSparkTrail(x: number, y: number, scale: number, angle: number) {
		const spark: SpriteProxy = this.sparkTrail.addShifted(x, y, 0, this.activePlayerHueShift + Random.plusMinus(35));
		spark.rotation = angle + 180 + Random.plusMinus(15);
		spark.scale = scale;
	}

	clearResidualEffects(diceGroup: DiceGroup): void {
		this.clearSprites(diceGroup);
		this.clearTextEffects(diceGroup);
	}

	clearSprites(diceGroup: DiceGroup) {
		//console.log(`this.clearSpritesByGroup - ${DiceGroup[diceGroup]}`);
		this.clearSpritesByGroup(this.cloverRing, diceGroup);
		this.clearSpritesByGroup(this.magicRingRed, diceGroup);
		this.clearSpritesByGroup(this.magicRingGold, diceGroup);
		this.clearSpritesByGroup(this.magicRingGreen, diceGroup);
		this.clearSpritesByGroup(this.magicRingBlue, diceGroup);
		this.clearSpritesByGroup(this.magicRingPurple, diceGroup);
		this.clearSpritesByGroup(this.magicRingMaroon, diceGroup);
		this.clearSpritesByGroup(this.badLuckRing, diceGroup);
		this.clearSpritesByGroup(this.freeze, diceGroup);
		this.clearSpritesByGroup(this.haloSpinRed, diceGroup);
		this.clearSpritesByGroup(this.haloSpinBlue, diceGroup);
		this.clearSpritesByGroup(this.haloSpinFuschia, diceGroup);
		this.clearSpritesByGroup(this.haloSpinGreen, diceGroup);
		this.clearSpritesByGroup(this.haloSpinLavender, diceGroup);
		this.clearSpritesByGroup(this.haloSpinMaroon, diceGroup);
		this.clearSpritesByGroup(this.haloSpinPurple, diceGroup);
		this.clearSpritesByGroup(this.haloSpinTeal, diceGroup);
		this.clearSpritesByGroup(this.haloSpinYellow, diceGroup);
		this.clearSpritesByGroup(this.damageFire, diceGroup);
		this.clearSpritesByGroup(this.damageFireB, diceGroup);
		this.clearSpritesByGroup(this.damageCold, diceGroup);
		this.clearSpritesByGroup(this.damageNecroticHead, diceGroup);
		this.clearSpritesByGroup(this.damageNecroticHeadB, diceGroup);
		this.clearSpritesByGroup(this.damageNecroticFog, diceGroup);
		this.clearSpritesByGroup(this.damageAcid, diceGroup);
		this.clearSpritesByGroup(this.damageAcidB, diceGroup);
		this.clearSpritesByGroup(this.topDieCloudRed, diceGroup);
		this.clearSpritesByGroup(this.topDieCloudWhite, diceGroup);
		this.clearSpritesByGroup(this.topDieCloudBlack, diceGroup);
		this.clearSpritesByGroup(this.topDieCloudBlue, diceGroup);
		this.clearSpritesByGroup(this.topDieCloudPurple, diceGroup);
		this.clearSpritesByGroup(this.topDieCloudGreen, diceGroup);
		this.clearSpritesByGroup(this.damageRadiant, diceGroup);
		this.clearSpritesByGroup(this.damageForce, diceGroup);
		this.clearSpritesByGroup(this.damageForceB, diceGroup);
		this.clearSpritesByGroup(this.damageBludgeoningMace, diceGroup);
		this.clearSpritesByGroup(this.damageBludgeoningFist, diceGroup);
		this.clearSpritesByGroup(this.damageBludgeoningWeapon, diceGroup);
		this.clearSpritesByGroup(this.damageSpinningCloudTrail, diceGroup);
		this.clearSpritesByGroup(this.damagePoison, diceGroup);
		this.clearSpritesByGroup(this.damagePiercingDagger, diceGroup);
		this.clearSpritesByGroup(this.damagePiercingThickSword, diceGroup);
		this.clearSpritesByGroup(this.damagePiercingTooth, diceGroup);
		this.clearSpritesByGroup(this.damagePiercingTrident, diceGroup);
		this.clearSpritesByGroup(this.damageSlashingSword, diceGroup);
		this.clearSpritesByGroup(this.damageSlashingAx, diceGroup);
		this.clearSpritesByGroup(this.damageSlashingLance, diceGroup);
		this.clearSpritesByGroup(this.damageThunder, diceGroup);
		this.clearSpritesByGroup(this.baneBack, diceGroup);
		this.clearSpritesByGroup(this.blessBack, diceGroup);
		this.clearSpritesByGroup(this.baneFront, diceGroup);
		this.clearSpritesByGroup(this.blessFront, diceGroup);
		this.clearSpritesByGroup(this.superiorityFire, diceGroup);
		this.clearSpritesByGroup(this.superiorityDragonHead, diceGroup);
		this.clearSpritesByGroup(this.superiorityDragonHeadB, diceGroup);
		this.clearSpritesByGroup(this.superiorityDragonHeadC, diceGroup);
		this.clearSpritesByGroup(this.superiorityDragonHeadD, diceGroup);
		this.clearSpritesByGroup(this.health, diceGroup);
		this.clearSpritesByGroup(this.healthB, diceGroup);
		this.clearSpritesByGroup(this.damagePsychic, diceGroup);
		this.clearSpritesByGroup(this.damagePsychicB, diceGroup);
		this.clearSpritesByGroup(this.damageLightningA, diceGroup);
		this.clearSpritesByGroup(this.damageLightningB, diceGroup);
		this.clearSpritesByGroup(this.damageLightningC, diceGroup);
		this.clearSpritesByGroup(this.damageLightningCloud, diceGroup);
		this.clearSpritesByGroup(this.freezePop, diceGroup);
		this.clearSpritesByGroup(this.inspirationParticles, diceGroup);
		//this.clearSpritesByGroup(this.halos.sprites, diceGroup);
		//this.clearSpritesByGroup(this.stars.sprites, diceGroup);
		//this.clearSpritesByGroup(this.d20Fire.sprites, diceGroup);
	}

	clearSpritesByGroup(sprites: Sprites, diceGroup: DiceGroup) {
		if (sprites.animationStyle !== AnimationStyle.Loop)
			return;
		sprites.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof DieSpriteData) {
				if (sprite.data.group === diceGroup)
					sprite.fadeOutNow(250);
			}
		});
	}

	clearTextEffects(diceGroup: DiceGroup) {
		this.animations.animationProxies.forEach((element: AnimatedElement) => {
			if (!element.data || element.data === diceGroup)
				element.fadeOutNow(250);
		});
	}

	rgbToHSL(rgb) {
		// strip the leading # if it's there
		rgb = rgb.replace(/^\s*#|\s*$/g, '');

		// convert 3 char codes --> 6, e.g. `E0F` --> `EE00FF`
		if (rgb.length === 3) {
			rgb = rgb.replace(/(.)/g, '$1$1');
		}

		const r = parseInt(rgb.substr(0, 2), 16) / 255,
			g = parseInt(rgb.substr(2, 2), 16) / 255,
			b = parseInt(rgb.substr(4, 2), 16) / 255,
			cMax = Math.max(r, g, b),
			cMin = Math.min(r, g, b),
			delta = cMax - cMin,
			l = (cMax + cMin) / 2;

		let h = 0, s = 0;

		if (delta === 0) {
			h = 0;
		}
		else if (cMax === r) {
			h = 60 * (((g - b) / delta) % 6);
		}
		else if (cMax === g) {
			h = 60 * (((b - r) / delta) + 2);
		}
		else {
			h = 60 * (((r - g) / delta) + 4);
		}

		if (delta === 0) {
			s = 0;
		}
		else {
			s = (delta / (1 - Math.abs(2 * l - 1)))
		}

		return {
			h: h,
			s: s,
			l: l
		}
	}

	addGroundBurst(die: IDie, screenPos: Vector, dieObject: IDieObject) {
		let hueShift = 0;
		const hsl = this.rgbToHSL(die.diceColor);
		let saturation = 100;
		let brightness = 100;
		if (hsl) {
			hueShift = hsl.h;
			saturation = hsl.s * 100;
			brightness = (hsl.l - 0.5) * 100 + 100;
		}
		else if (die.creatureID >= 0)
			hueShift = this.getHueShift(die.creatureID);
		this.addGroundBurstAnimation(screenPos.x, screenPos.y, die.scale, hueShift, saturation, brightness);
		diceSounds.playDieBurst();
		hideDieIn(dieObject, 100);
	}

	addSmokeyPortal(die: IDie, screenPos: Vector, dieObject: IDieObject) {
		let hueShift = 0;
		const hsl = this.rgbToHSL(die.diceColor);
		let saturation = 100;
		const brightness = 100;
		if (hsl) {
			hueShift = hsl.h;
			saturation = hsl.s * 100;
			//brightness = (hsl.l - 0.5) * 100 + 100;
		}
		else if (die.creatureID >= 0)
			hueShift = this.getHueShift(die.creatureID);
		this.addSmokeyPortalAnimation(screenPos.x, screenPos.y, die.scale, hueShift + Random.plusMinus(40), saturation, brightness);
		diceSounds.playSmokyPortal();
		hideDieIn(dieObject, 80 * fps30);
	}


	addSteampunkTunnel(x: number, y: number, scale = 1, hueShift = 0, saturationPercent = -1, brightness = -1) {
		// no rotation on SteampunkTunnel - shadows expect light source from above.

		if (DiceLayer.numFiltersOnDieCleanup < DiceLayer.maxFiltersOnDieCleanup) {
			DiceLayer.numFiltersOnDieCleanup++;
			this.dieSteampunkTunnel.addShifted(x, y, 0, hueShift, saturationPercent, brightness).scale = scale;
		}
		else {
			this.dieSteampunkTunnel.add(x, y, 0).scale = scale;
		}
	}

	// TODO: Remove?
	addSneakAttack(x: number, y: number, scale: number, hueShift = 0, saturationPercent = -1, brightness = -1) {
		const rotation: number = Math.random() * 360;
		this.smokeExplosionBottom.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = rotation;
		this.smokeExplosionTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = rotation;
	}

	addDiceBomb(x: number, y: number, scale = 1, hueShift = 0, saturationPercent = -1, brightness = -1) {
		const rotation: number = Math.random() * 360;
		const bottomSprite: SpriteProxy = this.diceBombBase.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		bottomSprite.rotation = rotation;
		bottomSprite.scale = scale;
		const topSprite: SpriteProxy = this.diceBombTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		topSprite.rotation = rotation;
		topSprite.scale = scale;
		//this.diceBombBase.add(x, y, 0).rotation = Math.random() * 360;
		//this.diceBombTop.add(x, y, 0).rotation = Math.random() * 360;
	}

	addPortal(x: number, y: number, scale = 1, hueShift = 0, saturationPercent = -1, brightness = -1) {
		if (DiceLayer.numFiltersOnDieCleanup < DiceLayer.maxFiltersOnDieCleanup) {
			DiceLayer.numFiltersOnDieCleanup += 2;
			this.dicePortal.addShifted(x, y, 0, hueShift, saturationPercent, brightness).scale = scale;
			this.dicePortalTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).scale = scale;
		}
		else {
			this.dicePortal.add(x, y, 0).scale = scale;
			this.dicePortalTop.add(x, y, 0).scale = scale;
		}
	}

	grabDiceWithHand(x: number, y: number, scale = 1, hueShift = 0, saturationPercent = -1, brightness = -1) {
		const handRotation: number = 90 - Math.random() * 180;
		const grabLower: SpriteProxy = this.handGrabsDiceBottom.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		grabLower.rotation = handRotation;
		grabLower.scale = scale;
		const grabHigher: SpriteProxy = this.handGrabsDiceTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
		grabHigher.rotation = handRotation;
		grabHigher.scale = scale;
	}

	getDiceRollData(diceRollData: string): DiceRollData {
		const dto = JSON.parse(diceRollData);
		const diceRoll: DiceRollData = new DiceRollData();
		diceRoll.type = dto.Type;

		// TODO: Keep this in sync with changes in DiceRoll.cs
		diceRoll.diceGroup = dto.DiceGroup;
		diceRoll.rollId = dto.RollID;
		diceRoll.spellId = dto.SpellID;
		diceRoll.viewer = dto.Viewer;
		diceRoll.singleOwnerId = dto.SingleOwnerId;
		diceRoll.secondRollTitle = dto.SecondRollTitle;
		diceRoll.vantageKind = dto.VantageKind;
		diceRoll.damageHealthExtraDice = dto.DamageHealthExtraDice;
		diceRoll.diceDtos = dto.DiceDtos;
		diceRoll.modifier = dto.Modifier;
		diceRoll.cardModifiers = dto.CardModifiers;
		diceRoll.hiddenThreshold = dto.HiddenThreshold;
		diceRoll.isMagic = dto.IsMagic;
		diceRoll.suppressLegacyRoll = dto.SuppressLegacyRoll;
		diceRoll.spellName = dto.SpellName;
		diceRoll.onThrowSound = dto.OnThrowSound;
		diceRoll.throwPower = dto.ThrowPower;
		diceRoll.onFirstContactSound = dto.OnFirstContactSound;
		diceRoll.dieTotalMessage = dto.DieTotalMessage;
		diceRoll.textOutlineColor = dto.TextOutlineColor;
		diceRoll.textFillColor = dto.TextFillColor;
		diceRoll.onFirstContactEffect = dto.OnFirstContactEffect;
		diceRoll.onRollSound = dto.OnRollSound;
		diceRoll.minCrit = dto.MinCrit;
		diceRoll.groupInspiration = dto.GroupInspiration;
		diceRoll.successMessage = dto.SuccessMessage;
		diceRoll.failMessage = dto.FailMessage;
		diceRoll.additionalDiceOnHit = dto.AdditionalDiceOnHit;
		diceRoll.additionalDiceOnHitMessage = dto.AdditionalDiceOnHitMessage;
		diceRoll.critFailMessage = dto.CritFailMessage;
		diceRoll.critSuccessMessage = dto.CritSuccessMessage;
		diceRoll.numHalos = dto.NumHalos;
		diceRoll.rollScope = dto.RollScope;
		diceRoll.individualFilter = dto.IndividualFilter;
		diceRoll.skillCheck = dto.SkillCheck;
		diceRoll.damageType = dto.DamageType;
		diceRoll.savingThrow = dto.SavingThrow;
		diceRoll.minDamage = dto.MinDamage;
		diceRoll.effectBrightness = dto.EffectBrightness;
		diceRoll.effectScale = dto.EffectScale;
		diceRoll.effectRotation = dto.EffectRotation;
		diceRoll.effectSaturation = dto.EffectSaturation;
		diceRoll.effectHueShift = dto.EffectHueShift;
		diceRoll.onStopRollingSound = dto.OnStopRollingSound;

		for (let i = 0; i < dto.TrailingEffects.length; i++) {
			diceRoll.trailingEffects.push(new TrailingEffect(dto.TrailingEffects[i]));
		}

		for (let i = 0; i < dto.PlayerRollOptions.length; i++) {
			diceRoll.playerRollOptions.push(new PlayerRollOptions(dto.PlayerRollOptions[i]));
		}

		if (diceRoll.throwPower < 0.2)
			diceRoll.throwPower = 0.2;
		if (diceRoll.throwPower > 4.0)
			diceRoll.throwPower = 4.0;
		return diceRoll;
	}

	smallSpark(die: IDie, x: number, y: number, angle = -1): SpriteProxy {
		if (angle === -1)
			angle = Math.random() * 360;
		//let spark = this.diceSparks.addShifted(x, y, Math.round(Math.random() * this.diceSparks.sprites.length), Math.random() * 360);
		const spark = this.diceSparks.add(x, y, Sprites.startAtRandomFrame);
		this.initSprite(spark, die);
		spark.expirationDate = performance.now() + 500;
		spark.fadeOutTime = 0;
		spark.scale = 0.8;
		spark.autoScaleFactorPerSecond = 0.05;
		spark.opacity = 1;
		spark.rotation = angle;
		spark.scale = die.scale;
		return spark;
	}

	// TODO: remove this? No one seems to be calling.
	addSpiral(x: number, y: number, angle: number): SpriteProxy {
		const spiral = this.spirals.addShifted(x, y, 0, diceLayer.activePlayerHueShift + Random.plusMinus(20));
		spiral.rotation = Random.between(0, 360);
		return spiral;
	}

	addFangs(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		const fangs = this.fangs.addShifted(x, y, Math.round(Math.random() * this.fangs.spriteProxies.length), diceLayer.activePlayerHueShift + Random.plusMinus(20));
		this.initSprite(fangs, die);
		fangs.rotation = angle;
		fangs.scale = die.scale;
		fangs.expirationDate = performance.now() + 4000;
		fangs.fadeOutTime = 2000;
		fangs.fadeInTime = 500;
		fangs.opacity = 0.9;
		return fangs;
	}

	addPawPrint(die: IDie, x: number, y: number, angle: number): SpriteProxy {
		const pawPrint = this.pawPrints.addShifted(x, y, Math.round(Math.random() * this.pawPrints.spriteProxies.length), diceLayer.activePlayerHueShift + Random.plusMinus(20));
		pawPrint.rotation = angle;
		pawPrint.scale = die.scale;
		pawPrint.expirationDate = performance.now() + 4000;
		pawPrint.fadeOutTime = 2000;
		pawPrint.fadeInTime = 500;
		pawPrint.opacity = 0.9;
		return pawPrint;
	}

	addTrailingEffectFrom(die: IDie, sprites: Sprites, trailingEffect: TrailingEffect, x: number, y: number, angle: number, spriteScale: number): SpriteProxy {
		if (sprites.name === 'Spark')
			return this.smallSpark(die, x, y, angle);

		let hue = 0;
		if (trailingEffect.HueShift === "player")
			hue = diceLayer.activePlayerHueShift;
		else if (trailingEffect.HueShift)
			hue = +trailingEffect.HueShift;

		if (trailingEffect.HueShiftRandom !== 0)
			hue += Random.plusMinus(trailingEffect.HueShiftRandom);
		hue = Math.round(hue);

		if (sprites.name === 'HaloSpin')
			return this.addHaloSpin(die, x, y, die.scale, hue, angle);

		let index = 0;
		if (trailingEffect.StartIndex === Sprites.startAtRandomFrame)
			index = Math.round(Math.random() * sprites.spriteProxies.length);
		else
			index = trailingEffect.StartIndex;

		const effect: ColorShiftingSpriteProxy = sprites.addShifted(x, y, index, hue);
		effect.scale = die.scale;
		if (sprites.animationStyle === AnimationStyle.Loop) {
			this.initSprite(effect, die);
		}
		effect.rotation = angle + trailingEffect.RotationOffset + Random.plusMinus(trailingEffect.RotationOffsetRandom);
		if (trailingEffect.Lifespan)
			effect.expirationDate = performance.now() + trailingEffect.Lifespan;
		effect.fadeOutTime = trailingEffect.FadeOut;
		effect.fadeInTime = trailingEffect.FadeIn;
		effect.opacity = trailingEffect.Opacity;
		effect.brightness = Math.round(trailingEffect.Brightness);
		effect.saturationPercent = Math.round(trailingEffect.Saturation);
		effect.scale = spriteScale;
		return effect;
	}

	AddTrailingEffect(die: IDie, trailingEffect: TrailingEffect, x: number, y: number, angle: number, spriteScale: number): SpriteProxy {
		let result: SpriteProxy;
		let sprites: Sprites = this.allBackLayerEffects.getSpritesByName(trailingEffect.EffectType);

		if (sprites)
			result = this.addTrailingEffectFrom(die, sprites, trailingEffect, x, y, angle, spriteScale);

		if (trailingEffect.EffectType === "GroundBurstTrail") {
			result.fadeOutTime = 500;
			result.expirationDate = performance.now() + 5000;
		}

		if (trailingEffect.EffectType === 'Raven') {
			sprites = this.getRavens();
			result = this.addTrailingEffectFrom(die, sprites, trailingEffect, x, y, angle, spriteScale);
		}
		else if (trailingEffect.EffectType === 'FireTrails') {
			sprites = this.getFireTrails();
			result = this.addTrailingEffectFrom(die, sprites, trailingEffect, x, y, angle, spriteScale);
		}
		else if (trailingEffect.EffectType === 'FrostTrails') {
			sprites = this.getFrostTrails();
			result = this.addTrailingEffectFrom(die, sprites, trailingEffect, x, y, angle, spriteScale);
			result.fadeInTime = 500;
			result.fadeOutTime = 500;
			result.expirationDate = performance.now() + 5000;
		}

		sprites = this.allFrontLayerEffects.getSpritesByName(trailingEffect.EffectType);

		if (sprites)
			result = this.addTrailingEffectFrom(die, sprites, trailingEffect, x, y, angle, spriteScale);

		return result;
	}

	AddEffect(die: IDie, effectName: string, x: number, y: number, scale: number, hueShift = '0', saturation = 100, brightness = 100, angle = 0): SpriteProxy {
		let result: SpriteProxy;
		let hue: number;
		if (hueShift === 'player')
			hue = this.activePlayerHueShift;
		else
			hue = +hueShift;

		if (angle === -1)
			angle = Random.max(360);

		let sprites: Sprites = this.allBackLayerEffects.getSpritesByName(effectName);
		if (sprites)
			result = this.AddEffectFrom(die, sprites, x, y, scale, hue, saturation, brightness, angle);

		sprites = this.allFrontLayerEffects.getSpritesByName(effectName);

		if (sprites)
			result = this.AddEffectFrom(die, sprites, x, y, scale, hue, saturation, brightness, angle);

		return result;
	}

	AddEffectFrom(die: IDie, sprites: Sprites, x: number, y: number, scale: number, hueShift: number, saturation = 100, brightness = 100, angle = 0): SpriteProxy {
		const effect: ColorShiftingSpriteProxy = sprites.addShifted(x, y, 0, hueShift);
		if (sprites.animationStyle === AnimationStyle.Loop) {
			this.initSprite(effect, die);
		}
		effect.rotation = angle;
		effect.brightness = Math.round(brightness);
		effect.saturationPercent = Math.round(saturation);
		effect.scale = scale;
		return effect;
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

	clearDice(diceGroup: string): void {
		if (this.isPlayerGroup(diceGroup))
			diceRollerPlayers.removeRemainingDice();
		else
			diceRollerViewers.removeRemainingDice();
		//if (!removeRemainingDice())
		//	allDiceHaveBeenDestroyed(JSON.stringify(lastRollDiceData));
	}

	private isPlayerGroup(diceGroup: string) {
		return diceGroup === "Players";
	}

	rollDice(diceRollDto: string): void {
		const diceRollData: DiceRollData = this.getDiceRollData(diceRollDto);
		console.log(diceRollData);
		if (diceRollData.diceGroup === DiceGroup.Players)
			diceRollerPlayers.pleaseRollDice(diceRollData);
		else
			diceRollerViewers.pleaseRollDice(diceRollData);
	}

	getDieColor(playerID: number): string {
		const player: Character = this.getPlayer(playerID);
		if (player)
			return player.dieBackColor;

		return '#000000';
	}

	getDieFontColor(playerID: number): string {
		const player: Character = this.getPlayer(playerID);
		if (player)
			return player.dieFontColor;

		return '#ffffff';
	}

	getPlayer(playerID: number): Character {
		if (playerID < 0)
			return null;

		for (let i = 0; i < this.players.length; i++) {
			const player: Character = this.players[i];
			if (player.IntId === playerID)
				return player;
		}

		return null;
	}

	getPlayerName(playerID: number): string {
		const player: Character = this.getPlayer(playerID);
		if (player)
			return player.getFirstName();

		return '';
	}

	getHueShift(playerID: number): number {
		const player: Character = this.getPlayer(playerID);
		if (player)
			return player.hueShift;

		return 0;
	}

	playerChanged(playerID: number): void {
		this.playerID = playerID;
		//this.activePlayerDieFontColor = '#ffffff';

		this.activePlayerDieColor = this.getDieColor(playerID);
		this.activePlayerDieFontColor = this.getDieFontColor(playerID);
		this.activePlayerHueShift = this.getHueShift(playerID);
	}

	showSmallerMessageAt(centerDicePos: Vector, message: string, diceGroup: DiceGroup, fontColor: string, outlineColor: string) {
		console.log(`showSmallerMessageAt(${centerDicePos}, ${message}`);
		const textEffect: TextEffect = this.animations.addText(centerDicePos, message, this.totalDamageTime);
		textEffect.data = diceGroup;
		textEffect.targetScale = 3;
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.elasticIn = true;
		textEffect.scale = 3;
		textEffect.velocityY = -0.15;
		if (centerDicePos.x < 960)
			textEffect.velocityX = 0.25;
		else
			textEffect.velocityX = -0.25;

		textEffect.opacity = 0.90;

		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}
}

class IndividualRoll {
	constructor(public value: number, public numSides: number, public type: string, public damageType: DamageType, public dieCountsAs: DieCountsAs, public creatureId: number, public modifier: number) {
		if (creatureId === Number.MIN_VALUE)
			creatureId = Character.invalidCreatureId;
	}
}

class DiceDto {
	Quantity: number;
	ScoreMultiplier = 1;
	Scale: number;
	Sides: number;
	CreatureId: number;
	IsMagic: boolean;
	Modifier: number;
	Label: string;
	PlayerName: string;
	Data: string;
	Vantage: VantageKind;
	DamageType: DamageType;
	DieCountsAs: DieCountsAs;
	BackColor: string;
	FontColor: string;
}

class BonusRoll {
	isMagic = false;
	dieCountsAs: DieCountsAs = DieCountsAs.bonus;
	constructor(public diceStr: string, public description: string, public playerID: number, public dieBackColor: string = DiceLayer.bonusRollDieColor, public dieTextColor: string = DiceLayer.bonusRollFontColor, public playerName = '') {

	}
}

class CardModifier {
	CreatureId = Character.invalidCreatureId;
	BlameName: string;
	Offset = 0;
	Multiplier = 1;
	constructor() {

	}
}

class DiceRollData {
	diceGroup: DiceGroup;
	rollId: string;
	spellId: string;
	viewer: string;
	singleOwnerId: number;
	type: DiceRollType;
	vantageKind: VantageKind;
	damageHealthExtraDice: string;
	diceDtos: Array<DiceDto> = [];
	modifier: number;
	cardModifiers: Array<CardModifier> = [];
	minCrit: number;
	totalRoll: number;
	hiddenThreshold: number;
	isMagic: boolean;
	suppressLegacyRoll: boolean;
	spellName: string;
	onThrowSound: string;
	throwPower: number;
	itsAD20Roll: boolean;
	trailingEffects: Array<TrailingEffect> = new Array<TrailingEffect>();
	individualRolls: Array<IndividualRoll> = new Array<IndividualRoll>();
	playerRollOptions: Array<PlayerRollOptions> = new Array<PlayerRollOptions>();
	bonusRolls: Array<BonusRoll> = null;
	onFirstContactSound: string;
	dieTotalMessage: string;
	textOutlineColor: string;
	textFillColor: string;
	critSuccessMessage: string;
	successMessage: string;
	critFailMessage: string;
	failMessage: string;
	additionalDiceOnHit: string;
	additionalDiceOnHitMessage: string;
	onRollSound: number;
	numHalos: number;
	individualFilter: number;
	skillCheck: Skills;
	damageType: DamageType;
	savingThrow: Ability;
	rollScope: RollScope;
	wildMagic: WildMagic = WildMagic.none;
	groupInspiration: string;
	playBonusSoundAfter: number;
	bentLuckMultiplier: number;
	secondRollData: DiceRollData = null;
	startedBonusDiceRoll: boolean;
	showedVantageMessage: boolean;
	timeLastRolledMs: number;
	appliedVantage = false;
	maxInspirationDiceAllowed = 1;
	numInspirationDiceCreated = 0;
	hasMultiPlayerDice = false;
	multiplayerSummary: Array<PlayerRoll> = null;
	hasSingleIndividual = false;
	secondRollTitle: string;
	minDamage = 0;
	onFirstContactEffect: string;
	effectHueShift: string;
	effectBrightness = 100;
	effectScale = 1;
	effectRotation = 0;
	effectSaturation = 100;
	onStopRollingSound: string;

	getCardModifier(creatureId: number, cardModsList: Map<number, Array<string>>): number {
		let totalModifier = 0;

		if (cardModsList)
			if (!cardModsList.has(creatureId)) {
				cardModsList.set(creatureId, new Array<string>());
			}

		let modList: Array<string> = null;
		if (cardModsList)
			modList = cardModsList.get(creatureId);

		if (this.cardModifiers) {
			// Process addition first...
			this.cardModifiers.forEach((cardModifier: CardModifier) => {
				if (cardModifier.CreatureId === creatureId)
					if (cardModifier.Multiplier === 1) {
						totalModifier += cardModifier.Offset;
						if (modList)
							if (cardModifier.Offset > 0)
								modList.push(`+${cardModifier.Offset}`);
							else if (cardModifier.Offset < 0)
								modList.push(`${cardModifier.Offset}`);
					}
			});

			// Then multiplication...
			this.cardModifiers.forEach((cardModifier: CardModifier) => {
				if (cardModifier.CreatureId === creatureId)
					if (cardModifier.Multiplier !== 1) {
						totalModifier *= cardModifier.Multiplier;
						if (modList)
							modList.push(`x${cardModifier.Multiplier}`);
					}
			});
		}

		return totalModifier;
	}

	getFirstBonusRollDescription(): string {
		if (!this.bonusRolls || this.bonusRolls.length === 0)
			return null;
		return this.bonusRolls[0].description;
	}

	addBonusRoll(diceStr: string, description: string, playerID = Character.invalidCreatureId, dieBackColor: string = DiceLayer.bonusRollDieColor, dieTextColor: string = DiceLayer.bonusRollFontColor, playerName = ''): BonusRoll {
		if (!this.bonusRolls)
			this.bonusRolls = [];
		const bonusRoll: BonusRoll = new BonusRoll(diceStr, description, playerID, dieBackColor, dieTextColor, playerName);
		this.bonusRolls.push(bonusRoll);

		return bonusRoll;
	}

	addBonusDamageRoll(diceStr: string, description: string, playerID = Character.invalidCreatureId, dieBackColor: string = DiceLayer.damageDieBackgroundColor, dieTextColor: string = DiceLayer.damageDieFontColor): BonusRoll {
		if (!this.bonusRolls)
			this.bonusRolls = [];
		const bonusRoll: BonusRoll = new BonusRoll(diceStr, description, playerID, dieBackColor, dieTextColor);
		this.bonusRolls.push(bonusRoll);

		return bonusRoll;
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
		const scale: number = this.getScale(now);
		context.moveTo(this.x, this.y);
		context.lineTo(this.x + this.width * scale, this.y);
		context.stroke();
		context.globalAlpha = 1;
	}

	getVerticalThrust(now: number): number {
		return 0;
	}

	resizeToContent(left: number, top: number, width: number, height: number) {
		const rightSide: number = left + width;
		if (this.x + this.width < rightSide) {
			this.width = rightSide - this.x;
		}
	}
}

class AnimatedRectangle extends ScalableAnimation {
	margin = 5;
	constructor(x: number, y: number, public width: number, public height: number, public fillColor: string, public outlineColor: string, public lifeSpanMs: number, public lineThickness: number = 1) {
		super(x, y, lifeSpanMs);
	}

	render(context: CanvasRenderingContext2D, now: number) {
		context.globalAlpha = this.getAlpha(now) * this.opacity;
		context.strokeStyle = this.outlineColor;
		context.fillStyle = this.fillColor;
		context.lineWidth = this.lineThickness;
		context.lineJoin = "round";
		const scale: number = this.getScale(now);
		context.fillRect(this.x, this.y, this.width * scale, this.height * scale);
		context.strokeRect(this.x, this.y, this.width * scale, this.height * scale);
		context.globalAlpha = 1;
	}

	getVerticalThrust(now: number): number {
		return 0;
	}

	resizeToContent(left: number, top: number, width: number, height: number): any {
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
