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

	getSpritesByName(name: string): Sprites {
		for (let i = 0; i < this.allSprites.length; i++) {
			if (this.allSprites[i].name == name)
				return this.allSprites[i];
		}
		for (let i = 0; i < this.childCollections.length; i++) {
			let sprites: Sprites = this.childCollections[i].getSpritesByName(name);
			if (sprites != null)
				return sprites;
		}
		return null;
	}


  checkCollisionAgainst(compareCollection: SpriteCollection, collisionFoundFunction: (meteor: SpriteProxy, sprite: SpriteProxy, now: number) => void, now: number): void {
    this.allSprites.forEach(function (theseSprites: Sprites) {
      compareCollection.allSprites.forEach(function (compareSprites: Sprites) {
        theseSprites.checkCollisionAgainst(compareSprites, collisionFoundFunction, now);
      });
    });
	}

	checkCollisionAgainstSprites(compareSprites: Sprites, collisionFoundFunction: (meteor: SpriteProxy, sprite: SpriteProxy, now: number) => void, now: number): void {
		this.allSprites.forEach(function (theseSprites: Sprites) {
			theseSprites.checkCollisionAgainst(compareSprites, collisionFoundFunction, now);
		});
	}

  destroyAll(): void{
    this.allSprites.forEach(function (theseSprites: Sprites) { theseSprites.destroyAll(); });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.destroyAll(); });
  }

  destroyAllBy(lifeTimeMs: number): any {
    this.allSprites.forEach(function (theseSprites: Sprites) {
      theseSprites.destroyAllBy(lifeTimeMs);
    });

    this.childCollections.forEach(function (spriteCollection: SpriteCollection) {
        spriteCollection.destroyAllBy(lifeTimeMs);
      });
	}

	destroyAllInExactly(lifeTimeMs: number): any {
		this.allSprites.forEach(function (theseSprites: Sprites) {
			theseSprites.destroyAllInExactly(lifeTimeMs);
		});

		this.childCollections.forEach(function (spriteCollection: SpriteCollection) {
			spriteCollection.destroyAllInExactly(lifeTimeMs);
		});
	}

  draw(context: CanvasRenderingContext2D, now: number): void {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.draw(context, now) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.draw(context, now); });
  }

  destroy(matchData: any, destroyFunc?: (spriteProxy: SpriteProxy, spriteWidth: number, spriteHeight: number) => void): any {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.destroy(matchData, destroyFunc) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.destroy(matchData, destroyFunc) });
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

  updatePositions(now: number): any {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.updatePositions(now) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.updatePositions(now) });
  }
}
