class Part {
  constructor(fileName, frameCount, partStyle, offsetX, offsetY, frameRate, jiggleX, jiggleY) {
    this.offsetX = offsetX;
    this.offsetY = offsetY;
    this.frameRate = frameRate;
    this.jiggleX = jiggleX;
    this.jiggleY = jiggleY;
    this.partStyle = partStyle;
    this.images = [];
    this.frameIndex = 0;
    this.reverse = false;
    this.frameCount = frameCount;
    this.lastUpdateTime = null;
    for (var i = 0; i < frameCount; i++) {
      var image = new Image();
      image.src = "Assets/" + fileName + i + '.png';
      this.images.push(image);
    }
  }

  isOnLastFrame() {
    return this.frameIndex == this.frameCount - 1;
  }

  isOnFirstFrame() {
    return this.frameIndex == 0;
  }

  advanceFrameIfNecessary() {
    if (!this.lastUpdateTime) {
      this.lastUpdateTime = new Date();
      return;
    }

    var now = new Date();
    var msPassed = now - this.lastUpdateTime;
    if (msPassed < this.frameRate)
      return;

    if (this.partStyle == PartStyle.Static)
      return;
    if (this.partStyle == PartStyle.Random)
      this.frameIndex = Random.getInt(this.frameCount);

    if (this.reverse) {
      this.frameIndex--;
      if (this.frameIndex < 0)
        if (this.partStyle == PartStyle.Sequential)
          this.frameIndex = 0;
        else // PartStyle.Loop
          this.frameIndex = this.frameCount - 1;
    }
    else {
      this.frameIndex++;
      if (this.frameIndex >= this.frameCount)
        if (this.partStyle == PartStyle.Sequential)
          this.frameIndex = this.frameCount - 1;
        else // PartStyle.Loop
          this.frameIndex = 0;
    }

    this.lastUpdateTime = new Date();
  }

  getJiggle(amount) {
    if (amount == 0 || !amount)
      return 0;
    return Random.between(-amount, amount + 1);
  }

  draw(context, x, y) {
    this.advanceFrameIfNecessary();
    this.drawByIndex(context, x, y, this.frameIndex);
  }

  drawByIndex(context, x, y, frameIndex) {
    context.drawImage(this.images[frameIndex],
      x + this.offsetX + this.getJiggle(this.jiggleX),
      y + this.offsetY + this.getJiggle(this.jiggleY));

  }
}

var PartStyle = {
  'Static': 1,
  'Random': 2,
  'Sequential': 3,
  'Loop': 4,
}
Object.freeze(PartStyle);