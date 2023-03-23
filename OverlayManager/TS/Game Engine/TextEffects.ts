class TextEffect extends ScalableAnimation {
	text: string;
	fontColor: string;
	fontName: string;
	outlineColor: string;
	verticalThrust = 0;
	horizontalThrust = 0;
	textAlign: CanvasTextAlign = 'center';
	textBaseline: CanvasTextBaseline = 'middle';
	outlineThickness: number;
	fontSize: number;
	offsetX = 0;
	offsetY = 0;
	connectedShapes: Array<ScalableAnimation> = [];

	static readonly defaultFontSize: number = 18;

	constructor(x: number, y: number, lifeSpanMs = -1) {
		super(x, y, lifeSpanMs);
		this.text = 'test';
		this.fontColor = '#ffffff';
		this.outlineColor = '#000000';
		this.outlineThickness = 0.5;
		this.fontSize = TextEffect.defaultFontSize;
		this.opacity = 1;
	}

	getVerticalThrust(now: number): number {
		return this.verticalThrust;
	}

	getHorizontalThrust(now: number): number {
		return this.horizontalThrust;
	}

	render(context: CanvasRenderingContext2D, now: number) {
		const thisScale: number = this.getScale(now);
		const scaledFontSize: number = this.fontSize * thisScale;
		context.font = `${scaledFontSize}px ${this.fontName}`; //`-- <- Fixes memory leak
		context.textAlign = this.textAlign;
		context.textBaseline = this.textBaseline;
		context.fillStyle = this.fontColor;
		context.globalAlpha = this.getAlpha(now) * this.opacity;
		context.strokeStyle = this.outlineColor;
		context.lineWidth = this.outlineThickness * thisScale;
		context.lineJoin = "round";
		const x: number = this.x + this.offsetX;
		const y: number = this.y + this.offsetY;
		if (context.lineWidth > 0)
			context.strokeText(this.text, x, y);
		//console.log(`drawing "${this.text}" at (${x}, ${y})`);
		context.fillText(this.text, x, y);

		if (this.connectedShapes.length > 0) {
			const width: number = context.measureText(this.text).width;
			const height: number = scaledFontSize;
			let top: number;
			let left: number;
			if (this.textAlign === 'left')
				left = this.x;
			else if (this.textAlign === 'center')  //  || this.textAlign === 'justify'
				left = this.x - width / 2;
			else if (this.textAlign === 'right')
				left = this.x - width;

			if (this.textBaseline === 'top')
				top = this.y;
			else if (this.textBaseline === 'middle')
				top = this.y - height / 2;
			else if (this.textBaseline === 'bottom')
				top = this.y - height;

			for (let i = 0; i < this.connectedShapes.length; i++) {
				this.connectedShapes[i].resizeToContent(left, top, width, height);
			}
		}

		context.globalAlpha = 1;
	}
}