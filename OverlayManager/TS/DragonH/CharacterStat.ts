class BaseStat {
	constructor() {

	}

	render(context: CanvasRenderingContext2D, activeCharacter: Character, topData: number, bottomData: number): void {
	}
}

class BarStat extends BaseStat {
	width: number;
	height: number;
	minHsl: HueSatLight;
	maxHsl: HueSatLight;

	constructor(public propName: string, public minValue: number | string, public maxValue: number | string,
		public x1: number, public y1: number, public x2: number, public y2: number, public minColor: string, public maxColor) {
		super();
		this.width = x2 - x1;
		this.height = y2 - y1;
		this.minHsl = HueSatLight.fromHex(minColor);
		this.maxHsl = HueSatLight.fromHex(maxColor);
	}

	lastPercentComplete = -1;
	lastFillStyle: string;

	getFillStyle(percentComplete: number): string {
		if (percentComplete === this.lastPercentComplete) {
			return this.lastFillStyle;
		}

		this.lastPercentComplete = percentComplete;

		const hsl: HueSatLight = HueSatLight.blend(this.minHsl, this.maxHsl, percentComplete);

		this.lastFillStyle = hsl.toHex();

		return this.lastFillStyle;
	}

	render(context: CanvasRenderingContext2D, activeCharacter: Character, topData: number, bottomData: number): void {
		if (this.y1 <= topData || this.y2 >= bottomData) {
			return;
		}

		if (!activeCharacter) {
			return;
		}

		const minValue: number = this.getValue(this.minValue, activeCharacter);
		const maxValue: number = this.getValue(this.maxValue, activeCharacter);

		const totalDistance: number = maxValue - minValue;
		const value: number = this.getValue(this.propName, activeCharacter);
		const percentComplete: number = (value - minValue) / totalDistance;  // 0-1


		context.fillStyle = this.getFillStyle(percentComplete);


		//`![](20FA8FE153C4A7F3BB0598CD2A6F4A5F.png;;;0.03189,0.03189)


		const barHeight: number = percentComplete * this.height;
		const y: number = this.y2 - barHeight;
		context.fillRect(this.x1, y, this.width, barHeight); 
	}


	getValue(propertyValue: string | number, activeCharacter: Character): number {
		if (typeof propertyValue === 'string') {
			const value: number | string | boolean = activeCharacter.getPropValue(propertyValue);
			return +value;
		}
		return +propertyValue;
	}
}

class CharacterStat extends BaseStat {
	constructor(public name: string, public x: number, public y: number,
		public fontSize: number, public maxWidth: number, public textAlign: TextAlign = TextAlign.center, public textDisplay: TextDisplay = TextDisplay.normal) {
		super();

	}

	render(context: CanvasRenderingContext2D, activeCharacter: Character, topData: number, bottomData: number): void {
		if (this.y <= topData || this.y >= bottomData) {
			return;
		}

		if (!activeCharacter) {
			return;
		}

		const value: number | string | boolean = activeCharacter.getPropValue(this.name);

		if (value === undefined) {
			return;
		}

		if (typeof value === "boolean") {
			// TODO: Add support for double and half proficiency mods.
			this.fillCircle(value, context);
			return;
		}

		this.setAlignment(context);

		const fontSize: number = this.getFontSize();

		context.font = fontSize + 'px Calibri';
		context.textBaseline = 'middle';

		const valueStr: string = this.applyStyleGetValueStr(value, context, fontSize);

		if (valueStr !== null) {
			if (valueStr && (this.textDisplay & TextDisplay.eraseBackground) === TextDisplay.eraseBackground) {
				const saveStyle: string | CanvasGradient | CanvasPattern = context.fillStyle;
				context.fillStyle = '#e2d2b6';
				context.fillRect(this.x, this.y - this.fontSize / 2, this.maxWidth, this.fontSize);
				context.fillStyle = saveStyle;
			}

			context.fillText(valueStr, this.x, this.y);
		}
	}

	private applyStyleGetValueStr(value: string | number, context: CanvasRenderingContext2D, fontSize: number): string {
		let valueStr: string;
		if ((this.textDisplay & TextDisplay.plusMinus) === TextDisplay.plusMinus) {
			if (value > 0) {
				valueStr = '+' + value.toString();
				context.fillStyle = '#0073c0';
			}
			else if (value < 0) {
				valueStr = value.toString();
				context.fillStyle = '#c00000';
			}
			else {
				valueStr = '0';
				context.fillStyle = '#ad8557';
			}
		}
		else {
			context.fillStyle = '#3a1f0c';

			if ((this.textDisplay & TextDisplay.deemphasizeZero) === TextDisplay.deemphasizeZero) {
				valueStr = value as string;
				if (valueStr === '0')
					context.fillStyle = '#ad8557';
			}

			if (typeof value === "number") {
				valueStr = value.toLocaleString();
			}
			else {
				valueStr = value.toString();
			}
			if (value === '-') {
				context.fillStyle = '#895e45';
			}

			if ((this.textDisplay & TextDisplay.autoSize) === TextDisplay.autoSize) {
				if (this.maxWidth > 0)
					while (context.measureText(valueStr).width > this.maxWidth && fontSize > 7) {
						fontSize--;
						context.font = fontSize + 'px Calibri';
					}
			}
		}
		return valueStr;
	}

	private getFontSize() {
		let fontSize: number;
		if ((this.textDisplay & TextDisplay.autoSize) === TextDisplay.autoSize) {
			fontSize = Math.min(this.fontSize * 2, 32);
			if (fontSize === 0)
				fontSize = 32;
		}
		else {
			fontSize = this.fontSize * 2; // Font sizes are taken from PowerPoint, which has a world that is 960 wide. We're on a 1920 screen, so we need to multiply by two.
		}
		return fontSize;
	}

	private setAlignment(context: CanvasRenderingContext2D) {
		if (this.textAlign === TextAlign.center) {
			context.textAlign = 'center';
		}
		else {
			context.textAlign = 'left';
		}
	}

	private fillCircle(value: boolean, context: CanvasRenderingContext2D) {
		if (value) {
			context.beginPath();
			context.arc(this.x, this.y, this.fontSize, 0, MathEx.TWO_PI);
			context.fillStyle = '#714b1f';
			context.fill();
		}
	}
}

