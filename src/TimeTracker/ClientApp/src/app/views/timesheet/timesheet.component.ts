import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { AddTimesheetRowDialog, AddTimesheetRowDialogData } from 'src/app/dialogs/add-timesheet-row/add-timesheet-row.dialog';
import { GetTimeSheetRequest, GetTimeSheetResponse, TimeSheetClient, TimeSheetDateDto } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.css']
})
export class TimesheetComponent implements OnInit {
  clientId: number = 0;
  dates: TimeSheetDateDto[] = [];

  constructor(
    public dialog: MatDialog,
    private timeSheetClient: TimeSheetClient
  ) { }

  ngOnInit(): void {
    let dialogData: AddTimesheetRowDialogData = { };

    let dialogRef = this.dialog.open(AddTimesheetRowDialog, {
      ...DIALOG_DEFAULTS,
      data: dialogData
    });

    // dialogRef.afterClosed().subscribe(result => {
    //   if(result?.outcome == 'updated') {
    //     this.refreshTable();
    //   }
    // });
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
    const startDate = new Date();
    const endDate = new Date(startDate.getTime() + (60 * 60 * 24 * 7 * 1000));
    const request = new GetTimeSheetRequest({
      startDate: startDate,
      endDate: endDate,
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
