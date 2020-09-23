class AnswerEntry {
	constructor(public Index: number, public Value: number, public IsSelected: boolean, public AnswerText: string) {

	}
}

class AnswerMap {
	constructor(public Question: string, public Answers: Array<AnswerEntry>, public MinAnswers = 1, public MaxAnswers = 1) {

	}
}

class MessageBox {
	soundManager: SoundManager;
	scrollBack: Sprites;
	scrollFront: Sprites;
	selectionIndicator: Sprites;
	focusIndicator: Sprites;
	animations: Animations;

	constructor(soundManager: SoundManager) {
		this.soundManager = soundManager;
	}

	loadResources() {
		this.animations = new Animations();
		this.scrollBack = this.loadScroll('Back', 333);
		this.scrollFront = this.loadScroll('Front', 309, -2);
		this.focusIndicator = new Sprites(`InGameUI/FocusIndicator/Focus`, 173, fps30, AnimationStyle.Loop, true);
		this.focusIndicator.originX = 107;
		this.focusIndicator.originY = 107;

		this.selectionIndicator = new Sprites(`InGameUI/SelectionIndicator/SelectionIndicator`, 105, fps30, AnimationStyle.Loop, true);
		this.selectionIndicator.originX = 162;
		this.selectionIndicator.originY = 162;

		this.setForMove(this.scrollBack);
		this.setForMove(this.scrollFront);
		this.setForMove(this.focusIndicator);
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
		this.focusIndicator.updatePositions(nowMs);
	}

	draw(context: CanvasRenderingContext2D, nowMs: number) {
		this.scrollBack.draw(context, nowMs);
		this.animations.render(context, nowMs);
		this.selectionIndicator.draw(context, nowMs);
		this.focusIndicator.draw(context, nowMs);
		this.scrollFront.draw(context, nowMs);
	}

	answerHeight = 200;
	selectorEaseTime = 300;
	answerMap: AnswerMap;
	static readonly textBeginFrameIndex: number = 24;
	static readonly textEndFrameIndex: number = 299;
	static readonly textFadeInTime: number = 20 * fps30;
	static readonly textFadeOutTime: number = 6 * fps30;

	executeCommand(commandData: string) {
		console.log('commandData: ' + commandData);
		// TODO: Prevent multiple scroll UIs from appearing.
		if (commandData === 'Up') {
			if (this.focusIndicator.spriteProxies.length === 0)
				this.focusIndicator.add(960, 540);
			else {
				const sprite: SpriteProxy = this.focusIndicator.spriteProxies[0];
				sprite.ease(performance.now(), sprite.x, sprite.y, sprite.x, sprite.y - this.answerHeight, this.selectorEaseTime);
			}
		}
		else if (commandData === 'Down') {
			if (this.focusIndicator.spriteProxies.length === 0)
				this.focusIndicator.add(960, 540);
			else {
				const sprite: SpriteProxy = this.focusIndicator.spriteProxies[0];
				sprite.ease(performance.now(), sprite.x, sprite.y, sprite.x, sprite.y + this.answerHeight, this.selectorEaseTime);
			}
		}
		else if (commandData === 'Ask') {
		}
		else if (commandData === 'Toggle') {
			if (this.focusIndicator.spriteProxies.length === 0) {
				this.focusIndicator.add(960, 540);
				this.selectionIndicator.add(960, 540);
			}
			else {
				const sprite: SpriteProxy = this.focusIndicator.spriteProxies[0];
				this.selectionIndicator.add(sprite.x + this.focusIndicator.originX, sprite.y + this.focusIndicator.originY);
			}
		}
		else if (commandData === 'OK') {
			this.playToEndNow(this.scrollBack);
			this.playToEndNow(this.scrollFront);
			const now: number = performance.now();
			for (let i = 0; i < this.animations.animationProxies.length; i++) {
				this.animations.animationProxies[i].expirationDate = now + MessageBox.textFadeOutTime;
			}
			this.fadeOutNow(this.selectionIndicator);
			this.fadeOutNow(this.focusIndicator);
			// TODO: Send answer back to C# app.
		}
		else {

			this.answerMap = JSON.parse(commandData);
			this.show();
		}
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

	showingTextYet: boolean;
	hidingTextYet: boolean;

	show() {
		this.showingTextYet = false;
		this.hidingTextYet = false;
		this.scrollFront.add(960, 1080);
		const sprite: SpriteProxy = this.scrollBack.add(960, 1080);
		sprite.addOnFrameAdvanceCallback((sprite: SpriteProxy) => {
			if (!this.showingTextYet && sprite.frameIndex > MessageBox.textBeginFrameIndex) {
				this.showingTextYet = true;
				this.addText();
			}
		}
		);
	}

	addText() {
		const textEffect: TextEffect = this.animations.addText(new Vector(960, 240), this.answerMap.Question);
		textEffect.scale = 3;
		textEffect.fadeOutTime = MessageBox.textFadeOutTime;
		textEffect.fadeInTime = MessageBox.textFadeInTime;
		textEffect.fontName = 'Enchanted Land';
		textEffect.fontSize = 74;
		textEffect.fontColor = '#281002';
		textEffect.opacity = 0.88;
	}
}