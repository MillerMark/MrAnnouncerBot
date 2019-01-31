class TimeSpan {
  static readonly zero: TimeSpan = TimeSpan.fromActions(0);
  static readonly infinity: TimeSpan = TimeSpan.fromActions(Infinity);

  constructor(public timeMeasure: TimeMeasure, public count: number) {

  }

  static fromActions(actionCount: number): TimeSpan {
    return new TimeSpan(TimeMeasure.actions, actionCount);
  }

  static fromSeconds(seconds: number): TimeSpan {
    return new TimeSpan(TimeMeasure.seconds, seconds);
  }

  static fromMinutes(minutes: number): TimeSpan {
    return new TimeSpan(TimeMeasure.seconds, minutes * 60);
  }

  static fromHours(hours: number): TimeSpan {
    return TimeSpan.fromMinutes(hours * 60);
  }
}