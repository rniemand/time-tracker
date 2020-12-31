import { Component, OnInit } from '@angular/core';
import { UiService } from 'src/app/services/ui.service';
import { ClientsClient, ProductDto, ProductsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
  clientId: number = 0;

  constructor(
    private products: ProductsClient,
    private uiService: UiService,
    private clients: ClientsClient
  ) { }

  ngOnInit(): void {
    // this.products.getAll().toPromise().then();
  }

  clientSelected = (clientId: number) => {
    console.log('client selected', clientId);

    this.products.getAll(this.clientId).toPromise().then(
      (products: ProductDto[]) => {
        console.log(products);
      },
      this.uiService.handleClientError
    );

  }

}
