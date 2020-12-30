import { Subject } from "rxjs";
import { Injectable } from "@angular/core";
import { AuthClient, AuthenticationRequest, AuthenticationResponse } from "../time-tracker-api";
import { StorageService } from "./storage.service";
import { Router } from "@angular/router";
import { UiService } from "./ui.service";

const KEY_TOKEN = 'user.token';
const KEY_USER_INFO = 'user.info';

export interface UserInfo {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
}

@Injectable()
export class AuthService {
  authChanged = new Subject<boolean>();
  loggedIn: boolean = false;
  currentUser?: UserInfo;

  private _currentToken: string = '';

  constructor(
    private authClient: AuthClient,
    private storage: StorageService,
    private router: Router,
    private uiService: UiService
  ) {
    if(this.storage.hasItem(KEY_USER_INFO)) {
      this.currentUser = this.storage.getItem<UserInfo>(KEY_USER_INFO);
    }
    
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
          this.processAuthResponse(response);
          resolve(this.loggedIn);
        },
        (error: any) => {
          this.uiService.handleClientError(error);
          this.setLoggedInSate(false);
          resolve(false);
        }
      );
    });
  }

  logout = () => {
    this.setLoggedInSate(false);
    this.router.navigate(['/']);
    this.uiService.notify('Logged out', 1500);
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

  private updateCurrentUser = (response: AuthenticationResponse) => {
    let loggedIn = (response?.userId ?? 0) > 0;
    this.currentUser = undefined;

    if(!loggedIn) {
      if(this.storage.hasItem(KEY_USER_INFO)) {
        this.storage.removeItem(KEY_USER_INFO);
      }
      return;
    }
    
    this.currentUser = {
      id: response?.userId ?? 0,
      username: response?.username ?? '',
      firstName: response?.firstName ?? '',
      lastName: response?.lastName ?? ''
    };

    this.storage.setItem(KEY_USER_INFO, this.currentUser);
  }

  private processAuthResponse = (response: AuthenticationResponse) => {
    this.updateCurrentUser(response);

    if((response?.token?.length ?? 0) > 0) {
      this.setLoggedInSate(true, response.token);
    } else {
      this.setLoggedInSate(false);
    }
  }
}
