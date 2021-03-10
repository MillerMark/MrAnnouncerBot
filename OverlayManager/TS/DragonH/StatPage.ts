class StatPage {

	render(context: CanvasRenderingContext2D, activeCharacter: Character, topData = 0, bottomData = Infinity): any {
		this.allStats.forEach(function (stat: BaseStat) {
			stat.render(context, activeCharacter, topData, bottomData);
		});
	}

	allStats: Array<BaseStat> = new Array<BaseStat>();
	static readonly bigNumberFontSize: number = 16;

	constructor() {

	}

	static createMainStatsPage(): StatPage {
		const statPage: StatPage = new StatPage();

		const raceClassAlignmentX = 135;
		const levelInspXpY = 76;
		const tempHpFontSize = 10.5;
		const hitDiceFontSize = 11;
		const raceClassAlignmentFontSize = 9;
		const levelInspXpFontSize = 14;
		const acInitSpeedY = 189;
		const hpTempHpX = 138;
		const deathSave1X = 241;
		const deathSave2X = 268;
		const deathSave3X = 293;
		const deathSaveLifeY = 325;
		const deathSaveDeathY = 357;
		const deathSaveRadius = 10;
		const profPercGpY = 448;
		const maxSpellFontSize = 11;

		StatPage.addName(statPage);
		statPage.addBar('hitPoints', 0, 'maxHitPoints', 182, 306, 188, 399, '#e22500', '#0379bf');
		statPage.addStat('Level', 157, levelInspXpY, levelInspXpFontSize, 51);
		statPage.addStat('inspiration', 224, levelInspXpY, levelInspXpFontSize, 52);
		statPage.addStat('experiencePoints', 290, levelInspXpY, levelInspXpFontSize, 52, TextAlign.center, TextDisplay.autoSize);
		statPage.addStat('raceClass', raceClassAlignmentX, 38, raceClassAlignmentFontSize, 187, TextAlign.left);
		statPage.addStat('alignmentStr', raceClassAlignmentX, 129, raceClassAlignmentFontSize, 187, TextAlign.left);

		StatPage.addAbilities(statPage);
		statPage.addStat('armorClass', 135, acInitSpeedY, StatPage.bigNumberFontSize, 47);
		statPage.addStat('initiative', 211, acInitSpeedY, StatPage.bigNumberFontSize, 53, TextAlign.center, TextDisplay.plusMinus);
		statPage.addStat('baseWalkingSpeed', 281, acInitSpeedY, StatPage.bigNumberFontSize, 59);
		statPage.addStat('hitPoints', hpTempHpX, 323, StatPage.bigNumberFontSize, 61);
		statPage.addStat('tempHitPoints', hpTempHpX, 374, tempHpFontSize, 61, TextAlign.center, TextDisplay.plusMinus);
		//statPage.addStat('remainingHitDice', 183, 351, hitDiceFontSize, ??, TextAlign.left);
		statPage.addStat('deathSaveLife1', deathSave1X, deathSaveLifeY, deathSaveRadius);
		statPage.addStat('deathSaveLife2', deathSave2X, deathSaveLifeY, deathSaveRadius);
		statPage.addStat('deathSaveLife3', deathSave3X, deathSaveLifeY, deathSaveRadius);
		statPage.addStat('deathSaveDeath1', deathSave1X, deathSaveDeathY, deathSaveRadius);
		statPage.addStat('deathSaveDeath2', deathSave2X, deathSaveDeathY, deathSaveRadius);
		statPage.addStat('deathSaveDeath3', deathSave3X, deathSaveDeathY, deathSaveRadius);
		statPage.addStat('proficiencyBonus', 121, profPercGpY, StatPage.bigNumberFontSize, 53, TextAlign.center, TextDisplay.plusMinus);
		statPage.addStat('passivePerception', 200, profPercGpY, StatPage.bigNumberFontSize, 69);
		statPage.addStat('goldPieces', 288, profPercGpY, StatPage.bigNumberFontSize, 62, TextAlign.center, TextDisplay.autoSize);
		statPage.addStat('SpellCastingStr', 248, 282, maxSpellFontSize, 64, TextAlign.left, TextDisplay.autoSize | TextDisplay.deemphasizeZero);
		statPage.addStat('maxHitPoints', 152, 282, maxSpellFontSize, 37, TextAlign.left, TextDisplay.autoSize);

		statPage.addStat('resistancesVulnerabilitiesImmunitiesStr', 111, 246, 8, 202, TextAlign.left, TextDisplay.autoSize | TextDisplay.eraseBackground);

		const distanceBetweenSavingThrowEntries = 35;
		const savingThrowRadioX = 120;
		const savingThrowBonusX = 157;
		const savingThrowStartY = 526;
		const savingThrowRadius = 11;
		statPage.addStat('hasSavingThrowProficiencyStrength', savingThrowRadioX, savingThrowStartY, savingThrowRadius);
		statPage.addStat('hasSavingThrowProficiencyDexterity', savingThrowRadioX, savingThrowStartY + distanceBetweenSavingThrowEntries, savingThrowRadius);
		statPage.addStat('hasSavingThrowProficiencyConstitution', savingThrowRadioX, savingThrowStartY + 2 * distanceBetweenSavingThrowEntries, savingThrowRadius);
		statPage.addStat('hasSavingThrowProficiencyIntelligence', savingThrowRadioX, savingThrowStartY + 3 * distanceBetweenSavingThrowEntries, savingThrowRadius);
		statPage.addStat('hasSavingThrowProficiencyWisdom', savingThrowRadioX, savingThrowStartY + 4 * distanceBetweenSavingThrowEntries, savingThrowRadius);
		statPage.addStat('hasSavingThrowProficiencyCharisma', savingThrowRadioX, savingThrowStartY + 5 * distanceBetweenSavingThrowEntries, savingThrowRadius);

		const savingThrowModWidth = 30;
		statPage.addStat('savingThrowModStrength', savingThrowBonusX, savingThrowStartY, savingThrowRadius, savingThrowModWidth, TextAlign.center, TextDisplay.plusMinus);
		statPage.addStat('savingThrowModDexterity', savingThrowBonusX, savingThrowStartY + distanceBetweenSavingThrowEntries, savingThrowRadius, savingThrowModWidth, TextAlign.center, TextDisplay.plusMinus);
		statPage.addStat('savingThrowModConstitution', savingThrowBonusX, savingThrowStartY + 2 * distanceBetweenSavingThrowEntries, savingThrowRadius, savingThrowModWidth, TextAlign.center, TextDisplay.plusMinus);
		statPage.addStat('savingThrowModIntelligence', savingThrowBonusX, savingThrowStartY + 3 * distanceBetweenSavingThrowEntries, savingThrowRadius, savingThrowModWidth, TextAlign.center, TextDisplay.plusMinus);
		statPage.addStat('savingThrowModWisdom', savingThrowBonusX, savingThrowStartY + 4 * distanceBetweenSavingThrowEntries, savingThrowRadius, savingThrowModWidth, TextAlign.center, TextDisplay.plusMinus);
		statPage.addStat('savingThrowModCharisma', savingThrowBonusX, savingThrowStartY + 5 * distanceBetweenSavingThrowEntries, savingThrowRadius, savingThrowModWidth, TextAlign.center, TextDisplay.plusMinus);


		return statPage;
	}

	addBar(propName: string, minValue: number | string, maxValue: number | string, x1: number,
		y1: number, x2: number, y2: number, minColor: string, maxColor = '') {
		if (!maxColor)
			maxColor = minColor;

		this.allStats.push(new BarStat(propName, minValue, maxValue, x1, y1, x2, y2, minColor, maxColor));
	}


	static createSkillsStatsPage(): StatPage {
		const statPage: StatPage = new StatPage();

		StatPage.addName(statPage);
		StatPage.addAbilities(statPage);
		const percProfX = 222;
		statPage.addStat('passivePerception', percProfX, 44, StatPage.bigNumberFontSize, 78);
		statPage.addStat('proficiencyBonus', percProfX, 110, StatPage.bigNumberFontSize, 128, TextAlign.center, TextDisplay.plusMinus);

		const distanceBetweenSkills = 29;
		const skillRadioX = 124;
		const skillBonusX = 157;
		const skillStartY = 189;
		const skillRadius = 8;
		const skillModOffset = 1;
		const skillModFontSize = 10;

		const initialCap = function (str: string) { return str.charAt(0).toUpperCase() + str.slice(1); }

		const skills = Object.keys(Skills).filter((item) => {
			return isNaN(Number(item));
		});

		const acrobaticsStartIndex = 7;  // Skills.acrobatics starts at index 7.

		for (let i = acrobaticsStartIndex; i < skills.length; i++) {
			const yPos: number = skillStartY + (i - acrobaticsStartIndex) * distanceBetweenSkills;
			statPage.addStat('hasSkillProficiency' + initialCap(skills[i]), skillRadioX, yPos, skillRadius);
		}

		for (let i = acrobaticsStartIndex; i < skills.length; i++) {
			const yPos: number = skillModOffset + skillStartY + (i - acrobaticsStartIndex) * distanceBetweenSkills - 1;
			statPage.addStat('skillMod' + initialCap(skills[i]), skillBonusX, yPos, skillModFontSize, 31, TextAlign.center, TextDisplay.plusMinus);
		}

		return statPage;
	}

	static createEquipmentPage(): StatPage {
		const statPage: StatPage = new StatPage();

		const goldPiecesLoadY = 47;
		const loadWeightX = 261;
		const speedWeightY = 106;
		const goldPiecesSpeedX = 172;

		StatPage.addName(statPage);
		statPage.addStat('goldPieces', goldPiecesSpeedX, goldPiecesLoadY, StatPage.bigNumberFontSize, 70, TextAlign.center, TextDisplay.autoSize);
		statPage.addStat('load', loadWeightX, goldPiecesLoadY, StatPage.bigNumberFontSize, 58, TextAlign.center, TextDisplay.autoSize);
		statPage.addStat('baseWalkingSpeed', goldPiecesSpeedX, speedWeightY, StatPage.bigNumberFontSize);
		statPage.addStat('weight', loadWeightX, speedWeightY, StatPage.bigNumberFontSize);
		return statPage;
	}

	static createSpellPage(): StatPage {
		const statPage: StatPage = new StatPage();

		StatPage.addName(statPage);
		statPage.addStat('SpellCastingAbilityStr', 222, 43, StatPage.bigNumberFontSize, 162, TextAlign.center, TextDisplay.autoSize);
		statPage.addStat('SpellAttackBonusStr', 173, 107, StatPage.bigNumberFontSize, 72, TextAlign.center, TextDisplay.autoSize);
		statPage.addStat('SpellSaveDC', 267, 107, StatPage.bigNumberFontSize, 86, TextAlign.center, TextDisplay.autoSize);
		return statPage;
	}


	static addName(statPage: StatPage): void {
		const nameFontSize = 10.5;
		statPage.addStat('firstName', 68, 128, nameFontSize, 96);
	}

	static addAbilities(statPage: StatPage): void {
		const attributeTop = 205;
		const attributeLeft = 44;
		const attributeDistanceY = 91;
		const modOffset = 32;
		const modFontSize = 10;

		const abilities = Object.keys(Ability).filter((item) => {
			return isNaN(Number(item));
		});

		for (let i = 0; i < abilities.length; i++) {
			statPage.addStat('base' + StatPage.initialCap(abilities[i]), attributeLeft, attributeTop + i * attributeDistanceY, StatPage.bigNumberFontSize, 45);
			statPage.addStat(abilities[i] + 'Mod', attributeLeft, attributeTop + i * attributeDistanceY + modOffset, modFontSize, 30, TextAlign.center, TextDisplay.plusMinus);
		}
	}

	static initialCap(str: string): string {
		return str[0].toUpperCase() + str.substring(1);
	}

	addStat(name: string, x: number, y: number, fontSize: number, maxWidth = 0, textAlign: TextAlign = TextAlign.center, textDisplay: TextDisplay = TextDisplay.normal): void {
		this.allStats.push(new CharacterStat(name, x, y, fontSize, maxWidth, textAlign, textDisplay));
	}
}