enum EmitterShape {
  Circular = 1,
  Rectangular = 2
}

enum TargetType {
  ActivePlayer = 0,
  ActiveEnemy = 1,
  ScrollPosition = 2,
  ScreenPosition = 3
}

enum EffectKind {
  Animation = 0,
  Emitter = 1,
  SoundEffect = 2,
  GroupEffect = 3,
  Placeholder = 4
}

class DragonFrontGame extends GamePlusQuiz {
  emitter: Emitter;
  shouldDrawCenterCrossHairs: boolean = false;
  denseSmoke: Sprites;
  fireBallBack: Sprites;
  fireBallFront: Sprites;
  poof: Sprites;
  bloodGush: Sprites;
  bloodLarger: Sprites;
  bloodLarge: Sprites;
  bloodMedium: Sprites;
  bloodSmall: Sprites;
  bloodSmaller: Sprites;
  bloodSmallest: Sprites;
  charmed: Sprites;
  restrained: Sprites;
  sparkShower: Sprites;
  embersLarge: Sprites;
  embersMedium: Sprites;
  stars: Sprites;
  fumes: Sprites;
  allEffects: SpriteCollection;

  constructor(context: CanvasRenderingContext2D) {
    super(context);
  }

  update(timestamp: number) {
    this.updateGravity();
    super.update(timestamp);
  }

  updateScreen(context: CanvasRenderingContext2D, now: number) {
    this.allEffects.draw(context, now);

    super.updateScreen(context, now);

    if (this.shouldDrawCenterCrossHairs)
      drawCrossHairs(myContext, screenCenterX, screenCenterY);
  }

  removeAllGameElements(now: number): void {
    super.removeAllGameElements(now);
  }

  initialize() {
    super.initialize();
    gravityGames = new GravityGames();
    //Folders.assets = 'GameDev/Assets/DroneGame/';
  }

  start() {
    super.start();
    gravityGames.selectPlanet('Earth');
    gravityGames.newGame();

    this.updateGravity();
    if (this.emitter)
      this.world.addCharacter(this.emitter);
  }

