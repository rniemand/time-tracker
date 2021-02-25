import { DOCUMENT, LocationStrategy } from "@angular/common";
import { NgModule } from "@angular/core";
import { FlexLayoutModule } from '@angular/flex-layout';
import { AuthService } from "../services/auth.service";
import { LoggerService } from "../services/logger.service";
import { StorageService } from "../services/storage.service";
import { UiService } from "../services/ui.service";
import { UtilsService } from "../services/utils.service";

import { API_BASE_URL, AuthClient, ClientsClient, DailyTasksClient, ProductsClient, ProjectsClient, TimersClient, TimeSheetClient } from "../time-tracker-api";

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

    // Services
    AuthService,
    StorageService,
    UiService,
    LoggerService,
    UtilsService,
    
    // Clients
    AuthClient,
    ClientsClient,
    ProductsClient,
    ProjectsClient,
    TimersClient,
    DailyTasksClient,
    TimeSheetClient
  ]
})
export class TimeTrackerModule {}