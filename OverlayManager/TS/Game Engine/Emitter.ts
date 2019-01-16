class PositionPlusVelocity {
  constructor(public readonly position: Vector, public readonly velocity: Vector) {

  }
}

abstract class ParticleGenerator {
  constructor() {

  }

  private _edgeSpread: number;

  get edgeSpread(): number {
    return this._edgeSpread;
  }

  set edgeSpread(newValue: number) {
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

class CircleParticleGenerator extends ParticleGenerator {
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

class RectangularParticleGenerator extends ParticleGenerator {
  private halfWidth: number;
  private halfHeight: number;
  smallerPt: Point;
  largerPt: Point;
  innerLine: Line;
  smallestDistanceIn: number;

  constructor(width: number, height: number) {
    super();
    this.width = width;
    this.height = height;
  }

  private _width: number;

  get width(): number {
    return this._width;
  }

  set width(newValue: number) {
    this._width = newValue;
    this.calculateFields();
  }

  private _height: number;

  private calculateFields() {
    this.halfWidth = this._width / 2;
    this.halfHeight = this._height / 2;

    let yDistance: number = 0;
    let xDistance: number = 0;

    if (this.height < this.width) {
      // Short and fat.
      xDistance = this.halfWidth - this.halfHeight;
      this.smallestDistanceIn = this.halfHeight;
    }
    else if (this.height > this.width) {
      // Tall and thin.
      yDistance = this.halfHeight - this.halfWidth;
      this.smallestDistanceIn = this.halfWidth;
    }
    else {
      // square
      this.smallestDistanceIn = this.halfHeight;
    }

    this.smallerPt = new Point(-xDistance, -yDistance);
    this.largerPt = new Point(xDistance, yDistance);

    this.innerLine = new Line(this.smallerPt, this.largerPt);
  }

  get height(): number {
    return this._height;
  }

  set height(newValue: number) {
    this._height = newValue;
    this.calculateFields();
  }

  protected getInitialVelocity(offset: Vector, emitterVelocity: Vector, particleInitialVelocity: TargetValue, initialParticleDirection: Vector = Vector.zero) {
    let velocityOffset: Vector = initialParticleDirection;

    if (velocityOffset.length == 0) {
      let closestPointOnLine: Point = this.innerLine.getClosestPointOnLine(offset.toPoint())
      if (closestPointOnLine.x < this.smallerPt.x || closestPointOnLine.y < this.smallerPt.y)
        closestPointOnLine = this.smallerPt;

      if (closestPointOnLine.x > this.largerPt.x || closestPointOnLine.y > this.largerPt.y)
        closestPointOnLine = this.largerPt;

      velocityOffset = offset.subtract(closestPointOnLine.toVector());
    }

    let initialVelocity: Vector = emitterVelocity.add(velocityOffset.multiply(particleInitialVelocity.getValue() / velocityOffset.length));

    return initialVelocity;
  }

  edgeSpreadChanged() {
    // TODO: Create 4 rects based on emitterEdgeSpread to represent the area where we can add particles
  }

  getNewParticlePosition(position: Vector, emitterVelocity: Vector, particleInitialVelocity: TargetValue, initialParticleDirection: Vector): PositionPlusVelocity {
    //let innerMargin: number = Random.max(this.smallestDistanceIn * this.edgeSpread);

    // TODO: Randomly select one of the 4 rects (weighted by area), and pick a random spot there.

    let offset: Vector = new Vector(Random.max(this.width) - this.halfWidth,
      Random.max(this.height) - this.halfHeight);

    let initialVelocity: Vector = this.getInitialVelocity(offset, emitterVelocity, particleInitialVelocity, initialParticleDirection);

    return new PositionPlusVelocity(position.add(offset), initialVelocity);
  }
}


class Emitter extends WorldObject {
  particleGenerator: ParticleGenerator;
  particles: Array<Particle> = new Array<Particle>();
  opacity: number;
  particleFadeInTime: number;
  particleRadius: TargetValue;
  particleLifeSpanSeconds: number;
  particleRadiusVariance: number;
  particlesPerSecond: number;
  particleMaxOpacity: number;
  lastParticleCreationTime: number;
  particleGravity: number;
  particleGravityCenter: Vector;
  gravity: number;
  gravityCenter: Vector;
  particleInitialVelocity: TargetValue;
  initialParticleDirection: Vector;
  hue: TargetValue;
  saturation: TargetValue;
  brightness: TargetValue;
  wind: Vector;
  particleWind: Vector;
  particlesCreatedSoFar: number = 0;
  maxTotalParticles: number = Infinity;
  maxConcurrentParticles: number = 4000;
  particleMass: number = 1;
  minParticleSize: number = 0.5;
  airDensity: number = 0.025;
  particleAirDensity: number = 1;
  private stopped: boolean = false;
  stopping: boolean;
  percentParticlesToCreate: number = 1;

  constructor(position: Vector, velocity: Vector = Vector.zero) {
    super(position, velocity);
    this.particleMaxOpacity = 1;
    this.particleFadeInTime = 0.4;
    this.particleGenerator = new CircleParticleGenerator(10);
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
    this.emitterEdgeSpread = 1;
    //this.initialParticleDirection = Vector.zero;
  }

  private _radius: number;

  get radius(): number {
    return this._radius;
  }

  set radius(newValue: number) {
    this._radius = newValue;
    if (this.particleGenerator instanceof CircleParticleGenerator)
      this.particleGenerator.radius = this._radius;
    else {
      this.particleGenerator = new CircleParticleGenerator(this._radius);
      this.particleGenerator.edgeSpread = this.emitterEdgeSpread;
    }
  }

  private _emitterEdgeSpread: number;

  get emitterEdgeSpread(): number {
    return this._emitterEdgeSpread;
  }

  set emitterEdgeSpread(newValue: number) {
    if (this._emitterEdgeSpread === newValue) {
      return;
    }

    this._emitterEdgeSpread = newValue;
    this.particleGenerator.edgeSpread = newValue;
  }

  start(): any {
    this.particles = new Array<Particle>();
    this.stopped = false;
    this.stopping = false;
    this.percentParticlesToCreate = 1;
  }

  stop(): any {
    this.stopping = true;
  }


  setRectShape(width: number, height: number): void {
    if (this.particleGenerator instanceof RectangularParticleGenerator) {
      this.particleGenerator.width = width;
      this.particleGenerator.height = height;
    }
    else {
      this.particleGenerator = new RectangularParticleGenerator(width, height);
      this.particleGenerator.edgeSpread = this.emitterEdgeSpread;
    }
  }


  getParticleColor(): HueSatLight {
    return new HueSatLight(this.hue.getValue() / 360, this.saturation.getValue(), this.brightness.getValue());
  }

  addParticle(now: number): void {
    this.particlesCreatedSoFar++;
    var particleRadius: number = Math.max(this.particleRadius.getValue(), this.minParticleSize);

    let particleStart: PositionPlusVelocity = this.particleGenerator.getNewParticlePosition(
      this.position, this.velocity, this.particleInitialVelocity, this.initialParticleDirection);

    this.particles.push(new Particle(this, now, particleStart.position, particleStart.velocity, particleRadius, this.particleMass));
  }

  addParticles(now: number, amount: number) {
    if (this.stopped) {
      return;
    }

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

  // TODO: clean up emitter so no code called when no particles exist.
  update(now: number, timeScale: number, world: World): void {
    if (this.stopped)
      return;

    if (this.stopping) {
      this.percentParticlesToCreate -= 0.005;
      if (this.percentParticlesToCreate <= 0) {
        this.percentParticlesToCreate = 0;
        this.stopped = true;
        this.stopping = false;
        this.lastParticleCreationTime = undefined;
        super.update(now, timeScale, world);
        return;
      }
    }

    //super.update(now, timeScale, world);
    if (this.lastParticleCreationTime === undefined)
      this.lastParticleCreationTime = now;

    let secondsSinceLastParticleCreation: number = now - this.lastParticleCreationTime;
    let particlesToCreate: number = this.particlesPerSecond * secondsSinceLastParticleCreation * this.percentParticlesToCreate;
    this.addParticles(now, particlesToCreate);

    if (this.gravity != undefined) {
      let relativeGravity: Vector = this.gravityCenter.subtract(this.position).normalize(this.gravity);
      super.applyForce(new Force(relativeGravity, this.gravityCenter));
    }

    if (this.wind) {
      let relativeVelocity: Vector = this.wind.subtract(this.velocity);

      let acceleration = relativeVelocity.length * relativeVelocity.length;
      let magnitude = this.airDensity * acceleration;
      let force = relativeVelocity.normalize(magnitude);
      super.applyForce(new Force(force));
    }

    this.particles.forEach(function (particle: Particle) {
      particle.update(now, timeScale, world);
    });

    for (var i = this.particles.length - 1; i >= 0; i--) {
      let particle: Particle = this.particles[i];
      if (particle.hasExpired(now))
        this.particles.splice(i, 1);
    }
    super.update(now, timeScale, world);
  }

  render(now: number, timeScale: number, world: World): void {
    super.render(now, timeScale, world);

    this.particles.forEach(function (particle: Particle) {
      particle.render(now, timeScale, world);
    });

    //this.showparticleGeneratorDiagnostics(world);
  }

  private showparticleGeneratorDiagnostics(world: World) {
    if (this.particleGenerator instanceof RectangularParticleGenerator && this.particleGenerator.smallerPt) {
      world.ctx.beginPath();
      world.ctx.moveTo(this.position.x + this.particleGenerator.smallerPt.x, this.position.y + this.particleGenerator.smallerPt.y);
      world.ctx.lineTo(this.position.x + this.particleGenerator.largerPt.x, this.position.y + this.particleGenerator.largerPt.y);
      world.ctx.strokeStyle = '#f00';
      world.ctx.stroke();
      world.ctx.strokeStyle = '#00f';
      world.ctx.strokeRect(this.position.x - this.particleGenerator.width / 2, this.position.y - this.particleGenerator.height / 2, this.particleGenerator.width, this.particleGenerator.height);
    }
  }
}