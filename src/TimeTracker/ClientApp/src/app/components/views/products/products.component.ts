import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UiService } from 'src/app/services/ui.service';
import { ClientsClient, ProductDto, ProductsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
  clientId: number = 0;
  productsLoaded: boolean = false;
  displayedColumns: string[] = ['productName', 'options'];
  dataSource = new MatTableDataSource<ProductDto>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

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

    this.uiService.showLoader(true);
    this.products.getAll(this.clientId).toPromise().then(
      (products: ProductDto[]) => {
        this.dataSource = new MatTableDataSource(products);
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
        this.productsLoaded = true;
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );

  }

}
