import { Injectable } from "@angular/core";

@Injectable()
export class StorageService {
  setItem = <T>(key: string, value: T): void => {
    localStorage.setItem(key, JSON.stringify(value));
  }

  getItem = <TOut>(key: string): TOut => {
    let rawValue = localStorage.getItem(key);
    return JSON.parse(rawValue ?? '') as TOut;
  }

  removeItem(key: string): void {
    localStorage.removeItem(key);
  }

  hasItem(key: string): boolean {
    return localStorage.hasOwnProperty(key);
  }
}