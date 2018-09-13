var Part = (function () {
    function Part(fileName, frameCount, animationStyle, offsetX, offsetY, frameRate, jiggleX, jiggleY) {
        if (frameRate === void 0) { frameRate = 100; }
        this.animationStyle = animationStyle;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.frameRate = frameRate;
        this.jiggleX = jiggleX;
        this.jiggleY = jiggleY;
        this.images = [];
        this.frameIndex = 0;
        this.reverse = false;
        this.lastUpdateTime = null;
        var actualFrameCount = 0;
        for (var i = 0; i < frameCount; i++) {
            var image = new Image();
            image.src = 'Assets/' + fileName + i + '.png';
            this.images.push(image);
            actualFrameCount++;
        }
        this.frameCount = actualFrameCount;
    }
    Part.prototype.fileExists = function (url) {
        var http = new XMLHttpRequest();
        http.open('HEAD', url, false);
        http.send();
        return http.status != 404;
    };
    Part.prototype.isOnLastFrame = function () {
        return this.frameIndex == this.frameCount - 1;
    };
    Part.prototype.isOnFirstFrame = function () {
        return this.frameIndex == 0;
    };
    Part.prototype.advanceFrameIfNecessary = function () {
        if (!this.lastUpdateTime) {
            this.lastUpdateTime = new Date();
            return;
        }
        var now = new Date();
        var msPassed = now - this.lastUpdateTime;
        if (msPassed < this.frameRate)
            return;
        if (this.animationStyle == AnimationStyle.Static)
            return;
        if (this.animationStyle == AnimationStyle.Random)
            this.frameIndex = Random.getInt(this.frameCount);
        if (this.reverse) {
            this.frameIndex--;
            if (this.frameIndex < 0)
                if (this.animationStyle == AnimationStyle.Sequential)
                    this.frameIndex = 0;
                else
                    this.frameIndex = this.frameCount - 1;
        }
        else {
            this.frameIndex++;
            if (this.frameIndex >= this.frameCount)
                if (this.animationStyle == AnimationStyle.Sequential)
                    this.frameIndex = this.frameCount - 1;
                else
                    this.frameIndex = 0;
        }
        this.lastUpdateTime = performance.now();
    };
    Part.prototype.getJiggle = function (amount) {
        if (amount == 0 || !amount)
            return 0;
        return Random.between(-amount, amount + 1);
    };
    Part.prototype.draw = function (context, x, y) {
        this.advanceFrameIfNecessary();
        this.drawByIndex(context, x, y, this.frameIndex);
    };
    Part.prototype.drawByIndex = function (context, x, y, frameIndex) {
        context.drawImage(this.images[frameIndex], x + this.offsetX + this.getJiggle(this.jiggleX), y + this.offsetY + this.getJiggle(this.jiggleY));
    };
    return Part;
}());
//# sourceMappingURL=Part.js.map