class Particle {

  lastUpdateTime: number;

  constructor(public position: Vector, public velocity: Vector = Vector.fromPolar(0, 0), public radius: number = 1, public opacity: number = 1,
    public color: HueSatLight = HueSatLight.fromHex('f00')) {
  }

  updatePosition(now: number) {
    // TODO: Implement this...

    this.lastUpdateTime = now;
  }

  draw(context: CanvasRenderingContext2D, now: number) {
    context.beginPath();
    context.arc(this.position.x, this.position.y, this.radius, 0, MathEx.TWO_PI);
    context.fillStyle = this.color.toHex();
    context.fill();
  }
}