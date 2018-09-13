var SpriteProxy = (function () {
    function SpriteProxy(startingFrameNumber, x, y) {
        this.x = x;
        this.y = y;
        this.frameIndex = startingFrameNumber;
        this.timeStart = new Date();
        this.velocityX = 0;
        this.velocityY = 0;
        this.startX = x;
        this.startY = y;
    }
    SpriteProxy.prototype.bounce = function (left, top, right, bottom, width, height, now) {
        var secondsPassed = (now - this.timeStart) / 1000;
        var horizontalBounceDecay = 1;
        var verticalBounceDecay = 1;
        var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, 0);
        var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, Gravity.earth);
        var hitLeftWall = velocityX < 0 && this.x < left;
        var hitRightWall = velocityX > 0 && this.x + width > right;
        var hitTopWall = velocityY < 0 && this.y < top;
        var hitBottomWall = velocityY > 0 && this.y + height > bottom;
        var newVelocityX = this.velocityX;
        var newVelocityY = this.velocityY;
        if (hitLeftWall || hitRightWall)
            newVelocityX = -velocityX * horizontalBounceDecay;
        if (hitTopWall || hitBottomWall)
            newVelocityY = -velocityY * verticalBounceDecay;
        if (hitLeftWall || hitRightWall || hitTopWall || hitBottomWall)
            this.changeVelocity(newVelocityX, newVelocityY, now);
        return hitBottomWall;
    };
    SpriteProxy.prototype.advanceFrame = function (frameCount) {
        this.frameIndex++;
        if (this.frameIndex >= frameCount)
            this.frameIndex = 0;
    };
    SpriteProxy.prototype.changeVelocity = function (velocityX, velocityY, now) {
        this.timeStart = now;
        this.velocityX = velocityX;
        this.velocityY = velocityY;
        this.startX = this.x;
        this.startY = this.y;
    };
    SpriteProxy.prototype.updatePosition = function (now) {
        var secondsPassed = (now - this.timeStart) / 1000;
        var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, 0);
        this.x = this.startX + Physics.metersToPixels(xDisplacement);
        var yDisplacement = Physics.getDisplacement(secondsPassed, this.velocityY, Gravity.earth);
        this.y = this.startY + Physics.metersToPixels(yDisplacement);
    };
    return SpriteProxy;
}());
//# sourceMappingURL=SpriteProxy.js.map