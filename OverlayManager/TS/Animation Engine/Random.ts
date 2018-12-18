class Random {
  static getInt(upperBound) {
    return Math.floor(Math.random() * upperBound);
  }

  static between(lowerBound, upperBound) {
    return lowerBound + Math.floor(Math.random() * (upperBound - lowerBound));
  }
}
