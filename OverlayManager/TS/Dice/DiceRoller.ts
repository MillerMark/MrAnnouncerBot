var container, scene, camera, renderer, controls, stats, world, dice = [];
var scalingDice = [];
var specialDice = [];
var bodiesToRemove = [];
var bodiesToFree = [];
var diceSounds = new DiceSounds();

var waitingForSettle = false;
var diceHaveStoppedRolling = false;
var firstStopTime: number;
var diceValues = [];

function setNormalGravity() {
  world.gravity.set(0, -9.82 * 20, 0);
}

function init() { // From Rolling.html example.
  diceLayer = new DiceLayer();
  // SCENE
  // @ts-ignore - THREE
  scene = new THREE.Scene();

  // CAMERA
  //var SCREEN_WIDTH = window.innerWidth, SCREEN_HEIGHT = window.innerHeight;
  var SCREEN_WIDTH = 1920, SCREEN_HEIGHT = 1080;
  var lensFactor = 5;
  var VIEW_ANGLE = 45 / lensFactor, ASPECT = SCREEN_WIDTH / SCREEN_HEIGHT, NEAR = 0.01, FAR = 20000;
  // @ts-ignore - THREE
  camera = new THREE.PerspectiveCamera(VIEW_ANGLE, ASPECT, NEAR, FAR);
  scene.add(camera);
  camera.position.set(0, 30 * lensFactor, 0);
  // RENDERER
  // @ts-ignore - THREE
  renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
  renderer.setClearColor(0x000000, 0);

  renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);
  renderer.shadowMap.enabled = true;
  // @ts-ignore - THREE
  renderer.shadowMap.type = THREE.PCFSoftShadowMap;

  container = document.getElementById('ThreeJS');
  container.appendChild(renderer.domElement);
  // EVENTS
  // CONTROLS
  // @ts-ignore - THREE
  controls = new THREE.OrbitControls(camera, renderer.domElement);

  //// STATS
  //stats = new Stats();
  //stats.domElement.style.position = 'absolute';
  //stats.domElement.style.bottom = '0px';
  //stats.domElement.style.zIndex = 100;
  //container.appendChild(stats.domElement);

  // @ts-ignore - THREE
  let ambient = new THREE.AmbientLight('#ffffff', 0.35);
  scene.add(ambient);

  // @ts-ignore - THREE
  let directionalLight = new THREE.DirectionalLight('#ffffff', 0.25);
  directionalLight.position.x = -1000;
  directionalLight.position.y = 1000;
  directionalLight.position.z = 1000;
  scene.add(directionalLight);

  // @ts-ignore - THREE
  let light = new THREE.SpotLight(0xefdfd5, 0.7);
  light.position.x = 10;
  light.position.y = 100;
  light.position.z = 10;
  light.target.position.set(0, 0, 0);
  light.castShadow = true;
  light.shadow.camera.near = 50;
  light.shadow.camera.far = 110;
  light.shadow.mapSize.width = 1024;
  light.shadow.mapSize.height = 1024;

  scene.add(light);


  // @ts-ignore - THREE
  var material = new THREE.ShadowMaterial();
  material.opacity = 0.5;
  // @ts-ignore - THREE
  var geometry = new THREE.PlaneGeometry(1000, 1000, 1, 1);
  // @ts-ignore - THREE
  var mesh = new THREE.Mesh(geometry, material);
  mesh.receiveShadow = true;
  mesh.rotation.x = -Math.PI / 2;
  scene.add(mesh);

  ////////////
  // CUSTOM //
  ////////////
  // @ts-ignore - CANNON
  world = new CANNON.World();

  setNormalGravity();
  // @ts-ignore - CANNON
  world.broadphase = new CANNON.NaiveBroadphase();
  world.solver.iterations = 32;

  // @ts-ignore - DiceManager
  DiceManager.setWorld(world);

  // create the sphere's material
  const redWallMaterial =
    // @ts-ignore - THREE
    new THREE.MeshLambertMaterial(
      {
        color: 0xA00050
      });

  const blueWallMaterial =
    // @ts-ignore - THREE
    new THREE.MeshLambertMaterial(
      {
        color: 0x2000C0
      });

  const wallHeight = 24;
  const leftWallHeight = 24;
  const topWallHeight = 48;

  const wallThickness = 1;
  const leftWallWidth = 50;
  const leftWallX = -21.5;

  const dmLeftWallWidth = 9;
  const dmLeftWallX = 13;
  const dmLeftWallZ = -10;

  const topWallWidth = 50;
  const topWallZ = -12.5;

  const playerTopWallWidth = 30;
  const playerTopWallZ = 5;
  const playerTopWallX = -5;

  const playerRightWallWidth = 9;
  const playerRightWallX = 10;
  const playerRightWallZ = 9;

  const dmBottomWallWidth = 9;
  const dmBottomWallZ = -6;
  const dmBottomWallX = 17;

  const showWalls = false;
  const addPlayerWall = true;
  const addDungeonMasterWalls = true;
  if (showWalls) {

    // @ts-ignore - THREE
    const leftWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, leftWallHeight, leftWallWidth), redWallMaterial);
    leftWall.position.x = leftWallX;
    scene.add(leftWall);

    // @ts-ignore - THREE
    const topWall = new THREE.Mesh(new THREE.BoxGeometry(topWallWidth, topWallHeight, wallThickness), redWallMaterial);
    topWall.position.z = topWallZ;
    // @ts-ignore - THREE
    topWall.rotateOnAxis(new THREE.Vector3(1, 0, 0), -45)
    scene.add(topWall);

    if (addPlayerWall) {
      // @ts-ignore - THREE
      const playerTopWall = new THREE.Mesh(new THREE.BoxGeometry(playerTopWallWidth, wallHeight, wallThickness), redWallMaterial);
      playerTopWall.position.x = playerTopWallX;
      playerTopWall.position.z = playerTopWallZ;
      scene.add(playerTopWall);

      // @ts-ignore - THREE
      const playerRightWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, wallHeight, playerRightWallWidth), blueWallMaterial);
      playerRightWall.position.x = playerRightWallX;
      playerRightWall.position.z = playerRightWallZ;
      scene.add(playerRightWall);
    }

    if (addDungeonMasterWalls) {
      // @ts-ignore - THREE
      const dmBottomWall = new THREE.Mesh(new THREE.BoxGeometry(dmBottomWallWidth, wallHeight, wallThickness), redWallMaterial);
      dmBottomWall.position.x = dmBottomWallX;
      dmBottomWall.position.z = dmBottomWallZ;
      scene.add(dmBottomWall);

      // @ts-ignore - THREE
      const dmLeftWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, wallHeight, dmLeftWallWidth), redWallMaterial);
      dmLeftWall.position.x = dmLeftWallX;
      dmLeftWall.position.z = dmLeftWallZ;
      scene.add(dmLeftWall);
    }
  }


  // Floor
  // @ts-ignore - CANNON
  let floorBody = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.floorBodyMaterial });
  // @ts-ignore - CANNON
  floorBody.quaternion.setFromAxisAngle(new CANNON.Vec3(1, 0, 0), -Math.PI / 2);
  floorBody.name = 'floor';
  world.add(floorBody);

  //Walls
  // @ts-ignore - CANNON
  let rightWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.barrierBodyMaterial });
  rightWall.name = 'wall';
  // @ts-ignore - CANNON
  rightWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2);
  rightWall.position.x = 20.5;
  world.add(rightWall);

  // @ts-ignore - CANNON
  let bottomWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.barrierBodyMaterial });
  bottomWall.name = 'wall';
  // @ts-ignore - CANNON
  bottomWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2 * 2);
  bottomWall.position.z = 11.5;
  world.add(bottomWall);

  // @ts-ignore - CANNON
  var wallDiceContactMaterial = new CANNON.ContactMaterial(DiceManager.barrierBodyMaterial, DiceManager.diceBodyMaterial, { friction: 0.0, restitution: 0.9 });
  world.addContactMaterial(wallDiceContactMaterial);

  //let leftWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.floorBodyMaterial });
  //leftWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2);
  //leftWall.position.x = -20;
  //world.add(leftWall);

  // @ts-ignore - CANNON & DiceManager
  let topCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(topWallWidth, wallHeight, wallThickness)), material: DiceManager.barrierBodyMaterial });
  topCannonWall.name = 'wall';
  topCannonWall.position.z = topWallZ;
  // @ts-ignore - CANNON 
  topCannonWall.quaternion.setFromEuler(-Math.PI / 4, 0, 0);
  world.add(topCannonWall);

  // @ts-ignore - CANNON & DiceManager
  let leftCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, wallHeight, leftWallWidth)), material: DiceManager.barrierBodyMaterial });
  leftCannonWall.name = 'wall';
  leftCannonWall.position.x = leftWallX;
  world.add(leftCannonWall);


  if (addPlayerWall) {
    // @ts-ignore - CANNON & DiceManager
    let playerTopCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(playerTopWallWidth * 0.5, wallHeight, wallThickness)), material: DiceManager.barrierBodyMaterial });
    playerTopCannonWall.name = 'wall';
    playerTopCannonWall.position.x = playerTopWallX;
    playerTopCannonWall.position.z = playerTopWallZ;
    world.add(playerTopCannonWall);

    // @ts-ignore - CANNON & DiceManager
    let playerRightCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, wallHeight, playerRightWallWidth * 0.5)), material: DiceManager.barrierBodyMaterial });
    playerRightCannonWall.name = 'wall';
    playerRightCannonWall.position.x = playerRightWallX;
    playerRightCannonWall.position.z = playerRightWallZ;
    world.add(playerRightCannonWall);
  }

  if (addDungeonMasterWalls) {
    // @ts-ignore - CANNON & DiceManager
    let dmBottomCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(dmBottomWallWidth * 0.5, wallHeight, wallThickness)), material: DiceManager.barrierBodyMaterial });
    dmBottomCannonWall.name = 'wall';
    dmBottomCannonWall.position.x = dmBottomWallX;
    dmBottomCannonWall.position.z = dmBottomWallZ;
    world.add(dmBottomCannonWall);

    // @ts-ignore - CANNON & DiceManager
    let dmLeftCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, wallHeight, dmLeftWallWidth * 0.5)), material: DiceManager.barrierBodyMaterial });
    dmLeftCannonWall.name = 'wall';
    dmLeftCannonWall.position.x = dmLeftWallX;
    dmLeftCannonWall.position.z = dmLeftWallZ;
    world.add(dmLeftCannonWall);
  }

  //var groundShape = new CANNON.Plane();
  //var groundBody = new CANNON.Body({ mass: 0 });
  //groundBody.addShape(groundShape);
  //groundBody.quaternion.setFromAxisAngle(new CANNON.Vec3(1,0,0),-Math.PI/2);
  //world.add(groundBody);


  var needToHookEvents: boolean = true;
  var diceToRoll = 15;
  const dieScale = 1.5;
  for (var i = 0; i < diceToRoll; i++) {
    //var die = new DiceD20({ size: 1.5, backColor: colors[i] });
    // @ts-ignore - DiceD20
    var die = new DiceD20({ size: dieScale, backColor: '#D0D0ff' });
    scene.add(die.getObject());
    dice.push(die);
  }

  function restoreDieScale() {
    for (var i = 0; i < dice.length; i++) {
      let die = dice[i].getObject();
      die.scale.set(1, 1, 1);
    }
  }

  function randomDiceThrow() {
    diceLayer.clearLoopingAnimations();
    scalingDice = [];
    specialDice = [];
    bodiesToRemove = [];
    restoreDieScale();
    setNormalGravity();
    waitingForSettle = true;
    diceValues = [];

    for (var i = 0; i < dice.length; i++) {
      let yRand = Math.random() * 20
      let die = dice[i].getObject();
      die.position.x = -15 - (i % 3) * dieScale;
      die.position.y = 4 + Math.floor(i / 3) * dieScale;
      die.position.z = -13 + (i % 3) * dieScale;
      die.quaternion.x = (Math.random() * 90 - 45) * Math.PI / 180;
      die.quaternion.z = (Math.random() * 90 - 45) * Math.PI / 180;
      dice[i].updateBodyFromMesh();
      let rand = Math.random() * 5;
      die.body.velocity.set(35 + rand, 10 + yRand, 25 + rand);
      die.body.angularVelocity.set(20 * Math.random() - 10, 20 * Math.random() - 10, 20 * Math.random() - 10);

      diceValues.push({ dice: dice[i], value: Math.floor(Math.random() * 20 + 1) });
      die.body.name = 'die';
      console.log('die.scale.set(1, 1, 1);');
    }

    // @ts-ignore - DiceManager
    DiceManager.prepareValues(diceValues);

    diceHaveStoppedRolling = false;

    if (needToHookEvents) {
      needToHookEvents = false;
      for (var i = 0; i < dice.length; i++) {
        let die = dice[i].getObject();

        die.body.addEventListener("collide", function (e) {
          // @ts-ignore - DiceManager
          if (!DiceManager.throwRunning) {
            let relativeVelocity: number = Math.abs(Math.round(e.contact.getImpactVelocityAlongNormal()));
            //console.log(e.target.name + ' -> ' + e.body.name + ' at ' + relativeVelocity + 'm/s');

            let v = e.target.velocity;

            // <formula 2; targetSpeed = \sqrt{v.x^2 + v.y^2 + v.z^2}>  <-- LaTeX Formula

            //let targetSpeed: number = Math.sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
            //console.log('Target Speed: ' + targetSpeed);

            if (e.target.name === "die" && e.body.name === "die")
              diceSounds.playDiceHit(relativeVelocity / 10);
            else if (e.target.name === "die" && e.body.name === "floor")
              if (relativeVelocity < 8) {
                diceSounds.playSettle();
              }
              else
                diceSounds.playFloorHit(relativeVelocity / 35);
            else if (e.target.name === "die" && e.body.name === "wall")
              diceSounds.playWallHit(relativeVelocity / 40);
          }
        });
      }
    }
  }

  setInterval(randomDiceThrow, 9000);
  randomDiceThrow();
  requestAnimationFrame(animate);
}

