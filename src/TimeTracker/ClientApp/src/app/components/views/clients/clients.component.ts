import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-clients',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.css']
})
export class ClientsComponent implements OnInit {
  displayedColumns: string[] = ['clientName'];
  dataSource = new MatTableDataSource<ClientDto>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private clients: ClientsClient,
    private uiService: UiService
  ) { }
  
  ngOnInit(): void {
    this.uiService.showLoader(true);

    this.clients.getAll().toPromise().then(
      (clients: ClientDto[]) => {
        this.dataSource = new MatTableDataSource(clients);
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
      },
      this.uiService.handleClientError
    );
  }

}
