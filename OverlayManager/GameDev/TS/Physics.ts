const pixelsPersMeter = 50;

class Physics {
  static metersToPixels(meters) {
    return meters * pixelsPersMeter;
  }

  static getDisplacement(time, initialVelocity, acceleration) {
    var metersTravelled = initialVelocity * time + 0.5 * acceleration * time * time;
    return metersTravelled;
  }

  static getFinalVelocity(time, initialVelocity, acceleration) {
    var finalVelocity = initialVelocity + acceleration * time;
    return finalVelocity;
  }
}