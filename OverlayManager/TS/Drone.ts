 //` ![](204DC0A5D26C752B4ED0E8696EBE637B.png)
class Drone extends SpriteProxy {
  constructor(startingFrameNumber: number, public x: number, public y: number) {
    super(startingFrameNumber, x, y);
  }
}
