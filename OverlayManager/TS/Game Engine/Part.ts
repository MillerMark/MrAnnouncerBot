const globalFramesToLoad: number = 1;
const globalFramesToCount: number = 2;
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
			console.log(`Image "${name} was not found.`);

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

class Part {
	static loadSprites: boolean;
	loadedAllImages: boolean;
	frameIndex: number;
	reverse: boolean;
	lastUpdateTime: any;
	assetFolder: string;
	imagesLoaded: Promise<HTMLImageElement>;

	private _images: HTMLImageElement[];
	onImageLoaded: (image: HTMLImageElement) => void;

	get images(): HTMLImageElement[] {
		if (!this.loadedAllImages) {
			if (Part.loadSprites) {
				this.loadImages(false);
			}
			else {
				var image = new Image();
				this._images.push(image);
			}
			this.loadedAllImages = true;
		}
		return this._images;
	}

	private loadImages(onlyLoadOne: boolean) {
		var actualFrameCount = 0;
		var numDigits: number;
		if (this.frameCount > 999)
			numDigits = 4;
		else if (this.frameCount > 99)
			numDigits = 3;
		else if (this.frameCount > 9)
			numDigits = 2;
		else
			numDigits = 1;
		let framesToLoad: number = 1;
		let framesToCount: number = 1;
		if (this.frameCount > 60 && !globalBypassFrameSkip) {
			framesToLoad = globalFramesToLoad;
			framesToCount = globalFramesToCount;
			this.frameRate = framesToCount / framesToLoad;
		}

		let totalFramesToLoad = this.frameCount * framesToLoad / framesToCount - this._images.length;
		let frameIncrementor = this.frameCount / totalFramesToLoad;
		let absoluteStartIndex = this._images.length;

		if (onlyLoadOne) {
			totalFramesToLoad = 1;
		}

		for (var i = 0; i < totalFramesToLoad; i++) {
			var image = new Image();
			var indexStr: string = Math.round(absoluteStartIndex).toString();
			while (this.padFileIndex && indexStr.length < numDigits)
				indexStr = '0' + indexStr;
			image.src = this.assetFolder + this.fileName + indexStr + '.png';
			this._images.push(image);
			actualFrameCount++;
			absoluteStartIndex += frameIncrementor;
		}

		if (onlyLoadOne) {
			var self: any = this;
			this._images[0].onload = function () {
				self.onImageLoaded(self._images[0]);
			};
		}
		else 
			this.frameCount = actualFrameCount;
	}

	set images(newValue: HTMLImageElement[]) {
		this._images = newValue;
	}

	constructor(private fileName: string, public frameCount: number,
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
		this.loadImages(true);
	}

	fileExists(url) {
		var http = new XMLHttpRequest();
		http.open('HEAD', url, false);
		http.send();
		return http.status != 404;
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

	getJiggle(amount: number) {
		if (amount == 0 || !amount)
			return 0;
		return Random.intBetween(-amount, amount);
	}

	draw(context: CanvasRenderingContext2D, x: number, y: number) {
		this.advanceFrameIfNecessary();
		this.drawByIndex(context, x, y, this.frameIndex);
	}

	drawByIndex(context: CanvasRenderingContext2D, x: number, y: number, frameIndex: number, rotation: number = 0, centerX: number = 0, centerY: number = 0): void {
		if (frameIndex < 0)
			return;
		if (!this.images[frameIndex]) {
			console.error('frameIndex: ' + frameIndex + ', fileName: ' + this.fileName);
			return;
		}

		var needToRestoreContext: boolean = false;

		if (rotation != 0) {
			context.save();
			needToRestoreContext = true;
			context.translate(centerX, centerY);
			context.rotate(rotation * Math.PI / 180);
			context.translate(-centerX, -centerY);
		}

		context.drawImage(this.images[frameIndex],
			x + this.offsetX + this.getJiggle(this.jiggleX),
			y + this.offsetY + this.getJiggle(this.jiggleY));

		if (needToRestoreContext) {
			context.restore();
		}
	}

	drawCroppedByIndex(context: CanvasRenderingContext2D, x: number, y: number, frameIndex: number,
		sx: number, sy: number, sw: number, sh: number, dw: number, dh: number): void {
		//` ![](4E7BDCDC4E1A78AB2CC6D9EF427CBD98.png)
		let dx: number = x + this.offsetX + this.getJiggle(this.jiggleX);
		let dy: number = y + this.offsetY + this.getJiggle(this.jiggleY);
		context.drawImage(this.images[frameIndex], sx, sy, sw, sh, dx, dy, dw, dh);
	}
}