enum EmitterShape {
	Circular = 1,
	Rectangular = 2
}

enum TargetType {
	ActivePlayer = 0,
	ActiveEnemy = 1,
	ScrollPosition = 2,
	ScreenPosition = 3
}

enum VantageKind {
	Normal,
	Advantage,
	Disadvantage
}

enum EffectKind {
	Animation = 0,
	Emitter = 1,
	SoundEffect = 2,
	GroupEffect = 3,
	Placeholder = 4
}

class DragonFrontGame extends GamePlusQuiz {
	readonly clockMargin: number = 14;
	emitter: Emitter;
	shouldDrawCenterCrossHairs: boolean = false;
	denseSmoke: Sprites;
	fireBallBack: Sprites;
	fireBallFront: Sprites;
	poof: Sprites;
	clock: Sprites;
	clockPanel: Sprites;
	bloodGushA: Sprites;
	bloodGushB: Sprites;
	bloodGushC: Sprites;

	charmed: Sprites;
	restrained: Sprites;
	sparkShower: Sprites;
	embersLarge: Sprites;
	embersMedium: Sprites;
	nameplateParts: Sprites;
	nameplateMain: Sprites;
	fireWall: Sprites;
	stars: Sprites;
	fumes: Sprites;
	allEffects: SpriteCollection;
	dndClock: SpriteProxy;
	dndClockPanel: SpriteProxy;
	dndTimeStr: string;
	dragonFrontSounds: DragonFrontSounds;
	players: Array<Character> = [];

	constructor(context: CanvasRenderingContext2D) {
		super(context);
		this.dragonFrontSounds = new DragonFrontSounds('GameDev/Assets/DragonH/SoundEffects');
	}

	update(timestamp: number) {
		this.updateGravity();
		super.update(timestamp);
	}

	updateScreen(context: CanvasRenderingContext2D, now: number) {
		this.allEffects.draw(context, now);

		super.updateScreen(context, now);

		if (this.shouldDrawCenterCrossHairs)
			drawCrossHairs(myContext, screenCenterX, screenCenterY);

		this.drawTime(context);
		this.showNameplates(context, now);
	}

	private drawTime(context: CanvasRenderingContext2D) {
		if (!this.dndTimeStr)
			return;

		const horizontalMargin: number = 10;
		const verticalMargin: number = 15;
		const textHeight: number = 28;
		context.font = textHeight + "px Baskerville Old Face";
		let boxWidth: number = context.measureText(this.dndTimeStr).width + 2 * horizontalMargin;
		let boxHeight: number = textHeight + 2 * verticalMargin;
		let centerX: number = screenWidth - this.clockPanel.originX - this.clockMargin;
		let centerY: number = screenHeight - textHeight / 2 - verticalMargin;
		//context.fillStyle = "#3b3581";
		//context.fillRect(centerX - boxWidth / 2, centerY - boxHeight / 2, boxWidth, boxHeight);
		if (this.inCombat)
			context.fillStyle = "#500506";
		else
			context.fillStyle = "#0b0650";
		context.textAlign = "center";
		context.textBaseline = "middle";
		context.fillText(this.dndTimeStr, centerX, centerY);
	}

	removeAllGameElements(now: number): void {
		super.removeAllGameElements(now);
	}

	initialize() {
		super.initialize();
		gravityGames = new GravityGames();
		Folders.assets = 'GameDev/Assets/DroneGame/';  // So GravityGames can load planet Earth?
	}

	start() {
		super.start();
		gravityGames.selectPlanet('Earth');
		gravityGames.newGame();

		this.updateGravity();
		if (this.emitter)
			this.world.addCharacter(this.emitter);
	}

