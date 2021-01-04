import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ClientDto, RawTimerDto, TimersClient } from 'src/app/time-tracker-api';

export interface TimerSeriesDialogData {
  rootTimerId: number;
}

@Component({
  selector: 'app-timer-series-dialog',
  templateUrl: './timer-series.dialog.html',
  styleUrls: ['./timer-series.dialog.css']
})
export class TimerSeriesDialog implements OnInit {
  displayedColumns: string[] = ['clientName', 'productName', 'projectName', 'entryEndTimeUtc', 'entryState', 'entryRunningTimeSec', 'options'];
  dataSource = new MatTableDataSource<RawTimerDto>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    public dialogRef: MatDialogRef<TimerSeriesDialog>,
    @Inject(MAT_DIALOG_DATA) public data: TimerSeriesDialogData,
    private timersClient: TimersClient
  ) { }

  ngOnInit(): void {
    this.timersClient.getTimerSeries(this.data.rootTimerId).toPromise().then(
      (entries: RawTimerDto[]) => {
        this.dataSource = new MatTableDataSource(entries);
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
      },
      (error: any) => {
        console.error(error);
      }
    );

  }

  onNoClick(): void {
    this.dialogRef.close();
  }

}
