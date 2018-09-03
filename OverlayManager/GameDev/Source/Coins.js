class Coin {
  constructor(startingFrameNumber, x, y) {
    this.frameCount = 165;
    this.x = x;
    this.y = y;
    this.frameIndex = startingFrameNumber;
  }

  advanceFrame() {
    this.frameIndex++;
    if (this.frameIndex >= this.frameCount)
      this.frameIndex = 0;
  }
}

class Coins {
  get coinSize() {
  	
    return 64;
  }
  constructor(left, top, right, bottom) {
    this.coins = []
    this.frameCount = 165;
    this.spinningCoin = new Part("Spinning Coin/SpinningCoin", this.frameCount, PartStyle.Loop, 0, 0, 5);
    this.lastTimeWeAdvancedTheFrame = new Date();
    var x = left;
    var y = top;
    const coinMargin = 12;
    var coinCount = 0;
    while (coinCount < 10000) {
      coinCount++;
      this.coins.push(new Coin(Random.getInt(this.frameCount), x, y));
      x += this.coinSize + coinMargin;
      if (x > right - this.coinSize) {
        x = left;
        y += this.coinSize + coinMargin;
        if (y > bottom - this.coinSize) {
          break;
        }
      }
    }
  }

  advanceFrame(now) {
    var msPassed = now - this.lastTimeWeAdvancedTheFrame;
    if (msPassed < 5)
      return;
    this.lastTimeWeAdvancedTheFrame = now;
    this.coins.forEach(function (coin) {
      coin.advanceFrame();
    });
  }

  collect(left, top, width, height) {
    const margin = 10;
    left -= margin;
    top -= margin;
    var right = left + width + margin;
    var bottom = top + height + margin;
    var self = this;
    for (var i = this.coins.length - 1; i >= 0; i--) {
      var coin = this.coins[i];
      var centerX = coin.x + this.coinSize / 2;
      var centerY = coin.y + this.coinSize / 2;
      if (centerX > left && centerX < right && centerY > top & centerY < bottom) {
        this.coins.splice(i, 1);
        new Audio('Assets/Sound Effects/CollectCoin.wav').play();
      }
      
    }
  }

  draw(context, now) {
    this.advanceFrame(now);
    var self = this;
    this.coins.forEach(function (coin) {
      self.spinningCoin.drawByIndex(context, coin.x, coin.y, coin.frameIndex);
    });
  }
}









