var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myMapCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

activeMap = new MapGame(myContext);
activeMap.run();

requestAnimationFrame(mapLoop);

// Consider moving to Game so each game has its own loop, it's encapsulated and multiple games can
// run concurrently if needed.
function mapLoop(nowMs: DOMHighResTimeStamp) {
  requestAnimationFrame(mapLoop);
	if (activeMap) {
    activeMap.nowMs = nowMs;
    activeMap.update(nowMs);
  }
}
