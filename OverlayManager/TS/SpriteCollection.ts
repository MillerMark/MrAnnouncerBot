class SpriteCollection {
  allSprites: Sprites[];
  childCollections: SpriteCollection[];

  constructor() {
    this.allSprites = new Array<Sprites>();
    this.childCollections = new Array<SpriteCollection>();
  }

  add(sprites: Sprites): void {
    this.allSprites.push(sprites);
  }

  addCollection(spriteCollection: SpriteCollection): void {
    this.childCollections.push(spriteCollection);
  }

  bounce(left: number, top: number, right: number, bottom: number, now: number): void {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.bounce(left, top, right, bottom, now) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.bounce(left, top, right, bottom, now) });
  }

  checkCollisionAgainst(compareCollection: SpriteCollection, collisionFoundFunction: (meteor: SpriteProxy, sprite: SpriteProxy, now: number) => void, now: number): void {
    this.allSprites.forEach(function (theseSprites: Sprites) {
      compareCollection.allSprites.forEach(function (compareSprites: Sprites) {
        theseSprites.checkCollisionAgainst(compareSprites, collisionFoundFunction, now);
      });
    });
  }

  destroyAllBy(lifeTimeMs: number): any {
    this.allSprites.forEach(function (theseSprites: Sprites) {
      theseSprites.sprites.forEach(function (sprite: SpriteProxy) {
        sprite.destroyBy(lifeTimeMs);
      });
    });

    this.childCollections.forEach(function (spriteCollection: SpriteCollection) {
        spriteCollection.destroyAllBy(lifeTimeMs);
      });
  }

  draw(context: CanvasRenderingContext2D, now: number): void {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.draw(context, now) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.draw(context, now); });
  }

  destroy(userId: string, destroyFunc?: (spriteProxy: SpriteProxy, spriteWidth: number, spriteHeight: number) => void): any {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.destroy(userId, destroyFunc) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.destroy(userId, destroyFunc) });
  }

  changingDirection(now: number): any {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.changingDirection(now) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.changingDirection(now) });
  }

  find(matchData: string): SpriteProxy {
    for (var i = 0; i < this.allSprites.length; i++) {
      let sprites: Sprites = this.allSprites[i];
      let foundSprite: SpriteProxy = sprites.find(matchData);
      if (foundSprite)
        return foundSprite;
    }

    for (var i = 0; i < this.childCollections.length; i++) {
      let spriteCollection: SpriteCollection = this.childCollections[i];
      let foundSprite: SpriteProxy = spriteCollection.find(matchData);
      if (foundSprite)
        return foundSprite;
    }
    return null;
  }
}
