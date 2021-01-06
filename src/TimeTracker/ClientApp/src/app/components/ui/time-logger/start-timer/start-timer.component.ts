import { AfterViewInit, Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { StorageService } from 'src/app/services/storage.service';
import { UiService } from 'src/app/services/ui.service';
import { RawTimerDto, TimersClient } from 'src/app/time-tracker-api';
import { TimeLoggerEvent } from '../time-logger.component';

const KEY_VIEW_STATE = 'tt.start_timer.state';

interface ViewState {
  clientId: number,
  productId: number,
  projectId: number
}

@Component({
  selector: 'app-start-timer',
  templateUrl: './start-timer.component.html',
  styleUrls: ['./start-timer.component.css']
})
export class StartTimerComponent implements OnInit, AfterViewInit {
  @Output('onEvent') onEvent = new EventEmitter<TimeLoggerEvent>();
  clientId: number = 0;
  productId: number = 0;
  projectId: number = 0;
  viewInitialized: boolean = false;

  constructor(
    private timersClient: TimersClient,
    private authService: AuthService,
    private uiService: UiService,
    private storage: StorageService
  ) { }
  

  ngOnInit(): void {
    this.loadViewState();
  }

  ngAfterViewInit(): void {
    this.viewInitialized = true;
  }

  onProjectChange = () => {
  }

  startTimer = () => {
    let newEntry = new RawTimerDto({
      'parentTimerId': 0,
      'rootTimerId': 0,
      'clientId': this.clientId,
      'productId': this.productId,
      'projectId': this.projectId,
      'userId': this.authService.currentUser?.id ?? 0
    });
    
    this.uiService.showLoader(true);
    this.timersClient.startNewTimer(newEntry).toPromise().then(
      (success: boolean) => {
        if(!success)
          return;
          
        this.onEvent.emit({
          type: 'timer.created',
          source: 'StartTimerComponent'
        });
      },
      this.uiService.handleClientError
    );
  }

  // Template methods
  clientChanged = () => {
    this.productId = 0;
    this.projectId = 0;
    this.saveViewState();
  }

  productChanged = () => {
    this.projectId = 0;
    this.saveViewState();
  }

  projectChanged = () => {
    this.saveViewState();
  }


  // Internal methids
  private saveViewState = () => {
    if(!this.viewInitialized)
      return;

    let state: ViewState = {
      clientId: this.clientId,
      productId: this.productId,
      projectId: this.projectId
    };

    this.storage.setItem(KEY_VIEW_STATE, state);

    this.onEvent.emit({
      type: 'state_changed',
      source: 'StartTimerComponent',
      data: {
        clientId: this.clientId,
        productId: this.productId,
        projectId: this.projectId
      }
    });
  }

  private loadViewState = () => {
    if(!this.storage.hasItem(KEY_VIEW_STATE))
      return;

    let savedState = this.storage.getItem<ViewState>(KEY_VIEW_STATE);
    this.clientId = savedState?.clientId ?? 0;
    this.productId = savedState?.productId ?? 0;
    this.projectId = savedState?.projectId ?? 0;
  }

}
