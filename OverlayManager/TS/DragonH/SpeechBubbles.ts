enum SpeechType {
  None,
  Thinks,
  Says,
  Listening,
  Thinking,
  HideThoughts,
}

class SpeechData {
  okayToDraw = false;
  x: number;
  y: number;
  width: number;
  height: number;
  textColor: string;
  playerId: number;
  fontSize: number;
  static readonly maxTextWidth: number = 124;
  static readonly maxTextFontSize: number = 90;

  paragraph: ParagraphWrapData;
  static styleDelimiters: LayoutDelimiters[] = [
    new LayoutDelimiters(LayoutStyle.bold, '*', '*')
    //,
    //new LayoutDelimiters(LayoutStyle.italic, '[', ']')
  ];

  constructor(wordWrapper: WordWrapper, wordRenderer: WordRenderer, context: CanvasRenderingContext2D, text: string, idealWidth: number, idealAspectRatio: number, fontSize: number, fontName: string, topBottomReducePercent: number, maxHeight: number = undefined) {
    this.setFontSize(context, fontName, wordWrapper, wordRenderer, fontSize);

    const maxAttempts = 8;
    let attemptCount = 0;
    let lastWidthAdjustPercent = 0.5;
    let width: number = idealWidth;
    let lastWidth: number = idealWidth;
    let lastDeltaPercentageAwayFromIdeal: number = Number.MAX_VALUE;
    do {
      attemptCount++;
      wordRenderer.fontSize = this.fontSize;
      this.paragraph = wordWrapper.getWordWrappedLinesForParagraphs(context, text, width, SpeechData.styleDelimiters, wordRenderer, topBottomReducePercent);
      const currentAspectRatio: number = this.paragraph.getAspectRatio();

      const thisDeltaPercentageAwayFromIdeal: number = Math.abs(currentAspectRatio / idealAspectRatio - 1);
      if (lastDeltaPercentageAwayFromIdeal === Number.MAX_VALUE || thisDeltaPercentageAwayFromIdeal < lastDeltaPercentageAwayFromIdeal) {
        lastWidth = width;
        lastDeltaPercentageAwayFromIdeal = thisDeltaPercentageAwayFromIdeal;
      }
      else if (thisDeltaPercentageAwayFromIdeal > lastDeltaPercentageAwayFromIdeal) {
        width = lastWidth;
        lastWidthAdjustPercent /= 2;
        continue;
      }

      if (currentAspectRatio > idealAspectRatio) {
        width -= width * lastWidthAdjustPercent;
      }
      else if (currentAspectRatio < idealAspectRatio) {
        width += width * lastWidthAdjustPercent;
      }
      else
        break;

      lastWidthAdjustPercent /= 2;
    } while (attemptCount < maxAttempts);

    let fontScale: number = idealWidth / this.paragraph.getLongestLineWidth();
    this.fontSize = Math.min(this.fontSize * fontScale, SpeechData.maxTextFontSize);
    this.fontSizeChanged(context, fontName, wordWrapper, wordRenderer, text, idealWidth, fontScale, topBottomReducePercent);

    this.paragraph = wordWrapper.getWordWrappedLinesForParagraphs(context, text, idealWidth, SpeechData.styleDelimiters, wordRenderer, topBottomReducePercent);

    let numFontAdjusts: number = 0;
    let longestLineWidth: number = this.paragraph.getLongestLineWidth();
    while (longestLineWidth > idealWidth && numFontAdjusts < 5)
    {
      fontScale = (1.0 + idealWidth / longestLineWidth) / 2.0;
      this.fontSize = Math.min(this.fontSize * fontScale, SpeechData.maxTextFontSize);
      this.fontSizeChanged(context, fontName, wordWrapper, wordRenderer, text, idealWidth, fontScale, topBottomReducePercent);
      this.paragraph = wordWrapper.getWordWrappedLinesForParagraphs(context, text, idealWidth, SpeechData.styleDelimiters, wordRenderer, topBottomReducePercent);
      longestLineWidth = this.paragraph.getLongestLineWidth();
      numFontAdjusts++;
    }

    if (maxHeight) {
      let totalHeight: number = this.paragraph.getParagraphHeight();
      let numTries: number = 0;
      while (totalHeight > maxHeight && numTries < 3) {
        // Reduce the font size so all the text fits on the specified height.
        let scaleFactor: number = maxHeight / totalHeight;
        this.fontSize *= scaleFactor;
        fontScale *= scaleFactor;
        this.fontSizeChanged(context, fontName, wordWrapper, wordRenderer, text, idealWidth, fontScale, topBottomReducePercent);
        this.paragraph = wordWrapper.getWordWrappedLinesForParagraphs(context, text, idealWidth, SpeechData.styleDelimiters, wordRenderer, topBottomReducePercent);
        totalHeight = this.paragraph.getParagraphHeight();
        numTries++;
      }
    }
  }

  private fontSizeChanged(context: CanvasRenderingContext2D, fontName: string, wordWrapper: WordWrapper, wordRenderer: WordRenderer, text: string, width: number, fontScale: number, topBottomReducePercent: number) {
    this.setFontSize(context, fontName, wordWrapper, wordRenderer, this.fontSize);
    this.paragraph = wordWrapper.getWordWrappedLinesForParagraphs(context, text, width * fontScale, SpeechData.styleDelimiters, wordRenderer, topBottomReducePercent);
  }

