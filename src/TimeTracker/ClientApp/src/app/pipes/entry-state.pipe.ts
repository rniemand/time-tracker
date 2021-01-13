import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'entryState'
})
export class EntryStatePipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    if(typeof(value) === 'number')
      return this.fromInt(value);

    return null;
  }

  // Internal methods
  private fromInt = (value: number) => {
    switch(value) {
      case 0: return '-';
      case 1: return 'Completed';
      case 2: return 'User Paused';
      case 3: return 'User Stopped';
      case 4: return 'Service Paused';
      case 5: return 'Cron Paused';
      default: return 'Unknown';
    }
  }

}
