class WindupData {
	constructor(
		public Effect: string,
		public Name: string,
		public Scale: number,
		public Expiration: number,
		public FadeIn: number,
		public FadeOut: number,
		public Hue: number,
		public Saturation: number,
		public Brightness: number,
		public SoundFileName: string,
		public Rotation: number,
		public AutoRotation: number,
		public DegreesOffset: number,
		public Velocity: Vector,
		public Offset: Vector,
		public Force: Vector,
		public ForceAmount: number,
		public FlipHorizontal: boolean,
		public FlipVertical: boolean) {
	}
}
