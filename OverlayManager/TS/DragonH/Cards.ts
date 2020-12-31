enum HandOrientation {
	SelectedCardAbove,
	SelectedCardBelow
}

class CardStateData {
	found = true;
	constructor(public cardName: string, public characterId: number) {

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
	Command: string;
}

class CardManager {
	knownCards: Sprites;
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
		//this.knownCards.add(960, 540, 0);
	}

	update(nowMs: number) {
		this.knownCards.updatePositions(nowMs);
	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
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
		let { xPos, yPos } = this.getCardCenter(card);

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
	static readonly inHandScale: number = CardManager.inHandCardHeight / CardManager.cardHeight;
	static readonly inHandCardWidth: number = CardManager.cardWidth * CardManager.inHandScale;
	static readonly inHandCardSpace: number = CardManager.inHandCardWidth + CardManager.inHandCardSeparatorMargin;

	static readonly playerCardCenterY: number = 500;
	private getCardCenter(card: StreamlootsCard) {
		let xPos = 960;
		let yPos = 540;
		const playerIndex: number = this.iGetPlayerX.getPlayerIndexFromName(card.Target);
		if (playerIndex < 0) {
			const creature: InGameCreature = this.iGetCreatureX.getInGameCreatureByName(card.Target);
			if (creature) {
				xPos = this.iGetCreatureX.getX(creature) + InGameCreatureManager.miniScrollWidth / 2.0;
				yPos = InGameCreatureManager.NpcScrollHeight + CardManager.cardHeight / 2;
			}
		}
		else {
			yPos = CardManager.playerCardCenterY;
			xPos = this.iGetPlayerX.getPlayerX(playerIndex);
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
				const yAdjustOverlapScrollBottom = -30;
				yPos = InGameCreatureManager.NpcScrollHeight + CardManager.inHandCardHeight / 2 + yAdjustOverlapScrollBottom;
			}
		}
		else {
			yPos = CardManager.playerCardCenterY + CardManager.cardHeight / 2 + CardManager.verticalSpaceBetweenCardsAndSelectedCard + CardManager.inHandCardHeight / 2;
			xPos = this.iGetPlayerX.getPlayerX(this.iGetPlayerX.getPlayerIndex(characterId));
		}
		return { xPos, yPos };
	}

	showHands(hands: StreamlootsHand[]) {
		hands.forEach((hand: StreamlootsHand) => {
			this.showHand(hand);
		});
	}

	showHand(hand: StreamlootsHand) {
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
		const initialStartX: number = xPos - handWidth / 2.0 + CardManager.inHandCardSpace / 2;
		let xStart: number = initialStartX;
		let needToReorderCards = false;
		this.clearAllCardFoundFlags(hand.CharacterId);

		hand.Cards.forEach((card: StreamlootsCard) => {
			// TODO: Support secret cards.
			const imageIndex: number = this.knownCards.addImage(this.getCardName(card));

			if (this.alreadyFoundCard(card.CardName, hand.CharacterId)) {
				needToReorderCards = true;
			}
			else {
				const cardSprite: SpriteProxy = this.knownCards.add(xStart, yPos, imageIndex);
				cardSprite.scale = CardManager.inHandScale;
				xStart += CardManager.inHandCardSpace;
				cardSprite.data = new CardStateData(card.CardName, hand.CharacterId);
			}
		});

		this.removeAnyOrphanCards(hand.CharacterId);

		xStart = initialStartX;
		if (needToReorderCards) {
			this.reorderCards(hand, handOrientation, xStart, yPos);
		}
	}

	reorderCards(hand: StreamlootsHand, handOrientation: HandOrientation, xStart: number, yPos: number) {
		const allCardSprites: Array<SpriteProxy> = this.getAllCardSprites(hand);
		allCardSprites.forEach((sprite: SpriteProxy) => {
			// TODO: Use Ease for animation.
			this.knownCards.setXY(sprite, xStart, yPos);
			xStart += CardManager.inHandCardSpace;
		});
	}

	removeAnyOrphanCards(characterId: number) {
		this.knownCards.spriteProxies = this.knownCards.spriteProxies.filter((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				if (sprite.data.characterId === characterId && !sprite.data.found) {
					return false;
				}
			}
			return true;
		});
	}

	getAllCardSprites(hand: StreamlootsHand): SpriteProxy[] {
		const allCardSprites: Array<SpriteProxy> = [];
		this.knownCards.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				if (sprite.data.characterId === hand.CharacterId) {
					allCardSprites.push(sprite);
				}
			}
		});
		return allCardSprites;
	}

	hideHandUI(hand: StreamlootsHand) {

	}

	alreadyFoundCard(cardName: string, characterId: number): boolean {
		for (let i = 0; i < this.knownCards.spriteProxies.length; i++) {
			const cardSprite: SpriteProxy = this.knownCards.spriteProxies[i];
			if (cardSprite.data instanceof CardStateData) {
				if (cardSprite.data.characterId === characterId && cardSprite.data.cardName === cardName && !cardSprite.data.found) {
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

}
