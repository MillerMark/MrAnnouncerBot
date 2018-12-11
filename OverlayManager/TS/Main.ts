const screenWidth: number = 1920;
const screenHeight: number = 1080;

let loadCopyrightedContent: boolean = true;

function putMeteorOnDrone(meteorProxy: SpriteProxy, droneProxy: SpriteProxy, now: number): void {
  let drone: Drone = <Drone>droneProxy;
  let meteor: Meteor = <Meteor>meteorProxy;
  if (drone && meteor) {
    drone.addMeteor(meteor, now);
  }
}

function updateScreen() {
  myContext.clearRect(0, 0, screenWidth, screenHeight);
  var now = performance.now();

  purplePortals.sprites.forEach((portal: SpriteProxy) => {
    if (portal instanceof Portal) {
      portal.checkApproaching(allDrones, now);
    }
  })

  myRocket.updatePosition(now);
  myRocket.bounce(0, 0, screenWidth, screenHeight, now);

  var coinsCollected: number = coins.collect(myRocket.x, myRocket.y, 310, 70);
  if (coinsCollected > 0) {
    new Audio(Folders.assets + 'Sound Effects/CollectCoin.wav').play();
    if (gravityGames.activeGame.score)
      gravityGames.activeGame.score.value += coinsCollected;
  }

  allSeeds.bounce(0, 0, screenWidth, screenHeight, now);

  beesYellow.bounce(0, 0, screenWidth, screenHeight, now);
  allDrones.bounce(0, 0, screenWidth, screenHeight, now);
  //greenSeeds.bounce(0, 0, screenWidth, screenHeight, now);

  allMeteors.bounce(0, 0, screenWidth, screenHeight, now);
  allMeteors.checkCollisionAgainst(allDrones, putMeteorOnDrone, now);

  //backgroundBanner.draw(myContext, 0, 0);

  if (!myRocket.isDocked)
    gravityGames.draw(myContext);

  allSplats.draw(myContext, now);
  portalBackground.draw(myContext, now);
  purplePortals.draw(myContext, now);

  //grass1.draw(myContext, now);
  //grass2.draw(myContext, now);
  //grass3.draw(myContext, now);
  //grass4.draw(myContext, now);

  beesYellow.updatePositions(now);
  allDrones.updatePositions(now);
  allMeteors.updatePositions(now);
  allWalls.updatePositions(now);
  endCaps.updatePositions(now);
  allSeeds.updatePositions(now);
  allSparks.updatePositions(now);
  
  wallBounce(now);

  allSeeds.draw(myContext, now);
  beesYellow.draw(myContext, now);
  allWalls.draw(myContext, now);
  droneGateways.draw(myContext, now);
  allDrones.draw(myContext, now);
  allMeteors.draw(myContext, now);

  endCaps.draw(myContext, now);

  myRocket.draw(myContext, now);
  purpleFlowers.draw(myContext, now);
  //blueFlowers.draw(myContext, now);
  redFlowers.draw(myContext, now);
  yellowFlowers1.draw(myContext, now);
  //yellowFlowers2.draw(myContext, now);
  yellowFlowers3.draw(myContext, now);

  redExplosions.draw(myContext, now);
  blueExplosions.draw(myContext, now);
  purpleExplosions.draw(myContext, now);
  droneExplosions.draw(myContext, now);
  //explosion.draw(myContext, 0, 0);

  allSparks.draw(myContext, now);
  sparkSmoke.draw(myContext, now);

  coins.draw(myContext, now);

  if (quiz)
    quiz.draw(myContext);
  //drawCrossHairs(myContext, crossX, crossY);
}

var allSplats = new SpriteCollection();

