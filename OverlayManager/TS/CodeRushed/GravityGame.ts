class GravityGame {
  startTime: Date;
  digits: Sprites ;
  public score: Digits;
  constructor() {
    this.startTime = new Date();

    if (loadCopyrightedContent) {
        this.digits = new Sprites("Numbers/Blue", 12, 0, AnimationStyle.Static);
        this.digits.sprites = [];
        this.digits.sprites.push(new SpriteProxy(0, 1000, 0));
        this.score = new Digits(DigitSize.small, 1000, 0);
        this.score.value = 0;
    }
  }

  draw(context: CanvasRenderingContext2D) {
    this.score.draw(context);
  }

  end(): any {
    if (!(activeGame instanceof DroneGame))
      return;
    activeGame.allWalls.destroyAll();
    activeGame.endCaps.destroyAll();
    activeGame.coins.destroyAll();
  }
}
