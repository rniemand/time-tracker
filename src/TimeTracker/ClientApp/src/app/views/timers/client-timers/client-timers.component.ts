import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient, DailyTaskDto, DailyTasksClient, ProductDto, ProductsClient } from 'src/app/time-tracker-api';

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
  productsCount: string = '0 Projects';
  tasksCount: string = '0 Daily Tasks';
  tasks: DailyTaskDto[] = [];

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  private _activePanel: number = 0;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private clietsClient: ClientsClient,
    private productsClient: ProductsClient,
    private tasksClient: DailyTasksClient
  ) { }

  ngOnInit(): void {
    this.clientId = this.route.snapshot.params?.clientId ?? 0;

    let tab = (this.route.snapshot.queryParams?.tab as string ?? '' ).toLowerCase().trim();
    if(tab === 'dt') this._activePanel = 1;

    this.refreshView();
  }

  isActivePanel = (id: number) => {
    return this._activePanel === id;
  }

  // Internal methods
  private refreshView = () => {
    this.uiService.showLoader();

    this.loadClientInfo()
      .then(this.loadProjects)
      .then(this.loadDailyTasks)
      .finally(() => { this.uiService.hideLoader(); });
  }

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
          this.productsCount = `${products.length} Products`;

          resolve();
        },
        (error: any) => { reject(error); }
      );
    });
  }

  private loadDailyTasks = () => {
    return new Promise<void>((resolve) => {
      this.tasks = [];
      this.tasksClient.getClientTasks(this.clientId).toPromise().then(
        (tasks: DailyTaskDto[]) => {
          this.tasks = tasks;
          this.tasksCount = `${tasks.length} Daily Tasks`;
          resolve();
        }
      );
    });
  }
}
