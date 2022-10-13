class Answer {
	centerY: number;
	constructor(public Index: number, public Value: number, public IsSelected: boolean, public AnswerText: string) {

	}
}

class QuestionAnswerMap {
	constructor(public Question: string, public Answers: Array<Answer>, public MinAnswers = 1, public MaxAnswers = 1) {

	}
}

class MessageBox {
	wordRenderer: WordRenderer;
	wordWrapper: WordWrapper;
	soundManager: SoundManager;
	scrollBack: Sprites;
	scrollFront: Sprites;
	selectionIndicatorSprites: Sprites;
	focusIndicatorSprites: Sprites;
	animations: Animations;
	focusIndex = 0;
	static styleDelimiters: LayoutDelimiters[];
	titleParagraph: ParagraphWrapData;
	lastAnswer: TextEffect;
	discoverabilityTop = 0;

	constructor(soundManager: SoundManager) {
		this.soundManager = soundManager;
		this.initializeWordWrapper();
	}

	initializeWordWrapper() {
		MessageBox.styleDelimiters = [
			new LayoutDelimiters(LayoutStyle.bold, '**', '**')
		];

		this.wordWrapper = new WordWrapper();

		this.wordRenderer = new WordRenderer();
		this.wordRenderer.fontName = SpellBook.detailFontName;
		this.wordRenderer.fontWeightBold = SpellBook.detailFontWeight;
		this.wordRenderer.fontSize = SpellBook.detailFontSize;
		this.wordRenderer.emphasisColor = SpellBook.emphasisColor;
		this.wordRenderer.emphasisFontHeightIncrease = SpellBook.emphasisFontHeightIncrease;
		this.wordRenderer.emphasisFontStyleAscender = SpellBook.emphasisFontStyleAscender;
		this.wordRenderer.bulletIndent = SpellBook.bulletIndent;
		this.wordRenderer.bulletColor = SpellBook.bulletColor;
		this.wordRenderer.textColor = SpellBook.textColor;
		this.wordRenderer.tableLineColor = SpellBook.tableLineColor;
	}

	loadResources() {
		this.animations = new Animations();
		this.scrollBack = this.loadScroll('Back', 333);
		this.scrollFront = this.loadScroll('Front', 309, -2);
		this.focusIndicatorSprites = new Sprites(`InGameUI/FocusIndicator/Focus`, 173, fps30, AnimationStyle.Loop, true);
		this.focusIndicatorSprites.originX = 107;
		this.focusIndicatorSprites.originY = 107;

		this.selectionIndicatorSprites = new Sprites(`InGameUI/SelectionIndicator2/SelectionIndicator`, 105, fps30, AnimationStyle.Loop, true);
		this.selectionIndicatorSprites.originX = 118;
		this.selectionIndicatorSprites.originY = 118;

		this.setForMove(this.scrollBack);
		this.setForMove(this.scrollFront);
		this.setForMove(this.focusIndicatorSprites);
		this.setForMove(this.selectionIndicatorSprites);
	}

	private setForMove(sprites: Sprites) {
		sprites.moves = true;
		sprites.disableGravity();
	}

	private loadScroll(imageName: string, frameCount: number, segmentOffsetX = 0): Sprites {
		const sprites: Sprites = new Sprites(`InGameUI/Scroll/Scroll_${imageName}`, frameCount, fps30, AnimationStyle.Loop, true);
		sprites.returnFrameIndex = 43;
		sprites.segmentSize = 238;
		sprites.originX = 497 + segmentOffsetX;
		sprites.originY = 988;
		return sprites;
	}

	update(nowMs: number) {
		this.animations.updatePositions(nowMs);
		this.animations.removeExpiredAnimations(nowMs);
		this.scrollBack.updatePositions(nowMs);
		this.scrollFront.updatePositions(nowMs);
		this.selectionIndicatorSprites.updatePositions(nowMs);
		this.focusIndicatorSprites.updatePositions(nowMs);
	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
		this.scrollBack.draw(context, nowMs);
		this.animations.render(context, nowMs);
		this.selectionIndicatorSprites.draw(context, nowMs);
		this.focusIndicatorSprites.draw(context, nowMs);
		this.renderTitle(context, nowMs);
		this.renderMultiSelectDiscoverability(context, nowMs);
		this.scrollFront.draw(context, nowMs);
	}

