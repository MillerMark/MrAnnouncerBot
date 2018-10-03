var quiz: Quiz;

const pollTime: number = 40;
const panicTime: number = 10;
const reviewTime: number = 16;
const fadeOutTime: number = 1;

const quizTop: number = 88;
const quizRight: number = 1915;

class QuizResult {
  constructor(public choice: string, public percent: number) {

  }
}

class Quiz {
  votes: Array<Vote>;
  colors: Array<string>;
  results: Array<number>;
  startTime: number;
  countdownTimer: Digits = new Digits(DigitSize.small, quizRight, quizTop);
  votesChanged: boolean;
  lowerUserChoice: string;
  panicMessageSent: boolean;
  pollClosedMessageSent: boolean;

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
    this.panicMessageSent = false;
    this.pollClosedMessageSent = false;
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

  compareQuizResults(a: QuizResult, b: QuizResult): number {
    return Math.sign(b.percent - a.percent);
  }

  draw(context: CanvasRenderingContext2D) {
    const panicCutoff: number = pollTime - panicTime;
    const totalUpTime: number = pollTime + reviewTime;

    let now = performance.now();
    let secondsPassed: number = (now - this.startTime) / 1000;
    if (secondsPassed > totalUpTime) {
      quiz = null;
      return;
    }

    if (secondsPassed > pollTime && !this.pollClosedMessageSent) {
      this.pollClosedMessageSent = true;
      let totalVotes: number = 0;
      this.results.forEach(function (result) { totalVotes += result; });

      if (totalVotes > 0) {
        this.showPollResults(totalVotes);
      }
    }

    if (secondsPassed > panicCutoff && !this.panicMessageSent) {
      this.panicMessageSent = true;
      chat(`Polls are closing in ${panicTime} seconds!`);
    }

    if (secondsPassed > totalUpTime - fadeOutTime) {
      var secondsFromEnd: number = totalUpTime - secondsPassed;
      context.globalAlpha = secondsFromEnd / fadeOutTime;
    }

    if (this.votesChanged)
      this.tabulateNewResults();
    const x: number = 1404;
    const width: number = 511;
    const height: number = 431;

    if (secondsPassed >= pollTime)
      context.fillStyle = '#fefeda';  // Poll finished background color.
    else if (secondsPassed > panicCutoff)
      context.fillStyle = '#e5d0cf';  // Time to panic - poll is almost closing.
    else
      context.fillStyle = '#cfd6e5';  // Normal poll background color.
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
    context.textAlign = "left";
    context.fillText(this.mainQuestion, x + leftMargin, quizTop + topMargin);
    const verticalResultTopMargin: number = 20;
    const verticalResultBottomMargin: number = 10;
    const verticalInnerMargin: number = 20;
    const barHorzMargin: number = 10;
    const barTextLeft: number = 5;
    const choiceTextBarMargin: number = 5;
    let voteTop: number = quizTop + questionFontSize + verticalResultTopMargin;
    let resultHeight: number = height - questionFontSize - verticalResultTopMargin - verticalResultBottomMargin;
    let availableBarHeight: number = resultHeight - (this.choices.length - 1) * verticalInnerMargin;
    let choiceHeight: number = availableBarHeight / this.choices.length;
    let barHeight: number = choiceHeight - choiceFontSize - choiceTextBarMargin;

    var voteWidth: number = this.getVoteWidth(width - barHorzMargin * 2);
    var self = this;
    var resultIndex: number = 0;
    this.choices.forEach(function (item) {
      context.fillStyle = self.colors[resultIndex];
      context.font = choiceFontSize + 'px Arial';
      context.textAlign = "left";
      context.fillText(item, x + barTextLeft, voteTop);
      var numVotes: number = this.results[resultIndex];
      var barTop: number = voteTop + choiceFontSize + choiceTextBarMargin;
      context.fillRect(x + barHorzMargin, barTop, numVotes * voteWidth, barHeight);

      var lineX: number = x + barHorzMargin + voteWidth;
      if (numVotes > 1) {
        var saveGlobalAlpha: number = context.globalAlpha;
        context.globalAlpha = Math.max(0, saveGlobalAlpha * 0.3);
        context.beginPath();

        for (var i = 0; i < numVotes - 1; i++) {
          context.moveTo(lineX, barTop);
          context.lineTo(lineX, barTop + barHeight);
          lineX += voteWidth;
        }

        context.lineWidth = 1;
        context.strokeStyle = '#fff';
        context.stroke();
        context.globalAlpha = saveGlobalAlpha;
      }

      if (numVotes != 0) {
        const voteResultFontSize: number = Math.round(choiceFontSize * 0.8);
        context.font = voteResultFontSize + 'px Arial';
        context.fillStyle = '#fff';
        const textMargin: number = 5;
        context.textAlign = "right";
        context.fillText(numVotes.toString(), x + barHorzMargin + numVotes * voteWidth - textMargin, barTop + textMargin);
      }

      voteTop += choiceHeight + verticalInnerMargin;
      resultIndex++;
      if (resultIndex >= self.colors.length)
        resultIndex = 0;
    }, this);

    if (secondsPassed < pollTime) {
      this.countdownTimer.value = Math.round(pollTime - secondsPassed);
      this.countdownTimer.draw(context);
    }
    else

      context.globalAlpha = 1;
  }

  showPollResults(totalVotes: number): void {
    chat(`--------------------------`);
    if (totalVotes === 1)
      chat(`Polls are closed with 1 vote recorded. Results:`);
    else
      chat(`Polls are closed with ${totalVotes} votes recorded. Results:`);
    let quizResults: Array<QuizResult> = new Array<QuizResult>();
    var resultIndex: number = 0;
    this.choices.forEach(function (choice) {
      var answer: string = choice.trim();
      if (choice.length > 0) {
        let firstChar: string = answer.charAt(0);
        if (firstChar === '1' ||
          firstChar === '2' ||
          firstChar === '3' ||
          firstChar === '4' ||
          firstChar === '5' ||
          firstChar === '6' ||
          firstChar === '7' ||
          firstChar === '8' ||
          firstChar === '9') {

          answer = answer.substr(1);

          if (answer.length > 0) {
            let firstChar: string = answer.charAt(0);
            if (firstChar === '.') {
              answer = answer.substr(1).trim();
            }
          }
        }
      }
      quizResults.push(new QuizResult(answer, this.results[resultIndex] / totalVotes));
      resultIndex++;
    }, this);

    quizResults.sort(this.compareQuizResults);

    quizResults.forEach(function (result) {
      chat(result.choice + ': ' + Math.round(result.percent * 10000) / 100 + '%');
    });
  }


  getVoteWidth(width: number): any {
    var maxVotes: number = 0;
    this.results.forEach(function (result) {
      maxVotes = Math.max(maxVotes, result);
    });

    if (maxVotes < 5)
      maxVotes = 5;

    return width / maxVotes;
  }

  clearResults(): any {
    this.results = new Array<number>(this.choices.length);
    for (var i = 0; i < this.results.length; i++) {
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

  choiceMatches(choice: string): boolean {
    return choice.trim().substring(0, this.lowerUserChoice.length).toLowerCase() === this.lowerUserChoice;
  }

  getChoiceIndex(userChoice: string): number {
    this.lowerUserChoice = userChoice.toLowerCase();
    return this.choices.findIndex(this.choiceMatches, this);
  }
}
