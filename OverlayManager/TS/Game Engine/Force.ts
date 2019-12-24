// Base class for forces that can be applied to animated objects.
class Force {
	private _isActive: boolean = true;
	private _isExpired: boolean = false;

	constructor(
		protected _acceleration: Vector = Vector.zero,
		private _position: Vector = Vector.zero,
		private readonly _mass: number = 1) {
	}

	get isActive() { return this._isActive; }
	set isActive(value) { this._isActive = value; }
	// Allow ability to determine if force should be removed from any collections.
	get isExpired() { return this._isExpired; }
	set isExpired(value) { this._isExpired = value; }
	get position() { return this._position; }
	set position(value) { this._position = value; }
	get mass() { return this._mass; }
	get acceleration() { return this._acceleration; }

	// Most forces will depend on the mass of the object it is applied to.
	// Probably will want to allow for calculation of whether or not the character
	// is in range of the force.  Gravity is everywhere (most of the time) but a
	// fan (or wind in general) may only affect characters in range.
	// f = ma
	calculateForceFor(character: WorldObject) {
		return this.acceleration.multiply(character.mass);
	}

	applyForceTo(character: WorldObject) {
		if (!this.isActive || this.isExpired) return;

		character.applyForce(this);
	}
}

