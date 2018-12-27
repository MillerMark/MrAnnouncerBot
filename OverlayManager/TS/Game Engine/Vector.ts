class Vector {
  constructor(public readonly x: number, public readonly y: number) {
		
  }

  add(vector: Vector): Vector {
  	return new Vector(this.x + vector.x, this.y + vector.y);
  }

  static fromPolar(degrees: number, distance: number) {
    // 1° × π/180 
    var radians: number = degrees * Math.PI / 180;

    /* 
     x = r × cos( θ )
     y = r × sin( θ )
    */
    return new Vector(distance * Math.cos(radians), distance * Math.sin(radians));
  }
}