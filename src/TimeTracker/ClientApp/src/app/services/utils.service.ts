import { Injectable } from "@angular/core";

export interface HumanTime {
  hours: number,
  mins: number,
  seconds: number
}

@Injectable()
export class UtilsService {

  public toHumanTime = (seconds: number): HumanTime => {
    let mapped: HumanTime = { hours: 0, mins: 0, seconds: 0 };

    if(seconds >= 3600) {
      mapped.hours = Math.floor(seconds / 3600);
      seconds -= (3600 * mapped.hours);
    }

    if(seconds >= 60) {
      mapped.mins = Math.floor(seconds / 60);
      seconds -= (60 * mapped.mins);
    }

    if(seconds < 60) {
      mapped.seconds = seconds;
    }

    return mapped;
  }

  public shortTimeString = (seconds: number): string => {
    let mapped = this.toHumanTime(seconds);

    let hours = mapped.hours.toString().padStart(2, '0');
    let mins = mapped.mins.toString().padStart(2, '0');
    let secs = mapped.seconds.toString().padStart(2, '0');

    if(mapped.hours === 0)
      return `${mins}:${secs}`;

    return `${hours}:${mins}:${secs}`;
  }
  
}
