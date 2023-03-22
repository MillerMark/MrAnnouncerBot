var gravityGames: GravityGames;
var myRocket: Rocket;

class DroneGame extends GamePlusQuiz {
	//emitter: Emitter;
	//pinkSeeds: Sprites;
	yellowSeeds: Sprites;
	//purpleSeeds: Sprites;
	sparkSmoke: Sprites;
	//beesYellow: Sprites;
	redExplosions: Sprites;
	//blueExplosions: Sprites;
	//purpleExplosions: Sprites;
	//redFlowers: Sprites;
	yellowPetunias: Sprites;
	superFlowerBack: Sprites;
	superFlowerFront: Sprites;
	//yellowFlowers3: Sprites;
	//purpleFlowers: Sprites;
	purplePortals: Sprites;
	portalBackground: Sprites;
	backgroundBanner: Part;
	coins: Sprites;
	animations: Animations = new Animations();
  lastCommand: string;

	constructor(context: CanvasRenderingContext2D) {
		super(context);
  }

  //`![Meteor](5F8B49E97A5F459E6434A11E7FD272BE.png)
  addMeteors() {
    this.allMeteors = new SpriteCollection();

    const meteorFrameInterval: number = 38;
    this.redMeteors = new Sprites("Spinning Rock/Red/Meteor", 63, meteorFrameInterval, AnimationStyle.Loop, false, addMeteorExplosion);
    this.redMeteors.moves = true;

    this.blueMeteors = new Sprites("Spinning Rock/Blue/Meteor", 63, meteorFrameInterval, AnimationStyle.Loop, false, addMeteorExplosion);
    this.blueMeteors.moves = true;

    this.purpleMeteors = new Sprites("Spinning Rock/Purple/Meteor", 63, meteorFrameInterval, AnimationStyle.Loop, false, addMeteorExplosion);
    this.purpleMeteors.moves = true;

    this.allMeteors.add(this.redMeteors);
    this.allMeteors.add(this.blueMeteors);
    this.allMeteors.add(this.purpleMeteors);
  }

  //` ![](2B073D0DC3C289F9E5723CAB5FD45014.png;;;0.02717,0.02717)
  allWalls: SpriteCollection;
  horizontalSolidWall: WallSprites;
  verticalSolidWall: WallSprites;
  horizontalDoubleWall: WallSprites;
  verticalDoubleWall: WallSprites;
  horizontalDashedWall: WallSprites;
  verticalDashedWall: WallSprites;
  endCaps: Sprites;

  //` ![](F8F205C07F3FCE8EEA5341ED98A92B36.png)  ![](77833405FDDE3ACB175756288F7BE0F8.png)
  loadDrones(color: string): Sprites {
    const drones = new Sprites(`Drones/${color}/Drone`, 30, fps30, AnimationStyle.Loop);
    drones.segmentSize = 2;
    drones.removeOnHitFloor = false;
    drones.moves = true;
    this.allDrones.add(drones);
    return drones;
  }

  loadSmoke(): Sprites {
    const smoke = new Sprites(`Smoke/Smoke`, 150, fps47, AnimationStyle.CenterLoop, true);
    smoke.segmentSize = 47;
    smoke.returnFrameIndex = 30;
    smoke.resumeFrameIndex = 77;
    smoke.originX = 60;
    smoke.originY = 61;
    smoke.moves = true;
    smoke.disableGravity();
    smoke.verticalThrustOverride = -0.15;
    return smoke;
  }



	updateScreen(context: CanvasRenderingContext2D, nowMs: number) {
		super.updateScreen(context, nowMs);
		this.purplePortals.spriteProxies.forEach((portal: SpriteProxy) => {
			if (portal instanceof Portal) {
				portal.checkApproaching(this.allDrones, nowMs);
			}
		});

		//this.emitter.update(now);
		myRocket.updatePosition(nowMs);
		myRocket.bounce(0, 0, screenWidth, screenHeight, nowMs);

		this.collectCoins(nowMs);

		this.allSeeds.bounce(0, 0, screenWidth, screenHeight, nowMs);

		//this.beesYellow.bounce(0, 0, screenWidth, screenHeight, now);
		this.allDrones.bounce(0, 0, screenWidth, screenHeight, nowMs);
		//greenSeeds.bounce(0, 0, screenWidth, screenHeight, now);

		this.allMeteors.checkCollisionAgainstSprites(this.boomboxes, this.voteSongDown, nowMs);
		this.allMeteors.bounce(0, 0, screenWidth, screenHeight, nowMs);
		this.allMeteors.checkCollisionAgainst(this.allDrones, this.putMeteorOnDrone, nowMs);
		//console.log('this.allMeteors.checkCollisionAgainstSprites(this.boomboxes...');

		//backgroundBanner.draw(myContext, 0, 0);

		if (!myRocket.isDocked)
			gravityGames.draw(myContext);

		this.allSplats.draw(myContext, nowMs);
		this.portalBackground.draw(myContext, nowMs);
		this.purplePortals.draw(myContext, nowMs);

		//grass1.draw(myContext, now);
		//grass2.draw(myContext, now);
		//grass3.draw(myContext, now);
		//grass4.draw(myContext, now);

		//this.beesYellow.updatePositions(now);
    this.smokeSprites.updatePositions(nowMs);
		this.allDrones.updatePositions(nowMs);
		this.allMeteors.updatePositions(nowMs);
		this.allWalls.updatePositions(nowMs);
		this.endCaps.updatePositions(nowMs);
		this.allSeeds.updatePositions(nowMs);
		this.allSparks.updatePositions(nowMs);

		this.wallBounce(nowMs);

		//this.beesYellow.draw(myContext, now);
		this.allWalls.draw(myContext, nowMs);
		this.droneGateways.draw(myContext, nowMs);
		this.warpIns.draw(myContext, nowMs);
    this.boomboxes.draw(myContext, nowMs);
    this.smokeSprites.draw(myContext, nowMs);
		this.allDrones.draw(myContext, nowMs);
		this.allMeteors.draw(myContext, nowMs);

		this.endCaps.draw(myContext, nowMs);

		this.allSeeds.draw(myContext, nowMs);
		myRocket.draw(myContext, nowMs);
		//this.purpleFlowers.draw(myContext, now);
		//blueFlowers.draw(myContext, now);
		//this.redFlowers.draw(myContext, now);
		this.yellowPetunias.draw(myContext, nowMs);
		this.superFlowerBack.draw(myContext, nowMs);
		this.superFlowerFront.draw(myContext, nowMs);
		//yellowFlowers2.draw(myContext, now);
		//this.yellowFlowers3.draw(myContext, now);
		this.redExplosions.draw(myContext, nowMs);
		//this.blueExplosions.draw(myContext, now);
		//this.purpleExplosions.draw(myContext, now);
		this.droneExplosions.draw(myContext, nowMs);
		//explosion.draw(myContext, 0, 0);
		this.allSparks.draw(myContext, nowMs);
		this.sparkSmoke.draw(myContext, nowMs);
		this.coins.draw(myContext, nowMs);

		//this.emitter.draw(myContext, now);
		//drawCrossHairs(myContext, 830, 540);
		//drawCrossHairs(myContext, 830 + 340, 540);
		this.animations.removeExpiredAnimations(nowMs);
    this.animations.render(myContext, nowMs);

    if (this.lastCommand)
    {
      myContext.textAlign = 'left';
      myContext.textBaseline = 'top';
      myContext.font = '20px Arial';
      myContext.fillStyle = '#fff';
      myContext.strokeStyle = '#000';
      myContext.fillText(this.lastCommand, 0, 0);
      myContext.strokeText(this.lastCommand, 0, 0);
    }
	}

