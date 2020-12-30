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
		const imageIndex: number = this.knownCards.addImage(card.CardName);
		const sprite: SpriteProxy = this.knownCards.add(960, 540, imageIndex);
		//sprite.autoRotationDegeesPerSecond = 10;
		const entryTime = 1500;
		sprite.ease(performance.now(), Random.between(-500, 1920), 1080, 960 - this.knownCards.originX, 540 - this.knownCards.originY, entryTime);
		const degreesToSpin = 90;
		const initialScale = 0.5;
		sprite.setInitialScale(initialScale);
		//sprite.autoScaleFactorPerSecond = (1 / initialScale) / (entryTime / 1000);
		sprite.autoScaleFactorPerSecond = 1.8;
		sprite.autoScaleMaxScale = 1;
		sprite.easeSpin(performance.now(), -degreesToSpin, Random.between(-4, 4), entryTime);
		sprite.expirationDate = performance.now() + 4000;
		sprite.fadeInTime = 600;
		sprite.fadeOutTime = 1000;
	}
}