	loadResources(): void {
		super.loadResources();
		Folders.assets = 'GameDev/Assets/DragonH/';
		const fps30: number = 33;
		const fps20: number = 50;
		const fps15: number = 67;
		this.denseSmoke = new Sprites('Smoke/Dense/DenseSmoke', 116, fps30, AnimationStyle.Sequential, true);
		this.denseSmoke.name = 'DenseSmoke';
		this.denseSmoke.originX = 309;
		this.denseSmoke.originY = 723;

		this.fireBallBack = new Sprites('FireBall/Back/BackFireBall', 88, fps30, AnimationStyle.Sequential, true);
		this.fireBallBack.name = 'FireBallBack';
		this.fireBallBack.originX = 190;
		this.fireBallBack.originY = 1080;

		this.fireBallFront = new Sprites('FireBall/Front/FireBallFront', 88, fps30, AnimationStyle.Sequential, true);
		this.fireBallFront.name = 'FireBallFront';
		this.fireBallFront.originX = 190;
		this.fireBallFront.originY = 1080;

		this.poof = new Sprites('Smoke/Poof/Poof', 67, fps30, AnimationStyle.Sequential, true);
		this.poof.name = 'Puff';
		this.poof.originX = 229;
		this.poof.originY = 698;

		this.clockPanel = new Sprites('Clock/TimeDisplayPanel', 2, fps30, AnimationStyle.Static);
		this.clockPanel.name = 'ClockPanel';
		this.clockPanel.originX = 278;
		this.clockPanel.originY = 37;

		let clockX: number = this.getClockX();
		let clockY: number = screenHeight - 30;

		this.dndClockPanel = this.clockPanel.add(clockX, clockY);

		this.clock = new Sprites('Clock/SunMoonDial', 2, fps30, AnimationStyle.Static);
		this.clock.name = 'Clock';
		this.clock.originX = 247;
		this.clock.originY = 251;

		this.fireWall = new Sprites('FireWall/FireWall', 121, fps20, AnimationStyle.Loop, true);
		this.fireWall.name = 'FireWall';
		this.fireWall.originX = 300;
		this.fireWall.originY = 300;

		this.dndClock = this.clock.add(clockX, clockY);

		this.sparkShower = new Sprites('Sparks/Big/BigSparks', 64, fps30, AnimationStyle.Sequential, true);
		this.sparkShower.name = 'SparkShower';
		this.sparkShower.originX = 443;
		this.sparkShower.originY = 595;

		this.embersLarge = new Sprites('Embers/Large/EmberLarge', 93, fps30, AnimationStyle.Sequential, true);
		this.embersLarge.name = 'EmbersLarge';
		this.embersLarge.originX = 431;
		this.embersLarge.originY = 570;
		//this.embersLarge.originX = 504;
		//this.embersLarge.originY = 501;

		this.embersMedium = new Sprites('Embers/Medium/EmberMedium', 91, fps30, AnimationStyle.Sequential, true);
		this.embersMedium.name = 'EmbersMedium';
		//this.embersMedium.originX = 431;
		//this.embersMedium.originY = 570;
		this.embersMedium.originX = 504;
		this.embersMedium.originY = 501;

		this.nameplateParts = new Sprites('Nameplates/Piece', 3, fps30, AnimationStyle.Static, true);
		this.nameplateParts.originX = 19;
		this.nameplateParts.originY = 8;

		this.nameplateMain = new Sprites('Nameplates/Backplate', 1, fps30, AnimationStyle.Static, true);
		this.nameplateMain.originX = 179;
		this.nameplateMain.originY = 4;

		this.nameplateParts.addShifted(0, DragonFrontGame.nameCenterY, 0, 0);
		this.nameplateParts.addShifted(0, DragonFrontGame.nameCenterY, 1, 0);
		this.nameplateParts.addShifted(0, DragonFrontGame.nameCenterY, 2, 0);

		this.stars = new Sprites('SpinningStars/SpinningStars', 120, fps20, AnimationStyle.Loop, true);
		this.stars.name = 'Stars';
		this.stars.originX = 155;
		this.stars.originY = 495;

		this.fumes = new Sprites('Fumes/Fumes', 112, fps30, AnimationStyle.Loop, true);
		this.fumes.name = 'Fumes';
		this.fumes.originX = 281;
		this.fumes.originY = 137;

		this.bloodGushA = new Sprites('Blood/Gush/A/GushA', 69, fps30, AnimationStyle.Sequential, true);
		this.bloodGushA.name = 'BloodGush';
		this.bloodGushA.originX = 20;
		this.bloodGushA.originY = 1080;


		this.bloodGushB = new Sprites('Blood/Gush/B/GushB', 78, fps30, AnimationStyle.Sequential, true);
		this.bloodGushB.name = 'BloodGush';
		this.bloodGushB.originX = 57;
		this.bloodGushB.originY = 1080;

		this.bloodGushC = new Sprites('Blood/Gush/C/GushC', 89, fps30, AnimationStyle.Sequential, true);
		this.bloodGushC.name = 'BloodGush';
		this.bloodGushC.originX = 114;
		this.bloodGushC.originY = 1080;

		this.charmed = new Sprites('Charmed/Charmed', 179, fps30, AnimationStyle.Loop, true);
		this.charmed.name = 'Heart';
		this.charmed.originX = 155;
		this.charmed.originY = 269;

		this.restrained = new Sprites('Restrained/Chains', 20, fps30, AnimationStyle.Loop, true);
		this.restrained.name = 'Restrained';
		this.restrained.originX = 213;
		this.restrained.originY = 299;

		this.allEffects = new SpriteCollection();
		this.allEffects.add(this.denseSmoke);
		this.allEffects.add(this.poof);
		this.allEffects.add(this.clock);
		this.allEffects.add(this.fireBallBack);
		this.allEffects.add(this.fireBallFront);
		this.allEffects.add(this.stars);
		this.allEffects.add(this.fumes);
		this.allEffects.add(this.sparkShower);
		this.allEffects.add(this.embersLarge);
		this.allEffects.add(this.embersMedium);
		this.allEffects.add(this.bloodGushA);
		this.allEffects.add(this.bloodGushB);
		this.allEffects.add(this.bloodGushC);
		this.allEffects.add(this.charmed);
		this.allEffects.add(this.restrained);
		this.allEffects.add(this.fireWall);
		this.allEffects.add(this.clockPanel);
	}

