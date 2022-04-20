class VideoFeedDto {
  constructor(public X: number, public Y: number, public Scale: number, public Rotation: number) {

  }
}

class HeadBox {
  constructor(public left: number, public top: number, public right: number, public bottom: number) {

  }
}

enum TargetingState {
  none,
  startedTargeting,
  mainThrustTowardsCourse,
  coastingTowardsTargetX,
  breakingForTargetX,
  movingToTargetY
}


class MarkFliesGame extends GamePlusQuiz {
  spaceshipSprites: Sprites;
  flameSprites: Sprites;
  animations: Animations = new Animations();
  spaceshipProxy: SpriteProxy;
  flameProxy: SpriteProxy;
  targeting: boolean;
  targetX: number;
  targetY: number;
  targetingStopTime: number;
  targetScale: number;
  targetingStartTime: number;
  markHeadBox: HeadBox;
  maintainAltitude: boolean = true;

  constructor(context: CanvasRenderingContext2D) {
    super(context);
  }

  updateScreen(context: CanvasRenderingContext2D, nowMs: number) {
    super.updateScreen(context, nowMs);

    this.spaceshipSprites.updatePositions(nowMs);
    this.flameSprites.updatePositions(nowMs);

    this.keepSpaceshipInBounds(nowMs);
    this.updateThrustFromTilt(nowMs);
    this.updateVideoFeedFromShip();
    this.flyMarkTowardTarget(nowMs);

    this.flameSprites.draw(markFliesContext, nowMs);
    this.spaceshipSprites.draw(markFliesContext, nowMs);

    this.animations.removeExpiredAnimations(nowMs);
    this.animations.render(markFliesContext, nowMs);

    this.updateFlameToMatchThrust();

    this.drawDiagnostics(context, nowMs);
  }

  updateFlameToMatchThrust() {
    if (this.lastThrust === this.thrust)
      return;
    this.lastThrust = this.thrust;
    if (this.spaceshipProxy)
      this.setScale(this.spaceshipProxy.scale);
    else
      this.setScale(1);
  }

  drawDiagnostics(context: CanvasRenderingContext2D, nowMs: number) {
    if (!this.spaceshipProxy)
      return;
    this.drawTarget(context, nowMs);
    this.drawBoxAroundMarksHead(context, nowMs);
    this.drawFinalTrajectory(context, nowMs);
    this.drawSpaceshipCenter(context, nowMs);
    this.drawWallBounceRect(context, nowMs);
  }

  drawTarget(context: CanvasRenderingContext2D, nowMs: number) {
    if (this.targeting) {
      context.beginPath();
      context.strokeStyle = "#ffffff";
      context.moveTo(0, this.targetY);
      context.lineTo(screenWidth, this.targetY);
      context.moveTo(this.targetX, 0);
      context.lineTo(this.targetX, screenHeight);
      context.stroke();

      const stateOffset: number = 10;
      context.beginPath();
      context.fillStyle = "#202080";
      context.fillRect(this.targetX + stateOffset, this.targetY + stateOffset, 600, 80);
      context.fill();

      context.fillStyle = "#fff";
      context.font = "44px serif";
      context.textAlign = "left";
      context.textBaseline = "top";
      context.fillText(TargetingState[this.targetingState].toString(), this.targetX + stateOffset, this.targetY + stateOffset);
      // 
    }
  }

  drawWallBounceRect(context: CanvasRenderingContext2D, nowMs: number) {
    if (!this.wallBounceRect)
      return;

    const extraTimeForWallBounceRect: number = 3000;
    const wallBounceRectFadeoutTime: number = this.targetingStopTime + extraTimeForWallBounceRect;
    if (wallBounceRectFadeoutTime < nowMs)
      return;

    const width: number = this.wallBounceRect.right - this.wallBounceRect.left;
    const height: number = this.wallBounceRect.bottom - this.wallBounceRect.top;
    const timerPercentageRemaining: number = (wallBounceRectFadeoutTime - nowMs) / (this.timeToTargetSeconds * 1000 + extraTimeForWallBounceRect);
    context.globalAlpha = timerPercentageRemaining;
    context.beginPath();
    context.fillStyle = "#bb66ff";
    context.fillRect(this.wallBounceRect.left, this.wallBounceRect.top, width, height);
    context.fill();
    context.globalAlpha = 1;
  }

