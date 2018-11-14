const coinBlockWidth: number = 32;
const coinMargin: number = 12;
const blockMargin: number = 12;
const blockSize: number = 44;
const topMargin: number = 12;
const leftMargin: number = 12;

class Game {
  startTime: Date;
  digits: Sprites = new Sprites("Numbers/Blue", 12, 0, AnimationStyle.Static);
  public score: Digits;
  constructor() {
    this.startTime = new Date();
    this.digits.sprites = [];
    this.digits.sprites.push(new SpriteProxy(0, 1000, 0));
    this.score = new Digits(DigitSize.small, 1000, 0);
    this.score.value = 0;
  }

  draw(context: CanvasRenderingContext2D) {
    this.score.draw(context);
  }

  end(): any {
    allWalls.destroyAll();
    endCaps.destroyAll();
    coins.destroyAll();
  }
}

class GravityGames {
  activePlanet: Planet = null;
  planets: Planet[] = [];
  planetSurface: Part;
  activeGame: Game;


  newGame() {
    this.activeGame = new Game();
  }

  constructor() {

    //` ![](ADA02D1B4E37D1836A73BE11024DE9AD.png;;;0.00872,0.00872)
    var earth: Planet = new Planet('Earth', 9.8, 149.6, 5.97, 12756, 5514, 11.2, 23.9, 24, 365.2, 29.8, 0, 0.017, 23.4, 15, 1, 1, false, true, 'earth');
    //` ![](2EA5331AC1B69756EDB184EB5997EFAF.png;;;0.00872,0.00872)
    var mercury: Planet = new Planet('Mercury', 3.7, 57.9, 0.33, 4879, 5427, 4.3, 1407.6, 4222.6, 88, 47.4, 7, 0.205, 0.034, 167, 0, 0, false, true, 'mercury');
    //` ![](BB492F653A571E63E425D56AA5512DD3.png;;;0.00872,0.00872)
    var venus: Planet = new Planet('Venus', 8.9, 108.2, 4.87, 12104, 5243, 10.4, -5832.5, 2802, 224.7, 35, 3.4, 0.007, 177.4, 464, 92, 0, false, false, 'venus');
    //` ![](A96C922E7566C724B375C5D2F7A895E9.png;;;0.00872,0.00872)
    var mars: Planet = new Planet('Mars', 3.7, 227.9, 0.642, 6792, 3933, 5, 24.6, 24.7, 687, 24.1, 1.9, 0.094, 25.2, -65, 0.01, 2, false, false, 'mars');
    //`![](64417F2A450A7A56419E229A49E378CD.png;;;0.00872,0.00872)
    var neptune: Planet = new Planet('Neptune', 11, 4495.1, 102, 49528, 1638, 23.5, 16.1, 16.1, 59800, 5.4, 1.8, 0.011, 28.3, -200, undefined, 14, true, true, 'neptune');
    //`![](D4B38549C7C0A76715A8C4C000ACE2F3.png;;;0.00872,0.00872)
    var jupiter: Planet = new Planet('Jupiter', 23.1, 778.6, 1898, 142984, 1326, 59.5, 9.9, 9.9, 4331, 13.1, 1.3, 0.049, 3.1, -110, undefined, 67, true, true, 'jupiter');
    //`![](D56CFE4BC8A6D254685FE11E9346FBE9.png;;;0.00872,0.00872)
    var sun: Planet = new Planet('Sun', 274, 0, 1.94 * Math.pow(10, 27), 1.39 * Math.pow(10, 6), 1410, 618, 609.6, undefined, undefined, undefined, undefined, undefined, undefined, 5506.85, 8.68 * Math.pow(10, - 4), 0, false, true, 'sun');
    //`![](750FB5A3C4996042B8CB0497C7FF88BD.png;;;0.00872,0.00872)
    var uranus: Planet = new Planet('Uranus', 8.7, 2872.5, 86.8, 51118, 1271, 21.3, -17.2, 17.2, 30589, 6.8, 0.8, 0.046, 97.8, -195, undefined, 27, true, true, 'uranus');
    //`![](BAEBE18F0FC74D6C9E8BB1E70BC5AF63.png;;;0.00872,0.00872)
    var moon: Planet = new Planet('Moon', 1.6, 0.384, 0.073, 3475, 3340, 2.4, 655.7, 708.7, 27.3, 1, 5.1, 0.055, 6.7, -20, 0, 0, false, false, 'moon');
    //`![](600E64298818BB51AD56847BA5020676.png;;;0.00872,0.00872)
    var pluto: Planet = new Planet('Pluto', 0.7, 5906.4, 0.0146, 2370, 2095, 1.3, -153.3, 153.3, 90560, 4.7, 17.2, 0.244, 122.5, -225, 0.00001, 5, false, undefined, 'pluto');
    //`![](30A344602A04E9959730D7DCC5F55370.png;;;0.00872,0.00872)
    var saturn: Planet = new Planet('Saturn', 9, 1433.5, 568, 120536, 687, 35.5, 10.7, 10.7, 10747, 9.7, 2.5, 0.057, 26.7, -140, undefined, 62, true, true, 'saturn');

    this.planets.push(earth);
    this.planets.push(mercury);
    this.planets.push(venus);
    this.planets.push(mars);
    this.planets.push(pluto);
    this.planets.push(sun);
    this.planets.push(uranus);
    this.planets.push(neptune);
    this.planets.push(jupiter);
    this.planets.push(saturn);
    this.planets.push(moon);
    this.activePlanet = earth;
  }

