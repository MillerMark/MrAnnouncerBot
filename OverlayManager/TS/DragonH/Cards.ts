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

class RevealCardStateData {
	constructor(public revealCardIndex: number, public xPos: number, public yPos: number, public offscreenY: number, public hueShift: number, public userName: string, public characterId: number, public fillColor: string, public outlineColor: string
	) {

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
	FillColor: string;
	OutlineColor: string;
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
	CardsToPlay: StreamlootsCard[];
	CardsToReveal: StreamlootsCard[];
}

class CardCommandDto {
	Command: string;
}

class ViewerRollDto {
	Name: string;
	RollId: string;
	RollStr: string;
	FontColor: string;
	OutlineColor: string;
	QueuePosition: number;
	constructor() {

	}
}

class ViewerRollQueueEntry extends ViewerRollDto {
	found = false;
	textEffect: TextEffect;
	constructor() {
		super();
	}

	initializeFrom(viewerRollDto: ViewerRollDto) {
		this.Name = viewerRollDto.Name;
		this.RollId = viewerRollDto.RollId;
		this.RollStr = viewerRollDto.RollStr;
		this.FontColor = viewerRollDto.FontColor;
		this.OutlineColor = viewerRollDto.OutlineColor;
		this.QueuePosition = viewerRollDto.QueuePosition;
	}
}

class ViewerQueueDto extends CardCommandDto {
	ViewerRollDto: Array<ViewerRollDto>;

}

class CardHandDto extends CardCommandDto {
	Purchase: StreamlootsPurchase;
	Card: StreamlootsCard;
	CharacterId: number;
	Hands: Array<StreamlootsHand>;
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
	parentTextAnimations: Animations;

	constructor() {
	}

	initialize(iGetPlayerX: IGetPlayerX, iGetCreatureX: IGetCreatureX, soundManager: ISoundManager, animations: Animations) {
		this.iGetPlayerX = iGetPlayerX;
		this.soundManager = soundManager;
		this.iGetCreatureX = iGetCreatureX;
		this.parentTextAnimations = animations;
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

	getCardImageName(card: StreamlootsCard): string {
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

	static readonly creditLifespanMs: number = 3500;

	showCard(card: StreamlootsCard, characterId: number) {
		let { xPos, yPos } = this.getBigCardCenter(card);

		// TODO: Consider making showCard show the new card as selected, like this:
		//` ![](E259CDC8221A324229E251427533EF88.png)
		const imageIndex: number = this.knownCards.addImage(this.getCardImageName(card));
		const sprite: SpriteProxy = this.knownCards.add(960, 540, imageIndex);
		sprite.scale = CardManager.justPlayedCardScale;
		const entryTime = 1500;
		sprite.ease(performance.now(), Random.between(-500, 1920), 1080, xPos - this.knownCards.originX, yPos - this.knownCards.originY, entryTime);
		const degreesToSpin = 90;
		const initialScale = 0.5;
		sprite.setInitialScale(initialScale);
		sprite.autoScaleFactorPerSecond = 1.8;
		sprite.autoScaleMaxScale = 1;
		sprite.easeSpin(performance.now(), -degreesToSpin, Random.between(-3, 3), entryTime);
		sprite.expirationDate = performance.now() + 4800;
		sprite.fadeInTime = 600;
		sprite.fadeOutTime = 1000;
		console.log('showCard.characterId: ' + characterId);
		const credit: TextEffect = this.addCardPlayedByCredit(xPos, yPos, card.UserName, characterId, card.FillColor, card.OutlineColor);
		credit.expirationDate = performance.now() + entryTime + CardManager.creditLifespanMs;
		credit.delayStart = 3 * entryTime / 4;
	}

	static readonly cardHeight: number = 424;
	static readonly verticalSpaceBetweenCardsAndSelectedCard: number = 6;
	static readonly cardWidth: number = 280;
	static readonly inHandCardHeight: number = 88;
	static readonly inHandCardSeparatorMargin: number = 3;
	static readonly selectedCardScale: number = 1;
	static readonly justPlayedCardScale: number = 1;
	static readonly selectionTransitionTime: number = 220;  // ms
	static readonly selectionTransitionFadeOutTime: number = 130;  // ms
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
			else {
				const newPos: Vector = this.getNewBigCardJustPlayedPosition();
				xPos = newPos.x;
				yPos = newPos.y;
			}
		}
		else {
			xPos = this.iGetPlayerX.getPlayerX(playerIndex);
			yPos = CardManager.playerCardCenterY;
		}
		return { xPos, yPos };
	}

