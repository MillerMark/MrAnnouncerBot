enum WorldView {
	Normal,
	Flipped
}

class ConditionState {
	stacked: boolean;
	thrown: boolean;
	constructor(public creatureId: number, public conditionIndex: number, public finalX: number, public finalY: number) {
		this.thrown = true;
	}
}

class ConditionManager {
	static readonly conditionIconLength = 102;
	static readonly npcConditionScale: number = 0.75;
	static readonly conditionMargin: number = 6;
	static readonly conditionLengthIncludingMargin: number = ConditionManager.conditionIconLength * ConditionManager.npcConditionScale + ConditionManager.conditionMargin;

	conditionCollection: SpriteCollection = new SpriteCollection();
	conditions: Sprites;
	conditionGlow: Sprites;
	conditionBlast: Sprites;

	constructor() {

	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
		this.conditionCollection.draw(context, nowMs);
	}

	update(nowMs: number) {
		this.conditionCollection.updatePositions(nowMs);
	}

	loadResources() {
		this.conditionCollection = new SpriteCollection();

		this.conditions = new Sprites('Conditions/Conditions', 17, fps30, AnimationStyle.Static);
		this.conditions.originX = 102;
		this.conditions.originY = 102;
		this.conditions.disableGravity();
		this.conditions.moves = true;

		this.conditionGlow = new Sprites('Conditions/Glow/ConditionGlow', 90, fps30, AnimationStyle.Loop, true);
		this.conditionGlow.originX = 254;
		this.conditionGlow.originY = 252;
		this.conditionGlow.disableGravity();
		this.conditionGlow.moves = true;

		this.conditionBlast = new Sprites('Conditions/Blast/ConditionConfettiBlast', 83, fps30, AnimationStyle.Sequential, true);
		this.conditionBlast.originX = 241;
		this.conditionBlast.originY = 322;

		this.conditionCollection.add(this.conditionGlow);
		this.conditionCollection.add(this.conditions);
		this.conditionCollection.add(this.conditionBlast);
	}

	static readonly plateMargin = 7;

	// HACK: plateWidthAdjust is magic number to solve width calculation issues. Some day, fix. #NoComeuppance
	static readonly plateWidthAdjust = 31;

	// TODO: Consider consolidating parameters to a single parameter.
	private getNameplateLeftEdge(iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, iGetPlayerX: IGetPlayerX & ITextFloater, playerIndex: number, players: Character[]): number {
		return iGetPlayerX.getPlayerTargetX(iNameplateRenderer, context, iGetPlayerX, playerIndex, players) - ConditionManager.plateMargin;
	}

	getSpritesAbove(sprites: Sprites, sprite: SpriteProxy, worldView: WorldView): Array<SpriteProxy> {
		const compareConditionState: ConditionState = sprite.data as ConditionState;
		if (!compareConditionState)
			return null;

		return this.getSpritesFromStack(compareConditionState.creatureId, sprites, worldView, sprite.y);
	}

	getSpritesFromStack(creatureId: number, sprites: Sprites, worldView: WorldView = WorldView.Normal, yThreshold: number = undefined): Array<SpriteProxy> {
		const spritesAbove: Array<SpriteProxy> = [];
		for (let i = 0; i < sprites.sprites.length; i++) {
			const testSprite: SpriteProxy = sprites.sprites[i];
			const conditionState: ConditionState = testSprite.data as ConditionState;
			if (conditionState)
				if (creatureId === conditionState.creatureId)
					if (conditionState.stacked || conditionState.thrown) {
						if (yThreshold === undefined)
							spritesAbove.push(testSprite);
						else if (worldView === WorldView.Normal) {
							if (testSprite.y < yThreshold) {
								// This sprite is above.
								spritesAbove.push(testSprite);
							}
						}
						else if (testSprite.y > yThreshold) {
							// This sprite is below (but really above in a flipped world view).
							spritesAbove.push(testSprite);
						}
					}
		}
		if (worldView === WorldView.Normal) {
			return spritesAbove.sort((s1, s2) => s2.y - s1.y);
		}
		else {
			return spritesAbove.sort((s1, s2) => s1.y - s2.y);
		}
	}

