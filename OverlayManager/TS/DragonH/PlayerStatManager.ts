interface IPlayerStats {
	ReadyToRollDice: boolean;
	PlayerId: number;
}


interface IAllPlayerStats {
	LatestCommand: string;
	LatestData: string;
	Players: Array<PlayerStats>;
}


class PlayerStats implements IPlayerStats {
	ReadyToRollDice: boolean;
	PlayerId: number;

	deserialize(playerStatsDto: IPlayerStats): PlayerStats {
		if (!playerStatsDto)
			return this;

		this.PlayerId = playerStatsDto.PlayerId;
		this.ReadyToRollDice = playerStatsDto.ReadyToRollDice;
		return this;
	}

	constructor() {

	}
}

class AllPlayerStats implements IAllPlayerStats {
	LatestCommand: string;
	LatestData: string;
	Players: Array<PlayerStats> = [];
	readyToRollFullDragon: Sprites;
	readyToRollLightDie: Sprites;
	readyToRollDarkDie: Sprites;
	readyToRollWilly: Sprites;
	readyToRollFred: Sprites;
	readyToRollMiles: Sprites;
	readyToRollLady: Sprites;
	readyToRollMerkin: Sprites;
	readyToRollCutie: Sprites;
	readyToRollDragonHands: Sprites;
	readyToRollDieCollection: SpriteCollection;

	deserialize(allPlayerStatsDto: IAllPlayerStats): AllPlayerStats {
		this.LatestCommand = allPlayerStatsDto.LatestCommand;
		this.LatestData = allPlayerStatsDto.LatestData;
		this.Players = [];
		for (let i = 0; i < allPlayerStatsDto.Players.length; i++) {
			console.log(allPlayerStatsDto.Players[i]);
			this.Players.push(new PlayerStats().deserialize(allPlayerStatsDto.Players[i]));
		}

		console.log(this);
		return this;
	}

	handleCommand(iGetPlayerX: IGetPlayerX, mostRecentPlayerStats: AllPlayerStats) {
		this.LatestData = mostRecentPlayerStats.LatestData;
		this.LatestCommand = mostRecentPlayerStats.LatestCommand;

		this.addMissingPlayers(mostRecentPlayerStats);
		this.handleCommandForExistingPlayers(iGetPlayerX, mostRecentPlayerStats);
		this.cleanUpNonExistantPlayers(mostRecentPlayerStats);
	}