	private getClockX(): number {
		return screenWidth - this.clockPanel.originX - this.clockMargin;
	}

	executeCommand(command: string, params: string, userInfo: UserInfo, now: number): boolean {
		if (super.executeCommand(command, params, userInfo, now))
			return true;
		if (command === "Cross2") {
			this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
		}
	}

	testBloodEmitter(): void {
		this.emitter = new Emitter(new Vector(450, 1080));
		this.emitter.radius = 1;
		this.emitter.saturation.target = 0.9;
		this.emitter.saturation.relativeVariance = 0.2;
		this.emitter.hue.absoluteVariance = 10;
		this.emitter.hue.target = 0;
		this.emitter.brightness.target = 0.4;
		this.emitter.brightness.relativeVariance = 0.3;
		this.emitter.particlesPerSecond = 1400;

		this.emitter.particleRadius.target = 2.5;
		this.emitter.particleRadius.relativeVariance = 0.8;

		this.emitter.particleLifeSpanSeconds = 2;
		this.emitter.maxTotalParticles = 2000;
		this.emitter.particleGravity = 9.8;
		this.emitter.particleInitialVelocity.target = 0.8;
		this.emitter.particleInitialVelocity.relativeVariance = 0.25;
		this.emitter.gravity = 0;
		this.emitter.airDensity = 0; // 0 == vaccuum.
		this.emitter.particleAirDensity = 0.1;  // 0 == vaccuum.

		let sprayAngle: number = Random.intBetween(270 - 45, 270 + 45);
		let minVelocity: number = 9;
		let maxVelocity: number = 16;
		let angleAwayFromUp: number = Math.abs(sprayAngle - 270);
		if (angleAwayFromUp < 15)
			maxVelocity = 10;
		else if (angleAwayFromUp > 35)
			minVelocity = 13;
		else
			maxVelocity = 18;
		this.emitter.bonusParticleVelocityVector = Vector.fromPolar(sprayAngle, Random.intBetween(minVelocity, maxVelocity));

		this.emitter.renderOldestParticlesLast = true;
	}

