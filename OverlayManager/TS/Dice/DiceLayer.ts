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

const fps40: number = 1000 / 40;
const fps30: number = 33;
const fps25: number = 40;
const fps20: number = 50;
const matchNormalDieColorsToSpecial: boolean = true;
//const damageDieBackgroundColor: string = '#e49836';
//const damageDieFontColor: string = '#000000';
const damageDieBackgroundColor: string = '#a40017';
const damageDieFontColor: string = '#ffffff';

const successFontColor: string = '#ffffff';
const successOutlineColor: string = '#000000';
const failFontColor: string = '#000000';
const failOutlineColor: string = '#ffffff';

class DiceLayer {
  static readonly bonusRollDieColor: string = '#adfff4'; // '#ff81bf'; // '#a3ffe6'
  static readonly bonusRollFontColor: string = '#000000';
  static matchOozeToDieColor: boolean = true;
  textEffects: TextEffects = new TextEffects();
  diceFrontCanvas: HTMLCanvasElement;
  diceBackCanvas: HTMLCanvasElement;
  diceFrontContext: CanvasRenderingContext2D;
  diceBackContext: CanvasRenderingContext2D;
  diceFireball: Sprites;
  sparkTrail: Sprites;
  haloSpins: Sprites;
  //d20Fire: Sprites;
  puff: Sprites;
  freeze: Sprites;
  freezePop: Sprites;
  diceSparks: Sprites;
  sneakAttackTop: Sprites;
  sneakAttackBottom: Sprites;
  pawPrints: Sprites;
  //stars: Sprites;
  dicePortal: Sprites;
  handGrabsDiceTop: Sprites;
  handGrabsDiceBottom: Sprites;
  dicePortalTop: Sprites;
  magicRing: Sprites;
  spirals: Sprites;
  halos: Sprites;
  ravens: Sprites[];
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
  playerID: number;

  constructor() {
    this.loadSprites();
  }

