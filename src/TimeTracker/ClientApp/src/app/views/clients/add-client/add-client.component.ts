import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { ClientsClient, ClientDto } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-add-client',
  templateUrl: './add-client.component.html',
  styleUrls: ['./add-client.component.css']
})
export class AddClientComponent implements OnInit {
  addForm: FormGroup;

  constructor(
    private authService: AuthService,
    private clients: ClientsClient,
    private uiService: UiService,
    private router: Router
  ) {
    this.addForm =  new FormGroup({
      'clientName': new FormControl(null, [Validators.required]),
      'clientEmail': new FormControl(null, [Validators.email])
    });
  }

  ngOnInit(): void {
  }

  onSubmit = () => {
    let clientDto = new ClientDto({
      ...this.addForm.value,
      userId: this.authService.currentUser?.id ?? 0
    });

    this.uiService.showLoader(true);
    this.clients.addClient(clientDto).toPromise().then(
      (success: boolean) => {
        this.router.navigate(['/clients']);
        this.uiService.notify(`Added Client: ${clientDto.clientName} (${clientDto.clientId})`);
      },
      this.uiService.handleClientError
    );
  }

}
