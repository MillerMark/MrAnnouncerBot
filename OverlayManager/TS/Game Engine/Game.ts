﻿let loadCopyrightedContent: boolean = true;

const screenWidth: number = 1920;
const screenHeight: number = 1080;

class Game {
  protected framesPerSecond: number = 60;
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
    this.world.render(this.now, this.secondsPerFrame);
    this.updateScreen(this.world.ctx, this.nowMs);
  }

  updateScreen(context: CanvasRenderingContext2D, now: number): any {

  }

  start() {
    this.secondsPerFrame = 1 / this.framesPerSecond;
  }

  initialize(): void {
    Part.loadSprites = loadCopyrightedContent;
  }

  executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string, now: number) {
    if (command === "TestCommand") {
      return this.test(params, userId, userName, displayName, color, now);
    }
    return false;
  }

  test(testCommand: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    return false;
  }

  removeAllGameElements(now: number): void {
  }

  loadResources(): void {

  }

  protected updateGravity() {
    if (!this.gravity) {
      this.gravity = new GravityForce(gravityGames.activePlanet.gravity);
      this.planetName = gravityGames.activePlanet.name;
      this.world.addForce(this.gravity);
      return;
    }

    if (this.planetName !== gravityGames.activePlanet.name) {
      this.gravity.gravityConstant = gravityGames.activePlanet.gravity;
      this.planetName = gravityGames.activePlanet.name;
    }
  }
}