	test(testCommand: string, userInfo: UserInfo, now: number): boolean {
		if (super.test(testCommand, userInfo, now))
			return true;

		if (testCommand === "Cross2") {
			console.log('draw Cross Hairs');
			this.shouldDrawCenterCrossHairs = !this.shouldDrawCenterCrossHairs;
		}

		if (testCommand === 'clear') {
			this.world.removeCharacter(this.emitter);
			this.testBloodEmitter();
			this.world.addCharacter(this.emitter);
			return true;
		}

		if (testCommand === 'blood') {
			console.log('Blood');
			this.world.removeCharacter(this.emitter);
			this.testBloodEmitter();
			this.world.addCharacter(this.emitter);
			return true;
		}

		// poof 40 50
		if (testCommand.startsWith("poof")) {
			let split: string[] = testCommand.split(' ');

			let hue: number = 0;
			let saturation: number = 100;
			let brightness: number = 100;

			if (split.length === 2) {
				hue = +split[1];
			}
			else if (split.length === 3) {
				hue = +split[1];
				saturation = +split[2];
			}
			else if (split.length > 3) {
				hue = +split[1];
				saturation = +split[2];
				brightness = +split[3];
			}

			this.denseSmoke.sprites.push(new ColorShiftingSpriteProxy(0, new Vector(450 - this.denseSmoke.originX, 1080 - this.denseSmoke.originY)).setHueSatBrightness(hue, saturation, brightness));
		}


		if (testCommand.startsWith("fb")) {
			let split: string[] = testCommand.split(' ');

			let hue: number = 0;
			let saturation: number = 100;
			let brightness: number = 100;

			if (split.length === 2) {
				hue = +split[1];
			}
			else if (split.length === 3) {
				hue = +split[1];
				saturation = +split[2];
			}
			else if (split.length > 3) {
				hue = +split[1];
				saturation = +split[2];
				brightness = +split[3];
			}

			this.fireBallBack.sprites.push(new SpriteProxy(0, 450 - this.fireBallBack.originX, 1080 - this.fireBallBack.originY));
			this.fireBallFront.sprites.push(new ColorShiftingSpriteProxy(0, new Vector(450 - this.fireBallFront.originX, 1080 - this.fireBallFront.originY)).setHueSatBrightness(hue, saturation, brightness));
		}

		return false;
	}

	playerVideoLeftMargin = 10;
	playerVideoRightMargin = 1384;

	numberOfPlayers: number = 4;
	player1X: number = -1;
	player2X: number = -1; // + 317
	player3X: number = -1; // + 316
	player4X: number = -1;
	activePlayerX: number = this.player1X;

	getPlayerX(playerIndex: number): number {
		let distanceForPlayerVideos: number = this.playerVideoRightMargin - this.playerVideoLeftMargin;
		let distanceBetweenPlayers: number = distanceForPlayerVideos / this.numberOfPlayers;
		let halfDistanceBetweenPlayers: number = distanceBetweenPlayers / 2;
		return playerIndex * distanceBetweenPlayers + halfDistanceBetweenPlayers;
	}

	getPlayerIndex(playerId: number): number {
		for (var i = 0; i < this.players.length; i++) {
			let player: Character = this.players[i];
			if (player.playerID == playerId) {
				return i;
			}
		}
		return -1;
	}

	playerChanged(playerID: number, playerData: string): void {
		this.activePlayerX = this.getPlayerX(playerID);
	}

	getCenter(target: any): Vector {
		let result: Vector;
		if (target.targetType === TargetType.ScreenPosition)
			result = new Vector(target.screenPosition.x, target.screenPosition.y);
		else if (target.targetType === TargetType.ActivePlayer)
			result = new Vector(this.activePlayerX, 1080);
		else if (target.targetType === TargetType.ActiveEnemy)
			result = new Vector(1260, 1080);
		else if (target.targetType === TargetType.ScrollPosition)
			result = new Vector(150, 400);
		else
			result = new Vector(960, 540);

		return result.add(new Vector(target.targetOffset.x, target.targetOffset.y));
	}

	triggerSingleEffect(dto: any) {
		if (dto.timeOffsetMs > 0) {
			let offset: number = dto.timeOffsetMs;
			dto.timeOffsetMs = -1;
			setTimeout(this.triggerSingleEffect.bind(this), offset, dto);
			return;
		}

		if (dto.effectKind === EffectKind.SoundEffect) {
			this.triggerSoundEffect(dto);
			return;
		}

		if (dto.effectKind === EffectKind.Placeholder) {
			this.triggerPlaceholder(dto);
			return;
		}

		let center: Vector = this.getCenter(dto.target);

		if (dto.effectKind === EffectKind.Animation)
			this.triggerAnimation(dto, center);
		else if (dto.effectKind === EffectKind.Emitter)
			this.triggerEmitter(dto, center);
	}

