import { Component, EventEmitter, forwardRef, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatFormFieldAppearance } from '@angular/material/form-field';
import { DailyTaskDto, DailyTasksClient, IntListItem } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-daily-task-selector',
  templateUrl: './daily-task-selector.component.html',
  styleUrls: ['./daily-task-selector.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DailyTaskSelectorComponent),
      multi: true
    }
  ]
})
export class DailyTaskSelectorComponent implements OnInit, ControlValueAccessor, OnChanges {
  @Output('changed') onChange = new EventEmitter<number>();
  @Input('appearance') appearance: MatFormFieldAppearance = "outline";
  @Input('clientId') clientId: number = 0;
  @Input('class') class: string = "";
  
  taskId: number = 0;
  loading: boolean = true;
  entries: IntListItem[] = [];
  label: string = 'Select a client first';

  private _onChangeFn = (_: any) => { };

  constructor(
    private taskClient: DailyTasksClient
  ) { }
  
  

  ngOnInit(): void {
  }

  valueChanged = () => {

  }

  // ControlValueAccessor & OnChanges methods
  writeValue(obj: any): void {
    if(!obj) return;

    if(typeof(obj) === 'string') {
      let intValue = parseInt(obj);
      if(isNaN(intValue)) return;
      this.taskId = intValue;
      return;
    }

    if(typeof(obj) === 'number') {
      this.taskId = obj;
      return;
    }
  }

  registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }

  registerOnTouched(fn: any): void { }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes.hasOwnProperty('clientId')) {
      console.log('client id has changed', this.clientId);
      this.refreshTasks();
    }
  }

  // Internal mehtods
  private refreshTasks = () => {
    this.loading = true;
    this.entries = [];

    if(this.clientId === 0) {
      this.loading = false;
      return;
    }

    this.taskClient.getClientTasksList(this.clientId).toPromise().then(
      (tasks: IntListItem[]) => {
        this.entries = tasks;

        if(this.taskId <= 0 && tasks.length > 0) {
          this.taskId = tasks[0]?.value ?? 0;
        }

        this.loading = false;
      },
      (error: any) => {
        this.loading = false;
      }
    );
  }

}
