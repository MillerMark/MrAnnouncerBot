class TextEffects {
  textEffects: Array<TextEffect> = new Array<TextEffect>();
  constructor() {

  }

  clear() {
    this.textEffects = new Array<TextEffect>();
  }

  add(centerPos: Vector, text: string, lifeSpanMs: number = -1): TextEffect {
    let textEffect: TextEffect = new TextEffect(centerPos.x, centerPos.y, lifeSpanMs);
    textEffect.text = text;
    this.textEffects.push(textEffect);
    return textEffect;
  }

  render(context: CanvasRenderingContext2D, now: number) {
    this.textEffects.forEach(function (textEffect: TextEffect) {
      textEffect.render(context, now);
    });
  }

  removeExpiredText(now: number): void {
    for (var i = this.textEffects.length - 1; i >= 0; i--) {
      let textEffect: TextEffect = this.textEffects[i];
      if (textEffect.expirationDate) {
        if (!textEffect.stillAlive(now)) {
          textEffect.destroying();
          textEffect.removing();
          if (!textEffect.isRemoving)
            this.textEffects.splice(i, 1);
        }
      }
    }
  }

  updatePositions(now: number): void {
    for (var i = this.textEffects.length - 1; i >= 0; i--) {
      let textEffect: TextEffect = this.textEffects[i];
      textEffect.updatePosition(now);
    }
  }
}

class TextEffect extends AnimatedElement {
	elasticIn: boolean;
  text: string;
  fontColor: string;
  fontName: string;
	outlineColor: string;
	textAlign: string = 'center';
	textBaseline: string = 'middle';
  outlineThickness: number;
  fontSize: number;
  scale: number;
  targetScale: number = -1;
  originalScale: number;
  deltaScale: number = undefined;
  waitToScale: number = 0;


  constructor(x: number, y: number, lifeSpanMs: number = -1) {
    super(x, y, lifeSpanMs);
    this.text = 'test';
    this.fontColor = '#ffffff';
    this.outlineColor = '#000000';
    this.outlineThickness = 0.5;
    this.fontSize = 18;
    this.opacity = 1;
    this.scale = 1;
    this.originalScale = -1;
  }

  getVerticalThrust(now: number): number {
    return 0;
  }

  lerp(start: number, end: number, percentComplete: number) {
    return start + (end - start) * percentComplete;
  }

  lerpc(start: number, change: number, percentComplete: number) {
    return start + change * percentComplete;
	}

	static inElastic(t: number) {
		const inverseSpeed: number = 0.02;  // Smaller == faster.
		return t === 0 ? 0 : t === 1 ? 1 : (inverseSpeed - inverseSpeed / t) * Math.sin(25 * t) + 1;
	}

  render(context: CanvasRenderingContext2D, now: number) {
    let thisScale: number = this.scale;
    if (this.targetScale >= 0) {

      if (this.originalScale < 0)
        this.originalScale = this.scale;

      if (this.deltaScale === undefined)
        this.deltaScale = this.targetScale - this.originalScale;

      let scaleStartTime: number = this.timeStart + this.waitToScale;
      let timePassed: number = now - scaleStartTime;

      if (timePassed > 0) {
        let lifespan: number = this.expirationDate - scaleStartTime;

        let percentComplete: number;
        if (lifespan == 0)
          percentComplete = 1;
        else
          percentComplete = Math.max(Math.min(1, timePassed / lifespan), 0);

				let easedPercent: number;
				if (this.elasticIn)
					easedPercent = TextEffect.inElastic(percentComplete);
				else
					easedPercent = percentComplete;

				thisScale = this.lerpc(this.originalScale, this.deltaScale, easedPercent);
      }
    }

    context.font = `${this.fontSize * thisScale}px ${this.fontName}`; //`-- <- Fixes memory leak
		context.textAlign = this.textAlign;
		context.textBaseline = this.textBaseline;
    context.fillStyle = this.fontColor;
    context.globalAlpha = this.getAlpha(now) * this.opacity;
    context.strokeStyle = this.outlineColor;
    context.lineWidth = this.outlineThickness * thisScale;
    context.lineJoin = "round";
    context.strokeText(this.text, this.x, this.y);
    context.fillText(this.text, this.x, this.y);
    context.globalAlpha = 1;
  }
}