function animate() {
  updatePhysics();
  render();
  update();

  requestAnimationFrame(animate);
}

function anyDiceStillRolling(): boolean {
  for (var i = 0; i < dice.length; i++) {
    let die = dice[i].getObject();
    if (die.body.velocity.norm() > 10)
      return true;
  }
  return false;
}

function checkStillRolling() {
  if (diceHaveStoppedRolling)
    return;
  if (anyDiceStillRolling()) {
    waitingForSettle = true;
    return;
  }

  if (waitingForSettle) {
    waitingForSettle = false;
    firstStopTime = performance.now();
  }
  else {
    let thisTime: number = performance.now();
    if ((thisTime - firstStopTime) / 1000 > 1.5) {
      onDiceRollStopped();
    }
  }
}

function getDiceValue(dice: any) {
  for (var i = 0; i < diceValues.length; i++) {
    let thisDiceValueEntry: any = diceValues[i];
    if (thisDiceValueEntry.dice == dice)
      return thisDiceValueEntry.value;
  }
  return 0;
}

function getScreenCoordinates(element) {
  // @ts-ignore - THREE
  var screenVector = new THREE.Vector3();
  element.localToWorld(screenVector);

  screenVector.project(camera);

  var x = Math.round((screenVector.x + 1) * renderer.domElement.offsetWidth / 2);
  var y = Math.round((1 - screenVector.y) * renderer.domElement.offsetHeight / 2);

  return {
    'x': x,
    'y': y,
  };
}

