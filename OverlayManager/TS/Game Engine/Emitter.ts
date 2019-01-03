class Emitter extends WorldObject {
  particles: Array<Particle> = new Array<Particle>();
  radius: number;
  opacity: number;
  particleFadeInTime: number;
  particleRadius: TargetValue;
  particleLifeSpanSeconds: number;
  particleRadiusVariance: number;
  particlesPerSecond: number;
  lastParticleCreationTime: number;
  particleGravity: number;
  particleGravityCenter: Vector;
  gravity: number;
  gravityCenter: Vector;
  particleInitialVelocity: TargetValue;
  hue: TargetValue;
  saturation: TargetValue;
  brightness: TargetValue;
  wind: Vector;
  particleWind: Vector;
  particlesCreatedSoFar: number = 0;
  maxTotalParticles: number = Infinity;
  maxConcurrentParticles: number = 4000;
  particleMass: number = 1;
  particleVelocityDegrade: number = 0.5;
  minParticleSize: number = 0.5;
  airMass: number = 0.025;
  particleAirMass: number = 1;

  constructor(position: Vector, velocity: Vector = Vector.zero) {
    super(position, velocity);
    this.particleFadeInTime = 0.4;
    this.radius = 10;
    this.particleRadius = new TargetValue(1);
    this.particlesPerSecond = 500;
    this.opacity = 1;
    this.hue = new TargetValue(0, 0, 0, 360);
    this.hue.binding = TargetBinding.wrap;
    this.saturation = new TargetValue(1, 0, 0, 1);
    this.brightness = new TargetValue(0.5, 0, 0, 1);
    this.wind = Vector.zero;
    this.particleWind = Vector.zero;
    this.particleLifeSpanSeconds = 5;
    this.particleInitialVelocity = new TargetValue(1);
    this.particleGravity = gravityGames.activePlanet.gravity;
    this.gravityCenter = new Vector(screenCenterX, Physics.metersToPixels(gravityGames.activePlanet.diameter / 2));
    this.particleGravityCenter = this.gravityCenter;
  }

  getParticleColor(): HueSatLight {
    return new HueSatLight(this.hue.getValue() / 360, this.saturation.getValue(), this.brightness.getValue());
  }

  addParticle(now: number): void {
    this.particlesCreatedSoFar++;
    var particleRadius: number = Math.max(this.particleRadius.getValue(), this.minParticleSize);

    const nonZeroOffset: number = 0.0001;
    let offset: Vector = Vector.fromPolar(Random.max(360), Random.max(this.radius) + nonZeroOffset);

    let particlePosition: Vector = this.position.add(offset);

    let initialVelocity: Vector = this.velocity.multiply(this.particleVelocityDegrade).add(offset.multiply(this.particleInitialVelocity.getValue() / offset.length));
    this.particles.push(new Particle(this, now, particlePosition, initialVelocity, particleRadius, this.particleMass));
  }

  addParticles(now: number, amount: number) {
    let particlesToCreate: number = Math.floor(amount);
    if (this.particles.length + particlesToCreate > this.maxConcurrentParticles)
      particlesToCreate = this.maxConcurrentParticles - this.particles.length;

    if (particlesToCreate <= 0)
      return;

    try {
      for (var i = 0; i < particlesToCreate; i++) {
        if (this.particlesCreatedSoFar >= this.maxTotalParticles)
          return;
        this.addParticle(now);
      }
    }
    finally {
      this.lastParticleCreationTime = now;
    }
  }

  applyForce(force: Force) {
    if (!(force instanceof GravityForce))
      super.applyForce(force);
    this.particles.forEach(particle => particle.applyForce(force));
  }

  preUpdate(now: number, timeScale: number, world: World): void {
    super.preUpdate(now, timeScale, world);
    this.particles.forEach(particle => particle.preUpdate(now, timeScale, world));
  }

  update(now: number, timeScale: number, world: World): void {
    if (this.lastParticleCreationTime === undefined)
      this.lastParticleCreationTime = now;
    let secondsSinceLastParticleCreation: number = now - this.lastParticleCreationTime;
    let particlesToCreate: number = this.particlesPerSecond * secondsSinceLastParticleCreation;
    this.addParticles(now, particlesToCreate);

    if (this.gravity != undefined) {
      let relativeGravity: Vector = this.gravityCenter.subtract(this.position).normalize(this.gravity);
      super.applyForce(new Force(relativeGravity, this.gravityCenter));
    }

    if (this.wind)
    {
      super.update(now, timeScale, world);
      let relativeVelocity: Vector = this.wind.subtract(this.velocity);

      let acceleration = relativeVelocity.length * relativeVelocity.length;
      let magnitude = this.airMass * acceleration;
      let force = relativeVelocity.normalize(magnitude);
      super.applyForce(new Force(force));
    }

    super.update(now, timeScale, world);

    this.particles.forEach(function (particle: Particle) {
      particle.update(now, timeScale, world);
    });

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