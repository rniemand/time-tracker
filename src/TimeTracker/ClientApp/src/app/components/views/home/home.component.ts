import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { AuthClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(
    private authService: AuthService,
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

    // this.authClient.authenticate(new AuthenticationRequest({
    //   username: 'niemandr',
    //   password: 'password'
    // })).toPromise().then(
    //   (response: AuthenticationResponse) => {
    //     console.log(response?.token);
    //   },
    //   (error: any) => {
    //     console.error(error);
    //   }
    // );

    this.authService.login('niemandr', 'password');
  }

}
