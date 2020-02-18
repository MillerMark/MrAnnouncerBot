class SpritesEffectDto {
  constructor(public spriteName: string, public center: Vector, public startFrameIndex: number) {

  }
}

class SpritesEffect extends VisualEffect {
  lifeSpanMs: number;
  constructor(public spritesRef: Sprites, public visualEffectTarget: VisualEffectTarget, public startFrameIndex: number,
		public hueShift: number, public saturation: number, public brightness: number, public flipHorizontally: boolean = false,
		public flipVertically: boolean = false, public scale: number = 1, public rotation: number = 0, public autoRotation: number = 0,
		public velocityX: number = 0, public velocityY: number = 0) {
    super();
  }

	start(): SpriteProxy {
		let center: Vector = this.visualEffectTarget.getCenter();
		let sprite: SpriteProxy;
		if (this.hueShift !== 0 || this.saturation >= 0 || this.brightness >= 0) {
			sprite = new ColorShiftingSpriteProxy(this.startFrameIndex, this.visualEffectTarget.getCenter().add(
				new Vector(-this.spritesRef.originX, -this.spritesRef.originY)), this.lifeSpanMs)
				.setHueSatBrightness(this.hueShift, this.saturation, this.brightness);
			this.spritesRef.sprites.push(sprite);
    }
    else {
			sprite = this.spritesRef.add(center.x, center.y, this.startFrameIndex);
		}

		sprite.flipHorizontally = this.flipHorizontally;
		sprite.flipVertically = this.flipVertically;
		sprite.scale = this.scale;
		sprite.rotation = this.rotation;
		sprite.autoRotationDegeesPerSecond = this.autoRotation;
		sprite.velocityX = this.velocityX;
		sprite.velocityY = this.velocityY;
		return sprite;
  }
}