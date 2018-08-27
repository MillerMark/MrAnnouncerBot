CodeRushedPosX = 86;
const CodeRushedPosY = 159;

const LeftEngineX = 58 - CodeRushedPosX;
const LeftEngineY = 168 - CodeRushedPosY;
const LeftEngineRetractionOffsetX = 32;
const LeftEngineRetractionOffsetY = 0;

const LeftFlameX = 12 - CodeRushedPosX;
const LeftFlameY = 178 - CodeRushedPosY;
const LeftFlameJiggleX = 1;
const LeftFlameJiggleY = 0;


const RightEngineX = 375 - CodeRushedPosX;
const RightEngineY = 170 - CodeRushedPosY;
const RightEngineRetractionOffsetX = -32;
const RightEngineRetractionOffsetY = 0;


const RightFlameX = 392 - CodeRushedPosX;
const RightFlameY = 181 - CodeRushedPosY;
const RightFlameJiggleX = 1;
const RightFlameJiggleY = 0;


const BottomLeftEngineX = 132 - CodeRushedPosX;
const BottomLeftEngineY = 210 - CodeRushedPosY;
const BottomLeftEngineRetractionOffsetX = 0;
const BottomLeftEngineRetractionOffsetY = -46;


const BottomRightEngineX = 279 - CodeRushedPosX;
const BottomRightEngineY = 210 - CodeRushedPosY;
const BottomRightEngineRetractionOffsetX = 0;
const BottomRightEngineRetractionOffsetY = -46;


const BottomLeftFlameX = 137 - CodeRushedPosX;
const BottomLeftFlameY = 228 - CodeRushedPosY;
const BottomLeftFlameJiggleX = 0;
const BottomLeftFlameJiggleY = 2;

const MiniBottomLeftFlameX = 149 - CodeRushedPosX;
const MiniBottomLeftFlameY = 240 - CodeRushedPosY;
const MiniBottomLeftFlameJiggleX = 0;
const MiniBottomLeftFlameJiggleY = 2;



const BottomRightFlameX = 284 - CodeRushedPosX;
const BottomRightFlameY = 228 - CodeRushedPosY;
const BottomRightFlameJiggleX = 0;
const BottomRightFlameJiggleY = 2;

const MiniBottomRightFlameX = 296 - CodeRushedPosX;
const MiniBottomRightFlameY = 240 - CodeRushedPosY;
const MiniBottomRightFlameJiggleX = 0;
const MiniBottomRightFlameJiggleY = 1;



const ChuteExtendX = 144 - CodeRushedPosX;
const ChuteExtendY = 6 - CodeRushedPosY;

class Rocket {
  constructor(x, y) {
    const upperLeftOffsetX = 9;
    const upperLeftOffsetY = 9;

    x -= upperLeftOffsetX;
    y -= upperLeftOffsetY;

    this.x = x;
    this.y = y;
    const flameFrameInterval = 60;
    this.codeRushedBody = new Part("CodeRushed", 1, PartStyle.Static, x, y);
    this.leftEngine = new Part("LeftEngine", 1, PartStyle.Static, LeftEngineX + x, LeftEngineY + y);
    this.rightEngine = new Part("RightEngine", 1, PartStyle.Static, RightEngineX + x, RightEngineY + y);
    this.bottomLeftEngine = new Part("BottomEngine", 1, PartStyle.Static, BottomLeftEngineX + x, BottomLeftEngineY + y);
    this.bottomRightEngine = new Part("BottomEngine", 1, PartStyle.Static, BottomRightEngineX + x, BottomRightEngineY + y);

    this.bottomLeftFlame = new Part("FlameBottom", 6, PartStyle.Random, BottomLeftFlameX + x, BottomLeftFlameY + y, flameFrameInterval, BottomLeftFlameJiggleX, BottomLeftFlameJiggleY);
    this.bottomRightFlame = new Part("FlameBottom", 6, PartStyle.Random, BottomRightFlameX + x, BottomRightFlameY + y, flameFrameInterval, BottomRightFlameJiggleX, BottomRightFlameJiggleY);

    this.bottomLeftMiniFlame = new Part("MiniFlameBottom", 6, PartStyle.Random, MiniBottomLeftFlameX + x, MiniBottomLeftFlameY + y, flameFrameInterval, MiniBottomLeftFlameJiggleX, MiniBottomLeftFlameJiggleY);
    this.bottomRightMiniFlame = new Part("MiniFlameBottom", 6, PartStyle.Random, MiniBottomRightFlameX + x, MiniBottomRightFlameY + y, flameFrameInterval, MiniBottomRightFlameJiggleX, MiniBottomRightFlameJiggleY);

    this.leftFlame = new Part("FlameLeft", 6, PartStyle.Random, LeftFlameX + x, LeftFlameY + y, flameFrameInterval * 0.6, LeftFlameJiggleX, LeftFlameJiggleY);
    this.rightFlame = new Part("FlameRight", 6, PartStyle.Random, RightFlameX + x, RightFlameY + y, flameFrameInterval * 0.6, RightFlameJiggleX, RightFlameJiggleY);

    const chuteFrameInterval = 200;
    this.chute = new Part("ChuteExtend", 5, PartStyle.Sequential, ChuteExtendX + x, ChuteExtendY + y, chuteFrameInterval);
    this.chuteDeployed = false;
    this.chuteSailsAreFull = false;
    this.chuteRetracting = false;

    this.move(0, 0);

    this.worldAcceleration = Gravity.earth;
    this.stopped = true;

    this.timeStart = new Date();

    this.width = 298;   // Widh
    this.height = 52;
  }

