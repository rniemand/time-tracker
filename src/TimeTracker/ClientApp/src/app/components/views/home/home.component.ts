import { Component, OnInit } from '@angular/core';
import { AuthClient, AuthenticationRequest, AuthenticationResponse } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(
    private authClient: AuthClient
  ) { }

  ngOnInit(): void {
    // this.client.get().toPromise().then(
    //   (response: WeatherForecast[]) => {
    //     console.log(response);
    //   },
    //   (error: any) => {
    //     console.log(error);
    //   }
    // );

    this.authClient.authenticate(new AuthenticationRequest({
      username: 'niemandr',
      password: 'password'
    })).toPromise().then(
      (response: AuthenticationResponse) => {
        console.log(response);
      },
      (error: any) => {
        console.error(error);
      }
    );
  }

}
