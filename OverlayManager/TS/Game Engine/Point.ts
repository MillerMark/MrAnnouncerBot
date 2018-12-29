class Point {
  constructor(public x: number, public y: number) {
  }

  equals(point: Point): any {
    return this.x === point.x && this.y === point.y;
  }
}