import { NgModule } from "@angular/core";
import { FlexLayoutModule } from '@angular/flex-layout';

import { WeatherForecastClient } from "../time-tracker-api";

@NgModule({
  declarations: [],
  imports: [
    FlexLayoutModule
  ],
  exports: [
    FlexLayoutModule
  ],
  providers: [
    WeatherForecastClient
  ]
})
export class TimeTrackerModule {}