  loadSprites() {
    Part.loadSprites = true;

    globalBypassFrameSkip = true;

    this.allFrontLayerEffects = new SpriteCollection();
    this.allBackLayerEffects = new SpriteCollection();

    //! Items added later are drawn on top of earlier items.
    this.diceFireball = new Sprites("/Dice/Roll20Fireball/DiceFireball", 71, fps30, AnimationStyle.Sequential, true);
    this.diceFireball.originX = 104;
    this.diceFireball.originY = 155;
    this.allFrontLayerEffects.add(this.diceFireball);

    this.freeze = new Sprites("/Dice/Freeze/Freeze", 30, fps30, AnimationStyle.SequentialStop, true);
    this.freeze.originX = 80;
    this.freeze.originY = 80;
    this.allFrontLayerEffects.add(this.freeze);

    this.freezePop = new Sprites("/Dice/Freeze/Pop", 50, fps30, AnimationStyle.Sequential, true);
    this.freezePop.originX = 182;
    this.freezePop.originY = 136;
    this.allFrontLayerEffects.add(this.freezePop);

    this.sparkTrail = new Sprites("/Dice/SparkTrail/SparkTrail", 46, fps40, AnimationStyle.Sequential, true);
    this.sparkTrail.originX = 322;
    this.sparkTrail.originY = 152;
    this.allFrontLayerEffects.add(this.sparkTrail);

    this.pawPrints = new Sprites("/Dice/TigerPaw/TigerPaw", 76, fps30, AnimationStyle.Loop, true);
    this.pawPrints.originX = 50;
    this.pawPrints.originY = 66;
    this.allBackLayerEffects.add(this.pawPrints);

    //this.stars = new Sprites("/Dice/Star/Star", 60, fps30, AnimationStyle.Loop, true);
    //this.stars.originX = 170;
    //this.stars.originY = 165;
    //this.allBackLayerEffects.add(this.stars);

    //this.d20Fire = new Sprites("/Dice/D20Fire/D20Fire", 180, fps30, AnimationStyle.Loop, true);
    //this.d20Fire.originX = 151;
    //this.d20Fire.originY = 149;
    //this.d20Fire.returnFrameIndex = 72;
    //this.allBackLayerEffects.add(this.d20Fire);

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

    this.diceSparks = new Sprites("/Dice/Sparks/Spark", 49, fps20, AnimationStyle.Loop, true);
    this.diceSparks.originX = 170;
    this.diceSparks.originY = 158;
    this.allBackLayerEffects.add(this.diceSparks);

    this.magicRing = new Sprites("/Dice/MagicRing/MagicRing", 180, fps40, AnimationStyle.Loop, true);
    this.magicRing.returnFrameIndex = 60;
    this.magicRing.originX = 140;
    this.magicRing.originY = 112;
    this.allFrontLayerEffects.add(this.magicRing);

    this.halos = new Sprites("/Dice/Halo/Halo", 90, fps30, AnimationStyle.Loop, true);
    this.halos.originX = 190;
    this.halos.originY = 190;
    this.allFrontLayerEffects.add(this.halos);

    this.haloSpins = new Sprites("/Dice/PaladinSpin/PaladinSpin", 85, fps30, AnimationStyle.Loop, true);
    this.haloSpins.originX = 200;
    this.haloSpins.originY = 200;
    this.allFrontLayerEffects.add(this.haloSpins);

    this.ravens = [];
    this.loadRavens(3);

    this.spirals = new Sprites("/Dice/Spiral/Spiral", 64, fps20, AnimationStyle.Sequential, true);
    this.spirals.originX = 134;
    this.spirals.originY = 107;
    this.allBackLayerEffects.add(this.spirals);

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

    this.sneakAttackTop = new Sprites("/Dice/SneakAttack/SneakAttackTop", 91, fps30, AnimationStyle.Sequential, true);
    this.sneakAttackTop.originX = 373;
    this.sneakAttackTop.originY = 377;
    this.allFrontLayerEffects.add(this.sneakAttackTop);

    this.puff = new Sprites("/Dice/SneakAttack/Puff", 32, fps30, AnimationStyle.Sequential, true);
    this.puff.originX = 201;
    this.puff.originY = 404;
    this.allFrontLayerEffects.add(this.puff);

    this.sneakAttackBottom = new Sprites("/Dice/SneakAttack/SneakAttackBottom", 91, fps30, AnimationStyle.Sequential, true);
    this.sneakAttackBottom.originX = 373;
    this.sneakAttackBottom.originY = 377;
    this.allBackLayerEffects.add(this.sneakAttackBottom);
  }

  loadRavens(count: number): any {
    for (var i = 0; i < count; i++) {
      let raven = new Sprites(`/Dice/Ravens/${i + 1}/Ravens`, 36, fps30, AnimationStyle.Sequential, true);
      raven.originX = 44;
      raven.originY = 477;
      this.allBackLayerEffects.add(raven);
      this.ravens.push(raven);
    }
  }


  showResult(resultMessage: string, success: boolean): any {
    let textEffect: TextEffect = this.textEffects.add(new Vector(960, 150), resultMessage, 5000);
    if (success) {
      textEffect.fontColor = successFontColor;
      textEffect.outlineColor = successOutlineColor;
    }
    else {
      textEffect.fontColor = failFontColor;
      textEffect.outlineColor = failOutlineColor;
    }
    textEffect.scale = 2;
    textEffect.velocityY = 0.6;
    textEffect.opacity = 0.90;
    if (success)
      textEffect.targetScale = 12;
    else
      textEffect.targetScale = 2;
    textEffect.fadeOutTime = 800;
    textEffect.fadeInTime = 200;
  }

  showTotalDamage(totalDamage: number, success: boolean): any {
    let textEffect: TextEffect = this.textEffects.add(new Vector(960, 700), `Damage: ${totalDamage}`, 5000);
    textEffect.fontColor = damageDieBackgroundColor;
    textEffect.outlineColor = damageDieFontColor;
    textEffect.scale = 4;
    textEffect.velocityY = 0.6;
    textEffect.opacity = 0.90;
    if (success)
      textEffect.targetScale = 12;
    else 
      textEffect.targetScale = 2;
    textEffect.fadeOutTime = 800;
    textEffect.fadeInTime = 200;
  }

