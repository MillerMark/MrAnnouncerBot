var pixelsPerMeter = 50;

// All **time units** will be in **seconds**...
// All **distance units** will be in **meters**...
class Physics {
  constructor() {

  }

  static metersToPixels(meters: number): number {
    return meters * pixelsPerMeter;
  }

  static pixelsToMeters(pixels: number): any {
    return pixels / pixelsPerMeter;
  }

  static getDisplacement(time: number, initialVelocity: number, acceleration: number): number {
    return initialVelocity * time + acceleration * time * time / 2;
  }

  static getFinalVelocity(time: number, initialVelocity: number, acceleration: number): number {
    return initialVelocity + acceleration * time;
  }

  static getDropTime(heightMeters: number, acceleration: number) {
    return Math.sqrt(2 * heightMeters / acceleration)
  }
}