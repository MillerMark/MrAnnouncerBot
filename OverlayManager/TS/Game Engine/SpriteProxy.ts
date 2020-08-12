class SpriteProxy extends AnimatedElement {
	frameIntervalOverride: number;
	animationReverseOverride: boolean;
	haveCycledOnce: boolean;
	flipHorizontally: boolean;
	flipVertically: boolean;
	systemDrawn = true;
	owned: boolean;
	cropped: boolean;
	playToEndOnExpire = false;
	frameIndex: number;
	cropTop: number;
	cropLeft: number;
	cropRight: number;
	cropBottom: number;
	numFramesDrawn = 0;
	onCycleCallbacks: Array<(sprite: SpriteProxy, now: number) => void>;
	onFrameAdvanceCallbacks: Array<(sprite: SpriteProxy, returnFrameIndex: number, reverse: boolean, now: number) => void>;


	lastTimeWeAdvancedTheFrame: number;
	returnFrameIndexOverride: number = undefined; 

	constructor(startingFrameNumber: number, x: number, y: number, lifeSpanMs = -1) {
		super(x, y, lifeSpanMs);
		this.frameIndex = Math.floor(startingFrameNumber);
	}

	addOnCycleCallback(onAnimationCycled: (sprite: SpriteProxy, now: number) => void): void {
		if (!this.onCycleCallbacks)
			this.onCycleCallbacks = new Array<(sprite: SpriteProxy, now: number) => void>();
		this.onCycleCallbacks.push(onAnimationCycled);
	}

	removeAllCycleCallbacks(): void {
		this.onCycleCallbacks = null;
	}

	addOnFrameAdvanceCallback(onFrameAdvanced: (sprite: SpriteProxy, returnFrameIndex: number, reverse: boolean, now: number) => void): void {
		if (!this.onFrameAdvanceCallbacks)
			this.onFrameAdvanceCallbacks = new Array<(sprite: SpriteProxy, returnFrameIndex: number, reverse: boolean, now: number) => void>();
		this.onFrameAdvanceCallbacks.push(onFrameAdvanced);
	}

	removeAllFrameAdvanceCallbacks(): void {
		this.onFrameAdvanceCallbacks = null;
	}

	okayToDie(frameCount: number): boolean {
		return !this.playToEndOnExpire || this.frameIndex >= frameCount;
	}


	cycled(now: number) {
		this.haveCycledOnce = true;
		if (this.onCycleCallbacks) {
			this.onCycleCallbacks.forEach(function (oncycleCallback) {
				oncycleCallback(this, now);
			}, this);
		}
	}

	frameAdvanced(returnFrameIndex: number, reverse: boolean, nowMs: number) {
		if (this.onFrameAdvanceCallbacks) {
			this.onFrameAdvanceCallbacks.forEach(function (onFrameAdvanceCallback) {
				onFrameAdvanceCallback(this, returnFrameIndex, reverse, nowMs);
			}, this);
		}
	}

	advanceFrame(frameCount: number, nowMs: number, returnFrameIndex = 0, startIndex = 0, endBounds = 0, reverse = false, frameInterval: number = fps30, fileName = '') {
		if (nowMs < this.timeStart)
			return;

		let numFramesToAdvance: number;

		if (this.numFramesDrawn === 0) {
			numFramesToAdvance = 1;
			this.lastTimeWeAdvancedTheFrame = nowMs;
		}
		else {
			const msPassed = nowMs - this.lastTimeWeAdvancedTheFrame;
			if (msPassed < frameInterval)
				return;

			numFramesToAdvance = Math.floor(msPassed / frameInterval);
			if (numFramesToAdvance < 1)
				return;

			this.lastTimeWeAdvancedTheFrame += numFramesToAdvance * frameInterval;
		}

		this.numFramesDrawn += numFramesToAdvance;

		if (reverse) {
			this.frameIndex -= numFramesToAdvance;
		}
		else {
			this.frameIndex += numFramesToAdvance;
		}

		if (endBounds !== 0) {
			if (this.frameIndex >= endBounds && (!this.expirationDate || this.getLifeRemaining(nowMs) > 0)) {
				this.frameIndex = startIndex;
				this.cycled(nowMs);
			}
		}
		else if (!reverse) {
			if (this.frameIndex >= frameCount) {
				this.frameIndex = returnFrameIndex;
				this.cycled(nowMs);
			}
		}
		else {
			if (this.frameIndex <= 0) {
				this.frameIndex = returnFrameIndex;
				this.cycled(nowMs);
			}
		}
	}

	getHalfWidth(): number {
		return 0;
	}

	getHalfHeight(): number {
		return 0;
	}

	isHitBy(thisSprite: SpriteProxy): boolean {
		const minDistanceForHit = 70;
		return this.getDistanceTo(thisSprite) < minDistanceForHit;
	}

	getDistanceTo(otherSprite: SpriteProxy): number {
		return this.getDistanceToXY(otherSprite.x, otherSprite.y);
	}

	pathVector(spriteWidth: number, spriteHeight: number): Line {
		const halfWidth: number = spriteWidth / 2;
		const halfHeight: number = spriteHeight / 2;
		return Line.fromCoordinates(this.lastX + halfWidth, this.lastY + halfHeight,
			this.x + halfWidth, this.y + halfHeight);
	}

	draw(baseAnimation: Part, context: CanvasRenderingContext2D, now: number, spriteWidth: number, spriteHeight: number,
		originX = 0, originY = 0): void {
		baseAnimation.drawByIndex(context, this.x, this.y, this.frameIndex, this.horizontalScale, this.verticalScale, this.rotation, this.x + originX, this.y + originY, this.flipHorizontally, this.flipVertically);
	}

	drawAdornments(context: CanvasRenderingContext2D, now: number): void {
		// Descendants can override if they want to draw on top of the sprite...
	}

	drawBackground(context: CanvasRenderingContext2D, now: number): void {
		// Descendants can override if they want to draw the background...
	}

	setScale(scale: number): SpriteProxy {
		this.scale = scale;
		return this;
	}
}


