import { Component, EventEmitter, forwardRef, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatFormFieldAppearance } from '@angular/material/form-field';
import { LoggerService } from 'src/app/services/logger.service';
import { DailyTasksClient, IntListItem } from 'src/app/time-tracker-api';

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
    private taskClient: DailyTasksClient,
    private logger: LoggerService
  ) { }

  ngOnInit(): void {
  }

  valueChanged = () => {
    if (this._onChangeFn) {
      this._onChangeFn(this.taskId);
    }

    this.onChange.emit(this.taskId);
  }

  // ControlValueAccessor & OnChanges methods
  writeValue(obj: any): void {
    if(!obj)
      return;

    if(typeof(obj) === 'string') {
      let intValue = parseInt(obj);

      if(isNaN(intValue))
        return;
      
      this.setTaskId(intValue);
    }
    else if(typeof(obj) === 'number') {
      this.taskId = obj;
    }
    else {
      this.logger.warn(`Unsupported type: ${typeof(obj)}`);
    }
  }

  registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }

  registerOnTouched(fn: any): void { }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes.hasOwnProperty('clientId')) {
      this.logger.trace(`ngOnChanges > clientId > ${this.clientId}`);
      this.taskId = 0;
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
          this.setTaskId(tasks[0]?.value ?? 0);
        }

        this.loading = false;
      },
      (error: any) => {
        this.loading = false;
      }
    );
  }

  private setTaskId = (taskId: number) => {
    this.taskId = taskId;
    this.valueChanged();
  }

}
