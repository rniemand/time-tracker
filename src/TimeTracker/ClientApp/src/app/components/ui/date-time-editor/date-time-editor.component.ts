import { Component, forwardRef, Input, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';

@Component({
  selector: 'app-date-time-editor',
  templateUrl: './date-time-editor.component.html',
  styleUrls: ['./date-time-editor.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DateTimeEditorComponent),
      multi: true
    }
  ]
})
export class DateTimeEditorComponent implements OnInit, ControlValueAccessor {
  editForm: FormGroup;
  pickedDate = new FormControl(new Date());
  hour: number = 0;
  
  private _onChangeFn = (_: any) => { };

  constructor() {
    this.editForm =  new FormGroup({
      'hour': new FormControl(0, [Validators.required]),
      'min': new FormControl(0, [Validators.required])
    });
  }
  
  ngOnInit(): void { }

  // ControlValueAccessor
  writeValue(obj: any): void {
    if(!obj) return;

    if(typeof(obj) === 'object') {
      if(obj instanceof Date) {
        this.setDate(obj);
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
    console.log('dateChanged', e.value);
  }

  arrayOne(n: number): any[] {
    return Array(n);
  }

  onSubmit = () => {
    console.log('here');
  }

  hourChanged = () => {
    this.editForm.patchValue({
      'hour': this.editForm.value.hour
    });
  }

  minChanged = () => {
    this.editForm.patchValue({
      'min': this.editForm.value.min
    });
  }


  // Internal methods
  private setDate = (date: Date) => {
    this.pickedDate = new FormControl(new Date(
      date.getFullYear(),
      date.getMonth(),
      date.getDay()
    ));


    console.log(date.getFullYear());
    console.log(date.getMonth());
    console.log(date.getDay());
    console.log(date.getHours());
    console.log(date.getMinutes());
    console.log(date.getSeconds());
  }
}
