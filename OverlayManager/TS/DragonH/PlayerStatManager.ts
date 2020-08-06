interface IPlayerStats {
	ReadyToRollDice: boolean;
	PlayerId: number;
	Vantage: VantageKind;
	MainDieSides: number;
	DiceStack: Array<DiceStackDto>;
}


interface IAllPlayerStats {
	LatestCommand: string;
	LatestData: string;
	RollingTheDiceNow: boolean;
	ActiveTurnCreatureID: number;
	Players: Array<PlayerStats>;
}

class DiceStackDto {
	NumSides: number;
	HueShift: number;
	Multiplier: number;
	DamageType: DamageType;

	matches(compareDiceStack: DiceStackDto) {
		return this.DamageType === compareDiceStack.DamageType &&
			this.HueShift === compareDiceStack.HueShift &&
			this.Multiplier === compareDiceStack.Multiplier &&
			this.NumSides === compareDiceStack.NumSides;
	}
}

class PlayerStats implements IPlayerStats {
	ReadyToRollDice: boolean;
	PlayerId: number;
	Vantage: VantageKind;
	MainDieSides = 20;
	DiceStack: Array<DiceStackDto> = [];

	deserialize(playerStatsDto: IPlayerStats): PlayerStats {
		if (!playerStatsDto)
			return this;

		this.PlayerId = playerStatsDto.PlayerId;
		this.Vantage = playerStatsDto.Vantage;
		this.ReadyToRollDice = playerStatsDto.ReadyToRollDice;
		this.MainDieSides = playerStatsDto.MainDieSides;
		this.DiceStack = playerStatsDto.DiceStack;
		return this;
	}

	onlyVantageHasChanged(latestPlayerStats: PlayerStats) {
		return this.PlayerId === latestPlayerStats.PlayerId &&
			this.Vantage !== latestPlayerStats.Vantage &&
			this.ReadyToRollDice === latestPlayerStats.ReadyToRollDice &&
			this.diceMatch(latestPlayerStats);
	}

	diceMatch(latestPlayerStats: PlayerStats) {
		return this.MainDieSides === latestPlayerStats.MainDieSides &&
			this.dieStacksMatch(latestPlayerStats.DiceStack);
	}

	dieStacksMatch(compareDiceStack: DiceStackDto[]) {
		if (compareDiceStack.length !== this.DiceStack.length)
			return false;
		for (let i = 0; i < compareDiceStack.length; i++) {
			if (!this.DiceStack[i].matches(compareDiceStack[i]))
				return false;
		}
		return true;
	}

	constructor() {

	}
}

class AllPlayerStats implements IAllPlayerStats {
	LatestCommand: string;
	LatestData: string;
	RollingTheDiceNow: boolean;
	Players: Array<PlayerStats> = [];
	ActiveTurnCreatureID: number;
	readyToRollFullDragon: Sprites;
	readyToRollDieSmoke: Sprites;
	readyToRollDragonBreath: Sprites;
	readyToRollLightDie: Sprites;
	readyToRollDarkDie: Sprites;
	readyToRollDragonHands: Sprites;
	readyToRollDieCollection: SpriteCollection;
	airExplosionCollection: SpriteCollection;

	nameplateTopLong: Sprites;
	nameplateTopMedium: Sprites;
	nameplateTopShort: Sprites;
	nameplateRight: Sprites;
	nameplateLeft: Sprites;

	deserialize(allPlayerStatsDto: IAllPlayerStats): AllPlayerStats {
		this.LatestCommand = allPlayerStatsDto.LatestCommand;
		this.LatestData = allPlayerStatsDto.LatestData;
		this.RollingTheDiceNow = allPlayerStatsDto.RollingTheDiceNow;
		this.ActiveTurnCreatureID = allPlayerStatsDto.ActiveTurnCreatureID;
		this.Players = [];
		for (let i = 0; i < allPlayerStatsDto.Players.length; i++) {
			//console.log(allPlayerStatsDto.Players[i]);
			this.Players.push(new PlayerStats().deserialize(allPlayerStatsDto.Players[i]));
		}

		//console.log(this);
		return this;
	}

