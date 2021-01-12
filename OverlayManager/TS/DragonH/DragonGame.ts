enum VectorCompassDirection {
	None,
	Up,
	Down,
	Left,
	Right,
	Forward,
	Backward
}

enum HandSide {
	Left,
	Right
}

interface ThrownObject {
	Position: ScaledPoint;
	Index: number;
	PositionZ: number;
}

class HandFollowingData {
	HandSide: HandSide;
	TrackingObjectIndex = -1;

	constructor(handSide: HandSide, trackingObjectIndex = -1) {
		this.HandSide = handSide;
		this.TrackingObjectIndex = trackingObjectIndex;
	}
}

class ScaledPoint {
	X: number;
	Y: number;
	Scale: number;

	constructor(x: number, y: number, scale: number) {
		this.X = x;
		this.Y = y;
		this.Scale = scale;
	}
}

class Finger2d {
	TipPosition: ScaledPoint;
	constructor() {

	}
}

class Point2D {
	constructor(public X: number, public Y: number) {

	}
}

class ScaledPlane {
	UpperLeft: ScaledPoint;
	LowerRight: ScaledPoint;
	UpperLeft2D: Point2D;
	LowerRight2D: Point2D;
	constructor() {

	}
}

class Hand2d {
	Speed: number;
	PalmDirection: VectorCompassDirection;
	SpeedDirection: VectorCompassDirection;
	ThrowDirection: VectorCompassDirection;
	FacingForwardOrBack: VectorCompassDirection;
	FloatingAttachPoint: ScaledPoint;
	PalmPosition: ScaledPoint;
	Fingers: Array<Finger2d>;
	Side: HandSide;
	Throwing: boolean;
	JustCaught: boolean;
	IsFist: boolean;
	IsFlat: boolean;
	ThrownObjectIndex: number;
	PalmAttachPoint: ScaledPoint;
	PositionZ: number;
	constructor() {

	}
}

enum TargetHand {
	None,
	Any,
	Left,
	Right,
	Both
}

interface HandEffectDto {
	EffectName: string;
	Scale: number;
	HueShift: number;
	FollowHand: boolean;
	OffsetX: number;
	OffsetY: number;
	TargetHand: TargetHand;
}

interface HandFxDto {
	HandEffects: Array<HandEffectDto>;
	KillFollowEffects: TargetHand;
}

interface SkeletalData2d {
	Hands: Array<Hand2d>;
	ThrownObjects: Array<ThrownObject>;
	BackPlane: ScaledPlane;
	FrontPlane: ScaledPlane;
	ActivePlane: ScaledPlane;
	ShowBackPlane: boolean;
	ShowFrontPlane: boolean;
	ShowActivePlane: boolean;
	ShowLiveHandPosition: boolean;
	HandEffect: HandFxDto;
}

interface IGetPlayerX {
	getPlayerX(playerIndex: number): number;
	getPlayerIndex(playerId: number): number;
	getPlayerIndexFromName(playerName: string): number;
	getPlayerFirstName(playerId: number): string;
	getPlayerTargetX(iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, playerIndex: number, players: Array<Character>);
}

class WealthChange {
	PlayerIds: Array<number>;
	Coins: Coins;
	constructor() {

	}
}

class Coins {
	NumGold: number;
	NumSilver: number;
	NumCopper: number;
	NumElectrum: number;
	NumPlatinum: number;
	TotalGold: number;
	constructor() {

	}
}

enum SpellTargetShape {
	None,
	Point,
	Line,
	Cone,
	Sphere,
	Circle,
	Cube
}

enum SpellTargetType {
	None,
	Creatures,
	AreaOfEffect,
	Item
}

enum SpellComponents {
	Verbal = 1,
	Somatic = 2,
	Material = 4
}

class Spell {
	Range: number;
	Name: string;
	OwnerId: number;
	Description: string;
	Components: SpellComponents;
	Material: string;
	RequiresConcentration: boolean;
	constructor() {

	}
}

enum AttackTargetType {
	Spell,
	Weapon
}

class Target {
	Type: AttackTargetType;
	SpellType: SpellTargetType;
	Shape: SpellTargetShape;
	CasterId: number;
	PlayerIds: Array<number>;
	Location: Vector;
	Range: number;
	constructor() {

	}
}

class CastedSpellDataDto {
	Spell: Spell;
	Target: Target;
	Windups: Array<WindupData> = new Array<WindupData>();
	constructor() {

	}
}

class SpellEffect {
	windups: Array<WindupData> = new Array<WindupData>();
	constructor(public name: string) {

	}
}

class KnownSpellsEffects {
	static spellEffects: Array<SpellEffect> = new Array<SpellEffect>();

	static addSpell(spellEffect: SpellEffect) {
		this.spellEffects.push(spellEffect);
	}

	static getSpell(name: string): SpellEffect {
		for (let i = 0; i < this.spellEffects.length; i++) {
			const spellEffect: SpellEffect = this.spellEffects[i];
			if (spellEffect.name === name)
				return spellEffect;
		}
	}

	static initializeSpells(): void {
		this.addSpell(KnownSpellsEffects.shieldOfFaith());
		this.addSpell(KnownSpellsEffects.sanctuary());
	}

	static getEffect(effectName: string, hue: number): WindupData {
		return new WindupData(effectName, hue);
	}

	static shieldOfFaith(): SpellEffect {
		const spell: SpellEffect = new SpellEffect('Shield of Faith');
		const part1: WindupData = KnownSpellsEffects.getEffect('Wide', -15);
		const part2: WindupData = KnownSpellsEffects.getEffect('Wide', 15);
		const part3: WindupData = KnownSpellsEffects.getEffect('Wide', 45);

		part2.Rotation = 45;
		part2.DegreesOffset = -60;

		part3.Rotation = -45;
		part3.DegreesOffset = 60;

		spell.windups.push(part1);
		spell.windups.push(part2);
		spell.windups.push(part3);
		return spell;
	}

	static sanctuary(): SpellEffect {
		const spell: SpellEffect = new SpellEffect('Sanctuary');
		const part1: WindupData = KnownSpellsEffects.getEffect('LiquidSparks', 170);
		const part2: WindupData = KnownSpellsEffects.getEffect('LiquidSparks', 200);
		const part3: WindupData = KnownSpellsEffects.getEffect('LiquidSparks', 230);

		part2.Rotation = 45;
		part2.DegreesOffset = -60;

		part3.Rotation = -45;
		part3.DegreesOffset = 60;

		spell.windups.push(part1);
		spell.windups.push(part2);
		spell.windups.push(part3);
		return spell;
	}

}


abstract class DragonGame extends GamePlusQuiz implements IGetPlayerX {
	textAnimations: Animations = new Animations();
	leapEffectSoundManager: SoundManager;
	skeletalData2d: SkeletalData2d;
	static maxFiltersPerWindup = 6;
	abstract layerSuffix: string;
	dndTimeStr: string;
	dndDateStr: string;

	fireBallBack: Sprites;
	fireBallFront: Sprites;
	smokeColumnBack: Sprites;
	smokeColumnFront: Sprites;

