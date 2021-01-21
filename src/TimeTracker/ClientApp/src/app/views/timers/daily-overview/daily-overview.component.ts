import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { EditTimerEntryDialog, EditTimerEntryDialogData } from 'src/app/dialogs/edit-timer-entry/edit-timer-entry.dialog';
import { UiService } from 'src/app/services/ui.service';
import { TimerDto, TimersClient, TimerType } from 'src/app/time-tracker-api';
import { isToday, shortTimeString } from 'src/app/utils/core.utils';

@Component({
  selector: 'app-daily-overview',
  templateUrl: './daily-overview.component.html',
  styleUrls: ['./daily-overview.component.css']
})
export class DailyOverviewComponent implements OnInit {
  displayedColumns: string[] = ['type', 'client', 'info', 'state', 'startTime', 'length', 'notes', 'controls'];
  dataSource = new MatTableDataSource<TimerDto>();
  maxDate: Date = new Date();
  startDate: Date = new Date(new Date().getTime() - 86400000);
  endDate: Date = new Date();
  loggedTime: string = '0:00';

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  private _currentHours: number = 24;
  private _hourStart: number = 6;
  private _runningTime: number = 0;

  constructor(
    private timerClient: TimersClient,
    private uiService: UiService,
    public dialog: MatDialog
  ) { }


  // public & template methods
  ngOnInit(): void {
    this.setTimeRange(this.toStartOfDay(new Date()));
    this.refreshView();
  }

  workRowClass = (timer: TimerDto) => {
    if(!timer?.startTimeUtc)
      return [];

    const styles: string[] = [];

    if(timer.entryType === TimerType.DailyTask)
      styles.push('task');

    if(isToday(timer.startTimeUtc))
      styles.push('today');

    return styles;
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
        this.refreshView();
      }
    });
  }

  dateChanged = (evt: MatDatepickerInputEvent<Date>) => {
    if(!evt?.value)
      return;

    this.setTimeRange(this.toStartOfDay(evt.value));
    this.refreshView();
  }

  hoursChanged = (e: any) => {
    if(!e?.target?.value)
      return;

    this._currentHours = parseInt(e.target.value);
  }

  updateRange = () => {
    this.setTimeRange(this.startDate);
    this.refreshView();
  }

  getTimerInfo = (timer: TimerDto) => {
    if(timer.entryType === TimerType.ProjectWork) {
      return timer.productName;
    }

    if(timer.entryType === TimerType.DailyTask) {
      return timer.taskName;
    }

    return '(unknown)';
  }


  // Internal methods
  private toStartOfDay = (date: Date) => {
    return new Date(
      date.getFullYear(),
      date.getMonth(),
      date.getDate(),
      this._hourStart
    );
  }

  private setTimeRange = (startDate: Date) => {
    this.startDate = startDate;
    this.endDate = new Date(startDate.getTime() + (this._currentHours * 3600000));
  }

  private refreshView = () => {
    this.uiService.showLoader(true);
    this.refreshTimers()
      .finally(() => { this.uiService.hideLoader(); });
  }

  private refreshTimers = () => {
    this._runningTime = 0;

    return new Promise<void>((resolve, reject) => {
      this.timerClient.listUserTimers(this.startDate, this.endDate).toPromise().then(
        (timers: TimerDto[]) => {
          // Total all running timer times
          timers.forEach((timer: TimerDto) => {
            this._runningTime += timer?.totalSeconds ?? 0;
          });

          // Configure the data table
          this.dataSource = new MatTableDataSource(timers);
          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;
          this.loggedTime = shortTimeString(this._runningTime);

          resolve();
        },
        (error: any) => { reject(error); }
      );
    });
  }

}
