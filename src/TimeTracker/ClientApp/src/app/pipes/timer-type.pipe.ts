import { Pipe, PipeTransform } from '@angular/core';
import { TimerDto, TimerType } from '../time-tracker-api';

@Pipe({
  name: 'timerType'
})
export class TimerTypePipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    if(value instanceof TimerDto) {
      return this.workType(value);
    }

    return '(unknown)';
  }

  private workType = (timer: TimerDto) => {
    return TimerType[timer?.entryType ?? 0];
  }

}