	selectorEaseTime = 180;
	questionAnswerMap: QuestionAnswerMap;
	static readonly textBeginFrameIndex: number = 28;
	static readonly textEndFrameIndex: number = 299;
	static readonly textFadeInTime: number = 20 * fps30;
	static readonly textFadeOutTime: number = 6 * fps30;
	static readonly frameScrollIsCompletelyOnScreen: number = 19;


	playSlidingSound(soundManager: SoundManager) {
		soundManager.safePlayMp3('InGameUI/Slidey[5]');
		soundManager.safePlayMp3('InGameUI/Stamp[7]');
	}

	executeCommand(commandData: string, context: CanvasRenderingContext2D, soundManager: SoundManager) {
		console.log('commandData: ' + commandData);
		// TODO: Prevent multiple scroll UIs from appearing.
		if (commandData === 'Up') {
			this.moveUp(soundManager);
		}
		else if (commandData === 'Down') {
			this.moveDown(soundManager);
		}
		else if (commandData === 'Ask') {
			// test.
		}
		else if (commandData === 'Toggle') {
			this.toggleAnswer(soundManager);
		}
		else if (commandData === 'OK') {
			this.acceptAnswer();
		}
		else {
			this.questionAnswerMap = JSON.parse(commandData);
			this.show(context, soundManager);
		}
	}

	selectFocusedAnswer() {
		this.questionAnswerMap.Answers[this.focusIndex].IsSelected = true;
	}

	focusedAnswerIsSelected(): boolean {
		if (this.focusIndexIsInvalid())
			return false;
		return this.questionAnswerMap.Answers[this.focusIndex].IsSelected;
	}

	private focusIndexIsInvalid() {
		return this.focusIndex < 0 || this.focusIndex >= this.questionAnswerMap.Answers.length;
	}

	getSelectedAnswerCount(): number {
		let count = 0;
		this.questionAnswerMap.Answers.forEach((answer: Answer) => {
			if (answer.IsSelected)
				count++;
		});
		return count;
	}

	private toggleAnswer(soundManager: SoundManager): void {
		if (!this.focusIndicator || this.focusIndexIsInvalid() || this.questionAnswerMap.MaxAnswers === 1) {
			return;
		}
		else {
			if (this.focusedAnswerIsSelected()) {
				this.deselectAnswer(soundManager, this.focusIndex);
			}
			else {
				this.addSelectionIndicatorSprite(this.focusIndex);
				this.questionAnswerMap.Answers[this.focusIndex].IsSelected = true;
				const selectedAnswerCount: number = this.getSelectedAnswerCount();
				if (selectedAnswerCount > this.questionAnswerMap.MaxAnswers)
					this.deselect(soundManager, selectedAnswerCount - this.questionAnswerMap.MaxAnswers);
				soundManager.safePlayMp3('InGameUI/ToggleSelected');
			}
		}
	}
	private deselectAnswer(soundManager: SoundManager, focusIndex: number) {
		this.questionAnswerMap.Answers[focusIndex].IsSelected = false;
		this.removeSelectionIndicatorSprite(focusIndex);
		soundManager.safePlayMp3('InGameUI/ToggleUnselected');
	}

	deselect(soundManager: SoundManager, numToDeselect: number) {
		let numCleared = 0;
		for (let i = 0; i < this.questionAnswerMap.Answers.length; i++) {
			if (i !== this.focusIndex && this.questionAnswerMap.Answers[i].IsSelected) {
				this.deselectAnswer(soundManager, i);
				numCleared++;
				if (numCleared >= numToDeselect)
					return;
			}
		}
	}

	removeSelectionIndicatorSprite(focusIndex: number): void {
		if (this.focusIndexIsInvalid())
			return;
		for (let i = 0; i < this.selectionIndicatorSprites.spriteProxies.length; i++) {
			if (this.selectionIndicatorSprites.spriteProxies[i].data === focusIndex) {
				console.log(`removing sprite at index [${i}]`);
				this.selectionIndicatorSprites.spriteProxies[i].fadeOutNow(300);
			}
		}
	}

	private addSelectionIndicatorSprite(focusIndex: number) {
		//const hueShift = 180;  // Blue
		const hueShift = 330;  // Red
		const yPos: number = this.questionAnswerMap.Answers[focusIndex].centerY;  // this.focusIndicator.y + this.focusIndicatorSprites.originY
		const selectionIndicator: SpriteProxy = this.selectionIndicatorSprites.addShifted(this.focusIndicator.x + this.focusIndicatorSprites.originX, yPos, -1, hueShift);
		selectionIndicator.scale = this.answerIndicatorScale * 1.1;
		selectionIndicator.data = this.focusIndex;
	}

