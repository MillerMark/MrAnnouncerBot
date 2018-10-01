function updateScreen() {
  const screenWidth: number = 1920;
  const screenHeight: number = 1080;

  myContext.clearRect(0, 0, screenWidth, screenHeight);
  var now = performance.now();
  myRocket.updatePosition(now);
  myRocket.bounce(0, 0, screenWidth, screenHeight, now);

  var coinsCollected: number = coins.collect(myRocket.x, myRocket.y, 310, 70);
  if (coinsCollected > 0) {
    new Audio(Folders.assets + 'Sound Effects/CollectCoin.wav').play();
    gravityGames.activeGame.score.value += coinsCollected;
  }

  pinkSeeds.bounce(0, 0, screenWidth, screenHeight, now);
  blueSeeds.bounce(0, 0, screenWidth, screenHeight, now);
  yellowSeeds.bounce(0, 0, screenWidth, screenHeight, now);
  purpleSeeds.bounce(0, 0, screenWidth, screenHeight, now);
  beesYellow.bounce(0, 0, screenWidth, screenHeight, now);
  dronesRed.bounce(0, 0, screenWidth, screenHeight, now);
  //greenSeeds.bounce(0, 0, screenWidth, screenHeight, now);
  redMeteors.bounce(0, 0, screenWidth, screenHeight, now);
  blueMeteors.bounce(0, 0, screenWidth, screenHeight, now);
  purpleMeteors.bounce(0, 0, screenWidth, screenHeight, now);

  //backgroundBanner.draw(myContext, 0, 0);

  if (!myRocket.isDocked)
    gravityGames.draw(myContext);

  coins.draw(myContext, now);
  //grass1.draw(myContext, now);
  //grass2.draw(myContext, now);
  //grass3.draw(myContext, now);
  //grass4.draw(myContext, now);

  pinkSeeds.draw(myContext, now);
  blueSeeds.draw(myContext, now);
  yellowSeeds.draw(myContext, now);
  purpleSeeds.draw(myContext, now);
  //greenSeeds.draw(myContext, now);
  beesYellow.draw(myContext, now);
  dronesRed.draw(myContext, now);
  redMeteors.draw(myContext, now);
  blueMeteors.draw(myContext, now);
  purpleMeteors.draw(myContext, now);
  myRocket.draw(myContext, now);
  redExplosions.draw(myContext, now);
  blueExplosions.draw(myContext, now);
  purpleExplosions.draw(myContext, now);
  purpleFlowers.draw(myContext, now);
  blueFlowers.draw(myContext, now);
  redFlowers.draw(myContext, now);
  yellowFlowers1.draw(myContext, now);
  yellowFlowers2.draw(myContext, now);
  yellowFlowers3.draw(myContext, now);
  if (quiz)
    quiz.draw(myContext);
  //explosion.draw(myContext, 0, 0);
}