	handleCommand(iGetPlayerX: IGetPlayerX, iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, soundManager: ISoundManager, mostRecentPlayerStats: AllPlayerStats, players: Array<Character>) {
		this.LatestData = mostRecentPlayerStats.LatestData;
		this.LatestCommand = mostRecentPlayerStats.LatestCommand;
		this.RollingTheDiceNow = mostRecentPlayerStats.RollingTheDiceNow;
		this.setActiveTurnCreatureID(iGetPlayerX, iNameplateRenderer, context, mostRecentPlayerStats.ActiveTurnCreatureID, players);
		this.addMissingPlayers(mostRecentPlayerStats);
		this.handleCommandForExistingPlayers(iGetPlayerX, soundManager, mostRecentPlayerStats);
		this.cleanUpNonExistantPlayers(mostRecentPlayerStats);
	}

	static readonly nameplateHighlightTop: number = 1080 - 48;

	private static readonly longPlateArcWidth: number = 374;
	private static readonly longPlateTotalWidth: number = 576;
	private static readonly mediumPlateArcWidth: number = 292;
	private static readonly mediumPlateTotalWidth: number = 480;
	private static readonly shortPlateArcWidth: number = 153;
	private static readonly shortPlateTotalWidth: number = 355;

	setActiveTurnCreatureID(iGetPlayerX: IGetPlayerX, iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, activeTurnCreatureID: number, players: Array<Character>) {
		if (this.ActiveTurnCreatureID === activeTurnCreatureID)
			return;

		this.ActiveTurnCreatureID = activeTurnCreatureID;
		this.cleanUpAllActiveTurnHighlighting();

		if (this.ActiveTurnCreatureID < 0) {  // No players have an active turn right now.
			return;
		}

		const playerIndex: number = iGetPlayerX.getPlayerIndex(this.ActiveTurnCreatureID);
		const x: number = iGetPlayerX.getPlayerX(playerIndex);
		const player: Character = players[playerIndex];
		const plateWidth: number = iNameplateRenderer.getPlateWidth(context, player, playerIndex);

		let sprites: Sprites;
		let horizontalScale = 1;
		if (plateWidth <= AllPlayerStats.shortPlateArcWidth * 1.2) {
			sprites = this.nameplateTopShort;
			horizontalScale = plateWidth / AllPlayerStats.shortPlateArcWidth;
		}
		else if (plateWidth >= AllPlayerStats.longPlateArcWidth * 0.8) {
			sprites = this.nameplateTopLong;
			horizontalScale = plateWidth / AllPlayerStats.longPlateArcWidth;
		}
		else {
			sprites = this.nameplateTopMedium;
			horizontalScale = plateWidth / AllPlayerStats.mediumPlateArcWidth;
		}

		const nameplateSprites: Sprites = sprites;
		const nameplateHighlightSprite: SpriteProxy = nameplateSprites.addShifted(x, AllPlayerStats.nameplateHighlightTop, -1, player.hueShift);
		nameplateHighlightSprite.horizontalScale = horizontalScale;

		const nameplateRightAdjust = -4;
		const nameplateLeftAdjust = 3;
		this.nameplateLeft.addShifted(x - plateWidth / 2 + nameplateLeftAdjust, AllPlayerStats.nameplateHighlightTop, -1, player.hueShift);
		this.nameplateRight.addShifted(x + plateWidth / 2 + nameplateRightAdjust, AllPlayerStats.nameplateHighlightTop, -1, player.hueShift);
	}

	cleanUpAllActiveTurnHighlighting() {
		console.log('cleanUpAllActiveTurnHighlighting');
		for (let i = 0; i < this.nameplateHighlightCollection.allSprites.length; i++) {
			const sprites: SpriteProxy[] = this.nameplateHighlightCollection.allSprites[i].sprites;
			for (let j = 0; j < sprites.length; j++) {
				sprites[j].fadeOutNow(500);
			}
		}
	}

	nameplateHighlightCollection: SpriteCollection = new SpriteCollection();

