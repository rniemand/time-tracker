import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UiService } from 'src/app/services/ui.service';
import { IntListItem, ProductsClient, ProjectDto, ProjectsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css']
})
export class ProjectsComponent implements OnInit {
  clientId: number = 0;
  productId: number = 0;
  projectsLoaded: boolean = false;
  displayedColumns: string[] = ['projectName', 'options'];
  dataSource = new MatTableDataSource<ProjectDto>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private uiService: UiService,
    private projectsClient: ProjectsClient
  ) { }

  ngOnInit(): void {
    
  }

  productSelected = (productId: number) => {
    if(this.clientId <= 0 || productId <= 0)
      return;

    console.log('productSelected', this.clientId, this.productId);

    this.uiService.showLoader(true);
    this.projectsLoaded = false;

    this.projectsClient.getAllForProduct(productId).toPromise().then(
      (projects: ProjectDto[]) => {
        this.dataSource = new MatTableDataSource(projects);
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
        this.projectsLoaded = true;
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

}
