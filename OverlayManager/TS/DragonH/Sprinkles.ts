enum Layer {
	Back,
	Front
}

class Sprinkles {
	layer: Layer;
	x: number;
	y: number;
	constructor() {
		this.layer = Layer.Back;
	}

	draw(context: CanvasRenderingContext2D, now: number, layer: Layer): void {
		if (layer != this.layer)
			return;

	}
}