  drawSpaceshipCenter(context: CanvasRenderingContext2D, nowMs: number) {
    const center: Point = this.spaceshipCenter;
    const edgeLength: number = 20;
    const halfEdgeLength: number = edgeLength / 2;
    context.beginPath();
    context.strokeStyle = "#ff8080";
    context.lineWidth = 2;
    context.rect(center.x - halfEdgeLength, center.y - halfEdgeLength, edgeLength, edgeLength);
    context.fillStyle = "#ffdddd";
    context.fillRect(center.x - halfEdgeLength, center.y - halfEdgeLength, edgeLength, edgeLength);
    context.stroke();
    context.fill();
  }

  drawFinalTrajectory(context: CanvasRenderingContext2D, nowMs: number) {
    if (!this.twoSecondTrajectoryPoint)
      return;
    const edgeLength: number = 20;
    context.beginPath();
    context.strokeStyle = "#4040ff";
    context.lineWidth = 2;
    this.drawRect(context, this.twoSecondTrajectoryPoint, edgeLength);
    context.fillStyle = "#ffffff";
    this.fillRect(context, this.twoSecondTrajectoryPoint, edgeLength);
    context.stroke();
    context.fill();
    if (nowMs < this.targetingStopTime) {
      const timerPercentageRemaining: number = (this.targetingStopTime - nowMs) / (this.timeToTargetSeconds * 1000);
      const timerBoxFactor: number = 1 + 3 * timerPercentageRemaining;
      context.strokeStyle = "#ff0000";
      this.drawRect(context, this.twoSecondTrajectoryPoint, edgeLength * timerBoxFactor);
    }
    context.stroke();
  }

  drawRect(context: CanvasRenderingContext2D, center: Point, edgeLength: number) {
    const halfEdgeLength: number = edgeLength / 2;
    context.rect(center.x - halfEdgeLength, center.y - halfEdgeLength, edgeLength, edgeLength);
  }

  fillRect(context: CanvasRenderingContext2D, center: Point, edgeLength: number) {
    const halfEdgeLength: number = edgeLength / 2;
    context.fillRect(center.x - halfEdgeLength, center.y - halfEdgeLength, edgeLength, edgeLength);
  }

  drawBoxAroundMarksHead(context: CanvasRenderingContext2D, nowMs: number) {
    context.beginPath();
    context.strokeStyle = "#ff0000";
    context.lineWidth = 2;
    context.rect(this.markHeadBox.left, this.markHeadBox.top, this.markHeadBox.right - this.markHeadBox.left, this.markHeadBox.bottom - this.markHeadBox.top);
    context.stroke();
  }

  recentlyTargeting: boolean;

  updateThrustFromTilt(nowMs: number) {
    if (!this.spaceshipProxy)
      return;

    if (this.targetingStopTime && nowMs < this.targetingStopTime) {
      this.recentlyTargeting = true;
      return;
    }

    if (this.recentlyTargeting) {
      this.recentlyTargeting = false;
      this.changingDirection(nowMs);
    }
    const thrustVector: Vector = this.getThrustVector();

    if (this.maintainAltitude)
    {
      const counteringThrustAgainstGravity: number = -this.getScaledGravity();
      const desiredThrust: number = counteringThrustAgainstGravity / thrustVector.y;
      this.thrust = desiredThrust / (MarkFliesGame.engineThrustPerLevel * this.spaceshipProxy.scale);
      if (this.thrust < MarkFliesGame.minimumThrust)
        this.thrust = MarkFliesGame.minimumThrust;
      if (this.thrust > MarkFliesGame.maximumThrust)
        this.thrust = MarkFliesGame.maximumThrust;
    }
    const availableThrust: number = this.getAvailableThrust();
    const horizontalThrust: number = thrustVector.x * availableThrust;
    const verticalThrust: number = thrustVector.y * availableThrust;  
    this.overrideThrust(nowMs, horizontalThrust, verticalThrust);
  }

