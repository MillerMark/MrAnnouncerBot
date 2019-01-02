var activeGame: Game;

function updateScreen() {
  if (activeGame)
	  activeGame.updateScreen(myContext, performance.now());
}

function executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string) {
  if (activeGame) {
    var now = performance.now();
    activeGame.executeCommand(command, params, userId, userName, displayName, color, now);
  }
}

var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

//activeGame = new DroneGame();
activeGame = new DragonGame(myContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeGame.initialize();
activeGame.loadResources();
activeGame.start();

// TODO: Consider having a startGame function in Game (setInterval call).
//setInterval(updateScreen, 10);
requestAnimationFrame(loop);

function loop(now: number) {
    requestAnimationFrame(loop);

    if (activeGame)
        activeGame.updateScreen(myContext, now);
}
