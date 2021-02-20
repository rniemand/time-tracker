import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface AddTimesheetRowDialogData {

}

@Component({
  selector: 'add-timesheet-row-dialog',
  templateUrl: './add-timesheet-row.dialog.html',
  styleUrls: ['./add-timesheet-row.dialog.css']
})
export class AddTimesheetRowDialog implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<AddTimesheetRowDialog>,
    @Inject(MAT_DIALOG_DATA) public data: AddTimesheetRowDialogData
  ) { }

  ngOnInit(): void {
    
  }

}