  getAvailableThrust(): number {
    return MarkFliesGame.engineThrustPerLevel * this.spaceshipProxy.scale * this.thrust;
  }

  getThrustVector(): Vector {
    return this.getVectorToShipCenter(this.getMarkHeadCenterPointIncludingTilt(this.spaceshipCenter)).normalize(1);
  }

  getScaledGravity(): number {
    return MarkFliesGame.gravitationalForce * this.spaceshipProxy.scale;
  }

  overrideThrust(nowMs: number, horizontalThrust: number, verticalThrust: number) {
    this.changingDirection(nowMs);
    this.spaceshipProxy.horizontalThrustOverride = horizontalThrust;
    this.flameProxy.horizontalThrustOverride = horizontalThrust;

    const verticalThrustWithGravity: number = verticalThrust + this.getScaledGravity();
    this.spaceshipProxy.verticalThrustOverride = verticalThrustWithGravity;
    this.flameProxy.verticalThrustOverride = verticalThrustWithGravity;
  }

  removeAllGameElements(now: number): void {
    super.removeAllGameElements(now);
  }

  initialize() {
    super.initialize();
    Folders.assets = 'GameDev/Assets/MarkFlies/';
  }

  start() {
    super.start();
  }

  loadResources(): void {
    super.loadResources();

    this.spaceshipSprites = new Sprites(`Spaceships/Donut/Donut`, 301, fps30, AnimationStyle.Loop, true);
    this.spaceshipSprites.originX = 320;
    this.spaceshipSprites.originY = 150;

    this.flameSprites = new Sprites(`Spaceships/Donut/Flames`, 301, fps30, AnimationStyle.Loop, true);
    this.flameSprites.originX = 332;
    this.flameSprites.originY = -29;

    this.spaceshipSprites.moves = true;
    this.flameSprites.moves = true;
  }

  controlSpaceship(command: string, data: string, userInfo: UserInfo, nowMs: number) {
    if (command === 'TestDeploy')
      this.testDeploy(data);
    if (command === 'Deploy')
      this.deploy(userInfo, data);
    if (command === 'Tilt')
      this.tilt(userInfo, data, nowMs);
    if (command === 'Thrust')
      this.setThrust(userInfo, data, nowMs);
    if (command === 'Goto')
      this.goto(userInfo, data, nowMs);
    else if (command === 'ResetMark')
      this.resetMark();
    //Tilt
    //Thrusters(Up / Down)
    //Back / Forward
    //Message
    //
    //Pudding
    //Lasers
    //Candy
    //Seeds
  }

  getMarkHeight(scale: number): number {
    return this.markHeight * scale;
  }

  get spaceshipCenter(): Point {
    return new Point(this.spaceshipProxy.x + this.spaceshipSprites.originX, this.spaceshipProxy.y + this.spaceshipSprites.originY);
  }

  getVectorToShipCenter(target: Point): Vector {
    const shipCenter: Point = this.spaceshipCenter;
    return new Vector(target.x - shipCenter.x, target.y - shipCenter.y);
  }

  moveImmediate(target: Point) {
    const delta: Vector = this.getVectorToShipCenter(target);
    this.moveImmediateDelta(delta.x, delta.y, performance.now());
  }

  setScale(targetScale: number) {
    this.spaceshipProxy.scale = targetScale;
    this.flameProxy.scale = targetScale * this.thrust / 6;
  }

  static readonly maximumThrust: number = 9.9;
  static readonly minimumThrust: number = 0;
  static readonly maximumChatRoomValue: number = 9.9;

  mapTarget(value: number, start: number, end: number): number {
    const percentComplete: number = Math.min(Math.max(value / MarkFliesGame.maximumChatRoomValue, 0), 1);
    return start + (end - start) * percentComplete;
  }

