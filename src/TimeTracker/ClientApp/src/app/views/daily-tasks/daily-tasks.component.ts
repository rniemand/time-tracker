import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient, DailyTaskDto, DailyTasksClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-daily-tasks',
  templateUrl: './daily-tasks.component.html',
  styleUrls: ['./daily-tasks.component.css']
})
export class DailyTasksComponent implements OnInit {
  displayedColumns: string[] = ['client', 'name', 'options'];
  dataSource = new MatTableDataSource<DailyTaskDto>();
  clientId: number = 0;
  client?: ClientDto = undefined;
  clientName: string = 'Client';

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private tasksClient: DailyTasksClient,
    private clientClient: ClientsClient
  ) { }

  ngOnInit(): void {
    this.clientId = this.route.snapshot.params?.clientId ?? 0;
    this.refreshView();
  }

  clientSelected = () => {
    this.refreshView();
  }

  // Internal methods
  private refreshView = () => {
    this.uiService.showLoader(true);

    this.loadClientInfo()
      .then(this.loadClientTasks)
      .finally(() => { this.uiService.hideLoader(); });
  }

  private loadClientInfo = () => {
    return new Promise<void>((resolve) => {
      if(this.clientId === 0) { resolve(); return; }
      this.clientClient.getClientById(this.clientId).toPromise().then(
        (client: ClientDto) => {
          this.client = client;
          this.clientName = client?.clientName ?? 'Client';
          resolve();
        }
      );
    });
  }

  private loadClientTasks = () => {
    return new Promise<void>((resolve) => {
      if(this.clientId === 0) { resolve(); return; }
      this.tasksClient.getClientTasks(this.clientId).toPromise().then(
        (tasks: DailyTaskDto[]) => {
          this.dataSource = new MatTableDataSource(tasks);
          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;
          resolve();
        }
      );
    });
  }
}
