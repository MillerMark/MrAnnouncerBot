class SpritesEffect extends VisualEffect {
  constructor(private sprites: Sprites, private visualEffectTarget: VisualEffectTarget, private startFrameIndex: number) {
    super();
  }

  start(): void {
    let center: Vector = this.visualEffectTarget.getCenter();
    this.sprites.add(center.x, center.y, this.startFrameIndex);
  }
}