	private loadNameplateHighlight(fileName: string, originX = 0, originY = 0): Sprites {
		const nameplateHighlight: Sprites = new Sprites(`Nameplates/ActiveTurn/${fileName}`, 94, fps30, AnimationStyle.Loop, true);
		nameplateHighlight.originX = originX;
		nameplateHighlight.originY = originY;
		this.nameplateHighlightCollection.add(nameplateHighlight);
		return nameplateHighlight;
	}

	loadResources() {
		this.nameplateTopLong = this.loadNameplateHighlight('NameplateTopLong', 306, 147);
		this.nameplateTopMedium = this.loadNameplateHighlight('NameplateTopMedium', 254, 145);
		this.nameplateTopShort = this.loadNameplateHighlight('NameplateTopShort', 195, 144);
		this.nameplateLeft = this.loadNameplateHighlight('NameplateLeft', 152, 123);
		this.nameplateRight = this.loadNameplateHighlight('NameplateRight', 119, 123);

		this.readyToRollDieCollection = new SpriteCollection();
		this.airExplosionCollection = new SpriteCollection();

		this.readyToRollFullDragon = this.createReadyToRollSprites('FullDragon', 281, 265);

		this.readyToRollDieSmoke = new Sprites(`PlayerReadyToRollDice/DieSmoke`, 87, fps30, AnimationStyle.Sequential, true);
		this.readyToRollDieSmoke.originX = 48;
		this.readyToRollDieSmoke.originY = -18;

		this.readyToRollDragonBreath = new Sprites(`PlayerReadyToRollDice/DragonBreath`, 250, fps30, AnimationStyle.Sequential, true);
		this.readyToRollDragonBreath.originX = 184;
		this.readyToRollDragonBreath.originY = 387;

		this.createReadyToRollVantageDieSprites('Disadvantage');
		this.createReadyToRollVantageDieSprites('Advantage');
		this.createReadyToRollDieSprites('d4Damage', -18, 0);
		this.createReadyToRollDieSprites('d6Damage', -18, -4);
		this.createReadyToRollDieSprites('d8Damage', -12, 0);
		this.createReadyToRollDieSprites('d10Damage', -8, 0);
		this.createReadyToRollDieSprites('d12Damage', -7, 0);

		this.readyToRollDarkDie = this.createReadyToRollDieSprites('DarkDie');
		this.readyToRollLightDie = this.createReadyToRollDieSprites('LightDie');
		this.createReadyToRollDieSprites('Willy');
		this.createReadyToRollDieSprites('Fred');
		this.createReadyToRollDieSprites('Miles');
		this.createReadyToRollDieSprites('Lady');
		this.createReadyToRollDieSprites('Merkin');
		this.createReadyToRollDieSprites('Cutie');

		this.readyToRollDragonHands = this.createReadyToRollSprites('DragonHands', 41, 22);

		this.createAirExplosion('A', 261, 274, 78);
		this.createAirExplosion('B', 375, 360, 65);
		this.createAirExplosion('C', 372, 372, 68);

		/* 
				DarkDie:
					Left = 251
					Top = 257

				LightDie:
					Left = 254
					Top = 260

				DragonHands:
					Left = 259
					Top = 265

				FullDragon:
					Left = 19
					Top = 22

		 * */
	}

	createAirExplosion(fileName: string, originX: number, originY: number, frameCount: number): Sprites {
		const sprites: Sprites = new Sprites(`Dice/AirExplosion/${fileName}/AirExplosion${fileName}`, frameCount, fps30, AnimationStyle.Sequential, true);
		sprites.originX = originX;
		sprites.originY = originY;
		this.airExplosionCollection.add(sprites);
		return sprites;
	}

	private createReadyToRollDieSprites(fileName: string, xOffset = 0, yOffset = 0): Sprites {
		const dieSprites: Sprites = this.createReadyToRollSprites(fileName, 48 + xOffset, 29 + yOffset);
		dieSprites.name = fileName;
		this.readyToRollDieCollection.add(dieSprites);
		return dieSprites;
	}

	private createReadyToRollVantageDieSprites(fileName: string): Sprites {
		const dieSprites: Sprites = this.createReadyToRollSprites(fileName, 48, 92);
		dieSprites.name = fileName.substr(fileName.indexOf('/') + 1);
		this.readyToRollDieCollection.add(dieSprites);
		return dieSprites;
	}

