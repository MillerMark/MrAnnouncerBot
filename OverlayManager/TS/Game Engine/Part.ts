const globalFramesToLoad: number = 1;
const globalFramesToCount: number = 2;

const fps100 = 10;
const fps90 = 1000 / 90;
const fps80 = 1000 / 80;
const fps70 = 1000 / 70;
const fps60= 16.66666667;
const fps50= 20;
const fps40= 25;
const fps30= 33.33333333;
const fps20= 50;
const fps25= 40;
const fps15= 66.66666667;
const fps4 = 250;
const fps2 = 500;

var globalBypassFrameSkip: boolean = false;

class ImageManager {
	private static _images = new Map<string, HTMLImageElement[]>();

	private static _imagesLoaded: Promise<void>;
	static get imagesLoaded() {
		return this._imagesLoaded || (this._imagesLoaded = this.loadImages());
	}

	static get(folder: string, filename: string) {
		const name = (folder + filename).toLowerCase();
		const images = this._images.get(name);

		if (!images)
			console.error(`Image "${name} was not found.`);

		return images || [];
	}

	private static getJson(response: Response) {
		if (response.ok)
			return response.json();

		throw new Error('Images.json not found.');
	}

	private static addImages(json: object) {
		for (const dir in json) {
			const dirInfo = json[dir];

			for (const name in dirInfo) {
				const urls: string[] = dirInfo[name];
				const key = dir.endsWith("/") ? dir + name : dir + "/" + name;

				const images: HTMLImageElement[] = urls.map(url => {
					const image = new Image();
					image.src = url;
					return image;
				});

				ImageManager._images.set(key, images);
			}
		}
	}

	private static handleError(error: Error) {
		console.log(error);
	}

	private static loadImages() {
		return fetch("GameDev/Assets/DiceRoller/images.json")
			.then(this.getJson)
			.then(this.addImages)
			.catch(this.handleError);
	}
}

class PartBackgroundLoader {
	static partsToLoad: Array<Part> = [];

	static remove(part: Part): void {
		for (var i = length - 1; i >= 0; i--) {
			if (PartBackgroundLoader.partsToLoad[i] == part)
				PartBackgroundLoader.partsToLoad.splice(i, 1);
		}
	}
	static add(part: Part): void {
		PartBackgroundLoader.partsToLoad.push(part);
		PartBackgroundLoader.initialize();
	}

	static loadImages() {
		if (PartBackgroundLoader.partsToLoad.length == 0) {
			if (PartBackgroundLoader.intervalHandle !== undefined) {
				clearInterval(PartBackgroundLoader.intervalHandle);
				PartBackgroundLoader.intervalHandle = undefined;
			}
			return;
		}
		var startTime: number = performance.now();
		PartBackgroundLoader.partsToLoad.sort((a, b) => (a.frameCount - b.frameCount));
		let partToLoad: Part = PartBackgroundLoader.partsToLoad.pop();
		console.log(`Background Loading ${partToLoad.fileName} with ${partToLoad.frameCount} images.`);
		partToLoad.images;  // Triggering the load on demand.
		var loadTime: number = performance.now() - startTime;
		PartBackgroundLoader.nextInterval = Math.min(2000, loadTime * 2);
	}

	static intervalHandle?: number = undefined;

	static nextInterval: number = 5000;

	static okayToStartLoading: boolean = false;

	static initialize() {
		if (PartBackgroundLoader.okayToStartLoading && PartBackgroundLoader.intervalHandle === undefined)
			PartBackgroundLoader.intervalHandle = setInterval(PartBackgroundLoader.loadImages, PartBackgroundLoader.nextInterval);
	}
}

// New idea: create offscreen canvases and contexts for EACH image frame!!!

//class CanvasContextPair {
//	originalImage: HTMLImageElement;
//	index: number;
//	sourceOffsetX: number;
//	sourceOffsetY: number;
//	row: number;
//	column: number;
//	static readonly maxCanvasWidth: number = 16000;
//	static readonly maxCanvasHeight: number = 16000;
//	numImagesPerRow: number;
//	numImagesPerColumn: number;

