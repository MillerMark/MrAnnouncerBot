var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myFrontCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

//activeGame = new DroneGame(myContext);
activeFrontGame = new DragonFrontGame(myContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeFrontGame.run();

activeFrontGame.startAnimating(30);

function frontGameLoop(nowMs: DOMHighResTimeStamp) {
	drawFrontGame(nowMs);
	requestAnimationFrame(frontGameLoop);
}

function drawFrontGame(nowMs: DOMHighResTimeStamp) {
	if (activeFrontGame) {
		activeFrontGame.nowMs = nowMs;
		activeFrontGame.update(nowMs);
	}
}

var fpsInterval: number;
var startTime: number;
var lastDrawTime: number;

function startAnimating(fps: number) {
	console.log('fps: ' + fps);
	fpsInterval = 1000 / fps;
	lastDrawTime = Date.now();
	startTime = lastDrawTime;
	requestAnimationFrame(animateFps);
}

function animateFps(nowMs: DOMHighResTimeStamp) {
	let now: number = Date.now();
	let elapsed: number = now - lastDrawTime;

	if (elapsed > fpsInterval) {
		lastDrawTime = now - (elapsed % fpsInterval);
		drawFrontGame(nowMs);
	}
	requestAnimationFrame(animateFps);
}