	private moveDown(soundManager: SoundManager) {
		if (this.focusIndex < this.questionAnswerMap.Answers.length - 1) {
			this.focusIndex++;
			this.updateFocusIndicator();
			this.playSlidingSound(soundManager);
		}
	}

	private moveUp(soundManager: SoundManager) {
		if (this.focusIndex > 0) {
			this.focusIndex--;
			this.updateFocusIndicator();
			this.playSlidingSound(soundManager);
		}
	}

	updateFocusIndicator() {
		//this.selectionIndex
		//this.answerHeight
		this.verticalScale;
		const sprite: SpriteProxy = this.focusIndicator;

		// TODO: Fix this math error. Close but not perfect...
		const newY: number = this.questionAnswerMap.Answers[this.focusIndex].centerY;  // << Fix

		sprite.ease(performance.now(), sprite.x, sprite.y, sprite.x, newY - this.focusIndicatorSprites.originY, this.selectorEaseTime);
	}

	playToEndNow(sprites: Sprites) {
		for (let i = 0; i < sprites.spriteProxies.length; i++) {
			const sprite: SpriteProxy = sprites.spriteProxies[i];
			sprite.playToEndNow = true;
			sprite.fadeOutAfter(0, 800);
		}
	}

	fadeOutNow(sprites: Sprites) {
		for (let i = 0; i < sprites.spriteProxies.length; i++) {
			sprites.spriteProxies[i].fadeOutNow(MessageBox.textFadeOutTime);
		}
	}

	show(context: CanvasRenderingContext2D, soundManager: SoundManager) {
		this.focusIndex = 0;
		this.distanceToMoveScroll = 0;
		this.showingTextYet = false;
		this.hidingTextYet = false;
		this.titleTop = undefined;
		this.answerLeft = undefined;
		this.calculateSize(context);

		// clear any existing scroll sprites.
		this.scrollBack.spriteProxies = [];
		this.scrollFront.spriteProxies = [];

		// adding the sprites...
		const scrollFront: SpriteProxy = this.scrollFront.add(960, 1080);
		this.scaleScroll(scrollFront);
		const scrollBack: SpriteProxy = this.scrollBack.add(960, 1080);
		this.scaleScroll(scrollBack);
		scrollBack.data = this.invocationIndex;
		scrollFront.data = this.invocationIndex;
		this.invocationIndex++;

		soundManager.playMp3In(0, 'InGameUI/ScrollUnfurl');
		soundManager.playMp3In(0, 'InGameUI/ScrollAppearAlert');
		soundManager.playMp3In(0, 'InGameUI/AiryWhoosh[2]');

		scrollBack.addOnFrameAdvanceCallback((sprite: SpriteProxy) => {
			if (!this.showingTextYet && sprite.frameIndex > MessageBox.textBeginFrameIndex && sprite.getAlpha(performance.now()) === 1) {
				this.showingTextYet = true;
				this.addText();
			}
			if (this.distanceToMoveScroll !== 0 && sprite.frameIndex >= MessageBox.frameScrollIsCompletelyOnScreen) {
				this.animateScrollUp(sprite);
				this.distanceToMoveScroll = 0;
			}
		}
		);
	}

	invocationIndex = 0;

	animateScrollUp(backSprite: SpriteProxy) {
		const framesUntilFullyOpen: number = MessageBox.textBeginFrameIndex - backSprite.frameIndex;
		let timeToMove = MessageBox.textFadeInTime / 2;
		if (framesUntilFullyOpen > 0)
			timeToMove += framesUntilFullyOpen * fps30;
		this.moveScrollSpriteUp(backSprite, timeToMove);
		this.scrollFront.spriteProxies.forEach((frontSprite: SpriteProxy) => {
			if (frontSprite.data === backSprite.data) {
				this.moveScrollSpriteUp(frontSprite, timeToMove);
			}
		});
	}

	focusIndicator: SpriteProxy;

	static readonly focusDiameter: number = 136;

	private moveScrollSpriteUp(sprite: SpriteProxy, timeToMove: number) {
		sprite.ease(performance.now(), sprite.x, sprite.y, sprite.x, sprite.y - this.distanceToMoveScroll, timeToMove);
	}

	private scaleScroll(scrollFront: SpriteProxy) {
		scrollFront.horizontalScale = this.horizontalScale;
		scrollFront.verticalScale = this.verticalScale;
	}

	answerIndicatorScale = 1;

