enum LayerLevel {
	Back,
	Front
}

enum SpeechType {
	None,
	Thinks,
	Says
}

class SpeechData {
	okayToDraw = false;
	x: number;
	y: number;
	width: number;
	height: number;
	textColor: string;
	playerId: number;
	fontSize: number;
	static readonly maxTextWidth: number = 124;
	static readonly maxTextFontSize: number = 90;

	paragraph: ParagraphWrapData;
	static styleDelimiters: LayoutDelimiters[] = [
		new LayoutDelimiters(LayoutStyle.bold, '*', '*')
		//,
		//new LayoutDelimiters(LayoutStyle.italic, '[', ']')
	];

	constructor(wordWrapper: WordWrapper, wordRenderer: WordRenderer, context: CanvasRenderingContext2D, text: string, idealWidth: number, idealAspectRatio: number, fontSize: number, fontName: string, topBottomReducePercent: number) {
		context.font = `${fontSize}px ${fontName}`;
		wordWrapper.fontSize = fontSize;
		wordRenderer.fontSize = fontSize;

		const maxAttempts = 8;
		let attemptCount = 0;
		let lastWidthAdjustPercent = 0.5;
		let width: number = idealWidth;
		let lastWidth: number = idealWidth;
		let lastDeltaPercentageAwayFromIdeal: number = Number.MAX_VALUE;
		do {
			attemptCount++;
			wordRenderer.fontSize = this.fontSize;
			this.paragraph = wordWrapper.getWordWrappedLinesForParagraphs(context, text, width, SpeechData.styleDelimiters, wordRenderer, topBottomReducePercent);
			const currentAspectRatio: number = this.paragraph.getAspectRatio();

			const thisDeltaPercentageAwayFromIdeal: number = Math.abs(currentAspectRatio / idealAspectRatio - 1);
			if (lastDeltaPercentageAwayFromIdeal === Number.MAX_VALUE || thisDeltaPercentageAwayFromIdeal < lastDeltaPercentageAwayFromIdeal) {
				lastWidth = width;
				lastDeltaPercentageAwayFromIdeal = thisDeltaPercentageAwayFromIdeal;
			}
			else if (thisDeltaPercentageAwayFromIdeal > lastDeltaPercentageAwayFromIdeal) {
				width = lastWidth;
				lastWidthAdjustPercent /= 2;
				continue;
			}

			if (currentAspectRatio > idealAspectRatio) {
				width -= width * lastWidthAdjustPercent;
			}
			else if (currentAspectRatio < idealAspectRatio) {
				width += width * lastWidthAdjustPercent;
			}
			else
				break;

			lastWidthAdjustPercent /= 2;
		} while (attemptCount < maxAttempts);

		const fontScale: number = idealWidth / this.paragraph.getLongestLineWidth();
		this.fontSize = Math.min(fontSize * fontScale, SpeechData.maxTextFontSize);
		context.font = `${this.fontSize}px ${fontName}`;
		wordWrapper.fontSize = this.fontSize;
		wordRenderer.fontSize = this.fontSize;
		this.paragraph = wordWrapper.getWordWrappedLinesForParagraphs(context, text, width * fontScale, SpeechData.styleDelimiters, wordRenderer, topBottomReducePercent);
	}
}

class SpeechBubbleManager {
	soundManager: SoundManager;
	wordRenderer: WordRenderer;
	wordWrapper: WordWrapper;
	speechBubbles: Sprites;
	thoughtBubbles: Sprites;

	static readonly textColor: string = '#1a0c0a';
	static readonly bulletColor: string = '#5b3c35';
	static readonly emphasisColor: string = '#a01a00';
	static readonly tableLineColor: string = '#5b3c35';
	static readonly fontName: string = 'ccbiffbamboom';
	static readonly fontSize: number = 20;
	static readonly bulletIndent = 8;
	static readonly emphasisFontHeightIncrease = 0;

	//`![](31D33771D101AC43215689BD6498E18E.png)
	static readonly emphasisFontStyleAscender = 13;  // This is the height from the baseline to the top of the tallest character.