var diceSettleTime: number = performance.now();

enum DieEffect {
  StinkyFumes,
  Ring,
  Fireball,
  Shockwave,
  ColoredSmoke
}

function getDieEffectDistance(): number {
  return Math.round(Math.random() * 5) * 40 + 40;
}

function onDiceRollStopped() {
  diceHaveStoppedRolling = true;
  diceSettleTime = performance.now();
  console.log('Dice have stopped rolling!');
  console.log(dice);
  console.log(diceValues);
  scalingDice = [];
  specialDice = [];
  bodiesToRemove = [];
  let diePortalTimeDistance: number = 0;
  for (var i = 0; i < dice.length; i++) {
    let thisDiceValue: number = getDiceValue(dice[i]);
    let die: any = dice[i].getObject();


    die.dieValue = thisDiceValue;
    diePortalTimeDistance += getDieEffectDistance();
    die.effectStartOffset = diePortalTimeDistance;
    die.effectKind = DieEffect.ColoredSmoke;
    die.needToStartEffect = true;
    specialDice.push(die);

    //// Warp all out:
    //diePortalTimeDistance += getDieEffectDistance();
    //die.effectStartOffset = diePortalTimeDistance;
    //die.needToStartEffect = true;
    //die.needToDrop = true;
    //scalingDice.push(die);

    continue;




    if (thisDiceValue == 1) {
      console.log('Rolled a one!');
      die.dieValue = thisDiceValue;
      diePortalTimeDistance += getDieEffectDistance();
      die.effectStartOffset = diePortalTimeDistance;
      die.effectKind = DieEffect.StinkyFumes;
      die.needToStartEffect = true;
      specialDice.push(die);
    }
    else if (thisDiceValue == 20) {
      console.log('Rolled a twenty!');
      die.dieValue = thisDiceValue;
      diePortalTimeDistance += getDieEffectDistance();
      die.effectStartOffset = diePortalTimeDistance;
      die.effectKind = DieEffect.Fireball;
      die.needToStartEffect = true;
      specialDice.push(die);
    }
    else if (thisDiceValue < 5) {
      //let screenPos = getCoordinates(die);
      diePortalTimeDistance += getDieEffectDistance();
      die.effectStartOffset = diePortalTimeDistance;
      die.needToStartEffect = true;
      die.needToDrop = true;
      scalingDice.push(die);

      //console.log('die.body.collisionResponse: ' + die.body.collisionResponse);
      //die.body.collisionResponse = 0;
      //die.body.mass = 0;
      //world.gravity.set(0, -9.82 * 2000, 0);
      //die.position.x = -99999;

      // @ts-ignore - CANNON
      //var localVelocity = new CANNON.Vec3(0, 00, 0);
      //die.body.velocity = localVelocity;

      //let factor: number = 5;
      //let xSpin: number = Math.random() * factor - factor / 2;
      //let ySpin: number = Math.random() * factor - factor / 2;
      //let zSpin: number = Math.random() * factor - factor / 2;

      //// @ts-ignore - THREE
      //die.body.angularVelocity.set(xSpin, ySpin, zSpin);
      //setTimeout(spinDie.bind(die), 200);
    }
    else if (thisDiceValue < 10) {
      die.dieValue = thisDiceValue;
      diePortalTimeDistance += getDieEffectDistance();
      die.effectStartOffset = diePortalTimeDistance;
      die.effectKind = DieEffect.ColoredSmoke;
      die.needToStartEffect = true;
      specialDice.push(die);
    }
    else if (thisDiceValue > 13) {
      die.dieValue = thisDiceValue;
      diePortalTimeDistance += getDieEffectDistance();
      die.effectStartOffset = diePortalTimeDistance;
      die.effectKind = DieEffect.Ring;
      die.needToStartEffect = true;
      specialDice.push(die);
    }
  }
}

