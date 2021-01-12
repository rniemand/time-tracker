import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { ProjectDto, ProjectsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-add-project',
  templateUrl: './add-project.component.html',
  styleUrls: ['./add-project.component.css']
})
export class AddProjectComponent implements OnInit {
  clientId: number = 0;
  productId: number = 0;
  addForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private uiService: UiService,
    private projectsClient: ProjectsClient,
    private router: Router
  ) {
    this.addForm =  new FormGroup({
      'clientId': new FormControl({ value: 0, disabled: true }, [Validators.required]),
      'productId': new FormControl(0, [Validators.required]),
      'userId': new FormControl(0, [Validators.required]),
      'projectName': new FormControl(null, [Validators.required])
    });
  }

  ngOnInit(): void {
    this.clientId = parseInt(this.route.snapshot.params?.clientId ?? 0);
    this.productId = parseInt(this.route.snapshot.params?.productId ?? 0);

    this.addForm.patchValue({
      'userId': this.authService.currentUser?.id ?? 0,
      'clientId': this.clientId,
      'productId': this.productId
    });
  }

  onSubmit = () => {
    let projectDto = new ProjectDto({
      ...this.addForm.value,
      userId: this.authService.currentUser?.id ?? 0,
      clientId: this.clientId
    });

    this.uiService.showLoader(true);

    this.projectsClient.addProject(projectDto).toPromise().then(
      (success: boolean) => {
        this.router.navigate(['/projects', this.clientId, this.productId]);
        this.uiService.notify(`Project added`, 1500);
      },
      this.uiService.handleClientError
    );
  }
}
