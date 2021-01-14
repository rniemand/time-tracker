import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-edit-daily-task',
  templateUrl: './edit-daily-task.component.html',
  styleUrls: ['./edit-daily-task.component.css']
})
export class EditDailyTaskComponent implements OnInit {
  taskId: number = 0;
  clientId: number = 0;

  constructor(
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.taskId = this.route.snapshot.params?.taskId ?? 0;
  }

  // Internal methods
  

}
