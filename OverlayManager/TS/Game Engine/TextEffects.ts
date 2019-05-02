class TextEffects {
  textEffects: Array<TextEffect> = new Array<TextEffect>();
  constructor() {

  }

  clear() {
    this.textEffects = new Array<TextEffect>();
  }

  add(centerPos: Vector, text: string): TextEffect {
    let textEffect: TextEffect = new TextEffect();
    textEffect.center = centerPos;
    textEffect.text = text;
    this.textEffects.push(textEffect);
    return textEffect;
  }

  render(context: CanvasRenderingContext2D, now: number) {
    this.textEffects.forEach(function (textEffect: TextEffect) {
      textEffect.render(context, now);
    });
  }
}

class TextEffect {
  text: string;
  fontColor: string;
  fontName: string;
  outlineColor: string;
  outlineThickness: number;
  center: Vector;
  fontSize: number;
  opacity: number;
  scale: number;

  constructor() {
    this.text = 'test';
    this.fontColor = '#ffffff';
    this.outlineColor = '#000000';
    this.outlineThickness = 1;
    this.center = new Vector(960, 540);
    this.fontSize = 18;
    this.opacity = 1;
    this.scale = 1;
  }

  render(context: CanvasRenderingContext2D, now: number) {
    const yOffset: number = 60;
    context.font = `${this.fontSize * this.scale}px ${this.fontName}`;
    context.textAlign = 'center';
    context.textBaseline = 'middle';
    context.fillStyle = this.fontColor;
    context.globalAlpha = this.opacity;
    context.strokeStyle = this.outlineColor;
    context.lineWidth = this.outlineThickness * this.scale * 2; // Half the stroke is outside the font.
    context.lineJoin = "round";
    context.strokeText(this.text, this.center.x, this.center.y + yOffset);
    context.fillText(this.text, this.center.x, this.center.y + yOffset);
    context.globalAlpha = 1;
  }
}

