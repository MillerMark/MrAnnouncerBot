const markFliesCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("markFliesCanvas");
const markFliesContext: CanvasRenderingContext2D = markFliesCanvas.getContext("2d");

activeMarkFliesGame = new MarkFliesGame(markFliesContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeMarkFliesGame.run();

requestAnimationFrame(markFliesGameLoop);

// Consider moving to Game so each game has its own loop, it's encapsulated and multiple games can
// run concurrently if needed.
function markFliesGameLoop(nowMs: DOMHighResTimeStamp) {
  requestAnimationFrame(markFliesGameLoop);
  if (activeMarkFliesGame) {
    activeMarkFliesGame.nowMs = nowMs;
    activeMarkFliesGame.update(nowMs);
  }
}