  private setFontSize(context: CanvasRenderingContext2D, fontName: string, wordWrapper: WordWrapper, wordRenderer: WordRenderer, fontSize: number) {
    //context.font = `${fontSize}px ${fontName}`;
    this.fontSize = fontSize;
    wordWrapper.fontSize = fontSize;
    wordRenderer.fontSize = fontSize;
    wordRenderer.fontChanged(context);
  }

  public static getScale(paragraphWidth: number, textWidth: number, paragraphHeight: number, textHeight: number) {
    let horizontalScale = 1;
    let verticalScale = 1;

    horizontalScale = paragraphWidth / textWidth;
    verticalScale = paragraphHeight / textHeight;

    const thisAspectRatio: number = horizontalScale / verticalScale;
    const maxAspectRatioFactor = 2;
    const maxAspectRatio: number = maxAspectRatioFactor / 1;
    const minAspectRatio: number = 1 / maxAspectRatioFactor;

    if (thisAspectRatio < minAspectRatio) {
      const horizontalScaleFactor = 1.2;
      horizontalScale = verticalScale * minAspectRatio * horizontalScaleFactor;
    }
    else if (thisAspectRatio > maxAspectRatio) {
      verticalScale = horizontalScale / maxAspectRatio;
    }
    return { horizontalScale, verticalScale };
  }

  public static getReadingTimeMs(speechStr: string) {
    const numWords: number = (speechStr.match(/\s/g) || []).length + 1;
    const averageReadingTimePerWordMs = 300;
    const safetyFactor = 2.3;
    const totalReadingTime: number = Math.max(1500, numWords * averageReadingTimePerWordMs * safetyFactor);
    return totalReadingTime;
  }
}

class ShowBookManager {
  wordRenderer: WordRenderer;
  wordWrapper: WordWrapper;
  book: Sprites;
  hands: Sprites;
  bookSprite: ColorShiftingSpriteProxy;
  handsSprite: SpriteProxy;
  timeoutHandle: number;

  constructor() {
    this.wordWrapper = new WordWrapper();
    this.wordRenderer = new WordRenderer();
    this.wordRenderer.rotation = -3.5;
    this.wordRenderer.fontName = 'solex';  // ccbiffbamboom
    this.wordRenderer.fontWeightBold = '400';
  }

  hasMixedCase(txt: string): boolean {
    for (let i = 1; i < txt.length; i++) {
      if (txt[i].toUpperCase() == txt[i])
        return true;
    }
    return false;
  }

  titleCase(title: string, withLowers: boolean = false): string {
    let result: string = title.replace(/([^\s:\-'])([^\s:\-']*)/g, (txt) => {
      if (this.hasMixedCase(txt))
        return txt;
      return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    }).replace(/Mc(.)/g, function (match, next) {
      return 'Mc' + next.toUpperCase();
    }).replace(/Coderush/g, 'CodeRush')
      .replace(/Typescript/g, 'TypeScript')
      .replace(/Javascript/g, 'JavaScript')
      .replace(/Jetbrains/g, 'PoopBrains')
      .replace(/Resharper/g, 'RePooper');

    const lowers = ['A', 'An', 'The', 'At', 'By', 'For', 'In', 'Of', 'On', 'To', 'Up', 'And', 'As', 'But', 'Or', 'Nor', 'Not'];
    for (let i = 0; i < lowers.length; i++) {
      result = result.replace(new RegExp('\\s' + lowers[i] + '\\s', 'g'), function (txt) {
        return txt.toLowerCase();
      });
    }

    const uppers = ['R&d', 'Vb', 'Ftw', 'Wtf'];
    for (let i = 0; i < uppers.length; i++) {
      result = result.replace(new RegExp('\\b' + uppers[i] + '\\b', 'g'), uppers[i].toUpperCase());
    }

    result = result.replace('\'S', '\'s').replace(' \'s', ' \'S');
    result = result.replace('\'T', '\'t').replace(' \'t', ' \'T');
    result = result.replace('\'Ll', '\'ll').replace('\'Re', '\'re');
    result = result.replace('I\'M', 'I\'m');

    return result;
  }

  private getParagraphSize(speechData: SpeechData) {
    const horizontalMarginBuffer = 1.15; // 115%
    const verticalMarginBuffer = 1.20; // 120%

    const paragraphWidth: number = speechData.paragraph.getLongestLineWidth() * horizontalMarginBuffer;
    const paragraphHeight: number = speechData.paragraph.getParagraphHeight() * verticalMarginBuffer;
    return { paragraphWidth, paragraphHeight };
  }

  static readonly verticalDrop: number = 30;

  createSpeechData(context: CanvasRenderingContext2D, textToShow: string): SpeechData {
    const bookTitleWidth: number = 200;
    const bookTitleHeight: number = 193 - ShowBookManager.verticalDrop;
    const aspectRatio: number = bookTitleWidth / bookTitleHeight;
    const topBottomReducePercent: number = 0;
    const idealFontSize: number = 22;
    this.wordRenderer.setActiveStyle(context, LayoutStyle.italic);
    const speechData: SpeechData = new SpeechData(this.wordWrapper, this.wordRenderer, context, textToShow, bookTitleWidth, aspectRatio, idealFontSize, this.wordRenderer.fontName, topBottomReducePercent, bookTitleHeight);

    speechData.okayToDraw = true;

    speechData.width = bookTitleWidth;
    speechData.height = bookTitleHeight;
    speechData.textColor = `#ffffff`;

    return speechData;
  }

