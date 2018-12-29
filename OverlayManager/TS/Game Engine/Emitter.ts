class Emitter {
  particles: Array<Particle> = new Array<Particle>();
  radius: number;
  opacity: number;
  color: HueSatLight;
  particleFadeInTime: number;
  particleRadius: number;
  particleLifeSpanSeconds: number;
  particleRadiusVariance: number;
  particlesPerSecond: number;
  lastParticleCreationTime: number;
  particleGravity: number;
  particleGravityCenter: Vector;

  constructor(public position: Vector, public velocity: Vector = Vector.fromPolar(0, 0)) {
    this.particleFadeInTime = 0.4;
    this.radius = 10;
    this.particleRadius = 1;
    this.particleRadiusVariance = 0;
    this.particlesPerSecond = 500;
    this.opacity = 1,
    this.color = HueSatLight.fromHex('f00');
    this.particleLifeSpanSeconds = 5;
    this.particleGravity = gravityGames.activePlanet.gravity;
    this.particleGravityCenter = new Vector(screenCenterX, 50000);
  }

  addParticle(now: number): void {
    var particleRadius: number = this.particleRadius;
    if (this.particleRadiusVariance > 0) {
      var halfRange: number = particleRadius * this.particleRadiusVariance;
      particleRadius = Random.between(particleRadius - halfRange, particleRadius + halfRange);
    }

    let offset: Vector = Vector.fromPolar(Random.max(360), Random.max(this.radius));
    let particlePosition: Vector = this.position.add(offset);

    // TODO: Consider ensuring velocity is along same vector as offset.
    const maxVelocityMps: number = 5;

    // this.velocity.add(offset.multiply(maxVelocityMps/this.radius))
    this.particles.push(new Particle(this, now, particlePosition, this.velocity, particleRadius));
  }

  addParticles(now: number, amount: number) {
    this.updateVelocity(now);
    let particlesToCreate: number = Math.floor(amount);
    if (particlesToCreate === 0)
      return;

    for (var i = 0; i < particlesToCreate; i++) {
      this.addParticle(now);
    }
    this.lastParticleCreationTime = now;
  }

  updateVelocity(now: number): void {
    // TODO: Implement this based on now.
  }

  update(now: number, secondsSinceLastUpdate: number): void {
    let secondsSinceLastParticleCreation: number = (now - this.lastParticleCreationTime || now) / 1000;
    let particlesToCreate: number = this.particlesPerSecond * secondsSinceLastParticleCreation;
    this.addParticles(now, particlesToCreate);

    this.particles.forEach(function (particle: Particle) {
      particle.update(now, secondsSinceLastUpdate);
    }, this);



    this.removeExpiredParticles(now);
  }

  private removeExpiredParticles(now: number) {
    for (var i = this.particles.length - 1; i >= 0; i--) {
      let particle: Particle = this.particles[i];
      if (particle.hasExpired(now))
        this.particles.splice(i, 1);
    }
  }

  draw(context: CanvasRenderingContext2D, now: number): void {
    this.particles.forEach(function (particle: Particle) {
      particle.draw(context, now);
    }, this);
  }
}