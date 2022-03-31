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

  constructor(context: CanvasRenderingContext2D) {
    super(context);
  }

  updateScreen(context: CanvasRenderingContext2D, nowMs: number) {
    super.updateScreen(context, nowMs);

    this.spaceshipSprites.updatePositions(nowMs);
    this.flameSprites.updatePositions(nowMs);

    this.keepSpaceshipInBounds(nowMs);

    this.flameSprites.draw(markFliesContext, nowMs);
    this.spaceshipSprites.draw(markFliesContext, nowMs);

    this.animations.removeExpiredAnimations(nowMs);
    this.animations.render(markFliesContext, nowMs);

    this.updateThrustFromTilt();
    this.updateVideoFeedFromShip();
  }

  updateThrustFromTilt() {
    if (!this.spaceshipProxy)
      return;

    if (this.spaceshipProxy.rotation == this.lastSpaceshipRotation)
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
    const horizontalThrust: number = horizontalFactor * MarkFliesGame.engineThrust * scale;
    const verticalThrust: number = verticalFactor * MarkFliesGame.engineThrust * scale;
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
    this.flameSprites.originY = -79;

    this.spaceshipSprites.moves = true;
    this.flameSprites.moves = true;
  }

  controlSpaceship(command: string, data: string, userInfo: UserInfo, nowMs: number) {
    if (command === "TestDeploy")
      this.testDeploy(data);
    if (command === "Deploy")
      this.deploy(userInfo, data);
    if (command === "Tilt")
      this.tilt(userInfo, data, nowMs);
    else if (command === "ResetMark")
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

  tilt(userInfo: UserInfo, data: string, nowMs) {
    let angle: number = parseFloat(data);
    if (this.spaceshipProxy) {
      let degreesToMove: number = this.getDegreesToRotate(angle, this.spaceshipProxy);
      let now: number = performance.now();
      this.spaceshipProxy.changingDirection(now);
      this.flameProxy.changingDirection(now);
      this.flameProxy.storeLastPosition();
      this.spaceshipProxy.rotateTo(angle, degreesToMove, 1500);
      this.flameProxy.rotateTo(angle, degreesToMove, 1500);
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

    const flyTime: number = 40000;
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

  readonly markDefaultX: number = 1175;
  readonly markDefaultY: number = 1050;
  static readonly engineThrust: number = 1.6;
 //readonly markDefaultX: number = 1175;
 // readonly markDefaultY: number = 550;
  readonly markDefaultScale: number = 1;
  readonly markDefaultRotation: number = 0;
  

  keepSpaceshipInBounds(nowMs: number) {
    if (!this.spaceshipProxy)
      return;

    const scale: number = this.spaceshipProxy.scale;
    const markHeight: number = 430;
    const markHeadHeight: number = markHeight / 3;
    const shipWidth: number = 600;
    const markScaledHeight: number = markHeight * scale;

    // TODO: Use trig to calculate Mark head position.
    const shipCenterX: number = this.spaceshipProxy.x + this.spaceshipSprites.originX;
    const shipLeftX: number = shipCenterX - shipWidth * scale / 2;
    const shipRightX: number = shipCenterX + shipWidth * scale / 2;
    const markHeadTop: number = this.spaceshipProxy.y + this.spaceshipSprites.originY - markScaledHeight;
    const markHeadBottom: number = markHeadTop + markHeadHeight * scale;

    if (markHeadTop < 0 || markHeadBottom > screenHeight) {
      this.spaceshipProxy.bounceBack(nowMs, Orientation.Horizontal);
      this.flameProxy.bounceBack(nowMs, Orientation.Horizontal);
    }

    if (shipLeftX < 0 || shipRightX > screenWidth)
    {
      this.spaceshipProxy.bounceBack(nowMs, Orientation.Vertical);
      this.flameProxy.bounceBack(nowMs, Orientation.Vertical);
    }
  }
}