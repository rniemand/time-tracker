import { EventEmitter } from '@angular/core';
import { Component, forwardRef, Input, OnInit, Output } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { TimerDto } from 'src/app/time-tracker-api';

interface TimeDuration {
  hours: number;
  minutes: number;
  seconds: number;
}

export interface DateTimeEditorEvent {
  type: string;
  timer?: TimerDto;
  startDate?: Date;
  durationSec?: number;
  notes?: string;
}

@Component({
  selector: 'app-edit-timer-entry',
  templateUrl: './edit-timer-entry.component.html',
  styleUrls: ['./edit-timer-entry.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EditTimerEntryComponent),
      multi: true
    }
  ]
})
export class EditTimerEntryComponent implements OnInit, ControlValueAccessor {
  @Output('changed') onChanged = new EventEmitter<DateTimeEditorEvent>();
  editForm: FormGroup;
  pickedDate = new FormControl(new Date());
  maxDate = new Date();
  duration: string = '-';
  startDate?: Date = undefined;
  endDate?: Date = undefined;
  
  private _onChangeFn = (_: any) => { };
  private _timer?: TimerDto = undefined;

  constructor() {
    this.editForm =  new FormGroup({
      'year': new FormControl(0, [Validators.required]),
      'month': new FormControl(0, [Validators.required]),
      'day': new FormControl(0, [Validators.required]),
      'hour': new FormControl(0, [Validators.required]),
      'min': new FormControl(0, [Validators.required]),
      'seconds': new FormControl(0, [Validators.required]),
      'durationHour': new FormControl(0, [Validators.required]),
      'durationMin': new FormControl(0, [Validators.required]),
      'durationSeconds': new FormControl(0, [Validators.required]),
      'notes': new FormControl(0, [Validators.required]),
    });
  }
  
  ngOnInit(): void { }

  // ControlValueAccessor
  writeValue(obj: any): void {
    if(!obj) return;

    if(typeof(obj) === 'object') {
      if(obj instanceof TimerDto) {
        this.setRawTimer(obj);
      }
      else {
        console.error('COMPLETE ME - ', obj);
      }

      return;
    }

    console.error('COMPLETE ME - ', obj);
  }

  registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }

  registerOnTouched(fn: any): void { }


  // Template methods
  dateChanged = (e: MatDatepickerInputEvent<any>) => {
    this.editForm.patchValue({
      'year': e.value.getFullYear(),
      'month': e.value.getMonth(),
      'day': e.value.getDate()
    });

    this.calculateValues();
  }

  arrayOne(n: number): any[] {
    return Array(n);
  }

  onSubmit = () => { }

  hourChanged = () => {
    this.editForm.patchValue({
      'hour': this.editForm.value.hour
    });
    this.calculateValues();
  }

  minChanged = () => {
    this.editForm.patchValue({
      'min': this.editForm.value.min
    });
    this.calculateValues();
  }

  durationHoursChanged = () => {
    this.editForm.patchValue({
      'durationHour': this.editForm.value.durationHour
    });
    this.calculateValues();
  }

  durationMinsChanged = () => {
    this.editForm.patchValue({
      'durationMin': this.editForm.value.durationMin
    });
    this.calculateValues();
  }

  durationSecondsChanged = () => {
    this.editForm.patchValue({
      'durationSeconds': this.editForm.value.durationSeconds
    });
    this.calculateValues();
  }

  notesChanged = () => {
    this.calculateValues();
  }


  // Internal methods
  private setRawTimer = (timer: TimerDto) => {
    if(!(timer?.startTimeUtc instanceof Date))
      return;

    let date = timer.startTimeUtc;
    this._timer = timer;

    let duration = this.extractDuration(timer?.totalSeconds ?? 0);
    this.pickedDate = new FormControl(new Date(
      date.getFullYear(),
      date.getMonth(),
      date.getDate()
    ));

    this.editForm.patchValue({
      'year': date.getFullYear(),
      'month': date.getMonth(),
      'day': date.getDate(),
      'hour': date.getHours(),
      'min': date.getMinutes(),
      'seconds': date.getSeconds(),
      'durationHour': duration.hours,
      'durationMin': duration.minutes,
      'durationSeconds': duration.seconds,
      'notes': timer.notes
    });

    this.calculateValues();
  }

  private extractDuration = (seconds: number): TimeDuration => {
    let duration: TimeDuration = {
      hours: 0,
      minutes: 0,
      seconds: 0
    };

    if(seconds >= 3600) {
      duration.hours = Math.floor(seconds / 3600);
      seconds -= (3600 * duration.hours);
    }

    if(seconds >= 60) {
      duration.minutes = Math.floor(seconds / 60);
      seconds -= (60 * duration.minutes);
    }

    duration.seconds = seconds;

    return duration;
  }

  private workTotalSeconds = (formData: any) => {
    let totalSeconds = 0;
    totalSeconds += (3600 * formData.durationHour);
    totalSeconds += (60 * formData.durationMin);
    totalSeconds += formData.durationSeconds;
    return totalSeconds;
  }

  private getHumanDuration = (formData: any, totalSeconds: number) => {
    let hours = (formData.durationHour as number).toString().padStart(2, '0');
    let mins = (formData.durationMin as number).toString().padStart(2, '0');
    let secs = (formData.durationSeconds as number).toString().padStart(2, '0');
    
    return `${hours}:${mins}:${secs} (${totalSeconds} seconds)`;
  }

  private calculateValues = () => {
    let formData = this.editForm.value;
    let totalSeconds = this.workTotalSeconds(formData);

    this.startDate = new Date(
      formData.year,
      formData.month,
      formData.day,
      formData.hour,
      formData.min,
      formData.seconds
    );

    this.duration = this.getHumanDuration(formData, totalSeconds);
    this.endDate = new Date(this.startDate.getTime() + (totalSeconds * 1000));

    this.onChanged.emit({
      type: 'valueChanged',
      timer: this._timer,
      durationSec: totalSeconds,
      startDate: this.startDate,
      notes: formData.notes
    });
  }
}
