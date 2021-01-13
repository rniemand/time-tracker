import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TimerSeriesDialog, TimerSeriesDialogData } from 'src/app/dialogs/timer-series/timer-series.dialog';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { LoggerService } from 'src/app/services/logger.service';
import { StorageService } from 'src/app/services/storage.service';
import { UiService } from 'src/app/services/ui.service';
import { TimerDto, TimersClient } from 'src/app/time-tracker-api';

const KEY_STATE = 'tt.list_timer.state';

interface ListTimersState {
  autoRefresh: boolean;
}

@Component({
  selector: 'app-list-timers',
  templateUrl: './list-timers.component.html',
  styleUrls: ['./list-timers.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ListTimersComponent implements OnInit, OnDestroy {
  timers: TimerDto[] = [];
  flipFlop: boolean = false;
  remaining: number = 30;
  autoRefresh: boolean = true;
  runningTimers: boolean = false;
  refreshingTimers: boolean = true;

  private _interval: any = null;
  private _decrementTimer: boolean = true;

  constructor(
    private timersClient: TimersClient,
    private uiService: UiService,
    public dialog: MatDialog,
    private storage: StorageService,
    private logger: LoggerService
  ) { }
  
  ngOnInit(): void {
    this.loadSavedState();
    this.startTicker();
    this.refreshTimers();
  }

  ngOnDestroy(): void {
    this.stopTicker();
  }


  // Template methods
  pauseTimer = (timer: TimerDto) => {
    let entryId = timer?.entryId ?? 0;
    if(entryId == 0) return;

    this.refreshingTimers = true;
    this.timersClient.pauseTimer(entryId).toPromise().then(
      (success: boolean) => {
        if(success) this.uiService.notify('Paused');
        this.refreshTimers();
      },
      this.handleClientError
    );
  }

  resumeTimer = (timer: TimerDto) => {
    let entryId = timer?.entryId ?? 0;
    if(entryId == 0) return;

    this.refreshingTimers = true;
    this.timersClient.resumeTimer(entryId).toPromise().then(
      (success: boolean) => {
        if(success) this.uiService.notify('Resumed');
        this.refreshTimers();
      },
      this.handleClientError
    );
  }

  resumeSingleTimer = (timer: TimerDto) => {
    let entryId = timer?.entryId ?? 0;
    if(entryId === 0) return;
    
    this.refreshingTimers = true;
    this.timersClient.resumeSingleTimer(entryId).toPromise().then(
      (success: boolean) => {
        if(success) this.uiService.notify('Resumed');
        this.refreshTimers();
      },
      this.handleClientError
    );
  }

  completeTimer = (timer: TimerDto) => {
    let entryId = timer?.entryId ?? 0;
    if(entryId == 0) return;

    if(!confirm(`Complete timer: ${timer.projectName} (${timer.productName})?`)) {
      return;
    }

    this.refreshingTimers = true;
    this.timersClient.completeTimer(entryId).toPromise().then(
      (success: boolean) => {
        if(success) this.uiService.notify('Timer completed');
        this.refreshTimers();
      },
      this.handleClientError
    );
  }

  refresh = () => {
    this.refreshTimers();
  }

  getClass = (timer: TimerDto) => {
    if(timer?.running) {
      return ['timer-entry', 'running'];
    }

    return ['timer-entry'];
  }

  timerHistory = (timer: TimerDto) => {
    let dialogData: TimerSeriesDialogData = {
      projectId: timer?.projectId ?? 0
    };

    this.stopTicker();
    let dialogRef = this.dialog.open(TimerSeriesDialog, {
      ...DIALOG_DEFAULTS,
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      this.startTicker();
    });
  }

  getTooltip = (timer: TimerDto) => {
    return `Client: ${timer?.clientName ?? 'Unknown'}`;
  }


  // Internal methods
  private refreshTimers = () => {
    this.saveCurrentState();
    this.timers = [];
    this.runningTimers = false;
    this.refreshingTimers = true;

    this.timersClient.getActiveTimers().toPromise().then(
      (timers: TimerDto[]) => {
        this.timers = timers;
        this.remaining = 30;
        this._decrementTimer = true;
        this.runningTimers = this.containsRunningTimers(timers);
        this.refreshingTimers = false;
      },
      this.handleClientError
    );
  }

  private tick = () => {
    this.flipFlop = !this.flipFlop;

    if(this._decrementTimer) {
      this.remaining -= 1;
    }

    if(this.remaining % 5 === 0) {
      this.saveCurrentState();
    }

    if(this.autoRefresh && this.remaining == 0) {
      this.remaining = -1;
      this._decrementTimer = false;
      this.refreshTimers();
    }
  }

  private startTicker = () => {
    this._interval = setInterval(this.tick, 1000);
  }

  private stopTicker = () => {
    if(this._interval) {
      clearInterval(this._interval);
      this._interval = null;
    }
  }

  private containsRunningTimers = (timers: TimerDto[]) => {
    if(timers.length === 0)
      return false;

    for(var i = 0; i < timers.length; i++) {
      if(timers[i]?.running) {
        return true;
      }
    }

    return false;
  }
  
  private saveCurrentState = () => {
    let state: ListTimersState = {
      autoRefresh: this.autoRefresh
    };

    this.storage.setItem(KEY_STATE, state);
    this.logger.trace('list timers state saved');
  }

  private loadSavedState = () => {
    let state: ListTimersState = {
      autoRefresh: true
    };

    if(this.storage.hasItem(KEY_STATE)) {
      state = this.storage.getItem<ListTimersState>(KEY_STATE);
      this.logger.trace('saved list timers state loaded');
    }

    this.autoRefresh = state.autoRefresh;
  }

  private handleClientError = (error: any) => {
    this.uiService.handleClientError(error);
    this.refreshingTimers = false;
  }
}
