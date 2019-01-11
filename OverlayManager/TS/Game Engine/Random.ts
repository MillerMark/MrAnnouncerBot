class MathEx {
  static readonly TWO_PI: number = 2 * Math.PI;

  static wrap(value: number, lowBounds: number, highBounds: number): number {
    let distance: number = highBounds - lowBounds;
    if (distance <= 0)
      throw new Error('highBounds must be greater than lowBounds!');

    while (value < lowBounds)
      value += distance;

    while (value > highBounds)
      value -= distance;

    return value;
  }

  static truncate(value: number, lowBounds: number, highBounds: number): number {
    return Math.max(Math.min(value, highBounds), lowBounds);
  }
}

class Random {

  static intBetweenDigitCount(lowerBounds: number, upperBounds: number): number {
    let result: number = Random.intBetween(0, 9);
    let numDigits: number = Random.intBetween(lowerBounds, upperBounds);
    for (var i = 0; i < numDigits; i++) {
      result = result * 10 + Random.intBetween(0, 9);
    }
    return result;
  }

  static intMaxDigitCount(upperBounds: number): number {
    return Random.intBetweenDigitCount(1, upperBounds);
  }

  static intMax(upperBound) {
    return Math.floor(Math.random() * upperBound);
  }

  // inclusive:
  static intBetween(lowerBound, upperBound) {
    return lowerBound + Math.floor(Math.random() * (upperBound - lowerBound + 1));
  }

  static between(min: number, max: number): number {
    return Math.random() * (max - min) + min;
  }

  static max(max: number): number {
    return Math.random() * max;
  }

  static getVarianceRelative(target: number, percentVariance: number): number {
    if (percentVariance > 0) {
      var halfRange: number = target * percentVariance;
      return Random.getVarianceAbsolute(target, halfRange);
    }
    return target;
  }


  static getVarianceAbsolute(target: number, absoluteVariance: number) {
    if (absoluteVariance === 0)
      return target;
    let low: number = target - absoluteVariance;
    let high: number = target + absoluteVariance;
    return Random.between(low, high);
  }
}
