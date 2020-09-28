function browserIsOBS(): boolean {
	// @ts-ignore - obsstudio
	return window !== undefined && window.obsstudio !== undefined && window.obsstudio.pluginVersion !== undefined;
}


class FrameRateChangeData {
	OverlayName: string;
	FrameRate: number;
	ShowFpsWindow: boolean;
	AllowColorShifting: boolean;
	MaxFiltersOnRoll: number;
	MaxFiltersPerWindup: number;
	MaxFiltersOnDieCleanup: number;
	AllowCanvasFilterCaching: boolean;
	BackgroundCanvasPainting: boolean;
	constructor() {

	}
}


let loadCopyrightedContent = true;  //! This is definitely NOT a constant.

const screenWidth = 1920;
const screenHeight = 1080;

class Game {
  lastFrameUpdate: number;
	//frontGameLoop(nowMs: DOMHighResTimeStamp) {
	//	this.drawGame(nowMs);
	//	requestAnimationFrame(this.frontGameLoop);
	//}

	drawGame(nowMs: DOMHighResTimeStamp) {
	}

	fpsInterval: number;
	startTime: number;
	lastDrawTime: number;

	startAnimating(fps: number): void {
		this.changeFramerate(fps);
		requestAnimationFrame(this.animateFps.bind(this));
	}

	fps: number;

	changeFramerate(fps: number): void {
		if (fps === -1)
			return;
		if (this.fps === fps)
			return;
		this.fps = fps;
		this.fpsInterval = 1000 / fps;
		this.secondsPerFrame = 1 / fps;
		this.lastDrawTime = Date.now();
		this.startTime = this.lastDrawTime;
	}

	animateFps(nowMs: DOMHighResTimeStamp) {
		try {
			let now: number = Date.now();
			let elapsed: number = now - this.lastDrawTime;

			if (elapsed > this.fpsInterval) {
				this.lastDrawTime = now - (elapsed % this.fpsInterval);
				this.drawGame(nowMs);
			}
		}
		finally {
			requestAnimationFrame(this.animateFps.bind(this));
		}
	}

	protected secondsPerFrame: number;
	protected world: World;
	protected gravity: GravityForce;
	protected planetName: string;

	protected now: number;
	protected priorNow: number;
	protected secondsToUpdate: number = 0;

	public nowMs: number = 0;

	constructor(protected readonly context: CanvasRenderingContext2D) {
		this.world = new World(this.context);
	}

	run(): void {
		this.initialize();
		this.loadResources();
		this.start();
		this.startAnimating(30);
	}

	update(timestamp: number) {
		// All game objects deal with seconds so do the conversion from milliseconds now pass it down.
		// That way there's no guesswork as to what a time represents.
		let now = timestamp / 1000;

		const elapsed = now - this.priorNow || now;
		this.secondsToUpdate += elapsed;
		this.priorNow = now;

		// We got called back too early (and not enough prior time to offset)... wait until the next call.
		if (this.secondsToUpdate < this.secondsPerFrame)
			return;

		// Adjust to the start of the updates.
		this.now = now - this.secondsToUpdate;

		// Update in secondsPerFrame increments. This reduces floating point errors.
		while (this.secondsToUpdate >= this.secondsPerFrame) {
			this.world.update(this.now, this.secondsPerFrame);

			this.secondsToUpdate -= this.secondsPerFrame;
			this.now += this.secondsPerFrame;
		}

		this.now = now;

		this.world.ctx.clearRect(0, 0, screenWidth, screenHeight);
		this.updateScreenBeforeWorldRender(this.world.ctx, this.nowMs);
		this.world.render(this.now, this.secondsPerFrame);
		let startUpdate: number = performance.now();
		this.updateScreen(this.world.ctx, this.nowMs);
		let endUpdate: number = performance.now();
		this.calculateFramerate(startUpdate, endUpdate);
	}

  updateScreenBeforeWorldRender(ctx: CanvasRenderingContext2D, nowMs: number) {
  }

	static readonly fpsHistoryCount: number = 150;
	timeBetweenFramesQueue = [];
	drawTimeForEachFrameQueue = [];

	calculateFramerate(startUpdate: number, endUpdate: number): any {
		if (this.lastFrameUpdate) {
			let timeBetweenFrames: number = endUpdate - this.lastFrameUpdate;
			this.timeBetweenFramesQueue.push(timeBetweenFrames);
			if (this.timeBetweenFramesQueue.length > Game.fpsHistoryCount)
				this.timeBetweenFramesQueue.shift();
		}

		let drawTimeForThisFrame: number = endUpdate - startUpdate;
		this.drawTimeForEachFrameQueue.push(drawTimeForThisFrame);
		if (this.drawTimeForEachFrameQueue.length > Game.fpsHistoryCount)
			this.drawTimeForEachFrameQueue.shift();

		this.lastFrameUpdate = endUpdate;
	}

	updateScreen(context: CanvasRenderingContext2D, nowMs: number): any {

	}

	start() {
	}

	initialize(): void {
		Part.loadSprites = loadCopyrightedContent;
	}

	executeCommand(command: string, params: string, userInfo: UserInfo, now: number) {
		if (command === "TestCommand") {
			return this.test(params, userInfo, now);
		}
		return false;
	}

	test(testCommand: string, userInfo: UserInfo, now: number): boolean {
		return false;
	}

	removeAllGameElements(now: number): void {
	}

	loadResources(): void {

	}

	protected updateGravity() {
		if (!gravityGames || !gravityGames.activePlanet)
			return;

		if (this.gravity) {
			if (this.planetName === gravityGames.activePlanet.name)
				return;

			this.world.removeForce(this.gravity);
		}

		this.gravity = new GravityForce(gravityGames.activePlanet.gravity);
		this.planetName = gravityGames.activePlanet.name;
		this.world.addForce(this.gravity);
	}
}
