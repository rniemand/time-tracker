import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-timers',
  templateUrl: './timers.component.html',
  styleUrls: ['./timers.component.css']
})
export class TimersComponent implements OnInit {
  displayedColumns: string[] = ['name', 'email', 'added', 'controls'];
  dataSource = new MatTableDataSource<ClientDto>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private uiService: UiService,
    private clientsClient: ClientsClient
  ) { }

  ngOnInit(): void {
    this.refreshClients();
  }


  // Internal methods
  private refreshClients = () => {
    this.uiService.showLoader(true);

    this.clientsClient.getAllClients().toPromise().then(
      (clients: ClientDto[]) => {
        this.dataSource = new MatTableDataSource(clients);
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
        
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

}
