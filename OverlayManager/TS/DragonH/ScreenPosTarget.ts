class ScreenPosTarget extends VisualEffectTarget {
  center: Vector;
  constructor(center: Vector) {
    super();
    this.center = center;
  }

  getCenter(): Vector {
    return this.center;
  }
}