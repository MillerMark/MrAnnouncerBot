class FpsWindow {
	static readonly lineColor: string = '#92a5d6';
	static readonly titleColor: string = '#3467eb';
	static readonly fpsActualColor: string = '#eb6734';
	static readonly titleSize: number = 24;
	static readonly titleMargin: number = 3;
	static readonly sectionWidth: number = 220;
	static readonly frameHeight: number = 200;
	static readonly bottomScreenMargin: number = 280;

	left: number;
	right: number;
	top: number;
	bottom: number;
	titleUnderlineY: number;
	graphTopY: number;
	graphBottomY: number;
	graphHeight: number;

	constructor(public title: string, public column: number) {
		this.left = screenWidth - FpsWindow.sectionWidth * (3 - this.column);
		this.right = this.left + FpsWindow.sectionWidth;
		this.bottom = screenHeight - FpsWindow.bottomScreenMargin;
		this.top = this.bottom - FpsWindow.frameHeight;
		this.titleUnderlineY = this.top + FpsWindow.titleSize + 2 * FpsWindow.titleMargin;
		let titleHalfHeightPlusMargin: number = FpsWindow.titleSize / 2 + FpsWindow.titleMargin;
		this.graphTopY = this.titleUnderlineY + FpsWindow.titleMargin;
		this.graphBottomY = this.bottom - titleHalfHeightPlusMargin;
		this.graphHeight = this.graphBottomY - this.graphTopY;
	}

	showAllFramerates(timeBetweenFramesQueue: number[], drawTimeForEachFrameQueue: number[], context: CanvasRenderingContext2D, now: number): any {
		this.drawBorderlines(context);
		this.drawTitles(context);
		this.graphFramerates(timeBetweenFramesQueue, drawTimeForEachFrameQueue, context);
	}

	graphFramerates(timeBetweenFramesQueue: number[], drawTimeForEachFrameQueue: number[], context: CanvasRenderingContext2D): void {
		const maxFrameRate: number = 60;

		let averageTimeBetweenFrames: number = this.getAverage(timeBetweenFramesQueue, 3000);
		let actualAverageFps: number = Math.round(1000 / averageTimeBetweenFrames);

		let clampedFps: number = MathEx.clamp(actualAverageFps, 0, maxFrameRate);
		let yPos: number = this.bottom - clampedFps * this.graphHeight / maxFrameRate;

		//let mostRecentFps: number = game.timeBetweenFramesQueue[game.timeBetweenFramesQueue.length - 1];
		//let mostRecentDrawsPerSec: number = Math.round(1000 / game.drawTimeForEachFrameQueue[game.drawTimeForEachFrameQueue.length - 1]);

		// Average draw time calculation....
		//context.fillStyle = FpsWindow.titleColor;
		//let averageDrawTimeForEachFrame: number = this.getAverage(game.drawTimeForEachFrameQueue);
		//let calculatedFpsBasedOnDrawTime: number = Math.round(1000 / averageDrawTimeForEachFrame);
		//context.fillText(calculatedFpsBasedOnDrawTime.toString(), x, this.top + FpsWindow.titleSize + 50);

		context.fillStyle = FpsWindow.fpsActualColor;
		context.textAlign = 'right';
		let digitWidth: number = context.measureText('0').width;
		context.fillText(actualAverageFps.toString(), this.right - digitWidth / 2, yPos);
		let rightEdge: number = this.right - 3 * digitWidth;
		let rightTime: number = 0;
		let graphWidth: number = rightEdge - this.left;
		const graphTimeSpan: number = 5000;  // ms
		let timeToPixelFactor: number = graphWidth / graphTimeSpan;
		for (let i = timeBetweenFramesQueue.length - 1; i >= 0; i--) {
			let timeThisFrame: number = timeBetweenFramesQueue[i];
			if (timeThisFrame == 0)
				continue;
			let y: number = this.bottom - MathEx.clamp(1000 / timeThisFrame, 0, maxFrameRate) * this.graphHeight / maxFrameRate;
			let leftTime: number = rightTime - timeThisFrame;
			let lineLeft: number = rightEdge + leftTime * timeToPixelFactor;
			if (lineLeft < this.left)
				break;
			let lineRight: number = rightEdge + rightTime * timeToPixelFactor;
			if (i === timeBetweenFramesQueue.length - 1)
				context.moveTo(lineRight, y);
			context.lineTo(lineLeft, y);
			rightTime = leftTime;
		}

		context.strokeStyle = FpsWindow.lineColor;
		context.stroke();
	}

	getAverage(queue: number[], timeToAverageMs: number = Number.MAX_VALUE, countLimit: number = -1): number {
		if (!queue || !queue.length)
			return 0;
		let total: number = 0;
		let upperLimit: number = queue.length;
		let lowerLimit: number = 0;
		if (countLimit !== -1) {
			lowerLimit = upperLimit - countLimit;
		}

		if (queue.length < upperLimit)
			upperLimit = queue.length;
		let numFramesProcessed: number = 0;
		for (let i = upperLimit - 1; i >= lowerLimit; i--) {
			numFramesProcessed++;
			total += queue[i];
			if (timeToAverageMs !== Number.MAX_VALUE)
				if (total > timeToAverageMs)
					break;
		}
		if (numFramesProcessed === 0)
			return 0;
		return total / numFramesProcessed;
	}

	private drawTitles(context: CanvasRenderingContext2D) {
		const titleFont: string = 'px Calibri';
		let titleCenterY: number = this.top + FpsWindow.titleSize / 2 + FpsWindow.titleMargin;
		context.font = FpsWindow.titleSize + titleFont;
		context.textAlign = 'center';
		context.textBaseline = 'middle';
		context.fillStyle = FpsWindow.titleColor;
		let x: number = this.left + FpsWindow.sectionWidth / 2.0;
		context.fillText(this.title, x, titleCenterY);
	}

	private drawBorderlines(context: CanvasRenderingContext2D) {
		context.fillStyle = '#ffffff';
		context.lineWidth = 1;
		let right: number = this.left + FpsWindow.sectionWidth;
		context.fillRect(this.left, this.top, FpsWindow.sectionWidth, FpsWindow.frameHeight);

		context.strokeStyle = FpsWindow.lineColor;
		context.beginPath();

		// Draw title underline...
		context.moveTo(this.left, this.titleUnderlineY);
		context.lineTo(right, this.titleUnderlineY);
		context.stroke();

		// Draw window outline...
		context.strokeRect(this.left, this.top, FpsWindow.sectionWidth, FpsWindow.frameHeight);
	}
}