  showBook(ctx: CanvasRenderingContext2D, title: string) {
    if (this.bookSprite) {
      this.bookSprite.fadeOutNow(250);
      this.handsSprite.fadeOutNow(250);
      this.bookSprite = null;
      this.handsSprite = null;
    }

    if (this.timeoutHandle) {
      clearTimeout(this.timeoutHandle);
      this.timeoutHandle = null;
    }

    let xPos: number = 500;
    const bookHeight: number = 323;
    const screenBottom: number = 1080;
    let yStartPos: number = screenBottom + bookHeight;
    let colorShift: number = Random.between(0, 360);
    this.bookSprite = this.book.addShifted(xPos, yStartPos, 0, colorShift);
    this.handsSprite = this.hands.add(xPos, yStartPos, 0);

    const entryTime: number = 800;
    const exitTime: number = 800;
    let startTime: number = performance.now();

    const titleReadingTimeMs: number = SpeechData.getReadingTimeMs(title);
    const extraTimeOnScreen: number = 2000;

    const timeOnScreen: number = entryTime + extraTimeOnScreen + titleReadingTimeMs + exitTime;

    let fromX: number = xPos - this.book.originX;
    let fromY: number = yStartPos - this.book.originY;
    let toX: number = xPos - this.book.originX;
    let toY: number = screenBottom - this.book.originY;

    this.bookSprite.data = this.createSpeechData(ctx, this.titleCase(title));
    this.bookSprite.ease(startTime, fromX, fromY, toX, toY, entryTime);
    this.handsSprite.ease(startTime, fromX, fromY, toX, toY, entryTime);

    this.timeoutHandle = setTimeout(() => {
      this.timeoutHandle = null;
      let now: number = performance.now();
      this.bookSprite.ease(now, toX, toY, fromX, fromY, exitTime);
      this.handsSprite.ease(now, toX, toY, fromX, fromY, exitTime);
    }, timeOnScreen - exitTime);
  }

  loadResources() {
    this.book = new Sprites('Rory/Book', 1, fps30, AnimationStyle.Static, true);
    this.book.originX = 174;
    this.book.originY = 298 - ShowBookManager.verticalDrop;
    this.book.moves = true;
    this.book.disableGravity();

    this.hands = new Sprites('Rory/Hands', 1, fps30, AnimationStyle.Static, true);
    this.hands.originX = 174;
    this.hands.originY = 298 - ShowBookManager.verticalDrop;
    this.hands.moves = true;
    this.hands.disableGravity();
  }

  updatePositions(nowMs: number) {
    this.book.updatePositions(nowMs);
    this.hands.updatePositions(nowMs);
  }

  draw(context: CanvasRenderingContext2D, nowMs: number) {
    this.book.draw(context, nowMs);
    this.drawText(context, this.book);
    this.hands.draw(context, nowMs);
  }

  drawSpriteText(context: CanvasRenderingContext2D, sprite: SpriteProxy) {
    context.textAlign = 'center';
    context.textBaseline = "middle";
    const speechData: SpeechData = sprite.data as SpeechData;
    if (speechData && speechData.okayToDraw) {
      this.wordRenderer.fontSize = speechData.fontSize;
      this.wordRenderer.setActiveStyle(context, LayoutStyle.italic);

      this.wordRenderer.textColor = speechData.textColor;
      const leftMargin: number = 35;
      let shortTopMargin: number = 35;
      let mediumTopMargin: number = 25;
      let tallTopMargin: number = 10;
      let paragraphHeight: number = speechData.paragraph.getParagraphHeight();
      const tallThreshold: number = 166;
      const shortThreshold: number = 100;
      const yOffset: number = 35;
      let centerY: number;
      const basicYOffset: number = sprite.y + yOffset + speechData.height / 3;
      if (paragraphHeight > tallThreshold)
        centerY = tallTopMargin + basicYOffset;
      else if (paragraphHeight < shortThreshold)
        centerY = shortTopMargin + basicYOffset;
      else
        centerY = mediumTopMargin + basicYOffset;

      let centerX: number = leftMargin + sprite.x + this.book.originX + speechData.width / 2;

      this.wordRenderer.renderParagraphs(context, speechData.paragraph.lineData, centerX, centerY, SpeechData.styleDelimiters);

      //drawCrossHairs(context, centerX, centerY);
    }
  }

  drawText(context: CanvasRenderingContext2D, sprites: Sprites) {
    sprites.spriteProxies.forEach((sprite: SpriteProxy) => {
      this.drawSpriteText(context, sprite);
    });
  }
}

class ThinkingManager {
  listeningCollection: SpriteCollection;
  thinkingCollection: SpriteCollection;
  constructor() {
    this.listeningCollection = new SpriteCollection();
    this.thinkingCollection = new SpriteCollection();
  }

  draw(context: CanvasRenderingContext2D, nowMs: number) {
    this.listeningCollection.draw(context, nowMs);
    this.thinkingCollection.draw(context, nowMs);
  }

