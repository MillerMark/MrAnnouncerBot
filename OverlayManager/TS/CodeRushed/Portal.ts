class Portal extends SpriteProxy {
  static readonly size: number = 160;
  meteorsToDrop: Array<MeteorDrop> = new Array<MeteorDrop>();
  background: SpriteProxy;

  constructor(startingFrameNumber: number, x: number, y: number) {
    super(startingFrameNumber, x, y);
    if (!(activeDroneGame instanceof DroneGame))
      return;
    
    this.background = activeDroneGame.portalBackground.add(x, y);
  }

  drop(): void {
    if (!(activeDroneGame instanceof DroneGame))
      return;
    activeDroneGame.purpleMeteors.spriteProxies.push(new Meteor(Random.intMax(activeDroneGame.purpleMeteors.baseAnimation.frameCount), this.x + (Portal.size - meteorWidth) / 2, this.y + (Portal.size - meteorHeight) / 2));
  }

  set delayStart(delayMs: number) {
    super.delayStart = delayMs;
    this.background.lifetimeStart = performance.now() + delayMs;
  }

  drawAdornments(context: CanvasRenderingContext2D, now: number): void {
    //this.drawDiagnostics(context, now);
  }

  drawDiagnostics(context: CanvasRenderingContext2D, now: number): void {
    for (let i = 0; i < this.meteorsToDrop.length; i++) {
      const futurePoint: FuturePoint = this.meteorsToDrop[i].futurePoint;

      drawCrossHairs(context, futurePoint.x, futurePoint.y);

      context.font = '20px Arial';

      const secondsToIntersect: number = (futurePoint.absoluteTimeMs - now) / 1000;

      if (secondsToIntersect < 0) {
        context.fillStyle = '#F60';
        const rectSize = 100;
        context.fillRect(futurePoint.x - rectSize / 2, futurePoint.y - rectSize / 2, rectSize, rectSize);
      }
      else {
        context.fillStyle = '#000';
        context.fillText(secondsToIntersect.toFixed(2), futurePoint.x, futurePoint.y);
      }
    }
  }

  removeMeteor(userId: string) {
    for (let i = 0; i < this.meteorsToDrop.length; i++) {
      if (this.meteorsToDrop[i].owner === userId) {
        this.meteorsToDrop.splice(i, 1);
      }
    }
  }

  queueMeteor(meteorDrop: MeteorDrop) {
    this.removeMeteor(meteorDrop.owner);
    this.meteorsToDrop.push(meteorDrop);
  }

  checkApproaching(allDrones: SpriteCollection, now: number): void {
    const centerX: number = this.x + Portal.size / 2;

    const checkDrone = (drone: SpriteProxy) => {
      if (drone instanceof Drone) {
        const futurePoint: FuturePoint = drone.getFuturePoint(centerX, now);
        if (futurePoint !== null) {
          const distanceToDrop: number = futurePoint.y - (this.y + Portal.size / 2);
          if (distanceToDrop > 0) {
            const secondsToCrossover = (futurePoint.absoluteTimeMs - now) / 1000;
            const dropTimeMs: number = Physics.getDropTimeSeconds(Physics.pixelsToMeters(distanceToDrop), gravityGames.activePlanet.gravity) * 1000;
            const dropTimeSeconds: number = dropTimeMs / 1000;

            //console.log('dropTimeSeconds: ' + dropTimeSeconds.toFixed(2) + ', secondsToCrossover: ' + secondsToCrossover.toFixed(2));
            if (dropTimeSeconds > secondsToCrossover) {
              //console.log('not enough time to drop!');
              //this.removeMeteor(drone.userId);
              return;
            }

            const absoluteDropTime: number = futurePoint.absoluteTimeMs - dropTimeMs;
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
      spriteLists.spriteProxies.forEach(checkDrone);
    });

    allDrones.childCollections.forEach((spriteCollection: SpriteCollection) => {
      spriteCollection.allSprites.forEach((spriteLists: Sprites) => {
        spriteLists.spriteProxies.forEach(checkDrone);
      });
    });

    this.dropReadyMeteors(now);
  }

  dropReadyMeteors(now: number): any {
    for (let i = this.meteorsToDrop.length - 1; i >= 0; i--) {
      const meteorDrop: MeteorDrop = this.meteorsToDrop[i];
      if (meteorDrop.futureDropTime - now < 0) {
        if (!meteorDrop.dropped) {
          this.drop();
          meteorDrop.dropped = true;
        }

        if (meteorDrop.beyondLandTime - now < 0) {
          this.meteorsToDrop.splice(i, 1);
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