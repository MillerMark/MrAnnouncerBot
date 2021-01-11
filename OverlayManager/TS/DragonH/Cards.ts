enum HandOrientation {
	SelectedCardAbove,
	SelectedCardBelow
}

class CardStateData {
	found = true;
	fadingOut = false;
	constructor(public guid: string, public characterId: number, public offset: number) {

	}
}

class StreamlootsPurchase {
	type: string;
	imageUrl: string;
	soundUrl: string;
	Purchaser: string;
	Recipient: string;
	Quantity: number;

	constructor() {

	}
}

class StreamlootsCard {
	PngFileFound: boolean;
	IsSecret: boolean;
	message: string;
	imageUrl: string;
	videoUrl: string;
	soundUrl: string;
	CardName: string;
	UserName: string;
	Target: string;
	Guid: string;
	constructor() {

	}
}

class StreamlootsHand {
	CharacterId: number;
	HueShift: number;
	IsShown: boolean;
	SelectedCard: StreamlootsCard;
	Cards: StreamlootsCard[];
}

class CardDto {
	Purchase: StreamlootsPurchase;
	Card: StreamlootsCard;
	Hands: Array<StreamlootsHand>;
	Command: string;
}

class CardManager {
	knownCards: Sprites;
	selectedCardGlow: Sprites;
	playedCardBackGlow: Sprites;
	playedCardFrontGlow: Sprites;
	secretCardBurn: Sprites;
	iGetPlayerX: IGetPlayerX;
	iGetCreatureX: IGetCreatureX;
	soundManager: ISoundManager;

	constructor() {
	}

	initialize(iGetPlayerX: IGetPlayerX, iGetCreatureX: IGetCreatureX, soundManager: ISoundManager) {
		this.iGetPlayerX = iGetPlayerX;
		this.soundManager = soundManager;
		this.iGetCreatureX = iGetCreatureX;
	}

	loadResources() {
		Folders.assets = 'GameDev/Assets/DragonH/';
		// TODO: Reorder parameters for Sprites constructor so fps is optional after AnimationStyle.
		this.knownCards = new Sprites('Cards', 0, fps30, AnimationStyle.Static);
		this.knownCards.moves = true;
		this.knownCards.disableGravity();
		this.knownCards.originX = 140;
		this.knownCards.originY = 212;
		this.knownCards.addImage('Secret Card');

		this.selectedCardGlow = new Sprites('Cards/Selected Card/Selected Card', 89, fps30, AnimationStyle.Loop, true);
		this.selectedCardGlow.moves = true;
		this.selectedCardGlow.disableGravity();
		this.selectedCardGlow.originX = 192;
		this.selectedCardGlow.originY = 266;

		this.playedCardBackGlow = new Sprites('Cards/Play Card/PlayCardBack', 38, fps30, AnimationStyle.Sequential, true);
		this.playedCardBackGlow.moves = true;
		this.playedCardBackGlow.disableGravity();
		this.playedCardBackGlow.originX = 301;
		this.playedCardBackGlow.originY = 404;

		this.playedCardFrontGlow = new Sprites('Cards/Play Card/PlayCardFront', 23, fps30, AnimationStyle.Sequential, true);
		this.playedCardFrontGlow.moves = true;
		this.playedCardFrontGlow.disableGravity();
		this.playedCardFrontGlow.originX = 276;
		this.playedCardFrontGlow.originY = 339;

		this.secretCardBurn = new Sprites('Cards/Secret Card Burn/SecretCardBurn', 73, fps30, AnimationStyle.Sequential, true);
		this.secretCardBurn.moves = true;
		this.secretCardBurn.disableGravity();
		this.secretCardBurn.originX = 205;
		this.secretCardBurn.originY = 381;
	}

	update(nowMs: number) {
		this.playedCardBackGlow.updatePositions(nowMs);
		this.knownCards.updatePositions(nowMs);
		this.selectedCardGlow.updatePositions(nowMs);
		this.playedCardFrontGlow.updatePositions(nowMs);
		this.secretCardBurn.updatePositions(nowMs);
	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
		this.playedCardBackGlow.draw(context, nowMs);
		this.selectedCardGlow.draw(context, nowMs);
		this.knownCards.draw(context, nowMs);
		this.playedCardFrontGlow.draw(context, nowMs);
		this.secretCardBurn.draw(context, nowMs);
		//this.showCardDiagnostics(context, nowMs);
	}

