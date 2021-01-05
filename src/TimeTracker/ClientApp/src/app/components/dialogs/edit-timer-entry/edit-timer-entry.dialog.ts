import { Component, Inject, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RawTimerDto } from 'src/app/time-tracker-api';
import { TimerSeriesDialog } from '../timer-series/timer-series.dialog';

export interface EditTimerEntryDialogData {
  entry: RawTimerDto;
}

@Component({
  selector: 'app-edit-timer-entry',
  templateUrl: './edit-timer-entry.dialog.html',
  styleUrls: ['./edit-timer-entry.dialog.css']
})
export class EditTimerEntryDialog implements OnInit {
  startDate = new FormControl(new Date());
  maxDate: Date = new Date();

  constructor(
    public dialogRef: MatDialogRef<TimerSeriesDialog>,
    @Inject(MAT_DIALOG_DATA) public data: EditTimerEntryDialogData
  ) { }

  ngOnInit(): void {
    this.startDate = new FormControl(this.data?.entry?.entryStartTimeUtc ?? new Date());

    console.log(this.data.entry);
    console.log(this.startDate);
  }

  dateChanged = (e: MatDatepickerInputEvent<any>) => {
    console.log(e.value);
  }

}
