class CardManager {
	knownCards: Sprites;
	constructor() {
	}

	loadResources() {
		Folders.assets = 'GameDev/Assets/DragonH/';
		// TODO: Reorder parameters for Sprites constructor so fps is optional after AnimationStyle.
		this.knownCards = new Sprites('Cards', 0, fps30, AnimationStyle.Static);
		this.knownCards.addImage('Secret Card');
	}
}