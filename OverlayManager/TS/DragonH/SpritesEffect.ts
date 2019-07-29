class SpritesEffectDto {
  constructor(public spriteName: string, public center: Vector, public startFrameIndex: number) {

  }
}

class SpritesEffect extends VisualEffect {
  lifeSpanMs: number;
  constructor(private spritesRef: Sprites, private visualEffectTarget: VisualEffectTarget, private startFrameIndex: number,
		private hueShift: number, private saturation: number, private brightness: number, private flipHorizontally: boolean = false) {
    super();
  }

  start(): void {
    let center: Vector = this.visualEffectTarget.getCenter();
		if (this.hueShift !== 0 || this.saturation >= 0 || this.brightness >= 0) {
			let sprite: ColorShiftingSpriteProxy = new ColorShiftingSpriteProxy(this.startFrameIndex, this.visualEffectTarget.getCenter().add(
				new Vector(-this.spritesRef.originX, -this.spritesRef.originY)), this.lifeSpanMs)
				.setHueSatBrightness(this.hueShift, this.saturation, this.brightness);
			sprite.flipHorizontally = this.flipHorizontally;
			this.spritesRef.sprites.push(sprite);
    }
    else {
      this.spritesRef.add(center.x, center.y, this.startFrameIndex);
    }
  }
}