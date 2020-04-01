var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myCanvas");
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");

activeBackGame = new DragonBackGame(myContext);

// Suggestion: This is too much outside knowledge of inner workings... Should just call start().
activeBackGame.run();
