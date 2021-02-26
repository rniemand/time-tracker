import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import { TimeEntryEditorEvent, TimeSheetEntryInfo } from 'src/app/components/ui/time-entry-editor/time-entry-editor.component';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { AddTimesheetRowDialog, AddTimesheetRowDialogData, AddTimesheetRowDialogResult } from 'src/app/dialogs/add-timesheet-row/add-timesheet-row.dialog';
import { AddTimeSheetEntryRequest, GetTimeSheetRequest, GetTimeSheetResponse, ProductDto, ProjectDto, TimeSheetClient, TimeSheetEntryDto } from 'src/app/time-tracker-api';
import { getBaseDate, getStartOfWeek, isToday } from 'src/app/utils/core.utils';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {
  clientId: number = 0;
  startDate!: Date;
  startDateFc!: FormControl;
  minDate!: Date;
  endDate!: Date;
  projects: ProjectDto[] = [];
  products: { [key: number]: ProductDto } = {};
  entries: TimeSheetEntryInfo[] = [];
  colspan: number = 3;
  projectTimes: { [key: number]: number } = {};
  dailyTimes: { [key: number]: number } = {};
  totalLoggedTime: number = 0;
  updating: boolean = false;

  constructor(
    public dialog: MatDialog,
    private timeSheetClient: TimeSheetClient
  ) { }

  ngOnInit(): void {
    this.setDateRange(getStartOfWeek(), 7);
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

  dateSelected = (event: MatDatepickerInputEvent<Date>) => {
    if(!event?.value) { return; }
    this.setDateRange(event.value, 7);
    this.refreshView();
  }

  getProductName = (project: ProjectDto) => {
    const productId = project?.productId ?? 0;
    if(!this.products.hasOwnProperty(productId)) {
      return 'Unknown';
    }
    
    return this.products[productId].productName ?? 'N/A';
  }

  onEntryEvent = (event: TimeEntryEditorEvent) => {
    this.updating = event?.apiCallRunning ?? false;
  }


  // Internal methods
  private updateTimeSheet = (response: GetTimeSheetResponse) => {
    this.resetView();

    this.projects = response?.projects ?? [];
    (response?.products ?? []).forEach((product: ProductDto) => {
      this.products[product?.productId ?? 0] = product;
    });

    const startDate = response?.startDate ?? this.startDate;
    this.setDates(startDate, response?.dayCount ?? 7);

    const entries = response?.entries ?? [];
    this.colspan = this.entries.length + 2;
    entries.forEach(this.setEntryInfo);
  }

  private resetView = () => {
    this.entries = [];
    this.projects = [];
    this.products = {};
    this.projectTimes = {};
    this.dailyTimes = { 0: 0 };
    this.totalLoggedTime = 0;
  }

  private setDateRange = (startDate: Date, numDays: number) => {
    this.startDate = startDate;
    this.startDateFc = new FormControl(this.startDate);
    this.endDate = new Date(this.startDate.getTime() + (numDays * 24 * 60 * 60 * 1000));
    
    this.minDate = new Date(
      this.startDate.getFullYear() - 1,
      this.startDate.getMonth(),
      this.startDate.getDate()
    );
  }

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

    this.updating = true;
    this.resetView();

    this.loadTimeSheet()
      .finally(() => {
        this.updating = false;
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
