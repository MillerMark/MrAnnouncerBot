var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myFrontCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

activeDroneGame = new DroneGame(myContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeDroneGame.run();

requestAnimationFrame(droneGameLoop);

// Consider moving to Game so each game has its own loop, it's encapsulated and multiple games can
// run concurrently if needed.
function droneGameLoop(nowMs: DOMHighResTimeStamp) {
  requestAnimationFrame(droneGameLoop);
  if (activeDroneGame) {
    activeDroneGame.nowMs = nowMs;
    activeDroneGame.update(nowMs);
  }
}