  thrust: number = 5;
  lastThrust: number = 5;

  setThrust(userInfo: UserInfo, data: string, nowMs: number) {
    const thrustValue: number = parseFloat(data);
    this.thrust = this.mapTarget(thrustValue, 1, 9);
    this.changingDirection(nowMs);
    this.updateThrustFromTilt(nowMs);
  }

  tilt(userInfo: UserInfo, data: string, nowMs) {
    let angle: number = parseFloat(data);
    this.easeTilt(angle, nowMs);
  }

  easeTilt(angle: number, nowMs: number, tiltTime: number = 1500) {
    if (this.spaceshipProxy) {
      let degreesToMove: number = this.getDegreesToRotate(angle, this.spaceshipProxy);
      this.changingDirection(nowMs);
      this.flameProxy.storeLastPosition();
      this.spaceshipProxy.easeSpin(nowMs, this.spaceshipProxy.rotation, angle, tiltTime);
      this.flameProxy.easeSpin(nowMs, this.flameProxy.rotation, angle, tiltTime);
    }
  }

  deploy(userInfo: UserInfo, data: string) {
    let shipColor: number = 300;
    let flameColor: number = 220;

    if (data) {
      const parts: string[] = data.split(',');
      if (parts.length >= 2) {
        shipColor = parseFloat(parts[0].trim());
        flameColor = parseFloat(parts[1].trim());
      }
    }

    this.createNewSpaceship(this.markDefaultX, this.markDefaultY, this.markDefaultScale, this.markDefaultRotation, shipColor, flameColor);
    this.updateVideoFeedFromShip();
  }

  executeCommand(command: string, params: string, userInfo: UserInfo, now: number): boolean {
    if (super.executeCommand(command, params, userInfo, now))
      return true;

    if (command === "Swat") {
      //this.destroyAllDronesOverMark();
      return true;
    }
    return false;
  }

  test(testCommand: string, userInfo: UserInfo, now: number): boolean {
    if (super.test(testCommand, userInfo, now))
      return true;

    if (testCommand === 'donut') {
      this.testDeploy("300, 220");

      return true;
    }

    return false;
  }

  private testDeploy(data: string) {
    let shipColor: number = 300;
    let flameColor: number = 220;
    let X: number = 600;
    let Y: number = 340;

    let scale: number = 0.5;
    let rotation: number = 30;

    if (data) {
      const parts: string[] = data.split(',');
      if (parts.length >= 2) {
        shipColor = parseFloat(parts[0].trim());
        flameColor = parseFloat(parts[1].trim());
        if (parts.length >= 4) {
          X = parseInt(parts[2].trim());
          Y = parseInt(parts[3].trim());
          if (parts.length >= 5) {
            scale = parseFloat(parts[4].trim());
            if (parts.length >= 6) {
              rotation = parseFloat(parts[5].trim());
            }
          }
        }
      }
    }

    this.createNewSpaceship(X, Y, scale, rotation, shipColor, flameColor);
    this.updateVideoFeedFromShip();
  }

  lastSpaceshipX: number;
  lastSpaceshipY: number;
  lastSpaceshipScale: number;
  lastSpaceshipRotation: number;

  private updateVideoFeedFromShip() {
    if (!this.spaceshipProxy)
      return;

    if (this.lastSpaceshipX === this.spaceshipProxy.x &&
      this.lastSpaceshipY === this.spaceshipProxy.y &&
      this.lastSpaceshipScale === this.spaceshipProxy.scale &&
      this.lastSpaceshipRotation === this.spaceshipProxy.rotation)
      return;

    this.lastSpaceshipX = this.spaceshipProxy.x;
    this.lastSpaceshipY = this.spaceshipProxy.y;
    this.lastSpaceshipScale = this.spaceshipProxy.scale;
    this.lastSpaceshipRotation = this.spaceshipProxy.rotation;

    this.updateVideoFeedFromParams(this.lastSpaceshipX + this.spaceshipSprites.originX, this.lastSpaceshipY + this.spaceshipSprites.originY, this.lastSpaceshipScale, this.lastSpaceshipRotation);
  }