class ColorShiftingSpriteProxy extends SpriteProxy {
	private _hueShift: number = 0;

	get hueShift(): number {
		return this._hueShift;
	}

	set hueShift(newValue: number) {
		if (this._hueShift === newValue)
			return;
		this._hueShift = newValue;
		this.recalculateFilter(performance.now());
	}


	private _saturationPercent: number = 100;

	get saturationPercent(): number {
		return this._saturationPercent;
	}

	set saturationPercent(newValue: number) {
		if (this._saturationPercent === newValue)
			return;
		this._saturationPercent = newValue;
		this.recalculateFilter(performance.now());
	}

	private _brightness: number = 100;

	get brightness(): number {
		return this._brightness;
	}

	set brightness(newValue: number) {
		if (this._brightness === newValue)
			return;
		this._brightness = newValue;
		this.recalculateFilter(performance.now());
	}

	hueShiftPerSecond: number = 0;

	static globalAllowColorShifting: boolean = true;
	//static globalAllowCanvasFilterCaching: boolean = false;

	constructor(startingFrameNumber: number, public center: Vector, lifeSpanMs: number = -1) {
		super(startingFrameNumber, center.x, center.y, lifeSpanMs);
	}

	filter: string;

	recalculateFilter(now: number) {
		let hueShift: number = this.getCurrentHueShift(now);
		this.filter = ColorShiftingSpriteProxy.getFilter(hueShift, this.saturationPercent, this.brightness);
	}

