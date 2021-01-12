import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ValidationError } from 'src/app/services/ui.service';

export interface ValidationErrorDialogData {
  error: ValidationError;
}

@Component({
  selector: 'app-validation-error-dialog',
  templateUrl: './validation-error.dialog.html',
  styleUrls: ['./validation-error.dialog.css']
})
export class ValidationErrorDialog implements OnInit {
  error!: ValidationError;

  constructor(
    public dialogRef: MatDialogRef<ValidationErrorDialog>,
    @Inject(MAT_DIALOG_DATA) public data: ValidationErrorDialogData
  ) { }

  ngOnInit(): void {
    this.error = this.data.error;
  }

}
