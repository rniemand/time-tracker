import { Component, OnInit } from '@angular/core';
import { UiService } from 'src/app/services/ui.service';
import { RawTrackedTimeDto, TrackedTimeClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-list-timers',
  templateUrl: './list-timers.component.html',
  styleUrls: ['./list-timers.component.css']
})
export class ListTimersComponent implements OnInit {
  timers: RawTrackedTimeDto[] = [];

  constructor(
    private trackedTimeClient: TrackedTimeClient,
    private uiService: UiService
  ) { }

  ngOnInit(): void {
    this.timers = [];

    this.trackedTimeClient.getRunningTimers().toPromise().then(
      (timers: RawTrackedTimeDto[]) => {
        this.timers = timers;
      },
      this.uiService.handleClientError
    );
  }

}
