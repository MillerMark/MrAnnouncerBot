class AnimatedElement {
	name: string;
	isRemoving: boolean;
	autoRotationDegeesPerSecond: number = 0;
	autoScaleFactorPerSecond: number = 1;
	rotation: number;
	initialRotation: number;
	rotationStartTime: number;
	scaleStartTime: number;
	timeToRotate: number = 0;
	targetRotation: number = 0;
	degreesPerMs: number;
	lastRotationUpdate: number;
	opacity: number;
	expirationDate: number;
	timeStart: number;
	fadeInTime: number = 0;
	fadeOutTime: number = 4000;
	fadeOnDestroy: boolean = true;
	velocityX: number;
	velocityY: number;
	startX: number;
	startY: number;
	lastX: number;
	lastY: number;
	initialHorizontalScale: number = 1;
	initialVerticalScale: number = 1;
	verticalThrustOverride: number = undefined;
	horizontalThrustOverride: number = undefined;
	onExpire: () => void;

	constructor(public x: number, public y: number, lifeSpanMs: number = -1) {
		this.velocityX = 0;
		this.velocityY = 0;
		this.startX = x;
		this.startY = y;

		this.opacity = 1;
		this.rotation = 0;
		this.initialRotation = 0;
		this.initialHorizontalScale = 1;
		this.initialVerticalScale = 1;

		this.timeStart = performance.now();

		if (lifeSpanMs > 0)
			this.expirationDate = this.timeStart + lifeSpanMs;
		else
			this.expirationDate = null;
	}

	private _verticalScale: number = 1;
	private _horizontalScale: number = 1;

	get scale(): number {
		return this._horizontalScale;
	}

	set scale(newValue: number) {
		this._horizontalScale = newValue;
		this._verticalScale = newValue;
	}

	get verticalScale(): number {
		return this._verticalScale;
	}

	set verticalScale(newValue: number) {
		this._verticalScale = newValue;
	}

	get horizontalScale(): number {
		return this._horizontalScale;
	}

	set horizontalScale(newValue: number) {
		this._horizontalScale = newValue;
	}

	render(context: CanvasRenderingContext2D, now: number) {
		// Do nothing. Allow descendants to override.
	}

	rotateTo(targetRotation: number, degreesToMove: number, timeToRotate: number): void {
		if (timeToRotate == 0)
			return;

		//if (this.timeToRotate > 0) {  // Already rotating...
		//  // TODO: Figure out what to do when we are already rotating.
		//  return;
		//}

		this.rotationStartTime = performance.now();
		this.timeToRotate = timeToRotate;
		this.targetRotation = targetRotation;
		this.lastRotationUpdate = this.rotationStartTime;
		this.degreesPerMs = degreesToMove / timeToRotate;
	}

	easePointStillActive(now: number) {
		if (!this.easePoint)
			return false;
		return this.easePoint.getRemainingTime(now) > 0;
	}

	animate(nowMs: number) {
		if (nowMs < this.timeStart)
			return;

		if (this.timeToRotate > 0) {
			if (this.rotationStartTime + this.timeToRotate < nowMs) {
				// done rotating.
				this.timeToRotate = 0;
				this.rotation = this.targetRotation;
			}
			else {
				let timeSinceLastFrameAdvance: number = nowMs - this.lastRotationUpdate;
				let degreesToMove: number = this.degreesPerMs * timeSinceLastFrameAdvance;
				this.rotation += degreesToMove;
				if (this.rotation > 360) {
					this.rotation -= 360;
				}
				else if (this.rotation < 0) {
					this.rotation += 360;
				}
				this.lastRotationUpdate = nowMs;
			}
		}
		else if (this.autoRotationDegeesPerSecond != 0) {
			if (!this.rotationStartTime || this.rotationStartTime == 0)
				this.rotationStartTime = nowMs;
			else {
				let timeSpentRotatingSeconds: number = (nowMs - this.rotationStartTime) / 1000;
				this.rotation = this.initialRotation + timeSpentRotatingSeconds * this.autoRotationDegeesPerSecond;
			}
		}

		if (this.autoScaleFactorPerSecond != 1) {
			if (!this.scaleStartTime || this.scaleStartTime == 0) {
				this.scaleStartTime = nowMs;
				this._horizontalScale = this.initialHorizontalScale;
				this._verticalScale = this.initialVerticalScale;
			}
			else {
				let timeSpentScalingSeconds: number = (nowMs - this.scaleStartTime) / 1000;
				let scaleFactor: number = Math.pow(this.autoScaleFactorPerSecond, timeSpentScalingSeconds);
				this._horizontalScale = this.initialHorizontalScale * scaleFactor;
				this._verticalScale = this.initialVerticalScale * scaleFactor;
			}
		}
	}

	getDistanceToXY(x: number, y: number): number {
		const deltaX: number = this.x - x;
		const deltaY: number = this.y - y;
		return Math.sqrt(deltaX * deltaX + deltaY * deltaY);
	}

	set delayStart(delayMs: number) {
		this.timeStart = performance.now() + delayMs;
	}

	destroyBy(lifeTimeMs: number) {
		if (!this.expirationDate)
			this.expirationDate = performance.now() + Math.round(Math.random() * lifeTimeMs);
	}

	destroyAllInExactly(lifeTimeMs: number) {
		if (!this.expirationDate)
			this.expirationDate = performance.now() + lifeTimeMs;
	}

	stillAlive(now: number, frameCount = 0): boolean {
		return this.getLifeRemaining(now) >= 0 || !this.okayToDie(frameCount);
	}

	hasLifeRemaining(now: number) {
		return this.expirationDate === undefined || this.getLifeRemaining(now) > 0;
	}

	getLifeRemaining(now: number) {
		let lifeRemaining = 0;
		if (this.expirationDate) {
			lifeRemaining = this.expirationDate - now;
		}
		return lifeRemaining;
	}

	okayToDie(frameCount: number): boolean {
		// Allow descendants to change behavior
		console.log('okayToDie - return true;');
		return true;
	}

	fadingOut(now: number): boolean {
		const lifeRemaining: number = this.getLifeRemaining(now);
		return this.isFadingOut(lifeRemaining);
	}

	private isFadingOut(lifeRemaining: number): boolean {
		return lifeRemaining < this.fadeOutTime && this.fadeOnDestroy;
	}

	logData: boolean;

	getAlpha(now: number): number {
		const msAlive: number = now - this.timeStart;

		if (msAlive < 0)   // Not yet alive!!!
			return 0;


		if (msAlive < this.fadeInTime)
			return this.opacity * msAlive / this.fadeInTime;

		if (!this.expirationDate)
			return this.opacity;

		const lifeRemaining: number = this.getLifeRemaining(now);

		if (!this.hasLifeRemaining(now))
			return 0;

		if (this.isFadingOut(lifeRemaining)) {
			return this.opacity * lifeRemaining / this.fadeOutTime;
		}
		return this.opacity;
	}

	getHorizontalThrust(now: number): number {
		if (this.horizontalThrustOverride !== undefined)
			return this.horizontalThrustOverride;
		return 0;
	}

	getVerticalThrust(now: number): number {
		if (this.verticalThrustOverride !== undefined)
			return this.verticalThrustOverride;
		return gravityGames.activePlanet.gravity;
	}

	bounce(left: number, top: number, right: number, bottom: number, width: number, height: number, now: number) {
		const secondsPassed = (now - this.timeStart) / 1000;
		const horizontalBounceDecay = 0.9;
		const verticalBounceDecay = 0.9;

		const velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
		const velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalThrust(now));

		const hitLeftWall = velocityX < 0 && this.x < left;
		const hitRightWall = velocityX > 0 && this.x + width > right;

		const hitTopWall = velocityY < 0 && this.y < top;
		const hitBottomWall = velocityY > 0 && this.y + height > bottom;

		let newVelocityX = velocityX;
		let newVelocityY = velocityY;
		if (hitLeftWall || hitRightWall)
			newVelocityX = -velocityX * horizontalBounceDecay;
		if (hitTopWall || hitBottomWall)
			newVelocityY = -velocityY * verticalBounceDecay;

		if (hitLeftWall || hitRightWall || hitTopWall || hitBottomWall) {
			this.x = this.startX + Physics.metersToPixels(Physics.getDisplacement(secondsPassed, this.velocityX, this.getHorizontalThrust(now)));
			this.y = this.startY + Physics.metersToPixels(Physics.getDisplacement(secondsPassed, this.velocityY, this.getVerticalThrust(now)));
			this.changeVelocity(newVelocityX, newVelocityY, now);
		}
		return hitBottomWall;
	}

	changingDirection(now: number): void {
		const secondsPassed = (now - this.timeStart) / 1000;
		const velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
		const velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalThrust(now));
		this.changeVelocity(velocityX, velocityY, now);
	}

	changeVelocity(velocityX: number, velocityY: number, now: number) {
		this.timeStart = now;
		this.velocityX = velocityX;
		this.velocityY = velocityY;
		this.startX = this.x;
		this.startY = this.y;
	}

	matches(matchData: any): boolean {
		return matchData === null;   // Descendants can override if they want to implement a custom search/find functionality...
	}

	storeLastPosition() {
		this.lastX = this.x;
		this.lastY = this.y;
	}

	protected easePoint: EasePoint;

	ease(startTime: number, fromX: number, fromY: number, toX: number, toY: number, timeSpanMs: number) {
		this.easePoint = new EasePoint(startTime, startTime + timeSpanMs);
		this.easePoint.from(fromX, fromY);
		this.easePoint.to(toX, toY);
	}

	clearEasePoint() {
		this.startX = this.x;
		this.startY = this.y;
		this.easePoint = null;
	}

	protected updateEasePosition(nowMs: number) {
		this.x = this.easePoint.getX(nowMs);
		this.y = this.easePoint.getY(nowMs);
	}

	updatePosition(nowMs: number) {
		this.storeLastPosition();

		if (this.easePoint) {
			this.updateEasePosition(nowMs);
			if (this.easePoint.getRemainingTime(nowMs) === 0)
				this.clearEasePoint();
			return;
		}

		const secondsPassed = (nowMs - this.timeStart) / 1000;

		const xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, this.getHorizontalThrust(nowMs));
		this.x = this.startX + Physics.metersToPixels(xDisplacement);

		const yDisplacement = Physics.getDisplacement(secondsPassed, this.velocityY, this.getVerticalThrust(nowMs));
		this.y = this.startY + Physics.metersToPixels(yDisplacement);
	}

	setFadeTimes(fadeInTime: number, fadeOutTime: number): AnimatedElement {
		this.fadeInTime = fadeInTime;
		this.fadeOutTime = fadeOutTime;
		return this;
	}

	removing(): void {

	}

	destroying(): void {
		//if (this.name)
		//	console.log('destroying this sprite: ' + this.name);
		if (this.onExpire)
			this.onExpire();
	}
}