	private createReadyToRollSprites(fileName: string, originX: number, originY: number): Sprites {
		const sprites: Sprites = new Sprites(`PlayerReadyToRollDice/${fileName}`, 29, fps30, AnimationStyle.Loop, true);
		sprites.originX = originX;
		sprites.originY = originY;
		sprites.moves = true;
		sprites.disableGravity();
		return sprites;
	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
		this.nameplateHighlightCollection.draw(context, nowMs);
		this.readyToRollFullDragon.draw(context, nowMs);
		this.readyToRollDieCollection.draw(context, nowMs);
		this.readyToRollDragonHands.draw(context, nowMs);
		this.airExplosionCollection.draw(context, nowMs);
		this.readyToRollDieSmoke.draw(context, nowMs);
		this.readyToRollDragonBreath.draw(context, nowMs);
	}

	timeOfLastDragonBreath = 0;
	timeOfNextDragonBreath = -1;

	update(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, nowMs: number) {
		this.readyToRollFullDragon.updatePositions(nowMs);
		this.readyToRollDieCollection.updatePositions(nowMs);
		this.readyToRollDragonHands.updatePositions(nowMs);
		this.airExplosionCollection.updatePositions(nowMs);
		if (this.timeOfNextDragonBreath < nowMs) {
			this.timeOfNextDragonBreath = nowMs + Random.between(4000, 7000);
			this.breathFire(iGetPlayerX, soundManager, nowMs);
		}
	}

	breathFire(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, nowMs: number) {
		if (this.readyToRollFullDragon.sprites.length === 0)
			return;
		const dragonIndex: number = Math.floor(Random.max(this.readyToRollFullDragon.sprites.length));
		const sprite: SpriteProxy = this.readyToRollFullDragon.sprites[dragonIndex];
		if (sprite.easePointStillActive(nowMs) || sprite.velocityY !== 0 || sprite.verticalThrustOverride !== 0)
			return;

		const deltaX = 140;
		const deltaY = 260 - 46 * sprite.scale;
		const playerId: number = sprite.data as number;
		const x: number = iGetPlayerX.getPlayerX(iGetPlayerX.getPlayerIndex(playerId));
		const dragonBreath: SpriteProxy = this.readyToRollDragonBreath.addShifted(x + deltaX, sprite.y + deltaY, 0, Random.max(360));
		dragonBreath.scale = sprite.scale;
		dragonBreath.data = sprite.data;

		if (dragonBreath.scale < 0.65)
			soundManager.safePlayMp3('DiceDragons/FireBreathSmall[3]');
		else if (dragonBreath.scale < 0.85)
			soundManager.safePlayMp3('DiceDragons/FireBreathMedium[3]');
		else
			soundManager.safePlayMp3('DiceDragons/FireBreathLarge[5]');
	}

	private cleanUpNonExistantPlayers(mostRecentPlayerStats: AllPlayerStats) {
		for (let i = 0; i < this.Players.length; i++) {
			const mostRecentPlayerStat: PlayerStats = mostRecentPlayerStats.getPlayerStatsById(this.Players[i].PlayerId);
			if (!mostRecentPlayerStat) {
				// At this point, the data sent excludes one of the players we are tracking. Remove it if the command says to do so.
			}
		}
	}

	private handleCommandForExistingPlayers(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, latestPlayerStats: AllPlayerStats) {
		for (let i = 0; i < this.Players.length; i++) {
			const latestPlayerStat: PlayerStats = latestPlayerStats.getPlayerStatsById(this.Players[i].PlayerId);
			if (latestPlayerStat)
				this.handleCommandForPlayer(iGetPlayerX, soundManager, this.Players[i], this.LatestCommand, this.LatestData, latestPlayerStat);
		}
	}

	handleCommandForPlayer(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, playerStats: PlayerStats, command: string, data: string, latestPlayerStats: PlayerStats) {
		//console.log('command: ' + command);
		switch (command) {
			case 'Update':
				this.updatePlayer(iGetPlayerX, soundManager, playerStats, data, latestPlayerStats);
				break;
		}
	}

