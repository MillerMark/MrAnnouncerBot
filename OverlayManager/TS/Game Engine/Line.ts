

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

  slope() {
    return this.rise() / this.run();
  }

  extend(length: number): any {

    //`+Extending vector (0,0 → 8,6) by length 5:
        //`![](31C4E7169A9C932DA136CC38CA674B88.png;;;0.01434,0.01434)

     //`Challenge: determine the **x** and **y** **offsets** to extend this vector by the given *length*.

     //`-Pythagorean Theorem:
     //`  x² + y² = length²

     //`-Slope equation:
     //`  y/x = rise/run = slope
     //`  y = slope * x;

     //`-Substituting y:
     //`  x² + (slope * x)² = length²

     //`-Solve for x:
     //`  x²(slope² + 1) = length²
     //`  x = Math.sqrt(length²/(slope² + 1))

    if (this.run() === 0)
      return new Line(this.p1,
        new Point(this.p2.x, this.p2.y + length * Math.sign(this.rise())));

    let slope: number = this.slope();

    let extendX: number = Math.sqrt(length * length / (slope * slope + 1)) * Math.sign(this.run());
    let extendY: number = slope * extendX;
    
    return new Line(this.p1, new Point(this.p2.x + extendX, this.p2.y + extendY));
  }

  equals(line: Line): boolean {
    return this.p1.equals(line.p1) && this.p2.equals(line.p2);
  }

  matchesCoordinates(x1: number, y1: number, x2: number, y2: number): boolean {
    return this.p1.x === x1 && this.p1.y === y1 && this.p2.x === x2 && this.p2.y === y2;
  }

  //`!────────────────────────────────────────────────────────────────────────────────────
  //`!────────────────────────────────────────────────────────────────────────────────────

  static testMatchingSlopes() {
    let line1: Line = Line.fromCoordinates(0, 0, 8, 6);
    let line2: Line = Line.fromCoordinates(0, 0, 16, 12);
    this.assertTrue(line1.slope() === line2.slope());
  }

  static testExtension() {
    let line1: Line = Line.fromCoordinates(0, 0, 8, 6);
    let line2: Line = line1.extend(5);
    this.assertTrue(line2.matchesCoordinates(0, 0, 12, 9));

    line1 = Line.fromCoordinates(0, 0, -8, 6);
    line2 = line1.extend(5);
    this.assertTrue(line2.matchesCoordinates(0, 0, -12, 9));

    line1 = Line.fromCoordinates(0, 0, 8, -6);
    line2 = line1.extend(5);
    this.assertTrue(line2.matchesCoordinates(0, 0, 12, -9));

    line1 = Line.fromCoordinates(0, 0, -8, -6);
    line2 = line1.extend(5);
    this.assertTrue(line2.matchesCoordinates(0, 0, -12, -9));
  }

  static runTests() {
    this.testMatchingSlopes();
    this.testExtension();
  }

  static assertTrue(test: boolean) {
    if (!test)
      throw new DOMException();
  }
}