  resetMark() {
    console.log(`Resetting Mark...`);
    if (this.spaceshipProxy) {
      this.spaceshipProxy.fadeOutAfter(0);
      this.flameProxy.fadeOutAfter(0);
      this.spaceshipProxy = null;
    }
    this.updateVideoFeedFromParams(this.markDefaultX, this.markDefaultY, this.markDefaultScale, this.markDefaultRotation);
  }

  updateVideoFeedFromParams(X: number, Y: number, scale: number, rotation: number) {
    const videoFeedDto: VideoFeedDto = new VideoFeedDto(X, Y, scale, rotation);
    updateVideoFeed(JSON.stringify(videoFeedDto));
  }

  private createNewSpaceship(X: number, Y: number, scale: number, rotation: number, shipColor: number, flameColor: number) {
    if (this.spaceshipProxy) {
      this.spaceshipProxy.destroyBy(1000);
      this.flameProxy.destroyBy(1000);
    }

    const flyTime: number = 60000;
    this.spaceshipProxy = this.spaceshipSprites.addShifted(X, Y, 0, shipColor);
    this.spaceshipProxy.destroyAllInExactly(flyTime);
    this.spaceshipProxy.fadeInTime = 500;
    this.spaceshipProxy.fadeOutTime = 500;
    this.spaceshipProxy.scale = scale;
    this.spaceshipProxy.rotation = rotation;
    const counteringThrustAgainstGravity: number = -this.getScaledGravity();
    this.spaceshipProxy.verticalThrustOverride = counteringThrustAgainstGravity;

    this.flameProxy = this.flameSprites.addShifted(X, Y, 0, flameColor);
    this.flameProxy.destroyAllInExactly(flyTime);
    this.flameProxy.fadeInTime = 500;
    this.flameProxy.fadeOutTime = 500;
    this.flameProxy.scale = scale;
    this.flameProxy.rotation = rotation;
    this.flameProxy.verticalThrustOverride = counteringThrustAgainstGravity;
  }

  destroying(): void {

  }

  rotatePoint(pointToRotate: Point, angle: number, centerPoint: Point): Point {
    const s: number = Math.sin(angle);
    const c: number = Math.cos(angle);

    // translate point back to origin:
    const deltaX = pointToRotate.x - centerPoint.x;
    const deltaY = pointToRotate.y - centerPoint.y;

    // rotate point
    const newX: number = deltaX * c - deltaY * s;
    const newY: number = deltaX * s + deltaY * c;

    // translate point back:
    return new Point(newX + centerPoint.x, newY + centerPoint.y);
  }

  readonly markDefaultX: number = 1175;
  readonly markDefaultY: number = 1050;
  static readonly engineThrustFullThrottle: number = 2.8 * 4;
  static readonly engineThrustPerLevel: number = MarkFliesGame.engineThrustFullThrottle / 9;
  static readonly gravitationalForce: number = MarkFliesGame.engineThrustPerLevel * 5;
  //readonly markDefaultX: number = 1175;
  // readonly markDefaultY: number = 550;
  readonly markDefaultScale: number = 1;

  readonly markDefaultRotation: number = 0;

  markHeadScreenCenter: Point;

  readonly markHeight: number = 350;

  keepSpaceshipInBounds(nowMs: number) {
    if (!this.spaceshipProxy)
      return;


    //const markScaledHeight: number = this.markHeight * scale;

    this.markHeadBox = this.getMarkHeadBox(this.spaceshipCenter);

    // TODO: If rotation puts Mark's head out of bounds - we need to check for that and bring it back.

    if (this.spaceshipProxy.isRotating()) {
      // We are rotating!!! Force in bounds if needed.
      console.log(`We are rotating!!! Force in bounds if needed.`);
      this.forceInBounds(nowMs);
      return;
    }

    if (this.markHeadBox.top < 0 || this.markHeadBox.bottom > screenHeight) {
      this.spaceshipProxy.bounceBack(nowMs, Orientation.Horizontal);
      this.flameProxy.bounceBack(nowMs, Orientation.Horizontal);
    }

    if (this.markHeadBox.left < 0 || this.markHeadBox.right > screenWidth) {
      this.spaceshipProxy.bounceBack(nowMs, Orientation.Vertical);
      this.flameProxy.bounceBack(nowMs, Orientation.Vertical);
    }
  }

