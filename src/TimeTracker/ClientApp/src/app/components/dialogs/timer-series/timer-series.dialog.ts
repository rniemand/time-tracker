import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { UiService } from 'src/app/services/ui.service';
import { RawTimerDto, TimersClient } from 'src/app/time-tracker-api';
import { EditTimerEntryDialog, EditTimerEntryDialogData } from '../edit-timer-entry/edit-timer-entry.dialog';

export interface TimerSeriesDialogData {
  rootTimerId: number;
}

@Component({
  selector: 'app-timer-series-dialog',
  templateUrl: './timer-series.dialog.html',
  styleUrls: ['./timer-series.dialog.css']
})
export class TimerSeriesDialog implements OnInit {
  displayedColumns: string[] = ['client', 'product', 'endTime', 'state', 'length', 'notes', 'controls'];
  dataSource = new MatTableDataSource<RawTimerDto>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    public dialogRef: MatDialogRef<TimerSeriesDialog>,
    @Inject(MAT_DIALOG_DATA) public data: TimerSeriesDialogData,
    private timersClient: TimersClient,
    private uiService: UiService,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.refreshTable();
  }

  // Dialog methods
  onNoClick(): void {
    this.dialogRef.close();
  }

  editNote = (timer: RawTimerDto) => {
    let rawTimerId = timer?.rawTimerId ?? 0;
    if(rawTimerId == 0)
      return;

    let updatedNote = prompt('Edit note', timer.timerNotes);
    this.timersClient.updateNotes(rawTimerId, updatedNote ?? '').toPromise().then(
      (success: boolean) => {
        this.refreshTable();
        this.uiService.notify(`Note ${success ? 'updated' : 'updating failed'}`);
      },
      this.uiService.handleClientError
    );

  }

  editEntry = (timer: RawTimerDto) => {
    let dialogData: EditTimerEntryDialogData = { timer: timer };

    let dialogRef = this.dialog.open(EditTimerEntryDialog, {
      ...DIALOG_DEFAULTS,
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if(result && result?.outcome == 'user-closed' && result?.okClicked) {
        let castResult = result as EditTimerEntryDialogData;

        let updatedTimer = new RawTimerDto({
          ...castResult.timer,
          'entryStartTimeUtc': castResult.startDate,
          'entryRunningTimeSec': castResult.durationSeconds,
          'timerNotes': castResult.notes
        });

        this.uiService.showLoader(true);
        this.timersClient.updateTimerDuration(updatedTimer).toPromise().then(
          (success: boolean) => { this.refreshTable(); },
          this.uiService.handleClientError
        );
      }
    });
  }

  // Internal methods
  private refreshTable = () => {
    this.timersClient.getTimerSeries(this.data.rootTimerId).toPromise().then(
      (entries: RawTimerDto[]) => {
        this.dataSource = new MatTableDataSource(entries);
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
      },
      this.uiService.handleClientError
    );
  }
}
