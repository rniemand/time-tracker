import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient, ProductDto, ProductsClient, ProjectDto, ProjectsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-product-timers',
  templateUrl: './product-timers.component.html',
  styleUrls: ['./product-timers.component.css']
})
export class ProductTimersComponent implements OnInit {
  displayedColumns: string[] = ['client', 'product', 'project', 'controls'];
  dataSource = new MatTableDataSource<ProjectDto>();
  productId: number = 0;
  clientId: number = 0;
  product?: ProductDto = undefined;
  client?: ClientDto = undefined;
  productName: string = 'Product';
  clientName: string = 'Client';

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private productClient: ProductsClient,
    private clientClient: ClientsClient,
    private projectClient: ProjectsClient
  ) { }

  ngOnInit(): void {
    this.productId = this.route.snapshot.params?.productId ?? 0;

    this.uiService.showLoader(true);
    this.loadProduct()
      .then(this.loadClient)
      .then(this.loadProjects)
      .finally(() => { this.uiService.hideLoader(); });
  }

  // Internal methods
  private loadProduct = () => {
    return new Promise<void>((resolve) => {
      this.productClient.getProductById(this.productId).toPromise().then(
        (product: ProductDto) => {
          this.product = product;
          this.clientId = product?.clientId ?? 0;
          this.productName = product?.productName ?? 'Product';
          resolve();
        }
      );
    });
  }

  private loadClient = () => {
    return new Promise<void>((resolve) => {
      this.clientClient.getClientById(this.clientId).toPromise().then(
        (client: ClientDto) => {
          this.client = client;
          this.clientName = client?.clientName ?? 'Client';
          resolve();
        }
      );
    });
  }

  private loadProjects = () => {
    return new Promise<void>((resolve) => {
      this.projectClient.getProductProjects(this.productId).toPromise().then(
        (projects: ProjectDto[]) => {
          this.dataSource = new MatTableDataSource(projects);
          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;

          resolve();
        }
      );
    });
  }
}