	private getSpritesForCreature(sprites: Sprites, creatureId: number) {
		const spritesInStack: Array<SpriteProxy> = [];
		for (let i = 0; i < sprites.sprites.length; i++) {
			const testSprite: SpriteProxy = sprites.sprites[i];
			const conditionState: ConditionState = testSprite.data as ConditionState;
			if (conditionState)
				if (conditionState.creatureId === creatureId)
					if (conditionState.stacked || conditionState.thrown) {
						spritesInStack.push(testSprite);
					}
		}
		return spritesInStack;
	}

	private getConditionSpriteForPlayer(sprites: Sprites, matchConditionState: ConditionState): SpriteProxy {
		for (let i = 0; i < sprites.sprites.length; i++) {
			const testSprite: SpriteProxy = sprites.sprites[i];
			const conditionState: ConditionState = testSprite.data as ConditionState;
			if (conditionState)
				if (conditionState.creatureId === matchConditionState.creatureId && conditionState.conditionIndex === matchConditionState.conditionIndex)
					return testSprite;
		}
		return null;
	}

	private getStackHeightBelow(sprites: Sprites, sprite: SpriteProxy, scaledConditionHeight: number, worldView: WorldView) {
		const compareConditionState: ConditionState = sprite.data as ConditionState;
		if (!compareConditionState)
			return;
		const spritesBelow: Array<SpriteProxy> = [];
		for (let i = 0; i < sprites.sprites.length; i++) {
			const testSprite: SpriteProxy = sprites.sprites[i];
			const conditionState: ConditionState = testSprite.data as ConditionState;
			if (conditionState)
				if (compareConditionState.creatureId === conditionState.creatureId)
					if (conditionState.stacked || conditionState.thrown) {
						if (worldView === WorldView.Normal) {
							if (testSprite.y > sprite.y) {
								// This sprite is below.
								spritesBelow.push(testSprite);
							}
						}
						else if (testSprite.y < sprite.y) {
							// This sprite is below (but really above in a flipped world view).
							spritesBelow.push(testSprite);
						}
					}
		}
		return spritesBelow.length * scaledConditionHeight;
	}

	drawDiagnostics(context: CanvasRenderingContext2D, iNameplateRenderer: INameplateRenderer, iGetPlayerX: IGetPlayerX & ITextFloater, players: Character[]) {
		context.lineWidth = 4;
		for (let i = 0; i < players.length; i++) {
			const widthToTheLeft: number = this.getAvailableSpaceLeftOfPlate(iNameplateRenderer, context, iGetPlayerX, i, players);
			const x: number = this.getNameplateLeftEdge(iNameplateRenderer, context, iGetPlayerX, i, players);
			const y = 1000;

			context.beginPath();
			context.strokeStyle = players[i].dieBackColor;
			context.moveTo(x - widthToTheLeft, y - 9);
			context.lineTo(x - widthToTheLeft, y + 9);

			context.moveTo(x - widthToTheLeft, y);
			context.lineTo(x, y);
			context.stroke();

			context.textAlign = 'right';
			context.textBaseline = 'bottom';
			context.fillStyle = players[i].dieBackColor;
			context.fillText(Math.round(widthToTheLeft).toString(), x, y);
		}
	}

	private getAvailableSpaceLeftOfPlate(iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, iGetPlayerX: IGetPlayerX & ITextFloater, playerIndex: number, players: Array<Character>) {
		const leftEdgeThisNameplate: number = this.getNameplateLeftEdge(iNameplateRenderer, context, iGetPlayerX, playerIndex, players);

		if (playerIndex <= 0) {
			return leftEdgeThisNameplate;
		}

		const playerLeftIndex: number = playerIndex - 1;

		const previousPlateWidth: number = iNameplateRenderer.getPlateWidth(context, players[playerLeftIndex], playerLeftIndex);
		const previousPlateLeft: number = this.getNameplateLeftEdge(iNameplateRenderer, context, iGetPlayerX, playerLeftIndex, players);
		const previousPlateRight: number = previousPlateLeft + previousPlateWidth;

		return leftEdgeThisNameplate - previousPlateRight - ConditionManager.plateWidthAdjust;
	}

