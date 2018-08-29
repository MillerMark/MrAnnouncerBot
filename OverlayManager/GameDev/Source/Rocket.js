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
const MiniBottomLeftFlameY = 245 - CodeRushedPosY;
const MiniBottomLeftFlameJiggleX = 0;
const MiniBottomLeftFlameJiggleY = 2;



const BottomRightFlameX = 284 - CodeRushedPosX;
const BottomRightFlameY = 228 - CodeRushedPosY;
const BottomRightFlameJiggleX = 0;
const BottomRightFlameJiggleY = 2;

const MiniBottomRightFlameX = 296 - CodeRushedPosX;
const MiniBottomRightFlameY = 245 - CodeRushedPosY;
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
    var now = new Date();
    this.hoverThrusterRestoreTime = now;
    this.mainThrusterOfftime = now;
    this.leftThrusterOfftime = now;
    this.rightThrusterOfftime = now;


    this.loggingState = true;

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

    const chuteFrameInterval = 100;
    this.chute = new Part("ChuteExtend", 5, PartStyle.Sequential, ChuteExtendX + x, ChuteExtendY + y, chuteFrameInterval);
    this.chuteDeployed = false;
    this.chuteSailsAreFull = false;
    this.chuteRetracting = false;

    this.changeVelocity(0, 0);

    this.worldAcceleration = Gravity.earth;
    this.stopped = true;

    this.timeStart = new Date();

    this.width = 298;   // Widh
    this.height = 52;
  }

  deployChute() {
    this.logState('Deploying chute...');
    this.chuteDeployed = true;
    this.chuteSailsAreFull = false;
  }

  onChutesFullyOpen() {
    var currentTime = new Date();
    var secondsPassed = (currentTime - this.timeStart) / 1000;
    var newVelocity = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration());

    newVelocity /= 4;

    this.changeVelocity(this.velocityX, newVelocity);
    this.chuteSailsAreFull = true;
  }

  retractChutes() {
    this.logState('Retracting chutes...');
    this.changeVelocity(this.velocityX, this.velocityY);
    this.chuteDeployed = false;
    this.chuteSailsAreFull = false;
    this.chuteRetracting = true;
    this.chute.reverse = true;
  }

  getVerticalAcceleration() {
    if (this.chuteSailsAreFull)
      return 0;
    var now = new Date();
    //console.log('now: ' + now + ', hoverThrusterRestoreTime: ' + this.hoverThrusterRestoreTime + ', mainThrusterOfftime: ' + this.mainThrusterOfftime);
    if (this.hoverThrusterRestoreTime > now)
      return this.worldAcceleration / 2;
    if (this.mainThrusterOfftime > now)
      return -this.worldAcceleration / 2;

    // TODO: Add if engines are fully off or retracted, then return this.worldAcceleration.
    return 0;
  }

  getHorizontalAcceleration() {
    var now = new Date();
    const horizontalThrust = 5;
    var acceleration = 0;
    if (this.leftThrusterOfftime > now)
      acceleration += horizontalThrust;
    if (this.rightThrusterOfftime > now)
      acceleration -= horizontalThrust;
    return acceleration;
  }

  bounce(left, top, right, bottom) {
    var now = new Date();
    var secondsPassed = (now - this.timeStart) / 1000;
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
      this.changeVelocity(-velocityX, velocityY);
    else if (hitTopWall || hitBottomWall)
      this.changeVelocity(velocityX, -velocityY);
    else
      return;
  }

  getNowPlus(seconds) {
    var t = new Date();
    t.setSeconds(t.getSeconds() + seconds);
    return t;
  }

  changingDirection() {
    var now = new Date();
    var secondsPassed = (now - this.timeStart) / 1000;
    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration());
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration());
    this.changeVelocity(velocityX, velocityY);
  }

  killHoverThrusters() {
    this.changingDirection();
    this.logState('Shutting off hover thrusters...');
    this.wasKillingThrusters = true;
    this.hoverThrusterRestoreTime = this.getNowPlus(1);
  }

  fireMainThrusters() {
    this.changingDirection();
    this.logState('Firing main thrusters...');
    this.wasFiringMainThrusters = true;
    this.mainThrusterOfftime = this.getNowPlus(1);
  }

  fireLeftThruster() {
    this.changingDirection();
    this.logState('Firing left thruster...');
    this.wasFiringLeftThruster = true;
    this.leftThrusterOfftime = this.getNowPlus(1);
  }

  fireRightThruster() {
    this.changingDirection();
    this.logState('Firing right thruster...');
    this.wasFiringRightThruster = true;
    this.rightThrusterOfftime = this.getNowPlus(1);
  }

  changeVelocity(velocityX, velocityY) {
    //console.log('changeVelocity: this.timeStart = new Date();');
    this.timeStart = new Date();
    this.velocityX = velocityX;
    this.velocityY = velocityY;
    this.startX = this.x;
    this.startY = this.y;
    this.stopped = false;
  }

  changeVelocityBy(deltaVelocityX, deltaVelocityY) {
    var newVelocityX = this.velocityX * deltaVelocityX;
    var newVelocityY = this.velocityY * deltaVelocityY;

    if (Math.abs(newVelocityX) < 0.0008)
      newVelocityX = 0;
    if (Math.abs(newVelocityY) < 0.0008)
      newVelocityY = 0;

    if (newVelocityX == 0 && newVelocityY == 0 && newVelocityX == this.velocityX && newVelocityY == this.velocityY)
      return;

    this.changeVelocity(newVelocityX, newVelocityY);
  }

  updatePosition() {
    if (this.stopped) {
      return;
    }

    var now = new Date();
    var secondsPassed = (now - this.timeStart) / 1000;

    var hAccel = this.getHorizontalAcceleration();
    var vAccel = this.getVerticalAcceleration();

    //console.log('hAccel: ' + hAccel + ', vAccel: ' + vAccel);
    //console.log('secondsPassed: ' + secondsPassed);

    var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, hAccel);
    this.x = this.startX + Physics.metersToPixels(xDisplacement);

    var yDisplacement = Physics.getDisplacement(secondsPassed, this.velocityY, vAccel);
    this.y = this.startY + Physics.metersToPixels(yDisplacement);

    //console.log('Displacement: (' + xDisplacement + ', ' + yDisplacement + ')');
  }

  thrustersOff() {
    this.x = this.lastX;
    this.y = this.lastY;
    this.changeVelocity(this.lastVelocityX, this.lastVelocityY);
  }

  draw(context) {
    var now = new Date();
    var offsettingVerticalAcceleration = false;
    var verticalVelocityDecay = 1.0;
    var horizontalVelocityDecay = 1.0;

    if (this.mainThrusterOfftime > now) {
      this.bottomRightFlame.draw(context, this.x, this.y);
      this.bottomLeftFlame.draw(context, this.x, this.y);
      offsettingVerticalAcceleration = true;
    }
    else {
      if (this.wasFiringMainThrusters) {
        this.wasFiringMainThrusters = false;
        this.logState('Main thrusters off...');
        this.thrustersOff();
      }
      var hoverThrusterKillOffset = 0;
      if (this.hoverThrusterRestoreTime > now) {
        offsettingVerticalAcceleration = true;
        hoverThrusterKillOffset = 15;
      }
      else if (this.wasKillingThrusters) {
        this.logState('Hover thrusters restored...');
        this.wasKillingThrusters = false;
        this.thrustersOff();
      }

      this.bottomRightMiniFlame.draw(context, this.x, this.y - hoverThrusterKillOffset);
      this.bottomLeftMiniFlame.draw(context, this.x, this.y - hoverThrusterKillOffset);
    }
    if (!offsettingVerticalAcceleration && this.velocityY != 0) {
      // Stabilize vertical velocity if only the hover thrusters are firing.
      verticalVelocityDecay = 0.99;
    }



    var offsettingHorizontalAcceleration = false;

    if (this.rightThrusterOfftime > now) {
      offsettingHorizontalAcceleration = true;
      this.rightFlame.draw(context, this.x, this.y);
    }
    else if (this.wasFiringRightThruster) {
      this.logState('Right thruster off...');
      this.wasFiringRightThruster = false;
      this.thrustersOff();
    }

    if (this.leftThrusterOfftime > now) {
      offsettingHorizontalAcceleration = true;
      this.leftFlame.draw(context, this.x, this.y);
    }
    else if (this.wasFiringLeftThruster) {
      this.logState('Left thruster off...');
      this.wasFiringLeftThruster = false;
      this.thrustersOff();
    }

    if (!offsettingHorizontalAcceleration && this.velocityX != 0) {
      // Stabilize horizontal velocity if only the hover thrusters are firing.
      horizontalVelocityDecay = 0.99;
    }

    if (horizontalVelocityDecay != 1 || verticalVelocityDecay != 1)
      this.changeVelocityBy(horizontalVelocityDecay, verticalVelocityDecay);

    var secondsPassed = (now - this.timeStart) / 1000;
    this.lastVelocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration());
    this.lastVelocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration());
    this.lastX = this.x;
    this.lastY = this.y;


    this.leftEngine.draw(context, this.x, this.y);
    this.rightEngine.draw(context, this.x, this.y);
    this.bottomLeftEngine.draw(context, this.x, this.y);
    this.bottomRightEngine.draw(context, this.x, this.y);

    if (this.chuteDeployed) {
      this.chute.draw(context, this.x, this.y);
      if (this.chute.isOnLastFrame() && !this.chuteSailsAreFull) {
        this.logState('Chute fully open...');
        this.onChutesFullyOpen();
      }
    }
    else if (this.chuteRetracting) {
      this.chute.draw(context, this.x, this.y);
      if (this.chute.isOnFirstFrame()) {
        this.logState('Chute fully retracted...');
        this.chute.reverse = false;
        this.chuteRetracting = false;
      }
    }

    this.codeRushedBody.draw(context, this.x, this.y);
  }

  logState(message) {
    if (this.loggingState)
  	  console.log(message);
  }
}