	getBoomboxInstance(): Boombox {
		if (this.boomboxes && this.boomboxes.spriteProxies && this.boomboxes.spriteProxies.length > 0)
			return <Boombox>this.boomboxes.spriteProxies[0];
		return null;
	}


	removeAllGameElements(now: number): void {
		super.removeAllGameElements(now);
		this.allWalls.destroyAllBy(4000);
		this.portalBackground.destroyAllBy(0);
		this.purplePortals.destroyAllBy(4000);
		this.droneGateways.destroyAllBy(4000);
	}

	initialize() {
		super.initialize();
		Folders.assets = 'GameDev/Assets/DroneGame/';

		myRocket = new Rocket(0, 0);
		myRocket.x = 0;
		myRocket.y = 0;

		gravityGames = new GravityGames();
	}

	start() {
		super.start();
		gravityGames.selectPlanet('Earth');
		gravityGames.newGame();
	}

	loadResources(): void {
		//this.emitter = new Emitter(new Vector(screenCenterX, screenCenterY));
		//this.emitter.radius = 80;
		//this.emitter.particleRadius = 10;
		//this.emitter.particleRadiusVariance = 0.5;
		//this.emitter.addParticles(20);

		super.loadResources();

		this.coins = new Sprites("Spinning Coin/32x32/SpinningCoin", 59, 15, AnimationStyle.Loop, true, null,
			this.outlineGameSurfaceBeastBuild
			/* this.outlineGameSurfaceNoAdsNoMark */
			/* outlineGameSurface outlineChatRoom allButMark outlineCodeEditor */ /* fillChatRoom */);

		this.addSeeds();

		loadSoundEffects();

		//this.beesYellow = new Sprites("Bees/Yellow/BeeYellow", 18, 15, AnimationStyle.Loop);
		//this.beesYellow.segmentSize = 2;
		//this.beesYellow.removeOnHitFloor = false;
		//this.beesYellow.moves = true;

		this.loadWarpInAnimation();

		this.addDrones();

		this.addSplats();

		this.addMeteors();

		this.loadAllSparks();

		this.redExplosions = new Sprites("Explosion/Red/Explosion", 179, 5, AnimationStyle.Sequential);
		this.boomboxes = new Sprites("Boombox/Boombox", 13, 1000 / 15, AnimationStyle.Loop, true);
		this.boomboxes.originX = 87;
		this.boomboxes.originY = 37;
		//this.blueExplosions = new Sprites("Explosion/Blue/Explosion", 179, 5, AnimationStyle.Sequential);
		//this.purpleExplosions = new Sprites("Explosion/Purple/Explosion", 179, 5, AnimationStyle.Sequential);

		this.loadWalls();
		this.loadDroneExplosions();
		this.droneHealthLights = new Sprites('Drones/192x90/Indicator', 60, 0, AnimationStyle.Static);

		const smokeFrameInterval: number = 40;
		this.sparkSmoke = new Sprites(`FireWall/Smoke/Smoke`, 45, smokeFrameInterval, AnimationStyle.Sequential, true);
		this.sparkSmoke.originX = 121;
		this.sparkSmoke.originY = 170;

		globalBypassFrameSkip = true;
		//this.redFlowers = new Sprites("Flowers/Red/RedFlower", 293, 15, AnimationStyle.Loop, true);
		//this.redFlowers.returnFrameIndex = 128;

		const flowerFrameRate: number = 20;
		//const grassFrameRate: number = 25;

		this.yellowPetunias = new Sprites("Flowers/YellowPetunias1/YellowPetunias", 270, flowerFrameRate, AnimationStyle.Loop, true);
		this.yellowPetunias.returnFrameIndex = 64;
		this.yellowPetunias.originX = 133;
		this.yellowPetunias.originY = 227;

		this.superFlowerBack = new Sprites("Flowers/SuperFlower/SuperFlowerBack", 290, flowerFrameRate, AnimationStyle.Loop, true);
		this.superFlowerBack.returnFrameIndex = 99;
		this.superFlowerBack.originX = 160;
		this.superFlowerBack.originY = 277;

		this.superFlowerFront = new Sprites("Flowers/SuperFlower/SuperFlowerFront", 290, flowerFrameRate, AnimationStyle.Loop, true);
		this.superFlowerFront.returnFrameIndex = 99;
		this.superFlowerFront.originX = 160;
		this.superFlowerFront.originY = 277;

		//var yellowFlowers2 = new Sprites("Flowers/YellowPetunias2/YellowPetunias", 270, flowerFrameRate, AnimationStyle.Loop, true);
		//yellowFlowers2.returnFrameIndex = 64;
		//this.yellowFlowers3 = new Sprites("Flowers/YellowPetunias3/YellowPetunias", 253, flowerFrameRate, AnimationStyle.Loop, true);
		//this.yellowFlowers3.returnFrameIndex = 45;

		//this.purpleFlowers = new Sprites("Flowers/Purple/PurpleFlower", 320, flowerFrameRate, AnimationStyle.Loop, true);
		//this.purpleFlowers.returnFrameIndex = 151;

		this.droneGateways = new Gateways("Drones/Warp Gate/WarpGate", 73, 45, AnimationStyle.Loop, true);

		const portalFrameRate = 40;
		this.purplePortals = new Sprites("Portal/Purple/Portal", 82, portalFrameRate, AnimationStyle.Loop, true);
		this.portalBackground = new Sprites("Portal/Black Back/Back", 13, portalFrameRate, AnimationStyle.SequentialStop, true);
		this.purplePortals.returnFrameIndex = 13;

		globalBypassFrameSkip = false;

		Part.loadSprites = true;

		this.backgroundBanner = new Part("CodeRushedBanner", 1, AnimationStyle.Static, 200, 300);
	}

