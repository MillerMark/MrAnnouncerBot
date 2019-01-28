class DragonFrontGame extends GamePlusQuiz {
  shouldDrawCenterCrossHairs: boolean = false;

  constructor(context: CanvasRenderingContext2D) {
    super(context);
  }

  update(timestamp: number) {
    this.updateGravity();
    super.update(timestamp);
  }

  updateScreen(context: CanvasRenderingContext2D, now: number) {
    super.updateScreen(context, now);

    if (this.shouldDrawCenterCrossHairs)
      drawCrossHairs(myContext, screenCenterX, screenCenterY);
  }

  removeAllGameElements(now: number): void {
    super.removeAllGameElements(now);
  }

  initialize() {
    super.initialize();

    //Folders.assets = 'GameDev/Assets/DroneGame/';
  }

  start() {
    super.start();
    this.updateGravity();
  }

  loadResources(): void {
    super.loadResources();
  }

  executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    console.log('executeCommand');
    if (super.executeCommand(command, params, userId, userName, displayName, color, now))
      return true;
    if (command === "Cross2") {
      this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
    }
  }

  test(testCommand: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    console.log('test');
    if (super.test(testCommand, userId, userName, displayName, color, now))
      return true;

    if (testCommand === "Cross2") {
      console.log('draw Cross Hairs');
      this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
    }

    return false;
  }
} 