import { Component, EventEmitter, forwardRef, OnInit, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ClientsClient, IntListItem } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-client-selector',
  templateUrl: './client-selector.component.html',
  styleUrls: ['./client-selector.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ClientSelectorComponent),
      multi: true
    }
  ]
})
export class ClientSelectorComponent implements OnInit, ControlValueAccessor {
  @Output('changed') onChange = new EventEmitter<number>();
  clientId: number = 0;
  entries: IntListItem[] = [];
  loading: boolean = true;

  private _onChangeFn = (_: any) => { };

  constructor(
    private clientsClient: ClientsClient
  ) { }

  ngOnInit(): void {
    this.clientsClient.getClientList().toPromise().then(
      (entries: IntListItem[]) => {
        this.entries = entries;
        this.loading = false;

        if(this.clientId <= 0 && this.entries.length > 0) {
          this.setClientId(this.entries[0]?.value ?? 0);
        }
      },
      (error: any) => {
        console.error(error);
      }
    );
  }

  valueChanged = () => {
    if (this._onChangeFn) {
      this._onChangeFn(this.clientId);
    }

    this.onChange.next(this.clientId);
  }

  writeValue(obj: any): void {
    this.clientId = obj;
  }

  registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }

  registerOnTouched(fn: any): void { }

  // Internal methods
  private setClientId = (clientId: number) => {
    this.clientId = clientId;
    this.valueChanged();
  }

}
