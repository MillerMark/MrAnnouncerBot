enum TargetBinding {
  truncate,
  wrap,
  rock
}

class TargetValue {
  absoluteVariance: number;
  drift: number;
  binding: TargetBinding = TargetBinding.truncate;
  constructor(public target: number, public relativeVariance: number = 0, public lowBounds: number = 0, public highBounds: number = Infinity) {

  }

  getValue(): number {
    var result: number;

    if (this.absoluteVariance != undefined)
      result = Random.getVarianceAbsolute(this.target, this.absoluteVariance);
    else
      result = Random.getVarianceRelative(this.target, this.relativeVariance);

    if (this.drift != undefined) {
      this.target += this.drift;
    }

    if (this.binding === TargetBinding.truncate)
      return MathEx.truncate(result, this.lowBounds, this.highBounds);
    else if (this.binding === TargetBinding.wrap)
      return MathEx.wrap(result, this.lowBounds, this.highBounds);
    else if (this.drift != undefined) // rock
    {
      if (result < this.lowBounds) {
        result = this.lowBounds;
        this.drift = Math.abs(this.drift);
      }
      else if (result > this.highBounds) {
        result = this.highBounds;
        this.drift = -Math.abs(this.drift);
      }
      return result;
    }
  }
}