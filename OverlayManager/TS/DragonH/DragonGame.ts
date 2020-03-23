enum SpellTargetType {
	Player,
	Location,
	Enemy
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

class SpellTarget {
	Target: SpellTargetType;
	PlayerId: number;
	Location: Vector;
	Range: number;
	constructor() {

	}
}

class CastedSpellDataDto {
	Spell: Spell;
	Target: SpellTarget;
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
			let spellEffect: SpellEffect = this.spellEffects[i];
			if (spellEffect.name === name)
				return spellEffect;
		}
	}

	static initializeSpells(): void {
		this.addSpell(KnownSpellsEffects.shieldOfFaith());
		this.addSpell(KnownSpellsEffects.sanctuary());
	}

	static getEffect(effectName: string, hue: number): WindupData {
		let effect: WindupData = new WindupData(effectName, hue);
		return effect;
	}

	static shieldOfFaith(): SpellEffect {
		let spell: SpellEffect = new SpellEffect('Shield of Faith');
		let part1: WindupData = KnownSpellsEffects.getEffect('Wide', -15);
		let part2: WindupData = KnownSpellsEffects.getEffect('Wide', 15);
		let part3: WindupData = KnownSpellsEffects.getEffect('Wide', 45);

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
		let spell: SpellEffect = new SpellEffect('Sanctuary');
		let part1: WindupData = KnownSpellsEffects.getEffect('LiquidSparks', 170);
		let part2: WindupData = KnownSpellsEffects.getEffect('LiquidSparks', 200);
		let part3: WindupData = KnownSpellsEffects.getEffect('LiquidSparks', 230);

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


abstract class DragonGame extends GamePlusQuiz {
	abstract layerSuffix: string;
	sprinkles: Sprinkles;

	fireBallBack: Sprites;
	fireBallFront: Sprites;

	allWindupEffects: SpriteCollection;
	backLayerEffects: SpriteCollection;

	players: Array<Character> = [];

	dragonSharedSounds: SoundManager;

	protected ShowWaitingForInitializationMessage(context: CanvasRenderingContext2D, fontColor: string, message: string, yTop: number) {
		context.font = '38px Arial';
		context.textAlign = 'left';
		context.textBaseline = 'top';
		context.fillStyle = fontColor;
		context.globalAlpha = 0.5;
		context.fillText(message, 100, yTop);
		context.globalAlpha = 1;
	}

	animateSprinkles(commandData: string): any {
		this.sprinkles.executeCommand(commandData, performance.now());
	}

	drawSprinkles(context: CanvasRenderingContext2D, now: number, layer: Layer): void {
		this.sprinkles.draw(context, now, layer);
	}

	protected triggerSoundEffect(dto: any): void {
	}

	protected triggerEmitter(dto: any, center: Vector): void {
	}

	protected triggerAnimationDto(dto: any, center: Vector) {
	}

	protected triggerPlaceholder(dto: any): any {
		console.log('triggerPlaceholder - dto: ' + dto);
	}

	protected showHealthGain(playerId: number, healthGain: number, isTempHitPoints: boolean) {
		if (healthGain < 0)
			return;
		let playerIndex: number = this.getPlayerIndex(playerId);

		let timeOffset: number = 0;
		let numPlusses: number = Math.round((healthGain + 4) / 5); // More health == more plusses!
		for (let i = 0; i < numPlusses; i++) {
			this.launchPlus(playerId, playerIndex, isTempHitPoints, timeOffset);
			const fullSpinTime: number = 4000;
			timeOffset += fullSpinTime / 6 + fullSpinTime / 30;  // Each plus is *about* 1/6 of a full spin (plus a little more) behind the other.
		}
	}

	private launchPlus(playerId: number, playerIndex: number, isTempHitPoints: boolean, timeOffset: number) {
		let humanoidSizeScaleFactor: number = 1;
		let humanoidThrustScaleFactor: number = 1;
		let humanoidScaleOffset: number = 0;
		if (playerIndex == 0) {
			// It's Fred. Make it bigger and taller.
			humanoidSizeScaleFactor = 2.4;
			humanoidThrustScaleFactor = 2.6;
			humanoidScaleOffset = 0.06;
		}
		let x: number = this.getPlayerX(playerIndex);
		let center: Vector = new Vector(x, 1000);
		let hueShift: number = 220;
		if (isTempHitPoints)
			hueShift = 330;

		hueShift += Random.plusMinus(30);  // this.getHueShift(playerId)

		let saturation: number = 100;
		let brightness: number = 100;
		let horizontalFlip: boolean = false;
		let verticalFlip: boolean = false;
		let scale: number = 1.8 * humanoidSizeScaleFactor;
		let rotation: number = 0;
		let autoRotation: number = 0;
		let velocityX: number = 0;
		let velocityY: number = -0.14;
		let sprite: SpriteProxy = this.triggerAnimation('Health', center, 0, hueShift, saturation, brightness, horizontalFlip, verticalFlip, scale, rotation, autoRotation, velocityX, velocityY, 7500);
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

		for (var i = 0; i < this.players.length; i++) {
			let player: Character = this.players[i];
			if (player.playerID == playerID)
				return player;
		}

		return null;
	}

	getHueShift(playerID: number): number {
		let player: Character = this.getPlayer(playerID);
		if (player)
			return player.hueShift;

		return 0;
	}

	protected triggerAnimation(spriteName: string, center: Vector, startFrameIndex: number, hueShift: number, saturation: number, brightness: number, horizontalFlip: boolean, verticalFlip: boolean, scale: number, rotation: number, autoRotation: number, velocityX: number = 0, velocityY: number = 0, lifespan: number = 0): SpriteProxy {
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
			let offset: number = dto.timeOffsetMs;
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

		let center: Vector = this.getCenter(dto.target);

		if (dto.effectKind === EffectKind.Animation)
			this.triggerAnimationDto(dto, center);
		else if (dto.effectKind === EffectKind.Emitter)
			this.triggerEmitter(dto, center);
	}

	triggerEffect(effectData: string): void {
		let dto: any = JSON.parse(effectData);
		console.log(dto);

		if (dto.effectKind === EffectKind.GroupEffect) {
			for (var i = 0; i < dto.effectsCount; i++) {
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

	exitingCombat() {
	}

	enteringCombat() {
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
		let weapon: Sprites = new Sprites(`Weapons/${weaponName}/${animationName}`, 91, fps30, AnimationStyle.Loop, true);
		weapon.name = weaponName + '.' + animationName;
		weapon.originX = originX;
		weapon.originY = originY;
		weapon.returnFrameIndex = 11;
		weapon.segmentSize = 60;
		this.allWindupEffects.add(weapon);
		return weapon;
	}

	constructor(context: CanvasRenderingContext2D) {
		super(context);
		this.allWindupEffects = new SpriteCollection();
		this.backLayerEffects = new SpriteCollection();
		this.dragonSharedSounds = new SoundManager('GameDev/Assets/DragonH/SoundEffects');
		this.sprinkles = new Sprinkles();
	}

	initializePlayerData(playerData: string): any {
		let characters: Array<Character> = JSON.parse(playerData);
		this.players = [];
		characters.forEach(character => { this.players.push(new Character(character)) }, this);
	}

	clearWindup(windupName: string): void {
		console.log(`clearWindup(${windupName})`);
		// TODO: Use windupName to find specific sprites to clear.
		let now: number = performance.now();
		this.allWindupEffects.allSprites.forEach(function (sprites: Sprites) {
			sprites.sprites.forEach(function (sprite: SpriteProxy) {
				let noNameSpecified: boolean = !windupName;
				let destroyAllSprites: boolean = windupName === '*';
				let asteriskIndex: number = windupName.indexOf('*');

				let spriteHasNoName: boolean = sprite.name === null || sprite.name === '';
				let nameMatches: boolean = sprite.name === windupName;

				if (!destroyAllSprites && asteriskIndex >= 0) {
					let firstPart: string = windupName.substr(0, asteriskIndex);
					let lastPart: string = windupName.substr(asteriskIndex + 1);
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

				if (destroyAllSprites || noNameSpecified && spriteHasNoName || nameMatches)
					sprite.expirationDate = now + sprite.fadeOutTime;
			});
		});
	}

	castSpell(spellData: string): void {
		let spell: CastedSpellDataDto = JSON.parse(spellData);

		let playerX: number = this.getPlayerX(this.getPlayerIndex(spell.Target.PlayerId));
		//this.addWindups(spell.Windups, playerX, `${spell.Spell.Name}(${spell.Spell.OwnerId})`);
		this.addWindups(spell.Windups, playerX, `(${spell.Spell.OwnerId})`);
	}

	addWindups(windups: Array<WindupData>, playerX: number = this.activePlayerX, name: string = null): void {
		console.log('Adding Windups:');
		for (let i = 0; i < windups.length; i++) {
			let windup: WindupData = windups[i];
			if (windup === null) {
				console.error('windup == null');
				continue;
			}
			let sprites: Sprites = this.allWindupEffects.getSpritesByName(windup.Effect);
			if (!sprites)
				continue;
			let startingAngle: number = windup.DegreesOffset;
			let startingFrameIndex: number = startingAngle / 6 % 360;
			if (sprites) {
				let hue: number = windup.Hue;
				if (hue == -1)
					hue = Random.max(360);
				let sprite: SpriteProxy = sprites.addShifted(playerX + windup.Offset.x, 934 + windup.Offset.y, startingFrameIndex, hue, windup.Saturation, windup.Brightness);
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
				let gravityVector: Vector = new Vector(windup.Force.x, windup.Force.y).normalize(windup.ForceAmount);
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
		let windups: Array<WindupData> = JSON.parse(windupData);
		this.addWindups(windups, playerX);
	}

	playWindupSound(soundFileName: string): void {
		this.dragonSharedSounds.safePlayMp3('Windups/' + soundFileName);
	}

	updateScreen(context: CanvasRenderingContext2D, now: number) {
		this.backLayerEffects.updatePositions(now);
		this.backLayerEffects.draw(context, now);
		this.allWindupEffects.updatePositions(now);
		this.allWindupEffects.draw(context, now);
	}

	playerVideoLeftMargin = 10;
	playerVideoRightMargin = 1384;

	numberOfPlayers: number = 4;
	activePlayerX: number = -1;

	getPlayerX(playerIndex: number): number {
		let distanceForPlayerVideos: number = this.playerVideoRightMargin - this.playerVideoLeftMargin;
		let distanceBetweenPlayers: number = distanceForPlayerVideos / this.numberOfPlayers;
		let halfDistanceBetweenPlayers: number = distanceBetweenPlayers / 2;
		let horizontalNudge: number = 0;
		if (playerIndex == 0)  // Fred.
			horizontalNudge = 25;
		return playerIndex * distanceBetweenPlayers + halfDistanceBetweenPlayers + horizontalNudge;
	}

	getPlayerIndex(playerId: number): number {
		for (var i = 0; i < this.players.length; i++) {
			let player: Character = this.players[i];
			if (player.playerID == playerId) {
				return i;
			}
		}
		return -1;
	}

	playerChanged(playerId: number, pageID: number, playerData: string): void {
		let playerIndex: number = this.getPlayerIndex(playerId);
		if (playerIndex === -1)
			return;
		this.activePlayerX = this.getPlayerX(playerIndex);

		if (playerData) {
			let playerDto: any = JSON.parse(playerData);
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