	launchTheDragons(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, playerId: number, vantage: VantageKind, mainDieSides: number, diceStack: Array<DiceStackDto>) {
		const shoulderOffset = 140;
		const playerIndex: number = iGetPlayerX.getPlayerIndex(playerId);
		const playerX: number = iGetPlayerX.getPlayerX(playerIndex) + shoulderOffset;
		const minScale = 0.4;
		const maxScale = 1.2;
		const scale: number = Random.between(minScale, maxScale);
		//const scale = 1.2;
		let frameInterval: number = fps30;

		const dragonHueShift = Random.max(360);
		const dieShift = 0; // Random.max(360);
		const startFrameIndex: number = Random.max(this.readyToRollFullDragon.baseAnimation.frameCount);
		const dragonStartY = 1180;

		const dieOffset = 16;
		if (scale >= 0.8) {
			frameInterval = fps30;
		}
		else if (scale >= 0.7) {
			frameInterval = fps40;
		}
		else if (scale >= 0.6) {
			frameInterval = fps50;
		}
		else if (scale >= 0.5) {
			frameInterval = fps60;
		}
		else {
			frameInterval = fps70;
		}

		const dieSprites: Sprites = this.getMainDieForPlayer(iGetPlayerX, playerId, mainDieSides);

		const yStopVariance: number = Random.between(-25, 25);

		const fullDragon: SpriteProxy = this.readyToRollFullDragon.addShifted(playerX, dragonStartY + yStopVariance, startFrameIndex, dragonHueShift);
		const die: SpriteProxy = dieSprites.addShifted(playerX, dragonStartY + dieOffset + yStopVariance, startFrameIndex, dieShift);
		const disadvantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Disadvantage");
		const advantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Advantage");
		const dieDistanceDown = 103;
		const disadvantageDie: SpriteProxy = disadvantageDieSprites.addShifted(playerX, dragonStartY + dieOffset + yStopVariance + dieDistanceDown, startFrameIndex, dieShift);
		const advantageDie: SpriteProxy = advantageDieSprites.addShifted(playerX, dragonStartY + dieOffset + yStopVariance + dieDistanceDown, startFrameIndex, dieShift);

		advantageDie.opacity = 0;
		disadvantageDie.opacity = 0;

		switch (vantage) {
			case VantageKind.Advantage:
				advantageDie.opacity = 1;
				break;
			case VantageKind.Disadvantage:
				disadvantageDie.opacity = 1;
				break;
		}

		const dragonHands: SpriteProxy = this.readyToRollDragonHands.addShifted(playerX, dragonStartY + yStopVariance, startFrameIndex, dragonHueShift);

		this.prepareForEntrance(fullDragon, playerId, scale, frameInterval);
		this.prepareForEntrance(die, playerId, 1, frameInterval);
		this.prepareForEntrance(advantageDie, playerId, 1, frameInterval);
		this.prepareForEntrance(disadvantageDie, playerId, 1, frameInterval);
		this.prepareForEntrance(dragonHands, playerId, scale, frameInterval);
		soundManager.safePlayMp3('DiceDragons/DragonScreech[23]');
	}

	getMainDieForPlayer(iGetPlayerX: IGetPlayerX, playerId: number, mainDieSides: number): Sprites {
		console.log('mainDieSides: ' + mainDieSides);
		let dieSprites: Sprites;
		if (mainDieSides === 20)
			dieSprites = this.getDieSpritesForPlayer(iGetPlayerX, playerId);
		else if (mainDieSides === 12)
			dieSprites = this.readyToRollDieCollection.getSpritesByName('d12Damage');
		else if (mainDieSides === 10)
			dieSprites = this.readyToRollDieCollection.getSpritesByName('d10Damage');
		else if (mainDieSides === 8)
			dieSprites = this.readyToRollDieCollection.getSpritesByName('d8Damage');
		else if (mainDieSides === 6)
			dieSprites = this.readyToRollDieCollection.getSpritesByName('d6Damage');
		else if (mainDieSides === 4)
			dieSprites = this.readyToRollDieCollection.getSpritesByName('d4Damage');
		if (!dieSprites)
			if (Random.chancePercent(50))
				dieSprites = this.readyToRollLightDie;
			else
				dieSprites = this.readyToRollDarkDie;

		return dieSprites;
	}

