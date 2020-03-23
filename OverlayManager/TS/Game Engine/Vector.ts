class Rect {
  area: number;
  constructor(public left: number, public top: number, public right: number, public bottom: number) {
    let width: number = Math.max(this.right - this.left, 1);
    let height: number = Math.max(this.bottom - this.top, 1);
    this.area = width * height;
	}
}

class Vector {
  static readonly zero: Vector = new Vector(0, 0);
  // Suggestion: The term for this is magnitude. Probably best to use that or mag.
  // The magnitude is typically calculated using the dot product. That is one of
  // the key operations when working with vectors. Probably best to implement
  // that (basically the same as length but allowing two vectors to be dotted). It
	// will make it easier when looking up information/formulas on the web.
	private _length: number = undefined;
	
	get length(): number {
		if (this._length === undefined) {
			this._length = Math.sqrt(this.x * this.x + this.y * this.y);
		}
		return this._length;
	}
	
	set length(newValue: number) {
		this._length = newValue;
	}

  constructor(public x: number, public y: number) {
    //this.length = Math.sqrt(this.x * this.x + this.y * this.y);
  }

  add(vector: Vector): Vector {
    return new Vector(this.x + vector.x, this.y + vector.y);
  }
  
  addRef(vector: Vector): void {
		this.x += vector.x;
		this.y += vector.y;
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

  squaredKeepSign(): Vector {
    return new Vector(this.x * this.x * Math.sign(this.x), this.y * this.y * Math.sign(this.y));
  }

  normalize(length: number): Vector {
    let newX: number = this.getRatioX(length);
    let newY: number = this.getRatioY(length);
    return new Vector(newX, newY);
  }

  getRatioX(amount: number): number {
    if (this.length == 0)
      return 0;
    return amount * this.x / this.length;
  }

  getRatioY(amount: number): number {
    if (this.length == 0)
      return 0;
    return amount * this.y / this.length;
  }

  dot(other: Vector): number {
    return this.x * other.x + this.y * other.y;
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

  toPoint(): Point{
    return new Point(this.x, this.y);
  }
 

  /* It's hard to explain but simple to implement. 
    
   if you sub line end from line start... you get a vector from start to end... 
   you then do a new vector with (-vy, vx) and normalize it... which is the normal 
   to the line... you then need the distance to the origin which is normal.dot(start)... 
   the distance to point P is then p.dot(normal) - distance to origin
   point on line is then P.sub(normal.mult(distance)) */
}