function scaleFallingDice() {
  if (!scalingDice || scalingDice.length == 0)
    return;

  let totalFrames: number = 20;
  let waitToFallTime: number = 700;

  if (scalingDice && scalingDice.length > 0) {
    for (var i = 0; i < scalingDice.length; i++) {

      let die = scalingDice[i];
      let portalOpenTime: number = diceSettleTime + die.effectStartOffset;
      let startFallTime: number = diceSettleTime + waitToFallTime + die.effectStartOffset;

      let now: number = performance.now();

      if (now > portalOpenTime && die.needToStartEffect) {
        die.needToStartEffect = false;
        let screenPos = getScreenCoordinates(die);
        diceLayer.testPortal(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
        diceSounds.playOpenDiePortal();
      }

      if (now < startFallTime)
        continue;

      let fps20: number = 50; // ms
      let totalScaleDistance: number = 0.99;
      let totalTimeToScale: number = fps20 * totalFrames;  // ms
      let elapsedTime: number = now - startFallTime;

      if (elapsedTime > totalTimeToScale)
        continue;

      if (die.needToDrop === true) {

        die.needToDrop = false;
      }

      let percentTraveled: number = elapsedTime / totalTimeToScale;

      let distanceTraveled: number = percentTraveled * totalScaleDistance;

      let newScale: number = 1 - distanceTraveled;

      die.scale.set(newScale, newScale, newScale);
      //if (newScale < 0.35) {
      //  bodiesToRemove.push(die);
      //  // @ts-ignore - DiceManager
      //  //die.body.collisionResponse = 1;
      //  //die.body.mass = 1;
      //  //DiceManager.world.remove(die.body);
      //}
    }
  }
}

function highlightSpecialDice() {
  if (!specialDice || specialDice.length == 0)
    return;


  let now: number = performance.now();

  for (var i = 0; i < specialDice.length; i++) {
    let die = specialDice[i];
    if (die.needToStartEffect) {
      let effectStartTime: number = diceSettleTime + die.effectStartOffset;
      if (now > effectStartTime && die.needToStartEffect) {
        die.needToStartEffect = false;

        // die.dieValue is also available.
        let screenPos: any = getScreenCoordinates(die);

        if (die.effectKind === DieEffect.Ring) {
          diceLayer.testMagicRing(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
        }
        else if (die.effectKind === DieEffect.Fireball) {
          diceLayer.testD20Fire(screenPos.x, screenPos.y);
          diceLayer.testFireball(screenPos.x, screenPos.y);
          diceSounds.playFireball();
        }
        else if (die.effectKind === DieEffect.StinkyFumes) {
          diceLayer.testRoll1Stink(screenPos.x, screenPos.y);
        }
        else if (die.effectKind === DieEffect.ColoredSmoke) {
          diceLayer.testDiceBlow(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
          let newScale: number = 0.01;
          die.scale.set(newScale, newScale, newScale);
          diceSounds.playDiceBlow();
        }

      }
    }
  }
}

function updatePhysics() {
  //  if (bodiesToRemove && bodiesToRemove.length > 0) {
  //    console.log('removing bodies...');

  //    bodiesToRemove.forEach(function (body) {
  //      body.collisionResponse = true;
  //      body.mass = 1;
  //      world.remove(body);
  //    });
  //    bodiesToRemove = [];
  //  }

  world.step(1.0 / 60.0);

  for (var i in dice) {
    dice[i].updateMeshFromBody();
  }
  checkStillRolling();
  scaleFallingDice();
  highlightSpecialDice();
}

function update() {
  controls.update();
  if (stats) {
    stats.update();
  }
}

function render() {
  renderer.render(scene, camera);
  diceLayer.renderCanvas();
}

init();