  showBonusRoll(totalBonusStr: string): any {
    let textEffect: TextEffect = this.textEffects.add(new Vector(500, 900), totalBonusStr, 5000);
    textEffect.fontColor = DiceLayer.bonusRollDieColor;
    textEffect.outlineColor = DiceLayer.bonusRollFontColor;
    textEffect.scale = 3;
    textEffect.velocityY = 0.6;
    textEffect.opacity = 0.90;
    textEffect.targetScale = 9;
    textEffect.fadeOutTime = 800;
    textEffect.fadeInTime = 200;
  }

  showDieTotal(thisRollStr: string): void {
    let textEffect: TextEffect = this.textEffects.add(new Vector(960, 540), thisRollStr, 5000);
    textEffect.fontColor = this.activePlayerDieColor;
    textEffect.outlineColor = this.activePlayerDieFontColor;
    textEffect.scale = 10;
    textEffect.opacity = 0.90;
    textEffect.targetScale = 30;
    textEffect.fadeOutTime = 800;
    textEffect.fadeInTime = 200;
  }

  showRollModifier(rollModifier: number): void {
    if (rollModifier == 0)
      return;
    var rollModStr: string = rollModifier.toString();
    if (rollModifier > 0)
      rollModStr = '+' + rollModStr;
    let textEffect: TextEffect = this.textEffects.add(new Vector(960, 420), `(${rollModStr})`, 5000);
    textEffect.fontColor = this.activePlayerDieColor;
    textEffect.outlineColor = this.activePlayerDieFontColor;
    textEffect.velocityY = -0.7;
    textEffect.scale = 3;
    textEffect.opacity = 0.90;
    textEffect.targetScale = 5.5;
    textEffect.fadeOutTime = 800;
    textEffect.fadeInTime = 500;
  }

  addDieValueLabel(centerPos: Vector, value: string, highlight: boolean = false) {
    let textEffect: TextEffect = this.textEffects.add(centerPos, value, 5000);
    if (highlight)
      textEffect.fontColor = '#ff0000';
    textEffect.scale = 6;
    textEffect.opacity = 0.75;
    textEffect.targetScale = 0.5;
    textEffect.fadeOutTime = 800;
    textEffect.fadeInTime = 200;
  }

  indicateBonusRoll(message: string): any {
    let textEffect: TextEffect = this.textEffects.add(new Vector(960, 800), message, 3000);
    textEffect.scale = 6;
    textEffect.opacity = 0.75;
    textEffect.targetScale = 10;
    textEffect.fadeOutTime = 800;
    textEffect.fadeInTime = 800;
  }


  addVantageText(die: any, message: string, fontColor: string, outlineColor: string): any {
    let centerPos: Vector = getScreenCoordinates(die.getObject());
    let textEffect: TextEffect = this.textEffects.add(centerPos.add(new Vector(0, 80)), message, 1500);
    textEffect.fontColor = fontColor;
    textEffect.outlineColor = outlineColor;
    textEffect.scale = 3;
    textEffect.waitToScale = 400;
    textEffect.fadeOutTime = 800;
    textEffect.fadeInTime = 600;
    textEffect.targetScale = 1;
  }

  addVantageTextAfter(die: any, message: string, fontColor: string, outlineColor: string, timeout: number = 0) {
    if (timeout > 0)
      setTimeout(function () {
        this.addVantageText(die, message, fontColor, outlineColor);
      }.bind(this), timeout);
    else
      this.addVantageText(die, message, fontColor, outlineColor);
  }

  addDisadvantageText(die: any, timeout: number = 0): any {
    this.addVantageTextAfter(die, 'Disadvantage', this.activePlayerDieColor, this.activePlayerSpecialDieColor, timeout);
  }

  addAdvantageText(die: any, timeout: number = 0): any {
    this.addVantageTextAfter(die, 'Advantage', this.activePlayerDieColor, this.activePlayerSpecialDieColor, timeout);
  }


