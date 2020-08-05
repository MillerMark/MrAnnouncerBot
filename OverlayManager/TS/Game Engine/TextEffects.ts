class ScalableAnimation extends AnimatedElement {
	elasticIn: boolean;
	scale: number;
	targetScale = -1;
	originalScale: number;
	deltaScale: number = undefined;
	waitToScale = 0;

	constructor(x: number, y: number, lifeSpanMs = -1) {
		super(x, y, lifeSpanMs);
		this.scale = 1;
		this.originalScale = -1;
	}

	lerp(start: number, end: number, percentComplete: number) {
		return start + (end - start) * percentComplete;
	}

	static lerpc(start: number, change: number, percentComplete: number) {
		return start + change * percentComplete;
	}

	resizeToContent(left: number, top: number, width: number, height: number) {
		// Descendants can override...
	}

	getScale(now: number) {
		let thisScale: number = this.scale;
		if (this.targetScale >= 0) {

			if (this.originalScale < 0)
				this.originalScale = this.scale;

			if (this.deltaScale === undefined)
				this.deltaScale = this.targetScale - this.originalScale;

			const scaleStartTime: number = this.timeStart + this.waitToScale;
			const timePassed: number = now - scaleStartTime;

			if (timePassed > 0) {
				const lifespan: number = this.expirationDate - scaleStartTime;

				let percentComplete: number;
				if (lifespan === 0)
					percentComplete = 1;
				else
					percentComplete = Math.max(Math.min(1, timePassed / lifespan), 0);

				let easedPercent: number;
				if (this.elasticIn)
					easedPercent = EasePoint.inElastic(percentComplete);
				else
					easedPercent = percentComplete;

				thisScale = ScalableAnimation.lerpc(this.originalScale, this.deltaScale, easedPercent);
			}
		}
		return thisScale;
	}
}

class Animations {
	animations: Array<AnimatedElement> = new Array<AnimatedElement>();
	constructor() {

	}

	clear() {
		this.animations = new Array<AnimatedElement>();
	}

	addText(centerPos: Vector, text: string, lifeSpanMs: number = -1): TextEffect {
		let textEffect: TextEffect = new TextEffect(centerPos.x, centerPos.y, lifeSpanMs);
		textEffect.text = text;
		this.animations.push(textEffect);
		return textEffect;
	}

	addLine(x: number, y: number, width: number, color: string, lifespan: number, lineThickness: number): AnimatedLine {
		let line: AnimatedLine = new AnimatedLine(x, y, width, color, lifespan, lineThickness);
		line.scale = 1;
		line.targetScale = 1;
		this.animations.push(line);
		return line;
	}

	addRectangle(x: number, y: number, width: number, height: number, fillColor: string, outlineColor: string, lifespanMs: number, lineThickness: number = 1, opacity: number = 1): AnimatedRectangle {
		let rectangle: AnimatedRectangle = new AnimatedRectangle(x, y, width, height, fillColor, outlineColor, lifespanMs, lineThickness);
		rectangle.opacity = opacity;
		rectangle.scale = 1;
		rectangle.targetScale = 1;
		this.animations.push(rectangle);
		return rectangle;
	}

	render(context: CanvasRenderingContext2D, now: number) {
		this.animations.forEach(function (animatedElement: AnimatedElement) {
			animatedElement.render(context, now);
		});
	}

	removeExpiredAnimations(now: number): void {
		for (var i = this.animations.length - 1; i >= 0; i--) {
			let animatedElement: AnimatedElement = this.animations[i];
			if (animatedElement.expirationDate) {
				if (!animatedElement.stillAlive(now)) {
					animatedElement.destroying();
					animatedElement.removing();
					if (!animatedElement.isRemoving) {
						this.animations.splice(i, 1);
					}
				}
			}
		}
	}

	updatePositions(now: number): void {
		for (var i = this.animations.length - 1; i >= 0; i--) {
			let animatedElement: AnimatedElement = this.animations[i];
			animatedElement.updatePosition(now);
		}
	}
}

class TextEffect extends ScalableAnimation {
	text: string;
	fontColor: string;
	fontName: string;
	outlineColor: string;
	verticalThrust: number = 0;
	horizontalThrust: number = 0;
	textAlign: CanvasTextAlign = 'center';
	textBaseline: CanvasTextBaseline = 'middle';
	outlineThickness: number;
	fontSize: number;
	offsetX: number = 0;
	offsetY: number = 0;
	connectedShapes: Array<ScalableAnimation> = [];

	constructor(x: number, y: number, lifeSpanMs: number = -1) {
		super(x, y, lifeSpanMs);
		this.text = 'test';
		this.fontColor = '#ffffff';
		this.outlineColor = '#000000';
		this.outlineThickness = 0.5;
		this.fontSize = 18;
		this.opacity = 1;
	}

	getVerticalThrust(now: number): number {
		return this.verticalThrust;
	}

	getHorizontalThrust(now: number): number {
		return this.horizontalThrust;
	}

	render(context: CanvasRenderingContext2D, now: number) {
		let thisScale: number = this.getScale(now);
		let scaledFontSize: number = this.fontSize * thisScale;
		context.font = `${scaledFontSize}px ${this.fontName}`; //`-- <- Fixes memory leak
		context.textAlign = this.textAlign;
		context.textBaseline = this.textBaseline;
		context.fillStyle = this.fontColor;
		context.globalAlpha = this.getAlpha(now) * this.opacity;
		context.strokeStyle = this.outlineColor;
		context.lineWidth = this.outlineThickness * thisScale;
		context.lineJoin = "round";
		context.strokeText(this.text, this.x + this.offsetX, this.y + this.offsetY);
		context.fillText(this.text, this.x + this.offsetX, this.y + this.offsetY);

		if (this.connectedShapes.length > 0) {
			let width: number = context.measureText(this.text).width;
			let height: number = scaledFontSize;
			let top: number;
			let left: number;
			if (this.textAlign == 'left')
				left = this.x;
			else if (this.textAlign == 'center')  //  || this.textAlign === 'justify'
				left = this.x - width / 2;
			else if (this.textAlign == 'right')
				left = this.x - width;

			if (this.textBaseline == 'top')
				top = this.y;
			else if (this.textBaseline == 'middle')
				top = this.y - height / 2;
			else if (this.textBaseline == 'bottom')
				top = this.y - height;

			for (var i = 0; i < this.connectedShapes.length; i++) {
				this.connectedShapes[i].resizeToContent(left, top, width, height);
			}
		}

		context.globalAlpha = 1;
	}
}