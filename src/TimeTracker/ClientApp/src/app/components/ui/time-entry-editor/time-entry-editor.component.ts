import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AddTimeSheetEntryRequest, GetTimeSheetResponse, ProjectDto, TimeSheetClient } from 'src/app/time-tracker-api';

export interface TimeSheetEntryInfo {
  entryDate: Date;
  entryTime: number;
  startDate: Date;
  endDate: Date;
  entryTimes: { [key: number]: number };
  weekend: boolean;
  today: boolean;
}

@Component({
  selector: 'app-time-entry-editor',
  templateUrl: './time-entry-editor.component.html',
  styleUrls: ['./time-entry-editor.component.scss']
})
export class TimeEntryEditorComponent implements OnInit {
  @Input('info') info!: TimeSheetEntryInfo;
  @Input('project') project!: ProjectDto;
  @Input('startDate') startDate!: Date;
  @Input('endDate') endDate!: Date;
  @Output('onUpdate') onUpdate = new EventEmitter<GetTimeSheetResponse>();
  @ViewChild('loggedTime', { static: false }) loggedTime?: ElementRef;
  
  editMode: boolean = false;
  apiCallRunning: boolean = false;
  currentValue: number = 0;
  private originalValue: number = 0;

  constructor(
    private timeSheetClient: TimeSheetClient
  ) { }

  ngOnInit(): void {
    this.currentValue = 0;

    const projectId = this.project?.projectId ?? 0;
    if(projectId > 0 && this.info.entryTimes.hasOwnProperty(projectId)) {
      this.currentValue = this.info.entryTimes[projectId];
    }

    this.originalValue = this.currentValue;
  }

  editValue = () => {
    this.editMode = true;

    setTimeout(() => {
      this.loggedTime?.nativeElement?.focus();
      this.loggedTime?.nativeElement?.select();
    }, 10);
  }

  onBlur = () => {
    this.updateLoggedTime();
  }

  onKeyDown = (e: any) => {
    const keyCode = e?.keyCode ?? 0;
    if(keyCode !== 13) { return; }
    this.updateLoggedTime();
  }

  // Internal methods
  private updateLoggedTime = () => {
    this.editMode = false;

    if(this.originalValue === this.currentValue) {
      return;
    }

    this.apiCallRunning = true;
    const request = new AddTimeSheetEntryRequest({
      projectId: this.project.projectId,
      entryDate: this.info.entryDate,
      startDate: this.startDate,
      endDate: this.endDate,
      loggedTimeMin: Math.floor(this.currentValue * 60)
    });
    
    this.timeSheetClient.updateEntry(request).toPromise().then((response: GetTimeSheetResponse) => {
      this.onUpdate.emit(response);
    })
    .finally(() => {
      this.apiCallRunning = false;
    });
  }

}
