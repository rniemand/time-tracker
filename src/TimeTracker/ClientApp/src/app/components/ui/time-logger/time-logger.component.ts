import { EventEmitter } from '@angular/core';
import { Component, OnInit, Output, ViewChild } from '@angular/core';
import { ListTimersComponent } from './list-timers/list-timers.component';

export interface TimeLoggerEvent {
  type: string;
  source: string;
  data?: any;
}

@Component({
  selector: 'app-time-logger',
  templateUrl: './time-logger.component.html',
  styleUrls: ['./time-logger.component.css']
})
export class TimeLoggerComponent implements OnInit {
  @Output('onEvent') onEvent = new EventEmitter<TimeLoggerEvent>();
  @ViewChild('runningTimers', { static: true }) runningTimers!: ListTimersComponent;
  selectedTab: number = 2;

  constructor() { }

  ngOnInit(): void { }

  // Template methods
  handleEvent = (e: TimeLoggerEvent) => {
    if(e.type === 'timer.created') {
      this.runningTimers.refresh();
      this.selectedTab = 0;
    }

    this.onEvent.next(e);
  }

}
