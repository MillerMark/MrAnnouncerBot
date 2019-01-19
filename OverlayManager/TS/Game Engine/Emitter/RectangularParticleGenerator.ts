class RectangularParticleGenerator extends ParticleGenerator {
  private halfWidth: number;
  private halfHeight: number;
  smallerPt: Point;
  largerPt: Point;
  innerLine: Line;
  smallestDistanceIn: number;
  top: number;
  bottom: number;
  left: number;
  right: number;
  upperRect: Rect;
  leftRect: Rect;
  rightRect: Rect;
  bottomRect: Rect;
  totalArea: number;

  constructor(width: number, height: number) {
    super();
    this.width = width;
    this.height = height;
  }

  private _width: number;

  get width(): number {
    return this._width;
  }

  set width(newValue: number) {
    this._width = newValue;
    this.calculateFields();
  }

  private _height: number;

  private calculateFields() {
    this.halfWidth = this._width / 2;
    this.halfHeight = this._height / 2;

    let yDistance: number = 0;
    let xDistance: number = 0;

    if (this.height < this.width) {
      // Short and fat.
      xDistance = this.halfWidth - this.halfHeight;
      this.smallestDistanceIn = this.halfHeight;
    }
    else if (this.height > this.width) {
      // Tall and thin.
      yDistance = this.halfHeight - this.halfWidth;
      this.smallestDistanceIn = this.halfWidth;
    }
    else {
      // square
      this.smallestDistanceIn = this.halfHeight;
    }

    this.smallerPt = new Point(-xDistance, -yDistance);
    this.largerPt = new Point(xDistance, yDistance);

    this.innerLine = new Line(this.smallerPt, this.largerPt);

    this.top = 0 - this.halfHeight;
    this.bottom = this.halfHeight;
    this.left = 0 - this.halfWidth;
    this.right = this.halfWidth;
  }

  get height(): number {
    return this._height;
  }

  set height(newValue: number) {
    this._height = newValue;
    this.calculateFields();
  }

  protected getInitialVelocity(offset: Vector, emitterVelocity: Vector, particleInitialVelocity: TargetValue, initialParticleDirection: Vector = Vector.zero) {
    let velocityOffset: Vector = initialParticleDirection;

    if (velocityOffset.length == 0) {
      let closestPointOnLine: Point = this.innerLine.getClosestPointOnLine(offset.toPoint())
      if (closestPointOnLine.x < this.smallerPt.x || closestPointOnLine.y < this.smallerPt.y)
        closestPointOnLine = this.smallerPt;

      if (closestPointOnLine.x > this.largerPt.x || closestPointOnLine.y > this.largerPt.y)
        closestPointOnLine = this.largerPt;

      velocityOffset = offset.subtract(closestPointOnLine.toVector());
    }

    let initialVelocity: Vector = emitterVelocity.add(velocityOffset.multiply(particleInitialVelocity.getValue() / velocityOffset.length));

    return initialVelocity;
  }
  // ![](98B88E81F7520BB924CDC62EBC1DDF87.png;;;0.02526,0.02526)

  edgeSpreadChanged() {
    let border: number = this.edgeSpread * this.smallestDistanceIn;
    let innerTop: number = this.top + border;
    let innerLeft: number = this.left + border;
    let innerBottom: number = this.bottom - border;
    let innerRight: number = this.right - border;

    this.upperRect = new Rect(this.left, this.top, this.right, innerTop);
    this.leftRect = new Rect(this.left, innerTop, innerLeft, innerBottom);
    this.rightRect = new Rect(innerRight, innerTop, this.right, innerBottom);
    this.bottomRect = new Rect(this.left, innerBottom, this.right, this.bottom);

    this.totalArea = this.upperRect.area + this.leftRect.area + this.rightRect.area + this.bottomRect.area;
  }

  getNewParticlePosition(position: Vector, emitterVelocity: Vector, particleInitialVelocity: TargetValue, initialParticleDirection: Vector): PositionPlusVelocity {
    let rectIndexer: number = Random.max(this.totalArea);
    let rectToGenerateIn: Rect;
    if (rectIndexer < this.upperRect.area)
      rectToGenerateIn = this.upperRect;
    else {
      rectIndexer -= this.upperRect.area;

      if (rectIndexer < this.leftRect.area)
        rectToGenerateIn = this.leftRect;
      else {
        rectIndexer -= this.leftRect.area;

        if (rectIndexer < this.rightRect.area)
          rectToGenerateIn = this.rightRect;
        else {
          rectToGenerateIn = this.bottomRect;
        }
      }
    }

    let offset: Vector = new Vector(Random.between(rectToGenerateIn.left, rectToGenerateIn.right),
      Random.between(rectToGenerateIn.top, rectToGenerateIn.bottom));

    let initialVelocity: Vector = this.getInitialVelocity(offset, emitterVelocity, particleInitialVelocity, initialParticleDirection);

    return new PositionPlusVelocity(position.add(offset), initialVelocity);
  }
}