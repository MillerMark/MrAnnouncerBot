abstract class DragonGame extends GamePlusQuiz {
	abstract layerSuffix: string;
	fireBallBack: Sprites;
	fireBallFront: Sprites;

	allWindupEffects: SpriteCollection;
	backLayerEffects: SpriteCollection;

	players: Array<Character> = [];

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
	}

	initializePlayerData(playerData: string): any {
		let characters: Array<Character> = JSON.parse(playerData);
		this.players = [];
		characters.forEach(function (character) { this.players.push(new Character(character)) }, this);
	}

	clearWindup(windupName: string): void {
		// TODO: Use windupName to find specific sprites to clear.
		let now: number = performance.now();
		this.allWindupEffects.allSprites.forEach(function (sprites: Sprites) {
			sprites.sprites.forEach(function (sprite: SpriteProxy) {
				sprite.expirationDate = now + sprite.fadeOutTime;
			});
		});
	}

	addWindup(windupData: string): void {
		let windups: Array<WindupData> = JSON.parse(windupData);

		for (let i = 0; i < windups.length; i++) {
			let windup: WindupData = windups[i];
			let sprites: Sprites = this.allWindupEffects.getSpritesByName(windup.Effect);
			if (!sprites)
				continue;
			let startingAngle: number = windup.DegreesOffset;
			let startingFrameIndex: number = startingAngle / 6 % 360;
			if (sprites) {
				let hue: number = windup.Hue;
				if (hue == -1)
					hue = Random.max(360);
				let sprite: SpriteProxy = sprites.addShifted(this.activePlayerX + windup.Offset.x, 934 + windup.Offset.y, startingFrameIndex, hue, windup.Saturation, windup.Brightness);
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
				if (windup.SoundFileName)
					this.playWindupSound(windup.SoundFileName);
			}
		}
	}

	playWindupSound(soundFileName: string): void {
		// Do nothing - let descendants override to see who is going to play the sound.
	}

	updateScreen(context: CanvasRenderingContext2D, now: number) {
		this.backLayerEffects.updatePositions(now);
		this.backLayerEffects.draw(context, now);
		this.allWindupEffects.updatePositions(now);
		this.allWindupEffects.draw(context, now);
	}

	initialize() {
		let saveAssets: string = Folders.assets;
		super.initialize();
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
		Folders.assets = saveAssets;
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
}