	private getConditionScale(iNameplateRenderer: INameplateRenderer, context: CanvasRenderingContext2D, iGetPlayerX: IGetPlayerX & ITextFloater, playerIndex: number, players: Character[], offsetCount = 0) {
		const widthToTheLeft: number = this.getAvailableSpaceLeftOfPlate(iNameplateRenderer, context, iGetPlayerX, playerIndex, players);
		return Math.min(1, (widthToTheLeft * 2.0 / 3.0) / ConditionManager.conditionIconLength);
	}

	static readonly inGameCreatureScrollBottom: number = InGameCreatureManager.creatureScrollHeight + InGameCreatureManager.inGameStatTopMargin;

	private addCondition(rightEdge: number, conditionIndex: number, creatureId: number,
		soundManager: ISoundManager, scale = 1, hueShift = 0, worldView: WorldView = WorldView.Normal) {

		const existingStackHeight: number = this.getStackHeight(creatureId, scale);

		let bottomEdgeY: number;
		if (worldView === WorldView.Normal)
			bottomEdgeY = 1080 - existingStackHeight;
		else
			bottomEdgeY = existingStackHeight + ConditionManager.conditionIconLength * scale + ConditionManager.inGameCreatureScrollBottom;

		const sprite: SpriteProxy = this.conditions.addShifted(rightEdge, bottomEdgeY, conditionIndex, hueShift);
		this.prepareConditionForEntrance(this.conditions, sprite, creatureId, conditionIndex, scale, worldView, soundManager);
		const glow: SpriteProxy = this.conditionGlow.addShifted(rightEdge, bottomEdgeY, -1, hueShift);
		this.prepareConditionForEntrance(this.conditionGlow, glow, creatureId, conditionIndex, scale, worldView);
	}

	getStackHeight(playerId: number, scale: number): number {
		let numStacked = 0;
		for (let i = 0; i < this.conditions.sprites.length; i++) {
			const conditionState: ConditionState = this.conditions.sprites[i].data as ConditionState
			if (conditionState && conditionState.creatureId === playerId && (conditionState.thrown || conditionState.stacked)) {
				numStacked++;
			}
		}
		return numStacked * ConditionManager.conditionIconLength * scale;
	}

	private prepareConditionForEntrance(sprites: Sprites, sprite: SpriteProxy, creatureId: number, conditionIndex: number, scale: number, worldView: WorldView, soundManager: ISoundManager = null) {

		// ![](FD746B628AC803464F2303D5B6404D95.png)

		const fallDistanceFactor = 0.5; // As a percentage of the tile height.
		const fallDistancePx = ConditionManager.conditionIconLength * fallDistanceFactor * scale;

		sprite.data = new ConditionState(creatureId, conditionIndex, sprite.x, sprite.y);
		sprite.scale = scale;
		const accelerationFactor = 1.8;
		let yFactor: number;
		if (worldView === WorldView.Normal)
			yFactor = 1;
		else
			yFactor = -1;
		const earthGravity = 9.8;  // m/s
		sprite.verticalThrustOverride = earthGravity * accelerationFactor * yFactor;
		sprite.velocityY = -6 * yFactor;
		const apexY: number = sprites.originY + sprite.y - fallDistancePx * yFactor;

		const screenHeightPx = 1080;
		let startingY: number;
		if (worldView === WorldView.Normal)
			startingY = screenHeightPx + ConditionManager.conditionIconLength * scale;
		else
			startingY = 0;

		const distanceToApexPx: number = Math.abs(startingY - apexY);
		const timeToApexSeconds = Physics.getDropTimeSeconds(Physics.pixelsToMeters(distanceToApexPx), sprite.verticalThrustOverride * yFactor);
		const dropTimeToLandSeconds: number = Physics.getDropTimeSeconds(Physics.pixelsToMeters(fallDistancePx), sprite.verticalThrustOverride * yFactor);
		const throwVelocityY: number = -Physics.getFinalVelocityMetersPerSecond(timeToApexSeconds, 0, sprite.verticalThrustOverride * yFactor) * yFactor;
		const totalTravelTimeMs: number = (timeToApexSeconds + dropTimeToLandSeconds) * 1000;

		const horizontalTravelFactor = 1.2;
		const horizontalTravelDistancePx: number = horizontalTravelFactor * ConditionManager.conditionIconLength * scale;
		sprite.x -= horizontalTravelDistancePx;

		const throwVelocityX: number = Physics.pixelsToMeters(horizontalTravelDistancePx) / (totalTravelTimeMs / 1000);
		sprite.y = startingY - sprites.originY;
		sprite.changeVelocity(throwVelocityX, throwVelocityY, performance.now());

		if (soundManager)
			soundManager.safePlayMp3('Conditions/Whoosh[5]');

		setTimeout(this.conditionLanded.bind(this), totalTravelTimeMs, sprite, soundManager);
	}