  addThinkingAnimation(collection: SpriteCollection, name: string, frameCount: number): void {
    let originX: number;
    let originY: number;
    const baseAnimationName: string = `SpeechBubbles/Thinking/${name}`;
    let sprites: Sprites = new Sprites(baseAnimationName, frameCount, fps30, AnimationStyle.Loop, true);
    sprites.originX = 89;
    sprites.originY = 80;
    sprites.name = name;
    collection.add(sprites);
  }

  static readonly thinkingMaxLifetimeMs: number = 5000;

  showRandomAnimation(collection: SpriteCollection, xPos: number, yPos: number, playerId: number, ownerBubble: SpriteProxy): Listener {
    const randomThoughtSprites: Sprites = collection.getRandom();
    if (!randomThoughtSprites)
      return;

    return Listener.create(randomThoughtSprites, xPos + randomThoughtSprites.originX, yPos + randomThoughtSprites.originY, playerId, ownerBubble);
  }

  showRandomListeningAnimation(xPos: number, yPos: number, playerId: number, ownerBubble: SpriteProxy) {
    var listener: Listener = this.showRandomAnimation(this.listeningCollection, xPos + 45, yPos + 40, playerId, ownerBubble);
    listener.setHueSatBrightness(220, 80, 120);
  }

  showRandomThinkingAnimation(xPos: number, yPos: number, playerId: number, ownerBubble: SpriteProxy) {
    var listener: Listener = this.showRandomAnimation(this.thinkingCollection, xPos, yPos, playerId, ownerBubble);
    listener.setHueSatBrightness(0, 100, 120);
  }


  loadResources()
  {
    this.addThinkingAnimation(this.thinkingCollection, 'A', 38);
    this.addThinkingAnimation(this.thinkingCollection, 'B', 19);
    this.addThinkingAnimation(this.thinkingCollection, 'C', 44);
    this.addThinkingAnimation(this.listeningCollection, 'D', 49);
    this.addThinkingAnimation(this.listeningCollection, 'E', 29);
    this.addThinkingAnimation(this.listeningCollection, 'F', 29);
    this.addThinkingAnimation(this.listeningCollection, 'G', 29);
    this.addThinkingAnimation(this.listeningCollection, 'H', 34);
    this.addThinkingAnimation(this.thinkingCollection, 'I', 31);
    this.addThinkingAnimation(this.listeningCollection, 'J', 27);
    this.addThinkingAnimation(this.listeningCollection, 'K', 25);
  }
}

class Listener extends ColorShiftingSpriteProxy {
  constructor(startingFrameNumber: number, center: Vector) {
    super(startingFrameNumber, center, ThinkingManager.thinkingMaxLifetimeMs);
  }

  playerId: number;
  ownerBubble: SpriteProxy;

  matches(matchData: any): boolean {
    return this.playerId === matchData;
  }

  static create(spriteArray: Sprites, x: number, y: number, playerId: number, ownerBubble: SpriteProxy): Listener {
    let thinking: Listener = new Listener(0, new Vector(x - spriteArray.originX, y - spriteArray.originY));

    thinking.playerId = playerId;
    thinking.ownerBubble = ownerBubble;

    thinking.fadeInTime = 400;
    thinking.fadeOutTime = 400;
    thinking.expirationDate = performance.now() + thinking.fadeInTime + ThinkingManager.thinkingMaxLifetimeMs + thinking.fadeOutTime;

    spriteArray.spriteProxies.push(thinking);

    return thinking;
  }
}

class SpeechBubbleManager {
  soundManager: SoundManager;
  wordRenderer: WordRenderer;
  wordWrapper: WordWrapper;
  speechBubbles: Sprites;
  thoughtBubbles: Sprites;
  thinkingManager: ThinkingManager;

  static readonly textColor: string = '#1a0c0a';
  static readonly bulletColor: string = '#5b3c35';
  static readonly emphasisColor: string = '#a01a00';
  static readonly tableLineColor: string = '#5b3c35';
  static readonly fontName: string = 'ccbiffbamboom';
  static readonly fontSize: number = 20;
  static readonly bulletIndent = 8;
  static readonly emphasisFontHeightIncrease = 0;

  //`![](31D33771D101AC43215689BD6498E18E.png)
  static readonly emphasisFontStyleAscender = 13;  // This is the height from the baseline to the top of the tallest character.

  idealTextAspectRatio = 1;

  constructor(private iGetPlayerX: IGetPlayerX, private iGetCreatureX: IGetCreatureX = null) {
    this.thinkingManager = new ThinkingManager();
    this.soundManager = new SoundManager('GameDev/Assets/DragonH/SoundEffects/SpeechBubbles');
    this.wordWrapper = new WordWrapper();

    this.wordRenderer = new WordRenderer();
    this.wordRenderer.fontName = SpeechBubbleManager.fontName;
    this.wordRenderer.fontSize = SpeechBubbleManager.fontSize;
    this.wordRenderer.emphasisColor = SpeechBubbleManager.emphasisColor;
    this.wordRenderer.emphasisFontHeightIncrease = SpeechBubbleManager.emphasisFontHeightIncrease;
    this.wordRenderer.emphasisFontStyleAscender = SpeechBubbleManager.emphasisFontStyleAscender;
    this.wordRenderer.bulletIndent = SpeechBubbleManager.bulletIndent;
    this.wordRenderer.bulletColor = SpeechBubbleManager.bulletColor;
    this.wordRenderer.textColor = SpeechBubbleManager.textColor;
    this.wordRenderer.tableLineColor = SpeechBubbleManager.tableLineColor;
  }

