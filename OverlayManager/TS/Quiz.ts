var quiz: Quiz;

class Quiz {
  votes: Array<Vote>;
  colors: Array<string>;
  startTime: number;

  constructor(private mainQuestion: string, private choices: Array<string>) {
    this.startTime = performance.now();
    quiz = this;
    this.colors.push('#3069b2'); // ![](D79731789B2110AD05A22E06227B155A.png)
    this.colors.push('#914f7f'); // ![](EC4FB44F8F07276C6D21EFDD1E687B39.png)
  }

  vote(userId: string, vote: string) {
    var existingVote: Vote = this.getVote(userId);
    if (existingVote)
      existingVote.vote = vote;
    else
      this.votes.push(new Vote(userId, vote));
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
    const x: number = 1404;
    const y: number = 88;
    const width: number = 511;
    const height: number = 431;
    context.fillStyle = '#cfd6e5';
    const margin: number = 10;
    context.textBaseline = 'top';
    context.fillRect(x, y, width, height);
    const questionFontSize: number = 30;
    const choiceFontSize: number = 24;
    context.font = questionFontSize + 'px Arial';
    context.fillStyle = '#000';
    context.fillText(this.mainQuestion, x + margin, y + margin);
    const verticalResultTopMargin: number = 20;
    const verticalResultBottomMargin: number = 10;
    const verticalInnerMargin: number = 20;
    const barLeft: number = 10;
    const choiceTextBarMargin: number = 10;
    let barTop: number = y + questionFontSize + verticalResultTopMargin;
    let resultHeight: number = height - questionFontSize - verticalResultTopMargin - verticalResultBottomMargin;
    let availableBarHeight: number = resultHeight - (this.choices.length - 1) * verticalInnerMargin;
    let choiceHeight: number = availableBarHeight / this.choices.length;
    let barHeight: number = choiceHeight - choiceFontSize - choiceTextBarMargin;

    context.font = choiceFontSize + 'px Arial';

    this.choices.forEach(function (item) {
      context.fillText(item, x, barTop);
      context.fillRect(x + barLeft, barTop + choiceFontSize + choiceTextBarMargin, 100, barHeight);
      barTop += choiceHeight + verticalInnerMargin;
    });
  }
}