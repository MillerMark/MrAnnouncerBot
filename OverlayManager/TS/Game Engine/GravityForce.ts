class GravityForce extends Force {
    constructor(gravityConstant: number) {
        super(new Vector(0, gravityConstant));
    }

    get gravityConstant() { return this.acceleration.y; }
}
