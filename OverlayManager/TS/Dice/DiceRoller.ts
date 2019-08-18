var diceRollData: DiceRollData;
var showDieValues: boolean = false;
var totalDamagePlusModifier: number = 0;
var totalHealthPlusModifier: number = 0;
var totalExtraPlusModifier: number = 0;
var totalRoll: number = 0;
var totalBonus: number = 0;
var d20RollValue: number = -1;
var attemptedRollWasSuccessful: boolean = false;
var attemptedRollWasNarrowlySuccessful: boolean = false;
var diceToRoll = 10;
var secondsBetweenRolls: number = 12;
var removeDiceImmediately: boolean = false;
const dieScale: number = 1.5;
var repeatRandomThrow: boolean = false;
var onBonusThrow: boolean = false;
var setupBonusRoll: boolean = false;
var waitingForBonusRollToComplete: boolean = false;
var bonusRollStartTime: number = 0;
var randomDiceThrowIntervalId: number = 0;
var damageModifierThisRoll: number = 0;
var healthModifierThisRoll: number = 0;
var extraModifierThisRoll: number = 0;

enum DieEffect {
	Ring,
	Lucky,
	Fireball,
	Portal,
	Shockwave,
	ColoredSmoke,
	Bomb,
	SteamPunkTunnel,
	HandGrab,
	Random
}

//`!-------------------------------------------------------

var container, scene, camera, renderer, controls, stats, world, dice = [];
var scalingDice = [];
var specialDice = [];
//var bodiesToremove = [];
var bodiesToFree = [];
var diceSounds = new DiceSounds('GameDev/Assets/DragonH/SoundEffects');

var waitingForSettle = false;
var allDiceHaveStoppedRolling = false;
var firstStopTime: number;
var diceValues = [];

function setNormalGravity() {
	world.gravity.set(0, -9.82 * 20, 0);
}

function restoreDieScale() {
	for (var i = 0; i < dice.length; i++) {
		let die = dice[i].getObject();
		die.scale.set(1, 1, 1);
	}
}

function clearAllDice() {
	if (!dice || dice.length === 0)
		return;
	for (var i = 0; i < dice.length; i++) {
		let die = dice[i];
		let dieObject = die.getObject();
		scene.remove(dieObject);
		die.clear();
	}
	dice = [];
	specialDice = [];
	scalingDice = [];
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

	function randomDiceThrow() {
		clearBeforeRoll();

		let dieValues: number = 10;
		for (var i = 0; i < diceToRoll; i++) {
			// @ts-ignore - DiceD20
			var die = new DiceD10x01({ size: dieScale, backColor: '#D0D0ff' });
			scene.add(die.getObject());
			dice.push(die);
		}
		needToHookEvents = true;

		for (var i = 0; i < dice.length; i++) {
			let yRand = Math.random() * 20
			let dieObject = dice[i].getObject();
			dieObject.position.x = -15 - (i % 3) * dieScale;
			dieObject.position.y = 4 + Math.floor(i / 3) * dieScale;
			dieObject.position.z = -13 + (i % 3) * dieScale;
			dieObject.quaternion.x = (Math.random() * 90 - 45) * Math.PI / 180;
			dieObject.quaternion.z = (Math.random() * 90 - 45) * Math.PI / 180;
			dice[i].updateBodyFromMesh();
			let rand = Math.random() * 5;
			dieObject.body.velocity.set(35 + rand, 10 + yRand, 25 + rand);
			dieObject.body.angularVelocity.set(20 * Math.random() - 10, 20 * Math.random() - 10, 20 * Math.random() - 10);

			diceValues.push({ dice: dice[i], value: Math.floor(Math.random() * dieValues + 1) });
			dieObject.body.name = 'die';
		}

		allDiceHaveStoppedRolling = false;

		// @ts-ignore - DiceManager
		DiceManager.prepareValues(diceValues);

		if (needToHookEvents) {
			// Test to see if this is related to the memory leak:

			needToHookEvents = false;
			for (var i = 0; i < dice.length; i++) {
				let die = dice[i].getObject();

				die.body.addEventListener("collide", handleDieCollision);
			}
		}
	}

	if (repeatRandomThrow) {
		randomDiceThrowIntervalId = setInterval(randomDiceThrow, secondsBetweenRolls * 1000);
		randomDiceThrow();
	}
	requestAnimationFrame(animate);
}

function movePointAtAngle(point: Vector, angleInDegrees: number, distance): Vector {
	let angleInRadians: number = angleInDegrees * Math.PI / 180;
	return new Vector(point.x + (Math.sin(angleInRadians) * distance), point.y - (Math.cos(angleInRadians) * distance));
}

function trailsSparks(diceRollData: DiceRollData): boolean {
	return diceRollData.type == DiceRollType.WildMagic || diceRollData.type == DiceRollType.BendLuckAdd || diceRollData.type == DiceRollType.BendLuckSubtract;
}

function handleDieCollision(e: any) {
	// @ts-ignore - DiceManager
	if (DiceManager.throwRunning || dice.length == 0)
		return;

	let relativeVelocity: number = Math.abs(Math.round(e.contact.getImpactVelocityAlongNormal()));
	//console.log(e.target.name + ' -> ' + e.body.name + ' at ' + relativeVelocity + 'm/s');

	if (e.target.name === "die" && e.body.name === "die")
		diceSounds.playDiceHit(relativeVelocity / 10);
	else if (e.target.name === "die" && e.body.name === "floor") {
		if (e.target.parentDie.hasNotHitFloorYet) {
			e.target.parentDie.hasNotHitFloorYet = false;
			dieFirstHitsFloor(e.target.parentDie);
		}
		if (relativeVelocity < 8) {
			diceSounds.playSettle();
		}
		else {
			diceSounds.playFloorHit(relativeVelocity / 35);

			if (trailsSparks(diceRollData)) {
				if (relativeVelocity > 12) {
					if (!e.target.parentDie.sparks)
						e.target.parentDie.sparks = [];
					let pos: Vector = getScreenCoordinates(e.target.parentDie.getObject());
					if (pos)
						e.target.parentDie.sparks.push(diceLayer.smallSpark(pos.x, pos.y));
					diceSounds.playRandom('Dice/Zap', 4);
				}
			}
		}
	}
	else if (e.target.name === "die" && e.body.name === "wall")
		diceSounds.playWallHit(relativeVelocity / 40);
}

function dieFirstHitsFloor(die: any) {
	if (diceRollData.onFirstContactSound) {
		diceSounds.safePlayMp3(diceRollData.onFirstContactSound);
		diceRollData.onFirstContactSound = null;
	}

	if (die.rollType == DieCountsAs.inspiration) {
		diceSounds.safePlayMp3('inspiration');
	}

	if (die.rollType == DieCountsAs.totalScore || die.rollType == DieCountsAs.inspiration) {
		// Move the effect closer to the center of the screen...
		let percentageOnDie: number = 0.7;
		let percentageOffDie: number = 1 - percentageOnDie;

		let mainEffectFunc: any;
		switch (diceRollData.onFirstContactEffect) {
			case SpriteType.SmokeExplosion:
				mainEffectFunc = diceLayer.addSneakAttack.bind(diceLayer);
				break;
		}
		if (mainEffectFunc) {
			let pos: Vector = getScreenCoordinates(die.getObject());
			if (pos)
				mainEffectFunc(pos.x * percentageOnDie + percentageOffDie * 960, pos.y * percentageOnDie + percentageOffDie * 540, diceLayer.activePlayerHueShift);
		}
	}
}

function positionTrailingSprite(die: any, addPrintFunc: (x: number, y: number, angle: number) => SpriteProxy, minForwardDistanceBetweenPrints: number, leftRightDistanceBetweenPrints: number = 0, index: number = 0): SpriteProxy {
	if (die.rollType == DieCountsAs.totalScore || die.rollType == DieCountsAs.inspiration || die.rollType == DieCountsAs.bentLuck) {
		let pos: Vector = getScreenCoordinates(die.getObject());
		if (!pos)
			return null;
		if (die.lastPos.length <= index)
			die.lastPos.push(new Vector(-100, -100));
		let deltaX: number = pos.x - die.lastPos[index].x;
		let deltaY: number = pos.y - die.lastPos[index].y;

		//` <formula 2; \sqrt{deltaX^2 + deltaY^2}>
		let distanceSinceLastPoint: number = Math.sqrt(deltaX * deltaX + deltaY * deltaY);


		// ![](44408656431640F1B13DDCA10C7B507D.png;;;0.04947,0.04744)
		if (distanceSinceLastPoint > minForwardDistanceBetweenPrints) {

			//` <formula 3; \frac{atan2(deltaY,deltaX) * 180}{\pi} + 90^{\circ}>
			let angle = Math.atan2(deltaY, deltaX) * 180 / Math.PI + 90;
			var angleToMovePawPrint: number = 90;
			if (die.lastPawPrintOnLeft)
				angleToMovePawPrint = -90;
			die.lastPawPrintOnLeft = !die.lastPawPrintOnLeft;
			let pawPrintPos: Vector = movePointAtAngle(pos, angle + angleToMovePawPrint, leftRightDistanceBetweenPrints);
			let spriteProxy: SpriteProxy = addPrintFunc(pawPrintPos.x, pawPrintPos.y, angle);
			//diceLayer.addPawPrint(pawPrintPos.x, pawPrintPos.y, angle);
			die.lastPos[index] = pos;
			return spriteProxy;
		}
	}
	return null;
}

//function placePuff(die: any) {
//  if (!die.isDamage) {
//    let pos: Vector = getScreenCoordinates(die.getObject());
//    let deltaX: number = pos.x - die.lastPos.x;
//    let deltaY: number = pos.y - die.lastPos.y;

//    //` <formula 2; \sqrt{deltaX^2 + deltaY^2}>
//    let distanceSinceLastPoint: number = Math.sqrt(deltaX * deltaX + deltaY * deltaY);


//    const minForwardDistanceBetweenPuffs = 150;
//    if (distanceSinceLastPoint > minForwardDistanceBetweenPuffs) {

//      //` <formula 3; \frac{atan2(deltaY,deltaX) * 180}{\pi} + 90^{\circ}>
//      let angle = Math.atan2(deltaY, deltaX) * 180 / Math.PI + 90;
//      diceLayer.addPuff(pos.x, pos.y, angle);
//      die.lastPos = pos;
//    }
//  }
//}

function animate() {
	if (!testing) {
		updatePhysics();
		render();
		update();
	}

	requestAnimationFrame(animate);
}

function anyDiceStillRolling(): boolean {
	for (var i = 0; i < dice.length; i++) {
		if (!dice[i].isFinished())
			return true;
		//let dieObject = dice[i].getObject();
		//if (dieObject.body.velocity.norm() > 5)
		//	return true;
	}
	return false;
}

enum WildMagic {
	wildMagicMinute,
	seeInvisibleCreatures,
	modronAppearsOneMinute,
	regain5hpPerTurnForOneMinute,
	castMagicMissile,
	castFireball,
	heightChange,
	castConfusionOnSelf,
	beardOfFeathers,
	castGreaseCenteredOnSelf,
	spellTargetsDisadvantagedSavingThrowForOneMinute,
	skinTurnsBlue,
	thirdEyeAdvantageWisdomChecks,
	castTimeBonusActionOneMinute,
	teleportUpTo60Feet,
	astralPlaneUntilEndOfNextTurn,
	maximizeDamageOnSpellCastInNextMinute,
	ageChange,
	flumphs,
	regainHitPoints,
	pottedPlant,
	teleportUpTo20FeetBonusActionOneMinute,
	castLevitateOnSelf,
	unicorn,
	cannotSpeakPinkBubbles,
	spectralShieldPlus2ArmorClassNextMinute,
	alcoholImmunity,
	hairFallsOutGrowsBack24Hours,
	fireTouchOneMinute,
	regainLowestLevelExpendedSpellSlot,
	shoutWhenSpeakingOneMinute,
	castFogCloudCenteredOnSelf,
	lightningDamageUpToThreeCreatures,
	frightenedByNearestCreatureUntilEndOfNextTurn,
	allCreatures30FeetInvisibleOneMinute,
	resistanceToAllDamageNextMinute,
	randomCreaturePoisoned1d4Hours,
	glowBrightOneMinuteCreaturesEndingTurn5FeetBlinded,
	castPolymorphToSheepOnSelf,
	butterfliesAndPetals10FeetOneMinute,
	takeOneAdditionalActionImmediately,
	allCreaturesWithin30FeetTake1d10NecroticDamage,
	castMirrorImage,
	castFlyOnRandomCreatureWithin60Feet,
	invisibleSilentNextMinute,
	immortalOneMinute,
	increaseSizeOneMinute,
	allCreatures30FeetVulnerableToPiercingDamageOneMinute,
	faintEtheralMusicOneMinute,
	regainSorceryPoints
}