function handleKeyDown(evt) {
  const Key_B = 66;
  const Key_C = 67;
  const Key_D = 68;
  const Key_G = 71;
  const Key_M = 77;
  const Key_O = 79;
  const Key_P = 80;
  const Key_S = 83;
  const Key_Up = 38;
  const Key_Right = 39;
  const Key_Left = 37;
  const Key_Down = 40;

  var now = performance.now();
  evt = evt || window.event;
  if (evt.keyCode == 13) {
    if (!started || myRocket.isDocked) {
      started = true;
      myRocket.launch(now);
    }
    else if (myRocket.enginesRetracted)
      myRocket.extendEngines(now);
    else
      myRocket.retractEngines(now);
    return false;
  }
  else if (evt.keyCode == Key_Up) {
    myRocket.fireMainThrusters(now);
    return false;
  }
  else if (evt.keyCode == Key_Down) {
    myRocket.killHoverThrusters(now);
    return false;
  }
  else if (evt.keyCode == Key_Right) {
    myRocket.fireLeftThruster(now);
    return false;
  }
  else if (evt.keyCode == Key_Left) {
    myRocket.fireRightThruster(now);
    return false;
  }
  else if (evt.keyCode == Key_D) {
    myRocket.dock(now);
  }
  else if (evt.keyCode == Key_G) {
    myRocket.dropSeed(now, 'grass');
  }
  else if (evt.keyCode == Key_M) {
    myRocket.dropMeteor(now);
  }
  else if (evt.keyCode == Key_O) {
    myRocket.releaseDrone(now, '', '', '', '');
  }
  else if (evt.keyCode == Key_S) {
    myRocket.dropSeed(now);
  }
  else if (evt.keyCode == Key_P) {
    gravityGames.cyclePlanet();
  }
  else if (evt.keyCode == Key_B) {
    myRocket.releaseBee(now, '', '', '', '');
  }
  else if (evt.keyCode == Key_C) {
    if (myRocket.chuteDeployed)
      myRocket.retractChutes(now);
    else
      myRocket.deployChute(now);
    return false;
  }
}

function buildCenterRect(sprites) {
  sprites.fillRect(100, 100, myCanvas.clientWidth - 100, myCanvas.clientHeight - 100, 12);
}

function buildInnerRect(sprites) {
  sprites.fillRect(400, 200, myCanvas.clientWidth - 400, myCanvas.clientHeight - 200, 12);
}

function fillScreenRect(sprites) {
  sprites.fillRect(0, 0, myCanvas.clientWidth, myCanvas.clientHeight, 12);
}

function fillScreenRectMinusTwitchBanner(sprites) {
  sprites.fillRect(0, 0, myCanvas.clientWidth, myCanvas.clientHeight, 12);
  sprites.collect(100, 260, 1650, 240);
}

function outlineScreenRect(sprites) {
  sprites.outlineRect(0, 0, myCanvas.clientWidth, myCanvas.clientHeight, 11, rectangleDrawingSegment.bottom, 4);
}

function outlineMargin(sprites, margin) {
  sprites.outlineRect(2 * margin, margin, myCanvas.clientWidth - 2 * margin, myCanvas.clientHeight - margin, 12);
}

const coinMargin = 12;
function outlineGameSurface(sprites: Sprites) {
  sprites.layout(
    '*************************' + '\n' +
    '*                       *' + '\n' +
    '*                       *' + '\n' +
    '*                       *' + '\n' +
    '*                       *' + '\n' +
    '*                       *' + '\n' +
    '*                       *' + '\n' +
    '*                       *' + '\n' +
    '*                       *' + '\n' +
    '*                       *' + '\n' + // 10
    '*                       *' + '\n' +
    '*                       *' + '\n' +
    '*                       *' + '\n' +
    '*************************', coinMargin);
}

function fillChatRoom(sprites: Sprites) {
  sprites.layout(
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                  *******' + '\n' +
    '                  *******' + '\n' +
    '                  *******' + '\n' + // 10
    '                  *******' + '\n' +
    '                  *******' + '\n' +
    '                  *******' + '\n' +
    '                  *******', coinMargin);
}

function outlineChatRoom(sprites: Sprites) {
  sprites.layout(
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                         ' + '\n' +
    '                  *******' + '\n' +
    '                  *     *' + '\n' +
    '                  *     *' + '\n' + // 10
    '                  *     *' + '\n' +
    '                  *     *' + '\n' +
    '                  *     *' + '\n' +
    '                  *******', coinMargin);
}

function outlineCodeEditor(sprites: Sprites) {
  sprites.layout(
    '                         ' + '\n' +
    '******************       ' + '\n' +
    '*                *       ' + '\n' +
    '*                *       ' + '\n' +
    '*                *       ' + '\n' +
    '*                *       ' + '\n' +
    '*                *       ' + '\n' +
    '*                *       ' + '\n' +
    '*                *       ' + '\n' +
    '*                *       ' + '\n' + // 10
    '*                *       ' + '\n' +
    '*                *       ' + '\n' +
    '*                *       ' + '\n' +
    '***********     **       ', coinMargin);
}

