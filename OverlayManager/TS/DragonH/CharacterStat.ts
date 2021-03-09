﻿class CharacterStat {
  constructor(public name: string, public x: number, public y: number,
    public fontSize: number, public maxWidth: number, public textAlign: TextAlign = TextAlign.center, public textDisplay: TextDisplay = TextDisplay.normal) {

  }

  render(context: CanvasRenderingContext2D, activeCharacter: Character, topData: number, bottomData: number): any {
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
      this.fillCircle(value, context);
      return;
    }

    this.setAlignment(context);

    const fontSize: number = this.getFontSize();

    context.font = fontSize + 'px Calibri';
    context.textBaseline = 'middle';

    const valueStr: string = this.applyStyleGetValueStr(value, context, fontSize);

    if (valueStr !== null) {
      context.fillText(valueStr, this.x, this.y);
    }
  }

  private applyStyleGetValueStr(value: string | number, context: CanvasRenderingContext2D, fontSize: number): string {
    let valueStr: string;
    if (this.textDisplay === TextDisplay.plusMinus) {
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
      if (this.textDisplay === TextDisplay.deemphasizeZero) {
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
      else {
        context.fillStyle = '#3a1f0c';
      }
      if (this.textDisplay === TextDisplay.autoSize || this.textDisplay === TextDisplay.deemphasizeZero) {
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
    if (this.textDisplay === TextDisplay.autoSize) {
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