class EaseBase {
	constructor(public easeStartTime: number, public easeEndTime: number, public elasticIn: boolean = true) {

	}

	getEasedValue(now: number, originalValue: number, targetValue: number) {
		const deltaScale: number = targetValue - originalValue;

		const timePassed: number = this.getTimePassed(now);

		if (timePassed <= 0) {
			return originalValue;
		}

		const lifespan: number = this.getLifeSpan();

		let percentComplete: number;
		if (lifespan === 0)
			percentComplete = 1;
		else
			percentComplete = Math.max(Math.min(1, timePassed / lifespan), 0);

		let easedPercent: number;
		if (this.elasticIn)
			easedPercent = EaseBase.inOutParametricBlend(percentComplete);
		else
			easedPercent = percentComplete;

		return originalValue + deltaScale * easedPercent;
	}

	getLifeSpan(): number {
		return this.easeEndTime - this.easeStartTime;
	}

	getTimePassed(now: number): number {
		return now - this.easeStartTime;
	}

	static inElastic(t: number) {
		const inverseSpeed = 0.02;  // Smaller == faster.
		return t === 0 ? 0 : t === 1 ? 1 : (inverseSpeed - inverseSpeed / t) * Math.sin(25 * t) + 1;
	}

	static inOutSine(t: number): number {
		return -(Math.cos(Math.PI * t) - 1) / 2;
	}

	static inOutParametricBlend(t: number): number {
		const sqt: number = t * t;
		return sqt / (2.0 * (sqt - t) + 1.0);
	}

	static inOutElastic(t: number) {
		const c5 = (2 * Math.PI) / 4.5;

		return t === 0
			? 0
			: t === 1
				? 1
				: t < 0.5
					? -(Math.pow(2, 20 * t - 10) * Math.sin((20 * t - 11.125) * c5)) / 2
					: (Math.pow(2, -20 * t + 10) * Math.sin((20 * t - 11.125) * c5)) / 2 + 1;
	}

	getRemainingTime(now: number) {
		return Math.max(0, this.getLifeSpan() - this.getTimePassed(now));
	}
}

class EaseValue extends EaseBase {
	toValue: number;
	fromValue: number;

	to(toValue: number) {
		this.toValue = toValue;
	}

	from(fromValue: number) {
		this.fromValue = fromValue;
	}

	getValue(now: number): number {
		return this.getEasedValue(now, this.fromValue, this.toValue);
	}
}

class EasePoint extends EaseBase {
	constructor(public easeStartTime: number, public easeEndTime: number, public elasticIn: boolean = true) {
		super(easeStartTime, easeEndTime, elasticIn);
	}

	toX: number;
	toY: number;
	fromX: number;
	fromY: number;

	to(toX: number, toY: number) {
		this.toX = toX;
		this.toY = toY;
	}

	from(fromX: number, fromY: number) {
		this.fromX = fromX;
		this.fromY = fromY;
	}

	getX(now: number): number {
		return this.getEasedValue(now, this.fromX, this.toX);
	}

	getY(now: number): number {
		return this.getEasedValue(now, this.fromY, this.toY);
	}
}