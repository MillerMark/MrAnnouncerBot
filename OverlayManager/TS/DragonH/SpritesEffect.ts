class SpritesEffectDto {
  constructor(public spriteName: string, public center: Vector, public startFrameIndex: number) {

  }
}

class SpritesEffect extends VisualEffect {
  lifeSpanMs: number;
  scale: number = 1;
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
			sprite.scale = this.scale;
			this.spritesRef.sprites.push(sprite);
    }
    else {
			let sprite: SpriteProxy = this.spritesRef.add(center.x, center.y, this.startFrameIndex);
			sprite.scale = this.scale;
    }
  }
}