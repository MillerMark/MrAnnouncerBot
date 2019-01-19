abstract class ParticleGenerator {
  constructor() {

  }

  private _edgeSpread: number;

  get edgeSpread(): number {
    return this._edgeSpread;
  }

  set edgeSpread(newValue: number) {
    newValue = MathEx.clamp(newValue, 0, 1);
    if (this._edgeSpread === newValue)
      return;
    this._edgeSpread = newValue;
    this.edgeSpreadChanged();
  }

  edgeSpreadChanged() {
    // Do nothing. Let descendants override.
  }

  abstract getNewParticlePosition(position: Vector, emitterVelocity: Vector, particleInitialVelocity: TargetValue, initialParticleDirection: Vector): PositionPlusVelocity;

  protected getInitialVelocity(offset: Vector, emitterVelocity: Vector, particleInitialVelocity: TargetValue, initialParticleDirection: Vector = Vector.zero) {
    let velocityOffset: Vector = initialParticleDirection;

    if (velocityOffset.length == 0)
      velocityOffset = offset;
    let initialVelocity: Vector = emitterVelocity.add(velocityOffset.multiply(particleInitialVelocity.getValue() / velocityOffset.length));

    return initialVelocity;
  }
}