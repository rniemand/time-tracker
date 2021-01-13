import { Pipe, PipeTransform } from '@angular/core';
import { UtilsService } from '../services/utils.service';
import { TimerDto } from '../time-tracker-api';

@Pipe({
  name: 'runningTimer'
})
export class RunningTimerPipe implements PipeTransform {

  constructor(
    private utils: UtilsService
  ) {}

  transform(value: unknown, ...args: unknown[]): unknown {
    if(!value || !(value instanceof TimerDto))
      return '-';
      
    return this.workDisplayString(value);
  }

  private workDisplayString = (timer: TimerDto) => {
    if(!timer?.running) {
      return 'use pausedTimer pipe';
    }

    if(!timer?.startTimeUtc) {
      return '-';
    }

    let runningSeconds = Math.floor(((new Date()).getTime() - timer.startTimeUtc.getTime()) / 1000);
    return this.utils.shortTimeString(runningSeconds);
  }
}
