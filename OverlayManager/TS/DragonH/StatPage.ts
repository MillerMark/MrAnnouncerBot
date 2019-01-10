class StatPage {
  characterStats: Array<CharacterStat> = new Array<CharacterStat>();
  constructor() {

  }

  static createMainStatsPage(): StatPage {
    let statPage: StatPage = new StatPage();

    const attributeTop: number = 205;
    const attributeLeft: number = 44;
    const attributeDistanceY: number = 91;
    const modOffset: number = 32;
    const raceClassAlignmentX: number = 135;
    const levelInspXpY: number = 74;
    const nameFontSize: number = 10.5;
    const tempHpFontSize: number = 10.5;
    const hitDiceFontSize: number = 11;
    const modFontSize: number = 10;
    const bigNumberFontSize: number = 16;
    const raceClassAlignmentFontSize: number = 9;
    const levelInspXpFontSize: number = 14;
    const acInitSpeedY: number = 187;
    const hpTempHpX: number = 138;
    const deathSave1X: number = 228;
    const deathSave2X: number = 255;
    const deathSave3X: number = 280;
    const deathSaveLifeY: number = 253;
    const deathSaveDeathY: number = 284;
    const deathSaveRadius: number = 10;

    statPage.addStat('name', 68, 127, nameFontSize);
    statPage.addStat('level', 157, levelInspXpY, levelInspXpFontSize);
    statPage.addStat('inspiration', 224, levelInspXpY, levelInspXpFontSize);
    statPage.addStat('experience', 290, levelInspXpY, levelInspXpFontSize);
    statPage.addStat('raceClass', raceClassAlignmentX, 36, raceClassAlignmentFontSize, TextAlign.left);
    statPage.addStat('alignment', raceClassAlignmentX, 129, raceClassAlignmentFontSize, TextAlign.left);
    statPage.addStat('strength', attributeLeft, attributeTop, bigNumberFontSize);
    statPage.addStat('strengthMod', attributeLeft, attributeTop + modOffset, modFontSize, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('dexterity', attributeLeft, attributeTop + attributeDistanceY, bigNumberFontSize);
    statPage.addStat('dexterityMod', attributeLeft, attributeTop + attributeDistanceY + modOffset, modFontSize, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('constitution', attributeLeft, attributeTop + 2 * attributeDistanceY, bigNumberFontSize);
    statPage.addStat('constitutionMod', attributeLeft, attributeTop + 2 * attributeDistanceY + modOffset, modFontSize, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('intelligence', attributeLeft, attributeTop + 3 * attributeDistanceY, bigNumberFontSize);
    statPage.addStat('intelligenceMod', attributeLeft, attributeTop + 3 * attributeDistanceY + modOffset, modFontSize, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('wisdom', attributeLeft, attributeTop + 4 * attributeDistanceY, bigNumberFontSize);
    statPage.addStat('wisdomMod', attributeLeft, attributeTop + 4 * attributeDistanceY + modOffset, modFontSize, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('charisma', attributeLeft, attributeTop + 5 * attributeDistanceY, bigNumberFontSize);
    statPage.addStat('charismaMod', attributeLeft, attributeTop + 5 * attributeDistanceY + modOffset, modFontSize, TextAlign.center, TextDisplay.plusMinus);
    statPage.addStat('armorClass', 135, acInitSpeedY, bigNumberFontSize);
    statPage.addStat('initiative', 203, acInitSpeedY, bigNumberFontSize);
    statPage.addStat('speed', 272, acInitSpeedY, bigNumberFontSize);
    statPage.addStat('hitPoints', hpTempHpX, 251, bigNumberFontSize);
    statPage.addStat('tempHitPoints', hpTempHpX, 299, tempHpFontSize);
    statPage.addStat('hitDice', 183, 349, hitDiceFontSize, TextAlign.left);
    statPage.addStat('deathSaveLife1', deathSave1X, deathSaveLifeY, deathSaveRadius);
    statPage.addStat('deathSaveLife2', deathSave2X, deathSaveLifeY, deathSaveRadius);
    statPage.addStat('deathSaveLife3', deathSave3X, deathSaveLifeY, deathSaveRadius);
    statPage.addStat('deathSaveDeath1', deathSave1X, deathSaveDeathY, deathSaveRadius);
    statPage.addStat('deathSaveDeath2', deathSave2X, deathSaveDeathY, deathSaveRadius);
    statPage.addStat('deathSaveDeath3', deathSave3X, deathSaveDeathY, deathSaveRadius);

    return statPage;
  }

  addStat(name: string, x: number, y: number, size: number, textAlign: TextAlign = TextAlign.center, textDisplay: TextDisplay = TextDisplay.normal): any {
    this.characterStats.push(new CharacterStat(name, x, y, size, textAlign, textDisplay));
  }
}