	handEffectsCollection: SpriteCollection = new SpriteCollection();
	handHeldFireball: Sprites;
	handHeldSmoke: Sprites;
	handHeldFloatingSmoke: Sprites;
	smokeExtinguish: Sprites;
	handHeldFireRiseA: Sprites;
	handHeldFireRiseB: Sprites;
	handHeldFireRiseC: Sprites;
	smokePoofA: Sprites;
	smokePoofB: Sprites;
	smokePoofC: Sprites;
	smokePoofD: Sprites;
	smokePoofE: Sprites;


	allWindupEffects: SpriteCollection;
	backLayerEffects: SpriteCollection;

	players: Array<Character> = [];

	dragonSharedSounds: SoundManager;

	getPlayerTargetX(iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, playerIndex: number, players: Array<Character>) {
		const player: Character = players[playerIndex];

		let plateWidth = 0;
		if (player)
			plateWidth = iNameplateRenderer.getPlateWidth(context, player, playerIndex);

		let plateAdjust = 0;
		if (plateWidth) {
			plateAdjust -= plateWidth / 2 + ConditionManager.plateMargin;
		}

		return this.getPlayerX(playerIndex) + plateAdjust;
	}



	protected ShowWaitingForInitializationMessage(context: CanvasRenderingContext2D, fontColor: string, message: string, yTop: number) {
		context.font = '38px Arial';
		context.textAlign = 'left';
		context.textBaseline = 'top';
		context.fillStyle = fontColor;
		context.globalAlpha = 0.5;
		context.fillText(message, 100, yTop);
		context.globalAlpha = 1;
	}

	updateClock(clockData: string): void {
		let dto: any = JSON.parse(clockData);
		this.inCombat = dto.InCombat;
		this.inTimeFreeze = dto.InTimeFreeze;
		const timeStrs: string[] = dto.Time.split(',');
		this.dndTimeStr = timeStrs[0];
		this.dndDateStr = dto.Time.substr(timeStrs[0].length + 2).trim();
		this.updateClockFromDto(dto);
	}

	addFloatingText(xPos: number, text: string, fontColor: string, outlineColor: string, yPos = 1080): TextEffect {
		const textEffect: TextEffect = this.textAnimations.addText(new Vector(xPos, yPos), text, 3500);
		textEffect.fontColor = fontColor;
		textEffect.outlineColor = outlineColor;
		textEffect.scale = 1;
		textEffect.targetScale = 6;
		textEffect.fadeOutTime = 2500;
		textEffect.fadeInTime = 600;
		textEffect.velocityX = 0;
		if (yPos > 540)
			textEffect.velocityY = -6;
		else 
			textEffect.velocityY = 6;
		textEffect.verticalThrust = 1.3;
		return textEffect;
	}

	protected renderTextAnimations(context: CanvasRenderingContext2D, nowMs: number) {
		this.textAnimations.removeExpiredAnimations(nowMs);
		this.textAnimations.updatePositions(nowMs);
		this.textAnimations.render(context, nowMs);
	}

	hideClock = false;
	protected updateClockFromDto(dto) {
		this.hideClock = dto.HideClock;
	}

	protected triggerSoundEffect(dto): void {
	}

	protected triggerEmitter(dto, center: Vector): void {
	}

	protected triggerAnimationDto(dto, center: Vector) {
	}

	protected triggerPlaceholder(dto): void {
		console.log('triggerPlaceholder - dto: ' + dto);
	}

	protected showHealthGain(playerId: number, healthGain: number, isTempHitPoints: boolean) {
		if (healthGain < 0)
			return;
		const playerIndex: number = this.getPlayerIndex(playerId);

		let timeOffset = 0;
		let numPlusses: number = Math.round((healthGain + 4) / 5); // More health == more plusses!
		const maxPlusses = 10;
		if (numPlusses > maxPlusses)
			numPlusses = maxPlusses;
		for (let i = 0; i < numPlusses; i++) {
			this.launchPlus(playerId, playerIndex, isTempHitPoints, timeOffset);
			const fullSpinTime = 4000;
			timeOffset += fullSpinTime / 6 + fullSpinTime / 30;  // Each plus is *about* 1/6 of a full spin (plus a little more) behind the other.
		}
	}

	private launchPlus(playerId: number, playerIndex: number, isTempHitPoints: boolean, timeOffset: number) {
		let humanoidSizeScaleFactor = 1;
		let humanoidThrustScaleFactor = 1;
		let humanoidScaleOffset = 0;
		if (playerIndex === 0) {
			// It's Fred. Make it bigger and taller.
			humanoidSizeScaleFactor = 2.4;
			humanoidThrustScaleFactor = 2.6;
			humanoidScaleOffset = 0.06;
		}
		const x: number = this.getPlayerX(playerIndex);
		const center: Vector = new Vector(x, 1000);
		let hueShift = 220;
		if (isTempHitPoints)
			hueShift = 330;

		hueShift += Random.plusMinus(30);  // this.getHueShift(playerId)

		const saturation = 100;
		const brightness = 100;
		const horizontalFlip = false;
		const verticalFlip = false;
		const scale: number = 1.8 * humanoidSizeScaleFactor;
		const rotation = 0;
		const autoRotation = 0;
		const velocityX = 0;
		const velocityY = -0.14;
		const sprite: SpriteProxy = this.triggerAnimation('Health', center, 0, hueShift, saturation, brightness, horizontalFlip, verticalFlip, scale, rotation, autoRotation, velocityX, velocityY, 7500);
		sprite.fadeInTime = 400;
		sprite.fadeOutTime = 800;
		sprite.verticalThrustOverride = -0.09 * humanoidThrustScaleFactor;
		sprite.autoScaleFactorPerSecond = 0.89 + humanoidScaleOffset;
		sprite.timeStart = performance.now() + timeOffset;
		sprite.expirationDate += timeOffset;
	}

	getPlayer(playerID: number): Character {
		if (playerID < 0)
			return null;

		for (let i = 0; i < this.players.length; i++) {
			const player: Character = this.players[i];
			if (player.playerID === playerID)
				return player;
		}

		return null;
	}

	getHueShift(playerID: number): number {
		const player: Character = this.getPlayer(playerID);
		if (player)
			return player.hueShift;

		return 0;
	}