	idealTextAspectRatio = 1;

	constructor(private iGetPlayerX: IGetPlayerX, private iGetCreatureX: IGetCreatureX = null) {
		this.soundManager = new SoundManager('GameDev/Assets/DragonH/SoundEffects/SpeechBubbles');
		this.wordWrapper = new WordWrapper();

		this.wordRenderer = new WordRenderer();
		this.wordRenderer.fontName = SpeechBubbleManager.fontName;
		this.wordRenderer.fontSize = SpeechBubbleManager.fontSize;
		this.wordRenderer.emphasisColor = SpeechBubbleManager.emphasisColor;
		this.wordRenderer.emphasisFontHeightIncrease = SpeechBubbleManager.emphasisFontHeightIncrease;
		this.wordRenderer.emphasisFontStyleAscender = SpeechBubbleManager.emphasisFontStyleAscender;
		this.wordRenderer.bulletIndent = SpeechBubbleManager.bulletIndent;
		this.wordRenderer.bulletColor = SpeechBubbleManager.bulletColor;
		this.wordRenderer.textColor = SpeechBubbleManager.textColor;
		this.wordRenderer.tableLineColor = SpeechBubbleManager.tableLineColor;
	}

	loadResources() {
		this.speechBubbles = new Sprites('SpeechBubbles/SpeechBubble', 38, fps30, AnimationStyle.Loop, true);
		this.speechBubbles.returnFrameIndex = 8;
		this.speechBubbles.segmentSize = 15;
		this.speechBubbles.originX = 14;
		this.speechBubbles.originY = 242;

		this.thoughtBubbles = new Sprites('SpeechBubbles/ThoughtBubble', 38, fps30, AnimationStyle.Loop, true);
		this.thoughtBubbles.returnFrameIndex = 8;
		this.thoughtBubbles.segmentSize = 15;
		this.thoughtBubbles.originX = 54;
		this.thoughtBubbles.originY = 242;
	}

	static readonly dungeonMasterId: number = 100;

	// Expected syntax: {playerId} {says|thinks}: {message to say or think}
	// 
	sayOrThinkSomething(context: CanvasRenderingContext2D, speechStr: string, layerLevel: LayerLevel) {
		const { playerId, speechType, textToShow, colorStr }: { playerId: number; speechType: SpeechType; textToShow: string; colorStr: string } = this.getSpeechCommandParts(speechStr);

		console.log('sayOrThinkSomething - colorStr: ' + colorStr);

		if (playerId === Number.MIN_VALUE || speechType === SpeechType.None)
			return;

		//const isBackGame: boolean = this.iGetCreatureX !== null;
		//if (isBackGame) {
		//	if (layerLevel === LayerLevel.Front)
		//		return;
		//}
		//else if (layerLevel === LayerLevel.Back)
		//	return;
		if (layerLevel === LayerLevel.Back)
			if (playerId >= 0)
				return;

		if (layerLevel === LayerLevel.Front)
			if (playerId < 0)
				return;

		if (!this.iGetCreatureX && playerId < 0) {
			console.error(`!this.iGetCreatureX && playerId (${playerId}) < 0`);
			return;
		}

		this.playBubbleEntranceSound(speechType);

		this.hideAnyBubblesBelongingToPlayer(playerId);

		const { playerX, playerY }: { playerX: number; playerY: number } = this.getPlayerXY(playerId);

		let { textWidth, topBottomReducePercent, textHeight, sprites, offsetX, textStartX, offsetY, textStartY }: { textWidth: number; topBottomReducePercent: number; textHeight: number; sprites: Sprites; offsetX: number; textStartX: number; offsetY: number; textStartY: number } = this.initializeBasedOnBubbleType(speechType);

		const speechData: SpeechData = new SpeechData(this.wordWrapper, this.wordRenderer, context, textToShow, textWidth, this.idealTextAspectRatio, SpeechBubbleManager.fontSize, SpeechBubbleManager.fontName, topBottomReducePercent);

		const { paragraphWidth, paragraphHeight }: { paragraphWidth: number; paragraphHeight: number } = this.getParagraphSize(speechData);


		const { horizontalScale, verticalScale } = this.getScale(paragraphWidth, textWidth, paragraphHeight, textHeight);

		//const scaledOffsetY: number = sprites.originY + offsetY * verticalScale;


		let scaledOffsetX: number | undefined;
		let flippedOffsetX;
		let flippedHorizontalTextOffset;
		let flippedHorizontally;
		({ scaledOffsetX, flippedOffsetX, flippedHorizontalTextOffset, flippedHorizontally, textStartX } = this.checkForHorizontalFlip(sprites, offsetX, horizontalScale, playerX, textStartX, textWidth));

		let xPos = playerX + scaledOffsetX + flippedOffsetX;

		const verticalTextOffsetBecauseNoDescenders: number = this.wordRenderer.fontSize * 0.12;   // Shift down a bit since there are no descenders in the Comic font.
		let yPos: number = playerY - offsetY * verticalScale;

		const { scaledPosOffsetX, scaledPosOffsetY } = this.getScaledPosOffset(horizontalScale, sprites, verticalScale);

		speechData.x = xPos + flippedHorizontalTextOffset - scaledPosOffsetX + textStartX * horizontalScale;
		speechData.y = this.getSpeechDataY(yPos, verticalTextOffsetBecauseNoDescenders, scaledPosOffsetY, textStartY, verticalScale, speechData);

		let flipVertically;
		({ flipVertically, xPos, yPos, textStartY } = this.checkForVerticalFlip(speechData, playerId, speechType, verticalScale, xPos, yPos, textStartY, textHeight, verticalTextOffsetBecauseNoDescenders, scaledPosOffsetY));

		speechData.width = textWidth * horizontalScale;
		speechData.height = textHeight * verticalScale;
		speechData.textColor = colorStr;
		speechData.playerId = playerId;
		this.createNewSprite(sprites, xPos, scaledPosOffsetX, yPos, scaledPosOffsetY, speechStr, flippedHorizontally, flipVertically, speechData, horizontalScale, verticalScale);
	}

