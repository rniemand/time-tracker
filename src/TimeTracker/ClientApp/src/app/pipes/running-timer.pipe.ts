import { Pipe, PipeTransform } from '@angular/core';
import { TimerEndReason, TrackedTimeDto } from '../time-tracker-api';

interface MappedTime {
  hours: string,
  mins: string,
  seconds: string
}

@Pipe({
  name: 'runningTimer'
})
export class RunningTimerPipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    if(typeof(value) === 'number')
      return this.toHumanTime(value);

    if(value instanceof TrackedTimeDto)
      return this.handleTimerDto(value);

    if(!value || !(value instanceof Date))
      return '00:00:00';

    if(typeof(args[0]) === 'number' && args[0] > 0) {
      return this.toHumanTime(args[0]);
    }

    let runningSeconds = Math.floor(((new Date()).getTime() - (value as Date).getTime()) / 1000);
    return this.toHumanTime(runningSeconds);
  }

  private toHumanTime = (seconds: number) => {
    if(seconds == 0)
      return '-';

    let mapped: MappedTime = { hours: '00', mins: '00', seconds: '00' };

    if(seconds >= 3600) {
      let hours = Math.floor(seconds / 3600);
      mapped.hours = hours.toString();
      seconds -= (3600 * hours);
    }

    if(seconds >= 60) {
      let mins = Math.floor(seconds / 60);
      mapped.mins = mins.toString().padStart(2, '0');
      seconds -= (60 * mins);
    }

    if(seconds < 60) {
      mapped.seconds = seconds.toString().padStart(2, '0');
    }

    if(mapped.hours === '00')
      return `${mapped.mins}:${mapped.seconds}`;

    return `${mapped.hours}:${mapped.mins}:${mapped.seconds}`;
  }

  private fromStartDate = (startDate: Date) => {
    let runningSeconds = Math.floor(((new Date()).getTime() - (startDate as Date).getTime()) / 1000);
    return this.toHumanTime(runningSeconds);
  }

  private workEntryState = (timer: TrackedTimeDto) => {
    switch(timer?.endReason ?? 0) {
      case 0: return 'unknown';
      case 1: return 'completed';
      case 2: return 'user-paused';
      case 3: return 'user-stopped';
      case 4: return 'service-paused';
      case 5: return 'cron-paused';
      default: return 'N/A';
    }
  }

  private handleTimerDto = (timer: TrackedTimeDto) => {
    if((timer?.running ?? false) === true) {
      if(!timer?.startTimeUtc) return '!';
      return this.fromStartDate(timer?.startTimeUtc);
    }

    let endReason: TimerEndReason = timer?.endReason ?? TimerEndReason.Unknown;
    if(endReason !== TimerEndReason.Unknown) {
      return `${this.workEntryState(timer)} (${this.toHumanTime(timer?.totalSeconds ?? 0)})`;
    }

    return 'ERR';
  }
}
