import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { DailyTaskDto, DailyTasksClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-add-daily-task',
  templateUrl: './add-daily-task.component.html',
  styleUrls: ['./add-daily-task.component.css']
})
export class AddDailyTaskComponent implements OnInit {
  clientId: number = 0;
  addForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private authService: AuthService,
    private tasksClient: DailyTasksClient,
    private router: Router
  ) {
    this.addForm =  new FormGroup({
      'clientId': new FormControl(0, [Validators.required]),
      'userId': new FormControl(0, [Validators.required]),
      'taskName': new FormControl(null, [Validators.required])
    });
  }

  ngOnInit(): void {
    this.clientId = this.route.snapshot.params?.clientId ?? 0;

    this.addForm.patchValue({
      'clientId': this.clientId,
      'userId': this.authService.currentUser?.id ?? 0
    });
  }

  onSubmit = () => {
    let taskDto = new DailyTaskDto({
      ...this.addForm.value
    });

    this.uiService.showLoader(true);
    this.tasksClient.addDailyTask(taskDto).toPromise().then(
      (success: boolean) => {
        if(success) this.uiService.notify('Task added');
        this.router.navigate(['/daily-tasks', taskDto?.clientId ?? this.clientId]);
      },
      this.uiService.handleClientError
    );
  }

}
