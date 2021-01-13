import { Pipe, PipeTransform } from '@angular/core';
import { UtilsService } from '../services/utils.service';
import { TimerDto, TimerState } from '../time-tracker-api';

@Pipe({
  name: 'pausedTimer'
})
export class PausedTimerPipe implements PipeTransform {

  constructor(
    private utils: UtilsService
  ) { }

  transform(value: unknown, ...args: unknown[]): unknown {
    if(!value || !(value instanceof TimerDto))
      return '-';

    return this.workDisplayString(value);
  }

  private workDisplayString = (timer: TimerDto) => {
    if(timer.running) {
      return 'use runningTimer pipe';
    }

    let stateString = TimerState[timer?.entryState ?? 0];
    let runningTime = this.utils.shortTimeString(timer?.totalSeconds ?? 0);
    return `${stateString} (${runningTime})`;
  }
}
