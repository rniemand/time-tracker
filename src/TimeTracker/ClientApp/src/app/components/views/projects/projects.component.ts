import { Component, OnInit } from '@angular/core';
import { UiService } from 'src/app/services/ui.service';
import { IntListItem, ProductsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css']
})
export class ProjectsComponent implements OnInit {
  clientId: number = 0;
  productId: number = 0;
  products: IntListItem[] = [];

  constructor(
    private productsClient: ProductsClient,
    private uiService: UiService
  ) { }

  ngOnInit(): void {
    
  }

  productSelected = (productId: number) => {
    console.log('productSelected', this.clientId, this.productId);
  }

}
