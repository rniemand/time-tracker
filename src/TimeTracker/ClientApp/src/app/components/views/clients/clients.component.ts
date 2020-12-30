import { Component, OnInit } from '@angular/core';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-clients',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.css']
})
export class ClientsComponent implements OnInit {

  constructor(
    private clients: ClientsClient,
    private uiService: UiService
  ) { }

  ngOnInit(): void {
    this.clients.getAll().toPromise().then(
      (clients: ClientDto[]) => {
        console.log(clients);
      },
      this.uiService.handleClientError
    );
  }

}
