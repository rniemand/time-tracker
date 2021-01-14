import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { DailyTaskDto, DailyTasksClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-edit-daily-task',
  templateUrl: './edit-daily-task.component.html',
  styleUrls: ['./edit-daily-task.component.css']
})
export class EditDailyTaskComponent implements OnInit {
  editForm: FormGroup;
  taskId: number = 0;
  clientId: number = 0;
  task?: DailyTaskDto = undefined;

  constructor(
    private route: ActivatedRoute,
    private tasksClient: DailyTasksClient,
    private uiService: UiService,
    private authService: AuthService,
    private router: Router
  ) {
    this.editForm = new FormGroup({
      'clientId': new FormControl(0, [Validators.required]),
      'userId': new FormControl(0, [Validators.required]),
      'taskName': new FormControl(null, [Validators.required])
    });
  }

  ngOnInit(): void {
    this.taskId = this.route.snapshot.params?.taskId ?? 0;
    this.refreshView();
  }

  onSubmit = () => {
    let taskDto = new DailyTaskDto({
      ...this.task,
      ...this.editForm.value
    });

    this.uiService.showLoader(true);
    this.tasksClient.updateTask(taskDto).toPromise().then(
      (success: boolean) => {
        if(success) this.uiService.notify('Updated');
        this.router.navigate(['/daily-tasks', taskDto?.clientId ?? this.clientId]);
      },
      this.uiService.handleClientError
    );
  }

  // Internal methods
  private refreshView = () => {
    this.uiService.showLoader(true);

    this.loadTask()
      .finally(() => { this.uiService.hideLoader(); });
  }

  private loadTask = () => {
    return new Promise<void>((resolve) => {
      this.tasksClient.getTaskById(this.taskId).toPromise().then(
        (task: DailyTaskDto) => {
          this.task = task;
          this.clientId = task?.clientId ?? 0;

          this.editForm.patchValue({
            'clientId': task?.clientId ?? 0,
            'userId': this.authService.currentUser?.id ?? 0,
            'taskName': task?.taskName
          });

          resolve();
        }
      );
    });
  }

}