  mouseDownInCanvas(e) {
    //if (effectOverride != undefined) {
    //  var enumIndex: number = <number>effectOverride;
    //  let totalElements: number = Object.keys(DieEffect).length / 2;
    //  enumIndex++;
    //  if (enumIndex >= totalElements)
    //    enumIndex = 0;
    //  effectOverride = <DieEffect>enumIndex;
    //  console.log('New effect: ' + DieEffect[effectOverride]);
    //}
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
    this.textEffects.removeExpiredText(now);
    this.textEffects.updatePositions(now);
    this.textEffects.render(this.diceFrontContext, now);
  }

  addFireball(x: number, y: number) {
    this.diceFireball.add(x, y, 0).rotation = 90;
  }

  addMagicRing(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
    let magicRing = this.magicRing.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
    magicRing.rotation = Math.random() * 360;
    return magicRing;
  }

  addFreezeBubble(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
    let freeze: SpriteProxy = this.freeze.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
    freeze.rotation = Math.random() * 360;
    freeze.fadeInTime = 500;
    return freeze;
  }

  addFeezePop(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
    let freezePop = this.freezePop.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
    freezePop.rotation = Math.random() * 360;
    return freezePop;
  }

  addHalo(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1): SpriteProxy {
    let halo = this.halos.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
    halo.rotation = Math.random() * 360;
    halo.opacity = 0.9;
    halo.fadeInTime = 500;
    halo.fadeOutTime = 500;
    halo.fadeOnDestroy = true;
    halo.autoRotationDegeesPerSecond = 10;
    return halo;
  }

  addHaloSpin(x: number, y: number, hueShift: number = 0, angle: number): SpriteProxy {
    let haloSpin = this.haloSpins.addShifted(x, y, 0, hueShift);
    haloSpin.rotation = angle;
    haloSpin.fadeInTime = 500;
    haloSpin.fadeOutTime = 500;
    haloSpin.fadeOnDestroy = true;
    return haloSpin;
  }


  addRaven(x: number, y: number, angle: number) {
    let index: number = Math.floor(Math.random() * this.ravens.length);
    let raven = this.ravens[index].addShifted(x, y, 0, this.activePlayerHueShift + Random.plusMinus(35));
    raven.rotation = angle + 180 + Random.plusMinus(45);
    //diceSounds.playRandom('BirdFlap', 4);
    return raven;
  }

  blowColoredSmoke(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    this.diceBlowColoredSmoke.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
  }

  //addD20Fire(x: number, y: number) {
  //  this.d20Fire.add(x, y).rotation = Math.random() * 360;
  //}

  addPuff(x: number, y: number, angle: number) {
    let hueShift = this.activePlayerHueShift + Random.plusMinus(10);
    var angleShift: number = 0;
    if (Math.random() > 0.5)
      angleShift = 180;
    this.puff.addShifted(x, y, 0, hueShift).rotation = angle + angleShift;
  }

  addSparkTrail(x: number, y: number, angle: number) {
    this.sparkTrail.addShifted(x, y, 0, this.activePlayerHueShift + Random.plusMinus(35)).rotation = angle + 180 + Random.plusMinus(15);
  }

  clearResidualEffects(): any {
    this.magicRing.sprites = [];
    this.halos.sprites = [];
    this.haloSpins.sprites = [];
    //this.stars.sprites = [];
    //this.d20Fire.sprites = [];
    this.textEffects.clear();
  }


  addSteampunkTunnel(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    // no rotation on SteampunkTunnel - shadows expect light source from above.
    this.dieSteampunkTunnel.addShifted(x, y, 0, hueShift, saturationPercent, brightness);
  }

  addSneakAttack(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    let rotation: number = Math.random() * 360;
    this.sneakAttackBottom.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = rotation;
    this.sneakAttackTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = rotation;
  }

  addDiceBomb(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
    this.diceBombBase.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
    this.diceBombTop.addShifted(x, y, 0, hueShift, saturationPercent, brightness).rotation = Math.random() * 360;
  }

