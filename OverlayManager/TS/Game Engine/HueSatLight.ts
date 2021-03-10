 //` ![](45E77D94A1CD4CD6C4058A46BB98799C.png)

class HueSatLight {

  // blendAmount is a number between 0 and 1
  static blend(hsl1: HueSatLight, hsl2: HueSatLight, blendAmount: number): HueSatLight {
    const [r1, g1, b1] = hsl1.toRgb();
    const [r2, g2, b2] = hsl2.toRgb();

    const red: number = r1 + (r2 - r1) * blendAmount;
    const green: number = g1 + (g2 - g1) * blendAmount;
    const blue: number = b1 + (b2 - b1) * blendAmount;

    return HueSatLight.fromRgb(red, green, blue);
  }

  // hue, saturation, and light are all between 0 and 1.
  constructor(public hue: number, public saturation: number, public light: number) {

  }

  getPerceivedBrightness(): number {
    const rgb = this.toRgb();
    const red: number = rgb[0];
    const green: number = rgb[1];
    const blue: number = rgb[2];

    return Math.sqrt(red * red * 0.241 + green * green * 0.691 + blue * blue * 0.068);
  }

  isBright(): boolean {
    return this.getPerceivedBrightness() > 0.5;
  }

  isDark(): boolean {
    return this.getPerceivedBrightness() < 0.5;
  }

  toRgb(): [number, number, number] {
    let red, green, blue;

    if (this.saturation === 0) {
      red = green = blue = this.light; // achromatic
    }
    else {
      function hue2rgb(p, q, t) {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1 / 6) return p + (q - p) * 6 * t;
        if (t < 1 / 2) return q;
        if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
        return p;
      }

      const q = this.light < 0.5 ? this.light * (1 + this.saturation) : this.light + this.saturation - this.light * this.saturation;
      const p = 2 * this.light - q;

      red = hue2rgb(p, q, this.hue + 1 / 3);
      green = hue2rgb(p, q, this.hue);
      blue = hue2rgb(p, q, this.hue - 1 / 3);
    }

    return [red, green, blue];
  }

  toHex(): string {
    const rgb = this.toRgb();
    const red: number = rgb[0];
    const green: number = rgb[1];
    const blue: number = rgb[2];

    function int2hex(value: number) {
      let hex: string = Math.round(value * 255).toString(16);
      if (hex.length === 1)
        hex = '0' + hex;
      return hex;
    }

    const redHex: string = int2hex(red);
    const greenHex: string = int2hex(green);
    const blueHex: string = int2hex(blue);

    return '#' + redHex + greenHex + blueHex;
  }

  static clone(hsl: HueSatLight): HueSatLight {
    return new HueSatLight(hsl.hue, hsl.saturation, hsl.light);
  }

  static fromRgb(red: number, green: number, blue: number): HueSatLight {
    const max = Math.max(red, green, blue);
    const min = Math.min(red, green, blue);
    const sum = max + min;
    let hue, sat;
    const light = sum / 2;

    if (max === min) {
      hue = sat = 0;   // gray scale
    }
    else {
      const dif = max - min;
      sat = light > 0.5 ? dif / (2 - sum) : dif / sum;

      switch (max) {
        case red:
          hue = (green - blue) / dif + (green < blue ? 6 : 0);
          break;
        case
          green: hue = (blue - red) / dif + 2;
          break;
        case
          blue: hue = (red - green) / dif + 4;
          break;
      }

      hue /= 6;
    }

    function inbounds(value: number): number {
      return Math.max(0, Math.min(value, 1));
    }

    return new HueSatLight(inbounds(hue), inbounds(sat), inbounds(light));
  }

  static fromRgbBytes(red: number, green: number, blue: number): HueSatLight {
    red /= 255;
    green /= 255;
    blue /= 255;
    return this.fromRgb(red, green, blue);
  }

	static fromHex(hex: string) {
    // Expand shorthand form (e.g. "08f") to full form (e.g. "0088FF")
    const shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;
    hex = hex.replace(shorthandRegex, function (m, r, g, b) {
      return r + r + g + g + b + b;
    });

    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);

    if (!result)
      return null;

    let red = parseInt(result[1], 16);
    let green = parseInt(result[2], 16);
    let blue = parseInt(result[3], 16);

    return HueSatLight.fromRgbBytes(red, green, blue);
  }
}