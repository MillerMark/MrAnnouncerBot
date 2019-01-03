class World {
    /*
      Class used to manage all the forces and animated objects. Makes it easier to maintain.
    */
    private readonly _topLeft: Vector;
    private readonly _topRight: Vector;
    private readonly _bottomLeft: Vector;
    private readonly _bottomRight: Vector;
    private readonly _size: Vector;
    private readonly _center: Vector;
    private _forces: Force[] = [];
    private _characters: WorldObject[] = [];

    constructor(public readonly ctx: CanvasRenderingContext2D, x?: number, y?: number, width?: number, height?: number) {
        x = x || 0;
        y = y || 0;
        width = width || this.ctx.canvas.width;
        height = height || this.ctx.canvas.height;

        this._size = new Vector(width, height);
        this._topLeft = new Vector(x, y);
        this._topRight = new Vector(this.x + this.width - 1, this.y);
        this._bottomLeft = new Vector(this.x, this.y + this.height - 1);
        this._bottomRight = new Vector(this.x + this.width - 1, this.y + this.height - 1);
        this._center = new Vector(this.x + this.width / 2, this.y + this.height / 2);
    }

    get x() { return this._topLeft.x; }
    get y() { return this._topLeft.y; }
    get topLeft() { return this._topLeft; }
    get topRight() { return this._topRight; }
    get bottomLeft() { return this._bottomLeft; }
    get bottomRight() { return this._bottomRight; }
    get width() { return this._size.x; }
    get height() { return this._size.y; }
    get center() { return this._center; }
    get forces(): ReadonlyArray<Force> { return this._forces; }
    get characters(): ReadonlyArray<WorldObject> { return this._characters; }

    clear() {
        this._forces = [];
        this._characters = [];
    }

    addForce(force: Force) { this._forces.push(force); }
    removeForce(force: Force) { this.remove(force, this._forces); }

    // Consider notifying characters when they are added so they can have access
    // to this world's properties.  That way there is more encapsulation
    // and less code outside that needs to deal with character/world interaction.
    addCharacter(character: WorldObject) { this._characters.push(character); }
    removeCharacter(character: WorldObject) { this.remove(character, this._characters); }

    update(now: number, timeScale: number) {
        // We could remove expired forces here.
        const world = this;
        // Call pre-update on ALL characters first.
        this.characters.forEach(character => character.preUpdate(now, timeScale, world));

        this.characters.forEach(character => {
            // Apply ALL forces to A character first.
            world.forces.forEach(force => force.applyForceTo(character));
            // Update character after all forces applied.
            character.update(now, timeScale, world);
        });

        this.characters.forEach(character => character.postUpdate(now, timeScale, world));
    }

    render(now: number, timeScale: number) {
        const world = this;
        this.characters.forEach(character => character.render(now, timeScale, world));
    }

    // Can add this to Array.prototype so it's not reimplemented everywhere.
    private remove<T>(item: T, list: T[]) {
        const index = list.indexOf(item);

        if (index >= 0)
            list.splice(index, 1);
    }
}
