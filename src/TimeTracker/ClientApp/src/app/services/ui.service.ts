import { EventEmitter, Injectable } from "@angular/core";
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
  showLoaderChange = new EventEmitter<boolean>();
  loaderVisible: boolean = false;

  private _closeOnError: boolean = false;
  
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

  handleClientError = (error: any) => {
    // TODO: [COMPLETE] Complete me
    console.error(error);

    if(this._closeOnError) {
      this.hideLoader();
    }
  }

  showLoader(closeOnError: boolean = false) {
    // Address race condition
    setTimeout(() => {
      this._closeOnError = closeOnError;
      this.loaderVisible = true;
      this.showLoaderChange.emit(true);
    }, 1);
  }

  hideLoader() {
    this._closeOnError = false;
    this.loaderVisible = false;
    this.showLoaderChange.emit(false);
  }
}
