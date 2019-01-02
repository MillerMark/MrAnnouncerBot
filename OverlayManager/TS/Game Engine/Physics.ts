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

  static getDropTimeWithVelocity(heightMeters: number, initialVelocity: number, acceleration: number) {
    //     [−vi ± √(        vi2     + 2   g              y)           ] / g
    return (-initialVelocity + Math.sqrt(initialVelocity * initialVelocity + 2 * heightMeters * acceleration)) / acceleration;
  }

  static calcWindForce(density: number, area: number, velocity: Vector) {
    // ref: https://sciencing.com/convert-wind-speed-force-5985528.html
    // air mass (Am) = density * area
    // acceleration (a) = windspeed * windspeed
    // F = Am * a
    let airMass = density * area;
    let acceleration = velocity.length * velocity.length;
    let magnitude = airMass * acceleration;
    let force = velocity.normalize(magnitude);

    return force;
  }
}