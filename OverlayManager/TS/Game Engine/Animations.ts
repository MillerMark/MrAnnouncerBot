class Animations {
  animationProxies: Array<AnimatedElement> = new Array<AnimatedElement>();
  constructor() {

  }

  clear() {
    this.animationProxies = new Array<AnimatedElement>();
  }

  addText(centerPos: Vector, text: string, lifeSpanMs = -1): TextEffect {
    const textEffect: TextEffect = new TextEffect(centerPos.x, centerPos.y, lifeSpanMs);
    textEffect.text = text;
    this.animationProxies.push(textEffect);
    return textEffect;
  }

  addLine(x: number, y: number, width: number, color: string, lifespan: number, lineThickness: number): AnimatedLine {
    const line: AnimatedLine = new AnimatedLine(x, y, width, color, lifespan, lineThickness);
    line.scale = 1;
    line.targetScale = 1;
    this.animationProxies.push(line);
    return line;
  }

  addRectangle(x: number, y: number, width: number, height: number, fillColor: string, outlineColor: string, lifespanMs: number, lineThickness: number = 1, opacity: number = 1): AnimatedRectangle {
    const rectangle: AnimatedRectangle = new AnimatedRectangle(x, y, width, height, fillColor, outlineColor, lifespanMs, lineThickness);
    rectangle.opacity = opacity;
    rectangle.scale = 1;
    rectangle.targetScale = 1;
    this.animationProxies.push(rectangle);
    return rectangle;
  }

  render(context: CanvasRenderingContext2D, now: number) {
    this.animationProxies.forEach(function (animatedElement: AnimatedElement) {
      animatedElement.render(context, now);
    });
  }

  removeExpiredAnimations(now: number): void {
    for (let i = this.animationProxies.length - 1; i >= 0; i--) {
      const animatedElement: AnimatedElement = this.animationProxies[i];
      if (animatedElement.expirationDate) {
        if (!animatedElement.stillAlive(now)) {
          animatedElement.destroying();
          animatedElement.removing();
          if (!animatedElement.isRemoving) {
            this.animationProxies.splice(i, 1);
          }
        }
      }
    }
  }

  updatePositions(now: number): void {
    for (let i = this.animationProxies.length - 1; i >= 0; i--) {
      const animatedElement: AnimatedElement = this.animationProxies[i];
      animatedElement.updatePosition(now);
    }
  }
}