import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { ClientClient, ClientDto } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-add-client',
  templateUrl: './add-client.component.html',
  styleUrls: ['./add-client.component.css']
})
export class AddClientComponent implements OnInit {
  addForm: FormGroup;

  constructor(
    private authService: AuthService,
    private clientClient: ClientClient,
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
    this.clientClient.addClient(clientDto).toPromise().then(
      (client: ClientDto) => {
        this.router.navigate(['/clients']);
        this.uiService.notify(`Added Client: ${client.clientName} (${client.clientId})`);
      },
      this.uiService.handleClientError
    );
  }

}
