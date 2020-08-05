//! Synchronize with WindupDto in WindupDto.ts.
class WindupData {
	constructor(
		public Effect: string,
		public Hue: number,
		public Opacity: number = 1,
		public Saturation: number = 100,
		public Brightness: number = 100,
		public Scale: number = 1,
		public Lifespan: number = undefined,
		public FadeIn: number = 500,
		public FadeOut: number = 900,
		public StartSound: string = '',
		public EndSound: string = '',
		public Rotation: number = 0,
		public AutoRotation: number = 0,
		public DegreesOffset: number = 0,
		public Velocity: Vector = Vector.zero,
		public Offset: Vector = Vector.zero,
		public Force: Vector = Vector.zero,
		public ForceAmount: number = 0,
		public PlayToEndOnExpire: boolean = false,
		public FlipHorizontal: boolean = false,
		public FlipVertical: boolean = false,
		public Name: string = '',
		public TargetScale: number = 1
	) {
	}
}
