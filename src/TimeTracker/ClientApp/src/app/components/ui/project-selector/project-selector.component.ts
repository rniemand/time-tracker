import { Component, EventEmitter, forwardRef, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatFormFieldAppearance } from '@angular/material/form-field';
import { IntListItem, ProjectsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-project-selector',
  templateUrl: './project-selector.component.html',
  styleUrls: ['./project-selector.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ProjectSelectorComponent),
      multi: true
    }
  ]
})
export class ProjectSelectorComponent implements OnInit, ControlValueAccessor, OnChanges {
  @Output('changed') onChange = new EventEmitter<number>();
  @Input('productId') productId: number = 0;
  @Input('appearance') appearance: MatFormFieldAppearance = "outline";
  @Input('class') class: string = "";
  projectId: number = 0;
  loading: boolean = true;
  label: string = 'Select a product first';
  entries: IntListItem[] = [];

  private _onChangeFn = (_: any) => { };

  constructor(
    private projectsClient: ProjectsClient
  ) { }
  
  ngOnInit(): void {
  }

  valueChanged = () => {
    if (this._onChangeFn) {
      this._onChangeFn(this.projectId);
    }

    this.onChange.next(this.projectId);
  }

  writeValue(obj: any): void {
    if(typeof(obj) === 'object' && isNaN(obj)) {
      this.projectId = 0;
    }
    else if(typeof(obj) === 'string') {
      this.projectId = parseInt(obj);
    }
    else {
      this.projectId = obj;
    }
  }

  registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }

  registerOnTouched(fn: any): void { }

  ngOnChanges(changes: SimpleChanges): void {
    // productId
    if(changes.hasOwnProperty('productId')) {
      this.refreshProjects();
    }
  }

  // Internal methods
  private refreshProjects = () => {
    this.loading = true;
    this.entries = [];
    this.label = 'Select a product first';

    if(isNaN(this.productId) || this.productId <= 0) {
      return;
    }

    this.projectsClient.getProjectsAsList(this.productId).toPromise().then(
      (projects: IntListItem[]) => {
        this.entries = projects;
        this.label = 'Select a project';

        if(this.projectId <= 0 && this.entries.length > 0) {
          this.setProjectId(this.entries[0]?.value ?? 0);
        }

        this.loading = false;
      },
      (error: any) => {
        this.label = 'Error fetching projects';
        this.loading = false;
      }
    );
  }

  private setProjectId = (projectId: number) => {
    this.projectId = projectId;
    this.valueChanged();
  }
}