	prepareForEntrance(sprite: SpriteProxy, playerId: number, scale = 1, frameInterval = fps30) {
		const dragonRestingHeight = 420;
		sprite.data = playerId;
		sprite.scale = scale;
		sprite.frameIntervalOverride = frameInterval;
		sprite.ease(performance.now(), sprite.x, sprite.y, sprite.x, sprite.y - dragonRestingHeight, 1000);
	}

	private getDieSpritesForPlayer(iGetPlayerX: IGetPlayerX, playerId: number) {
		let playerFirstName: string = iGetPlayerX.getPlayerFirstName(playerId);
		if (playerFirstName)
			if (playerFirstName === "L'il")
				playerFirstName = 'Cutie';
		return this.readyToRollDieCollection.getSpritesByName(playerFirstName);
	}

	dropDieAndBlowUpTheDragons(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, playerId: number, mainDieSides: number, diceStack: Array<DiceStackDto>) {
		const dieSprites: Sprites = this.getMainDieForPlayer(iGetPlayerX, playerId, mainDieSides);
		if (dieSprites)
			this.dropSpriteByPlayerId(dieSprites.sprites, playerId);
		else {
			this.dropSpriteByPlayerId(this.readyToRollDarkDie.sprites, playerId);
			this.dropSpriteByPlayerId(this.readyToRollLightDie.sprites, playerId);
		}

		const disadvantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Disadvantage");
		const advantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Advantage");
		this.dropSpriteByPlayerId(disadvantageDieSprites.sprites, playerId);
		this.dropSpriteByPlayerId(advantageDieSprites.sprites, playerId);

		const verticalThrust = -3.3;
		const horizontalThrust: number = Random.between(-1.7, 1.7);
		this.ascendDragons(this.readyToRollFullDragon, playerId, horizontalThrust, verticalThrust);
		this.ascendDragons(this.readyToRollDragonHands, playerId, horizontalThrust, verticalThrust);

		const dragonSprite: SpriteProxy = this.getDragonSpriteByPlayerId(this.readyToRollFullDragon.sprites, playerId);
		this.showAirExplosionLater(dragonSprite, soundManager);
		this.fadeOutSpriteByPlayerId(this.readyToRollDragonBreath.sprites, playerId);
	}

	ascendDragons(dragonSprites: Sprites, playerId: number, horizontalThrust: number, verticalThrust: number) {
		for (let i = 0; i < dragonSprites.sprites.length; i++) {
			const sprite: SpriteProxy = dragonSprites.sprites[i];
			if (sprite.data === playerId) {
				sprite.verticalThrustOverride = verticalThrust;
				sprite.horizontalThrustOverride = horizontalThrust;
				sprite.clearEasePoint();
				sprite.changeVelocity(0, 0, performance.now());
			}
		}
	}

	showAirExplosionLater(dragonSprite: SpriteProxy, soundManager: ISoundManager) {
		const timeout: number = Random.between(800, 2500);
		setTimeout(this.showAirExplosion.bind(this), timeout, dragonSprite, soundManager);
	}

	showAirExplosion(dragonSprite: SpriteProxy, soundManager: ISoundManager) {
		if (!dragonSprite)
			return;
		const playerId: number = dragonSprite.data as number;
		this.hideDragonByPlayerId(this.readyToRollFullDragon.sprites, playerId);
		this.hideDragonByPlayerId(this.readyToRollDragonHands.sprites, playerId);

		const airExplosionIndex: number = Math.floor(Random.max(this.airExplosionCollection.allSprites.length));
		this.airExplosionCollection.allSprites[airExplosionIndex].add(dragonSprite.x + this.readyToRollFullDragon.originX, dragonSprite.y + this.readyToRollFullDragon.originY);

		soundManager.safePlayMp3('DieBurst[5]');

		// HACK: Feels wrong. Fixes a state bug, but we have to clear *after* the dragon dice have dropped. I think we are okay because it's only volatile for 1.8 seconds after the DM presses the button to roll the dice.
		for (let i = 0; i < this.Players.length; i++) {
			this.Players[i].ReadyToRollDice = false;
			this.Players[i].Vantage = VantageKind.Normal;
		}
	}