function handleKeyDown(evt) {
  const Key_B = 66;
  const Key_C = 67;
  const Key_D = 68;
  //const Key_G = 71;
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
      gravityGames.newGame();
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
  //else if (evt.keyCode == Key_G) {
  //  myRocket.dropSeed(now, 'grass');
  //}
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

function outlineGameSurface(sprites: Sprites) {
  sprites.layout(
    '*******************************************' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' + // 10
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' + // 20
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*******************************************', coinMargin);
}

function outlineGameSurfaceNoAdsNoMark(sprites: Sprites) {
  sprites.layout(
    '*******************************************' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' + // 10
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' + // 20
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*                                         *' + '\n' +
    '*****************             *************', coinMargin);
}

function fillChatRoom(sprites: Sprites) {
  sprites.layout(
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' + // 10
    '                                           ' + '\n' +
    '                               ************' + '\n' +
    '                               ************' + '\n' +
    '                               ************' + '\n' +
    '                               ************' + '\n' +
    '                               ************' + '\n' +
    '                               ************' + '\n' +
    '                               ************' + '\n' +
    '                               ************' + '\n' + // 20
    '                               ************' + '\n' +
    '                               ************' + '\n' +
    '                               ************' + '\n' +
    '                               ************' + '\n' +
    '                               ************', coinMargin);
}

function outlineChatRoom(sprites: Sprites) {
  sprites.layout(
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' +
    '                                           ' + '\n' + // 10
    '                                           ' + '\n' +
    '                               ************' + '\n' +
    '                               *          *' + '\n' +
    '                               *          *' + '\n' +
    '                               *          *' + '\n' +
    '                               *          *' + '\n' +
    '                               *          *' + '\n' +
    '                               *          *' + '\n' +
    '                               *          *' + '\n' + // 20
    '                               *          *' + '\n' +
    '                               *          *' + '\n' +
    '                               *          *' + '\n' +
    '                               *          *' + '\n' +
    '                               ************', coinMargin);
}

function outlineCodeEditor(sprites: Sprites) {
  sprites.layout(
    '********************************           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' + // 10
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' + // 20
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '*                              *           ' + '\n' +
    '********************************           ', coinMargin);
}

function allButMark(sprites: Sprites) {
  sprites.layout(
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' + // 10
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '********************************           ' + '\n' +
    '*********************** ********           ' + '\n' +
    '**********************   *******           ' + '\n' +
    '*********************     ******           ' + '\n' +
    '********************       *****           ' + '\n' +
    '********************       *****           ' + '\n' +
    '********************       *****           ' + '\n' + // 20
    '********************       *****           ' + '\n' +
    '*******************         ****           ' + '\n' +
    '******************           ***           ' + '\n' +
    '******************           ***           ', coinMargin);
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
  const flowerLifeSpan: number = 120 * 1000;
  spriteArray.sprites.push(new SpriteProxy(0, x - spriteArray.spriteWidth / 2, screenHeight - spriteArray.spriteHeight + y, flowerLifeSpan));
}

function plantSeeds(seeds, x) {
  if (seeds === pinkSeeds)
    plantSeed(redFlowers, x + 50, 0);
  //else if (seeds === blueSeeds)
  //  plantSeed(blueFlowers, x + 50, 5);
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
    let randomYellow: number = Math.random() * 4;
    if (randomYellow < 2)
      plantSeed(yellowFlowers1, x + 50, 5);
    //else if (randomYellow < 2)
    //  plantSeed(yellowFlowers2, x + 50, 0);
    else
      plantSeed(yellowFlowers3, x + 50, 0);
  }
  new Audio(Folders.assets + 'Sound Effects/MeteorHit.wav').play();
}

// TODO: consider moving to the meteor class.
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
  let thisDroneExplosion: Sprites = droneExplosions.allSprites[Math.floor(Math.random() * droneExplosions.allSprites.length)];
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
    if (isSuperUser(userName)) {
      selfDestructAllDrones();
      removeAllGameElements();
    }

    if (started && !myRocket.isDocked) {
      chat('docking...');
      myRocket.dock(now);
    }
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
  else if (command === "Toss") {
    tossMeteor(userId, params);
  }
  else if (command === "TestCommand") {
    test(params, userId, userName, displayName, color);
  }
  // TODO: Support !vote x
}