	private createNewSprite(sprites: Sprites, xPos: any, scaledPosOffsetX: number, yPos: number, scaledPosOffsetY: number, speechStr: string, flippedHorizontally: any, flipVertically: any, speechData: SpeechData, horizontalScale: number, verticalScale: number) {
		//console.log(`yPos: ${yPos}, scaledPosOffsetY: ${scaledPosOffsetY}, speechData.y: ${speechData.y}`);
		//console.log(speechData);
		const sprite: SpriteProxy = sprites.add(xPos - scaledPosOffsetX, yPos - scaledPosOffsetY);
		sprite.playToEndOnExpire = true;
		sprite.fadeInTime = 400;
		sprite.fadeOutTime = 467;

		const totalReadingTime: number = this.getTotalReadingTime(speechStr);

		sprite.expirationDate = performance.now() + sprite.fadeInTime + totalReadingTime + sprite.fadeOutTime;
		sprite.flipHorizontally = flippedHorizontally;
		sprite.flipVertically = flipVertically;
		sprite.addOnFrameAdvanceCallback(this.spriteAdvanceFrame.bind(this));
		sprite.data = speechData;
		sprite.horizontalScale = horizontalScale;
		sprite.verticalScale = verticalScale;
	}

	private getTotalReadingTime(speechStr: string) {
		const numWords: number = (speechStr.match(/\s/g) || []).length + 1;
		const averageReadingTimePerWordMs = 300;
		const safetyFactor = 2;
		const totalReadingTime: number = Math.max(1500, numWords * averageReadingTimePerWordMs * safetyFactor);
		return totalReadingTime;
	}

