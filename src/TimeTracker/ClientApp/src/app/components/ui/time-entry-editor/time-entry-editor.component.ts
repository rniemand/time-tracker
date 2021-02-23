import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ProjectDto } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-time-entry-editor',
  templateUrl: './time-entry-editor.component.html',
  styleUrls: ['./time-entry-editor.component.scss']
})
export class TimeEntryEditorComponent implements OnInit {
  @Input('project') project!: ProjectDto;
  @ViewChild('loggedTime', { static: false }) loggedTime?: ElementRef;
  
  editMode: boolean = false;
  apiCallRunning: boolean = false;
  currentValue: number = 0;
  private originalValue: number = 0;

  constructor() { }

  ngOnInit(): void {
    this.currentValue = 0;
    this.originalValue = 0;
  }

  editValue = () => {
    console.log('edit me baby!', this.project);
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
    this.apiCallRunning = true;

    console.log('updateLoggedTime', this.originalValue, this.currentValue);

    console.log({
      loggedTime: this.currentValue,
      projectId: this.project.projectId
    });
  }

}