//	constructor(context: CanvasRenderingContext2D, images: HTMLImageElement[], index: number) {
//		this.index = index;
//		this.originalImage = images[index];
//		this.numImagesPerRow = Math.floor(CanvasContextPair.maxCanvasWidth / this.originalImage.width)
//		this.numImagesPerColumn = Math.floor(CanvasContextPair.maxCanvasHeight / this.originalImage.height)
//		this.cacheImage(context);
//	}

//	cacheImage(context: CanvasRenderingContext2D) {
//		if (!context)
//			return;
//		if (!this.originalImage)
//			return;
//		let imageWidth: number = this.originalImage.width;
//		let imageHeight: number = this.originalImage.height;
//		if (imageWidth == 0)
//			return;
//		this.row = Math.floor(this.index / this.numImagesPerRow);
//		this.column = Math.round(this.index - this.row * this.numImagesPerRow);
//		this.sourceOffsetX = this.column * imageWidth;
//		this.sourceOffsetY = this.row * imageHeight;
//		// Draw the image at the cached location....
//		context.drawImage(this.originalImage, this.sourceOffsetX, this.sourceOffsetY);
//		this.originalImage = null;
//	}

//	imageHasBeenCached(context: CanvasRenderingContext2D): boolean {
//		if (!this.originalImage)
//			return true;
//		this.cacheImage(context);
//		return this.originalImage == null;
//	}
//}

//class CachedImages {
//	cachedImages: CanvasContextPair[] = [];
//	canvas: HTMLCanvasElement;
//	context: CanvasRenderingContext2D;
//	lastRead: Date;
//	imageWidth: number;
//	imageHeight: number;

//	constructor(images: HTMLImageElement[], filter) {
//		this.canvas = document.createElement('canvas');
//		if (images[0].width === 0) {
//			throw new DOMException('images[0].width is 0!!!!')
//		}
//		this.imageWidth = images[0].width;
//		this.imageHeight = images[0].height;
//		let canvasContextPair: CanvasContextPair = new CanvasContextPair(null, images, 0)
//		this.cachedImages.push(canvasContextPair);

//		this.canvas.width = this.imageWidth * canvasContextPair.numImagesPerRow;
//		this.canvas.height = this.imageHeight * canvasContextPair.numImagesPerColumn;
//		this.context = this.canvas.getContext('2d');
//		this.context.filter = filter;

//		canvasContextPair.cacheImage(this.context);

//		for (let i = 1; i < images.length; i++) {
//			this.cachedImages.push(new CanvasContextPair(this.context, images, i));
//		}
//	}

//	getSourceOffsetX(frameIndex: number): number {
//		return this.cachedImages[frameIndex].sourceOffsetX;
//	}

//	getSourceOffsetY(frameIndex: number): number {
//		return this.cachedImages[frameIndex].sourceOffsetY;
//	}


//	getCanvas(frameIndex: number): CanvasImageSource {
//		// TODO: garbage-collect this based on age of lastRead!!!!
//		this.lastRead = new Date();
//		if (this.cachedImages[frameIndex].imageHasBeenCached(this.context))
//			return this.canvas;
//		return null;
//	}
//}

class Part {
	// TODO: Move filteredImages to it's own static class.
	//filteredImages: Map<string, CachedImages> = new Map<string, CachedImages>();

	//filterImages(filter: string) {
	//	if (this.filteredImages[filter])
	//		return;
	//	this.createFilteredImages(filter);
	//}

	//createFilteredImages(filter: string) {
	//	console.log(`Filtering images for ${filter}...`);
	//	this.filteredImages[filter] = new CachedImages(this.images, filter);
	//	console.log(`Done filtering images for ${filter}...`);
	//}


	static loadSprites: boolean;
	loadedAllImages: boolean;
	frameIndex: number;
	reverse: boolean;
	lastUpdateTime: any;
	assetFolder: string;
	imagesLoaded: Promise<HTMLImageElement>;

	private _images: HTMLImageElement[];
	onImageLoaded: (image: HTMLImageElement) => void;
	framesToCount: number;
	framesToLoad: number;

	get images(): HTMLImageElement[] {
		if (!this.loadedAllImages) {
			if (Part.loadSprites) {
				this.loadImages(false);
			}
			else {
				const image = new Image();
				this._images.push(image);
			}
			this.loadedAllImages = true;
		}
		return this._images;
	}

