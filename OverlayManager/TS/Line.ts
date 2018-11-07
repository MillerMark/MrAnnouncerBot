class Line {
  constructor(public p1: Point, public p2: Point) {
  }

  static fromCoordinates(x1: number, y1: number, x2: number, y2: number) {
    return new Line(new Point(x1, y1), new Point(x2, y2));
  }

  // returns true if testLine intersects with this line.
  intersectsWith(testLine: Line): boolean {
    let det: number;
    let gamma: number;
    let lambda: number;

    let bRise: number = testLine.rise();
    let aRise: number = this.rise();
    let bRun: number = testLine.run();
    let aRun: number = this.run();
    det = aRun * bRise - bRun * aRise;
    if (det === 0) {
      return false;
    } else {
      let cornerX: number = testLine.p2.x - this.p1.x;
      let cornerY: number = testLine.p2.y - this.p1.y;
      lambda = (bRise * cornerX - bRun * cornerY) / det;
      gamma = (aRun * cornerY - aRise * cornerX) / det;
      return (0 < lambda && lambda < 1) && (0 < gamma && gamma < 1);
    }
  };

  rise(): number {
    return this.p2.y - this.p1.y;
  }

  run(): number {
    return this.p2.x - this.p1.x;
  }
}