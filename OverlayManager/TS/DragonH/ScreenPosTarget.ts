class ScreenPosTarget extends VisualEffectTarget {
  center: Vector;
  constructor(centerX: number, centerY: number) {
    super();
    this.center = new Vector(centerX, centerY);
  }

  getCenter(): Vector {
    return this.center;
  }
}