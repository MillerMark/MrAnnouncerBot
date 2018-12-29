class Vector {
  static readonly zero: Vector = new Vector(0, 0);
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

  getXComponent(amount: number): number {
    return amount * this.x / this.length;
  }

  getYComponent(amount: number): number {
    return amount * this.y / this.length;
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