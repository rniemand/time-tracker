import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { UiService } from 'src/app/services/ui.service';
import { ProjectDto, ProjectsClient } from 'src/app/time-tracker-api';

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
    private projectsClient: ProjectsClient,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.processRoute();
  }

  productSelected = (productId: number) => {
    if(this.clientId <= 0 || productId <= 0)
      return;
      
    this.uiService.showLoader(true);
    this.projectsLoaded = false;

    this.projectsClient.getProductProjects(productId).toPromise().then(
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

  // Internal methods
  private processRoute = () => {
    if(this.route.snapshot.params.hasOwnProperty('clientId')) {
      this.clientId = parseInt(this.route.snapshot.params.clientId);
    }

    if(this.route.snapshot.params.hasOwnProperty('productId')) {
      this.productId = parseInt(this.route.snapshot.params.productId);
    }
  }

}
