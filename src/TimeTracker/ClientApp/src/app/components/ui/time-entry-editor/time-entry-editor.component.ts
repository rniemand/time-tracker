import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ProjectDto, TimeSheetDateDto } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-time-entry-editor',
  templateUrl: './time-entry-editor.component.html',
  styleUrls: ['./time-entry-editor.component.scss']
})
export class TimeEntryEditorComponent implements OnInit {
  @Input('date') date!: TimeSheetDateDto;
  @Input('project') project!: ProjectDto;
  @ViewChild('loggedTime', { static: false }) loggedTime?: ElementRef;
  editMode: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

  editValue = () => {
    console.log('edit me baby!', this.date, this.project);
    this.editMode = true;

    setTimeout(() => {
      this.loggedTime?.nativeElement?.focus();
      this.loggedTime?.nativeElement?.select();
    }, 10);
  }

  onBlur = () => {
    console.log('onBlur');
  }

  onKeyDown = (e: any) => {
    const keyCode = e?.keyCode ?? 0;
    if(keyCode === 13) {
      console.log('pressed enter');
    }
  }

}
