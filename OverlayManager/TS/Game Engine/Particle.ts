class Particle extends WorldObject {
  birthTime: number;
  displacement: Vector;

  constructor(public emitter: Emitter, now: number, position: Vector, velocity: Vector = Vector.fromPolar(0, 0), public radius: number = 1, public opacity: number = 1,
    public color: HueSatLight = HueSatLight.fromHex('f00')) {
    super(position, velocity, 1);
    this.birthTime = now;
  }

  // Most descendants would only need to override render! No physics logic here :)
  // Would also need to add bouncing and moved off screen logic in the base.
  render(now: number, timeScale: number, world: World) {
    super.update(now, timeScale, world);

    if (this.hasExpired(now))
      return;

    if (now < this.birthTime)
      return;

    const context = world.ctx;
    this.calculateOpacity(context, now);

    context.beginPath();
    context.arc(this.position.x, this.position.y, this.radius, 0, MathEx.TWO_PI);
    context.fillStyle = this.color.toHex();
    context.fill();

    context.globalAlpha = 1;
  }

  private calculateOpacity(context: CanvasRenderingContext2D, now: number) {
    let timeAliveSeconds: number = now - this.birthTime;
    if (timeAliveSeconds < this.emitter.particleFadeInTime) {
      let percentageBorn: number = timeAliveSeconds / this.emitter.particleFadeInTime;
      context.globalAlpha = percentageBorn;
    }
    else {
        let lifeSpanSeconds: number = timeAliveSeconds;
      let percentageLived: number = lifeSpanSeconds / this.emitter.particleLifeSpanSeconds;
      context.globalAlpha = 1 - percentageLived;
    }
  }

  hasExpired(now: number): boolean {
    let lifeSpanSeconds: number = now - this.birthTime;
    return lifeSpanSeconds > this.emitter.particleLifeSpanSeconds;
  }
}
