// Base class for objects to be animated.
// Could be derived from Force since objects can apply force
// to other objects.  Just keeping it simple for now.
class WorldObject {
  protected _acceleration: Vector;
  private _appliedForce: Vector;
  private readonly _inverseMass: number;
  private _priorPosition: Vector;
  private _priorVelocity: Vector;
  private _localForces: Force[];

  constructor(
    protected _position: Vector = Vector.zero,
    protected _velocity: Vector = Vector.zero,
    private readonly _mass: number = 1) {

    if (this._mass == 0)
      this._inverseMass = 0;
    else
      this._inverseMass = 1 / this._mass;
    this.resetParams();
  }

  // TODO: Add a bounds property. This will allow checking for collisions on a "large" scale.
  //       Will also allow common properties like center, topLeft, etc...
  get position() { return this._position; }
  set position(value) { this._position = value; }
  get mass() { return this._mass; }
  get inverseMass() { return this._inverseMass; }
  get acceleration() { return this._acceleration; }
  get priorPosition() { return this._priorPosition; }
  get velocity() { return this._velocity; }
  get priorVelocity() { return this._priorVelocity; }
  get localForces(): ReadonlyArray<Force> { return this._localForces || []; }

  addLocalForce(force: Force) {
    if (!this._localForces)
      this._localForces = [];

    this._localForces.push(force);
  }

  removeLocalForce(force: Force) {
    if (!this._localForces) return;

    this._localForces.remove(force);
  }

  // Adds to the currently applied force.
  // Physics: Acceleration is the sum of all forces applied to an object.
  // Taking the force instance instead of the force vector.
  // This allows screening out forces based on some criteria and debug
  // logging of which forces are being applied.
  applyForce(force: Force) {
    const forceValue = force.calculateForceFor(this);
    this._appliedForce = this._appliedForce.add(forceValue);
  }

  // Separate so descendants have more control in what to override.
  resetParams() {
    this._appliedForce = Vector.zero;
    this._acceleration = Vector.zero;

    // Update the prior values here (called from preUpdate). That way, the prior
    // and the current value will be accessible from postUpdate and render in
    // descendants.
    this._priorPosition = this.position;
    this._priorVelocity = this.velocity;
  }

  // a = f / m.  Alternatively, could call the physics engine. Multiplying by inverse mass to save a division.
  protected updateAcceleration(now: number) { this._acceleration = this._appliedForce.multiply(this.inverseMass); }

  // v = v + a.  Alternatively, could call the physics engine.
  // Using the prior value as a base here so the update can be called again if forces are added later.
  // This will allow objects to act as forces on other objects later if needed (update can be called again and it
  // will still calculate correctly using the additional applied forces).
  protected updateVelocity(now: number, acceleration: Vector) { this._velocity = this.priorVelocity.add(acceleration); }

  // Using the prior value here for the same reason as in updateVelocity.
  protected updatePosition(now: number, velocity: Vector) { this._position = this.priorPosition.add(velocity); }

  // Allows resetting variables in preparation for update.
  // Separating this from update allows descendants to only override the pieces they
  // need and inherit common behavior.  Also allows future extensibily for objects
  // to act as forces on other objects.
  preUpdate(now: number, timeScale: number, world: World) { this.resetParams(); }

  update(now: number, timeScale: number, world: World) {
    if (this._localForces)
      this._localForces.forEach(force => force.applyForceTo(this));

    // This process is called integration. There are other ways to integrate but this is the simplest.
    this.updateAcceleration(now);
    // Using the simple, straightforward translation from physics to update position by calculating
    // velocity and then updating the position. This is called "Explicit Euler".
    // More accuracy can be achieved by using "Symplectic Euler", which is simply reversing the order
    // so position is updated then velocity.
    // NOTE: We are multiplying acceleration and velocity by timeScale (frame duration). Our
    //       calculations are in whole seconds but we update subseconds (one update per frame).
    //       We therefore need to scale the values down. e.g. If we need to move 60 meters in one second
    //       and we are processing at 60 frames per second, we would move 1 meter each frame.
    this.updateVelocity(now, this.acceleration.multiply(timeScale));
    this.updatePosition(now, this.velocity.metersToPixels().multiply(timeScale));
  }

  // Allows updating any variables in preparation for the next update.
  // Same reasoning as per preUpdate.
  postUpdate(now: number, timeScale: number, world: World) { }

  // Taking timeScale here in case... e.g. for interpolation.
  render(now: number, timeScale: number, world: World) {
  }

  // TODO: Need logic for bouncing here.
}
