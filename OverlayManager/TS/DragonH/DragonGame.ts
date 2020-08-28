interface IGetPlayerX {
	getPlayerX(playerIndex: number): number;
	getPlayerIndex(playerId: number): number;
	getPlayerFirstName(playerId: number): string;
	getPlayerTargetX(iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, iGetPlayerX: IGetPlayerX & ITextFloater, playerIndex: number, players: Array<Character>);
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
	static maxFiltersPerWindup = 6;
	abstract layerSuffix: string;
	dndTimeStr: string;
	dndDateStr: string;

	fireBallBack: Sprites;
	fireBallFront: Sprites;
	smokeColumnBack: Sprites;
	smokeColumnFront: Sprites;

	allWindupEffects: SpriteCollection;
	backLayerEffects: SpriteCollection;

	players: Array<Character> = [];

	dragonSharedSounds: SoundManager;

	getPlayerTargetX(iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, iGetPlayerX: IGetPlayerX & ITextFloater, playerIndex: number, players: Array<Character>) {
		const player: Character = players[playerIndex];

		let plateWidth = 0;
		if (player)
			plateWidth = iNameplateRenderer.getPlateWidth(context, player, playerIndex);

		let plateAdjust = 0;
		if (plateWidth) {
			plateAdjust -= plateWidth / 2 + ConditionManager.plateMargin;
		}

		return iGetPlayerX.getPlayerX(playerIndex) + plateAdjust;
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

	protected updateClockFromDto(dto) {
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
	}

	loadSpell(spellName: string): Sprites {
		let spell: Sprites = new Sprites(`PlayerEffects/Spells/${spellName}/${spellName}${this.layerSuffix}`, 60, fps30, AnimationStyle.Loop, true);
		spell.name = spellName;
		spell.originX = 390;
		spell.originY = 232;
		spell.moves = true;
		this.allWindupEffects.add(spell);
		return spell;
	}

	loadHealthSpinUp(): Sprites {
		let health: Sprites = new Sprites(`PlayerEffects/Health/PlusSpin${this.layerSuffix}`, 120, fps30, AnimationStyle.Loop, true);
		health.name = 'Health';
		health.originX = 315;
		health.originY = 67;
		health.moves = true;
		this.allWindupEffects.add(health);
		return health;
	}

	loadWeapon(weaponName: string, animationName: string, originX: number, originY: number): Sprites {
		const weapon: Sprites = new Sprites(`Weapons/${weaponName}/${animationName}`, 91, fps30, AnimationStyle.Loop, true);
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
	}

	initializePlayerData(playerData: string): any {
		const characters: Array<Character> = JSON.parse(playerData);
		this.players = [];
		characters.forEach(character => { this.players.push(new Character(character)) }, this);
	}

	clearWindup(windupName: string): void {
		console.log(`clearWindup(${windupName})`);
		// TODO: Use windupName to find specific sprites to clear.
		const now: number = performance.now();
		this.allWindupEffects.allSprites.forEach(function (sprites: Sprites) {
			sprites.sprites.forEach(function (sprite: SpriteProxy) {
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
		console.log('Adding Windups:');
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
				console.log('  Windup: "' + sprite.name + '"');
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
}
