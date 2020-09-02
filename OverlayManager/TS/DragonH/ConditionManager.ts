enum WorldView {
	Normal,
	Flipped
}

class ConditionState {
	stacked: boolean;
	thrown: boolean;
	stackIndex: number;
	constructor(public creatureId: number, public conditionFrameIndex: number, public finalX: number, public finalY: number) {
		this.thrown = true;
	}
}

class ConditionManager {
	static readonly conditionIconLength = 102;
	static readonly numNpcConditionsPerRow: number = 3;
	static readonly npcConditionWidth: number = (InGameCreatureManager.creatureScrollWidth - InGameCreatureManager.creatureHorizontalWhiteSpace) / ConditionManager.numNpcConditionsPerRow;
	static readonly npcConditionScale: number = ConditionManager.npcConditionWidth / ConditionManager.conditionIconLength;
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

		if (this.conditions.spriteProxies.length === 0)
			return;

		context.fillStyle = '#000000';
		context.font = '14px Arial';
		context.textAlign = 'left';
		context.textBaseline = 'bottom';

		const verticalOffset = -7;
		const horizontalOffset = 3;
		const sprite: SpriteProxy = this.conditions.spriteProxies[0];
		const scale: number = sprite.scale;
		const spriteWidth: number = ConditionManager.conditionIconLength * scale;
		const originX: number = this.conditions.originX;
		const originY: number = this.conditions.originY;

		this.conditions.spriteProxies.forEach((sprite) => {
			const conditionState: ConditionState = sprite.data as ConditionState;
			if (conditionState)
				context.fillText(conditionState.conditionFrameIndex.toString(), horizontalOffset + sprite.x + originX - spriteWidth, sprite.y + originY + verticalOffset);
		});
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

		const flippedWorldView: WorldView = this.getFlippedWorldView(worldView);