	conditionLanded(sprite: SpriteProxy, soundManager: ISoundManager) {
		sprite.verticalThrustOverride = 0;
		const conditionState: ConditionState = sprite.data as ConditionState;
		if (conditionState) {
			sprite.x = conditionState.finalX;
			sprite.y = conditionState.finalY;
			conditionState.stacked = true;
			conditionState.thrown = false;
		}
		sprite.changeVelocity(0, 0, performance.now());
		if (soundManager)
			soundManager.safePlayMp3('Conditions/Clack[5]');
	}

	private removeCondition(creatureId: number, conditionIndex: number, nameplateLeftEdge: number, scale: number, worldView: WorldView, soundManager: ISoundManager) {
		this.removeConditionByCreatureIdAndFrameIndex(this.conditionGlow, creatureId, conditionIndex, nameplateLeftEdge, scale, worldView);
		this.removeConditionByCreatureIdAndFrameIndex(this.conditions, creatureId, conditionIndex, nameplateLeftEdge, scale, worldView, soundManager);
	}

	removeConditionByCreatureIdAndFrameIndex(sprites: Sprites, creatureId: number, conditionIndex: number, nameplateLeftEdge: number, scale: number, worldView: WorldView, soundManager: ISoundManager = null) {
		for (let i = 0; i < sprites.sprites.length; i++) {
			const sprite: SpriteProxy = sprites.sprites[i];
			const conditionState = sprite.data as ConditionState;
			if (conditionState && conditionState.creatureId === creatureId && conditionState.conditionIndex === conditionIndex)
				this.removeConditionWithAnimation(sprites, sprite, nameplateLeftEdge, scale, worldView, soundManager);
		}
	}

	animateSpritesVertically(sprites: SpriteProxy[], newX: number, newY: number, scaledSpriteHeight: number, originX: number, originY: number, worldView: WorldView, soundManager: ISoundManager = null) {
		let startTime: number = performance.now();
		let indexToMove = 0;
		let yFactor: number;
		if (worldView === WorldView.Normal)
			yFactor = 1;
		else
			yFactor = -1;

		for (let i = 0; i < sprites.length; i++) {
			const sprite: SpriteProxy = sprites[i];
			const moveTime = 500;
			const delayBetweenMoves = 100;
			const targetX: number = newX - originX;
			const targetY: number = newY - originY;
			newY -= scaledSpriteHeight * yFactor;

			if (sprite.x === targetX && sprite.y === targetY)
				continue;

			sprite.ease(startTime, sprite.x, sprite.y, targetX, targetY, moveTime);
			startTime += delayBetweenMoves;
			if (soundManager)
				soundManager.playMp3In(delayBetweenMoves * indexToMove + moveTime, 'Conditions/Clack[6]');
			indexToMove++;
		}
	}

	removeConditionWithAnimation(sprites: Sprites, sprite: SpriteProxy, nameplateLeftEdge: number, scale: number, worldView: WorldView, soundManager: ISoundManager = null) {
		const spritesAbove: Array<SpriteProxy> = this.getSpritesAbove(sprites, sprite, worldView);

		const conditionIconLength: number = ConditionManager.conditionIconLength * scale;
		const stackHeightBelow: number = this.getStackHeightBelow(sprites, sprite, conditionIconLength, worldView);

		const startY: number = this.getStackingStartY(worldView, conditionIconLength, stackHeightBelow);

		this.animateSpritesVertically(spritesAbove, nameplateLeftEdge, startY, conditionIconLength, sprites.originX, sprites.originY, worldView, soundManager);
		if (soundManager) { // Only true when removing main condition sprite (false when removing glow).
			const blast: SpriteProxy = this.conditionBlast.addShifted(sprite.x + sprites.originX, sprite.y + sprites.originY, 0, (sprite as ColorShiftingSpriteProxy).hueShift);
			blast.scale = scale;
			soundManager.safePlayMp3('Conditions/Blast');
			soundManager.playMp3In(100, 'Conditions/BubblePop[5]');
		}
		sprite.fadeOutNow(200);
	}

