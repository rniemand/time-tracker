import { Component, OnInit } from '@angular/core';
import { UiService } from 'src/app/services/ui.service';
import { ProductsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {

  constructor(
    private products: ProductsClient,
    private uiService: UiService
  ) { }

  ngOnInit(): void {
    // this.products.getAll().toPromise().then();
  }

}
