enum RollScope {
	ActivePlayer,
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
	Initiative,
	WildMagicD20Check,
	InspirationOnly,
	AddOnDice,
	NonCombatInitiative,
	HPCapacity
}

enum SpriteType {
	PawPrint,
	Raven,
	Spiral,
	Smoke,
	SmokeExplosion,
	SparkTrail,
	SmallSparks,
	Fangs
}

class TrailingEffect {
	constructor(dto: any) {
		this.MinForwardDistanceBetweenPrints = dto.MinForwardDistanceBetweenPrints;
		this.LeftRightDistanceBetweenPrints = dto.LeftRightDistanceBetweenPrints;
		this.Index = dto.Index;
		this.OnPrintPlaySound = dto.OnPrintPlaySound;
		this.MedianSoundInterval = dto.MedianSoundInterval;
		this.PlusMinusSoundInterval = dto.PlusMinusSoundInterval;
		this.NumRandomSounds = dto.NumRandomSounds;
		this.intervalBetweenSounds = 0;

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
		this.HueShift = dto.HueShift;
		this.HueShiftRandom = dto.HueShiftRandom;
		this.Saturation = dto.Saturation;
		this.Brightness = dto.Brightness;
		this.RotationOffset = dto.RotationOffset;
		this.RotationOffsetRandom = dto.RotationOffsetRandom;
		this.FlipHalfTime = dto.FlipHalfTime;
	}