  deployChute() {
    this.chuteDeployed = true;
    this.chuteSailsAreFull = false;
  }

  getVerticalAcceleration() {
    if (this.chuteSailsAreFull)
      return 0;
    return this.worldAcceleration;
  }

  onChutesFullyOpen() {
    var currentTime = new Date();
    var secondsPassed = (currentTime - this.timeStart) / 1000;
    var newVelocity = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration());

    newVelocity /= 4;

    this.move(this.velocityX, newVelocity);
    this.chuteSailsAreFull = true;
  }

  retractChutes() {
    this.move(this.velocityX, this.velocityY);
    this.chuteDeployed = false;
    this.chuteSailsAreFull = false;
    this.chuteRetracting = true;
    this.chute.reverse = true;
  }

  getHorizontalAcceleration() {
    return 0;
  }

  bounce(left, top, right, bottom) {
    var currentTime = new Date();
    var secondsPassed = (currentTime - this.timeStart) / 1000;
    const horizontalBounceDecay = 0.94;
    const verticalBounceDecay = 0.90;

    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration()) * horizontalBounceDecay;
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration()) * verticalBounceDecay;

    var hitLeftWall = velocityX < 0 && this.x < left;
    var hitRightWall = velocityX > 0 && this.x + this.width > right;

    var hitTopWall = velocityY < 0 && this.y < top;
    var hitBottomWall = velocityY > 0 && this.y + this.height > bottom;

    if (hitBottomWall)
      this.retractChutes();

    if (hitLeftWall || hitRightWall)
      this.move(-velocityX, velocityY);
    else if (hitTopWall || hitBottomWall)
      this.move(velocityX, -velocityY);
    else
      return;
  }

  fireMainThrusters() {
    var t = new Date();
    t.setSeconds(t.getSeconds() + 1);
    this.mainThrusterOfftime = t;
  }

  fireLeftThruster() {
    var t = new Date();
    t.setSeconds(t.getSeconds() + 1);
    this.leftThrusterOfftime = t;
  }

  fireRightThruster() {
    var t = new Date();
    t.setSeconds(t.getSeconds() + 1);
    this.rightThrusterOfftime = t;
  }

  move(velocityX, velocityY) {
    this.velocityX = velocityX;
    this.velocityY = velocityY;
    this.startX = this.x;
    this.startY = this.y;
    this.timeStart = new Date();
    this.stopped = false;
  }

  updatePosition() {
    if (this.stopped) {
      return;
    }

    var currentTime = new Date();
    var secondsPassed = (currentTime - this.timeStart) / 1000;

    var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, this.getHorizontalAcceleration());
    this.x = this.startX + Physics.metersToPixels(xDisplacement);

    var yDisplacement = Physics.getDisplacement(secondsPassed, this.velocityY, this.getVerticalAcceleration());
    this.y = this.startY + Physics.metersToPixels(yDisplacement);
  }

  draw(context) {
    var now = new Date();
    if (this.mainThrusterOfftime > now) {
      this.bottomRightFlame.draw(context, this.x, this.y);
      this.bottomLeftFlame.draw(context, this.x, this.y);
    }
    else {
      context.globalAlpha = 0.5;
      this.bottomRightMiniFlame.draw(context, this.x, this.y);
      this.bottomLeftMiniFlame.draw(context, this.x, this.y);
      context.globalAlpha = 1;      
    }

    // TODO: Add "Nice Function Name" to Mr. Announcer Guy.

    if (this.rightThrusterOfftime > now)
      this.rightFlame.draw(context, this.x, this.y);

    if (this.leftThrusterOfftime > now)
      this.leftFlame.draw(context, this.x, this.y);

    this.leftEngine.draw(context, this.x, this.y);
    this.rightEngine.draw(context, this.x, this.y);
    this.bottomLeftEngine.draw(context, this.x, this.y);
    this.bottomRightEngine.draw(context, this.x, this.y);

    if (this.chuteDeployed) {
      this.chute.draw(context, this.x, this.y);
      if (this.chute.isOnLastFrame() && !this.chuteSailsAreFull) {
        this.onChutesFullyOpen();
      }
    }
    else if (this.chuteRetracting) {
      this.chute.draw(context, this.x, this.y);
      if (this.chute.isOnFirstFrame()) {
        this.chute.reverse = false;
        this.chuteRetracting = false;
      }
    }

    this.codeRushedBody.draw(context, this.x, this.y);
  }
}
