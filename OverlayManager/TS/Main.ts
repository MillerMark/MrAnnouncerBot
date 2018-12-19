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

activeGame = new DroneGame();

var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

activeGame.initialize();
activeGame.loadResources();
activeGame.start();

// TODO: Consider having a startGame function in Game (setInterval call).
setInterval(updateScreen, 10);


