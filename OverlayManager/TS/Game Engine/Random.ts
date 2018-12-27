class MathEx {
  static readonly TWO_PI: number = 2 * Math.PI;
}

class Random {
  static intMax(upperBound) {
    return Math.floor(Math.random() * upperBound);
  }

  static intBetween(lowerBound, upperBound) {
    return lowerBound + Math.floor(Math.random() * (upperBound - lowerBound));
  }

  static between(min: number, max: number): number {
    return Math.random() * (max - min) + min;
  }

  static max(max: number): number {
    return Math.random() * max;
  }
}
