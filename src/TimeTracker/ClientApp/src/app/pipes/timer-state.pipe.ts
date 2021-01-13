import { Pipe, PipeTransform } from '@angular/core';
import { UtilsService } from '../services/utils.service';
import { TimerDto, TimerState } from '../time-tracker-api';

@Pipe({
  name: 'timerState'
})
export class TimerStatePipe implements PipeTransform {

  constructor(
    private utils: UtilsService
  ) { }

  transform(value: unknown, ...args: unknown[]): unknown {
    if(!value) return '(n/a)';

    if(value instanceof TimerDto)
      return this.getTimerState(value, ...args);

    return '(err)';
  }

  private getTimerState = (timer: TimerDto, ...args: unknown[]) => {
    if(args && args.length > 0 && typeof(args[0]) === 'string') {
      let cleanArg = args[0].toLowerCase().trim();

      if(cleanArg === 'running') {
        return (timer?.running ?? false) ? 'Running' : 'Stopped';
      }

      if(cleanArg === 'runningtime') {
        return this.getRunningTime(timer);
      }
      
      return `UNKNOWN (${cleanArg})`;
    }

    return TimerState[timer?.entryState ?? 0];
  }

  private getRunningTime = (timer: TimerDto) => {
    if((timer?.running ?? false) === true)
      return '...';

    return this.utils.shortTimeString(timer?.totalSeconds ?? 0);
  }

}
