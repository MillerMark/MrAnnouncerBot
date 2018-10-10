let loadCopyrightedContent: boolean = true;

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

  allSeeds.bounce(0, 0, screenWidth, screenHeight, now);

  beesYellow.bounce(0, 0, screenWidth, screenHeight, now);
  allDrones.bounce(0, 0, screenWidth, screenHeight, now);
  //greenSeeds.bounce(0, 0, screenWidth, screenHeight, now);
  redMeteors.bounce(0, 0, screenWidth, screenHeight, now);
  blueMeteors.bounce(0, 0, screenWidth, screenHeight, now);
  purpleMeteors.bounce(0, 0, screenWidth, screenHeight, now);

  //backgroundBanner.draw(myContext, 0, 0);

  if (!myRocket.isDocked)
    gravityGames.draw(myContext);

  allSplats.draw(myContext, now);

  coins.draw(myContext, now);
  //grass1.draw(myContext, now);
  //grass2.draw(myContext, now);
  //grass3.draw(myContext, now);
  //grass4.draw(myContext, now);

  allSeeds.draw(myContext, now);

  beesYellow.draw(myContext, now);
  allDrones.draw(myContext, now);
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

  droneExplosions.draw(myContext, now);
  if (quiz)
    quiz.draw(myContext);
  //explosion.draw(myContext, 0, 0);
}

var allSplats = new SpriteCollection();

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

function addMeteorExplosion(meteors: Sprites, x: number) {
  if (meteors === redMeteors)
    redExplosions.sprites.push(new SpriteProxy(0, x - redExplosions.spriteWidth / 2 + 50, 0));
  if (meteors === blueMeteors)
    blueExplosions.sprites.push(new SpriteProxy(0, x - blueExplosions.spriteWidth / 2 + 50, 0));
  if (meteors === purpleMeteors)
    purpleExplosions.sprites.push(new SpriteProxy(0, x - purpleExplosions.spriteWidth / 2 + 50, 0));
  new Audio(Folders.assets + 'Sound Effects/MeteorHit.wav').play();
}

function addDroneExplosion(drone: SpriteProxy, spriteWidth: number, spriteHeight: number): void {
  let x: number = drone.x + spriteWidth / 2;
  let y: number = drone.y + spriteHeight / 2;
  let thisDroneExplosion: Sprites = droneExplosions.allSprites[Math.round(Math.random() * droneExplosions.allSprites.length)];
  thisDroneExplosion.sprites.push(new SpriteProxy(0, x - thisDroneExplosion.spriteWidth / 2, y - thisDroneExplosion.spriteHeight / 2));
  new Audio(Folders.assets + 'Sound Effects/DroneGoBoom.wav').play();
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

function whisper(userName: string, message: string) {
  connection.invoke("Whisper", userName, message);
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
    clearQuiz(userName);
  }
  else if (command === "red" || command === "orange" || command === "amber" || command === "yellow" ||
    command === "green" || command === "cyan" || command === "blue" || command === "indigo"
    || command === "violet" || command === "magenta" || command === "black" || command === "white") {
    paint(userId, command, params);
  }
  else if (command === "ChangeDroneVelocity") {
    changeDroneVelocity(userId, params);
  }
  else if (command === "DroneUp") {
    droneUp(userId, params);
  }
  else if (command === "DroneDown") {
    droneDown(userId, params);
  }
  else if (command === "DroneLeft") {
    droneLeft(userId, params);
  }
  else if (command === "DroneRight") {
    droneRight(userId, params);
  }
  
  // TODO: Support !vote x
}

function droneRight(userId: string, params: string) {
  let userDrone: Drone = <Drone>allDrones.find(userId);
  if (!userDrone)
    return;

  userDrone.droneRight(params);
}

function droneLeft(userId: string, params: string) {
  let userDrone: Drone = <Drone>allDrones.find(userId);
  if (!userDrone)
    return;

  userDrone.droneLeft(params);
}

function droneUp(userId: string, params: string) {
  let userDrone: Drone = <Drone>allDrones.find(userId);
  if (!userDrone)
    return;

  userDrone.droneUp(params);
}

function droneDown(userId: string, params: string) {
  let userDrone: Drone = <Drone>allDrones.find(userId);
  if (!userDrone)
    return;

  userDrone.droneDown(params);
}
function changeDroneVelocity(userId: string, params: string) {
  let userDrone: Drone = <Drone>allDrones.find(userId);
  if (!userDrone)
    return;

  let parameters: string[] = params.split(',');
  if (parameters.length < 2)
    return;
  let now: number = performance.now();
  userDrone.changingDirection(now);
  userDrone.changeVelocity(+parameters[0], +parameters[1], now);
}

