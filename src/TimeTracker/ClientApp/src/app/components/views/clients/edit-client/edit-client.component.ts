import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UiService } from 'src/app/services/ui.service';
import { ClientsClient, ClientDto } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-edit-client',
  templateUrl: './edit-client.component.html',
  styleUrls: ['./edit-client.component.css']
})
export class EditClientComponent implements OnInit {
  clientId: number = 0;
  editForm: FormGroup;
  client?: ClientDto = undefined;

  constructor(
    private route: ActivatedRoute,
    private clients: ClientsClient,
    private uiService: UiService,
    private router: Router
  ) {
    this.editForm = new FormGroup({
      'clientName': new FormControl(null, [Validators.required]),
      'clientEmail': new FormControl(null, [Validators.email])
    });
  }

  ngOnInit(): void {
    this.clientId = this.route.snapshot.params.id;

    this.uiService.showLoader(true);
    this.clients.getClientById(this.clientId).toPromise().then(
      (client: ClientDto) => {
        this.client = client;

        this.editForm.patchValue({
          'clientName': client.clientName,
          'clientEmail': client.clientEmail
        });

        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

  onSubmit = () => {
    let updatedClient = new ClientDto({
      ...this.client,
      ...this.editForm.value
    });
    
    this.uiService.showLoader(true);
    this.clients.updateClient(updatedClient).toPromise().then(
      (client: ClientDto) => {
        this.router.navigate(['/clients']);
        this.uiService.notify(`Client '${client.clientName}' updated`, 1500);
      },
      this.uiService.handleClientError
    );
  }
}