  loadResources() {
    this.speechBubbles = new Sprites('SpeechBubbles/SpeechBubble', 38, fps30, AnimationStyle.Loop, true);
    this.speechBubbles.returnFrameIndex = 8;
    this.speechBubbles.segmentSize = 15;
    this.speechBubbles.originX = 14;
    this.speechBubbles.originY = 242;

    this.thoughtBubbles = new Sprites('SpeechBubbles/ThoughtBubble', 38, fps30, AnimationStyle.Loop, true);
    this.thoughtBubbles.returnFrameIndex = 8;
    this.thoughtBubbles.segmentSize = 15;
    this.thoughtBubbles.originX = 54;
    this.thoughtBubbles.originY = 242;

    this.thinkingManager.loadResources();
  }

  static readonly dungeonMasterId: number = 100;

  // Expected syntax: {playerId} {says|thinks}: {message to say or think}
  // 
  sayOrThinkSomething(context: CanvasRenderingContext2D, speechStr: string) {
    const { playerId, speechType, textToShow, colorStr, xOffset }: { playerId: number; speechType: SpeechType; textToShow: string; colorStr: string; xOffset: number } = this.getSpeechCommandParts(speechStr);
    if (speechType !== SpeechType.Listening)
    {
      const thinker: SpriteProxy = this.thinkingManager.thinkingCollection.find(playerId);
      if (thinker instanceof Listener) {
        thinker.ownerBubble.fadeOutNow(200);
        thinker.fadeOutNow(200);
      }

      const listener: SpriteProxy = this.thinkingManager.listeningCollection.find(playerId);

      if (listener instanceof Listener)
      {
        if (speechType === SpeechType.Thinking)
          this.thinkingManager.showRandomThinkingAnimation(listener.x, listener.y, playerId, listener.ownerBubble);
        else
          listener.ownerBubble.fadeOutNow(200);

        listener.fadeOutNow(200);
        return;
      }

      if (speechType === SpeechType.HideThoughts)
        return;
    }

    //console.log('sayOrThinkSomething - colorStr: ' + colorStr);
    //sconsole.log('sayOrThinkSomething - speechStr: ' + speechStr);

    if (playerId === Number.MIN_VALUE || playerId === Creature.invalidCreatureId || speechType === SpeechType.None)
      return;

    if (!this.iGetCreatureX && playerId < 0) {
      console.error(`!this.iGetCreatureX && playerId (${playerId}) < 0`);
      return;
    }

    this.playBubbleEntranceSound(speechType);

    this.hideAnyBubblesBelongingToPlayer(playerId);

    const { playerX, playerY }: { playerX: number; playerY: number } = this.getPlayerXY(playerId);

    let { textWidth, topBottomReducePercent, textHeight, sprites, offsetX, textStartX, offsetY, textStartY }: { textWidth: number; topBottomReducePercent: number; textHeight: number; sprites: Sprites; offsetX: number; textStartX: number; offsetY: number; textStartY: number } = this.initializeBasedOnBubbleType(speechType);

    let speechData: SpeechData = null;
    let hScale: number;
    let vScale: number;

    if (speechType === SpeechType.Listening) {
      hScale = 0.77;
      vScale = 1;
    }
    else 
     {
      speechData = new SpeechData(this.wordWrapper, this.wordRenderer, context, textToShow, textWidth, this.idealTextAspectRatio, SpeechBubbleManager.fontSize, SpeechBubbleManager.fontName, topBottomReducePercent);

      const { paragraphWidth, paragraphHeight }: { paragraphWidth: number; paragraphHeight: number } = this.getParagraphSize(speechData);

      const { horizontalScale, verticalScale } = SpeechData.getScale(paragraphWidth, textWidth, paragraphHeight, textHeight);
      hScale = horizontalScale;
      vScale = verticalScale;
     }
    

    //const scaledOffsetY: number = sprites.originY + offsetY * verticalScale;

    let scaledOffsetX: number | undefined;
    let flippedOffsetX;
    let flippedHorizontalTextOffset;
    let flippedHorizontally;
    ({ scaledOffsetX, flippedOffsetX, flippedHorizontalTextOffset, flippedHorizontally, textStartX } = this.checkForHorizontalFlip(sprites, offsetX, hScale, playerX, textStartX, textWidth));

    let xPos = playerX + xOffset + scaledOffsetX + flippedOffsetX;

    const verticalTextOffsetBecauseNoDescenders: number = this.wordRenderer.fontSize * 0.12;   // Shift down a bit since there are no descenders in the Comic font.
    let yPos: number = playerY - offsetY * vScale;

    const { scaledPosOffsetX, scaledPosOffsetY } = this.getScaledPosOffset(sprites, hScale, vScale);

    if (speechData)
    {
      speechData.x = xPos + flippedHorizontalTextOffset - scaledPosOffsetX + textStartX * hScale;
      speechData.y = this.getSpeechDataY(yPos, verticalTextOffsetBecauseNoDescenders, scaledPosOffsetY, textStartY, vScale, speechData);
    }

    let flipVertically;
    ({ flipVertically, xPos, yPos, textStartY } = this.checkForVerticalFlip(speechData, playerId, speechType, vScale, xPos, yPos, textStartY, textHeight, verticalTextOffsetBecauseNoDescenders, scaledPosOffsetY));

    let totalReadingTime: number;
    if (speechData)
    {
      speechData.width = textWidth * hScale;
      speechData.height = textHeight * vScale;
      speechData.textColor = colorStr;
      speechData.playerId = playerId;
      totalReadingTime = SpeechData.getReadingTimeMs(speechStr);
    }
    else 
      totalReadingTime = ThinkingManager.thinkingMaxLifetimeMs;

    this.createNewSprite(sprites, xPos - scaledPosOffsetX, yPos - scaledPosOffsetY, speechStr, playerId, flippedHorizontally, flipVertically, speechData, speechType, hScale, vScale, totalReadingTime);
  }

