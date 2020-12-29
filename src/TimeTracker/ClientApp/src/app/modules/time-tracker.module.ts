import { DOCUMENT, LocationStrategy } from "@angular/common";
import { NgModule } from "@angular/core";
import { FlexLayoutModule } from '@angular/flex-layout';

import { API_BASE_URL, AuthClient, WeatherForecastClient } from "../time-tracker-api";

export function getBaseUrl(locationStrategy: LocationStrategy, document: any): string {
  let baseHref = locationStrategy.getBaseHref();

  if(baseHref === '/')
    return '';

  let baseUrl = `${document.location.origin}${locationStrategy.getBaseHref()}`;

  if(baseUrl.endsWith('/') == false)
    return baseUrl;

  return baseUrl.substring(0, baseUrl.length -1);
}

@NgModule({
  declarations: [],
  imports: [
    FlexLayoutModule
  ],
  exports: [
    FlexLayoutModule
  ],
  providers: [
    { provide: API_BASE_URL, useFactory: getBaseUrl, deps: [LocationStrategy, DOCUMENT] },
    
    WeatherForecastClient,
    AuthClient
  ]
})
export class TimeTrackerModule {}