	triggerPlaceholder(dto: any): any {
		console.log('triggerPlaceholder - dto: ' + dto);
	}

	triggerEffect(effectData: string): void {
		let dto: any = JSON.parse(effectData);
		console.log(dto);

		if (dto.effectKind === EffectKind.GroupEffect) {
			for (var i = 0; i < dto.effectsCount; i++) {
				this.triggerSingleEffect(dto.effects[i]);
			}
		}
		else {
			this.triggerSingleEffect(dto);
		}
	}

	private _inCombat: boolean;

	get inCombat(): boolean {
		return this._inCombat;
	}

	set inCombat(newValue: boolean) {
		if (this._inCombat == newValue)
			return;


		this._inCombat = newValue;
		if (this._inCombat) {
			this.dndClock.frameIndex = 1;
			this.dndClockPanel.frameIndex = 1;
			//this.createFireWallBehindClock();
			this.createFireBallBehindClock(330);
		}
		else {
			this.dndClock.frameIndex = 0;
			this.dndClockPanel.frameIndex = 0;
			this.fireWall.sprites = [];
			this.createFireBallBehindClock(200);
		}
	}

	private createFireBallBehindClock(hue: number): any {
		let x: number;
		let y: number;
		x = this.getClockX() - 90;
		y = screenHeight - this.clockPanel.originY;
		let pos: Vector = new Vector(x - this.fireBallBack.originX, y - this.fireBallBack.originY);
		this.fireBallBack.sprites.push(new ColorShiftingSpriteProxy(0, pos).setHueSatBrightness(hue));
		this.fireBallFront.sprites.push(new ColorShiftingSpriteProxy(0, pos).setHueSatBrightness(hue));
		this.dragonFrontSounds.playHeavyPoof();
	}

	private createFireWallBehindClock() {
		const displayMargin: number = 16;
		let fireWall: SpriteProxy = this.fireWall.add(this.getClockX(), screenHeight - this.clockPanel.originY * 2 + displayMargin);
		fireWall.expirationDate = performance.now() + 11000;
		fireWall.fadeOutTime = 2000;
		this.dragonFrontSounds.playFlameOn();
	}

	getDegreesToRotate(targetRotation: number, sprite: SpriteProxy): number {
		let degreesToMove: number = targetRotation - sprite.rotation;
		if (degreesToMove < 0) {
			degreesToMove += 360;
		}

		return degreesToMove;
	}

	updateClock(clockData: string): void {
		let dto: any = JSON.parse(clockData);
		this.inCombat = dto.InCombat;
		this.dndTimeStr = dto.Time;
		let fullSpins: number = dto.FullSpins;
		let afterSpinMp3: string = dto.AfterSpinMp3;

		let degreesToMove: number = this.getDegreesToRotate(dto.Rotation, this.dndClock);
		let timeToRotate: number;
		if (degreesToMove < 1) {
			if (fullSpins >= 1) {
				degreesToMove = 360;
				timeToRotate = 2600;
				this.dragonFrontSounds.playGear2_6();
			}
			else
				timeToRotate = 250;
		}
		else if (degreesToMove < 10) {
			timeToRotate = 150;
			this.dragonFrontSounds.playGear0_2();
		}
		else if (degreesToMove < 20) {
			timeToRotate = 250;
			this.dragonFrontSounds.playGear0_25();
		}
		else if (degreesToMove < 45) {
			timeToRotate = 500;
			this.dragonFrontSounds.playGear0_5();
		}
		else if (degreesToMove < 90) {
			timeToRotate = 1000;
			this.dragonFrontSounds.playGear1_0();
		}
		else if (degreesToMove < 180) {
			timeToRotate = 1800;
			this.dragonFrontSounds.playGear1_8();
		}
		else {
			timeToRotate = 2600;
			this.dragonFrontSounds.playGear2_6();
		}

		if (afterSpinMp3) {
			this.dragonFrontSounds.playMp3In(timeToRotate + 500, `TimeAmbiance/${afterSpinMp3}`);
		}

		this.dndClock.rotateTo(dto.Rotation, degreesToMove, timeToRotate);
	}

