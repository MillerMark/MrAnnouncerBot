class Portal extends SpriteProxy {
  static readonly size: number = 160;
  meteorsToDrop: Array<MeteorDrop> = new Array<MeteorDrop>();
  background: SpriteProxy;

  constructor(startingFrameNumber: number, public x: number, public y: number) {
    super(startingFrameNumber, x, y);
    this.background = portalBackground.add(x, y);
  }

  drop(): void {
    purpleMeteors.sprites.push(new Meteor(Random.getInt(purpleMeteors.baseAnimation.frameCount), this.x + (Portal.size - meteorWidth) / 2, this.y + (Portal.size - meteorHeight) / 2));
  }

  set delayStart(delayMs: number) {
    super.delayStart = delayMs;
    this.background.timeStart = performance.now() + delayMs;
  }

  drawAdornments(context: CanvasRenderingContext2D, now: number): void {
    //this.drawDiagnostics(context, now);
  }

  drawDiagnostics(context: CanvasRenderingContext2D, now: number): void {
    for (var i = 0; i < this.meteorsToDrop.length; i++) {
      let futurePoint: FuturePoint = this.meteorsToDrop[i].futurePoint;

      drawCrossHairs(context, futurePoint.x, futurePoint.y);

      context.font = '20px Arial';

      let secondsToIntersect: number = (futurePoint.absoluteTimeMs - now) / 1000;

      if (secondsToIntersect < 0) {
        context.fillStyle = '#F60';
        const rectSize: number = 100;
        context.fillRect(futurePoint.x - rectSize / 2, futurePoint.y - rectSize / 2, rectSize, rectSize);
      }
      else {
        context.fillStyle = '#000';
        context.fillText(secondsToIntersect.toFixed(2), futurePoint.x, futurePoint.y);
      }
    }
  }

  removeMeteor(userId: string) {
    for (var i = 0; i < this.meteorsToDrop.length; i++) {
      if (this.meteorsToDrop[i].owner === userId) {
        this.meteorsToDrop.splice(i, 1);
      }
    }
  }

  queueMeteor(meteorDrop: MeteorDrop) {
    this.removeMeteor(meteorDrop.owner);
    console.log('this.meteorsToDrop.push(meteorDrop);');
    this.meteorsToDrop.push(meteorDrop);
  }

  checkApproaching(allDrones: SpriteCollection, now: number): any {
    let centerX: number = this.x + Portal.size / 2;

    let checkDrone = (drone: SpriteProxy) => {
      if (drone instanceof Drone) {
        let futurePoint: FuturePoint = drone.getFuturePoint(centerX, now);
        if (futurePoint != null) {
          let distanceToDrop: number = futurePoint.y - (this.y + Portal.size / 2);
          if (distanceToDrop > 0) {
            let secondsToCrossover = (futurePoint.absoluteTimeMs - now) / 1000;
            let dropTimeMs: number = Physics.getDropTime(Physics.pixelsToMeters(distanceToDrop), gravityGames.activePlanet.gravity) * 1000;
            let dropTimeSeconds: number = dropTimeMs / 1000;

            console.log('dropTimeSeconds: ' + dropTimeSeconds.toFixed(2) + ', secondsToCrossover: ' + secondsToCrossover.toFixed(2));
            if (dropTimeSeconds > secondsToCrossover) {
              console.log('not enough time to drop!');
              //this.removeMeteor(drone.userId);
              return;
            }

            let absoluteDropTime: number = futurePoint.absoluteTimeMs - dropTimeMs;
            if (absoluteDropTime > now)
              if (absoluteDropTime - now < 10000)
                this.queueMeteor(new MeteorDrop(absoluteDropTime, drone.userId, futurePoint));
              else
                this.removeMeteor(drone.userId);
          }
        }
      }
    }

    allDrones.allSprites.forEach((spriteLists: Sprites) => {
      spriteLists.sprites.forEach(checkDrone);
    });

    allDrones.childCollections.forEach((spriteCollection: SpriteCollection) => {
      spriteCollection.allSprites.forEach((spriteLists: Sprites) => {
        spriteLists.sprites.forEach(checkDrone);
      });
    });

    this.dropReadyMeteors(now);
  }

  dropReadyMeteors(now: number): any {
    for (var i = this.meteorsToDrop.length - 1; i >= 0; i--) {
      let meteorDrop: MeteorDrop = this.meteorsToDrop[i];
      if (meteorDrop.futureDropTime - now < 0) {
        if (!meteorDrop.dropped) {
          this.drop();
          meteorDrop.dropped = true;
        }

        if (meteorDrop.beyondLandTime - now < 0) {
          this.meteorsToDrop.splice(i, 1);
          console.log('this.meteorsToDrop.splice(i, 1);');
        }
      }
    }
  }
  /* 
   1. Max Future prediction cut-time.
      * Based on height and gravity.
      * Only look this far ahead.
      
   2. What *qualified* drones are crossing my x in the next ____ (cutoff time) seconds.
      * Any? If so, add them to the queue.
        * [X] Cross time. Expected altitude. Calculate the drop time.
      * qualified means not carrying, && NOT dropped.
          => We need to track dropped meteors until they explode or **are caught**.
          => We need to track queued meteors and bind them to specific drones.
      
   3. Scan frequency????
      * If we notice perf issues, we'll impose a min time span between checks
  */
}

class MeteorDrop {
  dropped: boolean;
  beyondLandTime: number;
  constructor(public futureDropTime: number, public owner: string, public futurePoint: FuturePoint) {
    this.beyondLandTime = futurePoint.absoluteTimeMs + 5000;
  }
}