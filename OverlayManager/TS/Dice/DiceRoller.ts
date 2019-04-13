var container, scene, camera, renderer, controls, stats, world, dice = [];
var scalingDice = [];
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
  const wallMaterial =
    // @ts-ignore - THREE
    new THREE.MeshLambertMaterial(
      {
        color: 0xA00050
      });

  const wallThickness = 1;
  const leftWallWidth = 50;
  const leftWallHeight = 3;
  const leftWallX = -21.5;

  const topWallWidth = 50;
  const topWallHeight = 3;
  const topWallZ = -12.5;

  const playerTopWallWidth = 30;
  const playerTopWallHeight = 3;
  const playerTopWallZ = 5;
  const playerTopWallX = -5;

  const showWalls = false;
  const addPlayerWall = true;
  if (showWalls) {

    // @ts-ignore - THREE
    const leftWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, leftWallHeight, leftWallWidth), wallMaterial);
    leftWall.position.x = leftWallX;
    scene.add(leftWall);

    // @ts-ignore - THREE
    const topWall = new THREE.Mesh(new THREE.BoxGeometry(topWallWidth, topWallHeight, wallThickness), wallMaterial);
    topWall.position.z = topWallZ;
    scene.add(topWall);

    if (addPlayerWall) {
      // @ts-ignore - THREE
      const playerTopWall = new THREE.Mesh(new THREE.BoxGeometry(playerTopWallWidth, playerTopWallHeight, wallThickness), wallMaterial);
      playerTopWall.position.x = playerTopWallX;
      playerTopWall.position.z = playerTopWallZ;
      scene.add(playerTopWall);
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

  // @ts-ignore - CANNON
  let topCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(topWallWidth, topWallHeight, wallThickness)), material: DiceManager.barrierBodyMaterial });
  topCannonWall.name = 'wall';
  topCannonWall.position.z = topWallZ;
  world.add(topCannonWall);

  // @ts-ignore - CANNON
  let leftCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, leftWallHeight, leftWallWidth)), material: DiceManager.barrierBodyMaterial });
  leftCannonWall.name = 'wall';
  leftCannonWall.position.x = leftWallX;
  world.add(leftCannonWall);


  if (addPlayerWall) {
    // @ts-ignore - CANNON
    let playerTopCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(playerTopWallWidth * 0.5, playerTopWallHeight, wallThickness)), material: DiceManager.barrierBodyMaterial });
    playerTopCannonWall.name = 'wall';
    playerTopCannonWall.position.x = playerTopWallX;
    playerTopCannonWall.position.z = playerTopWallZ;
    world.add(playerTopCannonWall);
  }

  //var groundShape = new CANNON.Plane();
  //var groundBody = new CANNON.Body({ mass: 0 });
  //groundBody.addShape(groundShape);
  //groundBody.quaternion.setFromAxisAngle(new CANNON.Vec3(1,0,0),-Math.PI/2);
  //world.add(groundBody);


  var needToHookEvents: boolean = true;
  var diceToRoll = 10;
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
    scalingDice = [];
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

  setInterval(randomDiceThrow, 7000);
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

function getCoordinates(element) {
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

function onDiceRollStopped() {
  diceHaveStoppedRolling = true;
  diceSettleTime = performance.now();
  console.log('Dice have stopped rolling!');
  console.log(dice);
  console.log(diceValues);
  scalingDice = [];
  for (var i = 0; i < dice.length; i++) {
    let thisDiceValue: number = getDiceValue(dice[i]);
    let die: any = dice[i].getObject();
    if (thisDiceValue == 20) {
      console.log('Rolled a twenty!');
      let screenPos = getCoordinates(die);
      console.log(screenPos);
      diceLayer.testFireball(screenPos.x, screenPos.y);
    }
    else if (thisDiceValue < 10) {
      let screenPos = getCoordinates(die);
      scalingDice.push(die);

      // <formula \omega il>

      //world.gravity.set(0, -9.82 * 1000, 0);
      die.body.collisionResponse = 0;
      die.body.mass = 0;
      //die.position.x = -99999;

      // @ts-ignore - CANNON
      var localVelocity = new CANNON.Vec3(0, 00, 0);
      die.body.velocity = localVelocity;

      let factor: number = 5;
      let xSpin: number = Math.random() * factor - factor / 2;
      let ySpin: number = Math.random() * factor - factor / 2;
      let zSpin: number = Math.random() * factor - factor / 2;

      // @ts-ignore - THREE
      die.body.angularVelocity.set(xSpin, ySpin, zSpin);
      //setTimeout(spinDie.bind(die), 200);

      diceLayer.testPortal(screenPos.x, screenPos.y);
    }
  }
}

function scaleFallingDice() {
  if (!scalingDice || scalingDice.length == 0)
    return;

  let totalFrames: number = 28;
  let fps20: number = 50; // ms
  let totalScaleDistance: number = 0.7;
  let totalTimeToScale: number = fps20 * totalFrames;  // ms
  let elapsedTime: number = performance.now() - diceSettleTime;

  if (elapsedTime > totalTimeToScale)
    return;

  let percentTraveled: number = elapsedTime / totalTimeToScale;

  let distanceTraveled: number = percentTraveled * totalScaleDistance;

  let newScale: number = 1 - distanceTraveled;

  if (scalingDice && scalingDice.length > 0) {
    for (var i = 0; i < scalingDice.length; i++) {
      let die = scalingDice[i];
      die.scale.set(newScale, newScale, newScale);
      if (newScale < 0.35) {
        // @ts-ignore - DiceManager
        //die.body.collisionResponse = 1;
        //die.body.mass = 1;
        //DiceManager.world.remove(die.body);
      }
    }

    // <formula s u r l y D \epsilon \nu>
  }
}

function updatePhysics() {
  world.step(1.0 / 60.0);

  for (var i in dice) {
    dice[i].updateMeshFromBody();
  }
  checkStillRolling();
  scaleFallingDice();
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