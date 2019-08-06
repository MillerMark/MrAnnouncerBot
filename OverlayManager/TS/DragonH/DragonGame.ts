abstract class DragonGame extends GamePlusQuiz {
	abstract layerSuffix: string;
	spellGhost: Sprites;
	spellFairy: Sprites;
	spellPlasma: Sprites;
	spellSmoke: Sprites;
	spellFire: Sprites;
	spellNarrow: Sprites;
	spellOrb: Sprites;
	spellTrails: Sprites;
	spellWide: Sprites;
	spellLiquidSparks: Sprites;
	spellWindupEffects: SpriteCollection;

	players: Array<Character> = [];


	loadSpell(spellName: string): Sprites {
		let spell: Sprites = new Sprites(`PlayerEffects/Spells/${spellName}/${spellName}${this.layerSuffix}`, 60, fps30, AnimationStyle.Loop, true);
		spell.name = spellName;
		spell.originX = 390;
		spell.originY = 232;
		spell.moves = true;
		this.spellWindupEffects.add(spell);
		return spell;
	}

	constructor(context: CanvasRenderingContext2D) {
		super(context);
		this.spellWindupEffects = new SpriteCollection();
	}

	initializePlayerData(playerData: string): any {
		let characters: Array<Character> = JSON.parse(playerData);
		this.players = [];
		characters.forEach(function (character) { this.players.push(new Character(character)) }, this);
	}

	clearWindup(windupName: string): void {
		this.spellWindupEffects.allSprites.forEach(function (sprites: Sprites) { sprites.sprites = [] });
	}

	addWindup(windupData: string): void {
		let windups: Array<WindupData> = JSON.parse(windupData);

		for (let i = 0; i < windups.length; i++) {
			let windup: WindupData = windups[i];
			let sprites: Sprites = this.spellWindupEffects.getSpritesByName(windup.Effect);
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
		this.spellWindupEffects.updatePositions(now);
		this.spellWindupEffects.draw(context, now);
	}

	initialize() {
		let saveAssets: string = Folders.assets;
		super.initialize();
		Folders.assets = 'GameDev/Assets/DragonH/';
		this.spellSmoke = this.loadSpell('Smoke');
		this.spellGhost = this.loadSpell('Ghost');
		this.spellFairy = this.loadSpell('Fairy');
		this.spellPlasma = this.loadSpell('Plasma');
		this.spellFire = this.loadSpell('Fire');
		this.spellLiquidSparks = this.loadSpell('LiquidSparks');
		this.spellNarrow = this.loadSpell('Narrow');
		this.spellOrb = this.loadSpell('Orb');
		this.spellTrails = this.loadSpell('Trails');
		this.spellWide = this.loadSpell('Wide');
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
		return playerIndex * distanceBetweenPlayers + halfDistanceBetweenPlayers;
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
		this.activePlayerX = this.getPlayerX(playerId);
		let playerIndex: number = this.getPlayerIndex(playerId);
		if (playerIndex === -1)
			return;

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
