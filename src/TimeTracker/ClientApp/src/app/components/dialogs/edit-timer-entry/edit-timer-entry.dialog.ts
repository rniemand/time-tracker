import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RawTimerDto } from 'src/app/time-tracker-api';
import { DateTimeEditorEvent } from '../../ui/edit-timer-entry/edit-timer-entry.component';
import { TimerSeriesDialog } from '../timer-series/timer-series.dialog';

export interface EditTimerEntryDialogData {
  timer: RawTimerDto;
  startDate?: Date;
  durationSeconds?: number;
  okClicked?: boolean;
  outcome?: string;
  notes?: string;
}

@Component({
  selector: 'app-edit-timer-entry-dialog',
  templateUrl: './edit-timer-entry.dialog.html',
  styleUrls: ['./edit-timer-entry.dialog.css']
})
export class EditTimerEntryDialog implements OnInit {
  timer?: RawTimerDto;
  startDate?: Date;
  durationSeconds: number = 0;
  notes?: string;
  hasChange: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<TimerSeriesDialog>,
    @Inject(MAT_DIALOG_DATA) public data: EditTimerEntryDialogData
  ) { }

  ngOnInit(): void {
    this.timer = this.data.timer;
  }

  entryChanged = (e: DateTimeEditorEvent) => {
    if(e?.type != 'valueChanged')
      return;

    this.startDate = e.startDate;
    this.durationSeconds = e?.durationSec ?? 0;
    this.hasChange = true;
    this.notes = e?.notes ?? '';
  }

  closeDialog(okClicked: boolean): void {
    this.dialogRef.close({
      ...this.data,
      startDate: this.startDate,
      durationSeconds: this.durationSeconds,
      okClicked: okClicked,
      outcome: 'user-closed',
      notes: this.notes
    });
  }

}
