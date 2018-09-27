var quiz: Quiz;

const quizTop: number = 88;
const quizRight: number = 1915;

class Quiz {
  votes: Array<Vote>;
  colors: Array<string>;
  results: Array<number>;
  startTime: number;
  countdownTimer: Digits = new Digits(DigitSize.small, quizRight, quizTop);
  votesChanged: boolean;

  constructor(private mainQuestion: string, private choices: Array<string>) {
    this.startTime = performance.now();
    this.votes = new Array<Vote>();
    this.clearResults();

    quiz = this;
    this.colors = new Array<string>();
    this.colors.push('#3069b2'); // ![](D79731789B2110AD05A22E06227B155A.png)
    this.colors.push('#914f7f'); // ![](EC4FB44F8F07276C6D21EFDD1E687B39.png)
    this.colors.push('#347070'); // ![](22E624C8FE1865DBED041108AC1230CC.png)
    this.colors.push('#9b6565'); // ![](68936B4B9356D4CF499EF690B9FD81E9.png)
    this.colors.push('#826aa6'); // ![](B5142450560091386AD03C978AE461CC.png)
    this.colors.push('#766a54'); // ![](E8D0F5D2182624D03E8869EF9C13E6DA.png)
    this.votesChanged = false;
  }

  vote(userId: string, choice: string) {
    var existingVote: Vote = this.getVote(userId);
    if (existingVote)
      existingVote.choice = choice;
    else
      this.votes.push(new Vote(userId, choice));
    this.votesChanged = true;
  }

  getVote(userId: string): Vote {
    for (var i = 0; i < this.votes.length; i++) {
      var thisVote: Vote = this.votes[i];
      if (thisVote.userId === userId)
        return thisVote;
    }
    return null;
  }

  draw(context: CanvasRenderingContext2D) {
    let now = performance.now();
    let secondsPassed: number = (now - this.startTime) / 1000;
    if (secondsPassed > 60)
      return;

    if (this.votesChanged)
      this.tabulateNewResults();
    const x: number = 1404;
    const width: number = 511;
    const height: number = 431;
    context.fillStyle = '#cfd6e5';
    const leftMargin: number = 10;
    const topMargin: number = 6;
    context.textBaseline = 'top';
    context.fillRect(x, quizTop, width, height);
    const questionFontSize: number = 30;
    const choiceFontSizeBig: number = 24;
    const choiceFontSizeSmall: number = 18;
    let choiceFontSize: number = choiceFontSizeBig;
    if (this.choices.length > 4)
      choiceFontSize = choiceFontSizeSmall;
    context.font = questionFontSize + 'px Arial';
    context.fillStyle = '#000';
    context.fillText(this.mainQuestion, x + leftMargin, quizTop + topMargin);
    const verticalResultTopMargin: number = 20;
    const verticalResultBottomMargin: number = 10;
    const verticalInnerMargin: number = 20;
    const barLeft: number = 10;
    const barTextLeft: number = 5;
    const choiceTextBarMargin: number = 5;
    let barTop: number = quizTop + questionFontSize + verticalResultTopMargin;
    let resultHeight: number = height - questionFontSize - verticalResultTopMargin - verticalResultBottomMargin;
    let availableBarHeight: number = resultHeight - (this.choices.length - 1) * verticalInnerMargin;
    let choiceHeight: number = availableBarHeight / this.choices.length;
    let barHeight: number = choiceHeight - choiceFontSize - choiceTextBarMargin;

    context.font = choiceFontSize + 'px Arial';

    var self = this;
    var resultIndex: number = 0;
    this.choices.forEach(function (item) {
      context.fillStyle = self.colors[resultIndex];
      context.fillText(item, x + barTextLeft, barTop);
      context.fillRect(x + barLeft, barTop + choiceFontSize + choiceTextBarMargin, this.results[resultIndex] * 10, barHeight);
      barTop += choiceHeight + verticalInnerMargin;
      resultIndex++;
      if (resultIndex >= self.colors.length)
        resultIndex = 0;
    }, this);

    this.countdownTimer.value = Math.round(60 - secondsPassed);
    this.countdownTimer.draw(context);
  }

  clearResults(): any {
    this.results = new Array<number>(this.choices.length);
    for (var i = 0; i < this.choices.length; i++) {
      this.results[i] = 0;
    }
  }

  tabulateNewResults(): any {
    this.clearResults();
    this.votes.forEach(function (vote) {
      var resultIndex: number = 0;
      this.choices.forEach(function (choice) {
        if (choice.trim().substring(0, vote.choice.length).toLowerCase() === vote.choice.toLowerCase()) {
          this.results[resultIndex]++;
          return;
        }
        else
          resultIndex++;
      }, this);
    }, this);
  }
}