  private createNewSprite(sprites: Sprites, xPos: number, yPos: number, speechStr: string, playerId: number, flippedHorizontally: any, flipVertically: any, speechData: SpeechData, speechType: SpeechType, horizontalScale: number, verticalScale: number, totalReadingTime: number) {
    //console.log(`yPos: ${yPos}, scaledPosOffsetY: ${scaledPosOffsetY}, speechData.y: ${speechData.y}`);
    //console.log(speechData);

    const sprite: SpriteProxy = sprites.add(xPos, yPos);
    sprite.playToEndOnExpire = true;
    sprite.fadeInTime = 400;
    sprite.fadeOutTime = 467;

    sprite.expirationDate = performance.now() + sprite.fadeInTime + totalReadingTime + sprite.fadeOutTime;
    sprite.flipHorizontally = flippedHorizontally;
    sprite.flipVertically = flipVertically;
    sprite.addOnFrameAdvanceCallback(this.spriteAdvanceFrame.bind(this));
    sprite.data = speechData;
    sprite.horizontalScale = horizontalScale;
    sprite.verticalScale = verticalScale;

    if (speechType == SpeechType.Listening)
    {
      // TODO: Consider adding support for flippedHorizontally and flipVertically.
      this.thinkingManager.showRandomListeningAnimation(xPos - sprites.originX, yPos - sprites.originY, playerId, sprite);
    }
  }

  private checkForVerticalFlip(speechData: SpeechData, playerId: number, speechType: SpeechType, verticalScale: number, xPos: any, yPos: number, textStartY: number, height: number, verticalTextOffsetBecauseNoDescenders: number, scaledPosOffsetY: number) {
    let flipVertically = false;

    if (speechData && speechData.y < 0) // Make sure top of text is on screen.
    {
      if (playerId === SpeechBubbleManager.dungeonMasterId) {
        flipVertically = true;
        if (speechType === SpeechType.Thinks || speechType === SpeechType.Listening) {
          const invertedSpeechBubbleOffsetX = 70;
          const invertedSpeechBubbleOffsetY = 125;
          speechData.x += invertedSpeechBubbleOffsetX;
          speechData.y += invertedSpeechBubbleOffsetY * verticalScale;
          xPos += invertedSpeechBubbleOffsetX;
          yPos += invertedSpeechBubbleOffsetY;
        }
        textStartY = -textStartY + height;
        speechData.y = this.getSpeechDataY(yPos, verticalTextOffsetBecauseNoDescenders, scaledPosOffsetY, textStartY, verticalScale, speechData);
      }
      else if (playerId < 0) {  //! in-game creatures.
        flipVertically = true;
        if (speechType === SpeechType.Thinks || speechType === SpeechType.Listening) {
          const invertedSpeechBubbleOffsetX = -60;
          const invertedSpeechBubbleOffsetY = 125;
          speechData.x += invertedSpeechBubbleOffsetX;
          speechData.y += invertedSpeechBubbleOffsetY * verticalScale;
          xPos += invertedSpeechBubbleOffsetX;
          yPos += invertedSpeechBubbleOffsetY;
        }
        textStartY = -textStartY + height;
        speechData.y = this.getSpeechDataY(yPos, verticalTextOffsetBecauseNoDescenders, scaledPosOffsetY, textStartY, verticalScale, speechData);
      }
      else {
        const delta: number = 0 - speechData.y;
        speechData.y = 0;
        yPos += delta;
      }
    }
    return { flipVertically, xPos, yPos, textStartY };
  }

  private checkForHorizontalFlip(sprites: Sprites, offsetX: number, horizontalScale: number, playerX: number, textStartX: number, width: number) {
    let scaledOffsetX: number = sprites.originX + offsetX * horizontalScale;
    let flippedHorizontalTextOffset = 0;
    let flippedOffsetX = 0;
    const maxRightPos = 1400;
    let flippedHorizontally = false;
    if (playerX + scaledOffsetX > maxRightPos) {
      scaledOffsetX = -scaledOffsetX;
      textStartX = -textStartX - width;
      flippedHorizontalTextOffset = -this.wordRenderer.fontSize / 6;
      flippedOffsetX = 60;
      flippedHorizontally = true;
    }
    return { scaledOffsetX, flippedOffsetX, flippedHorizontalTextOffset, flippedHorizontally, textStartX };
  }