function paint(userId: string, command: string, params: string) {
  let userDrone: Drone = <Drone>allDrones.find(userId);
  if (!userDrone)
    return;

  userDrone.dropPaint(command, params);
}

function clearQuiz(userName: string) {
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
  if (quiz.getChoiceIndex(choice) < 0) {
    whisper(userName, 'We could not find that choice "' + choice + '".');
    return;
  }

  quiz.vote(userId, choice);
  whisper(userName, 'Your vote has been recorded. Nice job participating in democracy!');
}

function showLastQuizResults(now: number, params: string) {

}

// params are expected to be in the form of "!quiz What would you rather be?, 1. Bee, 2. Drone"
function startQuiz(now: number, cmd: string, userId: string, userName: string) {
  if (userName != "coderushed" && userName != "rorybeckercoderush") {
    chat('Only Rory and Mark can start a poll.');
    return;
  }

  let lines: Array<string> = cmd.split(',');
  let choices: Array<string> = lines.slice(1);
  if (choices.length < 2) {
    chat('Polls must include at least two comma-separated choices.');
    return;
  }
  const question = lines[0];
  new Quiz(question, choices);

  chat(`Polls are open - ${lines[0]}`);
  for (var i = 0; i < choices.length; i++) {
    chat(choices[i]);
  }
  chat('Enter your choice here in the chat window or in a separate DM whisper to me.');
}

function moveAbsolute(now: number, params: string, userId: string) {
}

function moveRelative(now: number, params: string, userId: string) {
}

var pinkSeeds: Sprites;
var blueSeeds: Sprites;
var yellowSeeds: Sprites;
var purpleSeeds: Sprites;

var dronesRed: Sprites;
var dronesBlue: Sprites;
var allDrones: SpriteCollection;
var allSeeds: SpriteCollection;

var redSplotches: SplatSprites;
var blackSplotches: SplatSprites;
var whiteSplotches: SplatSprites;
var orangeSplotches: SplatSprites;
var amberSplotches: SplatSprites;
var yellowSplotches: SplatSprites;
var greenSplotches: SplatSprites;
var blueSplotches: SplatSprites;
var cyanSplotches: SplatSprites;
var indigoSplotches: SplatSprites;
var violetSplotches: SplatSprites;
var magentaSplotches: SplatSprites;

function loadDrones(color: string): Sprites {
  let drones = new Sprites(`Drones/${color}/Drone`, 30, 15, AnimationStyle.Loop);
  drones.segmentSize = 2;
  drones.removeOnHitFloor = false;
  drones.moves = true;
  allDrones.add(drones);
  return drones;
}

function loadWatercolors(color: string): SpriteCollection {
  const numFolders: number = 9;
  var spriteCollection: SpriteCollection = new SpriteCollection();
  for (var folderIndex = 1; folderIndex <= numFolders; folderIndex++) {
    let watercolors = new Sprites(`Paint/Watercolors/${color}/${folderIndex}/WaterColor`, 142, 15, AnimationStyle.SequentialStop, true);
    spriteCollection.add(watercolors);
  }

  return spriteCollection;
}

function loadSplat(color: string): SplatSprites {
  return new SplatSprites(color, 39, 15, AnimationStyle.SequentialStop, true);
}

function addSplats() {
  redSplotches = loadSplat('Red');
  blackSplotches = loadSplat('Black');
  whiteSplotches = loadSplat('White');
  orangeSplotches = loadSplat('Orange');
  amberSplotches = loadSplat('Amber');
  yellowSplotches = loadSplat('Yellow');
  greenSplotches = loadSplat('Green');
  blueSplotches = loadSplat('Blue');
  cyanSplotches = loadSplat('Cyan');
  indigoSplotches = loadSplat('Indigo');
  violetSplotches = loadSplat('Violet');
  magentaSplotches = loadSplat('Magenta');

  allSplats.add(blackSplotches);
  allSplats.add(redSplotches);
  allSplats.add(orangeSplotches);
  allSplats.add(amberSplotches);
  allSplats.add(yellowSplotches);
  allSplats.add(greenSplotches);
  allSplats.add(cyanSplotches);
  allSplats.add(blueSplotches);
  allSplats.add(indigoSplotches);
  allSplats.add(violetSplotches);
  allSplats.add(magentaSplotches);
  allSplats.add(whiteSplotches);
}


function addDrones() {
  allDrones = new SpriteCollection();

  dronesRed = loadDrones('Red');
  dronesBlue = loadDrones('Blue');
}

