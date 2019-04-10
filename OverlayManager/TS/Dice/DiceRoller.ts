var container, scene, camera, renderer, controls, stats, world, dice = [];
var diceSounds = new DiceSounds();
//import * as CANNON from './lib/dice/node_modules/cannon/build/cannon.js';


function init() { // From Rolling.html example.
  // SCENE
  scene = new THREE.Scene();
  // CAMERA
  //var SCREEN_WIDTH = window.innerWidth, SCREEN_HEIGHT = window.innerHeight;
  var SCREEN_WIDTH = 1920, SCREEN_HEIGHT = 1080;
  var VIEW_ANGLE = 45, ASPECT = SCREEN_WIDTH / SCREEN_HEIGHT, NEAR = 0.01, FAR = 20000;
  camera = new THREE.PerspectiveCamera(VIEW_ANGLE, ASPECT, NEAR, FAR);
  scene.add(camera);
  camera.position.set(0, 30, 0);
  // RENDERER
  renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
  renderer.setClearColor(0x000000, 0);

  renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);
  renderer.shadowMap.enabled = true;
  renderer.shadowMap.type = THREE.PCFSoftShadowMap;

  container = document.getElementById('ThreeJS');
  container.appendChild(renderer.domElement);
  // EVENTS
  // CONTROLS
  controls = new THREE.OrbitControls(camera, renderer.domElement);

  //// STATS
  //stats = new Stats();
  //stats.domElement.style.position = 'absolute';
  //stats.domElement.style.bottom = '0px';
  //stats.domElement.style.zIndex = 100;
  //container.appendChild(stats.domElement);

  let ambient = new THREE.AmbientLight('#ffffff', 0.35);
  scene.add(ambient);

  let directionalLight = new THREE.DirectionalLight('#ffffff', 0.25);
  directionalLight.position.x = -1000;
  directionalLight.position.y = 1000;
  directionalLight.position.z = 1000;
  scene.add(directionalLight);

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


  var material = new THREE.ShadowMaterial();
  material.opacity = 0.5;

  var geometry = new THREE.PlaneGeometry(1000, 1000, 1, 1);
  var mesh = new THREE.Mesh(geometry, material);
  mesh.receiveShadow = true;
  mesh.rotation.x = -Math.PI / 2;
  scene.add(mesh);

  ////////////
  // CUSTOM //
  ////////////
  world = new CANNON.World();

  world.gravity.set(0, -9.82 * 20, 0);
  world.broadphase = new CANNON.NaiveBroadphase();
  world.solver.iterations = 32;

  DiceManager.setWorld(world);

  // create the sphere's material
  const wallMaterial =
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
    const leftWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, leftWallHeight, leftWallWidth), wallMaterial);
    leftWall.position.x = leftWallX;
    scene.add(leftWall);

    const topWall = new THREE.Mesh(new THREE.BoxGeometry(topWallWidth, topWallHeight, wallThickness), wallMaterial);
    topWall.position.z = topWallZ;
    scene.add(topWall);

    if (addPlayerWall) {
      const playerTopWall = new THREE.Mesh(new THREE.BoxGeometry(playerTopWallWidth, playerTopWallHeight, wallThickness), wallMaterial);
      playerTopWall.position.x = playerTopWallX;
      playerTopWall.position.z = playerTopWallZ;
      scene.add(playerTopWall);
    }
  }


  // Floor
  let floorBody = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.floorBodyMaterial });
  floorBody.quaternion.setFromAxisAngle(new CANNON.Vec3(1, 0, 0), -Math.PI / 2);
  floorBody.name = 'floor';
  world.add(floorBody);

  //Walls
  let rightWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.barrierBodyMaterial });
  rightWall.name = 'wall';
  rightWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2);
  rightWall.position.x = 20.5;
  world.add(rightWall);

  let bottomWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.barrierBodyMaterial });
  bottomWall.name = 'wall';
  bottomWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2 * 2);
  bottomWall.position.z = 11.5;
  world.add(bottomWall);

  var wallDiceContactMaterial = new CANNON.ContactMaterial(DiceManager.barrierBodyMaterial, DiceManager.diceBodyMaterial, { friction: 0.0, restitution: 0.9 });
  world.addContactMaterial(wallDiceContactMaterial);

  //let leftWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.floorBodyMaterial });
  //leftWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2);
  //leftWall.position.x = -20;
  //world.add(leftWall);

  let topCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(topWallWidth, topWallHeight, wallThickness)), material: DiceManager.barrierBodyMaterial });
  topCannonWall.name = 'wall';
  topCannonWall.position.z = topWallZ;
  world.add(topCannonWall);

  let leftCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, leftWallHeight, leftWallWidth)), material: DiceManager.barrierBodyMaterial });
  leftCannonWall.name = 'wall';
  leftCannonWall.position.x = leftWallX;
  world.add(leftCannonWall);


  if (addPlayerWall) {
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


  var colors = ['#ff0000', '#ffff00', '#00ff00', '#0000ff', '#ff00ff'];
  var needToHookEvents: boolean = true;
  var diceToRoll = 8;
  for (var i = 0; i < diceToRoll; i++) {
    //var die = new DiceD20({ size: 1.5, backColor: colors[i] });
    var die = new DiceD20({ size: 1.5, backColor: '#D0D0ff' });
    scene.add(die.getObject());
    dice.push(die);
  }

  function randomDiceThrow() {
    var diceValues = [];

    for (var i = 0; i < dice.length; i++) {
      let yRand = Math.random() * 20
      let die = dice[i].getObject();
      die.position.x = -15 - (i % 3) * 1.5;
      die.position.y = 4 + Math.floor(i / 3) * 1.5;
      die.position.z = -13 + (i % 3) * 1.5;
      die.quaternion.x = (Math.random() * 90 - 45) * Math.PI / 180;
      die.quaternion.z = (Math.random() * 90 - 45) * Math.PI / 180;
      dice[i].updateBodyFromMesh();
      let rand = Math.random() * 5;
      die.body.velocity.set(35 + rand, 10 + yRand, 25 + rand);
      die.body.angularVelocity.set(20 * Math.random() - 10, 20 * Math.random() - 10, 20 * Math.random() - 10);

      diceValues.push({ dice: dice[i], value: Math.floor(Math.random() * 20 + 1) });
      die.body.name = 'die';
    }

    DiceManager.prepareValues(diceValues);

    if (needToHookEvents) {
      needToHookEvents = false;
      for (var i = 0; i < dice.length; i++) {
        let die = dice[i].getObject(); ~
          die.body.addEventListener("collide", function (e) {
            if (!DiceManager.throwRunning) {
              let relativeVelocity: number = Math.abs(Math.round(e.contact.getImpactVelocityAlongNormal()));
              //console.log(e.target.name + ' -> ' + e.body.name + ' at ' + relativeVelocity + 'm/s');

              let v = e.target.velocity;

              // <formula 2.5; targetSpeed = \sqrt{x^2 + y^2 + z^2}>
              let targetSpeed: number = Math.sqrt(v.x * v.x + v.y * v.y + v.z * v.z);

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

  setInterval(randomDiceThrow, 5000);
  randomDiceThrow();

  requestAnimationFrame(animate);
}

function animate() {
  updatePhysics();
  render();
  update();

  requestAnimationFrame(animate);
}

function updatePhysics() {
  world.step(1.0 / 60.0);

  for (var i in dice) {
    dice[i].updateMeshFromBody();
  }
}

function update() {

  controls.update();
  if (stats) {
    stats.update();
  }
}

function render() {
  renderer.render(scene, camera);
}

init();