function bonusRollDealsDamage(damageStr: string, description: string = '', playerID: number = -1): void {
	let bonusRoll: BonusRoll = diceRollData.addBonusRoll(damageStr, description, playerID, DiceLayer.damageDieBackgroundColor, DiceLayer.damageDieFontColor);
	bonusRoll.dieCountsAs = DieCountsAs.damage;
}

function needToRollBonusDice() {
	if (onBonusThrow || setupBonusRoll)
		return false;

	modifyTotalRollForTestingPurposes();
	checkAttackBonusRolls();
	checkSkillCheckBonusRolls();
	checkWildMagicBonusRolls();

	return diceRollData.bonusRolls && diceRollData.bonusRolls.length > 0;
}

function checkAttackBonusRolls() {
	if (isAttack(diceRollData)) {
		let isCriticalHit: boolean = d20RollValue >= diceRollData.minCrit;
		//console.log('isCriticalHit: ' + isCriticalHit);
		if (isCriticalHit && !diceRollData.secondRollData) {
			//console.log('diceRollData.damageHealthExtraDice: ' + diceRollData.damageHealthExtraDice);
			bonusRollDealsDamage(diceRollData.damageHealthExtraDice, '', getFirstPlayerId());
			//console.log('checkAttackBonusRolls(1) - Roll Bonus Dice: ' + diceRollData.bonusRolls.length);
		}
		getRollResults();
		if (attemptedRollWasSuccessful && diceRollData.minDamage > 0 && !diceRollData.secondRollData) {
			let extraRollStr: string = "";
			for (let i = 0; i < dice.length; i++) {
				let die = dice[i];
				if (die.rollType == DieCountsAs.damage && die.getTopNumber() < diceRollData.minDamage) {
					let damageStr: string = "";
					if (die.damageStr) {
						damageStr = `(${die.damageStr}:damage)`;
					}
					extraRollStr += `1d${die.values}${damageStr},`;
					removeDie(die, 0, DieEffect.HandGrab);
					const greatWeaponFightingTextDelay = 900;
					diceLayer.addDieTextAfter(die, 'Great Weapon Fighting', diceLayer.activePlayerDieColor, diceLayer.activePlayerDieColor, greatWeaponFightingTextDelay);
					die.inPlay = false;
					removeDieEffectsForSingleDie(die);
				}
			}
			if (extraRollStr.endsWith(',')) {
				extraRollStr = extraRollStr.substr(0, extraRollStr.length - 1);
			}
			if (extraRollStr) {
				//console.log('extraRollStr: ' + extraRollStr);
				bonusRollDealsDamage(extraRollStr, '', getFirstPlayerId());
				//console.log('checkAttackBonusRolls(2) - Roll Bonus Dice: ' + diceRollData.bonusRolls.length);
			}
		}
	}
}

function getFirstPlayerId() {
	let playerID: number = -1;
	if (diceRollData.playerRollOptions.length > 0)
		playerID = diceRollData.playerRollOptions[0].PlayerID;
	return playerID;
}

function checkSkillCheckBonusRolls() {
	if (diceRollData.type == DiceRollType.SkillCheck) {
		for (let i = 0; i < dice.length; i++) {
			let die = dice[i];
			if (die.inPlay && die.rollType == DieCountsAs.totalScore && die.isD20 && die.getTopNumber() == 20) {
				let dieColor: string = diceLayer.activePlayerDieColor;
				let dieTextColor: string = diceLayer.activePlayerDieFontColor;
				if (die.playerID >= 0) {
					dieColor = diceLayer.getDieColor(die.playerID);
					dieTextColor = diceLayer.getDieFontColor(die.playerID);
				}
				diceRollData.addBonusRoll('1d20', '', die.playerID, dieColor, dieTextColor);
			}
		}
	}
}

function checkWildMagicBonusRolls() {
	if (diceRollData.type == DiceRollType.WildMagic) {
		let rollValue: number = 0;
		for (let i = 0; i < dice.length; i++) {
			let die = dice[i];
			if (die.inPlay && (die.rollType == DieCountsAs.totalScore || die.rollType == DieCountsAs.inspiration))
				rollValue += die.getTopNumber();
		}
		totalRoll = rollValue + diceRollData.modifier;
		modifyTotalRollForTestingPurposes();
		diceRollData.playBonusSoundAfter = 2500;
		announceWildMagicResult(totalRoll);
		if (totalRoll == 0 || totalRoll == 99)
			diceRollData.wildMagic = WildMagic.regainSorceryPoints;
		else if (totalRoll < 3)
			diceRollData.wildMagic = WildMagic.wildMagicMinute;
		else if (totalRoll < 5)
			diceRollData.wildMagic = WildMagic.seeInvisibleCreatures;
		else if (totalRoll < 7)
			diceRollData.wildMagic = WildMagic.modronAppearsOneMinute;
		else if (totalRoll < 9)
			diceRollData.wildMagic = WildMagic.castFireball;
		else if (totalRoll < 11)
			diceRollData.wildMagic = WildMagic.castMagicMissile;
		else if (totalRoll < 13) {
			diceRollData.addBonusRoll('1d10', 'Inches Changed: ');
			diceRollData.wildMagic = WildMagic.heightChange;
			diceRollData.playBonusSoundAfter = 700;
		}
		else if (totalRoll < 15)
			diceRollData.wildMagic = WildMagic.castConfusionOnSelf;
		else if (totalRoll < 17)
			diceRollData.wildMagic = WildMagic.regain5hpPerTurnForOneMinute;
		else if (totalRoll < 19)
			diceRollData.wildMagic = WildMagic.beardOfFeathers;
		else if (totalRoll < 21)
			diceRollData.wildMagic = WildMagic.castGreaseCenteredOnSelf;
		else if (totalRoll < 23)
			diceRollData.wildMagic = WildMagic.spellTargetsDisadvantagedSavingThrowForOneMinute;
		else if (totalRoll < 25)
			diceRollData.wildMagic = WildMagic.skinTurnsBlue;
		else if (totalRoll < 27)
			diceRollData.wildMagic = WildMagic.thirdEyeAdvantageWisdomChecks;
		else if (totalRoll < 29)
			diceRollData.wildMagic = WildMagic.castTimeBonusActionOneMinute;
		else if (totalRoll < 31)
			diceRollData.wildMagic = WildMagic.teleportUpTo60Feet;
		else if (totalRoll < 33)
			diceRollData.wildMagic = WildMagic.astralPlaneUntilEndOfNextTurn;
		else if (totalRoll < 35)
			diceRollData.wildMagic = WildMagic.maximizeDamageOnSpellCastInNextMinute;
		else if (totalRoll < 37) {
			diceRollData.addBonusRoll('1d10', 'Years Changed: ');
			diceRollData.wildMagic = WildMagic.ageChange;
			diceRollData.playBonusSoundAfter = 700;
		}
		else if (totalRoll < 39) {
			diceRollData.addBonusRoll('1d6', 'Flumphs: ');
			diceRollData.wildMagic = WildMagic.flumphs;
		}
		else if (totalRoll < 41) {
			diceRollData.addBonusRoll('2d10', 'HP Regained: ');
			diceRollData.wildMagic = WildMagic.regainHitPoints;
		}
		else if (totalRoll < 43)
			diceRollData.wildMagic = WildMagic.pottedPlant;
		else if (totalRoll < 45)
			diceRollData.wildMagic = WildMagic.teleportUpTo20FeetBonusActionOneMinute;
		else if (totalRoll < 47)
			diceRollData.wildMagic = WildMagic.castLevitateOnSelf;
		else if (totalRoll < 49)
			diceRollData.wildMagic = WildMagic.unicorn;
		else if (totalRoll < 51)
			diceRollData.wildMagic = WildMagic.cannotSpeakPinkBubbles;
		else if (totalRoll < 53)
			diceRollData.wildMagic = WildMagic.spectralShieldPlus2ArmorClassNextMinute;
		else if (totalRoll < 55) {
			diceRollData.addBonusRoll('5d6', 'Days Immune: ');
			diceRollData.wildMagic = WildMagic.alcoholImmunity;
		}
		else if (totalRoll < 57)
			diceRollData.wildMagic = WildMagic.hairFallsOutGrowsBack24Hours;
		else if (totalRoll < 59)
			diceRollData.wildMagic = WildMagic.fireTouchOneMinute;
		else if (totalRoll < 61)
			diceRollData.wildMagic = WildMagic.regainLowestLevelExpendedSpellSlot;
		else if (totalRoll < 63)
			diceRollData.wildMagic = WildMagic.shoutWhenSpeakingOneMinute;
		else if (totalRoll < 65)
			diceRollData.wildMagic = WildMagic.castFogCloudCenteredOnSelf;
		else if (totalRoll < 67) {
			diceRollData.wildMagic = WildMagic.lightningDamageUpToThreeCreatures;
			diceRollData.addBonusDamageRoll('4d10(lightning)', 'Lightning Damage: ');
		}
		else if (totalRoll < 69)
			diceRollData.wildMagic = WildMagic.frightenedByNearestCreatureUntilEndOfNextTurn;
		else if (totalRoll < 71)
			diceRollData.wildMagic = WildMagic.allCreatures30FeetInvisibleOneMinute;
		else if (totalRoll < 73)
			diceRollData.wildMagic = WildMagic.resistanceToAllDamageNextMinute;
		else if (totalRoll < 75) {
			diceRollData.addBonusRoll('1d4', 'Hours Poisoned: ');
			diceRollData.wildMagic = WildMagic.randomCreaturePoisoned1d4Hours;
		}
		else if (totalRoll < 77)
			diceRollData.wildMagic = WildMagic.glowBrightOneMinuteCreaturesEndingTurn5FeetBlinded;
		else if (totalRoll < 79)
			diceRollData.wildMagic = WildMagic.castPolymorphToSheepOnSelf;
		else if (totalRoll < 81)
			diceRollData.wildMagic = WildMagic.butterfliesAndPetals10FeetOneMinute;
		else if (totalRoll < 83)
			diceRollData.wildMagic = WildMagic.takeOneAdditionalActionImmediately;
		else if (totalRoll < 85) {
			diceRollData.addBonusDamageRoll('1d10(necrotic)', 'Necrotic Damage: ');
			diceRollData.wildMagic = WildMagic.allCreaturesWithin30FeetTake1d10NecroticDamage;
		}
		else if (totalRoll < 87)
			diceRollData.wildMagic = WildMagic.castMirrorImage;
		else if (totalRoll < 89)
			diceRollData.wildMagic = WildMagic.castFlyOnRandomCreatureWithin60Feet;
		else if (totalRoll < 91)
			diceRollData.wildMagic = WildMagic.invisibleSilentNextMinute;
		else if (totalRoll < 93)
			diceRollData.wildMagic = WildMagic.immortalOneMinute;
		else if (totalRoll < 95)
			diceRollData.wildMagic = WildMagic.increaseSizeOneMinute;
		else if (totalRoll < 97)
			diceRollData.wildMagic = WildMagic.allCreatures30FeetVulnerableToPiercingDamageOneMinute;
		else if (totalRoll < 99)
			diceRollData.wildMagic = WildMagic.faintEtheralMusicOneMinute;
	}
}

function announceWildMagicResult(totalRoll: number) {
	var mp3BaseName: string = '99-00';
	if (totalRoll > 0 && totalRoll < 99) {
		let secondNumber: number = Math.floor((totalRoll + 1) / 2) * 2;
		let firstNumber: number = secondNumber - 1;
		let firstStr: string = firstNumber.toString();
		if (firstNumber < 10)
			firstStr = '0' + firstStr;
		let secondStr: string = secondNumber.toString();
		if (secondNumber < 10)
			secondStr = '0' + secondStr;
		mp3BaseName = firstStr + '-' + secondStr;
	}
	diceSounds.playMp3In(1800, 'Announcer/Wild Magic/' + mp3BaseName);
	diceSounds.safePlayMp3('WildMagic/' + mp3BaseName);
}

function modifyTotalRollForTestingPurposes() {
	//d20RollValue = 20; // critical hit.
	//totalRoll = 20;
	//totalRoll = 35; // age change
}

const bubbleId: string = 'bubble';

function freezeDie(die: any) {
	let body: any = die.getObject().body;
	body.mass = 0;
	body.updateMassProperties();
	body.velocity.set(0, 0, 0);
	body.angularVelocity.set(0, 0, 0);
}

function freezeExistingDice() {
	for (var i = 0; i < dice.length; i++) {
		let die = dice[i];
		freezeDie(die);
		if (die.attachedSprites && die.inPlay) {
			let hueShift: number = 0;
			if (die.playerID >= 0)
				hueShift = diceLayer.getHueShift(die.playerID);
			else
				hueShift = diceLayer.activePlayerHueShift;

			let bubble: SpriteProxy = diceLayer.addFreezeBubble(960, 540, hueShift, 100, 100);

			bubble.data = bubbleId;
			die.attachedSprites.push(bubble);
			diceSounds.playRandom('ice/Freeze', 5);
			die.origins.push(new Vector(diceLayer.freeze.originX, diceLayer.freeze.originY));
		}
	}
}