	private checkForVerticalFlip(speechData: SpeechData, playerId: number, speechType: SpeechType, verticalScale: number, xPos: any, yPos: number, textStartY: number, height: number, verticalTextOffsetBecauseNoDescenders: number, scaledPosOffsetY: number) {
		let flipVertically = false;

		if (speechData.y < 0) // Make sure top of text is on screen.
		{
			// TODO: Handle playerId < 0 for in-game creatures.
			if (playerId === SpeechBubbleManager.dungeonMasterId) {
				flipVertically = true;
				if (speechType === SpeechType.Thinks) {
					const invertedSpeechBubbleOffsetX = 70;
					const invertedSpeechBubbleOffsetY = 125;
					speechData.x += invertedSpeechBubbleOffsetX;
					speechData.y += invertedSpeechBubbleOffsetY * verticalScale;
					xPos += invertedSpeechBubbleOffsetX;
					yPos += invertedSpeechBubbleOffsetY;
				}
				textStartY = -textStartY + height;
				speechData.y = this.getSpeechDataY(yPos, verticalTextOffsetBecauseNoDescenders, scaledPosOffsetY, textStartY, verticalScale, speechData);
			}
			else if (playerId < 0) {
				flipVertically = true;
				if (speechType === SpeechType.Thinks) {
					const invertedSpeechBubbleOffsetX = -60;
					const invertedSpeechBubbleOffsetY = 125;
					speechData.x += invertedSpeechBubbleOffsetX;
					speechData.y += invertedSpeechBubbleOffsetY * verticalScale;
					xPos += invertedSpeechBubbleOffsetX;
					yPos += invertedSpeechBubbleOffsetY;
				}
				textStartY = -textStartY + height;
				speechData.y = this.getSpeechDataY(yPos, verticalTextOffsetBecauseNoDescenders, scaledPosOffsetY, textStartY, verticalScale, speechData);
			}
			else {
				const delta: number = 0 - speechData.y;
				speechData.y = 0;
				yPos += delta;
			}
		}
		return { flipVertically, xPos, yPos, textStartY };
	}

	private checkForHorizontalFlip(sprites: Sprites, offsetX: number, horizontalScale: number, playerX: number, textStartX: number, width: number) {
		let scaledOffsetX: number = sprites.originX + offsetX * horizontalScale;
		let flippedHorizontalTextOffset = 0;
		let flippedOffsetX = 0;
		const maxRightPos = 1400;
		let flippedHorizontally = false;
		if (playerX + scaledOffsetX > maxRightPos) {
			scaledOffsetX = -scaledOffsetX;
			textStartX = -textStartX - width;
			flippedHorizontalTextOffset = -this.wordRenderer.fontSize / 6;
			flippedOffsetX = 60;
			flippedHorizontally = true;
		}
		return { scaledOffsetX, flippedOffsetX, flippedHorizontalTextOffset, flippedHorizontally, textStartX };
	}

	private initializeBasedOnBubbleType(speechType: SpeechType) {
		let sprites: Sprites;

		const speechBubbleOffsetX = 90;
		const speechBubbleOffsetY = 192;
		const speechBubbleTextOffsetX = 18;
		const speechBubbleTextOffsetY = 195;

		const speechBubbleTextWidth = 199;
		const speechBubbleTextHeight = 115;
		const speechBubbleAspectRatio = speechBubbleTextWidth / speechBubbleTextHeight;


		const thoughtBubbleOffsetX = 114;
		const thoughtBubbleOffsetY = 296;
		const thoughtBubbleTextOffsetX = -18;
		const thoughtBubbleTextOffsetY = 222;

		const thoughtBubbleTextWidth = 220;
		const thoughtBubbleTextHeight = 134;
		const thoughtBubbleAspectRatio = thoughtBubbleTextWidth / thoughtBubbleTextHeight;


		let offsetX: number;
		let offsetY: number;
		let textStartX: number;
		let textStartY: number;

		let textWidth: number;
		let textHeight: number;

		let topBottomReducePercent: number;

		if (speechType === SpeechType.Thinks) {
			sprites = this.thoughtBubbles;
			offsetX = thoughtBubbleOffsetX;
			offsetY = thoughtBubbleOffsetY;
			textStartX = thoughtBubbleTextOffsetX;
			textStartY = thoughtBubbleTextOffsetY;
			textWidth = thoughtBubbleTextWidth;
			textHeight = thoughtBubbleTextHeight;
			topBottomReducePercent = 20;
			this.idealTextAspectRatio = thoughtBubbleAspectRatio;
		}
		else {
			sprites = this.speechBubbles;
			offsetX = speechBubbleOffsetX;
			offsetY = speechBubbleOffsetY;
			textStartX = speechBubbleTextOffsetX;
			textStartY = speechBubbleTextOffsetY;
			textWidth = speechBubbleTextWidth;
			textHeight = speechBubbleTextHeight;
			topBottomReducePercent = 0;
			this.idealTextAspectRatio = speechBubbleAspectRatio;
		}
		return { textWidth, topBottomReducePercent, textHeight, sprites, offsetX, textStartX, offsetY, textStartY };
	}

