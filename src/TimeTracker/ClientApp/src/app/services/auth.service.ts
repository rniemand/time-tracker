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
      this._currentToken = this.storage.getItem<string>(KEY_TOKEN);
      this.loggedIn = true;
    }
  }

  login = (username: string, password: string) => {
    this.authClient.authenticate(new AuthenticationRequest({
      username: username,
      password: password
    })).toPromise().then(
      (response: AuthenticationResponse) => {
        console.log(response);

        if((response?.token?.length ?? 0) > 0) {
          this.storage.setItem(KEY_TOKEN, response.token);
          this.loggedIn = true;
        }
      },
      (error: any) => {
        console.error(error);
      }
    );
  }

  logout = () => {
    this.storage.removeItem(KEY_TOKEN);
  }

  getAuthToken = () => {
    if(!this.storage.hasItem(KEY_TOKEN))
      return null;

    return this.storage.getItem<string>(KEY_TOKEN);
  }
}