function tossMeteor(userId: string, params: string) {
  let numbers: string[] = params.split(',');
  if (!numbers || numbers.length < 2) {
    numbers = ['0', '0'];
  }
  // TODO: If third parameter is "x", kill all thrusters as we toss the meteor.
  let userDrone: Drone = <Drone>allDrones.find(userId);
  if (!userDrone)
    return;
  userDrone.tossMeteor(numbers[0], numbers[1]);
}

function selfDestructAllDrones() {
  allDrones.destroyAllBy(3000);
}

function removeAllGameElements() {
  allWalls.destroyAllBy(4000);
  portalBackground.destroyAllBy(0);
  purplePortals.destroyAllBy(4000);
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

function isSuperUser(userName: string) {
  return userName === "coderushed" || userName === "rorybeckercoderush";
}

function clearQuiz(userName: string) {
  if (!isSuperUser(userName)) {
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
//var blueSeeds: Sprites;
var yellowSeeds: Sprites;
var purpleSeeds: Sprites;

function wallBounce(now: number): void {
  allDrones.allSprites.forEach(function (drones: Sprites) {
    drones.sprites.forEach(function (drone: Drone) {
      horizontalSolidWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, now);
      verticalSolidWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, now);
      horizontalDashedWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, now);
      verticalDashedWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, now);
      horizontalDoubleWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, now);
      verticalDoubleWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, now);
    });
  });

  allMeteors.allSprites.forEach(function (meteors: Sprites) {
    meteors.sprites.forEach(function (meteor: Meteor) {
      horizontalSolidWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, now);
      verticalSolidWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, now);
      horizontalDashedWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, now);
      verticalDashedWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, now);
      horizontalDoubleWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, now);
      verticalDoubleWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, now);
    });
  });
  // TODO: Check for meteors as well.
}

var dronesRed: Sprites;
var droneGateways: Sprites;
//var dronesBlue: Sprites;
var allDrones: SpriteCollection;
var allSeeds: SpriteCollection;

var redSplats: SplatSprites;
var blackSplats: SplatSprites;
var whiteSplats: SplatSprites;
var orangeSplats: SplatSprites;
var amberSplats: SplatSprites;
var yellowSplats: SplatSprites;
var greenSplats: SplatSprites;
var blueSplats: SplatSprites;
var cyanSplats: SplatSprites;
var indigoSplats: SplatSprites;
var violetSplats: SplatSprites;
var magentaSplats: SplatSprites;

//` ![](2B073D0DC3C289F9E5723CAB5FD45014.png;;;0.02717,0.02717)

var allWalls: SpriteCollection;
var horizontalSolidWall: WallSprites;
var verticalSolidWall: WallSprites;
var horizontalDoubleWall: WallSprites;
var verticalDoubleWall: WallSprites;
var horizontalDashedWall: WallSprites;
var verticalDashedWall: WallSprites;
var endCaps: Sprites;

function loadWall(orientation: Orientation, style: WallStyle, folder: string, expectedFrameCount: number): WallSprites {
  var orientationStr: string;
  if (orientation === Orientation.Horizontal)
    orientationStr = 'Horizontal';
  else
    orientationStr = 'Vertical';
  const frameInterval: number = 85;
  let wall = new WallSprites(`FireWall/${orientationStr}/${folder}/Wall`, expectedFrameCount, frameInterval, AnimationStyle.Loop, true);
  wall.orientation = orientation;
  return wall;
}

