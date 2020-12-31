import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css']
})
export class ProjectsComponent implements OnInit {
  clientId: number = 0;

  constructor() { }

  ngOnInit(): void {
  }

  clientSelected = (clientId: number) => {
    console.log('clientid', clientId);
  }

}