  getMarkHeadBox(shipCenter: Point): HeadBox {
    this.markHeadScreenCenter = this.getMarkHeadCenterPointIncludingTilt(shipCenter);

    const scale: number = this.spaceshipProxy.scale;
    const markHeadHeight: number = this.markHeight / 4;
    const top: number = this.markHeadScreenCenter.y - markHeadHeight * scale;
    const bottom: number = this.markHeadScreenCenter.y + markHeadHeight * scale;
    const left: number = this.markHeadScreenCenter.x - markHeadHeight * scale;
    const right: number = this.markHeadScreenCenter.x + markHeadHeight * scale;
    return new HeadBox(left, top, right, bottom);
  }

  getMarkHeadCenterPointIncludingTilt(shipCenter: Point) {
    const scale: number = this.spaceshipProxy.scale;
    const markHeadCenterY: number = shipCenter.y - this.getMarkHeight(scale);
    const markCenterHead = new Point(shipCenter.x, markHeadCenterY);
    const angleInRadians: number = this.spaceshipProxy.rotation * Math.PI / 180;
    return this.rotatePoint(markCenterHead, angleInRadians, shipCenter);
  }

  forceInBounds(nowMs: number) {
    let deltaX: number = 0;
    let deltaY: number = 0;

    if (this.markHeadBox.top < 0) {
      deltaY = -this.markHeadBox.top;
    }

    if (this.markHeadBox.bottom > screenHeight) {
      deltaY = screenHeight - this.markHeadBox.bottom;
    }

    if (this.markHeadBox.left < 0) {
      deltaX = -this.markHeadBox.left;
    }

    if (this.markHeadBox.right > screenWidth) {
      deltaX = screenWidth - this.markHeadBox.right;
    }

    if (deltaY === 0 && deltaX === 0)
      return;

    this.moveImmediateDelta(deltaX, deltaY, nowMs);
  }

  moveImmediateDelta(deltaX: number, deltaY: number, nowMs: number) {
    this.moveDelta(deltaX, deltaY);
    this.changingDirection(nowMs);
  }

  twoSecondTrajectoryPoint: Point;

  changingDirection(nowMs: number) {
    this.spaceshipProxy.changingDirection(nowMs);
    this.flameProxy.changingDirection(nowMs);
  }

  moveDelta(deltaX: number, deltaY: number) {
    this.spaceshipProxy.move(deltaX, deltaY);
    this.flameProxy.move(deltaX, deltaY);
  }

  readonly timeToTargetSeconds: number = 2;

  wallBounceRect: Rect;
  readonly wallBounceRectThickness: number = 100;
  targetingState: TargetingState;