function loadWalls() {
  endCaps = new Sprites(`FireWall/EndCaps/Cap`, 4, 0, AnimationStyle.Static);
  endCaps.baseAnimation.jiggleX = 1;
  endCaps.baseAnimation.jiggleY = 1;

  allWalls = new SpriteCollection();

  horizontalDashedWall = loadWall(Orientation.Horizontal, WallStyle.Dashed, 'Dashed/Right', 100);
  horizontalDashedWall.moves = true;
  allWalls.add(horizontalDashedWall);

  verticalDashedWall = loadWall(Orientation.Vertical, WallStyle.Dashed, 'Dashed/Up', 100);
  verticalDashedWall.moves = true;
  allWalls.add(verticalDashedWall);

  horizontalSolidWall = loadWall(Orientation.Horizontal, WallStyle.Solid, 'Solid', 94);
  horizontalSolidWall.moves = true;
  allWalls.add(horizontalSolidWall);

  horizontalDoubleWall = loadWall(Orientation.Horizontal, WallStyle.Double, 'Double', 94);
  horizontalDoubleWall.moves = true;
  allWalls.add(horizontalDoubleWall);

  verticalDoubleWall = loadWall(Orientation.Vertical, WallStyle.Double, 'Double', 94);
  verticalDoubleWall.moves = true;
  allWalls.add(verticalDoubleWall);

  verticalSolidWall = loadWall(Orientation.Vertical, WallStyle.Solid, 'Solid', 90);
  verticalSolidWall.moves = true;
  allWalls.add(verticalSolidWall);
}

//` ![](F8F205C07F3FCE8EEA5341ED98A92B36.png)  ![](77833405FDDE3ACB175756288F7BE0F8.png)
function loadDrones(color: string): Sprites {
  let drones = new Sprites(`Drones/${color}/Drone`, 30, 15, AnimationStyle.Loop);
  drones.segmentSize = 2;
  drones.removeOnHitFloor = false;
  drones.moves = true;
  allDrones.add(drones);
  return drones;
}

var droneHealthLights: Sprites;

function addDrones() {
  allDrones = new SpriteCollection();

  //dronesRed = loadDrones('Red');
  //dronesBlue = loadDrones('Blue');
  dronesRed = loadDrones('192x90');
}

//function loadWatercolors(color: string): SpriteCollection {
//  const numFolders: number = 9;
//  var spriteCollection: SpriteCollection = new SpriteCollection();
//  for (var folderIndex = 1; folderIndex <= numFolders; folderIndex++) {
//    let watercolors = new Sprites(`Paint/Watercolors/${color}/${folderIndex}/WaterColor`, 142, 15, AnimationStyle.SequentialStop, true);
//    spriteCollection.add(watercolors);
//  }

//  return spriteCollection;
//}


//` ![](147F1DE74025D1BED9947B18C213853F.png)
function loadSplat(color: string): SplatSprites {
  return new SplatSprites(color, 39, 15, AnimationStyle.SequentialStop, true);
}

function addSplats() {
  redSplats = loadSplat('Red');
  blackSplats = loadSplat('Black');
  whiteSplats = loadSplat('White');
  orangeSplats = loadSplat('Orange');
  amberSplats = loadSplat('Amber');
  yellowSplats = loadSplat('Yellow');
  greenSplats = loadSplat('Green');
  blueSplats = loadSplat('Blue');
  cyanSplats = loadSplat('Cyan');
  indigoSplats = loadSplat('Indigo');
  violetSplats = loadSplat('Violet');
  magentaSplats = loadSplat('Magenta');

  allSplats.add(blackSplats);
  allSplats.add(redSplats);
  allSplats.add(orangeSplats);
  allSplats.add(amberSplats);
  allSplats.add(yellowSplats);
  allSplats.add(greenSplats);
  allSplats.add(cyanSplats);
  allSplats.add(blueSplats);
  allSplats.add(indigoSplats);
  allSplats.add(violetSplats);
  allSplats.add(magentaSplats);
  allSplats.add(whiteSplats);
}


