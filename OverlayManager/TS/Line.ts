class Line {
  constructor(public p1: Point, public p2: Point) {
  }

  static fromCoordinates(x1: number, y1: number, x2: number, y2: number) {
    return new Line(new Point(x1, y1), new Point(x2, y2));
  }

  rise(): number {
    return this.p2.y - this.p1.y;
  }

  run(): number {
    return this.p2.x - this.p1.x;
  }
}