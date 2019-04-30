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
  PercentageRoll,
  WildMagic
}

class TextEffects {
  textEffects: Array<TextEffect> = new Array<TextEffect>();
	constructor() {
		
  }

  clear() {
    this.textEffects = new Array<TextEffect>();
  }

  add(centerPos: Vector, text: string): TextEffect {
    let textEffect: TextEffect = new TextEffect();
    textEffect.center = centerPos;
    textEffect.text = text;
    this.textEffects.push(textEffect);
    return textEffect;
  }

  render(context: CanvasRenderingContext2D, now: number) {
    this.textEffects.forEach(function (textEffect: TextEffect) {
      textEffect.render(context, now);
    });
  }
}

class TextEffect {
  text: string;
  fontColor: string;
  fontName: string;
  outlineColor: string;
  outlineThickness: number;
  center: Vector;
  fontSize: number;
  opacity: number;
  scale: number;

	constructor() {
    this.text = 'test';
    this.fontColor = '#ffffff';
    this.outlineColor = '#000000';
    this.outlineThickness = 1;
    this.center = new Vector(960, 540);
    this.fontSize = 18;
    this.opacity = 1;
    this.scale = 1;
  }

  render(context: CanvasRenderingContext2D, now: number) {
    const yOffset: number = 60;
    context.font = `${this.fontSize * this.scale}px ${this.fontName}`;
    context.textAlign = 'center';
    context.textBaseline = 'middle';
    context.fillStyle = this.fontColor;
    context.globalAlpha = this.opacity;
    context.strokeStyle = this.outlineColor;
    context.lineWidth = this.outlineThickness * this.scale * 2; // Half the stroke is outside the font.
    context.lineJoin = "round";
    context.strokeText(this.text, this.center.x, this.center.y + yOffset);
    context.fillText(this.text, this.center.x, this.center.y + yOffset);
    context.globalAlpha = 1;
  }
}

class DiceLayer {
  static matchOozeToDieColor: boolean = false;
  textEffects: TextEffects = new TextEffects();
  diceFrontCanvas: HTMLCanvasElement;
  diceBackCanvas: HTMLCanvasElement;
  diceFrontContext: CanvasRenderingContext2D;
  diceBackContext: CanvasRenderingContext2D;
  diceFireball: Sprites;
  d20Fire: Sprites;
  roll1Stink: Sprites;
  diceSparks: Sprites;
  pawPrints: Sprites;
  stars: Sprites;
  dicePortal: Sprites;
  handGrabsDiceTop: Sprites;
  handGrabsDiceBottom: Sprites;
  dicePortalTop: Sprites;
  magicRing: Sprites;
  diceBlowColoredSmoke: Sprites;
  diceBombBase: Sprites;
  diceBombTop: Sprites;
  dieSteampunkTunnel: Sprites;
  allFrontLayerEffects: SpriteCollection;
  allBackLayerEffects: SpriteCollection;
  activePlayerDieColor: string = '#fcd5a6';
  activePlayerSpecialDieColor: string = '#4e2c04';
  activePlayerDieFontColor: string = '#000000';
  activePlayerSpecialDieFontColor: string = '#ffffff';
  activePlayerHueShift: number = 0;

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

    this.pawPrints = new Sprites("/Dice/TigerPaw/TigerPaw", 60, fps30, AnimationStyle.Sequential, true);
    this.pawPrints.originX = 38;
    this.pawPrints.originY = 63;
    this.allBackLayerEffects.add(this.pawPrints);

    this.stars = new Sprites("/Dice/Star/Star", 60, fps30, AnimationStyle.Loop, true);
    this.stars.originX = 170;
    this.stars.originY = 165;
    this.allBackLayerEffects.add(this.stars);

    this.d20Fire = new Sprites("/Dice/D20Fire/D20Fire", 180, fps30, AnimationStyle.Loop, true);
    this.d20Fire.originX = 151;
    this.d20Fire.originY = 149;
    this.d20Fire.returnFrameIndex = 72;
    this.allBackLayerEffects.add(this.d20Fire);

    this.dicePortal = new Sprites("/Dice/DiePortal/DiePortal", 73, fps30, AnimationStyle.Sequential, true);
    this.dicePortal.originX = 189;
    this.dicePortal.originY = 212;
    this.allBackLayerEffects.add(this.dicePortal);

    this.handGrabsDiceTop = new Sprites("/Dice/HandGrab/HandGrabsDiceTop", 54, fps30, AnimationStyle.Sequential, true);
    this.handGrabsDiceTop.originX = 153;
    this.handGrabsDiceTop.originY = 127;
    this.allFrontLayerEffects.add(this.handGrabsDiceTop);