	bigCardPositionAvailable(xPos: number, depth = 0): boolean {
		const now: number = performance.now();
		const foundMap: Map<number, number> = new Map();
		for (let i = 0; i < this.knownCards.spriteProxies.length; i++) {
			const card: SpriteProxy = this.knownCards.spriteProxies[i];
			if (card.scale === CardManager.justPlayedCardScale || card.autoScaleMaxScale === CardManager.justPlayedCardScale) {
				if (!card.fadingOut(now)) {
					let toX: number = card.x + this.knownCards.originX;
					if (card.easePointStillActive(now))
						toX = card.easePoint.toX + this.knownCards.originX;
					if (toX === xPos) {
						if (foundMap.has(toX))
							foundMap.set(toX, foundMap.get(toX) + 1);
						else
							foundMap.set(toX, 1);
						if (foundMap.get(toX) >= depth)
							return false;
					}
				}
			}
		}
		return true;
	}

	getNewBigCardJustPlayedPosition(): Vector {
		const yPos = 540;
		const centerX = 960;
		let xPos = centerX;

		const cardOffset: number = CardManager.justPlayedCardScale * CardManager.cardWidth * 1.08;
		for (let depth = 0; depth < 8; depth++) {
			if (this.bigCardPositionAvailable(centerX, depth))
				return new Vector(centerX, yPos);
			for (let i = 1; i < 3; i++) {
				xPos = centerX + cardOffset * i;
				if (this.bigCardPositionAvailable(xPos, depth))
					return new Vector(xPos, yPos);
				xPos = centerX - cardOffset * i;
				if (this.bigCardPositionAvailable(xPos, depth))
					return new Vector(xPos, yPos);
			}
		}

		return new Vector(centerX, yPos);
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

	playCards(hands: StreamlootsHand[]) {
		hands.forEach((hand: StreamlootsHand) => {
			this.playCardsFromHand(hand);
		});
		this.updateHands(hands);
	}

	revealCards(hands: StreamlootsHand[]) {
		const timeBetweenHands = 150;
		let delayStart = 0;
		hands.forEach((hand: StreamlootsHand) => {
			if (this.revealCardsFromHand(hand, delayStart))
				delayStart += timeBetweenHands;
		});
		this.updateHands(hands);
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
			const imageIndex: number = this.knownCards.addImage(this.getCardImageName(card));

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
				this.playCardDealtSound();
			}
		});

		this.removeAnyOrphanCards(hand.CharacterId);

		if (foundExistingCards) {
			this.reorderCards(hand, xPos, yPos, leftOffsetToStart);
		}

		if (!this.selectionChanged(hand))
			return;

		this.removeExistingSelectionGlow(hand.CharacterId);

