interface IPlayerStats {
	ReadyToRollDice: boolean;
	PlayerId: number;
	Vantage: VantageKind;
	Conditions: Conditions;
	DiceStack: Array<DiceStackDto>;
	ConcentratedSpell: string;
	ConcentratedSpellDurationSeconds: number;
	PercentConcentrationComplete: number;
	JustBrokeConcentration: boolean;
	IsTargeted: boolean;
}


interface IAllPlayerStats {
	LatestCommand: string;
	LatestData: string;
	RollingTheDiceNow: boolean;
	HideSpellScrolls: boolean;
	ActiveTurnCreatureID: number;
	Players: Array<PlayerStats>;
}

class DiceStackDto {
	NumSides: number;
	HueShift: number;
	Multiplier: number;
	DamageType: DamageType;

	constructor(diceStack: DiceStackDto) {
		this.NumSides = diceStack.NumSides;
		this.HueShift = diceStack.HueShift;
		this.Multiplier = diceStack.Multiplier;
		this.DamageType = diceStack.DamageType;
	}

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
	Conditions: Conditions;
	ConcentratedSpell: string;
	ConcentratedSpellDurationSeconds: number;
	PercentConcentrationComplete: number;
	JustBrokeConcentration: boolean;
	IsTargeted: boolean;
	DiceStack: Array<DiceStackDto> = [];

	deserialize(playerStatsDto: IPlayerStats): PlayerStats {
		if (!playerStatsDto)
			return this;

		this.PlayerId = playerStatsDto.PlayerId;
		this.Vantage = playerStatsDto.Vantage;
		this.Conditions = playerStatsDto.Conditions;
		this.ConcentratedSpell = playerStatsDto.ConcentratedSpell;
		this.PercentConcentrationComplete = playerStatsDto.PercentConcentrationComplete;
		this.ConcentratedSpellDurationSeconds = playerStatsDto.ConcentratedSpellDurationSeconds;
		this.JustBrokeConcentration = playerStatsDto.JustBrokeConcentration;
		this.IsTargeted = playerStatsDto.IsTargeted;
		this.ReadyToRollDice = playerStatsDto.ReadyToRollDice;
		this.DiceStack = [];

		for (let i = 0; i < playerStatsDto.DiceStack.length; i++) {
			this.DiceStack.push(new DiceStackDto(playerStatsDto.DiceStack[i]));
		}
		return this;
	}

	onlyVantageHasChanged(latestPlayerStats: PlayerStats) {
		return this.PlayerId === latestPlayerStats.PlayerId &&
			this.Vantage !== latestPlayerStats.Vantage &&
			this.ReadyToRollDice === latestPlayerStats.ReadyToRollDice &&
			this.diceMatch(latestPlayerStats);
	}

