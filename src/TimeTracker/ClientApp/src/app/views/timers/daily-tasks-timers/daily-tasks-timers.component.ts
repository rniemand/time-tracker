import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { EditTimerEntryDialog, EditTimerEntryDialogData } from 'src/app/dialogs/edit-timer-entry/edit-timer-entry.dialog';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient, DailyTaskDto, DailyTasksClient, TimerDto, TimersClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-daily-tasks-timers',
  templateUrl: './daily-tasks-timers.component.html',
  styleUrls: ['./daily-tasks-timers.component.css']
})
export class DailyTasksTimersComponent implements OnInit {
  displayedColumns: string[] = ['state', 'startTime', 'endTime', 'length', 'notes', 'controls'];
  dataSource = new MatTableDataSource<TimerDto>();
  clientId: number = 0;
  taskId: number = 0;
  clientName: string = 'Client';
  taskName: string = 'Task';
  task?: DailyTaskDto = undefined;
  client?: ClientDto = undefined;

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private taskClient: DailyTasksClient,
    private clientClient: ClientsClient,
    private timerClient: TimersClient,
    public dialog: MatDialog
  ) { }


  ngOnInit(): void {
    this.taskId = this.route.snapshot.params?.taskId ?? 0;
    this.refreshView();
  }

  editEntry = (timer: TimerDto) => {
    let dialogData: EditTimerEntryDialogData = {
      timer: timer
    };

    let dialogRef = this.dialog.open(EditTimerEntryDialog, {
      ...DIALOG_DEFAULTS,
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if(result?.outcome == 'updated') {
        this.refreshTimers();
      }
    });
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
      if(this.taskId === 0) {
        resolve();
        return;
      }

      this.timerClient.getDailyTaskEntries(this.taskId).toPromise().then(
        (timers: TimerDto[]) => {
          this.dataSource = new MatTableDataSource(timers);
          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;

          resolve();
        }
      );
    });
  }

  private refreshTimers = () => {
    this.uiService.showLoader(true);
    this.loadTaskEntries().finally(() => { this.uiService.hideLoader(); });
  }

}
