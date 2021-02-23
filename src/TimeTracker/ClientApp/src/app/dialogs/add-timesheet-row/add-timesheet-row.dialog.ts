import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProductSelectorComponent } from 'src/app/components/ui/product-selector/product-selector.component';
import { ProjectSelectorComponent } from 'src/app/components/ui/project-selector/project-selector.component';

export interface AddTimesheetRowDialogData {
  clientId: number;
  startDate: Date;
  endDate: Date;
}

export interface AddTimesheetRowDialogResult {
  clientId: number;
  productId: number;
  projectId: number;
  addLine: boolean;
  startDate: Date;
  endDate: Date;
  productName: string;
  projectName: string;
}

@Component({
  selector: 'add-timesheet-row-dialog',
  templateUrl: './add-timesheet-row.dialog.html',
  styleUrls: ['./add-timesheet-row.dialog.css']
})
export class AddTimesheetRowDialog implements OnInit {
  @ViewChild('products', { static: true }) products!: ProductSelectorComponent;
  @ViewChild('projects', { static: true }) projects!: ProjectSelectorComponent;

  clientId: number = 0;
  productId: number = 0;
  productName: string = '';
  projectId: number = 0;
  projectName: string = '';

  constructor(
    public dialogRef: MatDialogRef<AddTimesheetRowDialog>,
    @Inject(MAT_DIALOG_DATA) public data: AddTimesheetRowDialogData
  ) { }

  ngOnInit(): void {
    this.clientId = this.data?.clientId ?? 0;
  }

  productChanged = () => {
    this.productName = this.products.productName;
  }

  projectChanged = () => {
    this.projectName = this.projects.projectName;
  }

  cancel(): void {
    this.dialogRef.close(this.generateCloseOutcome(false));
  }

  addLine = () => {
    this.dialogRef.close(this.generateCloseOutcome(true));
  }

  // Internal methods
  private generateCloseOutcome = (addLine: boolean): AddTimesheetRowDialogResult => {
    return {
      clientId: this.clientId,
      productId: this.productId,
      projectId: this.projectId,
      addLine: addLine,
      startDate: this.data.startDate,
      endDate: this.data.endDate,
      productName: this.productName,
      projectName: this.projectName
    };
  }

}
