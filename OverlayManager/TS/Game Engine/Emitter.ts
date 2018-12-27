class Emitter {
  lastUpdateTime: number;
  particles: Array<Particle> = new Array<Particle>();
  radius: number;
  opacity: number;
  color: HueSatLight;
  particleRadius: number;
  particleRadiusVariance: number;
  particlesPerSecond: number;

  constructor(public position: Vector, public velocity: Vector = Vector.fromPolar(0, 0)) {
    this.radius = 10;
    this.particleRadius = 1;
    this.particleRadiusVariance = 0;
    this.particlesPerSecond = 500;
    this.opacity = 1,
    this.color = HueSatLight.fromHex('f00');
  }

  addParticle(): void {
    var particleRadius: number = this.particleRadius;
    if (this.particleRadiusVariance > 0) {
      var halfRange: number = particleRadius * this.particleRadiusVariance;
      particleRadius = Random.between(particleRadius - halfRange, particleRadius + halfRange);
    }


    let offset: Vector = Vector.fromPolar(Random.max(360), Random.max(this.radius));
    let particlePosition: Vector = this.position.add(offset);
    // TODO: Consider ensuring velocity is along same vector as offset.
    this.particles.push(new Particle(particlePosition, this.velocity, particleRadius));
  }

  addParticles(amount: number) {
    for (var i = 0; i < amount; i++) {
      this.addParticle();
   }
  }

  updatePosition(now: number): void {
    this.particles.forEach(function (particle: Particle) {
      particle.updatePosition(now);
    });
    this.lastUpdateTime = now;
  }

  draw(context: CanvasRenderingContext2D, now: number): void {
    this.particles.forEach(function (particle: Particle) {
      particle.draw(context, now);
    });
  }
}