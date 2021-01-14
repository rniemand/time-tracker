import { Injectable } from "@angular/core";
import { StorageService } from "./storage.service";

const KEY_LOGGER_ENABLED = 'dev.logger.enabled';
// localStorage.setItem('dev.logger.enabled', 'true')
// localStorage.removeItem('dev.logger.enabled')

@Injectable()
export class LoggerService {
  private enabled: boolean = false;

  constructor(private storage: StorageService) {
    if(this.storage.hasItem(KEY_LOGGER_ENABLED)) {
      this.enabled = true;
    }
  }

  trace = (message: string) => {
    if(!this.enabled)
      return;

    console.log(`[TRACE] ${message}`);
  }

  warn = (message: string) => {
    if(!this.enabled)
      return;
    
    console.warn(message);
  }
}