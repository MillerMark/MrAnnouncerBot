class SpellBook {
	// titles:
	static readonly str_CastingTime: string = 'Casting Time: ';
	static readonly str_Range: string = 'Range: ';
	static readonly str_Components: string = 'Components: ';
	static readonly str_Duration: string = 'Duration: ';

	static readonly str_ConcentrationPrefix: string = 'Concentration, ';

	// appearance:
	static readonly titleFontName: string = 'Modesto Condensed Bold';
	static readonly detailFontName: string = 'mrs-eaves';
	static readonly titleFontIdealSize: number = 30;
	static readonly titleLeftMargin: number = 25;
	static readonly titleFirstIconSpacing: number = 10;
	static readonly iconSpacing: number = 8;
	static readonly iconScaleDenominator: number = 38; // Divide title font size by this to get concentration icon scale
	static readonly titleFontCenterYAdjust: number = 3;
	static readonly iconSize: number = 36;
	static readonly detailFontSize: number = 18;
	static readonly castingSubDetailIndent: number = 8;
	static readonly spellDetailsWidth: number = 220;
	static readonly schoolOfMagicIndent: number = 20;
	static readonly schoolOfMagicWidth: number = 115 + SpellBook.schoolOfMagicIndent;
	static readonly spellDescriptionWidth: number = 320;
	static readonly titleLevelMargin: number = 3;
	static readonly detailSpacing: number = 4;
	static readonly abjurationAdjust: number = 4;
	static readonly emphasisFontHeightIncrease: number = 3;
	static readonly emphasisFontStyleAscender: number = 19;
	static readonly bulletIndent: number = SpellBook.detailFontSize * 1.2;

	static readonly fadeInTime: number = 500;


	static styleDelimiters: Array<LayoutDelimiters>;

	static readonly levelDetailsMargin: number = 6;
	static readonly maxSpellBookHeight: number = 1000;

	static readonly bookSpellHeightAdjust: number = 19; // Magic number based on spell book height calculations - always seem to be off by 12.

	// ![](F8879E41D1F82E6B2F1C5885DBB02DD4.png)
	static readonly bookGlowTopMargin: number = 85;

	// ![](09A034744A84F0C0C4A81FD1E58B7ED2.png)
	static readonly bookGlowLeftMargin: number = 88;

	// ![](2DF27D9DAA37DB8F3F1EF93291E493C7.png;;;0.01472,0.01472)
	static readonly bookGlowHeight: number = 626;


	// ![](EC5995AEDA28089361922471641200D0.png;;;0.00982,0.00982)
	static readonly fredHeadX: number = 355;  // right side of Fred's arrows.
	static readonly fredHeadY: number = 710;  // top of Fred's head.
	static readonly fredShoulderX: number = 380;  // top of Fred's shoulder.
	static readonly fredShoulderY: number = 893;  // right side of Fred's shoulder.

	// positioning logic:
	// 1. Can we get the spell above Fred's head.
	// 2. If not, move it to the top of the screen and to the right of Fred's head.

	static readonly spellPageRightEdge: number = 350;
	static readonly maxTitleWidth: number = SpellBook.spellPageRightEdge - SpellBook.titleLeftMargin;

	static readonly bottomWindowHeight: number = 1080;
	static readonly spellBottomHeight: number = 1022;

	// ![](C1C6705D2CAADF7C85591DA2A1E0A6DF.png)
	static readonly spellBottomMargin: number = 23;

	// ![](C1726232AA0D358FB5CC3FE2FE14D260.png)  schoolOfMagicHeight
	static readonly schoolOfMagicHeight: number = 109;

	// ![](ADB813D0BFBAA54EC6A49957A0FB6E2E.png)
	static readonly schoolOfMagicAbjurationHeight: number = 117;

	// ![](BCA0565D701D0D116C5132DE9B222FD6.png)
	public static readonly spellHeaderHeight: number = 79;
	static readonly textColor: string = '#1a0c0a';
	static readonly bulletColor: string = '#5b3c35';
	static readonly emphasisColor: string = '#a01a00';
	static readonly tableLineColor: string = '#5b3c35';

	wordRenderer: WordRenderer;
	wordWrapper: WordWrapper;
	spellBookBack: Sprites;
	spellBookTop: Sprites;
	schoolOfMagic: Sprites;
	concentrationIcon: Sprites;
	morePowerIcon: Sprites;
	bookGlow: Sprites;
	bookBurn: Sprites;
	spellBookAppearBig: Sprites;
	spellBookAppearMedium: Sprites;
	spellBookAppearSmall: Sprites;
	lastPlayerId: number;
	calculatedFontYOffset = 0;
	lastSpellName: string;
	titleTopLeft: Vector;
	levelSchoolTopLeft: Vector;
	titleColors: Array<string>;
	hueShifts: Array<number>;
	titleFontSize: number = SpellBook.titleFontIdealSize;
	spellDetailsTopLeft: Vector;
	spellDescriptionTopLeft: Vector;
	spellBookBackHeight = 0;
	horizontalScale = 1;
	descriptionParagraphs: ParagraphWrapData;
	titleWidth: number;
	lastSpellSlotLevel: number;
	wrappedSubCastingLines: LineWrapData[];
	wrappedSubRangeLines: LineWrapData[];
	wrappedSubComponentMaterialLines: LineWrapData[];
	castingTime: string;
	componentSummary: string;
	rangeSummary: string;
	availableSpellDetailsWidth: number;
	schoolOfMagicAdjust: number;
	spellIcon: HTMLImageElement;
	spellIconFrame: HTMLImageElement;

	constructor() {
		this.initializeWordWrapHelpers();

		this.loadFonts();
		this.loadColors();
		this.spellIconFrame = new Image();
		this.spellIconFrame.src = 'GameDev/Assets/DragonH/Scroll/Spells/SpellIconFrame.png';
	}

	private loadColors() {
		this.titleColors = [];
		this.titleColors.push('#000000'); // None
		this.titleColors.push('#2a6470'); // Abjuration    
		this.titleColors.push('#414695'); // Illusion			 
		this.titleColors.push('#602f81'); // Conjuration	 
		this.titleColors.push('#7b2f56'); // Enchantment   
		this.titleColors.push('#733831'); // Necromancy    
		this.titleColors.push('#216333'); // Evocation			
		this.titleColors.push('#2c5818'); // Transmutation	
		this.titleColors.push('#594f09'); // Divination
		this.titleColors.push('#7c5d0c'); // Heavenly
		this.titleColors.push('#9a2c3d'); // Satanic

		this.hueShifts = [];
		this.hueShifts.push(0);		// None
		this.hueShifts.push(191); // Abjuration     
		this.hueShifts.push(238); // Illusion			 
		this.hueShifts.push(277); // Conjuration	   
		this.hueShifts.push(331); // Enchantment    
		this.hueShifts.push(7);		// Necromancy     
		this.hueShifts.push(137); // Evocation			 
		this.hueShifts.push(101); // Transmutation	 
		this.hueShifts.push(52);  // Divination		 
		this.hueShifts.push(44);  // Heavenly
		this.hueShifts.push(352); // Satanic
	}

	initializeWordWrapHelpers() {
		SpellBook.styleDelimiters = [
			new LayoutDelimiters(LayoutStyle.calculated, '«', '»', this.calculatedFontYOffset),
			new LayoutDelimiters(LayoutStyle.bold, '**', '**'),
			new LayoutDelimiters(LayoutStyle.italic, '*', '*')
		];

		this.wordWrapper = new WordWrapper();

		this.wordRenderer = new WordRenderer();
		this.wordRenderer.fontName = SpellBook.detailFontName;
		this.wordRenderer.fontSize = SpellBook.detailFontSize;
		this.wordRenderer.emphasisColor = SpellBook.emphasisColor;
		this.wordRenderer.emphasisFontHeightIncrease = SpellBook.emphasisFontHeightIncrease;
		this.wordRenderer.emphasisFontStyleAscender = SpellBook.emphasisFontStyleAscender;
		this.wordRenderer.bulletIndent = SpellBook.bulletIndent;
		this.wordRenderer.bulletColor = SpellBook.bulletColor;
		this.wordRenderer.textColor = SpellBook.textColor;
		this.wordRenderer.tableLineColor = SpellBook.tableLineColor;
	}

	private loadFonts() {
		// @ts-ignore - FontFace
		const junctionFont = new FontFace('Modesto Condensed Bold', 'url(GameDev/Assets/DragonH/Fonts/617b1b0fd4637fda235c73f27d530305.woff)');
		junctionFont.load().then(function (loadedFace) {
			// @ts-ignore - document.fonts
			document.fonts.add(loadedFace);
			document.body.style.fontFamily = '"Modesto Condensed Bold", Arial';
		}).catch(function (error) {
			console.log('Font loading error: ' + error);
		});
	}

	drawSpellTitle(now: number, context: CanvasRenderingContext2D, spell: ActiveSpellData): void {
		this.setTitleFont(context, this.titleFontSize);
		context.textBaseline = 'top';
		context.textAlign = 'left';
		//context.fillStyle = this.titleColors[spell.schoolOfMagic];
		context.fillStyle = this.titleColors[SchoolOfMagic.Necromancy];
		context.fillText(spell.name, this.titleTopLeft.x, this.titleTopLeft.y);
	}

	getSpellLevelSchoolWidth(context: CanvasRenderingContext2D, spell: ActiveSpellData): number {
		this.wordRenderer.setDetailFontNormal(context);
		const levelStr: string = this.getLevelStr(spell);
		const castAtLevelStr = '';
		const castStr: string = this.getSchoolCastingStr(levelStr, spell, castAtLevelStr);

		let width: number = context.measureText(castStr).width;

		if (spell.morePowerfulAtHigherLevels && spell.spellSlotLevel > spell.spellLevel) {
			if (spell.powerComesFromCasterLevel) {
				width += context.measureText(`, cast by a level ${spell.playerLevel} adventurer`).width;
			}
			else {
				this.wordRenderer.setDetailFontBold(context);
				width += context.measureText('upcast ').width;

				this.wordRenderer.setDetailFontNormal(context);
				width += context.measureText('to ').width;

				this.wordRenderer.setDetailFontBold(context);
				width += context.measureText(`slot level ${spell.spellSlotLevel}`).width;
			}

			this.wordRenderer.setActiveStyle(context, LayoutStyle.normal);
			return width;
		}
	}

	private getLevelStr(spell: ActiveSpellData) {
		if (spell.spellLevel === -1)
			return 'Special';

		if (spell.spellLevel === 0)
			return 'Cantrip';

		return `Level ${spell.spellLevel}`;
	}

	drawSpellLevelSchool(now: number, context: CanvasRenderingContext2D, spell: ActiveSpellData): void {
		this.wordRenderer.setDetailFontNormal(context);
		context.fillStyle = SpellBook.textColor;
		const levelStr: string = this.getLevelStr(spell);
		const castAtLevelStr = '';

		const castStr: string = this.getSchoolCastingStr(levelStr, spell, castAtLevelStr);
		let x: number = this.levelSchoolTopLeft.x;
		const y: number = this.levelSchoolTopLeft.y;
		context.fillText(castStr, x, y);

		if (spell.morePowerfulAtHigherLevels && spell.spellSlotLevel > spell.spellLevel) {
			x += context.measureText(castStr).width;
			const commaSepStr = ', ';
			context.fillText(commaSepStr, x, y);
			x += context.measureText(commaSepStr).width;

			if (spell.powerComesFromCasterLevel) {
				const castByAStr = 'cast by a ';
				this.wordRenderer.setDetailFontNormal(context);
				context.fillText(castByAStr, x, y);
				x += context.measureText(castByAStr).width;

				const levelStr = `level ${spell.playerLevel} `;
				this.wordRenderer.setDetailFontBold(context);
				context.fillStyle = SpellBook.emphasisColor;
				context.fillText(levelStr, x, y);
				x += context.measureText(levelStr).width;

				this.wordRenderer.setDetailFontNormal(context);
				context.fillStyle = SpellBook.textColor;
				context.fillText('adventurer', x, y);
			}
			else {
				const upcastStr = 'upcast ';
				this.wordRenderer.setDetailFontBold(context);
				context.fillText(upcastStr, x, y);
				x += context.measureText(upcastStr).width;

				const atStr = 'to ';
				this.wordRenderer.setDetailFontNormal(context);
				context.fillText(atStr, x, y);
				x += context.measureText(atStr).width;

				this.wordRenderer.setDetailFontBold(context);
				context.fillStyle = SpellBook.emphasisColor;
				context.fillText(`slot level ${spell.spellSlotLevel}`, x, y);
				context.fillStyle = SpellBook.textColor;
			}


			this.wordRenderer.setDetailFontNormal(context);
		}
	}

	private getSchoolCastingStr(levelStr: string, spell: ActiveSpellData, castAtLevelStr: string): string {
		return `${levelStr} ${this.toSchoolDisplayName(spell.schoolOfMagic)}${castAtLevelStr}`;
	}

	drawSpellDescription(now: number, context: CanvasRenderingContext2D, spell: ActiveSpellData): void {
		this.wordRenderer.setDetailFontNormal(context);
		context.fillStyle = SpellBook.textColor;
		const x: number = this.spellDescriptionTopLeft.x;
		const y: number = this.spellDescriptionTopLeft.y;
		this.wordRenderer.activeStyle = LayoutStyle.normal;
		this.wordRenderer.renderParagraphs(context, this.descriptionParagraphs.lineData, x, y, SpellBook.styleDelimiters);
	}
	
	getDetailSize(context: CanvasRenderingContext2D, spell: ActiveSpellData): Vector {
		this.wordRenderer.setDetailFontNormal(context);

		let lineCount = 0;
		let spacerCount = 0;
		let height = 0;
		let width = 0;

		height += SpellBook.detailFontSize;  // Casting time.
		lineCount++; // Casting time.
		this.wrappedSubCastingLines = [];
		this.wrappedSubRangeLines = [];
		this.wrappedSubComponentMaterialLines = [];

		function checkWidth(testWidth: number) {
			if (testWidth > width)
				width = testWidth;
		}

		const commaPos: number = spell.castingTimeStr.indexOf(',');

		if (commaPos < 0)
			this.castingTime = spell.castingTimeStr;
		else {
			this.castingTime = spell.castingTimeStr.substr(0, commaPos).trim();
			const remainingCastingDetails: string = spell.castingTimeStr.substr(commaPos + 1).trim();
			this.wrappedSubCastingLines = this.getWrappedSubDetailLines(context, remainingCastingDetails);
			const castingLinesExtraCount: number = this.wrappedSubCastingLines.length;
			height += SpellBook.detailFontSize * castingLinesExtraCount;
			lineCount += castingLinesExtraCount;
		}

		checkWidth(this.measureDetail(context, SpellBook.str_CastingTime, this.castingTime));

		height += SpellBook.detailSpacing;
		spacerCount++;

		height += SpellBook.detailFontSize + SpellBook.detailSpacing;  // Range
		lineCount++;
		spacerCount++;
		const parenPos: number = spell.rangeStr.indexOf('(');

		if (parenPos < 0)
			this.rangeSummary = spell.rangeStr;
		else {
			this.rangeSummary = spell.rangeStr.substr(0, parenPos).trim();
			const remainingRangeDetails: string = spell.rangeStr.substr(parenPos).trim();
			this.wrappedSubRangeLines = this.getWrappedSubDetailLines(context, remainingRangeDetails);
			const rangeLinesExtraCount: number = this.wrappedSubRangeLines.length;
			height += SpellBook.detailFontSize * rangeLinesExtraCount;
			lineCount += rangeLinesExtraCount;
		}

		checkWidth(this.measureDetail(context, SpellBook.str_Range, this.rangeSummary));

		if (spell.componentsStr) {
			height += SpellBook.detailFontSize;
			lineCount++;

			const parenIndex = spell.componentsStr.indexOf('(');

			if (parenIndex > 0) {
				this.componentSummary = spell.componentsStr.substr(0, parenIndex).trim();
				const componentMaterials: string = spell.componentsStr.substr(parenIndex);
				if (componentMaterials) {
					this.wrappedSubComponentMaterialLines = this.getWrappedSubDetailLines(context, componentMaterials);
					const componentsExtraLines: number = this.wrappedSubComponentMaterialLines.length;

					height += SpellBook.detailFontSize * componentsExtraLines;
					lineCount += componentsExtraLines;
				}
			}
			else
				this.componentSummary = spell.componentsStr;

			checkWidth(this.measureDetail(context, SpellBook.str_Components, this.componentSummary));

			height += SpellBook.detailSpacing;
			spacerCount++;
		}

		height += SpellBook.detailFontSize + SpellBook.detailSpacing;   // Duration
		lineCount++;

		const durationStr: string = this.getDurationStr(spell);
		checkWidth(this.measureDetail(context, SpellBook.str_Duration, durationStr));

		console.log('Detail: lineCount: ' + lineCount);
		console.log('Detail: spacerCount: ' + spacerCount);
		return new Vector(width, height);
	}

	drawSpellDetails(now: number, context: CanvasRenderingContext2D, spell: ActiveSpellData): void {
		context.fillStyle = SpellBook.textColor;
		const x: number = this.spellDetailsTopLeft.x + this.schoolOfMagicAdjust;
		let y: number = this.spellDetailsTopLeft.y;


		this.showDetail(context, SpellBook.str_CastingTime, this.castingTime, x, y);
		y += SpellBook.detailFontSize;

		if (this.wrappedSubCastingLines) {
			const castingTimeX: number = x + SpellBook.castingSubDetailIndent;
			this.wordRenderer.setDetailFontNormal(context);
			y = this.wrapDetailLines(context, this.wrappedSubCastingLines, castingTimeX, y);
		}
		y += SpellBook.detailSpacing;



		this.showDetail(context, SpellBook.str_Range, this.rangeSummary, x, y);
		y += SpellBook.detailFontSize;
		if (this.wrappedSubRangeLines) {
			const materialX: number = x + SpellBook.castingSubDetailIndent;
			this.wordRenderer.setDetailFontNormal(context);
			y = this.wrapDetailLines(context, this.wrappedSubRangeLines, materialX, y);
		}
		y += SpellBook.detailSpacing;


		if (spell.componentsStr) {
			this.showDetail(context, SpellBook.str_Components, this.componentSummary, x, y);
			y += SpellBook.detailFontSize;

			if (this.wrappedSubComponentMaterialLines) {
				const materialX: number = x + SpellBook.castingSubDetailIndent;
				this.wordRenderer.setDetailFontNormal(context);
				y = this.wrapDetailLines(context, this.wrappedSubComponentMaterialLines, materialX, y);
			}
			y += SpellBook.detailSpacing;
		}

		const durationStr: string = this.getDurationStr(spell);
		this.showDetail(context, SpellBook.str_Duration, durationStr, x, y);
	}

	private getDurationStr(spell: ActiveSpellData): string {
		let durationStr: string = spell.durationStr;
		const concentrationPrefix: string = SpellBook.str_ConcentrationPrefix;
		if (durationStr.startsWith(concentrationPrefix))
			durationStr = spell.durationStr.substr(concentrationPrefix.length);
		return durationStr;
	}

	// TODO: rename "wrap" to "render"
	private wrapDetailLines(context: CanvasRenderingContext2D, lines: LineWrapData[], x: number, y: number) {
		for (let i = 0; i < lines.length; i++) {
			context.fillText(lines[i].line, x, y);
			y += SpellBook.detailFontSize;
		}
		return y;
	}

	private getWrappedSubDetailLines(context: CanvasRenderingContext2D, detailStr: string) {
		return this.wordWrapper.getWordWrappedLines(context, detailStr, this.availableSpellDetailsWidth * this.horizontalScale - SpellBook.castingSubDetailIndent, SpellBook.styleDelimiters, this.wordRenderer);
	}

	showDetail(context: CanvasRenderingContext2D, label: string, data: string, x: number, y: number): void {
		this.wordRenderer.setDetailFontNormal(context);
		context.fillText(label, x, y);
		x += context.measureText(label).width;
		this.wordRenderer.setDetailFontBold(context);
		context.fillText(data, x, y);
	}

	measureDetail(context: CanvasRenderingContext2D, label: string, data: string): number {
		let result = 0;
		this.wordRenderer.setDetailFontNormal(context);
		result += context.measureText(label).width;
		this.wordRenderer.setDetailFontBold(context);
		result += context.measureText(data).width;
		return result;
	}

	toSchoolDisplayName(schoolOfMagic: SchoolOfMagic): string {
		switch (schoolOfMagic) {
			case SchoolOfMagic.Abjuration: return 'abjuration';
			case SchoolOfMagic.Conjuration: return 'conjuration';
			case SchoolOfMagic.Divination: return 'divination';
			case SchoolOfMagic.Enchantment: return 'enchantment';
			case SchoolOfMagic.Evocation: return 'evocation';
			case SchoolOfMagic.Illusion: return 'illusion';
			case SchoolOfMagic.Necromancy: return 'necromancy';
			case SchoolOfMagic.Transmutation: return 'transmutation';
			case SchoolOfMagic.Heavenly: return 'heavenly power';
			case SchoolOfMagic.Satanic: return 'satanic power';
		}
		return '';
	}

	private setTitleFont(context: CanvasRenderingContext2D, fontSize: number) {
		context.font = `${fontSize}px ${SpellBook.titleFontName}`;
	}

	spellbookAppearTime: number;
	bookAlpha: number;

	draw(nowSec: number,
		context: CanvasRenderingContext2D,
		x: number,
		y: number,
		player: Character): void {

		const spell: ActiveSpellData = player.ActiveSpell;

		if (!spell)
			return;

		const nowMs: number = nowSec * 1000;
		if (this.lastSpellName !== spell.name || this.lastSpellSlotLevel !== spell.spellSlotLevel) {
			this.createSpellBook(spell, player, context, x, y, nowMs);
		}
		const timeIn: number = nowMs - this.spellbookAppearTime;
		if (timeIn < SpellBook.fadeInTime) {
			const percentThroughFadeIn: number = timeIn / SpellBook.fadeInTime;
			this.bookAlpha = percentThroughFadeIn;
		}
		else
			this.bookAlpha = 1;

		this.bookGlow.draw(context, nowMs);

		if (this.spellBookBack.spriteProxies.length > 0) {
			const firstBackSprite = this.spellBookBack.spriteProxies[0];
			// ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)
			const w: number = this.spellBookBack.spriteWidth;
			const h: number = this.spellBookBack.spriteHeight;
			firstBackSprite.opacity = this.bookAlpha;
			this.spellBookBack.drawCropped(context, nowMs, firstBackSprite.x, firstBackSprite.y + h - this.spellBookBackHeight, 0, h - this.spellBookBackHeight, w, this.spellBookBackHeight, w * this.horizontalScale, this.spellBookBackHeight);
		}

		this.spellBookTop.opacity = this.bookAlpha;
		this.spellBookTop.draw(context, nowMs);
		this.drawSchoolOfMagic(context, nowMs);
		this.concentrationIcon.draw(context, nowMs);
		this.morePowerIcon.draw(context, nowMs);

		this.drawSpellTitle(nowSec, context, spell);
		this.drawSpellLevelSchool(nowSec, context, spell);
		this.drawSpellDetails(nowSec, context, spell);
		this.drawSpellDescription(nowSec, context, spell);

		this.bookBurn.draw(context, nowMs);
		this.drawSpellBookAppear(nowSec, context, spell);
	}

	drawSchoolOfMagic(context: CanvasRenderingContext2D, nowMs: number): void {
		if (this.hasCustomIcon) {
			const x: number = SpellBook.spellIconMarginLeft + this.schoolOfMagicTopLeft.x;
			const y: number = SpellBook.spellIconMarginTop + this.schoolOfMagicTopLeft.y;
			context.drawImage(this.spellIcon, x, y, SpellBook.spellIconTargetWidth, SpellBook.spellIconTargetWidth);
			context.drawImage(this.spellIconFrame, x - 2, y - 2);
		}
		else
			this.schoolOfMagic.draw(context, nowMs);
	}

	soundManager: DragonBackSounds;
	private createSpellBook(spell: ActiveSpellData, player: Character, context: CanvasRenderingContext2D, x: number, y: number, nowMs: number) {
		this.lastSpellName = spell.name;
		this.lastSpellSlotLevel = spell.spellSlotLevel;
		this.lastPlayerId = player.playerID;
		let scale = 1;
		while (scale < 3 && !this.createSpellPage(context, x, y, spell, scale)) {
			scale += 0.01;
		}
		this.findSpellIcon(spell);
		this.spellbookAppearTime = nowMs;
		this.addSpellBookAppearSoundEffects(spell);
	}

	hasCustomIcon: boolean;

	static readonly spellIconTargetWidth: number = 100;
	static readonly spellIconMarginLeft: number = 5;
	static readonly spellIconMarginTop: number = 2;

	convertToValidFilename(str): string {
		return (str.replace(/[/|\\:*?"<>]/g, "_"));
	}

	findSpellIcon(spell: ActiveSpellData): void {
		this.spellIcon = new Image();

		this.hasCustomIcon = false;
		const spellIconName: string = 'GameDev/Assets/DragonH/Scroll/Spells/Icons/' + this.convertToValidFilename(spell.name) + '.png';
		console.log('spellIconName: ' + spellIconName);

		this.spellIcon.onload = () => {
			this.hasCustomIcon = true;
			console.log('spell icon loaded: ' + spell.name);
		};

		this.spellIcon.src = spellIconName;
	}

	setSoundManager(dragonBackSounds: DragonBackSounds): void {
		this.soundManager = dragonBackSounds;
	}

	addSpellBookAppearSoundEffects(spell: ActiveSpellData): void {
		if (spell.schoolOfMagic === SchoolOfMagic.None)
			return;
		this.soundManager.safePlayMp3('SpellBook/PageFlip[2]');
		this.soundManager.playMp3In(380, 'SpellBook/SpellBookOpen[5]');
		const schoolOfMagicStartTime = 1150;
		switch (spell.schoolOfMagic) {
			case SchoolOfMagic.Abjuration:
				this.soundManager.playMp3In(schoolOfMagicStartTime, 'SpellBook/Abjuration');
				break;
			case SchoolOfMagic.Illusion:
				this.soundManager.playMp3In(schoolOfMagicStartTime, 'SpellBook/Illusion');
				break;
			case SchoolOfMagic.Conjuration:
				this.soundManager.playMp3In(schoolOfMagicStartTime, 'SpellBook/Conjuration');
				break;
			case SchoolOfMagic.Enchantment:
				this.soundManager.playMp3In(schoolOfMagicStartTime, 'SpellBook/Enchantment');
				break;
			case SchoolOfMagic.Necromancy:
			case SchoolOfMagic.Satanic:
				this.soundManager.playMp3In(schoolOfMagicStartTime, 'SpellBook/Necromancy');
				break;
			case SchoolOfMagic.Evocation:
				this.soundManager.playMp3In(schoolOfMagicStartTime, 'SpellBook/Evocation');
				break;
			case SchoolOfMagic.Transmutation:
				this.soundManager.playMp3In(schoolOfMagicStartTime, 'SpellBook/Transmutation');
				break;
			case SchoolOfMagic.Divination:
			case SchoolOfMagic.Heavenly:
				this.soundManager.playMp3In(schoolOfMagicStartTime, 'SpellBook/Divination');
				break;
		}
	}

	drawSpellBookAppear(nowMs: number, context: CanvasRenderingContext2D, spell: ActiveSpellData): void {
		const timeSec: number = nowMs * 1000;
		this.spellBookAppearBig.draw(context, timeSec);
		this.spellBookAppearMedium.draw(context, timeSec);
		this.spellBookAppearSmall.draw(context, timeSec);
	}


	getTitleFontSize(context: CanvasRenderingContext2D, name: string, requiresConcentration: boolean, morePowerfulAtHigherLevels: boolean): number {
		let fontSize: number = SpellBook.titleFontIdealSize;
		this.setTitleFont(context, fontSize);
		this.titleWidth = context.measureText(name).width;

		function getIconSpace(iconScale: number, horizontalScale: number): number {
			let iconSpace = 0;
			if (requiresConcentration || morePowerfulAtHigherLevels) {
				iconSpace += SpellBook.titleFirstIconSpacing * horizontalScale + SpellBook.iconSize * iconScale;
				if (requiresConcentration && morePowerfulAtHigherLevels)
					iconSpace += SpellBook.iconSpacing * horizontalScale + SpellBook.iconSize * iconScale;
			}
			return iconSpace;
		}

		const maxTitleWidth: number = SpellBook.maxTitleWidth * this.horizontalScale;

		while (this.titleWidth > maxTitleWidth - getIconSpace(this.getIconScale(fontSize), this.horizontalScale)) {
			fontSize--;
			if (fontSize <= 6) {
				return fontSize;
			}
			this.setTitleFont(context, fontSize);
			this.titleWidth = context.measureText(name).width;
		}
		return fontSize;
	}

	schoolOfMagicTopLeft: Vector;

	loadResources(): void {
		this.spellBookBack = new Sprites("Scroll/Spells/BookBottom", 1, 0, AnimationStyle.Static);
		this.spellBookBack.originX = 0;
		this.spellBookBack.originY = 999;
		this.spellBookTop = new Sprites("Scroll/Spells/BookTop", 1, 0, AnimationStyle.Static);
		this.schoolOfMagic = new Sprites("Scroll/Spells/SchoolsOfMagic", 10, 0, AnimationStyle.Static);
		this.concentrationIcon = new Sprites("Scroll/Spells/Concentration/Concentration", 10, 0, AnimationStyle.Static);
		this.concentrationIcon.originX = 0;
		this.concentrationIcon.originY = SpellBook.iconSize;
		this.morePowerIcon = new Sprites("Scroll/Spells/MorePower/MorePower", 10, 0, AnimationStyle.Static);
		this.morePowerIcon.originX = 0;
		this.morePowerIcon.originY = SpellBook.iconSize;
		this.bookGlow = new Sprites("Scroll/Spells/BookMagic/BookMagic", 119, fps30, AnimationStyle.Loop, true);
		this.bookBurn = new Sprites("Scroll/Spells/Appear/BookBurn/BookBurn", 28, fps40, AnimationStyle.Sequential, true);

		const saveBypassFrameSkip: boolean = globalBypassFrameSkip;
		globalBypassFrameSkip = false;
		this.spellBookAppearBig = new Sprites("Scroll/Spells/Appear/Big/Big", 129, fps100, AnimationStyle.Sequential, true);
		this.spellBookAppearMedium = new Sprites("Scroll/Spells/Appear/Medium/Medium", 116, fps100, AnimationStyle.Sequential, true);
		this.spellBookAppearSmall = new Sprites("Scroll/Spells/Appear/Small/Small", 83, fps50, AnimationStyle.Sequential, true);
		globalBypassFrameSkip = saveBypassFrameSkip;

		this.spellBookAppearBig.originX = 399;
		this.spellBookAppearBig.originY = 429

		this.spellBookAppearMedium.originX = 324;
		this.spellBookAppearMedium.originY = 346

		this.spellBookAppearSmall.originX = 171;
		this.spellBookAppearSmall.originY = 171

	}

	createSpellPage(context: CanvasRenderingContext2D, x: number, y: number, spell: ActiveSpellData, horizontalScale: number): boolean {
		this.horizontalScale = horizontalScale;

		this.wordRenderer.setDetailFontNormal(context);
		const maxSpellDescriptionWidth: number = SpellBook.spellDescriptionWidth * this.horizontalScale;
		this.descriptionParagraphs = this.wordWrapper.getWordWrappedLinesForParagraphs(context, spell.description, maxSpellDescriptionWidth, SpellBook.styleDelimiters, this.wordRenderer);

		if (this.descriptionParagraphs.maxTableWidth > maxSpellDescriptionWidth)
			return false;

		if (this.getSpellLevelSchoolWidth(context, spell) > maxSpellDescriptionWidth)
			return false;

		this.spellBookBack.spriteProxies = [];
		this.spellBookTop.spriteProxies = [];
		this.schoolOfMagic.spriteProxies = [];
		this.concentrationIcon.spriteProxies = [];
		this.morePowerIcon.spriteProxies = [];
		this.bookGlow.spriteProxies = [];
		this.bookBurn.spriteProxies = [];

		this.spellBookAppearBig.spriteProxies = [];
		this.spellBookAppearMedium.spriteProxies = [];
		this.spellBookAppearSmall.spriteProxies = [];

		let left: number = x + 12;
		let top: number = y - 33;


		this.titleFontSize = this.getTitleFontSize(context, spell.name, spell.requiresConcentration, spell.morePowerfulAtHigherLevels);

		const descriptionLinesWeNeedToAdd: number = this.descriptionParagraphs.lineData.length;
		const descriptionHeightWeNeedToAdd: number = descriptionLinesWeNeedToAdd * (SpellBook.detailFontSize);


		this.schoolOfMagicAdjust = 0;
		this.availableSpellDetailsWidth = SpellBook.spellDetailsWidth;
		let schoolOfMagicWidth: number = SpellBook.schoolOfMagicWidth;

		let schoolOfMagicIndent: number;
		if (spell.schoolOfMagic === SchoolOfMagic.None) {
			schoolOfMagicIndent = 0;
			schoolOfMagicWidth = 0;
			this.availableSpellDetailsWidth += SpellBook.schoolOfMagicWidth;
		}
		else {
			if (spell.schoolOfMagic === SchoolOfMagic.Abjuration) {
				this.schoolOfMagicAdjust = SpellBook.abjurationAdjust;
				this.availableSpellDetailsWidth -= SpellBook.abjurationAdjust;
			}

			schoolOfMagicIndent = SpellBook.schoolOfMagicIndent - this.schoolOfMagicAdjust;
		}


		const detailSize: Vector = this.getDetailSize(context, spell);
		const detailsHeight: number = Math.max(detailSize.y + SpellBook.detailFontSize, this.getSchoolOfMagicHeight(spell.schoolOfMagic));

		let totalSpellPageHeight: number = SpellBook.spellHeaderHeight + detailsHeight + descriptionHeightWeNeedToAdd;

		if (this.getSpellDescriptionBottom(top, totalSpellPageHeight) > SpellBook.bottomWindowHeight) {
			top = this.getNewTop(SpellBook.bottomWindowHeight, totalSpellPageHeight);
		}

		let spellBottom: number = this.getSpellDescriptionBottom(top, totalSpellPageHeight);
		if (spellBottom > SpellBook.fredHeadY) {
			top = this.getNewTop(SpellBook.fredHeadY, totalSpellPageHeight);
		}

		if (top < 0) {
			top = 0;

			if (this.getSpellDescriptionBottom(top, totalSpellPageHeight) > SpellBook.maxSpellBookHeight)
				return false;  // Not enough space for the spell at this horizontal scale.
		}

		spellBottom = this.getSpellDescriptionBottom(top, totalSpellPageHeight);

		if (spellBottom > SpellBook.fredShoulderY) {
			left = Math.max(left, SpellBook.fredShoulderX);
		}
		else if (spellBottom > SpellBook.fredHeadY) {
			left = Math.max(left, SpellBook.fredHeadX);
		}

		this.spellDescriptionTopLeft = new Vector(left + SpellBook.titleLeftMargin, top + SpellBook.spellHeaderHeight + detailsHeight);
		this.titleTopLeft = new Vector(left + SpellBook.titleLeftMargin, top + 27);
		this.levelSchoolTopLeft = new Vector(left + SpellBook.titleLeftMargin, this.titleTopLeft.y + this.titleFontSize + SpellBook.titleLevelMargin);
		this.schoolOfMagicTopLeft = new Vector(left + schoolOfMagicIndent, this.levelSchoolTopLeft.y + SpellBook.detailFontSize + SpellBook.levelDetailsMargin);

		this.spellDetailsTopLeft = new Vector(left + schoolOfMagicWidth, this.schoolOfMagicTopLeft.y);
		if (detailSize.x > this.availableSpellDetailsWidth * horizontalScale)
			return false;

		const lowerSpellBookTop: number = top + totalSpellPageHeight;

		this.spellBookBackHeight = totalSpellPageHeight;
		const spellBookBack: SpriteProxy = this.spellBookBack.add(left, lowerSpellBookTop, 0);
		spellBookBack.fadeInTime = SpellBook.fadeInTime;
		spellBookBack.horizontalScale = this.horizontalScale;
		spellBookBack.timeStart = 0;
		const spellBookFront: SpriteProxy = this.spellBookTop.add(left, top, 0);
		spellBookFront.fadeInTime = SpellBook.fadeInTime;
		spellBookFront.timeStart = 0;
		spellBookFront.horizontalScale = this.horizontalScale;

		const schoolOfMagicIndex = spell.schoolOfMagic - 1;
		if (spell.schoolOfMagic > SchoolOfMagic.None) {
			const schoolOfMagicIcon: SpriteProxy = this.schoolOfMagic.add(this.schoolOfMagicTopLeft.x, this.schoolOfMagicTopLeft.y, schoolOfMagicIndex);
			schoolOfMagicIcon.timeStart = 0;
			schoolOfMagicIcon.fadeInTime = SpellBook.fadeInTime;
		}

		const iconScale: number = this.getIconScale(this.titleFontSize);
		let iconX = left + SpellBook.titleLeftMargin + this.titleWidth + SpellBook.titleFirstIconSpacing * this.horizontalScale;
		const iconY = this.titleTopLeft.y + this.titleFontSize;

		if (spell.requiresConcentration) {
			//let concentratingIcon: SpriteProxy = this.concentrationIcon.add(iconX, iconY, schoolOfMagicIndex);
			const concentratingIcon: SpriteProxy = this.concentrationIcon.add(iconX, iconY, SchoolOfMagic.Necromancy - 1);
			concentratingIcon.scale = iconScale;
			concentratingIcon.fadeInTime = SpellBook.fadeInTime;
			iconX += SpellBook.iconSize * iconScale + SpellBook.iconSpacing * this.horizontalScale;
		}

		if (spell.morePowerfulAtHigherLevels) {
			//let morePowerful: SpriteProxy = this.morePowerIcon.add(iconX, iconY, schoolOfMagicIndex);
			const morePowerful: SpriteProxy = this.morePowerIcon.add(iconX, iconY, SchoolOfMagic.Necromancy - 1);
			morePowerful.scale = iconScale;
			morePowerful.fadeInTime = SpellBook.fadeInTime;
		}

		// TODO: Consider adding SpellBook.spellBottomMargin instead of SpellBook.bookSpellHeightAdjust
		totalSpellPageHeight += SpellBook.bookSpellHeightAdjust;

		//` <formula 2.5; verticalScaleGlow = \frac{totalSpellPageHeight}{bookGlowHeight}>

		const verticalScaleGlow: number = totalSpellPageHeight / SpellBook.bookGlowHeight;
		const topScaledMargin: number = SpellBook.bookGlowTopMargin * verticalScaleGlow;

		const hueShift: number = this.hueShifts[spell.schoolOfMagic];
		const bookGlow: ColorShiftingSpriteProxy = this.bookGlow.addShifted(left - SpellBook.bookGlowLeftMargin * this.horizontalScale, top - topScaledMargin, 0, hueShift);
		bookGlow.fadeInTime = SpellBook.fadeInTime;
		bookGlow.horizontalScale = this.horizontalScale;
		bookGlow.verticalScale = verticalScaleGlow;

		const bookBurn: ColorShiftingSpriteProxy = this.bookBurn.addShifted(left - SpellBook.bookGlowLeftMargin * this.horizontalScale, top - topScaledMargin, 0, hueShift);
		bookBurn.horizontalScale = this.horizontalScale;
		bookBurn.verticalScale = verticalScaleGlow;

		const bookGlowWidth: number = horizontalScale * (this.bookGlow.spriteWidth - SpellBook.bookGlowLeftMargin * 2);

		const centerX: number = left + bookGlowWidth / 2.0;
		const centerY: number = top + totalSpellPageHeight / 2.0;

		const verticalScaleBig: number = 1.2 * totalSpellPageHeight / this.spellBookAppearBig.spriteHeight;
		const horizontalScaleBig: number = 1.2 * bookGlowWidth / this.spellBookAppearBig.spriteWidth;

		const big: SpriteProxy = this.spellBookAppearBig.addShifted(centerX, centerY, 0, 21);
		big.horizontalScale = horizontalScaleBig;
		big.verticalScale = verticalScaleBig;
		const hueShiftDelta: number = Random.plusMinusBetween(20, 40);
		const mediumHueShift: number = hueShift - hueShiftDelta;
		const medium: SpriteProxy = this.spellBookAppearMedium.addShifted(centerX, centerY, 0, mediumHueShift, this.getSaturation(mediumHueShift));
		medium.timeStart = performance.now() + 9 * fps30;  // medium starts 9 frames in.
		medium.horizontalScale = horizontalScaleBig;
		medium.verticalScale = verticalScaleBig;
		const smallHueShift: number = hueShift + hueShiftDelta;
		const small: SpriteProxy = this.spellBookAppearSmall.addShifted(centerX, centerY, 0, smallHueShift, this.getSaturation(smallHueShift));
		small.timeStart = performance.now() + 16 * fps30;  // small starts 16 frames in.
		small.horizontalScale = horizontalScaleBig;
		small.verticalScale = verticalScaleBig;
		return true;
	}

	getSaturation(hueShift: number): number {
		let saturation = 75;  // Reduce saturation for significant hue shifts 
		if (hueShift < 15 || hueShift > 345)
			saturation = 100;
		return saturation;
	}

	private getIconScale(titleFontSize: number): number {
		return Math.min(1, titleFontSize / SpellBook.iconScaleDenominator);
	}

	getSchoolOfMagicHeight(schoolOfMagic: SchoolOfMagic): number {
		if (schoolOfMagic === SchoolOfMagic.Abjuration)
			return SpellBook.schoolOfMagicAbjurationHeight;
		return SpellBook.schoolOfMagicHeight;
	}

	private getNewTop(bottom: number, totalSpellPageHeight: number): number {
		return bottom - totalSpellPageHeight - SpellBook.spellBottomMargin;
	}

	private getSpellDescriptionBottom(top: number, totalSpellPageHeight: number) {
		return top + totalSpellPageHeight + SpellBook.spellBottomMargin;
	}
}