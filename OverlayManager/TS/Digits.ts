enum DigitSize {
  small,
  medium,
  large
}

class Digits {
  private _value: number;

  digits: Sprites;
  margin: number;
  constructor(public size: DigitSize, public right: number, public top: number) {
    if (size === DigitSize.large)
      this.digits = new Sprites("Numbers/Blue", 12, 0, AnimationStyle.Static);
    else if (size === DigitSize.medium)
      this.digits = new Sprites("Numbers/Small/Blue", 12, 0, AnimationStyle.Static);
    else 
      this.digits = new Sprites("Numbers/Tiny/Blue", 12, 0, AnimationStyle.Static);
    this.digits.sprites = [];
    this.digits.sprites.push(new SpriteProxy(0, right, 0));
    this._value = 0;

    if (this.size === DigitSize.large)
      this.margin = -20;
    else if (this.size === DigitSize.medium)
      this.margin = -10;
    else
      this.margin = -5;
  }

  get value(): number {
    return this._value;
  }

  set value(newValue: number) {
  const GroupingSeparatorIndex: number = 11;
    if (this._value != newValue) {
      this._value = newValue;
      this.digits.sprites = [];
      
      var digitStr: string = this._value.toString();
      var x = this.right - this.digits.spriteWidth;
      var digitWidth: number = this.digits.spriteWidth;
      var digitPlace: number = 1;
      for (var i = digitStr.length - 1; i >= 0; i--) {
        var thisDigit: number = +digitStr.charAt(i);
        this.digits.sprites.push(new SpriteProxy(thisDigit, x, this.top));
        if (digitPlace % 4 == 0)
          this.digits.sprites.push(new SpriteProxy(GroupingSeparatorIndex, x, this.top));
        x -= (digitWidth + this.margin);
        digitPlace++;
      }
    }
  }

  draw(context: CanvasRenderingContext2D) {
    let numDigits: number = this.digits.sprites.length;
    let digitWidth: number = this.digits.spriteWidth;
    let width: number = numDigits * (digitWidth + this.margin) - this.margin;
    let left: number = this.right - width;
    context.fillStyle = '#030315';
    context.fillRect(left, this.top, width, this.digits.spriteHeight);
    this.digits.draw(context, performance.now());
  }
}