	protected triggerAnimation(spriteName: string, center: Vector, startFrameIndex: number, hueShift: number, saturation: number, brightness: number, horizontalFlip: boolean, verticalFlip: boolean, scale: number, rotation: number, autoRotation: number, velocityX = 0, velocityY = 0, lifespan = 0): SpriteProxy {
		let sprites: Sprites;
		for (let i = 0; i < this.allWindupEffects.allSprites.length; i++) {
			if (spriteName === this.allWindupEffects.allSprites[i].name) {
				sprites = this.allWindupEffects.allSprites[i];
				break;
			}
		}
		if (!sprites) {
			console.error(`"${spriteName}" sprite not found.`);
			return null;
		}
		else {
			let spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(center), startFrameIndex, hueShift, saturation, brightness, horizontalFlip, verticalFlip, scale, rotation, autoRotation, velocityX, velocityY);
			let sprite: SpriteProxy = spritesEffect.start();
			if (lifespan > 0)
				sprite.expirationDate = performance.now() + lifespan;
			return sprite;
		}
	}

	protected triggerSingleEffect(dto: any) {
		if (dto.timeOffsetMs > 0) {
			const offset: number = dto.timeOffsetMs;
			dto.timeOffsetMs = -1;
			setTimeout(this.triggerSingleEffect.bind(this), offset, dto);
			return;
		}

		if (dto.effectKind === EffectKind.SoundEffect) {
			this.triggerSoundEffect(dto);
			return;
		}

		if (dto.effectKind === EffectKind.Placeholder) {
			this.triggerPlaceholder(dto);
			return;
		}

		const center: Vector = this.getCenter(dto.target);

		if (dto.effectKind === EffectKind.Animation) {
			this.triggerAnimationDto(dto, center);
		}
		else if (dto.effectKind === EffectKind.Emitter)
			this.triggerEmitter(dto, center);
	}

	triggerEffect(effectData: string): void {
		let dto: any = JSON.parse(effectData);

		if (dto.effectKind === EffectKind.GroupEffect) {
			for (let i = 0; i < dto.effectsCount; i++) {
				this.triggerSingleEffect(dto.effects[i]);
			}
		}
		else {
			this.triggerSingleEffect(dto);
		}
	}


	private _inCombat: boolean;

	get inCombat(): boolean {
		return this._inCombat;
	}

	set inCombat(newValue: boolean) {
		if (this._inCombat == newValue)
			return;


		this._inCombat = newValue;
		if (this._inCombat) {
			this.enteringCombat();
		}
		else {
			this.exitingCombat();
		}
	}


	private _inTimeFreeze: boolean;

	get inTimeFreeze(): boolean {
		return this._inTimeFreeze;
	}

	set inTimeFreeze(newValue: boolean) {
		if (this._inTimeFreeze == newValue)
			return;


		this._inTimeFreeze = newValue;
		if (this._inTimeFreeze) {
			this.enteringTimeFreeze();
		}
		else {
			this.exitingTimeFreeze();
		}
	}

	clockLayerEffects: SpriteCollection;
	readonly clockMargin: number = 0;
	readonly clockOffsetX: number = -20;
	readonly clockBottomY: number = screenHeight - 26; // 232
	readonly clockScale: number = 0.52;
	static readonly ClockOriginX: number = 196;

	protected getClockX(): number {
		return screenWidth - this.clockScale * DragonGame.ClockOriginX - this.clockMargin + this.clockOffsetX;
	}

	protected getClockY(): number {
		return this.clockBottomY - 30;
	}

	protected drawClockLayerEffects(context: CanvasRenderingContext2D, now: number) {
		if (this.hideClock)
			return;
		this.clockLayerEffects.updatePositions(now);
		this.clockLayerEffects.draw(context, now);
	}

	exitingCombat() {
	}

	enteringCombat() {
	}


	exitingTimeFreeze() {
	}

	enteringTimeFreeze() {
	}

	loadResources() {
		super.loadResources();
		Folders.assets = 'GameDev/Assets/DragonH/';
		this.fireBallBack = new Sprites('FireBall/Back/BackFireBall', 88, fps30, AnimationStyle.Sequential, true);
		this.fireBallBack.name = 'FireBallBack';
		this.fireBallBack.originX = 190;
		this.fireBallBack.originY = 1080;

		this.fireBallFront = new Sprites('FireBall/Front/FireBallFront', 88, fps30, AnimationStyle.Sequential, true);
		this.fireBallFront.name = 'FireBallFront';
		this.fireBallFront.originX = 190;
		this.fireBallFront.originY = 1080;

		// TODO: Consider adding fireballs to another SpriteCollection. Not really windups.
		this.backLayerEffects.add(this.fireBallBack);
		this.backLayerEffects.add(this.fireBallFront);
		KnownSpellsEffects.initializeSpells();
		this.clockLayerEffects = new SpriteCollection();
		this.loadHandEffectResources();
	}

	loadSpell(spellName: string): Sprites {
		const spell: Sprites = new Sprites(`PlayerEffects/Spells/${spellName}/${spellName}${this.layerSuffix}`, 60, fps30, AnimationStyle.Loop, true);
		spell.name = spellName;
		spell.originX = 390;
		spell.originY = 232;
		spell.moves = true;
		this.allWindupEffects.add(spell);
		return spell;
	}

	loadHealthSpinUp(): Sprites {
		const health: Sprites = new Sprites(`PlayerEffects/Health/PlusSpin${this.layerSuffix}`, 120, fps30, AnimationStyle.Loop, true);
		health.name = 'Health';
		health.originX = 315;
		health.originY = 67;
		health.moves = true;
		this.allWindupEffects.add(health);
		return health;
	}

	loadWeapon(weaponName: string, animationName: string, originX: number, originY: number, frameCount = 91): Sprites {
		const weapon: Sprites = new Sprites(`Weapons/${weaponName}/${animationName}`, frameCount, fps30, AnimationStyle.Loop, true);
		weapon.name = weaponName + '.' + animationName;
		weapon.originX = originX;
		weapon.originY = originY;
		weapon.returnFrameIndex = 13;
		weapon.segmentSize = 57;
		this.allWindupEffects.add(weapon);
		return weapon;
	}

	constructor(context: CanvasRenderingContext2D) {
		super(context);
		this.allWindupEffects = new SpriteCollection();
		this.backLayerEffects = new SpriteCollection();
		this.dragonSharedSounds = new SoundManager('GameDev/Assets/DragonH/SoundEffects');
		this.leapEffectSoundManager = new SoundManager('GameDev/Assets/DragonH/SoundEffects/Leap Effects');
	}

	initializePlayerData(playerData: string): any {
		const characters: Array<Character> = JSON.parse(playerData);
		this.players = [];
		characters.forEach(character => { this.players.push(new Character(character)) }, this);
	}

	clearWindup(windupName: string): void {
		//console.log(`clearWindup(${windupName})`);
		// TODO: Use windupName to find specific sprites to clear.
		const now: number = performance.now();
		this.allWindupEffects.allSprites.forEach(function (sprites: Sprites) {
			sprites.spriteProxies.forEach(function (sprite: SpriteProxy) {
				const noNameSpecified = !windupName;
				const destroyAllSprites: boolean = windupName === '*';
				const asteriskIndex: number = windupName.indexOf('*');

				const spriteHasNoName: boolean = sprite.name === null || sprite.name === '';

				//if (!spriteHasNoName)
				//	console.log('sprite.name: ' + sprite.name);

				let nameMatches: boolean = sprite.name === windupName;

				if (!destroyAllSprites && asteriskIndex >= 0) {
					const firstPart: string = windupName.substr(0, asteriskIndex);
					const lastPart: string = windupName.substr(asteriskIndex + 1);
					let firstPartMatches: boolean;
					let lastPartMatches: boolean;
					if (firstPart)
						firstPartMatches = sprite.name && sprite.name.startsWith(firstPart);
					else
						firstPartMatches = true;
					if (lastPart)
						lastPartMatches = sprite.name && sprite.name.endsWith(lastPart);
					else
						lastPartMatches = true;
					nameMatches = firstPartMatches && lastPartMatches;
				}

				if (destroyAllSprites || noNameSpecified && spriteHasNoName || nameMatches) {
					sprite.expirationDate = now + sprite.fadeOutTime;
				}
			});
		});
	}

	castSpell(spellData: string): void {
		const spell: CastedSpellDataDto = JSON.parse(spellData);

		const playerX: number = this.getPlayerX(this.getPlayerIndex(spell.Target.CasterId));
		//this.addWindups(spell.Windups, playerX, `${spell.Spell.Name}(${spell.Spell.OwnerId})`);
		this.addWindups(spell.Windups, playerX, `(${spell.Spell.OwnerId})`);
	}

	addWindups(windups: Array<WindupData>, playerX: number = this.activePlayerX, name: string = null): void {
		//console.log('Adding Windups:');
		for (let i = 0; i < windups.length; i++) {
			const windup: WindupData = windups[i];
			if (windup === null) {
				console.error('windup == null');
				continue;
			}
			const sprites: Sprites = this.allWindupEffects.getSpritesByName(windup.Effect);
			if (!sprites)
				continue;
			const startingAngle: number = windup.DegreesOffset;
			const startingFrameIndex: number = startingAngle / 6 % 360;
			if (sprites) {
				let hue: number = windup.Hue;
				if (hue === -1)
					hue = Random.max(360);
				const sprite: SpriteProxy = sprites.addShifted(playerX + windup.Offset.x, 934 + windup.Offset.y, startingFrameIndex, hue, windup.Saturation, windup.Brightness);
				if (name)
					sprite.name = windup.Name + name;
				else
					sprite.name = windup.Name;
				//console.log('  Windup: "' + sprite.name + '"');
				sprite.opacity = windup.Opacity;

				if (windup.Lifespan)
					sprite.expirationDate = sprite.timeStart + windup.Lifespan;
				sprite.fadeInTime = windup.FadeIn;
				sprite.velocityX = windup.Velocity.x;
				sprite.velocityY = windup.Velocity.y;
				sprite.fadeOutTime = windup.FadeOut;
				sprite.playToEndOnExpire = windup.PlayToEndOnExpire;
				sprite.rotation = windup.Rotation;
				const gravityVector: Vector = new Vector(windup.Force.x, windup.Force.y).normalize(windup.ForceAmount);
				sprite.horizontalThrustOverride = gravityVector.x;
				sprite.verticalThrustOverride = gravityVector.y;
				sprite.scale = windup.Scale;
				sprite.autoRotationDegeesPerSecond = windup.AutoRotation;
				sprite.initialRotation = windup.Rotation;
				sprite.flipHorizontally = windup.FlipHorizontal;
				if (windup.StartSound)
					this.playWindupSound(windup.StartSound);
				if (windup.EndSound)
					sprite.onExpire = function () {
						this.playWindupSound(windup.EndSound);
					}.bind(this);
			}
		}
		console.log('');
	}

	addWindupFromStr(windupData: string, playerX: number = this.activePlayerX): void {
		const windups: Array<WindupData> = JSON.parse(windupData);
		this.addWindups(windups, playerX);
	}

	playWindupSound(soundFileName: string): void {
		this.dragonSharedSounds.safePlayMp3('Windups/' + soundFileName);
	}

	updateScreen(context: CanvasRenderingContext2D, nowMs: number) {
		this.backLayerEffects.updatePositions(nowMs);
		this.backLayerEffects.draw(context, nowMs);
		this.allWindupEffects.updatePositions(nowMs);
		this.allWindupEffects.draw(context, nowMs);
	}

	playerVideoLeftMargin = 10;
	playerVideoRightMargin = 1384;

	numberOfPlayers = 4;
	activePlayerX = -1;

	getPlayerX(playerIndex: number): number {
		const distanceForPlayerVideos: number = this.playerVideoRightMargin - this.playerVideoLeftMargin;
		const distanceBetweenPlayers: number = distanceForPlayerVideos / this.numberOfPlayers;
		const halfDistanceBetweenPlayers: number = distanceBetweenPlayers / 2;
		let horizontalNudge = 0;
		if (playerIndex === 0)  // Fred.
			horizontalNudge = 25;
		return playerIndex * distanceBetweenPlayers + halfDistanceBetweenPlayers + horizontalNudge;
	}

	getPlayerIndex(playerId: number): number {
		for (let i = 0; i < this.players.length; i++) {
			const player: Character = this.players[i];
			if (player.playerID === playerId) {
				return i;
			}
		}
		return -1;
	}

	getPlayerIndexFromName(playerName: string): number {
		const lowerName: string = playerName.toLowerCase().trim();
		for (let i = 0; i < this.players.length; i++) {
			const player: Character = this.players[i];
			if (player.name.toLowerCase() === lowerName) {
				return i;
			}
			if (player.firstName.toLowerCase() === lowerName) {
				return i;
			}
		}

		for (let i = 0; i < this.players.length; i++) {
			const player: Character = this.players[i];
			if (player.firstName.toLowerCase().startsWith(lowerName)) {
				return i;
			}
		}

		for (let i = 0; i < this.players.length; i++) {
			const player: Character = this.players[i];
			if (player.name.toLowerCase().endsWith(lowerName)) {
				return i;
			}
		}

		return -1;
	}

	getPlayerFirstName(playerId: number): string {
		for (let i = 0; i < this.players.length; i++) {
			const player: Character = this.players[i];
			if (player.playerID === playerId) {
				return player.firstName;
			}
		}
		return null;
	}

	playerChanged(playerId: number, pageID: number, playerData: string): void {
		const playerIndex: number = this.getPlayerIndex(playerId);
		if (playerIndex === -1)
			return;
		this.activePlayerX = this.getPlayerX(playerIndex);

		if (playerData) {
			const playerDto = JSON.parse(playerData);
			this.players[playerIndex].copyAttributesFrom(playerDto);
		}
	}

	getCenter(target: any): Vector {
		let result: Vector;
		if (target.targetType === TargetType.ScreenPosition)
			result = new Vector(target.screenPosition.x, target.screenPosition.y);
		else if (target.targetType === TargetType.ActivePlayer)
			result = new Vector(this.activePlayerX, 1080);
		else if (target.targetType === TargetType.ActiveEnemy)
			result = new Vector(1260, 1080);
		else if (target.targetType === TargetType.ScrollPosition)
			result = new Vector(150, 400);
		else
			result = new Vector(960, 540);

		return result.add(new Vector(target.targetOffset.x, target.targetOffset.y));
	}

	initialize() {
		let saveAssets: string = Folders.assets;
		super.initialize();
		let saveBypassFrameSkip: boolean = globalBypassFrameSkip;
		globalBypassFrameSkip = false;
		Folders.assets = 'GameDev/Assets/DragonH/';
		this.loadSpell('Smoke');
		this.loadSpell('Ghost');
		this.loadSpell('Fairy');
		this.loadSpell('Plasma');
		this.loadSpell('Fire');
		this.loadSpell('LiquidSparks');
		this.loadSpell('Narrow');
		this.loadSpell('Orb');
		this.loadSpell('Trails');
		this.loadSpell('Wide');
		this.loadHealthSpinUp();
		globalBypassFrameSkip = saveBypassFrameSkip;
		Folders.assets = saveAssets;
	}

	updateSkeletalData(skeletalData: string): void {
		this.skeletalData2d = JSON.parse(skeletalData) as SkeletalData2d;
		if (this.skeletalData2d) {
			if (this.skeletalData2d.HandEffect)
				this.changeHandEffects(this.skeletalData2d);
			this.checkForThrowsAndCatches(this.skeletalData2d, this.handHeldFireball);
			this.checkForThrowsAndCatches(this.skeletalData2d, this.handHeldSmoke);
		}
	}

	calibrationCursorSprite: SpriteProxy;
	calibrationDiscoverabilitySprite: SpriteProxy;
	calibrationPosition: string;
	calibrationCursor: Sprites;
	calibrationDiscoverability: Sprites;

	calibrateLeapMotion(calibrationData: string) {
		//console.log(`calibrateLeapMotion...`);
		const dto = JSON.parse(calibrationData);
		if (dto.DiscoverabilityIndex === -1) {
			this.calibrationCursor.spriteProxies = [];
			this.calibrationDiscoverability.spriteProxies = [];
			this.calibrationCursorSprite = null;
			this.calibrationDiscoverabilitySprite = null;
			this.calibrationPosition = null;
			return;
		}
		if (!this.calibrationCursorSprite) {
			this.calibrationCursorSprite = this.calibrationCursor.add(dto.X, dto.Y);
			this.calibrationDiscoverabilitySprite = this.calibrationDiscoverability.add(500, 350);
		}
		else {
			//console.log(`(${dto.X - this.calibrationCursor.originX}, ${dto.Y - this.calibrationCursor.originY})`);
			this.calibrationCursorSprite.x = dto.X - this.calibrationCursor.originX;
			this.calibrationCursorSprite.y = dto.Y - this.calibrationCursor.originY;
			this.calibrationCursorSprite.scale = dto.Scale;
			this.calibrationDiscoverabilitySprite.frameIndex = dto.DiscoverabilityIndex;
		}
		// X, Y, DiscoverabilityIndex
		// If  we are done and need to hide everything.
		this.calibrationPosition = `Fingertip: (${Math.round(dto.FingertipPosition.x)}, ${Math.round(dto.FingertipPosition.y)}, ${Math.round(dto.FingertipPosition.z)})`;
	}

	static readonly fingerWidth: number = 44;


	showDiagnosticsLiveHandPosition(context: CanvasRenderingContext2D, nowMs: number) {
		const handFillColors = ['#ff0000', '#0000ff'];
		const fingerFillColors = ['#ff8080', '#8080ff'];
		let colorIndex = 0;
		context.globalAlpha = 0.5;

		this.skeletalData2d.Hands.forEach((hand: Hand2d) => {

			context.fillStyle = handFillColors[colorIndex];
			context.beginPath();
			// TODO: Move DragonGame.fingerWidth and related code to a separate class.
			context.arc(hand.PalmPosition.X, hand.PalmPosition.Y, Math.max(2 * DragonGame.fingerWidth * hand.PalmPosition.Scale / 2.0, 20), 0, 2 * Math.PI);
			context.fill();

			context.fillStyle = '#ffffff';
			const fontSize: number = 32 * hand.PalmPosition.Scale;
			context.font = `${fontSize}px Arial`;
			context.fillText(VectorCompassDirection[hand.PalmDirection].toString(), hand.PalmPosition.X, hand.PalmPosition.Y);
			context.fillText((Math.round(hand.PositionZ * 100) / 100).toString(), hand.PalmPosition.X, hand.PalmPosition.Y + fontSize);

			hand.Fingers.forEach((finger: Finger2d) => {
				context.fillStyle = fingerFillColors[colorIndex];
				context.beginPath();
				// TODO: Move DragonGame.fingerWidth and related code to a separate class.
				context.arc(finger.TipPosition.X, finger.TipPosition.Y, Math.max(DragonGame.fingerWidth * finger.TipPosition.Scale / 2.0, 10), 0, 2 * Math.PI);
				context.fill();
			});

			colorIndex++;
			if (colorIndex >= handFillColors.length)
				colorIndex = 0;
		});

		if (this.skeletalData2d.ShowBackPlane) {
			this.drawPlane(context, this.skeletalData2d.BackPlane, '#800000');
		}

		if (this.skeletalData2d.ShowFrontPlane) {
			this.drawPlane(context, this.skeletalData2d.FrontPlane, '#000080');
		}

		context.globalAlpha = 1;
	}

	atOrigin(point: Point2D): boolean {
		if (!point)
			return true;
		return point.X === 0 && point.Y === 0;
	}

	drawPlane(context: CanvasRenderingContext2D, plane: ScaledPlane, fillStyle: string) {
		if (!this.atOrigin(plane.UpperLeft2D) && !this.atOrigin(plane.LowerRight2D)) {
			context.globalAlpha = 0.3;
			context.fillStyle = fillStyle;
			const width: number = plane.LowerRight2D.X - plane.UpperLeft2D.X;
			const height: number = plane.LowerRight2D.Y - plane.UpperLeft2D.Y;
			context.fillRect(plane.UpperLeft2D.X, plane.UpperLeft2D.Y, width, height);
		}

		if (!this.atOrigin(plane.UpperLeft2D)) {
			//console.log(`Upper left: (${plane.UpperLeft2D.X}, ${plane.UpperLeft2D.Y})`);
			this.drawPoint(context, plane.UpperLeft2D, plane.UpperLeft.Scale);
		}

		if (!this.atOrigin(plane.LowerRight2D)) {
			this.drawPoint(context, plane.LowerRight2D, plane.LowerRight.Scale);
		}

		context.globalAlpha = 1;
	}

	drawPoint(context: CanvasRenderingContext2D, point: Point2D, scale: number) {
		context.globalAlpha = 0.8;
		context.beginPath();
		context.arc(point.X, point.Y, DragonGame.fingerWidth * scale / 2, 0, 2 * Math.PI);
		context.fill();
		context.globalAlpha = 1;
	}

	loadHandEffectResources() {
		this.smokePoofA = new Sprites('LeapMotion/Effects/SmokePoof/SmokePoofA', 119, 30, AnimationStyle.Sequential, true);
		this.smokePoofA.originX = 305;
		this.smokePoofA.originY = 841;
		this.handEffectsCollection.add(this.smokePoofA);

		this.smokePoofB = new Sprites('LeapMotion/Effects/SmokePoof/SmokePoofB', 115, 30, AnimationStyle.Sequential, true);
		this.smokePoofB.originX = 332;
		this.smokePoofB.originY = 716;
		this.handEffectsCollection.add(this.smokePoofB);

		this.smokePoofC = new Sprites('LeapMotion/Effects/SmokePoof/SmokePoofC', 120, 30, AnimationStyle.Sequential, true);
		this.smokePoofC.originX = 333;
		this.smokePoofC.originY = 715;
		this.handEffectsCollection.add(this.smokePoofC);

		this.smokePoofD = new Sprites('LeapMotion/Effects/SmokePoof/SmokePoofD', 121, 30, AnimationStyle.Sequential, true);
		this.smokePoofD.originX = 321;
		this.smokePoofD.originY = 871;
		this.handEffectsCollection.add(this.smokePoofD);

		this.smokePoofE = new Sprites('LeapMotion/Effects/SmokePoof/SmokePoofE', 101, 30, AnimationStyle.Sequential, true);
		this.smokePoofE.originX = 311;
		this.smokePoofE.originY = 851;
		this.handEffectsCollection.add(this.smokePoofE);

		this.handHeldFireball = new Sprites('LeapMotion/Effects/FireBall/FireBall', 59, 30, AnimationStyle.Loop, true);
		this.handHeldFireball.originX = 317;
		this.handHeldFireball.originY = 647;
		this.handEffectsCollection.add(this.handHeldFireball);

		this.handHeldSmoke = new Sprites('LeapMotion/Effects/MagicSmokeLoop/MagicSmokeLoop', 326, 30, AnimationStyle.Loop, true);
		this.handHeldSmoke.originX = 225;
		this.handHeldSmoke.originY = 270;
		this.handEffectsCollection.add(this.handHeldSmoke);

		// TODO: Consider changing the floating smoke
		this.handHeldFloatingSmoke = new Sprites('LeapMotion/Effects/MagicSmokeLoop/MagicSmokeLoop', 326, 30, AnimationStyle.Loop, true);
		this.handHeldFloatingSmoke.originX = 225;
		this.handHeldFloatingSmoke.originY = 270;
		this.handHeldFloatingSmoke.moves = true;
		this.handHeldFloatingSmoke.disableGravity();
		this.handEffectsCollection.add(this.handHeldFloatingSmoke);

		this.smokeExtinguish = new Sprites('LeapMotion/Effects/FireBall/SmokeExtinguish', 76, 30, AnimationStyle.Sequential, true);
		this.smokeExtinguish.originX = 214;
		this.smokeExtinguish.originY = 662;
		this.handEffectsCollection.add(this.smokeExtinguish);

		this.handHeldFireRiseA = new Sprites('LeapMotion/Effects/FireBall/FireRiseA', 23, 30, AnimationStyle.Sequential, true);
		this.handHeldFireRiseA.originX = 192;
		this.handHeldFireRiseA.originY = 251;
		this.handHeldFireRiseA.moves = true;
		this.handHeldFireRiseA.disableGravity();
		this.handEffectsCollection.add(this.handHeldFireRiseA);

		this.handHeldFireRiseB = new Sprites('LeapMotion/Effects/FireBall/FireRiseB', 37, 30, AnimationStyle.Sequential, true);
		this.handHeldFireRiseB.originX = 116;
		this.handHeldFireRiseB.originY = 454;
		this.handHeldFireRiseB.moves = true;
		this.handHeldFireRiseB.disableGravity();
		this.handEffectsCollection.add(this.handHeldFireRiseB);

		this.handHeldFireRiseC = new Sprites('LeapMotion/Effects/FireBall/FireRiseC', 24, 30, AnimationStyle.Sequential, true);
		this.handHeldFireRiseC.originX = 238;
		this.handHeldFireRiseC.originY = 517;
		this.handHeldFireRiseC.moves = true;
		this.handHeldFireRiseC.disableGravity();
		this.handEffectsCollection.add(this.handHeldFireRiseC);
	}

	changeHandEffects(skeletalData2d: SkeletalData2d) {
		if (skeletalData2d.HandEffect.HandEffects)
			this.addHandEffects(skeletalData2d);
		if (skeletalData2d.HandEffect.KillFollowEffects !== TargetHand.None)
			this.killHandEffects(skeletalData2d.HandEffect.KillFollowEffects);
	}

	killHandEffects(KillFollowEffects: TargetHand) {
		// TODO: Implement this properly.	
		// HACK: 
		//this.destroyAllFireBalls();
	}

	destroyAllTrackingEffects() {
		if (this.handHeldFireball.spriteProxies.length > 0) {
			this.destroyAllFireballs();
		}

		if (this.handHeldSmoke.spriteProxies.length > 0) {
			// TODO: Play a sound effect in the Front overlay when the smoke extinguishes.
			this.handHeldSmoke.spriteProxies = [];
		}
	}

	fireBallSound: string;
	activeLeapFireballSound: HTMLAudioElement;

	private destroyAllFireballs() {
		if (this instanceof DragonFrontGame)
			this.leapEffectSoundManager.safePlayMp3('FireBallExtinguish');

		if (this.skeletalData2d.Hands.length > 0) {
			const firstHand: Hand2d = this.skeletalData2d.Hands[0];
			const attachPoint: ScaledPoint = this.getHandAttachPoint(firstHand);
			const addEffectToThisCanvas = this.shouldAddEffectToThisCanvas(firstHand, attachPoint);
			if (addEffectToThisCanvas) {
				const fireBall: ColorShiftingSpriteProxy = this.handHeldFireball.spriteProxies[0] as ColorShiftingSpriteProxy;

				if (fireBall) {
					const scale: number = fireBall.scale;
					const smokeExtinguish: SpriteProxy = this.smokeExtinguish.addShifted(fireBall.x + this.handHeldFireball.originX, fireBall.y + this.handHeldFireball.originY, 0, fireBall.hueShift);
					smokeExtinguish.scale = scale;
				}
			}
		}

		this.handHeldFireball.spriteProxies = [];
		this.activeLeapFireballSound.pause();
		this.activeLeapFireballSound = null;
	}

	addFireballSound() {
		if (this instanceof DragonFrontGame)
			this.leapEffectSoundManager.safePlayMp3('FireBallIgnite');

		if (this.activeLeapFireballSound)
			this.activeLeapFireballSound.pause();

		this.activeLeapFireballSound = this.leapEffectSoundManager.safePlayMp3ReturnAudio(this.fireBallSound);
	}

	addHandEffects(skeletalData2d: SkeletalData2d) {
		if (skeletalData2d.Hands.length === 0)
			return;

		skeletalData2d.HandEffect.HandEffects.forEach((handEffect: HandEffectDto) => {
			const firstHand: Hand2d = skeletalData2d.Hands[0];
			const attachPoint: ScaledPoint = this.getHandAttachPoint(firstHand);
			let addEffectToThisCanvas = this.shouldAddEffectToThisCanvas(firstHand, attachPoint);

			switch (handEffect.EffectName) {
				case 'SmokeA':
					if (addEffectToThisCanvas)
						this.addScaledSprite(this.smokePoofA, handEffect, attachPoint);
					break;
				case 'SmokeB':
					if (addEffectToThisCanvas)
						this.addScaledSprite(this.smokePoofB, handEffect, attachPoint);
					break;
				case 'SmokeC':
					if (addEffectToThisCanvas)
						this.addScaledSprite(this.smokePoofC, handEffect, attachPoint);
					break;
				case 'SmokeD':
					if (addEffectToThisCanvas)
						this.addScaledSprite(this.smokePoofD, handEffect, attachPoint);
					break;
				case 'SmokeE':
					if (addEffectToThisCanvas)
						this.addScaledSprite(this.smokePoofE, handEffect, attachPoint);
					break;
				case 'FireBall': {
					this.addTrackedSpritesToBothCanvases(skeletalData2d, this.handHeldFireball, handEffect, attachPoint);
					this.addFireballSound();
					break;
				}
				case 'MagicSmokeLoop': {
					this.addTrackedSpritesToBothCanvases(skeletalData2d, this.handHeldSmoke, handEffect, attachPoint);
					break;
				}
			}
		});
	}

	private shouldAddEffectToThisCanvas(firstHand: Hand2d, attachPoint: ScaledPoint) {
		const onFrontCanvas: boolean = this.getOnFrontCanvasFromHandPosition(firstHand, attachPoint);
		let addEffectToThisCanvas = false;
		if (onFrontCanvas && this instanceof DragonFrontGame)
			addEffectToThisCanvas = true;
		else if (!onFrontCanvas && this instanceof DragonBackGame)
			addEffectToThisCanvas = true;
		return addEffectToThisCanvas;
	}

	addTrackedSpritesToBothCanvases(skeletalData2d: SkeletalData2d, handHeldFireball: Sprites, handEffect: HandEffectDto, scaledPoint: ScaledPoint): SpriteProxy {
		const trackedSprite: SpriteProxy = this.addScaledSprite(handHeldFireball, handEffect, scaledPoint);
		trackedSprite.fadeInTime = 650;
		trackedSprite.data = new HandFollowingData(skeletalData2d.Hands[0].Side);
		return trackedSprite;
	}

	addScaledSprite(smokePoof: Sprites, handEffect: HandEffectDto, scaledPoint: ScaledPoint): SpriteProxy {
		const sprite: SpriteProxy = smokePoof.addShifted(scaledPoint.X, scaledPoint.Y, 0, handEffect.HueShift);
		this.setHandEffectScale(sprite, handEffect, scaledPoint.Scale);
		return sprite;
	}

	private setHandEffectScale(sprite: SpriteProxy, handEffect: HandEffectDto, scale: number) {
		(sprite as unknown as ScaleFactor).scaleFactor = handEffect.Scale;
		sprite.scale = handEffect.Scale * scale;
	}

	static readonly outOfBounds: number = -1000;

	static readonly outOfBoundsPt: ScaledPoint = new ScaledPoint(DragonGame.outOfBounds, DragonGame.outOfBounds, 1);

	private getFirstHandAttachPoint(skeletalData2d: SkeletalData2d): ScaledPoint {
		if (skeletalData2d.Hands.length > 0) {
			return this.getHandAttachPoint(skeletalData2d.Hands[0]);
		}
		return DragonGame.outOfBoundsPt;
	}

	getHand(skeletalData2d: SkeletalData2d, handSide: HandSide): Hand2d {
		for (let i = 0; i < skeletalData2d.Hands.length; i++) {
			if (skeletalData2d.Hands[i].Side === handSide) {
				return skeletalData2d.Hands[i];
			}
		}
		return null;
	}


	getHandPositionFromSide(skeletalData2d: SkeletalData2d, handSide: HandSide): ScaledPoint {
		const hand: Hand2d = this.getHand(skeletalData2d, handSide);

		if (hand !== null)
			return this.getHandAttachPoint(hand);

		return DragonGame.outOfBoundsPt;
	}

	getFloatingPositionFromSide(skeletalData2d: SkeletalData2d, handSide: HandSide): ScaledPoint {
		const hand: Hand2d = this.getHand(skeletalData2d, handSide);

		if (hand !== null)
			return this.getFloatingAttachPoint(hand);

		return DragonGame.outOfBoundsPt;
	}


	nextChildSpriteRiseCreationTime = 0;

	private getHandAttachPoint(hand: Hand2d): ScaledPoint {
		return this.inBoundsScaledPoint(hand.PalmAttachPoint);
	}

	private getFloatingAttachPoint(hand: Hand2d): ScaledPoint {
		return this.inBoundsScaledPoint(hand.FloatingAttachPoint);
	}

	showingHandSpeed = false;

	private inBoundsScaledPoint(scaledPoint: ScaledPoint): ScaledPoint {
		return new ScaledPoint(scaledPoint.X, scaledPoint.Y, Math.max(0.2, scaledPoint.Scale));
	}

	checkForThrowsAndCatches(skeletalData2d: SkeletalData2d, sprites: Sprites) {
		// TODO: Support multiple FireBalls/effects.
		if (sprites.spriteProxies.length === 0)
			return;
		const handFollowingData: HandFollowingData = sprites.spriteProxies[0].data as HandFollowingData;
		if (!handFollowingData)
			return;

		const hand: Hand2d = this.getHand(skeletalData2d, handFollowingData.HandSide);
		if (hand) {
			if (hand.Throwing) {
				handFollowingData.TrackingObjectIndex = hand.ThrownObjectIndex;
				//console.log(`THROWING!!!`);
				//this.addFloatingText(hand.PalmAttachPoint.X, 'Throwing ' + VectorCompassDirection[hand.ThrowDirection].toString(), '#800040', '#ffffff', hand.PalmAttachPoint.Y);
			}
			this.checkForCatch(hand, handFollowingData);
			if (hand.JustCaught)
				return;
		}

		const otherHand: Hand2d = this.getHand(skeletalData2d, this.getOtherSide(handFollowingData.HandSide));
		if (otherHand)
			this.checkForCatch(otherHand, handFollowingData);
	}

	getOtherSide(handSide: HandSide): HandSide {
		if (handSide === HandSide.Left)
			return HandSide.Right;
		else
			return HandSide.Left;
	}

	checkForCatch(hand: Hand2d, handFollowingData: HandFollowingData): void {
		if (hand.JustCaught) {
			handFollowingData.TrackingObjectIndex = -1;
			handFollowingData.HandSide = hand.Side;
			//this.addFloatingText(hand.PalmAttachPoint.X, 'Caught!', '#800040', '#ffffff', hand.PalmAttachPoint.Y);
		}
	}

	activeFireballVolumeFactor = 1;

	adjustFireballSound(positionZ: number) {
		if (!this.activeLeapFireballSound)
			return;
		const backPlane = 380;
		const frontPlane = 40;
		const percentVolumeBackPlane: number = (MathEx.clamp(positionZ, frontPlane, backPlane) - frontPlane) / (backPlane - frontPlane);
		const percentVolumeFrontPlane: number = 1 - percentVolumeBackPlane;
		let percentVolume: number;
		if (this instanceof DragonFrontGame) {
			percentVolume = percentVolumeFrontPlane;
		}
		else {
			percentVolume = percentVolumeBackPlane;
		}
		percentVolume = MathEx.toFixed(percentVolume, 1);
		if (this.activeLeapFireballSound.volume !== percentVolume)
			this.activeLeapFireballSound.volume = this.activeFireballVolumeFactor * percentVolume;
	}

	updateTrackingEffects(skeletalData2d: SkeletalData2d, sprites: Sprites, nowMs: number, getChildFloaterSprite: (x: number, y: number) => ColorShiftingSpriteProxy): void {
		const spriteProxies: SpriteProxy[] = sprites.spriteProxies;
		if (!this.hasTrackingEffects(spriteProxies))
			return;

		// TODO: This code (spriteProxies[0]) assumes only one fireball - add support for more:
		const sprite: ColorShiftingSpriteProxy = spriteProxies[0] as ColorShiftingSpriteProxy;

		const handFollowingData: HandFollowingData = sprite.data as HandFollowingData;
		if (!handFollowingData) {
			console.log(`No hand following data`);
			return;
		}

		const hand: Hand2d = this.getHand(skeletalData2d, handFollowingData.HandSide);

		//console.log(`hand.Speed: ${hand.Speed} ${VectorCompassDirection[hand.SpeedDirection].toString()}`);

		if (this.showingHandSpeed && hand) {
			this.showHandSpeed(hand);
		}

		let pos: ScaledPoint;
		const isThrown: boolean = handFollowingData.TrackingObjectIndex >= 0;
		let thrownObject: ThrownObject = null;

		if (isThrown)
			thrownObject = this.getThrownObjectFromIndex(skeletalData2d, handFollowingData.TrackingObjectIndex);

		if (thrownObject) {
			pos = thrownObject.Position;
			if (Math.abs(sprite.x - pos.X) > 5000) {
				this.destroyAllTrackingEffects();
				return;
			}
		}
		else {
			//pos = this.getHandPositionFromSide(skeletalData2d, handFollowingData.HandSide);
			pos = this.getFloatingPositionFromSide(skeletalData2d, handFollowingData.HandSide);

			if (hand && hand.IsFist) {
				this.destroyAllTrackingEffects();
				return;
			}
		}

		if (pos === DragonGame.outOfBoundsPt || pos.Y > 1480) {
			this.destroyAllTrackingEffects();
			return;
		}


		if (thrownObject)
			this.adjustFireballSound(thrownObject.PositionZ);
		else if (hand)
			this.adjustFireballSound(hand.PositionZ);

		let opacity: number;
		const onFrontCanvas: boolean = this.getOnFrontCanvas(isThrown, hand, thrownObject, pos);

		if (onFrontCanvas) {
			// We want to draw on the front plane.
			if (this instanceof DragonBackGame)
				opacity = 0;
			else
				opacity = 1;
		}
		else {
			// We want to draw on the back plane.
			if (this instanceof DragonFrontGame)
				opacity = 0;
			else
				opacity = 1;
		}

		sprite.opacity = opacity;

		if (opacity === 0)
			return;

		spriteProxies.forEach((trackedSprite: ColorShiftingSpriteProxy) => {
			trackedSprite.x = pos.X - sprites.originX;
			trackedSprite.y = pos.Y - sprites.originY;
			trackedSprite.scale = pos.Scale * (trackedSprite as unknown as ScaleFactor).scaleFactor;

			if (getChildFloaterSprite && nowMs > this.nextChildSpriteRiseCreationTime) {
				this.floatChild(sprites, trackedSprite, getChildFloaterSprite);
			}
		});

		if (getChildFloaterSprite)
			this.nextChildSpriteRiseCreationTime = nowMs + Random.between(5, 50);
	}

	private floatChild(parentSprites: Sprites, parentSprite: ColorShiftingSpriteProxy, getChildFlameSprite: (x: number, y: number) => ColorShiftingSpriteProxy) {
		const fireBallDiameter = 150;
		const fireBallRadius: number = fireBallDiameter / 2;
		const fireBallScaledRadius: number = fireBallRadius * parentSprite.scale;
		const fireRiseX: number = parentSprite.x + parentSprites.originX + Random.between(-fireBallScaledRadius, fireBallScaledRadius);

		const fireBallRiseHeight = 92;
		const fireballEmitterRectHeight = 82;
		const fireballEmitterRectHalfHeight = fireballEmitterRectHeight / 2;
		const fireballEmitterRectHalfHeightScaled = fireballEmitterRectHalfHeight * parentSprite.scale;
		const fireRiseY: number = parentSprite.y + parentSprites.originY - fireBallRiseHeight * parentSprite.scale + Random.between(-fireballEmitterRectHalfHeightScaled, fireballEmitterRectHalfHeightScaled);

		const childSprite: ColorShiftingSpriteProxy = getChildFlameSprite(fireRiseX, fireRiseY);
		//parentSprites.setXY(childSprite, fireRiseX, fireRiseY);
		childSprite.hueShift = parentSprite.hueShift + Random.plusMinus(30)
		childSprite.scale *= parentSprite.scale * Random.between(0.8, 1.2);
		childSprite.velocityY = -3.7 * parentSprite.scale * Random.between(0.9, 1.1);
	}

	private getOnFrontCanvas(isThrown: boolean, hand: Hand2d, thrownObject: ThrownObject, pos: ScaledPoint) {
		if (isThrown)
			return this.getOnFrontCanvasFromThrownObject(hand, thrownObject);
		else
			return this.getOnFrontCanvasFromHandPosition(hand, pos);
	}

	private getOnFrontCanvasFromHandPosition(hand: Hand2d, pos: ScaledPoint): boolean {
		return hand.FacingForwardOrBack === VectorCompassDirection.Forward || (pos.X > 1135 && pos.X < 1329);
	}

	private getOnFrontCanvasFromThrownObject(hand: Hand2d, thrownObject: ThrownObject) {
		const midPlaneZ = 330;
		if (hand)
			return hand.PositionZ > thrownObject.PositionZ;
		else
			return thrownObject.PositionZ < midPlaneZ;
	}

	private showHandSpeed(hand: Hand2d) {
		const barThickness = 5;
		let xPos: number;
		const screenHeight = 1080;
		const screenWidth = 1920;
		const barHeight: number = screenHeight * MathEx.clamp(hand.Speed / 2000, 0, 1);
		const yPos: number = 1080 - barHeight;
		this.context.fillStyle = '#ff0000';
		if (hand.Side === HandSide.Left)
			xPos = 0;
		else
			xPos = screenWidth - barThickness;
		this.context.fillRect(xPos, yPos, barThickness, barHeight);
	}

	getThrownObjectFromIndex(skeletalData2d: SkeletalData2d, TrackingObjectIndex: number): ThrownObject {
		if (!skeletalData2d.ThrownObjects || skeletalData2d.ThrownObjects.length === 0)
			return null;

		for (let i = 0; i < skeletalData2d.ThrownObjects.length; i++) {
			if (skeletalData2d.ThrownObjects[i].Index === TrackingObjectIndex)
				return skeletalData2d.ThrownObjects[i];
		}
		return null;
	}

	private hasTrackingEffects(spriteProxies: SpriteProxy[]) {
		return spriteProxies.length > 0;
	}

	updateSkeletalTrackingEffects(context: CanvasRenderingContext2D, nowMs: number) {
		if (this.skeletalData2d) {
			this.updateTrackingEffects(this.skeletalData2d, this.handHeldFireball, nowMs, this.getChildFlameSprite.bind(this));
			this.updateTrackingEffects(this.skeletalData2d, this.handHeldSmoke, nowMs, this.getChildSmokeSprite.bind(this));
			this.handEffectsCollection.updatePositions(nowMs);
			this.handEffectsCollection.draw(context, nowMs);
		}
	}

	private getChildFlameSprite(x: number, y: number): ColorShiftingSpriteProxy {
		let childSprites: Sprites;
		if (Random.chancePercent(33))
			childSprites = this.handHeldFireRiseA;
		if (Random.chancePercent(50))
			childSprites = this.handHeldFireRiseB;
		else
			childSprites = this.handHeldFireRiseC;

		return childSprites.addShifted(x, y, 0);
	}

	private getChildSmokeSprite(x: number, y: number): ColorShiftingSpriteProxy {
		const childSprite: ColorShiftingSpriteProxy = this.handHeldFloatingSmoke.addShifted(x, y, -1);
		childSprite.expirationDate = performance.now() + 900;
		childSprite.fadeOutTime = 400;
		childSprite.scale = Random.between(0.2, 0.45);
		childSprite.opacity = 0.7;
		childSprite.initialRotation = Random.max(360);
		childSprite.autoRotationDegeesPerSecond = Random.between(-10, 10);
		childSprite.fadeInTime = 300;
		return childSprite;
	}
}
