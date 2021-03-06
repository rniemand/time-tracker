import { HumanTime } from "../services/utils.service";

export const isToday = (someDate: Date) => {
  const today = new Date()
  return someDate.getDate() == today.getDate() &&
    someDate.getMonth() == today.getMonth() &&
    someDate.getFullYear() == today.getFullYear()
}

export const toHumanTime = (seconds: number): HumanTime => {
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

export const shortTimeString = (seconds: number): string => {
  let mapped = toHumanTime(seconds);

  let hours = mapped.hours.toString().padStart(2, '0');
  let mins = mapped.mins.toString().padStart(2, '0');
  let secs = mapped.seconds.toString().padStart(2, '0');

  if(mapped.hours === 0)
    return `${mins}:${secs}`;

  return `${hours}:${mins}:${secs}`;
}

export const getBaseDate = (now?: Date, forceUtc: boolean = false) => {
  if(!now) { now = new Date(); }

  if(forceUtc) {
    return new Date(Date.UTC(now.getFullYear(), now.getMonth(), now.getDate()));
  }

  return new Date(now.getFullYear(), now.getMonth(), now.getDate());
}

export const getShortDateString = (date: Date) => {
  const yyyy = date.getFullYear();
  const mm = (date.getMonth() + 1).toString().padStart(2, '0');
  const dd = date.getDate().toString().padStart(2, '0');

  return `${yyyy}-${mm}-${dd}`;
}

export const getStartOfWeek = () => {
  const daysBack = 5 - (((new Date()).getDay() - 6) * -1);
  return new Date(new Date().getTime() - (daysBack * 24 * 60 * 60 * 1000));
}