		if (hand.SelectedCard !== null) {
			this.playCardDealtSound();
			const angleOffset: number = Random.between(-2, 2);
			// Add big glow...
			const { xPos, yPos } = this.getBigCardCenter(hand.SelectedCard);
			this.addBigCardGlow(hand, xPos, yPos, angleOffset);

			// Add big card...
			this.addBigCard(hand, xPos, yPos, angleOffset);

			// Add small glow:
			this.addSmallCardGlow(hand);
		}
	}

	playCardDealtSound(delayMs = 0) {
		this.soundManager.playMp3In(delayMs, 'Cards/Deal[10]');
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

	private addBigCard(hand: StreamlootsHand, xPos: number, yPos: number, angleOffset = 0) {
		const imageIndex: number = this.knownCards.addImage(this.getCardImageName(hand.SelectedCard));
		const bigCard: SpriteProxy = this.knownCards.add(xPos, yPos, imageIndex);
		bigCard.scale = CardManager.selectedCardScale;
		bigCard.data = new CardStateData(hand.SelectedCard.Guid, hand.CharacterId, 0);
		bigCard.fadeInTime = CardManager.selectionTransitionTime;
		bigCard.rotation = angleOffset;
		this.addCardPlayedByCredit(xPos, yPos, hand.SelectedCard.UserName, hand.CharacterId, hand.SelectedCard.FillColor, hand.SelectedCard.OutlineColor);
	}

	static readonly CardMidScreen: number = 515;

	private addCardPlayedByCredit(xPos: number, yPos: number, UserName: string, characterId: number, fillColor: string, outlineColor: string): TextEffect {
		this.hideCreditsFor(characterId);

		const fontSize = 48;
		const textCardMargin = 8;
		const halfCardPlusTextOffset: number = CardManager.cardHeight / 2 + fontSize / 2 + textCardMargin;

		let verticalThrust: number;
		let yOffset: number;
		if (yPos < CardManager.CardMidScreen) {
			yOffset = -halfCardPlusTextOffset;
			verticalThrust = -0.06;
		}
		else {
			yOffset = halfCardPlusTextOffset;
			verticalThrust = 0.06;
		}

		const credit: TextEffect = this.parentTextAnimations.addText(new Vector(xPos, yPos + yOffset), UserName, CardManager.creditLifespanMs);
		credit.verticalThrust = verticalThrust;
		credit.fontSize = fontSize;
		credit.fontName = 'Enchanted Land';
		credit.fontColor = fillColor;
		credit.outlineColor = outlineColor;
		credit.outlineThickness = 4;
		credit.scale = 1;
		credit.fadeOutTime = 2900;
		credit.fadeInTime = 450;
		credit.textAlign = 'center';
		credit.textBaseline = 'middle';
		credit.data = characterId;
		credit.targetScale = 0.7;
		credit.autoScaleFactorPerSecond = 0.96;
		return credit;
	}

	private hideCreditsFor(characterId: number) {
		if (characterId === -2147483648)
			return;
		const now: number = performance.now();
		this.parentTextAnimations.animationProxies.forEach((animationProxy: AnimatedElement) => {
			if (animationProxy.data === characterId) {
				const lifeRemaining: number = animationProxy.getLifeRemaining(now);
				animationProxy.opacity = animationProxy.getAlpha(now);
				animationProxy.fadeOutNow(Math.min(200, lifeRemaining));
				if (animationProxy instanceof TextEffect) {
					animationProxy.targetScale = animationProxy.scale;
				}
			}
		});
	}

	private addBigCardGlow(hand: StreamlootsHand, xPos: number, yPos: number, angleOffset = 0) {
		const selectionGlow: SpriteProxy = this.selectedCardGlow.addShifted(xPos, yPos, -1, hand.HueShift + Random.between(-10, 10));
		selectionGlow.scale = CardManager.selectedCardScale;
		selectionGlow.data = new CardStateData(hand.SelectedCard.Guid, hand.CharacterId, 0);
		selectionGlow.fadeInTime = CardManager.selectionTransitionTime;
		selectionGlow.rotation = angleOffset;
	}

	private removeExistingSelectionGlow(characterId: number) {
		const timeBeforeFadeout = 200;
		this.selectedCardGlow.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData && sprite.data.characterId === characterId) {
				sprite.fadeOutAfter(timeBeforeFadeout, CardManager.selectionTransitionFadeOutTime);
				const bigCard: SpriteProxy = this.getCard(sprite.data.guid, characterId, CardManager.selectedCardScale);
				if (bigCard) {
					bigCard.fadeOutAfter(timeBeforeFadeout, CardManager.selectionTransitionFadeOutTime);
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
		this.knownCards.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				if (sprite.data.characterId === characterId && sprite.data.found === false && sprite.scale === CardManager.inHandScale) {
					sprite.fadeOutNow(500);
					sprite.data.fadingOut = true;
				}
			}
		});
	}

	getAllGlowSprites(hand: StreamlootsHand, scaleFilter: number): SpriteProxy[] {
		return this.getCardSpritesAtScale(this.selectedCardGlow, hand, scaleFilter);
	}

	getAllCardSprites(hand: StreamlootsHand, scaleFilter: number): SpriteProxy[] {
		return this.getCardSpritesAtScale(this.knownCards, hand, scaleFilter);
	}

	private getCardSpritesAtScale(sprites: Sprites, hand: StreamlootsHand, scaleFilter: number) {
		const resultSprites: Array<SpriteProxy> = [];
		sprites.spriteProxies.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
				if (sprite.data.characterId === hand.CharacterId && sprite.scale === scaleFilter && !sprite.data.fadingOut) {
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
		return this.getOffscreenYPlusAmount(hand, 50);
	}

	private getOffscreenRevealCardY(hand: StreamlootsHand) {
		return this.getOffscreenYPlusAmount(hand, 250);
	}

	private getOffscreenYPlusAmount(hand: StreamlootsHand, offscreenAmount: number): number {
		if (hand.CharacterId < 0)
			return -offscreenAmount;
		else
			return 1080 + offscreenAmount;
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
				if (cardSprite.data.characterId === characterId && cardSprite.data.guid === cardGuid && !cardSprite.data.found && !cardSprite.data.fadingOut && cardSprite.scale === matchingScale) {
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
		targetX += InGameCreatureManager.creatureScrollWidth / 2;

		const heldCards: Array<SpriteProxy> = this.getAllCardsHeldBy(creature);

		const glowCards: Array<SpriteProxy> = this.getAllActiveGlowHeldBy(creature);
		glowCards.forEach((sprite: SpriteProxy) => {
			if (sprite.data instanceof CardStateData) {
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

	static readonly timeBetweenCards: number = 1000;
	playCardsFromHand(hand: StreamlootsHand) {
		const hueShift: number = hand.HueShift;

		let delayStart = 0;
		if (hand.CardsToPlay.length > 0) {
			hand.CardsToPlay.forEach((card: StreamlootsCard) => {
				const { xPos, yPos } = this.getBigCardCenter(card);
				const cardName: string = this.getCardImageName(card);
				this.playCard(cardName, xPos, yPos, delayStart, hueShift, card.UserName, hand.CharacterId, card.FillColor, card.OutlineColor);

				delayStart += CardManager.timeBetweenCards;
			});
		}
	}

	private playCard(cardName: string, xPos: number, yPos: number, delayStart: number, hueShift: number, userName: string, characterId: number, fillColor: string, outlineColor: string) {
		const imageIndex: number = this.knownCards.addImage(cardName);
		const cardSprite: SpriteProxy = this.addBigCardSprite(this.knownCards, xPos, yPos, imageIndex, 0, delayStart);
		this.showCardPlayAnimation(cardSprite, delayStart, xPos, yPos, hueShift, userName, characterId, fillColor, outlineColor);
	}

	private showCardPlayAnimation(cardSprite: SpriteProxy, delayStart: number, xPos: number, yPos: number, hueShift: number, userName: string, characterId: number, fillColor: string, outlineColor: string) {
		cardSprite.fadeOutAfter(delayStart + 1000, 1000);
		const backGlow: SpriteProxy = this.addBigCardSprite(this.playedCardBackGlow, xPos, yPos, 0, hueShift + Random.between(-20, 20), delayStart);
		const frontGlow: SpriteProxy = this.addBigCardSprite(this.playedCardFrontGlow, xPos, yPos, 0, hueShift + Random.between(-20, 20), delayStart);
		this.soundManager.playMp3In(delayStart, 'Cards/Play[9]');
		const credit: TextEffect = this.addCardPlayedByCredit(xPos, yPos, userName, characterId, fillColor, outlineColor);
		this.addPlayCardMotion(credit, delayStart);
		this.correctMotionPaths(yPos, credit, cardSprite, backGlow, frontGlow);
	}

	private correctMotionPaths(yPos: number, credit: TextEffect, cardSprite: SpriteProxy, backGlow: SpriteProxy, frontGlow: SpriteProxy) {
		if (yPos > CardManager.CardMidScreen) {
			credit.verticalThrust = -credit.verticalThrust;
			cardSprite.verticalThrustOverride = -cardSprite.verticalThrustOverride;
			backGlow.verticalThrustOverride = -backGlow.verticalThrustOverride;
			frontGlow.verticalThrustOverride = -frontGlow.verticalThrustOverride;
			frontGlow.velocityY = -frontGlow.velocityY;
			backGlow.velocityY = -backGlow.velocityY;
			cardSprite.velocityY = -cardSprite.velocityY;
			credit.velocityY = 5;
		}
		else {
			credit.velocityY = -5;
		}
	}

	revealStep3(revealCard: SpriteProxy) {
		if (revealCard.data instanceof RevealCardStateData) {
			this.addPlayCardMotion(revealCard, 0);
			this.showCardPlayAnimation(revealCard, 0, revealCard.data.xPos, revealCard.data.yPos, revealCard.data.hueShift, revealCard.data.userName, revealCard.data.characterId, revealCard.data.fillColor, revealCard.data.outlineColor);
		}
	}

	revealStep2(secretCard: SpriteProxy) {
		const timeToShowCard = 2500;
		if (secretCard.data instanceof RevealCardStateData) {
			secretCard.fadeOutNow(350);
			const revealCard: SpriteProxy = this.knownCards.insert(secretCard.data.xPos, secretCard.data.yPos, secretCard.data.revealCardIndex);
			revealCard.data = secretCard.data;
			this.secretCardBurn.add(secretCard.data.xPos, secretCard.data.yPos, 0);
			this.soundManager.safePlayMp3('Spells/GunpowderFlare');
			setTimeout(this.revealStep3.bind(this), timeToShowCard, revealCard);
		}
	}

	revealCardsFromHand(hand: StreamlootsHand, timeOffset: number): boolean {
		if (hand.CardsToReveal.length === 0)
			return false;

		const hueShift: number = hand.HueShift;
		const now: number = performance.now();
		const timeToSpinIn = 700;
		const timeBetweenCards = timeToSpinIn + 2500;
		const offscreenY: number = this.getOffscreenRevealCardY(hand);
		const originX: number = this.knownCards.originX;
		const originY: number = this.knownCards.originY;
		let delayStart = timeOffset;
		hand.CardsToReveal.forEach((card: StreamlootsCard) => {
			const { xPos, yPos } = this.getBigCardCenter(card);
			const secretCardIndex: number = this.knownCards.addImage("Secret Card");
			const revealCardIndex: number = this.knownCards.addImage(card.CardName);
			const secretCard: SpriteProxy = this.knownCards.add(xPos, offscreenY, secretCardIndex);

			secretCard.ease(now + delayStart, xPos - originX, offscreenY - originY, xPos - originX, yPos - originY, timeToSpinIn);
			secretCard.easeSpin(now + delayStart, -70, 0, timeToSpinIn);
			secretCard.data = new RevealCardStateData(revealCardIndex, xPos, yPos, offscreenY, hueShift, card.UserName, hand.CharacterId, card.FillColor, card.OutlineColor);
			this.playCardDealtSound(delayStart + timeToSpinIn / 2);
			setTimeout(this.revealStep2.bind(this), delayStart + timeToSpinIn, secretCard);

			delayStart += timeBetweenCards;
		});
		return true;
	}

	private addBigCardSprite(sprites: Sprites, xPos: number, yPos: number, imageIndex: number, hueShift: number, delayStart: number): SpriteProxy {
		const cardSprite: SpriteProxy = sprites.addShifted(xPos, yPos, imageIndex, hueShift);
		this.addPlayCardMotion(cardSprite, delayStart);
		return cardSprite;
	}

	private addPlayCardMotion(element: AnimatedElement, delayStart: number) {
		element.delayStart = delayStart;
		element.fadeInTime = 300;
		element.verticalThrustOverride = 6;
		element.velocityY = -7;
		element.velocityX = 4;
	}

	viewerRollQueueEntries: Array<ViewerRollQueueEntry> = [];

	updateViewerRollQueue(viewerRollQueueDto: ViewerQueueDto) {
		// Inside a foreach across the list I'm storing here, mark all as not found.
		this.viewerRollQueueEntries.forEach((existingViewerRoll: ViewerRollQueueEntry) => {
			existingViewerRoll.found = false;
		});

		viewerRollQueueDto.ViewerRollDto.forEach((viewerRollDto: ViewerRollDto) => {
			if (!this.markAsFound(viewerRollDto.RollId, viewerRollDto.QueuePosition))
				this.addRollToQueue(viewerRollDto);
		});

		this.removeAnyOrphanViewerRollsInTheQueue();

		// Get everything in the right position.
		this.moveViewerRollQueueTextsIntoPosition();
	}

	removeAnyOrphanViewerRollsInTheQueue() {
		for (let i = this.viewerRollQueueEntries.length - 1; i >= 0; i--) {
			const existingViewerRoll: ViewerRollQueueEntry = this.viewerRollQueueEntries[i];
			if (!existingViewerRoll.found) {
				console.log(`Removing ${existingViewerRoll.RollId}...`);
				existingViewerRoll.textEffect.fadeOutNow(250);  // Fade it out.
				this.viewerRollQueueEntries.splice(i, 1);  // Remove this orphan
			}
		}
	}

	markAsFound(rollId: string, queuePosition: number) {
		let foundRoll = false;
		this.viewerRollQueueEntries.forEach((viewerRollQueueEntry: ViewerRollQueueEntry) => {
			if (viewerRollQueueEntry.RollId === rollId) {
				console.log('found: ' + viewerRollQueueEntry.RollId);
				viewerRollQueueEntry.found = true;
				viewerRollQueueEntry.QueuePosition = queuePosition;
				foundRoll = true;
			}
		});
		return foundRoll;
	}

	moveViewerRollQueueTextsIntoPosition() {
		const now: number = performance.now();
		const timeBetweenMoves = 150;
		let delay = 0;
		this.viewerRollQueueEntries.forEach((viewerRollQueueEntry: ViewerRollQueueEntry) => {
			const textEffect: TextEffect = viewerRollQueueEntry.textEffect;
			textEffect.ease(now + delay, textEffect.x, textEffect.y, textEffect.x, this.getQueueEntryY(viewerRollQueueEntry.QueuePosition), 400);
			delay += timeBetweenMoves;
		});
	}

	static readonly dieQueueTop: number = 435;
	static readonly dieQueueBottom: number = 894;
	static readonly dieQueueHeight: number = CardManager.dieQueueBottom - CardManager.dieQueueTop;
	static readonly numDieQueueEntries: number = 20;
	static readonly dieQueueFontSize: number = CardManager.dieQueueHeight / CardManager.numDieQueueEntries;

	addRollToQueue(viewerRollDto: ViewerRollDto) {
		const viewerRollQueueEntry: ViewerRollQueueEntry = new ViewerRollQueueEntry();
		viewerRollQueueEntry.found = true;
		viewerRollQueueEntry.initializeFrom(viewerRollDto);

		const queueEntryText = `${viewerRollQueueEntry.Name} - ${viewerRollQueueEntry.RollStr} `;
		const viewerName: TextEffect = this.parentTextAnimations.addText(new Vector(1920, this.getQueueEntryY(viewerRollQueueEntry.QueuePosition)), queueEntryText);
		viewerName.fontSize = CardManager.dieQueueFontSize;
		viewerName.fontName = DragonFrontGame.clockFontName;

		// brighten the color so we have high contrast against a darker background.
		const hsl: HueSatLight = HueSatLight.fromHex(viewerRollQueueEntry.FontColor);
		if (hsl.light < 0.75)
			hsl.light = 0.75;
		viewerName.fontColor = hsl.toHex();
		//viewerName.fontColor = viewerRollQueueEntry.FontColor;
		//viewerName.outlineColor = viewerRollQueueEntry.OutlineColor;
		viewerName.outlineThickness = 0;
		viewerName.scale = 1;
		viewerName.fadeInTime = 250;
		viewerName.textAlign = 'right';
		viewerName.textBaseline = 'alphabetic';
		console.log(viewerName);
		//credit.data = viewerRollQueueEntry;
		viewerRollQueueEntry.textEffect = viewerName;


		this.viewerRollQueueEntries.push(viewerRollQueueEntry);
	}

	getQueueEntryY(queuePosition: number): number {
		return CardManager.dieQueueBottom - CardManager.dieQueueFontSize * queuePosition;
	}
}