	private addFocusIndicator(x: number, y: number) {
		let hueShift = 0;
		if (this.questionAnswerMap.MaxAnswers > 1) {
			hueShift = -220;
		}
		this.focusIndicator = this.focusIndicatorSprites.addShifted(x, y, -1, hueShift);
		this.answerIndicatorScale = 0.6 * this.answerFontSize / MessageBox.focusDiameter;
		if (this.questionAnswerMap.MaxAnswers > 1) {
			this.focusIndicator.scale = 0.45 * this.answerIndicatorScale;
			//this.focusIndicator.opacity = 0.65;
		}
		else
			this.focusIndicator.scale = this.answerIndicatorScale;
		this.focusIndicator.fadeInTime = MessageBox.textFadeInTime;
	}


	showingTextYet: boolean;
	hidingTextYet: boolean;

	static readonly minTitleFontSize: number = 72;
	static readonly maxTitleFontSize: number = 210;
	static readonly titleTopMargin: number = 25;
	static readonly bottomMargin: number = 30;
	static readonly discoverabilityHintHeight: number = 30;

	static readonly minAnswerFontSize: number = 36;
	static readonly maxAnswerFontSize: number = 100;
	static readonly maxDialogHeight: number = 980;

	titleFontSize = MessageBox.maxTitleFontSize;
	titleWidth: number;
	actualScrollHeight: number;
	scrollTop = 0;
	answerFontSize = MessageBox.maxAnswerFontSize;

	static readonly fontName: string = 'Enchanted Land';
	static readonly fontColor: string = '#281002';
	static readonly hintColorWarning: string = '#8f0707';
	static readonly hintColorGood: string = '#612705';
	static readonly titleOpacity: number = 0.88;

	static readonly scrollWidth: number = 994;
	static readonly scrollHeight: number = 827;
	static readonly scrollAnimationHeight: number = 988;
	static readonly screenWidth: number = 1920;
	static readonly screenHeight: number = 1080;
	static readonly availableTitleWidth: number = 670;
	static readonly availableAnswerWidth: number = 70;
	static readonly availableTitleHeight: number = 112;
	static readonly maxWidthScale: number = MessageBox.screenWidth / MessageBox.scrollWidth;
	static readonly maxHeightScale: number = MessageBox.screenHeight / MessageBox.scrollHeight;

	static readonly maxTitleWidth: number = MessageBox.maxWidthScale * MessageBox.availableTitleWidth;
	static readonly maxTitleHeight: number = MessageBox.maxHeightScale * MessageBox.availableTitleHeight;

	horizontalScale = 1;
	verticalScale = 1;
	textWidth: number;
	textHeight: number;
	distanceToMoveScroll: number;

	calculateSize(context: CanvasRenderingContext2D) {
		this.horizontalScale = 1;
		this.verticalScale = 1;
		this.calculateTitleFont(context);
		this.calculateHorizontalScaleBasedOnTitle();
		this.titleLeft = MessageBox.screenWidth / 2 - this.titleWidth / 2;

		let maxCalculatedAnswerFontSize = MessageBox.maxAnswerFontSize;
		this.questionAnswerMap.Answers.forEach((answer) => {
			const answerMaxFont: number = this.getFontSizeToFit(context, answer.AnswerText, MessageBox.maxAnswerFontSize, MessageBox.availableTitleWidth * this.horizontalScale - maxCalculatedAnswerFontSize);

			if (answerMaxFont < maxCalculatedAnswerFontSize)
				maxCalculatedAnswerFontSize = answerMaxFont;
		});
		// TODO: adjust dialog scale as needed.
		this.answerFontSize = maxCalculatedAnswerFontSize;


		// TODO: shrink the answer font if we can't fit them all on screen.
		let discoverabilityHeight = 0;
		if (this.questionAnswerMap.MaxAnswers > 1)
			discoverabilityHeight = MessageBox.discoverabilityHintHeight;
		let testAnswerFontSize: number = maxCalculatedAnswerFontSize;
		while (this.calculateVerticalValues(discoverabilityHeight, testAnswerFontSize) > MessageBox.maxDialogHeight) {
			testAnswerFontSize--;
		}
		this.scrollTop = MessageBox.screenHeight / 3 - this.actualScrollHeight / 2;
		if (this.scrollTop < 0)
			this.scrollTop = 0;

		const scrollBottom: number = this.scrollTop + MessageBox.scrollAnimationHeight * this.verticalScale;
		this.titleTop = this.getTitleTop(MessageBox.titleTopMargin);
		console.log('this.titleTop from calculateSize: ' + this.titleTop);
		this.distanceToMoveScroll = MessageBox.screenHeight - scrollBottom;
		//console.log('this.distanceToMoveScroll: ' + this.distanceToMoveScroll);
	}

