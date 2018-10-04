 //` ![](204DC0A5D26C752B4ED0E8696EBE637B.png)
class Drone extends SpriteProxy {
  displayName: string;
  userId: string;
  backgroundColor: string = '#fff';
  outlineColor: string = '#888';
  height: number;
  width: number;


  private _color: string;
  
  get color(): string {
  	return this._color;
  }
  
  set color(newValue: string) {
    if (this._color === newValue)
      return;

    let hsl: HueSatLight = HueSatLight.fromHex(newValue);
    let outlineHsl: HueSatLight = HueSatLight.clone(hsl);
    if (hsl.isBright()) {
      hsl.light = 0.02;
      outlineHsl.light = 0.08;
    }
    else {
      hsl.light = 0.98;
      outlineHsl.light = 0.92;
    }

    this.backgroundColor = hsl.toHex();
    this.outlineColor = outlineHsl.toHex();
  	this._color = newValue;
  }
  constructor(startingFrameNumber: number, public x: number, public y: number) {
    super(startingFrameNumber, x, y);
  }

  getHorizontalThrust(): number {
    return 1;
  }

  getVerticalThrust(): number {
    return -1;
  }

  drawAdornments(context: CanvasRenderingContext2D, now: number): void {
    const fontSize: number = 14;
    context.font = fontSize + 'px Arial';
    
    context.textAlign = 'center';
    context.textBaseline = 'top'; //` ![](774083667316C80C98D43F9C370CC1C8.png;;0,68,400,130)

    let centerX: number = this.x + this.width / 2;
    let yTop: number = this.y + this.height * 1;
    let size = context.measureText(this.displayName);
    let halfWidth: number = size.width / 2;
    context.fillStyle = this.backgroundColor;
    const outlineSize: number = 3;
    context.fillRect(centerX - halfWidth - outlineSize, yTop - outlineSize, size.width + outlineSize * 2, fontSize + outlineSize * 2);

    context.strokeStyle = this.outlineColor;
    context.lineWidth = 2;
    context.strokeRect(centerX - halfWidth - outlineSize, yTop - outlineSize, size.width + outlineSize * 2, fontSize + outlineSize * 2);


    context.fillStyle = this.color;
    context.fillText(this.displayName, centerX, yTop);
  }

  matches(matchData: string): boolean {
    return this.userId == matchData;
  }
}