	showCardDiagnostics(context: CanvasRenderingContext2D, nowMs: number) {
		this.knownCards.spriteProxies.forEach((sprite: SpriteProxy) => {
			this.showCardDiagnostic(sprite, context, nowMs);
		});
	}

	getCardName(card: StreamlootsCard): string {
		if (card.IsSecret)
			return 'Secret Card';
		if (card.PngFileFound)
			return card.CardName;
		return 'Not Found';
	}

	showCardDiagnostic(sprite: SpriteProxy, context: CanvasRenderingContext2D, nowMs: number) {
		if (!(sprite.data instanceof CardStateData))
			return;

		context.font = '22px Arial';
		context.fillStyle = '#880000';
		context.textAlign = 'center';
		context.textBaseline = 'bottom';

		context.fillText(sprite.data.characterId.toString(), sprite.x + this.knownCards.originX, sprite.y + this.knownCards.originY - CardManager.inHandCardHeight / 2 - 5);
	}

	showCard(card: StreamlootsCard) {
		let { xPos, yPos } = this.getBigCardCenter(card);

		// TODO: Consider making showCard show the new card as selected, like this:
		//` ![](E259CDC8221A324229E251427533EF88.png)
		const imageIndex: number = this.knownCards.addImage(this.getCardName(card));
		const sprite: SpriteProxy = this.knownCards.add(960, 540, imageIndex);
		//sprite.autoRotationDegeesPerSecond = 10;
		const entryTime = 1500;
		sprite.ease(performance.now(), Random.between(-500, 1920), 1080, xPos - this.knownCards.originX, yPos - this.knownCards.originY, entryTime);
		const degreesToSpin = 90;
		const initialScale = 0.5;
		sprite.setInitialScale(initialScale);
		//sprite.autoScaleFactorPerSecond = (1 / initialScale) / (entryTime / 1000);
		sprite.autoScaleFactorPerSecond = 1.8;
		sprite.autoScaleMaxScale = 1;
		sprite.easeSpin(performance.now(), -degreesToSpin, Random.between(-3, 3), entryTime);
		sprite.expirationDate = performance.now() + 8000;
		sprite.fadeInTime = 600;
		sprite.fadeOutTime = 1000;
	}

	static readonly cardHeight: number = 424;
	static readonly verticalSpaceBetweenCardsAndSelectedCard: number = 6;
	static readonly cardWidth: number = 280;
	static readonly inHandCardHeight: number = 88;
	static readonly inHandCardSeparatorMargin: number = 3;
	static readonly selectedCardScale: number = 1;
	static readonly selectionTransitionTime: number = 220;  // ms
	static readonly inHandScale: number = CardManager.inHandCardHeight / CardManager.cardHeight;
	static readonly inHandCardWidth: number = CardManager.cardWidth * CardManager.inHandScale;
	static readonly inHandCardSpace: number = CardManager.inHandCardWidth + CardManager.inHandCardSeparatorMargin;

	static readonly playerCardCenterY: number = 500;
	static readonly yAdjustOverlapScrollBottom: number = -30;
	static readonly bigCardHandMarginBelowInGameCreatureScrolls: number = 8;

	private getBigCardCenter(card: StreamlootsCard) {
		let xPos = 960;
		let yPos = 540;
		const playerIndex: number = this.iGetPlayerX.getPlayerIndexFromName(card.Target);
		if (playerIndex < 0) {
			const creature: InGameCreature = this.iGetCreatureX.getInGameCreatureByName(card.Target);
			if (creature) {
				xPos = this.iGetCreatureX.getX(creature) + InGameCreatureManager.miniScrollWidth / 2.0;
				yPos = InGameCreatureManager.NpcScrollHeight + CardManager.yAdjustOverlapScrollBottom + CardManager.cardHeight / 2 + CardManager.bigCardHandMarginBelowInGameCreatureScrolls + CardManager.inHandCardHeight;
			}
		}
		else {
			xPos = this.iGetPlayerX.getPlayerX(playerIndex);
			yPos = CardManager.playerCardCenterY;
		}
		return { xPos, yPos };
	}

