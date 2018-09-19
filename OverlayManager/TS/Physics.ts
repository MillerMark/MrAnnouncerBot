var pixelsPerMeter = 50;

// ![Fix this code! It stinks!](4A0040990165BFD31A48E912DC8569F7.png;;;0.02010,0.02010)
// Class is awesome, but duplicated!!! 
// Consolidate it.

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