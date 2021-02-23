import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TimeSheetEntryInfo } from 'src/app/components/ui/time-entry-editor/time-entry-editor.component';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { AddTimesheetRowDialog, AddTimesheetRowDialogData, AddTimesheetRowDialogResult } from 'src/app/dialogs/add-timesheet-row/add-timesheet-row.dialog';
import { AuthService } from 'src/app/services/auth.service';
import { AddTimeSheetEntryRequest, GetTimeSheetRequest, GetTimeSheetResponse, ProjectDto, TimeSheetClient } from 'src/app/time-tracker-api';
import { getBaseDate } from 'src/app/utils/core.utils';

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
          console.log('need to add line!', outcome);

          // const request = new AddTimeSheetEntryRequest({
          //   projectId: outcome.projectId,
          //   loggedTimeMin: 0,
          //   entryDate: this.dates[0],
          //   endDate: this.endDate,
          //   startDate: this.startDate
          // });

          // this.timeSheetClient.updateEntry(request).toPromise().then(
          //   (response: GetTimeSheetResponse) => {
          //     this.updateTimeSheet(response);
          //   },
          //   (error: any) => {
          //     console.log(error);
          //   }
          // );
        }
      }
    });
  }

  onEntryChange = (timeSheet: GetTimeSheetResponse) => {
    this.updateTimeSheet(timeSheet);
  }


  // Internal methods
  private setDates = (startDate: Date, length: number) => {
    const entries: TimeSheetEntryInfo[] = [];
    const workingDate = getBaseDate(startDate);

    for(var i = 0; i < length; i++) {
      entries.push({
        entryDate: new Date(workingDate.getTime() + (24 * 60 * 60 * 1000 * (i + 1))),
        startDate: this.startDate,
        endDate: this.endDate,
        entryTimeMin: 0
      });
    }

    this.entries = entries;
  }

  private refreshView = () => {
    if(this.clientId == 0) { return; }
    this.entries = [];

    this.loadTimeSheet()
      .finally(() => {
        console.log('we are all done');
      })
  }

  private updateTimeSheet = (response: GetTimeSheetResponse) => {
    this.projects = response?.projects ?? [];
    this.colspan = this.entries.length + 2;

    const startDate = response?.startDate ?? this.startDate;
    this.setDates(startDate, response?.dayCount ?? 7);
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
