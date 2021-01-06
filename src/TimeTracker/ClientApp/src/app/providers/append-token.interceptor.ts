import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpEventType } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { catchError, tap } from 'rxjs/operators';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { UiService, ValidationError } from '../services/ui.service';

// https://angular.io/guide/http#intercepting-requests-and-responses
// https://github.com/cornflourblue/angular-9-jwt-authentication-example

@Injectable()
export class AppendTokenInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let accessToken = this.authService.getAuthToken();
    
    if(!accessToken)
      return next.handle(request);

    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`
      }
    });

    return next.handle(request);
  }
}

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(
      private authService: AuthService,
      private uiService: UiService
    ) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if (err.status === 401) {
                // auto logout if 401 response returned from api
                this.authService.logout();
                location.reload();
            }

            this.handleValidationError(err);
            return throwError(err.error.message || err.statusText);
        }))
    }

    // Internal methods
    private parseJson = (json: any): any => {
      if(typeof(json) !== 'string')
        return null;

      if(!json.startsWith('{') || !json.endsWith('}'))
        return null;

      try {
        return JSON.parse(json);
      }
      catch(err) {
        return null;
      }
    }

    private isValidationError = (obj: any): boolean => {
      if(!obj)
        return false;

      if(
        !obj.hasOwnProperty('error') ||
        !obj.hasOwnProperty('errors') ||
        !obj.hasOwnProperty('isValid') ||
        !obj.hasOwnProperty('ruleSetsExecuted')
      ) {
        return false;
      }

      return true;
    }

    private handleValidationError = (err: any) => {
      if(!err.hasOwnProperty('error') || !(err.error instanceof Blob))
        return;
      
      let castErr = err.error as Blob;
      if(castErr.type != 'application/json')
        return;

      const reader = new FileReader();
      reader.addEventListener('loadend', (e) => {
        const text = e.target?.result?.toString() ?? '';
        let json = this.parseJson(text);
        if(this.isValidationError(json)) {
          this.uiService.handleValidationError(json as ValidationError);
        }
      });
      reader.readAsText(err.error);
    }
}



@Injectable()
export class SessionTokenInterceptor implements HttpInterceptor {
    constructor(
      private authService: AuthService
    ) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(tap(evt => {
          if(evt.type !== HttpEventType.Response)
            return;

          if(!evt.headers.has('x-tt-session'))
            return;

          this.authService.updateAuthToken(evt.headers.get('x-tt-session') ?? '');
        }));
    }
}

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private authService: AuthService,
        private _uiService: UiService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.authService.loggedIn) {
            // logged in so return true
            return true;
        }

        // not logged in so redirect to login page with the return url
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        this._uiService.notify('You need to log in to access this content', 3000);
        return false;
    }
}