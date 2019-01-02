class DragonGame extends GamePlusQuiz {
  emitter: Emitter;
  yellowSeeds: Sprites;
  yellowFlowers: Sprites;
  scrollRolls: Sprites;
  scrollSlam: Sprites;
  scrollSlamlastUpdateTime: number;

  constructor(context: CanvasRenderingContext2D) {
    super(context);
  }

  update(timestamp: number) {
    this.updateGravity();
    super.update(timestamp);
  }

  updateScreen(context: CanvasRenderingContext2D, now: number) {
    super.updateScreen(context, now);

    this.scrollRolls.draw(context, now);
    this.scrollSlam.draw(context, now);

    myRocket.updatePosition(now);
    myRocket.bounce(0, 0, screenWidth, screenHeight, now);

    this.allSeeds.bounce(0, 0, screenWidth, screenHeight, now);

    //backgroundBanner.draw(myContext, 0, 0);

    if (!myRocket.isDocked)
      gravityGames.draw(myContext);

    this.allSeeds.updatePositions(now);

    this.wallBounce(now);

    this.allSeeds.draw(myContext, now);

    myRocket.draw(myContext, now);
    this.yellowFlowers.draw(myContext, now);
    //drawCrossHairs(myContext, crossX, crossY);

    this.scrollSlam.draw(context, now);
  }

  removeAllGameElements(now: number): void {
    super.removeAllGameElements(now);
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

    this.updateGravity();
    // If all characters were WorldObject descendants, this would be all that
    // is needed per game... just add the characters and let the world do the rest :)
    this.world.addCharacter(this.emitter);
  }

  loadDragonAssets() {
    var assetFolderName: string = Folders.assets;
    Folders.assets = 'GameDev/Assets/DragonH/';

    const fps30: number = 33; // 33 milliseconds == 30 fps
    this.scrollRolls = new Sprites("Scroll/Open/ScrollOpen", 23, fps30, AnimationStyle.SequentialStop, true);

    this.scrollSlam = new Sprites("Scroll/Slam/Slam", 8, fps30, AnimationStyle.Sequential, true);

    Folders.assets = assetFolderName;
  }

  loadResources(): void {
    //this.buildBlueParticleBall();
    //this.purpleMagic();
    //this.purpleBurst();
    this.orbital();
    //this.buildSmoke();

    super.loadResources();

    this.loadDragonAssets();

    this.addSeeds();

    globalBypassFrameSkip = true;

    const flowerFrameRate: number = 20;
    //const grassFrameRate: number = 25;

    this.yellowFlowers = new Sprites("Flowers/YellowPetunias1/YellowPetunias", 270, flowerFrameRate, AnimationStyle.Loop, true);
    this.yellowFlowers.returnFrameIndex = 64;

    globalBypassFrameSkip = false;

    Part.loadSprites = true;

    //this.backgroundBanner = new Part("CodeRushedBanner", 1, AnimationStyle.Static, 200, 300);
  }

  buildSmoke() {
    this.emitter = new Emitter(new Vector(screenCenterX, screenCenterY));
    this.emitter.radius = 99;
    this.emitter.saturation.target = 0;
    this.emitter.brightness.target = 0.5;
    this.emitter.brightness.relativeVariance = 0.5;
    this.emitter.particleRadius.target = 11;
    this.emitter.particleRadius.relativeVariance = 1;
    this.emitter.particlesPerSecond = 100;
    this.emitter.particleLifeSpanSeconds = 3;
    this.emitter.particleInitialVelocity.target = 0;
    this.emitter.particleGravity = 0.4;
    this.emitter.particleWind = Vector.fromPolar(270, 2);
  }

  orbital() {
    this.emitter = new Emitter(new Vector(1.1 * screenCenterX, screenCenterY), new Vector(0, 0));
    //this.emitter = new Emitter(new Vector(1.1 * screenCenterX, screenCenterY), new Vector(0, -6));
    this.emitter.radius = 11;
    this.emitter.hue.target = 145;
    this.emitter.hue.absoluteVariance = 35;
    this.emitter.saturation.target = 0.8;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.brightness.target = 0.5;
    this.emitter.particleRadius.target = 2.2;
    this.emitter.particleRadius.relativeVariance = 0.8;
    this.emitter.particlesPerSecond = 800;
    this.emitter.particleLifeSpanSeconds = 1.5;
    this.emitter.particleInitialVelocity.target = 0.8;
    this.emitter.particleInitialVelocity.relativeVariance = 0.5;
    this.emitter.particleGravity = 1;
    this.emitter.particleMass = 0;
    this.emitter.particleFadeInTime = 0.05;
    this.emitter.gravity = 9;
    this.emitter.gravityCenter = new Vector(screenCenterX, screenCenterY);
  }

  purpleMagic() {
    this.emitter = new Emitter(new Vector(1920, 1200), new Vector(-6, -9));
    this.emitter.radius = 3;
    this.emitter.hue.target = 270;
    this.emitter.hue.absoluteVariance = 75;
    this.emitter.saturation.target = 0.8;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.brightness.target = 0.5;
    this.emitter.particleRadius.target = 1.2;
    this.emitter.particleRadius.relativeVariance = 0.8;
    this.emitter.particlesPerSecond = 800;
    this.emitter.particleLifeSpanSeconds = 1.5;
    this.emitter.particleInitialVelocity.target = 0.8;
    this.emitter.particleInitialVelocity.relativeVariance = 0.5;
    this.emitter.particleGravity = 4;
    this.emitter.particleFadeInTime = 0.05;
    this.emitter.gravity = 3;
  }

  purpleBurst() {
    this.emitter = new Emitter(new Vector(screenCenterX, screenCenterY));
    this.emitter.radius = 3;
    this.emitter.hue.target = 270;
    this.emitter.hue.absoluteVariance = 75;
    this.emitter.saturation.target = 0.8;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.brightness.target = 0.5;
    this.emitter.particleRadius.target = 2;
    this.emitter.particleRadius.relativeVariance = 0.8;
    this.emitter.particlesPerSecond = 600;
    this.emitter.particleLifeSpanSeconds = 2;
    this.emitter.particleInitialVelocity.target = 3.4;
    this.emitter.particleInitialVelocity.relativeVariance = 0.5;
    this.emitter.particleGravity = 0;
    this.emitter.particleFadeInTime = 0.1;
    this.emitter.gravity = 0;
    this.emitter.maxTotalParticles = 300;
  }

  buildBlueParticleBall() {
    this.emitter = new Emitter(new Vector(1250, 180));
    this.emitter.radius = 0;
    this.emitter.hue.target = 220;
    this.emitter.hue.absoluteVariance = 25;
    this.emitter.saturation.target = 0.8;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.brightness.target = 0.5;
    this.emitter.brightness.relativeVariance = 0.5;
    this.emitter.particleRadius.target = 4;
    this.emitter.particleRadius.relativeVariance = 3;
    this.emitter.particlesPerSecond = 200;
    this.emitter.particleLifeSpanSeconds = 24;
    this.emitter.particleInitialVelocity.target = 1.3;
    this.emitter.particleInitialVelocity.relativeVariance = 0.3;
    this.emitter.particleGravityCenter = new Vector(screenCenterX, screenCenterY);
    this.emitter.particleGravity = 3;
  }

  executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    if (super.executeCommand(command, params, userId, userName, displayName, color, now))
      return true;
    if (command === "Launch") {
      if (!myRocket.started || myRocket.isDocked) {
        myRocket.started = true;
        myRocket.launch(now);
        gravityGames.newGame();
        chat('Launching...');
      }
    }
    else if (command === "Dock") {
      if (this.isSuperUser(userName)) {
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

  }

  test(testCommand: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    if (super.test(testCommand, userId, userName, displayName, color, now))
      return true;

    if (testCommand === 'slam') {
      this.scrollSlam.add(0, 0, 0);
      return true;
    }

    if (testCommand === 'open') {
      this.scrollRolls.sprites = [];
      this.scrollRolls.add(0, 0, 0);
      return true;
    }

    return false;
  }

  wallBounce(now: number): void {
  }

  plantSeeds(seeds, x) {
    this.plantSeed(this.yellowFlowers, x + 50, 5);
    new Audio(Folders.assets + 'Sound Effects/MeteorHit.wav').play();
  }

  allSeeds: SpriteCollection;

  //` ![](7582E2608FC840A8A6D3CC61B5A58CB6.png)
  addSeeds() {
    this.allSeeds = new SpriteCollection();

    let plantSeeds = this.plantSeeds.bind(this);

    this.yellowSeeds = new Sprites("Seeds/Yellow/YellowSeed", 16, 75, AnimationStyle.Loop, true, plantSeeds);
    this.yellowSeeds.moves = true;
    this.allSeeds.add(this.yellowSeeds);
  }

  plantSeed(spriteArray, x, y) {
    const flowerLifeSpan: number = 120 * 1000;
    spriteArray.sprites.push(new SpriteProxy(0, x - spriteArray.spriteWidth / 2, screenHeight - spriteArray.spriteHeight + y, flowerLifeSpan));
  }
} 