let loadCopyrightedContent: boolean = true;

const screenWidth: number = 1920;
const screenHeight: number = 1080;

class Game {
  constructor() {

  }

  updateScreen(context: CanvasRenderingContext2D, now: number): void {
    context.clearRect(0, 0, screenWidth, screenHeight);
    this.updateBackground(context, now);
    this.updateForeground(context, now);
  }

  updateForeground(context: CanvasRenderingContext2D, now: number): any {

  }

  updateBackground(context: CanvasRenderingContext2D, now: number): any {

  }

  start() {

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
}
