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

const BottomEngineAdjust = 40;
const BottomLeftEngineX = 132 - BottomEngineAdjust - CodeRushedPosX;
const BottomLeftEngineY = 210 - CodeRushedPosY;
const BottomLeftEngineRetractionOffsetX = 0;
const BottomLeftEngineRetractionOffsetY = -46;


const BottomRightEngineX = 279 + BottomEngineAdjust - CodeRushedPosX;
const BottomRightEngineY = 210 - CodeRushedPosY;
const BottomRightEngineRetractionOffsetX = 0;
const BottomRightEngineRetractionOffsetY = -46;


const BottomLeftFlameX = 137 - BottomEngineAdjust - CodeRushedPosX;
const BottomLeftFlameY = 228 - CodeRushedPosY;
const BottomLeftFlameJiggleX = 0;
const BottomLeftFlameJiggleY = 2;

const MiniBottomLeftFlameX = 149 - BottomEngineAdjust - CodeRushedPosX;
const MiniBottomLeftFlameY = 245 - CodeRushedPosY;
const MiniBottomLeftFlameJiggleX = 0;
const MiniBottomLeftFlameJiggleY = 2;



const BottomRightFlameX = 284 + BottomEngineAdjust - CodeRushedPosX;
const BottomRightFlameY = 228 - CodeRushedPosY;
const BottomRightFlameJiggleX = 0;
const BottomRightFlameJiggleY = 2;

const MiniBottomRightFlameX = 296 + BottomEngineAdjust - CodeRushedPosX;
const MiniBottomRightFlameY = 245 - CodeRushedPosY;
const MiniBottomRightFlameJiggleX = 0;
const MiniBottomRightFlameJiggleY = 1;