	private calculateVerticalValues(discoverabilityHeight: number, testAnswerFontSize: number): number {
		this.answerFontSize = testAnswerFontSize;
		this.discoverabilityTop = this.getTitleBottomFromTopOfScroll() + this.answerFontSize * this.questionAnswerMap.Answers.length;
		const totalHeight: number = this.discoverabilityTop + MessageBox.bottomMargin + discoverabilityHeight;
		this.verticalScale = totalHeight / MessageBox.scrollHeight;
		//console.log('this.verticalScale: ' + this.verticalScale);
		this.actualScrollHeight = MessageBox.scrollHeight * this.verticalScale;
		return this.actualScrollHeight;
	}

	private getTitleBottomFromTopOfScroll() {
		return MessageBox.titleTopMargin + this.titleFontSize * this.titleParagraph.lineData.length;
	}

	private calculateHorizontalScaleBasedOnTitle() {
		if (this.titleFontSize < MessageBox.minTitleFontSize) {
			this.horizontalScale = MessageBox.minTitleFontSize / this.titleFontSize;
			if (this.horizontalScale > MessageBox.maxWidthScale)
				this.horizontalScale = MessageBox.maxWidthScale;
			this.titleFontSize *= this.horizontalScale;
		}
		this.titleWidth = this.titleParagraph.getLongestLineWidth();
		if (this.titleWidth > MessageBox.availableTitleWidth) {
			this.horizontalScale = this.titleWidth / MessageBox.availableTitleWidth;
		}
	}

	getFontSizeToFit(context: CanvasRenderingContext2D, text: string, maxFontSize: number, maxWidth: number): number {
		let fontSize: number = maxFontSize;
		context.font = this.getFont(fontSize);
		while (context.measureText(text).width > maxWidth) {
			fontSize--;
			context.font = this.getFont(fontSize);
		}
		return fontSize;
	}

	private getFont(fontSize: number): string {
		return `${fontSize}px ${MessageBox.fontName}`;
	}

	//private addTitle(title: string, top: number) {
	//	const textEffect: TextEffect = this.animations.addText(new Vector(960, top), title);
	//	textEffect.textBaseline = 'top';
	//	textEffect.fontSize = this.titleFontSize;
	//	this.prepareTextEffect(textEffect);
	//	textEffect.opacity = MessageBox.titleOpacity;
	//}

	private addAnswer(answerText: string, left: number, top: number): TextEffect {
		const textEffect: TextEffect = this.animations.addText(new Vector(left, top), answerText);
		textEffect.fontSize = this.answerFontSize;
		textEffect.textBaseline = 'middle';
		textEffect.textAlign = 'left';
		this.prepareTextEffect(textEffect);
		textEffect.opacity = 1;
		return textEffect;
	}

	private prepareTextEffect(textEffect: TextEffect) {
		textEffect.scale = 1;
		textEffect.fadeOutTime = MessageBox.textFadeOutTime;
		textEffect.fadeInTime = MessageBox.textFadeInTime;
		textEffect.fontName = MessageBox.fontName;
		textEffect.fontColor = MessageBox.fontColor;
	}

	titleTop: number = undefined;
	answerLeft: number = undefined;

	addText() {
		this.answerLeft = 960 - this.titleWidth / 2 + this.answerFontSize;

		// TODO: Stop calling addTitle if we're going to draw the titles ourselves.
		//this.addTitle(this.questionAnswerMap.Question, this.titleTop);

		const focusIndicatorRadius: number = this.answerFontSize / 2;
		//console.log(`this.titleFontSize == ${this.titleFontSize}, this.titleParagraph.lineData.length = ${this.titleParagraph.lineData.length}`);
		let centerY: number = this.scrollTop + this.getTitleBottomFromTopOfScroll() + focusIndicatorRadius;
		this.addFocusIndicator(this.answerLeft - focusIndicatorRadius, centerY);
		this.questionAnswerMap.Answers.forEach((answer: Answer) => {
			answer.centerY = centerY;
			this.lastAnswer = this.addAnswer(answer.AnswerText, this.answerLeft, centerY);
			centerY += this.answerFontSize;
		});
	}

