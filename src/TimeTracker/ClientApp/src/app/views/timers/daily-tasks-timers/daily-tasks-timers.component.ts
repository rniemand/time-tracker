import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient, DailyTaskDto, DailyTasksClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-daily-tasks-timers',
  templateUrl: './daily-tasks-timers.component.html',
  styleUrls: ['./daily-tasks-timers.component.css']
})
export class DailyTasksTimersComponent implements OnInit {
  clientId: number = 0;
  taskId: number = 0;
  clientName: string = 'Client';
  taskName: string = 'Task';
  task?: DailyTaskDto = undefined;
  client?: ClientDto = undefined;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private taskClient: DailyTasksClient,
    private clientClient: ClientsClient
  ) { }

  ngOnInit(): void {
    this.taskId = this.route.snapshot.params?.taskId ?? 0;
    this.refreshView();
  }

  // Internal methods
  private refreshView = () => {
    this.uiService.showLoader(true);

    this.loadTaskInfo()
      .then(this.loadClientInfo)
      .then(this.loadTaskEntries)
      .finally(() => { this.uiService.hideLoader(); });
  }

  private loadTaskInfo = () => {
    return new Promise<void>((resolve) => {
      if(this.taskId === 0) {
        resolve();
        return;
      }

      this.taskClient.getTaskById(this.taskId).toPromise().then(
        (task: DailyTaskDto) => {
          this.clientId = task?.clientId ?? 0;
          this.task = task;
          this.taskName = task?.taskName ?? 'Task';

          resolve();
        }
      );
    });
  }

  private loadClientInfo = () => {
    return new Promise<void>((resolve) => {
      if(this.clientId === 0) {
        resolve();
        return;
      }

      this.clientClient.getClientById(this.clientId).toPromise().then(
        (client: ClientDto) => {
          this.clientName = client?.clientName ?? 'Client';
          this.client = client;
          resolve();
        }
      );
    });
  }

  private loadTaskEntries = () => {
    return new Promise<void>((resolve) => {

      resolve();
    });
  }

}