	triggerSoundEffect(dto: any): void {
		console.log("Playing " + Folders.assets + 'SoundEffects/' + dto.soundFileName);
		new Audio(Folders.assets + 'SoundEffects/' + dto.soundFileName).play();
	}

	triggerEmitter(dto: any, center: Vector): void {
		this.world.removeCharacter(this.emitter);

		console.log('emitter: ' + dto);

		this.emitter = new Emitter(new Vector(center.x, center.y), new Vector(dto.emitterInitialVelocity.x, dto.emitterInitialVelocity.y));
		if (dto.emitterShape === EmitterShape.Circular) {
			this.emitter.radius = dto.radius;
		}
		else {
			this.emitter.setRectShape(dto.width, dto.height);
		}

		this.emitter.particleMass = dto.particleMass;
		this.emitter.initialParticleDirection = new Vector(dto.particleInitialDirection.x, dto.particleInitialDirection.y);
		this.emitter.particleWind = new Vector(dto.particleWindDirection.x, dto.particleWindDirection.y);
		this.emitter.minParticleSize = dto.minParticleSize;
		this.emitter.gravityCenter = new Vector(dto.gravityCenter.x, dto.gravityCenter.y);
		this.emitter.particleFadeInTime = dto.fadeInTime;
		this.emitter.wind = new Vector(dto.emitterWindDirection.x, dto.emitterWindDirection.y);

		this.transferTargetValue(this.emitter.brightness, dto.brightness);
		this.transferTargetValue(this.emitter.hue, dto.hue);
		this.transferTargetValue(this.emitter.saturation, dto.saturation);

		this.emitter.maxConcurrentParticles = dto.maxConcurrentParticles;
		this.emitter.particleMaxOpacity = dto.maxOpacity;

		this.emitter.particlesPerSecond = dto.particlesPerSecond;

		this.transferTargetValue(this.emitter.particleRadius, dto.particleRadius);

		this.emitter.emitterEdgeSpread = dto.edgeSpread;

		this.emitter.particleLifeSpanSeconds = dto.lifeSpan;
		if (dto.maxTotalParticles === 'Infinity') {
			this.emitter.maxTotalParticles = Infinity;
		}
		else {
			this.emitter.maxTotalParticles = dto.maxTotalParticles;
		}

		this.emitter.particleGravity = dto.particleGravity;
		this.emitter.particleGravityCenter = new Vector(dto.particleGravityCenter.x, dto.particleGravityCenter.y);
		this.transferTargetValue(this.emitter.particleInitialVelocity, dto.particleInitialVelocity);

		this.emitter.gravity = dto.emitterGravity;
		this.emitter.airDensity = dto.emitterAirDensity;
		this.emitter.particleAirDensity = dto.particleAirDensity;

		this.emitter.bonusParticleVelocityVector = new Vector(dto.bonusVelocityVector.x, dto.bonusVelocityVector.y);

		this.emitter.renderOldestParticlesLast = dto.renderOldestParticlesLast;

		this.world.addCharacter(this.emitter);
		console.log(this.emitter);
	}


	private transferTargetValue(brightness: TargetValue, anyTargetValue: any): void {
		brightness.target = anyTargetValue.value;
		brightness.relativeVariance = anyTargetValue.relativeVariance;
		brightness.absoluteVariance = anyTargetValue.absoluteVariance;
		brightness.drift = anyTargetValue.drift;
		brightness.binding = anyTargetValue.targetBinding;
	}

