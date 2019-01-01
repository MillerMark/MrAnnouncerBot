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
  maxParticles: number = Infinity;

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
    this.particleGravityCenter = new Vector(screenCenterX, 999999);
    this.gravity = gravityGames.activePlanet.gravity;
    this.gravityCenter = new Vector(screenCenterX, 999999);
  }

  getParticleColor(): HueSatLight {
    return new HueSatLight(this.hue.getValue() / 360, this.saturation.getValue(), this.brightness.getValue());
  }

  addParticle(now: number): void {
    this.particlesCreatedSoFar++;
    var particleRadius: number = this.particleRadius.getValue();

    const nonZeroOffset: number = 0.0001;
    let offset: Vector = Vector.fromPolar(Random.max(360), Random.max(this.radius) + nonZeroOffset);

    let particlePosition: Vector = this.position.add(offset);

    let initialVelocity: Vector = this.velocity.add(offset.multiply(this.particleInitialVelocity.getValue() / offset.length));
    this.particles.push(new Particle(this, now, particlePosition, initialVelocity, particleRadius));
  }

  addParticles(now: number, amount: number) {
    let particlesToCreate: number = Math.floor(amount);
    if (particlesToCreate === 0)
      return;

    try {
      for (var i = 0; i < particlesToCreate; i++) {
        if (this.particlesCreatedSoFar >= this.maxParticles)
          return;
        this.addParticle(now);
      }
    }
    finally {
      this.lastParticleCreationTime = now;
    }
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

//  update(now: number, secondsSinceLastUpdate: number): void {
//    let relativeGravity = this.gravityCenter.subtract(this.position);
//    let gravityX: number = relativeGravity.getRatioX(this.gravity);
//    let gravityY: number = relativeGravity.getRatioY(this.gravity);

//    let displacementX: number = Physics.metersToPixels(Physics.getDisplacement(secondsSinceLastUpdate, this.velocity.x + this.wind.x, gravityX));
//    let displacementY: number = Physics.metersToPixels(Physics.getDisplacement(secondsSinceLastUpdate, this.velocity.y + this.wind.y, gravityY));

//    this.position = new Vector(this.position.x + displacementX, this.position.y + displacementY);

    let newVelocityX: number = Physics.getFinalVelocity(secondsSinceLastUpdate, this.velocity.x, gravityX);
    let newVelocityY: number = Physics.getFinalVelocity(secondsSinceLastUpdate, this.velocity.y, gravityY);
    this.velocity = new Vector(newVelocityX, newVelocityY);

    this.removeExpiredParticles(now);
    super.update(now, timeScale, world);
  }
//    let secondsSinceLastParticleCreation: number = (now - this.lastParticleCreationTime || now) / 1000;
//    let particlesToCreate: number = this.particlesPerSecond * secondsSinceLastParticleCreation;
//    this.addParticles(now, secondsSinceLastUpdate, particlesToCreate);

    //this.particles.forEach(function (particle: Particle) {
    //  particle.update(now, secondsSinceLastUpdate);
    //}, this);

    for (var i = this.particles.length - 1; i >= 0; i--) {
      let particle: Particle = this.particles[i];
      if (particle.hasExpired(now))
        this.particles.splice(i, 1);
      else 
        particle.update(now, secondsSinceLastUpdate);
    }

    //this.removeExpiredParticles(now);
  }

  render(now: number, timeScale: number, world: World): void {
    super.render(now, timeScale, world);

  //private removeExpiredParticles(now: number) {
  //  for (var i = this.particles.length - 1; i >= 0; i--) {
  //    let particle: Particle = this.particles[i];
  //    if (particle.hasExpired(now))
  //      this.particles.splice(i, 1);
  //  }
  //}

//  draw(context: CanvasRenderingContext2D, now: number): void {
    this.particles.forEach(function (particle: Particle) {
      particle.render(now, timeScale, world);
    });
  }
}