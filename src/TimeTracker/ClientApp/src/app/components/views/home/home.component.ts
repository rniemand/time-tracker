import { Component, OnInit } from '@angular/core';
import { WeatherForecast, WeatherForecastClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private client: WeatherForecastClient) { }

  ngOnInit(): void {
    this.client.get().toPromise().then(
      (response: WeatherForecast[]) => {
        console.log(response);
      },
      (error: any) => {
        console.log(error);
      }
    );
  }

}
