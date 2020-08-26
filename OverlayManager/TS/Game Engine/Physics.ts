const pixelsPerMeter = 50;

// All **time units** will be in **seconds**...
// All **distance units** will be in **meters**...
class Physics {
  static metersToPixels(meters: number): number {
    return meters * pixelsPerMeter;
  }

  static pixelsToMeters(pixels: number): number {
    return pixels / pixelsPerMeter;
  }

  static getDisplacementMeters(time: number, initialVelocity: number, acceleration: number): number {

    //` <formula #ffffe0;3; d = v_i t + \frac{at^2}{2}>

    return initialVelocity * time + acceleration * time * time / 2;
  }


  static getFinalVelocityMetersPerSecond(time: number, initialVelocity: number, acceleration: number): number {

    //` <formula #ffffe0;1.5; v_f = v_i + at>

    return initialVelocity + acceleration * time;
  }


  static getDropTimeSeconds(heightMeters: number, acceleration: number, initialVelocity = 0) {

    //` <formula #ffffe0;3; t = \frac{-v_i + \sqrt{v_i^2 + 2ha}}{a}>

    return (-initialVelocity + Math.sqrt(initialVelocity * initialVelocity + 2 * heightMeters * acceleration)) / acceleration;
  }

  
}