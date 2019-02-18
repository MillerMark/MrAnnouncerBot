class GravityGame {
  startTime: Date;
  digits: Sprites;
  public score: Digits;
  constructor() {
    this.startTime = new Date();

    if (loadCopyrightedContent) {
      let saveFolder: string = Folders.assets;
      Folders.assets = 'GameDev/Assets/DroneGame/';
      this.digits = new Sprites("Numbers/Blue", 12, 0, AnimationStyle.Static);
      this.digits.sprites = [];
      this.digits.sprites.push(new SpriteProxy(0, 1000, 0));
      this.score = new Digits(DigitSize.small, 1000, 0);
      this.score.value = 0;
      Folders.assets = saveFolder;
    }
  }

  draw(context: CanvasRenderingContext2D) {
    this.score.draw(context);
  }

  end(): any {
    if (activeDroneGame instanceof DroneGame) {
      activeDroneGame.allWalls.destroyAll();
      activeDroneGame.endCaps.destroyAll();
      activeDroneGame.coins.destroyAll();
    }
  }
}
