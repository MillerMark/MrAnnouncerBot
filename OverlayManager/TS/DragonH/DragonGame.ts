var gravityGames2: GravityGames;
var myRocket2: Rocket;

class DragonGame extends GamePlusQuiz {
  emitter: Emitter;
  yellowSeeds: Sprites;
  yellowFlowers1: Sprites;
  portalBackground: Sprites;
  backgroundBanner: Part;

  constructor() {
    super();
  }

  updateScreen(context: CanvasRenderingContext2D, now: number) {
    super.updateScreen(context, now);

    this.emitter.updatePosition(now);
    myRocket2.updatePosition(now);
    myRocket2.bounce(0, 0, screenWidth, screenHeight, now);

    this.allSeeds.bounce(0, 0, screenWidth, screenHeight, now);

    //backgroundBanner.draw(myContext, 0, 0);

    if (!myRocket2.isDocked)
      gravityGames2.draw(myContext);

    this.allSeeds.updatePositions(now);
    
    this.wallBounce(now);

    this.allSeeds.draw(myContext, now);

    myRocket2.draw(myContext, now);
    this.yellowFlowers1.draw(myContext, now);
    this.emitter.draw(myContext, now);
    //drawCrossHairs(myContext, crossX, crossY);
  }

  removeAllGameElements(now: number): void {
    super.removeAllGameElements(now);
  }

  initialize() {
    super.initialize();
    Folders.assets = 'GameDev/Assets/DroneGame/';

    myRocket2 = new Rocket(0, 0);
    myRocket2.x = 0;
    myRocket2.y = 0;

    gravityGames2 = new GravityGames();
  }

  start() {
    super.start();
    gravityGames2.selectPlanet('Earth');
    gravityGames2.newGame();
  }

  loadResources(): void {
    this.emitter = new Emitter(new Vector(screenCenterX, screenCenterY));
    this.emitter.radius = 80;
    this.emitter.particleRadius = 10;
    this.emitter.particleRadiusVariance = 0.5;
    this.emitter.addParticles(20);

    super.loadResources();

    this.addSeeds();

    globalBypassFrameSkip = true;

    const flowerFrameRate: number = 20;
    //const grassFrameRate: number = 25;

    this.yellowFlowers1 = new Sprites("Flowers/YellowPetunias1/YellowPetunias", 270, flowerFrameRate, AnimationStyle.Loop, true);
    this.yellowFlowers1.returnFrameIndex = 64;

    globalBypassFrameSkip = false;

    Part.loadSprites = true;

    this.backgroundBanner = new Part("CodeRushedBanner", 1, AnimationStyle.Static, 200, 300);
  }

  executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    if (super.executeCommand(command, params, userId, userName, displayName, color, now))
      return true;
    if (command === "Launch") {
      if (!myRocket2.started || myRocket2.isDocked) {
        myRocket2.started = true;
        myRocket2.launch(now);
        gravityGames2.newGame();
        chat('Launching...');
      }
    }
    else if (command === "Dock") {
      if (this.isSuperUser(userName)) {
        this.removeAllGameElements(now);
      }

      if (myRocket2.started && !myRocket2.isDocked) {
        chat('docking...');
        myRocket2.dock(now);
      }
    }
    else if (command === "ChangePlanet") {
      gravityGames2.selectPlanet(params);
    }
    else if (command === "Left") {
      myRocket2.fireRightThruster(now, params);
    }
    else if (command === "Right") {
      myRocket2.fireLeftThruster(now, params);
    }
    else if (command === "Up") {
      myRocket2.fireMainThrusters(now, params);
    }
    else if (command === "Down") {
      myRocket2.killHoverThrusters(now, params);
    }
    else if (command === "Drop") {
      myRocket2.dropMeteor(now);
    }
    else if (command === "Chutes") {
      if (myRocket2.chuteDeployed)
        myRocket2.retractChutes(now);
      else
        myRocket2.deployChute(now);
    }
    else if (command === "Retract") {
      myRocket2.retractEngines(now);
    }
    else if (command === "Extend") {
      myRocket2.extendEngines(now);
    }
    else if (command === "Seed") {
      myRocket2.dropSeed(now, params);
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
    this.plantSeed(this.yellowFlowers1, x + 50, 5);
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