	descendTheDragons(iGetPlayerX: IGetPlayerX, playerId: number, mainDieSides: number, diceStack: Array<DiceStackDto>) {
		this.dropSpriteByPlayerId(this.readyToRollFullDragon.sprites, playerId);
		const dieSprites: Sprites = this.getMainDieForPlayer(iGetPlayerX, playerId, mainDieSides);
		if (dieSprites)
			this.dropSpriteByPlayerId(dieSprites.sprites, playerId);
		else {
			this.dropSpriteByPlayerId(this.readyToRollDarkDie.sprites, playerId);
			this.dropSpriteByPlayerId(this.readyToRollLightDie.sprites, playerId);
		}

		this.dropSpriteFromCollectionByPlayerId(this.readyToRollDieCollection, playerId);
		this.dropSpriteByPlayerId(this.readyToRollDragonHands.sprites, playerId);
		this.fadeOutSpriteByPlayerId(this.readyToRollDragonBreath.sprites, playerId);
	}

	dropSpriteFromCollectionByPlayerId(dieCollection: SpriteCollection, playerId: number) {
		for (let i = 0; i < dieCollection.allSprites.length; i++) {
			const sprites: Sprites = dieCollection.allSprites[i];
			this.dropSpriteByPlayerId(sprites.sprites, playerId);
		}
	}

	dieThrowVelocity = 3;

	getDragonSpriteByPlayerId(sprites: SpriteProxy[], playerId: number): SpriteProxy {
		for (let i = 0; i < sprites.length; i++) {
			if (sprites[i].data === playerId) {
				return sprites[i];
			}
		}
		return null;
	}

	hideDragonByPlayerId(sprites: SpriteProxy[], playerId: number) {
		const fadeOutTime = 500;
		for (let i = 0; i < sprites.length; i++) {
			if (sprites[i].data === playerId) {
				sprites[i].fadeOutNow(fadeOutTime);
				sprites[i].changeVelocity(0, -this.dieThrowVelocity, performance.now());
			}
		}
	}

	dropSpriteByPlayerId(sprites: SpriteProxy[], playerId: number) {
		for (let i = 0; i < sprites.length; i++) {
			if (sprites[i].data === playerId) {
				sprites[i].clearEasePoint();
				sprites[i].expirationDate = performance.now() + 3000;
				sprites[i].verticalThrustOverride = 9.8;
				sprites[i].fadeOutTime = 0;
				sprites[i].autoRotationDegeesPerSecond = 2;
				sprites[i].changeVelocity(0, -this.dieThrowVelocity, performance.now());
			}
		}
	}

	fadeOutSpriteByPlayerId(sprites: SpriteProxy[], playerId: number) {
		for (let i = 0; i < sprites.length; i++) {
			if (sprites[i].data === playerId) {
				sprites[i].fadeOutNow(500);
			}
		}
	}