	private triggerAnimation(dto: any, center: Vector) {
		let sprites: Sprites;
		let horizontallyFlippable: boolean = false;
		//console.log('dto.spriteName: ' + dto.spriteName);
		if (dto.spriteName === 'DenseSmoke')
			sprites = this.denseSmoke;
		else if (dto.spriteName === 'Poof')
			sprites = this.poof;

		// TODO: Update blood animation to pass in a severity number instead of a string in WPF app.
		else if (dto.spriteName === 'BloodGush') {
			sprites = this.bloodGushA;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodLarger') {
			sprites = this.bloodGushB;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodLarge') {
			sprites = this.bloodGushC;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodMedium') {
			sprites = this.bloodGushA;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodSmall') {
			sprites = this.bloodGushB;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodSmaller') {
			sprites = this.bloodGushC;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'BloodSmallest') {
			sprites = this.bloodGushA;
			horizontallyFlippable = true;
		}
		else if (dto.spriteName === 'Heart')
			sprites = this.charmed;
		else if (dto.spriteName === 'Restrained')
			sprites = this.restrained;
		else if (dto.spriteName === 'SparkShower')
			sprites = this.sparkShower;
		else if (dto.spriteName === 'EmbersLarge')
			sprites = this.embersLarge;
		else if (dto.spriteName === 'EmbersMedium')
			sprites = this.embersMedium;
		else if (dto.spriteName === 'Stars')
			sprites = this.stars;
		else if (dto.spriteName === 'Fumes')
			sprites = this.fumes;
		else if (dto.spriteName === 'FireBall')
			sprites = this.fireBallBack;

		let flipHorizontally: boolean = false;

		if (horizontallyFlippable && Random.chancePercent(50))
			flipHorizontally = true;

		let spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(center), dto.startFrameIndex, dto.hueShift, dto.saturation, dto.brightness, flipHorizontally);
		spritesEffect.start();