//` ![](7582E2608FC840A8A6D3CC61B5A58CB6.png)
function addSeeds() {
  allSeeds = new SpriteCollection();

  pinkSeeds = new Sprites("Seeds/Pink/PinkSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
  pinkSeeds.moves = true;
  allSeeds.add(pinkSeeds);

  //blueSeeds = new Sprites("Seeds/Blue/BlueSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
  //blueSeeds.moves = true;
  //allSeeds.add(blueSeeds);

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

function drawCrossHairs(context: CanvasRenderingContext2D, x: number, y: number) {
  const crossHalfSize: number = 8;
  context.beginPath();
  context.strokeStyle = '#f00';
  context.moveTo(x - crossHalfSize, y);
  context.lineTo(x + crossHalfSize, y);
  context.moveTo(x, y - crossHalfSize);
  context.lineTo(x, y + crossHalfSize);
  context.stroke();
}

//` ![](D4E24ABDF6E5B9F063E242EBB0ADA55E.png;;0,35,173,210)
var allSparks: SpriteCollection;
var downAndRightSparks: Sprites;
var downAndLeftSparks: Sprites;
var left1Sparks: Sprites;
var left2Sparks: Sprites;
var right1Sparks: Sprites;
var right2Sparks: Sprites;
var upAndRightSparks: Sprites;
var upAndLeftSparks: Sprites;

function loadSparks(folder: string, frameCount: number, originX: number, originY: number): Sprites {
  const sparkFrameInterval: number = 40;
  let sparks: Sprites = new Sprites(`FireWall/Sparks/${folder}/Spark`, frameCount, sparkFrameInterval, AnimationStyle.Sequential, true);
  //sparks.moves = true;
  sparks.originX = originX;
  sparks.originY = originY;
  return sparks;
}

function loadAllSparks() {
  allSparks = new SpriteCollection();
  downAndLeftSparks = loadSparks('Down and Left', 13, 90, 2);
  downAndRightSparks = loadSparks('Down and Right', 13, 5, 1);
  left1Sparks = loadSparks('Left', 8, 178, 19);
  left2Sparks = loadSparks('Left 2', 9, 121, 66);
  right1Sparks = loadSparks('Right', 8, 4, 23);
  right2Sparks = loadSparks('Right 2', 9, 2, 68);
  upAndRightSparks = loadSparks('Up and Right', 9, 6, 178);
  upAndLeftSparks = loadSparks('Up and Left', 9, 88, 176);

  allSparks.add(downAndLeftSparks);
  allSparks.add(downAndRightSparks);
  allSparks.add(left1Sparks);
  allSparks.add(right1Sparks);
  allSparks.add(left2Sparks);
  allSparks.add(right2Sparks);
  allSparks.add(upAndRightSparks);
  allSparks.add(upAndLeftSparks);
}

var sparkSmoke: Sprites;


var allMeteors: SpriteCollection;
var redMeteors: Sprites;
var blueMeteors: Sprites;
var purpleMeteors: Sprites;

//`![Meteor](5F8B49E97A5F459E6434A11E7FD272BE.png)
function addMeteors() {
  allMeteors = new SpriteCollection();

  const meteorFrameInterval: number = 38;
  redMeteors = new Sprites("Spinning Rock/Red/Meteor", 63, meteorFrameInterval, AnimationStyle.Loop, false, addMeteorExplosion);
  redMeteors.moves = true;

  blueMeteors = new Sprites("Spinning Rock/Blue/Meteor", 63, meteorFrameInterval, AnimationStyle.Loop, false, addMeteorExplosion);
  blueMeteors.moves = true;

  purpleMeteors = new Sprites("Spinning Rock/Purple/Meteor", 63, meteorFrameInterval, AnimationStyle.Loop, false, addMeteorExplosion);
  purpleMeteors.moves = true;

  allMeteors.add(redMeteors);
  allMeteors.add(blueMeteors);
  allMeteors.add(purpleMeteors);
}

function runStartupTests() {
  Line.runTests();
}

//runStartupTests();

var gravityGames = new GravityGames();

document.onkeydown = handleKeyDown;

let globalLoadSprites: boolean = loadCopyrightedContent;

var myCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("myCanvas");

var myRocket = new Rocket(0, 0);
var started = false;
myRocket.x = 0;
myRocket.y = 0;

var coins = new Sprites("Spinning Coin/32x32/SpinningCoin", 59, 15, AnimationStyle.Loop, true, null, outlineGameSurfaceNoAdsNoMark/* outlineGameSurface outlineChatRoom allButMark outlineCodeEditor */ /* fillChatRoom */);

addSeeds();

loadZaps();

//var greenSeeds = new Sprites("Seeds/Yellow/YellowSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
//greenSeeds.moves = true;

var beesYellow = new Sprites("Bees/Yellow/BeeYellow", 18, 15, AnimationStyle.Loop);
beesYellow.segmentSize = 2;
beesYellow.removeOnHitFloor = false;
beesYellow.moves = true;

addDrones();

addSplats();

addMeteors();

loadAllSparks();

var redExplosions: Sprites = new Sprites("Explosion/Red/Explosion", 179, 5, AnimationStyle.Sequential);
var blueExplosions: Sprites = new Sprites("Explosion/Blue/Explosion", 179, 5, AnimationStyle.Sequential);
var purpleExplosions: Sprites = new Sprites("Explosion/Purple/Explosion", 179, 5, AnimationStyle.Sequential);

loadWalls();
loadDroneExplosions();
droneHealthLights = new Sprites('Drones/192x90/Indicator', 60, 0, AnimationStyle.Static);

const smokeFrameInterval: number = 40;
sparkSmoke = new Sprites(`FireWall/Smoke/Smoke`, 45, smokeFrameInterval, AnimationStyle.Sequential, true);
sparkSmoke.originX = 121;
sparkSmoke.originY = 170;

globalBypassFrameSkip = true;
var redFlowers = new Sprites("Flowers/Red/RedFlower", 293, 15, AnimationStyle.Loop, true);
redFlowers.returnFrameIndex = 128;

const flowerFrameRate: number = 20;
const grassFrameRate: number = 25;

var yellowFlowers1 = new Sprites("Flowers/YellowPetunias1/YellowPetunias", 270, flowerFrameRate, AnimationStyle.Loop, true);
yellowFlowers1.returnFrameIndex = 64;
//var yellowFlowers2 = new Sprites("Flowers/YellowPetunias2/YellowPetunias", 270, flowerFrameRate, AnimationStyle.Loop, true);
//yellowFlowers2.returnFrameIndex = 64;
var yellowFlowers3 = new Sprites("Flowers/YellowPetunias3/YellowPetunias", 253, flowerFrameRate, AnimationStyle.Loop, true);
yellowFlowers3.returnFrameIndex = 45;

//var blueFlowers = new Sprites("Flowers/Blue/BlueFlower", 320, flowerFrameRate, AnimationStyle.Loop, true);
//blueFlowers.returnFrameIndex = 151;

var purpleFlowers = new Sprites("Flowers/Purple/PurpleFlower", 320, flowerFrameRate, AnimationStyle.Loop, true);
purpleFlowers.returnFrameIndex = 151;

droneGateways = new Sprites("Drones/Warp Gate/WarpGate", 73, 45, AnimationStyle.Loop, true);

const portalFrameRate: number = 40;
var purplePortals = new Sprites("Portal/Purple/Portal", 82, portalFrameRate, AnimationStyle.Loop, true);
var portalBackground = new Sprites("Portal/Black Back/Back", 13, portalFrameRate, AnimationStyle.SequentialStop, true);
purplePortals.returnFrameIndex = 13;

globalBypassFrameSkip = false;

globalLoadSprites = true;

var backgroundBanner = new Part("CodeRushedBanner", 1, AnimationStyle.Static, 200, 300);
var myContext: CanvasRenderingContext2D = myCanvas.getContext("2d");
gravityGames.selectPlanet('Earth');
gravityGames.newGame();

setInterval(updateScreen, 10);


var crossX: number = screenWidth / 2;
var crossY: number = screenHeight / 2;

function test(params: string, userId: string, userName: string, displayName: string, color: string) {
  if (params === 'game') {
    gravityGames.startGame(wallBoxesTest);
  }

  if (params === '+') {
    gravityGames.startGame(wallIntersectionTest);
  }

  if (params === 'delta') {
    gravityGames.startGame(wallChangeTest);
  }

  if (params === 'edge') {
    gravityGames.startGame(wallEdgeTest);
  }

  if (params === 'simple') {
    gravityGames.startGame(simpleWallChangeTest);
  }

  if (params === 'corner') {
    gravityGames.startGame(wallCornerTest);
  }

  if (params === 'top') {
    gravityGames.startGame(startTopTest);
  }

  if (params === 'sample') {
    gravityGames.startGame(sampleGame);
  }

  if (params === 'sample2') {
    gravityGames.startGame(sampleGame2);
  }

  if (params === 'sample3') {
    gravityGames.startGame(sampleGame3);
  }

  if (params === 'sample4') {
    gravityGames.startGame(sampleGame4);
  }

  if (params === 'gate') {
    gravityGames.startGame(gatewayTest);
  }

  if (params === 'portal drop') {
    purplePortals.sprites.forEach((portal: SpriteProxy) => {
      if (portal instanceof Portal) {
        portal.drop();
      }
    })
  }

  if (params === 'drop') {
    horizontalDashedWall.sprites.push(new Wall(0, 200 + endCaps.spriteWidth / 2, 300, Orientation.Horizontal, WallStyle.Dashed, 400));
    horizontalSolidWall.sprites.push(new Wall(0, 200 + endCaps.spriteWidth / 2, 600, Orientation.Horizontal, WallStyle.Solid, 400));
  }

  if (params === 'bounce') {
    horizontalDashedWall.sprites.push(new Wall(0, 200 + endCaps.spriteWidth / 2, 300, Orientation.Horizontal, WallStyle.Dashed, 400));
    horizontalDoubleWall.sprites.push(new Wall(0, 200 + endCaps.spriteWidth / 2, 800, Orientation.Horizontal, WallStyle.Double, 400));
  }

  if (params === 'rect') {
    horizontalSolidWall.sprites.push(new Wall(0, crossX - 300, crossY - 300, Orientation.Horizontal, WallStyle.Solid, 500));
    verticalSolidWall.sprites.push(new Wall(0, crossX + 200, crossY, Orientation.Vertical, WallStyle.Solid, 400));
    verticalDashedWall.sprites.push(new Wall(0, crossX - 780, crossY, Orientation.Vertical, WallStyle.Dashed, 400));
    horizontalDashedWall.sprites.push(new Wall(0, crossX - 300, crossY + 300, Orientation.Horizontal, WallStyle.Dashed, 500));
  }

  if (params === 'smoke')
    sparkSmoke.add(crossX, crossY);

  if (params === 'sparks 1')
    downAndRightSparks.add(crossX, crossY);
  else if (params === 'sparks 2')
    downAndLeftSparks.add(crossX, crossY);
  else if (params === 'sparks 3')
    left1Sparks.add(crossX, crossY);
  else if (params === 'sparks 4')
    left2Sparks.add(crossX, crossY);
  else if (params === 'sparks 5')
    right1Sparks.add(crossX, crossY);
  else if (params === 'sparks 6')
    right2Sparks.add(crossX, crossY);
  else if (params === 'sparks 7')
    upAndRightSparks.add(crossX, crossY);
  else if (params === 'sparks 8')
    upAndLeftSparks.add(crossX, crossY);
}