	private getTitleTop(topMargin: number) {
		return this.scrollTop + topMargin;
	}

	titleLeft: number;

	private calculateTitleFont(context: CanvasRenderingContext2D) {
		this.titleFontSize = this.getFontSizeToFit(context, this.questionAnswerMap.Question, MessageBox.maxTitleFontSize, MessageBox.availableTitleWidth);

		if (this.titleFontSize < MessageBox.minTitleFontSize)
			this.titleFontSize = MessageBox.minTitleFontSize;

		context.font = this.getFont(this.titleFontSize);
		this.wordRenderer.fontName = MessageBox.fontName;
		this.wordRenderer.fontSize = this.titleFontSize;
		this.titleParagraph = this.wordWrapper.getWordWrappedLinesForParagraphs(context, this.questionAnswerMap.Question, MessageBox.maxTitleWidth, MessageBox.styleDelimiters, this.wordRenderer);
		if (this.titleParagraph.lineData.length === 2) {
			const percentageWidthSecondLine: number = this.titleParagraph.lineData[1].width / this.titleParagraph.lineData[0].width;
			if (percentageWidthSecondLine < 0.50) {
				this.titleParagraph = this.wordWrapper.getWordWrappedLinesForParagraphs(context, this.questionAnswerMap.Question, 0.75 * MessageBox.maxTitleWidth, MessageBox.styleDelimiters, this.wordRenderer);
			}
		}
	}

	renderMultiSelectDiscoverability(context: CanvasRenderingContext2D, nowMs: number) {
		if (!this.questionAnswerMap)
			return;
		if (this.questionAnswerMap.MaxAnswers <= 1)
			return;
		if (this.answerLeft === undefined)
			return;
		context.textBaseline = 'top';
		context.font = `${44}px ${MessageBox.fontName}`;
		const topMargin = 20;
		if (!this.lastAnswer) {
			context.globalAlpha = 0;
		}
		else {
			context.globalAlpha = this.lastAnswer.getAlpha(nowMs);
		}
		try {
			const numAnswers: number = this.getSelectedAnswerCount();

			let emphasisPunctuation: string;
			if (numAnswers === this.questionAnswerMap.MaxAnswers) {
				emphasisPunctuation = '.';
				context.fillStyle = MessageBox.hintColorGood;
			}
			else {
				emphasisPunctuation = '!';
				context.fillStyle = MessageBox.hintColorWarning;
			}
			context.fillText(`${numAnswers} of ${this.questionAnswerMap.MaxAnswers} selected${emphasisPunctuation}`, this.answerLeft, this.discoverabilityTop + topMargin);
		}
		finally {
			context.globalAlpha = 1;
		}
	}

	renderTitle(context: CanvasRenderingContext2D, nowMs: number) {
		if (this.titleTop === undefined)
			return;
		if (!this.lastAnswer)
			return;

		if (this.lastAnswer.expirationDate && this.lastAnswer.getLifeRemainingMs(nowMs) <= 0) {
			this.lastAnswer = null;
			return;
		}
		this.wordRenderer.fontName = MessageBox.fontName;
		this.wordRenderer.fontSize = this.titleFontSize;
		this.wordRenderer.setDetailFontNormal(context);
		context.fillStyle = MessageBox.fontColor;
		context.textBaseline = 'top';
		context.globalAlpha = this.lastAnswer.getAlpha(nowMs) * 0.8;
		try {
			this.wordRenderer.activeStyle = LayoutStyle.normal;
			if (this.titleParagraph) {
				//console.log(`drawing title at (${this.titleLeft}, ${this.titleTop})`);
				this.wordRenderer.renderParagraphs(context, this.titleParagraph.lineData, this.titleLeft, this.titleTop, MessageBox.styleDelimiters);
			}
		}
		finally {
			context.globalAlpha = 1;
		}
	}

	private acceptAnswer() {
		this.playToEndNow(this.scrollBack);
		this.playToEndNow(this.scrollFront);
		const now: number = performance.now();
		for (let i = 0; i < this.animations.animationProxies.length; i++) {
			this.animations.animationProxies[i].expirationDate = now + MessageBox.textFadeOutTime;
		}
		this.fadeOutNow(this.selectionIndicatorSprites);
		this.fadeOutNow(this.focusIndicatorSprites);
		if (this.getSelectedAnswerCount() === 0)
			this.selectFocusedAnswer();
		// Send answer back to C# app.
		inGameUIResponse(JSON.stringify(this.questionAnswerMap));
	}
}