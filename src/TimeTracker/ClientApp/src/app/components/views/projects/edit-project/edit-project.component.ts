import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UiService } from 'src/app/services/ui.service';
import { ProjectDto, ProjectsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-edit-project',
  templateUrl: './edit-project.component.html',
  styleUrls: ['./edit-project.component.css']
})
export class EditProjectComponent implements OnInit {
  clientId: number = 0;
  productId: number = 0;
  projectId: number = 0;
  editForm: FormGroup;
  project?: ProjectDto;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private projectsClient: ProjectsClient,
    private router: Router
  ) {
    this.editForm = new FormGroup({
      'clientId': new FormControl({ value: 0, disabled: true }, [Validators.required]),
      'productId': new FormControl(0, [Validators.required]),
      'userId': new FormControl(0, [Validators.required]),
      'projectName': new FormControl(null, [Validators.required])
    });
  }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.params?.projectId ?? 0;
    this.loadProject();
  }

  onSubmit = () => {
    let updatedProject: ProjectDto = new ProjectDto({
      ...this.project,
      ...this.editForm.value
    });
    
    this.uiService.showLoader(true);
    this.projectsClient.updateProject(updatedProject).toPromise().then(
      (project: ProjectDto) => {
        this.router.navigate(['/projects', project.clientId, project.productId]);
        this.uiService.notify(`Project '${project.projectName}' updated`, 1500);
      },
      this.uiService.handleClientError
    );
  }

  // Internal methods
  private loadProject = () => {
    this.uiService.showLoader(true);

    this.projectsClient.getProjectById(this.projectId).toPromise().then(
      (project: ProjectDto) => {
        this.clientId = project?.clientId ?? 0;
        this.productId = project?.productId ?? 0;
        this.project = project;

        this.editForm.patchValue({
          'clientId': project?.clientId ?? 0,
          'productId': project?.productId ?? 0,
          'userId': project?.userId ?? 0,
          'projectName': project?.projectName
        });
        
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

}
