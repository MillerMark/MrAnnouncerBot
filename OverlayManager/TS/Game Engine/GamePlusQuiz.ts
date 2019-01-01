class GamePlusQuiz extends Game {
    constructor(context: CanvasRenderingContext2D) {
    super(context);
  }

  removeAllGameElements(now: number): void {
    super.removeAllGameElements(now);
    quiz = null;
  }

  updateForeground(context: CanvasRenderingContext2D, now: number) {
    super.updateForeground(context, now)
    if (quiz)
      quiz.draw(myContext);
  }

  initialize() {
    super.initialize();
  }

  test(testCommand: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    return super.test(testCommand, userId, userName, displayName, color, now);
  }

  executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string, now: number): boolean {
    if (super.executeCommand(command, params, userId, userName, displayName, color, now))
      return true;

    if (command === "StartQuiz") {
      this.startQuiz(now, params, userId, userName);
      return true;
    }
    if (command === "ShowLastQuizResults") {
      this.showLastQuizResults(now, params);
      return true;
    }
    if (command === "AnswerQuiz") {
      this.answerQuiz(params, userId);
      return true;
    }
    if (command === "SilentAnswerQuiz") {
      this.silentAnswerQuiz(params, userId, userName);
      return true;
    }
    if (command === "ClearQuiz") {
      this.clearQuiz(userName);
      return true;
    }

    // TODO: Support !vote x

    return false;

  }

  isSuperUser(userName: string) {
    return userName === "coderushed" || userName === "rorybeckercoderush";
  }

  clearQuiz(userName: string) {
    if (!this.isSuperUser(userName)) {
      chat('Only Rory and Mark can clear a quiz.');
      return;
    }
    if (quiz)
      quiz = null;
  }

  answerQuiz(choice: string, userId: string) {
    if (quiz)
      quiz.vote(userId, choice);
  }

  silentAnswerQuiz(choice: string, userId: string, userName: string) {
    if (!quiz)
      return;
    if (quiz.getChoiceIndex(choice) < 0) {
      whisper(userName, 'We could not find that choice "' + choice + '".');
      return;
    }

    quiz.vote(userId, choice);
    whisper(userName, 'Your vote has been recorded. Nice job participating in democracy!');
  }

  // params are expected to be in the form of "!quiz What would you rather be?, 1. Bee, 2. Drone"
  startQuiz(now: number, cmd: string, userId: string, userName: string) {
    if (userName != "coderushed" && userName != "rorybeckercoderush") {
      chat('Only Rory and Mark can start a poll.');
      return;
    }

    let lines: Array<string> = cmd.split(',');
    let choices: Array<string> = lines.slice(1);
    if (choices.length < 2) {
      chat('Polls must include at least two comma-separated choices.');
      return;
    }
    const question = lines[0];
    new Quiz(question, choices);

    chat(`Polls are open - ${lines[0]}`);
    for (var i = 0; i < choices.length; i++) {
      chat(choices[i]);
    }
    chat('Enter your choice here in the chat window or in a separate DM whisper to me.');
  }

  showLastQuizResults(now: number, params: string) {
    // TODO: Implement this!
  }
}