	set images(newValue: HTMLImageElement[]) {
		this._images = newValue;
	}


	private loadImages(onlyLoadOne: boolean) {
		let actualFrameCount = 0;
		let numDigits: number;
		if (this.frameCount > 999)
			numDigits = 4;
		else if (this.frameCount > 99)
			numDigits = 3;
		else if (this.frameCount > 9)
			numDigits = 2;
		else
			numDigits = 1;
		//let framesToLoad: number = 1;
		//let framesToCount: number = 1;
		//if (this.frameCount > 60 && !globalBypassFrameSkip) {
		//	framesToLoad = globalFramesToLoad;
		//	framesToCount = globalFramesToCount;
		//	this.frameRate = framesToCount / framesToLoad;
		//}

		this.frameRate = this.framesToCount / this.framesToLoad;

		let totalFramesToLoad = this.frameCount * this.framesToLoad / this.framesToCount;
		const frameIncrementor = (this.frameCount) / totalFramesToLoad;
		let absoluteStartIndex = 0;

		if (onlyLoadOne) {
			totalFramesToLoad = 1;
		}

		for (let i = 0; i < totalFramesToLoad; i++) {
			if (onlyLoadOne || i > 0) {
				const image = new Image();
				let indexStr: string = Math.round(absoluteStartIndex).toString();
				while (this.padFileIndex && indexStr.length < numDigits)
					indexStr = '0' + indexStr;
				image.src = this.assetFolder + this.fileName + indexStr + '.png';
				this._images.push(image);
			}

			actualFrameCount++;
			absoluteStartIndex += frameIncrementor;
		}

		if (onlyLoadOne) {
			//const self: Part = this;
			this._images[0].onload = function () {
				try {
					if (this.onImageLoaded) {
						this.onImageLoaded(this._images[0]);
					}
				}
				catch (ex) {
					console.log('Part/this: ' + this);
					console.error('ex: ' + ex);
				}
			}.bind(this);

			if (this.frameCount > 1) {
				PartBackgroundLoader.add(this);
			}
		}
		else {
			PartBackgroundLoader.remove(this);
			this.frameCount = actualFrameCount;
		}
	}

	constructor(public fileName: string, public frameCount: number,
		public animationStyle: AnimationStyle,
		private offsetX: number,
		private offsetY: number,
		private frameRate = 100,
		public jiggleX: number = 0,
		public jiggleY: number = 0,
		private padFileIndex: boolean = false) {
		this.assetFolder = Folders.assets;
		this.loadedAllImages = false;

		this.frameIndex = 0;
		this.reverse = false;
		this.lastUpdateTime = null;

		this._images = [];

		this.framesToLoad = 1;
		this.framesToCount = 1;

		// Commenting this optimization out because this code is running on a much faster machine.
		//if (this.frameCount > 60 && !globalBypassFrameSkip) {
		//	this.framesToLoad = globalFramesToLoad;
		//	this.framesToCount = globalFramesToCount;
		//}

		this.loadImages(true);
	}

	fileExists(url) {
		var http = new XMLHttpRequest();
		http.open('HEAD', url, false);
		http.send();
		return http.status !== 404;
	}

	isOnLastFrame() {
		return this.frameIndex == this.frameCount - 1;
	}

	isOnFirstFrame() {
		return this.frameIndex == 0;
	}

	advanceFrameIfNecessary() {
		if (!this.lastUpdateTime) {
			this.lastUpdateTime = performance.now();
			return;
		}

		var now: number = performance.now();
		var msPassed = now - this.lastUpdateTime;
		if (msPassed < this.frameRate)
			return;

		if (this.animationStyle == AnimationStyle.Static)
			return;
		if (this.animationStyle == AnimationStyle.Random)
			this.frameIndex = Random.intMax(this.frameCount);

		if (this.reverse) {
			this.frameIndex--;
			if (this.frameIndex < 0)
				if (this.animationStyle == AnimationStyle.Sequential)
					this.frameIndex = 0;
				else // AnimationStyle.Loop
					this.frameIndex = this.frameCount - 1;
		}
		else {
			this.frameIndex++;
			if (this.frameIndex >= this.frameCount)
				if (this.animationStyle == AnimationStyle.Sequential)
					this.frameIndex = this.frameCount - 1;
				else // AnimationStyle.Loop
					this.frameIndex = 0;
		}

		this.lastUpdateTime = performance.now();
	}

