class EmitterDistribution {
	orientation: DistributionOrientation;
	constructor(public centerX: number, public centerY: number, public width: number, public height: number, public spread: number) {
		this.orientation = DistributionOrientation.Vertical;
	}
}


class HighlightEmitter {
	width: number;
	height: number;
	radius: number;
	type: HighlightEmitterType;
	emitter: Emitter;

	constructor(public name: string, public center: Vector) {
		this.type = HighlightEmitterType.circular;
		this.radius = 3;
	}

	preUpdate(now: number, timeScale: number, world: World): any {
		if (this.emitter) {
			this.emitter.preUpdate(now, timeScale, world);
		}
	}

	update(now: number, timeScale: number, world: World): any {
		if (this.emitter) {
			this.emitter.update(now, timeScale, world);
		}
	}

	render(now: number, timeScale: number, world: World): void {
		if (this.emitter) {
			this.emitter.render(now, timeScale, world);
		}
	}

	setRectangular(width: number, height: number): HighlightEmitter {
		this.type = HighlightEmitterType.rectangular;
		this.width = width;
		this.height = height;
		return this;
	}

	setCircular(radius: number): HighlightEmitter {
		this.type = HighlightEmitterType.circular;
		this.radius = radius;
		return this;
	}

	start(): void {
		if (!this.emitter)
			this.emitter = HighlightEmitter.createBaseEmitter(this.center);

		if (this.type === HighlightEmitterType.circular) {
			this.emitter.radius = this.radius;
		}
		else {
			this.emitter.setRectShape(this.width, this.height);
		}

		const standardLength: number = 290;
		const idealParticlesPerSecond: number = 200;
		this.emitter.particlesPerSecond = idealParticlesPerSecond * this.getPerimeter() / standardLength;

		this.emitter.start();
	}

	stop() {
		if (this.emitter) {
			this.emitter.stop();
		}
	}

	getPerimeter(): number {
		if (this.type === HighlightEmitterType.circular) {
			return this.radius * MathEx.TWO_PI;
		}
		else {
			return 2 * this.width + 2 * this.height;
		}
	}

	static createBaseEmitter(center: Vector): Emitter {
		var emitter: Emitter;
		emitter = new Emitter(center);
		emitter.saturation.target = 0.9;
		emitter.saturation.relativeVariance = 0.2;
		emitter.hue = new TargetValue(40, 0, 0, 60);
		emitter.hue.absoluteVariance = 10;
		emitter.brightness.target = 0.7;
		emitter.brightness.relativeVariance = 0.5;
		emitter.particlesPerSecond = 300;
		emitter.particleRadius.target = 1;
		emitter.particleRadius.relativeVariance = 0.3;
		emitter.particleLifeSpanSeconds = 1.7;
		emitter.particleGravity = 0;
		emitter.particleInitialVelocity.target = 0.1;
		emitter.particleInitialVelocity.relativeVariance = 0.5;
		emitter.particleMaxOpacity = 0.8;
		emitter.particleAirDensity = 0;
		emitter.emitterEdgeSpread = 0.2;
		return emitter;
	}
}

class HighlightEmitterPages {
	emitters: Array<HighlightEmitter> = new Array<HighlightEmitter>();
	constructor() {

	}

	render(now: number, timeScale: number, world: World) {
		this.emitters.forEach(function (emitter: HighlightEmitter) {
			emitter.render(now, timeScale, world);
		});
	}

	find(itemID: string): HighlightEmitter {
		return this.emitters.find(s => s.name === itemID);
	}
}

