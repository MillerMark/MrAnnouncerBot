class CircularParticleGenerator extends ParticleGenerator {
  constructor(public radius: number) {
    super();
  }

  getNewParticlePosition(position: Vector, emitterVelocity: Vector, particleInitialVelocity: TargetValue, initialParticleDirection: Vector): PositionPlusVelocity {
    const nonZeroOffset: number = 0.0001;

    let particleDistance: number = Random.between(this.radius * (1 - this.edgeSpread), this.radius) + nonZeroOffset;
    let offset: Vector = Vector.fromPolar(Random.max(360), particleDistance);

    let initialVelocity: Vector = this.getInitialVelocity(offset, emitterVelocity, particleInitialVelocity, initialParticleDirection);

    return new PositionPlusVelocity(position.add(offset), initialVelocity);
  }
}