function popFrozenDice() {
	for (var i = 0; i < dice.length; i++) {
		let die = dice[i];
		if (!die.inPlay)
			continue;
		for (var j = 0; j < die.attachedSprites.length; j++) {
			let sprite: SpriteProxy = die.attachedSprites[j];
			if (sprite.data === bubbleId) {
				sprite.expirationDate = performance.now();

				let hueShift: number = 0;
				if (die.playerID >= 0)
					hueShift = diceLayer.getHueShift(die.playerID);

				die.attachedSprites[j] = diceLayer.addFeezePop(sprite.x, sprite.y, hueShift, 100, 100);
				die.origins[j] = new Vector(diceLayer.freezePop.originX, diceLayer.freezePop.originY);
				diceSounds.playRandom('ice/crack', 3);
			}
		}
	}
}

function rollBonusDice() {
	onBonusThrow = true;

	if (!diceRollData.bonusRolls)
		return;
	//console.log('rollBonusDice() - Roll Bonus Dice: ' + diceRollData.bonusRolls.length);
	for (var i = 0; i < diceRollData.bonusRolls.length; i++) {
		let bonusRoll: BonusRoll = diceRollData.bonusRolls[i];
		//console.log('bonusRoll.dieCountsAs: ' + bonusRoll.dieCountsAs);
		addDieFromStr(bonusRoll.playerID, bonusRoll.diceStr, bonusRoll.dieCountsAs, Random.between(1.2, 2.2), 0, bonusRoll.dieBackColor, bonusRoll.dieTextColor, bonusRoll.isMagic);
	}
	bonusRollStartTime = performance.now();
	waitingForBonusRollToComplete = true;
}

function isOdd(num) {
	return num % 2;
}

function playFinalRollSoundEffects() {
	if (!diceRollData)
		return;
	switch (diceRollData.wildMagic) {
		case WildMagic.wildMagicMinute: ; break;
		case WildMagic.seeInvisibleCreatures: ; break;
		case WildMagic.modronAppearsOneMinute: diceSounds.playWildMagic('modron'); break;
		case WildMagic.regain5hpPerTurnForOneMinute: ; break;
		case WildMagic.castMagicMissile: diceSounds.playWildMagic('magicMissile'); break;
		case WildMagic.castFireball: diceSounds.playWildMagic('fireball'); break;
		case WildMagic.heightChange:
		case WildMagic.ageChange:
			if (isOdd(totalBonus))
				diceSounds.playWildMagic('slideWhistleDown');
			else
				diceSounds.playWildMagic('slideWhistleUp');
			break;
		case WildMagic.castConfusionOnSelf: ; break;
		case WildMagic.beardOfFeathers: ; break;
		case WildMagic.castGreaseCenteredOnSelf: ; break;
		case WildMagic.spellTargetsDisadvantagedSavingThrowForOneMinute: ; break;
		case WildMagic.skinTurnsBlue: ; break;
		case WildMagic.thirdEyeAdvantageWisdomChecks: ; break;
		case WildMagic.castTimeBonusActionOneMinute: ; break;
		case WildMagic.teleportUpTo60Feet: ; break;
		case WildMagic.astralPlaneUntilEndOfNextTurn: ; break;
		case WildMagic.maximizeDamageOnSpellCastInNextMinute: ; break;
		case WildMagic.ageChange: ; break;
		case WildMagic.flumphs: ; break;
		case WildMagic.regainHitPoints: ; break;
		case WildMagic.pottedPlant: ; break;
		case WildMagic.teleportUpTo20FeetBonusActionOneMinute: ; break;
		case WildMagic.castLevitateOnSelf: ; break;
		case WildMagic.unicorn: ; break;
		case WildMagic.cannotSpeakPinkBubbles: ; break;
		case WildMagic.spectralShieldPlus2ArmorClassNextMinute: ; break;
		case WildMagic.alcoholImmunity: ; break;
		case WildMagic.hairFallsOutGrowsBack24Hours: ; break;
		case WildMagic.fireTouchOneMinute: ; break;
		case WildMagic.regainLowestLevelExpendedSpellSlot: ; break;
		case WildMagic.shoutWhenSpeakingOneMinute: ; break;
		case WildMagic.castFogCloudCenteredOnSelf: ; break;
		case WildMagic.lightningDamageUpToThreeCreatures: ; break;
		case WildMagic.frightenedByNearestCreatureUntilEndOfNextTurn: ; break;
		case WildMagic.allCreatures30FeetInvisibleOneMinute: ; break;
		case WildMagic.resistanceToAllDamageNextMinute: ; break;
		case WildMagic.randomCreaturePoisoned1d4Hours: ; break;
		case WildMagic.glowBrightOneMinuteCreaturesEndingTurn5FeetBlinded: ; break;
		case WildMagic.castPolymorphToSheepOnSelf: ; break;
		case WildMagic.butterfliesAndPetals10FeetOneMinute: ; break;
		case WildMagic.takeOneAdditionalActionImmediately: ; break;
		case WildMagic.allCreaturesWithin30FeetTake1d10NecroticDamage: ; break;
		case WildMagic.castMirrorImage: ; break;
		case WildMagic.castFlyOnRandomCreatureWithin60Feet: ; break;
		case WildMagic.invisibleSilentNextMinute: ; break;
		case WildMagic.immortalOneMinute: ; break;
		case WildMagic.increaseSizeOneMinute: ; break;
		case WildMagic.allCreatures30FeetVulnerableToPiercingDamageOneMinute: ; break;
		case WildMagic.faintEtheralMusicOneMinute: ; break;
		case WildMagic.regainSorceryPoints: ; break;
	}
}

function getDiceValue(die: any) {
	for (var i = 0; i < diceValues.length; i++) {
		let thisDiceValueEntry: any = diceValues[i];
		if (thisDiceValueEntry.dice == die) {
			if (thisDiceValueEntry.value != die.getUpsideValue())
				console.error(`thisDiceValueEntry.value (${thisDiceValueEntry.value}) != dice.getUpsideValue() (${die.getUpsideValue()})`);
			return thisDiceValueEntry.value;
		}
	}
	return 0;
}

function getScreenCoordinates(element): Vector {
	if (element == null)
		return null;
	// @ts-ignore - THREE
	var screenVector = new THREE.Vector3();
	element.localToWorld(screenVector);

	screenVector.project(camera);

	var x = Math.round((screenVector.x + 1) * renderer.domElement.offsetWidth / 2);
	var y = Math.round((1 - screenVector.y) * renderer.domElement.offsetHeight / 2);

	return new Vector(x, y);
}

function getDieEffectDistance(): number {
	return Math.round(Math.random() * 5) * 40 + 40;
}

function removeNonVantageDieNow(die: any) {
	if (!die)
		return;
	diceRollData.appliedVantage = true;
	removeDie(die, 0, DieEffect.HandGrab);

	for (var i = 0; i < dice.length; i++) {
		if (dice[i] === die) {
			die.inPlay = false; // This line may be redundant.
			removeDieEffectsForSingleDie(die);
		}
	}
}

function removeRemainingDice() {
	// TODO: Make sure we can call this robustly at any time.
	let dieEffectInterval: number = 0;
	removeDieEffects();
	var effectOverride = getRandomEffect();
	for (var i = 0; i < dice.length; i++) {
		if (dice[i].inPlay)
			dieEffectInterval = removeDie(dice[i], dieEffectInterval, effectOverride);
	}
}