	getWiggle(amount: number) {
		if (amount == 0 || !amount)
			return 0;
		return Random.intBetween(-amount, amount);
	}

	draw(context: CanvasRenderingContext2D, x: number, y: number, horizontalScale = 1, verticalScale = -1) {
		this.advanceFrameIfNecessary();
		// TODO: Update all the calls so vertical scale is specified and change verticalScale's default value to 1?
		if (verticalScale === -1)
			verticalScale = horizontalScale;
		this.drawByIndex(context, x, y, this.frameIndex, horizontalScale, verticalScale);
	}

	drawByIndex(context: CanvasRenderingContext2D, x: number, y: number, frameIndex: number, horizontalScale = 1, verticalScale = -1, rotation = 0, centerX = 0, centerY = 0, flipHorizontally = false, flipVertically = false /* , filterCache: CachedImages = null */): void {
		if (frameIndex < 0)
			return;

		//if (!filterCache)
			if (!this.images[frameIndex]) {
				console.error(`Image not found for ${this.fileName}*.png at frameIndex: ${frameIndex}`);
				return;
			}

		if (verticalScale === -1)
			verticalScale = horizontalScale;

		const scaling: boolean = flipHorizontally || flipVertically || horizontalScale !== 1 || verticalScale !== 1;
		const rotating: boolean = rotation !== 0;
		const transforming: boolean = rotating || scaling;

		if (transforming) {
			context.save();
			context.translate(centerX, centerY);
		}

		if (rotating) {
			context.rotate(rotation * Math.PI / 180);
		}

		if (scaling) {
			let horizontalFlipScale = 1;
			let verticalFlipScale = 1;
			if (flipHorizontally)
				horizontalFlipScale = -1;
			if (flipVertically)
				verticalFlipScale = -1;
			context.scale(horizontalFlipScale * horizontalScale, verticalFlipScale * verticalScale);
		}


		if (transforming) {
			context.translate(-centerX, -centerY);
		}

		const wiggleX: number = this.getWiggle(this.jiggleX);
		const wiggleY: number = this.getWiggle(this.jiggleY);
		const dx: number = Math.round(x + this.offsetX + wiggleX);
		const dy: number = Math.round(y + this.offsetY + wiggleY);

		//let canvasImageSource: CanvasImageSource;
		//if (filterCache) {
		//	canvasImageSource = filterCache.getCanvas(frameIndex);
		//	if (canvasImageSource === null)
		//		console.error(`canvasImageSource for ${this.fileName} at frameIndex ${frameIndex} not found.`);
		//}
		//if (canvasImageSource) {
		//	// TODO: If we change to a single-canvas solution, then we should call context.cropImage instead.
		//	console.log(`drawing cached image for ${this.fileName} at frameIndex ${frameIndex}.`);
		//	let sourceOffsetX: number = filterCache.getSourceOffsetX(frameIndex);
		//	let sourceOffsetY: number = filterCache.getSourceOffsetY(frameIndex);
		//	let width: number = filterCache.imageWidth;
		//	let height: number = filterCache.imageHeight;
		//	//` ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)
		//	context.drawImage(canvasImageSource, sourceOffsetX, sourceOffsetY, width, height, dx, dy, width, height);
		//}
		//else
			context.drawImage(this.images[frameIndex], dx, dy);


		if (transforming) {
			context.restore();
		}
	}


	drawCroppedByIndex(context: CanvasRenderingContext2D, x: number, y: number, frameIndex: number,
		sx: number, sy: number, sw: number, sh: number, dw: number, dh: number): void {
		//` ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)
		let dx: number = x + this.offsetX + this.getWiggle(this.jiggleX);
		let dy: number = y + this.offsetY + this.getWiggle(this.jiggleY);
		context.drawImage(this.images[frameIndex], sx, sy, sw, sh, dx, dy, dw, dh);
	}
}