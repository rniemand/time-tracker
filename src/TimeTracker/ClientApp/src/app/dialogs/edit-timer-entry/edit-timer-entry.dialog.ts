import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UiService } from 'src/app/services/ui.service';
import { TrackedTimeDto, TimersClient } from 'src/app/time-tracker-api';
import { DateTimeEditorEvent } from './../../components/ui/edit-timer-entry/edit-timer-entry.component';
import { TimerSeriesDialog } from '../timer-series/timer-series.dialog';

export interface EditTimerEntryDialogData {
  timer: TrackedTimeDto;
}

@Component({
  selector: 'app-edit-timer-entry-dialog',
  templateUrl: './edit-timer-entry.dialog.html',
  styleUrls: ['./edit-timer-entry.dialog.css']
})
export class EditTimerEntryDialog implements OnInit {
  timer?: TrackedTimeDto;
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
    let entryId = this.timer?.entryId ?? 0;
    if(entryId === 0) return;

    let updatedTimer = new TrackedTimeDto({
      ...this.timer,
      'startTimeUtc': this.startDate,
      'totalSeconds': this.durationSeconds,
      'notes': this.notes
    });

    this.uiService.showLoader(true);
    this.timersClient.updateTimerDuration(entryId, updatedTimer).toPromise().then(
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
