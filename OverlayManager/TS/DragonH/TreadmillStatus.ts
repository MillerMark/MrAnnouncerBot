class TreadmillStatus {
	private speedText: AnimatedText | null = null;
	private unitText: AnimatedText | null = null;
	private distanceText: AnimatedText | null = null;
	private lastSpeedStr: string = '';
	private lastDistStr: string = '';
	private isFadingOut: boolean = false;

	private static readonly fadeOutMs: number = 3500;
	private static readonly speedScale: number = 1.5;
	private static readonly unitScale: number = 0.75;
	private static readonly distanceScale: number = 0.8;
	private static readonly speedY: number = 920;
	private static readonly distanceY: number = 1010;
	private static readonly rightEdgeX: number = 1880;
	private static readonly hueShift: number = 180;
	private static readonly unitGapPx: number = 20;

	constructor(private animatedTextManager: AnimatedTextManager) { }

	private static formatSpeed(speedKph: number): string {
		return speedKph.toFixed(1);
	}

	private static formatDistance(distanceKm: number): string {
		const meters = distanceKm * 1000;
		if (meters < 1000)
			return `${Math.round(meters)} m`;
		return `${distanceKm.toFixed(1)} km`;
	}

	private fadeOutAllSprites(): void {
		const fadeText = (text: AnimatedText | null) => {
			if (!text) return;
			text.spriteMap.forEach((sprite: SpriteProxy) => {
				sprite.fadeOutNow(TreadmillStatus.fadeOutMs);
			});
		};
		fadeText(this.speedText);
		fadeText(this.unitText);
		fadeText(this.distanceText);
	}

	private removeAll(): void {
		if (this.speedText) { this.animatedTextManager.remove(this.speedText); this.speedText = null; }
		if (this.unitText) { this.animatedTextManager.remove(this.unitText); this.unitText = null; }
		if (this.distanceText) { this.animatedTextManager.remove(this.distanceText); this.distanceText = null; }
		this.lastSpeedStr = '';
		this.lastDistStr = '';
	}

	private rebuild(speedKph: number, distanceKm: number): void {
		this.removeAll();
		const speedStr = TreadmillStatus.formatSpeed(speedKph);
		const distStr = TreadmillStatus.formatDistance(distanceKm);

		// "kph" right-aligned at rightEdgeX; speed right-aligned just to the left of "kph"
		const unitWidth = this.animatedTextManager.getTextWidth('kph', TreadmillStatus.unitScale);
		const speedEndX = TreadmillStatus.rightEdgeX - unitWidth - TreadmillStatus.unitGapPx;

		this.unitText = this.animatedTextManager.addText('kph', TreadmillStatus.rightEdgeX, TreadmillStatus.speedY, TreadmillStatus.hueShift, 'right', TreadmillStatus.unitScale);
		this.speedText = this.animatedTextManager.addText(speedStr, speedEndX, TreadmillStatus.speedY, TreadmillStatus.hueShift, 'right', TreadmillStatus.speedScale);
		this.distanceText = this.animatedTextManager.addText(distStr, TreadmillStatus.rightEdgeX, TreadmillStatus.distanceY, TreadmillStatus.hueShift, 'right', TreadmillStatus.distanceScale);

		this.lastSpeedStr = speedStr;
		this.lastDistStr = distStr;
	}

	update(speedKph: number, distanceKm: number): void {
		if (this.isFadingOut) return;

		const wasVisible = this.speedText !== null;
		const nowZero = speedKph <= 0;

		if (nowZero && wasVisible) {
			this.isFadingOut = true;
			this.fadeOutAllSprites();
			const speedRef = this.speedText;
			const unitRef = this.unitText;
			const distRef = this.distanceText;
			this.speedText = null;
			this.unitText = null;
			this.distanceText = null;
			setTimeout(() => {
				if (speedRef) this.animatedTextManager.remove(speedRef);
				if (unitRef) this.animatedTextManager.remove(unitRef);
				if (distRef) this.animatedTextManager.remove(distRef);
				this.isFadingOut = false;
			}, TreadmillStatus.fadeOutMs);
			return;
		}

		if (!nowZero) {
			const speedStr = TreadmillStatus.formatSpeed(speedKph);
			const distStr = TreadmillStatus.formatDistance(distanceKm);
			if (speedStr !== this.lastSpeedStr || distStr !== this.lastDistStr)
				this.rebuild(speedKph, distanceKm);
		}
	}
}
