import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { AddTimesheetRowDialog, AddTimesheetRowDialogData, AddTimesheetRowDialogResult } from 'src/app/dialogs/add-timesheet-row/add-timesheet-row.dialog';
import { AuthService } from 'src/app/services/auth.service';
import { GetTimeSheetRequest, GetTimeSheetResponse, TimeSheetClient, TimeSheetDateDto } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.css']
})
export class TimesheetComponent implements OnInit {
  clientId: number = 0;
  startDate: Date = new Date();
  endDate: Date = new Date((new Date()).getTime() + (60 * 60 * 24 * 7 * 1000))
  dates: TimeSheetDateDto[] = [];

  constructor(
    public dialog: MatDialog,
    private timeSheetClient: TimeSheetClient,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
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

  clientSelected = (clientId: number) => {
    this.clientId = clientId;
    this.refreshView();
  }


  // Internal methods
  private refreshView = () => {
    this.dates = [];

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
          this.dates = response?.dates ?? [];
          
          console.log(response);

          resolve();
        },
        (error: any) => { reject(error); }
      );
    });
  }

}