	shiftColor(baseAnimation: Part, context: CanvasRenderingContext2D, now: number): string {
		if (!ColorShiftingSpriteProxy.globalAllowColorShifting)
			return null;

		if (this.hueShiftPerSecond != 0)
			this.recalculateFilter(now);

		if (this.filter) {
			let oldFilter: string = (context as any).filter;
			//if (ColorShiftingSpriteProxy.globalAllowCanvasFilterCaching) {
			//	baseAnimation.filterImages(this.filter);
			//	return null;
			//}
			(context as any).filter = this.filter;  // Brings the pain!
			return oldFilter;
		}
		return null;
	}

	hasLifeRemaining(now: number) {
		return super.hasLifeRemaining(now) || this.playToEndOnExpire;
	}

	draw(baseAnimation: Part, context: CanvasRenderingContext2D, now: number, spriteWidth: number, spriteHeight: number,
		originX = 0, originY = 0): void {
		let saveFilter: string;
		const allowColorShifting: boolean = ColorShiftingSpriteProxy.globalAllowColorShifting;
		if (allowColorShifting)
			saveFilter = this.shiftColor(baseAnimation, context, now);
		try {
			/* 
			 * Knowing the frameIndex, the hueShift, the brightness, and the saturationPercent,
			 * we could look up the equivalent in a dictionary of cached Sprites.
			 * baseAnimation.drawByIndex(context, this.x, this.y, this.frameIndex, this.horizontalScale, this.verticalScale, this.rotation, this.x + originX, this.y + originY, this.flipHorizontally, this.flipVertically);
			 * 
			 * Note that we CAN NOT cache if this.hueShiftPerSecond != 0
			 * */
			//let cachedImages: CachedImages;
			//if (this.filter && ColorShiftingSpriteProxy.globalAllowCanvasFilterCaching)
			//	cachedImages = baseAnimation.filteredImages[this.filter];
			//baseAnimation.drawByIndex(context, this.x, this.y, this.frameIndex, this.horizontalScale, this.verticalScale, this.rotation, this.x + originX, this.y + originY, this.flipHorizontally, this.flipVertically, cachedImages);
			super.draw(baseAnimation, context, now, spriteWidth, spriteHeight, originX, originY);
		}
		finally {
			if (allowColorShifting && saveFilter)
				(context as any).filter = saveFilter;
		}
	}

	getCurrentHueShiftDelta(now: number): number {
		if (this.hueShiftPerSecond !== 0) {
			let secondsPassed: number = (now - this.timeStart) / 1000;
			return secondsPassed * this.hueShiftPerSecond % 360;
		}
		return 0;
	}

	getCurrentHueShift(now: number): number {
		return this.hueShift + this.getCurrentHueShiftDelta(now);
	}

	static readonly defaultGrayscale: number = 0;
	static readonly defaultBrightness: number = 100;
	static readonly defaultHueShift: number = 0;

	setHueSatBrightness(hueShift: number, saturationPercent: number = -1, brightness: number = -1): ColorShiftingSpriteProxy {
		this.hueShift = Math.round(hueShift);
		if (saturationPercent >= 0)
			this.saturationPercent = Math.round(saturationPercent);
		if (brightness >= 0)
			this.brightness = Math.round(brightness);
		return this;
	}

	setColorShiftMilestone(now: number): void {
		this.hueShift = this.getCurrentHueShift(now);
		this.changeVelocity(this.velocityX, this.velocityY, now); // Resets timeStart, used for calculating hue shifts.
	}

	static getFilter(hueShift: number, saturationPercent: number, brightness: number): string {
		let filter: string = '';
		if (hueShift !== ColorShiftingSpriteProxy.defaultHueShift)
			filter += `hue-rotate(${hueShift}deg) `;
		let grayScale: number = 100 - saturationPercent;
		if (grayScale !== ColorShiftingSpriteProxy.defaultGrayscale)
			filter += `grayscale(${(grayScale).toString()}%) `;
		if (brightness !== ColorShiftingSpriteProxy.defaultBrightness)
			filter += `brightness(${brightness}%)`;

		filter = filter.trim();
		return filter;
	}
}