  private initializeBasedOnBubbleType(speechType: SpeechType) {
    let sprites: Sprites;

    const speechBubbleOffsetX = 90;
    const speechBubbleOffsetY = 192;
    const speechBubbleTextOffsetX = 18;
    const speechBubbleTextOffsetY = 195;

    const speechBubbleTextWidth = 199;
    const speechBubbleTextHeight = 115;
    const speechBubbleAspectRatio = speechBubbleTextWidth / speechBubbleTextHeight;


    const thoughtBubbleOffsetX = 114;
    const thoughtBubbleOffsetY = 296;
    const thoughtBubbleTextOffsetX = -18;
    const thoughtBubbleTextOffsetY = 222;

    const thoughtBubbleTextWidth = 220;
    const thoughtBubbleTextHeight = 134;
    const thoughtBubbleAspectRatio = thoughtBubbleTextWidth / thoughtBubbleTextHeight;


    let offsetX: number;
    let offsetY: number;
    let textStartX: number;
    let textStartY: number;

    let textWidth: number;
    let textHeight: number;

    let topBottomReducePercent: number;

    if (speechType === SpeechType.Thinks || speechType === SpeechType.Listening || speechType === SpeechType.Thinking) {
      sprites = this.thoughtBubbles;
      offsetX = thoughtBubbleOffsetX;
      offsetY = thoughtBubbleOffsetY;
      textStartX = thoughtBubbleTextOffsetX;
      textStartY = thoughtBubbleTextOffsetY;
      textWidth = thoughtBubbleTextWidth;
      textHeight = thoughtBubbleTextHeight;
      topBottomReducePercent = 20;
      this.idealTextAspectRatio = thoughtBubbleAspectRatio;
    }
    else {
      sprites = this.speechBubbles;
      offsetX = speechBubbleOffsetX;
      offsetY = speechBubbleOffsetY;
      textStartX = speechBubbleTextOffsetX;
      textStartY = speechBubbleTextOffsetY;
      textWidth = speechBubbleTextWidth;
      textHeight = speechBubbleTextHeight;
      topBottomReducePercent = 0;
      this.idealTextAspectRatio = speechBubbleAspectRatio;
    }
    return { textWidth, topBottomReducePercent, textHeight, sprites, offsetX, textStartX, offsetY, textStartY };
  }

  private getScaledPosOffset(sprites: Sprites, horizontalScale: number, verticalScale: number) {
    let scaledPosOffsetX = 0;
    let scaledPosOffsetY = 0;

    if (horizontalScale !== 1) {
      scaledPosOffsetX = sprites.originX * (1 - horizontalScale);
    }

    if (verticalScale !== 1) {
      scaledPosOffsetY = sprites.originY * (1 - verticalScale);
    }
    return { scaledPosOffsetX, scaledPosOffsetY };
  }

  private getParagraphSize(speechData: SpeechData) {
    const horizontalMarginBuffer = 1.15; // 115%
    const verticalMarginBuffer = 1.20; // 120%

    const paragraphWidth: number = speechData.paragraph.getLongestLineWidth() * horizontalMarginBuffer;
    const paragraphHeight: number = speechData.paragraph.getParagraphHeight() * verticalMarginBuffer;
    return { paragraphWidth, paragraphHeight };
  }

  private playBubbleEntranceSound(speechType: SpeechType) {
    if (speechType === SpeechType.Thinks) {
      this.soundManager.safePlayMp3('ThoughtBubbleAppear');
    }
    else if (speechType === SpeechType.Says) {
      this.soundManager.safePlayMp3('SpeechBubbleAppear');
    }
  }

  private getPlayerXY(playerId: number) {
    let playerX: number;
    const screenBottom = 1080;
    const dungeonMasterX = 1700;
    const dungeonMasterY = 350;
    const inGameY = 270;

    let playerY: number;
    if (playerId === SpeechBubbleManager.dungeonMasterId) {
      playerX = dungeonMasterX;
      playerY = dungeonMasterY;
    }
    else if (playerId >= 0) { // Player... 
      playerX = this.iGetPlayerX.getPlayerX(this.iGetPlayerX.getPlayerIndex(playerId));
      playerY = screenBottom;
    }
    else {
      const inGameCreature: InGameCreature = this.iGetCreatureX.getInGameCreatureByIndex(-playerId);
      if (inGameCreature !== null)
        playerX = this.iGetCreatureX.getX(inGameCreature);

      playerY = inGameY;
    }
    return { playerX, playerY };
  }

  private getSpeechCommandParts(speechStr: string) {
    let spaceIndex = 0;
    let textToShow = null;
    let xOffset: number = 0;
    let playerId: number = Creature.invalidCreatureId;
    let speechType: SpeechType = SpeechType.None;
    let colorStr = '#000000';

    //console.log('speechStr: ' + speechStr);
    ({ speechStr, colorStr } = this.getColorStr(speechStr, colorStr));
    ({ speechStr, xOffset } = this.getXOffset(speechStr, xOffset));

    const firstColonPos: number = speechStr.indexOf(':');
    if (firstColonPos > 0) {
      const firstPart: string = speechStr.substr(0, firstColonPos);
      textToShow = speechStr.substr(firstColonPos + 1).trim();
      speechType = this.getSpeechType(firstPart);
      spaceIndex = firstPart.indexOf(' ');
      if (spaceIndex >= 0)
        playerId = +firstPart.substr(0, spaceIndex).trim();
    }
    return { playerId, speechType, textToShow, colorStr, xOffset };
  }

