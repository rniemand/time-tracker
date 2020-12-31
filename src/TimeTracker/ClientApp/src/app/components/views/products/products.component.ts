import { Component, OnInit } from '@angular/core';
import { UiService } from 'src/app/services/ui.service';
import { ProductClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {

  constructor(
    private productClient: ProductClient,
    private uiService: UiService
  ) { }

  ngOnInit(): void {
    // this.productClient.getAll().toPromise().then();
  }

}
