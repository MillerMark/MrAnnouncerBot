class Rocket {
  x: number;
  y: number;
  hoverThrusterRestoreTime: number;
  mainThrusterOfftime: number;
  leftThrusterOfftime: number;
  rightThrusterOfftime: number;
  lastHorizontalAcceleration: number;
  lastVerticalAcceleration: number;
  loggingState: boolean;
  hoverThrustersAudio: HTMLAudioElement;
  hoverDropThrustersAudio: HTMLAudioElement;
  mainThrustersAudio: HTMLAudioElement;
  motorExtendAudio: HTMLAudioElement;
  codeRushedBody: Part;
  leftEngine: Part;
  motorRetractAudio: HTMLAudioElement;
  rightEngine: Part;
  bottomLeftEngine: Part;
  bottomRightEngine: Part;
  bottomLeftFlame: Part;
  bottomRightFlame: Part;
  bottomLeftMiniFlame: Part;
  bottomRightMiniFlame: Part;
  leftFlame: Part;
  rightFlame: Part;
  chute: Part;
  chuteDeployed: boolean;
  chuteSailsAreFull: boolean;
  chuteRetracting: boolean;
  isDocked: boolean;
  stopped: boolean;
  timeStart: number;
  width: number;
  height: number;
  enginesRetracted: boolean;
  velocityY: any;
  velocityX: any;
  wasFiringRightThruster: boolean;
  sideThrustersAudio: any;
  wasFiringLeftThruster: any;
  startX: any;
  startY: any;
  wasFiringMainThrusters: boolean;
  wasKillingThrusters: boolean;
  lastX: any;
  lastY: any;
  extendingEngines: boolean;
  extensionStartTime: any;
  extensionEndTime: number;
  retractingEngines: boolean;
  retractionStartTime: any;
  retractionEndTime: number;
  docking: boolean;
  isDocking: boolean;
  isUndocking: boolean;
  dockedLeft: boolean;
  dockedTop: boolean;
  drawing: boolean;
  enginesExtended: boolean;
  lastVelocityX: number;
  lastVelocityY: number;

  constructor(x, y) {
    const upperLeftOffsetX = 9;
    const upperLeftOffsetY = 9;

    x -= upperLeftOffsetX;
    y -= upperLeftOffsetY;

    this.x = x;
    this.y = y;
    const flameFrameInterval = 60;
    var now = performance.now();
    this.hoverThrusterRestoreTime = now;
    this.mainThrusterOfftime = now;
    this.leftThrusterOfftime = now;
    this.rightThrusterOfftime = now;

    this.lastHorizontalAcceleration = 0;
    this.lastVerticalAcceleration = 0;

    this.loggingState = true;

    this.hoverThrustersAudio = new Audio(Folders.assets + 'Sound Effects/Hover Thruster Loop.wav');
    this.hoverThrustersAudio.loop = true;

    this.hoverDropThrustersAudio = new Audio(Folders.assets + 'Sound Effects/Hover Thruster Drop Loop.wav');
    this.hoverDropThrustersAudio.loop = true;

    this.mainThrustersAudio = new Audio(Folders.assets + 'Sound Effects/MainThrustersLoop.wav');
    this.mainThrustersAudio.loop = true;

    this.sideThrustersAudio = new Audio(Folders.assets + 'Sound Effects/HorizontalRocketLoop.wav');
    this.sideThrustersAudio.loop = true;

    this.motorExtendAudio = new Audio(Folders.assets + 'Sound Effects/EngineExtend4.wav');
    this.motorRetractAudio = new Audio(Folders.assets + 'Sound Effects/EngineRetract.wav');

    this.codeRushedBody = new Part("CodeRushed", 1, AnimationStyle.Static, x, y);

    this.leftEngine = new Part("LeftEngine", 1, AnimationStyle.Static, LeftEngineX + x, LeftEngineY + y);

    this.rightEngine = new Part("RightEngine", 1, AnimationStyle.Static, RightEngineX + x, RightEngineY + y);

    this.bottomLeftEngine = new Part("BottomEngine", 1, AnimationStyle.Static, BottomLeftEngineX + x, BottomLeftEngineY + y);
    this.bottomRightEngine = new Part("BottomEngine", 1, AnimationStyle.Static, BottomRightEngineX + x, BottomRightEngineY + y);


    this.bottomLeftFlame = new Part("FlameBottom", 6, AnimationStyle.Random, BottomLeftFlameX + x, BottomLeftFlameY + y, flameFrameInterval, BottomLeftFlameJiggleX, BottomLeftFlameJiggleY);
    this.bottomRightFlame = new Part("FlameBottom", 6, AnimationStyle.Random, BottomRightFlameX + x, BottomRightFlameY + y, flameFrameInterval, BottomRightFlameJiggleX, BottomRightFlameJiggleY);

    this.bottomLeftMiniFlame = new Part("MiniFlameBottom", 6, AnimationStyle.Random, MiniBottomLeftFlameX + x, MiniBottomLeftFlameY + y, flameFrameInterval, MiniBottomLeftFlameJiggleX, MiniBottomLeftFlameJiggleY);
    this.bottomRightMiniFlame = new Part("MiniFlameBottom", 6, AnimationStyle.Random, MiniBottomRightFlameX + x, MiniBottomRightFlameY + y, flameFrameInterval, MiniBottomRightFlameJiggleX, MiniBottomRightFlameJiggleY);

    this.leftFlame = new Part("FlameLeft", 6, AnimationStyle.Random, LeftFlameX + x, LeftFlameY + y, flameFrameInterval * 0.6, LeftFlameJiggleX, LeftFlameJiggleY);
    this.rightFlame = new Part("FlameRight", 6, AnimationStyle.Random, RightFlameX + x, RightFlameY + y, flameFrameInterval * 0.6, RightFlameJiggleX, RightFlameJiggleY);

    const chuteFrameInterval = 100;
    this.chute = new Part("ChuteExtend", 5, AnimationStyle.Sequential, ChuteExtendX + x, ChuteExtendY + y, chuteFrameInterval);
    this.chuteDeployed = false;
    this.chuteSailsAreFull = false;
    this.chuteRetracting = false;

    this.changeVelocity(0, 0, now);
    this.isDocked = true;

    this.stopped = true;

    this.timeStart = performance.now();

    this.width = 298;   // Widh
    this.height = 52;
    this.enginesRetracted = true;
  }

  get worldAcceleration(): number {
    return gravityGames.activePlanet.gravity;
  }

  deployChute(now) {
    if (this.isDocked)
      return;
    this.logState('Deploying chute...');
    this.chuteDeployed = true;
    this.chuteSailsAreFull = false;
    this.retractEngines(now);
  }

  onChutesFullyOpen(now) {
    var secondsPassed = (now - this.timeStart) / 1000;
    var newVelocity = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));

    if (newVelocity > ChuteMaxVelocity)
      newVelocity > ChuteMaxVelocity;

    this.changeVelocity(this.velocityX, newVelocity, now);
    this.chuteSailsAreFull = true;
  }

  retractChutes(now) {
    this.logState('Retracting chutes...');
    this.changeVelocity(this.velocityX, this.velocityY, now);
    this.chuteDeployed = false;
    this.chuteSailsAreFull = false;
    this.chuteRetracting = true;
    this.chute.reverse = true;
  }

  getVerticalAcceleration(now) {
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
    //console.log('now: ' + now + ', hoverThrusterRestoreTime: ' + this.hoverThrusterRestoreTime + ', mainThrusterOfftime: ' + this.mainThrusterOfftime);
    if (this.hoverThrusterRestoreTime > now)
      return VerticalThrust;
    if (this.mainThrusterOfftime > now)
      return -VerticalThrust;

    // TODO: Add if engines are fully off or retracted, then return this.worldAcceleration.
    return 0;
  }

  getHorizontalAcceleration(now) {
    if (this.isDocked)
      return 0;
    if (this.docking)
      if (this.dockedLeft)
        return 0;
      else
        return -HorizontalThrust;

    var acceleration = 0;
    if (this.leftThrusterOfftime > now)
      acceleration += HorizontalThrust;
    if (this.rightThrusterOfftime > now)
      acceleration -= HorizontalThrust;
    return acceleration;
  }

  showHit(wallHit, wallName) {
    if (wallHit)
      console.log('Hit ' + wallName + ' wall.');
  }

  bounce(left: number, top: number, right: number, bottom: number, now: number) {
    var secondsPassed = (now - this.timeStart) / 1000;
    var horizontalBounceDecay = 0.74;
    var verticalBounceDecay = 0.60;
    if (this.docking) {
      horizontalBounceDecay /= 4;
      verticalBounceDecay /= 4;
    }

    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration(now));
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));

    var hitLeftWall = velocityX < 0 && this.x < left;
    var hitRightWall = velocityX > 0 && this.x + this.width > right;

    var hitTopWall = velocityY < 0 && this.y < top;
    var hitBottomWall = velocityY > 0 && this.y + this.height > bottom;

    //this.showHit(hitLeftWall, 'left');
    //this.showHit(hitRightWall, 'right');
    //this.showHit(hitBottomWall, 'bottom');
    //this.showHit(hitTopWall, 'top');

    if (hitBottomWall && myRocket.chuteDeployed)
      this.retractChutes(now);

    var newVelocityX = velocityX;
    var newVelocityY = velocityY;
    if (hitLeftWall || hitRightWall)
      newVelocityX = -velocityX * horizontalBounceDecay;
    if (hitTopWall || hitBottomWall)
      newVelocityY = -velocityY * verticalBounceDecay;

    if (this.docking && hitTopWall) {
      newVelocityY = 0;
      this.y = 0;
      this.dockedTop = true;
      this.mainThrustersAudio.pause();
    }

    if (this.docking && hitLeftWall) {
      newVelocityX = 0;
      this.x = 0;
      this.dockedLeft = true;
      this.sideThrustersAudio.pause();
    }

    if (this.docking && this.dockedLeft && this.dockedTop) {
      this.docking = false;
      this.dockedLeft = false;
      this.dockedTop = false;
      this.isDocked = true;
      this.retractEngines(now);
    }

    if (hitBottomWall && Math.abs(newVelocityY) < 0.1) {
      newVelocityY = 0;
      newVelocityX = newVelocityX * 0.99;
      this.y = bottom - this.height;
    }

    if (hitLeftWall || hitRightWall || hitTopWall || hitBottomWall)
      this.changeVelocity(newVelocityX, newVelocityY, now);
  }

  changingDirection(now) {
    var secondsPassed = (now - this.timeStart) / 1000;
    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration(now));
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));
    this.changeVelocity(velocityX, velocityY, now);
  }

  dock(now) {
    this.logState('Docking...');
    if (this.enginesRetracted)
      this.extendEngines(now);
    this.changingDirection(now);
    this.docking = true;
  }

  extendEngines(now) {
    this.changingDirection(now);
    this.logState('Extending engines...');
    this.extendingEngines = true;
    this.extensionStartTime = now;
    this.extensionEndTime = now + 400;
    this.motorExtendAudio.play();
  }

  retractEngines(now) {
    if (this.docking)
      return;
    this.hoverThrustersAudio.pause();
    this.hoverDropThrustersAudio.pause();
    this.mainThrustersAudio.pause();
    this.sideThrustersAudio.pause();
    this.changingDirection(now);
    this.logState('Retracting engines...');
    this.retractingEngines = true;
    this.retractionStartTime = now;
    this.retractionEndTime = now + 400;
    this.killInProgressVerticalDrop(now);
    this.killInProgressVerticalThrust(now);
    this.killInProgressHorizontalThrust(now);
    this.motorRetractAudio.play();
  }

  killInProgressHorizontalThrust(now) {
    if (this.leftThrusterOfftime > now) {
      this.leftThrusterOfftime = now;
      this.wasFiringLeftThruster = false;
    }
    if (this.rightThrusterOfftime > now) {
      this.rightThrusterOfftime = now;
      this.wasFiringRightThruster = false;
    }
  }

  killInProgressVerticalThrust(now) {
    if (this.mainThrusterOfftime > now) {
      this.mainThrusterOfftime = now;
      this.wasFiringMainThrusters = false;
    }
  }

  killHoverThrusters(now, duration?) {
    if (this.docking)
      return;
    this.changingDirection(now);
    this.logState('Shutting off hover thrusters...');
    this.wasKillingThrusters = true;
    this.hoverThrusterRestoreTime = now + this.getMs(duration);
    this.killInProgressVerticalThrust(now);
    this.hoverThrustersAudio.pause();
    this.hoverDropThrustersAudio.play();
  }

  killInProgressVerticalDrop(now) {
    if (this.hoverThrusterRestoreTime > now) {
      this.hoverThrusterRestoreTime = now;
      this.wasKillingThrusters = false;
    }
  }

  fireMainThrusters(now, duration?) {
    if (this.retractingEngines || this.enginesRetracted || this.docking)
      return;
    this.changingDirection(now);
    this.logState('Firing main thrusters...');
    this.wasFiringMainThrusters = true;
    this.mainThrusterOfftime = now + this.getMs(duration);
    this.killInProgressVerticalDrop(now);
    this.mainThrustersAudio.play();
  }

  fireRightQuick() {
    var now: number = performance.now();
    myRocket.fireRightThruster(now);
    myRocket.rightThrusterOfftime = now + 400;
  }

  fireMainQuick() {
    var now = performance.now();
    myRocket.mainThrustersAudio.play();
    myRocket.wasFiringMainThrusters = true;
    myRocket.fireMainThrusters(now);
    myRocket.mainThrusterOfftime = now + 600;
  }

  fireLeftThruster(now, duration?) {
    if (this.retractingEngines || this.enginesRetracted || this.docking)
      return;
    this.changingDirection(now);
    this.logState('Firing left thruster...');
    this.wasFiringLeftThruster = true;
    this.leftThrusterOfftime = now + this.getMs(duration);
    this.sideThrustersAudio.play();
    if (this.wasFiringRightThruster) {
      this.wasFiringRightThruster = false;
      this.rightThrusterOfftime = now;
    }
  }

  getMs(duration): number {
    if (!duration)
      return 1000;
    else {
      var result: number = +duration * 1000;
      if (result > 5000) {
        result = 5000;
        // TODO: report out of range...
      }
      else if (result < 0) {
        result = 0;
        // TODO: report out of range...
      }
      return result;
    }
  }

  fireRightThruster(now, duration?) {
    if (this.retractingEngines || this.enginesRetracted || this.docking)
      return;
    this.changingDirection(now);
    this.logState('Firing right thruster...');
    this.wasFiringRightThruster = true;
    this.rightThrusterOfftime = now + this.getMs(duration);
    this.sideThrustersAudio.play();

    if (this.wasFiringLeftThruster) {
      this.wasFiringLeftThruster = false;
      this.leftThrusterOfftime = now;
    }
  }

  changeVelocity(velocityX: number, velocityY: number, now: number) {
    this.timeStart = now;
    this.velocityX = velocityX;
    this.velocityY = velocityY;
    this.startX = this.x;
    this.startY = this.y;
    this.stopped = false;
  }

  changeVelocityBy(deltaVelocityX: number, deltaVelocityY: number, now: number) {
    var secondsPassed = (now - this.timeStart) / 1000;
    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration(now));
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));

    var newVelocityX = velocityX * deltaVelocityX;
    var newVelocityY = velocityY * deltaVelocityY;

    if (Math.abs(newVelocityX) < 0.0008)
      newVelocityX = 0;
    if (Math.abs(newVelocityY) < 0.0008)
      newVelocityY = 0;

    if (newVelocityX == 0 && newVelocityY == 0 && newVelocityX == this.velocityX && newVelocityY == this.velocityY)
      return;

    this.changeVelocity(newVelocityX, newVelocityY, now);
  }

  updatePosition(now: number) {
    if (this.stopped) {
      return;
    }

    var secondsPassed = (now - this.timeStart) / 1000;

    var hAccel = this.getHorizontalAcceleration(now);
    var vAccel = this.getVerticalAcceleration(now);

    var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, hAccel);

    if ((this.wasFiringLeftThruster && this.leftThrusterOfftime <= now) || (this.wasFiringRightThruster && this.rightThrusterOfftime <= now)) {
      this.startX = this.x;
    }

    var newX = this.startX + Physics.metersToPixels(xDisplacement);
    this.x = newX;
    //if (this.x < 0) {
    //  debugger;
    //}


    var yDisplacement = Physics.getDisplacement(secondsPassed, this.velocityY, vAccel);
    if (this.chuteSailsAreFull && yDisplacement > ChuteMaxVelocity * secondsPassed)
      yDisplacement = ChuteMaxVelocity * secondsPassed;

    if ((this.wasFiringMainThrusters && this.mainThrusterOfftime <= now) || (this.wasKillingThrusters && this.hoverThrusterRestoreTime <= now)) {
      console.log('this.startY = this.y;');
      this.startY = this.y;
    }

    var newY = this.startY + Physics.metersToPixels(yDisplacement);

    this.y = newY;

    //if (this.y < 0) {
    //  debugger;
    //}
  }

  thrustersOff(now) {
    this.x = this.lastX;
    //if (this.x < 0) {
    //  debugger;
    //}
    this.y = this.lastY;
    this.changeVelocity(this.lastVelocityX, this.lastVelocityY, now);
  }

  launch(now) {
    this.isDocked = false;
    this.isUndocking = true;
    this.isDocking = false;
    this.dockedLeft = false;
    this.dockedTop = false;
    this.changeVelocity(1.7, 0, now);
    this.extendEngines(now);
    this.hoverThrustersAudio.play();
    setTimeout(this.fireMainQuick, 390);
    setTimeout(this.fireRightQuick, 500);
  }

  checkRightThruster(now) {
    if ((this.docking && !this.dockedLeft) || this.rightThrusterOfftime > now) {
      this.sideThrustersAudio.play();
      return true;
    }
    else if (this.wasFiringRightThruster) {
      this.sideThrustersAudio.pause();
      this.logState('Right thruster off...');
      this.wasFiringRightThruster = false;
      this.thrustersOff(now);
    }
    return false;
  }

  checkLeftThruster(now) {
    if (this.leftThrusterOfftime > now) {
      return true;
    }
    else if (this.wasFiringLeftThruster) {
      this.sideThrustersAudio.pause();
      this.logState('Left thruster off...');
      this.wasFiringLeftThruster = false;
      this.thrustersOff(now);
    }
    return false;
  }

  draw(context: CanvasRenderingContext2D, now) {
    if (this.drawing)
      return;

    let secondsPassed = (now - this.timeStart) / 1000;
    this.drawing = true;

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
      var totalTime: number;
      var percentageIn;

      if (this.extendingEngines) {
        totalTime = this.extensionEndTime - this.extensionStartTime;
        timeIn = now - this.extensionStartTime;
        var percentageOut = timeIn / totalTime
        if (percentageOut > 1) {
          if (this.isUndocking) {
            this.isUndocking = false;
            this.velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration(now));
            this.velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));
          }

          this.hoverThrustersAudio.play();

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
    var drawBottomFullFlames = false;
    var drawBottomMiniFlames = false;

    if (!this.enginesRetracted && !this.retractingEngines && !this.extendingEngines) {
      if ((this.docking && !this.dockedTop) || this.mainThrusterOfftime > now) {
        this.mainThrustersAudio.play();
        drawBottomFullFlames = true;
        offsettingVerticalAcceleration = true;
      }
      else {
        if (this.wasFiringMainThrusters) {
          this.wasFiringMainThrusters = false;
          this.logState('Main thrusters off...');
          this.mainThrustersAudio.pause();
          this.thrustersOff(now);
        }
        var hoverThrusterKillOffset = 0;
        if (this.hoverThrusterRestoreTime > now) {
          offsettingVerticalAcceleration = true;
          hoverThrusterKillOffset = 15;
        }
        else if (this.wasKillingThrusters) {
          this.logState('Hover thrusters restored...');
          this.hoverThrustersAudio.play();
          this.hoverDropThrustersAudio.pause();
          this.wasKillingThrusters = false;
          this.thrustersOff(now);
        }

        drawBottomMiniFlames = true;
      }

      if (!offsettingVerticalAcceleration && this.velocityY != 0) {
        // Stabilize vertical velocity if only the hover thrusters are firing.
        verticalVelocityDecay = 0.99;
      }

      var offsettingHorizontalAcceleration = false;

      if (this.checkRightThruster(now)) {
        offsettingHorizontalAcceleration = true;
        this.rightFlame.draw(context, this.x, this.y);
      }

      if (this.checkLeftThruster(now)) {
        offsettingHorizontalAcceleration = true;
        this.leftFlame.draw(context, this.x, this.y);
      }

      if (!offsettingHorizontalAcceleration && this.velocityX != 0) {
        // Stabilize horizontal velocity if only the hover thrusters are firing.
        horizontalVelocityDecay = 0.99;
      }

      if (horizontalVelocityDecay != 1 || verticalVelocityDecay != 1) {
        this.changeVelocityBy(horizontalVelocityDecay, verticalVelocityDecay, now);
      }
    }

    this.lastVelocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration(now));
    this.lastVelocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));
    this.lastX = this.x;
    this.lastY = this.y;


    if (drawBottomFullFlames) {
      this.bottomRightFlame.draw(context, this.x, this.y);
      this.bottomLeftFlame.draw(context, this.x, this.y);
    }
    if (drawBottomMiniFlames) {
      this.bottomRightMiniFlame.draw(context, this.x, this.y - hoverThrusterKillOffset);
      this.bottomLeftMiniFlame.draw(context, this.x, this.y - hoverThrusterKillOffset);
    }

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
        this.onChutesFullyOpen(now);
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

  releaseBee(now: number, params: string, userId: string, displayName: string, color: string): any {
    this.createSprite(beesYellow, now);
  }

  releaseDrone(now: number, params: string, userId: string, displayName: string, color: string): any {
    this.createSprite(dronesRed, now);
  }

  logState(message) {
    if (this.loggingState)
      console.log(message);
  }

  dropFromOne(now, sprites1, sprites2, sprites3, sprites4?) {
    var spriteArray = null;
    var randomNumber: number;
    if (sprites4) {
      randomNumber = Math.random() * 4;
      if (randomNumber < 1)
        spriteArray = sprites1;
      else if (randomNumber < 2)
        spriteArray = sprites2;
      else if (randomNumber < 3)
        spriteArray = sprites3;
      else
        spriteArray = sprites4;
    }
    else {
      randomNumber = Math.random() * 3;
      if (randomNumber < 1)
        spriteArray = sprites1;
      else if (randomNumber < 2)
        spriteArray = sprites2;
      else
        spriteArray = sprites3;
    }

    this.createSprite(spriteArray, now);
  }

  createSprite(spriteArray, now) {
    var x = this.x + this.width / 2 - 40;
    var y = this.y;

    var secondsPassed = (now - this.timeStart) / 1000;
    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration(now));
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));

    var newSprite = new SpriteProxy(Random.getInt(spriteArray.baseAnimation.frameCount), x, y);

    newSprite.changeVelocity(velocityX, velocityY, now);
    spriteArray.sprites.push(newSprite);
  }

  dropMeteor(now) {
    this.dropFromOne(now, redMeteors, blueMeteors, purpleMeteors);
  }

  dropSeed(now: number, args?: string) {
    if (args && args.toLowerCase() === 'pink')
      this.createSprite(pinkSeeds, now);
    else if (args && args.toLowerCase() === 'blue')
      this.createSprite(blueSeeds, now);
    else if (args && args.toLowerCase() === 'yellow')
      this.createSprite(yellowSeeds, now);
    else if (args && args.toLowerCase() === 'purple')
      this.createSprite(purpleSeeds, now);
    //else if (args && args.toLowerCase() === 'grass')
    //  this.createSprite(greenSeeds, now);
    else 
      this.dropFromOne(now, pinkSeeds, blueSeeds, purpleSeeds, yellowSeeds);
  }
}

const VerticalThrust: number = 3;
const HorizontalThrust: number = 3;
const CodeRushedPosX = 86;
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