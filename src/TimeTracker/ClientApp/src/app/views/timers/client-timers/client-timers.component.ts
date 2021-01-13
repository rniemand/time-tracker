import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient, ProductDto, ProductsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-client-timers',
  templateUrl: './client-timers.component.html',
  styleUrls: ['./client-timers.component.css']
})
export class ClientTimersComponent implements OnInit {
  displayedColumns: string[] = ['client', 'name', 'controls'];
  dataSource = new MatTableDataSource<ProductDto>();
  clientId: number = 0;
  client?: ClientDto = undefined;
  clientName: string = 'Client';

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private clietsClient: ClientsClient,
    private productsClient: ProductsClient
  ) { }

  ngOnInit(): void {
    this.clientId = this.route.snapshot.params?.clientId ?? 0;

    this.uiService.showLoader();
    this.loadClientInfo()
      .then(this.loadProjects, this.uiService.handleClientError)
      .then(
        () => { this.uiService.hideLoader(); },
        this.uiService.handleClientError
      );
  }

  // Internal methods
  private loadClientInfo = () => {
    return new Promise<void>((resolve, reject) => {
      this.clietsClient.getClientById(this.clientId).toPromise().then(
        (client: ClientDto) => {
          this.client = client;
          this.clientName = client?.clientName ?? 'Client';
          resolve();
        },
        (error: any) => { reject(error); }
      );
    });
  }

  private loadProjects = () => {
    return new Promise<void>((resolve, reject) => {
      this.productsClient.getAllProducts(this.clientId).toPromise().then(
        (products: ProductDto[]) => {
          this.dataSource = new MatTableDataSource(products);
          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;

          resolve();
        },
        (error: any) => { reject(error); }
      );
    });
  }
}
