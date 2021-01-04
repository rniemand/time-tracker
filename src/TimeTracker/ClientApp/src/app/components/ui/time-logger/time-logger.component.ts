import { Component, OnInit, ViewChild } from '@angular/core';
import { ListTimersComponent } from './list-timers/list-timers.component';

@Component({
  selector: 'app-time-logger',
  templateUrl: './time-logger.component.html',
  styleUrls: ['./time-logger.component.css']
})
export class TimeLoggerComponent implements OnInit {
  @ViewChild('runningTimers', { static: true }) runningTimers!: ListTimersComponent;

  constructor() { }

  ngOnInit(): void { }

  timerCreated = () => {
    this.runningTimers.refresh();
  }

}