function addSeeds() {
  allSeeds = new SpriteCollection();

  pinkSeeds = new Sprites("Seeds/Pink/PinkSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
  pinkSeeds.moves = true;
  allSeeds.add(pinkSeeds);

  blueSeeds = new Sprites("Seeds/Blue/BlueSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
  blueSeeds.moves = true;
  allSeeds.add(blueSeeds);

  yellowSeeds = new Sprites("Seeds/Yellow/YellowSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
  yellowSeeds.moves = true;
  allSeeds.add(yellowSeeds);

  purpleSeeds = new Sprites("Seeds/Purple/PurpleSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
  purpleSeeds.moves = true;
  allSeeds.add(purpleSeeds);
}

var droneExplosions: SpriteCollection;

function loadDroneExplosions() {
  droneExplosions = new SpriteCollection();
  const numDroneExplosions: number = 11;
  for (var i = 1; i <= numDroneExplosions; i++) {
    droneExplosions.add(new Sprites('Drones/Explosions/' + i + '/Explosion', 53, 5, AnimationStyle.Sequential, true));
  }
}

var gravityGames = new GravityGames();

document.onkeydown = handleKeyDown;

let globalLoadSprites: boolean = loadCopyrightedContent;

var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myCanvas");

var myRocket = new Rocket(0, 0);
var started = false;
myRocket.x = 0;
myRocket.y = 0;

var coins = new Sprites("Spinning Coin/SpinningCoin", 165, 5, AnimationStyle.Loop, false, null, outlineChatRoom /* allButMark */ /* outlineCodeEditor  */ /* fillChatRoom */);

addSeeds();

//var greenSeeds = new Sprites("Seeds/Yellow/YellowSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
//greenSeeds.moves = true;

var beesYellow = new Sprites("Bees/Yellow/BeeYellow", 18, 15, AnimationStyle.Loop);
beesYellow.segmentSize = 2;
beesYellow.removeOnHitFloor = false;
beesYellow.moves = true;

addDrones();

addSplats();

var redMeteors: Sprites = new Sprites("Spinning Rock/Red/Meteor", 63, 50, AnimationStyle.Loop, false, addMeteorExplosion);
redMeteors.moves = true;

var blueMeteors: Sprites = new Sprites("Spinning Rock/Blue/Meteor", 63, 50, AnimationStyle.Loop, false, addMeteorExplosion);
blueMeteors.moves = true;

var purpleMeteors: Sprites = new Sprites("Spinning Rock/Purple/Meteor", 63, 50, AnimationStyle.Loop, false, addMeteorExplosion);
purpleMeteors.moves = true;

var redExplosions: Sprites = new Sprites("Explosion/Red/Explosion", 179, 5, AnimationStyle.Sequential);
var blueExplosions: Sprites = new Sprites("Explosion/Blue/Explosion", 179, 5, AnimationStyle.Sequential);
var purpleExplosions: Sprites = new Sprites("Explosion/Purple/Explosion", 179, 5, AnimationStyle.Sequential);

loadDroneExplosions();

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

globalLoadSprites = true;

//var grass1 = new Sprites("Grass/1/Grass", 513, grassFrameRate, AnimationStyle.SequentialStop, true);
//var grass2 = new Sprites("Grass/2/Grass", 513, grassFrameRate, AnimationStyle.SequentialStop, true);
//var grass3 = new Sprites("Grass/3/Grass", 589, grassFrameRate, AnimationStyle.SequentialStop, true);
//var grass4 = new Sprites("Grass/4/Grass", 589, grassFrameRate, AnimationStyle.SequentialStop, true);

var backgroundBanner = new Part("CodeRushedBanner", 1, AnimationStyle.Static, 200, 300);
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");
setInterval(updateScreen, 10);
gravityGames.selectPlanet('Earth');
var red: HueSatLight = HueSatLight.fromHex('#ff3329');
var orange: HueSatLight = HueSatLight.fromHex('#ff9229');
var yellow: HueSatLight = HueSatLight.fromHex('#ffd929');
var limeGreen: HueSatLight = HueSatLight.fromHex('#76ff29');
var teal: HueSatLight = HueSatLight.fromHex('#29ffc3');
var blue: HueSatLight = HueSatLight.fromHex('#296dff');
var violet: HueSatLight = HueSatLight.fromHex('#9e29ff');
var pink: HueSatLight = HueSatLight.fromHex('#ff2965');
var redRight: HueSatLight = HueSatLight.fromHex('#ff2933');

var toHex: string = redRight.toHex();