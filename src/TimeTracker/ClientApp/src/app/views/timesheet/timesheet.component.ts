import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { AddTimesheetRowDialog, AddTimesheetRowDialogData, AddTimesheetRowDialogResult } from 'src/app/dialogs/add-timesheet-row/add-timesheet-row.dialog';
import { AuthService } from 'src/app/services/auth.service';
import { GetTimeSheetRequest, GetTimeSheetResponse, ProjectDto, TimeSheetClient } from 'src/app/time-tracker-api';
import { getBaseDate } from 'src/app/utils/core.utils';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {
  clientId: number = 0;
  startDate: Date = new Date();
  endDate: Date = new Date((new Date()).getTime() + (60 * 60 * 24 * 7 * 1000));
  projects: ProjectDto[] = [];
  dates: Date[] = [];
  colspan: number = 3;

  constructor(
    public dialog: MatDialog,
    private timeSheetClient: TimeSheetClient,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.setDates();
  }

  clientSelected = (clientId: number) => {
    this.clientId = clientId;
    this.refreshView();
  }

  addRow = () => {
    let dialogData: AddTimesheetRowDialogData = {
      userId: this.authService.currentUser?.id ?? 0,
      clientId: 1, //this.clientId
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
        }
      }
    });
  }


  // Internal methods
  private setDates = (startDate?: Date) => {
    const dates: Date[] = [];

    const workingDate = getBaseDate(startDate);

    for(var i = 0; i < 7; i++) {
      dates.push(new Date(workingDate.getTime() + (24 * 60 * 60 * 1000 * (i + 1))));
    }

    this.dates = dates;
  }

  private refreshView = () => {
    if(this.clientId == 0) {
      return;
    }

    this.loadTimeSheet()
      .finally(() => {
        console.log('we are all done');
      })
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
          this.projects = response?.projects ?? [];
          this.colspan = this.dates.length + 2;

          console.log(response);

          resolve();
        },
        (error: any) => { reject(error); }
      );
    });
  }

}
