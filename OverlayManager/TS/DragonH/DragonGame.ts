class DragonGame extends GamePlusQuiz {
  emitter: Emitter;
  yellowSeeds: Sprites;
  yellowFlowers: Sprites;
  lastUpdateTime: number;

  constructor() {
    super();
  }

  updateScreen(context: CanvasRenderingContext2D, now: number) {
    super.updateScreen(context, now);

    var secondsSinceLastUpdate: number = (now - this.lastUpdateTime) / 1000;

    this.emitter.update(now, secondsSinceLastUpdate);
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
    this.emitter.draw(myContext, now);
    //drawCrossHairs(myContext, crossX, crossY);

    this.lastUpdateTime = now;
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
    this.lastUpdateTime = performance.now();
  }

  loadResources(): void {
    this.emitter = new Emitter(new Vector(1250, 180));
    this.emitter.radius = 40;
    this.emitter.particleRadius = 10;
    this.emitter.particleRadiusVariance = 0.5;
    this.emitter.particlesPerSecond = 20;
    this.emitter.particleLifeSpanSeconds = 8;
    //this.emitter.addParticles(20);

    super.loadResources();

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