import { Subject } from "rxjs";
import { Injectable } from "@angular/core";
import { AuthClient, AuthenticationRequest, AuthenticationResponse } from "../time-tracker-api";
import { StorageService } from "./storage.service";

const KEY_TOKEN = 'user.token';

@Injectable()
export class AuthService {
  authChanged = new Subject<boolean>();
  loggedIn: boolean = false;

  private _currentToken: string = '';

  constructor(
    private authClient: AuthClient,
    private storage: StorageService
  ) {
    // Check to see if we are logged in
    if(this.storage.hasItem(KEY_TOKEN)) {
      // TODO: [VALIDATION] Add some form of token validation here
      this.setLoggedInSate(true, this.storage.getItem<string>(KEY_TOKEN));
    }
  }

  login = (username: string, password: string) => {
    let authRequest = new AuthenticationRequest({
      username: username,
      password: password
    });

    return new Promise<boolean>((resolve, reject) => {
      this.authClient.authenticate(authRequest).toPromise().then(
        (response: AuthenticationResponse) => {
          if((response?.token?.length ?? 0) > 0) {
            this.setLoggedInSate(true, response.token);
          } else {
            this.setLoggedInSate(false);
          }

          resolve(this.loggedIn);
        },
        (error: any) => {
          console.error(error);

          this.setLoggedInSate(false);
          resolve(false);
        }
      );
    });
  }

  logout = () => {
    this.setLoggedInSate(false);
  }

  getAuthToken = () => {
    if(this._currentToken.length < 1)
      return null;
    
    return this._currentToken;
  }

  // Internal methods
  private setLoggedInSate = (loggedIn: boolean, token?: string) => {
    this.loggedIn = loggedIn;

    if(this.loggedIn === false) {
      this._currentToken = '';
      
      if(this.storage.hasItem(KEY_TOKEN)) {
        this.storage.removeItem(KEY_TOKEN);
      }
    }

    if(typeof(token) === 'string' && token.length > 0) {
      this._currentToken = token;
      this.storage.setItem(KEY_TOKEN, token);
    }

    this.authChanged.next(this.loggedIn);
  }
}
