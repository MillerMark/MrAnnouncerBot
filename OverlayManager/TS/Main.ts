var activeGame: Game;

var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

//activeGame = new DroneGame(myContext);
activeGame = new DragonGame(myContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeGame.run();

requestAnimationFrame(gameLoop);

function executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string) {
  if (activeGame) {
    activeGame.executeCommand(command, params, userId, userName, displayName, color, activeGame.nowMs);
  }
}

// Consider moving to Game so each game has its own loop, it's encapsulated and multiple games can
// run concurrently if needed.
function gameLoop(nowMs: DOMHighResTimeStamp) {
  requestAnimationFrame(gameLoop);

  if (activeGame) {
    activeGame.nowMs = nowMs;
    activeGame.update(nowMs);
  }
}