	private getScaledPosOffset(horizontalScale: number, sprites: Sprites, verticalScale: number) {
		let scaledPosOffsetX = 0;
		let scaledPosOffsetY = 0;

		if (horizontalScale !== 1) {
			scaledPosOffsetX = sprites.originX * (1 - horizontalScale);
		}

		if (verticalScale !== 1) {
			scaledPosOffsetY = sprites.originY * (1 - verticalScale);
		}
		return { scaledPosOffsetX, scaledPosOffsetY };
	}

	private getParagraphSize(speechData: SpeechData) {
		const horizontalMarginBuffer = 1.15; // 115%
		const verticalMarginBuffer = 1.20; // 120%

		const paragraphWidth: number = speechData.paragraph.getLongestLineWidth() * horizontalMarginBuffer;
		const paragraphHeight: number = speechData.paragraph.getParagraphHeight() * verticalMarginBuffer;
		return { paragraphWidth, paragraphHeight };
	}

	private getScale(paragraphWidth: number, textWidth: number, paragraphHeight: number, textHeight: number) {
		let horizontalScale = 1;
		let verticalScale = 1;

		horizontalScale = paragraphWidth / textWidth;
		verticalScale = paragraphHeight / textHeight;

		const thisAspectRatio: number = horizontalScale / verticalScale;
		const maxAspectRatioFactor = 2;
		const maxAspectRatio: number = maxAspectRatioFactor / 1;
		const minAspectRatio: number = 1 / maxAspectRatioFactor;

		if (thisAspectRatio < minAspectRatio) {
			const horizontalScaleFactor = 1.2;
			horizontalScale = verticalScale * minAspectRatio * horizontalScaleFactor;
		}
		else if (thisAspectRatio > maxAspectRatio) {
			verticalScale = horizontalScale / maxAspectRatio;
		}
		return { horizontalScale, verticalScale };
	}

	private playBubbleEntranceSound(speechType: SpeechType) {
		if (speechType === SpeechType.Thinks) {
			this.soundManager.safePlayMp3('ThoughtBubbleAppear');
		}
		else {
			this.soundManager.safePlayMp3('SpeechBubbleAppear');
		}
	}

	private getPlayerXY(playerId: number) {
		let playerX: number;
		const screenBottom = 1080;
		const dungeonMasterX = 1700;
		const dungeonMasterY = 350;
		const inGameY = 270;

		let playerY: number;
		if (playerId === SpeechBubbleManager.dungeonMasterId) {
			playerX = dungeonMasterX;
			playerY = dungeonMasterY;
		}
		else if (playerId >= 0) { // Player... 
			playerX = this.iGetPlayerX.getPlayerX(this.iGetPlayerX.getPlayerIndex(playerId));
			playerY = screenBottom;
		}
		else {
			const inGameCreature: InGameCreature = this.iGetCreatureX.getInGameCreatureByIndex(-playerId);
			if (inGameCreature !== null)
				playerX = this.iGetCreatureX.getX(inGameCreature);

			//playerY = dungeonMasterY;
			playerY = inGameY;
		}
		return { playerX, playerY };
	}

