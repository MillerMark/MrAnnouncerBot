const globalFramesToLoad: number = 1;
const globalFramesToCount: number = 2;
var globalBypassFrameSkip: boolean = false;

class Part {
  static loadSprites: boolean; // Was globalLoadSprites
  images: HTMLImageElement[];
  frameIndex: number;
  reverse: boolean;
  lastUpdateTime: any;
  frameCount: number;

  constructor(private fileName: string, frameCount: number,
    private animationStyle: AnimationStyle,
    private offsetX: number,
    private offsetY: number,
    private frameRate = 100,
    public jiggleX: number = 0,
    public jiggleY: number = 0,
    padFileIndex: boolean = false) {

    this.images = [];
    this.frameIndex = 0;
    this.reverse = false;
    this.lastUpdateTime = null;
    var actualFrameCount = 0;
    var numDigits: number;
    if (frameCount > 999)
      numDigits = 4;
    else if (frameCount > 99)
      numDigits = 3;
    else if (frameCount > 9)
      numDigits = 2;
    else
      numDigits = 1;

    let framesToLoad: number = 1;
    let framesToCount: number = 1;

    if (frameCount > 60 && !globalBypassFrameSkip) {
      framesToLoad = globalFramesToLoad;
      framesToCount = globalFramesToCount;
      frameRate = framesToCount / framesToLoad;
    }

    if (Part.loadSprites) {
      let totalFramesToLoad = frameCount * framesToLoad / framesToCount;
      let frameIncrementor = frameCount / totalFramesToLoad;
      let absoluteIndex = 0;

      for (var i = 0; i < totalFramesToLoad; i++) {
        var image = new Image();
        var indexStr: string = Math.round(absoluteIndex).toString();
        while (padFileIndex && indexStr.length < numDigits)
          indexStr = '0' + indexStr;
        image.src = Folders.assets + fileName + indexStr + '.png';
        this.images.push(image);

        actualFrameCount++;
        absoluteIndex += frameIncrementor;
      }
      //for (var i = 0; i < frameCount; i++) {
      //  var image = new Image();
      //  var indexStr: string = i.toString();
      //  while (padFileIndex && indexStr.length < numDigits)
      //    indexStr = '0' + indexStr;
      //  image.src = Folders.assets + fileName + indexStr + '.png';
      //  this.images.push(image);
      //  actualFrameCount++;
      //}
    }
    else {
      var image = new Image();
      this.images.push(image);
    }

    this.frameCount = actualFrameCount;
  }

  fileExists(url) {
    var http = new XMLHttpRequest();
    http.open('HEAD', url, false);
    http.send();
    return http.status != 404;
  }

  isOnLastFrame() {
    return this.frameIndex == this.frameCount - 1;
  }

  isOnFirstFrame() {
    return this.frameIndex == 0;
  }

  advanceFrameIfNecessary() {
    if (!this.lastUpdateTime) {
      this.lastUpdateTime = performance.now();
      return;
    }

    var now: number = performance.now();
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
        else // AnimationStyle.Loop
          this.frameIndex = this.frameCount - 1;
    }
    else {
      this.frameIndex++;
      if (this.frameIndex >= this.frameCount)
        if (this.animationStyle == AnimationStyle.Sequential)
          this.frameIndex = this.frameCount - 1;
        else // AnimationStyle.Loop
          this.frameIndex = 0;
    }

    this.lastUpdateTime = performance.now();
  }

  getJiggle(amount: number) {
    if (amount == 0 || !amount)
      return 0;
    return Random.between(-amount, amount + 1);
  }

  draw(context: CanvasRenderingContext2D, x: number, y: number) {
    this.advanceFrameIfNecessary();
    this.drawByIndex(context, x, y, this.frameIndex);
  }

  drawByIndex(context: CanvasRenderingContext2D, x: number, y: number, frameIndex: number): void {
    if (!this.images[frameIndex]) {
      console.error('frameIndex: ' + frameIndex + ', fileName: ' + this.fileName);
    }
    else context.drawImage(this.images[frameIndex],
      x + this.offsetX + this.getJiggle(this.jiggleX),
      y + this.offsetY + this.getJiggle(this.jiggleY));
  }

  drawCroppedByIndex(context: CanvasRenderingContext2D, x: number, y: number, frameIndex: number,
    sx: number, sy: number, sw: number, sh: number, dw: number, dh: number): void {
                     //` ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)
    let dx: number = x + this.offsetX + this.getJiggle(this.jiggleX);
    let dy: number = y + this.offsetY + this.getJiggle(this.jiggleY);
    context.drawImage(this.images[frameIndex], sx, sy, sw, sh, dx, dy, dw, dh);
  }
}