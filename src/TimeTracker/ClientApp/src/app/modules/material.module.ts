import { NgModule } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSortModule } from '@angular/material/sort';
import { MatPaginatorModule } from '@angular/material/paginator';

const modules = [
  MatSidenavModule,
  MatProgressSpinnerModule,
  MatIconModule,
  MatListModule,
  MatMenuModule,
  MatButtonModule,
  MatToolbarModule,
  MatSnackBarModule,
  MatCardModule,
  MatInputModule,
  MatAutocompleteModule,
  MatFormFieldModule,
  MatTooltipModule,
  MatButtonToggleModule,
  MatChipsModule,
  MatDialogModule,
  MatTableModule,
  MatTabsModule,
  MatTooltipModule,
  MatSortModule,
  MatPaginatorModule
];


@NgModule({
  declarations: [],
  imports: [...modules],
  exports: [...modules]
})
export class MaterialModule {}