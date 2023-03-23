class ScalableAnimation extends AnimatedElement {
  elasticIn: boolean;
  targetScale = -1;
  originalScale: number;
  deltaScale: number = undefined;
  waitToScale = 0;

  constructor(x: number, y: number, lifeSpanMs = -1) {
    super(x, y, lifeSpanMs);
    this.scale = 1;
    this.originalScale = -1;
  }

  lerp(start: number, end: number, percentComplete: number) {
    return start + (end - start) * percentComplete;
  }

  static lerpc(start: number, change: number, percentComplete: number) {
    return start + change * percentComplete;
  }

  resizeToContent(left: number, top: number, width: number, height: number) {
    // Descendants can override...
  }

  getScale(now: number) {
    let thisScale: number = this.scale;
    if (this.targetScale >= 0) {

      if (this.originalScale < 0)
        this.originalScale = this.scale;

      if (this.deltaScale === undefined)
        this.deltaScale = this.targetScale - this.originalScale;

      const scaleStartTime: number = this.lifetimeStart + this.waitToScale;
      const timePassed: number = now - scaleStartTime;

      if (timePassed > 0) {
        const lifespan: number = this.expirationDate - scaleStartTime;

        let percentComplete: number;
        if (lifespan === 0)
          percentComplete = 1;
        else
          percentComplete = Math.max(Math.min(1, timePassed / lifespan), 0);

        let easedPercent: number;
        if (this.elasticIn)
          easedPercent = EasePoint.inElastic(percentComplete);
        else
          easedPercent = percentComplete;

        thisScale = ScalableAnimation.lerpc(this.originalScale, this.deltaScale, easedPercent);
      }
    }
    return thisScale;
  }
}