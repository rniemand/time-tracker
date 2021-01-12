import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UiService } from 'src/app/services/ui.service';
import { RawTimerDto, TimersClient } from 'src/app/time-tracker-api';
import { DateTimeEditorEvent } from './../../components/ui/edit-timer-entry/edit-timer-entry.component';
import { TimerSeriesDialog } from '../timer-series/timer-series.dialog';

export interface EditTimerEntryDialogData {
  timer: RawTimerDto;
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
    @Inject(MAT_DIALOG_DATA) public data: EditTimerEntryDialogData,
    private uiService: UiService,
    private timersClient: TimersClient
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

  closeDialog(outcome: string): void {
    this.dialogRef.close({
      outcome: outcome
    });
  }

  saveChanges = () => {
    let rawTimerId = this.timer?.rawTimerId ?? 0;
    if(rawTimerId === 0)
      return;

    let updatedTimer = new RawTimerDto({
      ...this.timer,
      'entryStartTimeUtc': this.startDate,
      'entryRunningTimeSec': this.durationSeconds,
      'timerNotes': this.notes
    });

    this.uiService.showLoader(true);
    this.timersClient.updateTimerDuration(rawTimerId, updatedTimer).toPromise().then(
      (success: boolean) => {
        this.uiService.hideLoader();
        this.closeDialog('updated');
      },
      (error: any) => {
        this.uiService.hideLoader();
        this.closeDialog(`error: ${error}`);
      }
    );
  }
}