  addPortal(x: number, y: number, hueShift: number = 0, saturationPercent: number = -1, brightness: number = -1) {
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
    diceRoll.throwPower = dto.ThrowPower;
    if (diceRoll.throwPower < 0.2)
      diceRoll.throwPower = 0.2;
    if (diceRoll.throwPower > 2.0)
      diceRoll.throwPower = 2.0;
    return diceRoll;
  }

  spark(x: number, y: number): SpriteProxy {
    let spark = this.diceSparks.addShifted(x, y, Math.round(Math.random() * this.diceSparks.sprites.length), Math.random() * 360);
    spark.expirationDate = performance.now() + 180;
    spark.fadeOutTime = 0;
    spark.opacity = 0.8;
    return spark;
  }

  addSpiral(x: number, y: number, angle: number): SpriteProxy {
    let spiral = this.spirals.addShifted(x, y, Math.round(Math.random() * this.spirals.sprites.length), diceLayer.activePlayerHueShift + Random.plusMinus(20));
    spiral.rotation = Random.between(0, 360);
    return spiral;
  }

  addPawPrint(x: number, y: number, angle: number): SpriteProxy {
    let pawPrint = this.pawPrints.addShifted(x, y, Math.round(Math.random() * this.pawPrints.sprites.length), diceLayer.activePlayerHueShift);
    pawPrint.rotation = angle;
    pawPrint.expirationDate = performance.now() + 4000;
    pawPrint.fadeOutTime = 2000;
    pawPrint.fadeInTime = 500;
    pawPrint.opacity = 0.9;
    return pawPrint;
  }

  //addStar(x: number, y: number): SpriteProxy {
  //  let star = this.stars.addShifted(x, y, Math.round(Math.random() * this.stars.sprites.length), diceLayer.activePlayerHueShift);
  //  star.autoRotationDegeesPerSecond = 15 + Math.round(Math.random() * 20);
  //  if (Math.random() < 0.5)
  //    star.autoRotationDegeesPerSecond = -star.autoRotationDegeesPerSecond;
  //  star.fadeInTime = 1000;
  //  star.fadeOutTime = 500;
  //  star.opacity = 0.75;
  //  return star;
  //}

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
    this.playerID = playerID;
    this.activePlayerDieFontColor = '#000000';
    this.activePlayerSpecialDieFontColor = '#ffffff';
    if (playerID === 0) { // Kent
      this.activePlayerDieColor = '#feb6b6'; // '#fcd5a6';
      this.activePlayerSpecialDieColor = '#710138'; // '#720102'; //'#4e2c04';
      //this.activePlayerDieColor = '#fec75e'; // '#fcd5a6';
      //this.activePlayerSpecialDieColor = '#422b00'; //'#4e2c04';
      this.activePlayerHueShift = 0; // 39;
    }
    else if (playerID === 1) {  // Kayla
      this.activePlayerDieColor = '#a6fcc0';
      this.activePlayerSpecialDieColor = '#00641d';
      //this.activePlayerSpecialDieFontColor = '#ddffe7';
      this.activePlayerHueShift = 138;
    }
    else if (playerID === 2) {    // Mark
      this.activePlayerDieColor = '#c0A0ff';
      this.activePlayerSpecialDieColor = '#401260';
      //this.activePlayerSpecialDieFontColor = '#f0e2fa';
      this.activePlayerHueShift = 260;
    }
    else if (playerID === 3) {    // Karen
      this.activePlayerDieColor = '#9fd2ff'; // '#a6d2fc'; // '#a6f9fc';
      this.activePlayerSpecialDieColor = '#04315a';
      //this.activePlayerSpecialDieFontColor = '#e4f2fe';
      this.activePlayerHueShift = 210;
    }

    if (matchNormalDieColorsToSpecial) {
      this.activePlayerDieFontColor = this.activePlayerSpecialDieFontColor;
      this.activePlayerDieColor = this.activePlayerSpecialDieColor;
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
  throwPower: number;
  constructor() {

  }
}
