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
		this.selectedCardGlow.originX = 192;
		this.selectedCardGlow.originY = 266;
	}

	update(nowMs: number) {
		this.knownCards.updatePositions(nowMs);
		this.selectedCardGlow.updatePositions(nowMs);
	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
		this.selectedCardGlow.draw(context, nowMs);
		this.knownCards.draw(context, nowMs);
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
	static readonly inHandScale: number = CardManager.inHandCardHeight / CardManager.cardHeight;
	static readonly inHandCardWidth: number = CardManager.cardWidth * CardManager.inHandScale;
	static readonly inHandCardSpace: number = CardManager.inHandCardWidth + CardManager.inHandCardSeparatorMargin;

	static readonly playerCardCenterY: number = 500;
	static readonly yAdjustOverlapScrollBottom: number = -30;
	static readonly bigCardHandMargin: number = 4;

	private getBigCardCenter(card: StreamlootsCard) {
		let xPos = 960;
		let yPos = 540;
		const playerIndex: number = this.iGetPlayerX.getPlayerIndexFromName(card.Target);
		if (playerIndex < 0) {
			const creature: InGameCreature = this.iGetCreatureX.getInGameCreatureByName(card.Target);
			if (creature) {
				xPos = this.iGetCreatureX.getX(creature) + InGameCreatureManager.miniScrollWidth / 2.0;
				yPos = InGameCreatureManager.NpcScrollHeight + CardManager.yAdjustOverlapScrollBottom + CardManager.cardHeight / 2 + CardManager.bigCardHandMargin + CardManager.inHandCardHeight;
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

	showHandUI(hand: StreamlootsHand, handOrientation: HandOrientation, xPos: number, yPos: number) {
		const handWidth: number = hand.Cards.length * CardManager.inHandCardSpace;
		const leftOffsetToStart: number = -handWidth / 2.0 + CardManager.inHandCardSpace / 2;
		let needToReorderCards = false;
		this.clearAllCardFoundFlags(hand.CharacterId);
		let offset = leftOffsetToStart;
		hand.Cards.forEach((card: StreamlootsCard) => {
			// TODO: Support secret cards.
			const imageIndex: number = this.knownCards.addImage(this.getCardName(card));

			if (this.alreadyFoundCard(card.Guid, hand.CharacterId, CardManager.inHandScale)) {
				needToReorderCards = true;
			}
			else {
				const offscreenY: number = this.getOffscreenY(hand);
				const cardSprite: SpriteProxy = this.knownCards.add(xPos + offset, offscreenY, imageIndex);
				cardSprite.scale = CardManager.inHandScale;
				cardSprite.data = new CardStateData(card.Guid, hand.CharacterId, offset);
				offset += CardManager.inHandCardSpace;
				cardSprite.fadeInTime = 500;
				cardSprite.ease(performance.now(), cardSprite.x, offscreenY - this.knownCards.originY, cardSprite.x, yPos - this.knownCards.originY, 500);
			}
		});

		//this.removeAnyOrphanCards(hand.CharacterId);

		if (needToReorderCards) {
			this.reorderCards(hand, xPos, yPos, leftOffsetToStart);
		}

		this.removeExistingSelectionGlow(hand.CharacterId);

		if (hand.SelectedCard !== null) {
			// Add big glow...
			const { xPos, yPos } = this.getBigCardCenter(hand.SelectedCard);
			this.addBigCardGlow(hand, xPos, yPos);

			// Add big card...
			this.addBigCard(hand, xPos, yPos);

			// TODO: Add small glow will need an index to differentiate among duplicate cards with the same name.

			// Add small glow:
			const smallCardSprite: SpriteProxy = this.getCard(hand.SelectedCard.Guid, hand.CharacterId, CardManager.inHandScale);
			if (smallCardSprite)
			hand.Cards.forEach((card: StreamlootsCard) => {
				if (card.Guid === hand.SelectedCard.Guid) {
					const selectionSmallGlow: SpriteProxy = this.selectedCardGlow.add(smallCardSprite.x, smallCardSprite.y, -1);
					selectionSmallGlow.scale = CardManager.inHandScale;
					selectionSmallGlow.data = new CardStateData(hand.SelectedCard.Guid, hand.CharacterId, 0);
					selectionSmallGlow.fadeInTime = 500;
				}
			});
		}
	}

	private addBigCard(hand: StreamlootsHand, xPos: number, yPos: number) {
		const imageIndex: number = this.knownCards.addImage(this.getCardName(hand.SelectedCard));
		const cardSprite: SpriteProxy = this.knownCards.add(xPos, yPos, imageIndex);
		cardSprite.scale = CardManager.selectedCardScale;
		cardSprite.data = new CardStateData(hand.SelectedCard.Guid, hand.CharacterId, 0);
		cardSprite.fadeInTime = 500;
	}

	private addBigCardGlow(hand: StreamlootsHand, xPos: number, yPos: number) {
		const selectionGlow: SpriteProxy = this.selectedCardGlow.add(xPos, yPos, -1);
		selectionGlow.scale = CardManager.selectedCardScale;
		selectionGlow.data = new CardStateData(hand.SelectedCard.Guid, hand.CharacterId, 0);
		selectionGlow.fadeInTime = 500;
	}

	removeExistingSelectionGlow(characterId: number) {
		this.selectedCardGlow.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData && sprite.data.characterId === characterId)
				sprite.fadeOutNow(500);
		});
	}

	reorderCards(hand: StreamlootsHand, xPos: number, yPos: number, leftOffsetToStart: number) {
		const allCardSprites: Array<SpriteProxy> = this.getAllCardSprites(hand, CardManager.inHandScale);
		let offset: number = leftOffsetToStart;
		allCardSprites.forEach((sprite: SpriteProxy) => {
			sprite.ease(performance.now(), sprite.x, sprite.y, xPos + offset - this.knownCards.originX, yPos - this.knownCards.originY, 500);
			if (sprite.data instanceof CardStateData) {
				sprite.data.offset = offset;
			}

			offset += CardManager.inHandCardSpace;
		});
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

	getAllCardSprites(hand: StreamlootsHand, scaleFilter: number): SpriteProxy[] {
		const allCardSprites: Array<SpriteProxy> = [];
		this.knownCards.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				if (sprite.data.characterId === hand.CharacterId && sprite.scale === scaleFilter) {
					allCardSprites.push(sprite);
				}
			}
		});
		return allCardSprites;
	}

	hideHandUI(hand: StreamlootsHand) {
		this.knownCards.spriteProxies.forEach((sprite: SpriteProxy) => {
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
				if (cardSprite.data.characterId === characterId)
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

	// Called when a creature's x-position is animated and it slides to the left or right.
	onMoveCreature(creature: InGameCreature, targetX: number, delayMs: number) {
		//console.log('targetX: ' + targetX);
		targetX += InGameCreatureManager.creatureScrollWidth / 2;
		const heldCards: Array<SpriteProxy> = this.getAllCardsHeldBy(creature);
		heldCards.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				const adjustedX: number = targetX + sprite.data.offset - this.knownCards.originX;
				sprite.ease(performance.now() + delayMs, sprite.x, sprite.y, adjustedX, sprite.y, InGameCreatureManager.leftRightMoveTime);
			}
		});
	}
}