    this.handGrabsDiceBottom = new Sprites("/Dice/HandGrab/HandGrabsDiceBottom", 54, fps30, AnimationStyle.Sequential, true);
    this.handGrabsDiceBottom.originX = 153;
    this.handGrabsDiceBottom.originY = 127;
    this.allBackLayerEffects.add(this.handGrabsDiceBottom);

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

  addDieValueLabel(centerPos: Vector, value: string, highlight: boolean = false) {
    let textEffect: TextEffect = this.textEffects.add(centerPos, value);
    if (highlight)
      textEffect.fontColor = '#ff0000';
    textEffect.scale = 4;
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
    this.textEffects.render(this.diceFrontContext, now);
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

  clearResidualEffects(): any {
    this.magicRing.sprites = [];
    this.stars.sprites = [];
    this.d20Fire.sprites = [];
    this.roll1Stink.sprites = [];
    this.textEffects.clear();
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

  testDiceGrab(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    var handRotation: number = 90 - Math.random() * 180;
    this.handGrabsDiceBottom.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = handRotation;
    this.handGrabsDiceTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = handRotation;
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
    diceRoll.isSneakAttack = dto.IsSneakAttack;
    diceRoll.isPaladinSmiteAttack = dto.IsPaladinSmiteAttack;
    diceRoll.isWildAnimalAttack = dto.IsWildAnimalAttack;
    return diceRoll;
  }

  spark(x: number, y: number): SpriteProxy {
    let spark = this.diceSparks.addShifted(x, y, Math.round(Math.random() * this.diceSparks.sprites.length), Math.random() * 360);
    spark.expirationDate = performance.now() + 180;
    spark.fadeOutTime = 0;
    spark.opacity = 0.8;
    return spark;
  }

  addPawPrint(x: number, y: number, angle: number): SpriteProxy {
    let pawPrint = this.pawPrints.addShifted(x, y, Math.round(Math.random() * this.pawPrints.sprites.length), diceLayer.activePlayerHueShift);
    pawPrint.rotation = angle;
    pawPrint.expirationDate = performance.now() + 2000;
    pawPrint.fadeOutTime = 500;
    pawPrint.fadeInTime = 500;
    pawPrint.opacity = 0.8;
    return pawPrint;
  }

  addStar(x: number, y: number): SpriteProxy {
    let star = this.stars.addShifted(x, y, Math.round(Math.random() * this.stars.sprites.length), diceLayer.activePlayerHueShift);
    star.autoRotationDegeesPerSecond = 15 + Math.round(Math.random() * 20);
    if (Math.random() < 0.5)
      star.autoRotationDegeesPerSecond = -star.autoRotationDegeesPerSecond;
    star.fadeInTime = 1000;
    star.fadeOutTime = 500;
    star.opacity = 0.75;
    return star;
  }

  clearDice(): void {
    removeRemainingDice();
  }

  rollDice(diceRollDto: string): void {
    let diceRollData: DiceRollData = this.getDiceRollData(diceRollDto);
    console.log(diceRollData);
    pleaseRollDice(diceRollData);

    //  { "Type": 1, "Kind": 0, "DamageDice": "2d8+6", "Modifier": 1.0, "HiddenThreshold": 12.0, "IsMagic": true }
  }

  playerChanged(playerID: number): void {
    this.activePlayerDieFontColor = '#000000';
    this.activePlayerSpecialDieFontColor = '#ffffff';
    if (playerID === 0) {
      this.activePlayerDieColor = '#fcd5a6';
      this.activePlayerSpecialDieColor = '#4e2c04';
      this.activePlayerHueShift = 33;
      //this.activePlayerDieColor = '#000000';
      //this.activePlayerDieFontColor = '#ffffff';
    }
    else if (playerID === 1) {
      this.activePlayerDieColor = '#a6fcc0';
      this.activePlayerSpecialDieColor = '#00641d';
      this.activePlayerHueShift = 138;
    }
    else if (playerID === 2) {
      this.activePlayerDieColor = '#c0A0ff';
      this.activePlayerSpecialDieColor = '#401260';
      this.activePlayerHueShift = 260;
    }
    else if (playerID === 3) {
      this.activePlayerDieColor = '#a6d2fc'; // '#a6f9fc';
      this.activePlayerSpecialDieColor = '#04315a';
      this.activePlayerHueShift = 210;
    }
  }
}

class DiceRollData {
  type: DiceRollType;
  kind: DiceRollKind;
  damageDice: string;
  modifier: number;
  hiddenThreshold: number;
  isMagic: boolean;
  isSneakAttack: boolean;
  isPaladinSmiteAttack: boolean;
  isWildAnimalAttack: boolean;
  constructor() {

  }
}
