class MapGame extends Game {
	constructor(context: CanvasRenderingContext2D) {
		super(context);
	}

	update(timestamp: number) {
		super.update(timestamp);
	}

	updateScreen(context: CanvasRenderingContext2D, nowMs: number) {
		super.updateScreen(context, nowMs);
	}

	removeAllGameElements(now: number): void {
		super.removeAllGameElements(now);
	}

	initialize() {
		super.initialize();
		Folders.assets = 'GameDev/Assets/DragonH/';
		//gravityGames = new GravityGames();
		//Folders.assets = 'GameDev/Assets/DroneGame/';  // So GravityGames can load planet Earth?
	}

	start() {
		super.start();
	}

	loadResources(): void {
		super.loadResources();
		Folders.assets = 'GameDev/Assets/DragonH/';

	}

	executeCommand(command: string, params: string, userInfo: UserInfo, now: number): boolean {
		if (super.executeCommand(command, params, userInfo, now))
			return true;

		return false;
	}

	test(testCommand: string, userInfo: UserInfo, now: number): boolean {
		if (super.test(testCommand, userInfo, now))
			return true;

		return false;
	}

}
