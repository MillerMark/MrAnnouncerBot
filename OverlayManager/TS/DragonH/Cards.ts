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
	}

	showCard(card: StreamlootsCard) {
		let xPos = 960;
		let yPos = 540;
		const playerIndex: number = this.iGetPlayerX.getPlayerIndexFromName(card.Target);
		if (playerIndex < 0) {
			const creature: InGameCreature = this.iGetCreatureX.getInGameCreatureByName(card.Target);
			if (creature) {
				xPos = this.iGetCreatureX.getX(creature) + InGameCreatureManager.miniScrollWidth / 2.0;
				const cardHeight = 424;
				yPos = InGameCreatureManager.NpcScrollHeight + cardHeight / 2;
			}
			// TODO: Get the creature position.
		}
		else {
			yPos = 500;
			xPos = this.iGetPlayerX.getPlayerX(playerIndex);
		}

		const imageIndex: number = this.knownCards.addImage(card.CardName);
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
}
