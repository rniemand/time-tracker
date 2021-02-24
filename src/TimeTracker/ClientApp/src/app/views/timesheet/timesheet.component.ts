import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TimeSheetEntryInfo } from 'src/app/components/ui/time-entry-editor/time-entry-editor.component';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { AddTimesheetRowDialog, AddTimesheetRowDialogData, AddTimesheetRowDialogResult } from 'src/app/dialogs/add-timesheet-row/add-timesheet-row.dialog';
import { AuthService } from 'src/app/services/auth.service';
import { AddTimeSheetEntryRequest, GetTimeSheetRequest, GetTimeSheetResponse, ProjectDto, TimeSheetClient, TimeSheetEntryDto } from 'src/app/time-tracker-api';
import { getBaseDate, isToday } from 'src/app/utils/core.utils';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {
  clientId: number = 0;
  startDate!: Date;
  endDate!: Date;
  projects: ProjectDto[] = [];
  entries: TimeSheetEntryInfo[] = [];
  colspan: number = 3;
  projectTimes: { [key: number]: number } = {};
  dailyTimes: { [key: number]: number } = {};
  totalLoggedTime: number = 0;

  constructor(
    public dialog: MatDialog,
    private timeSheetClient: TimeSheetClient,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.startDate = new Date();
    this.endDate = new Date((new Date()).getTime() + (60 * 60 * 24 * 7 * 1000));
  }

  clientSelected = (clientId: number) => {
    this.clientId = clientId;
    this.refreshView();
  }

  addRow = () => {
    let dialogData: AddTimesheetRowDialogData = {
      clientId: this.clientId,
      startDate: this.startDate,
      endDate: this.endDate
    };

    let dialogRef = this.dialog.open(AddTimesheetRowDialog, {
      ...DIALOG_DEFAULTS,
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if(result && result.hasOwnProperty('addLine')) {
        const outcome = result as AddTimesheetRowDialogResult;
        if(outcome.addLine) {
          const request = new AddTimeSheetEntryRequest({
            projectId: outcome.projectId,
            loggedTimeMin: 0,
            entryDate: this.startDate,
            endDate: this.endDate,
            startDate: this.startDate
          });

          this.timeSheetClient.updateEntry(request).toPromise().then(
            (response: GetTimeSheetResponse) => {
              this.updateTimeSheet(response);
            },
            (error: any) => {
              console.log(error);
            }
          );
        }
      }
    });
  }

  onEntryChange = (timeSheet: GetTimeSheetResponse) => {
    this.updateTimeSheet(timeSheet);
  }

  getProjectTotalTime = (project: ProjectDto) => {
    const projectId = project?.projectId ?? 0;
    if(projectId == 0 || !this.projectTimes.hasOwnProperty(projectId)) {
      return 0;
    }

    return this.projectTimes[projectId];
  }

  getDailyTotalTime = (time: number) => {
    if(this.dailyTimes.hasOwnProperty(time)) {
      return this.dailyTimes[time];
    }

    return 0;
  }

  getDateClass = (entry: TimeSheetEntryInfo) => {
    const classes: string[] = ['date'];

    if(entry.weekend) {
      classes.push('weekend');
    }

    if(entry.today) {
      classes.push('today');
    }

    return classes;
  }


  // Internal methods
  private setDates = (startDate: Date, length: number) => {
    const entries: TimeSheetEntryInfo[] = [];
    const workingTime = getBaseDate(startDate, true).getTime();

    for(var i = 0; i < length; i++) {
      const currentDate = new Date(workingTime + (24 * 60 * 60 * 1000 * (i + 1)));
      const currentDay = currentDate.getDay();

      entries.push({
        entryDate: currentDate,
        entryTime: currentDate.getTime(),
        startDate: this.startDate,
        endDate: this.endDate,
        entryTimes: {},
        weekend: currentDay === 0 || currentDay === 6,
        today: isToday(currentDate)
      });
    }

    this.entries = entries;
  }

  private refreshView = () => {
    if(this.clientId == 0) { return; }
    this.entries = [];

    this.loadTimeSheet()
      .finally(() => {
        // All done
      })
  }

  private getEntryInfo = (entryDate?: Date): TimeSheetEntryInfo | undefined => {
    if(!entryDate) { return undefined; }

    const wanted = entryDate.getTime();
    for(var i = 0; i < this.entries.length; i++) {
      if(this.entries[i].entryTime == wanted) {
        return this.entries[i];
      }
    }

    return undefined;
  }

  private setEntryInfo = (entry: TimeSheetEntryDto) => {
    const info = this.getEntryInfo(entry.entryDate);
    if(!info) { return; }

    const projectId = entry?.projectId ?? 0;
    const entryTime = parseFloat(((entry?.entryTimeMin ?? 0) / 60).toFixed(2));

    info.entryTimes[projectId] = entryTime;
    
    if(!this.projectTimes.hasOwnProperty(projectId)) {
      this.projectTimes[projectId] = 0;
    }
    this.projectTimes[projectId] += entryTime;

    if(!this.dailyTimes.hasOwnProperty(info.entryTime)) {
      this.dailyTimes[info.entryTime] = 0;
    }
    this.dailyTimes[info.entryTime] += entryTime;
    this.dailyTimes[0] += entryTime;
    this.totalLoggedTime += entryTime;
  }

  private updateTimeSheet = (response: GetTimeSheetResponse) => {
    this.projects = response?.projects ?? [];
    this.projectTimes = {};
    this.dailyTimes = { 0: 0 };
    this.totalLoggedTime = 0;

    const startDate = response?.startDate ?? this.startDate;
    this.setDates(startDate, response?.dayCount ?? 7);

    const entries = response?.entries ?? [];
    this.colspan = this.entries.length + 2;
    entries.forEach(this.setEntryInfo);
  }

  private loadTimeSheet = () => {
    const request = new GetTimeSheetRequest({
      startDate: this.startDate,
      endDate: this.endDate,
      clientId: 1
    });

    return new Promise<void>((resolve, reject) => {
      this.timeSheetClient.getTimeSheet(request).toPromise().then(
        (response: GetTimeSheetResponse) => {
          this.updateTimeSheet(response);
          resolve();
        },
        (error: any) => { reject(error); }
      );
    });
  }

}
