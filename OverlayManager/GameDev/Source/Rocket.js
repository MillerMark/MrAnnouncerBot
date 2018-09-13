var Rocket = (function () {
    function Rocket(x, y) {
        var upperLeftOffsetX = 9;
        var upperLeftOffsetY = 9;
        x -= upperLeftOffsetX;
        y -= upperLeftOffsetY;
        this.x = x;
        this.y = y;
        var flameFrameInterval = 60;
        var now = new Date();
        this.hoverThrusterRestoreTime = now;
        this.mainThrusterOfftime = now;
        this.leftThrusterOfftime = now;
        this.rightThrusterOfftime = now;
        this.lastHorizontalAcceleration = 0;
        this.lastVerticalAcceleration = 0;
        this.loggingState = true;
        this.hoverThrustersAudio = new Audio('Assets/Sound Effects/Hover Thruster Loop.wav');
        this.hoverThrustersAudio.loop = true;
        this.hoverDropThrustersAudio = new Audio('Assets/Sound Effects/Hover Thruster Drop Loop.wav');
        this.hoverDropThrustersAudio.loop = true;
        this.mainThrustersAudio = new Audio('Assets/Sound Effects/MainThrustersLoop.wav');
        this.mainThrustersAudio.loop = true;
        this.mainThrustersAudio = new Audio('Assets/Sound Effects/HorizontalRocketLoop.wav');
        this.mainThrustersAudio.loop = true;
        this.motorExtendAudio = new Audio('Assets/Sound Effects/EngineExtend4.wav');
        this.motorRetractAudio = new Audio('Assets/Sound Effects/EngineRetract.wav');
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
        var chuteFrameInterval = 100;
        this.chute = new Part("ChuteExtend", 5, AnimationStyle.Sequential, ChuteExtendX + x, ChuteExtendY + y, chuteFrameInterval);
        this.chuteDeployed = false;
        this.chuteSailsAreFull = false;
        this.chuteRetracting = false;
        this.changeVelocity(0, 0, now);
        this.isDocked = true;
        this.worldAcceleration = Gravity.earth;
        this.stopped = true;
        this.timeStart = now;
        this.width = 298;
        this.height = 52;
        this.enginesRetracted = true;
    }
    Rocket.prototype.deployChute = function (now) {
        if (this.isDocked)
            return;
        this.logState('Deploying chute...');
        this.chuteDeployed = true;
        this.chuteSailsAreFull = false;
        this.retractEngines(now);
    };
    Rocket.prototype.onChutesFullyOpen = function (now) {
        var secondsPassed = (now.getTime() - this.timeStart.getTime()) / 1000;
        var newVelocity = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));
        if (newVelocity > ChuteMaxVelocity)
            newVelocity > ChuteMaxVelocity;
        this.changeVelocity(this.velocityX, newVelocity, now);
        this.chuteSailsAreFull = true;
    };
    Rocket.prototype.retractChutes = function (now) {
        this.logState('Retracting chutes...');
        this.changeVelocity(this.velocityX, this.velocityY, now);
        this.chuteDeployed = false;
        this.chuteSailsAreFull = false;
        this.chuteRetracting = true;
        this.chute.reverse = true;
    };
    Rocket.prototype.getVerticalAcceleration = function (now) {
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
        if (this.hoverThrusterRestoreTime > now)
            return this.worldAcceleration / 2;
        if (this.mainThrusterOfftime > now)
            return -this.worldAcceleration / 2;
        return 0;
    };
    Rocket.prototype.getHorizontalAcceleration = function (now) {
        if (this.isDocked)
            return 0;
        var horizontalThrust = 5;
        if (this.docking)
            if (this.dockedLeft)
                return 0;
            else
                return -horizontalThrust;
        var acceleration = 0;
        if (this.leftThrusterOfftime > now)
            acceleration += horizontalThrust;
        if (this.rightThrusterOfftime > now)
            acceleration -= horizontalThrust;
        return acceleration;
    };
    Rocket.prototype.bounce = function (left, top, right, bottom, now) {
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
        if (hitBottomWall && myRocket.chuteDeployed)
            this.retractChutes(now);
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
        if (hitLeftWall || hitRightWall || hitTopWall || hitBottomWall)
            this.changeVelocity(newVelocityX, newVelocityY, now);
    };
    Rocket.prototype.getNowPlus = function (seconds) {
        var t = new Date();
        t.setMilliseconds(t.getMilliseconds() + seconds * 1000);
        return t;
    };
    Rocket.prototype.changingDirection = function (now) {
        var secondsPassed = (now - this.timeStart) / 1000;
        var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration(now));
        var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));
        this.changeVelocity(velocityX, velocityY, now);
    };
    Rocket.prototype.dock = function (now) {
        this.logState('Docking...');
        if (this.enginesRetracted)
            this.extendEngines(now);
        this.changingDirection(now);
        this.docking = true;
    };
    Rocket.prototype.extendEngines = function (now) {
        this.changingDirection(now);
        this.logState('Extending engines...');
        this.extendingEngines = true;
        this.extensionStartTime = now;
        this.extensionEndTime = this.getNowPlus(0.4);
        this.motorExtendAudio.play();
    };
    Rocket.prototype.retractEngines = function (now) {
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
        this.retractionEndTime = this.getNowPlus(0.4);
        this.killInProgressVerticalDrop(now);
        this.killInProgressVerticalThrust(now);
        this.killInProgressHorizontalThrust(now);
        this.motorRetractAudio.play();
    };
    Rocket.prototype.killInProgressHorizontalThrust = function (now) {
        if (this.leftThrusterOfftime > now) {
            this.leftThrusterOfftime = now;
            this.wasFiringLeftThruster = false;
        }
        if (this.rightThrusterOfftime > now) {
            this.rightThrusterOfftime = now;
            this.wasFiringRightThruster = false;
        }
    };
    Rocket.prototype.killInProgressVerticalThrust = function (now) {
        if (this.mainThrusterOfftime > now) {
            this.mainThrusterOfftime = now;
            this.wasFiringMainThrusters = false;
        }
    };
    Rocket.prototype.killHoverThrusters = function (now) {
        if (this.docking)
            return;
        this.changingDirection(now);
        this.logState('Shutting off hover thrusters...');
        this.wasKillingThrusters = true;
        this.hoverThrusterRestoreTime = this.getNowPlus(1);
        this.killInProgressVerticalThrust(now);
        this.hoverThrustersAudio.pause();
        this.hoverDropThrustersAudio.play();
    };
    Rocket.prototype.killInProgressVerticalDrop = function (now) {
        if (this.hoverThrusterRestoreTime > now) {
            this.hoverThrusterRestoreTime = now;
            this.wasKillingThrusters = false;
        }
    };
    Rocket.prototype.fireMainThrusters = function (now) {
        if (this.retractingEngines || this.enginesRetracted || this.docking)
            return;
        this.changingDirection(now);
        this.logState('Firing main thrusters...');
        this.wasFiringMainThrusters = true;
        this.mainThrusterOfftime = this.getNowPlus(1);
        this.killInProgressVerticalDrop(now);
        this.mainThrustersAudio.play();
    };
    Rocket.prototype.fireRightQuick = function () {
        var now = new Date();
        myRocket.fireRightThruster(now);
        myRocket.rightThrusterOfftime = myRocket.getNowPlus(0.4);
    };
    Rocket.prototype.fireMainQuick = function () {
        var now = new Date();
        myRocket.mainThrustersAudio.play();
        myRocket.wasFiringMainThrusters = true;
        myRocket.fireMainThrusters(now);
        myRocket.mainThrusterOfftime = myRocket.getNowPlus(0.6);
    };
    Rocket.prototype.fireLeftThruster = function (now) {
        if (this.retractingEngines || this.enginesRetracted || this.docking)
            return;
        this.changingDirection(now);
        this.logState('Firing left thruster...');
        this.wasFiringLeftThruster = true;
        this.leftThrusterOfftime = this.getNowPlus(1);
        this.sideThrustersAudio.play();
        if (this.wasFiringRightThruster) {
            this.wasFiringRightThruster = false;
            this.rightThrusterOfftime = now;
        }
    };
    Rocket.prototype.fireRightThruster = function (now) {
        if (this.retractingEngines || this.enginesRetracted || this.docking)
            return;
        this.changingDirection(now);
        this.logState('Firing right thruster...');
        this.wasFiringRightThruster = true;
        this.rightThrusterOfftime = this.getNowPlus(1);
        this.sideThrustersAudio.play();
        if (this.wasFiringLeftThruster) {
            this.wasFiringLeftThruster = false;
            this.leftThrusterOfftime = now;
        }
    };
    Rocket.prototype.changeVelocity = function (velocityX, velocityY, now) {
        this.timeStart = now;
        this.velocityX = velocityX;
        this.velocityY = velocityY;
        this.startX = this.x;
        this.startY = this.y;
        this.stopped = false;
    };
    Rocket.prototype.changeVelocityBy = function (deltaVelocityX, deltaVelocityY, now) {
        var secondsPassed = (now.getTime() - this.timeStart.getTime()) / 1000;
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
    };
    Rocket.prototype.updatePosition = function (now) {
        if (this.stopped) {
            return;
        }
        var secondsPassed = (now.getTime() - this.timeStart.getTime()) / 1000;
        var hAccel = this.getHorizontalAcceleration(now);
        var vAccel = this.getVerticalAcceleration(now);
        var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, hAccel);
        if ((this.wasFiringLeftThruster && this.leftThrusterOfftime <= now) || (this.wasFiringRightThruster && this.rightThrusterOfftime <= now)) {
            this.startX = this.x;
        }
        var newX = this.startX + Physics.metersToPixels(xDisplacement);
        this.x = newX;
        var yDisplacement = Physics.getDisplacement(secondsPassed, this.velocityY, vAccel);
        if (this.chuteSailsAreFull && yDisplacement > ChuteMaxVelocity * secondsPassed)
            yDisplacement = ChuteMaxVelocity * secondsPassed;
        if ((this.wasFiringMainThrusters && this.mainThrusterOfftime <= now) || (this.wasKillingThrusters && this.hoverThrusterRestoreTime <= now)) {
            console.log('this.startY = this.y;');
            this.startY = this.y;
        }
        var newY = this.startY + Physics.metersToPixels(yDisplacement);
        this.y = newY;
    };
    Rocket.prototype.thrustersOff = function (now) {
        this.x = this.lastX;
        this.y = this.lastY;
        this.changeVelocity(this.lastVelocityX, this.lastVelocityY, now);
    };
    Rocket.prototype.launch = function (now) {
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
    };
    Rocket.prototype.checkRightThruster = function (now) {
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
    };
    Rocket.prototype.checkLeftThruster = function (now) {
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
    };
    Rocket.prototype.draw = function (context, now) {
        if (this.drawing)
            return;
        this.drawing = true;
        var bottomLeftEngineRetractionOffsetX = 0;
        var bottomLeftEngineRetractionOffsetY = 0;
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
                var percentageOut = timeIn / totalTime;
                if (percentageOut > 1) {
                    if (this.isUndocking) {
                        this.isUndocking = false;
                        var secondsPassed = (now - this.timeStart) / 1000;
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
                horizontalVelocityDecay = 0.99;
            }
            if (horizontalVelocityDecay != 1 || verticalVelocityDecay != 1) {
                this.changeVelocityBy(horizontalVelocityDecay, verticalVelocityDecay, now);
            }
        }
        var secondsPassed = (now - this.timeStart) / 1000;
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
    };
    Rocket.prototype.logState = function (message) {
        if (this.loggingState)
            console.log(message);
    };
    Rocket.prototype.dropMeteor = function (now) {
        var x = this.x + this.width / 2 - 40;
        var y = this.y;
        var secondsPassed = (new Date().getTime() - this.timeStart.getTime()) / 1000;
        var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalAcceleration(now));
        var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalAcceleration(now));
        var newMeteor = new SpriteProxy(Random.getInt(redMeteors.baseAnimation.frameCount), x, y);
        newMeteor.changeVelocity(velocityX, velocityY, now);
        redMeteors.sprites.push(newMeteor);
    };
    return Rocket;
}());
var CodeRushedPosX = 86;
var CodeRushedPosY = 159;
var LeftEngineX = 58 - CodeRushedPosX;
var LeftEngineY = 168 - CodeRushedPosY;
var LeftEngineRetractionOffsetX = 32;
var LeftEngineRetractionOffsetY = 0;
var LeftFlameX = 12 - CodeRushedPosX;
var LeftFlameY = 178 - CodeRushedPosY;
var LeftFlameJiggleX = 1;
var LeftFlameJiggleY = 0;
var RightEngineX = 375 - CodeRushedPosX;
var RightEngineY = 170 - CodeRushedPosY;
var RightEngineRetractionOffsetX = -32;
var RightEngineRetractionOffsetY = 0;
var RightFlameX = 392 - CodeRushedPosX;
var RightFlameY = 181 - CodeRushedPosY;
var RightFlameJiggleX = 1;
var RightFlameJiggleY = 0;
var BottomEngineAdjust = 40;
var BottomLeftEngineX = 132 - BottomEngineAdjust - CodeRushedPosX;
var BottomLeftEngineY = 210 - CodeRushedPosY;
var BottomLeftEngineRetractionOffsetX = 0;
var BottomLeftEngineRetractionOffsetY = -46;
var BottomRightEngineX = 279 + BottomEngineAdjust - CodeRushedPosX;
var BottomRightEngineY = 210 - CodeRushedPosY;
var BottomRightEngineRetractionOffsetX = 0;
var BottomRightEngineRetractionOffsetY = -46;
var BottomLeftFlameX = 137 - BottomEngineAdjust - CodeRushedPosX;
var BottomLeftFlameY = 228 - CodeRushedPosY;
var BottomLeftFlameJiggleX = 0;
var BottomLeftFlameJiggleY = 2;
var MiniBottomLeftFlameX = 149 - BottomEngineAdjust - CodeRushedPosX;
var MiniBottomLeftFlameY = 245 - CodeRushedPosY;
var MiniBottomLeftFlameJiggleX = 0;
var MiniBottomLeftFlameJiggleY = 2;
var BottomRightFlameX = 284 + BottomEngineAdjust - CodeRushedPosX;
var BottomRightFlameY = 228 - CodeRushedPosY;
var BottomRightFlameJiggleX = 0;
var BottomRightFlameJiggleY = 2;
var MiniBottomRightFlameX = 296 + BottomEngineAdjust - CodeRushedPosX;
var MiniBottomRightFlameY = 245 - CodeRushedPosY;
var MiniBottomRightFlameJiggleX = 0;
var MiniBottomRightFlameJiggleY = 1;
var ChuteExtendX = 144 - CodeRushedPosX;
var ChuteExtendY = 6 - CodeRushedPosY;
var ChuteMaxVelocity = 2;
//# sourceMappingURL=Rocket.js.map