class CoinManager {
	constructor() {

	}
	allCoins: SpriteCollection = new SpriteCollection();

	loadCoin(name: string): Sprites {
		let coins: Sprites = new Sprites(`Coins/${name}`, 60, fps30, AnimationStyle.Loop, true);
		coins.name = name;
		coins.originX = 17;
		coins.originY = 17;
		coins.moves = true;
		return coins;
	}

	loadResources() {
		this.allCoins.add(this.loadCoin('Gold'));
		this.allCoins.add(this.loadCoin('Silver'));
		this.allCoins.add(this.loadCoin('Copper'));
		this.allCoins.add(this.loadCoin('Electrum'));
		this.allCoins.add(this.loadCoin('Platinum'));
	}

	addCoinsByName(numCoins: number, coinName: string, x: number): any {
		let sprites: Sprites = this.allCoins.getSpritesByName(coinName);
		if (!sprites)
			return;
		let now: number = performance.now();
		if (numCoins > 0) {
			for (let i = 0; i < numCoins; i++)
				this.addDroppingCoin(sprites, x, now);
		}
		else {
			numCoins = Math.abs(numCoins);
			for (let i = 0; i < numCoins; i++)
				this.addFlyingCoin(sprites, x, now);
		}
	}

	private addDroppingCoin(sprites: Sprites, x: number, now: number) {
		const y = 0;
		const sprite: SpriteProxy = sprites.add(x, y, -1);
		sprite.verticalThrustOverride = 9.8;
		if (Random.chancePercent(50))
			sprite.animationReverseOverride = true;
		sprite.frameIntervalOverride = fps30 * Random.between(0.5, 2);
		sprite.timeStart = now + Random.between(0, 800);
		sprite.velocityX = Random.between(-1.75, 1.75);
		sprite.velocityY = Random.between(0, -3);
		sprite.rotation = Random.max(360);
		sprite.autoRotationDegeesPerSecond = Random.between(-10, 10);
		sprite.initialRotation = sprite.rotation;
		sprite.expirationDate = sprite.timeStart + 3000;
		sprite.fadeOutTime = 0;
		sprite.fadeInTime = 0;
	}

	static readonly halfHeightDmFrame: number = 212;
	static readonly halfWidthDmFrame: number = 213;
	static readonly gravity: number = 9.8;

	private addFlyingCoin(sprites: Sprites, x: number, now: number) {
		const y: number = screenHeight;
		x += Random.between(-100, 100);
		const sprite: SpriteProxy = sprites.add(x, y, -1);
		sprite.verticalThrustOverride = CoinManager.gravity;
		if (Random.chancePercent(50))
			sprite.animationReverseOverride = true;
		sprite.frameIntervalOverride = fps30 * Random.between(0.5, 2);
		sprite.timeStart = now + Random.between(0, 800);

		const airTime: number = CoinManager.getAirTimeToDmBoxSec();

		sprite.rotation = Random.max(360);
		sprite.autoRotationDegeesPerSecond = Random.between(-10, 10);
		sprite.initialRotation = sprite.rotation;

		sprite.velocityX = Physics.pixelsToMeters(screenWidth - CoinManager.halfWidthDmFrame - x) / airTime;
		sprite.velocityY = -Physics.getFinalVelocity(airTime, 0, CoinManager.gravity);
		sprite.expirationDate = sprite.timeStart + airTime * 1000;
		sprite.fadeOutTime = 300;
		sprite.fadeInTime = 0;
	}

	static getAirTimeToDmBoxSec(): number {
		return CoinManager.getAirTimeSec(screenHeight - CoinManager.halfHeightDmFrame);
	}

	static getAirTimeFullDropSec(): number {
		return CoinManager.getAirTimeSec(screenHeight);
	}

	static getAirTimeSec(totalHeightPx: number) {
		let heightMeters: number = Physics.pixelsToMeters(totalHeightPx);
		return Physics.getDropTime(heightMeters, CoinManager.gravity);
	}

	addCoins(coins: Coins, x: number) {
		if (coins.NumGold != 0)
			this.addCoinsByName(coins.NumGold, 'Gold', x);
		if (coins.NumSilver != 0)
			this.addCoinsByName(coins.NumSilver, 'Silver', x);
		if (coins.NumCopper != 0)
			this.addCoinsByName(coins.NumCopper, 'Copper', x);
		if (coins.NumElectrum != 0)
			this.addCoinsByName(coins.NumElectrum, 'Electrum', x);
		if (coins.NumPlatinum != 0)
			this.addCoinsByName(coins.NumPlatinum, 'Platinum', x);
	}

	draw(context: CanvasRenderingContext2D, now: number) {
		context.globalAlpha = 1;
		this.allCoins.updatePositions(now);
		this.allCoins.draw(context, now);
	}
}