  targetXYZ(x: number, y: number, z: number) {
    this.targetingState = TargetingState.startedTargeting;
    this.targetScale = this.mapTarget(z, 0.25, 1.1);

    this.targetX = this.mapTarget(x, 0, 1919);
    this.targetY = this.mapTarget(y, 0, 1079) + this.getMarkHeight(this.targetScale);

    this.setScale(this.targetScale);
    this.targeting = true;
    this.targetingStartTime = this.nowMs;
    this.targetingStopTime = performance.now() + this.timeToTargetSeconds * 1000;

    // TODO: Why is the left wall not indicating a bounce??? WE DON'T KNOW!!!! Console log could help us here.
    // TODO: Trajectory does not account for bouncing.
    // Figure out the current head position relative to the spaceship center.
    this.twoSecondTrajectoryPoint = this.getTrajectory(this.timeToTargetSeconds);
    const trajectoryHeadBox = this.getMarkHeadBox(this.twoSecondTrajectoryPoint);
    const currentHeadBox = this.getMarkHeadBox(this.spaceshipCenter);

    let timeToReachTheTop: number = Number.MAX_VALUE;
    let timeToReachTheBottom: number = Number.MAX_VALUE;
    let timeToReachTheLeft: number = Number.MAX_VALUE;
    let timeToReachTheRight: number = Number.MAX_VALUE;

    if (trajectoryHeadBox.top < 0) {
      timeToReachTheTop = this.getVerticalCollisionTime(currentHeadBox.top);
    }

    if (trajectoryHeadBox.bottom > screenHeight) {
      const distanceToBottomPixels: number = screenHeight - currentHeadBox.bottom;
      timeToReachTheBottom = this.getVerticalCollisionTime(distanceToBottomPixels);
    }

    if (trajectoryHeadBox.left < 0) {
      timeToReachTheLeft = this.getHorizontalCollisionTime(currentHeadBox.left);
    }

    if (trajectoryHeadBox.right > screenWidth) {
      const distanceToRightPixels: number = screenWidth - currentHeadBox.right;
      timeToReachTheRight = this.getHorizontalCollisionTime(distanceToRightPixels);
    }

    const shortestTimeToBorder: number = Math.min(timeToReachTheLeft, timeToReachTheRight, timeToReachTheBottom, timeToReachTheTop);

    this.wallBounceRect = undefined;
    if (shortestTimeToBorder < Number.MAX_VALUE) {
      // We have a bounce on at least one of the four walls.
      if (timeToReachTheTop == shortestTimeToBorder) {
        this.wallBounceRect = new Rect(0, 0, screenWidth, this.wallBounceRectThickness);
      }
      else if (timeToReachTheLeft == shortestTimeToBorder) {
        this.wallBounceRect = new Rect(0, 0, this.wallBounceRectThickness, screenHeight);
      }
      else if (timeToReachTheRight == shortestTimeToBorder) {
        this.wallBounceRect = new Rect(screenWidth - this.wallBounceRectThickness, 0, screenWidth, screenHeight);
      }
      else if (timeToReachTheBottom == shortestTimeToBorder) {
        this.wallBounceRect = new Rect(0, screenHeight - this.wallBounceRectThickness, screenWidth, screenHeight);
      }
    }

    const deltaX: number = this.targetX - this.twoSecondTrajectoryPoint.x;
    const maxAngle: number = 70;
    const angle: number = maxAngle * deltaX / screenWidth;
    this.easeTilt(angle, performance.now());
    this.intervalBetweenUpdates = 2000;
    this.nextCourseCorrection = this.nowMs + this.intervalBetweenUpdates;
    // TODO: We might calc thrust based on overall distance to target.
    // TODO: We still need to update as we move. I want to be careful modifying the tilt in the first moments
  }

  getHorizontalCollisionTime(distancePixels: number): number {
    const distanceMeters: number = Physics.pixelsToMeters(distancePixels);
    let time: number = Physics.getDropTimeSeconds(distanceMeters, this.spaceshipProxy.getHorizontalThrust(this.nowMs), this.spaceshipProxy.velocityX);
    if (time < 0)
      time = Number.MAX_VALUE;
    return time;
  }

  getVerticalCollisionTime(distancePixels: number): number {
    const distanceMeters: number = Physics.pixelsToMeters(distancePixels);
    let time: number = Physics.getDropTimeSeconds(distanceMeters, this.spaceshipProxy.getVerticalThrust(this.nowMs), this.spaceshipProxy.velocityY);
    if (time < 0)
      time = Number.MAX_VALUE;
    return time;
  }

