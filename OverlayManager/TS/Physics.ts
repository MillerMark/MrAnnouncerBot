var pixelsPerMeter = 50;

//` ![](B0F27E859A79813CD50EFC3F4592B199.png;;;0.02904,0.02904)
 // Class is awesome, but **duplicated**!!! 
 // **Consolidate it.**

class Physics {
  constructor() {

  }

  static metersToPixels(meters: number): number {
    return meters * pixelsPerMeter;
  }

  // All time units will be in seconds...
  // All distance units will be in meters...
  static getDisplacement(time: number, initialVelocity: number, acceleration: number): number {
    return initialVelocity * time + acceleration * time * time / 2;
  }

  static getFinalVelocity(time: number, initialVelocity: number, acceleration: number): number {
    return initialVelocity + acceleration * time;
  }
}