	diceMatch(latestPlayerStats: PlayerStats) {
		return this.dieStacksMatch(latestPlayerStats.DiceStack);
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

class PlayerStatManager implements IAllPlayerStats {
	LatestCommand: string;
	LatestData: string;
	RollingTheDiceNow: boolean;
	HideSpellScrolls: boolean;
	Players: Array<PlayerStats> = [];
	ActiveTurnCreatureID: number;
	readyToRollFullDragon: Sprites;
	readyToRollLightningCord: Sprites;
	readyToRollDieSmoke: Sprites;
	readyToRollDragonBreath: Sprites;
	readyToRollLightDie: Sprites;
	readyToRollDarkDie: Sprites;
	readyToRollDragonHands: Sprites;
	readyToRollDieCollection: SpriteCollection;
	airExplosionCollection: SpriteCollection;
	playerTargetSprites: Sprites;

	concentratedSpellSprites: SpriteCollection;

	nameplateTopLong: Sprites;
	nameplateTopMedium: Sprites;
	nameplateTopShort: Sprites;
	nameplateRight: Sprites;
	nameplateLeft: Sprites;

	concentrationWhiteSmoke: Sprites;
	concentrationSpellNameScroll: Sprites;
	concentrationScrollOpenFire: Sprites;
	concentrationHourglassEnds: Sprites;
	concentrationIcon: Sprites;
	concentrationExplosion: Sprites;
	concentrationHourglassSand: Sprites;


	deserialize(allPlayerStatsDto: IAllPlayerStats): PlayerStatManager {
		this.LatestCommand = allPlayerStatsDto.LatestCommand;
		this.LatestData = allPlayerStatsDto.LatestData;
		this.RollingTheDiceNow = allPlayerStatsDto.RollingTheDiceNow;
		this.HideSpellScrolls = allPlayerStatsDto.HideSpellScrolls;
		this.ActiveTurnCreatureID = allPlayerStatsDto.ActiveTurnCreatureID;
		this.Players = [];
		for (let i = 0; i < allPlayerStatsDto.Players.length; i++) {
			//console.log(allPlayerStatsDto.Players[i]);
			this.Players.push(new PlayerStats().deserialize(allPlayerStatsDto.Players[i]));
		}

		//console.log(this);
		return this;
	}

	handleCommand(iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, soundManager: ISoundManager, mostRecentPlayerStats: PlayerStatManager, players: Array<Character>, conditionManager: ConditionManager) {
		this.LatestData = mostRecentPlayerStats.LatestData;
		this.LatestCommand = mostRecentPlayerStats.LatestCommand;
		this.RollingTheDiceNow = mostRecentPlayerStats.RollingTheDiceNow;
		if (this.HideSpellScrolls !== mostRecentPlayerStats.HideSpellScrolls) {
			this.HideSpellScrolls = mostRecentPlayerStats.HideSpellScrolls;
			this.hideOrShowSpellScrolls(iGetPlayerX, players);
		}

		this.setActiveTurnCreatureID(iGetPlayerX, iNameplateRenderer, context, mostRecentPlayerStats.ActiveTurnCreatureID, players);
		this.addMissingPlayers(mostRecentPlayerStats);
		this.handleCommandForExistingPlayers(iGetPlayerX, iNameplateRenderer, soundManager, context, mostRecentPlayerStats, players, conditionManager);
		this.cleanUpNonExistantPlayers(mostRecentPlayerStats);
	}

	static readonly nameplateHighlightTop: number = 1080 - 48;

	private static readonly longPlateArcWidth: number = 374;
	private static readonly longPlateTotalWidth: number = 576;
	private static readonly mediumPlateArcWidth: number = 292;
	private static readonly mediumPlateTotalWidth: number = 480;
	private static readonly shortPlateArcWidth: number = 153;
	private static readonly shortPlateTotalWidth: number = 355;

	moveAllTargets(iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, players: Array<Character>) {

		players.forEach((player, index) => {
			const targetSprite: SpriteProxy = this.getSpriteForPlayer(this.playerTargetSprites, player.playerID);
			if (targetSprite) {
				const x: number = iGetPlayerX.getPlayerTargetX(iNameplateRenderer, context, index, players) - this.playerTargetSprites.originX;

				if (targetSprite.x !== x) {
					targetSprite.ease(performance.now(), targetSprite.x, targetSprite.y, x, targetSprite.y, 360);
				}
			}
		});
	}

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

		if (!player)
			return;

		const plateWidth: number = iNameplateRenderer.getPlateWidth(context, player, playerIndex);

		let sprites: Sprites;
		let horizontalScale = 1;
		if (plateWidth <= PlayerStatManager.shortPlateArcWidth * 1.2) {
			sprites = this.nameplateTopShort;
			horizontalScale = plateWidth / PlayerStatManager.shortPlateArcWidth;
		}
		else if (plateWidth >= PlayerStatManager.longPlateArcWidth * 0.8) {
			sprites = this.nameplateTopLong;
			horizontalScale = plateWidth / PlayerStatManager.longPlateArcWidth;
		}
		else {
			sprites = this.nameplateTopMedium;
			horizontalScale = plateWidth / PlayerStatManager.mediumPlateArcWidth;
		}

		const nameplateSprites: Sprites = sprites;
		const nameplateHighlightSprite: SpriteProxy = nameplateSprites.addShifted(x, PlayerStatManager.nameplateHighlightTop, -1, player.hueShift);
		nameplateHighlightSprite.horizontalScale = horizontalScale;

		const nameplateRightAdjust = -4;
		const nameplateLeftAdjust = 3;
		this.nameplateLeft.addShifted(x - plateWidth / 2 + nameplateLeftAdjust, PlayerStatManager.nameplateHighlightTop, -1, player.hueShift);
		this.nameplateRight.addShifted(x + plateWidth / 2 + nameplateRightAdjust, PlayerStatManager.nameplateHighlightTop, -1, player.hueShift);
	}

	cleanUpAllActiveTurnHighlighting() {
		for (let i = 0; i < this.nameplateHighlightCollection.allSprites.length; i++) {
			const sprites: SpriteProxy[] = this.nameplateHighlightCollection.allSprites[i].spriteProxies;
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

	static readonly minSandSegmentFrame: number = 35;
	static readonly maxSandSegmentFrame: number = 320;
	static readonly totalSandFrames: number = 347;

	loadResources() {
		this.nameplateTopLong = this.loadNameplateHighlight('NameplateTopLong', 306, 147);
		this.nameplateTopMedium = this.loadNameplateHighlight('NameplateTopMedium', 254, 145);
		this.nameplateTopShort = this.loadNameplateHighlight('NameplateTopShort', 195, 144);
		this.nameplateLeft = this.loadNameplateHighlight('NameplateLeft', 152, 123);
		this.nameplateRight = this.loadNameplateHighlight('NameplateRight', 119, 123);

		this.readyToRollDieCollection = new SpriteCollection();
		this.airExplosionCollection = new SpriteCollection();
		this.concentratedSpellSprites = new SpriteCollection();

		this.concentrationWhiteSmoke = this.loadConcentrationSprites('WhiteSmoke', 150, 63, 168, AnimationStyle.Loop);
		this.concentrationSpellNameScroll = this.loadConcentrationSprites('SpellNameScroll', 52, 186, 38, AnimationStyle.SequentialStop);
		this.concentrationSpellNameScroll.frameInterval = fps60;
		this.concentrationScrollOpenFire = this.loadConcentrationSprites('ScrollOpenFire', 77, 103, 71, AnimationStyle.Sequential);
		this.concentrationHourglassEnds = this.loadConcentrationSprites('HourglassEnds', 31, 57, 56, AnimationStyle.SequentialStop);
		this.concentrationIcon = this.loadConcentrationSprites('Concentration', 1, 14, 16, AnimationStyle.Static);
		this.concentrationExplosion = this.loadConcentrationSprites('FireHideScroll', 99, 191, 280, AnimationStyle.Sequential);
		this.concentrationHourglassSand = this.loadConcentrationSprites('HourglassSand', 348, 47, 41, AnimationStyle.Loop);
		this.concentrationHourglassSand.returnFrameIndex = PlayerStatManager.minSandSegmentFrame;
		this.concentrationHourglassSand.segmentSize = 3;
		this.concentrationHourglassSand.name = 'hourglass';


		this.readyToRollFullDragon = this.createReadyToRollSprites('FullDragon', 281, 265);
		this.readyToRollLightningCord = this.createReadyToRollSprites('LightningCord', 28, -35);

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
		this.createAllReadyToRollSprites();

		this.readyToRollDragonHands = this.createReadyToRollSprites('DragonHands', 41, 22);

		// TODO: Just be aware this.playerTargetSprites is a duplicate load of sprites that are also loaded in the InGameCreatureManager which is used by the DragonBackGame.
		this.playerTargetSprites = new Sprites('Scroll/InGameCreatures/EnemyTarget/EnemyTarget', 102, fps30, AnimationStyle.Loop, true);
		this.playerTargetSprites.originX = 41;
		this.playerTargetSprites.originY = 45;
		this.playerTargetSprites.moves = true;
		this.playerTargetSprites.disableGravity();

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

	private createAllReadyToRollSprites() {
		this.createReadyToRollDieSprites('Willy');
		this.createReadyToRollDieSprites('Fred');
		//this.createReadyToRollDieSprites('Rojniss');
		//this.createReadyToRollDieSprites('Endell');
		//this.createReadyToRollDieSprites('Miles');
		this.createReadyToRollDieSprites('Lady');
		this.createReadyToRollDieSprites('Merkin');
		this.createReadyToRollDieSprites('Slizzie');
		this.createReadyToRollDieSprites('Stumpy');
		this.createReadyToRollDieSprites('Cutie');
	}

	loadConcentrationSprites(fileName: string, frameCount: number, originX: number, originY: number, animationStyle: AnimationStyle): Sprites {
		const sprites: Sprites = new Sprites(`Spell Name Scroll/${fileName}`, frameCount, fps30, animationStyle, true);
		sprites.originX = originX;
		sprites.originY = originY;
		sprites.moves = true;
		sprites.disableGravity();
		this.concentratedSpellSprites.add(sprites);
		return sprites;
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

	drawConcentratedSpellSprites(context: CanvasRenderingContext2D, nowMs: number) {
		this.concentrationWhiteSmoke.draw(context, nowMs);
		this.concentrationSpellNameScroll.draw(context, nowMs);
		this.concentratedSpellNames.render(context, nowMs);
		this.concentrationScrollOpenFire.draw(context, nowMs);
		this.concentrationHourglassEnds.draw(context, nowMs);
		this.concentrationIcon.draw(context, nowMs);
		this.concentrationHourglassSand.draw(context, nowMs);
		this.concentrationExplosion.draw(context, nowMs);
	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
		this.drawConcentratedSpellSprites(context, nowMs);
		this.nameplateHighlightCollection.draw(context, nowMs);
		this.drawReadyToRollDice(context, nowMs);
	}

	drawTopLevelStatus(context: CanvasRenderingContext2D, nowMs: number) {
		this.playerTargetSprites.draw(context, nowMs);
	}


	timeOfLastDragonBreath = 0;
	timeOfNextDragonBreath = -1;

	private drawReadyToRollDice(context: CanvasRenderingContext2D, nowMs: number) {
		this.readyToRollFullDragon.draw(context, nowMs);
		this.readyToRollLightningCord.draw(context, nowMs);
		this.readyToRollDieCollection.draw(context, nowMs);
		this.readyToRollDragonHands.draw(context, nowMs);
		this.allTextEffects.render(context, nowMs);
		this.airExplosionCollection.draw(context, nowMs);
		this.readyToRollDieSmoke.draw(context, nowMs);
		this.readyToRollDragonBreath.draw(context, nowMs);
	}

	update(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, nowMs: number) {
		this.allTextEffects.removeExpiredAnimations(nowMs);
		//this.allTextEffects.updatePositions(nowMs);
		this.readyToRollFullDragon.updatePositions(nowMs);
		this.readyToRollLightningCord.updatePositions(nowMs);
		this.readyToRollDieCollection.updatePositions(nowMs);
		this.readyToRollDragonHands.updatePositions(nowMs);
		this.airExplosionCollection.updatePositions(nowMs);
		this.concentratedSpellSprites.updatePositions(nowMs);
		this.concentratedSpellNames.updatePositions(nowMs);
		this.concentratedSpellNames.removeExpiredAnimations(nowMs);
		this.playerTargetSprites.updatePositions(nowMs);
		if (this.timeOfNextDragonBreath < nowMs) {
			this.timeOfNextDragonBreath = nowMs + Random.between(4000, 7000);
			this.breathFire(iGetPlayerX, soundManager, nowMs);
		}
	}

	breathFire(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, nowMs: number) {
		if (this.readyToRollFullDragon.spriteProxies.length === 0)
			return;
		const dragonIndex: number = Math.floor(Random.max(this.readyToRollFullDragon.spriteProxies.length));
		const sprite: SpriteProxy = this.readyToRollFullDragon.spriteProxies[dragonIndex];
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

	private cleanUpNonExistantPlayers(mostRecentPlayerStats: PlayerStatManager) {
		for (let i = 0; i < this.Players.length; i++) {
			const mostRecentPlayerStat: PlayerStats = mostRecentPlayerStats.getPlayerStatsById(this.Players[i].PlayerId);
			if (!mostRecentPlayerStat) {
				// At this point, the data sent excludes one of the players we are tracking. Remove it if the command says to do so.
			}
		}
	}

	delayBetweenDeletes = 0;
	lastTimeCommandHandled: number;

	aboutToHandleCommand() {
		const timeCommandHandled: number = performance.now();
		if (!this.lastTimeCommandHandled || timeCommandHandled - this.lastTimeCommandHandled > 300) {
			this.delayBetweenDeletes = 0;
		}
		this.lastTimeCommandHandled = timeCommandHandled;
	}

	private handleCommandForExistingPlayers(iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, soundManager: ISoundManager, context: CanvasRenderingContext2D, latestPlayerStats: PlayerStatManager, players: Array<Character>, conditionManager: ConditionManager) {
		this.aboutToHandleCommand();
		for (let i = 0; i < this.Players.length; i++) {
			const latestPlayerStat: PlayerStats = latestPlayerStats.getPlayerStatsById(this.Players[i].PlayerId);
			if (latestPlayerStat)
				this.handleCommandForPlayer(iGetPlayerX, iNameplateRenderer, soundManager, context, this.Players[i], this.LatestCommand, this.LatestData, latestPlayerStat, players, conditionManager);
		}
	}

	handleCommandForPlayer(iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, soundManager: ISoundManager, context: CanvasRenderingContext2D, existingPlayerStats: PlayerStats, command: string, data: string, latestPlayerStats: PlayerStats, players: Array<Character>, conditionManager: ConditionManager) {
		//console.log('command: ' + command);
		switch (command) {
			case 'Update':
				this.updatePlayer(iGetPlayerX, iNameplateRenderer, soundManager, context, existingPlayerStats, data, latestPlayerStats, players);
				conditionManager.updatePlayerConditions(existingPlayerStats, latestPlayerStats, context, players);
				break;

			case 'HourglassUpdate':
				this.updateHourglass(iGetPlayerX, soundManager, existingPlayerStats, data, latestPlayerStats, players);
				break;

			case 'ReStackConditions':
				if (conditionManager)
					conditionManager.restackPlayerConditions(context, existingPlayerStats, players);
				break;
		}
	}

	updateHourglass(iGetPlayerX: IGetPlayerX & ITextFloater, soundManager: ISoundManager, existingPlayerStats: PlayerStats, data: string, latestPlayerStats: PlayerStats, players: Character[]) {
		existingPlayerStats.PercentConcentrationComplete = latestPlayerStats.PercentConcentrationComplete;
		const hourglass: SpriteProxy = this.getSpriteForPlayer(this.concentrationHourglassSand, existingPlayerStats.PlayerId);

		if (hourglass) {
			//console.log('existingPlayerStats.PercentConcentrationComplete: ' + existingPlayerStats.PercentConcentrationComplete);
			if (existingPlayerStats.PercentConcentrationComplete === 100) {
				// only setting the frameIndex to the last segment.
				//console.log('setting the frameIndex to the last segment....');
				hourglass.frameIndex = PlayerStatManager.maxSandSegmentFrame - this.concentrationHourglassSand.segmentSize - 1;
				//console.log('hourglass.frameIndex: ' + hourglass.frameIndex);
				return;
			}
			if (hourglass.frameIndex < PlayerStatManager.minSandSegmentFrame)
				return;
			const framesBetween: number = PlayerStatManager.maxSandSegmentFrame - PlayerStatManager.minSandSegmentFrame;
			const frameOffset: number = Math.round(existingPlayerStats.PercentConcentrationComplete * framesBetween / 100);
			hourglass.frameIndex = PlayerStatManager.minSandSegmentFrame + frameOffset + 1;
			//console.log('hourglass.frameIndex: ' + hourglass.frameIndex);
		}
	}

	allTextEffects: Animations = new Animations();

	launchTheDragons(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, playerId: number, vantage: VantageKind, diceStack: Array<DiceStackDto>) {
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

		const yStopVariance: number = Random.between(-25, 25);
		const dieDistanceDown = 103;

		let multiDiceAdjustment = 0;
		if (diceStack.length > 1) {
			multiDiceAdjustment = (diceStack.length - 1) * dieDistanceDown;
		}
		const dragonTopY = 1180 + yStopVariance - multiDiceAdjustment;
		const dieOffsetY = 20;
		let dieTop: number = dragonTopY + dieOffsetY;
		let dieStartFrameIndex = startFrameIndex;
		const numFramesBehindEachDie = 8;
		const lightningCordHueShift: number = Random.max(360);

		for (let i = 0; i < diceStack.length; i++) {
			const dieStackEntry: DiceStackDto = diceStack[i];
			const dieSprites: Sprites = this.getDieForPlayerWithSides(iGetPlayerX, playerId, dieStackEntry);
			let dieOffsetX = 0;
			if (dieStackEntry.NumSides === 4)
				dieOffsetX = -16;
			else if (dieStackEntry.NumSides === 6)
				dieOffsetX = 3;
			const die: SpriteProxy = dieSprites.addShifted(playerX + dieOffsetX, dieTop, dieStartFrameIndex, dieShift);
			this.prepareForEntrance(die, playerId, 1, frameInterval);
			if (dieStackEntry.Multiplier !== 1 || dieStackEntry.DamageType !== DamageType.None || dieStackEntry.NumSides !== 20) {
				this.addDieDescriptor(dieStackEntry, playerX, dieTop, die);
			}

			if (i < diceStack.length - 1) {
				// At least one more die in the stack, so we need a lightning cord....
				const lightningCord: SpriteProxy = this.readyToRollLightningCord.addShifted(playerX, dieTop, dieStartFrameIndex, lightningCordHueShift);
				this.prepareForEntrance(lightningCord, playerId, 1, frameInterval);
			}

			dieStartFrameIndex -= numFramesBehindEachDie;
			while (dieStartFrameIndex < 0)
				dieStartFrameIndex += dieSprites.baseAnimation.frameCount;

			dieTop += dieDistanceDown;
		}

		const fullDragon: SpriteProxy = this.readyToRollFullDragon.addShifted(playerX, dragonTopY, startFrameIndex, dragonHueShift);

		const disadvantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Disadvantage");
		const advantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Advantage");
		const disadvantageDie: SpriteProxy = disadvantageDieSprites.addShifted(playerX, dieTop, startFrameIndex, dieShift);
		const advantageDie: SpriteProxy = advantageDieSprites.addShifted(playerX, dieTop, startFrameIndex, dieShift);

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

		const dragonHands: SpriteProxy = this.readyToRollDragonHands.addShifted(playerX, dragonTopY, startFrameIndex, dragonHueShift);

		this.prepareForEntrance(fullDragon, playerId, scale, frameInterval);

		this.prepareForEntrance(advantageDie, playerId, 1, frameInterval);
		this.prepareForEntrance(disadvantageDie, playerId, 1, frameInterval);
		this.prepareForEntrance(dragonHands, playerId, scale, frameInterval);
		soundManager.safePlayMp3('DiceDragons/DragonScreech[23]');
	}

	static readonly rightEdgeCutOff: number = 900;  // Die descriptors appearing after this point will be right-aligned.

	private addDieDescriptor(dieStackEntry: DiceStackDto, playerX: number, dieTop: number, die: SpriteProxy) {
		let textOffsetX: number;
		const textOffsetY = 11;
		let alignment: CanvasTextAlign;
		if (playerX < PlayerStatManager.rightEdgeCutOff) {
			textOffsetX = 50;
			alignment = 'left';
			if (dieStackEntry.NumSides > 6)
				textOffsetX += 15;
		}
		else {
			alignment = 'right';
			textOffsetX = -50;
			if (dieStackEntry.NumSides > 6)
				textOffsetX -= 15;
		}

		const textEffect: TextEffect = new TextEffect(playerX + textOffsetX, dieTop + textOffsetY);
		textEffect.textAlign = alignment;

		let damageTypeStr = '';
		if (dieStackEntry.DamageType !== DamageType.None)
			damageTypeStr = ` (${DamageType[dieStackEntry.DamageType]})`;

		textEffect.text = `${dieStackEntry.Multiplier}d${dieStackEntry.NumSides} ${damageTypeStr}`;

		textEffect.fontSize = 30;
		textEffect.outlineThickness = 2;
		this.allTextEffects.animationProxies.push(textEffect);
		die.attach(textEffect);
	}

	getDieForPlayerWithSides(iGetPlayerX: IGetPlayerX, playerId: number, diceStackEntry: DiceStackDto): Sprites {
		//console.log('diceStackEntry: ' + diceStackEntry);
		let dieSprites: Sprites;
		const mainDieSides: number = diceStackEntry.NumSides;
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

	dropDieAndBlowUpTheDragons(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager, playerId: number, diceStack: Array<DiceStackDto>) {
		for (let i = 0; i < diceStack.length; i++) {
			const dieSprites: Sprites = this.getDieForPlayerWithSides(iGetPlayerX, playerId, diceStack[i]);
			if (dieSprites) {
				this.dropSpriteByPlayerId(dieSprites.spriteProxies, playerId);
			}
		}

		this.dropSpriteByPlayerId(this.readyToRollLightningCord.spriteProxies, playerId);

		this.dropSpriteByPlayerId(this.readyToRollDarkDie.spriteProxies, playerId);
		this.dropSpriteByPlayerId(this.readyToRollLightDie.spriteProxies, playerId);

		const disadvantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Disadvantage");
		const advantageDieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName("Advantage");
		this.dropSpriteByPlayerId(disadvantageDieSprites.spriteProxies, playerId);
		this.dropSpriteByPlayerId(advantageDieSprites.spriteProxies, playerId);

		const verticalThrust = -3.3;
		const horizontalThrust: number = Random.between(-1.7, 1.7);
		this.ascendDragons(this.readyToRollFullDragon, playerId, horizontalThrust, verticalThrust);
		this.ascendDragons(this.readyToRollDragonHands, playerId, horizontalThrust, verticalThrust);

		const dragonSprite: SpriteProxy = this.getDragonSpriteByPlayerId(this.readyToRollFullDragon.spriteProxies, playerId);
		this.showAirExplosionLater(dragonSprite, soundManager);
		this.fadeOutSpriteByPlayerId(this.readyToRollDragonBreath.spriteProxies, playerId);
	}

	ascendDragons(dragonSprites: Sprites, playerId: number, horizontalThrust: number, verticalThrust: number) {
		for (let i = 0; i < dragonSprites.spriteProxies.length; i++) {
			const sprite: SpriteProxy = dragonSprites.spriteProxies[i];
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
		this.hideDragonByPlayerId(this.readyToRollFullDragon.spriteProxies, playerId);
		this.hideDragonByPlayerId(this.readyToRollDragonHands.spriteProxies, playerId);

		const airExplosionIndex: number = Math.floor(Random.max(this.airExplosionCollection.allSprites.length));
		this.airExplosionCollection.allSprites[airExplosionIndex].add(dragonSprite.x + this.readyToRollFullDragon.originX, dragonSprite.y + this.readyToRollFullDragon.originY);

		soundManager.safePlayMp3('DieBurst[5]');

		// HACK: Feels wrong. Fixes a state bug, but we have to clear *after* the dragon dice have dropped. I think we are okay because it's only volatile for 1.8 seconds after the DM presses the button to roll the dice.
		for (let i = 0; i < this.Players.length; i++) {
			this.Players[i].ReadyToRollDice = false;
			this.Players[i].Vantage = VantageKind.Normal;
		}
	}

	descendTheDragons(iGetPlayerX: IGetPlayerX, playerId: number, diceStack: Array<DiceStackDto>) {
		this.dropSpriteByPlayerId(this.readyToRollFullDragon.spriteProxies, playerId);
		for (let i = 0; i < diceStack.length; i++) {
			const dieSprites: Sprites = this.getDieForPlayerWithSides(iGetPlayerX, playerId, diceStack[i]);
			if (dieSprites) {
				dieSprites.spriteProxies.forEach(function (spriteProxy) { spriteProxy.fadeOutNow(500); });
				this.dropSpriteByPlayerId(dieSprites.spriteProxies, playerId);
			}
		}

		this.dropSpriteByPlayerId(this.readyToRollDarkDie.spriteProxies, playerId);
		this.dropSpriteByPlayerId(this.readyToRollLightDie.spriteProxies, playerId);
		this.dropSpriteByPlayerId(this.readyToRollLightningCord.spriteProxies, playerId);

		this.dropSpriteFromCollectionByPlayerId(this.readyToRollDieCollection, playerId);
		this.dropSpriteByPlayerId(this.readyToRollDragonHands.spriteProxies, playerId);
		this.fadeOutSpriteByPlayerId(this.readyToRollDragonBreath.spriteProxies, playerId);
	}

	dropSpriteFromCollectionByPlayerId(dieCollection: SpriteCollection, playerId: number) {
		for (let i = 0; i < dieCollection.allSprites.length; i++) {
			const sprites: Sprites = dieCollection.allSprites[i];
			this.dropSpriteByPlayerId(sprites.spriteProxies, playerId);
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
				sprites[i].fadeOutAttachedElements(500);
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

	updatePlayer(iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, soundManager: ISoundManager, context: CanvasRenderingContext2D, existingPlayerStats: PlayerStats, data: string, latestPlayerStats: PlayerStats, players: Array<Character>) {
		this.updatePlayerDice(existingPlayerStats, latestPlayerStats, iGetPlayerX, soundManager);
		this.updatePlayerTargeting(existingPlayerStats, latestPlayerStats, iGetPlayerX, iNameplateRenderer, soundManager, context, players);
		this.updateConcentratedSpell(existingPlayerStats, latestPlayerStats, iGetPlayerX, soundManager, players);
	}

	concentratedSpellNames: Animations = new Animations();


	private updatePlayerDice(existingPlayerStats: PlayerStats, latestPlayerStats: PlayerStats, iGetPlayerX: IGetPlayerX, soundManager: ISoundManager) {
		if (existingPlayerStats.ReadyToRollDice !== latestPlayerStats.ReadyToRollDice) {
			this.launchOrDescendTheDragons(existingPlayerStats, latestPlayerStats, iGetPlayerX, soundManager);
		}
		else if (existingPlayerStats.onlyVantageHasChanged(latestPlayerStats)) {
			this.makeVantageMatch(existingPlayerStats, latestPlayerStats, soundManager);
		}
		else if (!existingPlayerStats.diceMatch(latestPlayerStats)) {
			this.makeDiceMatch(iGetPlayerX, existingPlayerStats, latestPlayerStats, soundManager);
		}
		if (this.RollingTheDiceNow) {
			this.dropDieAndBlowUpTheDragons(iGetPlayerX, soundManager, existingPlayerStats.PlayerId, existingPlayerStats.DiceStack);
		}
	}

	private makeVantageMatch(existingPlayerStats: PlayerStats, latestPlayerStats: PlayerStats, soundManager: ISoundManager) {
		existingPlayerStats.Vantage = latestPlayerStats.Vantage;
		this.showOrHideVantageDie(soundManager, existingPlayerStats.PlayerId, existingPlayerStats.Vantage);
	}

	private makeDiceMatch(iGetPlayerX: IGetPlayerX, existingPlayerStats: PlayerStats, latestPlayerStats: PlayerStats, soundManager: ISoundManager) {
		this.descendTheDragons(iGetPlayerX, existingPlayerStats.PlayerId, existingPlayerStats.DiceStack);
		existingPlayerStats.DiceStack = latestPlayerStats.DiceStack;
		existingPlayerStats.ReadyToRollDice = latestPlayerStats.ReadyToRollDice;
		if (existingPlayerStats.ReadyToRollDice) {
			this.launchTheDragons(iGetPlayerX, soundManager, existingPlayerStats.PlayerId, existingPlayerStats.Vantage, existingPlayerStats.DiceStack);
		}
	}

	private launchOrDescendTheDragons(existingPlayerStats: PlayerStats, latestPlayerStats: PlayerStats, iGetPlayerX: IGetPlayerX, soundManager: ISoundManager) {
		existingPlayerStats.ReadyToRollDice = latestPlayerStats.ReadyToRollDice;
		existingPlayerStats.DiceStack = latestPlayerStats.DiceStack;
		if (existingPlayerStats.ReadyToRollDice) {
			existingPlayerStats.Vantage = latestPlayerStats.Vantage;
			existingPlayerStats.DiceStack = latestPlayerStats.DiceStack;
			this.launchTheDragons(iGetPlayerX, soundManager, existingPlayerStats.PlayerId, existingPlayerStats.Vantage, existingPlayerStats.DiceStack);
		}
		else {
			this.descendTheDragons(iGetPlayerX, latestPlayerStats.PlayerId, latestPlayerStats.DiceStack);
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
		for (let i = 0; i < sprites.spriteProxies.length; i++) {
			const sprite: SpriteProxy = sprites.spriteProxies[i];
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

	private addMissingPlayers(mostRecentPlayerStats: PlayerStatManager) {
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

	fadeOutNow(sprites: SpriteProxy[], playerId: number, fadeOutTimeMs: number) {
		for (let i = 0; i < sprites.length; i++) {
			if (sprites[i].data === playerId) {
				sprites[i].fadeOutNow(fadeOutTimeMs);
			}
		}
	}

	fadeOutAfter(delayMs: number, sprites: SpriteProxy[], playerId: number, fadeOutTimeMs: number) {
		for (let i = 0; i < sprites.length; i++) {
			if (sprites[i].data === playerId) {
				sprites[i].fadeOutAfter(delayMs, fadeOutTimeMs);
			}
		}
	}

	startSpellDestructionAnimation(soundManager: ISoundManager, playerId: number) {
		const hourglass: SpriteProxy = this.getSpriteForPlayer(this.concentrationHourglassSand, playerId);
		const fadeOutTime = 500;
		const framesToEnd: number = PlayerStatManager.totalSandFrames - PlayerStatManager.maxSandSegmentFrame;
		const timeToEndMS: number = this.delayBetweenDeletes + framesToEnd * this.concentrationHourglassSand.frameInterval;

		if (hourglass) {
			const explosion: SpriteProxy = this.concentrationExplosion.add(hourglass.x + this.concentrationHourglassSand.originX, hourglass.y + this.concentrationHourglassSand.originY, 0);
			explosion.delayStart = timeToEndMS;
			hourglass.frameIndex = PlayerStatManager.maxSandSegmentFrame;
			hourglass.playToEndNow = true;
			//console.log('------------');
			//console.log('this.concentrationHourglassSand.spriteProxies.length: ' + this.concentrationHourglassSand.spriteProxies.length);
			//console.log('timeToEndMS: ' + timeToEndMS);
			hourglass.data = undefined; // To disassociate this sprite from the player.
			hourglass.fadeOutAfter(timeToEndMS, fadeOutTime);
			soundManager.playMp3In(timeToEndMS, 'Spells/GunpowderFlare');
		}

		this.fadeOutAfter(timeToEndMS, this.concentrationWhiteSmoke.spriteProxies, playerId, fadeOutTime);
		this.fadeOutAfter(timeToEndMS, this.concentrationSpellNameScroll.spriteProxies, playerId, fadeOutTime);
		this.fadeOutAfter(timeToEndMS, this.concentrationHourglassEnds.spriteProxies, playerId, fadeOutTime);
		this.fadeOutAfter(timeToEndMS, this.concentrationIcon.spriteProxies, playerId, fadeOutTime);
		this.fadeOutAfter(timeToEndMS, this.concentrationHourglassSand.spriteProxies, playerId, fadeOutTime);
		for (let i = 0; i < this.concentratedSpellNames.animationProxies.length; i++) {
			if (this.concentratedSpellNames.animationProxies[i].data === playerId) {
				this.concentratedSpellNames.animationProxies[i].fadeOutAfter(timeToEndMS, fadeOutTime);
			}
		}
	}

	private static readonly scrollNameTextWidths = [
		77,   // frame index: 17
		83,   // frame index: 18
		90,   // frame index: 19
		95,   // frame index: 20
		107,
		115,
		123,
		131,
		144,
		152,
		158,
		167,
		180,
		187,   // frame index: 30
		195,
		205,
		216,
		224,
		231,
		237,
		252,
		260,
		267,
		274,   // frame index: 40
		284,
		292,
		298,
		306,
		312,
		318,
		323,
		328,
		333,
		339,   // frame index: 50
		342,   // frame index: 51
		346    // frame index: 52
	];

	private static readonly spellNameScrollStartIndex = 17;
	private static readonly spellNameScrollMaxIndex = 52;
	getScrollFrameIndexFrom(neededWidth: number): number {
		for (let i = 0; i < PlayerStatManager.scrollNameTextWidths.length; i++) {
			if (neededWidth < PlayerStatManager.scrollNameTextWidths[i])
				return PlayerStatManager.spellNameScrollStartIndex + i;
		}
		return PlayerStatManager.spellNameScrollMaxIndex;
	}

	getScrollWidthFrom(frameIndex: number): number {
		const scrollRollerWidth = 26;

		if (frameIndex < PlayerStatManager.spellNameScrollStartIndex)
			frameIndex = PlayerStatManager.spellNameScrollStartIndex

		if (frameIndex > PlayerStatManager.spellNameScrollMaxIndex)
			frameIndex = PlayerStatManager.spellNameScrollMaxIndex;

		return PlayerStatManager.scrollNameTextWidths[frameIndex - PlayerStatManager.spellNameScrollStartIndex] + scrollRollerWidth;
	}

	updateConcentratedSpell(existingPlayerStats: PlayerStats, latestPlayerStats: PlayerStats, iGetPlayerX: IGetPlayerX & ITextFloater, soundManager: ISoundManager, players: Array<Character>) {
		if (existingPlayerStats.ConcentratedSpell === latestPlayerStats.ConcentratedSpell)
			return;

		const x: number = iGetPlayerX.getPlayerX(iGetPlayerX.getPlayerIndex(existingPlayerStats.PlayerId));

		if (existingPlayerStats.ConcentratedSpell) {
			this.destroyConcentratedSpell(iGetPlayerX, existingPlayerStats, players, x, soundManager);
		}

		if (latestPlayerStats.ConcentratedSpell) {
			this.showSpellConcentrationAnimation(latestPlayerStats, x, existingPlayerStats, players, iGetPlayerX);
		}

		existingPlayerStats.ConcentratedSpell = latestPlayerStats.ConcentratedSpell;
		existingPlayerStats.ConcentratedSpellDurationSeconds = latestPlayerStats.ConcentratedSpellDurationSeconds;
	}

	updatePlayerTargeting(existingPlayerStats: PlayerStats, latestPlayerStats: PlayerStats, iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, soundManager: ISoundManager, context: CanvasRenderingContext2D, players: Array<Character>) {
		if (existingPlayerStats.IsTargeted === latestPlayerStats.IsTargeted)
			return;

		const playerId: number = latestPlayerStats.PlayerId;
		const x: number = iGetPlayerX.getPlayerTargetX(iNameplateRenderer, context, iGetPlayerX.getPlayerIndex(playerId), players);

		if (latestPlayerStats.IsTargeted) {
			//const centerNameplateY = 1052;
			const bottomAlignedNameplateY = 1060;
			const target: SpriteProxy = this.playerTargetSprites.addShifted(x, bottomAlignedNameplateY, -1, 220);
			target.fadeInTime = 500;
			target.data = playerId;
			soundManager.safePlayMp3('Windups/ArrowDrawQuick');
		}
		else {
			const target: SpriteProxy = this.getSpriteForPlayer(this.playerTargetSprites, playerId);
			if (target)
				target.fadeOutNow(500);
		}
		existingPlayerStats.IsTargeted = latestPlayerStats.IsTargeted;
	}

	private showSpellConcentrationAnimation(latestPlayerStats: PlayerStats, x: number, existingPlayerStats: PlayerStats, players: Character[], iGetPlayerX: IGetPlayerX & ITextFloater) {
		const spellName: string = latestPlayerStats.ConcentratedSpell;
		const spellScrollY: number = this.getSpellScrollY();
		const spellNameFontSize = 36;
		myContext.font = `${spellNameFontSize}px Enchanted Land`;
		const textWidth: number = myContext.measureText(spellName).width;

		const concentrationIconWidth = 32;
		const concentrationIconHalfWidth: number = concentrationIconWidth / 2;
		const spellNameConcentrationIconMargin = 6;
		const fullScrollWidth: number = textWidth + concentrationIconWidth + spellNameConcentrationIconMargin;
		const scrollStopFrame: number = this.getScrollFrameIndexFrom(fullScrollWidth);
		const hourglassMargin = 28;
		const hourglassOffset = -this.getScrollWidthFrom(scrollStopFrame) / 2 - hourglassMargin;

		const spellNameOffsetX: number = -concentrationIconHalfWidth - spellNameConcentrationIconMargin / 2;
		const concentrationIconOffsetX: number = fullScrollWidth / 2 - concentrationIconHalfWidth;

		const smoke: SpriteProxy = this.concentrationWhiteSmoke.add(x + hourglassOffset, spellScrollY, -1);
		smoke.data = existingPlayerStats.PlayerId;

		this.concentrationScrollOpenFire.add(x, spellScrollY, -1);

		const spellNameText: TextEffect = new TextEffect(x + spellNameOffsetX, spellScrollY);
		spellNameText.text = spellName;
		spellNameText.data = existingPlayerStats.PlayerId;
		spellNameText.fontName = 'Enchanted Land';
		spellNameText.fontSize = spellNameFontSize;
		spellNameText.fontColor = '#410300';
		spellNameText.fadeInTime = 500;
		this.concentratedSpellNames.animationProxies.push(spellNameText);

		const hourglassEnds: SpriteProxy = this.concentrationHourglassEnds.add(x + hourglassOffset, spellScrollY, 0);
		hourglassEnds.data = existingPlayerStats.PlayerId;

		const concentrationIcon: SpriteProxy = this.concentrationIcon.add(x + concentrationIconOffsetX, spellScrollY, 0);
		concentrationIcon.autoRotationDegeesPerSecond = CharacterStatsScroll.spinningConcentrationIconDegreesPerSecond;
		concentrationIcon.data = existingPlayerStats.PlayerId;
		concentrationIcon.opacity = 0.5;

		const hourglassSand: ColorShiftingSpriteProxy = this.concentrationHourglassSand.addShifted(x + hourglassOffset, spellScrollY, 0);
		hourglassSand.data = existingPlayerStats.PlayerId;

		if (players) {
			const player: Character = players[iGetPlayerX.getPlayerIndex(existingPlayerStats.PlayerId)];
			if (player) {
				hourglassSand.hueShift = player.hueShift;
			}
		}

		const spellNameScroll: SpriteProxy = this.concentrationSpellNameScroll.add(x, spellScrollY, 0);
		spellNameScroll.data = existingPlayerStats.PlayerId;
		spellNameScroll.addOnFrameAdvanceCallback((sprite: SpriteProxy) => {
			if (sprite.frameIndex > scrollStopFrame)
				sprite.frameIndex = scrollStopFrame;
		});
	}

	private destroyConcentratedSpell(iGetPlayerX: IGetPlayerX & ITextFloater, existingPlayerStats: PlayerStats, players: Character[], x: number, soundManager: ISoundManager) {
		this.showSpellDispelled(iGetPlayerX, existingPlayerStats, players, x);
		this.startSpellDestructionAnimation(soundManager, existingPlayerStats.PlayerId);
		this.delayBetweenDeletes += 200;
		//console.log('this.delayBetweenDeletes: ' + this.delayBetweenDeletes);
	}

	private showSpellDispelled(iGetPlayerX: IGetPlayerX & ITextFloater, existingPlayerStats: PlayerStats, players: Character[], x: number) {
		const playerIndex: number = iGetPlayerX.getPlayerIndex(existingPlayerStats.PlayerId);
		const player: Character = players[playerIndex];
		let outlineColor = '#000000';
		let fillColor = '#ffffff';
		if (player) {
			outlineColor = player.dieFontColor;
			fillColor = player.dieBackColor;
		}

		const floatingText: TextEffect = iGetPlayerX.addFloatingText(x, `${existingPlayerStats.ConcentratedSpell} dispelled.`, fillColor, outlineColor);
		floatingText.delayStart = this.delayBetweenDeletes;
	}

	getSpellScrollY(): number {
		const spellScrollY = 1000;
		const hideOffset = 50;
		if (this.HideSpellScrolls)
			return spellScrollY + hideOffset;
		return spellScrollY;
	}

	hideOrShowSpellScrolls(iGetPlayerX: IGetPlayerX & ITextFloater, players: Array<Character>) {
		const spellScrollY: number = this.getSpellScrollY();
		this.moveSpritesVertically(this.concentrationWhiteSmoke, spellScrollY);
		this.moveSpritesVertically(this.concentrationScrollOpenFire, spellScrollY);
		this.moveAnimationsVertically(this.concentratedSpellNames.animationProxies, spellScrollY);
		this.moveSpritesVertically(this.concentrationHourglassEnds, spellScrollY);
		this.moveSpritesVertically(this.concentrationIcon, spellScrollY);
		this.moveSpritesVertically(this.concentrationHourglassSand, spellScrollY);
		this.moveSpritesVertically(this.concentrationSpellNameScroll, spellScrollY);
		if (!this.HideSpellScrolls)
			this.showRemainingTime(iGetPlayerX, players);
	}

	showRemainingTime(iGetPlayerX: IGetPlayerX & ITextFloater, players: Array<Character>) {
		let delayMs = 0;
		const timeBetweenMessages = 250;
		this.Players.forEach((player) => {
			//if (player.ConcentratedSpell)
			//	console.log('player.ConcentratedSpellDurationSeconds: ' + player.ConcentratedSpellDurationSeconds);
			if (player.ConcentratedSpell && player.ConcentratedSpellDurationSeconds > 0) {
				const playerIndex: number = iGetPlayerX.getPlayerIndex(player.PlayerId);
				const character: Character = players[playerIndex];
				let outlineColor = '#000000';
				let fillColor = '#ffffff';
				if (character) {
					outlineColor = character.dieFontColor;
					fillColor = character.dieBackColor;
				}

				const x: number = iGetPlayerX.getPlayerX(playerIndex);
				const timeRemainingStr: string = this.getRemainingTimeStr(player.ConcentratedSpellDurationSeconds * (1 - player.PercentConcentrationComplete / 100));
				const floatingText: TextEffect = iGetPlayerX.addFloatingText(x, `${timeRemainingStr} remaining`, fillColor, outlineColor);
				floatingText.delayStart = delayMs;
				delayMs += timeBetweenMessages;
			}
		});
	}

	getRemainingTimeStr(seconds: number): string {
		const numDecimalPlaces = 1;

		if (seconds < 60) {
			const secondsStr: string = Math.round(seconds).toString();
			if (secondsStr === '1')
				return '1 second';
			else
				return `${secondsStr} seconds`;
		}

		const minutes: number = seconds / 60;
		if (minutes < 60) {
			const minutesStr: string = MathEx.toFixed(minutes, numDecimalPlaces).toString();
			if (minutesStr === '1')
				return '1 minute';
			else
				return `${minutesStr} minutes`;
		}

		const hours: number = minutes / 60;
		if (hours < 24) {
			const hourStr: string = MathEx.toFixed(hours, numDecimalPlaces).toString();
			if (hourStr === '1')
				return `1 hour`;
			else
				return `${hourStr} hours`;
		}

		const days: number = hours / 24;
		const dayStr: string = MathEx.toFixed(days, numDecimalPlaces).toString();
		if (dayStr === '1')
			return '1 day';
		else
			return `${dayStr} days`;
	}

	static readonly timeToMoveScroll: number = 400;

	moveSpritesVertically(sprites: Sprites, targetY: number) {
		sprites.spriteProxies.forEach((sprite) => {
			if (sprite.y !== targetY) {
				sprite.ease(performance.now(), sprite.x, sprite.y, sprite.x, targetY - sprites.originY, PlayerStatManager.timeToMoveScroll);
			}
		});
	}

	moveAnimationsVertically(animations: AnimatedElement[], targetY: number) {
		animations.forEach((animation) => {
			if (animation.y !== targetY) {
				animation.ease(performance.now(), animation.x, animation.y, animation.x, targetY, PlayerStatManager.timeToMoveScroll);
			}
		});
	}
}