	private getHandCenter(characterId: number) {
		let xPos = 960;
		let yPos = 540;
		if (characterId < 0) {
			const creature: InGameCreature = this.iGetCreatureX.getInGameCreatureByIndex(-characterId);
			if (creature) {
				xPos = this.iGetCreatureX.getX(creature) + InGameCreatureManager.miniScrollWidth / 2.0;
				yPos = InGameCreatureManager.NpcScrollHeight + CardManager.yAdjustOverlapScrollBottom + CardManager.inHandCardHeight / 2;
			}
		}
		else {
			yPos = CardManager.playerCardCenterY + CardManager.cardHeight / 2 + CardManager.verticalSpaceBetweenCardsAndSelectedCard + CardManager.inHandCardHeight / 2;
			xPos = this.iGetPlayerX.getPlayerX(this.iGetPlayerX.getPlayerIndex(characterId));
		}
		return { xPos, yPos };
	}

	updateHands(hands: StreamlootsHand[]) {
		hands.forEach((hand: StreamlootsHand) => {
			this.updateHand(hand);
		});
	}

	updateHand(hand: StreamlootsHand) {
		if (hand.IsShown) {
			this.makeSureAllCardsAreInView(hand);
		}
		else {
			if (this.handIsShown(hand.CharacterId))
				this.hideHandUI(hand);
		}
	}

	makeSureAllCardsAreInView(hand: StreamlootsHand) {
		const handOrientation: HandOrientation = this.getHandOrientation(hand.CharacterId);
		const { xPos, yPos } = this.getHandCenter(hand.CharacterId);

		this.showHandUI(hand, handOrientation, xPos, yPos);
	}

	getHandOrientation(characterId: number): HandOrientation {
		if (characterId < 0)
			return HandOrientation.SelectedCardBelow;
		return HandOrientation.SelectedCardAbove;
	}

	static readonly notSelectedOffset: number = 8;

	getSelectionVerticalOffset(yPos: number, guid: string, selectedCard: StreamlootsCard): number {
		let directionMultiplier: number;
		if (yPos > 540)
			directionMultiplier = 1;
		else
			directionMultiplier = -1;

		const selectedOffset = 0;

		if (!selectedCard || guid !== selectedCard.Guid) {
			return CardManager.notSelectedOffset * directionMultiplier;
		}

		return selectedOffset * directionMultiplier;
	}

	showHandUI(hand: StreamlootsHand, handOrientation: HandOrientation, xPos: number, yPos: number) {
		const handWidth: number = hand.Cards.length * CardManager.inHandCardSpace;
		const leftOffsetToStart: number = -handWidth / 2.0 + CardManager.inHandCardSpace / 2;
		let foundExistingCards = false;
		this.clearAllCardFoundFlags(hand.CharacterId);
		let offset = leftOffsetToStart;
		hand.Cards.forEach((card: StreamlootsCard) => {
			// TODO: Support secret cards.
			const imageIndex: number = this.knownCards.addImage(this.getCardName(card));

			if (this.alreadyFoundCard(card.Guid, hand.CharacterId, CardManager.inHandScale)) {
				foundExistingCards = true;
			}
			else {
				const offscreenY: number = this.getOffscreenY(hand);
				const cardSprite: SpriteProxy = this.knownCards.add(xPos + offset, offscreenY, imageIndex);
				cardSprite.scale = CardManager.inHandScale;
				cardSprite.data = new CardStateData(card.Guid, hand.CharacterId, offset);
				offset += CardManager.inHandCardSpace;
				cardSprite.fadeInTime = 500;
				const selectionOffset: number = this.getSelectionVerticalOffset(yPos, card.Guid, hand.SelectedCard);
				cardSprite.ease(performance.now(), cardSprite.x, offscreenY - this.knownCards.originY, cardSprite.x, yPos - this.knownCards.originY + selectionOffset, 500);
			}
		});

		if (foundExistingCards) {
			this.reorderCards(hand, xPos, yPos, leftOffsetToStart);
		}

		if (!this.selectionChanged(hand))
			return;

		this.removeExistingSelectionGlow(hand.CharacterId);

		if (hand.SelectedCard !== null) {
			// Add big glow...
			const { xPos, yPos } = this.getBigCardCenter(hand.SelectedCard);
			this.addBigCardGlow(hand, xPos, yPos);

			// Add big card...
			this.addBigCard(hand, xPos, yPos);

			// Add small glow:
			this.addSmallCardGlow(hand);
		}
	}

