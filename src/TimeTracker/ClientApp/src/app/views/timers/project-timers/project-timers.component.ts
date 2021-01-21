import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { DIALOG_DEFAULTS } from 'src/app/constants';
import { EditTimerEntryDialog, EditTimerEntryDialogData } from 'src/app/dialogs/edit-timer-entry/edit-timer-entry.dialog';
import { UiService } from 'src/app/services/ui.service';
import { ClientDto, ClientsClient, ProductDto, ProductsClient, ProjectDto, ProjectsClient, TimerDto, TimersClient } from 'src/app/time-tracker-api';
import { isToday } from 'src/app/utils/core.utils';

@Component({
  selector: 'app-project-timers',
  templateUrl: './project-timers.component.html',
  styleUrls: ['./project-timers.component.css']
})
export class ProjectTimersComponent implements OnInit {
  displayedColumns: string[] = ['state', 'startTime', 'endTime', 'length', 'notes', 'controls'];
  dataSource = new MatTableDataSource<TimerDto>();
  projectId: number = 0;
  productId: number = 0;
  clientId: number = 0;
  project?: ProjectDto = undefined;
  product?: ProductDto = undefined;
  client?: ClientDto = undefined;
  projectName: string = 'Project';
  productName: string = 'Product';
  clientName: string = 'Client';

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private projectClient: ProjectsClient,
    private productClient: ProductsClient,
    private clientClient: ClientsClient,
    private timerClient: TimersClient,
    public dialog: MatDialog
  ) { }


  ngOnInit(): void {
    this.projectId = this.route.snapshot.params?.projectId ?? 0;

    this.uiService.showLoader(true);
    this.loadProject()
      .then(this.loadProduct)
      .then(this.loadClient)
      .then(this.loadTimers)
      .finally(() => { this.uiService.hideLoader(); });
  }

  editEntry = (timer: TimerDto) => {
    let dialogData: EditTimerEntryDialogData = {
      timer: timer
    };

    let dialogRef = this.dialog.open(EditTimerEntryDialog, {
      ...DIALOG_DEFAULTS,
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if(result?.outcome == 'updated') {
        this.refreshTimers();
      }
    });
  }

  workRowClass = (timer: TimerDto) => {
    if(!timer?.startTimeUtc)
      return [];

    if(isToday(timer.startTimeUtc))
      return ['today'];

    return [];
  }


  // Internal methods
  private loadProject = () => {
    return new Promise<void>((resolve) => {
      this.projectClient.getProjectById(this.projectId).toPromise().then(
        (project: ProjectDto) => {
          this.project = project;
          this.projectName = project?.projectName ?? 'Project';
          this.productId = project?.productId ?? 0;
          resolve();
        }
      );
    });
  }

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

  private loadTimers = () => {
    return new Promise<void>((resolve) => {
      this.timerClient.getProjectEntries(this.projectId).toPromise().then(
        (timers: TimerDto[]) => {
          this.dataSource = new MatTableDataSource(timers);
          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;

          resolve();
        }
      );
    });
  }

  private refreshTimers = () => {
    this.uiService.showLoader();

    this.timerClient.getProjectEntries(this.projectId).toPromise().then(
      (timers: TimerDto[]) => {
        this.dataSource = new MatTableDataSource(timers);
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
        
        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

}
