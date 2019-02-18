var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myFrontCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

//activeGame = new DroneGame(myContext);
activeFrontGame = new DragonFrontGame(myContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeFrontGame.run();

requestAnimationFrame(frontGameLoop);

// Consider moving to Game so each game has its own loop, it's encapsulated and multiple games can
// run concurrently if needed.
function frontGameLoop(nowMs: DOMHighResTimeStamp) {
  requestAnimationFrame(frontGameLoop);
  if (activeFrontGame) {
    activeFrontGame.nowMs = nowMs;
    activeFrontGame.update(nowMs);
  }
}