	getStackingStartY(worldView: WorldView, conditionIconLength: number, stackHeightBelow = 0) {
		if (worldView === WorldView.Normal)
			return 1080 - stackHeightBelow;
		else
			return ConditionManager.inGameCreatureScrollBottom + stackHeightBelow + conditionIconLength;
	}

	updatePlayerConditions(existingPlayerStats: PlayerStats, latestPlayerStats: PlayerStats, iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, soundManager: ISoundManager, context: CanvasRenderingContext2D, players: Array<Character>): void {
		if (existingPlayerStats.Conditions === latestPlayerStats.Conditions)
			return;

		const playerId: number = latestPlayerStats.PlayerId;
		const playerIndex: number = iGetPlayerX.getPlayerIndex(playerId);

		const scale = this.getConditionScale(iNameplateRenderer, context, iGetPlayerX, playerIndex, players);

		const nameplateLeftEdge: number = this.getNameplateLeftEdge(iNameplateRenderer, context, iGetPlayerX, iGetPlayerX.getPlayerIndex(playerId), players);

		let hueShift = 0;

		if (playerId >= 0 && players && iGetPlayerX) {
			const playerIndex: number = iGetPlayerX.getPlayerIndex(playerId);
			const player: Character = players[playerIndex];
			if (player) {
				hueShift = player.hueShift;
			}
		}
		this.updateConditions(existingPlayerStats.Conditions, latestPlayerStats.Conditions, playerId, nameplateLeftEdge, scale, soundManager, hueShift);

		existingPlayerStats.Conditions = latestPlayerStats.Conditions;
	}

	updateConditions(existingConditions: Conditions, compareConditions: Conditions, creatureId: number, rightEdge: number, scale: number, soundManager: ISoundManager, hueShift = 0, worldView: WorldView = WorldView.Normal) {
		let conditionIndex = 0;
		for (const item in Conditions) {
			if (!isNaN(Number(item))) {
				continue;
			}

			const condition: Conditions = Conditions[item as keyof typeof Conditions];
			if (condition !== Conditions.None) {
				if ((existingConditions & condition) !== (compareConditions & condition)) {
					if ((existingConditions & condition) === condition) { // Bit is set.
						this.removeCondition(creatureId, conditionIndex, rightEdge, scale, worldView, soundManager);
					}
					else {
						this.addCondition(rightEdge, conditionIndex, creatureId, soundManager, scale, hueShift, worldView);
					}
				}
				conditionIndex++;
			}
		}
	}

	restackPlayerConditions(iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, soundManager: ISoundManager, context: CanvasRenderingContext2D, existingPlayerStats: PlayerStats, players: Character[], worldView: WorldView = WorldView.Normal) {
		if (!existingPlayerStats.Conditions)
			return;

		const playerIndex: number = iGetPlayerX.getPlayerIndex(existingPlayerStats.PlayerId);
		const targetX: number = this.getNameplateLeftEdge(iNameplateRenderer, context, iGetPlayerX, playerIndex, players);

		const scale = this.getConditionScale(iNameplateRenderer, context, iGetPlayerX, playerIndex, players);
		this.stackConditionSprites(existingPlayerStats.PlayerId, targetX, scale, worldView, soundManager);

	}

	stackConditionSprites(playerId: number, targetX: number, scale: number, worldView: WorldView, soundManager: ISoundManager) {
		const targetY = 1080;

		const conditionSprites: Array<SpriteProxy> = this.getSpritesForCreature(this.conditions, playerId);
		this.animateSpritesVertically(conditionSprites, targetX, targetY, ConditionManager.conditionIconLength * scale, this.conditions.originX, this.conditions.originY, worldView, soundManager);

		const glowSprites: Array<SpriteProxy> = this.getSpritesForCreature(this.conditionGlow, playerId);
		this.animateSpritesVertically(glowSprites, targetX, targetY, ConditionManager.conditionIconLength * scale, this.conditionGlow.originX, this.conditionGlow.originY, worldView);
	}