		return this.getEarlierSprites(sprites, compareConditionState.finalX, compareConditionState.finalY, compareConditionState.creatureId, flippedWorldView);
	}

	getLaterSpritesFromStack(creatureId: number, sprites: Sprites, compareX: number, compareY: number, worldView: WorldView = WorldView.Normal): Array<SpriteProxy> {
		const flippedWorldView: WorldView = this.getFlippedWorldView(worldView);

		return this.getEarlierSprites(sprites, compareX, compareY, creatureId, flippedWorldView);
	}

    private getFlippedWorldView(worldView: WorldView) {
        let flippedWorldView: WorldView;
        if (worldView === WorldView.Normal)
            flippedWorldView = WorldView.Flipped;
        else
            flippedWorldView = WorldView.Normal;
        return flippedWorldView;
    }

	getAllSpritesFromStack(sprites: Sprites, creatureId: number): Array<SpriteProxy> {
		const returnSprites: Array<SpriteProxy> = [];
		for (let i = 0; i < sprites.spriteProxies.length; i++) {
			const testSprite: SpriteProxy = sprites.spriteProxies[i];
			const conditionState: ConditionState = testSprite.data as ConditionState;
			if (conditionState)
				if (creatureId === conditionState.creatureId)
					if (conditionState.stacked || conditionState.thrown) {
						returnSprites.push(testSprite);
					}
		}
		return returnSprites;
	}

	//getEarlierSpritesFromStack(creatureId: number, sprites: Sprites, worldView: WorldView = WorldView.Normal, yThreshold: number = undefined): Array<SpriteProxy> {
	//	const spritesAbove: Array<SpriteProxy> = [];
	//	for (let i = 0; i < sprites.spriteProxies.length; i++) {
	//		const testSprite: SpriteProxy = sprites.spriteProxies[i];
	//		const conditionState: ConditionState = testSprite.data as ConditionState;
	//		if (conditionState)
	//			if (creatureId === conditionState.creatureId)
	//				if (conditionState.stacked || conditionState.thrown) {
	//					if (yThreshold === undefined)
	//						spritesAbove.push(testSprite);
	//					else if (worldView === WorldView.Normal) {
	//						if (conditionState.finalY < yThreshold) {
	//							// This sprite is above.
	//							spritesAbove.push(testSprite);
	//						}
	//					}
	//					else if (conditionState.finalY > yThreshold) {
	//						// This sprite is below (but really above in a flipped world view).
	//						spritesAbove.push(testSprite);
	//					}
	//				}
	//	}
	//	if (worldView === WorldView.Normal) {
	//		return spritesAbove.sort((s1, s2) => s2.y - s1.y);
	//	}
	//	else {
	//		return spritesAbove.sort((s1, s2) => s1.y - s2.y);
	//	}
	//}

	private getSpritesForCreature(sprites: Sprites, creatureId: number) {
		const spritesInStack: Array<SpriteProxy> = [];
		for (let i = 0; i < sprites.spriteProxies.length; i++) {
			const testSprite: SpriteProxy = sprites.spriteProxies[i];
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
		for (let i = 0; i < sprites.spriteProxies.length; i++) {
			const testSprite: SpriteProxy = sprites.spriteProxies[i];
			const conditionState: ConditionState = testSprite.data as ConditionState;
			if (conditionState)
				if (conditionState.creatureId === matchConditionState.creatureId && conditionState.conditionFrameIndex === matchConditionState.conditionFrameIndex)
					return testSprite;
		}
		return null;
	}

	private getNumConditionsBefore(sprites: Sprites, sprite: SpriteProxy, scaledConditionHeight: number, worldView: WorldView, numColumns = 1) {
		const compareConditionState: ConditionState = sprite.data as ConditionState;
		if (!compareConditionState)
			return;
		const spritesBelow: Array<SpriteProxy> = this.getEarlierSprites(sprites, compareConditionState.finalX, compareConditionState.finalY, compareConditionState.creatureId, worldView);
		return spritesBelow.length * scaledConditionHeight;
	}

	private getEarlierSprites(sprites: Sprites, compareX: number, compareY: number, creatureId: number, worldView: WorldView) {
		const spritesBelow: Array<SpriteProxy> = [];
		for (let i = 0; i < sprites.spriteProxies.length; i++) {
			const testSprite: SpriteProxy = sprites.spriteProxies[i];
			const conditionState: ConditionState = testSprite.data as ConditionState;
			if (conditionState)
				if (creatureId === conditionState.creatureId)
					if (conditionState.stacked || conditionState.thrown) {
						if (worldView === WorldView.Normal) {
							if (conditionState.finalY > compareY) {
								// This sprite is below.
								spritesBelow.push(testSprite);
							}
							else if (conditionState.finalY === compareY) {
								if (conditionState.finalX > compareX)
									// This sprite is before.
									spritesBelow.push(testSprite);
							}
						}
						else { // world view is flipped.
							if (conditionState.finalY < compareY) {
								// This sprite is below (but really above in a flipped world view).
								spritesBelow.push(testSprite);
							}
							else if (conditionState.finalY === compareY) {
								if (conditionState.finalX < compareX) {
									// This sprite is before.
									spritesBelow.push(testSprite);
								}
							}
						}
					}
		}
		if (worldView === WorldView.Normal) {
			return spritesBelow.sort((s1, s2) => {
				const state2: ConditionState = s2.data as ConditionState;
				const state1: ConditionState = s1.data as ConditionState;
				if (state1.finalY !== state2.finalY)
					return state2.finalY - state1.finalY;
				else
					return state2.finalX - state1.finalX;
			});
		}
		else {
			return spritesBelow.sort((s1, s2) => {
				const state2: ConditionState = s2.data as ConditionState;
				const state1: ConditionState = s1.data as ConditionState;
				if (state1.finalY !== state2.finalY)
					return state1.finalY - state2.finalY;
				else
					return state1.finalX - state2.finalX;
			});
		}
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

	private addCondition(rightEdge: number, conditionFrameIndex: number, creatureId: number,
		soundManager: ISoundManager, scale = 1, hueShift = 0, worldView: WorldView = WorldView.Normal, numColumns = 1) {

		const iconActualWidth: number = ConditionManager.conditionIconLength * scale;
		const numConditionsStacked: number = this.getNumConditionsFor(creatureId);
		const rowsComplete: number = Math.floor(numConditionsStacked / numColumns);
		const numTilesToLeft: number = numConditionsStacked % numColumns;
		const indent: number = numTilesToLeft * iconActualWidth;
		const existingStackHeight: number = rowsComplete * iconActualWidth;

		let bottomEdgeY: number;
		if (worldView === WorldView.Normal)
			bottomEdgeY = 1080 - existingStackHeight;
		else
			bottomEdgeY = existingStackHeight + ConditionManager.conditionIconLength * scale + ConditionManager.inGameCreatureScrollBottom;

		const sprite: SpriteProxy = this.conditions.addShifted(rightEdge + indent, bottomEdgeY, conditionFrameIndex, hueShift);
		this.prepareConditionForEntrance(this.conditions, sprite, creatureId, conditionFrameIndex, scale, worldView, soundManager);
		const glow: SpriteProxy = this.conditionGlow.addShifted(rightEdge + indent, bottomEdgeY, -1, hueShift);
		this.prepareConditionForEntrance(this.conditionGlow, glow, creatureId, conditionFrameIndex, scale, worldView);
	}

	getNumConditionsFor(playerId: number): number {
		let numStacked = 0;
		for (let i = 0; i < this.conditions.spriteProxies.length; i++) {
			const conditionState: ConditionState = this.conditions.spriteProxies[i].data as ConditionState
			if (conditionState && conditionState.creatureId === playerId && (conditionState.thrown || conditionState.stacked)) {
				numStacked++;
			}
		}
		return numStacked;
	}

	getStackHeight(playerId: number, scale: number): number {
		return this.getNumConditionsFor(playerId) * ConditionManager.conditionIconLength * scale;
	}

	private prepareConditionForEntrance(sprites: Sprites, sprite: SpriteProxy, creatureId: number, conditionFrameIndex: number, scale: number, worldView: WorldView, soundManager: ISoundManager = null) {

		// ![](FD746B628AC803464F2303D5B6404D95.png)

		const fallDistanceFactor = 0.5; // As a percentage of the tile height.
		const fallDistancePx = ConditionManager.conditionIconLength * fallDistanceFactor * scale;

		sprite.data = new ConditionState(creatureId, conditionFrameIndex, sprite.x, sprite.y);
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

	private removeCondition(id: number, conditionFrameIndex: number, nameplateLeftEdge: number, scale: number, worldView: WorldView, numColumns: number, soundManager: ISoundManager) {
		this.removeConditionByIdAndFrameIndex(this.conditionGlow, id, conditionFrameIndex, nameplateLeftEdge, scale, worldView, numColumns);
		this.removeConditionByIdAndFrameIndex(this.conditions, id, conditionFrameIndex, nameplateLeftEdge, scale, worldView, numColumns, soundManager);
	}

	removeConditionByIdAndFrameIndex(sprites: Sprites, creatureId: number, conditionFrameIndex: number, nameplateLeftEdge: number, scale: number, worldView: WorldView, numColumns: number, soundManager: ISoundManager = null) {
		for (let i = 0; i < sprites.spriteProxies.length; i++) {
			const sprite: SpriteProxy = sprites.spriteProxies[i];
			const conditionState = sprite.data as ConditionState;
			if (conditionState && conditionState.creatureId === creatureId && conditionState.conditionFrameIndex === conditionFrameIndex)
				this.removeConditionWithAnimation(sprites, sprite, nameplateLeftEdge, scale, worldView, numColumns, soundManager);
		}
	}

	animateSpritesIntoPosition(sprites: SpriteProxy[], numEarlierConditions: number, numColumns: number, newX: number, newY: number, scaledSpriteEdgeLength: number, originX: number, originY: number, worldView: WorldView, soundManager: ISoundManager = null) {
		let startTime: number = performance.now();
		let indexToMove = 0;
		let yFactor: number;
		if (worldView === WorldView.Normal)
			yFactor = 1;
		else
			yFactor = -1;

		let row: number = Math.floor(numEarlierConditions / numColumns);
		let column: number = numEarlierConditions % numColumns;

		for (let i = 0; i < sprites.length; i++) {
			const sprite: SpriteProxy = sprites[i];
			const moveTime = 500;
			const delayBetweenMoves = 100;
			const targetX: number = newX - originX + column * scaledSpriteEdgeLength;
			const targetY: number = newY - originY - yFactor * row * scaledSpriteEdgeLength + yFactor * numEarlierConditions * scaledSpriteEdgeLength;
			column++;
			if (column >= numColumns) {
				column = 0;
				row++;
			}

			//newY -= scaledSpriteEdgeLength * yFactor;

			if (sprite.x === targetX && sprite.y === targetY)
				continue;

			sprite.ease(startTime, sprite.x, sprite.y, targetX, targetY, moveTime);
			startTime += delayBetweenMoves;
			if (soundManager)
				soundManager.playMp3In(delayBetweenMoves * indexToMove + moveTime, 'Conditions/Clack[6]');
			indexToMove++;
		}
	}

	removeConditionWithAnimation(sprites: Sprites, spriteToRemove: SpriteProxy, nameplateLeftEdge: number, scale: number, worldView: WorldView, numColumns: number, soundManager: ISoundManager = null) {
		const compareState: ConditionState = spriteToRemove.data as ConditionState;
		const spritesToAnimate: Array<SpriteProxy> = this.getSpritesAbove(sprites, spriteToRemove, worldView);
		console.log(spritesToAnimate);

		const conditionIconLength: number = ConditionManager.conditionIconLength * scale;
		const numEarlierConditions: number = this.getEarlierSprites(sprites, compareState.finalX, compareState.finalY, compareState.creatureId, worldView).length;
		console.log('numEarlierConditions: ' + numEarlierConditions);
		//const rowsComplete: number = Math.floor(numEarlierConditions / numColumns);
		//const numTilesToLeft: number = numEarlierConditions % numColumns;
		//const indent: number = numTilesToLeft * iconActualWidth;
		//const existingStackHeight: number = rowsComplete * iconActualWidth;


		const stackHeightBelow: number = this.getNumConditionsBefore(sprites, spriteToRemove, conditionIconLength, worldView);
		const startY: number = this.getStackingStartY(worldView, conditionIconLength, stackHeightBelow);
		this.animateSpritesIntoPosition(spritesToAnimate, numEarlierConditions, numColumns, nameplateLeftEdge, startY, conditionIconLength, sprites.originX, sprites.originY, worldView, soundManager);

		if (soundManager) { // Only true when removing main condition sprite (false when removing glow).
			const blast: SpriteProxy = this.conditionBlast.addShifted(spriteToRemove.x + sprites.originX, spriteToRemove.y + sprites.originY, 0, (spriteToRemove as ColorShiftingSpriteProxy).hueShift);
			blast.scale = scale;
			soundManager.safePlayMp3('Conditions/Blast');
			soundManager.playMp3In(100, 'Conditions/BubblePop[5]');
		}
		spriteToRemove.fadeOutNow(200);
	}

	getStackingStartY(worldView: WorldView, numConditionIcons: number, stackHeightBelow = 0) {
		if (worldView === WorldView.Normal)
			return 1080 - stackHeightBelow;
		else
			return ConditionManager.inGameCreatureScrollBottom + stackHeightBelow + numConditionIcons;
	}

	static readonly numPlayerConditionColumns: number = 1;

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
		this.updateConditions(existingPlayerStats.Conditions, latestPlayerStats.Conditions, playerId, nameplateLeftEdge, scale, soundManager, hueShift, WorldView.Normal, ConditionManager.numPlayerConditionColumns);

		existingPlayerStats.Conditions = latestPlayerStats.Conditions;
	}

	updateConditions(existingConditions: Conditions, compareConditions: Conditions, creatureId: number, rightEdge: number, scale: number, soundManager: ISoundManager, hueShift = 0, worldView: WorldView = WorldView.Normal, numColumns = 1) {
		let conditionFrameIndex = 0;
		for (const item in Conditions) {
			if (!isNaN(Number(item))) {
				continue;
			}

			const condition: Conditions = Conditions[item as keyof typeof Conditions];
			if (condition !== Conditions.None) {
				if ((existingConditions & condition) !== (compareConditions & condition)) {
					if ((existingConditions & condition) === condition) { // Bit is set.
						this.removeCondition(creatureId, conditionFrameIndex, rightEdge, scale, worldView, numColumns, soundManager);
					}
					else {
						this.addCondition(rightEdge, conditionFrameIndex, creatureId, soundManager, scale, hueShift, worldView, numColumns);
					}
				}
				conditionFrameIndex++;
			}
		}
	}

	restackPlayerConditions(iGetPlayerX: IGetPlayerX & ITextFloater, iNameplateRenderer: INameplateRenderer, soundManager: ISoundManager, context: CanvasRenderingContext2D, existingPlayerStats: PlayerStats, players: Character[], worldView: WorldView = WorldView.Normal) {
		if (!existingPlayerStats.Conditions)
			return;

		const playerIndex: number = iGetPlayerX.getPlayerIndex(existingPlayerStats.PlayerId);
		const targetX: number = this.getNameplateLeftEdge(iNameplateRenderer, context, iGetPlayerX, playerIndex, players);

		const scale = this.getConditionScale(iNameplateRenderer, context, iGetPlayerX, playerIndex, players);
		this.stackConditionSprites(existingPlayerStats.PlayerId, targetX, scale, worldView, soundManager, ConditionManager.numPlayerConditionColumns);

	}

	stackConditionSprites(playerId: number, targetX: number, scale: number, worldView: WorldView, soundManager: ISoundManager, numColumns: number) {
		const targetY = 1080;

		const conditionSprites: Array<SpriteProxy> = this.getSpritesForCreature(this.conditions, playerId);
		this.animateSpritesIntoPosition(conditionSprites, 0, numColumns, targetX, targetY, ConditionManager.conditionIconLength * scale, this.conditions.originX, this.conditions.originY, worldView, soundManager);

		const glowSprites: Array<SpriteProxy> = this.getSpritesForCreature(this.conditionGlow, playerId);
		this.animateSpritesIntoPosition(glowSprites, 0, numColumns, targetX, targetY, ConditionManager.conditionIconLength * scale, this.conditionGlow.originX, this.conditionGlow.originY, worldView);
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

			this.stackConditionSprites(player.playerID, x, scale, worldView, soundManager, ConditionManager.numPlayerConditionColumns);
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
		const conditionSpritesToMove: Array<SpriteProxy> = this.getAllSpritesFromStack(sprites, creature.Index);
		this.animateSpritesIntoPosition(conditionSpritesToMove, 0, ConditionManager.numNpcConditionsPerRow, x, startY, conditionIconLength, sprites.originX, sprites.originY, WorldView.Flipped, soundManager);
	}

	moveNpcConditionsTo(creatureIndex: number, targetX: number, moveTime: number, delayMs: number, soundManager: SoundManager) {
		this.moveNpcConditionSpritesTo(this.conditions, creatureIndex, targetX, moveTime, delayMs);
		this.moveNpcConditionSpritesTo(this.conditionGlow, creatureIndex, targetX, moveTime, delayMs);
	}

	moveNpcConditionSpritesTo(sprites: Sprites, creatureId: number, targetX: number, moveTime: number, delayMs: number) {
		// Since we are moving X we must account for the originX. Also NpcCondition sprites are anchored at the bottom right of the tile.
		const adjustedX: number = targetX - sprites.originX + ConditionManager.conditionLengthIncludingMargin;
		sprites.spriteProxies.forEach((sprite) => {
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
		sprites.spriteProxies.forEach((sprite) => {
			const conditionState: ConditionState = sprite.data as ConditionState;
			if (conditionState && conditionState.creatureId === creatureId) {
				sprite.fadeOutAfter(delayMs, fadeTimeMs);
			}
		});
	}

}

