enum DiceRollKind {
  Normal,
  Advantage,
  Disadvantage
}

enum DiceRollType {
  SkillCheck,
  Attack,
  SavingThrow,
  FlatD20,
  DeathSavingThrow,
  PercentageRoll
}

class DiceLayer {
  diceFrontCanvas: HTMLCanvasElement;
  diceBackCanvas: HTMLCanvasElement;
  diceFrontContext: CanvasRenderingContext2D;
  diceBackContext: CanvasRenderingContext2D;
  diceFireball: Sprites;
  d20Fire: Sprites;
  roll1Stink: Sprites;
  diceSparks: Sprites;
  dicePortal: Sprites;
  dicePortalTop: Sprites;
  magicRing: Sprites;
  diceBlowColoredSmoke: Sprites;
  diceBombBase: Sprites;
  diceBombTop: Sprites;
  dieSteampunkTunnel: Sprites;
  allFrontLayerEffects: SpriteCollection;
  allBackLayerEffects: SpriteCollection;

  constructor() {
    this.loadSprites();
  }

  loadSprites() {
    Part.loadSprites = true;

    const fps40: number = 1000 / 40;
    const fps30: number = 33;
    const fps20: number = 50;

    globalBypassFrameSkip = true;

    this.allFrontLayerEffects = new SpriteCollection();
    this.allBackLayerEffects = new SpriteCollection();

    this.diceFireball = new Sprites("/Dice/Roll20Fireball/DiceFireball", 71, fps30, AnimationStyle.Sequential, true);
    this.diceFireball.originX = 104;
    this.diceFireball.originY = 155;
    this.allFrontLayerEffects.add(this.diceFireball);

    this.d20Fire = new Sprites("/Dice/D20Fire/D20Fire", 180, fps30, AnimationStyle.Loop, true);
    this.d20Fire.originX = 151;
    this.d20Fire.originY = 149;
    this.d20Fire.returnFrameIndex = 72;
    this.allBackLayerEffects.add(this.d20Fire);

    this.dicePortal = new Sprites("/Dice/DiePortal/DiePortal", 73, fps30, AnimationStyle.Sequential, true);
    this.dicePortal.originX = 189;
    this.dicePortal.originY = 212;
    this.allBackLayerEffects.add(this.dicePortal);

    this.dicePortalTop = new Sprites("/Dice/DiePortal/DiePortalTop", 73, fps30, AnimationStyle.Sequential, true);
    this.dicePortalTop.originX = 189;
    this.dicePortalTop.originY = 212;
    this.allFrontLayerEffects.add(this.dicePortalTop);

    this.roll1Stink = new Sprites("/Dice/Roll1/Roll", 172, fps30, AnimationStyle.Loop, true);
    this.roll1Stink.originX = 150;
    this.roll1Stink.originY = 150;
    this.allBackLayerEffects.add(this.roll1Stink);

    this.diceSparks = new Sprites("/Dice/Sparks/Spark", 49, fps20, AnimationStyle.Loop, true);
    this.diceSparks.originX = 170;
    this.diceSparks.originY = 158;
    this.allBackLayerEffects.add(this.diceSparks);

    this.magicRing = new Sprites("/Dice/MagicRing/MagicRing", 180, fps40, AnimationStyle.Loop, true);
    this.magicRing.returnFrameIndex = 60;
    this.magicRing.originX = 140;
    this.magicRing.originY = 112;
    this.allFrontLayerEffects.add(this.magicRing);

    this.diceBlowColoredSmoke = new Sprites("/Dice/Blow/DiceBlow", 41, fps40, AnimationStyle.Sequential, true);
    this.diceBlowColoredSmoke.originX = 178;
    this.diceBlowColoredSmoke.originY = 170;
    this.allFrontLayerEffects.add(this.diceBlowColoredSmoke);

    this.diceBombBase = new Sprites("/Dice/DieBomb/DieBombBase", 49, fps30, AnimationStyle.Sequential, true);
    this.diceBombBase.originX = 295;
    this.diceBombBase.originY = 316;
    this.allBackLayerEffects.add(this.diceBombBase);

    this.dieSteampunkTunnel = new Sprites("/Dice/SteampunkTunnel/SteampunkTunnelBack", 178, 28, AnimationStyle.Sequential, true);
    this.dieSteampunkTunnel.originX = 142;
    this.dieSteampunkTunnel.originY = 145;
    this.allBackLayerEffects.add(this.dieSteampunkTunnel);

    this.diceBombTop = new Sprites("/Dice/DieBomb/DieBombTop", 39, fps30, AnimationStyle.Sequential, true);
    this.diceBombTop.originX = 295;
    this.diceBombTop.originY = 316;
    this.allFrontLayerEffects.add(this.diceBombTop);
  }

