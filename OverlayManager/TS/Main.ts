var activeGame: Game;

var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

//activeGame = new DroneGame();
activeGame = new DragonGame(myContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeGame.initialize();
activeGame.loadResources();
activeGame.start();

// Change the following to //* to use setInterval
/*
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

// TODO: Consider having a startGame function in Game (setInterval call).
setInterval(updateScreen, 10);
/*/
requestAnimationFrame(gameLoop);

function executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string) {
    if (activeGame) {
        activeGame.executeCommand(command, params, userId, userName, displayName, color, activeGame.nowMs);
    }
}

// Putting this here because this is where the setInterval call is.
// Recommend each game has its own loop so it's encapsulated and multiple games can
// run concurrently if needed.
function gameLoop(now: DOMHighResTimeStamp) {
    requestAnimationFrame(gameLoop);

    if (activeGame) {
        activeGame.nowMs = now;
        activeGame.updateScreen(myContext, now);
    }
}
//*/