function allButMark(sprites: Sprites) {
  sprites.layout(
    '                         ' + '\n' +
    '******************       ' + '\n' +
    '******************       ' + '\n' +
    '******************       ' + '\n' +
    '******************       ' + '\n' +
    '******************       ' + '\n' +
    '******************       ' + '\n' +
    '******************       ' + '\n' +
    '******************       ' + '\n' +
    '************* ****       ' + '\n' + // 10
    '************   ***       ' + '\n' +
    '************   ***       ' + '\n' +
    '************   ***       ' + '\n' +
    '***********     **       ', coinMargin);
}


function outlineBigRect(sprites) {
  outlineMargin(sprites, 100);
}

function outlineMediumRect(sprites) {
  outlineMargin(sprites, 200);
}

function outlineSmallRect(sprites) {
  outlineMargin(sprites, 300);
}

function plantSeed(spriteArray, x, y) {
  spriteArray.sprites.push(new SpriteProxy(0, x - spriteArray.spriteWidth / 2, 1080 - spriteArray.spriteHeight + y));
}

function plantSeeds(seeds, x) {
  if (seeds === pinkSeeds)
    plantSeed(redFlowers, x + 50, 0);
  else if (seeds === blueSeeds)
    plantSeed(blueFlowers, x + 50, 5);
  else if (seeds === purpleSeeds)
    plantSeed(purpleFlowers, x + 50, 5);
  //else if (seeds === greenSeeds) {
  //  let randomGrass: number = Math.random() * 10;
  //  if (randomGrass < 1)
  //    plantSeed(grass1, x + 50, 0);
  //  else if (randomGrass < 2)
  //    plantSeed(grass2, x + 50, 0);
  //  else if (randomGrass < 3)
  //    plantSeed(grass3, x + 50, 0);
  //  else
  //    plantSeed(grass4, x + 50, 0);
  //}
  else if (seeds === yellowSeeds) {
    let randomYellow: number = Math.random() * 3;
    if (randomYellow < 1)
      plantSeed(yellowFlowers1, x + 50, 5);
    else if (randomYellow < 2)
      plantSeed(yellowFlowers2, x + 50, 0);
    else
      plantSeed(yellowFlowers3, x + 50, 0);
  }
  new Audio(Folders.assets + 'Sound Effects/MeteorHit.wav').play();
}

function addExplosion(meteors, x) {
  if (meteors === redMeteors)
    redExplosions.sprites.push(new SpriteProxy(0, x - redExplosions.spriteWidth / 2 + 50, 0));
  if (meteors === blueMeteors)
    blueExplosions.sprites.push(new SpriteProxy(0, x - blueExplosions.spriteWidth / 2 + 50, 0));
  if (meteors === purpleMeteors)
    purpleExplosions.sprites.push(new SpriteProxy(0, x - purpleExplosions.spriteWidth / 2 + 50, 0));
  new Audio(Folders.assets + 'Sound Effects/MeteorHit.wav').play();
}

var connection;

function connectToSignalR(signalR) {
  connection = new signalR.HubConnectionBuilder().withUrl("/CodeRushedHub").configureLogging(signalR.LogLevel.Information).build();
  window.onload = function () {
    connection.start().catch(err => console.error(err.toString()));
    connection.on("ExecuteCommand", executeCommand);
  };
}

function chat(message: string) {
  connection.invoke("Chat", message);
}

function whisper(message: string) {
  connection.invoke("Chat", message);
}

function executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string) {
  var now = performance.now();
  if (command === "Launch") {
    if (!started || myRocket.isDocked) {
      started = true;
      myRocket.launch(now);
      gravityGames.newGame();
      chat('Launching...');
    }
  }
  else if (command === "Dock") {
    if (started && !myRocket.isDocked)
      chat('docking...');
    myRocket.dock(now);
  }
  else if (command === "ChangePlanet") {
    gravityGames.selectPlanet(params);
  }
  else if (command === "Left") {
    myRocket.fireRightThruster(now, params);
  }
  else if (command === "Right") {
    myRocket.fireLeftThruster(now, params);
  }
  else if (command === "Up") {
    myRocket.fireMainThrusters(now, params);
  }
  else if (command === "Down") {
    myRocket.killHoverThrusters(now, params);
  }
  else if (command === "Drop") {
    myRocket.dropMeteor(now);
  }
  else if (command === "Chutes") {
    if (myRocket.chuteDeployed)
      myRocket.retractChutes(now);
    else
      myRocket.deployChute(now);
  }
  else if (command === "Retract") {
    myRocket.retractEngines(now);
  }
  else if (command === "Extend") {
    myRocket.extendEngines(now);
  }
  else if (command === "Seed") {
    myRocket.dropSeed(now, params);
  }
  else if (command === "Bee") {
    myRocket.releaseBee(now, params, userId, displayName, color);
  }
  else if (command === "Drone") {
    myRocket.releaseDrone(now, params, userId, displayName, color);
  }
  else if (command === "MoveRelative") {
    moveRelative(now, params, userId);
  }
  else if (command === "MoveAbsolute") {
    moveAbsolute(now, params, userId);
  }
  else if (command === "StartQuiz") {
    startQuiz(now, params, userId, userName);
  }
  else if (command === "ShowLastQuizResults") {
    showLastQuizResults(now, params);
  }
  else if (command === "AnswerQuiz") {
    answerQuiz(params, userId);
  }
  else if (command === "SilentAnswerQuiz") {
    silentAnswerQuiz(params, userId, userName);
  }
  else if (command === "ClearQuiz") {
    clearQuiz(now, params, userId, userName);
  }
  // TODO: Support !vote x
}

function clearQuiz(now: number, params: string, userId: string, userName: string) {
  if (userName != "coderushed" && userName != "rorybeckercoderush") {
    chat('Only Rory and Mark can clear a quiz.');
    return;
  }
  if (quiz)
    quiz = null;
}

function answerQuiz(choice: string, userId: string) {
  if (quiz)
    quiz.vote(userId, choice);
}

function silentAnswerQuiz(choice: string, userId: string, userName: string) {
  if (!quiz)
    return;
  quiz.vote(userId, choice);
}

function showLastQuizResults(now: number, params: string) {

}

// params are expected to be in the form of "!quiz What would you rather be?, 1. Bee, 2. Drone"
function startQuiz(now: number, cmd: string, userId: string, userName: string) {
  if (userName != "coderushed" && userName != "rorybeckercoderush") {
    chat('Only Rory and Mark can start a quiz.');
    return;
  }
    
  let lines: Array<string> = cmd.split(',');
  let choices: Array<string> = lines.slice(1);
  if (choices.length < 2) {
    chat('Quizzes must have at least two comma-separated choices.');
    return;
  }
  new Quiz(lines[0], choices);
  
  chat('Starting Quiz');
  //myRocket.releaseBee(now, params, '', '', '');
}

function moveAbsolute(now: number, params: string, userId: string) {
}

function moveRelative(now: number, params: string, userId: string) {
}

var gravityGames = new GravityGames();

document.onkeydown = handleKeyDown;
var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myCanvas");
var coins = new Sprites("Spinning Coin/SpinningCoin", 165, 5, AnimationStyle.Loop, false, null, outlineChatRoom /* allButMark */ /* outlineCodeEditor  */ /* fillChatRoom */);

