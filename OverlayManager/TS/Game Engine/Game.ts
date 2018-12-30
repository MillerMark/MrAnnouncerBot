let loadCopyrightedContent: boolean = false;

const screenWidth: number = 1920;
const screenHeight: number = 1080;

class Game {
    protected framesPerSecond: number = 100; // Defaulting to 100 because main is hardcoded to 10ms;
    protected secondsPerFrame: number;
    protected world: World;
    protected gravity: GravityForce;
    protected planetName: string;

    protected now: number;
    protected priorNow: number;
    protected secondsToUpdate: number = 0;

  constructor(protected readonly context: CanvasRenderingContext2D) {

  }

  // Suggestion: This should be using requestAnimationFrame and should be registered from start().
  // Each game should have its own timer.  Eventually there are going to be some adverse graphic effects
  // with setInterval. At the very minimum, due to the way setInteral queues callbacks, it should be
  // changed to setTimeout.
  // The idea here is to phase out the updateScreen method. Once all animated objects descend from
  // WorldObject, the maintenance will be much simpler and descendants should never even need to override
  // this method.
  update(timestamp: number) {
      // All game objects deal with seconds so do the conversion from milliseconds now pass it down.
      // That way there's no guesswork as to what a time represents.
      let now = timestamp / 1000;

      const elapsed = now - this.priorNow || now;
      this.secondsToUpdate += elapsed;
      this.priorNow = now;

      // We got called back too early (and not enough prior time to offset)... wait until the next call.
      if (this.secondsToUpdate < this.secondsPerFrame) return;

      // Adjust to the start of the updates.
      this.now = now - this.secondsToUpdate;

      // Update in secondsPerFrame increments. This reduces floating point errors.
      while (this.secondsToUpdate >= this.secondsPerFrame) {
          this.world.update(this.now, this.secondsPerFrame);

          this.secondsToUpdate -= this.secondsPerFrame;
          this.now += this.secondsPerFrame;
      }

      this.now = now;
      this.world.render(this.now, this.secondsPerFrame);
  }

  updateScreen(context: CanvasRenderingContext2D, now: number): void {
    context.clearRect(0, 0, screenWidth, screenHeight);
    this.update(now);
    this.updateBackground(context, now);
    this.updateForeground(context, now);
  }

  updateForeground(context: CanvasRenderingContext2D, now: number): any {

  }

  updateBackground(context: CanvasRenderingContext2D, now: number): any {

  }

  start() {
      this.secondsPerFrame = 1 / this.framesPerSecond;
      this.world = new World(this.context);
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