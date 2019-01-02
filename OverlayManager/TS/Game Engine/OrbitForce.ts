class OrbitForce extends Force {
    // Universal gravity constant... should be in Physics.ts. (I just picked an arbritraty number)
    private static readonly G = 0.3;

    constructor(
        position: Vector,
        mass: number,
        private readonly _minDistance: number,
        private readonly _maxDistance: number) {
        super(undefined, position, mass);
    }

    calculateForceFor(character: WorldObject) {
        // Gravitational attraction formula: F = ((G * m1 * m2) / r^2) * direction between the two
        // F  = force
        // G  = universal gravity constant
        // m1 = mass of object 1
        // m2 = mass of object 2
        // r  = distance between the two objects

        // Get the direction.
        let direction = this.position.subtract(character.position);

        // Get the distance between the two.
        let distance = direction.length;

        // Make it so we are always attracting... Alternatively, we could ignore objects out of range.
        distance = Math.max(distance, this._minDistance);
        distance = Math.min(distance, this._maxDistance);
        const distanceSquared = distance * distance;
        
        // Calculate strength.
        const strength = (OrbitForce.G * this.mass * character.mass) / distanceSquared;

        // Scale the direction vector to have a magnitude equal to strength.
        const force = direction.normalize().multiply(strength);


        return force;
    }
}
