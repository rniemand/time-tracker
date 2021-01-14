import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-start-daily-task',
  templateUrl: './start-daily-task.component.html',
  styleUrls: ['./start-daily-task.component.css']
})
export class StartDailyTaskComponent implements OnInit {
  clientId: number = 0;
  taskId: number = 0;

  constructor() { }

  ngOnInit(): void {
  }

  clientChanged = () => {
    console.log(this.clientId);
  }

  taskChanged = () => {
    console.log(this.taskId);
  }

}
