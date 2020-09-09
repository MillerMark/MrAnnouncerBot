class MathEx {
  static readonly TWO_PI: number = 2 * Math.PI;

  static wrap(value: number, lowBounds: number, highBounds: number): number {
    const distance: number = highBounds - lowBounds;
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

  static clamp(value: number, lowBounds: number, highBounds: number): any {
    if (value < lowBounds)
      value = lowBounds;
    if (value > highBounds)
      value = highBounds;
    return value;
  }

  static toFixed(number: number, decimalPlaces: number) {
    return parseFloat(number.toFixed(decimalPlaces));
  }
}

class Random {
  static chancePercent(percent: number): boolean {
    return Random.max(100) <= percent;
  }

  static intBetweenDigitCount(lowerBounds: number, upperBounds: number): number {
    let result: number = Random.intBetween(0, 9);
    const numDigits: number = Random.intBetween(lowerBounds, upperBounds);
    for (let i = 0; i < numDigits; i++) {
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

	static plusMinusBetween(min: number, max: number): number {
		if (Random.max(100) < 50)
			return Random.between(min, max);
		else 
			return -Random.between(min, max);
  }

  static plusMinus(value: number): number {
    return -value + Math.random() * 2 * value;
  }

  static max(max: number): number {
    return Math.random() * max;
  }

  static getVarianceRelative(target: number, percentVariance: number): number {
    if (percentVariance > 0) {
      const halfRange: number = target * percentVariance;
      return Random.getVarianceAbsolute(target, halfRange);
    }
    return target;
  }


  static getVarianceAbsolute(target: number, absoluteVariance: number) {
    if (absoluteVariance === 0)
      return target;
    const low: number = target - absoluteVariance;
    const high: number = target + absoluteVariance;
    return Random.between(low, high);
  }
}
