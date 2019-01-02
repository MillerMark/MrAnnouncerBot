enum TargetBinding {
  truncate,
  wrap
}

class TargetValue {
  absoluteVariance: number;
  binding: TargetBinding = TargetBinding.truncate;
  constructor(public target: number, public relativeVariance: number = 0, public lowBounds: number = 0, public highBounds: number = Infinity) {

  }
  getValue(): number {
    var result: number;
    if (this.absoluteVariance)
      result = Random.getVarianceAbsolute(this.target, this.absoluteVariance);
    else
      result = Random.getVarianceRelative(this.target, this.relativeVariance);
    if (this.binding === TargetBinding.truncate)
      return MathEx.truncate(result, this.lowBounds, this.highBounds);
    else
      return MathEx.wrap(result, this.lowBounds, this.highBounds);
  }
}