	intervalBetweenSounds: number;
	NumRandomSounds: number;
	MinForwardDistanceBetweenPrints: number;
	LeftRightDistanceBetweenPrints: number;
	Index: number;
	OnPrintPlaySound: string;
	MedianSoundInterval: number;
	PlusMinusSoundInterval: number;

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
	HueShift: string;
	HueShiftRandom: number;
	Saturation: number;
	Brightness: number;
	RotationOffset: number;
	RotationOffsetRandom: number;
	FlipHalfTime: boolean;
}

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
	diceShockwave: Sprites;
	diceBurst: Sprites;
	diceSmoke: Sprites;
	cloverRing: Sprites;
	badLuckRing: Sprites;
	sparkTrail: Sprites;
	haloSpins: Sprites;
	damageFire: Sprites;
	damageCold: Sprites;
	damageNecroticHead: Sprites;
	damageNecroticFog: Sprites;
	damageAcid: Sprites;
	topDieCloud: Sprites;
	damageRadiant: Sprites;
	damageForce: Sprites;
	damageBludgeoningMace: Sprites;
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
	damageSlashingAx: Sprites;
	damageThunder: Sprites;
	superiorityFire: Sprites;
	superiorityDragonHead: Sprites;
	superiorityDragonMouth: Sprites;
	damagePsychic: Sprites;
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
	magicRing: Sprites;
	spirals: Sprites;
	fangs: Sprites;
	halos: Sprites;
	ravens: Sprites[];
	fireTrails: Sprites[];
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


		this.diceShockwave = new Sprites("/Dice/BlowToDust/Shockwave", 44, fps30, AnimationStyle.Sequential, true);
		this.diceShockwave.originX = 188;
		this.diceShockwave.originY = 188;
		this.allBackLayerEffects.add(this.diceShockwave);


		this.diceBurst = new Sprites("/Dice/BlowToDust/DieBurst", 66, fps30, AnimationStyle.SequentialStop, true);
		this.diceBurst.originX = 188;
		this.diceBurst.originY = 188;
		this.allFrontLayerEffects.add(this.diceBurst);


		this.diceSmoke = new Sprites("/Dice/BlowToDust/SmokeTop", 62, fps30, AnimationStyle.Sequential, true);
		this.diceSmoke.originX = 188;
		this.diceSmoke.originY = 188;
		this.allFrontLayerEffects.add(this.diceSmoke);


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

		this.magicRing = new Sprites("/Dice/MagicRing/MagicRing", 180, fps40, AnimationStyle.Loop, true);
		this.magicRing.name = 'MagicRing';
		this.magicRing.returnFrameIndex = 60;
		this.magicRing.originX = 140;
		this.magicRing.originY = 112;
		this.allFrontLayerEffects.add(this.magicRing);

		this.halos = new Sprites("/Dice/Halo/Halo", 90, fps30, AnimationStyle.Loop, true);
		this.halos.name = 'Halo';
		this.halos.originX = 190;
		this.halos.originY = 190;
		this.allFrontLayerEffects.add(this.halos);

		this.haloSpins = new Sprites("/Dice/PaladinSpin/PaladinSpin", 85, fps30, AnimationStyle.Loop, true);
		this.haloSpins.name = 'HaloSpin';
		this.haloSpins.originX = 200;
		this.haloSpins.originY = 200;
		this.allFrontLayerEffects.add(this.haloSpins);

		this.damageFire = new Sprites("/Dice/Damage/Fire/Fire", 89, fps30, AnimationStyle.Loop, true);
		this.damageFire.originX = 133;
		this.damageFire.originY = 133;
		this.allBackLayerEffects.add(this.damageFire);

		this.damageCold = new Sprites("/Dice/Damage/Cold/Cold", 58, fps30, AnimationStyle.Loop, true);
		this.damageCold.originX = 123;
		this.damageCold.originY = 123;
		this.allBackLayerEffects.add(this.damageCold);

		this.damageNecroticHead = new Sprites("/Dice/Damage/Necrotic/NecroticHead", 195, fps30, AnimationStyle.Loop, true);
		this.damageNecroticHead.originX = 143;
		this.damageNecroticHead.originY = 98;
		this.allBackLayerEffects.add(this.damageNecroticHead);

		this.damageNecroticFog = new Sprites("/Dice/Damage/Necrotic/NecroticFog", 103, fps20, AnimationStyle.Loop, true);
		this.damageNecroticFog.originX = 86;
		this.damageNecroticFog.originY = 86;
		this.allBackLayerEffects.add(this.damageNecroticFog);

		this.damageAcid = new Sprites("/Dice/Damage/Acid/Acid", 93, fps30, AnimationStyle.Loop, true);
		this.damageAcid.returnFrameIndex = 19;
		this.damageAcid.originX = 93;
		this.damageAcid.originY = 103;
		this.allBackLayerEffects.add(this.damageAcid);

		this.topDieCloud = new Sprites("/Dice/Damage/TopCloud/TopCloud", 96, fps30, AnimationStyle.Loop, true);
		this.topDieCloud.name = 'TopCloud';
		this.topDieCloud.originX = 61;
		this.topDieCloud.originY = 90;
		this.allFrontLayerEffects.add(this.topDieCloud);

		this.damageRadiant = new Sprites("/Dice/Damage/Radiant/Radiant", 38, fps30, AnimationStyle.Loop, true);
		this.damageRadiant.originX = 100;
		this.damageRadiant.originY = 100;
		this.allBackLayerEffects.add(this.damageRadiant);

		this.damageForce = new Sprites("/Dice/Damage/Force/Force", 90, fps30, AnimationStyle.Loop, true);
		this.damageForce.originX = 176;
		this.damageForce.originY = 64;
		this.allBackLayerEffects.add(this.damageForce);

		this.damageSpinningCloudTrail = new Sprites("/Dice/Damage/Bludgeoning/CloudTrail", 96, fps30, AnimationStyle.Loop, true);
		this.damageSpinningCloudTrail.name = 'CloudTrail';
		this.damageSpinningCloudTrail.originX = 155;
		this.damageSpinningCloudTrail.originY = 183;
		this.allBackLayerEffects.add(this.damageSpinningCloudTrail);

		this.damageBludgeoningMace = new Sprites("/Dice/Damage/Bludgeoning/mace", 84, fps30, AnimationStyle.Loop, true);
		this.damageBludgeoningMace.originX = 0;
		this.damageBludgeoningMace.originY = 52;
		this.allBackLayerEffects.add(this.damageBludgeoningMace);

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

		this.damageSlashingAx = new Sprites("/Dice/Damage/Slashing/SlashingAx", 68, fps30, AnimationStyle.Loop, true);
		this.damageSlashingAx.originX = 174;
		this.damageSlashingAx.originY = 31;
		this.allBackLayerEffects.add(this.damageSlashingAx);

		this.damageThunder = new Sprites("/Dice/Damage/Thunder/Thunder", 79, fps30, AnimationStyle.Loop, true);
		this.damageThunder.originX = 110;
		this.damageThunder.originY = 108;
		this.allBackLayerEffects.add(this.damageThunder);

		this.superiorityDragonHead = new Sprites("/Dice/Damage/Superiority/SuperiorityDragon", 81, fps20, AnimationStyle.Loop, true);
		this.superiorityDragonHead.originX = 74;
		this.superiorityDragonHead.originY = 205;
		this.allBackLayerEffects.add(this.superiorityDragonHead);

		this.superiorityDragonMouth = new Sprites("/Dice/Damage/Superiority/SuperiorityMouth", 81, fps20, AnimationStyle.Loop, true);
		this.superiorityDragonMouth.originX = 74;
		this.superiorityDragonMouth.originY = 205;
		this.allBackLayerEffects.add(this.superiorityDragonMouth);

		this.superiorityFire = new Sprites("/Dice/Damage/Superiority/SuperiorityFire", 103, fps30, AnimationStyle.Loop, true);
		this.superiorityFire.originX = 82;
		this.superiorityFire.originY = 86;
		this.allBackLayerEffects.add(this.superiorityFire);

		this.damagePsychic = new Sprites("/Dice/Damage/Psychic/Psychic", 75, fps30, AnimationStyle.Loop, true);
		this.damagePsychic.originX = 145;
		this.damagePsychic.originY = 143;
		this.allBackLayerEffects.add(this.damagePsychic);

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
		this.loadFireTrails();

		this.spirals = new Sprites("/Dice/Spiral/Spiral", 64, fps40, AnimationStyle.Sequential, true);
		this.spirals.name = 'Spiral';
		this.spirals.originX = 134;
		this.spirals.originY = 107;
		this.allBackLayerEffects.add(this.spirals);

		this.fangs = new Sprites("/Dice/Fang/Fang", 79, fps30, AnimationStyle.Loop, true);
		this.fangs.name = 'Fang';
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

	attachDamageFire(die: any, hueShift: number = 0): void {
		die.attachedDamage = true;
		die.attachedSprites.push(this.addDamageFire(960, 540, Random.max(360), hueShift));
		die.origins.push(this.damageFire.getOrigin());

		this.addTopDieCloud(die, 5, Random.between(20, 45), 0.4, 0, 0, 0);

		diceSounds.safePlayMp3('Dice/Damage/Fire');
	}

	attachLabel(die: any, label: string, backgroundColor: string, textColor: string): void {
		label = label.trim();
		if (label.startsWith('"') && label.endsWith('"'))  // Remove quotes.
			label = label.substr(1, label.length - 2);
		let textEffect: TextEffect = this.animations.addText(new Vector(960, 540), label);
		textEffect.fontSize = 30;
		textEffect.offsetY = 80;
		textEffect.fontColor = textColor;
		textEffect.outlineColor = backgroundColor;
		die.attachedLabels.push(textEffect);
	}

	attachDamageCold(die: any): void {
		die.attachedDamage = true;
		die.attachedSprites.push(this.addDamageCold(960, 540, 0));
		die.origins.push(this.damageCold.getOrigin());
		this.addTopDieCloud(die, 5, Random.between(20, 45), 0.6, 0, 0, 150);
		diceSounds.safePlayMp3('Dice/Damage/Cold');
	}

	attachDamageNecrotic(die: any): void {
		die.attachedDamage = true;

		const numHeads: number = 3;

		let rotationOffset: number = Random.between(0, 360 / numHeads);
		let autoRotation: number = -Random.between(20, 45);

		die.attachedSprites.push(this.addDamageNecroticFog(960, 540, rotationOffset));
		die.origins.push(this.damageNecroticFog.getOrigin());

		for (var i = 0; i < numHeads; i++) {
			die.attachedSprites.push(this.addDamageNecroticHead(960, 540, rotationOffset + 0, autoRotation));
			die.origins.push(this.damageNecroticHead.getOrigin());
			rotationOffset += 360 / numHeads;
		}

		die.attachedSprites.push(this.addDamageNecroticFog(960, 540, rotationOffset));
		die.origins.push(this.damageNecroticFog.getOrigin());

		this.addTopDieCloud(die, 3, Random.between(20, 45), 0.9, 30, 75, 75);

		diceSounds.safePlayMp3('Dice/Damage/Necrotic');
	}

	addTopDieCloud(die: any, count: number, autoRotationDegeesPerSecond: number, opacity: number, hueShift: number, saturation: number = -1, brightness: number = -1): void {
		let angleBetweenClouds: number = 360 / count;
		let rotationOffset: number = Random.between(0, angleBetweenClouds);
		for (var i = 0; i < count; i++) {
			let topDieCloud = this.topDieCloud.addShifted(0, 0, -1, hueShift, saturation, brightness);
			topDieCloud.rotation = rotationOffset;
			topDieCloud.initialRotation = rotationOffset;
			rotationOffset += angleBetweenClouds;
			topDieCloud.autoRotationDegeesPerSecond = autoRotationDegeesPerSecond;
			topDieCloud.fadeInTime = 500;
			topDieCloud.fadeOutTime = 500;
			topDieCloud.opacity = opacity;
			topDieCloud.fadeOnDestroy = true;

			die.attachedSprites.push(topDieCloud);
			die.origins.push(this.topDieCloud.getOrigin());
		}
	}


	attachDamageAcid(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.plusMinus(24);
		die.attachedSprites.push(this.addDamageAcid(960, 540, rotationOffset));
		die.origins.push(this.damageAcid.getOrigin());
		this.addTopDieCloud(die, 3, Random.between(20, 45), 0.5, 100, 200);
		diceSounds.safePlayMp3('Dice/Damage/Acid');
	}


	attachDamageRadiant(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.between(0, 360);
		die.attachedSprites.push(this.addDamageRadiant(960, 540, rotationOffset));
		die.origins.push(this.damageRadiant.getOrigin());
		this.addTopDieCloud(die, 4, Random.between(20, 45), 0.5, 50, 100, 150);
		diceSounds.safePlayMp3('Dice/Damage/Radiant');
	}

	attachDamagePoison(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.between(0, 360);
		let hueShift: number = Random.plusMinus(30);
		die.attachedSprites.push(this.addDamagePoison(960, 540, rotationOffset, hueShift));
		die.origins.push(this.damagePoison.getOrigin());
		this.addTopDieCloud(die, 4, Random.between(20, 45), 0.7, 100 + hueShift, 100, 100);
		diceSounds.safePlayMp3('Dice/Damage/Poison');
	}

	attachDamagePiercing(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.between(0, 360);
		let autoRotation: number = Random.plusMinusBetween(5, 10);
		this.addDagger(die, rotationOffset, autoRotation);
		rotationOffset += 90;
		this.addSword(die, rotationOffset, autoRotation);
		rotationOffset += 90;
		this.addTrident(die, rotationOffset, autoRotation);
		rotationOffset += 90;
		this.addTooth(die, rotationOffset, autoRotation);

		diceSounds.safePlayMp3('Dice/Damage/Piercing');
	}

	private addSword(die: any, rotationOffset: number, autoRotation: number) {
		die.attachedSprites.push(this.addDamagePiercingThickSword(960, 540, rotationOffset, autoRotation));
		die.origins.push(this.damagePiercingThickSword.getOrigin());
	}

	private addTooth(die: any, rotationOffset: number, autoRotation: number) {
		die.attachedSprites.push(this.addDamagePiercingTooth(960, 540, rotationOffset, autoRotation));
		die.origins.push(this.damagePiercingTooth.getOrigin());
	}

	private addTrident(die: any, rotationOffset: number, autoRotation: number) {
		die.attachedSprites.push(this.addDamagePiercingTrident(960, 540, rotationOffset, autoRotation));
		die.origins.push(this.damagePiercingTrident.getOrigin());
	}

	private addDagger(die: any, rotationOffset: number, autoRotation: number) {
		die.attachedSprites.push(this.addDamagePiercingDagger(960, 540, rotationOffset, autoRotation));
		die.origins.push(this.damagePiercingDagger.getOrigin());
	}

	attachDamageSlashing(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.between(0, 360);
		let autoRotation: number = Random.plusMinusBetween(60, 120);

		const numBlades: number = 5;
		let degreesBetweenBlades: number = 360 / numBlades;
		for (var i = 0; i < numBlades; i++) {
			let randomNum: number = Random.max(100);
			if (randomNum < 33) {
				die.attachedSprites.push(this.addDamageSlashingSword(960, 540, rotationOffset, autoRotation));
				die.origins.push(this.damageSlashingSword.getOrigin());
			}
			else if (randomNum < 66) {
				die.attachedSprites.push(this.addDamageSlashingLance(960, 540, rotationOffset, autoRotation));
				die.origins.push(this.damageSlashingLance.getOrigin());
			}
			else {
				die.attachedSprites.push(this.addDamageSlashingAx(960, 540, rotationOffset, autoRotation));
				die.origins.push(this.damageSlashingAx.getOrigin());
			}
			rotationOffset += degreesBetweenBlades;
		}

		diceSounds.safePlayMp3('Dice/Damage/Slashing');
	}

	attachHealth(die: any): void {
		die.attachedSprites.push(this.addHealth(960, 540));
		die.origins.push(this.health.getOrigin());
		diceSounds.safePlayMp3('Dice/health');
	}

	attachDamageThunder(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.between(0, 360);
		die.attachedSprites.push(this.addDamageThunder(960, 540, rotationOffset));
		die.origins.push(this.damageThunder.getOrigin());
		this.addTopDieCloud(die, 3, Random.between(20, 45), 0.8, 260 + Random.plusMinus(20), 100, 50);
		diceSounds.safePlayMp3('Dice/Damage/Thunder');
	}

	attachSuperiority(die: any): void {
		const numHeads: number = 3;

		let rotationOffset: number = Random.between(0, 360 / numHeads);
		let autoRotation: number = Random.plusMinusBetween(20, 45);

		die.attachedSprites.push(this.addSuperiorityFire(960, 540, rotationOffset));
		die.origins.push(this.superiorityFire.getOrigin());

		for (var i = 0; i < numHeads; i++) {
			var startingFrameIndex: number = Random.intMax(this.superiorityDragonHead.baseAnimation.frameCount);
			die.attachedSprites.push(this.addSuperiorityDragonHead(960, 540, startingFrameIndex, rotationOffset + 0, autoRotation));
			die.origins.push(this.superiorityDragonHead.getOrigin());

			die.attachedSprites.push(this.addSuperiorityDragonMouth(960, 540, startingFrameIndex, rotationOffset + 0, autoRotation));
			die.origins.push(this.superiorityDragonMouth.getOrigin());
			rotationOffset += 360 / numHeads;
		}

		//this.addTopDieCloud(die, 5, Random.between(20, 45), 0.2, 0, 0, 100);

		diceSounds.safePlayMp3('Dice/Damage/DragonRoar');
	}

	attachDamageForce(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.plusMinus(60);
		die.attachedSprites.push(this.addDamageForce(960, 540, rotationOffset));
		die.origins.push(this.damageForce.getOrigin());

		die.attachedSprites.push(this.addDamageForce(960, 540, rotationOffset + 120));
		die.origins.push(this.damageForce.getOrigin());

		die.attachedSprites.push(this.addDamageForce(960, 540, rotationOffset + 240));
		die.origins.push(this.damageForce.getOrigin());

		this.addTopDieCloud(die, 2, Random.between(20, 45), 0.4, 250 + Random.plusMinus(20), 100, 50);

		diceSounds.safePlayMp3('Dice/Damage/Force');
	}

	attachDamageBludgeoning(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.plusMinus(60);
		let autoRotationDegeesPerSecond: number = Random.between(60, 100);

		this.addBludgeoningCloud(die, rotationOffset, autoRotationDegeesPerSecond);
		this.addBludgeoningCloud(die, rotationOffset + 120, autoRotationDegeesPerSecond);
		this.addBludgeoningCloud(die, rotationOffset + 240, autoRotationDegeesPerSecond);


		die.attachedSprites.push(this.addDamageBludgeoningMace(960, 540, rotationOffset, autoRotationDegeesPerSecond));
		die.origins.push(this.damageBludgeoningMace.getOrigin());

		die.attachedSprites.push(this.addDamageBludgeoningWeapon(960, 540, rotationOffset + 120, autoRotationDegeesPerSecond, 0));
		die.origins.push(this.damageBludgeoningWeapon.getOrigin());

		die.attachedSprites.push(this.addDamageBludgeoningWeapon(960, 540, rotationOffset + 240, autoRotationDegeesPerSecond, 1));
		die.origins.push(this.damageBludgeoningWeapon.getOrigin());

		diceSounds.safePlayMp3('Dice/Damage/Bludgeoning');
	}

	addBludgeoningCloud(die: any, rotationOffset: number, autoRotationDegeesPerSecond: number): void {
		die.attachedSprites.push(this.addDamageSpinningCloudTrail(960, 540, rotationOffset + 240, autoRotationDegeesPerSecond, -1));
		die.origins.push(this.damageSpinningCloudTrail.getOrigin());
	}

	attachDamagePsychic(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.max(360);
		die.attachedSprites.push(this.addDamagePsychic(960, 540, rotationOffset));
		die.origins.push(this.damagePsychic.getOrigin());
		this.addTopDieCloud(die, 3, Random.between(20, 45), 1, Random.max(360), 100, 150);
		diceSounds.safePlayMp3('Dice/Damage/Psychic');
	}


	attachDamageLightning(die: any): void {
		die.attachedDamage = true;
		let rotationOffset: number = Random.max(120);

		die.attachedSprites.push(this.addDamageLightningCloud(960, 540, Random.max(360)));
		die.origins.push(this.damageLightningCloud.getOrigin());

		let autoRotateDPS: number = Random.plusMinusBetween(10, 30);

		die.attachedSprites.push(this.addDamageLightningA(960, 540, rotationOffset, autoRotateDPS));
		die.origins.push(this.damageLightningA.getOrigin());

		die.attachedSprites.push(this.addDamageLightningB(960, 540, rotationOffset + 120, autoRotateDPS));
		die.origins.push(this.damageLightningB.getOrigin());

		die.attachedSprites.push(this.addDamageLightningC(960, 540, rotationOffset + 240, autoRotateDPS));
		die.origins.push(this.damageLightningC.getOrigin());

		this.addTopDieCloud(die, 3, Random.between(20, 45), 0.4, 250 + Random.plusMinus(30), 100, 150);
		diceSounds.safePlayMp3('Dice/Damage/Lightning');
	}


	initializePlayerData(playerData: string): any {
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

	loadFireTrails(): void {
		let fireTrailA = new Sprites(`/Dice/FireTrail/A/FireTrailA`, 68, fps30, AnimationStyle.Sequential, true);
		fireTrailA.originX = 115;
		fireTrailA.originY = 444;
		this.allBackLayerEffects.add(fireTrailA);
		this.fireTrails.push(fireTrailA);

		let fireTrailB = new Sprites(`/Dice/FireTrail/B/FireTrailB`, 61, fps30, AnimationStyle.Sequential, true);
		fireTrailB.originX = 73;
		fireTrailB.originY = 641;
		this.allBackLayerEffects.add(fireTrailB);
		this.fireTrails.push(fireTrailB);

		let fireTrailC = new Sprites(`/Dice/FireTrail/C/FireTrailC`, 36, fps30, AnimationStyle.Sequential, true);
		fireTrailC.originX = 142;
		fireTrailC.originY = 174;
		this.allBackLayerEffects.add(fireTrailC);
		this.fireTrails.push(fireTrailC);

		let fireTrailD = new Sprites(`/Dice/FireTrail/D/FireTrailD`, 78, fps30, AnimationStyle.Sequential, true);
		fireTrailD.originX = 110;
		fireTrailD.originY = 325;
		this.allBackLayerEffects.add(fireTrailD);
		this.fireTrails.push(fireTrailD);
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

	showMultiplayerResults(title: string, rollSummary: PlayerRoll[], hiddenThreshold: number = 0): any {
		let x: number = 10;
		let y: number = 10;
		const lineHeight: number = 70;
		let backgroundRect: AnimatedRectangle = this.addBackgroundRect(2, 2, 50, 50 /* lineHeight * initiativeSummary.length + margins * 2 */, DiceLayer.multiplePlayerSummaryDuration);
		this.setMultiplayerFades(backgroundRect);
		var lastRollWasHigherThanThreshold: boolean = true;
		if (title) {
			let titleEffect: TextEffect = this.showMultiplayerResultTitle(title, x, y);
			titleEffect.connectedShapes.push(backgroundRect);
			titleEffect.fontSize = 15;
			y += lineHeight;
			backgroundRect.height += lineHeight;
			x += 30;
		}

		let knownPlayerNames: Array<TextEffect> = [];

		var line: AnimatedLine = null;
		for (var i = 0; i < rollSummary.length; i++) {
			let playerRoll: PlayerRoll = rollSummary[i];
			if (playerRoll.roll + playerRoll.modifier < hiddenThreshold && lastRollWasHigherThanThreshold) {
				if (hiddenThreshold != 0) {
					const separatorHeight: number = 20;
					line = this.addSeparatorHorizontalLine(x, y, 10, '#ff0000', DiceLayer.multiplePlayerSummaryDuration);
					this.setMultiplayerFades(line);
					y += separatorHeight;
					backgroundRect.height += separatorHeight;
				}
				lastRollWasHigherThanThreshold = false;
			}
			let effect: TextEffect = this.showPlayerRoll(playerRoll, x, y);
			effect.connectedShapes.push(backgroundRect);
			knownPlayerNames.push(effect);
			y += lineHeight;
		}

		if (line) {
			knownPlayerNames.forEach(function (playerName: TextEffect) { playerName.connectedShapes.push(line) });
		}

		knownPlayerNames
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
			playerRoll.name = diceLayer.getPlayerName(playerRoll.playerId);
		var message: string = `${playerRoll.name}: ${playerRoll.roll + playerRoll.modifier}`;
		if (playerRoll.modifier > 0)
			message += ` (+${playerRoll.modifier})`;
		else if (playerRoll.modifier < 0)
			message += ` (${playerRoll.modifier})`;

		let textEffect: TextEffect = this.animations.addText(new Vector(x, y), message, DiceLayer.multiplePlayerSummaryDuration);
		textEffect.fontColor = this.getDieColor(playerRoll.playerId);
		textEffect.outlineColor = this.getDieFontColor(playerRoll.playerId);
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

	showTotalHealthDamage(totalHealthDamageStr: string, success: boolean, label: string, fontColor: string, outlineColor: string): any {
		var damageTime: number = this.totalDamageTime;
		if (!success)
			damageTime = 2 * damageTime / 3;
		let textEffect: TextEffect = this.animations.addText(new Vector(960, 750), `${label}${totalHealthDamageStr}`, damageTime);
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

	showDieTotal(thisRollStr: string, playerId: number = -1): void {
		let textEffect: TextEffect = this.animations.addText(new Vector(960, 540), thisRollStr, this.totalRollScoreTime);
		this.setTextColorToPlayer(textEffect, playerId);
		textEffect.elasticIn = true;
		textEffect.scale = 10;
		textEffect.opacity = 0.90;
		textEffect.targetScale = 30;
		textEffect.fadeOutTime = 800;
		textEffect.fadeInTime = 200;
	}

	private setTextColorToPlayer(textEffect: TextEffect, playerID: number = -1) {
		if (playerID == -1)
			playerID = this.playerID;
		textEffect.fontColor = this.getDieColor(playerID);
		textEffect.outlineColor = this.getDieFontColor(playerID);
	}

	showRollModifier(rollModifier: number, luckBend: number = 0, playerId: number = -1): void {
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
		this.setTextColorToPlayer(textEffect, playerId);
		textEffect.elasticIn = true;
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

	reportAddOnRoll(message: string, luckMultiplier: number): any {
		if (!message)
			return;
		let textEffect: TextEffect = this.animations.addText(new Vector(960, 540), message, 3000);

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
			this.diceFrontContext.globalAlpha = 0.5;
			this.diceFrontContext.fillText("Dice Roller is waiting for player data to be initialized.", 100, 100);
			this.diceFrontContext.globalAlpha = 1;
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

	addDiceBurst(x: number, y: number, hueShift: number, saturation: number = 100, lightness: number = 100) {
		let shockwave: SpriteProxy = this.diceShockwave.addShifted(x, y, 0, hueShift + Random.plusMinus(50), saturation, lightness);
		shockwave.rotation = Math.random() * 360;
		let dieBurst: SpriteProxy = this.diceBurst.addShifted(x, y, 0, hueShift, saturation, lightness);
		dieBurst.rotation = Math.random() * 360;
		dieBurst.fadeOutTime = 800;
		dieBurst.expirationDate = performance.now() + 2400;
		let smokeTop: SpriteProxy = this.diceSmoke.addShifted(x, y, 0, hueShift + Random.plusMinus(50), saturation, lightness);
		smokeTop.rotation = Math.random() * 360;
		smokeTop.delayStart = 3 * fps30;
		shockwave.delayStart = 6 * fps30;
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

	addDamageFire(x: number, y: number, angle: number, hueShift: number = 0): SpriteProxy {
		let damageFire = this.damageFire.addShifted(x, y, -1, hueShift);
		damageFire.rotation = angle;
		damageFire.fadeInTime = 500;
		damageFire.fadeOutTime = 500;
		damageFire.fadeOnDestroy = true;
		return damageFire;
	}

	addDamageCold(x: number, y: number, angle: number): SpriteProxy {
		let damageCold = this.damageCold.addShifted(x, y, -1, Random.plusMinus(20));
		damageCold.rotation = angle;
		damageCold.fadeInTime = 500;
		damageCold.fadeOutTime = 500;
		damageCold.fadeOnDestroy = true;
		return damageCold;
	}


	addDamageNecroticHead(x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let damageNecroticHead = this.damageNecroticHead.addShifted(x, y, -1, Random.plusMinus(70) - 20, Random.max(50) + 50);
		damageNecroticHead.rotation = angle;
		damageNecroticHead.autoRotationDegeesPerSecond = autoRotation;
		damageNecroticHead.initialRotation = angle;
		damageNecroticHead.fadeInTime = 500;
		damageNecroticHead.fadeOutTime = 500;
		damageNecroticHead.fadeOnDestroy = true;
		return damageNecroticHead;
	}

	addDamageNecroticFog(x: number, y: number, angle: number): SpriteProxy {
		let damageNecroticFog = this.damageNecroticFog.add(x, y, -1);
		damageNecroticFog.rotation = angle;
		damageNecroticFog.fadeInTime = 500;
		damageNecroticFog.fadeOutTime = 500;
		damageNecroticFog.fadeOnDestroy = true;
		return damageNecroticFog;
	}

	addDamageAcid(x: number, y: number, angle: number): SpriteProxy {
		let damageAcid = this.damageAcid.add(x, y, 0);
		damageAcid.rotation = angle;
		damageAcid.fadeInTime = 500;
		damageAcid.fadeOutTime = 500;
		damageAcid.fadeOnDestroy = true;
		return damageAcid;
	}

	addDamageRadiant(x: number, y: number, angle: number): SpriteProxy {
		let damageRadiant = this.damageRadiant.add(x, y, -1);
		damageRadiant.rotation = angle;
		damageRadiant.fadeInTime = 500;
		damageRadiant.fadeOutTime = 500;
		damageRadiant.fadeOnDestroy = true;
		return damageRadiant;
	}

	addDamageForce(x: number, y: number, angle: number): SpriteProxy {
		let damageForce = this.damageForce.addShifted(x, y, -1, Random.plusMinus(30));
		damageForce.rotation = angle;
		damageForce.autoRotationDegeesPerSecond = 5;
		damageForce.initialRotation = angle;
		damageForce.fadeInTime = 500;
		damageForce.fadeOutTime = 500;
		damageForce.fadeOnDestroy = true;
		return damageForce;
	}

	addDamageBludgeoningMace(x: number, y: number, angle: number, autoRotationDegeesPerSecond: number): SpriteProxy {
		let damageBludgeoningMace = this.damageBludgeoningMace.add(x, y, -1);
		damageBludgeoningMace.rotation = angle;
		damageBludgeoningMace.autoRotationDegeesPerSecond = autoRotationDegeesPerSecond;
		damageBludgeoningMace.initialRotation = angle;
		damageBludgeoningMace.fadeInTime = 500;
		damageBludgeoningMace.fadeOutTime = 500;
		damageBludgeoningMace.fadeOnDestroy = true;
		return damageBludgeoningMace;
	}

	addDamageBludgeoningWeapon(x: number, y: number, angle: number, autoRotationDegeesPerSecond: number, startingFrame: number): SpriteProxy {
		let damageBludgeoningWeapon = this.damageBludgeoningWeapon.add(x, y, startingFrame);
		damageBludgeoningWeapon.rotation = angle;
		damageBludgeoningWeapon.autoRotationDegeesPerSecond = autoRotationDegeesPerSecond;
		damageBludgeoningWeapon.initialRotation = angle;
		damageBludgeoningWeapon.fadeInTime = 500;
		damageBludgeoningWeapon.fadeOutTime = 500;
		damageBludgeoningWeapon.fadeOnDestroy = true;
		return damageBludgeoningWeapon;
	}

	addDamageSpinningCloudTrail(x: number, y: number, angle: number, autoRotationDegeesPerSecond: number, startingFrame: number): SpriteProxy {
		let damageSpinningCloudTrail = this.damageSpinningCloudTrail.addShifted(x, y, startingFrame, this.activePlayerHueShift);
		damageSpinningCloudTrail.rotation = angle;
		damageSpinningCloudTrail.opacity = 0.5;
		damageSpinningCloudTrail.autoRotationDegeesPerSecond = autoRotationDegeesPerSecond;
		damageSpinningCloudTrail.initialRotation = angle;
		damageSpinningCloudTrail.fadeInTime = 500;
		damageSpinningCloudTrail.fadeOutTime = 500;
		damageSpinningCloudTrail.fadeOnDestroy = true;
		return damageSpinningCloudTrail;
	}

	addDamagePoison(x: number, y: number, angle: number, hueShift: number): SpriteProxy {
		let damagePoison = this.damagePoison.addShifted(x, y, -1, hueShift);
		damagePoison.rotation = angle;
		damagePoison.fadeInTime = 500;
		damagePoison.fadeOutTime = 500;
		damagePoison.fadeOnDestroy = true;
		return damagePoison;
	}

	addDamagePiercingDagger(x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let damagePiercingDagger = this.damagePiercingDagger.add(x, y, -1);
		damagePiercingDagger.autoRotationDegeesPerSecond = autoRotation;
		damagePiercingDagger.initialRotation = angle;
		damagePiercingDagger.rotation = angle;
		damagePiercingDagger.fadeInTime = 500;
		damagePiercingDagger.fadeOutTime = 500;
		damagePiercingDagger.fadeOnDestroy = true;
		return damagePiercingDagger;
	}

	addDamagePiercingThickSword(x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let damagePiercingThickSword = this.damagePiercingThickSword.add(x, y, -1);
		damagePiercingThickSword.autoRotationDegeesPerSecond = autoRotation;
		damagePiercingThickSword.initialRotation = angle;
		damagePiercingThickSword.rotation = angle;
		damagePiercingThickSword.fadeInTime = 500;
		damagePiercingThickSword.fadeOutTime = 500;
		damagePiercingThickSword.fadeOnDestroy = true;
		return damagePiercingThickSword;
	}

	addDamagePiercingTooth(x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let damagePiercingTooth = this.damagePiercingTooth.add(x, y, -1);
		damagePiercingTooth.autoRotationDegeesPerSecond = autoRotation;
		damagePiercingTooth.initialRotation = angle;
		damagePiercingTooth.rotation = angle;
		damagePiercingTooth.fadeInTime = 500;
		damagePiercingTooth.fadeOutTime = 500;
		damagePiercingTooth.fadeOnDestroy = true;
		return damagePiercingTooth;
	}

	addDamagePiercingTrident(x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let damagePiercingTrident = this.damagePiercingTrident.add(x, y, -1);
		damagePiercingTrident.autoRotationDegeesPerSecond = autoRotation;
		damagePiercingTrident.initialRotation = angle;
		damagePiercingTrident.rotation = angle;
		damagePiercingTrident.fadeInTime = 500;
		damagePiercingTrident.fadeOutTime = 500;
		damagePiercingTrident.fadeOnDestroy = true;
		return damagePiercingTrident;
	}

	addDamageSlashingSword(x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let damageSlashingSword = this.damageSlashingSword.addShifted(x, y, -1, Random.max(360), 30 + Random.max(50));
		damageSlashingSword.autoRotationDegeesPerSecond = autoRotation;
		damageSlashingSword.initialRotation = angle;
		damageSlashingSword.rotation = angle;
		damageSlashingSword.fadeInTime = 500;
		damageSlashingSword.fadeOutTime = 500;
		damageSlashingSword.fadeOnDestroy = true;
		return damageSlashingSword;
	}


	addDamageSlashingAx(x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let damageSlashingAx = this.damageSlashingAx.addShifted(x, y, -1, Random.max(360), 30 + Random.max(50));
		damageSlashingAx.autoRotationDegeesPerSecond = autoRotation;
		damageSlashingAx.initialRotation = angle;
		damageSlashingAx.rotation = angle;
		damageSlashingAx.fadeInTime = 500;
		damageSlashingAx.fadeOutTime = 500;
		damageSlashingAx.fadeOnDestroy = true;
		return damageSlashingAx;
	}

	addDamageSlashingLance(x: number, y: number, angle: number, autoRotation: number): SpriteProxy {
		let damageSlashingLance = this.damageSlashingLance.addShifted(x, y, -1, Random.max(360), 20 + Random.max(50));
		damageSlashingLance.autoRotationDegeesPerSecond = autoRotation;
		damageSlashingLance.initialRotation = angle;
		damageSlashingLance.rotation = angle;
		damageSlashingLance.fadeInTime = 500;
		damageSlashingLance.fadeOutTime = 500;
		damageSlashingLance.fadeOnDestroy = true;
		return damageSlashingLance;
	}

	addHealth(x: number, y: number): SpriteProxy {
		let health = this.health.addShifted(x, y, -1, Random.max(40) - 10);
		health.fadeInTime = 500;
		health.fadeOutTime = 500;
		health.fadeOnDestroy = true;
		return health;
	}


	addDamageThunder(x: number, y: number, angle: number): SpriteProxy {
		let damageThunder = this.damageThunder.addShifted(x, y, -1, Random.plusMinus(30));
		damageThunder.rotation = angle;
		damageThunder.fadeInTime = 500;
		damageThunder.fadeOutTime = 500;
		damageThunder.fadeOnDestroy = true;
		return damageThunder;
	}
	addSuperiorityFire(x: number, y: number, angle: number): SpriteProxy {
		let superiorityFire = this.superiorityFire.addShifted(x, y, -1, Random.plusMinus(30));
		superiorityFire.rotation = angle;
		superiorityFire.fadeInTime = 500;
		superiorityFire.fadeOutTime = 500;
		superiorityFire.fadeOnDestroy = true;
		return superiorityFire;
	}

	addSuperiorityDragonHead(x: number, y: number, startingFrameIndex: number, angle: number, autoRotation: number): SpriteProxy {
		var headSaturation: number = 100;
		var headBrightness: number = 100;
		if (Random.chancePercent(20)) {
			headSaturation = 0;
			if (Random.chancePercent(33)) {
				headBrightness = 60;
			}
			else if (Random.chancePercent(33)) {
				headBrightness = 200;
			}
		}
		let superiorityDragonHead = this.superiorityDragonHead.addShifted(x, y, startingFrameIndex, Random.max(360), headSaturation, headBrightness);
		superiorityDragonHead.rotation = angle;
		superiorityDragonHead.initialRotation = angle;
		superiorityDragonHead.autoRotationDegeesPerSecond = autoRotation;
		superiorityDragonHead.fadeInTime = 500;
		superiorityDragonHead.fadeOutTime = 500;
		superiorityDragonHead.fadeOnDestroy = true;
		return superiorityDragonHead;
	}

	addSuperiorityDragonMouth(x: number, y: number, startingFrameIndex: number, angle: number, autoRotation: number): SpriteProxy {
		let superiorityMouth = this.superiorityDragonMouth.add(x, y, startingFrameIndex);
		superiorityMouth.rotation = angle;
		superiorityMouth.initialRotation = angle;
		superiorityMouth.autoRotationDegeesPerSecond = autoRotation;
		superiorityMouth.fadeInTime = 500;
		superiorityMouth.fadeOutTime = 500;
		superiorityMouth.fadeOnDestroy = true;
		return superiorityMouth;
	}

	addDamagePsychic(x: number, y: number, angle: number): SpriteProxy {
		let damagePsychic = this.damagePsychic.addShifted(x, y, -1, Random.plusMinus(60));
		damagePsychic.rotation = angle;
		damagePsychic.autoRotationDegeesPerSecond = Random.plusMinusBetween(5, 25);
		damagePsychic.initialRotation = angle;
		damagePsychic.fadeInTime = 500;
		damagePsychic.fadeOutTime = 500;
		damagePsychic.fadeOnDestroy = true;
		return damagePsychic;
	}


	addDamageLightningA(x: number, y: number, angle: number, autoRotateDegreesPerSecond: number): SpriteProxy {
		let damageLightningA = this.damageLightningA.addShifted(x, y, 0, Random.plusMinus(20));
		damageLightningA.rotation = angle;
		damageLightningA.autoRotationDegeesPerSecond = autoRotateDegreesPerSecond;
		damageLightningA.initialRotation = angle;
		damageLightningA.fadeInTime = 500;
		damageLightningA.fadeOutTime = 500;
		damageLightningA.fadeOnDestroy = true;
		return damageLightningA;
	}


	addDamageLightningB(x: number, y: number, angle: number, autoRotateDegreesPerSecond: number): SpriteProxy {
		let damageLightningB = this.damageLightningB.addShifted(x, y, 0, Random.plusMinus(20));
		damageLightningB.rotation = angle;
		damageLightningB.autoRotationDegeesPerSecond = autoRotateDegreesPerSecond;
		damageLightningB.initialRotation = angle;
		damageLightningB.fadeInTime = 500;
		damageLightningB.fadeOutTime = 500;
		damageLightningB.fadeOnDestroy = true;
		return damageLightningB;
	}

	addDamageLightningC(x: number, y: number, angle: number, autoRotateDegreesPerSecond: number): SpriteProxy {
		let damageLightningC = this.damageLightningC.addShifted(x, y, 0, Random.plusMinus(20));
		damageLightningC.rotation = angle;
		damageLightningC.autoRotationDegeesPerSecond = autoRotateDegreesPerSecond;
		damageLightningC.initialRotation = angle;
		damageLightningC.fadeInTime = 500;
		damageLightningC.fadeOutTime = 500;
		damageLightningC.fadeOnDestroy = true;
		return damageLightningC;
	}

	addDamageLightningCloud(x: number, y: number, angle: number): SpriteProxy {
		let damageLightningCloud = this.damageLightningCloud.addShifted(x, y, 0, Random.plusMinus(20));
		damageLightningCloud.rotation = angle;
		damageLightningCloud.fadeInTime = 500;
		damageLightningCloud.fadeOutTime = 500;
		damageLightningCloud.fadeOnDestroy = true;
		return damageLightningCloud;
	}


	addInspirationParticles(x: number, y: number, rotationDegeesPerSecond: number, hueShift: number = 0): SpriteProxy {
		let inspirationParticlesProxy = this.inspirationParticles.addShifted(x, y, 0, hueShift);
		inspirationParticlesProxy.fadeInTime = 500;
		inspirationParticlesProxy.fadeOutTime = 500;
		inspirationParticlesProxy.fadeOnDestroy = true;
		inspirationParticlesProxy.autoRotationDegeesPerSecond = rotationDegeesPerSecond;
		inspirationParticlesProxy.initialRotation = Math.random() * 360;
		return inspirationParticlesProxy;
	}

	addInspirationSmoke(x: number, y: number, angle: number): SpriteProxy {
		let inspirationSmokeProxy = this.inspirationSmoke.add(x, y, -1);
		inspirationSmokeProxy.rotation = angle;
		inspirationSmokeProxy.fadeInTime = 500;
		inspirationSmokeProxy.fadeOutTime = 1000;
		inspirationSmokeProxy.fadeOnDestroy = true;
		inspirationSmokeProxy.expirationDate = performance.now() + 3000;
		return inspirationSmokeProxy;
	}

	addRipple(x: number, y: number, angle: number, hueShift: number = 0): ColorShiftingSpriteProxy {
		let rippleProxy = this.ripples.addShifted(x, y, 0, hueShift);
		rippleProxy.rotation = angle;
		return rippleProxy;
	}

	addRaven(x: number, y: number, angle: number) {
		let ravens: Sprites = this.getRavens();
		let raven = ravens.addShifted(x, y, 0, this.activePlayerHueShift + Random.plusMinus(35));
		raven.rotation = angle + 180 + Random.plusMinus(45);
		return raven;
	}

	getRavens() {
		let index: number = Math.floor(Math.random() * this.ravens.length);
		return this.ravens[index];
	}

	getFireTrails() {
		let index: number = Math.floor(Math.random() * this.fireTrails.length);
		return this.fireTrails[index];
	}

	blowColoredSmoke(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
		this.diceBlowColoredSmoke.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
	}

	//addD20Fire(x: number, y: number) {
	//  this.d20Fire.add(x, y).rotation = Math.random() * 360;
	//}

	addSmoke(x: number, y: number, angle: number) {
		let hueShift = this.activePlayerHueShift + Random.plusMinus(10);
		var angleShift: number = 0;
		if (Math.random() > 0.5)
			angleShift = 180;
		this.smoke.addShifted(x, y, 0, hueShift).rotation = angle + angleShift;
	}

	addSparkTrail(x: number, y: number, angle: number) {
		this.sparkTrail.addShifted(x, y, 0, this.activePlayerHueShift + Random.plusMinus(35)).rotation = angle + 180 + Random.plusMinus(15);
	}

	clearResidualEffects(): any {
		this.cloverRing.sprites = [];
		this.magicRing.sprites = [];
		this.badLuckRing.sprites = [];
		this.freeze.sprites = [];
		this.halos.sprites = [];
		this.haloSpins.sprites = [];
		this.damageFire.sprites = [];
		this.damageCold.sprites = [];
		this.damageNecroticHead.sprites = [];
		this.damageNecroticFog.sprites = [];
		this.damageAcid.sprites = [];
		this.topDieCloud.sprites = [];
		this.damageRadiant.sprites = [];
		this.damageForce.sprites = [];
		this.damageBludgeoningMace.sprites = [];
		this.damageBludgeoningWeapon.sprites = [];
		this.damageSpinningCloudTrail.sprites = [];
		this.damagePoison.sprites = [];
		this.damagePiercingDagger.sprites = [];
		this.damagePiercingThickSword.sprites = [];
		this.damagePiercingTooth.sprites = [];
		this.damagePiercingTrident.sprites = [];
		this.damageSlashingSword.sprites = [];
		this.damageSlashingAx.sprites = [];
		this.damageSlashingLance.sprites = [];
		this.damageThunder.sprites = [];
		this.superiorityFire.sprites = [];
		this.superiorityDragonHead.sprites = [];
		this.superiorityDragonMouth.sprites = [];
		this.health.sprites = [];
		this.damagePsychic.sprites = [];
		this.damageLightningA.sprites = [];
		this.damageLightningB.sprites = [];
		this.damageLightningC.sprites = [];
		this.damageLightningCloud.sprites = [];
		this.freezePop.sprites = [];
		this.inspirationParticles.sprites = [];
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
		this.smokeExplosionBottom.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = rotation;
		this.smokeExplosionTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = rotation;
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
		diceRoll.secondRollTitle = dto.SecondRollTitle;
		diceRoll.vantageKind = dto.VantageKind;
		diceRoll.damageHealthExtraDice = dto.DamageDice;
		diceRoll.modifier = dto.Modifier;
		diceRoll.hiddenThreshold = dto.HiddenThreshold;
		diceRoll.isMagic = dto.IsMagic;
		diceRoll.spellName = dto.SpellName;
		diceRoll.onThrowSound = dto.OnThrowSound;
		diceRoll.throwPower = dto.ThrowPower;
		diceRoll.onFirstContactSound = dto.OnFirstContactSound;
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

		for (var i = 0; i < dto.TrailingEffects.length; i++) {
			diceRoll.trailingEffects.push(new TrailingEffect(dto.TrailingEffects[i]));
		}

		for (var i = 0; i < dto.PlayerRollOptions.length; i++) {
			diceRoll.playerRollOptions.push(new PlayerRollOptions(dto.PlayerRollOptions[i]));
		}

		if (diceRoll.throwPower < 0.2)
			diceRoll.throwPower = 0.2;
		if (diceRoll.throwPower > 4.0)
			diceRoll.throwPower = 4.0;
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
		let spiral = this.spirals.addShifted(x, y, 0, diceLayer.activePlayerHueShift + Random.plusMinus(20));
		spiral.rotation = Random.between(0, 360);
		return spiral;
	}

	addFangs(x: number, y: number, angle: number): SpriteProxy {
		let fangs = this.fangs.addShifted(x, y, Math.round(Math.random() * this.fangs.sprites.length), diceLayer.activePlayerHueShift + Random.plusMinus(20));
		fangs.rotation = angle;
		fangs.expirationDate = performance.now() + 4000;
		fangs.fadeOutTime = 2000;
		fangs.fadeInTime = 500;
		fangs.opacity = 0.9;
		return fangs;
	}

	addPawPrint(x: number, y: number, angle: number): SpriteProxy {
		let pawPrint = this.pawPrints.addShifted(x, y, Math.round(Math.random() * this.pawPrints.sprites.length), diceLayer.activePlayerHueShift + Random.plusMinus(20));
		pawPrint.rotation = angle;
		pawPrint.expirationDate = performance.now() + 4000;
		pawPrint.fadeOutTime = 2000;
		pawPrint.fadeInTime = 500;
		pawPrint.opacity = 0.9;
		return pawPrint;
	}

	AddTrailingEffectFrom(sprites: Sprites, trailingEffect: TrailingEffect, x: number, y: number, angle: number): SpriteProxy {
		let index: number = 0;
		if (trailingEffect.StartIndex == -1)
			index = Math.round(Math.random() * sprites.sprites.length);
		else
			index = trailingEffect.StartIndex;

		let hue: number = 0;
		if (trailingEffect.HueShift == "player")
			hue = diceLayer.activePlayerHueShift;
		else if (trailingEffect.HueShift)
			hue = +trailingEffect.HueShift;

		if (trailingEffect.HueShiftRandom != 0)
			hue += Random.plusMinus(trailingEffect.HueShiftRandom);

		let effect: ColorShiftingSpriteProxy = sprites.addShifted(x, y, index, hue);
		effect.rotation = angle + trailingEffect.RotationOffset + Random.plusMinus(trailingEffect.RotationOffsetRandom);
		if (trailingEffect.Lifespan)
			effect.expirationDate = performance.now() + trailingEffect.Lifespan;
		effect.fadeOutTime = trailingEffect.FadeOut;
		effect.fadeInTime = trailingEffect.FadeIn;
		effect.opacity = trailingEffect.Opacity;
		effect.brightness = trailingEffect.Brightness;
		effect.saturationPercent = trailingEffect.Saturation;
		effect.scale = trailingEffect.Scale;
		return effect;
	}

	AddTrailingEffect(trailingEffect: TrailingEffect, x: number, y: number, angle: number): SpriteProxy {
		let result: SpriteProxy;
		let sprites: Sprites = this.allBackLayerEffects.getSpritesByName(trailingEffect.EffectType);
		if (sprites)
			result = this.AddTrailingEffectFrom(sprites, trailingEffect, x, y, angle);

		if (trailingEffect.EffectType === 'Raven') {
			sprites = this.getRavens();
			result = this.AddTrailingEffectFrom(sprites, trailingEffect, x, y, angle);
		}
		else if (trailingEffect.EffectType === 'FireTrails') {
			sprites = this.getFireTrails();
			result = this.AddTrailingEffectFrom(sprites, trailingEffect, x, y, angle);
		}

		sprites = this.allFrontLayerEffects.getSpritesByName(trailingEffect.EffectType);

		if (sprites)
			result = this.AddTrailingEffectFrom(sprites, trailingEffect, x, y, angle);

		return result;
	}

	AddEffect(effectName: string, x: number, y: number, scale: number, hueShift: string = '0', saturation: number = 100, brightness: number = 100, angle: number = 0): SpriteProxy {
		let result: SpriteProxy;
		let hue: number;
		if (hueShift == 'player')
			hue = this.activePlayerHueShift;
		else
			hue = +hueShift;

		if (angle === -1)
			angle = Random.max(360);

		let sprites: Sprites = this.allBackLayerEffects.getSpritesByName(effectName);
		if (sprites)
			result = this.AddEffectFrom(sprites, x, y, scale, hue, saturation, brightness, angle);

		sprites = this.allFrontLayerEffects.getSpritesByName(effectName);

		if (sprites)
			result = this.AddEffectFrom(sprites, x, y, scale, hue, saturation, brightness, angle);

		return result;
	}

	AddEffectFrom(sprites: Sprites, x: number, y: number, scale: number, hueShift: number, saturation: number = 100, brightness: number = 100, angle: number = 0): SpriteProxy {
		let effect: ColorShiftingSpriteProxy = sprites.addShifted(x, y, 0, hueShift);
		effect.rotation = angle;
		effect.brightness = brightness;
		effect.saturationPercent = saturation;
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

	clearDice(): void {
		removeRemainingDice();
		//if (!removeRemainingDice())
		//	allDiceHaveBeenDestroyed(JSON.stringify(lastRollDiceData));
	}

	rollDice(diceRollDto: string): void {
		let diceRollData: DiceRollData = this.getDiceRollData(diceRollDto);
		console.log(diceRollData);
		pleaseRollDice(diceRollData);

		//  { "Type": 1, "Kind": 0, "DamageDice": "2d8+6", "Modifier": 1.0, "HiddenThreshold": 12.0, "IsMagic": true }
	}

	getDieColor(playerID: number): string {
		let player: Character = this.getPlayer(playerID);
		if (player)
			return player.dieBackColor;

		return '#000000';
	}

	getPlayer(playerID: number): Character {
		if (playerID < 0)
			return null;

		for (var i = 0; i < this.players.length; i++) {
			let player: Character = this.players[i];
			if (player.playerID == playerID)
				return player;
		}

		return null;
	}

	getDieFontColor(playerID: number): string {
		let player: Character = this.getPlayer(playerID);
		if (player)
			return player.dieFontColor;

		return this.activePlayerDieFontColor;
	}

	getPlayerName(playerID: number): string {
		let player: Character = this.getPlayer(playerID);
		if (player)
			return player.getFirstName();

		return '';
	}

	getHueShift(playerID: number): number {

		let player: Character = this.getPlayer(playerID);
		if (player)
			return player.hueShift;

		return 0;
	}

	playerChanged(playerID: number): void {
		this.playerID = playerID;
		this.activePlayerDieFontColor = '#ffffff';

		this.activePlayerDieColor = this.getDieColor(playerID);
		this.activePlayerHueShift = this.getHueShift(playerID);
	}
}

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

class IndividualRoll {
	constructor(public value: number, public numSides: number, public type: string) {
		
	}
}

class DiceRollData {
	type: DiceRollType;
	vantageKind: VantageKind;
	damageHealthExtraDice: string;
	modifier: number;
	minCrit: number;
	hiddenThreshold: number;
	isMagic: boolean;
	spellName: string;
	onThrowSound: string;
	throwPower: number;
	itsAD20Roll: boolean;
	trailingEffects: Array<TrailingEffect> = new Array<TrailingEffect>();
	individualRolls: Array<IndividualRoll> = new Array<IndividualRoll>();
	playerRollOptions: Array<PlayerRollOptions> = new Array<PlayerRollOptions>();
	bonusRolls: Array<BonusRoll> = null;
	onFirstContactSound: string;
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
	appliedVantage: boolean = false;
	maxInspirationDiceAllowed: number = 1;
	numInspirationDiceCreated: number = 0;
	hasMultiPlayerDice: boolean = false;
	multiplayerSummary: Array<PlayerRoll> = null;
	hasSingleIndividual: boolean = false;
	secondRollTitle: string;
	minDamage: number = 0;
	onFirstContactEffect: string;
	effectHueShift: string;
	effectBrightness: number = 100;
	effectScale: number = 1;
	effectRotation: number = 0;
	effectSaturation: number = 100;
	onStopRollingSound: string;

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
	dieCountsAs: DieCountsAs = DieCountsAs.bonus;
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

	resizeToContent(left: number, top: number, width: number, height: number) {
		let rightSide: number = left + width;
		if (this.x + this.width < rightSide) {
			this.width = rightSide - this.x;
		}
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
