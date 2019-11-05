class SpellBook {
	static readonly titleFontName: string = 'Modesto Condensed Bold';
	static readonly detailFontName: string = 'mrs-eaves';
	static readonly detailBoldFontName: string = '700 mrs-eaves Bold';
	static readonly titleFontIdealSize: number = 30;
	static readonly titleLeftMargin: number = 25;
	static readonly detailFontSize: number = 18;
	static readonly spellDetailsWidth: number = 220;
	static readonly schoolOfMagicWidth: number = 144;
	static readonly spellDescriptionWidth: number = 330;
	static readonly titleLevelMargin: number = 3;
	static readonly detailSpacing: number = 4;
	static readonly levelDetailsMargin: number = 6;
	static readonly spellPageRightEdge: number = 332;
	static readonly bottomWindowHeight: number = 1080;
	static readonly spellBottomHeight: number = 1022;

	// ![](C1C6705D2CAADF7C85591DA2A1E0A6DF.png)
	static readonly spellBottomMargin: number = 23;

	// ![](C1726232AA0D358FB5CC3FE2FE14D260.png)  schoolOfMagicHeight
	static readonly schoolOfMagicHeight: number = 105;

	// ![](BCA0565D701D0D116C5132DE9B222FD6.png)
	static readonly spellHeaderHeight: number = 79;
	static readonly textColor: string = '#2d1611';

	spellBookBack: Sprites;
	spellBookTop: Sprites;
	schoolOfMagic: Sprites;
	lastPlayerId: number;
	lastSpellName: string;
	titleTopLeft: Vector;
	levelSchoolTopLeft: Vector;
	titleColors: Array<string>;
	hueShifts: Array<number>;
	titleFontSize: number = SpellBook.titleFontIdealSize;
	spellDetailsTopLeft: Vector;
	spellDescriptionTopLeft: Vector;
  spellBookBackHeight: number = 0;

	constructor() {
		this.loadFonts();
		this.loadColors();
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

		this.hueShifts = [];
		this.hueShifts.push(0); // None
		this.hueShifts.push(191); // Abjuration     
		this.hueShifts.push(238); // Illusion			 
		this.hueShifts.push(277); // Conjuration	   
		this.hueShifts.push(331); // Enchantment    
		this.hueShifts.push(7);		// Necromancy     
		this.hueShifts.push(137); // Evocation			 
		this.hueShifts.push(101); // Transmutation	 
		this.hueShifts.push(52);  // Divination		 
	}

	private getWordWrappedLines(context: CanvasRenderingContext2D, text: string, maxWidth: number): any {
		var words = text.split(" ");
		var lines = [];
		var currentLine = words[0];

		for (var i = 1; i < words.length; i++) {
			var word = words[i];
			var width = context.measureText(currentLine + " " + word).width;
			if (width < maxWidth) {
				currentLine += " " + word;
			} else {
				lines.push(currentLine);
				currentLine = word;
			}
		}
		lines.push(currentLine);
		return lines;
	}

	private getWordWrappedLinesForParagraphs(context: CanvasRenderingContext2D, text: string, maxWidth: number): any {
		return text.split("\n").map(para => this.getWordWrappedLines(context, para, maxWidth)).reduce((a, b) => a.concat(b), []);
	}


	private loadFonts() {
		// @ts-ignore - FontFace
		var junction_font = new FontFace('Modesto Condensed Bold', 'url(GameDev/Assets/DragonH/Fonts/617b1b0fd4637fda235c73f27d530305.woff)');
		junction_font.load().then(function (loaded_face) {
			// @ts-ignore - document.fonts
			document.fonts.add(loaded_face);
			document.body.style.fontFamily = '"Modesto Condensed Bold", Arial';
		}).catch(function (error) {
			console.log('Font loading error: ' + error);
		});
	}

	drawSpellTitle(now: number, context: CanvasRenderingContext2D, spell: ActiveSpellData): any {
		this.setTitleFont(context, this.titleFontSize);
		context.fillStyle = this.titleColors[spell.schoolOfMagic];
		context.fillText(spell.name, this.titleTopLeft.x, this.titleTopLeft.y, SpellBook.spellPageRightEdge);
	}

	drawSpellLevelSchool(now: number, context: CanvasRenderingContext2D, spell: ActiveSpellData): any {
		this.setDetailFontNormal(context);
		context.fillStyle = SpellBook.textColor;
		let levelStr: string;
		if (spell.spellLevel == 0)
			levelStr = 'Cantrip';
		else
			levelStr = `Level ${spell.spellLevel}`;

		context.fillText(`${levelStr} ${this.toSchoolDisplayName(spell.schoolOfMagic)}`, this.levelSchoolTopLeft.x, this.levelSchoolTopLeft.y, SpellBook.spellPageRightEdge);
	}

	drawSpellDescription(now: number, context: CanvasRenderingContext2D, spell: ActiveSpellData): void {
		context.fillStyle = SpellBook.textColor;
		let x: number = this.spellDescriptionTopLeft.x;
		let y: number = this.spellDescriptionTopLeft.y;

		let lines = this.getWordWrappedLinesForParagraphs(context, spell.description, SpellBook.spellDescriptionWidth);
		for (let i = 0; i < lines.length; i++) {
			context.fillText(lines[i], x, y);
			y += SpellBook.detailFontSize;
		}
	}

	getDetailHeight(context: CanvasRenderingContext2D, spell: ActiveSpellData): number {
		this.setDetailFontNormal(context);

		let lineCount: number = 0;
		let spacerCount: number = 0;
		let height: number = 0;

		let castingLines = spell.castingTimeStr.split(',');

		if (castingLines.length > 0) {
			height += SpellBook.detailFontSize;
			lineCount++;
			if (castingLines.length > 1) {
				let castingLinesExtraCount: number = this.getDetailLineCount(context, castingLines[1].trim());
				console.log('castingLinesExtraCount: ' + castingLinesExtraCount);
				height += SpellBook.detailFontSize * castingLinesExtraCount;
				lineCount += castingLinesExtraCount;
			}
			height += SpellBook.detailSpacing;
			spacerCount++;
		}

		height += SpellBook.detailFontSize + SpellBook.detailSpacing;  // Range
		lineCount++;
		spacerCount++;

		if (spell.componentsStr) {
			height += SpellBook.detailFontSize;
			lineCount++;

			let parenIndex = spell.componentsStr.indexOf('(');
			let componentMaterials: string = '';
			if (parenIndex > 0) {
				componentMaterials = spell.componentsStr.substr(parenIndex);
			}

			if (componentMaterials) {
				let componentsExtraLines: number = this.getDetailLineCount(context, componentMaterials);
				console.log('componentsExtraLines: ' + componentsExtraLines);
				height += SpellBook.detailFontSize * componentsExtraLines;
				lineCount += componentsExtraLines;
			}
			height += SpellBook.detailSpacing;
			spacerCount++;
		}

		height += SpellBook.detailFontSize + SpellBook.detailSpacing;   // Duration
		lineCount++;
		console.log('Detail: lineCount: ' + lineCount);
		console.log('Detail: spacerCount: ' + spacerCount);
		return height;
	}

	drawSpellDetails(now: number, context: CanvasRenderingContext2D, spell: ActiveSpellData): void {
		context.fillStyle = SpellBook.textColor;
		let x: number = this.spellDetailsTopLeft.x;
		let y: number = this.spellDetailsTopLeft.y;

		let castingLines = spell.castingTimeStr.split(',');

		if (castingLines.length > 0) {
			this.showDetail(context, 'Casting Time: ', castingLines[0], x, y);
			y += SpellBook.detailFontSize;
			let castingTimeX: number = x + 4;
			if (castingLines.length > 1) {
				y = this.wrapDetailLines(context, castingLines[1].trim(), castingTimeX, y);
			}
			y += SpellBook.detailSpacing;
		}

		this.showDetail(context, 'Range: ', spell.rangeStr, x, y);
		y += SpellBook.detailFontSize + SpellBook.detailSpacing;

		if (spell.componentsStr) {
			let parenIndex = spell.componentsStr.indexOf('(');
			let componentSummary: string;
			let componentMaterials: string = '';
			if (parenIndex > 0) {
				componentSummary = spell.componentsStr.substr(0, parenIndex);
				componentMaterials = spell.componentsStr.substr(parenIndex);
				// BUG: Looks like we might be leaving on the trailing paren.
			}
			else
				componentSummary = spell.componentsStr;
			this.showDetail(context, 'Components: ', componentSummary, x, y);
			y += SpellBook.detailFontSize;

			if (componentMaterials) {
				const materialX: number = x + 7;
				this.setDetailFontNormal(context);
				y = this.wrapDetailLines(context, componentMaterials, materialX, y);
			}
			y += SpellBook.detailSpacing;
		}

		let durationStr: string = spell.durationStr;
		const concentrationPrefix: string = 'Concentration, ';
		if (durationStr.startsWith(concentrationPrefix))
			durationStr = spell.durationStr.substr(concentrationPrefix.length);
		this.showDetail(context, 'Duration: ', durationStr, x, y);
	}

	private wrapDetailLines(context: CanvasRenderingContext2D, detailStr: string, x: number, y: number) {
		let lines = this.getWordWrappedLines(context, detailStr, SpellBook.spellDetailsWidth);
		for (let i = 0; i < lines.length; i++) {
			context.fillText(lines[i], x, y);
			y += SpellBook.detailFontSize;
		}
		return y;
	}

	private getDetailLineCount(context: CanvasRenderingContext2D, detailStr: string) {
		return this.getWordWrappedLines(context, detailStr, SpellBook.spellDetailsWidth).length;
	}

	showDetail(context: CanvasRenderingContext2D, label: string, data: string, x: number, y: number): void {
		this.setDetailFontNormal(context);
		context.fillText(label, x, y, SpellBook.spellPageRightEdge);
		x += context.measureText(label).width;
		this.setDetailFontBold(context);
		context.fillText(data, x, y, SpellBook.spellPageRightEdge);
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
		}
		return '';
	}

	private setTitleFont(context: CanvasRenderingContext2D, fontSize: number) {
		context.font = `${fontSize}px ${SpellBook.titleFontName}`;
	}

	private setDetailFontNormal(context: CanvasRenderingContext2D) {
		context.font = `${SpellBook.detailFontSize}px ${SpellBook.detailFontName}`;
	}

	private setDetailFontBold(context: CanvasRenderingContext2D) {
		context.font = `${SpellBook.detailFontSize}px ${SpellBook.detailBoldFontName}`;
	}

	draw(now: number,
		context: CanvasRenderingContext2D,
		x: number,
		y: number,
		player: Character): void {

		let spell: ActiveSpellData = player.spellActivelyCasting;

		if (this.lastSpellName != spell.name) {
			this.lastSpellName = spell.name;
			this.lastPlayerId = player.playerID;

			this.createSpellPage(context, x, y, spell);
		}

		if (this.spellBookBack.sprites.length > 0) {
			let firstBackSprite = this.spellBookBack.sprites[0];
			// ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)
			let w: number = this.spellBookBack.spriteWidth;
			let h: number = this.spellBookBack.spriteHeight;
			this.spellBookBack.drawCropped(context, now * 1000, firstBackSprite.x, firstBackSprite.y + h - this.spellBookBackHeight, 0, h - this.spellBookBackHeight, w, this.spellBookBackHeight, w, this.spellBookBackHeight);
		}

		this.spellBookTop.draw(context, now * 1000);
		this.schoolOfMagic.draw(context, now * 1000);

		this.drawSpellTitle(now, context, spell);
		this.drawSpellLevelSchool(now, context, spell);
		this.drawSpellDetails(now, context, spell);
		this.drawSpellDescription(now, context, spell);
	}

	createSpellPage(context: CanvasRenderingContext2D, x: number, y: number, spell: ActiveSpellData): any {
		this.spellBookBack.sprites = [];
		this.spellBookTop.sprites = [];
		this.schoolOfMagic.sprites = [];

		let left: number = x - 10;
		let top: number = y - 33;


		//spellPageRightEdge
		this.titleFontSize = this.getTitleFontSize(context, left, spell.name);


		let desiredPageHeight: number = 269;  // Min of 269 (y-offset of 24). Max of 431.
		const bookBottomBottomMargin: number = 269 - 24;
		let bookBottomYOffset: number = desiredPageHeight - bookBottomBottomMargin;

		this.setDetailFontNormal(context);
		let descriptionLines = this.getWordWrappedLinesForParagraphs(context, spell.description, SpellBook.spellDescriptionWidth);

		let descriptionLinesWeNeedToAdd: number = descriptionLines.length;
		console.log('descriptionLinesWeNeedToAdd: ' + descriptionLinesWeNeedToAdd);
		let descriptionHeightWeNeedToAdd: number = descriptionLinesWeNeedToAdd * (SpellBook.detailFontSize);

		// ![](7735854D5F01FCAC67394960A782BCAE.png) spellPageHeaderHeight
		let spellPageHeaderHeight: number = 193;
		let detailsHeight: number = Math.max(this.getDetailHeight(context, spell) + SpellBook.detailFontSize, SpellBook.schoolOfMagicHeight);
		console.log('detailsHeight: ' + detailsHeight);

		console.log('descriptionHeightWeNeedToAdd: ' + descriptionHeightWeNeedToAdd);

		let totalSpellPageHeight: number = SpellBook.spellHeaderHeight + detailsHeight + descriptionHeightWeNeedToAdd;
		console.log('totalSpellPageHeight: ' + totalSpellPageHeight);

		if (top + totalSpellPageHeight + SpellBook.spellBottomMargin > SpellBook.bottomWindowHeight) {
			top = SpellBook.bottomWindowHeight - totalSpellPageHeight - SpellBook.spellBottomMargin;
		}

		this.spellDescriptionTopLeft = new Vector(left + SpellBook.titleLeftMargin, top + SpellBook.spellHeaderHeight + detailsHeight);
		this.titleTopLeft = new Vector(left + SpellBook.titleLeftMargin, top + 27);
		this.levelSchoolTopLeft = new Vector(left + SpellBook.titleLeftMargin, this.titleTopLeft.y + this.titleFontSize + SpellBook.titleLevelMargin);
		let schoolOfMagicTopLeft: Vector = new Vector(left + 25, this.levelSchoolTopLeft.y + SpellBook.detailFontSize + SpellBook.levelDetailsMargin);
		this.spellDetailsTopLeft = new Vector(left + SpellBook.schoolOfMagicWidth, schoolOfMagicTopLeft.y);
		//let spellBookBottomTop: number = top + bookBottomYOffset + lineSpaceWeNeedToAdd + detailExpansionHeight;
		let lowerSpellBookTop: number = top + totalSpellPageHeight;

		this.spellBookBackHeight = totalSpellPageHeight;
		this.spellBookBack.add(left, lowerSpellBookTop, 0);


		// ![](01228CBD17DFA0F5D4DB8929C674BE40.png)  spellBookHeaderGoodHeight
		//const spellBookHeaderGoodHeight: number = 211;
		//if (lowerSpellBookTop > top + spellBookHeaderGoodHeight) {
		//	// We need a second top behind the one we are about to add to extend the book even more.
		//	this.spellBookTop.add(left, lowerSpellBookTop - spellBookHeaderGoodHeight, 0).timeStart = 0;
		//}

		this.spellBookTop.add(left, top, 0).timeStart = 0;

		if (spell.schoolOfMagic > SchoolOfMagic.None)
			this.schoolOfMagic.add(schoolOfMagicTopLeft.x, schoolOfMagicTopLeft.y, spell.schoolOfMagic - 1).timeStart = 0;
	}

	getTitleFontSize(context: CanvasRenderingContext2D, left: number, name: string): number {
		let fontSize: number = SpellBook.titleFontIdealSize;
		this.setTitleFont(context, fontSize);
		while (SpellBook.titleLeftMargin + context.measureText(name).width > SpellBook.spellPageRightEdge) {
			fontSize--;
			if (fontSize <= 6) {
				return fontSize;
			}
			this.setTitleFont(context, fontSize);
		}
		return fontSize;
	}

	loadResources(): any {
		this.spellBookBack = new Sprites("Scroll/Spells/BookBottom", 1, 0, AnimationStyle.Static);
		this.spellBookBack.originX = 0;
		this.spellBookBack.originY = 999;
		this.spellBookTop = new Sprites("Scroll/Spells/BookTop", 1, 0, AnimationStyle.Static);
		this.schoolOfMagic = new Sprites("Scroll/Spells/SchoolsOfMagic", 8, 0, AnimationStyle.Static);
	}
}