const ChuteExtendX = 144 - CodeRushedPosX;
const ChuteExtendY = 6 - CodeRushedPosY;
const ChuteMaxVelocity = 2; // m/s

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
    this.isDocked = true;

    this.worldAcceleration = Gravity.earth;
    this.stopped = true;

    this.timeStart = new Date();

    this.width = 298;   // Widh
    this.height = 52;
    this.enginesRetracted = true;
  }

  
  deployChute() {
    if (this.isDocked)
      return;
    this.logState('Deploying chute...');
    this.chuteDeployed = true;
    this.chuteSailsAreFull = false;
  }

  onChutesFullyOpen() {
    var currentTime = new Date();
    var secondsPassed = (currentTime - this.timeStart) / 1000;
    var newVelocity = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration());

    if (newVelocity > ChuteMaxVelocity)
      newVelocity > ChuteMaxVelocity;

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
    if (this.isDocked)
      return 0;

    if (this.docking)
      if (this.dockedTop)
        return 0;
      else 
        return -this.worldAcceleration / 2;

    if (this.chuteSailsAreFull)
      return 0;
    if (this.retractingEngines || this.enginesRetracted)
      return this.worldAcceleration;
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
    if (this.isDocked)
      return 0;
    const horizontalThrust = 5;
    if (this.docking)
      if (this.dockedLeft)
        return 0;
      else 
        return -horizontalThrust;

    var now = new Date();
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
    var horizontalBounceDecay = 0.74;
    var verticalBounceDecay = 0.60;
    if (this.docking) {
      horizontalBounceDecay /= 4;
      verticalBounceDecay /= 4;
    }

    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration());
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration());

    var hitLeftWall = velocityX < 0 && this.x < left;
    var hitRightWall = velocityX > 0 && this.x + this.width > right;

    var hitTopWall = velocityY < 0 && this.y < top;
    var hitBottomWall = velocityY > 0 && this.y + this.height > bottom;

    if (hitBottomWall && myRocket.chuteDeployed)
      this.retractChutes();

    var newVelocityX = this.velocityX;
    var newVelocityY = this.velocityY;
    if (hitLeftWall || hitRightWall)
      newVelocityX = -velocityX * horizontalBounceDecay;
    if (hitTopWall || hitBottomWall)
      newVelocityY = -velocityY * verticalBounceDecay;

    if (this.docking && hitTopWall) {
      newVelocityY = 0;
      this.y = 0;
      this.dockedTop = true;
    }

    if (this.docking && hitLeftWall) {
      newVelocityX = 0;
      this.x = 0;
      this.dockedLeft = true;
    }

    if (this.docking && this.dockedLeft && this.dockedTop) {
      this.docking = false;
      this.dockedLeft = false;
      this.dockedTop = false;
      this.isDocked = true;
      this.retractEngines();
    }


    if (hitLeftWall || hitRightWall || hitTopWall || hitBottomWall)
      this.changeVelocity(newVelocityX, newVelocityY);
  }

  getNowPlus(seconds) {
    var t = new Date();
    t.setMilliseconds(t.getMilliseconds() + seconds * 1000);
    return t;
  }

  changingDirection() {
    var now = new Date();
    var secondsPassed = (now - this.timeStart) / 1000;
    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration());
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration());
    this.changeVelocity(velocityX, velocityY);
  }

  dock() {
    this.logState('Docking...');
    if (this.enginesRetracted)
      this.extendEngines();
    this.changingDirection();
    this.docking = true;
  }

  extendEngines() {
    this.changingDirection();
    this.logState('Extending engines...');
    this.extendingEngines = true;
    var now = new Date();
    this.extensionStartTime = now;
    this.extensionEndTime = this.getNowPlus(0.4);
  }

  retractEngines() {
    this.changingDirection();
    this.logState('Retracting engines...');
    this.retractingEngines = true;
    var now = new Date();
    this.retractionStartTime = now;
    this.retractionEndTime = this.getNowPlus(0.4);
    this.killInProgressVerticalDrop();
    this.killInProgressVerticalThrust();
    this.killInProgressHorizontalThrust();
  }

  killInProgressHorizontalThrust() {
    var now = new Date();
    if (this.leftThrusterOfftime > now) {
      this.leftThrusterOfftime = now;
      this.wasFiringLeftThruster = false;
    }
    if (this.rightThrusterOfftime > now) {
      this.rightThrusterOfftime = now;
      this.wasFiringRightThruster = false;
    }
  }

  killInProgressVerticalThrust() {
    var now = new Date();
    if (this.mainThrusterOfftime > now) {
      this.mainThrusterOfftime = now;
      this.wasFiringMainThrusters = false;
    }
  }

  killHoverThrusters() {
    if (this.docking)
       return;
    this.changingDirection();
    this.logState('Shutting off hover thrusters...');
    this.wasKillingThrusters = true;
    this.hoverThrusterRestoreTime = this.getNowPlus(1);
    this.killInProgressVerticalThrust();
  }

  killInProgressVerticalDrop() {
    var now = new Date();
    if (this.hoverThrusterRestoreTime > now) {
      this.hoverThrusterRestoreTime = now;
      this.wasKillingThrusters = false;
    }
  }

  fireMainThrusters() {
    if (this.retractingEngines || this.enginesRetracted || this.docking)
      return;
    this.changingDirection();
    this.logState('Firing main thrusters...');
    this.wasFiringMainThrusters = true;
    this.mainThrusterOfftime = this.getNowPlus(1);
    this.killInProgressVerticalDrop();
  }

  fireLeftThruster() {
    if (this.retractingEngines || this.enginesRetracted || this.docking)
      return;
    this.changingDirection();
    this.logState('Firing left thruster...');
    this.wasFiringLeftThruster = true;
    this.leftThrusterOfftime = this.getNowPlus(1);
  }

  fireRightThruster() {
    if (this.retractingEngines || this.enginesRetracted || this.docking)
      return;
    this.changingDirection();
    this.logState('Firing right thruster...');
    this.wasFiringRightThruster = true;
    this.rightThrusterOfftime = this.getNowPlus(1);
  }

  changeVelocity(velocityX, velocityY) {
    this.timeStart = new Date();
    this.velocityX = velocityX;
    this.velocityY = velocityY;
    this.startX = this.x;
    this.startY = this.y;
    this.stopped = false;
  }

  changeVelocityBy(deltaVelocityX, deltaVelocityY) {
    var now = new Date();
    var secondsPassed = (now - this.timeStart) / 1000;
    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration());
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration());

    var newVelocityX = velocityX * deltaVelocityX;
    var newVelocityY = velocityY * deltaVelocityY;

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
    if (this.chuteSailsAreFull && yDisplacement > ChuteMaxVelocity * secondsPassed)
      yDisplacement = ChuteMaxVelocity * secondsPassed;

    this.y = this.startY + Physics.metersToPixels(yDisplacement);

    //console.log('Displacement: (' + xDisplacement + ', ' + yDisplacement + ')');
  }

  thrustersOff() {
    this.x = this.lastX;
    this.y = this.lastY;
    this.changeVelocity(this.lastVelocityX, this.lastVelocityY);
  }

  launch() {
    myRocket.isDocked = false;
    myRocket.isDocking = false;
    myRocket.dockedLeft = false;
    myRocket.dockedTop = false;
    myRocket.changeVelocity(2.4, 0);
    myRocket.extendEngines();
  }

  draw(context) {
    if (this.drawing)
      return;
    this.drawing = true;
    var now = new Date();

    var bottomLeftEngineRetractionOffsetX = 0
    var bottomLeftEngineRetractionOffsetY = 0
    var bottomRightEngineRetractionOffsetX = 0;
    var bottomRightEngineRetractionOffsetY = 0;
    var rightEngineRetractionOffsetX = 0;
    var rightEngineRetractionOffsetY = 0;
    var leftEngineRetractionOffsetX = 0;
    var leftEngineRetractionOffsetY = 0;

    if (this.retractingEngines || this.extendingEngines) {
      var timeIn;
      var totalTime;
      var percentageIn;

      if (this.extendingEngines) {
        totalTime = this.extensionEndTime - this.extensionStartTime;
        timeIn = now - this.extensionStartTime;
        var percentageOut = timeIn / totalTime
        if (percentageOut > 1) {
          this.extendingEngines = false;
          percentageOut = 1;
          this.enginesExtended = true;
          this.enginesRetracted = false;
        }
        percentageIn = 1 - percentageOut;
      }
      else {
        timeIn = now - this.retractionStartTime;
        totalTime = this.retractionEndTime - this.retractionStartTime;
        percentageIn = timeIn / totalTime;
        if (percentageIn > 1) {
          this.retractingEngines = false;
          percentageIn = 1;
          this.enginesRetracted = true;
          this.enginesExtended = false;
        }
      }
      
      
      bottomLeftEngineRetractionOffsetX = BottomLeftEngineRetractionOffsetX * percentageIn;
      bottomLeftEngineRetractionOffsetY = BottomLeftEngineRetractionOffsetY * percentageIn;

      bottomRightEngineRetractionOffsetX = BottomRightEngineRetractionOffsetX * percentageIn;
      bottomRightEngineRetractionOffsetY = BottomRightEngineRetractionOffsetY * percentageIn;

      leftEngineRetractionOffsetX = LeftEngineRetractionOffsetX * percentageIn;
      leftEngineRetractionOffsetY = LeftEngineRetractionOffsetY * percentageIn;

      rightEngineRetractionOffsetX = RightEngineRetractionOffsetX * percentageIn;
      rightEngineRetractionOffsetY = RightEngineRetractionOffsetY * percentageIn;
    }

    var offsettingVerticalAcceleration = false;
    var verticalVelocityDecay = 1.0;
    var horizontalVelocityDecay = 1.0;

    if (!this.enginesRetracted && !this.retractingEngines && !this.extendingEngines) {
      if ((this.docking && !this.dockedTop) || this.mainThrusterOfftime > now) {
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

      if ((this.docking && !this.dockedLeft) || this.rightThrusterOfftime > now) {
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

      if (horizontalVelocityDecay != 1 || verticalVelocityDecay != 1) {
        this.changeVelocityBy(horizontalVelocityDecay, verticalVelocityDecay);
      }
    }

    var secondsPassed = (now - this.timeStart) / 1000;
    this.lastVelocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration());
    this.lastVelocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration());
    this.lastX = this.x;
    this.lastY = this.y;


    if (!this.enginesRetracted || this.extendingEngines) {
      this.leftEngine.draw(context, this.x + leftEngineRetractionOffsetX, this.y + leftEngineRetractionOffsetY);
      this.rightEngine.draw(context, this.x + rightEngineRetractionOffsetX, this.y + rightEngineRetractionOffsetY);
      this.bottomLeftEngine.draw(context, this.x + bottomLeftEngineRetractionOffsetX, this.y + bottomLeftEngineRetractionOffsetY);
      this.bottomRightEngine.draw(context, this.x + bottomRightEngineRetractionOffsetX, this.y + bottomRightEngineRetractionOffsetY);
    }

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
    this.drawing = false;
  }

  logState(message) {
    if (this.loggingState)
  	  console.log(message);
  }
}