	loadResources() {
		this.readyToRollDieCollection = new SpriteCollection();

		this.readyToRollFullDragon = this.createReadyToRollSprites('FullDragon', 281, 265);

		this.readyToRollDarkDie = this.createReadyToRollDieSprites('DarkDie');
		this.readyToRollLightDie = this.createReadyToRollDieSprites('LightDie');
		this.readyToRollWilly = this.createReadyToRollDieSprites('Willy');
		this.readyToRollFred = this.createReadyToRollDieSprites('Fred');
		this.readyToRollMiles = this.createReadyToRollDieSprites('Miles');
		this.readyToRollLady = this.createReadyToRollDieSprites('Lady');
		this.readyToRollMerkin = this.createReadyToRollDieSprites('Merkin');
		this.readyToRollCutie = this.createReadyToRollDieSprites('Cutie');

		this.readyToRollDragonHands = this.createReadyToRollSprites('DragonHands', 41, 22);

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

	private createReadyToRollDieSprites(fileName: string): Sprites {
		const dieSprites: Sprites = this.createReadyToRollSprites(fileName, 48, 29);
		dieSprites.name = fileName;
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
		this.readyToRollFullDragon.draw(context, nowMs);
		this.readyToRollDieCollection.draw(context, nowMs);
		this.readyToRollDragonHands.draw(context, nowMs);
	}

	update(timestamp: number) {
		this.readyToRollFullDragon.updatePositions(timestamp);
		this.readyToRollDieCollection.updatePositions(timestamp);
		this.readyToRollDragonHands.updatePositions(timestamp);
	}

	private cleanUpNonExistantPlayers(mostRecentPlayerStats: AllPlayerStats) {
		for (let i = 0; i < this.Players.length; i++) {
			const mostRecentPlayerStat: PlayerStats = mostRecentPlayerStats.getPlayerStatsById(this.Players[i].PlayerId);
			if (!mostRecentPlayerStat) {
				// At this point, the data sent excludes one of the players we are tracking. Remove it if the command says to do so.
			}
		}
	}

	private handleCommandForExistingPlayers(iGetPlayerX: IGetPlayerX, latestPlayerStats: AllPlayerStats) {
		for (let i = 0; i < this.Players.length; i++) {
			const latestPlayerStat: PlayerStats = latestPlayerStats.getPlayerStatsById(this.Players[i].PlayerId);
			if (latestPlayerStat)
				this.handleCommandForPlayer(iGetPlayerX, this.Players[i], this.LatestCommand, this.LatestData, latestPlayerStat);
		}
	}

	handleCommandForPlayer(iGetPlayerX: IGetPlayerX, playerStats: PlayerStats, command: string, data: string, latestPlayerStats: PlayerStats) {
		console.log('command: ' + command);
		switch (command) {
			case 'Update':
				this.updatePlayer(iGetPlayerX, playerStats, data, latestPlayerStats);
				break;
		}
	}

	showReadyToRollDice(iGetPlayerX: IGetPlayerX, playerId: number) {
		const playerIndex: number = iGetPlayerX.getPlayerIndex(playerId);
		const playerX: number = iGetPlayerX.getPlayerX(playerIndex);
		const hueShift: number = Random.max(360);
		let dieShift: number = Random.max(360);
		const startFrameIndex: number = Random.max(this.readyToRollFullDragon.baseAnimation.frameCount);
		this.readyToRollFullDragon.addShifted(playerX, 900, startFrameIndex, hueShift).data = playerId;

		let dieSprites: Sprites = this.getDieSpritesForPlayer(iGetPlayerX, playerId);

		if (dieSprites)
			dieShift = 0;
		else
			if (Random.chancePercent(50))
				dieSprites = this.readyToRollLightDie;
			else
				dieSprites = this.readyToRollDarkDie;

		dieSprites.addShifted(playerX, 900, startFrameIndex, dieShift).data = playerId;
		this.readyToRollDragonHands.addShifted(playerX, 900, startFrameIndex, hueShift).data = playerId;
	}

	private getDieSpritesForPlayer(iGetPlayerX: IGetPlayerX, playerId: number) {
		let playerFirstName: string = iGetPlayerX.getPlayerFirstName(playerId);
		if (playerFirstName)
			if (playerFirstName === "L'il")
				playerFirstName = 'Cutie';
		const dieSprites: Sprites = this.readyToRollDieCollection.getSpritesByName(playerFirstName);
		return dieSprites;
	}

	hideReadyToRollDice(iGetPlayerX: IGetPlayerX, playerId: number) {
		this.hideDragonByPlayerId(this.readyToRollFullDragon.sprites, playerId);
		const dieSprites: Sprites = this.getDieSpritesForPlayer(iGetPlayerX, playerId);
		if (dieSprites)
			this.hideDieByPlayerId(dieSprites.sprites, playerId);
		else 
		{
			this.hideDieByPlayerId(this.readyToRollDarkDie.sprites, playerId);
			this.hideDieByPlayerId(this.readyToRollLightDie.sprites, playerId);
		}
		this.hideDragonByPlayerId(this.readyToRollDragonHands.sprites, playerId);
	}

	dieThrowVelocity = 3;

	hideDragonByPlayerId(sprites: SpriteProxy[], playerId: number) {
		for (let i = 0; i < sprites.length; i++) {
			if (sprites[i].data === playerId) {
				sprites[i].fadeOutNow(1000);
				sprites[i].changeVelocity(0, -this.dieThrowVelocity, performance.now());
			}
		}
	}

	hideDieByPlayerId(sprites: SpriteProxy[], playerId: number) {
		for (let i = 0; i < sprites.length; i++) {
			if (sprites[i].data === playerId) {
				sprites[i].expirationDate = performance.now() + 3000;
				sprites[i].verticalThrustOverride = 9.8;
				sprites[i].fadeOutTime = 0;
				sprites[i].autoRotationDegeesPerSecond = 2;
				sprites[i].changeVelocity(0, -this.dieThrowVelocity, performance.now());
			}
		}
	}

	updatePlayer(iGetPlayerX: IGetPlayerX, playerStats: PlayerStats, data: string, latestPlayerStats: PlayerStats) {
		console.log('updatePlayer...');
		if (playerStats.ReadyToRollDice !== latestPlayerStats.ReadyToRollDice) {
			console.log('ReadyToRollDice  has changed!');
			playerStats.ReadyToRollDice = latestPlayerStats.ReadyToRollDice;
			if (playerStats.ReadyToRollDice) {
				console.log('this.showReadyToRollDice...');
				this.showReadyToRollDice(iGetPlayerX, playerStats.PlayerId);
			}
			else
				this.hideReadyToRollDice(iGetPlayerX, playerStats.PlayerId);
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

