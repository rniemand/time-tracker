import { Pipe, PipeTransform } from '@angular/core';

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
      mapped.hours = hours.toString().padStart(2, '0');
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

}
