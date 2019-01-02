class Vector {
  static readonly zero: Vector = new Vector(0, 0);
  // Suggestion: The term for this is magnitude. Probably best to use that or mag.
  // The magnitude is typically calculated using the dot product. That is one of
  // the key operations when working with vectors. Probably best to implement
  // that (basically the same as length but allowing two vectors to be dotted). It
  // will make it easier when looking up information/formulas on the web.
  length: number;

  constructor(public readonly x: number, public readonly y: number) {
    this.length = Math.sqrt(this.x * this.x + this.y * this.y);
  }

  add(vector: Vector): Vector {
    return new Vector(this.x + vector.x, this.y + vector.y);
  }

  subtract(vector: Vector): Vector {
    return new Vector(this.x - vector.x, this.y - vector.y);
  }

  multiply(factor: number): Vector {
    return new Vector(this.x * factor, this.y * factor);
  }

  divide(factor: number): Vector {
    // Dividing by zero is undefined.  For our purposes, we return the original instance.
    return factor !== 0 ? new Vector(this.x / factor, this.y / factor) : this;
  }

  // Suggestion: The term for this is "normalize" (and mult).  Typically, this is separated into two
  // operations. The first gets a normalized vector (the this.x, this.y / magnitude portion) and
  // the second is the multiplication. You can combine the operations but you should return a vector.
  // This is another key operation of vectors.  A lot of formulas call for adjusting the magnitude.
  // As before, it is probably best to stick with accepted conventions to make it easier to get
  // help on the web.
  getXComponent(amount: number): number {
    return amount * this.x / this.length;
  }

  getYComponent(amount: number): number {
    return amount * this.y / this.length;
  }

  normalize() {
    return this.divide(this.length);
  }

  // Feels icky putting this here... oh well :)
  metersToPixels() {
    return new Vector(Physics.metersToPixels(this.x), Physics.metersToPixels(this.y));
  }

  // Suggestion: Every math function works with radians. To be consistent, I would use radians
  // here and then create another function such as "fromPolarDegrees" that just calls this one
  // with the degrees converted to radians. Other option, "fromRadians", "fromDegrees" and make
  // make the distance (magnitude) parameter optional. It could get confusing if this is mixed
  // with other operations that expect radians. With "degrees" in the name, it makes it clear.
  static fromPolar(degrees: number, distance: number) {
    // 1° × π/180 
    var radians: number = degrees * Math.PI / 180;

    /* 
     x = r × cos( θ )
     y = r × sin( θ )
    */
    return new Vector(distance * Math.cos(radians), distance * Math.sin(radians));
  }

   toString() {
    return `(${this.x.toFixed(3)}, ${this.y.toFixed(3)})`;
  }
}