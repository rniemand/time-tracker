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
      case 1: return 'Running';
      case 2: return 'Paused';
      case 3: return 'Completed';
      default: return 'Unknown';
    }
  }

}
