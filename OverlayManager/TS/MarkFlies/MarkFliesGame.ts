class MarkFliesGame extends GamePlusQuiz {
	donutSpaceship: Sprites;
	spaceshipFlame: Sprites;
	animations: Animations = new Animations();

	constructor(context: CanvasRenderingContext2D) {
		super(context);
	}

	updateScreen(context: CanvasRenderingContext2D, nowMs: number) {
		super.updateScreen(context, nowMs);
		
		this.donutSpaceship.updatePositions(nowMs);
		this.spaceshipFlame.updatePositions(nowMs);

		this.spaceshipFlame.draw(markFliesContext, nowMs);
		this.donutSpaceship.draw(markFliesContext, nowMs);
		
		this.animations.removeExpiredAnimations(nowMs);
		this.animations.render(markFliesContext, nowMs);
	}

	removeAllGameElements(now: number): void {
		super.removeAllGameElements(now);
		//this.allWalls.destroyAllBy(4000);
	}

	initialize() {
		super.initialize();
		Folders.assets = 'GameDev/Assets/MarkFlies/';
	}

	start() {
		super.start();
	}

	loadResources(): void {
		super.loadResources();

		this.donutSpaceship = new Sprites(`Spaceships/Donut/Donut`, 301, fps30, AnimationStyle.Loop, true);
		this.donutSpaceship.originX = 320;
		this.donutSpaceship.originY = 150;

		this.spaceshipFlame = new Sprites(`Spaceships/Donut/Flames`, 301, fps30, AnimationStyle.Loop, true);
		this.spaceshipFlame.originX = 332;
		this.spaceshipFlame.originY = -79;
	}

	executeCommand(command: string, params: string, userInfo: UserInfo, now: number): boolean {
		if (super.executeCommand(command, params, userInfo, now))
			return true;

		if (command === "Swat") {
			//this.destroyAllDronesOverMark();
			return true;
		}
		return false;
	}

	test(testCommand: string, userInfo: UserInfo, now: number): boolean {
		if (super.test(testCommand, userInfo, now))
			return true;

		if (testCommand === 'donut') {
			const spaceship: SpriteProxy = this.donutSpaceship.addShifted(600, 340, 0, 300);
			spaceship.destroyAllInExactly(30000);
			spaceship.fadeInTime = 500;
			spaceship.fadeOutTime = 500;
			const flames: SpriteProxy = this.spaceshipFlame.addShifted(600, 340, 0, 220);
			flames.destroyAllInExactly(30000);
			flames.fadeInTime = 500;
			flames.fadeOutTime = 500;

			return true;
		}

		return false;
	}

	destroying(): void {

	}
}