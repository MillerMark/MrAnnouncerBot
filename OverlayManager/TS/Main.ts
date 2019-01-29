var activeBackGame: Game;
var activeFrontGame: Game;

var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

//activeGame = new DroneGame(myContext);
activeBackGame = new DragonGame(myContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeBackGame.run();

requestAnimationFrame(gameLoop);

// Consider moving to Game so each game has its own loop, it's encapsulated and multiple games can
// run concurrently if needed.
function gameLoop(nowMs: DOMHighResTimeStamp) {
  requestAnimationFrame(gameLoop);

  if (activeBackGame) {
    activeBackGame.nowMs = nowMs;
    activeBackGame.update(nowMs);
  }
}
