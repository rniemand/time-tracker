import { Injectable } from "@angular/core";
import { MatSnackBar, MatSnackBarHorizontalPosition, MatSnackBarVerticalPosition } from "@angular/material/snack-bar";

export interface NotifyOptions {
  message: string;
  action?: string;
  horizontalPosition?: MatSnackBarHorizontalPosition,
  verticalPosition?: MatSnackBarVerticalPosition,
  duration?: number
}

@Injectable()
export class UiService {
  constructor(
    private _snackBar: MatSnackBar
  ) { }
  
  notify = (options: NotifyOptions | string, duration?: number) => {
    let castOptions: NotifyOptions = { message: '' };

    if(typeof(options) === 'string') {
      castOptions.message = options;
    } else {
      castOptions = options;
    }

    if(typeof(duration) === 'number' && duration > 0) {
      castOptions.duration = duration;
    }

    this._snackBar.open(castOptions.message, castOptions.action, {
      duration: castOptions?.duration ?? 5 * 1000,
      horizontalPosition: castOptions?.horizontalPosition ?? 'right',
      verticalPosition: castOptions?.verticalPosition ?? 'bottom'
    });
  }
}