	movePlayerConditions(iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, soundManager: ISoundManager, context: CanvasRenderingContext2D, players: Array<Character>, worldView: WorldView = WorldView.Normal) {
		players.forEach((player, index) => {
			const conditionSprites: Array<SpriteProxy> = this.getSpritesForCreature(this.conditions, player.playerID);
			const x: number = this.getNameplateLeftEdge(iNameplateRenderer, context, iGetPlayerX, index, players);
			const scale: number = this.getConditionScale(iNameplateRenderer, context, iGetPlayerX, index, players);

			for (let i = 0; i < conditionSprites.length; i++) {

				const conditionSprite: SpriteProxy = conditionSprites[i];
				if (conditionSprite) {
					this.moveAndScaleConditionSprite(conditionSprite, x - this.conditions.originX, scale);
					const glowSprite: SpriteProxy = this.getConditionSpriteForPlayer(this.conditionGlow, conditionSprite.data as ConditionState);
					this.moveAndScaleConditionSprite(glowSprite, x - this.conditionGlow.originX, scale);
				}
			}

			this.stackConditionSprites(player.playerID, x, scale, worldView, soundManager);
		});
	}

	private moveAndScaleConditionSprite(conditionSprite: SpriteProxy, x: number, scale: number) {
		if (conditionSprite.x !== x) {
			conditionSprite.ease(performance.now(), conditionSprite.x, conditionSprite.y, x, conditionSprite.y, 360);
		}
		if (conditionSprite.scale !== scale) {
			conditionSprite.scale = scale;
		}
	}

	restackNpcConditions(creature: InGameCreature, x: number, soundManager: SoundManager) {
		const conditionIconLength: number = ConditionManager.conditionIconLength * ConditionManager.npcConditionScale;
		const startY: number = this.getStackingStartY(WorldView.Flipped, conditionIconLength);
		this.restackNpcConditionsFor(this.conditions, creature, x, startY, conditionIconLength, soundManager);
		this.restackNpcConditionsFor(this.conditionGlow, creature, x, startY, conditionIconLength, soundManager);
	}

	private restackNpcConditionsFor(sprites: Sprites, creature: InGameCreature, x: number, startY: number, conditionIconLength: number, soundManager: SoundManager) {
		const conditionSpritesToMove: Array<SpriteProxy> = this.getSpritesFromStack(creature.Index, sprites, WorldView.Flipped, undefined);
		this.animateSpritesVertically(conditionSpritesToMove, x, startY, conditionIconLength, sprites.originX, sprites.originY, WorldView.Flipped, soundManager);
	}

	moveNpcConditionsTo(creatureIndex: number, targetX: number, moveTime: number, delayMs: number, soundManager: SoundManager) {
		this.moveNpcConditionSpritesTo(this.conditions, creatureIndex, targetX, moveTime, delayMs);
		this.moveNpcConditionSpritesTo(this.conditionGlow, creatureIndex, targetX, moveTime, delayMs);
	}

	moveNpcConditionSpritesTo(sprites: Sprites, creatureId: number, targetX: number, moveTime: number, delayMs: number) {
		// Since we are moving X we must account for the originX. Also NpcCondition sprites are anchored at the bottom right of the tile.
		const adjustedX: number = targetX - sprites.originX + ConditionManager.conditionLengthIncludingMargin;
		sprites.sprites.forEach((sprite) => {
			const conditionState: ConditionState = sprite.data as ConditionState;
			if (conditionState && conditionState.creatureId === creatureId) {
				sprite.ease(performance.now() + delayMs, sprite.x, sprite.y, adjustedX, sprite.y, moveTime);
			}
		});
	}

	fadeOutConditions(creatureIndex: number, delayMs: number, fadeTimeMs: number) {
		this.fadeOutNpcConditionSprites(this.conditions, creatureIndex, delayMs, fadeTimeMs);
		this.fadeOutNpcConditionSprites(this.conditionGlow, creatureIndex, delayMs, fadeTimeMs);
	}

	fadeOutNpcConditionSprites(sprites: Sprites, creatureId: number, delayMs: number, fadeTimeMs: number) {
		sprites.sprites.forEach((sprite) => {
			const conditionState: ConditionState = sprite.data as ConditionState;
			if (conditionState && conditionState.creatureId === creatureId) {
				sprite.fadeOutAfter(delayMs, fadeTimeMs);
			}
		});
	}

}