  executeCommand(command: string, params: string, userInfo: UserInfo, now: number): boolean {
    //this.lastCommand = command;
		if (super.executeCommand(command, params, userInfo, now))
			return true;

		if (command === "Swat") {
			this.destroyAllDronesOverMark();
		}
		else if (command === "Launch") {
			if (!myRocket.started || myRocket.isDocked) {
				myRocket.started = true;
				myRocket.launch(now);
				gravityGames.newGame();
				chat('Launching...');
			}
		}
		else if (command === "Dock") {
			if (this.isSuperUser(userInfo.userName)) {
				this.selfDestructAllDrones();
				this.removeAllGameElements(now);
			}

			if (myRocket.started && !myRocket.isDocked) {
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
			myRocket.releaseBee(now, params, userInfo.userId, userInfo.displayName, userInfo.color);
		}
		else if (command === "Drone") {
			const gatewayNum: number = +params;
			needToGetCoins(userInfo.userId);
      if (gatewayNum)
        this.droneGateways.releaseDrone(now, userInfo.userId, userInfo.displayName, userInfo.color, userInfo.profileImageUrl, gatewayNum);
      else
        myRocket.releaseDrone(now, userInfo.userId, userInfo.displayName, userInfo.color, userInfo.profileImageUrl);
		}
		else if (command === "MoveRelative") {
			this.moveRelative(now, params, userInfo.userId);
		}
		else if (command === "MoveAbsolute") {
			this.moveAbsolute(now, params, userInfo.userId);
		}
		else if (command === "red" || command === "orange" || command === "amber" || command === "yellow" ||
			command === "green" || command === "cyan" || command === "blue" || command === "indigo"
			|| command === "violet" || command === "magenta" || command === "black" || command === "white") {
      this.paint(userInfo.userId, command, params);
    }

    else if (command === "SmokeOn") {
      this.smokeOn(userInfo.userId, params, now);
    }

    else if (command === "SmokeOff") {
      this.smokeOff(userInfo.userId);
    }

    else if (command === "SmokeColor") {
      this.smokeColor(userInfo.userId, params);
    }


    else if (command === "SmokeLifetime") {
      this.smokeLifetime(userInfo.userId, params);
    }

		else if (command === "ChangeDroneVelocity") {
			this.changeDroneVelocity(userInfo.userId, params);
		}
		else if (command === "FlyTo") {
			this.flyTo(userInfo.userId, params);
		}
		else if (command === "DroneUp") {
			this.droneUp(userInfo.userId, params);
		}
		else if (command === "DroneDown") {
			this.droneDown(userInfo.userId, params);
		}
		else if (command === "DroneLeft") {
			this.droneLeft(userInfo.userId, params);
		}
		else if (command === "DroneRight") {
			this.droneRight(userInfo.userId, params);
		}
		else if (command === "Toss") {
			this.tossMeteor(userInfo.userId, params);
		}
		else if (command === 'Music') {
			if (params === 'off')
				this.stopMusic();
			else if (params.startsWith('supress'))
				this.stopMusic();
			else
				this.startMusic();
		}
		else if (command === 'Genre') {
			Boombox.changeGenre(params);
		}
		else if (command.startsWith('Volume')) {
			let volStr: string = params;
			if (volStr === '')
				Boombox.reportVolume();
			else {
				volStr = volStr.trim();
				if (volStr.startsWith('up'))
					Boombox.volumeUp();
				else if (volStr.startsWith('down'))
					Boombox.volumeDown();
				else
					Boombox.setVolumeTo(volStr);
			}
			this.startMusic();
			return true;
		}
	}
    
	destroyAllDronesOverMark(): void {
		this.allDrones.allSprites.forEach(function (drones: Sprites) {
			drones.spriteProxies.forEach(function (drone: Drone) {
				if (drone.x > 885 && drone.x < 1236 && drone.y > 737)
					drone.selfDestruct();
			}, this);
		}, this);
	}

	test(testCommand: string, userInfo: UserInfo, now: number): boolean {
		if (super.test(testCommand, userInfo, now))
			return true;

		if (testCommand === 'game') {
			gravityGames.startGame(wallBoxesTest);
			return true;
		}

		if (testCommand === 'cg1') {
			gravityGames.startGame(coinGame1);
			return true;
		}

		if (testCommand === 'cg2') {
			gravityGames.startGame(coinGame2);
			return true;
		}


		if (testCommand === 'cg3') {
			gravityGames.startGame(coinGame3);
			return true;
		}

		if (testCommand === 'cg4') {
			gravityGames.startGame(coinGame4);
			return true;
		}

		if (testCommand === 'arm') {
			arm(userInfo.userId);
			return true;
		}

		if (testCommand === 'disarm') {
			disarm(userInfo.userId);
			return true;
		}

		if (testCommand === 'fire') {
			fire(userInfo.userId);
			return true;
		}

		if (testCommand === '+') {
			gravityGames.startGame(wallIntersectionTest);
			return true;
		}

		if (testCommand === 'delta') {
			gravityGames.startGame(wallChangeTest);
			return true;
		}

		if (testCommand === 'edge') {
			gravityGames.startGame(wallEdgeTest);
			return true;
		}

		if (testCommand === 'simple') {
			gravityGames.startGame(simpleWallChangeTest);
			return true;
		}

		if (testCommand === 'portalTest') {
			gravityGames.startGame(portalTest);
			return true;
		}

		if (testCommand === 'corner') {
			gravityGames.startGame(wallCornerTest);
			return true;
		}

		if (testCommand === 'top') {
			gravityGames.startGame(startTopTest);
			return true;
		}

		if (testCommand === 'sample') {
			gravityGames.startGame(sampleGame);
			return true;
		}

		if (testCommand === 'sample2') {
			gravityGames.startGame(sampleGame2);
			return true;
		}

		if (testCommand === 'sample3') {
			gravityGames.startGame(sampleGame3);
			return true;
		}

		if (testCommand === 'det1') {
			gravityGames.startGame(detonationGame1);
			return true;
		}

		if (testCommand === 'det0') {
			gravityGames.startGame(detonationGame0);
			return true;
		}

		if (testCommand === 'sample4') {
			gravityGames.startGame(sampleGame4);
			return true;
		}

		if (testCommand === 'gate') {
			gravityGames.startGame(gatewayTest);
			return true;
		}

		if (testCommand === 'portal drop') {
			this.purplePortals.spriteProxies.forEach((portal: SpriteProxy) => {
				if (portal instanceof Portal) {
					portal.drop();
				}
			});
			return true;
		}

		if (testCommand.startsWith("bye ")) {
			let user: string = testCommand.substring(3);;
		}

		if (testCommand === 'drop') {
			this.horizontalDashedWall.spriteProxies.push(new Wall(0, 200 + this.endCaps.spriteWidth / 2, 300, Orientation.Horizontal, WallStyle.Dashed, 400));
			this.horizontalSolidWall.spriteProxies.push(new Wall(0, 200 + this.endCaps.spriteWidth / 2, 600, Orientation.Horizontal, WallStyle.Solid, 400));
			return true;
		}

		if (testCommand === 'bounce') {
			this.horizontalDashedWall.spriteProxies.push(new Wall(0, 200 + this.endCaps.spriteWidth / 2, 300, Orientation.Horizontal, WallStyle.Dashed, 400));
			this.horizontalDoubleWall.spriteProxies.push(new Wall(0, 200 + this.endCaps.spriteWidth / 2, 800, Orientation.Horizontal, WallStyle.Double, 400));
			return true;
		}

		if (testCommand === 'rect') {
			this.horizontalSolidWall.spriteProxies.push(new Wall(0, screenCenterX - 300, screenCenterY - 300, Orientation.Horizontal, WallStyle.Solid, 500));
			this.verticalSolidWall.spriteProxies.push(new Wall(0, screenCenterX + 200, screenCenterY, Orientation.Vertical, WallStyle.Solid, 400));
			this.verticalDashedWall.spriteProxies.push(new Wall(0, screenCenterX - 780, screenCenterY, Orientation.Vertical, WallStyle.Dashed, 400));
			this.horizontalDashedWall.spriteProxies.push(new Wall(0, screenCenterX - 300, screenCenterY + 300, Orientation.Horizontal, WallStyle.Dashed, 500));
			return true;
		}

		if (testCommand === 'smoke') {
			this.sparkSmoke.add(screenCenterX + 280, screenCenterY + 160);
			return true;
		}

		if (testCommand === 'mix') {
			gravityGames.startGame(mixedTest);
			return true;
		}

		if (testCommand === 'wi') {
			this.warpIns.add(screenCenterX, screenCenterY, 0);
			return true;
		}

		if (testCommand === 'sparks 1') {
			this.downAndRightSparks.add(screenCenterX, screenCenterY);
			return true;
		}
		else if (testCommand === 'sparks 2') {
			this.downAndLeftSparks.add(screenCenterX, screenCenterY);
			return true;
		}
		else if (testCommand === 'sparks 3') {
			this.left1Sparks.add(screenCenterX, screenCenterY);
			return true;
		}
		else if (testCommand === 'sparks 4') {
			this.left2Sparks.add(screenCenterX, screenCenterY);
			return true;
		}
		else if (testCommand === 'sparks 5') {
			this.right1Sparks.add(screenCenterX, screenCenterY);
			return true;
		}
		else if (testCommand === 'sparks 6') {
			this.right2Sparks.add(screenCenterX, screenCenterY);
			return true;
		}
		else if (testCommand === 'sparks 7') {
			this.upAndRightSparks.add(screenCenterX, screenCenterY);
			return true;
		}
		else if (testCommand === 'sparks 8') {
			this.upAndLeftSparks.add(screenCenterX, screenCenterY);
			return true;
		}

		return false;
	}

	musicPlaying: boolean;

	startMusic(): void {
		if (this.musicPlaying)
			return;

		this.musicPlaying = true;

		let boomboxSprite: Boombox = new Boombox(0, new Vector(10, 1000));
		this.boomboxes.spriteProxies.push(boomboxSprite);

		boomboxSprite.fadeInTime = 3000;
		boomboxSprite.playRandomSong();
		boomboxSprite.hueShiftPerSecond = 90;

    /* 
		  Commands:
				* start the music
			  * change the song
			  *	play favorite song
			  *	like (fly through the boom box?) - drone starts groovin' - color shifting the drone.
			  *	dislike
			  *	mark song as favorite
			  *	change player volume????
			  * change the genre
			  * stop the music
		  */
		chat("Available music genres: " + this.getGenreDisplayList())
		let boombox: Boombox = Boombox.getInstance();
		if (boombox)
			chat(`Active genre: ` + boombox.activeGenre);
	}

	getGenreDisplayList(): string {
		const boombox: Boombox = Boombox.getInstance();
		if (boombox)
			return boombox.getGenreDisplayList();
		return '(error - boombox instance not found)';
	}

	stopMusic(): void {
		if (!this.musicPlaying)
			return;

		this.musicPlaying = false;
		Boombox.stopMusic();
		//this.boomboxes.sprites = [];
	}

	tossMeteor(userId: string, params: string) {
		let numbers: string[] = params.split(',');
		if (!numbers || numbers.length < 2) {
			numbers = ['0', '0'];
		}
		// TODO: If third parameter is "x", kill all thrusters as we toss the meteor.
		const userDrone: Drone = this.getDrone(userId);
		if (!userDrone)
			return;
		userDrone.tossMeteor(numbers[0], numbers[1]);
	}

	selfDestructAllDrones() {
		this.allDrones.destroyAllBy(3000);
	}

	droneRight(userId: string, params: string) {
		const userDrone: Drone = this.getDrone(userId);
		if (!userDrone)
			return;

		userDrone.droneRight(params);
	}

	droneLeft(userId: string, params: string) {
		const userDrone: Drone = this.getDrone(userId);
		if (!userDrone)
			return;

		userDrone.droneLeft(params);
	}

	droneUp(userId: string, params: string) {
		const userDrone: Drone = this.getDrone(userId);
		if (!userDrone)
			return;

		userDrone.droneUp(params);
	}

	flyTo(userId: string, params: string) {
		const userDrone: Drone = this.getDrone(userId);
		if (!userDrone)
			return;

		let numbers: string[] = params.split(',');
		if (!numbers || numbers.length < 2) {
			numbers = ['0', '0'];
		}

		userDrone.flyTo(+numbers[0], +numbers[1]);
	}

	private getDrone(userId: string): Drone {
		return this.allDrones.find(userId) as Drone;
	}

	droneDown(userId: string, params: string) {
		const userDrone: Drone = this.getDrone(userId);
		if (!userDrone)
			return;

		userDrone.droneDown(params);
	}

	changeDroneVelocity(userId: string, params: string) {
		const userDrone: Drone = this.getDrone(userId);
		if (!userDrone)
			return;

		const parameters: string[] = params.split(',');
		if (parameters.length < 2)
			return;
		const now: number = performance.now();
		userDrone.changingDirection(now);
		userDrone.changeVelocity(+parameters[0], +parameters[1], now);
  }

  smokeOff(userId: string) {
    let userDrone: Drone = <Drone>this.allDrones.find(userId);
    if (!userDrone)
      return;
    userDrone.smokeOff();
  }

  smokeOn(userId: string, params: string, now: number) {
    let userDrone: Drone = <Drone>this.allDrones.find(userId);
    if (!userDrone)
      return;
    userDrone.smokeOn(params, now);
  }

  smokeColor(userId: string, params: string) {
    let userDrone: Drone = <Drone>this.allDrones.find(userId);
    if (!userDrone)
      return;
    userDrone.setSmokeColor(params);
  }

  smokeLifetime(userId: string, params: string) {
    let userDrone: Drone = <Drone>this.allDrones.find(userId);
    if (!userDrone)
      return;
    userDrone.setSmokeLifetime(params);
  }

	paint(userId: string, command: string, params: string) {
		let userDrone: Drone = <Drone>this.allDrones.find(userId);
		if (!userDrone)
			return;

		userDrone.dropPaint(command, params);
	}

	moveAbsolute(now: number, params: string, userId: string) {
	}

	moveRelative(now: number, params: string, userId: string) {
	}

	wallBounce(nowMs: number): void {
		this.allDrones.allSprites.forEach(function (drones: Sprites) {
			drones.spriteProxies.forEach(function (drone: Drone) {
				this.horizontalSolidWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, nowMs);
				this.verticalSolidWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, nowMs);
				this.horizontalDashedWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, nowMs);
				this.verticalDashedWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, nowMs);
				this.horizontalDoubleWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, nowMs);
				this.verticalDoubleWall.wallBounce(drone, drones.spriteWidth, drones.spriteHeight, nowMs);
			}, this);
		}, this);

		this.allMeteors.allSprites.forEach(function (meteors: Sprites) {
			meteors.spriteProxies.forEach(function (meteor: Meteor) {
				this.horizontalSolidWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, nowMs);
				this.verticalSolidWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, nowMs);
				this.horizontalDashedWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, nowMs);
				this.verticalDashedWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, nowMs);
				this.horizontalDoubleWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, nowMs);
				this.verticalDoubleWall.wallBounce(meteor, meteors.spriteWidth, meteors.spriteHeight, nowMs);
			}, this);
		}, this);
		// TODO: Check for meteors as well.
	}
	allMeteors: SpriteCollection;
	redMeteors: Sprites;
	blueMeteors: Sprites;
	purpleMeteors: Sprites;

	buildCenterRect(sprites) {
		sprites.fillRect(100, 100, myCanvas.clientWidth - 100, myCanvas.clientHeight - 100, 12);
	}

	buildInnerRect(sprites) {
		sprites.fillRect(400, 200, myCanvas.clientWidth - 400, myCanvas.clientHeight - 200, 12);
	}

	fillScreenRect(sprites) {
		sprites.fillRect(0, 0, myCanvas.clientWidth, myCanvas.clientHeight, 12);
	}

	fillScreenRectMinusTwitchBanner(sprites) {
		sprites.fillRect(0, 0, myCanvas.clientWidth, myCanvas.clientHeight, 12);
		sprites.collect(100, 260, 1650, 240);
	}

	outlineScreenRect(sprites) {
		sprites.outlineRect(0, 0, myCanvas.clientWidth, myCanvas.clientHeight, 11, rectangleDrawingSegment.bottom, 4);
	}

	outlineMargin(sprites, margin) {
		sprites.outlineRect(2 * margin, margin, myCanvas.clientWidth - 2 * margin, myCanvas.clientHeight - margin, 12);
	}

	outlineGameSurface(sprites: Sprites) {
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

	outlineGameSurfaceNoAdsNoMark(sprites: Sprites) {
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
			'                                          *' + '\n' +
			'    *************             *************', coinMargin);
	}

	outlineGameSurfaceBeastBuild(sprites: Sprites) {
		sprites.layout(
			'***********************************        ' + '\n' +
			'*                                          ' + '\n' +
			'*                                          ' + '\n' +
			'*                                          ' + '\n' +
			'*                                          ' + '\n' +
			'*                                          ' + '\n' +
			'*                                          ' + '\n' +
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
			'                                          *' + '\n' +
			'    *************             *************', coinMargin);
	}

	fillChatRoom(sprites: Sprites) {
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

	outlineChatRoom(sprites: Sprites) {
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

	outlineCodeEditor(sprites: Sprites) {
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

	allButMark(sprites: Sprites) {
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

	outlineBigRect(sprites) {
		this.outlineMargin(sprites, 100);
	}

	outlineMediumRect(sprites) {
		this.outlineMargin(sprites, 200);
	}

	outlineSmallRect(sprites) {
		this.outlineMargin(sprites, 300);
	}

	plantSeeds(seeds, x) {
		//if (seeds === this.pinkSeeds)
		//  this.plantSeed(this.redFlowers, x + 50, 0);
		////else if (seeds === blueSeeds)
		////  plantSeed(blueFlowers, x + 50, 5);
		//else if (seeds === this.purpleSeeds)
		//  this.plantSeed(this.purpleFlowers, x + 50, 5);
		////else if (seeds === greenSeeds) {
		////  let randomGrass: number = Math.random() * 10;
		////  if (randomGrass < 1)
		////    plantSeed(grass1, x + 50, 0);
		////  else if (randomGrass < 2)
		////    plantSeed(grass2, x + 50, 0);
		////  else if (randomGrass < 3)
		////    plantSeed(grass3, x + 50, 0);
		////  else
		////    plantSeed(grass4, x + 50, 0);
		////}
		//else
		if (seeds === this.yellowSeeds) {
			let randomYellow: number = Math.random() * 10;
			if (randomYellow < 2)
				this.plantSeed(this.yellowPetunias, x + 50, 5);
			else {
				let angle: number = Random.plusMinus(15);
				this.plantSeed(this.superFlowerBack, x + 50, 0, false, angle);
				this.plantSeed(this.superFlowerFront, x + 50, 0, true, angle);
			}
		}
		new Audio(Folders.assets + 'Sound Effects/MeteorHit.wav').play();
	}

	dronesRed: Sprites;
	boomboxes: Sprites;
	droneGateways: Gateways;
	allDrones: SpriteCollection;
  allSeeds: SpriteCollection;

  // TODO: Update this to a variety of paint shapes with the ability to hue shift paint.
  redSplats: SplatSprites;
	blackSplats: SplatSprites;
	whiteSplats: SplatSprites;
	orangeSplats: SplatSprites;
	amberSplats: SplatSprites;
	yellowSplats: SplatSprites;
	greenSplats: SplatSprites;
	blueSplats: SplatSprites;
	cyanSplats: SplatSprites;
	indigoSplats: SplatSprites;
	violetSplats: SplatSprites;
  magentaSplats: SplatSprites;

  smokeSprites: Sprites;

	loadWall(orientation: Orientation, style: WallStyle, folder: string, expectedFrameCount: number): WallSprites {
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

	loadWalls() {
		this.endCaps = new Sprites(`FireWall/EndCaps/Cap`, 4, 0, AnimationStyle.Static);
		this.endCaps.baseAnimation.jiggleX = 1;
		this.endCaps.baseAnimation.jiggleY = 1;

		this.allWalls = new SpriteCollection();

		this.horizontalDashedWall = this.loadWall(Orientation.Horizontal, WallStyle.Dashed, 'Dashed/Right', 100);
		this.horizontalDashedWall.moves = true;
		this.allWalls.add(this.horizontalDashedWall);

		this.verticalDashedWall = this.loadWall(Orientation.Vertical, WallStyle.Dashed, 'Dashed/Up', 100);
		this.verticalDashedWall.moves = true;
		this.allWalls.add(this.verticalDashedWall);

		this.horizontalSolidWall = this.loadWall(Orientation.Horizontal, WallStyle.Solid, 'Solid', 94);
		this.horizontalSolidWall.moves = true;
		this.allWalls.add(this.horizontalSolidWall);

		this.horizontalDoubleWall = this.loadWall(Orientation.Horizontal, WallStyle.Double, 'Double', 94);
		this.horizontalDoubleWall.moves = true;
		this.allWalls.add(this.horizontalDoubleWall);

		this.verticalDoubleWall = this.loadWall(Orientation.Vertical, WallStyle.Double, 'Double', 94);
		this.verticalDoubleWall.moves = true;
		this.allWalls.add(this.verticalDoubleWall);

		this.verticalSolidWall = this.loadWall(Orientation.Vertical, WallStyle.Solid, 'Solid', 90);
		this.verticalSolidWall.moves = true;
		this.allWalls.add(this.verticalSolidWall);
	}
  
	warpIns: Sprites;

	loadWarpInAnimation() {
		this.warpIns = new Sprites(`Drones/Warp In/DroneWarpIn`, 15, 30, AnimationStyle.SequentialStop);
		this.warpIns.originX = 96;
		this.warpIns.originY = 45;
	}

	droneHealthLights: Sprites;

	addDrones() {
		this.allDrones = new SpriteCollection();

		//dronesRed = loadDrones('Red');
		//dronesBlue = loadDrones('Blue');
    this.dronesRed = this.loadDrones('192x90');
    this.smokeSprites = this.loadSmoke();
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
	loadSplat(color: string): SplatSprites {
		return new SplatSprites(color, 39, 15, AnimationStyle.SequentialStop, true);
	}

	allSplats: SpriteCollection;

	addSplats() {
		this.allSplats = new SpriteCollection();
		this.redSplats = this.loadSplat('Red');
		this.blackSplats = this.loadSplat('Black');
		this.whiteSplats = this.loadSplat('White');
		this.orangeSplats = this.loadSplat('Orange');
		this.amberSplats = this.loadSplat('Amber');
		this.yellowSplats = this.loadSplat('Yellow');
		this.greenSplats = this.loadSplat('Green');
		this.blueSplats = this.loadSplat('Blue');
		this.cyanSplats = this.loadSplat('Cyan');
		this.indigoSplats = this.loadSplat('Indigo');
		this.violetSplats = this.loadSplat('Violet');
		this.magentaSplats = this.loadSplat('Magenta');
		this.allSplats.add(this.blackSplats);
		this.allSplats.add(this.redSplats);
		this.allSplats.add(this.orangeSplats);
		this.allSplats.add(this.amberSplats);
		this.allSplats.add(this.yellowSplats);
		this.allSplats.add(this.greenSplats);
		this.allSplats.add(this.cyanSplats);
		this.allSplats.add(this.blueSplats);
		this.allSplats.add(this.indigoSplats);
		this.allSplats.add(this.violetSplats);
		this.allSplats.add(this.magentaSplats);
		this.allSplats.add(this.whiteSplats);
	}


	droneExplosions: SpriteCollection;

	loadDroneExplosions() {
		this.droneExplosions = new SpriteCollection();
		const numDroneExplosions: number = 11;
		for (var i = 1; i <= numDroneExplosions; i++) {
			this.droneExplosions.add(new Sprites('Drones/Explosions/' + i + '/Explosion', 53, 5, AnimationStyle.Sequential, true));
		}
	}

	//` ![](7582E2608FC840A8A6D3CC61B5A58CB6.png)
	addSeeds() {
		this.allSeeds = new SpriteCollection();

		let plantSeeds = this.plantSeeds.bind(this);

		//this.pinkSeeds = new Sprites("Seeds/Pink/PinkSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
		//this.pinkSeeds.moves = true;
		//this.allSeeds.add(this.pinkSeeds);

		this.yellowSeeds = new Sprites("Seeds/Yellow/YellowSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
		this.yellowSeeds.moves = true;
		this.allSeeds.add(this.yellowSeeds);

		//this.purpleSeeds = new Sprites("Seeds/Purple/PurpleSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
		//this.purpleSeeds.moves = true;
		//this.allSeeds.add(this.purpleSeeds);
	}

	//` ![](D4E24ABDF6E5B9F063E242EBB0ADA55E.png;;0,35,173,210)
	allSparks: SpriteCollection;
	downAndRightSparks: Sprites;
	downAndLeftSparks: Sprites;
	left1Sparks: Sprites;
	left2Sparks: Sprites;
	right1Sparks: Sprites;
	right2Sparks: Sprites;
	upAndRightSparks: Sprites;
	upAndLeftSparks: Sprites;

	loadSparks(folder: string, frameCount: number, originX: number, originY: number): Sprites {
		const sparkFrameInterval: number = 40;
		let sparks: Sprites = new Sprites(`FireWall/Sparks/${folder}/Spark`, frameCount, sparkFrameInterval, AnimationStyle.Sequential, true);
		//sparks.moves = true;
		sparks.originX = originX;
		sparks.originY = originY;
		return sparks;
	}

	loadAllSparks() {
		this.allSparks = new SpriteCollection();
		this.downAndLeftSparks = this.loadSparks('Down and Left', 13, 90, 2);
		this.downAndRightSparks = this.loadSparks('Down and Right', 13, 5, 1);
		this.left1Sparks = this.loadSparks('Left', 8, 178, 19);
		this.left2Sparks = this.loadSparks('Left 2', 9, 121, 66);
		this.right1Sparks = this.loadSparks('Right', 8, 4, 23);
		this.right2Sparks = this.loadSparks('Right 2', 9, 2, 68);
		this.upAndRightSparks = this.loadSparks('Up and Right', 9, 6, 178);
		this.upAndLeftSparks = this.loadSparks('Up and Left', 9, 88, 176);

		this.allSparks.add(this.downAndLeftSparks);
		this.allSparks.add(this.downAndRightSparks);
		this.allSparks.add(this.left1Sparks);
		this.allSparks.add(this.right1Sparks);
		this.allSparks.add(this.left2Sparks);
		this.allSparks.add(this.right2Sparks);
		this.allSparks.add(this.upAndRightSparks);
		this.allSparks.add(this.upAndLeftSparks);
	}

	plantSeed(spriteArray: Sprites, x, y, canShiftHue: boolean = false, angle: number = 0) {
		const flowerLifeSpan: number = 120 * 1000;
		let heightDrop: number = Random.max(9) * 10;
		let hueShift: number = 0;
		if (canShiftHue) {
			hueShift = Random.max(360);

			let numTries: number = 0;
			const maxTries: number = 30;
			while (this.hueIsGreen(hueShift) && numTries < maxTries) {
				numTries++;
				hueShift = Random.max(360);
			}
			if (this.hueIsGreen(hueShift))
				hueShift = 0;
		}

		let flower: SpriteProxy = spriteArray.addShifted(x, screenHeight + y + heightDrop, 0, hueShift);
		flower.expirationDate = performance.now() + flowerLifeSpan;
		flower.rotation = angle;
	}

	hueIsGreen(hueShift: number): boolean {
		return hueShift > 60 && hueShift < 190;
	}

	putMeteorOnDrone(meteorProxy: SpriteProxy, droneProxy: SpriteProxy, now: number): void {
		let drone: Drone = <Drone>droneProxy;
		let meteor: Meteor = <Meteor>meteorProxy;
		if (drone && meteor) {
			drone.addMeteor(meteor, now);
		}
	}

	voteSongDown(meteorProxy: SpriteProxy, boomboxProxy: SpriteProxy, now: number): void {
		if (boomboxProxy instanceof Boombox)
			boomboxProxy.voteSongDown(now);

		if (meteorProxy instanceof Meteor)
			meteorProxy.blowUp();
	}


	collectCoinsInRect(x: number, y: number, width: number, height: number): number {
		var coinsCollected: number = this.coins.collect(x, y, width, height);
		if (coinsCollected > 0) {
			new Audio(Folders.assets + 'Sound Effects/CollectCoin.wav').play();
			if (gravityGames.activeGame.score)
				gravityGames.activeGame.score.value += coinsCollected;
		}
		return coinsCollected;
	}

	collectCoins(now: number) {
		this.collectCoinsInRect(myRocket.x, myRocket.y, 310, 70);

		this.allDrones.allSprites.forEach(function (drones: Sprites) {
			drones.spriteProxies.forEach(function (drone: Drone) {
				const margin: number = 8;
				let coinsFound = this.collectCoinsInRect(drone.x + margin, drone.y + margin, Drone.width * drone.scale - margin / 2, Drone.height * drone.scale - margin / 2);
				if (coinsFound > 0) {
					connection.invoke("AddCoins", drone.userId, coinsFound);
					drone.coinCount += coinsFound;
					drone.justCollectedCoins(now);
				}
			}, this);
		}, this);

		//dronesRed.sprites.forEach
	}
}

class Genre {
	constructor(public name: string, public count: number) {

	}
}

class Boombox extends ColorShiftingSpriteProxy {
	genres: Array<Genre> = [];
	activeGenre: string;
	activeSongCount: number;
	static volume = 2;
	static saveVolume = 0;
	static readonly suppressVolumeLevel: number = 0.5;
	soundManager: SoundManager = new SoundManager('GameDev/Assets/DroneGame/Music');
	activeSong: HTMLAudioElement;
	static suppressingVolume = false;
	static suppressingVolumeEnds: number;

	constructor(startingFrameNumber: number, public center: Vector, lifeSpanMs = -1) {
		super(startingFrameNumber, center, lifeSpanMs);
		//this.addSongs('Adventure', 4);
		this.addSongs('Techno', 80);
		this.addSongs('Action', 108);
		this.addSongs('Rock', 177);
		this.addSongs('Funk', 103);
		this.addSongs('EDM', 108);
		this.selectRandomGenre();
	}

	selectRandomGenre(): void {
		const index: number = Math.floor(Math.random() * this.genres.length);
		this.activeSongCount = this.genres[index].count;
		this.activeGenre = this.genres[index].name;
	}

	static suppressVolume(seconds: number, now: number): void {
		Boombox.suppressingVolume = true;
		const boombox: Boombox = Boombox.getInstance();
		if (boombox !== null) {
			if (Boombox.volume !== Boombox.suppressVolumeLevel) {
				Boombox.saveVolume = Boombox.volume;
				Boombox.volume = Boombox.suppressVolumeLevel
				boombox.setVolumeForActiveSong();
			}
			Boombox.suppressingVolumeEnds = now + seconds * 1000;
		}

		console.log(`Suppressing music volume for ${seconds} seconds.`);
	}

	static getInstance(): Boombox {
		if (!(activeDroneGame instanceof DroneGame))
			return null;

		return activeDroneGame.getBoomboxInstance();
	}

	static changeGenre(newGenre: string): void {
		const boombox: Boombox = Boombox.getInstance();
		if (boombox) {
			boombox.changeActiveGenre(newGenre);

			if (activeDroneGame instanceof DroneGame) {
				const textEffect: TextEffect = activeDroneGame.animations.addText(new Vector(100, 1000), newGenre, 4000);
				textEffect.scale = 3;
				textEffect.targetScale = 5;
				textEffect.fadeOutTime = 800;
				textEffect.fadeInTime = 800;
				textEffect.velocityX = 3;
				textEffect.velocityY = 2;
			}
		}
	}

	static stopMusic(): void {
		let boombox: Boombox = Boombox.getInstance();
		if (boombox) {
			//boombox.stopActiveSong();
			boombox.clearActiveTimeout();
			boombox.expirationDate = performance.now() + 3000;
		}
	}

	stopActiveSong() {
		if (this.activeSong)
			this.activeSong.pause();
	}

	changeActiveGenre(newGenre: string): void {
		this.genres.forEach(function (genre: Genre) {
			if (genre.name.toLowerCase() == newGenre.toLowerCase()) {
				this.activeGenre = genre.name;
				this.activeSongCount = genre.count;
				chat(`Switching to ${this.activeGenre}, right after this song.`);
			}
		}, this);
	}

	getGenreDisplayList(): string {
		let genreDisplayList: string = '';
		for (var i = 0; i < this.genres.length - 1; i++) {
			genreDisplayList += this.genres[i].name + ', ';
		}
		genreDisplayList += 'and ' + this.genres[this.genres.length - 1].name;
		return genreDisplayList;
	}


	// Here we are in a class

	static trimVolume() {
		Boombox.volume = Math.round(Boombox.volume);
		if (Boombox.volume > 11)
			Boombox.volume = 11;
		if (Boombox.volume < 1)
			Boombox.volume = 1;
	}

	static volumeDown(): void {
		Boombox.setVolumeTo(Boombox.volume - 1);
	}


	static volumeUp(): void {
		Boombox.setVolumeTo(Boombox.volume + 1);
	}

	static setVolumeTo(volStr: number | string): void {
		let newVolume: number = +volStr;
		if (!newVolume)
			return;

		Boombox.volume = newVolume;
		Boombox.actOnNewVolume();
	}

	static actOnNewVolume(): void {
		Boombox.trimVolume();
		const boombox: Boombox = Boombox.getInstance();
		if (boombox)
			boombox.setVolumeForActiveSong();

		Boombox.reportVolume();
	}

	static reportVolume(): any {
		if (Boombox.suppressingVolume) {
			const timeRemainingSec: number = (Boombox.suppressingVolumeEnds - performance.now()) / 1000;
			chat(`Volume is now: ${Boombox.volume} (Suppressing to: ${Boombox.suppressVolumeLevel} - ends in ${timeRemainingSec} sec)`);
		}
		else 
			chat(`Volume is now: ${Boombox.volume}`);
	}

	setVolumeForActiveSong() {
		let actualVolume: number;
		const thisVolume: number = Math.round(Boombox.volume);
		if (thisVolume >= 4)
			actualVolume = thisVolume - 3;
		else
			switch (thisVolume) {
				case 3:
					actualVolume = 0.75;
					break;
				case 2:
					actualVolume = 0.5;
					break;
				case 1:
					actualVolume = 0.25;
					break;
			}
		if (this.activeSong)
			this.activeSong.volume = this.fadeVolumeMultiplier * actualVolume / 8;
	}

	addSongs(genre: string, count: number) {
		this.genres.push(new Genre(genre, count));
	}

	playRandomSong() {
		const fiveMinutes: number = 5 * 60 * 1000;
		let index: number = Random.intBetween(1, this.activeSongCount);
		let tries = 0;
		const maxTries = 30;
		let fileName: string = this.getFileName(index);
		while (tries < maxTries && this.soundManager.playedRecently(fileName, fiveMinutes)) {
			index = Random.intBetween(1, this.activeSongCount);
			fileName = this.getFileName(index);
			tries++;
		}
		//console.log('playRandomSong - fileName: ' + fileName);
		if (this.activeSong) {
			this.activeSong.pause();
		}
		this.activeSong = this.soundManager.safePlayMp3ReturnAudio(fileName, fiveMinutes);
		if (!this.activeSong)
			this.activeSong = this.soundManager.safePlayMp3ReturnAudio(fileName, 0);

		this.setVolumeForActiveSong();

		this.clearActiveTimeout();

		this.activeSong.addEventListener('loadedmetadata', function () {
			const duration: number = this.activeSong.duration;

			//const testingCrossFade: boolean = true;
			//if (testingCrossFade) {
			//	const startTimeFromEnd: number = 20;
			//	let newStartTime: number = duration - startTimeFromEnd;
			//	this.activeSong.currentTime = newStartTime;
			//	duration = startTimeFromEnd;
			//}

			this.activeTimeout = setTimeout(this.playNextSong.bind(this), duration * 1000 - Boombox.crossFadeTime);
		}.bind(this), false);
	}

	clearActiveTimeout(): any {
		if (this.activeTimeout) {
			clearTimeout(this.activeTimeout);
			this.activeTimeout = null;
		}
	}

	activeTimeout: any;

	static readonly crossFadeTime: number = 5000;

	playNextSong() {
		const boomboxSprite: Boombox = new Boombox(0, new Vector(10, 1000));

		if (activeDroneGame instanceof DroneGame)
			activeDroneGame.boomboxes.spriteProxies.unshift(boomboxSprite);

		boomboxSprite.fadeInTime = Boombox.crossFadeTime;
		boomboxSprite.playRandomSong();
		boomboxSprite.hueShiftPerSecond = 90;
		this.fadeOutTime = Boombox.crossFadeTime;
		this.expirationDate = performance.now() + this.fadeOutTime;
	}

	draw(baseAnimation: Part, context: CanvasRenderingContext2D, now: number, spriteWidth: number, spriteHeight: number,
		originX = 0, originY = 0): void {
		const newFadeVolumeMultiplier: number = Math.round(this.getAlpha(now) * 10) / 10;
		if (newFadeVolumeMultiplier !== this.fadeVolumeMultiplier) {
			this.fadeVolumeMultiplier = newFadeVolumeMultiplier;
			this.setVolumeForActiveSong();
		}
		super.draw(baseAnimation, context, now, spriteWidth, spriteHeight, originX, originY);

		if (Boombox.suppressingVolume && now > Boombox.suppressingVolumeEnds) {
			Boombox.suppressingVolume = false;
			const boombox: Boombox = Boombox.getInstance();
			if (boombox !== null) {
				Boombox.volume = Boombox.saveVolume;
				boombox.setVolumeForActiveSong();
			}
		}
	}

	destroying(): void {
		if (this.activeSong)
			this.activeSong.pause();
		this.activeSong = null;
	}

	fadeVolumeMultiplier = 1;

	getFileName(index: number): string {
		return `${this.activeGenre}/song (${index.toString()})`;
	}

	voteSongDown(now: number): any {
		this.playRandomSong();
	}

	isHitBy(thisSprite: SpriteProxy): boolean {
		const minDistanceForHit = 110;
		if (thisSprite instanceof Meteor && thisSprite.owned)
			return false;

		return this.getDistanceTo(thisSprite) < minDistanceForHit;
	}
}