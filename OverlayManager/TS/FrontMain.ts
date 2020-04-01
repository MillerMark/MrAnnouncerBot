var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myFrontCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

//activeGame = new DroneGame(myContext);
activeFrontGame = new DragonFrontGame(myContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeFrontGame.run();