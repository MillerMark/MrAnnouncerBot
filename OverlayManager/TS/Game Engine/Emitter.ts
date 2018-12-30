class Emitter extends WorldObject {
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

  constructor(position: Vector, velocity: Vector = Vector.zero) {
    super(position, velocity);
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
    let particlesToCreate: number = Math.floor(amount);
    if (particlesToCreate === 0)
      return;

    for (var i = 0; i < particlesToCreate; i++) {
      this.addParticle(now);
    }
    this.lastParticleCreationTime = now;
  }

  //updateVelocity(now: number): void {
  //  // TODO: Implement this based on now.
  //}

  applyForce(force: Force) {
    // Temporarily disable movement (gravity).
    //super.applyForce(force);
    this.particles.forEach(particle => particle.applyForce(force));
  }

  preUpdate(now: number, timeScale: number, world: World): void {
    super.update(now, timeScale, world);
    this.particles.forEach(particle => particle.preUpdate(now, timeScale, world));
  }

  // No physics logic here :) Descendants can just focus on what they do!
  update(now: number, timeScale: number, world: World): void {
    let secondsSinceLastParticleCreation: number = now - this.lastParticleCreationTime || now;
    let particlesToCreate: number = this.particlesPerSecond * secondsSinceLastParticleCreation;
    this.addParticles(now, particlesToCreate);

    this.particles.forEach(function (particle: Particle) {
      particle.update(now, timeScale, world);
    });


    this.removeExpiredParticles(now);
    super.update(now, timeScale, world);
  }

  private removeExpiredParticles(now: number) {
    for (var i = this.particles.length - 1; i >= 0; i--) {
      let particle: Particle = this.particles[i];
      if (particle.hasExpired(now))
        this.particles.splice(i, 1);
    }
  }

  render(now: number, timeScale: number, world: World): void {
    super.render(now, timeScale, world);

    this.particles.forEach(function (particle: Particle) {
      particle.render(now, timeScale, world);
    });
  }
}