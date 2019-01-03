class RelativeGravity extends Force {
    constructor(position: Vector, public gravity: number) {
        super(undefined, position);
    }

    calculateForceFor(character: WorldObject) {
        let relativeGravity: Vector = this.position.subtract(character.position).normalize(this.gravity);
        return relativeGravity;
    }
}
