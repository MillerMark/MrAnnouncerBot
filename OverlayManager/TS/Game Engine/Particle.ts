class Particle extends WorldObject {
  birthTime: number;
  displacement: Vector;
  color: HueSatLight;

  constructor(public emitter: Emitter, now: number, position: Vector, velocity: Vector = Vector.fromPolar(0, 0), public radius: number = 1, public opacity: number = 1) {
    super(position, velocity, 1);
    this.birthTime = now;
    this.color = emitter.getParticleColor();
  }

  // Most descendants would only need to override render! No physics logic here :)
  // Would also need to add bouncing and moved off screen logic in the base.
  render(now: number, timeScale: number, world: World) {
    super.update(now, timeScale, world);

//  update(now: number, secondsSinceLastUpdate: number): void {
//    let relativeGravity = this.emitter.particleGravityCenter.subtract(this.position);
//    let gravityX: number = relativeGravity.getRatioX(this.emitter.particleGravity);
//    let gravityY: number = relativeGravity.getRatioY(this.emitter.particleGravity);
//
//    let displacementX: number = Physics.metersToPixels(Physics.getDisplacement(secondsSinceLastUpdate, this.velocity.x - this.emitter.velocity.x + this.emitter.particleWind.x, gravityX));
//    let displacementY: number = Physics.metersToPixels(Physics.getDisplacement(secondsSinceLastUpdate, this.velocity.y - this.emitter.velocity.y + this.emitter.particleWind.y, gravityY));
//
//    this.position = new Vector(this.position.x + displacementX, this.position.y + displacementY);
//
//    let newVelocityX: number = Physics.getFinalVelocity(secondsSinceLastUpdate, this.velocity.x, gravityX);
//    let newVelocityY: number = Physics.getFinalVelocity(secondsSinceLastUpdate, this.velocity.y, gravityY);
//    this.velocity = new Vector(newVelocityX, newVelocityY);
//  }


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
