import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-start-timer',
  templateUrl: './start-timer.component.html',
  styleUrls: ['./start-timer.component.css']
})
export class StartTimerComponent implements OnInit {
  clientId: number = 0;
  productId: number = 0;
  projectId: number = 0;

  constructor() { }

  ngOnInit(): void {
  }

  onProjectChange = () => {
  }

  startTimer = () => {
    console.log(this);
  }

}