function removeDie(die: any, dieEffectInterval: number, effectOverride: DieEffect = undefined) {
	if (!die.inPlay)
		return;

	if (effectOverride === undefined || effectOverride === DieEffect.Random) {
		var numTries: number = 0;
		effectOverride = getRandomEffect();
		while (effectOverride === DieEffect.Random && numTries < 10) {
			effectOverride = getRandomEffect();
			numTries++;
		}
		if (effectOverride === DieEffect.Random)
			effectOverride = DieEffect.Bomb;
	}

	let dieObject: any = die.getObject();
	if (dieObject) {
		dieObject.removeTime = performance.now();
		dieObject.effectKind = effectOverride;
		dieEffectInterval += getDieEffectDistance();
		dieObject.effectStartOffset = dieEffectInterval;
		dieObject.needToStartEffect = true;
		specialDice.push(die);  // DieEffect.Portal too???

		if (effectOverride === DieEffect.Portal) {
			dieObject.needToDrop = true;
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
		else if (effectOverride === DieEffect.HandGrab) {
			dieObject.hideOnScaleStop = true;
			dieObject.needToDrop = true;
			scalingDice.push(die);
		}
		else if (effectOverride === DieEffect.SteamPunkTunnel) {
			dieObject.hideOnScaleStop = true;
			dieObject.needToDrop = true;
			scalingDice.push(die);
		}
	}

	die.inPlay = false;
	return dieEffectInterval;
}

function getRandomEffect() {
	let random: number = Math.random() * 100;
	if (random < 20)
		return DieEffect.Bomb;
	else if (random < 40)
		return DieEffect.ColoredSmoke;
	else if (random < 60)
		return DieEffect.Portal;
	else if (random < 80)
		return DieEffect.SteamPunkTunnel;
	else
		return DieEffect.Random;
}

function scaleFallingDice() {
	if (!scalingDice || scalingDice.length == 0)
		return;

	var hiddenDie = [];
	if (scalingDice && scalingDice.length > 0) {
		for (var i = 0; i < scalingDice.length; i++) {

			let dieObject = scalingDice[i].getObject();
			if (dieObject == null)
				continue;
			let portalOpenTime: number = dieObject.removeTime + dieObject.effectStartOffset;

			let now: number = performance.now();
			let waitToFallTime: number;
			if (dieObject.effectKind == DieEffect.SteamPunkTunnel) {
				waitToFallTime = 35 * 30;
			}
			else if (dieObject.effectKind == DieEffect.HandGrab) {
				waitToFallTime = 34 * 30;
			}
			else { // DieEffect.Portal
				waitToFallTime = 700;
			}

			if (now > portalOpenTime && dieObject.needToStartEffect) {
				dieObject.needToStartEffect = false;
				let screenPos: Vector = getScreenCoordinates(dieObject);

				if (dieObject.effectKind == DieEffect.SteamPunkTunnel) {
					if (screenPos)
						diceLayer.addSteampunkTunnel(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
					diceSounds.playSteampunkTunnel();
				}
				else if (dieObject.effectKind == DieEffect.HandGrab) {
					let saturation: number = 100;
					let hueShift: number;
					if (DiceLayer.matchOozeToDieColor)
						if (scalingDice[i].rollType != DieCountsAs.totalScore && scalingDice[i].rollType != DieCountsAs.inspiration)
							hueShift = 0;
						else
							hueShift = diceLayer.activePlayerHueShift;
					else {
						hueShift = getNonGreenHueShift();
						if (Math.random() < 0.1)
							saturation = 0;
					}

					if (screenPos)
						diceLayer.testDiceGrab(screenPos.x, screenPos.y, hueShift, saturation, 100);
					diceSounds.playHandGrab();
				}
				else {  // DieEffect.Portal
					if (screenPos)
						diceLayer.addPortal(screenPos.x, screenPos.y, getNonGreenHueShift(), 100, 100);
					diceSounds.playOpenDiePortal();
				}
			}

			let startFallTime: number = dieObject.removeTime + waitToFallTime + dieObject.effectStartOffset;

			if (now < startFallTime)
				continue;

			let totalFrames: number;
			let fps30: number = 33; // ms

			let totalScaleDistance: number;

			let elapsedTime: number = now - startFallTime;

			if (dieObject.effectKind == DieEffect.SteamPunkTunnel) {
				totalFrames = 45;
				totalScaleDistance = 0.9;
			}
			else if (dieObject.effectKind == DieEffect.HandGrab) {
				totalFrames = 30;
				totalScaleDistance = 0.99;
			}
			else {
				totalFrames = 40;
				totalScaleDistance = 0.99;
			}

			let totalTimeToScale: number = fps30 * totalFrames;  // ms

			if (elapsedTime > totalTimeToScale) {
				if (dieObject.hideOnScaleStop) {
					dieObject.hideOnScaleStop = false;
					hideDie(dieObject);
					hiddenDie.push(scalingDice[i]);
				}
				continue;
			}

			if (dieObject.needToDrop === true) {

				dieObject.needToDrop = false;
			}

			let percentTraveled: number = elapsedTime / totalTimeToScale;

			let distanceTraveled: number = percentTraveled * totalScaleDistance;

			let newScale: number = 1 - distanceTraveled;

			if (newScale <= hiddenDieScale) {
				hideDie(dieObject);
			}
			else {
				dieObject.scale.set(newScale, newScale, newScale);
			}
			//if (newScale < 0.35) {
			//  bodiesToRemove.push(die);
			//  // @ts-ignore - DiceManager
			//  //die.body.collisionResponse = 1;
			//  //die.body.mass = 1;
			//  //DiceManager.world.remove(die.body);
			//}
		}
	}
	removeDiceFromArray(hiddenDie, scalingDice);
	removeDiceFromArray(hiddenDie, dice);
}

function getNonGreenHueShift() {
	let hueShift = Math.floor(Math.random() * 360);
	var tryCount: number = 0;
	while (hueShift > 30 && hueShift < 160 && tryCount++ < 20) {
		hueShift = Math.floor(Math.random() * 360);
	}
	return hueShift;
}

function hideDieIn(dieObject: any, ms: number) {
	dieObject.hideTime = performance.now() + ms;
	dieObject.needToHideDie = true;
}

const hiddenDieScale: number = 0.01;

function hideDie(dieObject: any) {
	dieObject.scale.set(hiddenDieScale, hiddenDieScale, hiddenDieScale);
	dieObject.isHidden = true;
	clearAllDiceIfHidden();
}

function clearAllDiceIfHidden() {
	for (var i = 0; i < dice.length; i++) {
		let die: any = dice[i];
		let dieObject = die.getObject();
		if (!dieObject.isHidden)
			return;
	}
	clearAllDice();
}

function removeDieEffects() {
	for (var i = 0; i < dice.length; i++) {
		let die: any = dice[i];
		removeDieEffectsForSingleDie(die);
	}
}

function removeDieEffectsForSingleDie(die: any) {
	var now: number = performance.now();
	if (die.attachedSprites)
		for (var i = 0; i < die.attachedSprites.length; i++) {
			let sprite: SpriteProxy = die.attachedSprites[i];
			const fadeOutTime = 1200;
			sprite.expirationDate = now + fadeOutTime;
			sprite.fadeOutTime = fadeOutTime;
		}
	die.attachedSprites = [];
	die.origins = [];
}

function highlightSpecialDice() {
	if (!specialDice || specialDice.length == 0)
		return;


	let now: number = performance.now();

	var hiddenDie = [];
	for (var i = 0; i < specialDice.length; i++) {
		let dieObject = specialDice[i].getObject();

		if (dieObject.needToHideDie) {
			if (dieObject.hideTime < now) {
				dieObject.needToHideDie = false;
				hideDie(dieObject);
				hiddenDie.push(specialDice[i]);
			}
		}

		if (dieObject.needToStartEffect) {
			let effectStartTime: number = dieObject.removeTime + dieObject.effectStartOffset;
			if (now > effectStartTime && dieObject.needToStartEffect) {

				dieObject.needToStartEffect = false;

				// die.dieValue is also available.
				let screenPos: Vector = getScreenCoordinates(dieObject);
				if (!screenPos)
					continue;

				if (dieObject.effectKind === DieEffect.Lucky) {
					diceLayer.addLuckyRing(screenPos.x, screenPos.y);
				}
				if (dieObject.effectKind === DieEffect.Ring) {
					diceLayer.addMagicRing(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
				}
				else if (dieObject.effectKind === DieEffect.Fireball) {
					//diceLayer.addD20Fire(screenPos.x, screenPos.y);
					diceLayer.addFireball(screenPos.x, screenPos.y);
					diceSounds.playFireball();
				}
				else if (dieObject.effectKind === DieEffect.Bomb) {
					diceLayer.addDiceBomb(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
					diceSounds.playDieBomb();
					hideDieIn(dieObject, 700);
				}
				else if (dieObject.effectKind === DieEffect.SteamPunkTunnel) {
					diceLayer.addSteampunkTunnel(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
					//diceLayer.playSteampunkTunnel();
					//hideDieIn(die, 700);
				}
				else if (dieObject.effectKind === DieEffect.HandGrab) {
					diceLayer.testDiceGrab(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
					//diceSounds.playHandGrab();
					//hideDieIn(die, 41 * 30);
				}
				else if (dieObject.effectKind === DieEffect.ColoredSmoke) {
					let saturation: number = 100;
					let brightnessBase: number = 50;
					let rand: number = Math.random() * 100;
					if (rand < 5) {
						saturation = 0;
						brightnessBase = 110;
					}
					else if (rand < 10) {
						saturation = 25;
						brightnessBase = 100;
					}
					else if (rand < 25) {
						saturation = 50;
						brightnessBase = 90;
					}
					else if (rand < 75) {
						saturation = 75;
						brightnessBase = 70;
					}

					diceLayer.blowColoredSmoke(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), saturation, brightnessBase + Math.random() * 80);
					hideDie(dieObject);
					hiddenDie.push(specialDice[i]);
					diceSounds.playDiceBlow();
				}
			}
		}
	}
	removeDiceFromArray(hiddenDie, specialDice);
	removeDiceFromArray(hiddenDie, dice);
}

function removeSingleDieFromArray(dieToRemove: any, list: any) {
	for (var i = 0; i < list.length; i++) {
		if (dieToRemove == list[i]) {
			list.splice(i, 1);
			return;
		}
	}
}

function removeDiceFromArray(dieToRemove: any, list: any) {
	for (var i = 0; i < dieToRemove; i++) {
		removeSingleDieFromArray(dieToRemove[i], list);
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

	if (!animationsShouldBeDone && diceRemainingInPlay() == 0 && allDiceHaveStoppedRolling &&
		(scalingDice == null || scalingDice.length == 0) &&
		(specialDice == null || specialDice.length == 0)) {
		animationsShouldBeDone = true;
		//console.log('animationsShouldBeDone = true;');
		diceRollData = null;
	}
}

function diceRemainingInPlay(): number {
	var count: number = 0;
	for (var i in dice) {
		if (dice[i].inPlay) {
			count++;
		}
	}
	return count;
}



var animationsShouldBeDone: boolean;

function prepareDie(die: any, throwPower: number, xPositionModifier: number = 0) {
	prepareBaseDie(die, throwPower, xPositionModifier);

	var newValue: number = Math.floor(Math.random() * die.values + 1);
	diceValues.push({ dice: die, value: newValue });
}

function prepareBaseDie(die: any, throwPower: number, xPositionModifier: number = 0) {
	let dieObject = die.getObject();
	scene.add(dieObject);
	die.inPlay = true;
	die.attachedSprites = [];
	die.origins = [];
	dice.push(die);

	die.hasNotHitFloorYet = true;

	let index: number = dice.length;
	let yVelocityModifier: number = Math.random() * 10 * throwPower;
	dieObject.position.x = xPositionModifier + -15 - (index % 3) * dieScale;
	dieObject.position.y = 4 + Math.floor(index / 3) * dieScale;
	dieObject.position.z = -13 + (index % 3) * dieScale;
	dieObject.quaternion.x = (Math.random() * 90 - 45) * Math.PI / 180;
	dieObject.quaternion.z = (Math.random() * 90 - 45) * Math.PI / 180;
	let xVelocityMultiplier: number = 1;
	if (xPositionModifier != 0)
		xVelocityMultiplier = -1;
	die.updateBodyFromMesh();
	let xVelocityModifier: number = Math.random() * 20 * throwPower;
	let zVelocityModifier: number = Math.random() * 20 * throwPower;
	dieObject.body.velocity.set(xVelocityMultiplier * (35 + xVelocityModifier), 10 + yVelocityModifier, 25 + zVelocityModifier);
	let angularModifierLimit: number = 20 * throwPower;
	let angularModifierHalfLimit: number = angularModifierLimit / 2;
	let xAngularRotationModifier: number = Math.random() * angularModifierLimit;
	let yAngularRotationModifier: number = Math.random() * angularModifierLimit;
	let zAngularRotationModifier: number = Math.random() * angularModifierLimit;
	dieObject.body.angularVelocity.set(xAngularRotationModifier - angularModifierHalfLimit, yAngularRotationModifier - angularModifierHalfLimit, zAngularRotationModifier - angularModifierHalfLimit);
	dieObject.body.name = 'die';
	dieObject.body.addEventListener("collide", handleDieCollision);
	die.lastPos = [];
	die.lastPos.push(new Vector(-100, -100));
	die.lastPawPrintOnLeft = false;
}

function prepareD10x10Die(die: any, throwPower: number, xPositionModifier: number = 0) {
	prepareBaseDie(die, throwPower, xPositionModifier);
	var newValue: number = Math.floor(Math.random() * die.values) + 1;
	diceValues.push({ dice: die, value: newValue });
}

function prepareD10x01Die(die: any, throwPower: number, xPositionModifier: number = 0) {
	prepareBaseDie(die, throwPower, xPositionModifier);
	var newValue: number = Math.floor(Math.random() * die.values) + 1;
	diceValues.push({ dice: die, value: newValue });
}

function clearBeforeRoll() {
	diceLayer.clearResidualEffects();
	scalingDice = [];
	specialDice = [];
	restoreDieScale();
	setNormalGravity();
	waitingForSettle = true;
	setupBonusRoll = false;
	waitingForBonusRollToComplete = false;
	onBonusThrow = false;
	diceValues = [];
	totalDamagePlusModifier = 0;
	totalHealthPlusModifier = 0;
	totalExtraPlusModifier = 0;
	totalRoll = 0;
	d20RollValue = -1;
	totalBonus = 0;
	damageModifierThisRoll = 0;
	//console.log('damageModifierThisRoll = 0;');
	healthModifierThisRoll = 0;
	extraModifierThisRoll = 0;
	clearAllDice();
	allDiceHaveStoppedRolling = false;
}

function queueRoll(diceRollData: DiceRollData) {
	console.log('queueRoll - TODO');
	// TODO: queue this roll for later when the current roll has stopped.
}

function addD100(diceRollData: DiceRollData, backgroundColor: string, textColor: string, throwPower: number = 1, xPositionModifier: number = 0) {

	// @ts-ignore - DiceD10x10
	var die = new DiceD10x10({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
	prepareD10x10Die(die, throwPower, xPositionModifier);
	die.rollType = DieCountsAs.totalScore;
	if (diceRollData.isMagic) {
		die.attachedSprites.push(diceLayer.addMagicRing(960, 540, Math.floor(Math.random() * 360), 100, 100));
		die.origins.push(new Vector(diceLayer.magicRing.originX, diceLayer.magicRing.originY));
	}

	// @ts-ignore - DiceD10x01
	die = new DiceD10x01({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
	prepareD10x01Die(die, throwPower, xPositionModifier);
	die.rollType = DieCountsAs.totalScore;
	if (diceRollData.isMagic) {
		die.attachedSprites.push(diceLayer.addMagicRing(960, 540, Math.floor(Math.random() * 360), 100, 100));
		die.origins.push(new Vector(diceLayer.magicRing.originX, diceLayer.magicRing.originY));
	}
}

function addDie(dieStr: string, damageType: DamageType, rollType: DieCountsAs, backgroundColor: string, textColor: string, throwPower: number = 1, xPositionModifier: number = 0, isMagic: boolean = false, playerID: number = -1): any {
	let countPlusDie: string[] = dieStr.split('d');
	if (countPlusDie.length != 2)
		throw new Error(`Issue with die format string: "${dieStr}". Unable to throw dice.`);
	let count: number = 1;
	if (countPlusDie[0])
		count = +countPlusDie[0];
	let dieKind: string = countPlusDie[1];
	let lastDieAdded: any = null;
	for (var i = 0; i < count; i++) {
		// @ts-ignore - DiceD4
		let die: DiceObject = null;
		switch (dieKind) {
			case '4':
				// @ts-ignore - DiceD4
				die = new DiceD4({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case '6':
				// @ts-ignore - DiceD6
				die = new DiceD6({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case '8':
				// @ts-ignore - DiceD8
				die = new DiceD8({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case '10':
				// @ts-ignore - DiceD10
				die = new DiceD10({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case '1001':
				// @ts-ignore - DiceD10x01
				die = new DiceD10x01({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case '1010':
				// @ts-ignore - DiceD10x01
				die = new DiceD10x10({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case '12':
				// @ts-ignore - DiceD12
				die = new DiceD12({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case '20':
				// @ts-ignore - DiceD20
				die = new DiceD20({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
		}
		lastDieAdded = die;
		if (die === null) {
			throw new Error(`Die not found: "${dieStr}". Unable to throw dice.`);
		}
		die.playerID = playerID;
		prepareDie(die, throwPower, xPositionModifier);

		if (die) {
			if (rollType == DieCountsAs.damage || rollType == DieCountsAs.bonus) {
				switch (damageType) {
					case DamageType.Fire:
						diceLayer.attachDamageFire(die);
						die.damageStr = 'fire';
						break;
					case DamageType.Cold:
						diceLayer.attachDamageCold(die);
						die.damageStr = 'cold';
						break;
					case DamageType.Necrotic:
						diceLayer.attachDamageNecrotic(die);
						die.damageStr = 'necrotic';
						break;
					case DamageType.Acid:
						diceLayer.attachDamageAcid(die);
						die.damageStr = 'acid';
						break;
					case DamageType.Piercing:
						diceLayer.attachDamagePiercing(die);
						die.damageStr = 'piercing';
						break;
					case DamageType.Radiant:
						diceLayer.attachDamageRadiant(die);
						die.damageStr = 'radiant';
						break;
					case DamageType.Poison:
						diceLayer.attachDamagePoison(die);
						die.damageStr = 'poison';
						break;
					case DamageType.Slashing:
						diceLayer.attachDamageSlashing(die);
						die.damageStr = 'slashing';
						break;
					case DamageType.Thunder:
						diceLayer.attachDamageThunder(die);
						die.damageStr = 'thunder';
						break;
					case DamageType.Force:
						diceLayer.attachDamageForce(die);
						die.damageStr = 'force';
						break;
					case DamageType.Psychic:
						diceLayer.attachDamagePsychic(die);
						die.damageStr = 'psychic';
						break;
					case DamageType.Bludgeoning:
						diceLayer.attachDamageBludgeoning(die);
						die.damageStr = 'bludgeoning';
						break;
					case DamageType.Lightning:
						diceLayer.attachDamageLightning(die);
						die.damageStr = 'lightning';
						break;
					case DamageType.Superiority:
						diceLayer.attachSuperiority(die);
						die.damageStr = 'superiority';
						break;
				}
			}
			else if ((rollType == DieCountsAs.health || rollType == DieCountsAs.totalScore) && damageType == DamageType.Superiority) {
				diceLayer.attachSuperiority(die);
			}
			if (rollType == DieCountsAs.health) {
				diceLayer.attachHealth(die);
			}
		}

		die.rollType = rollType;

		if (die.rollType === DieCountsAs.inspiration) {
			let rotation: number = Random.between(4, 8);
			let hueShift: number = Random.between(20, 30);
			addInspirationParticles(die, playerID, hueShift, rotation);
			addInspirationParticles(die, playerID, -hueShift, -rotation);

			die.attachedSprites.push(diceLayer.addInspirationSmoke(960, 540, Math.floor(Math.random() * 360)));
			die.origins.push(diceLayer.inspirationSmoke.getOrigin());
		}

		if (isMagic) {
			die.attachedSprites.push(diceLayer.addMagicRing(960, 540, Math.floor(Math.random() * 360), 100, 100));
			die.origins.push(diceLayer.magicRing.getOrigin());
		}
	}
	return lastDieAdded;
}

function addInspirationParticles(die: any, playerID: number, hueShiftOffset: number, rotationDegeesPerSecond: number) {
	die.attachedSprites.push(diceLayer.addInspirationParticles(960, 540, rotationDegeesPerSecond, diceLayer.getHueShift(playerID) + hueShiftOffset));
	die.origins.push(diceLayer.inspirationParticles.getOrigin());
}

enum DieCountsAs {
	totalScore,
	inspiration,
	damage,
	health,
	extra,
	bonus,
	bentLuck
}

function addDieFromStr(playerID: number, diceStr: string, dieCountsAs: DieCountsAs, throwPower: number, xPositionModifier: number = 0, backgroundColor: string = undefined, fontColor: string = undefined, isMagic: boolean = false): any {
	if (!diceStr)
		return;
	let allDice: string[] = diceStr.split(',');
	if (backgroundColor === undefined)
		backgroundColor = DiceLayer.damageDieBackgroundColor;
	if (fontColor === undefined)
		fontColor = DiceLayer.damageDieFontColor;
	let modifier: number = 0;
	let damageType: DamageType = diceRollData.damageType;
	allDice.forEach(function (dieSpec: string) {
		let thisBackgroundColor: string = backgroundColor;
		let thisFontColor: string = fontColor;
		let thisDieCountsAs: DieCountsAs = dieCountsAs;
		let parenIndex: number = dieSpec.indexOf('(');
		if (parenIndex >= 0) {
			var damageStr: string = dieSpec.substring(parenIndex + 1);
			let closeParenIndex: number = damageStr.indexOf(')');
			if (closeParenIndex >= 0) {
				damageStr = damageStr.substr(0, closeParenIndex);
				let colonIndex: number = damageStr.indexOf(':');
				if (colonIndex >= 0) {
					let rollTypeOverride: string = damageStr.substr(colonIndex + 1).toLowerCase();
					//console.log('rollTypeOverride: ' + rollTypeOverride);
					if (rollTypeOverride == 'health') {
						thisDieCountsAs = DieCountsAs.health;
						thisBackgroundColor = DiceLayer.healthDieBackgroundColor;
						thisFontColor = DiceLayer.healthDieFontColor;
					}
					else if (rollTypeOverride == 'damage') {
						thisDieCountsAs = DieCountsAs.damage;
						thisBackgroundColor = DiceLayer.damageDieBackgroundColor;
						thisFontColor = DiceLayer.damageDieFontColor;
					}
					else if (rollTypeOverride == 'roll') {
						thisDieCountsAs = DieCountsAs.totalScore;
						thisBackgroundColor = diceLayer.activePlayerDieColor;
						thisFontColor = diceLayer.activePlayerDieFontColor;
					}
					damageStr = damageStr.substr(0, colonIndex);
					console.log('damageStr: ' + damageStr);
				}


				damageType = damageTypeFromStr(damageStr);
			}
			dieSpec = dieSpec.substr(0, parenIndex);
		}

		let dieAndModifier = dieSpec.split('+');
		if (dieAndModifier.length == 2)
			modifier += +dieAndModifier[1];
		let dieStr: string = dieAndModifier[0];
		addDie(dieStr, damageType, thisDieCountsAs, thisBackgroundColor, thisFontColor, throwPower, xPositionModifier, isMagic, playerID);
	});

	damageModifierThisRoll += modifier;
	//console.log(`damageModifierThisRoll += modifier; (${damageModifierThisRoll})`);
	healthModifierThisRoll += modifier;
	extraModifierThisRoll += modifier;
}

function damageTypeFromStr(str: string): DamageType {
	str = str.toLowerCase();
	if (str === 'acid')
		return DamageType.Acid;

	if (str === 'bludgeoning')
		return DamageType.Bludgeoning;

	if (str === 'cold')
		return DamageType.Cold;

	if (str === 'fire')
		return DamageType.Fire;

	if (str === 'force')
		return DamageType.Force;

	if (str === 'lightning')
		return DamageType.Lightning;

	if (str === 'superiority')
		return DamageType.Superiority;

	if (str === 'necrotic')
		return DamageType.Necrotic;

	if (str === 'piercing')
		return DamageType.Piercing;

	if (str === 'poison')
		return DamageType.Poison;

	if (str === 'psychic')
		return DamageType.Psychic;

	if (str === 'radiant')
		return DamageType.Radiant;

	if (str === 'slashing')
		return DamageType.Slashing;

	if (str === 'thunder')
		return DamageType.Thunder;

	return DamageType.None;
}

function update() {
	controls.update();
	if (stats) {
		stats.update();
	}
}

function render() {
	renderer.render(scene, camera);

	updateDieRollSpecialEffects();

	diceLayer.renderCanvas();
}

init();

function updateDieRollSpecialEffects() {
	for (var i = 0; i < dice.length; i++) {
		let die = dice[i];

		if (die.rollType == DieCountsAs.bentLuck)
			addTrailingEffects(die, diceRollData.secondRollData.trailingEffects);
		else
			addTrailingEffects(die, diceRollData.trailingEffects);

		if (die.rollType == DieCountsAs.inspiration) {
			const distanceBetweenRipples: number = 80;
			let ripple: ColorShiftingSpriteProxy = <ColorShiftingSpriteProxy>positionTrailingSprite(die, diceLayer.addRipple.bind(diceLayer), distanceBetweenRipples, 0, diceRollData.trailingEffects.length);

			if (ripple) {
				ripple.opacity = 0.5;
				ripple.hueShift = diceLayer.getHueShift(die.playerID) + Random.plusMinus(30);
			}
		}

		if (die.attachedSprites && die.attachedSprites.length > 0) {
			let screenPos: Vector = getScreenCoordinates(die.getObject());
			for (var j = 0; j < die.attachedSprites.length; j++) {
				let centerX: number = screenPos.x - die.origins[j].x;
				let centerY: number = screenPos.y - die.origins[j].y;
				let sprite: SpriteProxy = die.attachedSprites[j];
				sprite.x = centerX;
				sprite.y = centerY;
			}
		}
		if (die.sparks) {
			let screenPos: Vector = getScreenCoordinates(die.getObject());
			let newX: number = screenPos.x - diceLayer.diceSparks.originX;
			let newY: number = screenPos.y - diceLayer.diceSparks.originY;
			die.sparks.forEach(function (spark: SpriteProxy) {
				const newLocationWeight = 0.1;
				spark.x = spark.x * (1 - newLocationWeight) + newX * newLocationWeight;
				spark.y = spark.y * (1 - newLocationWeight) + newY * newLocationWeight;
			});
		}
	}
}

function addTrailingEffects(die: any, trailingEffects: Array<TrailingEffect>) {
	for (var j = 0; j < trailingEffects.length; j++) {
		let trailingEffect: TrailingEffect = trailingEffects[j];
		let addSpriteFunc: any;
		if (trailingEffect.Type === SpriteType.PawPrint)
			addSpriteFunc = diceLayer.addPawPrint.bind(diceLayer);
		else if (trailingEffect.Type === SpriteType.Raven)
			addSpriteFunc = diceLayer.addRaven.bind(diceLayer);
		else if (trailingEffect.Type === SpriteType.Smoke)
			addSpriteFunc = diceLayer.addPuff.bind(diceLayer);
		else if (trailingEffect.Type === SpriteType.SparkTrail)
			addSpriteFunc = diceLayer.addSparkTrail.bind(diceLayer);
		else if (trailingEffect.Type === SpriteType.SmallSparks)
			addSpriteFunc = diceLayer.smallSpark.bind(diceLayer);
		else if (trailingEffect.Type === SpriteType.Fangs)
			addSpriteFunc = diceLayer.addFangs.bind(diceLayer);
		else if (trailingEffect.Type === SpriteType.Spiral)
			addSpriteFunc = diceLayer.addSpiral.bind(diceLayer);
		else
			continue;

		if (positionTrailingSprite(die, addSpriteFunc, trailingEffect.MinForwardDistanceBetweenPrints,
			trailingEffect.LeftRightDistanceBetweenPrints, j)) {
			if (trailingEffect.OnPrintPlaySound) {
				if (trailingEffect.intervalBetweenSounds == 0)
					trailingEffect.intervalBetweenSounds = trailingEffect.MinSoundInterval + Random.plusMinus(trailingEffect.PlusMinusSoundInterval);
				if (diceSounds.playRandom(trailingEffect.OnPrintPlaySound, trailingEffect.NumRandomSounds,
					trailingEffect.intervalBetweenSounds)) {
					trailingEffect.intervalBetweenSounds = trailingEffect.MinSoundInterval + Random.plusMinus(trailingEffect.PlusMinusSoundInterval);
				}
			}
		}
	}
}

var rollingOnlyAddOnDice: boolean;

function needToRollAddOnDice(diceRollDto: DiceRollData, bentLuckMultiplier: number = 0) {
	if (diceRollData && diceRollData.secondRollData)
		return;

	if (diceRollData == null) {
		diceRollData = diceRollDto;
		rollingOnlyAddOnDice = true;
	}

	diceRollData.bentLuckMultiplier = bentLuckMultiplier;
	diceRollData.secondRollData = diceRollDto;
	diceRollData.secondRollData.vantageKind == diceRollData.vantageKind;

	secondRollTryCount = 0;
	throwAdditionalDice();
}

var secondRollTryCount: number = 0;

function throwAdditionalDice() {
	secondRollTryCount++;

	// @ts-ignore - DiceManager
	if (!rollingOnlyAddOnDice && !allDiceHaveStoppedRolling && secondRollTryCount < 30) {
		setTimeout(throwAdditionalDice, 300);
		return;
	}

	freezeExistingDice();
	diceLayer.clearTextEffects();

	let localDiceRollData: DiceRollData = getMostRecentDiceRollData(diceRollData);

	if (isLuckBent(localDiceRollData))
		diceLayer.reportAddOnRoll(diceRollData.secondRollTitle, diceRollData.bentLuckMultiplier);
	else {
		if (!diceRollData.damageHealthExtraDice)
			localDiceRollData.itsAD20Roll = true;
		else if (localDiceRollData.type == DiceRollType.LuckRollHigh || localDiceRollData.type == DiceRollType.LuckRollLow)
			localDiceRollData.itsAD20Roll = true;

		diceLayer.reportAddOnRoll(diceRollData.secondRollTitle, diceRollData.bentLuckMultiplier);
	}

	diceSounds.safePlayMp3('PaladinThunder');
	if (rollingOnlyAddOnDice)
		rollAddOnDice();
	else
		setTimeout(rollAddOnDice, 2500);
}

function rollAddOnDice() {
	if (!diceRollData)
		return;

	allDiceHaveStoppedRolling = false;

	let xPositionModifier: number = 0;

	if (Math.random() * 100 < 50)
		xPositionModifier = 26;  // Throw from the right to the left.

	let localDiceRollData: DiceRollData;

	localDiceRollData = getMostRecentDiceRollData(localDiceRollData);

	let die: any;
	let throwPower: number = diceRollData.throwPower * 1.2;

	var dieBack: string;
	var dieFont: string;

	if (diceRollData.bentLuckMultiplier < 0) {
		// Bad luck
		dieBack = DiceLayer.badLuckDieColor;
		dieFont = DiceLayer.badLuckFontColor;
	}
	else {
		dieBack = DiceLayer.goodLuckDieColor;
		dieFont = DiceLayer.goodLuckFontColor;
	}

	if (isLuckBent(localDiceRollData)) {
		die = addDie('d4', DamageType.None, DieCountsAs.bentLuck, dieBack, dieFont, throwPower, xPositionModifier, false);
		die.isLucky = true;
	}
	else {
		if (localDiceRollData.itsAD20Roll) {   // TODO: Send itsAD20Roll from DM app.
			die = addD20(localDiceRollData, dieBack, dieFont, xPositionModifier);
			die.isLucky = true;  // TODO: Send IsLucky from DM app.
		}
		else {
			//let saveDamageModifier: number = damageModifierThisRoll;
			addDieFromStr(localDiceRollData.playerRollOptions[0].PlayerID, localDiceRollData.damageHealthExtraDice, DieCountsAs.totalScore, throwPower);
			//damageModifierThisRoll += saveDamageModifier;
		}
	}


	const isGoodLuck: boolean = isLuckBent(localDiceRollData) && diceRollData.bentLuckMultiplier > 0 || localDiceRollData.type == DiceRollType.LuckRollHigh;
	const isBadLuck: boolean = isLuckBent(localDiceRollData) && diceRollData.bentLuckMultiplier < 0 || localDiceRollData.type == DiceRollType.LuckRollLow;

	if (isGoodLuck) {
		//console.log('addGoodLuckEffects...');
		addGoodLuckEffects(die);
	}
	else if (isBadLuck) {
		//console.log('addBadLuckEffects...');
		addBadLuckEffects(die);
	}
}

function getMostRecentDiceRollData(localDiceRollData: DiceRollData) {
	if (diceRollData.secondRollData)
		localDiceRollData = diceRollData.secondRollData;
	else
		localDiceRollData = diceRollData;
	return localDiceRollData;
}

function isLuckBent(localDiceRollData: DiceRollData) {
	return localDiceRollData.type == DiceRollType.BendLuckAdd || localDiceRollData.type == DiceRollType.BendLuckSubtract;
}

class MockDie {
	constructor(public name: string, public topNumber: number) {
		this.isD20 = true;
		this.inPlay = true;
	}
	isD20: boolean;
	inPlay: boolean;
	getTopNumber(): number {
		return this.topNumber;
	}
	getObject(): any {
		return null;
	}
}

var testing: boolean = false;

function getDiceInPlay(): number {
	let diceInPlay: number = 0;
	for (var i = 0; i < dice.length; i++) {
		let die = dice[i];
		if (die.inPlay) {
			diceInPlay++;
		}
	}

	return diceInPlay;
}

function testD20Removal(roll1: number, roll2: number, roll3: number, vantage: VantageKind, type: DiceRollType): MockDie {
	dice = [];
	diceRollData.vantageKind = vantage;
	diceRollData.type = type;
	diceRollData.itsAD20Roll = true;
	dice.push(new MockDie('A', roll1));
	dice.push(new MockDie('B', roll2));

	let diceInPlay: number = getDiceInPlay();

	if (diceInPlay != 2)
		console.error('a) diceInPlay: ' + diceInPlay);
	removeD20s();
	diceInPlay = getDiceInPlay();
	if (diceInPlay != 1)
		console.error('b) diceInPlay: ' + diceInPlay);

	dice.push(new MockDie('C', roll3));
	removeD20s();
	diceInPlay = 0;
	let returnDie: MockDie;
	for (var i = 0; i < dice.length; i++) {
		let die = dice[i];
		if (die.inPlay) {
			diceInPlay++;
			returnDie = die;
		}
	}
	if (diceInPlay != 1)
		console.error('c) diceInPlay: ' + diceInPlay);
	return returnDie;
}

function addD20(diceRollData: DiceRollData, d20BackColor: string, d20FontColor: string, xPositionModifier: number, playerID: number = -1) {
	// @ts-ignore - DiceD20
	var die = new DiceD20({ size: dieScale, backColor: d20BackColor, fontColor: d20FontColor });
	die.isD20 = true;
	prepareDie(die, diceRollData.throwPower, xPositionModifier);
	die.rollType = DieCountsAs.totalScore;
	die.playerID = playerID;
	return die;
}

function addBadLuckEffects(die: any) {
	die.attachedSprites.push(diceLayer.addBadLuckRing(960, 540));
	die.origins.push(new Vector(diceLayer.badLuckRing.originX, diceLayer.badLuckRing.originY));
}

function addGoodLuckEffects(die: any) {
	die.attachedSprites.push(diceLayer.addLuckyRing(960, 540));
	die.origins.push(new Vector(diceLayer.cloverRing.originX, diceLayer.cloverRing.originY));
}

function removeMultiplayerD20s(): any {
	//console.log('removeMultiplayerD20s...');
	needToClearD20s = false;
	let localDiceRollData: DiceRollData = getMostRecentDiceRollData(diceRollData);
	const isWildMagic: boolean = localDiceRollData.type == DiceRollType.WildMagic;
	if (isWildMagic || !localDiceRollData.itsAD20Roll)
		return;
	const vantageTextDelay = 900;
	//console.log('diceRollData.appliedVantage: ' + diceRollData.appliedVantage);

	let playerEdgeRolls: Array<number> = [];
	let otherPlayersDie: Array<any> = [];
	for (var j = 0; j < diceLayer.players.length; j++) {
		playerEdgeRolls.push(-1);
		otherPlayersDie.push(null);
	}

	for (var i = 0; i < dice.length; i++) {
		let die = dice[i];
		let playerId: number = die.playerID;
		let topNumber = die.getTopNumber();

		if (die.isD20) {
			if (playerEdgeRolls[playerId] == -1)
				playerEdgeRolls[playerId] = topNumber;
			else if (die.kind == VantageKind.Advantage) {
				if (playerEdgeRolls[playerId] <= topNumber) {
					removeNonVantageDieNow(otherPlayersDie[playerId]);
					diceLayer.addAdvantageText(otherPlayersDie[playerId], vantageTextDelay);
					playerEdgeRolls[playerId] = topNumber;
				}
				else {  // Disadvantage
					removeNonVantageDieNow(die);
					diceLayer.addAdvantageText(die, vantageTextDelay);
				}
			}
			else if (die.kind == VantageKind.Disadvantage) {
				if (playerEdgeRolls[playerId] >= topNumber) {
					removeNonVantageDieNow(otherPlayersDie[playerId]);
					diceLayer.addDisadvantageText(otherPlayersDie[playerId], vantageTextDelay);
					playerEdgeRolls[playerId] = topNumber;
				}
				else {
					removeNonVantageDieNow(die);
					diceLayer.addDisadvantageText(die, vantageTextDelay);
				}
			}
			otherPlayersDie[playerId] = die;
		}
	}
}

function removeD20s(): number {
	//console.log('removeD20s...');
	needToClearD20s = false;
	let edgeRollValue: number = -1;
	let localDiceRollData: DiceRollData = getMostRecentDiceRollData(diceRollData);
	const isLuckRollHigh: boolean = localDiceRollData.type == DiceRollType.LuckRollHigh;
	const isLuckRollLow: boolean = localDiceRollData.type == DiceRollType.LuckRollLow;
	const isLuckRoll: boolean = isLuckRollHigh || isLuckRollLow;
	const isWildMagic: boolean = localDiceRollData.type == DiceRollType.WildMagic;
	if (isWildMagic || !localDiceRollData.itsAD20Roll)
		return edgeRollValue;
	let otherDie: any = null;
	const vantageTextDelay = 900;
	//console.log('diceRollData.appliedVantage: ' + diceRollData.appliedVantage);

	if (!diceRollData.appliedVantage) {
		for (var i = 0; i < dice.length; i++) {
			let die = dice[i];
			if (die.isLucky)
				continue;

			// TODO: Check behavior of Sorcerer's luck and bent luck rolls and see if we need to change those.
			//const isNormal: boolean = die.kind == DiceRollKind.Normal;
			//if (isNormal && !isLuckRoll)
			//	continue;

			let topNumber = die.getTopNumber();

			if (die.isD20) {
				if (edgeRollValue == -1)
					edgeRollValue = topNumber;
				else if (die.kind == VantageKind.Advantage) {
					if (edgeRollValue <= topNumber) {
						removeNonVantageDieNow(otherDie);
						diceLayer.addAdvantageText(otherDie, vantageTextDelay);
						edgeRollValue = topNumber;
					}
					else {  // Disadvantage
						removeNonVantageDieNow(die);
						diceLayer.addAdvantageText(die, vantageTextDelay);
					}
				}
				else if (die.kind == VantageKind.Disadvantage) {
					if (edgeRollValue >= topNumber) {
						removeNonVantageDieNow(otherDie);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						diceLayer.addDisadvantageText(otherDie, vantageTextDelay);
						//}
						edgeRollValue = topNumber;
					}
					else {
						removeNonVantageDieNow(die);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						diceLayer.addDisadvantageText(die, vantageTextDelay);
						//}
					}
				}
				otherDie = die;
			}
		}
	}

	if (isLuckRoll) {
		edgeRollValue = -1;
		otherDie = null;
		for (var i = 0; i < dice.length; i++) {
			let die = dice[i];
			if (!die.inPlay)
				continue;
			let topNumber = die.getTopNumber();

			if (die.isD20) {
				if (edgeRollValue == -1)
					edgeRollValue = topNumber;
				else if (isLuckRollHigh) {
					if (edgeRollValue <= topNumber) {
						removeNonVantageDieNow(otherDie);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						if (!otherDie.isLucky)
							diceLayer.addAdvantageText(otherDie, vantageTextDelay, true);
						//}
						edgeRollValue = topNumber;
					}
					else {
						removeNonVantageDieNow(die);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						if (!die.isLucky)
							diceLayer.addAdvantageText(die, vantageTextDelay, true);
						//}
					}
				}
				else if (isLuckRollLow) {
					if (edgeRollValue >= topNumber) {
						removeNonVantageDieNow(otherDie);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						if (!otherDie.isLucky)
							diceLayer.addDisadvantageText(otherDie, vantageTextDelay, true);
						//}
						edgeRollValue = topNumber;
					}
					else {
						removeNonVantageDieNow(die);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						if (!die.isLucky)
							diceLayer.addDisadvantageText(die, vantageTextDelay, true);
						//}
					}
				}
				otherDie = die;
			}
		}
	}
	return edgeRollValue;
}

var needToClearD20s: boolean = true;


function onDiceRollStopped() {
	onBonusThrow = false;
	setupBonusRoll = false;
	needToClearD20s = true;
	allDiceHaveStoppedRolling = true;
	//console.log('Dice have stopped rolling!');
	//diceHaveStoppedRolling(null);

	if (diceRollData.type == DiceRollType.WildMagic) {
		diceSounds.playRandom('WildMagicFinale/drum', 18);
	}

	if (attemptedRollWasSuccessful) {
		onSuccess();
	}
	else {
		onFailure();
	}

	let diceData = {
		'playerID': diceLayer.playerID,
		'success': attemptedRollWasSuccessful,
		'roll': totalRoll,
		'hiddenThreshold': diceRollData.hiddenThreshold,
		'damage': totalDamagePlusModifier,
		'health': totalHealthPlusModifier,
		'extra': totalExtraPlusModifier,
		'multiplayerSummary': diceRollData.multiplayerSummary,
		'type': diceRollData.type,
		'skillCheck': diceRollData.skillCheck,
		'savingThrow': diceRollData.savingThrow,
		'bonus': totalBonus,
	};

	diceHaveStoppedRolling(JSON.stringify(diceData));

	if (removeDiceImmediately)
		removeRemainingDice();
}

function onSuccess() {

}

function onFailure() {

}

function isAttack(diceRollData: DiceRollData): boolean {
	return diceRollData.type == DiceRollType.Attack || diceRollData.type == DiceRollType.ChaosBolt;
}

function checkStillRolling() {
	if (allDiceHaveStoppedRolling)
		return;

	if (anyDiceStillRolling()) {
		waitingForSettle = true;
		return;
	}

	if (setupBonusRoll && !waitingForBonusRollToComplete) {
		waitingForSettle = true;
		return;
	}

	let now: number = performance.now();

	if (waitingForBonusRollToComplete && bonusRollStartTime != 0) {
		if (now - bonusRollStartTime < 1000) {
			return;
		}
		waitingForBonusRollToComplete = false;
		setupBonusRoll = false;
	}

	if (waitingForSettle) {
		waitingForSettle = false;
		firstStopTime = now;
	}
	else {
		let thisTime: number = now;
		if ((thisTime - firstStopTime) / 1000 > 1.5) {
			if (needToClearD20s) {
				if (diceRollData.hasMultiPlayerDice) {
					removeMultiplayerD20s();
				}
				else {
					d20RollValue = removeD20s();
				}
				showSpecialLabels();
			}

			if (needToRollBonusDice()) {
				if (!setupBonusRoll) {
					setupBonusRoll = true;
					//if (diceRollData.type == DiceRollType.WildMagic)
					//	showRollTotal();
					if (!diceRollData.startedBonusDiceRoll) {
						freezeExistingDice();
						diceRollData.startedBonusDiceRoll = true;
						if (isAttack(diceRollData) && d20RollValue >= diceRollData.minCrit)
							diceLayer.indicateBonusRoll('Damage Bonus!');
						else
							diceLayer.indicateBonusRoll('Bonus Roll!');
						setTimeout(rollBonusDice, 2500);
					}
				}
			}
			else {
				showSpecialLabels(true);
				popFrozenDice();
				reportRollResults(getRollResults());
				if (diceRollData.playBonusSoundAfter)
					setTimeout(playFinalRollSoundEffects, diceRollData.playBonusSoundAfter);
				onDiceRollStopped();
			}
		}
	}
}

function showSpecialLabels(onlyBonusDice: boolean = false) {
	/*
			1 / Acid
			2 / Cold
			3 / Fire
			4 / Force
			5 / Lightning
			6 / Poison
			7 / Psychic
			8 / Thunder 
	*/

	if (diceRollData.type == DiceRollType.ChaosBolt) {
		for (var i = 0; i < dice.length; i++) {
			let die = dice[i];
			if (die.rollType == DieCountsAs.damage && die.attachedDamage !== true) {
				let topNumber = die.getTopNumber();
				var message: string = '';
				switch (topNumber) {
					case 1:
						message = 'Acid';
						diceLayer.attachDamageAcid(die);
						break;
					case 2:
						message = 'Cold';
						diceLayer.attachDamageCold(die);
						break;
					case 3:
						message = 'Fire';
						diceLayer.attachDamageFire(die);
						break;
					case 4:
						message = 'Force';
						diceLayer.attachDamageForce(die);
						break;
					case 5:
						message = 'Lightning';
						diceLayer.attachDamageLightning(die);
						break;
					case 6:
						message = 'Poison';
						diceLayer.attachDamagePoison(die);
						break;
					case 7:
						message = 'Psychic';
						diceLayer.attachDamagePsychic(die);
						break;
					case 8:
						message = 'Thunder';
						diceLayer.attachDamageThunder(die);
						break;
				}
				if (message) {
					diceLayer.addDieTextAfter(die, message, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, 900, 7000);
				}
			}
		}
	}
}

function getModifier(diceRollData: DiceRollData, player: Character): number {
	if (player)
		if (diceRollData.type === DiceRollType.Initiative)
			return player.initiative;
		else if (diceRollData.type === DiceRollType.SkillCheck) {
			return player.getSkillMod(diceRollData.skillCheck);
		}
		else if (diceRollData.type === DiceRollType.SavingThrow) {
			return player.getSavingThrowMod(diceRollData.savingThrow);
		}
	return 0;
}

function getRollResults(): RollResults {
	let d20RollValue: Array<number> = [0, 0, 0, 0, 0, 0, 0, 0, 0];
	let inspirationValue: Array<number> = [0, 0, 0, 0, 0, 0, 0, 0, 0];
	let luckValue: Array<number> = [0, 0, 0, 0, 0, 0, 0, 0, 0];
	let totalDamage: number = 0;
	let maxDamage: number = 0;
	let totalHealth: number = 0;
	let totalExtra: number = 0;

	let singlePlayerId: number = 0;
	let playerIdForTextMessages: number = -1;

	//console.log(`getRollResults - dice.length = ${dice.length}`);
	for (var i = 0; i < dice.length; i++) {
		let die = dice[i];
		if (!die.inPlay)
			continue;

		let topNumber = die.getTopNumber();

		if (diceRollData.hasMultiPlayerDice) {
			if (diceRollData.multiplayerSummary == null)
				diceRollData.multiplayerSummary = [];
			let playerRoll: PlayerRoll = diceRollData.multiplayerSummary.find((value, index, obj) => value.playerId == die.playerID);
			if (playerRoll)
				playerRoll.roll += topNumber;
			else {
				let modifier: number = 0;
				if (diceLayer.players && diceLayer.players.length > 0) {
					let player: Character = diceLayer.getPlayer(die.playerID);
					modifier = getModifier(diceRollData, player);
				}

				diceRollData.multiplayerSummary.push(new PlayerRoll(topNumber, die.playerName, die.playerID, modifier));
			}
			let scaleAdjust: number = 1;
			if (die.rollType === DieCountsAs.inspiration) {
				scaleAdjust = 0.7;
			}

			if (!die.playerName) {
				die.playerName = diceLayer.getPlayerName(die.playerID);
			}

			diceLayer.addDieTextAfter(die, die.playerName, diceLayer.getDieColor(die.playerID), diceLayer.activePlayerDieFontColor, 0, 8000, scaleAdjust);
		}

		let playerID: number;
		if (die.playerID === undefined || die.playerID < 0)
			continue;

		playerID = die.playerID;
		if (diceRollData.hasSingleIndividual) {
			singlePlayerId = playerID;
			playerIdForTextMessages = playerID;
		}

		if (!d20RollValue[playerID])
			d20RollValue[playerID] = 0;

		if (!inspirationValue[playerID])
			inspirationValue[playerID] = 0;

		if (!luckValue[playerID])
			luckValue[playerID] = 0;

		switch (die.rollType) {
			case DieCountsAs.totalScore:
				//console.log(`DieCountsAs.totalScore (${topNumber})`);
				d20RollValue[playerID] += topNumber;
				break;
			case DieCountsAs.inspiration:
				//console.log(`DieCountsAs.inspiration (${topNumber})`);
				inspirationValue[playerID] += topNumber;
				break;
			case DieCountsAs.bentLuck:
				//console.log(`DieCountsAs.bentLuck (${topNumber * diceRollData.bentLuckMultiplier})`);
				luckValue[playerID] += topNumber * diceRollData.bentLuckMultiplier;
				break;
			case DieCountsAs.bonus:
				//console.log(`DieCountsAs.bonus (${topNumber})`);
				totalBonus += topNumber;
				break;
			case DieCountsAs.damage:
				//console.log(`DieCountsAs.damage (${topNumber})`);
				totalDamage += topNumber;
				maxDamage += die.values;
				break;
			case DieCountsAs.health:
				//console.log(`DieCountsAs.health (${topNumber})`);
				totalHealth += topNumber;
				break;
			case DieCountsAs.extra:
				//console.log(`DieCountsAs.extra (${topNumber})`);
				totalExtra += topNumber;
				break;
		}
	} // for

	let skillSavingModifier: number = 0;
	if (diceRollData.type == DiceRollType.SkillCheck || diceRollData.type == DiceRollType.SavingThrow) {
		let player: Character = diceLayer.getPlayer(singlePlayerId);
		skillSavingModifier = getModifier(diceRollData, player);
	}

	totalRoll = d20RollValue[singlePlayerId] + inspirationValue[singlePlayerId] + luckValue[singlePlayerId] + diceRollData.modifier + skillSavingModifier;

	if (!diceRollData.hasMultiPlayerDice && d20RollValue[singlePlayerId] > 0) {
		if (diceRollData.type == DiceRollType.SkillCheck)
			totalRoll += totalBonus;
	}

	modifyTotalRollForTestingPurposes();

	attemptedRollWasSuccessful = totalRoll >= diceRollData.hiddenThreshold;
	attemptedRollWasNarrowlySuccessful = attemptedRollWasSuccessful && (totalRoll - diceRollData.hiddenThreshold < 2);
	//console.log('damageModifierThisRoll: ' + damageModifierThisRoll);
	totalDamagePlusModifier = totalDamage + damageModifierThisRoll;
	totalHealthPlusModifier = totalHealth + healthModifierThisRoll;
	totalExtraPlusModifier = totalExtra + extraModifierThisRoll;

	return new RollResults(d20RollValue, singlePlayerId, luckValue, playerIdForTextMessages, skillSavingModifier, totalDamage, totalHealth, totalExtra, maxDamage);
}

class RollResults {
	constructor(public d20RollValue: number[], public singlePlayerId: number, public luckValue: number[],
		public playerIdForTextMessages: number, public skillSavingModifier: number, public totalDamage: number,
		public totalHealth: number, public totalExtra: number, public maxDamage: number) {
	}
}

function reportRollResults(rollResults: RollResults) {
	let d20RollValue: number[] = rollResults.d20RollValue;
	let singlePlayerId: number = rollResults.singlePlayerId;
	let luckValue: number[] = rollResults.luckValue;
	let playerIdForTextMessages: number = rollResults.playerIdForTextMessages;
	let skillSavingModifier: number = rollResults.skillSavingModifier;
	let totalDamage: number = rollResults.totalDamage;
	let totalHealth: number = rollResults.totalHealth;
	let totalExtra: number = rollResults.totalExtra;
	let maxDamage: number = rollResults.maxDamage;

	let title: string = '';
	if (diceRollData.type == DiceRollType.Initiative)
		title = 'Initiative:';
	else if (diceRollData.type == DiceRollType.NonCombatInitiative)
		title = 'Non-combat Initiative:';
	else if (diceRollData.type == DiceRollType.SkillCheck)
		title = `${getSkillCheckName()} Check:`;
	else if (diceRollData.type == DiceRollType.SavingThrow)
		title = `${getSavingThrowName()} Saving Throw:`;

	if (diceRollData.multiplayerSummary) {
		diceRollData.multiplayerSummary.sort((a, b) => (b.roll + b.modifier) - (a.roll + a.modifier));
		diceLayer.showMultiplayerResults(title, diceRollData.multiplayerSummary, diceRollData.hiddenThreshold);
	}

	if (!diceRollData.hasMultiPlayerDice && d20RollValue[singlePlayerId] > 0) {
		if (diceRollData.modifier != 0)
			diceLayer.showRollModifier(diceRollData.modifier, luckValue[singlePlayerId], playerIdForTextMessages);
		if (skillSavingModifier != 0)
			diceLayer.showRollModifier(skillSavingModifier, luckValue[singlePlayerId], playerIdForTextMessages);
		diceLayer.showDieTotal(`${totalRoll}`, playerIdForTextMessages);
	}
	if (totalBonus > 0 && !diceRollData.hasMultiPlayerDice && diceRollData.type != DiceRollType.SkillCheck) {
		let bonusRollStr: string = 'Bonus Roll: ';
		let bonusRollOverrideStr: string = diceRollData.getFirstBonusRollDescription();
		if (bonusRollOverrideStr)
			bonusRollStr = bonusRollOverrideStr;
		switch (diceRollData.wildMagic) {
			case WildMagic.heightChange:
			case WildMagic.ageChange:
				if (isOdd(totalBonus))
					totalBonus = -totalBonus;
				break;
		}
		diceLayer.showBonusRoll(`${bonusRollStr}${totalBonus}`, DiceLayer.bonusRollFontColor, DiceLayer.bonusRollDieColor);
	}

	if (totalDamage > 0) {
		diceLayer.showTotalHealthDamage(totalDamagePlusModifier.toString(), attemptedRollWasSuccessful, 'Damage: ', DiceLayer.damageDieBackgroundColor, DiceLayer.damageDieFontColor);
		diceLayer.showDamageHealthModifier(damageModifierThisRoll, attemptedRollWasSuccessful, DiceLayer.damageDieBackgroundColor, DiceLayer.damageDieFontColor);
	}
	if (totalHealth > 0) {
		diceLayer.showTotalHealthDamage('+' + totalHealthPlusModifier.toString(), attemptedRollWasSuccessful, 'Health: ', DiceLayer.healthDieBackgroundColor, DiceLayer.healthDieFontColor);
		diceLayer.showDamageHealthModifier(healthModifierThisRoll, attemptedRollWasSuccessful, DiceLayer.healthDieBackgroundColor, DiceLayer.healthDieFontColor);
	}
	if (totalExtra > 0) {
		diceLayer.showTotalHealthDamage(totalExtraPlusModifier.toString(), attemptedRollWasSuccessful, '', DiceLayer.extraDieBackgroundColor, DiceLayer.extraDieFontColor);
		diceLayer.showDamageHealthModifier(extraModifierThisRoll, attemptedRollWasSuccessful, DiceLayer.extraDieBackgroundColor, DiceLayer.extraDieFontColor);
	}
	showSuccessFailMessages(title, d20RollValue[singlePlayerId]);
	maxDamage += damageModifierThisRoll;
	if (diceRollData.secondRollData)
		playSecondaryAnnouncerCommentary(diceRollData.secondRollData.type, d20RollValue[singlePlayerId], totalDamagePlusModifier, maxDamage);
	else
		playAnnouncerCommentary(diceRollData.type, d20RollValue[singlePlayerId], totalDamagePlusModifier, maxDamage);
	return maxDamage;
}

function playSecondaryAnnouncerCommentary(type: DiceRollType, d20RollValue: number, totalDamage: number = 0, maxDamage: number = 0): void {
	// TODO: Implement this.
}

function playAnnouncerCommentary(type: DiceRollType, d20RollValue: number, totalDamage: number = 0, maxDamage: number = 0): void {
	if (diceRollData.hasMultiPlayerDice) {
		diceSounds.playMultiplayerCommentary(type, d20RollValue);
		return;
	}
	if (diceRollData.type == DiceRollType.WildMagic) {
		diceSounds.playWildMagicCommentary(type, d20RollValue);
		return;
	}

	if (diceRollData.type == DiceRollType.PercentageRoll) {
		diceSounds.playPercentageRollCommentary(type, d20RollValue);
		return;
	}

	if (diceRollData.type == DiceRollType.DamageOnly) {
		diceSounds.playDamageCommentary(type, d20RollValue, totalDamage, maxDamage);
		return;
	}

	if (diceRollData.type == DiceRollType.HealthOnly) {
		diceSounds.playHealthCommentary(type, d20RollValue);
		return;
	}

	if (diceRollData.type == DiceRollType.ExtraOnly) {
		diceSounds.playExtraCommentary(type, d20RollValue);
		return;
	}

	if (diceRollData.type == DiceRollType.Attack) {
		diceSounds.playAttackCommentary(d20RollValue, totalDamage, maxDamage);
		return;
	}

	if (diceRollData.type == DiceRollType.SkillCheck) {
		diceSounds.playSkillCheckCommentary(d20RollValue, diceRollData.skillCheck);
		return;
	}

	if (diceRollData.type == DiceRollType.SavingThrow) {
		diceSounds.playSavingThrowCommentary(d20RollValue, diceRollData.savingThrow);
		return;
	}

	if (diceRollData.type == DiceRollType.ChaosBolt) {
		diceSounds.playChaosBoltCommentary(d20RollValue, diceRollData.savingThrow);
		return;
	}

	if (diceRollData.type == DiceRollType.FlatD20) {
		diceSounds.playFlatD20Commentary(d20RollValue);
		return;
	}

	if (diceRollData.type == DiceRollType.WildMagicD20Check) {
		diceSounds.playWildMagicD20CheckCommentary(d20RollValue);
		return;
	}
}

function getSkillCheckName() {
	let enumAsStr: string = Object.keys(Skills).find(key => Skills[key] === diceRollData.skillCheck);
	let initialCapEnum: string = '';
	if (enumAsStr)
		initialCapEnum = enumAsStr.charAt(0).toUpperCase() + enumAsStr.slice(1);
	else
		initialCapEnum = 'Skill';
	if (diceRollData.skillCheck === Skills.animalHandling)
		initialCapEnum = 'Animal Handling';
	else if (diceRollData.skillCheck === Skills.slightOfHand)
		initialCapEnum = 'Slight of Hand';
	return initialCapEnum;
}

function getSavingThrowName() {
	let enumAsStr: string = Object.keys(Ability).find(key => Ability[key] === diceRollData.savingThrow);
	let initialCapEnum: string = '';
	if (enumAsStr)
		initialCapEnum = enumAsStr.charAt(0).toUpperCase() + enumAsStr.slice(1);
	else
		initialCapEnum = 'Ability';
	return initialCapEnum;
}

function showSuccessFailMessages(title: string, rawD20RollValue: number) {
	if (title)
		title += ' ';
	if (!diceRollData.hasMultiPlayerDice && diceRollData.type != DiceRollType.WildMagic &&
		diceRollData.type != DiceRollType.PercentageRoll &&
		diceRollData.type != DiceRollType.DamageOnly &&
		diceRollData.type != DiceRollType.HealthOnly &&
		diceRollData.type != DiceRollType.ExtraOnly) {
		if (attemptedRollWasSuccessful)
			if (rawD20RollValue >= diceRollData.minCrit) {
				diceLayer.showResult(title + diceRollData.critSuccessMessage, attemptedRollWasSuccessful);
			}
			else {
				diceLayer.showResult(title + diceRollData.successMessage, attemptedRollWasSuccessful);
			}
		else if (rawD20RollValue == 1)
			diceLayer.showResult(title + diceRollData.critFailMessage, attemptedRollWasSuccessful);
		else
			diceLayer.showResult(title + diceRollData.failMessage, attemptedRollWasSuccessful);
	}
}

function pleaseRollDice(diceRollDto: DiceRollData) {
	//testing = true;
	//diceRollData = diceRollDto;

	//if (testD20Removal(3, 2, 1, DiceRollKind.Disadvantage, DiceRollType.LuckRollHigh).name != 'A') debugger;
	//if (testD20Removal(4, 5, 7, DiceRollKind.Disadvantage, DiceRollType.LuckRollLow).name != 'B') debugger;
	//return;

	animationsShouldBeDone = false;
	rollingOnlyAddOnDice = false;


	if (diceRollDto.type == DiceRollType.BendLuckAdd || diceRollDto.type == DiceRollType.LuckRollHigh) {
		needToRollAddOnDice(diceRollDto, +1);
		return;
	}
	else if (diceRollDto.type == DiceRollType.BendLuckSubtract || diceRollDto.type == DiceRollType.LuckRollLow) {
		needToRollAddOnDice(diceRollDto, -1);
		return;
	}
	else if (diceRollDto.type == DiceRollType.AddOnDice) {
		needToRollAddOnDice(diceRollDto, 0);
		return;
	}

	diceRollData = diceRollDto;
	diceRollData.timeLastRolledMs = performance.now();
	attemptedRollWasSuccessful = false;
	attemptedRollWasNarrowlySuccessful = false;

	if (randomDiceThrowIntervalId != 0) {
		clearInterval(randomDiceThrowIntervalId);
		randomDiceThrowIntervalId = 0;
	}

	// @ts-ignore - DiceManager
	if (DiceManager.throwRunning) {
		queueRoll(diceRollData);
		return;
	}

	let xPositionModifier: number = 0;

	if (Math.random() * 100 < 50)
		xPositionModifier = 26;  // Throw from the right to the left.

	clearBeforeRoll();

	let playerID: number = -1;
	if (diceRollData.playerRollOptions.length == 1)
		playerID = diceRollData.playerRollOptions[0].PlayerID;

	if (diceRollData.type == DiceRollType.WildMagic) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		addD100(diceRollData, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, diceRollData.throwPower, xPositionModifier);
	}
	else if (diceRollData.type == DiceRollType.Initiative || diceRollData.type == DiceRollType.NonCombatInitiative) {
		diceRollData.modifier = 0;
		diceRollData.maxInspirationDiceAllowed = 4;

		diceRollData.itsAD20Roll = true;
		for (var i = 0; i < diceLayer.players.length; i++) {
			let player: Character = diceLayer.players[i];
			let initiativeBonus: number;
			if (diceRollData.type == DiceRollType.NonCombatInitiative)
				initiativeBonus = 0;
			else
				initiativeBonus = player.rollInitiative;

			addDiceForPlayer(player.playerID, xPositionModifier, initiativeBonus, player.inspiration);
		}

		diceRollData.hasMultiPlayerDice = true;
	}
	else if (diceRollData.type == DiceRollType.DamageOnly) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		addDieFromStr(playerID, diceRollData.damageHealthExtraDice, DieCountsAs.damage, diceRollData.throwPower, xPositionModifier);
	}
	else if (diceRollData.type == DiceRollType.HealthOnly) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		addDieFromStr(playerID, diceRollData.damageHealthExtraDice, DieCountsAs.health, diceRollData.throwPower, xPositionModifier, DiceLayer.healthDieBackgroundColor, DiceLayer.healthDieFontColor);
	}
	else if (diceRollData.type == DiceRollType.ExtraOnly) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		addDieFromStr(playerID, diceRollData.damageHealthExtraDice, DieCountsAs.extra, diceRollData.throwPower, xPositionModifier, DiceLayer.extraDieBackgroundColor, DiceLayer.extraDieFontColor);
	}
	else if (diceRollData.type == DiceRollType.InspirationOnly) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		for (var i = 0; i < diceRollData.playerRollOptions.length; i++) {
			let playerRollOptions: PlayerRollOptions = diceRollData.playerRollOptions[i];
			addDiceForPlayer(playerRollOptions.PlayerID, xPositionModifier, playerRollOptions.VantageKind, playerRollOptions.Inspiration, 0);
		}
	}
	else {
		diceRollData.itsAD20Roll = true;

		if (diceRollData.rollScope == RollScope.ActivePlayer) {
			let activePlayerRollOptions: PlayerRollOptions = diceRollData.playerRollOptions[0];
			// TODO: I think there's a bug here active player's inspiration needs to be used.
			let vantageKind: VantageKind = diceRollData.vantageKind;
			if (diceRollData.playerRollOptions.length == 1) {
				vantageKind = activePlayerRollOptions.VantageKind;
			}
			addDiceForPlayer(activePlayerRollOptions.PlayerID, xPositionModifier, vantageKind, diceRollData.groupInspiration);
			diceRollData.hasSingleIndividual = true;
		}
		else if (diceRollData.rollScope == RollScope.Individuals) {
			diceRollData.playerRollOptions.forEach(function (playerRollOption: PlayerRollOptions) {
				addDiceForPlayer(playerRollOption.PlayerID, xPositionModifier, playerRollOption.VantageKind, playerRollOption.Inspiration);
			});
			diceRollData.hasMultiPlayerDice = diceRollData.playerRollOptions.length > 1;
			diceRollData.hasSingleIndividual = diceRollData.playerRollOptions.length == 1;
		}

		if (isAttack(diceRollData)) {
			addDieFromStr(playerID, diceRollData.damageHealthExtraDice, DieCountsAs.damage, diceRollData.throwPower, xPositionModifier);
		}
	}

	try {
		// @ts-ignore - DiceManager
		DiceManager.prepareValues(diceValues);
	}
	catch (ex) {
		console.log('exception on call to DiceManager.prepareValues: ' + ex);
	}

	if (diceRollData.type == DiceRollType.WildMagic) {
		diceSounds.safePlayMp3('WildMagicRoll');
	}
	if (diceRollData.isPaladinSmiteAttack) {
		diceSounds.safePlayMp3('PaladinThunder');
	}
	if (diceRollData.isSneakAttack) {
		diceSounds.safePlayMp3('SneakAttackWhoosh');
	}

	//startedRoll = true;
}

function addDiceForPlayer(playerID: number, xPositionModifier: number, kind: VantageKind, inspiration: string = '', numD20s: number = 1) {
	let d20BackColor: string = diceLayer.getDieColor(playerID);
	let d20FontColor: string = diceLayer.getDieFontColor(playerID);
	if (kind !== VantageKind.Normal)
		numD20s = 2;

	for (var i = 0; i < numD20s; i++) {
		var die = addD20(diceRollData, d20BackColor, d20FontColor, xPositionModifier);
		die.playerID = playerID;
		die.playerName = diceLayer.getPlayerName(playerID);
		die.kind = kind;
		if (diceRollData.isMagic) {
			die.attachedSprites.push(diceLayer.addMagicRing(960, 540, Math.floor(Math.random() * 360), 100, 100));
			die.origins.push(new Vector(diceLayer.magicRing.originX, diceLayer.magicRing.originY));
		}
		if (diceRollData.numHalos > 0) {
			let angleDelta: number = 360 / diceRollData.numHalos;
			let angle: number = Math.random() * 360;
			for (var j = 0; j < diceRollData.numHalos; j++) {
				die.attachedSprites.push(diceLayer.addHaloSpin(960, 540, diceLayer.activePlayerHueShift + Random.plusMinus(30), angle));
				die.origins.push(diceLayer.haloSpins.getOrigin());
				angle += angleDelta;
			}
		}
	}

	if (inspiration) {
		addDieFromStr(playerID, inspiration, DieCountsAs.inspiration, diceRollData.throwPower, xPositionModifier, d20BackColor, d20FontColor, diceRollData.isMagic);
	}
}

class PlayerRoll {
	constructor(public roll: number, public name: string, public playerId: number, public modifier: number = 0) {

	}
}