  mouseDownInCanvas(e) {
    if (effectOverride != undefined) {
      var enumIndex: number = <number>effectOverride;
      let totalElements: number = Object.keys(DieEffect).length / 2;
      enumIndex++;
      if (enumIndex >= totalElements)
        enumIndex = 0;
      effectOverride = <DieEffect>enumIndex;
      console.log('New effect: ' + DieEffect[effectOverride]);
    }
  }

  getContext() {
    this.diceFrontCanvas = <HTMLCanvasElement>document.getElementById("diceFrontCanvas");
    this.diceFrontCanvas.onmousedown = this.mouseDownInCanvas;
    this.diceBackCanvas = <HTMLCanvasElement>document.getElementById("diceBackCanvas");
    this.diceFrontContext = this.diceFrontCanvas.getContext("2d");
    this.diceBackContext = this.diceBackCanvas.getContext("2d");
  }

  renderCanvas() {
    if (!this.diceFrontContext || !this.diceBackContext)
      this.getContext();

    this.diceFrontContext.clearRect(0, 0, 1920, 1080);
    this.diceBackContext.clearRect(0, 0, 1920, 1080);
    var now: number = performance.now();
    this.allFrontLayerEffects.updatePositions(now);
    this.allBackLayerEffects.updatePositions(now);
    this.allFrontLayerEffects.draw(this.diceFrontContext, now);
    this.allBackLayerEffects.draw(this.diceBackContext, now);
  }

  testFireball(x: number, y: number) {
    this.diceFireball.add(x, y, 0).rotation = 90;
  }

  addMagicRing(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
    let magicRing = this.magicRing.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
    magicRing.rotation = Math.random() * 360;
    return magicRing;
  }

  blowColoredSmoke(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    this.diceBlowColoredSmoke.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
  }

  testD20Fire(x: number, y: number) {
    this.d20Fire.add(x, y).rotation = Math.random() * 360;
  }

  testRoll1Stink(x: number, y: number) {
    this.roll1Stink.add(x, y).rotation = Math.random() * 360;
  }

  clearLoopingAnimations(): any {
    this.magicRing.sprites = [];
    this.d20Fire.sprites = [];
    this.roll1Stink.sprites = [];
  }


  testSteampunkTunnel(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    // no rotation on SteampunkTunnel - shadows built to 
    this.dieSteampunkTunnel.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
  }

  testDiceBomb(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    this.diceBombBase.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
    this.diceBombTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
  }

  testPortal(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    this.dicePortal.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
    this.dicePortalTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
  }

  getDiceRollData(diceRollData: string): DiceRollData {
    let dto: any = JSON.parse(diceRollData);
    let diceRoll: DiceRollData = new DiceRollData();
    diceRoll.type = dto.Type;
    diceRoll.kind = dto.Kind;
    diceRoll.damageDice = dto.DamageDice;
    diceRoll.modifier = dto.Modifier;
    diceRoll.hiddenThreshold = dto.HiddenThreshold;
    diceRoll.isMagic = dto.IsMagic;
    return diceRoll;
  }

  spark(x: number, y: number): SpriteProxy {
    let spark = this.diceSparks.addShifted(x, y, Math.round(Math.random() * this.diceSparks.sprites.length), Math.random() * 360);
    spark.expirationDate = performance.now() + 180;
    spark.fadeOutTime = 0;
    spark.opacity = 0.8;
    return spark;
  }

  rollDice(diceRollDto: string): any {
    let diceRollData: DiceRollData = this.getDiceRollData(diceRollDto);
    console.log(diceRollData);
    pleaseRollDice(diceRollData);

    //  { "Type": 1, "Kind": 0, "DamageDice": "2d8+6", "Modifier": 1.0, "HiddenThreshold": 12.0, "IsMagic": true }
  }
}

class DiceRollData {
  type: DiceRollType;
  kind: DiceRollKind;
  damageDice: string;
  modifier: number;
  hiddenThreshold: number;
  isMagic: boolean;
  constructor() {

  }
}