	selectionChanged(hand: StreamlootsHand): boolean {
		const existingGlow: Array<SpriteProxy> = this.getAllActiveGlowByCharacterId(hand.CharacterId);
		if (existingGlow.length > 0) {
			if (!hand.SelectedCard)
				return true;
			const activeGlowByGuid: Array<SpriteProxy> = this.getAllActiveGlowByGuid(hand.SelectedCard.Guid);
			const foundExistingGlow: boolean = activeGlowByGuid.length > 0;
			return !foundExistingGlow;
		}

		// There are no glowing selection-indicating sprites on screen for this character.
		if (hand.SelectedCard)
			return true;
		else 
			return false;
	}

	private addSmallCardGlow(hand: StreamlootsHand) {
		const smallCardSprite: SpriteProxy = this.getCard(hand.SelectedCard.Guid, hand.CharacterId, CardManager.inHandScale);
		if (smallCardSprite)
			hand.Cards.forEach((card: StreamlootsCard) => {
				if (card.Guid === hand.SelectedCard.Guid) {
					let selectionOffset: number;
					if (smallCardSprite.y < 540)
						selectionOffset = CardManager.notSelectedOffset;
					else
						selectionOffset = -CardManager.notSelectedOffset;

					//console.log('addSmallCardGlow/selectionOffset: ' + selectionOffset);
					const selectionSmallGlow: SpriteProxy = this.selectedCardGlow.addShifted(smallCardSprite.x + this.knownCards.originX, smallCardSprite.y + this.knownCards.originY + selectionOffset, -1, hand.HueShift);
					selectionSmallGlow.scale = CardManager.inHandScale;
					if (smallCardSprite.data instanceof CardStateData)
						selectionSmallGlow.data = new CardStateData(hand.SelectedCard.Guid, hand.CharacterId, smallCardSprite.data.offset);
					else 
						selectionSmallGlow.data = new CardStateData(hand.SelectedCard.Guid, hand.CharacterId, 0);
					selectionSmallGlow.fadeInTime = CardManager.selectionTransitionTime;
				}
			});
	}

	private addBigCard(hand: StreamlootsHand, xPos: number, yPos: number) {
		const imageIndex: number = this.knownCards.addImage(this.getCardName(hand.SelectedCard));
		const cardSprite: SpriteProxy = this.knownCards.add(xPos, yPos, imageIndex);
		cardSprite.scale = CardManager.selectedCardScale;
		cardSprite.data = new CardStateData(hand.SelectedCard.Guid, hand.CharacterId, 0);
		cardSprite.fadeInTime = CardManager.selectionTransitionTime;
	}

	private addBigCardGlow(hand: StreamlootsHand, xPos: number, yPos: number) {
		const selectionGlow: SpriteProxy = this.selectedCardGlow.addShifted(xPos, yPos, -1, hand.HueShift);
		selectionGlow.scale = CardManager.selectedCardScale;
		selectionGlow.data = new CardStateData(hand.SelectedCard.Guid, hand.CharacterId, 0);
		selectionGlow.fadeInTime = CardManager.selectionTransitionTime;
	}

