import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { RawTimerDto, TimersClient } from 'src/app/time-tracker-api';
import { TimeLoggerEvent } from '../../ui/time-logger/time-logger.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  loggedIn: boolean = false;
  clientId: number = 0;
  productId: number = 0;
  projectId: number = 0;
  timer?: RawTimerDto;

  private subscriptions: Subscription[] = [];

  constructor(
    private authService: AuthService,
    private timersClient: TimersClient
  ) { }

  ngOnInit(): void {
    this.loggedIn = this.authService.loggedIn;


    this.timersClient.getRunningTimers().toPromise().then(
      (timers: RawTimerDto[]) => {
        if(timers.length > 0) {
          this.timer = timers[0];
        }
      },
      (error: any) => {
        console.error(error);
      }
    );

    this.subscriptions.push(this.authService.authChanged.subscribe(
      (loggedIn: boolean) => {
        this.loggedIn = loggedIn;
      }
    ));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub: Subscription) => {
      sub?.unsubscribe();
    });
  }

  // Template methods
  timeLoggerEvent = (e: TimeLoggerEvent) => {
    if(e.type === 'state_changed' && e.source == 'StartTimerComponent') {
      this.clientId = e.data?.clientId ?? 0;
      this.productId = e.data?.productId ?? 0;
      this.projectId = e.data?.projectId ?? 0;
    }
  }
}