  loadResources(): void {
    super.loadResources();
    Folders.assets = 'GameDev/Assets/DragonH/';
    const fps30: number = 33;
    const fps20: number = 50;
    const fps15: number = 67;
    this.denseSmoke = new Sprites('Smoke/Dense/DenseSmoke', 116, fps30, AnimationStyle.Sequential, true);
    this.denseSmoke.name = 'DenseSmoke';
    this.denseSmoke.originX = 309;
    this.denseSmoke.originY = 723;

    this.fireBallBack = new Sprites('FireBall/Back/BackFireBall', 88, fps30, AnimationStyle.Sequential, true);
    this.fireBallBack.name = 'FireBallBack';
    this.fireBallBack.originX = 190;
    this.fireBallBack.originY = 1080;

    this.fireBallFront = new Sprites('FireBall/Front/FireBallFront', 88, fps30, AnimationStyle.Sequential, true);
    this.fireBallFront.name = 'FireBallFront';
    this.fireBallFront.originX = 190;
    this.fireBallFront.originY = 1080;

    this.poof = new Sprites('Smoke/Poof/Poof', 67, fps30, AnimationStyle.Sequential, true);
    this.poof.name = 'Puff';
    this.poof.originX = 229;
    this.poof.originY = 698;

    this.sparkShower = new Sprites('Sparks/Big/BigSparks', 64, fps30, AnimationStyle.Sequential, true);
    this.sparkShower.name = 'SparkShower';
    this.sparkShower.originX = 443;
    this.sparkShower.originY = 595;

    this.embersLarge = new Sprites('Embers/Large/EmberLarge', 93, fps30, AnimationStyle.Sequential, true);
    this.embersLarge.name = 'EmbersLarge';
    this.embersLarge.originX = 504;
    this.embersLarge.originY = 501;

    this.embersMedium = new Sprites('Embers/Medium/EmberMedium', 91, fps30, AnimationStyle.Sequential, true);
    this.embersMedium.name = 'EmbersMedium';
    this.embersMedium.originX = 431;
    this.embersMedium.originY = 570;

    this.stars = new Sprites('SpinningStars/SpinningStars', 120, fps20, AnimationStyle.Loop, true);
    this.stars.name = 'Stars';
    this.stars.originX = 155;
    this.stars.originY = 495;

    this.fumes = new Sprites('Fumes/Fumes', 112, fps30, AnimationStyle.Loop, true);
    this.fumes.name = 'Fumes';
    this.fumes.originX = 281;
    this.fumes.originY = 137;

    this.bloodGush = new Sprites('Blood/BloodGush/BloodGush', 77, fps30, AnimationStyle.Sequential, true);
    this.bloodGush.name = 'BloodGush';
    this.bloodGush.originX = 39;
    this.bloodGush.originY = 1075;

    this.bloodLarger = new Sprites('Blood/BloodLarger/BloodLarger', 49, fps15, AnimationStyle.Sequential, true);
    this.bloodLarger.name = 'BloodLarger';
    this.bloodLarger.originX = 471;
    this.bloodLarger.originY = 678;

    this.bloodLarge = new Sprites('Blood/BloodLarge/BloodLarge', 49, fps15, AnimationStyle.Sequential, true);
    this.bloodLarge.name = 'BloodLarge';
    this.bloodLarge.originX = 378;
    this.bloodLarge.originY = 547;

    this.bloodMedium = new Sprites('Blood/BloodMedium/BloodMedium', 49, fps15, AnimationStyle.Sequential, true);
    this.bloodMedium.name = 'BloodMedium';
    this.bloodMedium.originX = 328;
    this.bloodMedium.originY = 647;

    this.bloodSmall = new Sprites('Blood/BloodSmall/BloodSmall', 49, fps15, AnimationStyle.Sequential, true);
    this.bloodSmall.name = 'BloodSmall';
    this.bloodSmall.originX = 246;
    this.bloodSmall.originY = 498;

    this.bloodSmaller = new Sprites('Blood/BloodSmaller/BloodSmaller', 49, fps15, AnimationStyle.Sequential, true);
    this.bloodSmaller.name = 'BloodSmaller';
    this.bloodSmaller.originX = 191;
    this.bloodSmaller.originY = 348;

    this.bloodSmallest = new Sprites('Blood/BloodSmallest/BloodSmallest', 49, fps15, AnimationStyle.Sequential, true);
    this.bloodSmallest.name = 'BloodSmallest';
    this.bloodSmallest.originX = 150;
    this.bloodSmallest.originY = 300;

    this.charmed = new Sprites('Charmed/Charmed', 179, fps30, AnimationStyle.Loop, true);
    this.charmed.name = 'Heart';
    this.charmed.originX = 155;
    this.charmed.originY = 269;

    this.restrained = new Sprites('Restrained/Chains', 20, fps30, AnimationStyle.Loop, true);
    this.restrained.name = 'Restrained';
    this.restrained.originX = 213;
    this.restrained.originY = 299;

    this.allEffects = new SpriteCollection();
    this.allEffects.add(this.denseSmoke);
    this.allEffects.add(this.poof);
    this.allEffects.add(this.fireBallBack);
    this.allEffects.add(this.fireBallFront);
    this.allEffects.add(this.stars);
    this.allEffects.add(this.fumes);
    this.allEffects.add(this.sparkShower);
    this.allEffects.add(this.embersLarge);
    this.allEffects.add(this.embersMedium);
    this.allEffects.add(this.bloodGush);
    this.allEffects.add(this.bloodLarger);
    this.allEffects.add(this.bloodLarge);
    this.allEffects.add(this.bloodMedium);
    this.allEffects.add(this.bloodSmall);
    this.allEffects.add(this.bloodSmaller);
    this.allEffects.add(this.bloodSmallest);
    this.allEffects.add(this.charmed);
    this.allEffects.add(this.restrained);
  }
  executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    if (super.executeCommand(command, params, userId, userName, displayName, color, now))
      return true;
    if (command === "Cross2") {
      this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
    }
  }

  testBloodEmitter(): void {
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
    this.emitter.maxTotalParticles = 2000;
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

  test(testCommand: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    if (super.test(testCommand, userId, userName, displayName, color, now))
      return true;

    if (testCommand === "Cross2") {
      console.log('draw Cross Hairs');
      this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
    }

    if (testCommand === 'clear') {
      this.world.removeCharacter(this.emitter);
      this.testBloodEmitter();
      this.world.addCharacter(this.emitter);
      return true;
    }

    if (testCommand === 'blood') {
      console.log('Blood');
      this.world.removeCharacter(this.emitter);
      this.testBloodEmitter();
      this.world.addCharacter(this.emitter);
      return true;
    }

    // poof 40 50
    if (testCommand.startsWith("poof")) {
      let split: string[] = testCommand.split(' ');

      let hue: number = 0;
      let saturation: number = 100;
      let brightness: number = 100;

      if (split.length === 2) {
        hue = +split[1];
      }
      else if (split.length === 3) {
        hue = +split[1];
        saturation = +split[2];
      }
      else if (split.length > 3) {
        hue = +split[1];
        saturation = +split[2];
        brightness = +split[3];
      }

      this.denseSmoke.sprites.push(new ColorShiftingSpriteProxy(0, new Vector(450 - this.denseSmoke.originX, 1080 - this.denseSmoke.originY)).setHueSatBrightness(hue, saturation, brightness));
    }


    if (testCommand.startsWith("fb")) {
      let split: string[] = testCommand.split(' ');

      let hue: number = 0;
      let saturation: number = 100;
      let brightness: number = 100;

      if (split.length === 2) {
        hue = +split[1];
      }
      else if (split.length === 3) {
        hue = +split[1];
        saturation = +split[2];
      }
      else if (split.length > 3) {
        hue = +split[1];
        saturation = +split[2];
        brightness = +split[3];
      }

      this.fireBallBack.sprites.push(new SpriteProxy(0, 450 - this.fireBallBack.originX, 1080 - this.fireBallBack.originY));
      this.fireBallFront.sprites.push(new ColorShiftingSpriteProxy(0, new Vector(450 - this.fireBallFront.originX, 1080 - this.fireBallFront.originY)).setHueSatBrightness(hue, saturation, brightness));
    }

    return false;
  }

  readonly player1X: number = 246;
  readonly player2X: number = 562;
  readonly player3X: number = 879;
  readonly player4X: number = 1195;
  activePlayerX: number = this.player1X;

  playerChanged(playerID: number): void {
    if (playerID === 0)
      this.activePlayerX = this.player1X;
    else if (playerID === 1)
      this.activePlayerX = this.player2X;
    else if (playerID === 2)
      this.activePlayerX = this.player3X;
    else if (playerID === 3)
      this.activePlayerX = this.player4X;
  }

  getCenter(target: any): Vector {
    let result: Vector;
    if (target.targetType === TargetType.ScreenPosition)
      result = new Vector(target.screenPosition.x, target.screenPosition.y);
    else if (target.targetType === TargetType.ActivePlayer)
      result = new Vector(this.activePlayerX, 1080);
    else if (target.targetType === TargetType.ActiveEnemy)
      result = new Vector(1260, 1080);
    else if (target.targetType === TargetType.ScrollPosition)
      result = new Vector(150, 400);
    else
      result = new Vector(960, 540);

    return result.add(new Vector(target.targetOffset.x, target.targetOffset.y));
  }

  triggerSingleEffect(dto: any) {
    if (dto.timeOffsetMs > 0) {
      let offset: number = dto.timeOffsetMs;
      dto.timeOffsetMs = -1;
      setTimeout(this.triggerSingleEffect.bind(this), offset, dto);
      return;
    }

    if (dto.effectKind === EffectKind.SoundEffect) {
      this.triggerSoundEffect(dto);
      return;
    }

    if (dto.effectKind === EffectKind.Placeholder) {
      this.triggerPlaceholder(dto);
      return;
    }

    let center: Vector = this.getCenter(dto.target);

    if (dto.effectKind === EffectKind.Animation)
      this.triggerAnimation(dto, center);
    else if (dto.effectKind === EffectKind.Emitter)
      this.triggerEmitter(dto, center);
  }

  triggerPlaceholder(dto: any): any {
    console.log('triggerPlaceholder - dto: ' + dto);
  }

  triggerEffect(effectData: string): void {
    let dto: any = JSON.parse(effectData);
    console.log(dto);

    if (dto.effectKind === EffectKind.GroupEffect) {
      for (var i = 0; i < dto.effectsCount; i++) {
        this.triggerSingleEffect(dto.effects[i]);
      }
    }
    else {
      this.triggerSingleEffect(dto);
    }
  }

  triggerSoundEffect(dto: any): void {
    console.log("Playing " + Folders.assets + 'SoundEffects/' + dto.soundFileName);
    new Audio(Folders.assets + 'SoundEffects/' + dto.soundFileName).play();
  }

  triggerEmitter(dto: any, center: Vector): void {
    this.world.removeCharacter(this.emitter);

    console.log('emitter: ' + dto);

    this.emitter = new Emitter(new Vector(center.x, center.y), new Vector(dto.emitterInitialVelocity.x, dto.emitterInitialVelocity.y));
    if (dto.emitterShape === EmitterShape.Circular) {
      this.emitter.radius = dto.radius;
    }
    else {
      this.emitter.setRectShape(dto.width, dto.height);
    }

    this.emitter.particleMass = dto.particleMass;
    this.emitter.initialParticleDirection = new Vector(dto.particleInitialDirection.x, dto.particleInitialDirection.y);
    this.emitter.particleWind = new Vector(dto.particleWindDirection.x, dto.particleWindDirection.y);
    this.emitter.minParticleSize = dto.minParticleSize;
    this.emitter.gravityCenter = new Vector(dto.gravityCenter.x, dto.gravityCenter.y);
    this.emitter.particleFadeInTime = dto.fadeInTime;
    this.emitter.wind = new Vector(dto.emitterWindDirection.x, dto.emitterWindDirection.y);

    this.transferTargetValue(this.emitter.brightness, dto.brightness);
    this.transferTargetValue(this.emitter.hue, dto.hue);
    this.transferTargetValue(this.emitter.saturation, dto.saturation);

    this.emitter.maxConcurrentParticles = dto.maxConcurrentParticles;
    this.emitter.particleMaxOpacity = dto.maxOpacity;

    this.emitter.particlesPerSecond = dto.particlesPerSecond;

    this.transferTargetValue(this.emitter.particleRadius, dto.particleRadius);

    this.emitter.emitterEdgeSpread = dto.edgeSpread;

    this.emitter.particleLifeSpanSeconds = dto.lifeSpan;
    if (dto.maxTotalParticles === 'Infinity') {
      this.emitter.maxTotalParticles = Infinity;
    }
    else {
      this.emitter.maxTotalParticles = dto.maxTotalParticles;
    }

    this.emitter.particleGravity = dto.particleGravity;
    this.emitter.particleGravityCenter = new Vector(dto.particleGravityCenter.x, dto.particleGravityCenter.y);
    this.transferTargetValue(this.emitter.particleInitialVelocity, dto.particleInitialVelocity);

    this.emitter.gravity = dto.emitterGravity;
    this.emitter.airDensity = dto.emitterAirDensity;
    this.emitter.particleAirDensity = dto.particleAirDensity;

    this.emitter.bonusParticleVelocityVector = new Vector(dto.bonusVelocityVector.x, dto.bonusVelocityVector.y);

    this.emitter.renderOldestParticlesLast = dto.renderOldestParticlesLast;

    this.world.addCharacter(this.emitter);
    console.log(this.emitter);
  }


  private transferTargetValue(brightness: TargetValue, anyTargetValue: any): void {
    brightness.target = anyTargetValue.value;
    brightness.relativeVariance = anyTargetValue.relativeVariance;
    brightness.absoluteVariance = anyTargetValue.absoluteVariance;
    brightness.drift = anyTargetValue.drift;
    brightness.binding = anyTargetValue.targetBinding;
  }

  private triggerAnimation(dto: any, center: Vector) {
    let sprites: Sprites;
    //console.log('dto.spriteName: ' + dto.spriteName);
    if (dto.spriteName === 'DenseSmoke')
      sprites = this.denseSmoke;
    else if (dto.spriteName === 'Poof')
      sprites = this.poof;
    else if (dto.spriteName === 'BloodGush')
      sprites = this.bloodGush;
    else if (dto.spriteName === 'BloodLarger')
      sprites = this.bloodLarger;
    else if (dto.spriteName === 'BloodLarge')
      sprites = this.bloodLarge;
    else if (dto.spriteName === 'BloodMedium')
      sprites = this.bloodMedium;
    else if (dto.spriteName === 'BloodSmall')
      sprites = this.bloodSmall;
    else if (dto.spriteName === 'BloodSmaller')
      sprites = this.bloodSmaller;
    else if (dto.spriteName === 'BloodSmallest')
      sprites = this.bloodSmallest;
    else if (dto.spriteName === 'Heart')
      sprites = this.charmed;
    else if (dto.spriteName === 'Restrained')
      sprites = this.restrained;
    else if (dto.spriteName === 'SparkShower')
      sprites = this.sparkShower;
    else if (dto.spriteName === 'EmbersLarge')
      sprites = this.embersLarge;
    else if (dto.spriteName === 'EmbersMedium')
      sprites = this.embersMedium;
    else if (dto.spriteName === 'Stars')
      sprites = this.stars;
    else if (dto.spriteName === 'Fumes')
      sprites = this.fumes;
    else if (dto.spriteName === 'FireBall')
      sprites = this.fireBallBack;
    let spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(center), dto.startFrameIndex, dto.hueShift, dto.saturation, dto.brightness);
    spritesEffect.start();

    if (dto.spriteName === 'FireBall') {
      sprites = this.fireBallFront;
      let spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(center), dto.startFrameIndex, dto.secondaryHueShift, dto.secondarySaturation, dto.secondaryBrightness);
      spritesEffect.start();
    }

  }
} 