  startGame(layout: string[]): void {
    if (this.activeGame)
      this.activeGame.end();
    this.newGame();
    this.addGameWalls(layout);
    this.addElements(layout, 'o', this.addPortal);
    this.addElements(layout, '*', this.addCoin);
  }

  addElements(layout: string[], charToMatch: string, addElement: (column: number, row: number) => void): void {
    for (var row = 0; row < layout.length; row++) {
      let line: string = layout[row];
      for (var column = 0; column < line.length; column++) {
        let char: string = line[column];
        if (char === charToMatch)  // Match!
          addElement(column, row);
      }
    }
  }

  addCoin(column: number, row: number): void {
    let x: number = coinMargin + column * (coinBlockWidth + coinMargin);
    let y: number = coinMargin + row * (coinBlockWidth + coinMargin);
    coins.sprites.push(new SpriteProxy(Random.getInt(coins.baseAnimation.frameCount), x, y));
  }

  addPortal(column: number, row: number): void {
    let x: number = coinMargin + column * (coinBlockWidth + coinMargin) + coinBlockWidth / 2;
    let y: number = coinMargin + row * (coinBlockWidth + coinMargin) + coinBlockWidth / 2;
    let portal: Portal = new Portal(0, x - Portal.size / 2, y - Portal.size / 2);
    portal.delayStart = Math.random() * 900;
    purplePortals.sprites.push(portal);
  }

  addHorizontalWall(wallStyle: WallStyle, startColumn: number, endColumn: number, row: number) {
    if (startColumn === 0 && endColumn === 0)
      return;

    endColumn++;

    let wallSprites: SpriteProxy[] = null;
    switch (wallStyle) {
      case WallStyle.Dashed: {
        wallSprites = horizontalDashedWall.sprites;
        break;
      }
      case WallStyle.Solid: {
        wallSprites = horizontalSolidWall.sprites;
        break;
      }
      case WallStyle.Double: {
        wallSprites = horizontalDoubleWall.sprites;
        break;
      }
      default:
        return;
    }
    if (!wallSprites)
      return;

    let centerX: number = blockSize * (startColumn + endColumn) / 2 - blockMargin / 2;
    let centerY: number = row * blockSize + endCaps.spriteHeight / 2 - blockSize / 2 + topMargin / 2;
    let length: number = (endColumn - startColumn) * blockSize - 2 * leftMargin;
    wallSprites.push(new Wall(0, centerX + leftMargin, centerY + topMargin, Orientation.Horizontal, wallStyle, length));
  }

  addVerticalWall(wallStyle: WallStyle, column: number, startRow: number, endRow: number): any {
    if (startRow === 0 && endRow === 0)
      return;

    endRow++;

    let wallSprites: SpriteProxy[] = null;
    switch (wallStyle) {
      case WallStyle.Dashed: {
        wallSprites = verticalDashedWall.sprites;
        break;
      }
      case WallStyle.Solid: {
        wallSprites = verticalSolidWall.sprites;
        break;
      }
      case WallStyle.Double: {
        wallSprites = verticalDoubleWall.sprites;
        break;
      }
      default:
        return;
    }
    if (!wallSprites)
      return;

    let centerY: number = blockSize * (startRow + endRow) / 2 - blockMargin / 2;
    let centerX: number = column * 44 + endCaps.spriteWidth / 2 - blockSize / 2 + topMargin / 2;
    let length: number = (endRow - startRow) * blockSize - 2 * leftMargin;

    wallSprites.push(new Wall(0, centerX + leftMargin, centerY + topMargin, Orientation.Vertical, wallStyle, length));
  }


  /* 
     ¦  Dashed vertical line
     -  Dashed horizontal line
     ═  Bouncy horizontal line
     ║  Bouncy vertical line
     |  Solid vertical line
     ─  Solid horizontal line
     +  Intersection
  */

