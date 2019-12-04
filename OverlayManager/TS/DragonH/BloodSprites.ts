class BloodSprites extends Sprites {
	totallyContained: boolean;
	height: number;
	constructor(baseAnimationName: string, expectedFrameCount: number, frameInterval: number, animationStyle: AnimationStyle, padFileIndex: boolean = false, hitFloorFunc?, onLoadedFunc?) {
		super(baseAnimationName, expectedFrameCount, frameInterval, animationStyle, padFileIndex, hitFloorFunc, onLoadedFunc);
	}
}