	updatePlayer(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, playerStats: PlayerStats, data: string, latestPlayerStats: PlayerStats) {
		if (playerStats.ReadyToRollDice !== latestPlayerStats.ReadyToRollDice) {
			playerStats.ReadyToRollDice = latestPlayerStats.ReadyToRollDice;

			if (playerStats.ReadyToRollDice) {
				playerStats.Vantage = latestPlayerStats.Vantage;
				playerStats.MainDieSides = latestPlayerStats.MainDieSides;
				playerStats.DiceStack = latestPlayerStats.DiceStack;
				this.launchTheDragons(iGetPlayerX, soundManager, playerStats.PlayerId, playerStats.Vantage, latestPlayerStats.MainDieSides, playerStats.DiceStack);
			}
			else {
				this.descendTheDragons(iGetPlayerX, latestPlayerStats.PlayerId, latestPlayerStats.MainDieSides, latestPlayerStats.DiceStack);
			}
		}
		else if (playerStats.onlyVantageHasChanged(latestPlayerStats)) {
			playerStats.Vantage = latestPlayerStats.Vantage;
			this.showOrHideVantageDie(soundManager, playerStats.PlayerId, playerStats.Vantage);
		}
		else if (!playerStats.diceMatch(latestPlayerStats)) {
			this.descendTheDragons(iGetPlayerX, playerStats.PlayerId, playerStats.MainDieSides, playerStats.DiceStack);

			playerStats.MainDieSides = latestPlayerStats.MainDieSides;
			playerStats.DiceStack = latestPlayerStats.DiceStack;
			playerStats.ReadyToRollDice = latestPlayerStats.ReadyToRollDice;

			if (playerStats.ReadyToRollDice) {
				this.launchTheDragons(iGetPlayerX, soundManager, playerStats.PlayerId, playerStats.Vantage, playerStats.MainDieSides, playerStats.DiceStack);
			}
		}
		else {
			// console.log('Die already match!');
		}

		if (this.RollingTheDiceNow) {
			this.dropDieAndBlowUpTheDragons(iGetPlayerX, soundManager, playerStats.PlayerId, latestPlayerStats.MainDieSides, playerStats.DiceStack);
		}
	}

	showOrHideVantageDie(soundManager: ISoundManager, playerId: number, vantage: VantageKind) {
		soundManager.safePlayMp3('DiceDragons/DieSmoke');

		const disadvantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Disadvantage");
		const advantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Advantage");

		let hueShift = 0;
		let saturation = 100;

		switch (vantage) {
			case VantageKind.Normal:
				saturation = 0;
				break;
			case VantageKind.Advantage:
				hueShift = 210;
				break;
		}

		const sprite: SpriteProxy = this.getSpriteForPlayer(advantageDieSprites, playerId);
		if (sprite)
			this.readyToRollDieSmoke.addShifted(sprite.x, sprite.y, 0, hueShift, saturation);


		let newOpacity: number;
		if (vantage === VantageKind.Advantage)
			newOpacity = 1;
		else
			newOpacity = 0;

		setTimeout(this.changeDieOpacity.bind(this), 6 * fps30, advantageDieSprites, playerId, newOpacity);

		if (vantage === VantageKind.Disadvantage)
			newOpacity = 1;
		else
			newOpacity = 0;

		setTimeout(this.changeDieOpacity.bind(this), 6 * fps30, disadvantageDieSprites, playerId, newOpacity);
	}

	changeDieOpacity(sprites: Sprites, playerId: number, newOpacity: number) {
		this.setOpaciteForDie(sprites, playerId, newOpacity);
	}

	getSpriteForPlayer(sprites: Sprites, playerId: number): SpriteProxy {
		for (let i = 0; i < sprites.sprites.length; i++) {
			const sprite: SpriteProxy = sprites.sprites[i];
			if (sprite.data === playerId)
				return sprite;
		}
		return null;
	}

	setOpaciteForDie(sprites: Sprites, playerId: number, opacity: number) {
		const sprite: SpriteProxy = this.getSpriteForPlayer(sprites, playerId);
		if (sprite) {
			sprite.opacity = opacity;
		}
	}

	private addMissingPlayers(mostRecentPlayerStats: AllPlayerStats) {
		for (let i = 0; i < mostRecentPlayerStats.Players.length; i++) {
			const existingPlayerStats: PlayerStats = this.getPlayerStatsById(mostRecentPlayerStats.Players[i].PlayerId);
			if (!existingPlayerStats) {
				this.createNewPlayerStats(mostRecentPlayerStats.Players[i].PlayerId);
			}
		}
	}

	private createNewPlayerStats(playerId: number) {
		const newPlayerStats: PlayerStats = new PlayerStats();
		newPlayerStats.PlayerId = playerId;
		this.Players.push(newPlayerStats);
	}

	getPlayerStatsById(playerId: number): PlayerStats {
		for (let i = 0; i < this.Players.length; i++) {
			if (this.Players[i].PlayerId === playerId)
				return this.Players[i];
		}
		return null;
	}
}

class PlayerStatManager {
}

