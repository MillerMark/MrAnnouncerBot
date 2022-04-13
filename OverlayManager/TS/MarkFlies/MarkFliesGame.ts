class VideoFeedDto {
  constructor(public X: number, public Y: number, public Scale: number, public Rotation: number) {

  }
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
  stopTargetingTime: number;
  targetScale: number;
  targetingStartTime: number;

  constructor(context: CanvasRenderingContext2D) {
    super(context);
  }

  updateScreen(context: CanvasRenderingContext2D, nowMs: number) {
    super.updateScreen(context, nowMs);

    this.spaceshipSprites.updatePositions(nowMs);
    this.flameSprites.updatePositions(nowMs);
    if (this.targeting)
      this.moveSpaceshipTowardTarget(nowMs);

    this.keepSpaceshipInBounds(nowMs);

    this.flameSprites.draw(markFliesContext, nowMs);
    this.spaceshipSprites.draw(markFliesContext, nowMs);

    this.animations.removeExpiredAnimations(nowMs);
    this.animations.render(markFliesContext, nowMs);

    this.updateThrustFromTilt(nowMs);
    this.updateVideoFeedFromShip();

    this.updateThrust();

    this.drawDiagnostics(context, nowMs);
  }

  updateThrust() {
    if (this.lastThrust === this.thrust)
      return;
    this.lastThrust = this.thrust;
    this.setScale(this.spaceshipProxy.scale);
  }

  drawDiagnostics(context: CanvasRenderingContext2D, nowMs: number) {
    if (!this.spaceshipProxy)
      return;
    //this.drawBoxAroundMarksHead(context);
    this.drawFinalTrajectory(context);
    this.drawSpaceshipCenter(context);
  }

  drawSpaceshipCenter(context: CanvasRenderingContext2D) {
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

  drawFinalTrajectory(context: CanvasRenderingContext2D) {
    if (!this.finalPoint)
      return;
    const edgeLength: number = 20;
    const halfEdgeLength: number = edgeLength / 2;
    context.beginPath();
    context.strokeStyle = "#4040ff";
    context.lineWidth = 2;
    context.rect(this.finalPoint.x - halfEdgeLength, this.finalPoint.y - halfEdgeLength, edgeLength, edgeLength);
    context.fillStyle = "#ffffff";
    context.fillRect(this.finalPoint.x - halfEdgeLength, this.finalPoint.y - halfEdgeLength, edgeLength, edgeLength);
    context.stroke();
    context.fill();
  }

  drawBoxAroundMarksHead(context: CanvasRenderingContext2D) {
    context.beginPath();
    context.strokeStyle = "#ff0000";
    context.lineWidth = 2;
    context.rect(this.markHeadLeft, this.markHeadTop, this.markHeadRight - this.markHeadLeft, this.markHeadBottom - this.markHeadTop);
    context.stroke();
  }

  recentlyTargeting: boolean;

  updateThrustFromTilt(nowMs: number) {
    if (!this.spaceshipProxy)
      return;

    if (this.stopTargetingTime && performance.now() < this.stopTargetingTime) {
      console.log(`this.overrideThrust(0, 0);`);
      this.overrideThrust(0, 0);
      this.recentlyTargeting = true;
      return;
    }

    if (this.recentlyTargeting) {
      this.recentlyTargeting = false;
      this.changingDirection(nowMs);
    }
    else if (this.spaceshipProxy.rotation == this.lastSpaceshipRotation)
      return;

    // TODO: Use trig to calculate correct percentage of horizontal and vertical thrust!!!

    let rotation: number = this.spaceshipProxy.rotation % 360;
    let horizontalFactor: number;
    let verticalFactor: number;

    if (rotation < 180)  // Moving right
    {
      horizontalFactor = (90 - Math.abs(rotation - 90)) / 90;
    }
    else {
      horizontalFactor = -(90 - Math.abs(rotation - 270)) / 90;
    }

    if (rotation < 90)  // Moving up
    {
      verticalFactor = -(90 - Math.abs(rotation)) / 90;
    }
    else if (rotation > 270)  // Moving up
    {
      verticalFactor = -(90 - Math.abs(rotation - 360)) / 90;
    }
    else {  // Moving down...
      verticalFactor = (90 - Math.abs(rotation - 180)) / 90;
    }

    const scale: number = this.spaceshipProxy.scale;
    const horizontalThrust: number = horizontalFactor * MarkFliesGame.engineThrust * scale * this.thrust;
    const verticalThrust: number = verticalFactor * MarkFliesGame.engineThrust * scale * this.thrust;
    this.overrideThrust(horizontalThrust, verticalThrust);
  }

  overrideThrust(horizontalThrust: number, verticalThrust: number) {
    this.spaceshipProxy.horizontalThrustOverride = horizontalThrust;
    this.flameProxy.horizontalThrustOverride = horizontalThrust;
    this.spaceshipProxy.verticalThrustOverride = verticalThrust;
    this.flameProxy.verticalThrustOverride = verticalThrust;
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

  getDeltaToShipCenter(targetX: number, targetY: number): Point {
    const shipCenter: Point = this.spaceshipCenter;
    return new Point(targetX - shipCenter.x, targetY - shipCenter.y);
  }

  moveImmediate(targetX: number, targetY: number) {
    const delta: Point = this.getDeltaToShipCenter(targetX, targetY);
    this.moveImmediateDelta(delta.x, delta.y, performance.now());
  }

  setScale(targetScale: number) {
    this.spaceshipProxy.scale = targetScale;
    this.flameProxy.scale = targetScale * this.thrust / 6;
  }

  mapTarget(value: number, start: number, end: number): number {
    const percentComplete: number = Math.min(Math.max(value / 9.9, 0), 1);
    return start + (end - start) * percentComplete;
  }

  thrust: number = 5;
  lastThrust: number = 5;

  setThrust(userInfo: UserInfo, data: string, nowMs) {
    const thrustValue: number = parseFloat(data);
    this.thrust = this.mapTarget(thrustValue, 1, 9);
  }

  tilt(userInfo: UserInfo, data: string, nowMs) {
    let angle: number = parseFloat(data);
    this.easeTilt(angle, nowMs);
  }

  easeTilt(angle: number, nowMs: number) {
    if (this.spaceshipProxy) {
      let degreesToMove: number = this.getDegreesToRotate(angle, this.spaceshipProxy);
      this.changingDirection(nowMs);
      this.flameProxy.storeLastPosition();
      this.spaceshipProxy.easeSpin(nowMs, this.spaceshipProxy.rotation, angle, 1500);
      this.flameProxy.easeSpin(nowMs, this.flameProxy.rotation, angle, 1500);
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
    this.spaceshipProxy.verticalThrustOverride = -MarkFliesGame.engineThrust * scale;

    this.flameProxy = this.flameSprites.addShifted(X, Y, 0, flameColor);
    this.flameProxy.destroyAllInExactly(flyTime);
    this.flameProxy.fadeInTime = 500;
    this.flameProxy.fadeOutTime = 500;
    this.flameProxy.scale = scale;
    this.flameProxy.rotation = rotation;
    this.flameProxy.verticalThrustOverride = -MarkFliesGame.engineThrust * scale;
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
  static readonly engineThrust: number = 2.8 / 9;
  //readonly markDefaultX: number = 1175;
  // readonly markDefaultY: number = 550;
  readonly markDefaultScale: number = 1;

  readonly markDefaultRotation: number = 0;
  markHeadTop: number;
  markHeadBottom: number;
  markHeadLeft: number;
  markHeadRight: number;

  markScreenCenter: Point;

  readonly markHeight: number = 350;

  keepSpaceshipInBounds(nowMs: number) {
    if (!this.spaceshipProxy)
      return;

    const scale: number = this.spaceshipProxy.scale;

    const markHeadHeight: number = this.markHeight / 4;
    const shipWidth: number = 600;
    const markScaledHeight: number = this.markHeight * scale;

    const currentRotationDegrees: number = this.spaceshipProxy.rotation;

    // TODO: Use trig to calculate Mark head position.
    const shipCenterX: number = this.spaceshipProxy.x + this.spaceshipSprites.originX;
    const shipCenterY: number = this.spaceshipProxy.y + this.spaceshipSprites.originY;
    const shipCenter: Point = new Point(shipCenterX, shipCenterY);
    const shipLeftX: number = shipCenterX - shipWidth * scale / 2;
    const shipRightX: number = shipCenterX + shipWidth * scale / 2;

    const markHeadCenterY: number = shipCenterY - this.getMarkHeight(scale);
    const markCenterHead = new Point(shipCenterX, markHeadCenterY);
    const angleInRadians: number = this.spaceshipProxy.rotation * Math.PI / 180;
    this.markScreenCenter = this.rotatePoint(markCenterHead, angleInRadians, shipCenter);

    this.markHeadTop = this.markScreenCenter.y - markHeadHeight * scale;
    this.markHeadBottom = this.markScreenCenter.y + markHeadHeight * scale;
    this.markHeadLeft = this.markScreenCenter.x - markHeadHeight * scale;
    this.markHeadRight = this.markScreenCenter.x + markHeadHeight * scale;

    // TODO: If rotation puts Mark's head out of bounds - we need to check for that and bring it back.

    if (this.spaceshipProxy.isRotating()) {
      // We are rotating!!! Force in bounds if needed.
      console.log(`We are rotating!!! Force in bounds if needed.`);
      this.forceInBounds(nowMs);
      return;
    }

    if (this.markHeadTop < 0 || this.markHeadBottom > screenHeight) {
      this.spaceshipProxy.bounceBack(nowMs, Orientation.Horizontal);
      this.flameProxy.bounceBack(nowMs, Orientation.Horizontal);
    }

    if (this.markHeadLeft < 0 || this.markHeadRight > screenWidth) {
      this.spaceshipProxy.bounceBack(nowMs, Orientation.Vertical);
      this.flameProxy.bounceBack(nowMs, Orientation.Vertical);
    }
  }

  forceInBounds(nowMs: number) {
    let deltaX: number = 0;
    let deltaY: number = 0;

    if (this.markHeadTop < 0) {
      deltaY = -this.markHeadTop;
    }

    if (this.markHeadBottom > screenHeight) {
      deltaY = screenHeight - this.markHeadBottom;
    }

    if (this.markHeadLeft < 0) {
      deltaX = -this.markHeadLeft;
    }

    if (this.markHeadRight > screenWidth) {
      deltaX = screenWidth - this.markHeadRight;
    }

    if (deltaY === 0 && deltaX === 0)
      return;

    this.moveImmediateDelta(deltaX, deltaY, nowMs);
  }

  moveImmediateDelta(deltaX: number, deltaY: number, nowMs: number) {
    this.moveDelta(deltaX, deltaY);
    this.changingDirection(nowMs);
  }

  finalPoint: Point;

  changingDirection(nowMs: number) {
    this.spaceshipProxy.changingDirection(nowMs);
    this.flameProxy.changingDirection(nowMs);
  }

  moveDelta(deltaX: number, deltaY: number) {
    this.spaceshipProxy.move(deltaX, deltaY);
    this.flameProxy.move(deltaX, deltaY);
  }

  targetXYZ(x: number, y: number, z: number) {
    this.targetScale = this.mapTarget(z, 0.25, 1.1);

    this.targetX = this.mapTarget(x, 0, 1919);
    this.targetY = this.mapTarget(y, 0, 1079) + this.getMarkHeight(this.targetScale);
    console.log(`this.targetX = ${this.targetX}`);

    // TODO: Ease this:
    this.setScale(this.targetScale);
    this.targeting = true;
    this.targetingStartTime = this.nowMs;
    const timeToTargetSeconds: number = 2;
    this.stopTargetingTime = performance.now() + timeToTargetSeconds * 1000;
    console.log(`this.stopTargetingTime = ${this.stopTargetingTime}`);
    this.finalPoint = this.getTrajectory(timeToTargetSeconds);
    this.changingDirection(this.nowMs);
    this.overrideThrust(0, 0);

    // if finalPoint is out of bounds, we need to simulate the bounce back in.

    // We need to know where we will be based on velocity alone in ___ amount of time.
    // I want to use the delta to calculate our moves.
    // I want to do this (correcting thrust) a few times in the journey.
    const delta: Point = this.getDeltaToShipCenter(this.finalPoint.x, this.finalPoint.y);
    //if (delta.x > 0) {
    //  this.easeTilt(60, performance.now());
    //  // Moving right...

    //}
    //else
    //  this.easeTilt(-60, performance.now());

    // TODO: Calculate an initial needed thrust based on horizontal distance to target.
    //this.moveImmediate(this.targetX, this.targetY);
  }

  private getTrajectory(timeToTargetSeconds: number) {
    const deltaToCurrentTrajectoryX: number = Physics.metersToPixels(Physics.getDisplacementMeters(timeToTargetSeconds, this.spaceshipProxy.velocityX, 0));
    const deltaToCurrentTrajectoryY: number = Physics.metersToPixels(Physics.getDisplacementMeters(timeToTargetSeconds, this.spaceshipProxy.velocityY, 0));
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

  moveSpaceshipTowardTarget(nowMs: number) {
    // TODO: Fill this in later.
  }

}