  addHorizontalGameWalls(layout: string[]): void {
    // Process horizontally...
    let collectingWallStyle: WallStyle = WallStyle.None;
    for (var row = 0; row < layout.length; row++) {
      let line: string = layout[row];
      let wallStartColumn: number = 0;
      let wallEndColumn: number = 0;

      let addBrickToWall = (wallStyle: WallStyle) => {
        if (intersectionCount > 0) {
          if (collectingWallStyle === wallStyle) {
            wallEndColumn += intersectionCount;
          }
          else {
            wallEndColumn += intersectionCount / 2;
          }
        }

        if (collectingWallStyle !== wallStyle) {
          this.addHorizontalWall(collectingWallStyle, wallStartColumn, wallEndColumn, row);
          wallStartColumn = column - intersectionCount / 2;
        }

        wallEndColumn = column;
        collectingWallStyle = wallStyle;

        if (intersectionCount > 0)
          intersectionCount = 0;
      }
      let intersectionCount: number = 0;

      for (var column = 0; column < line.length; column++) {
        let char: string = line[column];

        // TODO: Handle "+" intersections.

        if (char === '-') {
          addBrickToWall(WallStyle.Dashed);
        }
        else if (char === '═') {
          addBrickToWall(WallStyle.Double);
        }
        else if (char === '─') {
          addBrickToWall(WallStyle.Solid);
        }
        else if (char === '+') {
          intersectionCount++;
          //addBrickToWall(collectingWallStyle);
        }
        else if (collectingWallStyle != WallStyle.None) {
          this.addHorizontalWall(collectingWallStyle, wallStartColumn, wallEndColumn + intersectionCount / 2, row);
          intersectionCount = 0;
          collectingWallStyle = WallStyle.None;
        }
        else 
          intersectionCount = 0;
      }

      if (collectingWallStyle != WallStyle.None) {
        this.addHorizontalWall(collectingWallStyle, wallStartColumn, wallEndColumn, row);
        collectingWallStyle = WallStyle.None;
      }
    }
  }

  addVerticalGameWalls(layout: string[]): any {
    let collectingWallStyle: WallStyle = WallStyle.None;

    let wallStartRow: number = 0;
    let wallEndRow: number = 0;

    let addBrickToWall = (wallStyle: WallStyle) => {
      if (intersectionCount > 0) {
        if (collectingWallStyle === wallStyle) {
          wallEndRow += intersectionCount;
        }
        else {
          wallEndRow += intersectionCount / 2;
        }
      }

      if (collectingWallStyle !== wallStyle) {
        this.addVerticalWall(collectingWallStyle, column, wallStartRow, wallEndRow);
        wallStartRow = row - intersectionCount / 2;
      }
      wallEndRow = row;
      collectingWallStyle = wallStyle;

      if (intersectionCount > 0)
        intersectionCount = 0;
    }

    let intersectionCount: number = 0;

    for (var column = 0; column < layout[0].length; column++) {
      for (var row = 0; row < layout.length; row++) {
        let line: string = layout[row];
        let char: string = line[column];

        // TODO: Handle "+" intersections.

        if (char === '¦') {
          addBrickToWall(WallStyle.Dashed);
        }
        else if (char === '║') {
          addBrickToWall(WallStyle.Double);
        }
        else if (char === '|') {
          addBrickToWall(WallStyle.Solid);
        }
        else if (char === '+') {
          intersectionCount++;
        }
        else if (collectingWallStyle != WallStyle.None) {
          this.addVerticalWall(collectingWallStyle, column, wallStartRow, wallEndRow + intersectionCount / 2);
          intersectionCount = 0;
          collectingWallStyle = WallStyle.None;
        }
        else 
          intersectionCount = 0;
      }

      if (collectingWallStyle != WallStyle.None) {
        this.addVerticalWall(collectingWallStyle, column, wallStartRow, wallEndRow);
        collectingWallStyle = WallStyle.None;
      }
    }
  }

  addGameWalls(layout: string[]): void {
    this.addHorizontalGameWalls(layout);
    this.addVerticalGameWalls(layout);
  }

  cyclePlanet() {
    for (var i = 0; i < this.planets.length; i++) {
      if (this.planets[i] == this.activePlanet) {
        if (i < this.planets.length - 1)
          this.setActivePlanet(this.planets[i + 1]);
        else
          this.setActivePlanet(this.planets[0]);
        return;
      }
    }
  }

  draw(context: CanvasRenderingContext2D) {
    const planetWidth: number = 552;
    const planetHeight: number = 246;

    this.planetSurface.draw(context, screenWidth - planetWidth, screenHeight - planetHeight);
    gravityGames.activeGame.draw(context);
  }

  setActivePlanet(planet: Planet) {
    let now: number = performance.now();
    myRocket.changingDirection(now);
    allMeteors.changingDirection(now);
    allSeeds.changingDirection(now);

    beesYellow.changingDirection(now);
    this.activePlanet = planet;
    this.planetSurface = new Part('Planets/' + planet.imageFileName, 1, AnimationStyle.Static, 0, 0);
  }

  selectPlanet(planetName: string): boolean {
    var self = this;
    this.planets.forEach(function (planet) {
      if (planet.name.toLowerCase() === planetName.toLowerCase()) {
        self.setActivePlanet(planet);
        return true;
      }
    });
    return false;
  }
}