	removeExistingSelectionGlow(characterId: number) {
		this.selectedCardGlow.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData && sprite.data.characterId === characterId) {
				sprite.fadeOutNow(CardManager.selectionTransitionTime);
				const bigCard: SpriteProxy = this.getCard(sprite.data.guid, characterId, CardManager.selectedCardScale);
				if (bigCard) {
					bigCard.fadeOutNow(CardManager.selectionTransitionTime);
				}
			}
		});
	}

	reorderCards(hand: StreamlootsHand, xPos: number, yPos: number, leftOffsetToStart: number) {
		const allCardSprites: Array<SpriteProxy> = this.getAllCardSprites(hand, CardManager.inHandScale);
		let offset: number = leftOffsetToStart;
		allCardSprites.forEach((sprite: SpriteProxy) => {
			const selectionOffset: number = this.getSelectionVerticalOffset(yPos, sprite.data.guid, hand.SelectedCard);
			sprite.ease(performance.now(), sprite.x, sprite.y, xPos + offset - this.knownCards.originX, yPos - this.knownCards.originY + selectionOffset, CardManager.selectionTransitionTime);

			if (sprite.data instanceof CardStateData) {
				const glowSprite: SpriteProxy = this.getGlowSprite(sprite.data.guid, CardManager.inHandScale);
				if (glowSprite) {
					//console.log(`Found glowSprite - easing it from (${glowSprite.x}, ${glowSprite.y}) to (${xPos + offset - this.selectedCardGlow.originX}, ${yPos - this.selectedCardGlow.originY}).`);
					glowSprite.ease(performance.now(), glowSprite.x, glowSprite.y, xPos + offset - this.selectedCardGlow.originX, yPos - this.selectedCardGlow.originY + selectionOffset, CardManager.selectionTransitionTime);

					if (glowSprite.data instanceof CardStateData) {
						glowSprite.data.offset = offset;
					}
				}
				sprite.data.offset = offset;
			}

			offset += CardManager.inHandCardSpace;
		});
	}

	getGlowSprite(guid: string, matchingScale: number): SpriteProxy {
		console.log(this.selectedCardGlow.spriteProxies);
		let result: SpriteProxy = null;
		this.selectedCardGlow.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				if (sprite.data.guid === guid)
					if (sprite.scale === matchingScale) {
						console.log(`found a match!`);
						result = sprite;
					}
			}
		});
		return result;
	}

	removeAnyOrphanCards(characterId: number) {
		this.knownCards.spriteProxies = this.knownCards.spriteProxies.filter((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				if (sprite.data.characterId === characterId && !sprite.data.found) {
					sprite.fadeOutNow(500);
				}
			}
		});
	}

	getAllGlowSprites(hand: StreamlootsHand, scaleFilter: number): SpriteProxy[] {
		return this.getCharacterSpritesAtScale(this.selectedCardGlow, hand, scaleFilter);
	}

	getAllCardSprites(hand: StreamlootsHand, scaleFilter: number): SpriteProxy[] {
		return this.getCharacterSpritesAtScale(this.knownCards, hand, scaleFilter);
	}

	private getCharacterSpritesAtScale(sprites: Sprites, hand: StreamlootsHand, scaleFilter: number) {
		const resultSprites: Array<SpriteProxy> = [];
		sprites.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				if (sprite.data.characterId === hand.CharacterId && sprite.scale === scaleFilter) {
					resultSprites.push(sprite);
				}
			}
		});
		return resultSprites;
	}

	hideHandUI(hand: StreamlootsHand) {
		this.hideSpritesForHand(this.knownCards, hand);
		this.hideSpritesForHand(this.selectedCardGlow, hand);
	}

	private hideSpritesForHand(sprites: Sprites, hand: StreamlootsHand) {
		sprites.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				if (sprite.data.characterId === hand.CharacterId) {
					//sprite.fadeOutNow(500);
					const offscreenY: number = this.getOffscreenY(hand);
					//console.log('offscreenY: ' + offscreenY);
					//console.log(`sprite.y: ${sprite.y}, offscreenY - this.knownCards.originY: ${offscreenY - this.knownCards.originY}`);
					//sprite.ease(performance.now(), 50, 50, 1900, 1000, 500);
					sprite.ease(performance.now(), sprite.x, sprite.y, sprite.x, offscreenY - this.knownCards.originY, 500);
					sprite.fadeOutNow(500);
					sprite.data.fadingOut = true;
				}
			}
		});
	}

	private getOffscreenY(hand: StreamlootsHand) {
		let newY: number;
		const offscreenAmount = 50;
		if (hand.CharacterId < 0)
			newY = -offscreenAmount;
		else
			newY = 1080 + offscreenAmount;
		return newY;
	}

	getCard(cardGuid: string, characterId: number, matchingScale: number): SpriteProxy {
		for (let i = 0; i < this.knownCards.spriteProxies.length; i++) {
			const cardSprite: SpriteProxy = this.knownCards.spriteProxies[i];
			if (cardSprite.data instanceof CardStateData) {
				if (cardSprite.data.characterId === characterId &&
					cardSprite.data.guid === cardGuid &&
					cardSprite.scale === matchingScale) {
					return cardSprite;
				}
			}
		}
		return null;
	}

	alreadyFoundCard(cardGuid: string, characterId: number, matchingScale: number): boolean {
		for (let i = 0; i < this.knownCards.spriteProxies.length; i++) {
			const cardSprite: SpriteProxy = this.knownCards.spriteProxies[i];
			if (cardSprite.data instanceof CardStateData) {
				if (cardSprite.data.characterId === characterId &&
					cardSprite.data.guid === cardGuid &&
					!cardSprite.data.found && !cardSprite.data.fadingOut &&
					cardSprite.scale === matchingScale) {
					cardSprite.data.found = true;
					return true;
				}
			}
		}
		return false;
	}

	clearAllCardFoundFlags(characterId: number): void {
		for (let i = 0; i < this.knownCards.spriteProxies.length; i++) {
			const cardSprite: SpriteProxy = this.knownCards.spriteProxies[i];
			if (cardSprite.data instanceof CardStateData) {
				if (cardSprite.data.characterId === characterId)
					cardSprite.data.found = false;
			}
		}
	}

	handIsShown(characterId: number): boolean {
		for (let i = 0; i < this.knownCards.spriteProxies.length; i++) {
			const cardSprite: SpriteProxy = this.knownCards.spriteProxies[i];
			if (cardSprite.data instanceof CardStateData) {
				if (cardSprite.data.characterId === characterId && !cardSprite.data.fadingOut)
					return true;
			}
		}
		return false;
	}

	getAllCardsHeldBy(creature: InGameCreature): Array<SpriteProxy> {
		const characterId: number = -creature.Index;
		const heldCards: Array<SpriteProxy> = new Array<SpriteProxy>();

		for (let i = 0; i < this.knownCards.spriteProxies.length; i++) {
			const cardSprite: SpriteProxy = this.knownCards.spriteProxies[i];
			if (cardSprite.data instanceof CardStateData) {
				if (cardSprite.data.characterId === characterId)
					heldCards.push(cardSprite);
			}
		}

		return heldCards;
	}

	getAllActiveGlowHeldBy(creature: InGameCreature): Array<SpriteProxy> {
		const characterId: number = -creature.Index;
		return this.getAllActiveGlowByCharacterId(characterId);
	}

	getAllActiveGlowByCharacterId(characterId: number): Array<SpriteProxy> {
		const glowCards: Array<SpriteProxy> = new Array<SpriteProxy>();

		for (let i = 0; i < this.selectedCardGlow.spriteProxies.length; i++) {
			const glowSprite: SpriteProxy = this.selectedCardGlow.spriteProxies[i];
			if (glowSprite.data instanceof CardStateData) {
				if (glowSprite.data.characterId === characterId && !glowSprite.data.fadingOut)
					glowCards.push(glowSprite);
			}
		}

		return glowCards;
	}

	getAllActiveGlowByGuid(guid: string): Array<SpriteProxy> {
		const glowCards: Array<SpriteProxy> = new Array<SpriteProxy>();

		for (let i = 0; i < this.selectedCardGlow.spriteProxies.length; i++) {
			const glowSprite: SpriteProxy = this.selectedCardGlow.spriteProxies[i];
			if (glowSprite.data instanceof CardStateData) {
				if (glowSprite.data.guid === guid && !glowSprite.data.fadingOut)
					glowCards.push(glowSprite);
			}
		}

		return glowCards;
	}

	// Called when an in-game creature's x-position is animated and it slides to the left or right.
	onMoveCreature(creature: InGameCreature, targetX: number, delayMs: number) {
		//console.log('targetX: ' + targetX);
		targetX += InGameCreatureManager.creatureScrollWidth / 2;

		const heldCards: Array<SpriteProxy> = this.getAllCardsHeldBy(creature);

		const glowCards: Array<SpriteProxy> = this.getAllActiveGlowHeldBy(creature);
		glowCards.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				//const selectionOffset: number = this.getSelectedGlowOffset(heldCards, sprite.data.guid);
				const adjustedX: number = targetX + sprite.data.offset - this.selectedCardGlow.originX;
				sprite.ease(performance.now() + delayMs, sprite.x, sprite.y, adjustedX, sprite.y, InGameCreatureManager.leftRightMoveTime);
			}
		});

		heldCards.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				const adjustedX: number = targetX + sprite.data.offset - this.knownCards.originX;
				sprite.ease(performance.now() + delayMs, sprite.x, sprite.y, adjustedX, sprite.y, InGameCreatureManager.leftRightMoveTime);
			}
		});
	}
}
