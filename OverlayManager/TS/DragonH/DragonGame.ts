class DragonGame extends GamePlusQuiz {
  emitter: Emitter;
  yellowSeeds: Sprites;
  yellowFlowers: Sprites;
  scrollSlamlastUpdateTime: number;
  shouldDrawCenterCrossHairs: boolean = false;
  characterStatsScroll: CharacterStatsScroll;

  constructor(context: CanvasRenderingContext2D) {
    super(context);
  }

  update(timestamp: number) {
    this.updateGravity();
    super.update(timestamp);
  }

  updateScreen(context: CanvasRenderingContext2D, now: number) {
    super.updateScreen(context, now);

    myRocket.updatePosition(now);
    myRocket.bounce(0, 0, screenWidth, screenHeight, now);

    this.allSeeds.bounce(0, 0, screenWidth, screenHeight, now);

    //backgroundBanner.draw(myContext, 0, 0);

    if (!myRocket.isDocked)
      gravityGames.draw(myContext);

    this.allSeeds.updatePositions(now);

    this.wallBounce(now);

    this.allSeeds.draw(myContext, now);

    //myRocket.draw(myContext, now);
    this.yellowFlowers.draw(myContext, now);

    if (this.shouldDrawCenterCrossHairs)
      drawCrossHairs(myContext, screenCenterX, screenCenterY);
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

    this.initializeScroll();
  }

  initializeScroll(): any {
    this.characterStatsScroll = new CharacterStatsScroll();
    this.world.addCharacter(this.characterStatsScroll);

    //this.characterStatsScroll.characters.push(Character.newTestElf());
    //this.characterStatsScroll.characters.push(Character.newTestBarbarian());
    //this.characterStatsScroll.characters.push(Character.newTestWizard());
    //this.characterStatsScroll.characters.push(Character.newTestDruid());
    this.characterStatsScroll.pages.push(StatPage.createMainStatsPage());
    this.characterStatsScroll.pages.push(StatPage.createSkillsStatsPage());
    this.characterStatsScroll.pages.push(StatPage.createEquipmentPage());
    this.characterStatsScroll.selectedCharacterIndex = 0;
    this.characterStatsScroll.selectedStatPageIndex = 0;

    this.characterStatsScroll.state = ScrollState.none;
  }

  start() {
    super.start();
    gravityGames.selectPlanet('Earth');
    gravityGames.newGame();

    this.updateGravity();
    // If all characters were WorldObject descendants, this would be all that
    // is needed per game... just add the characters and let the world do the rest :)
    if (this.emitter)
      this.world.addCharacter(this.emitter);
  }

  loadDragonAssets() {
  }

  loadResources(): void {
    //this.blueBall();
    //this.purpleMagic();
    //this.purpleBurst();
    //this.orbital();
    //this.buildSmoke();
    //this.buildTestParticle();

    super.loadResources();


    this.characterStatsScroll.loadResources();

    Folders.assets = 'GameDev/Assets/DroneGame/';

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

  buildTestGoldParticle(): any {
    //this.emitter = new Emitter(new Vector(1920, 1080), new Vector(-11, -14));
    let width: number = 37;
    let height: number = 69;
    this.emitter = new Emitter(new Vector(800, 460));
    this.emitter.setRectShape(width, height);
    this.emitter.saturation.target = 0.9;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.hue = new TargetValue(40, 0, 0, 60);
    //this.emitter.hue.binding = TargetBinding.rock;
    this.emitter.hue.absoluteVariance = 10;
    //this.emitter.hue.target = 240;
    //this.emitter.hue.drift = 0.6;
    this.emitter.brightness.target = 0.7;
    this.emitter.brightness.relativeVariance = 0.5;
    this.emitter.particlesPerSecond = 300;

    this.emitter.particleRadius.target = 1.5;
    this.emitter.particleRadius.relativeVariance = 0.3;

    this.emitter.particleLifeSpanSeconds = 1.7;
    this.emitter.particleGravity = 0;
    this.emitter.particleInitialVelocity.target = 0.2;
    this.emitter.particleInitialVelocity.relativeVariance = 0.5;
    this.emitter.particleMaxOpacity = 0.5;
    //this.emitter.airDensity = 0.2; // 0 == vaccuum.
    this.emitter.particleAirDensity = 0;  // 0 == vaccuum.
    //this.emitter.wind = new Vector(0, 0);
    //this.emitter.particleWind = new Vector(0, -3);
  }

  wind(): void {
    this.emitter = new Emitter(new Vector(1920 / 2, 1080 / 2));
    this.emitter.setRectShape(400, 1);
    this.emitter.saturation.target = 0.9;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.hue.absoluteVariance = 10;
    this.emitter.hue.target = 240;
    this.emitter.hue.drift = 0.6;
    this.emitter.brightness.target = 0.7;
    this.emitter.brightness.relativeVariance = 0.5;
    this.emitter.particlesPerSecond = 100;

    this.emitter.particleRadius.target = 2.5;
    this.emitter.particleRadius.relativeVariance = 0.8;

    this.emitter.particleLifeSpanSeconds = 12;
    this.emitter.particleGravity = -0.2;
    this.emitter.particleInitialVelocity.target = 0;
    this.emitter.particleInitialVelocity.relativeVariance = 0.5;
    this.emitter.gravity = 0;
    this.emitter.airDensity = 0; // 0 == vaccuum.
    this.emitter.particleAirDensity = 0.1;  // 0 == vaccuum.
    this.emitter.particleWind = new Vector(2, -0.1);
  }

  blood(): void {
    this.emitter = new Emitter(new Vector(450, 1080));
    this.emitter.radius = 1;
    this.emitter.saturation.target = 0.9;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.hue.absoluteVariance = 10;
    this.emitter.hue.target = 0;
    this.emitter.brightness.target = 0.4;
    this.emitter.brightness.relativeVariance = 0.3;
    this.emitter.particlesPerSecond = 1400;

    this.emitter.particleRadius.target = 2.5;
    this.emitter.particleRadius.relativeVariance = 0.8;

    this.emitter.particleLifeSpanSeconds = 2;
    this.emitter.particleGravity = 9.8;
    this.emitter.particleInitialVelocity.target = 0.8;
    this.emitter.particleInitialVelocity.relativeVariance = 0.25;
    this.emitter.gravity = 0;
    this.emitter.airDensity = 0; // 0 == vaccuum.
    this.emitter.particleAirDensity = 0.1;  // 0 == vaccuum.

    let sprayAngle: number = Random.intBetween(270 - 45, 270 + 45);
    let minVelocity: number = 9;
    let maxVelocity: number = 16;
    let angleAwayFromUp: number = Math.abs(sprayAngle - 270);
    if (angleAwayFromUp < 15)
      maxVelocity = 10;
    else if (angleAwayFromUp > 35)
      minVelocity = 13;
    else
      maxVelocity = 18;
    this.emitter.bonusParticleVelocityVector = Vector.fromPolar(sprayAngle, Random.intBetween(minVelocity, maxVelocity));

    this.emitter.renderOldestParticlesLast = true;
  }

  buildSmoke() {
    this.emitter = new Emitter(new Vector(screenCenterX + 90, screenCenterY + 160));
    this.emitter.radius = 66;
    this.emitter.saturation.target = 0;
    this.emitter.brightness.target = 0.8;
    this.emitter.brightness.relativeVariance = 0.1;
    this.emitter.particleRadius.target = 11;
    this.emitter.particleRadius.relativeVariance = 0.7;
    this.emitter.particlesPerSecond = 20;
    this.emitter.particleLifeSpanSeconds = 5;
    this.emitter.particleInitialVelocity.target = 0.15;
    this.emitter.particleGravity = -0.3;
    this.emitter.particleWind = Vector.fromPolar(270, 2);
  }

  orbital() {
    this.emitter = new Emitter(new Vector(1.1 * screenCenterX, screenCenterY), new Vector(0, -6));
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
    this.emitter.particleGravity = -5;
    this.emitter.particleMass = 0;
    this.emitter.particleFadeInTime = 0.05;
    this.emitter.airDensity = 0;
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

  edgeTest() {
    //this.emitter = new Emitter(new Vector(1046, 790));
    this.emitter = new Emitter(new Vector(1920 * 0.5, 1080 / 2));
    //this.emitter.radius = 160;
    this.emitter.setRectShape(300, 100);
    this.emitter.emitterEdgeSpread = 0;
    this.emitter.hue.target = 270;
    this.emitter.hue.absoluteVariance = 75;
    this.emitter.saturation.target = 0.8;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.brightness.target = 0.5;
    this.emitter.particleRadius.target = 1.2;
    this.emitter.particleRadius.relativeVariance = 0.8;
    this.emitter.particlesPerSecond = 2000;
    this.emitter.particleLifeSpanSeconds = 1.5;
    this.emitter.particleInitialVelocity.target = 0.8;
    this.emitter.particleInitialVelocity.relativeVariance = 0.5;
    this.emitter.particleGravity = 0;
    this.emitter.particleFadeInTime = 0.05;
    this.emitter.gravity = 0;
    this.emitter.particleMaxOpacity = 0.4;
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

  blueBall() {
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
    this.emitter.airDensity = 0; // 0 == vaccuum.
    this.emitter.particleAirDensity = 0;  // 0 == vaccuum.
  }

  solo() {
    this.emitter = new Emitter(new Vector(screenCenterX, screenCenterY));
    this.emitter.radius = 0;
    this.emitter.hue.target = 0;
    this.emitter.hue.absoluteVariance = 25;
    this.emitter.saturation.target = 0.8;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.brightness.target = 0.5;
    this.emitter.brightness.relativeVariance = 0.5;
    this.emitter.particleRadius.target = 4;
    this.emitter.particlesPerSecond = 1;
    this.emitter.particleLifeSpanSeconds = 1;
    this.emitter.particleInitialVelocity.target = 10;
    this.emitter.particleGravityCenter = new Vector(screenCenterX, screenCenterY);
    this.emitter.particleGravity = 20;
  }

  rainbow() {
    this.emitter = new Emitter(new Vector(2000, 1350), new Vector(-10, -18));
    this.emitter.radius = 20;
    this.emitter.hue.absoluteVariance = 10;
    this.emitter.hue.target = 240;
    this.emitter.hue.drift = 0.06;
    this.emitter.saturation.target = 0.8;
    this.emitter.saturation.relativeVariance = 0.2;
    this.emitter.brightness.target = 0.5;
    this.emitter.brightness.relativeVariance = 0.5;
    this.emitter.particleRadius.target = 3;
    this.emitter.particleRadius.relativeVariance = 0.3;
    this.emitter.particlesPerSecond = 600;
    this.emitter.particleLifeSpanSeconds = 4.5;
    this.emitter.particleInitialVelocity.target = 0.5;
    //this.emitter.particleGravityCenter = new Vector(screenCenterX, screenCenterY);
    this.emitter.gravity = 3;
    this.emitter.particleGravity = 2;
    this.emitter.particleAirDensity = 0.1;  // 0 == vaccuum.
    this.emitter.particleWind = new Vector(2, -0.1);
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
    else if (command === "Cross") {
      this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
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

    if (testCommand === 'scroll') {
      this.characterStatsScroll.clear();
      this.initializeScroll();
      this.characterStatsScroll.open(this.now);
      return true;
    }

    if (testCommand === 'Emphasis1') {
      this.characterStatsScroll.page = ScrollPage.main;
      this.characterStatsScroll.addEmphasis(emphasisMain.Strength);
      return true;
    }

    if (testCommand === 'Emphasis2') {
      this.characterStatsScroll.page = ScrollPage.main;
      this.characterStatsScroll.addEmphasis(emphasisMain.HitPoints);
      return true;
    }

    if (testCommand === 'Emphasis3') {
      this.characterStatsScroll.page = ScrollPage.skills;
      this.characterStatsScroll.addEmphasis(emphasisSkills.SkillsAnimalHandling);
      return true;
    }

    if (testCommand === '2') {
      this.characterStatsScroll.state = ScrollState.none;
      this.characterStatsScroll.selectedCharacterIndex = 2;
      this.characterStatsScroll.open(this.now);
      return true;
    }

    if (testCommand === '1') {
      this.characterStatsScroll.state = ScrollState.none;
      this.characterStatsScroll.selectedCharacterIndex = 1;
      this.characterStatsScroll.open(this.now);
      return true;
    }

    if (testCommand === '0') {
      this.characterStatsScroll.state = ScrollState.none;
      this.characterStatsScroll.selectedCharacterIndex = 0;
      this.characterStatsScroll.open(this.now);
      return true;
    }

    if (testCommand === 'open') {
      this.characterStatsScroll.open(this.now);
      return true;
    }

    if (testCommand === 'page1') {
      this.characterStatsScroll.page = ScrollPage.main;
      return true;
    }

    if (testCommand === 'page2') {
      this.characterStatsScroll.page = ScrollPage.skills;
      return true;
    }

    if (testCommand === 'smoke') {
      this.world.removeCharacter(this.emitter);
      this.buildSmoke();
      this.world.addCharacter(this.emitter);
      return true;
    }

    if (testCommand === 'orbit') {
      this.world.removeCharacter(this.emitter);
      this.orbital();
      this.world.addCharacter(this.emitter);
      return true;
    }

    if (testCommand === 'edge') {
      this.world.removeCharacter(this.emitter);
      this.edgeTest();
      this.world.addCharacter(this.emitter);
      return true;
    }

    if (testCommand === 'wind') {
      this.world.removeCharacter(this.emitter);
      this.wind();
      this.world.addCharacter(this.emitter);
      return true;
    }

    //if (testCommand === 'blood') {
    //  this.world.removeCharacter(this.emitter);
    //  this.blood();
    //  this.world.addCharacter(this.emitter);
    //  return true;
    //}

    if (testCommand === 'gold') {
      this.world.removeCharacter(this.emitter);
      this.buildTestGoldParticle();
      this.world.addCharacter(this.emitter);
      return true;
    }

    if (testCommand === 'solo') {
      this.world.removeCharacter(this.emitter);
      this.solo();
      this.world.addCharacter(this.emitter);
      return true;
    }
    if (testCommand === 'rainbow') {
      this.world.removeCharacter(this.emitter);
      this.rainbow();
      this.world.addCharacter(this.emitter);
      return true;
    }

    if (testCommand === 'purpleMagic') {
      this.world.removeCharacter(this.emitter);
      this.purpleMagic();
      this.world.addCharacter(this.emitter);
      return true;
    }

    if (testCommand === 'purpleBurst') {
      this.world.removeCharacter(this.emitter);
      this.purpleBurst();
      this.world.addCharacter(this.emitter);
      return true;
    }

    if (testCommand === 'blueBall') {
      this.world.removeCharacter(this.emitter);
      this.blueBall();
      this.world.addCharacter(this.emitter);
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