  private getColorStr(speechStr: string, colorStr: string) {
    const hashPos: number = speechStr.lastIndexOf('#');
    if (hashPos > 0) {
      const openParenPos: number = speechStr.indexOf('(', hashPos - 1);
      if (openParenPos > 0 && hashPos === openParenPos + 1) {
        const closeParenPos: number = speechStr.indexOf(')', hashPos);
        if (closeParenPos > openParenPos) {
          colorStr = speechStr.substring(openParenPos + 1, closeParenPos);
          if (!colorStr)
            colorStr = '#000000';
          //console.log('colorStr: ' + colorStr);
        }
        speechStr = speechStr.substring(0, openParenPos) + speechStr.substring(closeParenPos + 1);
        //console.log('speechStr: ' + speechStr);
      }
    }
    return { speechStr, colorStr };
  }

  private getXOffset(speechStr: string, xOffset: number) {

    const openParenPos: number = speechStr.lastIndexOf('(');
    let multiplier: number = 1;
    if (openParenPos > 0) {
      let signPos: number = speechStr.indexOf('+', openParenPos);
      if (signPos != openParenPos + 1) {
        signPos = speechStr.indexOf('-', openParenPos);
        if (signPos != openParenPos + 1) {
          multiplier = 0;
        }
        else {
          multiplier = -1;
        }
      }
      if (multiplier !== 0) {
        const closeParenPos: number = speechStr.indexOf(')', openParenPos);
        if (closeParenPos > openParenPos) {
          xOffset = +speechStr.substring(openParenPos + 1, closeParenPos);
          if (!xOffset)
            xOffset = 0;
        }
        speechStr = speechStr.substring(0, openParenPos) + speechStr.substring(closeParenPos + 1);
      }
    }

    return { speechStr, xOffset };
  }

  private getSpeechDataY(yPos: number, verticalTextOffsetBecauseNoDescenders: number, scaledPosOffsetY: number, textStartY: number, verticalScale: number, speechData: SpeechData): number {
    return yPos + verticalTextOffsetBecauseNoDescenders - scaledPosOffsetY - textStartY * verticalScale - speechData.paragraph.lineData.length * speechData.fontSize / 2 + speechData.fontSize / 2;
  }

  hideAnyBubblesBelongingToPlayer(playerId: number) {
    this.hideAnyBubbleSpritesBelongingToPlayer(this.speechBubbles, playerId);
    this.hideAnyBubbleSpritesBelongingToPlayer(this.thoughtBubbles, playerId);
  }

  hideAnyBubbleSpritesBelongingToPlayer(bubbleSprites: Sprites, playerId: number) {
    bubbleSprites.spriteProxies.forEach((sprite: SpriteProxy) => {
      const speechData: SpeechData = sprite.data as SpeechData;
      if (speechData && speechData.playerId === playerId)
        sprite.fadeOutNow(400);
    });
  }

  spriteAdvanceFrame(sprite: SpriteProxy, returnFrameIndex: number, reverse: boolean, now: number): void {
    const endLoopAnimationFrameIndex = 24;
    const speechData: SpeechData = sprite.data as SpeechData;
    if (speechData) {
      speechData.okayToDraw = sprite.frameIndex >= returnFrameIndex && sprite.frameIndex < endLoopAnimationFrameIndex;
      if (sprite.frameIndex === endLoopAnimationFrameIndex + 1)
        this.soundManager.safePlayMp3('Pop[4]');
    }
  }

  getSpeechType(firstPart: string): SpeechType {
    const lower: string = firstPart.toLowerCase();
    if (lower.indexOf('says') >= 0)
      return SpeechType.Says;
    else if (lower.indexOf('thinks') >= 0)
      return SpeechType.Thinks;
    else if (lower.indexOf('listening') >= 0)
      return SpeechType.Listening;
    else if (lower.indexOf('thinking') >= 0)
      return SpeechType.Thinking;
    else if (lower.indexOf('hidethoughts') >= 0)
      return SpeechType.HideThoughts;
  }

  draw(context: CanvasRenderingContext2D, nowMs: number) {
    this.thoughtBubbles.draw(context, nowMs);
    this.speechBubbles.draw(context, nowMs);
    this.thinkingManager.draw(context, nowMs);
    this.drawText(context, this.thoughtBubbles);
    this.drawText(context, this.speechBubbles);
  }

  drawSpriteText(context: CanvasRenderingContext2D, sprite: SpriteProxy) {
    context.textAlign = 'center';
    context.textBaseline = "middle";
    const speechData: SpeechData = sprite.data as SpeechData;
    if (speechData && speechData.okayToDraw) {
      this.wordRenderer.fontSize = speechData.fontSize;
      this.wordRenderer.setActiveStyle(context, LayoutStyle.normal);

      this.wordRenderer.textColor = speechData.textColor;
      this.wordRenderer.renderParagraphs(context, speechData.paragraph.lineData, speechData.x + speechData.width / 2, speechData.y + speechData.height / 2, SpeechData.styleDelimiters);
    }
  }

  drawText(context: CanvasRenderingContext2D, sprites: Sprites) {
    sprites.spriteProxies.forEach((sprite: SpriteProxy) => {
      this.drawSpriteText(context, sprite);
    });
  }
}