		if (dto.spriteName === 'FireBall') {
			sprites = this.fireBallFront;
			let spritesEffect: SpritesEffect = new SpritesEffect(sprites, new ScreenPosTarget(center), dto.startFrameIndex, dto.secondaryHueShift, dto.secondarySaturation, dto.secondaryBrightness);
			spritesEffect.start();
		}
	}

	static readonly nameCenterY: number = 1056;
	static readonly nameplateHalfHeight: number = 24;

	initializePlayerData(playerData: string): any {
		this.players = JSON.parse(playerData);
		for (var i = 0; i < this.players.length; i++) {
			let centerX: number = this.getPlayerX(i);
			this.nameplateMain.addShifted(centerX, DragonFrontGame.nameCenterY - DragonFrontGame.nameplateHalfHeight, 0, 0);
		}

		//this.nameplateMain.add(0, DragonFrontGame.nameCenterY - DragonFrontGame.nameplateHalfHeight, 0);
		//this.nameplateParts.add(0, DragonFrontGame.nameCenterY, 0);
		//this.nameplateParts.add(0, DragonFrontGame.nameCenterY, 1);
		//this.nameplateParts.add(0, DragonFrontGame.nameCenterY, 2);
	}

	changePlayerHealth(playerHealthDto: string): void {
		console.log('playerHealth: ' + playerHealthDto);
		let playerHealth: PlayerHealth = JSON.parse(playerHealthDto);

		for (var i = 0; i < playerHealth.PlayerIds.length; i++) {
			this.showDamageForPlayer(playerHealth.DamageHealth, playerHealth.PlayerIds[i]);
		}
	}

	private showDamageForPlayer(damageHealth: number, playerId: number) {
		let flipHorizontally: boolean = false;
		if (Random.chancePercent(50))
			flipHorizontally = true;
		let damageHealthSprites: Sprites;
		let scale: number = 1;
		if (damageHealth < 0) {
			if (Random.chancePercent(33)) {
				damageHealthSprites = this.bloodGushA;
			}
			else if (Random.chancePercent(50)) {
				damageHealthSprites = this.bloodGushB;
			}
			else 
				damageHealthSprites = this.bloodGushC;

			let absDamage: number = -damageHealth;
			if (absDamage < 10) {
				scale = absDamage / 10;
			}
		}
		let x: number = this.getPlayerX(this.getPlayerIndex(playerId));
		let center: Vector = new Vector(x, 1080);

		let spritesEffect: SpritesEffect = new SpritesEffect(damageHealthSprites, new ScreenPosTarget(center), 0, 0, 100, 100, flipHorizontally);
		spritesEffect.scale = scale;
		spritesEffect.start();
	}

	showNameplate(context: CanvasRenderingContext2D, player: Character, playerID: number, now: number) {
		context.textAlign = 'center';
		context.textBaseline = 'middle';
		context.fillStyle = '#ffffff';
		context.font = '36px Blackadder ITC';

		let sprite: SpriteProxy = this.nameplateMain.sprites[playerID];

		const nameplateMaxWidth: number = 358;

		var hpStr: string;
		if (player.tempHitPoints > 0)
			hpStr = '+' + player.tempHitPoints;
		else
			hpStr = player.hitPoints.toString() + '/' + player.maxHitPoints;

		let centerX: number = this.getPlayerX(playerID);
		//const drawLines: boolean = false;
		//if (drawLines) {
		//	context.beginPath();
		//	context.moveTo(centerX, 1080);
		//	context.lineTo(centerX, 980);
		//	context.strokeStyle = '#ff0000';
		//	context.stroke();
		//}

		let hidingHitPoints: boolean = !this.inCombat && player.hitPoints === player.maxHitPoints;
		let hpWidth: number = context.measureText(hpStr).width;
		let nameWidth: number = context.measureText(player.name).width;
		let nameHpMargin: number = 25;
		let additionalWidth: number = hpWidth + nameHpMargin;
		if (hidingHitPoints) {
			hpWidth = 0;
			additionalWidth = 0;
			nameHpMargin = 0;
		}
		let additionalHalfWidth: number = additionalWidth / 2;
		let nameCenter: number = centerX - additionalHalfWidth;
		let hpCenter: number = nameCenter + nameWidth / 2 + nameHpMargin + hpWidth / 2;
		let totalNameplateTextWidth: number = nameWidth + nameHpMargin + hpWidth;

		const innerNameplateMargin: number = 8;
		let horizontalMargin: number = (nameplateMaxWidth - totalNameplateTextWidth) / 2 - innerNameplateMargin * 2;
		if (horizontalMargin < 0) {
			horizontalMargin = 0;
		}

		let plateWidth: number = this.nameplateMain.spriteWidth - 2 * horizontalMargin;

		let saveFilter: string = (context as any).filter;
		try {
			if (sprite instanceof ColorShiftingSpriteProxy) {
				sprite.hueShift = player.hueShift;
				sprite.shiftColor(context, now);
			}
			this.nameplateMain.baseAnimation.drawCroppedByIndex(context, sprite.x + horizontalMargin, sprite.y, 0, horizontalMargin, 0,
				plateWidth, this.nameplateMain.spriteHeight, plateWidth, this.nameplateMain.spriteHeight);

			let leftX: number = centerX - plateWidth / 2;
			this.nameplateParts.baseAnimation.drawByIndex(context, leftX - this.nameplateParts.originX, sprite.y - this.nameplateParts.originY, 0);
			let rightX: number = centerX + plateWidth / 2;
			this.nameplateParts.baseAnimation.drawByIndex(context, rightX - this.nameplateParts.originX, sprite.y - this.nameplateParts.originY, 1);
			if (!hidingHitPoints) {
				let separatorX: number = nameCenter + nameWidth / 2 + nameHpMargin / 2;
				this.nameplateParts.baseAnimation.drawByIndex(context, separatorX - this.nameplateParts.originX, sprite.y - this.nameplateParts.originY, 2);
			}
		}
		finally {
			(context as any).filter = saveFilter;
		}

		context.fillText(player.name, nameCenter, DragonFrontGame.nameCenterY);
		if (!hidingHitPoints) {
			context.fillText(hpStr, hpCenter, DragonFrontGame.nameCenterY);
		}

		//this.nameplateParts.addShifted(centerX - nameplateHalfWidth, nameplateY, 0, player.hueShift);
		//this.nameplateParts.addShifted(centerX + nameplateHalfWidth, nameplateY, 1, player.hueShift);
		//this.nameplateParts.addShifted(centerX + separatorOffset, nameplateY, 2, player.hueShift);
	}

	showNameplates(context: CanvasRenderingContext2D, now: number) {
		for (var i = 0; i < this.players.length; i++) {
			let player: Character = this.players[i];
			this.showNameplate(context, player, i, now);
		}
	}
}

class PlayerHealth {
	PlayerIds: Array<number> = new Array<number>();
	DamageHealth: number;
	constructor() {
	}
}
