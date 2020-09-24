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
	soundManager: SoundManager;
	scrollBack: Sprites;
	scrollFront: Sprites;
	selectionIndicator: Sprites;
	focusIndicatorSprites: Sprites;
	animations: Animations;
	focusIndex = 0;

	constructor(soundManager: SoundManager) {
		this.soundManager = soundManager;
	}

	loadResources() {
		this.animations = new Animations();
		this.scrollBack = this.loadScroll('Back', 333);
		this.scrollFront = this.loadScroll('Front', 309, -2);
		this.focusIndicatorSprites = new Sprites(`InGameUI/FocusIndicator/Focus`, 173, fps30, AnimationStyle.Loop, true);
		this.focusIndicatorSprites.originX = 107;
		this.focusIndicatorSprites.originY = 107;

		this.selectionIndicator = new Sprites(`InGameUI/SelectionIndicator/SelectionIndicator`, 105, fps30, AnimationStyle.Loop, true);
		this.selectionIndicator.originX = 162;
		this.selectionIndicator.originY = 162;

		this.setForMove(this.scrollBack);
		this.setForMove(this.scrollFront);
		this.setForMove(this.focusIndicatorSprites);
		this.setForMove(this.selectionIndicator);
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
		this.selectionIndicator.updatePositions(nowMs);
		this.focusIndicatorSprites.updatePositions(nowMs);
	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
		this.scrollBack.draw(context, nowMs);
		this.animations.render(context, nowMs);
		this.selectionIndicator.draw(context, nowMs);
		this.focusIndicatorSprites.draw(context, nowMs);
		this.scrollFront.draw(context, nowMs);
	}

	selectorEaseTime = 300;
	questionAnswerMap: QuestionAnswerMap;
	static readonly textBeginFrameIndex: number = 28;
	static readonly textEndFrameIndex: number = 299;
	static readonly textFadeInTime: number = 20 * fps30;
	static readonly textFadeOutTime: number = 6 * fps30;
	static readonly frameScrollIsCompletelyOnScreen: number = 19;

	executeCommand(commandData: string, context: CanvasRenderingContext2D) {
		console.log('commandData: ' + commandData);
		// TODO: Prevent multiple scroll UIs from appearing.
		if (commandData === 'Up') {
			this.moveUp();
		}
		else if (commandData === 'Down') {
			this.moveDown();
		}
		else if (commandData === 'Ask') {
			// test.
		}
		else if (commandData === 'Toggle') {
			this.toggleAnswer();
		}
		else if (commandData === 'OK') {
			this.acceptAnswer();
		}
		else {
			this.questionAnswerMap = JSON.parse(commandData);
			this.show(context);
		}
	}

	private acceptAnswer() {
		this.playToEndNow(this.scrollBack);
		this.playToEndNow(this.scrollFront);
		const now: number = performance.now();
		for (let i = 0; i < this.animations.animationProxies.length; i++) {
			this.animations.animationProxies[i].expirationDate = now + MessageBox.textFadeOutTime;
		}
		this.fadeOutNow(this.selectionIndicator);
		this.fadeOutNow(this.focusIndicatorSprites);
		if (this.getSelectedAnswerCount() === 0)
			this.selectFocusedAnswer();
		// Send answer back to C# app.
		inGameUIResponse(JSON.stringify(this.questionAnswerMap));
	}

	selectFocusedAnswer() {
		this.questionAnswerMap.Answers[this.focusIndex].IsSelected = true;
	}

	getSelectedAnswerCount(): number {
		let count = 0;
		this.questionAnswerMap.Answers.forEach((answer: Answer) => {
			if (answer.IsSelected)
				count++;
		});
		return count;
	}

	private toggleAnswer() {
		if (this.focusIndicatorSprites.spriteProxies.length === 0) {
			this.focusIndicatorSprites.add(960, 540);
			this.selectionIndicator.add(960, 540);
		}
		else {
			const sprite: SpriteProxy = this.focusIndicatorSprites.spriteProxies[0];
			this.selectionIndicator.add(sprite.x + this.focusIndicatorSprites.originX, sprite.y + this.focusIndicatorSprites.originY);
		}
	}

	private moveDown() {
		if (this.focusIndex < this.questionAnswerMap.Answers.length - 1) {
			this.focusIndex++;
			this.updateFocusIndicator();
		}
	}

	private moveUp() {
		if (this.focusIndex > 0) {
			this.focusIndex--;
			this.updateFocusIndicator();
		}
	}

	updateFocusIndicator() {
		//this.selectionIndex
		//this.answerHeight
		this.verticalScale;
		const sprite: SpriteProxy = this.focusIndicator;

		// TODO: Fix this math error. Close but not perfect...
		//const newY: number = this.getTitleBottom() + this.answerFontSize / 2 + this.answerFontSize * this.selectionIndex;  << Bug
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

	show(context: CanvasRenderingContext2D) {
		this.focusIndex = 0;
		this.distanceToMoveScroll = 0;
		this.showingTextYet = false;
		this.hidingTextYet = false;
		this.calculateSize(context);


		// adding the sprites...
		const scrollFront: SpriteProxy = this.scrollFront.add(960, 1080);
		this.scaleScroll(scrollFront);
		const scrollBack: SpriteProxy = this.scrollBack.add(960, 1080);
		this.scaleScroll(scrollBack);
		scrollBack.data = this.invocationIndex;
		scrollFront.data = this.invocationIndex;
		this.invocationIndex++;

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

	private addFocusIndicator(x: number, y: number) {
		this.focusIndicator = this.focusIndicatorSprites.add(x, y);
		this.focusIndicator.scale = 0.7 * this.answerFontSize / MessageBox.focusDiameter;
		this.focusIndicator.fadeInTime = MessageBox.textFadeInTime;
	}


	showingTextYet: boolean;
	hidingTextYet: boolean;

	static readonly minTitleFontSize: number = 72;
	static readonly maxTitleFontSize: number = 210;
	static readonly titleTopMargin: number = 15;
	static readonly minAnswerFontSize: number = 36;
	static readonly maxAnswerFontSize: number = 100;

	titleFontSize = MessageBox.maxTitleFontSize;
	titleWidth: number;
	actualScrollHeight: number;
	scrollTop = 0;
	answerFontSize = MessageBox.maxAnswerFontSize;

	static readonly fontName: string = 'Enchanted Land';
	static readonly fontColor: string = '#281002';
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

		let maxCalculatedAnswerFontSize = MessageBox.maxAnswerFontSize;
		this.questionAnswerMap.Answers.forEach((answer) => {
			const answerMaxFont: number = this.getFontSizeToFit(context, answer.AnswerText, MessageBox.maxAnswerFontSize, MessageBox.availableTitleWidth * this.horizontalScale - maxCalculatedAnswerFontSize);

			if (answerMaxFont < maxCalculatedAnswerFontSize)
				maxCalculatedAnswerFontSize = answerMaxFont;
		});
		// TODO: adjust dialog scale as needed.
		this.answerFontSize = maxCalculatedAnswerFontSize;


		// TODO: shrink the answer font if we can't fit them all on screen.
		const bottomMargin = 20;
		const totalHeight: number = this.getTitleBottom() + this.answerFontSize * this.questionAnswerMap.Answers.length + bottomMargin;
		this.verticalScale = totalHeight / MessageBox.scrollHeight;
		console.log('this.verticalScale: ' + this.verticalScale);

		this.actualScrollHeight = MessageBox.scrollHeight * this.verticalScale;
		this.scrollTop = MessageBox.screenHeight / 3 - this.actualScrollHeight / 2;
		if (this.scrollTop < 0)
			this.scrollTop = 0;

		const scrollBottom: number = this.scrollTop + MessageBox.scrollAnimationHeight * this.verticalScale;

		this.distanceToMoveScroll = MessageBox.screenHeight - scrollBottom;
		console.log('this.distanceToMoveScroll: ' + this.distanceToMoveScroll);
	}

	private getTitleBottom() {
		return MessageBox.titleTopMargin + this.titleFontSize;
	}

	private calculateHorizontalScaleBasedOnTitle() {
		if (this.titleFontSize < MessageBox.minTitleFontSize) {
			this.horizontalScale = MessageBox.minTitleFontSize / this.titleFontSize;
			if (this.horizontalScale > MessageBox.maxWidthScale)
				this.horizontalScale = MessageBox.maxWidthScale;
			this.titleFontSize *= this.horizontalScale;
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

	private calculateTitleFont(context: CanvasRenderingContext2D) {
		this.titleFontSize = this.getFontSizeToFit(context, this.questionAnswerMap.Question, MessageBox.maxTitleFontSize, MessageBox.availableTitleWidth);
		context.font = this.getFont(this.titleFontSize);
		this.titleWidth = context.measureText(this.questionAnswerMap.Question).width;
	}

	addText() {
		const left: number = 960 - this.titleWidth / 2 + this.answerFontSize;
		const topMargin = MessageBox.titleTopMargin;
		const top = this.scrollTop + topMargin;
		this.addTitle(this.questionAnswerMap.Question, top);
		const focusIndicatorRadius: number = this.answerFontSize / 2;
		let centerY: number = top + this.titleFontSize + focusIndicatorRadius;
		this.addFocusIndicator(left - focusIndicatorRadius, centerY);
		this.questionAnswerMap.Answers.forEach((answer: Answer) => {
			answer.centerY = centerY;
			this.addAnswer(answer.AnswerText, left, centerY);
			centerY += this.answerFontSize;
		});
	}

	private addTitle(title: string, top: number) {
		const textEffect: TextEffect = this.animations.addText(new Vector(960, top), title);
		textEffect.textBaseline = 'top';
		textEffect.fontSize = this.titleFontSize;
		this.prepareTextEffect(textEffect);
		textEffect.opacity = MessageBox.titleOpacity;
	}

	private addAnswer(answerText: string, left: number, top: number) {
		const textEffect: TextEffect = this.animations.addText(new Vector(left, top), answerText);
		textEffect.fontSize = this.answerFontSize;
		textEffect.textBaseline = 'middle';
		textEffect.textAlign = 'left';
		this.prepareTextEffect(textEffect);
		textEffect.opacity = 1;
	}

	private prepareTextEffect(textEffect: TextEffect) {
		textEffect.scale = 1;
		textEffect.fadeOutTime = MessageBox.textFadeOutTime;
		textEffect.fadeInTime = MessageBox.textFadeInTime;
		textEffect.fontName = MessageBox.fontName;
		textEffect.fontColor = MessageBox.fontColor;
	}
}