  private getTrajectory(timeToTargetSeconds: number) {
    const deltaToCurrentTrajectoryX: number = Physics.metersToPixels(Physics.getDisplacementMeters(timeToTargetSeconds, this.spaceshipProxy.velocityX, 0));
    const deltaToCurrentTrajectoryY: number = Physics.metersToPixels(Physics.getDisplacementMeters(timeToTargetSeconds, this.spaceshipProxy.velocityY, this.getScaledGravity()));
    const shipCenter: Point = this.spaceshipCenter;
    const finalPoint: Point = new Point(shipCenter.x + deltaToCurrentTrajectoryX, shipCenter.y + deltaToCurrentTrajectoryY);
    return finalPoint;
  }

  goto(userInfo: UserInfo, data: string, nowMs) {
    let x: number;
    let y: number;
    let z: number;
    if (data.length == 3) {
      x = parseFloat(data[0]);
      y = parseFloat(data[1]);
      z = parseFloat(data[2]);
    }
    else if (data.length == 6) {
      x = parseFloat(data[0] + '.' + data[1]);
      y = parseFloat(data[2] + '.' + data[3]);
      z = parseFloat(data[4] + '.' + data[5]);
    }
    else
      return;
    this.targetXYZ(x, y, z);
  }

  nextCourseCorrection: number;
  intervalBetweenUpdates: number;

  flyMarkTowardTarget(nowMs: number) {
    if (this.targeting !== true)
      return;
    if (nowMs < this.nextCourseCorrection)
      return;
    this.nextCourseCorrection += this.intervalBetweenUpdates;
    if (this.targetingState === TargetingState.startedTargeting)
      this.startCourseCorrection(nowMs);
    else if (this.targetingState === TargetingState.mainThrustTowardsCourse)
      this.mainThrustTowardsTarget(nowMs);
    else if (this.targetingState === TargetingState.coastingTowardsTargetX) {
      this.applyBrakingHorizontalThrust(nowMs);
    }
    else if (this.targetingState === TargetingState.breakingForTargetX) {
      this.headTowardTargetY(nowMs);
      this.targeting = false;  // Testing
    }

    // thrustVector.x
  }

  applyBrakingHorizontalThrust(nowMs: number) {

  }

  headTowardTargetY(nowMs: number) {
    this.targetingState = TargetingState.movingToTargetY;
  }

  mainThrustTowardsTarget(nowMs: number) {
    //const thrustVector: Vector = this.getThrustVector();
    //const availableThrust: number = this.getAvailableThrust();
    const markHeadCenter: Point = this.getMarkHeadCenterPointIncludingTilt(this.spaceshipCenter);

    const rotationDegrees: number = this.spaceshipProxy.rotation;



    // TODO: Adjust tilt to always point Mark toward target. But between -60 and +60 degresss.
    const pixelsToTargetX: number = this.targetX - markHeadCenter.x;
    const metersToTargetX: number = Physics.pixelsToMeters(pixelsToTargetX);
    if (this.spaceshipProxy.velocityX != 0) {
      const secondsToTargetX: number = Physics.getDropTimeSeconds(metersToTargetX, 0, this.spaceshipProxy.velocityX);
      console.log(`secondsToTargetX = ${secondsToTargetX}`);
      const goalBrakeTimeSeconds: number = 2;

      if (secondsToTargetX < goalBrakeTimeSeconds && this.targetingState === TargetingState.mainThrustTowardsCourse) {
        this.targetingState = TargetingState.coastingTowardsTargetX;

        const horizontalThrust: number = Physics.getThrustToDisplace(-metersToTargetX, this.spaceshipProxy.velocityX, goalBrakeTimeSeconds);
        const verticalThrust: number = -MarkFliesGame.gravitationalForce;
        this.thrust = 3;
        let brakeTimeSeconds: number = secondsToTargetX / 2;
        this.easeTilt(-this.spaceshipProxy.rotation, nowMs, brakeTimeSeconds * 1000);
        this.nextCourseCorrection = nowMs + brakeTimeSeconds * 1000;
      }
    }
  }
  startCourseCorrection(nowMs: number) {
    this.targetingState = TargetingState.mainThrustTowardsCourse;
    this.intervalBetweenUpdates = 100;
  }

}