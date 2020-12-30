import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { AuthClient, ClientDto, ClientsClient, DerivedBaseApiRequest } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(
    private authService: AuthService,
    private authClient: AuthClient,
    private uiService: UiService,
    private clients: ClientsClient
  ) { }

  ngOnInit(): void {
    // this.authService.login('niemandr', 'password');

    if(this.authService.loggedIn) {

      let model = new DerivedBaseApiRequest({
        test: "hello world"
      });

      this.clients.getAllClients(model).toPromise().then(
        (clients: ClientDto[]) => {
          console.log(clients);
        },
        (error: any) => {
          console.error(error);
        }
      );
    }

  }

}
