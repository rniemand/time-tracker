import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-home-card',
  templateUrl: './home-card.component.html',
  styleUrls: ['./home-card.component.css']
})
export class HomeCardComponent implements OnInit {
  @Input('title') title: string = 'Title';
  @Input('icon') icon: string = 'warning';

  constructor() { }

  ngOnInit(): void {
  }

}
