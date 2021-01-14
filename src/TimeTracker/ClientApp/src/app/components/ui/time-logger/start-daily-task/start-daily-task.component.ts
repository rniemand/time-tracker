import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { TimerDto, TimersClient, TimerType } from 'src/app/time-tracker-api';
import { TimeLoggerEvent } from '../time-logger.component';

@Component({
  selector: 'app-start-daily-task',
  templateUrl: './start-daily-task.component.html',
  styleUrls: ['./start-daily-task.component.css']
})
export class StartDailyTaskComponent implements OnInit {
  @Output('onEvent') onEvent = new EventEmitter<TimeLoggerEvent>();
  clientId: number = 0;
  taskId: number = 0;
  isValid: boolean = false;

  constructor(
    private authService: AuthService,
    private timersClient: TimersClient,
    private uiService: UiService
  ) { }

  ngOnInit(): void {
  }

  startTask = () => {
    let timerDto = new TimerDto({
      'clientId': this.clientId,
      'userId': this.authService.currentUser?.id ?? 0,
      'entryType': TimerType.DailyTask,
      'taskId': this.taskId
    });

    this.timersClient.startNew(timerDto).toPromise().then(
      (success: boolean) => {
        if(!success) return;
        this.onEvent.emit({
          type: 'timer.created',
          source: 'StartDailyTaskComponent'
        });
      },
      this.uiService.handleClientError
    );
  }

  clientChanged = () => {
    this.taskId = 0;
    this.isValid = this.taskId > 0;
  }

  taskChanged= () => {
    this.isValid = this.taskId > 0;
  }

}