	private getSpeechCommandParts(speechStr: string) {
		let spaceIndex = 0;
		let textToShow = null;
		let playerId: number = Number.MIN_VALUE;
		let speechType: SpeechType = SpeechType.None;
		let colorStr = '#000000';

		//console.log('speechStr: ' + speechStr);
		const openParenPos: number = speechStr.indexOf('(');
		if (openParenPos > 0) {
			const closeParenPos: number = speechStr.indexOf(')');
			if (closeParenPos > openParenPos) {
				colorStr = speechStr.substring(openParenPos + 1, closeParenPos);
				if (!colorStr)
					colorStr = '#000000'
				//console.log('colorStr: ' + colorStr);
			}
			speechStr = speechStr.substring(0, openParenPos) + speechStr.substring(closeParenPos + 1);
			//console.log('speechStr: ' + speechStr);
		}

		const firstColonPos: number = speechStr.indexOf(':');
		if (firstColonPos > 0) {
			const firstPart: string = speechStr.substr(0, firstColonPos);
			textToShow = speechStr.substr(firstColonPos + 1).trim();
			speechType = this.getSpeechType(firstPart);
			spaceIndex = firstPart.indexOf(' ');
			if (spaceIndex >= 0)
				playerId = +firstPart.substr(0, spaceIndex).trim();
		}
		return { playerId, speechType, textToShow, colorStr };
	}

	private getSpeechDataY(yPos: number, verticalTextOffsetBecauseNoDescenders: number, scaledPosOffsetY: number, textStartY: number, verticalScale: number, speechData: SpeechData): number {
		return yPos + verticalTextOffsetBecauseNoDescenders - scaledPosOffsetY - textStartY * verticalScale - speechData.paragraph.lineData.length * speechData.fontSize / 2 + speechData.fontSize / 2;
	}

	hideAnyBubblesBelongingToPlayer(playerId: number) {
		this.hideAnyBubbleSpritesBelongingToPlayer(this.speechBubbles, playerId);
		this.hideAnyBubbleSpritesBelongingToPlayer(this.thoughtBubbles, playerId);
	}

	hideAnyBubbleSpritesBelongingToPlayer(bubbleSprites: Sprites, playerId: number) {
		bubbleSprites.spriteProxies.forEach((sprite: SpriteProxy) => {
			const speechData: SpeechData = sprite.data as SpeechData;
			if (speechData && speechData.playerId === playerId)
				sprite.fadeOutNow(400);
		});
	}

	spriteAdvanceFrame(sprite: SpriteProxy, returnFrameIndex: number, reverse: boolean, now: number): void {
		const endLoopAnimationFrameIndex = 24;
		const speechData: SpeechData = sprite.data as SpeechData;
		if (speechData) {
			speechData.okayToDraw = sprite.frameIndex >= returnFrameIndex && sprite.frameIndex < endLoopAnimationFrameIndex;
		}
		if (sprite.frameIndex === endLoopAnimationFrameIndex + 1)
			this.soundManager.safePlayMp3('Pop[4]');
	}

	getSpeechType(firstPart: string): SpeechType {
		if (firstPart.toLowerCase().indexOf('says') >= 0)
			return SpeechType.Says;
		else
			return SpeechType.Thinks;
	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
		this.thoughtBubbles.draw(context, nowMs);
		this.speechBubbles.draw(context, nowMs);
		this.drawText(context, this.thoughtBubbles);
		this.drawText(context, this.speechBubbles);
	}

	drawSpriteText(context: CanvasRenderingContext2D, sprite: SpriteProxy) {
		context.textAlign = 'center';
		context.textBaseline = "middle";
		const speechData: SpeechData = sprite.data as SpeechData;
		if (speechData && speechData.okayToDraw) {
			this.wordRenderer.fontSize = speechData.fontSize;
			this.wordRenderer.setActiveStyle(context, LayoutStyle.normal);

			this.wordRenderer.textColor = speechData.textColor;
			this.wordRenderer.renderParagraphs(context, speechData.paragraph.lineData, speechData.x + speechData.width / 2, speechData.y + speechData.height / 2, SpeechData.styleDelimiters);
		}
	}

	drawText(context: CanvasRenderingContext2D, sprites: Sprites) {
		sprites.spriteProxies.forEach((sprite: SpriteProxy) => {
			this.drawSpriteText(context, sprite);
		});
	}
}