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
	soundManager: ISoundManager;

	constructor() {
	}

	initialize(iGetPlayerX: IGetPlayerX, soundManager: ISoundManager) {
		this.iGetPlayerX = iGetPlayerX;
		this.soundManager = soundManager;
	}

	loadResources() {
		Folders.assets = 'GameDev/Assets/DragonH/';
		// TODO: Reorder parameters for Sprites constructor so fps is optional after AnimationStyle.
		this.knownCards = new Sprites('Cards', 0, fps30, AnimationStyle.Static);
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
		const imageIndex: number = this.knownCards.addImage(card.CardName);
		const sprite: SpriteProxy = this.knownCards.add(960, 540, imageIndex);
		sprite.expirationDate = performance.now() + 4000;
		sprite.fadeInTime = 600;
		sprite.fadeOutTime = 1000;
	}
}
