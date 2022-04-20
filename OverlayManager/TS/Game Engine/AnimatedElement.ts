class AnimatedElement {
  data: any;
  tag: any;
  name: string;
  isRemoving: boolean;
  autoRotationDegeesPerSecond = 0;
  autoScaleFactorPerSecond = 1;
  autoScaleMaxScale = 100;
  rotation: number;
  initialRotation: number;
  rotationStartTime: number;
  scaleStartTime: number;
  timeToRotate = 0;
  targetRotation = 0;
  degreesPerMs: number;
  lastRotationUpdate: number;
  opacity: number;
  expirationDate: number;
  lifetimeStart: number;
  physicsTimeStart: number;
  fadeInTime = 0;
  fadeOutTime = 4000;
  fadeOnDestroy = true;
  velocityX: number;
  velocityY: number;
  startX: number;
  startY: number;
  lastX: number;
  lastY: number;
  initialHorizontalScale = 1;
  initialVerticalScale = 1;
  verticalThrustOverride: number = undefined;
  horizontalThrustOverride: number = undefined;
  onExpire: () => void;

  setInitialScale(scale: number) {
    this.initialHorizontalScale = scale;
    this.initialVerticalScale = scale;
  }


  fadeOutNow(fadeOutTime: number) {
    this.fadeOutAfter(0, fadeOutTime);
  }

  fadeOutAfter(delayMs: number, fadeOutTimeMs = -1) {
    if (fadeOutTimeMs >= 0)
      this.fadeOutTime = fadeOutTimeMs;
    this.expirationDate = performance.now() + delayMs + this.fadeOutTime;
  }

  private _x: number;

  get x(): number {
    return this._x;
  }

  set x(newValue: number) {
    const deltaX: number = newValue - this._x;
    if (deltaX === 0)
      return;
    this._x = newValue;
    this.justMoved(deltaX, 0);
  }

  private _y: number;

  get y(): number {
    return this._y;
  }

  set y(newValue: number) {
    const deltaY: number = newValue - this._y;
    if (deltaY === 0)
      return;
    this._y = newValue;
    this.justMoved(0, deltaY);
  }

  justMoved(deltaX: number, deltaY: number): void {
    if (this.attachedElements)
      this.attachedElements.forEach(function (element) { element.move(deltaX, deltaY); });
  }

  move(deltaX: number, deltaY: number) {
    this.x += deltaX;
    this.y += deltaY;
  }

  attachedElements: Array<AnimatedElement>;

  attach(animatedElement: AnimatedElement) {
    if (!this.attachedElements)
      this.attachedElements = [];
    this.attachedElements.push(animatedElement);
  }

  fadeOutAttachedElements(delayMs: number) {
    if (!this.attachedElements)
      return;
    this.attachedElements.forEach(function (element) { element.fadeOutNow(delayMs); });
  }

  constructor(x: number, y: number, lifeSpanMs = -1) {
    this.x = x;
    this.y = y;
    this.velocityX = 0;
    this.velocityY = 0;
    this.startX = x;
    this.startY = y;

    this.opacity = 1;
    this.rotation = 0;
    this.initialRotation = 0;
    this.initialHorizontalScale = 1;
    this.initialVerticalScale = 1;

    this.lifetimeStart = performance.now();
    this.physicsTimeStart = this.lifetimeStart;

    if (lifeSpanMs > 0)
      this.expirationDate = this.lifetimeStart + lifeSpanMs;
    else
      this.expirationDate = null;
  }

  private _verticalScale = 1;
  private _horizontalScale = 1;

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

  rotateTo(targetRotation: number, degreesToMovePerTime: number, timeToRotate: number): void {
    if (timeToRotate === 0)
      return;

    //if (this.timeToRotate > 0) {  // Already rotating...
    //  // TODO: Figure out what to do when we are already rotating.
    //  return;
    //}

    this.rotationStartTime = performance.now();
    this.timeToRotate = timeToRotate;
    this.targetRotation = targetRotation;
    this.lastRotationUpdate = this.rotationStartTime;
    this.degreesPerMs = degreesToMovePerTime / timeToRotate;
  }

  easePointStillActive(now: number) {
    if (!this.easePoint)
      return false;
    return this.easePoint.getRemainingTime(now) > 0;
  }

  easeRotationStillActive(now: number) {
    if (!this.easeRotation)
      return false;
    return this.easeRotation.getRemainingTime(now) > 0;
  }

  animate(nowMs: number) {
    if (nowMs < this.lifetimeStart)
      return;

    if (this.timeToRotate > 0) {
      if (this.rotationStartTime + this.timeToRotate < nowMs) {
        // done rotating.
        this.timeToRotate = 0;
        this.rotation = this.targetRotation;
      }
      else {
        const timeSinceLastFrameAdvance: number = nowMs - this.lastRotationUpdate;
        const degreesToMove: number = this.degreesPerMs * timeSinceLastFrameAdvance;
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
    else if (this.autoRotationDegeesPerSecond !== 0) {
      if (!this.rotationStartTime || this.rotationStartTime === 0)
        this.rotationStartTime = nowMs;
      else {
        const timeSpentRotatingSeconds: number = (nowMs - this.rotationStartTime) / 1000;
        this.rotation = this.initialRotation + timeSpentRotatingSeconds * this.autoRotationDegeesPerSecond;
      }
    }

    if (this.autoScaleFactorPerSecond !== 1) {
      if (!this.scaleStartTime || this.scaleStartTime === 0) {
        this.scaleStartTime = nowMs;
        this._horizontalScale = this.initialHorizontalScale;
        this._verticalScale = this.initialVerticalScale;
      }
      else {
        const timeSpentScalingSeconds: number = (nowMs - this.scaleStartTime) / 1000;
        const scaleFactor: number = Math.pow(this.autoScaleFactorPerSecond, timeSpentScalingSeconds);
        this._horizontalScale = Math.min(this.autoScaleMaxScale, this.initialHorizontalScale * scaleFactor);
        this._verticalScale = Math.min(this.autoScaleMaxScale, this.initialVerticalScale * scaleFactor);
        //this._horizontalScale = this.initialHorizontalScale * scaleFactor;
        //this._verticalScale = this.initialVerticalScale * scaleFactor;
      }
    }
  }

  getDistanceToXY(x: number, y: number): number {
    const deltaX: number = this.x - x;
    const deltaY: number = this.y - y;
    return Math.sqrt(deltaX * deltaX + deltaY * deltaY);
  }

  set delayStart(delayMs: number) {
    this.lifetimeStart = performance.now() + delayMs;
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
    return this.getLifeRemainingMs(now) >= 0 || !this.okayToDie(frameCount);
  }

  hasLifeRemaining(now: number) {
    return this.expirationDate === undefined || this.getLifeRemainingMs(now) > 0;
  }

  getLifeRemainingMs(now: number): number {
    let lifeRemaining = 0;
    if (this.expirationDate) {
      lifeRemaining = this.expirationDate - now;
    }
    return lifeRemaining;
  }

  okayToDie(frameCount: number): boolean {
    // Allow descendants to change behavior
    return true;
  }

  fadingOut(now: number): boolean {
    const lifeRemaining: number = this.getLifeRemainingMs(now);
    return this.isFadingOut(lifeRemaining);
  }

  private isFadingOut(lifeRemaining: number): boolean {
    return lifeRemaining < this.fadeOutTime && this.fadeOnDestroy;
  }

  logData: boolean;
  alreadyFadedIn: boolean;

  getAlpha(now: number): number {
    const msAlive: number = now - this.lifetimeStart;

    if (msAlive < 0)   // Not yet alive!!!
      return 0;

    if (msAlive < this.fadeInTime && !this.alreadyFadedIn)
      return this.opacity * msAlive / this.fadeInTime;

    this.alreadyFadedIn = true;

    if (!this.expirationDate)
      return this.opacity;

    const lifeRemaining: number = this.getLifeRemainingMs(now);

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
    const secondsPassed = (now - this.physicsTimeStart) / 1000;
    const horizontalBounceDecay = 0.9;
    const verticalBounceDecay = 0.9;

    const velocityX = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
    const velocityY = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityY, this.getVerticalThrust(now));

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
      this.x = this.startX + Physics.metersToPixels(Physics.getDisplacementMeters(secondsPassed, this.velocityX, this.getHorizontalThrust(now)));
      this.y = this.startY + Physics.metersToPixels(Physics.getDisplacementMeters(secondsPassed, this.velocityY, this.getVerticalThrust(now)));
      this.changeVelocity(newVelocityX, newVelocityY, now);
    }
    return hitBottomWall;
  }

  changingDirection(nowMs: number): void {
    const secondsPassed = (nowMs - this.physicsTimeStart) / 1000;
    const velocityX = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityX, this.getHorizontalThrust(nowMs));
    const velocityY = Physics.getFinalVelocityMetersPerSecond(secondsPassed, this.velocityY, this.getVerticalThrust(nowMs));
    this.changeVelocity(velocityX, velocityY, nowMs);
  }

  changeVelocity(velocityX: number, velocityY: number, nowMs: number) {
    this.physicsTimeStart = nowMs;
    this.velocityX = velocityX;
    this.velocityY = velocityY;
    this.startX = this.x;
    this.startY = this.y;
  }

  matches(matchData): boolean {
    return matchData === null;   // Descendants can override if they want to implement a custom search/find functionality...
  }

  storeLastPosition() {
    this.lastX = this.x;
    this.lastY = this.y;
  }

  easePoint: EasePoint;
  easeRotation: EaseValue;

  ease(startTime: number, fromX: number, fromY: number, toX: number, toY: number, timeSpanMs: number) {
    this.easePoint = new EasePoint(startTime, startTime + timeSpanMs);
    this.easePoint.from(fromX, fromY);
    this.easePoint.to(toX, toY);
  }

  easeSpin(startTime: number, fromValue: number, toValue: number, timeSpanMs: number) {
    this.easeRotation = new EaseValue(startTime, startTime + timeSpanMs);
    this.easeRotation.from(fromValue);
    this.easeRotation.to(toValue);
  }

  isRotating(): boolean {
    return (this.timeToRotate > 0 || this.easeRotation != null);
  }

  clearEasePoint() {
    this.startX = this.x;
    this.startY = this.y;
    this.easePoint = null;
    //console.log(`clearEasePoint();`);
  }

  clearEaseRotation() {
    this.easeRotation = null;
  }

  protected updateEasePosition(nowMs: number) {
    this.x = this.easePoint.getX(nowMs);
    this.y = this.easePoint.getY(nowMs);
  }

  protected updateEaseRotation(nowMs: number) {
    this.rotation = this.easeRotation.getValue(nowMs);
  }

  updatePosition(nowMs: number) {
    this.storeLastPosition();

    if (this.easeRotation) {
      this.updateEaseRotation(nowMs);
      if (this.easeRotation.getRemainingTime(nowMs) === 0)
        this.clearEaseRotation();
    }

    if (this.easePoint) {
      this.updateEasePosition(nowMs);
      if (this.easePoint.getRemainingTime(nowMs) === 0)
        this.clearEasePoint();
      return;
    }

    const secondsPassed = (nowMs - this.physicsTimeStart) / 1000;

    const xDisplacement = Physics.getDisplacementMeters(secondsPassed, this.velocityX, this.getHorizontalThrust(nowMs));
    this.x = this.startX + Physics.metersToPixels(xDisplacement);

    const yDisplacement = Physics.getDisplacementMeters(secondsPassed, this.velocityY, this.getVerticalThrust(nowMs));
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
