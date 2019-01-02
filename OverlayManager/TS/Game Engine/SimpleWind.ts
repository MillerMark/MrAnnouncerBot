// Wind that always applies acceleration and does not decay.
class SimpleWind extends Force {
    private _density: number = 1.229;
    private _force: Vector | null;

    constructor(position: Vector, private _windVelocity: Vector) {
        super(undefined, position);
    }

    get windVelocity() { return this._windVelocity; }

    set windVelocity(velocity: Vector) {
        this._windVelocity = velocity;
        this._force = null;
    }

    get density() { return this._density; }
    set density(value) { this._density = value; }

    calculateForceFor(character: WorldObject) {
        const area = 1; // Calculate proper impact area if needed.

        if (!this._force)
            this._force = Physics.calcWindForce(this.density, area, this.windVelocity);

        return this._force;
    }

}