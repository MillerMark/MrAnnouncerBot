
// ![](204DC0A5D26C752B4ED0E8696EBE637B.png)
class Drone extends SpriteProxy {
  constructor(startingFrameNumber: number, public x: number, public y: number) {
    super(startingFrameNumber, x, y);
  }
}


// ![](C5EB2EC713378A58AA48D1EA9E0B9B41.png;;;0.01153,0.01153)

class BumbleBee extends SpriteProxy {
  constructor(startingFrameNumber: number, public x: number , public y: number) {
    super(startingFrameNumber, x, y);
  }
}