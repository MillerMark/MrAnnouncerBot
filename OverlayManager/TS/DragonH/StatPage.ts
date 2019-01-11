class StatPage {
  render(context: CanvasRenderingContext2D, activeCharacter: Character, topData: number = 0, bottomData: number = Infinity): any {
    this.characterStats.forEach(function (stat: CharacterStat) {
      stat.render(context, activeCharacter, topData, bottomData);
      });
    }
  characterStats: Array<CharacterStat> = new Array<CharacterStat>();
  static readonly bigNumberFontSize: number = 16;

  constructor() {

  }

  static createMainStatsPage(): StatPage {
    let statPage: StatPage = new StatPage();

    const raceClassAlignmentX: number = 135;
    const levelInspXpY: number = 76;
    const tempHpFontSize: number = 10.5;
    const hitDiceFontSize: number = 11;
    const raceClassAlignmentFontSize: number = 9;
    const levelInspXpFontSize: number = 14;
    const acInitSpeedY: number = 189;
    const hpTempHpX: number = 138;
    const deathSave1X: number = 228;
    const deathSave2X: number = 255;
    const deathSave3X: number = 280;
    const deathSaveLifeY: number = 253;
    const deathSaveDeathY: number = 284;
    const deathSaveRadius: number = 10;
    const profPercGpY: number = 408;

    StatPage.addName(statPage);
    statPage.addStat('level', 157, levelInspXpY, levelInspXpFontSize);
    statPage.addStat('inspiration', 224, levelInspXpY, levelInspXpFontSize);
    statPage.addStat('experiencePoints', 290, levelInspXpY, 52, TextAlign.center, TextDisplay.autoSize);
    statPage.addStat('raceClass', raceClassAlignmentX, 38, raceClassAlignmentFontSize, TextAlign.left);
    statPage.addStat('alignment', raceClassAlignmentX, 129, raceClassAlignmentFontSize, TextAlign.left);

    StatPage.addAbilities(statPage);
    statPage.addStat('armorClass', 135, acInitSpeedY, StatPage.bigNumberFontSize);
    statPage.addStat('initiative', 203, acInitSpeedY, StatPage.bigNumberFontSize, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('speed', 272, acInitSpeedY, StatPage.bigNumberFontSize);
    statPage.addStat('hitPoints', hpTempHpX, 251, StatPage.bigNumberFontSize);
    statPage.addStat('tempHitPoints', hpTempHpX, 299, tempHpFontSize);
    statPage.addStat('hitDice', 183, 351, hitDiceFontSize, TextAlign.left);
    statPage.addStat('deathSaveLife1', deathSave1X, deathSaveLifeY, deathSaveRadius);
    statPage.addStat('deathSaveLife2', deathSave2X, deathSaveLifeY, deathSaveRadius);
    statPage.addStat('deathSaveLife3', deathSave3X, deathSaveLifeY, deathSaveRadius);
    statPage.addStat('deathSaveDeath1', deathSave1X, deathSaveDeathY, deathSaveRadius);
    statPage.addStat('deathSaveDeath2', deathSave2X, deathSaveDeathY, deathSaveRadius);
    statPage.addStat('deathSaveDeath3', deathSave3X, deathSaveDeathY, deathSaveRadius);
    statPage.addStat('proficiencyBonus', 122, profPercGpY, StatPage.bigNumberFontSize, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('passivePerception', 201, profPercGpY, StatPage.bigNumberFontSize);
    statPage.addStat('goldPieces', 289, profPercGpY, 70, TextAlign.center, TextDisplay.autoSize);

    const distanceBetweenSavingThrowEntries: number = 35;
    const savingThrowRadioX: number = 120;
    const savingThrowBonusX: number = 157;
    const savingThrowStartY: number = 482;
    const savingThrowRadius: number = 11;
    statPage.addStat('hasSavingThrowProficiencyStrength', savingThrowRadioX, savingThrowStartY, savingThrowRadius);
    statPage.addStat('hasSavingThrowProficiencyDexterity', savingThrowRadioX, savingThrowStartY + distanceBetweenSavingThrowEntries, savingThrowRadius);
    statPage.addStat('hasSavingThrowProficiencyConstitution', savingThrowRadioX, savingThrowStartY + 2 * distanceBetweenSavingThrowEntries, savingThrowRadius);
    statPage.addStat('hasSavingThrowProficiencyIntelligence', savingThrowRadioX, savingThrowStartY + 3 * distanceBetweenSavingThrowEntries, savingThrowRadius);
    statPage.addStat('hasSavingThrowProficiencyWisdom', savingThrowRadioX, savingThrowStartY + 4 * distanceBetweenSavingThrowEntries, savingThrowRadius);
    statPage.addStat('hasSavingThrowProficiencyCharisma', savingThrowRadioX, savingThrowStartY + 5 * distanceBetweenSavingThrowEntries, savingThrowRadius);

    statPage.addStat('savingThrowModStrength', savingThrowBonusX, savingThrowStartY, savingThrowRadius, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('savingThrowModDexterity', savingThrowBonusX, savingThrowStartY + distanceBetweenSavingThrowEntries, savingThrowRadius, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('savingThrowModConstitution', savingThrowBonusX, savingThrowStartY + 2 * distanceBetweenSavingThrowEntries, savingThrowRadius, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('savingThrowModIntelligence', savingThrowBonusX, savingThrowStartY + 3 * distanceBetweenSavingThrowEntries, savingThrowRadius, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('savingThrowModWisdom', savingThrowBonusX, savingThrowStartY + 4 * distanceBetweenSavingThrowEntries, savingThrowRadius, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('savingThrowModCharisma', savingThrowBonusX, savingThrowStartY + 5 * distanceBetweenSavingThrowEntries, savingThrowRadius, TextAlign.center, TextDisplay.plusMinus);


    return statPage;
  }

  static createSkillsStatsPage(): StatPage {
    let statPage: StatPage = new StatPage();

    StatPage.addName(statPage);
    StatPage.addAbilities(statPage);
    const percProfX: number = 222;
    statPage.addStat('passivePerception', percProfX, 44, StatPage.bigNumberFontSize);
    statPage.addStat('proficiencyBonus', percProfX, 110, StatPage.bigNumberFontSize, TextAlign.center, TextDisplay.plusMinus);
    
    const distanceBetweenSkills: number = 28.88;
    const skillRadioX: number = 124;
    const skillBonusX: number = 157;
    const skillStartY: number = 188;
    const skillRadius: number = 8;
    const skillModOffset: number = 1;
    const skillModFontSize: number = 10;

    var initialCap = function (str: string) { return str.charAt(0).toUpperCase() + str.slice(1); }

    const skills = Object.keys(Skills).filter((item) => {
      return isNaN(Number(item));
    });

    for (var i = 0; i < skills.length; i++) {
      statPage.addStat('hasSkillProficiency' + initialCap(skills[i]), skillRadioX, skillStartY + i * distanceBetweenSkills, skillRadius);
    }

    for (var i = 0; i < skills.length; i++) {
      statPage.addStat('skillMod' + initialCap(skills[i]), skillBonusX, skillModOffset + skillStartY + i * distanceBetweenSkills, skillModFontSize, TextAlign.center, TextDisplay.plusMinus);
    }

    return statPage;
  }
  static addName(statPage: StatPage): any {
    const nameFontSize: number = 10.5;
    statPage.addStat('name', 68, 128, nameFontSize);
  }

  static addAbilities(statPage: StatPage): any {
    const attributeTop: number = 205;
    const attributeLeft: number = 44;
    const attributeDistanceY: number = 91;
    const modOffset: number = 32;
    const modFontSize: number = 10;

    const abilities = Object.keys(Ability).filter((item) => {
      return isNaN(Number(item));
    });

    for (var i = 0; i < abilities.length; i++) {
      statPage.addStat(abilities[i], attributeLeft, attributeTop + i * attributeDistanceY, StatPage.bigNumberFontSize);
      statPage.addStat(abilities[i] + 'Mod', attributeLeft, attributeTop + i * attributeDistanceY + modOffset, modFontSize, TextAlign.center, TextDisplay.plusMinus);
    }
  }

  addStat(name: string, x: number, y: number, size: number, textAlign: TextAlign = TextAlign.center, textDisplay: TextDisplay = TextDisplay.normal): any {
    this.characterStats.push(new CharacterStat(name, x, y, size, textAlign, textDisplay));
  }
}