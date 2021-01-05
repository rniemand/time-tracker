import { Injectable } from "@angular/core";

@Injectable()
export class LoggerService {
  trace = (message: string) => {
    console.log(`[TRACE] ${message}`);
  }
}