var pinkSeeds = new Sprites("Seeds/Pink/PinkSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
pinkSeeds.moves = true;

var blueSeeds = new Sprites("Seeds/Blue/BlueSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
blueSeeds.moves = true;

var yellowSeeds = new Sprites("Seeds/Yellow/YellowSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
yellowSeeds.moves = true;

var purpleSeeds = new Sprites("Seeds/Purple/PurpleSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
purpleSeeds.moves = true;

//var greenSeeds = new Sprites("Seeds/Yellow/YellowSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
//greenSeeds.moves = true;

var beesYellow = new Sprites("Bees/Yellow/BeeYellow", 18, 15, AnimationStyle.Loop);
beesYellow.segmentSize = 2;
beesYellow.removeOnHitFloor = false;
beesYellow.moves = true;

var dronesRed = new Sprites("Drones/Red/Drone", 30, 15, AnimationStyle.Loop);
dronesRed.segmentSize = 2;
dronesRed.removeOnHitFloor = false;
dronesRed.moves = true;

var redMeteors = new Sprites("Spinning Rock/Red/Meteor", 63, 50, AnimationStyle.Loop, false, addExplosion);
redMeteors.moves = true;

var blueMeteors = new Sprites("Spinning Rock/Blue/Meteor", 63, 50, AnimationStyle.Loop, false, addExplosion);
blueMeteors.moves = true;

var purpleMeteors = new Sprites("Spinning Rock/Purple/Meteor", 63, 50, AnimationStyle.Loop, false, addExplosion);
purpleMeteors.moves = true;

var redExplosions = new Sprites("Explosion/Red/Explosion", 179, 5, AnimationStyle.Sequential);
var blueExplosions = new Sprites("Explosion/Blue/Explosion", 179, 5, AnimationStyle.Sequential);
var purpleExplosions = new Sprites("Explosion/Purple/Explosion", 179, 5, AnimationStyle.Sequential);

var redFlowers = new Sprites("Flowers/Red/RedFlower", 293, 15, AnimationStyle.Loop, true);
redFlowers.returnFrameIndex = 128;

const flowerFrameRate: number = 20;
const grassFrameRate: number = 25;

var yellowFlowers1 = new Sprites("Flowers/YellowPetunias1/YellowPetunias", 270, flowerFrameRate, AnimationStyle.Loop, true);
yellowFlowers1.returnFrameIndex = 64;
var yellowFlowers2 = new Sprites("Flowers/YellowPetunias2/YellowPetunias", 270, flowerFrameRate, AnimationStyle.Loop, true);
yellowFlowers2.returnFrameIndex = 64;
var yellowFlowers3 = new Sprites("Flowers/YellowPetunias3/YellowPetunias", 253, flowerFrameRate, AnimationStyle.Loop, true);
yellowFlowers3.returnFrameIndex = 45;

var blueFlowers = new Sprites("Flowers/Blue/BlueFlower", 320, flowerFrameRate, AnimationStyle.Loop, true);
blueFlowers.returnFrameIndex = 151;

var purpleFlowers = new Sprites("Flowers/Purple/PurpleFlower", 320, flowerFrameRate, AnimationStyle.Loop, true);
purpleFlowers.returnFrameIndex = 151;

//var grass1 = new Sprites("Grass/1/Grass", 513, grassFrameRate, AnimationStyle.SequentialStop, true);
//var grass2 = new Sprites("Grass/2/Grass", 513, grassFrameRate, AnimationStyle.SequentialStop, true);
//var grass3 = new Sprites("Grass/3/Grass", 589, grassFrameRate, AnimationStyle.SequentialStop, true);
//var grass4 = new Sprites("Grass/4/Grass", 589, grassFrameRate, AnimationStyle.SequentialStop, true);

var backgroundBanner = new Part("CodeRushedBanner", 1, AnimationStyle.Static, 200, 300);
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");
var myRocket = new Rocket(0, 0);
var started = false;
myRocket.x = 0;
myRocket.y = 0;
setInterval(updateScreen, 10);
gravityGames.selectPlanet('Earth');